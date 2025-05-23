using System;
using GorillaNetworking;
using TMPro;
using UnityEngine;

// Token: 0x020001E6 RID: 486
public class JoinTriggerUI : MonoBehaviour
{
	// Token: 0x06000B4C RID: 2892 RVA: 0x0003C6FB File Offset: 0x0003A8FB
	private void Awake()
	{
		this.joinTriggerRef.TryResolve<GorillaNetworkJoinTrigger>(out this.joinTrigger);
	}

	// Token: 0x06000B4D RID: 2893 RVA: 0x0003C70F File Offset: 0x0003A90F
	private void Start()
	{
		this.didStart = true;
		this.OnEnable();
	}

	// Token: 0x06000B4E RID: 2894 RVA: 0x0003C71E File Offset: 0x0003A91E
	private void OnEnable()
	{
		if (this.didStart)
		{
			this.joinTrigger.RegisterUI(this);
		}
	}

	// Token: 0x06000B4F RID: 2895 RVA: 0x0003C734 File Offset: 0x0003A934
	private void OnDisable()
	{
		this.joinTrigger.UnregisterUI(this);
	}

	// Token: 0x06000B50 RID: 2896 RVA: 0x0003C744 File Offset: 0x0003A944
	public void SetState(JoinTriggerVisualState state, Func<string> oldZone, Func<string> newZone, Func<string> oldGameMode, Func<string> newGameMode)
	{
		switch (state)
		{
		case JoinTriggerVisualState.ConnectionError:
			this.milestoneRenderer.sharedMaterial = this.template.Milestone_Error;
			this.screenBGRenderer.sharedMaterial = this.template.ScreenBG_Error;
			this.screenText.text = (this.template.showFullErrorMessages ? GorillaScoreboardTotalUpdater.instance.offlineTextErrorString : this.template.ScreenText_Error);
			return;
		case JoinTriggerVisualState.AlreadyInRoom:
			this.milestoneRenderer.sharedMaterial = this.template.Milestone_AlreadyInRoom;
			this.screenBGRenderer.sharedMaterial = this.template.ScreenBG_AlreadyInRoom;
			this.screenText.text = this.template.ScreenText_AlreadyInRoom.GetText(oldZone, newZone, oldGameMode, newGameMode);
			return;
		case JoinTriggerVisualState.InPrivateRoom:
			this.milestoneRenderer.sharedMaterial = this.template.Milestone_InPrivateRoom;
			this.screenBGRenderer.sharedMaterial = this.template.ScreenBG_InPrivateRoom;
			this.screenText.text = this.template.ScreenText_InPrivateRoom.GetText(oldZone, newZone, oldGameMode, newGameMode);
			return;
		case JoinTriggerVisualState.NotConnectedSoloJoin:
			this.milestoneRenderer.sharedMaterial = this.template.Milestone_NotConnectedSoloJoin;
			this.screenBGRenderer.sharedMaterial = this.template.ScreenBG_NotConnectedSoloJoin;
			this.screenText.text = this.template.ScreenText_NotConnectedSoloJoin.GetText(oldZone, newZone, oldGameMode, newGameMode);
			return;
		case JoinTriggerVisualState.LeaveRoomAndSoloJoin:
			this.milestoneRenderer.sharedMaterial = this.template.Milestone_LeaveRoomAndSoloJoin;
			this.screenBGRenderer.sharedMaterial = this.template.ScreenBG_LeaveRoomAndSoloJoin;
			this.screenText.text = this.template.ScreenText_LeaveRoomAndSoloJoin.GetText(oldZone, newZone, oldGameMode, newGameMode);
			return;
		case JoinTriggerVisualState.LeaveRoomAndPartyJoin:
			this.milestoneRenderer.sharedMaterial = this.template.Milestone_LeaveRoomAndGroupJoin;
			this.screenBGRenderer.sharedMaterial = this.template.ScreenBG_LeaveRoomAndGroupJoin;
			this.screenText.text = this.template.ScreenText_LeaveRoomAndGroupJoin.GetText(oldZone, newZone, oldGameMode, newGameMode);
			return;
		case JoinTriggerVisualState.AbandonPartyAndSoloJoin:
			this.milestoneRenderer.sharedMaterial = this.template.Milestone_AbandonPartyAndSoloJoin;
			this.screenBGRenderer.sharedMaterial = this.template.ScreenBG_AbandonPartyAndSoloJoin;
			this.screenText.text = this.template.ScreenText_AbandonPartyAndSoloJoin.GetText(oldZone, newZone, oldGameMode, newGameMode);
			return;
		case JoinTriggerVisualState.ChangingGameModeSoloJoin:
			this.milestoneRenderer.sharedMaterial = this.template.Milestone_ChangingGameModeSoloJoin;
			this.screenBGRenderer.sharedMaterial = this.template.ScreenBG_ChangingGameModeSoloJoin;
			this.screenText.text = this.template.ScreenText_ChangingGameModeSoloJoin.GetText(oldZone, newZone, oldGameMode, newGameMode);
			return;
		default:
			return;
		}
	}

	// Token: 0x04000DC5 RID: 3525
	[SerializeField]
	private XSceneRef joinTriggerRef;

	// Token: 0x04000DC6 RID: 3526
	private GorillaNetworkJoinTrigger joinTrigger;

	// Token: 0x04000DC7 RID: 3527
	[SerializeField]
	private MeshRenderer milestoneRenderer;

	// Token: 0x04000DC8 RID: 3528
	[SerializeField]
	private MeshRenderer screenBGRenderer;

	// Token: 0x04000DC9 RID: 3529
	[SerializeField]
	private TextMeshPro screenText;

	// Token: 0x04000DCA RID: 3530
	[SerializeField]
	private JoinTriggerUITemplate template;

	// Token: 0x04000DCB RID: 3531
	private bool didStart;
}
