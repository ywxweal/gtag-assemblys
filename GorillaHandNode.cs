using System;
using UnityEngine;

// Token: 0x02000609 RID: 1545
public class GorillaHandNode : MonoBehaviour
{
	// Token: 0x170003A3 RID: 931
	// (get) Token: 0x06002663 RID: 9827 RVA: 0x000BE0CC File Offset: 0x000BC2CC
	public bool isGripping
	{
		get
		{
			return this.PollGrip();
		}
	}

	// Token: 0x170003A4 RID: 932
	// (get) Token: 0x06002664 RID: 9828 RVA: 0x000BE0D4 File Offset: 0x000BC2D4
	public bool isLeftHand
	{
		get
		{
			return this._isLeftHand;
		}
	}

	// Token: 0x170003A5 RID: 933
	// (get) Token: 0x06002665 RID: 9829 RVA: 0x000BE0DC File Offset: 0x000BC2DC
	public bool isRightHand
	{
		get
		{
			return this._isRightHand;
		}
	}

	// Token: 0x06002666 RID: 9830 RVA: 0x000BE0E4 File Offset: 0x000BC2E4
	private void Awake()
	{
		this.Setup();
	}

	// Token: 0x06002667 RID: 9831 RVA: 0x000BE0EC File Offset: 0x000BC2EC
	private bool PollGrip()
	{
		if (this.rig == null)
		{
			return false;
		}
		bool flag = this.PollThumb() >= 0.25f;
		bool flag2 = this.PollIndex() >= 0.25f;
		bool flag3 = this.PollMiddle() >= 0.25f;
		return flag && flag2 && flag3;
	}

	// Token: 0x06002668 RID: 9832 RVA: 0x000BE140 File Offset: 0x000BC340
	private void Setup()
	{
		if (this.rig == null)
		{
			this.rig = base.GetComponentInParent<VRRig>();
		}
		if (this.rigidbody == null)
		{
			this.rigidbody = base.GetComponent<Rigidbody>();
		}
		if (this.collider == null)
		{
			this.collider = base.GetComponent<Collider>();
		}
		if (this.rig)
		{
			this.vrIndex = (this._isLeftHand ? this.rig.leftIndex : this.rig.rightIndex);
			this.vrThumb = (this._isLeftHand ? this.rig.leftThumb : this.rig.rightThumb);
			this.vrMiddle = (this._isLeftHand ? this.rig.leftMiddle : this.rig.rightMiddle);
		}
		this._isLeftHand = base.name.Contains("left", StringComparison.OrdinalIgnoreCase);
		this._isRightHand = base.name.Contains("right", StringComparison.OrdinalIgnoreCase);
		int num = 0;
		num |= 1024;
		num |= 2097152;
		num |= 16777216;
		base.gameObject.SetTag(this._isLeftHand ? UnityTag.GorillaHandLeft : UnityTag.GorillaHandRight);
		base.gameObject.SetLayer(UnityLayer.GorillaHand);
		this.rigidbody.includeLayers = num;
		this.rigidbody.excludeLayers = ~num;
		this.rigidbody.isKinematic = true;
		this.rigidbody.useGravity = false;
		this.rigidbody.constraints = RigidbodyConstraints.FreezeAll;
		this.collider.isTrigger = true;
		this.collider.includeLayers = num;
		this.collider.excludeLayers = ~num;
	}

	// Token: 0x06002669 RID: 9833 RVA: 0x000023F4 File Offset: 0x000005F4
	private void OnTriggerStay(Collider other)
	{
	}

	// Token: 0x0600266A RID: 9834 RVA: 0x000BE2FF File Offset: 0x000BC4FF
	private float PollIndex()
	{
		return Mathf.Clamp01(this.vrIndex.calcT / 0.88f);
	}

	// Token: 0x0600266B RID: 9835 RVA: 0x000BE317 File Offset: 0x000BC517
	private float PollMiddle()
	{
		return this.vrIndex.calcT;
	}

	// Token: 0x0600266C RID: 9836 RVA: 0x000BE317 File Offset: 0x000BC517
	private float PollThumb()
	{
		return this.vrIndex.calcT;
	}

	// Token: 0x04002ACD RID: 10957
	public VRRig rig;

	// Token: 0x04002ACE RID: 10958
	public Collider collider;

	// Token: 0x04002ACF RID: 10959
	public Rigidbody rigidbody;

	// Token: 0x04002AD0 RID: 10960
	[Space]
	[NonSerialized]
	public VRMapIndex vrIndex;

	// Token: 0x04002AD1 RID: 10961
	[NonSerialized]
	public VRMapThumb vrThumb;

	// Token: 0x04002AD2 RID: 10962
	[NonSerialized]
	public VRMapMiddle vrMiddle;

	// Token: 0x04002AD3 RID: 10963
	[Space]
	public GorillaHandSocket attachedToSocket;

	// Token: 0x04002AD4 RID: 10964
	[Space]
	[SerializeField]
	private bool _isLeftHand;

	// Token: 0x04002AD5 RID: 10965
	[SerializeField]
	private bool _isRightHand;

	// Token: 0x04002AD6 RID: 10966
	public bool ignoreSockets;
}
