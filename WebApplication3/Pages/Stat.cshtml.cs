using ChartExample.Models.Chart;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System;
using System.Data;


namespace WebApplication3.Pages
{
    public class StatModel : PageModel
    {
        public Models.DB db { get; set; }
        [BindProperty]
        public DataTable Name { get; set; }
       
        [BindProperty]
        public int RangeStart { get; set; }
        [BindProperty]
        public int Range { get; set; }
        public DataTable product { get; set; }

        public DataTable dt { get; set; }
        public DataTable chart1 { get; set; }

        public ChartJs BarChart { get; set; }
        public ChartJs PieChart { get; set; }
        public string ChartJson { get; set; }
        public string ChartJson1 { get; set; }
        public StatModel()
        {
            db = new Models.DB(); // Initialize db
            BarChart = new ChartJs();
            PieChart = new ChartJs();
        }
        public void OnGet()
        {
            Name = db.mostcomapny();
            Console.WriteLine(RangeStart);
            product = db.mostpaidproduct();
            dt = db.mostysalesagent();
            chart1 = db.inbetweenamount(RangeStart);
            RangeStart = Convert.ToInt32( HttpContext.Session.GetInt32("range"));
            Range = Convert.ToInt32(HttpContext.Session.GetInt32("range2"));

            Dictionary<string, int> productamount = db.productamount(RangeStart);
            Dictionary<string, int> finamount = db.finchart(Range);
            setUpBarChart(productamount);
            setUpPieChart(finamount);

            //if ((HttpContext.Session.GetInt32("ID")) == 0 || (HttpContext.Session.GetInt32("ID")) == null)
            //{
            //    return RedirectToPage("/index");
            //}
            //else
            //{
            //    if ((HttpContext.Session.GetInt32("DID")) == 50101)
            //    {
            //        return Page();
            //    }
            //    else
            //    {
            //        return RedirectToPage("/index");
            //    }


            //}
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
                ChartJson = JsonConvert.SerializeObject(BarChart, new JsonSerializerSettings
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
                ChartJson1 = JsonConvert.SerializeObject(PieChart, new JsonSerializerSettings
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




        public IActionResult  OnPostRange()
        {
            HttpContext.Session.SetInt32("range", RangeStart);
            Console.WriteLine(RangeStart);
            Console.WriteLine(Range);
            return RedirectToPage("/Stat");
            
        }

        public IActionResult OnPostFin()
        {
            HttpContext.Session.SetInt32("range2", Range);
            Console.WriteLine(Range);
            return RedirectToPage("/Stat");

        }

    }
}
