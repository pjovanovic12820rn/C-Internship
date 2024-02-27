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
    public async Task<IActionResult> GetImageAsBase64(){
        var people = await _timeEntryService.GetTimeEntriesAsync();

        List<Person> sortedPeople = people.Select(people => new Person { Name = people.Key, WorkingHours = Math.Round(people.Value.TotalHours, 2) })
                                        .OrderBy(p => p.WorkingHours)
                                        .ToList();

        try{
            var bitmap = _timeEntryService.GeneratePieChart(sortedPeople); 
            // Encode the SKBitmap to a PNG and then to a Base64 string
            using (var image = SKImage.FromBitmap(bitmap))
            {
            using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
            {
                var base64String = Convert.ToBase64String(data.ToArray());

                // Return the Base64 string in a JSON response
                return Json(new { ImageData = base64String });
            }
        }
        }
        catch(Exception ex){
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
