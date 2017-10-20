#region Using Directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TodoWeb.Models;
using TodoWeb.Services;
#endregion

namespace TodoWeb.Pages
{
    public class DeleteModel : PageModel
    {
        #region Private Instance Fields
        private readonly ITodoApiService _todoApiService;
        #endregion

        #region Public Constructor
        public DeleteModel(ITodoApiService todoApiService)
        {
            _todoApiService = todoApiService;
        }
        #endregion

        #region Public Instance Properties
        public TodoItem TodoItem { get; set; }
        #endregion

        #region Public Instance Methods
        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            TodoItem = await _todoApiService.GetTodoItemAsync(id);

            if (TodoItem == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var ok = await _todoApiService.DeleteTodoItemAsync(id);

            if (!ok)
            {
                return NotFound();
            }

            return RedirectToPage("./Index");
        }
        #endregion
    }
}
