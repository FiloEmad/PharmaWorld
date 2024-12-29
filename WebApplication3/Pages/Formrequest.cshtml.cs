using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication3.Pages
{
    public class FormrequestModel : PageModel
    {
        private readonly ILogger<FormrequestModel> _logger;
        public FormrequestModel(ILogger<FormrequestModel> logger, WebApplication3.Pages.Models.DB db)
        {
            _logger = logger;
            Db = db;
        }
        public WebApplication3.Pages.Models.DB Db { get; set; }

        [BindProperty(SupportsGet = true)]
        public string LeaveReason { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateOnly StartDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateOnly EndDate { get; set; }
        public IActionResult OnGet()
        {
            if ((HttpContext.Session.GetInt32("ID")) == 0 || (HttpContext.Session.GetInt32("ID")) == null)
            {
                return RedirectToPage("/index");
            }
            else
            {
                if ((HttpContext.Session.GetInt32("DID")) != 50101)
                {
                    return Page();
                }
                else
                {
                    return RedirectToPage("/index");
                }
            }
        }

        public IActionResult OnPostSubmitLeaveRequest()
        {
            Db.SendDayoffReq((int)HttpContext.Session.GetInt32("ID"), LeaveReason, StartDate, EndDate);
            return RedirectToPage("/PersonalInfo2");
        }
    }
}
