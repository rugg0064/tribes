public struct TribesScoreboardStruct
{
	public bool team;
	public int kills;
	public int deaths;

	public TribesScoreboardStruct(bool team)
	{
		this.team = team;
		this.kills = 0;
		this.deaths = 0;
	}

	public override bool Equals( object obj )
	{
		if ( !(obj is TribesScoreboardStruct) )
		{
			return false;
		}
		TribesScoreboardStruct other = (TribesScoreboardStruct) obj;
		return this.team == other.team &&
			this.kills == other.kills &&
			this.deaths == other.deaths;
	}

	public override int GetHashCode()
	{
		return this.team ? 1 : 0 << 31 + (this.deaths & 0x0000FFFF << 16) + (this.kills & 0x0000FFFF);
	}
}
