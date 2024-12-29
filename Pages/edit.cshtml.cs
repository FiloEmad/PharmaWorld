using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Reflection;


namespace WebApplication3.Pages
{
    public class Index1Model1 : PageModel
    {
        [BindProperty(SupportsGet = true)]
        [Required(ErrorMessage = "This field is required")]
        public Models.edit EDIT2 { get; set; }




        public IActionResult OnGet()
        {
            return Page();
            
        }


        public IActionResult OnPosteditfirstname()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("EDIT2.firstname")))
            {
                return Page(); 
            }
            else
            {
                HttpContext.Session.SetString("EDIT2.firstname", EDIT2.firstname);
                return RedirectToPage("/PersonalInfo2", new { EDIT1 = EDIT2 });
            }

            
        }

        public IActionResult OnPosteditlastname()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("EDIT2.lastname")))
            {
                return Page(); 
            }
            else
            {
                HttpContext.Session.SetString("EDIT1.surname", EDIT2.lastname);
                return RedirectToPage("/PersonalInfo2", new { EDIT1 = EDIT2 });
            }

            
        }

        public IActionResult OnPosteditphonenumber()
        {
            if (HttpContext.Session.GetInt32("EDIT2.phonenumber") == 0)
            {
                return Page();
            }
            else
            {
                HttpContext.Session.SetInt32("EDIT1.phonenumber", EDIT2.phonenumber);
                return RedirectToPage("/PersonalInfo2", new { EDIT1 = EDIT2 });
            }

            
        }

        public IActionResult OnPosteditaddress()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("EDIT2.adress")))
            {
                return Page(); 
            }
            else
            {
                HttpContext.Session.SetString("EDIT1.adress", EDIT2.adress);
                return RedirectToPage("/PersonalInfo2", new { EDIT1 = EDIT2 });
            }

            
        }

        public IActionResult OnPosteditpostcode()
        {
            if (HttpContext.Session.GetInt32("EDIT2.postcode") == 0)
            {
                return Page();
            }
            else
            {
                HttpContext.Session.SetInt32("EDIT1.postcode", EDIT2.postcode); 
                return RedirectToPage("/PersonalInfo2", new { EDIT1 = EDIT2 });
            }

            
        }

        public IActionResult OnPosteditcity()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("EDIT2.city")))
            {
                return Page(); // If city is empty, reload the page
            }
            else
            {
                HttpContext.Session.SetString("EDIT2.city", EDIT2.city);
                return RedirectToPage("/PersonalInfo2", new { EDIT1 = EDIT2 });
            }

        }  

        public IActionResult OnPosteditemail()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("EDIT2.email")))
            {
                return Page(); 
            }
            else
            {
                HttpContext.Session.SetString("EDIT2.email", EDIT2.email);
                return RedirectToPage("/PersonalInfo2", new { EDIT1 = EDIT2 });
            }

           
        }

        public IActionResult OnPostediteducation()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("EDIT2.education")))
            {
                return Page(); 
            }
            else
            {
                HttpContext.Session.SetString("EDIT2.education", EDIT2.education);
                return RedirectToPage("/PersonalInfo2", new { EDIT1 = EDIT2 });
            }

            
        }

        public IActionResult OnPosteditjob()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("EDIT2.job")))
            {
                return Page(); 
            }
            else
            {
                HttpContext.Session.SetString("EDIT2.job", EDIT2.job);
                return RedirectToPage("/PersonalInfo2", new { EDIT1 = EDIT2 });
            }

          
        }

        public IActionResult OnPosteditcountry()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("EDIT2.country")))
            {
                return Page(); 
            }
            else
            {
                HttpContext.Session.SetString("EDIT2.country", EDIT2.country);
                return RedirectToPage("/PersonalInfo2", new { EDIT1 = EDIT2 });
            }

            
        }

    }
}