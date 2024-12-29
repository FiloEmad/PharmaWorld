using ChartExample.Models.Chart;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Data;
using WebApplication3.Pages.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebApplication6.Pages.Shared
{
    public class finModel : PageModel
    {
        private readonly ILogger<finModel> _logger;
        public DataTable Dt { get; set; }
        public DataTable Dt2 { get; set; }
        public int Id { get; set; }        // ID of the record to edit
        [BindProperty]
        public int Amount { get; set; }
        [BindProperty]
        public int order {  get; set; }
        [BindProperty]
        public string Search {  get; set; }
        public ChartJs PieChart { get; set; }
        public string ChartJson { get; set; }
        public WebApplication3.Pages.Models.DB db { get; set; }

        public finModel(ILogger<finModel> logger, WebApplication3.Pages.Models.DB db)
        {
            _logger = logger;
            this.db = db;
            PieChart = new ChartJs();
        }

        public IActionResult OnGet(int? sortOrder, int? filterType)
        {
          
            
            if ((HttpContext.Session.GetInt32("ID")) == 0 || (HttpContext.Session.GetInt32("ID")) == null)
            {
                

                return RedirectToPage("/index");
            }
            else
            {
                if ((HttpContext.Session.GetInt32("DID")) == 50101 || (HttpContext.Session.GetInt32("DID")) == 50106)
                {
                    string search = HttpContext.Session.GetString("search");

                    if (!string.IsNullOrEmpty(search))
                    {
                        Dt2 = db.Searchfin(search);
                    }
                    else 
                    {
                        Dt2 = sortOrder.HasValue ? db.finSort(sortOrder.Value) : db.finSort(0);
                        if (filterType.HasValue)
                        {
                            Dt2 = db.Filterfin(filterType.Value);

                        }
                    }
                    Dictionary<string, int> finamount1 = db.deptt();
                    setUpPieChart(finamount1);
                    return Page();
                }
                else
                {
                    
                    return RedirectToPage("/index");
                }
            }
        }

        public IActionResult OnPostDelete(int id)
        {
            db.DeleteRecord(id);  // Delete the record with the given ID
            return RedirectToPage("/fin");  // Redirect back to the financial page
        }
        public IActionResult OnPostEdit(int id, int Amount)
        {
            db.UpdateRecordd( Amount, id);
            return RedirectToPage();
        }
        public IActionResult OnPostSearch()
        {
                if (!string.IsNullOrWhiteSpace(Search))
                {
                string input = Search;
                char[] charArray = input.ToCharArray();
                Array.Reverse(charArray);
                string reversed = new string(charArray);
                HttpContext.Session.SetString("search", reversed);

                //Console.OutputEncoding = System.Text.Encoding.UTF8;
                //Console.WriteLine(reversed);
                }
                else
                {
                    HttpContext.Session.Remove("search");
                }

                return RedirectToPage("/fin");
        }
        private void setUpPieChart(Dictionary<string, int> dataToDisplay)
        {
            try
            {
                // Ensure PieChart is properly instantiated
                if (PieChart == null)
                    PieChart = new ChartJs();

                // Ensure PieChart.data is properly instantiated
                if (PieChart.data == null)
                    PieChart.data = new Data();

                // Ensure labels and datasets are properly instantiated
                if (PieChart.data.labels == null)
                    PieChart.data.labels = new List<string>();
                if (PieChart.data.datasets == null)
                    PieChart.data.datasets = new List<Dataset>();

                // 1. Set up chart options
                PieChart.type = "pie";
                PieChart.options.responsive = true;

                // 2. Separate the received Dictionary data into labels and data
                var labelsArray = new List<string>();
                var dataArray = new List<double>();
                foreach (var data in dataToDisplay)
                {
                    labelsArray.Add(data.Key);
                    dataArray.Add(data.Value);
                }

                // Clear and populate PieChart.data.labels
                PieChart.data.labels.Clear();
                PieChart.data.labels.AddRange(labelsArray);

                // 3. Set up a dataset
                var firstDataset = new Dataset();
                firstDataset.label = " ";
                firstDataset.data = dataArray.ToArray();
                firstDataset.backgroundColor = GenerateRandomColors(dataArray.Count).ToArray();

                PieChart.data.datasets.Clear();
                PieChart.data.datasets.Add(firstDataset);

                // 4. Convert the object to JSON to inject into the front end
                ChartJson = JsonConvert.SerializeObject(PieChart, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                });
            }
            catch (Exception e)
            {
                Console.WriteLine("Error initializing the pie chart inside Index.cshtml.cs");
                throw e;
            }
        }
        private List<string> GenerateRandomColors(int count)
        {
            var colors = new List<string>();
            var random = new Random();

            for (int i = 0; i < count; i++)
            {
                var color = $"rgb({random.Next(256)}, {random.Next(256)}, {random.Next(256)})";
                colors.Add(color);
            }

            return colors;
        }
    }
}

