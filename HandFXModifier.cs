using System;
using UnityEngine;

// Token: 0x020001E0 RID: 480
public class HandFXModifier : FXModifier
{
	// Token: 0x06000B3F RID: 2879 RVA: 0x0003C5D3 File Offset: 0x0003A7D3
	private void Awake()
	{
		this.originalScale = base.transform.localScale;
	}

	// Token: 0x06000B40 RID: 2880 RVA: 0x0003C5E6 File Offset: 0x0003A7E6
	private void OnDisable()
	{
		base.transform.localScale = this.originalScale;
	}

	// Token: 0x06000B41 RID: 2881 RVA: 0x0003C5F9 File Offset: 0x0003A7F9
	public override void UpdateScale(float scale)
	{
		scale = Mathf.Clamp(scale, this.minScale, this.maxScale);
		base.transform.localScale = this.originalScale * scale;
	}

	// Token: 0x04000DB4 RID: 3508
	private Vector3 originalScale;

	// Token: 0x04000DB5 RID: 3509
	[SerializeField]
	private float minScale;

	// Token: 0x04000DB6 RID: 3510
	[SerializeField]
	private float maxScale;

	// Token: 0x04000DB7 RID: 3511
	[SerializeField]
	private ParticleSystem dustBurst;

	// Token: 0x04000DB8 RID: 3512
	[SerializeField]
	private ParticleSystem dustLinger;
}
