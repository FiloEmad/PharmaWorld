using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;

namespace WebApplication6.Pages
{
    public class ProductModel : PageModel
    {
        private readonly ILogger<ProductModel> _logger;
        public DataTable Dt { get; set; }
        public WebApplication3.Pages.Models.DB Db { get; set; }
        public string pname { get; set; }
        public string ptype { get; set; }
        public  DateTime expdate { get; set; }
        public string desc { get; set; }
        public int amount { get; set; }
        public int sellprice { get; set; }
        public int buyprice { get; set; }
   







        public ProductModel(ILogger<ProductModel> logger, WebApplication3.Pages.Models.DB db)
        {
            _logger = logger;
            Db = db;
        }

        public IActionResult OnGet()
        {
            Dt = Db.getProducts(); // Fetching products data from DB using your DB class method
            if ((HttpContext.Session.GetInt32("ID")) == 0 || (HttpContext.Session.GetInt32("ID")) == null)
            {
                return RedirectToPage("/index");
            }
            else
            {
                if ((HttpContext.Session.GetInt32("DID")) == 50101 || (HttpContext.Session.GetInt32("DID")) == 50108)
                {
                    return Page();
                }
                else
                {
                    return RedirectToPage("/index");
                }
            }
        }
        public IActionResult OnPostEdit(string pname, string ptype, DateTime expdate, string desc, int buyprice, int sellprice, int amount)
        {
            Db.AddProduct(pname, ptype, expdate, desc, buyprice, sellprice, amount);
            return RedirectToPage("/product");
        }

        public IActionResult OnPostDelete(int id)
        {
            Db.DeleteProduct(id);
            return RedirectToPage("/product");
        }


    }
}
