using Microsoft.Graph.Models;

namespace Azure_Semantic_Kernel_Workshop
{
  public interface IGraphService
  {
    Task<List<Event>> GetOutlookEventsAsync(DateTime day);
    Task AddEventToCalendarAsync(DateTime date, string subject, DateTime startTime, DateTime endTime);
  }
}

