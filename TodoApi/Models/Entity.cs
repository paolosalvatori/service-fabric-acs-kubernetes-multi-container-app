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
