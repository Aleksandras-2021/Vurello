namespace PSK.Server.DTOs;
public class TeamContributionsDTO
{
    public int TotalJobs { get; set; }

    public List<ContributionDetails> Contributions { get; set; } = new();

    public class ContributionDetails
    {
        public Guid MemberId { get; set; }
        public string Username { get; set; }
        public int CompletedJobs { get; set; }
    }
}



