using System;
using UnityEngine;

// Token: 0x020004A2 RID: 1186
public class GorillaWalkingGrab : MonoBehaviour
{
	// Token: 0x06001CB9 RID: 7353 RVA: 0x0008BA75 File Offset: 0x00089C75
	private void Start()
	{
		this.thisRigidbody = base.gameObject.GetComponent<Rigidbody>();
		this.positionHistory = new Vector3[this.historySteps];
		this.historyIndex = 0;
	}

	// Token: 0x06001CBA RID: 7354 RVA: 0x0008BAA0 File Offset: 0x00089CA0
	private void FixedUpdate()
	{
		this.historyIndex++;
		if (this.historyIndex >= this.historySteps)
		{
			this.historyIndex = 0;
		}
		this.positionHistory[this.historyIndex] = this.handToStickTo.transform.position;
		this.thisRigidbody.MovePosition(this.handToStickTo.transform.position);
		base.transform.rotation = this.handToStickTo.transform.rotation;
	}

	// Token: 0x06001CBB RID: 7355 RVA: 0x00002076 File Offset: 0x00000276
	private bool MakeJump()
	{
		return false;
	}

	// Token: 0x06001CBC RID: 7356 RVA: 0x0008BB28 File Offset: 0x00089D28
	private void OnCollisionStay(Collision collision)
	{
		if (!this.MakeJump())
		{
			Vector3 vector = Vector3.ProjectOnPlane(this.positionHistory[(this.historyIndex != 0) ? (this.historyIndex - 1) : (this.historySteps - 1)] - this.handToStickTo.transform.position, collision.GetContact(0).normal);
			Vector3 vector2 = this.thisRigidbody.transform.position - this.handToStickTo.transform.position;
			this.playspaceRigidbody.MovePosition(this.playspaceRigidbody.transform.position + vector - vector2);
		}
	}

	// Token: 0x04001FF5 RID: 8181
	public GameObject handToStickTo;

	// Token: 0x04001FF6 RID: 8182
	public float ratioToUse;

	// Token: 0x04001FF7 RID: 8183
	public float forceMultiplier;

	// Token: 0x04001FF8 RID: 8184
	public int historySteps;

	// Token: 0x04001FF9 RID: 8185
	public Rigidbody playspaceRigidbody;

	// Token: 0x04001FFA RID: 8186
	private Rigidbody thisRigidbody;

	// Token: 0x04001FFB RID: 8187
	private Vector3 lastPosition;

	// Token: 0x04001FFC RID: 8188
	private Vector3 maybeLastPositionIDK;

	// Token: 0x04001FFD RID: 8189
	private Vector3[] positionHistory;

	// Token: 0x04001FFE RID: 8190
	private int historyIndex;
}
