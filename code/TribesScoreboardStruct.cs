public struct TribesScoreboardStruct
{
	public int netID;
	public bool team;
	public int kills;
	public int deaths;

	public TribesScoreboardStruct(int netID, bool team)
	{
		this.netID = netID;
		this.team = team;
		this.kills = 0;
		this.deaths = 0;
	}

	public TribesScoreboardStruct giveDeath()
	{
		return new TribesScoreboardStruct()
		{
			netID = this.netID,
			team = this.team,
			kills = this.kills,
			deaths = this.deaths + 1
		};
	}

	public TribesScoreboardStruct giveKill()
	{
		return new TribesScoreboardStruct()
		{
			netID = this.netID,
			team = this.team,
			kills = this.kills + 1,
			deaths = this.deaths
		};
	}

	public override bool Equals( object obj )
	{
		if ( !(obj is TribesScoreboardStruct) )
		{
			return false;
		}
		TribesScoreboardStruct other = (TribesScoreboardStruct) obj;
		return this.netID == other.netID &&
			this.team == other.team &&
			this.kills == other.kills &&
			this.deaths == other.deaths;
	}

	public override int GetHashCode()
	{
		return (this.team ? 1 : 0 << 31 + (this.deaths & 0x0000FFFF << 16) + (this.kills & 0x0000FFFF)) ^ netID;
	}
}
