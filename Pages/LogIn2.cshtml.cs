using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace WebApplication3.Pages
{
    public class Index1Model : PageModel
    {
        [BindProperty(SupportsGet = true)]
        [Required(ErrorMessage = "This field is required")]
        public Models.User USER2 { get; set; }

        public IActionResult OnGet()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("name")))
            {
                return RedirectToPage("/index");
            }
            else
            {
                return Page();
            }
        }

        public IActionResult OnPostLogin()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("name")))
            {
                return Page();
            }
            else
            {
                HttpContext.Session.SetString("name", USER2.name);
                HttpContext.Session.SetString("password", USER2.password);

                if (USER2.name.Contains("t-") || USER2.name.Contains("m-") ||
                    USER2.name.Contains("s-") || USER2.name.Contains("f-") ||
                    USER2.name.Contains("p-"))
                {
                    return RedirectToPage("/index", new { USER1 = this.USER2 });
                }
                else
                {
                    return Page();
                }
            }
        }
    }
}
