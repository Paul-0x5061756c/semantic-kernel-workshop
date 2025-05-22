using System.ComponentModel;
using Microsoft.Graph.Models;
using Microsoft.SemanticKernel;

namespace Azure_Semantic_Kernel_Workshop
{
  public class CalendarPlugin
  {
    private readonly IGraphService _graphService;

    public CalendarPlugin(IGraphService graphService)
    {
      _graphService = graphService;
    }

    [KernelFunction("GetTodaysDateAndTime")]
    [Description("Get today's date and time")]
    [return: Description("Today's date and time")]
    public DateTime GetTodaysDateAndTime()
    {
      var nlTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Amsterdam");
      var nlTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, nlTimeZone);
      return nlTime;
    }

    [KernelFunction("GetCalendarEventsForToday")]
    [Description("Get all the calendar events for input date")]
    [return: Description("List of calendar events for input date with timezone as Europe/Amsterdam")]
    public async Task<List<Event>> GetCalendarEventsForToday(
        [Description("The date to get the events for")] DateTime date)
    {
      return await _graphService.GetOutlookEventsAsync(date);
      
    }

    [KernelFunction("AddEventToCalendar")]
    [Description("Add an event to the calendar")]
    public async Task AddEventToCalendar(
        [Description("The date to add the event for")] DateTime date,
        [Description("The subject of the event")] string subject,
        [Description("The start time of the event")] DateTime startTime,
        [Description("The end time of the event")] DateTime endTime
        )
    {
       await _graphService.AddEventToCalendarAsync(date, subject, startTime, endTime);
    }
  }
}
