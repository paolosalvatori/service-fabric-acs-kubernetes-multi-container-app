using Microsoft.EntityFrameworkCore;

namespace TodoWeb.Models
{
    /// <summary>
    /// TodoContext class.
    /// </summary>
    public class TodoContext : DbContext
    {
        /// <summary>
        /// Returns a new instance of the TodoContext class.
        /// </summary>
        /// <param name="options"></param>
        public TodoContext(DbContextOptions<TodoContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the TodoItems collectioon property.
        /// </summary>
        public DbSet<TodoItem> TodoItems { get; set; }

    }
}