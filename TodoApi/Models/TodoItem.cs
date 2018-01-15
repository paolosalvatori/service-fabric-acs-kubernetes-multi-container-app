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
using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations; 
#endregion

namespace TodoApi.Models
{
    /// <summary>
    /// Todo TodoItem
    /// </summary>
    public class TodoItem : Entity
    {
        #region Public Constructor
        /// <summary>
        /// 
        /// </summary>
        public TodoItem() : base("TodoItem")
        { }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the value of the Name property.
        /// </summary>
        [Required]
        [MinLength(5)]
        [MaxLength(1024)]
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value of the IsComplete property.
        /// </summary>
        [DefaultValue(false)]
        [JsonProperty("isComplete")]
        public bool IsComplete { get; set; } 
        #endregion
    }
}