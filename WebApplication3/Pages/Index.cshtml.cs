using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Reflection;
using WebApplication3.Pages.Models;

namespace WebApplication3.Pages
{
    public class IndexModel : PageModel
    {
        public DataTable Dt { get; set; }
        public DataTable Dt2 { get; set; }
        public Companyinfo company { get; set; }
        public DataTable Dt3 { get; set; }
        public DataTable products { get; set; }



        private readonly ILogger<IndexModel> _logger;
        
        public WebApplication3.Pages.Models.DB Db { get; set; }


        [BindProperty(SupportsGet = true)]
        public Models.User USER1 { get; set; }
        public IndexModel(ILogger<IndexModel> logger, Models.DB db)
        {
            _logger = logger;
            Db = db;
        }

        public void OnGet()
        {
            company = Db.getCompanyInfo();
            Dt = Db.offers();
            Dt2 = Db.offers2();
            Dt3 = Db.offers3();
            products=Db.getProducts();
            if ((HttpContext.Session.GetInt32("ID")) != 0 && (HttpContext.Session.GetInt32("ID")) != null)
            {
                USER1.ID = (int)HttpContext.Session.GetInt32("ID");
            }

        }

        public IActionResult OnPostLogin()
        {
            return RedirectToPage("/LogIn2");
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.SetInt32("ID", 0);

            return RedirectToPage("/index");
        }
    }
}

