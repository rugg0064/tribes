public struct TribesScoreboardStruct
{
	public bool team;
	public int deaths;

	public TribesScoreboardStruct(bool team)
	{
		this.team = team;
		this.deaths = 0;
	}

	public override string ToString()
	{
		return "Deaths: " + deaths.ToString() + " | Team: " + (team ? "Red" : "Blu");
	}

	public override bool Equals( object obj )
	{
		if ( !(obj is TribesScoreboardStruct) )
		{
			return false;
		}
		TribesScoreboardStruct other = (TribesScoreboardStruct) obj;
		return this.team == other.team &&
			this.deaths == other.deaths;
	}

	public override int GetHashCode()
	{
		return this.team ? 1 : 0 << 31 + (this.deaths & 0x7FFFFFFF);
		
	}
}
