﻿using System;
using UnityEngine;

// Token: 0x020008FD RID: 2301
public class GliderWindVolume : MonoBehaviour
{
	// Token: 0x060037E1 RID: 14305 RVA: 0x0010F88B File Offset: 0x0010DA8B
	public void SetProperties(float speed, float accel, AnimationCurve svaCurve, Vector3 windDirection)
	{
		this.maxSpeed = speed;
		this.maxAccel = accel;
		this.speedVsAccelCurve.CopyFrom(svaCurve);
		this.localWindDirection = windDirection;
	}

	// Token: 0x17000584 RID: 1412
	// (get) Token: 0x060037E2 RID: 14306 RVA: 0x0010F8AF File Offset: 0x0010DAAF
	public Vector3 WindDirection
	{
		get
		{
			return base.transform.TransformDirection(this.localWindDirection);
		}
	}

	// Token: 0x060037E3 RID: 14307 RVA: 0x0010F8C4 File Offset: 0x0010DAC4
	public Vector3 GetAccelFromVelocity(Vector3 velocity)
	{
		Vector3 windDirection = this.WindDirection;
		float num = Mathf.Clamp(Vector3.Dot(velocity, windDirection), -this.maxSpeed, this.maxSpeed) / this.maxSpeed;
		float num2 = this.speedVsAccelCurve.Evaluate(num) * this.maxAccel;
		return windDirection * num2;
	}

	// Token: 0x04003DC0 RID: 15808
	[SerializeField]
	private float maxSpeed = 30f;

	// Token: 0x04003DC1 RID: 15809
	[SerializeField]
	private float maxAccel = 15f;

	// Token: 0x04003DC2 RID: 15810
	[SerializeField]
	private AnimationCurve speedVsAccelCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

	// Token: 0x04003DC3 RID: 15811
	[SerializeField]
	private Vector3 localWindDirection = Vector3.up;
}
