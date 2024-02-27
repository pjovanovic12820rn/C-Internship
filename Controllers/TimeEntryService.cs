using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using C_Internship.Models;

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

        if(timeEntries != null)
        foreach (var entry in timeEntries)
        {
            TimeSpan duration = entry.EndTimeUtc - entry.StarTimeUtc;
            if(!string.IsNullOrEmpty(entry.EmployeeName)){
                if(!myDictionary.ContainsKey(entry.EmployeeName) ){
                    myDictionary.Add(entry.EmployeeName, duration);
                }
                else{
                    myDictionary[entry.EmployeeName] += duration;
                }}
        }

        return myDictionary;
    }
}
