using System;
using UnityEngine;

// Token: 0x020001E7 RID: 487
[CreateAssetMenu(fileName = "JoinTriggerUITemplate", menuName = "ScriptableObjects/JoinTriggerUITemplate")]
public class JoinTriggerUITemplate : ScriptableObject
{
	// Token: 0x04000DCC RID: 3532
	public Material Milestone_Error;

	// Token: 0x04000DCD RID: 3533
	public Material Milestone_AlreadyInRoom;

	// Token: 0x04000DCE RID: 3534
	public Material Milestone_InPrivateRoom;

	// Token: 0x04000DCF RID: 3535
	public Material Milestone_NotConnectedSoloJoin;

	// Token: 0x04000DD0 RID: 3536
	public Material Milestone_LeaveRoomAndSoloJoin;

	// Token: 0x04000DD1 RID: 3537
	public Material Milestone_LeaveRoomAndGroupJoin;

	// Token: 0x04000DD2 RID: 3538
	public Material Milestone_AbandonPartyAndSoloJoin;

	// Token: 0x04000DD3 RID: 3539
	public Material Milestone_ChangingGameModeSoloJoin;

	// Token: 0x04000DD4 RID: 3540
	public Material ScreenBG_Error;

	// Token: 0x04000DD5 RID: 3541
	public Material ScreenBG_AlreadyInRoom;

	// Token: 0x04000DD6 RID: 3542
	public Material ScreenBG_InPrivateRoom;

	// Token: 0x04000DD7 RID: 3543
	public Material ScreenBG_NotConnectedSoloJoin;

	// Token: 0x04000DD8 RID: 3544
	public Material ScreenBG_LeaveRoomAndSoloJoin;

	// Token: 0x04000DD9 RID: 3545
	public Material ScreenBG_LeaveRoomAndGroupJoin;

	// Token: 0x04000DDA RID: 3546
	public Material ScreenBG_AbandonPartyAndSoloJoin;

	// Token: 0x04000DDB RID: 3547
	public Material ScreenBG_ChangingGameModeSoloJoin;

	// Token: 0x04000DDC RID: 3548
	public string ScreenText_Error;

	// Token: 0x04000DDD RID: 3549
	public bool showFullErrorMessages;

	// Token: 0x04000DDE RID: 3550
	public JoinTriggerUITemplate.FormattedString ScreenText_AlreadyInRoom;

	// Token: 0x04000DDF RID: 3551
	public JoinTriggerUITemplate.FormattedString ScreenText_InPrivateRoom;

	// Token: 0x04000DE0 RID: 3552
	public JoinTriggerUITemplate.FormattedString ScreenText_NotConnectedSoloJoin;

	// Token: 0x04000DE1 RID: 3553
	public JoinTriggerUITemplate.FormattedString ScreenText_LeaveRoomAndSoloJoin;

	// Token: 0x04000DE2 RID: 3554
	public JoinTriggerUITemplate.FormattedString ScreenText_LeaveRoomAndGroupJoin;

	// Token: 0x04000DE3 RID: 3555
	public JoinTriggerUITemplate.FormattedString ScreenText_AbandonPartyAndSoloJoin;

	// Token: 0x04000DE4 RID: 3556
	public JoinTriggerUITemplate.FormattedString ScreenText_ChangingGameModeSoloJoin;

	// Token: 0x020001E8 RID: 488
	[Serializable]
	public struct FormattedString
	{
		// Token: 0x06000B53 RID: 2899 RVA: 0x0003C9F5 File Offset: 0x0003ABF5
		public string GetText(string oldZone, string newZone, string oldGameType, string newGameType)
		{
			if (this.formatter == null)
			{
				this.formatter = StringFormatter.Parse(this.formatText);
			}
			return this.formatter.Format(new string[] { oldZone, newZone, oldGameType, newGameType });
		}

		// Token: 0x06000B54 RID: 2900 RVA: 0x0003CA32 File Offset: 0x0003AC32
		public string GetText(Func<string> oldZone, Func<string> newZone, Func<string> oldGameType, Func<string> newGameType)
		{
			if (this.formatter == null)
			{
				this.formatter = StringFormatter.Parse(this.formatText);
			}
			return this.formatter.Format(oldZone, newZone, oldGameType, newGameType);
		}

		// Token: 0x04000DE5 RID: 3557
		[TextArea]
		[SerializeField]
		private string formatText;

		// Token: 0x04000DE6 RID: 3558
		[NonSerialized]
		private StringFormatter formatter;
	}
}
