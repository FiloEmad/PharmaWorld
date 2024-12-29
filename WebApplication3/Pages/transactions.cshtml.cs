 using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Specialized;
using System.Data;
using System.Security.Cryptography;
using System.Transactions;
using WebApplication3.Pages.Models;

namespace WebApplication6.Pages
{
    public class TransactionsModel : PageModel
    {
        private readonly ILogger<TransactionsModel> _logger;
        public DataTable Dt { get; set; }

        public WebApplication3.Pages.Models.DB Db { get; set; }
        

        public TransactionsModel(ILogger<TransactionsModel> logger, WebApplication3.Pages.Models.DB db)
        {
            _logger = logger;
            Db = db;
        }

        //public IActionResult OnGet()
        //{
        //    Dt = Db.gettransactions(); 
        //    if ((HttpContext.Session.GetInt32("ID")) == 0 || (HttpContext.Session.GetInt32("ID")) == null)
        //    {
        //        return RedirectToPage("/index");
        //    }
        //    else
        //    {
        //        if ((HttpContext.Session.GetInt32("DID")) == 50101 || (HttpContext.Session.GetInt32("DID")) == 5111 || (HttpContext.Session.GetInt32("DID")) == 50102)
        //        {
        //            return Page();
        //        }
        //        else
        //        {
        //            return RedirectToPage("/index");
        //        }
        //    }
        //}
        public IActionResult OnGet(int? sortOrder, int? filterType)
        {
            Dt = Db.gettransactions();
            if (HttpContext.Session.GetInt32("ID") == 0 || HttpContext.Session.GetInt32("ID") == null)
            {
                return RedirectToPage("/index");
            }

            if (HttpContext.Session.GetInt32("DID") == 50101 || HttpContext.Session.GetInt32("DID") == 5111 || HttpContext.Session.GetInt32("DID") == 50102)
            {
                // Default to showing all transactions, sorted by descending price
                Dt = sortOrder.HasValue ? Db.transSort(sortOrder.Value) : Db.transSort(0);

                // Apply filter if specified
                if (filterType.HasValue)
                {
                    Dt = Db.Filter(filterType.Value);
                }

                return Page();
            }

            return RedirectToPage("/index");
        }
   

        public IActionResult OnPostAdd(
            
            string transactionNumber,
            DateTime transactionDate,
            int salesAgentnumber,
            string transactionType,
            int productNumber,
            int customerNumber,
            string payment,
            int price,
            int amount,
            int paidAmount,
            int remainingAmount)
        {
           
            // Call the AddTransaction method with the values received from the form
            Db.AddTransaction(transactionDate, amount, salesAgentnumber, productNumber, customerNumber, transactionType, payment, price, paidAmount, remainingAmount);
            //AddTransaction(DateTime TDate, int Amount, int SalesID, int PID, int  CID, String sell_buy, string Payment, int Price, int PaidAmount, int Remainder)
            // After adding the transaction, optionally redirect or refresh the page
           // This will reload the page after the transaction is added
           return RedirectToPage("/transactions");
        }

       
    }

}
