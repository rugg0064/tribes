using Sandbox;
using Tribes;
public class TribesFlag : ModelEntity
{
	public bool following;
	public TribesPlayer followingPlayer;
	int followingBoneID;
	//public Transform followTransform;

	public TribesFlag() : base()
	{

	}

	public TribesFlag(string s) : base(s)
	{

	}

	public void stopFollowingBone()
	{
		following = false;
		this.EnableAllCollisions = true;
		this.Rotation = Rotation.Identity;
	}

	public void followPlayer(TribesPlayer p)
	{
		this.following = true;
		this.followingPlayer = p;
		this.followingBoneID = followingPlayer.GetBoneIndex("spine_2");
		this.EnableAllCollisions = false;
	}

	public void followBone(TribesPlayer followingPlayer, int boneid)
	{
		this.following = true;
		this.followingPlayer = followingPlayer;
		this.followingBoneID = boneid;
		//this.followTransform = followingPlayer.GetBoneTransform( boneid );
		this.EnableAllCollisions = false;

	}

	//[Event.Tick, Event.Frame]
	[Event.Tick]
	public void tick()
	{
		if(following)
		{
			Transform followingTransform = followingPlayer.GetBoneTransform( followingBoneID );
			this.Rotation = followingTransform.Rotation * Rotation.FromRoll(90.0f) * Rotation.FromPitch(90.0f);
			this.Position = followingTransform.Position + (followingTransform.Rotation.Right * 15) + (followingTransform.Rotation.Backward * 30);
		}
	}
}
