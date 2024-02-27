using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using C_Internship.Models;

namespace C_Internship.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TimeEntriesController : ControllerBase
{
    private readonly TimeEntryService _timeEntryService;

    public TimeEntriesController(TimeEntryService timeEntryService)
    {
        _timeEntryService = timeEntryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetTimeEntries()
    {
        //todo try catch
        var people = await _timeEntryService.GetTimeEntriesAsync();
        //List<KeyValuePair<string, TimeSpan>> sortedList = people.OrderBy(task => task.Value).ToList();
        List<Person> sortedPeople = people.Select(people => new Person { Name = people.Key, WorkingHours = people.Value.TotalHours })
                                                .OrderBy(p => p.WorkingHours)
                                                .ToList();
        
        foreach (var person in sortedPeople)
        {
            Console.WriteLine($"Name: {person.Name}, Working Hours: {person.WorkingHours}");
        }

        return Ok(sortedPeople);
    }

}
