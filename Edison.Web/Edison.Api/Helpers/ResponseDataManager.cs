using Edison.Api.Config;
using Edison.Core.Common.Models;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using AutoMapper;
using System;
using Microsoft.Azure.Documents;
using System.Net;
using Edison.Common.Interfaces;
using Edison.Common.DAO;

namespace Edison.Api.Helpers
{
    public class ResponseDataManager
    {
        private readonly ICosmosDBRepository<ResponseDAO> _repoResponses;
        private readonly IMapper _mapper;

        //TODO: Add proper enum
        private const int RESPONSE_STATE_ACTIVE = 1;
        private const int RESPONSE_STATE_INACTIVE = 0;

        public ResponseDataManager(IMapper mapper, ICosmosDBRepository<ResponseDAO> repoResponses)
        {
            _mapper = mapper;
            _repoResponses = repoResponses;
        }

        public async Task<ResponseModel> GetResponse(Guid responseId)
        {
            ResponseDAO response = await _repoResponses.GetItemAsync(responseId);
            return _mapper.Map<ResponseModel>(response);
        }

        public async Task<IEnumerable<ResponseLightModel>> GetResponses()
        {
            IEnumerable<ResponseDAO> responses = await _repoResponses.GetItemsAsync(
                p => p.ResponseState == 1 || (p.EndDate.Value != null && p.EndDate > DateTime.UtcNow.AddDays(-1)),
                p => new ResponseDAO()
                {
                    Id = p.Id,
                    ActionPlan = p.ActionPlan,
                    CreationDate = p.CreationDate,
                    EndDate = p.EndDate,
                    EventClusterIds = p.EventClusterIds,
                    Geolocation = p.Geolocation,
                    PrimaryEventClusterId = p.PrimaryEventClusterId,
                    ResponderUserId = p.ResponderUserId,
                    ResponseState = p.ResponseState
                }
                );

            return _mapper.Map<IEnumerable<ResponseLightModel>>(responses);
        }

        public async Task<IEnumerable<ResponseModel>> GetResponsesFromPointRadius(ResponseGeolocationModel responseGeolocationObj)
        {
            if (responseGeolocationObj == null || responseGeolocationObj.EventClusterEpicenterLocation == null)
                return new List<ResponseModel>();

            IEnumerable<ResponseDAO> responseObjs = await _repoResponses.GetItemsAsync(p => p.EndDate.Value == null);
            if (responseObjs == null)
                return null;

            List<ResponseDAO> output = new List<ResponseDAO>();
            GeolocationDAOObject daoGeocodeCenterPoint = _mapper.Map<GeolocationDAOObject>(responseGeolocationObj.EventClusterEpicenterLocation);
            foreach (var response in responseObjs)
                if (RadiusHelper.IsWithinRadius(daoGeocodeCenterPoint, response.Geolocation, response.ActionPlan.PrimaryRadius))
                    output.Add(response);
            return _mapper.Map<IEnumerable<ResponseModel>>(output);
        }

        public async Task<ResponseModel> CreateResponse(ResponseCreationModel responseObj)
        {
            ResponseDAO response = new ResponseDAO()
            {
                ActionPlan = _mapper.Map<ActionPlanDAOObject>(responseObj.ActionPlan),
                ResponderUserId = responseObj.ResponderUserId,
                ResponseState = RESPONSE_STATE_ACTIVE,
                PrimaryEventClusterId = responseObj.PrimaryEventClusterId,
                Geolocation = _mapper.Map<GeolocationDAOObject>(responseObj.Geolocation)
            };

            response.Id = await _repoResponses.CreateItemAsync(response);
            if (_repoResponses.IsDocumentKeyNull(response))
                throw new Exception($"An error occured when creating a new response");

            ResponseModel output =  _mapper.Map<ResponseModel>(response);
            return output;

        }

        public async Task<ResponseModel> AddEventClusterIdsToResponse(ResponseEventClustersUpdateModel responseUpdate)
        {
            ResponseDAO response = await _repoResponses.GetItemAsync(responseUpdate.ResponseId);
            if (response == null)
                throw new Exception($"No response found that matches responseid: {responseUpdate.ResponseId}");

            string etag = response.ETag;
            if (response.EventClusterIds == null)
                response.EventClusterIds = new List<Guid>();
            response.EventClusterIds = response.EventClusterIds.Concat(responseUpdate.EventClusterIds);
            response.ETag = etag;

            try
            {
                await _repoResponses.UpdateItemAsync(response);
            }
            catch (DocumentClientException e)
            {
                //Update concurrency issue, retrying
                if (e.StatusCode == HttpStatusCode.PreconditionFailed)
                    return await AddEventClusterIdsToResponse(responseUpdate);
                throw e;
            }

            return _mapper.Map<ResponseModel>(response);
        }

        public async Task<ResponseModel> LocateResponse(ResponseUpdateModel responseObj)
        {
            ResponseDAO response = await _repoResponses.GetItemAsync(responseObj.ResponseId);
            if (response == null)
                throw new Exception($"No response found that matches responseid: {responseObj.ResponseId}");

            string etag = response.ETag;
            response.Geolocation = _mapper.Map<GeolocationDAOObject>(responseObj.Geolocation);
            response.ETag = etag;

            try
            {
                await _repoResponses.UpdateItemAsync(response);
            }
            catch (DocumentClientException e)
            {
                //Update concurrency issue, retrying
                if (e.StatusCode == HttpStatusCode.PreconditionFailed)
                    return await LocateResponse(responseObj);
                throw e;
            }

            var output = _mapper.Map<ResponseModel>(response);
            return output;
        }

        public async Task<bool> SetSafeStatus(string userId, bool isSafe)
        {
            IEnumerable<ResponseDAO> activeResponses = await _repoResponses.GetItemsAsync(p => p.EndDate.Value == null && p.ActionPlan.AcceptSafeStatus);

            foreach(var activeResponse in activeResponses)
            {
                if (!await SetSafeStatus(activeResponse, userId, isSafe))
                    return false;
            }
            return true;
        }

        public async Task<bool> SetSafeStatus(ResponseDAO response, string userId, bool isSafe)
        {
            try
            {
                if (response.SafeUsers == null)
                    response.SafeUsers = new List<string>();

                if (isSafe && !response.SafeUsers.Contains(userId))
                    response.SafeUsers.Add(userId);
                else if (!isSafe && response.SafeUsers.Contains(userId))
                    response.SafeUsers.Remove(userId);
                else
                    return true; //no reason to update

                await _repoResponses.UpdateItemAsync(response);
                return true;
            }
            catch (DocumentClientException e)
            {
                //Update concurrency issue, retrying
                if (e.StatusCode == HttpStatusCode.PreconditionFailed)
                    response = await _repoResponses.GetItemAsync(response.Id);
                    return await SetSafeStatus(response, userId, isSafe);
                throw e;
            }
        }

        public async Task<ResponseModel> CloseResponse(ResponseCloseModel responseObj)
        {
            ResponseDAO response = await _repoResponses.GetItemAsync(responseObj.ResponseId);
            if (response == null)
                throw new Exception($"No response found that matches responseid: {responseObj.ResponseId}");

            string etag = response.ETag;
            response.ResponseState = RESPONSE_STATE_INACTIVE; //responseObj.State; Add for later alt states of completion
            response.EndDate = DateTime.UtcNow;
            response.ETag = etag;

            try
            {
                await _repoResponses.UpdateItemAsync(response);
            }
            catch (DocumentClientException e)
            {
                //Update concurrency issue, retrying
                if (e.StatusCode == HttpStatusCode.PreconditionFailed)
                    return await CloseResponse(responseObj);
                throw e;
            }

            var output = _mapper.Map<ResponseModel>(response);
            return output;
        }

        public async Task<bool> DeleteResponse(Guid responseId, bool responseExists = false)
        {
            if (!responseExists)
            {
                ResponseDAO response = await _repoResponses.GetItemAsync(responseId);
                if (response == null)
                    throw new Exception($"No response found that matches responseid: {responseId}");
            }

            try
            {
                await _repoResponses.DeleteItemAsync(responseId);
            }
            catch (DocumentClientException e)
            {
                //Update concurrency issue, retrying
                if (e.StatusCode == HttpStatusCode.PreconditionFailed)
                    return await DeleteResponse(responseId, true);
                throw e;
            }

            return true;
        }

        public async Task<ResponseModel> ChangeActionOnResponse(ResponseChangeActionPlanModel responseChangeAction)
        {
            ResponseDAO response = await _repoResponses.GetItemAsync(responseChangeAction.ResponseId);
            if (response == null)
                throw new Exception($"No response found that matches responseid: {responseChangeAction.ResponseId}");

            string etag = response.ETag;
            if (response.EventClusterIds == null)
                response.EventClusterIds = new List<Guid>();

            response = UpdateActionsOnResponseModel(response, responseChangeAction);

            response.ETag = etag;

            try
            {
                await _repoResponses.UpdateItemAsync(response);
            }
            catch (DocumentClientException e)
            {
                //Update concurrency issue, retrying
                if (e.StatusCode == HttpStatusCode.PreconditionFailed)
                    return await ChangeActionOnResponse(responseChangeAction);
                throw e;
            }

            return _mapper.Map<ResponseModel>(response);
        }

        private ResponseDAO UpdateActionsOnResponseModel(ResponseDAO response, ResponseChangeActionPlanModel responseChangeAction)
        {
            var openAddActions = responseChangeAction.Actions.Where(x => !x.IsCloseAction && x.ActionChangedString == "add");
            var openEditActions = responseChangeAction.Actions.Where(x => !x.IsCloseAction && x.ActionChangedString == "edit");
            var openDeleteActions = responseChangeAction.Actions.Where(x => !x.IsCloseAction && x.ActionChangedString == "delete");
            var closeAddActions = responseChangeAction.Actions.Where(x => x.IsCloseAction && x.ActionChangedString == "add");
            var closeEditActions = responseChangeAction.Actions.Where(x => x.IsCloseAction && x.ActionChangedString == "edit");
            var closeDeleteActions = responseChangeAction.Actions.Where(x => x.IsCloseAction && x.ActionChangedString == "delete");
            var openList = response.ActionPlan.OpenActions.ToList();
            var closeList = response.ActionPlan.CloseActions.ToList();

            if (openEditActions.Any() || openDeleteActions.Any())
                for(int i = 0; i < openList.Count(); i++)
                {
                    //Edit Open
                    if (openEditActions.Select(a => a.Action.ActionId).Contains(openList[i].ActionId))
                        openList[i] = _mapper.Map<ActionDAOObject>(openEditActions.First(a => a.Action.ActionId == openList[i].ActionId).Action);
                    //Delete Open
                    else if (openDeleteActions.Select(a => a.Action.ActionId).Contains(openList[i].ActionId))
                        openList.RemoveAt(i);
                }

            if (closeEditActions.Any() || closeDeleteActions.Any())
                for (int i = 0; i < closeList.Count(); i++)
                {
                    //Edit Close
                    if (closeEditActions.Select(a => a.Action.ActionId).Contains(closeList[i].ActionId))
                        closeList[i] = _mapper.Map<ActionDAOObject>(closeEditActions.First(a => a.Action.ActionId == closeList[i].ActionId).Action);
                    //Delete Close
                    else if (closeDeleteActions.Select(a => a.Action.ActionId).Contains(closeList[i].ActionId))
                        closeList.RemoveAt(i);
                }

            //Add Close
            if(closeAddActions.Any())
                closeList.AddRange(_mapper.Map<IEnumerable<ActionDAOObject>>(closeAddActions.Select(a => { a.Action.ActionId = Guid.NewGuid(); return a.Action; })));
            //Add Open
            if(openAddActions.Any())
                openList.AddRange(_mapper.Map<IEnumerable<ActionDAOObject>>(openAddActions.Select(a => { a.Action.ActionId = Guid.NewGuid(); return a.Action; })));

            response.ActionPlan.OpenActions = openList.AsEnumerable();
            response.ActionPlan.CloseActions = closeList.AsEnumerable();

            return response;
        }
    }
}
