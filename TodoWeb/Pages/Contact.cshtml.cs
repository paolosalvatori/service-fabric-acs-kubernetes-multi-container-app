using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TodoWeb.Pages
{
    public class ContactModel : PageModel
    {
        public string Message { get; set; }

        public void OnGet()
        {
            Message = "For any question, please contact:";
        }
    }
}
