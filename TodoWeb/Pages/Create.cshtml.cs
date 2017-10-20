#region Using Directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TodoWeb.Models;
using TodoWeb.Services;
using System;
#endregion

namespace TodoWeb.Pages
{
    public class CreateModel : PageModel
    {
        #region Private Instance Fields
        private readonly ITodoApiService _todoApiService;
        #endregion

        #region Public Constructor
        public CreateModel(ITodoApiService todoApiService)
        {
            _todoApiService = todoApiService;
        }
        #endregion

        #region Public Instance Properties
        [BindProperty]
        public TodoItem TodoItem { get; set; }
        #endregion

        #region Public Instance Methods
        public IActionResult OnGet()
        {
            TodoItem = new TodoItem
            {
                Id = Guid.NewGuid().ToString()
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _todoApiService.CreateTodoItemAsync(TodoItem);

            return RedirectToPage("./Index");
        }
        #endregion
    }
}