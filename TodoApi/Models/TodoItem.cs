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