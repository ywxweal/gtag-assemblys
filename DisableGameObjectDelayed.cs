using System;
using UnityEngine;

// Token: 0x0200098D RID: 2445
public class DisableGameObjectDelayed : MonoBehaviour
{
	// Token: 0x06003ABC RID: 15036 RVA: 0x00119202 File Offset: 0x00117402
	private void OnEnable()
	{
		this.enabledTime = Time.time;
	}

	// Token: 0x06003ABD RID: 15037 RVA: 0x0011920F File Offset: 0x0011740F
	private void Update()
	{
		if (Time.time > this.enabledTime + this.delayTime)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x04003F99 RID: 16281
	public float delayTime = 1f;

	// Token: 0x04003F9A RID: 16282
	public float enabledTime;
}
