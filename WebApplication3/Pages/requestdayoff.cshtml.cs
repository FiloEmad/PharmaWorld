using ChartExample.Models.Chart;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Data;
using WebApplication3.Pages.Models;

namespace WebApplication6.Pages
{
    public class PrivacyModel : PageModel
    {

        private readonly ILogger<PrivacyModel> _logger;
        public DataTable Dt { get; set; }
        public DataTable Dt2 { get; set; }
        public int did { get; set; }
        [BindProperty]
        public string Search {  get; set; }
        public ChartJs PieChart { get; set; }
        public ChartJs BarChart { get; set; }
        public string ChartJson { get; set; }
        public string ChartJson1 { get; set; }
        [BindProperty]
        public string Range {  get; set; }
        [BindProperty]
        public DateTime? Date { get; set; }
        public WebApplication3.Pages.Models.DB Db { get; set; }
        public PrivacyModel(ILogger<PrivacyModel> logger, WebApplication3.Pages.Models.DB db)
        {
            _logger = logger;
            Db = db;
            PieChart = new ChartJs();
            BarChart = new ChartJs();
        }
        // Simulate a table with employee requests using a dictionary
        public Dictionary<int, string> EmployeeRequests { get; private set; } = new Dictionary<int, string>
        {
            { 1, null }, // Request ID 1: No status yet
            { 2, null }  // Request ID 2: No status yet
        };

        public IActionResult OnGet(int? sortOrder, int? filterType)
        {
            Dt = Db.daysoff();
            Dt2 = Db.absence();
            if ((HttpContext.Session.GetInt32("ID")) == 0 || (HttpContext.Session.GetInt32("ID")) == null)
            {
                
                return RedirectToPage("/index");
            }
            else
            {
                if ((HttpContext.Session.GetInt32("DID")) == 50102|| (HttpContext.Session.GetInt32("DID")) == 50101)
                {
                    string search = HttpContext.Session.GetString("search");

                    if (!string.IsNullOrEmpty(search))
                    {
                        Dt = Db.searchdays(search);
                    }
                    else
                    {
                        if (sortOrder.HasValue)
                        {
                            Dt = Db.daysoffsort();

                        }
                        if (filterType.HasValue)
                        {
                            Dt = Db.daysofffilter(filterType.Value);

                        }
                    }

                    if (!string.IsNullOrEmpty(HttpContext.Session.GetString("SelectedDate")))
                    {
                       
                        string date = HttpContext.Session.GetString("SelectedDate");
                        Console.WriteLine(date);
                        Console.WriteLine("S");
                        Dictionary<string, int> finamount1 = Db.depdays2(date);
                        setUpBarChart(finamount1);
                    }
                    else
                    {
                        Console.WriteLine("hi");
                    }

                    Range = HttpContext.Session.GetString("range");
                    Console.WriteLine(Range);
                    if (!string.IsNullOrEmpty(Range))
                    {
                        Console.WriteLine("getnotempty");
                        Dictionary<string, int> finamount = Db.depdays(Range);
                        setUpPieChart(finamount);
                    }

                    return Page();
                }
                else
                {
                    return RedirectToPage("/index");
                }
            }
        }

        public IActionResult OnPostAccept(int id)
        {
            Db.accept(id);
            return RedirectToPage("/requestdayoff");
        }

        public IActionResult OnPostReject(int id)
        {
            Db.decline(id);
            return RedirectToPage("/requestdayoff");
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
            }
            else
            {
                HttpContext.Session.Remove("search");
            }

            return RedirectToPage("/requestdayoff");
        }
        public IActionResult OnPostRange()
        {
            if (!string.IsNullOrWhiteSpace(Range))
            {
                Console.WriteLine("post");
                string input = Range;
                char[] charArray = input.ToCharArray();
                Array.Reverse(charArray);
                string reversed = new string(charArray);
                HttpContext.Session.SetString("range", reversed);
            }
            else
            {
                HttpContext.Session.Remove("range");
            }
            return RedirectToPage("/requestdayoff");
        }

        
        public IActionResult OnPostHagar()
        {
            if (Date.HasValue) 
            {
                Console.WriteLine("post");
                HttpContext.Session.SetString("SelectedDate", Date.Value.ToString("yyyy-MM-dd"));
            }
                return RedirectToPage("/requestdayoff");
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
        private void setUpBarChart(Dictionary<string, int> dataToDisplay)
        {
            try
            {
                // 1. set up chart options 
                BarChart.type = "bar";
                BarChart.options.responsive = true;

                // 2. separate the received Dictionary data into labels and data
                var labelsArray = new List<string>();
                var dataArray = new List<double>();
                foreach (var data in dataToDisplay)
                {
                    labelsArray.Add(data.Key);
                    dataArray.Add(data.Value);
                }
                BarChart.data.labels = labelsArray;
                // 3. set up a dataset 
                var firsDataset = new Dataset();
                firsDataset.label = " ";
                firsDataset.data = dataArray.ToArray();
                BarChart.data.datasets.Add(firsDataset);
                // 4. finally, convert the object to json to be able to inject in 
                ChartJson1 = JsonConvert.SerializeObject(BarChart, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                });
            }
            catch (Exception e)
            {
                Console.WriteLine("Error initialising the bar chart inside Index.cshtml.cs");
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
