#region Using Directives
using System.Threading.Tasks;
using TodoApi.Models; 
#endregion

namespace TodoApi.Services
{
    /// <summary>
    /// This is an abstract base class for the notification service.
    /// </summary>
    public abstract class NotificationService : INotificationService
    {
        #region Virtual Methods
        /// <summary>
        /// Send to a notification to a given queue
        /// </summary>
        /// <param name="notification"></param>
        /// <returns></returns>
        public virtual Task SendNotificationAsync(Notification notification)
        {
            return Task.FromResult(0);
        } 
        #endregion
    }
}
