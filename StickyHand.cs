using System;
using GorillaExtensions;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x0200019D RID: 413
public class StickyHand : MonoBehaviour, ISpawnable
{
	// Token: 0x17000100 RID: 256
	// (get) Token: 0x06000A30 RID: 2608 RVA: 0x00035867 File Offset: 0x00033A67
	// (set) Token: 0x06000A31 RID: 2609 RVA: 0x0003586F File Offset: 0x00033A6F
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x17000101 RID: 257
	// (get) Token: 0x06000A32 RID: 2610 RVA: 0x00035878 File Offset: 0x00033A78
	// (set) Token: 0x06000A33 RID: 2611 RVA: 0x00035880 File Offset: 0x00033A80
	public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

	// Token: 0x06000A34 RID: 2612 RVA: 0x0003588C File Offset: 0x00033A8C
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this.myRig = rig;
		this.isLocal = rig.isLocal;
		this.flatHand.enabled = false;
		this.defaultLocalPosition = this.stringParent.transform.InverseTransformPoint(this.rb.transform.position);
		int num = ((this.CosmeticSelectedSide == ECosmeticSelectSide.Left) ? 1 : 2);
		this.stateBitIndex = VRRig.WearablePackedStatesBitWriteInfos[num].index;
	}

	// Token: 0x06000A35 RID: 2613 RVA: 0x000023F4 File Offset: 0x000005F4
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x06000A36 RID: 2614 RVA: 0x00035904 File Offset: 0x00033B04
	private void Update()
	{
		if (this.isLocal)
		{
			if (this.rb.isKinematic && (this.rb.transform.position - this.stringParent.transform.position).IsLongerThan(this.stringDetachLength))
			{
				this.Unstick();
			}
			else if (!this.rb.isKinematic && (this.rb.transform.position - this.stringParent.transform.position).IsLongerThan(this.stringTeleportLength))
			{
				this.rb.transform.position = this.stringParent.transform.TransformPoint(this.defaultLocalPosition);
			}
			this.myRig.WearablePackedStates = GTBitOps.WriteBit(this.myRig.WearablePackedStates, this.stateBitIndex, this.rb.isKinematic);
			return;
		}
		if (GTBitOps.ReadBit(this.myRig.WearablePackedStates, this.stateBitIndex) != this.rb.isKinematic)
		{
			if (this.rb.isKinematic)
			{
				this.Unstick();
				return;
			}
			this.Stick();
		}
	}

	// Token: 0x06000A37 RID: 2615 RVA: 0x00035A32 File Offset: 0x00033C32
	private void Stick()
	{
		this.thwackSound.Play();
		this.flatHand.enabled = true;
		this.regularHand.enabled = false;
		this.rb.isKinematic = true;
	}

	// Token: 0x06000A38 RID: 2616 RVA: 0x00035A63 File Offset: 0x00033C63
	private void Unstick()
	{
		this.schlupSound.Play();
		this.rb.isKinematic = false;
		this.flatHand.enabled = false;
		this.regularHand.enabled = true;
	}

	// Token: 0x06000A39 RID: 2617 RVA: 0x00035A94 File Offset: 0x00033C94
	private void OnCollisionStay(Collision collision)
	{
		if (!this.isLocal || this.rb.isKinematic)
		{
			return;
		}
		if ((this.rb.transform.position - this.stringParent.transform.position).IsLongerThan(this.stringMaxAttachLength))
		{
			return;
		}
		this.Stick();
		Vector3 point = collision.contacts[0].point;
		Vector3 normal = collision.contacts[0].normal;
		this.rb.transform.rotation = Quaternion.LookRotation(normal, this.rb.transform.up);
		Vector3 vector = this.rb.transform.position - point;
		vector -= Vector3.Dot(vector, normal) * normal;
		this.rb.transform.position = point + vector + this.surfaceOffsetDistance * normal;
	}

	// Token: 0x04000C52 RID: 3154
	[SerializeField]
	private MeshRenderer flatHand;

	// Token: 0x04000C53 RID: 3155
	[SerializeField]
	private MeshRenderer regularHand;

	// Token: 0x04000C54 RID: 3156
	[SerializeField]
	private Rigidbody rb;

	// Token: 0x04000C55 RID: 3157
	[SerializeField]
	private GameObject stringParent;

	// Token: 0x04000C56 RID: 3158
	[SerializeField]
	private float surfaceOffsetDistance;

	// Token: 0x04000C57 RID: 3159
	[SerializeField]
	private float stringMaxAttachLength;

	// Token: 0x04000C58 RID: 3160
	[SerializeField]
	private float stringDetachLength;

	// Token: 0x04000C59 RID: 3161
	[SerializeField]
	private float stringTeleportLength;

	// Token: 0x04000C5A RID: 3162
	[SerializeField]
	private SoundBankPlayer thwackSound;

	// Token: 0x04000C5B RID: 3163
	[SerializeField]
	private SoundBankPlayer schlupSound;

	// Token: 0x04000C5C RID: 3164
	private VRRig myRig;

	// Token: 0x04000C5D RID: 3165
	private bool isLocal;

	// Token: 0x04000C5E RID: 3166
	private int stateBitIndex;

	// Token: 0x04000C5F RID: 3167
	private Vector3 defaultLocalPosition;
}
