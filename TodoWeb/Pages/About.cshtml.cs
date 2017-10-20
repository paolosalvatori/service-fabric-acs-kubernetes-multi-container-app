using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TodoWeb.Pages
{
    public class AboutModel : PageModel
    {
        public string Message { get; set; }

        public void OnGet()
        {
            Message = "Docker Compose Demo";
        }
    }
}
