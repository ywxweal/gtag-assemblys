using System;
using UnityEngine;

// Token: 0x020000A3 RID: 163
public class CosmeticCritterButterfly : CosmeticCritter
{
	// Token: 0x1700004B RID: 75
	// (get) Token: 0x06000415 RID: 1045 RVA: 0x00017F2C File Offset: 0x0001612C
	public ParticleSystem.EmitParams GetEmitParams
	{
		get
		{
			return this.emitParams;
		}
	}

	// Token: 0x06000416 RID: 1046 RVA: 0x00017F34 File Offset: 0x00016134
	public void SetStartPos(Vector3 initialPos)
	{
		this.startPosition = initialPos;
	}

	// Token: 0x06000417 RID: 1047 RVA: 0x00017F40 File Offset: 0x00016140
	public override void SetRandomVariables()
	{
		this.direction = Random.insideUnitSphere;
		this.emitParams.startColor = Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f);
		this.particleSystem.Emit(this.emitParams, 1);
	}

	// Token: 0x06000418 RID: 1048 RVA: 0x00017F9D File Offset: 0x0001619D
	public override void Tick()
	{
		base.transform.position = this.startPosition + (float)base.GetAliveTime() * this.speed * this.direction;
	}

	// Token: 0x0400048A RID: 1162
	[Tooltip("The speed this Butterfly will move at.")]
	[SerializeField]
	private float speed = 1f;

	// Token: 0x0400048B RID: 1163
	[Tooltip("Emit one particle from this particle system when spawning.")]
	[SerializeField]
	private ParticleSystem particleSystem;

	// Token: 0x0400048C RID: 1164
	private Vector3 startPosition;

	// Token: 0x0400048D RID: 1165
	private Vector3 direction;

	// Token: 0x0400048E RID: 1166
	private ParticleSystem.EmitParams emitParams;
}
