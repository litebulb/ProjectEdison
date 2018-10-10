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
        private readonly WebApiConfiguration _config;
        private readonly ActionPlanDataManager _actionPlansDataManager;
        private readonly ICosmosDBRepository<ResponseDAO> _repoResponses;
        private readonly IMapper _mapper;

        //TODO: Add proper enum
        private const int RESPONSE_STATE_ACTIVE = 1;
        private const int RESPONSE_STATE_INACTIVE = 0;

        public ResponseDataManager(IOptions<WebApiConfiguration> config,
            IMapper mapper,
            ActionPlanDataManager actionPlansDataManager,
            ICosmosDBRepository<ResponseDAO> repoResponses)
        {
            _config = config.Value;
            _mapper = mapper;
            _repoResponses = repoResponses;
            _actionPlansDataManager = actionPlansDataManager;
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
            if (response.Id == Guid.Empty)
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

        public async Task<ResponseModel> UpdateResponse(ResponseUpdateModel responseObj)
        {
            ResponseDAO response = await _repoResponses.GetItemAsync(responseObj.ResponseId);
            if (response == null)
                throw new Exception($"No response found that matches responseid: {responseObj.ResponseId}");

            string etag = response.ETag;
            response.ResponseState = responseObj.State;
            response.ETag = etag;

            try
            {
                await _repoResponses.UpdateItemAsync(response);
            }
            catch (DocumentClientException e)
            {
                //Update concurrency issue, retrying
                if (e.StatusCode == HttpStatusCode.PreconditionFailed)
                    return await UpdateResponse(responseObj);
                throw e;
            }

            var output = _mapper.Map<ResponseModel>(response);
            return output;
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
    }
}
