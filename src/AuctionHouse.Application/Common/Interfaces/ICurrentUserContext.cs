using AuctionHouse.Domain.Entities;
using System.Threading.Tasks;

namespace AuctionHouse.Application.Common.Interfaces
{
    public interface ICurrentUserContext
    {
        /// <summary>
        /// Retrieves the user from the current HTTP context.
        /// </summary>
        /// <returns>A user.</returns>
        Task<User> GetCurrentUserContext();

        /// <summary>
        /// Retrieves the user's authentication token from
        /// the current HTTP context.
        /// </summary>
        /// <returns>A JWT authentication token.</returns>
        string GetCurrentUserToken();
    }
}
