using System;
using UnityEngine;

// Token: 0x02000498 RID: 1176
public class GorillaPlaySpaceForces : MonoBehaviour
{
	// Token: 0x06001CA3 RID: 7331 RVA: 0x0008B858 File Offset: 0x00089A58
	private void Start()
	{
		this.playspaceRigidbody = base.GetComponent<Rigidbody>();
		this.leftHandRigidbody = this.leftHand.GetComponent<Rigidbody>();
		this.leftHandCollider = this.leftHand.GetComponent<Collider>();
		this.rightHandRigidbody = this.rightHand.GetComponent<Rigidbody>();
		this.rightHandCollider = this.rightHand.GetComponent<Collider>();
	}

	// Token: 0x06001CA4 RID: 7332 RVA: 0x0008B8B5 File Offset: 0x00089AB5
	private void FixedUpdate()
	{
		if (Time.time >= 0.1f)
		{
			this.bodyCollider.transform.position = this.headsetTransform.position + this.bodyColliderOffset;
		}
	}

	// Token: 0x04001FD5 RID: 8149
	public GameObject rightHand;

	// Token: 0x04001FD6 RID: 8150
	public GameObject leftHand;

	// Token: 0x04001FD7 RID: 8151
	public Collider bodyCollider;

	// Token: 0x04001FD8 RID: 8152
	private Collider leftHandCollider;

	// Token: 0x04001FD9 RID: 8153
	private Collider rightHandCollider;

	// Token: 0x04001FDA RID: 8154
	public Transform rightHandTransform;

	// Token: 0x04001FDB RID: 8155
	public Transform leftHandTransform;

	// Token: 0x04001FDC RID: 8156
	private Rigidbody leftHandRigidbody;

	// Token: 0x04001FDD RID: 8157
	private Rigidbody rightHandRigidbody;

	// Token: 0x04001FDE RID: 8158
	public Vector3 bodyColliderOffset;

	// Token: 0x04001FDF RID: 8159
	public float forceConstant;

	// Token: 0x04001FE0 RID: 8160
	private Vector3 lastLeftHandPosition;

	// Token: 0x04001FE1 RID: 8161
	private Vector3 lastRightHandPosition;

	// Token: 0x04001FE2 RID: 8162
	private Rigidbody playspaceRigidbody;

	// Token: 0x04001FE3 RID: 8163
	public Transform headsetTransform;
}
