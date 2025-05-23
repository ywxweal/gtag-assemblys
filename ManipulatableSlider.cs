using System;
using UnityEngine;

// Token: 0x02000410 RID: 1040
public class ManipulatableSlider : ManipulatableObject
{
	// Token: 0x06001950 RID: 6480 RVA: 0x0007A9BE File Offset: 0x00078BBE
	private void Awake()
	{
		this.localSpace = base.transform.worldToLocalMatrix;
		this.startingPos = base.transform.localPosition;
	}

	// Token: 0x06001951 RID: 6481 RVA: 0x000023F4 File Offset: 0x000005F4
	protected override void OnStartManipulation(GameObject grabbingHand)
	{
	}

	// Token: 0x06001952 RID: 6482 RVA: 0x0007A9E2 File Offset: 0x00078BE2
	protected override void OnStopManipulation(GameObject releasingHand, Vector3 releaseVelocity)
	{
		if (this.applyReleaseVelocity)
		{
			this.velocity = this.localSpace.MultiplyVector(releaseVelocity);
		}
	}

	// Token: 0x06001953 RID: 6483 RVA: 0x0007AA00 File Offset: 0x00078C00
	protected override bool ShouldHandDetach(GameObject hand)
	{
		Vector3 position = base.transform.position;
		Vector3 position2 = hand.transform.position;
		return Vector3.SqrMagnitude(position - position2) > this.breakDistance * this.breakDistance;
	}

	// Token: 0x06001954 RID: 6484 RVA: 0x0007AA40 File Offset: 0x00078C40
	protected override void OnHeldUpdate(GameObject hand)
	{
		Vector3 vector = this.localSpace.MultiplyPoint3x4(hand.transform.position);
		vector.x = Mathf.Clamp(vector.x, this.minXOffset, this.maxXOffset);
		vector.y = Mathf.Clamp(vector.y, this.minYOffset, this.maxYOffset);
		vector.z = Mathf.Clamp(vector.z, this.minZOffset, this.maxZOffset);
		vector += this.startingPos;
		base.transform.localPosition = vector;
	}

	// Token: 0x06001955 RID: 6485 RVA: 0x0007AAD8 File Offset: 0x00078CD8
	protected override void OnReleasedUpdate()
	{
		if (this.velocity != Vector3.zero)
		{
			Vector3 vector = this.localSpace.MultiplyPoint(base.transform.position);
			vector += this.velocity * Time.deltaTime;
			if (vector.x < this.minXOffset)
			{
				vector.x = this.minXOffset;
				this.velocity.x = 0f;
			}
			else if (vector.x > this.maxXOffset)
			{
				vector.x = this.maxXOffset;
				this.velocity.x = 0f;
			}
			if (vector.y < this.minYOffset)
			{
				vector.y = this.minYOffset;
				this.velocity.y = 0f;
			}
			else if (vector.y > this.maxYOffset)
			{
				vector.y = this.maxYOffset;
				this.velocity.y = 0f;
			}
			if (vector.z < this.minZOffset)
			{
				vector.z = this.minZOffset;
				this.velocity.z = 0f;
			}
			else if (vector.z > this.maxZOffset)
			{
				vector.z = this.maxZOffset;
				this.velocity.z = 0f;
			}
			vector += this.startingPos;
			base.transform.localPosition = vector;
			this.velocity *= 1f - this.releaseDrag * Time.deltaTime;
			if (this.velocity.sqrMagnitude < 0.001f)
			{
				this.velocity = Vector3.zero;
			}
		}
	}

	// Token: 0x06001956 RID: 6486 RVA: 0x0007AC8C File Offset: 0x00078E8C
	public void SetProgress(float x, float y, float z)
	{
		x = Mathf.Clamp(x, 0f, 1f);
		y = Mathf.Clamp(y, 0f, 1f);
		z = Mathf.Clamp(z, 0f, 1f);
		Vector3 vector = this.startingPos;
		vector.x += Mathf.Lerp(this.minXOffset, this.maxXOffset, x);
		vector.y += Mathf.Lerp(this.minYOffset, this.maxYOffset, y);
		vector.z += Mathf.Lerp(this.minZOffset, this.maxZOffset, z);
		base.transform.localPosition = vector;
	}

	// Token: 0x06001957 RID: 6487 RVA: 0x0007AD39 File Offset: 0x00078F39
	public float GetProgressX()
	{
		return ((base.transform.localPosition - this.startingPos).x - this.minXOffset) / (this.maxXOffset - this.minXOffset);
	}

	// Token: 0x06001958 RID: 6488 RVA: 0x0007AD6B File Offset: 0x00078F6B
	public float GetProgressY()
	{
		return ((base.transform.localPosition - this.startingPos).y - this.minYOffset) / (this.maxYOffset - this.minYOffset);
	}

	// Token: 0x06001959 RID: 6489 RVA: 0x0007AD9D File Offset: 0x00078F9D
	public float GetProgressZ()
	{
		return ((base.transform.localPosition - this.startingPos).z - this.minZOffset) / (this.maxZOffset - this.minZOffset);
	}

	// Token: 0x04001C20 RID: 7200
	public float breakDistance = 0.2f;

	// Token: 0x04001C21 RID: 7201
	public float maxXOffset;

	// Token: 0x04001C22 RID: 7202
	public float minXOffset;

	// Token: 0x04001C23 RID: 7203
	public float maxYOffset;

	// Token: 0x04001C24 RID: 7204
	public float minYOffset;

	// Token: 0x04001C25 RID: 7205
	public float maxZOffset;

	// Token: 0x04001C26 RID: 7206
	public float minZOffset;

	// Token: 0x04001C27 RID: 7207
	public bool applyReleaseVelocity;

	// Token: 0x04001C28 RID: 7208
	public float releaseDrag = 1f;

	// Token: 0x04001C29 RID: 7209
	private Matrix4x4 localSpace;

	// Token: 0x04001C2A RID: 7210
	private Vector3 startingPos;

	// Token: 0x04001C2B RID: 7211
	private Vector3 velocity;
}
