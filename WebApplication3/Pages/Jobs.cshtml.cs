using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication3.Pages
{
    public class JobsModel : PageModel
    {
        public IActionResult OnGet()
        {
            if ((HttpContext.Session.GetInt32("ID")) == 0 || (HttpContext.Session.GetInt32("ID")) == null)
            {
                return RedirectToPage("/index");
            }
            else
            {
                if ((HttpContext.Session.GetInt32("DID")) == 50101)
                {
                    return Page();
                }
                else
                {
                    return RedirectToPage("/index");
                }
            }
        }
    }
}
