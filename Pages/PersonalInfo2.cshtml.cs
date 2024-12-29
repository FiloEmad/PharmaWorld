using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using WebApplication3.Pages.Models;

namespace WebApplication3.Pages
{
    public class EmployeeInfoModel : PageModel
    {
        // Employee model (In a real application, this might come from a database)
        public Employee Employee1 { get; set; }

        // OnGet method to initialize employee data
        public void OnGet()
        {
            // Ensure this line initializes the Employee object.
            Employee1 = new Employee
            {
                EmployeeId = 1,
                FirstName = "Ahmed",
                LastName = "Ali",
                PhoneNumber = "0123456789",
                Address = "Cairo, Egypt",
                Email = "ahmed@example.com",
                Job = "Software Engineer",
                Salary = "10,000 EGP"
            };
        }


        // Handler methods for form submissions
        public IActionResult OnPostEditFirstName()
        {
            // Handle the form submission for editing first name (show modal or logic)
            return Page();
        }
        public IActionResult OnPostSaveFirstName(string firstName)
        {
            // Check if Employee is null (it should not be in this context, but it's a good safety check)
            Employee1 = new Employee
            {
                EmployeeId = 1,
                FirstName = firstName,
                LastName = "dd",
                PhoneNumber = "0123456789",
                Address = "Cairo, Egypt",
                Email = "ahmed@example.com",
                Job = "Software Engineer",
                Salary = "10,000 EGP"
            };
            return RedirectToPage();
        }


        public IActionResult OnPostEditLastName()
        {
            return Page();
        }

        public IActionResult OnPostSaveLastName(string lastName)
        {
            Employee1 = new Employee
            {
                EmployeeId = 1,
                FirstName = lastName,
                LastName = "Ali",
                PhoneNumber = "0123456789",
                Address = "Cairo, Egypt",
                Email = "ahmed@example.com",
                Job = "Software Engineer",
                Salary = "10,000 EGP"
            }; return RedirectToPage();
        }

        public IActionResult OnPostEditPhone()
        {
            return Page();
        }

        public IActionResult OnPostSavePhone(string phone)
        {
            Employee1.PhoneNumber = phone;
            return RedirectToPage();
        }

        public IActionResult OnPostEditAddress()
        {
            return Page();
        }

        public IActionResult OnPostSaveAddress(string address)
        {
            Employee1.Address = address;
            return RedirectToPage();
        }

        public IActionResult OnPostEditEmail()
        {
            return Page();
        }

        public IActionResult OnPostSaveEmail(string email)
        {
            Employee1.Email = email;
            return RedirectToPage();
        }
    }

    // Employee class to hold employee data
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Job { get; set; }
        public string Salary { get; set; }
    }
}
