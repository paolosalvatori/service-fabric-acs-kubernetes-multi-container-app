#region Using Directives
using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations; 
#endregion

namespace TodoWeb.Models
{
    /// <summary>
    /// Todo TodoItem
    /// </summary>
    public class TodoItem
    {
        #region Public Properties
        /// <summary>
        /// Gets or sets the value of the Id property.
        /// </summary>
        [Required]
        [JsonProperty("id")]
        public string Id { get; set; }

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
        [DisplayName("IsComplete")]
        [JsonProperty("isComplete")]
        public bool IsComplete { get; set; } 
        #endregion
    }
}