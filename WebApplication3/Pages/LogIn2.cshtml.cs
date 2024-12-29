using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using WebApplication3.Pages.Models;

namespace WebApplication3.Pages
{
    public class Index1Model : PageModel
    {
        private readonly ILogger<Index1Model> _logger;
        public Index1Model(ILogger<Index1Model> logger, WebApplication3.Pages.Models.DB db)
        {
            _logger = logger;
            Db = db;
        }
        public WebApplication3.Pages.Models.DB Db { get; set; }

        [BindProperty(SupportsGet = true)]
        public Models.User USER2 { get; set; }
        public IActionResult OnGet()
        {
            if ((HttpContext.Session.GetInt32("ID"))!= 0 && (HttpContext.Session.GetInt32("ID")) != null)
            {
                return RedirectToPage("/index");
            }
            else
            {
                return Page();
            }
        }
        public IActionResult OnPost()
        {
            //if (!ModelState.IsValid)
            //{
            //    return Page();
            //}

            USER2 = Db.getEmpInfo(USER2.ID);

            if (USER2 != null)
            {
                HttpContext.Session.SetInt32("ID", USER2.ID);
                HttpContext.Session.SetInt32("DID", USER2.DID);
                return RedirectToPage("/index", new { USER1 = this.USER2 });
            }
            else
            {
                return Page();
            }
        }

    }
}
