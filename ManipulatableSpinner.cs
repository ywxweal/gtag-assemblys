using System;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x02000411 RID: 1041
[RequireComponent(typeof(BezierSpline))]
public class ManipulatableSpinner : ManipulatableObject
{
	// Token: 0x170002C8 RID: 712
	// (get) Token: 0x0600195B RID: 6491 RVA: 0x0007ADCD File Offset: 0x00078FCD
	// (set) Token: 0x0600195C RID: 6492 RVA: 0x0007ADD5 File Offset: 0x00078FD5
	public float angle { get; private set; }

	// Token: 0x0600195D RID: 6493 RVA: 0x0007ADDE File Offset: 0x00078FDE
	private void Awake()
	{
		this.spline = base.GetComponent<BezierSpline>();
	}

	// Token: 0x0600195E RID: 6494 RVA: 0x0007ADEC File Offset: 0x00078FEC
	protected override void OnStartManipulation(GameObject grabbingHand)
	{
		Vector3 position = grabbingHand.transform.position;
		float num = this.FindPositionOnSpline(position);
		this.previousHandT = num;
	}

	// Token: 0x0600195F RID: 6495 RVA: 0x000023F4 File Offset: 0x000005F4
	protected override void OnStopManipulation(GameObject releasingHand, Vector3 releaseVelocity)
	{
	}

	// Token: 0x06001960 RID: 6496 RVA: 0x0007AE14 File Offset: 0x00079014
	protected override bool ShouldHandDetach(GameObject hand)
	{
		if (!this.spline.Loop && (this.currentHandT >= 0.99f || this.currentHandT <= 0.01f))
		{
			return true;
		}
		Vector3 position = hand.transform.position;
		Vector3 point = this.spline.GetPoint(this.currentHandT);
		return Vector3.SqrMagnitude(position - point) > this.breakDistance * this.breakDistance;
	}

	// Token: 0x06001961 RID: 6497 RVA: 0x0007AE84 File Offset: 0x00079084
	protected override void OnHeldUpdate(GameObject hand)
	{
		float angle = this.angle;
		Vector3 position = hand.transform.position;
		this.currentHandT = this.FindPositionOnSpline(position);
		float num = this.currentHandT - this.previousHandT;
		if (this.spline.Loop)
		{
			if (num > 0.5f)
			{
				num -= 1f;
			}
			else if (num < -0.5f)
			{
				num += 1f;
			}
		}
		this.angle += num;
		this.previousHandT = this.currentHandT;
		if (this.applyReleaseVelocity && this.currentHandT <= 0.99f && this.currentHandT >= 0.01f)
		{
			this.tVelocity = (this.angle - angle) / Time.deltaTime;
		}
	}

	// Token: 0x06001962 RID: 6498 RVA: 0x0007AF40 File Offset: 0x00079140
	protected override void OnReleasedUpdate()
	{
		if (this.tVelocity != 0f)
		{
			this.angle += this.tVelocity * Time.deltaTime;
			if (Mathf.Abs(this.tVelocity) < this.lowSpeedThreshold)
			{
				this.tVelocity *= 1f - this.lowSpeedDrag * Time.deltaTime;
				return;
			}
			this.tVelocity *= 1f - this.releaseDrag * Time.deltaTime;
		}
	}

	// Token: 0x06001963 RID: 6499 RVA: 0x0007AFC8 File Offset: 0x000791C8
	private float FindPositionOnSpline(Vector3 grabPoint)
	{
		int i = 0;
		int num = 200;
		float num2 = 0.001f;
		float num3 = 1f / (float)num;
		float3 @float = base.transform.InverseTransformPoint(grabPoint);
		float num4 = 0f;
		float num5 = float.PositiveInfinity;
		while (i < num)
		{
			float num6 = math.distancesq(this.spline.GetPointLocal(num2), @float);
			if (num6 < num5)
			{
				num5 = num6;
				num4 = num2;
			}
			num2 += num3;
			i++;
		}
		return num4;
	}

	// Token: 0x06001964 RID: 6500 RVA: 0x0007B044 File Offset: 0x00079244
	public void SetAngle(float newAngle)
	{
		this.angle = newAngle;
	}

	// Token: 0x06001965 RID: 6501 RVA: 0x0007B04D File Offset: 0x0007924D
	public void SetVelocity(float newVelocity)
	{
		this.tVelocity = newVelocity;
	}

	// Token: 0x04001C2C RID: 7212
	public float breakDistance = 0.2f;

	// Token: 0x04001C2D RID: 7213
	public bool applyReleaseVelocity;

	// Token: 0x04001C2E RID: 7214
	public float releaseDrag = 1f;

	// Token: 0x04001C2F RID: 7215
	public float lowSpeedThreshold = 0.12f;

	// Token: 0x04001C30 RID: 7216
	public float lowSpeedDrag = 3f;

	// Token: 0x04001C31 RID: 7217
	private BezierSpline spline;

	// Token: 0x04001C32 RID: 7218
	private float previousHandT;

	// Token: 0x04001C33 RID: 7219
	private float currentHandT;

	// Token: 0x04001C34 RID: 7220
	private float tVelocity;
}
