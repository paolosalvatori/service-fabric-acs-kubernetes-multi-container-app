using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TodoApi.Models;

namespace TodoApi.Models
{
    /// <summary>
    /// TodoContext class.
    /// </summary>
    public class TodoContext : DbContext
    {
        #region Public Constructor
        /// <summary>
        /// Returns a new instance of the TodoContext class.
        /// </summary>
        /// <param name="options">DbContextOptions object</param>
        public TodoContext(DbContextOptions<TodoContext> options)
            : base(options)
        {
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the TodoItems collectioon property.
        /// </summary>
        public DbSet<TodoItem> TodoItems { get; set; } 
        #endregion

    }
}