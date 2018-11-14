using Edison.Core.Common.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Edison.Core.Interfaces
{
    public interface IResponseRestService
    {
        Task<ResponseModel> GetResponseDetail(Guid responseId);
        Task<IEnumerable<ResponseLightModel>> GetResponses();
        Task<IEnumerable<ResponseModel>> GetResponsesFromPointRadius(ResponseGeolocationModel responseGeolocationObj);
        Task<ResponseModel> CreateResponse(ResponseCreationModel responseObj);
        //Task<ResponseModel> UpdateResponse(ResponseUpdateModel responseObj);
        Task<ResponseModel> AddEventClusterIdsToResponse(ResponseEventClustersUpdateModel responseObj);
        Task<bool> DeleteResponse(Guid responseId);
        Task<ResponseModel> ChangeResponseAction(ResponseChangeActionPlanModel responseObj);
        Task<bool> SetSafeStatus(ResponseSafeUpdateModel responseSafeUpdateObj);
    }
}
