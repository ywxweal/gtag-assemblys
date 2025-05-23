using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000919 RID: 2329
public class RigOwnedPhysicsBody : MonoBehaviour
{
	// Token: 0x060038D5 RID: 14549 RVA: 0x00112078 File Offset: 0x00110278
	private void Awake()
	{
		this.hasTransformView = this.transformView != null;
		this.hasRigidbodyView = this.rigidbodyView != null;
		if (!this.hasTransformView && !this.hasRigidbodyView && this.otherComponents.Length == 0)
		{
			GTDev.LogError<string>("RigOwnedPhysicsBody has nothing to do! No TransformView, RigidbodyView, or otherComponents", null);
		}
		if (this.detachTransform)
		{
			if (this.hasTransformView)
			{
				this.transformView.transform.parent = null;
				return;
			}
			if (this.hasRigidbodyView)
			{
				this.rigidbodyView.transform.parent = null;
			}
		}
	}

	// Token: 0x060038D6 RID: 14550 RVA: 0x00112108 File Offset: 0x00110308
	private void OnEnable()
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		NetworkSystem.Instance.OnJoinedRoomEvent += this.OnNetConnect;
		NetworkSystem.Instance.OnReturnedToSinglePlayer += this.OnNetDisconnect;
		if (!this.hasRig)
		{
			this.rig = base.GetComponentInParent<VRRig>();
			this.hasRig = this.rig != null;
		}
		if (this.detachTransform)
		{
			if (this.hasTransformView)
			{
				this.transformView.gameObject.SetActive(true);
			}
			else if (this.hasRigidbodyView)
			{
				this.rigidbodyView.gameObject.SetActive(true);
			}
		}
		if (NetworkSystem.Instance.InRoom)
		{
			this.OnNetConnect();
			return;
		}
		this.OnNetDisconnect();
	}

	// Token: 0x060038D7 RID: 14551 RVA: 0x001121CC File Offset: 0x001103CC
	private void OnDisable()
	{
		NetworkSystem.Instance.OnJoinedRoomEvent -= this.OnNetConnect;
		NetworkSystem.Instance.OnReturnedToSinglePlayer -= this.OnNetDisconnect;
		if (this.detachTransform)
		{
			if (this.hasTransformView)
			{
				this.transformView.gameObject.SetActive(false);
			}
			else if (this.hasRigidbodyView)
			{
				this.rigidbodyView.gameObject.SetActive(false);
			}
		}
		this.OnNetDisconnect();
	}

	// Token: 0x060038D8 RID: 14552 RVA: 0x00112248 File Offset: 0x00110448
	private void OnNetConnect()
	{
		if (this.hasTransformView)
		{
			this.transformView.enabled = this.hasRig;
		}
		if (this.hasRigidbodyView)
		{
			this.rigidbodyView.enabled = this.hasRig;
		}
		MonoBehaviourPun[] array = this.otherComponents;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = this.hasRig;
		}
		if (!this.hasRig)
		{
			return;
		}
		PhotonView getView = this.rig.netView.GetView;
		List<Component> observedComponents = getView.ObservedComponents;
		if (this.hasTransformView)
		{
			this.transformView.SetIsMine(getView.IsMine);
			if (!observedComponents.Contains(this.transformView))
			{
				observedComponents.Add(this.transformView);
			}
		}
		if (this.hasRigidbodyView)
		{
			this.rigidbodyView.SetIsMine(getView.IsMine);
			if (!observedComponents.Contains(this.rigidbodyView))
			{
				observedComponents.Add(this.rigidbodyView);
			}
		}
		foreach (MonoBehaviourPun monoBehaviourPun in this.otherComponents)
		{
			if (!observedComponents.Contains(monoBehaviourPun))
			{
				observedComponents.Add(monoBehaviourPun);
			}
		}
	}

	// Token: 0x060038D9 RID: 14553 RVA: 0x00112360 File Offset: 0x00110560
	private void OnNetDisconnect()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		if (this.hasTransformView)
		{
			this.transformView.enabled = false;
		}
		if (this.hasRigidbodyView)
		{
			this.rigidbodyView.enabled = false;
		}
		MonoBehaviourPun[] array = this.otherComponents;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = false;
		}
		if (!this.hasRig || !NetworkSystem.Instance.InRoom)
		{
			return;
		}
		List<Component> observedComponents = this.rig.netView.GetView.ObservedComponents;
		if (this.hasTransformView)
		{
			observedComponents.Remove(this.transformView);
		}
		if (this.hasRigidbodyView)
		{
			observedComponents.Remove(this.rigidbodyView);
		}
		foreach (MonoBehaviourPun monoBehaviourPun in this.otherComponents)
		{
			observedComponents.Remove(monoBehaviourPun);
		}
	}

	// Token: 0x04003DFF RID: 15871
	private VRRig rig;

	// Token: 0x04003E00 RID: 15872
	public RigOwnedTransformView transformView;

	// Token: 0x04003E01 RID: 15873
	private bool hasTransformView;

	// Token: 0x04003E02 RID: 15874
	public RigOwnedRigidbodyView rigidbodyView;

	// Token: 0x04003E03 RID: 15875
	private bool hasRigidbodyView;

	// Token: 0x04003E04 RID: 15876
	public MonoBehaviourPun[] otherComponents;

	// Token: 0x04003E05 RID: 15877
	private bool hasRig;

	// Token: 0x04003E06 RID: 15878
	[Tooltip("To make a rigidbody unaffected by the movement of the holdable part, put this script on the holdable, make the RigOwnedRigidbodyView a child of it, and check this box")]
	[SerializeField]
	private bool detachTransform;
}
