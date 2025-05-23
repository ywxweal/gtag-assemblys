using System;
using UnityEngine;

// Token: 0x020009FC RID: 2556
public class LightningDispatcher : MonoBehaviour
{
	// Token: 0x14000072 RID: 114
	// (add) Token: 0x06003D22 RID: 15650 RVA: 0x001225E4 File Offset: 0x001207E4
	// (remove) Token: 0x06003D23 RID: 15651 RVA: 0x00122618 File Offset: 0x00120818
	public static event LightningDispatcher.DispatchLightningEvent RequestLightningStrike;

	// Token: 0x06003D24 RID: 15652 RVA: 0x0012264C File Offset: 0x0012084C
	public void DispatchLightning(Vector3 p1, Vector3 p2)
	{
		if (LightningDispatcher.RequestLightningStrike != null)
		{
			LightningStrike lightningStrike = LightningDispatcher.RequestLightningStrike(p1, p2);
			float num = Mathf.Max(new float[]
			{
				base.transform.lossyScale.x,
				base.transform.lossyScale.y,
				base.transform.lossyScale.z
			});
			lightningStrike.Play(p1, p2, this.beamWidthCM * 0.01f * num, this.soundVolumeMultiplier / num, LightningStrike.rand.NextFloat(this.minDuration, this.maxDuration), this.colorOverLifetime);
		}
	}

	// Token: 0x040040DB RID: 16603
	[SerializeField]
	private float beamWidthCM = 1f;

	// Token: 0x040040DC RID: 16604
	[SerializeField]
	private float soundVolumeMultiplier = 1f;

	// Token: 0x040040DD RID: 16605
	[SerializeField]
	private float minDuration = 0.05f;

	// Token: 0x040040DE RID: 16606
	[SerializeField]
	private float maxDuration = 0.12f;

	// Token: 0x040040DF RID: 16607
	[SerializeField]
	private Gradient colorOverLifetime;

	// Token: 0x020009FD RID: 2557
	// (Invoke) Token: 0x06003D27 RID: 15655
	public delegate LightningStrike DispatchLightningEvent(Vector3 p1, Vector3 p2);
}
