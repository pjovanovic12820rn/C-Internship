using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using C_Internship.Models;
using Microsoft.AspNetCore.Mvc;
using SkiaSharp;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace C_Internship.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TimeEntriesController : Controller
{
    private readonly TimeEntryService _timeEntryService;

    public TimeEntriesController(TimeEntryService timeEntryService)
    {
        _timeEntryService = timeEntryService;
    }

    [HttpGet("GetImageAsBase64")]
    public async Task<IActionResult> GetImageAsBase64()
    {

        try
        {
            // mora biti u try catch bloku ukoliko pukne GetTimeEntriesAsync metoda da ima isti error handling
            var people = await _timeEntryService.GetTimeEntriesAsync();

            List<Person> sortedPeople = people.Select(people => new Person { Name = people.Key, WorkingHours = Math.Round(people.Value.TotalHours, 2) })
                                            .OrderBy(p => p.WorkingHours)
                                            .ToList();

            return Json(new { ImageData = _timeEntryService.GeneratePieChart(sortedPeople) });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

    }

    [HttpGet]
    public async Task<IActionResult> GetTimeEntries()
    {
        //todo try catch
        var people = await _timeEntryService.GetTimeEntriesAsync();

        List<Person> sortedPeople = people.Select(people => new Person { Name = people.Key, WorkingHours = Math.Round(people.Value.TotalHours, 2) })
                                                .OrderBy(p => p.WorkingHours)
                                                .ToList();


        return Ok(sortedPeople);
    }

}
