namespace Data.Models;

public class JobProgress
{
    public string JobId { get; set; } = null!;
    public int LastCheckpoint { get; set; }
    public bool IsComplete { get; set; }

    private JobProgress()
    {
        
    }

    public JobProgress(string jobId, int lastCheckpoint, bool isComplete)
    {
        JobId = jobId;
        LastCheckpoint = lastCheckpoint;
        IsComplete = isComplete;
    }
}