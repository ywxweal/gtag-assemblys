using System;
using System.Collections;
using GorillaTagScripts.ModIO;
using UnityEngine;

// Token: 0x020006FA RID: 1786
public class CustomMapsLoadRoomMapButton : GorillaPressableButton
{
	// Token: 0x06002C73 RID: 11379 RVA: 0x000DB5F2 File Offset: 0x000D97F2
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		base.StartCoroutine(this.ButtonPressed_Local());
		if (CustomMapManager.CanLoadRoomMap())
		{
			CustomMapManager.ApproveAndLoadRoomMap();
		}
	}

	// Token: 0x06002C74 RID: 11380 RVA: 0x000DB613 File Offset: 0x000D9813
	private IEnumerator ButtonPressed_Local()
	{
		this.isOn = true;
		this.UpdateColor();
		yield return new WaitForSeconds(this.pressedTime);
		this.isOn = false;
		this.UpdateColor();
		yield break;
	}

	// Token: 0x040032C5 RID: 12997
	[SerializeField]
	private float pressedTime = 0.2f;
}
