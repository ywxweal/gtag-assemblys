using System;
using UnityEngine;

// Token: 0x0200040D RID: 1037
public class TestManipulatableCube : ManipulatableObject
{
	// Token: 0x0600193D RID: 6461 RVA: 0x0007A25C File Offset: 0x0007845C
	private void Awake()
	{
		this.localSpace = base.transform.worldToLocalMatrix;
		this.startingPos = base.transform.localPosition;
	}

	// Token: 0x0600193E RID: 6462 RVA: 0x000023F4 File Offset: 0x000005F4
	protected override void OnStartManipulation(GameObject grabbingHand)
	{
	}

	// Token: 0x0600193F RID: 6463 RVA: 0x0007A280 File Offset: 0x00078480
	protected override void OnStopManipulation(GameObject releasingHand, Vector3 releaseVelocity)
	{
		if (this.applyReleaseVelocity)
		{
			this.velocity = this.localSpace.MultiplyVector(releaseVelocity);
		}
	}

	// Token: 0x06001940 RID: 6464 RVA: 0x0007A29C File Offset: 0x0007849C
	protected override bool ShouldHandDetach(GameObject hand)
	{
		Vector3 position = base.transform.position;
		Vector3 position2 = hand.transform.position;
		return Vector3.SqrMagnitude(position - position2) > this.breakDistance * this.breakDistance;
	}

	// Token: 0x06001941 RID: 6465 RVA: 0x0007A2DC File Offset: 0x000784DC
	protected override void OnHeldUpdate(GameObject hand)
	{
		Vector3 vector = this.localSpace.MultiplyPoint3x4(hand.transform.position);
		vector.x = Mathf.Clamp(vector.x, this.minXOffset, this.maxXOffset);
		vector.y = Mathf.Clamp(vector.y, this.minYOffset, this.maxYOffset);
		vector.z = Mathf.Clamp(vector.z, this.minZOffset, this.maxZOffset);
		vector += this.startingPos;
		base.transform.localPosition = vector;
	}

	// Token: 0x06001942 RID: 6466 RVA: 0x0007A374 File Offset: 0x00078574
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

	// Token: 0x06001943 RID: 6467 RVA: 0x0007A525 File Offset: 0x00078725
	public Matrix4x4 GetLocalSpace()
	{
		return this.localSpace;
	}

	// Token: 0x06001944 RID: 6468 RVA: 0x0007A530 File Offset: 0x00078730
	public void SetCubeToSpecificPosition(Vector3 pos)
	{
		Vector3 vector = this.localSpace.MultiplyPoint3x4(pos);
		vector.x = Mathf.Clamp(vector.x, this.minXOffset, this.maxXOffset);
		vector.y = Mathf.Clamp(vector.y, this.minYOffset, this.maxYOffset);
		vector.z = Mathf.Clamp(vector.z, this.minZOffset, this.maxZOffset);
		vector += this.startingPos;
		base.transform.localPosition = vector;
	}

	// Token: 0x06001945 RID: 6469 RVA: 0x0007A5C0 File Offset: 0x000787C0
	public void SetCubeToSpecificPosition(float x, float y, float z)
	{
		Vector3 vector = new Vector3(0f, 0f, 0f);
		vector.x = Mathf.Clamp(x, this.minXOffset, this.maxXOffset);
		vector.y = Mathf.Clamp(y, this.minYOffset, this.maxYOffset);
		vector.z = Mathf.Clamp(z, this.minZOffset, this.maxZOffset);
		vector += this.startingPos;
		base.transform.localPosition = vector;
	}

	// Token: 0x04001C06 RID: 7174
	public float breakDistance = 0.2f;

	// Token: 0x04001C07 RID: 7175
	public float maxXOffset;

	// Token: 0x04001C08 RID: 7176
	public float minXOffset;

	// Token: 0x04001C09 RID: 7177
	public float maxYOffset;

	// Token: 0x04001C0A RID: 7178
	public float minYOffset;

	// Token: 0x04001C0B RID: 7179
	public float maxZOffset;

	// Token: 0x04001C0C RID: 7180
	public float minZOffset;

	// Token: 0x04001C0D RID: 7181
	public bool applyReleaseVelocity;

	// Token: 0x04001C0E RID: 7182
	public float releaseDrag = 1f;

	// Token: 0x04001C0F RID: 7183
	private Matrix4x4 localSpace;

	// Token: 0x04001C10 RID: 7184
	private Vector3 startingPos;

	// Token: 0x04001C11 RID: 7185
	private Vector3 velocity;
}
