using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Net;
using System.Numerics;
using System.Runtime.Intrinsics.X86;
using WebApplication3.Pages.Models;

namespace WebApplication3.Pages
{
    public class EmployeeInfoModel : PageModel
    {
        private readonly ILogger<EmployeeInfoModel> _logger;
        public EmployeeInfoModel(ILogger<EmployeeInfoModel> logger, WebApplication3.Pages.Models.DB db)
        {
            _logger = logger;
            Db = db;
        }
        public WebApplication3.Pages.Models.DB Db { get; set; }

        [BindProperty(SupportsGet = true)]
        public Models.User USER2 { get; set; }

        // OnGet method to initialize employee data
        public IActionResult OnGet()
        {
            if ((HttpContext.Session.GetInt32("ID")) == 0 || (HttpContext.Session.GetInt32("ID")) == null)
            {
                return RedirectToPage("/index");
            }
            else
            {
                USER2 = Db.getEmpInfo((int)HttpContext.Session.GetInt32("ID"));
                return Page();
            }
        }


        public IActionResult OnPostSaveFirstName(string firstName)
        {
            // Check if Employee is null (it should not be in this context, but it's a good safety check)
            USER2 = Db.EditEmpFname(firstName, (int)HttpContext.Session.GetInt32("ID"));
            return RedirectToPage();
        }



        public IActionResult OnPostSaveLastName(string lastName)
        {
            USER2 = Db.EditEmpLname(lastName, (int)HttpContext.Session.GetInt32("ID"));
            return RedirectToPage();
        }


        public IActionResult OnPostSavePhone(string phone)
        {
            USER2 = Db.EditEmpPhone(phone, (int)HttpContext.Session.GetInt32("ID"));
            return RedirectToPage();
        }

        public IActionResult OnPostSaveAddress(string address)
        {
            USER2 = Db.EditEmpAddress(address, (int)HttpContext.Session.GetInt32("ID"));
            return RedirectToPage();
        }

        public IActionResult OnPostSaveEmail(string email)
        {
            USER2 = Db.EditEmpEmail(email, (int)HttpContext.Session.GetInt32("ID"));
            return RedirectToPage();
        }

        public IActionResult OnPostReqDay()
        {
            return RedirectToPage("/Formrequest");
        }
    }

    // Employee class to hold employee data
}
