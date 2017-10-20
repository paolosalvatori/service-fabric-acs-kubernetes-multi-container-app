#region Using Directives
using System.Collections.Generic;
using System.Threading.Tasks;
using TodoWeb.Models;
#endregion

namespace TodoWeb.Services
{
    public interface ITodoApiService
    {
        Task<IEnumerable<TodoItem>> GetTodoItemsAsync();
        Task<TodoItem> GetTodoItemAsync(string id);
        Task<TodoItem> CreateTodoItemAsync(TodoItem item);
        Task<bool> UpdateTodoItemAsync(TodoItem item);
        Task<bool> DeleteTodoItemAsync(string id);
    }
}
