using System.Collections.Generic;
using System.Threading.Tasks;
using Cito.Cat.Core.Models.Session;
using Ims.Cat.Models;

namespace Cito.Cat.Core.Interfaces
{
    public interface ICatProcessor
    {
        /// <summary>
        /// Description of the algorithm.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Initialize session and return initial items to present. 
        /// </summary>
        /// <param name="sessionInitRequest"></param>
        /// <param name="sessionIdentifier"></param>
        /// <param name="sectionConfiguration"></param>
        /// <returns></returns>
        Task<CatResponse> InitializeSession(SessionDType sessionInitRequest, string sessionIdentifier);

        /// <summary>
        /// Determine next items to present.
        /// </summary>
        /// <param name="nextItemsRequest"></param>
        /// <param name="sectionConfiguration"></param>
        /// <returns></returns>
        Task<CatResponse> GetNextItems(ResultsDType nextItemsRequest);

        /// <summary>
        /// Get all the item ids that can be or are used in the adaptive section.
        /// </summary>
        /// <returns>A list of all the item ids that are referenced in the adaptive section.</returns>
        List<string> GetAllItemIds();
        
        /// <summary>
        /// Validate the prior data supplied in the session initialization request.
        /// </summary>
        /// <param name="priorData"></param>
        /// <returns></returns>
        List<string> ValidatePriorData(List<KeyValuePairDType> priorData);

        /// <summary>
        /// Validate the NextItemsRequest.
        /// </summary>
        /// <param name="nextItemsRequest"></param>
        /// <returns></returns>
        List<string> ValidateNextItemsRequest(ResultsDType nextItemsRequest);
    }
}