using System;
using UnityEngine;

// Token: 0x0200059A RID: 1434
public class GRCollectible : MonoBehaviour
{
	// Token: 0x0600230A RID: 8970 RVA: 0x000023F4 File Offset: 0x000005F4
	private void Awake()
	{
	}

	// Token: 0x0600230B RID: 8971 RVA: 0x000AF596 File Offset: 0x000AD796
	public void InvokeOnCollected()
	{
		Action onCollected = this.OnCollected;
		if (onCollected == null)
		{
			return;
		}
		onCollected();
	}

	// Token: 0x04002736 RID: 10038
	public GameEntity entity;

	// Token: 0x04002737 RID: 10039
	public int energyValue = 100;

	// Token: 0x04002738 RID: 10040
	public Action OnCollected;
}
