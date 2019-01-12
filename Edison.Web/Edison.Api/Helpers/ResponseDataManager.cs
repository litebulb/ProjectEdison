using System;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Azure.Documents;
using AutoMapper;
using Edison.Core.Common.Models;
using Edison.Common.Interfaces;
using Edison.Common.DAO;

namespace Edison.Api.Helpers
{
    /// <summary>
    /// Manager for the response repository
    /// </summary>
    public class ResponseDataManager
    {
        private readonly ICosmosDBRepository<ResponseDAO> _repoResponses;
        private readonly IMapper _mapper;

        //TODO: Add proper enum
        private const int RESPONSE_STATE_ACTIVE = 1;
        private const int RESPONSE_STATE_INACTIVE = 0;

        /// <summary>
        /// DI Constructor
        /// </summary>
        public ResponseDataManager(IMapper mapper, ICosmosDBRepository<ResponseDAO> repoResponses)
        {
            _mapper = mapper;
            _repoResponses = repoResponses;
        }

        /// <summary>
        /// Get a response full object by Id
        /// </summary>
        /// <param name="responseId">Response Id</param>
        /// <returns>ResponseModel</returns>
        public async Task<ResponseModel> GetResponse(Guid responseId)
        {
            ResponseDAO response = await _repoResponses.GetItemAsync(responseId);
            return _mapper.Map<ResponseModel>(response);
        }

        /// <summary>
        /// Get a list of responses light objects
        /// </summary>
        /// <returns>List of Response Light Model</returns>
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

        /// <summary>
        /// Get a list of responses that are within the radius of a point. The size of the radius is the primary radius of the action plan
        /// </summary>
        /// <param name="responseGeolocationObj">ResponseGeolocationModel</param>
        /// <returns>List of Response Model</returns>
        public async Task<IEnumerable<ResponseModel>> GetResponsesFromPointRadius(ResponseGeolocationModel responseGeolocationObj)
        {
            if (responseGeolocationObj == null || responseGeolocationObj.EventClusterGeolocationPointLocation == null)
                return new List<ResponseModel>();

            IEnumerable<ResponseDAO> responseObjs = await _repoResponses.GetItemsAsync(p => p.EndDate.Value == null);
            if (responseObjs == null)
                return null;

            List<ResponseDAO> output = new List<ResponseDAO>();
            GeolocationDAOObject daoGeocodeCenterPoint = _mapper.Map<GeolocationDAOObject>(responseGeolocationObj.EventClusterGeolocationPointLocation);
            foreach (var response in responseObjs)
                if (RadiusHelper.IsWithinRadius(daoGeocodeCenterPoint, response.Geolocation, response.ActionPlan.PrimaryRadius))
                    output.Add(response);
            return _mapper.Map<IEnumerable<ResponseModel>>(output);
        }

        /// <summary>
        /// Create a new response
        /// </summary>
        /// <param name="responseObj">ResponseCreationModel</param>
        /// <returns>ResponseModel</returns>
        public async Task<ResponseModel> CreateResponse(ResponseCreationModel responseObj)
        {
            //Instantiate the actions
            InstantiateResponseActions(responseObj.ActionPlan.OpenActions);
            InstantiateResponseActions(responseObj.ActionPlan.CloseActions);

            ResponseDAO response = new ResponseDAO()
            {
                ActionPlan = _mapper.Map<ResponseActionPlanDAOObject>(responseObj.ActionPlan),
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

        /// <summary>
        /// Set the safe status of a user
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="isSafe">True if the user is safe</param>
        /// <returns>true if the call succeeded</returns>
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

        /// <summary>
        /// Set the safe status of a user
        /// </summary>
        /// <param name="response">ResponseDAO</param>
        /// <param name="userId">User Id</param>
        /// <param name="isSafe">True if the user is safe</param>
        /// <returns>true if the call succeeded</returns>
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

        /// <summary>
        /// Locate a response by adding a geolocation. The call will fail if a geolocation already exist
        /// </summary>
        /// <param name="responseObj">ResponseUpdateModel</param>
        /// <returns>ResponseModel</returns>
        public async Task<ResponseModel> LocateResponse(ResponseUpdateModel responseObj)
        {
            ResponseDAO response = await _repoResponses.GetItemAsync(responseObj.ResponseId);
            if (response == null)
                throw new Exception($"No response found that matches responseid: {responseObj.ResponseId}");
            if (response.Geolocation != null)
                throw new Exception($"The response already had a geolocation: {responseObj.ResponseId}");

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

        /// <summary>
        /// Close a response by adding a end date.
        /// </summary>
        /// <param name="responseObj">EventSagaReceiveResponseClosed</param>
        /// <returns>ResponseModel</returns>
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

        /// <summary>
        /// Associated a set of Event Clusters Ids to a response
        /// </summary>
        /// <param name="responseObj">ResponseEventClustersUpdateModel</param>
        /// <returns>ResponseModel</returns>
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

        /// <summary>
        /// Delete a response. This call is for debugging purposes only
        /// </summary>
        /// <param name="responseId">Response Id</param>
        /// <param name="responseExists">True if the response exist, will skip the call</param>
        /// <returns>true if the call succeeded</returns>
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

        /// <summary>
        /// Complete an action and update the action object
        /// </summary>
        /// <param name="actionCompletionObj">ActionCompletionModel</param>
        /// <returns>true if the call succeeded</returns>
        public async Task<bool> CompleteAction(ActionCompletionModel actionCompletionObj)
        {
            ResponseDAO response = await _repoResponses.GetItemAsync(actionCompletionObj.ResponseId);
            if (response == null)
                throw new Exception($"No response found that matches responseid: {actionCompletionObj.ResponseId}");

            var action = response.ActionPlan.OpenActions.FirstOrDefault(p => p.ActionId == actionCompletionObj.ActionId);
            if (action == null)
                action = response.ActionPlan.CloseActions.FirstOrDefault(p => p.ActionId == actionCompletionObj.ActionId);

            if (action != null)
            {
                action.StartDate = actionCompletionObj.StartDate;
                action.EndDate = actionCompletionObj.EndDate;
                action.Status = actionCompletionObj.Status.ToString();
                action.ErrorMessage = actionCompletionObj.ErrorMessage;
            }
            else
                return false;

            try
            {
                await _repoResponses.UpdateItemAsync(response);
                return true;
            }
            catch (DocumentClientException e)
            {
                //Update concurrency issue, retrying
                if (e.StatusCode == HttpStatusCode.PreconditionFailed)
                    return await CompleteAction(actionCompletionObj);
                throw e;
            }
        }

        /// <summary>
        /// Add action to a response
        /// </summary>
        /// <param name="responseObj">ResponseChangeActionPlanModel</param>
        /// <returns>ResponseModel</returns>
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
                for (int i = 0; i < openList.Count(); i++)
                {
                    //Edit Open
                    if (openEditActions.Select(a => a.Action.ActionId).Contains(openList[i].ActionId))
                        openList[i] = _mapper.Map<ResponseActionDAOObject>(openEditActions.First(a => a.Action.ActionId == openList[i].ActionId).Action);
                    //Delete Open
                    else if (openDeleteActions.Select(a => a.Action.ActionId).Contains(openList[i].ActionId))
                        openList.RemoveAt(i);
                }

            if (closeEditActions.Any() || closeDeleteActions.Any())
                for (int i = 0; i < closeList.Count(); i++)
                {
                    //Edit Close
                    if (closeEditActions.Select(a => a.Action.ActionId).Contains(closeList[i].ActionId))
                        closeList[i] = _mapper.Map<ResponseActionDAOObject>(closeEditActions.First(a => a.Action.ActionId == closeList[i].ActionId).Action);
                    //Delete Close
                    else if (closeDeleteActions.Select(a => a.Action.ActionId).Contains(closeList[i].ActionId))
                        closeList.RemoveAt(i);
                }

            //Add Close
            if (closeAddActions.Any())
                closeList.AddRange(_mapper.Map<IEnumerable<ResponseActionDAOObject>>(closeAddActions.Select(a => { a.Action.ActionId = Guid.NewGuid(); return a.Action; })));
            //Add Open
            if (openAddActions.Any())
                openList.AddRange(_mapper.Map<IEnumerable<ResponseActionDAOObject>>(openAddActions.Select(a => { a.Action.ActionId = Guid.NewGuid(); return a.Action; })));

            response.ActionPlan.OpenActions = openList.AsEnumerable();
            response.ActionPlan.CloseActions = closeList.AsEnumerable();

            return response;
        }

        /// <summary>
        /// Instanciate an action object for a response.
        /// Reset Start Date, End Date, Status to "Not Started"
        /// </summary>
        /// <param name="actions">List of ResponseActionModel</param>
        private void InstantiateResponseActions(IEnumerable<ResponseActionModel> actions)
        {
            foreach (var action in actions)
                InstantiateResponseActions(action);
        }

        /// <summary>
        /// Instanciate an action object for a response.
        /// Reset Start Date, End Date, Status to "Not Started"
        /// </summary>
        /// <param name="actions">List of ActionChangedModel</param>
        private void InstantiateResponseActions(List<ActionChangedModel> actions)
        {
            foreach (var actionPlan in actions)
                InstantiateResponseActions(actionPlan.Action);
        }

        /// <summary>
        /// Instanciate an action object for a response.
        /// Reset Start Date, End Date, Status to "Not Started"
        /// </summary>
        /// <param name="action">ResponseActionModel</param>
        private void InstantiateResponseActions(ResponseActionModel action)
        {
            if (action.ActionId == Guid.Empty)
                action.ActionId = Guid.NewGuid();
            action.Status = ActionStatus.NotStarted;
            action.StartDate = null;
            action.EndDate = null;
        }
    }
}
