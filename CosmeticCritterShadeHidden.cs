using System;
using UnityEngine;

// Token: 0x020000AB RID: 171
public class CosmeticCritterShadeHidden : CosmeticCritter
{
	// Token: 0x06000437 RID: 1079 RVA: 0x000189BE File Offset: 0x00016BBE
	public void SetCenterAndRadius(Vector3 center, float radius)
	{
		this.orbitCenter = center;
		this.orbitRadius = radius;
	}

	// Token: 0x06000438 RID: 1080 RVA: 0x000189CE File Offset: 0x00016BCE
	public override void SetRandomVariables()
	{
		this.initialAngle = Random.Range(0f, 6.2831855f);
		this.orbitDirection = ((Random.value > 0.5f) ? 1f : (-1f));
	}

	// Token: 0x06000439 RID: 1081 RVA: 0x00018A04 File Offset: 0x00016C04
	public override void Tick()
	{
		float num = (float)base.GetAliveTime();
		float num2 = this.initialAngle + this.orbitDegreesPerSecond * num * this.orbitDirection;
		float num3 = this.verticalBobMagnitude * Mathf.Sin(num * this.verticalBobFrequency);
		base.transform.position = this.orbitCenter + new Vector3(this.orbitRadius * Mathf.Cos(num2), num3, this.orbitRadius * Mathf.Sin(num2));
	}

	// Token: 0x040004BA RID: 1210
	[Space]
	[Tooltip("How quickly the Shade orbits around the point where it spawned (the spawner's position).")]
	[SerializeField]
	private float orbitDegreesPerSecond;

	// Token: 0x040004BB RID: 1211
	[Tooltip("The strength of additional up-and-down motion while orbiting.")]
	[SerializeField]
	private float verticalBobMagnitude;

	// Token: 0x040004BC RID: 1212
	[Tooltip("The frequency of additional up-and-down motion while orbiting.")]
	[SerializeField]
	private float verticalBobFrequency;

	// Token: 0x040004BD RID: 1213
	private Vector3 orbitCenter;

	// Token: 0x040004BE RID: 1214
	private float initialAngle;

	// Token: 0x040004BF RID: 1215
	private float orbitRadius;

	// Token: 0x040004C0 RID: 1216
	private float orbitDirection;
}
