using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200060B RID: 1547
[DisallowMultipleComponent]
public class GorillaHandSocket : MonoBehaviour
{
	// Token: 0x170003A6 RID: 934
	// (get) Token: 0x0600266F RID: 9839 RVA: 0x000BE324 File Offset: 0x000BC524
	public GorillaHandNode attachedHand
	{
		get
		{
			return this._attachedHand;
		}
	}

	// Token: 0x170003A7 RID: 935
	// (get) Token: 0x06002670 RID: 9840 RVA: 0x000BE32C File Offset: 0x000BC52C
	public bool inUse
	{
		get
		{
			return this._inUse;
		}
	}

	// Token: 0x06002671 RID: 9841 RVA: 0x000BE334 File Offset: 0x000BC534
	public static bool FetchSocket(Collider collider, out GorillaHandSocket socket)
	{
		return GorillaHandSocket.gColliderToSocket.TryGetValue(collider, out socket);
	}

	// Token: 0x06002672 RID: 9842 RVA: 0x000BE342 File Offset: 0x000BC542
	public bool CanAttach()
	{
		return !this._inUse && this._sinceSocketStateChange.HasElapsed(this.attachCooldown, true);
	}

	// Token: 0x06002673 RID: 9843 RVA: 0x000BE360 File Offset: 0x000BC560
	public void Attach(GorillaHandNode hand)
	{
		if (!this.CanAttach())
		{
			return;
		}
		if (hand == null)
		{
			return;
		}
		hand.attachedToSocket = this;
		this._attachedHand = hand;
		this._inUse = true;
		this.OnHandAttach();
	}

	// Token: 0x06002674 RID: 9844 RVA: 0x000BE390 File Offset: 0x000BC590
	public void Detach()
	{
		GorillaHandNode gorillaHandNode;
		this.Detach(out gorillaHandNode);
	}

	// Token: 0x06002675 RID: 9845 RVA: 0x000BE3A8 File Offset: 0x000BC5A8
	public void Detach(out GorillaHandNode hand)
	{
		if (this._inUse)
		{
			this._inUse = false;
		}
		if (this._attachedHand == null)
		{
			hand = null;
			return;
		}
		hand = this._attachedHand;
		hand.attachedToSocket = null;
		this._attachedHand = null;
		this.OnHandDetach();
		this._sinceSocketStateChange = TimeSince.Now();
	}

	// Token: 0x06002676 RID: 9846 RVA: 0x000023F4 File Offset: 0x000005F4
	protected virtual void OnHandAttach()
	{
	}

	// Token: 0x06002677 RID: 9847 RVA: 0x000023F4 File Offset: 0x000005F4
	protected virtual void OnHandDetach()
	{
	}

	// Token: 0x06002678 RID: 9848 RVA: 0x000BE3FE File Offset: 0x000BC5FE
	protected virtual void OnUpdateAttached()
	{
		this._attachedHand.transform.position = base.transform.position;
	}

	// Token: 0x06002679 RID: 9849 RVA: 0x000BE41B File Offset: 0x000BC61B
	private void OnEnable()
	{
		if (this.collider == null)
		{
			return;
		}
		GorillaHandSocket.gColliderToSocket.TryAdd(this.collider, this);
	}

	// Token: 0x0600267A RID: 9850 RVA: 0x000BE43E File Offset: 0x000BC63E
	private void OnDisable()
	{
		if (this.collider == null)
		{
			return;
		}
		GorillaHandSocket.gColliderToSocket.Remove(this.collider);
	}

	// Token: 0x0600267B RID: 9851 RVA: 0x000BE460 File Offset: 0x000BC660
	private void Awake()
	{
		this.Setup();
	}

	// Token: 0x0600267C RID: 9852 RVA: 0x000BE468 File Offset: 0x000BC668
	private void FixedUpdate()
	{
		if (!this._inUse)
		{
			return;
		}
		if (!this._attachedHand)
		{
			return;
		}
		this.OnUpdateAttached();
	}

	// Token: 0x0600267D RID: 9853 RVA: 0x000BE488 File Offset: 0x000BC688
	private void Setup()
	{
		if (this.collider == null)
		{
			this.collider = base.GetComponent<Collider>();
		}
		int num = 0;
		num |= 1024;
		num |= 2097152;
		num |= 16777216;
		base.gameObject.SetTag(UnityTag.GorillaHandSocket);
		base.gameObject.SetLayer(UnityLayer.GorillaHandSocket);
		this.collider.isTrigger = true;
		this.collider.includeLayers = num;
		this.collider.excludeLayers = ~num;
		this._sinceSocketStateChange = TimeSince.Now();
	}

	// Token: 0x04002AD7 RID: 10967
	public Collider collider;

	// Token: 0x04002AD8 RID: 10968
	public float attachCooldown = 0.5f;

	// Token: 0x04002AD9 RID: 10969
	public HandSocketConstraint constraint;

	// Token: 0x04002ADA RID: 10970
	[NonSerialized]
	private GorillaHandNode _attachedHand;

	// Token: 0x04002ADB RID: 10971
	[NonSerialized]
	private bool _inUse;

	// Token: 0x04002ADC RID: 10972
	[NonSerialized]
	private TimeSince _sinceSocketStateChange;

	// Token: 0x04002ADD RID: 10973
	private static readonly Dictionary<Collider, GorillaHandSocket> gColliderToSocket = new Dictionary<Collider, GorillaHandSocket>(64);
}
