#region Copyright
//=======================================================================================
// Microsoft 
//
// This sample is supplemental to the technical guidance published on my personal
// blog at https://github.com/paolosalvatori. 
// 
// Author: Paolo Salvatori
//=======================================================================================
// Copyright (c) Microsoft Corporation. All rights reserved.
// 
// LICENSED UNDER THE APACHE LICENSE, VERSION 2.0 (THE "LICENSE"); YOU MAY NOT USE THESE 
// FILES EXCEPT IN COMPLIANCE WITH THE LICENSE. YOU MAY OBTAIN A COPY OF THE LICENSE AT 
// http://www.apache.org/licenses/LICENSE-2.0
// UNLESS REQUIRED BY APPLICABLE LAW OR AGREED TO IN WRITING, SOFTWARE DISTRIBUTED UNDER THE 
// LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY 
// KIND, EITHER EXPRESS OR IMPLIED. SEE THE LICENSE FOR THE SPECIFIC LANGUAGE GOVERNING 
// PERMISSIONS AND LIMITATIONS UNDER THE LICENSE.
//=======================================================================================
#endregion

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
