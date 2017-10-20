#region Using Directives
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using TodoWeb.Models;
using TodoWeb.Services;
#endregion

namespace TodoWeb.Pages
{
    public class IndexModel : PageModel
    {
        #region Private Instance Fields
        private readonly ITodoApiService _todoApiService;
        #endregion

        #region Public Constructor
        public IndexModel(ITodoApiService todoApiService)
        {
            _todoApiService = todoApiService;
        }
        #endregion

        #region Public Instance Properties
        public List<TodoItem> TodoItems { get; private set; }
        #endregion

        #region Public Instance Methods
        public async Task OnGetAsync()
        {
            // Retrieve all items from the TodoApi service
            var todoItems = await _todoApiService.GetTodoItemsAsync();
            TodoItems = new List<TodoItem>(todoItems);
        }
        #endregion
    }
}