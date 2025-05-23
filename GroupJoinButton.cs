using System;
using GorillaNetworking;
using Photon.Pun;

// Token: 0x02000650 RID: 1616
public class GroupJoinButton : GorillaPressableButton
{
	// Token: 0x06002857 RID: 10327 RVA: 0x000C922E File Offset: 0x000C742E
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		if (this.inPrivate)
		{
			GorillaComputer.instance.OnGroupJoinButtonPress(this.gameModeIndex, this.friendCollider);
		}
	}

	// Token: 0x06002858 RID: 10328 RVA: 0x000C9256 File Offset: 0x000C7456
	public void Update()
	{
		this.inPrivate = PhotonNetwork.InRoom && !PhotonNetwork.CurrentRoom.IsVisible;
		if (!this.inPrivate)
		{
			this.isOn = true;
		}
	}

	// Token: 0x04002D3C RID: 11580
	public int gameModeIndex;

	// Token: 0x04002D3D RID: 11581
	public GorillaFriendCollider friendCollider;

	// Token: 0x04002D3E RID: 11582
	public bool inPrivate;
}
