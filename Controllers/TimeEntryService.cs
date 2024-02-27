using Newtonsoft.Json;
using C_Internship.Models;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.SkiaSharp;
using SkiaSharp;

public class TimeEntryService
{
    private readonly HttpClient _httpClient;
    private const string ApiUrl = "https://rc-vault-fap-live-1.azurewebsites.net/api/gettimeentries?code=vO17RnE8vuzXzPJo5eaLLjXjmRW07law99QTD90zat9FfOQJKKUcgQ==";
    private const string ApiKey = "vO17RnE8vuzXzPJo5eaLLjXjmRW07law99QTD90zat9FfOQJKKUcgQ==";

    public TimeEntryService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Dictionary<string, TimeSpan>> GetTimeEntriesAsync()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, ApiUrl);
        request.Headers.Add("Authorization", $"Bearer {ApiKey}");

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        var timeEntries = JsonConvert.DeserializeObject<List<TimeEntry>>(json);

        Dictionary<string, TimeSpan> myDictionary = new Dictionary<string, TimeSpan>();

        if (timeEntries != null)
            foreach (var entry in timeEntries)
            {
                TimeSpan duration = entry.EndTimeUtc - entry.StarTimeUtc;
                if (!string.IsNullOrEmpty(entry.EmployeeName))
                {
                    if (!myDictionary.ContainsKey(entry.EmployeeName))
                    {
                        myDictionary.Add(entry.EmployeeName, duration);
                    }
                    else
                    {
                        myDictionary[entry.EmployeeName] += duration;
                    }
                }
            }

        return myDictionary;
    }

    public string GeneratePieChart(List<Person> people)
    {
        string base64Result = string.Empty;

        var model = new PlotModel { Title = "Total Time Worked by Employee" };
        //model.Background = OxyColor.FromRgb(255, 255, 255); 
        var pieSeries = new PieSeries { StrokeThickness = 2.0, InsideLabelPosition = 0.8, AngleSpan = 360, StartAngle = 0 };

        double totalTime = people.Sum(e => e.WorkingHours);
        foreach (var employee in people)
        {
            double percentage = (employee.WorkingHours / totalTime) * 100;
            pieSeries.Slices.Add(new PieSlice(employee.Name, percentage) { IsExploded = false });
        }

        model.Series.Add(pieSeries);

        var width = 600;
        var height = 400;
        try
        {
            var bitmap = new SKBitmap(width, height);
            var canvas = new SKCanvas(bitmap);
            canvas.Clear(SKColors.White);

            var renderContext = new SkiaRenderContext()
            {
                SkCanvas = canvas
            };

            using(MemoryStream ms = new MemoryStream())
            {
                PngExporter.Export(model, ms, width, height);

                var bytes = ms.ToArray();
                base64Result = Convert.ToBase64String(bytes);
            }

            //model.Update(true);
            //model.Render(renderContext, width, height);

            return base64Result;
        }
        catch (Exception)
        {
            throw;
        }
    }
}
