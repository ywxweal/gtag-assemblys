using System;
using UnityEngine;

// Token: 0x020000CD RID: 205
public class GhostLabButton : GorillaPressableButton, IBuildValidation
{
	// Token: 0x06000505 RID: 1285 RVA: 0x0001CED8 File Offset: 0x0001B0D8
	public bool BuildValidationCheck()
	{
		if (this.ghostLab == null)
		{
			Debug.LogError("ghostlab is missing", this);
			return false;
		}
		return true;
	}

	// Token: 0x06000506 RID: 1286 RVA: 0x0001CEF6 File Offset: 0x0001B0F6
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		this.ghostLab.DoorButtonPress(this.buttonIndex, this.forSingleDoor);
	}

	// Token: 0x040005E9 RID: 1513
	public GhostLab ghostLab;

	// Token: 0x040005EA RID: 1514
	public int buttonIndex;

	// Token: 0x040005EB RID: 1515
	public bool forSingleDoor;
}
