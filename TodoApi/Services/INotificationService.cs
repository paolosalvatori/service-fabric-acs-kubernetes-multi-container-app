#region Using Directives
using System.Threading.Tasks;
using TodoApi.Models; 
#endregion

namespace TodoApi.Services
{
    /// <summary>
    /// Interface implemented by notification services
    /// </summary>
    public interface INotificationService
    {
        #region Interface Methods
        /// <summary>
        /// Sends a notification using a messaging system.
        /// </summary>
        /// <param name="notification">Notification message</param>
        /// <returns>The task resulting from the async method execution.</returns>
        Task SendNotificationAsync(Notification notification); 
        #endregion
    }
}
