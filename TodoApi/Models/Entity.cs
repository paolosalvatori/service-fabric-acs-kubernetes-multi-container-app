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
using System;
using System.ComponentModel.DataAnnotations;
#endregion

namespace TodoApi.Models
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class Entity
    {
        #region Public Constructor
        /// <summary>
        /// Creates an instance of the entity class.
        /// </summary>
        /// <param name="type">The type of the object</param>
        public Entity(string type)
        {
            Type = type;
        }
        #endregion

        /// <summary>
        /// Gets or sets the unique identifier of the document/object.
        /// </summary>
        [Key]
        [Required]
        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Gets or sets the document type.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; private set; }
    }
}
