//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.RazorPages;
//using System.Data;

//namespace WebApplication6.Pages
//{
//    public class employeeModel : PageModel
//    {
//        private readonly ILogger<employeeModel> _logger;
//        public DataTable dt { get; set; }
//        public WebApplication3.Pages.Models.DB db { get; set; }
//        public string employees { get; set; }
//        public employeeModel(ILogger<employeeModel>logger ,WebApplication3.Pages.Models.DB db)
//        {
//            _logger = logger;
//            this.db = db;
//        }
//        public IActionResult OnGet()
//        {
//           dt= db.getEmployee();
//           if ((HttpContext.Session.GetInt32("ID")) == 0 || (HttpContext.Session.GetInt32("ID")) == null)
//           {
//               return RedirectToPage("/index");
//           }
//           else
//           {
//                if ((HttpContext.Session.GetInt32("DID")) == 50101)
//                {
//                    return Page();
//                }
//                else
//                {
//                    return RedirectToPage("/index");
//                }
//            }
//        }
//    }
//}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using WebApplication3.Pages.Models;
using System;
using System.Net;
using System.Numerics;
using System.Runtime.Intrinsics.X86;
using System.Reflection;

namespace WebApplication3.Pages
{
    public class employeeModel : PageModel
    {
        private readonly ILogger<employeeModel> _logger;
        public DataTable dt { get; set; }
        public WebApplication3.Pages.Models.DB db { get; set; }

        [BindProperty]
        public Models.User NewEmployee { get; set; }
        public employeeModel(ILogger<employeeModel> logger, WebApplication3.Pages.Models.DB db)
        {
            _logger = logger;
            this.db = db;
        }

        public IActionResult OnGet()
        {
            dt = db.getEmployee();
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

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Add the new employee to the database
            db.AddEmployee(NewEmployee);

            // Reload the data and refresh the page
            dt = db.getEmployee();
            return RedirectToPage();
        }
    }
}

