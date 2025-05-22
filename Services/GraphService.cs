using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace Azure_Semantic_Kernel_Workshop
{
public class GraphService : IGraphService
{
    private static GraphServiceClient _graphClient = null!;

    public GraphService(GraphServiceClient graphClient)
    {
        _graphClient = graphClient;
    }

    public async Task<List<Event>> GetOutlookEventsAsync(DateTime day)
    {
        var fromDate = day.Date.ToUniversalTime();
        var toDate = fromDate.AddDays(1);

        var events = await _graphClient.Me.CalendarView
            .GetAsync(config =>
            {
                config.QueryParameters.StartDateTime = fromDate.ToString("o");
                config.QueryParameters.EndDateTime = toDate.ToString("o");
                config.QueryParameters.Select = new[] { "subject", "start", "end", "location" };
                config.QueryParameters.Orderby = new[] { "start/dateTime" };
            });

        // convert events time to local time
        foreach (var calendarEvent in events?.Value ?? Enumerable.Empty<Event>())
        {
            if (calendarEvent.Start != null && calendarEvent.Start.DateTime != null)
            {
                calendarEvent.Start.DateTime = DateTime.Parse(calendarEvent.Start.DateTime).ToLocalTime().ToString("o");
            }
            if (calendarEvent.End != null && calendarEvent.End.DateTime != null)
            {
                calendarEvent.End.DateTime = DateTime.Parse(calendarEvent.End.DateTime).ToLocalTime().ToString("o");
            }
        }


        return events?.Value?.ToList() ?? new List<Event>();
    }

    public async Task AddEventToCalendarAsync(DateTime date, string subject, DateTime startTime, DateTime endTime)
    {
        var calendarEvent = new Event
        {
            Subject = subject,
            Start = new DateTimeTimeZone
            {
                DateTime = startTime.ToString("o"),
                TimeZone = "Europe/Amsterdam"
            },
            End = new DateTimeTimeZone
            {
                DateTime = endTime.ToString("o"),
                TimeZone = "Europe/Amsterdam"
            }
        };

        await _graphClient.Me.Events.PostAsync(calendarEvent);
    }
}

}
