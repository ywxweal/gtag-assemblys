using System;
using UnityEngine;

// Token: 0x020009F8 RID: 2552
public class SurfaceImpactFX : MonoBehaviour
{
	// Token: 0x06003D0F RID: 15631 RVA: 0x001221EC File Offset: 0x001203EC
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

	// Token: 0x06003D10 RID: 15632 RVA: 0x00122245 File Offset: 0x00120445
	public void SetScale(float scale)
	{
		this.fxMainModule.gravityModifierMultiplier = this.startingGravityModifier * scale;
		base.transform.localScale = this.startingScale * scale;
	}

	// Token: 0x040040CA RID: 16586
	public ParticleSystem particleFX;

	// Token: 0x040040CB RID: 16587
	public float startingGravityModifier;

	// Token: 0x040040CC RID: 16588
	public Vector3 startingScale = Vector3.one;

	// Token: 0x040040CD RID: 16589
	private ParticleSystem.MainModule fxMainModule;
}
