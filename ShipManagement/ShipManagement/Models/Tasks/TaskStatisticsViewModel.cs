namespace ShipManagement.Models.Tasks;

public class TaskStatisticsViewModel
{
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int PendingTasks { get; set; }
    public double? AverageCompletionTime { get; set; }
    public int LowPriorityTasks { get; set; }
    public int MediumPriorityTasks { get; set; }
    public int HighPriorityTasks { get; set; }
}