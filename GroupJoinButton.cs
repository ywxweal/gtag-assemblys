using System;
using GorillaNetworking;
using Photon.Pun;

// Token: 0x02000650 RID: 1616
public class GroupJoinButton : GorillaPressableButton
{
	// Token: 0x06002856 RID: 10326 RVA: 0x000C918A File Offset: 0x000C738A
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		if (this.inPrivate)
		{
			GorillaComputer.instance.OnGroupJoinButtonPress(this.gameModeIndex, this.friendCollider);
		}
	}

	// Token: 0x06002857 RID: 10327 RVA: 0x000C91B2 File Offset: 0x000C73B2
	public void Update()
	{
		this.inPrivate = PhotonNetwork.InRoom && !PhotonNetwork.CurrentRoom.IsVisible;
		if (!this.inPrivate)
		{
			this.isOn = true;
		}
	}

	// Token: 0x04002D3A RID: 11578
	public int gameModeIndex;

	// Token: 0x04002D3B RID: 11579
	public GorillaFriendCollider friendCollider;

	// Token: 0x04002D3C RID: 11580
	public bool inPrivate;
}
