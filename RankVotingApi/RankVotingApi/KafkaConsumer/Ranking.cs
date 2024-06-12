using System.Collections.Generic;

public class Ranking
{
    public string Id { get; set; }
    public string Name { get; set; }
    public IEnumerable<Candidate> Candidates { get; set; } = [];
}
