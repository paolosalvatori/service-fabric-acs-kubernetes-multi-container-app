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

namespace TodoWeb.Models
{
    #region snippet_LoggingEvents
    /// <summary>
    /// This class contains the event id of logging events.
    /// </summary>
    public class LoggingEvents
    {
        /// <summary>
        /// ListItems
        /// </summary>
        public const int ListItems = 1001;

        /// <summary>
        /// GetItem
        /// </summary>
        public const int GetItem = 1002;

        /// <summary>
        /// InsertItem
        /// </summary>
        public const int InsertItem = 1003;

        /// <summary>
        /// UpdateItem
        /// </summary>
        public const int UpdateItem = 1004;

        /// <summary>
        /// DeleteItem
        /// </summary>
        public const int DeleteItem = 1005;

        /// <summary>
        /// GetItemNotFound
        /// </summary>
        public const int GetItemNotFound = 4000;

        /// <summary>
        /// UpdateItemNotFound
        /// </summary>
        public const int UpdateItemNotFound = 4001;

        /// <summary>
        /// MethodCallDuration
        /// </summary>
        public const int MethodCallDuration = 5000;

        /// <summary>
        /// Configuration
        /// </summary>
        public const int Configuration = 5001;
    }
    #endregion
}
