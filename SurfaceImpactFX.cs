using System;
using UnityEngine;

// Token: 0x020009F8 RID: 2552
public class SurfaceImpactFX : MonoBehaviour
{
	// Token: 0x06003D0E RID: 15630 RVA: 0x00122114 File Offset: 0x00120314
	public void Awake()
	{
		if (this.particleFX == null)
		{
			this.particleFX = base.GetComponent<ParticleSystem>();
		}
		if (this.particleFX == null)
		{
			Debug.LogError("SurfaceImpactFX: No ParticleSystem found! Disabling component.", this);
			base.enabled = false;
			return;
		}
		this.fxMainModule = this.particleFX.main;
	}

	// Token: 0x06003D0F RID: 15631 RVA: 0x0012216D File Offset: 0x0012036D
	public void SetScale(float scale)
	{
		this.fxMainModule.gravityModifierMultiplier = this.startingGravityModifier * scale;
		base.transform.localScale = this.startingScale * scale;
	}

	// Token: 0x040040C9 RID: 16585
	public ParticleSystem particleFX;

	// Token: 0x040040CA RID: 16586
	public float startingGravityModifier;

	// Token: 0x040040CB RID: 16587
	public Vector3 startingScale = Vector3.one;

	// Token: 0x040040CC RID: 16588
	private ParticleSystem.MainModule fxMainModule;
}
