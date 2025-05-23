using System;
using System.Collections;
using GorillaTagScripts.ModIO;
using UnityEngine;

// Token: 0x020006FA RID: 1786
public class CustomMapsLoadRoomMapButton : GorillaPressableButton
{
	// Token: 0x06002C72 RID: 11378 RVA: 0x000DB54E File Offset: 0x000D974E
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		base.StartCoroutine(this.ButtonPressed_Local());
		if (CustomMapManager.CanLoadRoomMap())
		{
			CustomMapManager.ApproveAndLoadRoomMap();
		}
	}

	// Token: 0x06002C73 RID: 11379 RVA: 0x000DB56F File Offset: 0x000D976F
	private IEnumerator ButtonPressed_Local()
	{
		this.isOn = true;
		this.UpdateColor();
		yield return new WaitForSeconds(this.pressedTime);
		this.isOn = false;
		this.UpdateColor();
		yield break;
	}

	// Token: 0x040032C3 RID: 12995
	[SerializeField]
	private float pressedTime = 0.2f;
}
