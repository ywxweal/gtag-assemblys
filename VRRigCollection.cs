using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020009E1 RID: 2529
[RequireComponent(typeof(CompositeTriggerEvents))]
public class VRRigCollection : MonoBehaviour
{
	// Token: 0x170005E4 RID: 1508
	// (get) Token: 0x06003C81 RID: 15489 RVA: 0x00120BCE File Offset: 0x0011EDCE
	public List<RigContainer> Rigs
	{
		get
		{
			return this.containedRigs;
		}
	}

	// Token: 0x06003C82 RID: 15490 RVA: 0x00120BD6 File Offset: 0x0011EDD6
	private void OnEnable()
	{
		this.collisionTriggerEvents.CompositeTriggerEnter += this.OnRigTriggerEnter;
		this.collisionTriggerEvents.CompositeTriggerExit += this.OnRigTriggerExit;
	}

	// Token: 0x06003C83 RID: 15491 RVA: 0x00120C08 File Offset: 0x0011EE08
	private void OnDisable()
	{
		for (int i = this.containedRigs.Count - 1; i >= 0; i--)
		{
			this.RigDisabled(this.containedRigs[i]);
		}
		this.collisionTriggerEvents.CompositeTriggerEnter -= this.OnRigTriggerEnter;
		this.collisionTriggerEvents.CompositeTriggerExit -= this.OnRigTriggerExit;
	}

	// Token: 0x06003C84 RID: 15492 RVA: 0x00120C70 File Offset: 0x0011EE70
	private void OnRigTriggerEnter(Collider other)
	{
		Rigidbody attachedRigidbody = other.attachedRigidbody;
		RigContainer rigContainer;
		if (attachedRigidbody == null || !attachedRigidbody.TryGetComponent<RigContainer>(out rigContainer) || other != rigContainer.HeadCollider || this.containedRigs.Contains(rigContainer))
		{
			return;
		}
		VRRigEvents rigEvents = rigContainer.RigEvents;
		rigEvents.disableEvent = (Action<RigContainer>)Delegate.Combine(rigEvents.disableEvent, new Action<RigContainer>(this.RigDisabled));
		this.containedRigs.Add(rigContainer);
		Action<RigContainer> action = this.playerEnteredCollection;
		if (action == null)
		{
			return;
		}
		action(rigContainer);
	}

	// Token: 0x06003C85 RID: 15493 RVA: 0x00120CF8 File Offset: 0x0011EEF8
	private void OnRigTriggerExit(Collider other)
	{
		Rigidbody attachedRigidbody = other.attachedRigidbody;
		RigContainer rigContainer;
		if (attachedRigidbody == null || !attachedRigidbody.TryGetComponent<RigContainer>(out rigContainer) || other != rigContainer.HeadCollider || !this.containedRigs.Contains(rigContainer))
		{
			return;
		}
		VRRigEvents rigEvents = rigContainer.RigEvents;
		rigEvents.disableEvent = (Action<RigContainer>)Delegate.Remove(rigEvents.disableEvent, new Action<RigContainer>(this.RigDisabled));
		this.containedRigs.Remove(rigContainer);
		Action<RigContainer> action = this.playerLeftCollection;
		if (action == null)
		{
			return;
		}
		action(rigContainer);
	}

	// Token: 0x06003C86 RID: 15494 RVA: 0x00120D81 File Offset: 0x0011EF81
	private void RigDisabled(RigContainer rig)
	{
		this.collisionTriggerEvents.ResetColliderMask(rig.HeadCollider);
		this.collisionTriggerEvents.ResetColliderMask(rig.BodyCollider);
	}

	// Token: 0x06003C87 RID: 15495 RVA: 0x00120DA8 File Offset: 0x0011EFA8
	private bool HasRig(VRRig rig)
	{
		for (int i = 0; i < this.containedRigs.Count; i++)
		{
			if (this.containedRigs[i].Rig == rig)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06003C88 RID: 15496 RVA: 0x00120DE8 File Offset: 0x0011EFE8
	private bool HasRig(NetPlayer player)
	{
		for (int i = 0; i < this.containedRigs.Count; i++)
		{
			if (this.containedRigs[i].Creator == player)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0400406E RID: 16494
	public readonly List<RigContainer> containedRigs = new List<RigContainer>(10);

	// Token: 0x0400406F RID: 16495
	[SerializeField]
	private CompositeTriggerEvents collisionTriggerEvents;

	// Token: 0x04004070 RID: 16496
	public Action<RigContainer> playerEnteredCollection;

	// Token: 0x04004071 RID: 16497
	public Action<RigContainer> playerLeftCollection;
}
