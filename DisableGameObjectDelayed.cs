using System;
using UnityEngine;

// Token: 0x0200098D RID: 2445
public class DisableGameObjectDelayed : MonoBehaviour
{
	// Token: 0x06003ABD RID: 15037 RVA: 0x001192DA File Offset: 0x001174DA
	private void OnEnable()
	{
		this.enabledTime = Time.time;
	}

	// Token: 0x06003ABE RID: 15038 RVA: 0x001192E7 File Offset: 0x001174E7
	private void Update()
	{
		if (Time.time > this.enabledTime + this.delayTime)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x04003F9A RID: 16282
	public float delayTime = 1f;

	// Token: 0x04003F9B RID: 16283
	public float enabledTime;
}
