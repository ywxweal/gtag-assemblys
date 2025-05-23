using System;
using UnityEngine;

// Token: 0x020001FA RID: 506
public class RaceConsoleVisual : MonoBehaviour
{
	// Token: 0x06000BB4 RID: 2996 RVA: 0x0003E3B0 File Offset: 0x0003C5B0
	public void ShowRaceInProgress(int laps)
	{
		this.button1.sharedMaterial = this.inactiveButton;
		this.button3.sharedMaterial = this.inactiveButton;
		this.button5.sharedMaterial = this.inactiveButton;
		this.button1.transform.localPosition = Vector3.zero;
		this.button3.transform.localPosition = Vector3.zero;
		this.button5.transform.localPosition = Vector3.zero;
		switch (laps)
		{
		default:
			this.button1.sharedMaterial = this.selectedButton;
			this.button1.transform.localPosition = this.buttonPressedOffset;
			return;
		case 3:
			this.button3.sharedMaterial = this.selectedButton;
			this.button3.transform.localPosition = this.buttonPressedOffset;
			return;
		case 5:
			this.button5.sharedMaterial = this.selectedButton;
			this.button5.transform.localPosition = this.buttonPressedOffset;
			return;
		}
	}

	// Token: 0x06000BB5 RID: 2997 RVA: 0x0003E4C4 File Offset: 0x0003C6C4
	public void ShowCanStartRace()
	{
		this.button1.transform.localPosition = Vector3.zero;
		this.button3.transform.localPosition = Vector3.zero;
		this.button5.transform.localPosition = Vector3.zero;
		this.button1.sharedMaterial = this.pressableButton;
		this.button3.sharedMaterial = this.pressableButton;
		this.button5.sharedMaterial = this.pressableButton;
	}

	// Token: 0x04000E41 RID: 3649
	[SerializeField]
	private MeshRenderer button1;

	// Token: 0x04000E42 RID: 3650
	[SerializeField]
	private MeshRenderer button3;

	// Token: 0x04000E43 RID: 3651
	[SerializeField]
	private MeshRenderer button5;

	// Token: 0x04000E44 RID: 3652
	[SerializeField]
	private Vector3 buttonPressedOffset;

	// Token: 0x04000E45 RID: 3653
	[SerializeField]
	private Material pressableButton;

	// Token: 0x04000E46 RID: 3654
	[SerializeField]
	private Material selectedButton;

	// Token: 0x04000E47 RID: 3655
	[SerializeField]
	private Material inactiveButton;
}
