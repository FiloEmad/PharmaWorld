using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication3.Pages.Models;
using WebApplication6.Pages;

namespace WebApplication3.Pages
{
    public class C_infoModel : PageModel
    {
        private readonly ILogger<C_infoModel> _logger;

        public Companyinfo company { get; set; }
        public WebApplication3.Pages.Models.DB Db { get; set; }
        public C_infoModel(ILogger<C_infoModel> logger, WebApplication3.Pages.Models.DB db)
        {
            _logger = logger;
            Db = db;
        }
        public IActionResult OnGet()
        {
            company = Db.getCompanyInfo();
            if ((HttpContext.Session.GetInt32("ID")) == 0 || (HttpContext.Session.GetInt32("ID")) == null)
            {
                return RedirectToPage("/index");
            }
            else
            {
                if((HttpContext.Session.GetInt32("DID")) == 50101 )
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
