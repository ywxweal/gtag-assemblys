using System;
using UnityEngine;

// Token: 0x020009FF RID: 2559
public class LightningGenerator : MonoBehaviour
{
	// Token: 0x06003D2B RID: 15659 RVA: 0x0012272C File Offset: 0x0012092C
	private void Awake()
	{
		this.strikes = new LightningStrike[this.maxConcurrentStrikes];
		for (int i = 0; i < this.strikes.Length; i++)
		{
			if (i == 0)
			{
				this.strikes[i] = this.prototype;
			}
			else
			{
				this.strikes[i] = Object.Instantiate<LightningStrike>(this.prototype, base.transform);
			}
			this.strikes[i].gameObject.SetActive(false);
		}
	}

	// Token: 0x06003D2C RID: 15660 RVA: 0x0012279C File Offset: 0x0012099C
	private void OnEnable()
	{
		LightningDispatcher.RequestLightningStrike += this.LightningDispatcher_RequestLightningStrike;
	}

	// Token: 0x06003D2D RID: 15661 RVA: 0x001227AF File Offset: 0x001209AF
	private void OnDisable()
	{
		LightningDispatcher.RequestLightningStrike -= this.LightningDispatcher_RequestLightningStrike;
	}

	// Token: 0x06003D2E RID: 15662 RVA: 0x001227C2 File Offset: 0x001209C2
	private LightningStrike LightningDispatcher_RequestLightningStrike(Vector3 t1, Vector3 t2)
	{
		this.index = (this.index + 1) % this.strikes.Length;
		return this.strikes[this.index];
	}

	// Token: 0x040040E1 RID: 16609
	[SerializeField]
	private uint maxConcurrentStrikes = 10U;

	// Token: 0x040040E2 RID: 16610
	[SerializeField]
	private LightningStrike prototype;

	// Token: 0x040040E3 RID: 16611
	private LightningStrike[] strikes;

	// Token: 0x040040E4 RID: 16612
	private int index;
}
