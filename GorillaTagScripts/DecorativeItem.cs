using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts
{
	// Token: 0x02000B00 RID: 2816
	public class DecorativeItem : TransferrableObject
	{
		// Token: 0x060044E2 RID: 17634 RVA: 0x00137344 File Offset: 0x00135544
		public override bool ShouldBeKinematic()
		{
			return this.itemState == TransferrableObject.ItemStates.State2 || this.itemState == TransferrableObject.ItemStates.State4 || base.ShouldBeKinematic();
		}

		// Token: 0x060044E3 RID: 17635 RVA: 0x0014648E File Offset: 0x0014468E
		public override void OnSpawn(VRRig rig)
		{
			base.OnSpawn(rig);
			this.parent = base.transform.parent;
		}

		// Token: 0x060044E4 RID: 17636 RVA: 0x001373C7 File Offset: 0x001355C7
		protected override void Start()
		{
			base.Start();
			this.itemState = TransferrableObject.ItemStates.State4;
			this.currentState = TransferrableObject.PositionState.Dropped;
		}

		// Token: 0x060044E5 RID: 17637 RVA: 0x001464A8 File Offset: 0x001446A8
		private new void OnStateChanged()
		{
			TransferrableObject.ItemStates itemState = this.itemState;
			if (itemState == TransferrableObject.ItemStates.State2)
			{
				this.SnapItem(this.reliableState.isSnapped, this.reliableState.snapPosition);
				return;
			}
			if (itemState != TransferrableObject.ItemStates.State3)
			{
				return;
			}
			this.Respawn(this.reliableState.respawnPosition, this.reliableState.respawnRotation);
		}

		// Token: 0x060044E6 RID: 17638 RVA: 0x00146500 File Offset: 0x00144700
		protected override void LateUpdateShared()
		{
			base.LateUpdateShared();
			if (base.InHand())
			{
				this.itemState = TransferrableObject.ItemStates.State0;
			}
			DecorativeItem.DecorativeItemState itemState = (DecorativeItem.DecorativeItemState)this.itemState;
			if (itemState != this.previousItemState)
			{
				this.OnStateChanged();
			}
			this.previousItemState = itemState;
		}

		// Token: 0x060044E7 RID: 17639 RVA: 0x0014653F File Offset: 0x0014473F
		protected override void LateUpdateLocal()
		{
			base.LateUpdateLocal();
			if (this.itemState == TransferrableObject.ItemStates.State4 && this.worldShareableInstance && this.worldShareableInstance.guard.isTrulyMine)
			{
				this.InvokeRespawn();
			}
		}

		// Token: 0x060044E8 RID: 17640 RVA: 0x00146576 File Offset: 0x00144776
		public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
		{
			base.OnGrab(pointGrabbed, grabbingHand);
			this.itemState = TransferrableObject.ItemStates.State0;
		}

		// Token: 0x060044E9 RID: 17641 RVA: 0x00146587 File Offset: 0x00144787
		public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
		{
			if (!base.OnRelease(zoneReleased, releasingHand))
			{
				return false;
			}
			this.itemState = TransferrableObject.ItemStates.State1;
			this.Reparent(null);
			return true;
		}

		// Token: 0x060044EA RID: 17642 RVA: 0x00124434 File Offset: 0x00122634
		private void SetWillTeleport()
		{
			this.worldShareableInstance.SetWillTeleport();
		}

		// Token: 0x060044EB RID: 17643 RVA: 0x001465A8 File Offset: 0x001447A8
		public void Respawn(Vector3 randPosition, Quaternion randRotation)
		{
			if (base.InHand())
			{
				return;
			}
			if (this.shatterVFX && this.ShouldPlayFX())
			{
				this.PlayVFX(this.shatterVFX);
			}
			this.itemState = TransferrableObject.ItemStates.State3;
			this.SetWillTeleport();
			Transform transform = base.transform;
			transform.position = randPosition;
			transform.rotation = randRotation;
			if (this.reliableState)
			{
				this.reliableState.respawnPosition = randPosition;
				this.reliableState.respawnRotation = randRotation;
			}
		}

		// Token: 0x060044EC RID: 17644 RVA: 0x00096D7A File Offset: 0x00094F7A
		private void PlayVFX(GameObject vfx)
		{
			ObjectPools.instance.Instantiate(vfx, base.transform.position, true);
		}

		// Token: 0x060044ED RID: 17645 RVA: 0x00146624 File Offset: 0x00144824
		private bool Reparent(Transform _transform)
		{
			if (!this.allowReparenting)
			{
				return false;
			}
			if (this.parent)
			{
				this.parent.SetParent(_transform);
				base.transform.SetParent(this.parent);
				return true;
			}
			return false;
		}

		// Token: 0x060044EE RID: 17646 RVA: 0x00146660 File Offset: 0x00144860
		public void SnapItem(bool snap, Vector3 attachPoint)
		{
			if (!this.reliableState)
			{
				return;
			}
			if (snap)
			{
				AttachPoint currentAttachPointByPosition = DecorativeItemsManager.Instance.getCurrentAttachPointByPosition(attachPoint);
				if (!currentAttachPointByPosition)
				{
					this.reliableState.isSnapped = false;
					this.reliableState.snapPosition = Vector3.zero;
					return;
				}
				Transform attachPoint2 = currentAttachPointByPosition.attachPoint;
				if (!this.Reparent(attachPoint2))
				{
					this.reliableState.isSnapped = false;
					this.reliableState.snapPosition = Vector3.zero;
					return;
				}
				this.itemState = TransferrableObject.ItemStates.State2;
				base.transform.parent.localPosition = Vector3.zero;
				base.transform.localPosition = Vector3.zero;
				this.reliableState.isSnapped = true;
				if (this.audioSource && this.snapAudio && this.ShouldPlayFX())
				{
					this.audioSource.GTPlayOneShot(this.snapAudio, 1f);
				}
				currentAttachPointByPosition.SetIsHook(true);
			}
			else
			{
				this.Reparent(null);
				this.reliableState.isSnapped = false;
			}
			this.reliableState.snapPosition = attachPoint;
		}

		// Token: 0x060044EF RID: 17647 RVA: 0x0014677B File Offset: 0x0014497B
		private void InvokeRespawn()
		{
			if (this.itemState == TransferrableObject.ItemStates.State2)
			{
				return;
			}
			UnityAction<DecorativeItem> unityAction = this.respawnItem;
			if (unityAction == null)
			{
				return;
			}
			unityAction(this);
		}

		// Token: 0x060044F0 RID: 17648 RVA: 0x00146798 File Offset: 0x00144998
		private bool ShouldPlayFX()
		{
			return this.previousItemState == DecorativeItem.DecorativeItemState.isHeld || this.previousItemState == DecorativeItem.DecorativeItemState.dropped;
		}

		// Token: 0x060044F1 RID: 17649 RVA: 0x001467AF File Offset: 0x001449AF
		private void OnCollisionEnter(Collision other)
		{
			if (this.breakItemLayerMask != (this.breakItemLayerMask | (1 << other.gameObject.layer)))
			{
				return;
			}
			this.InvokeRespawn();
		}

		// Token: 0x0400479D RID: 18333
		public DecorativeItemReliableState reliableState;

		// Token: 0x0400479E RID: 18334
		public UnityAction<DecorativeItem> respawnItem;

		// Token: 0x0400479F RID: 18335
		public LayerMask breakItemLayerMask;

		// Token: 0x040047A0 RID: 18336
		private Coroutine respawnTimer;

		// Token: 0x040047A1 RID: 18337
		private Transform parent;

		// Token: 0x040047A2 RID: 18338
		private float _respawnTimestamp;

		// Token: 0x040047A3 RID: 18339
		private bool isSnapped;

		// Token: 0x040047A4 RID: 18340
		private Vector3 currentPosition;

		// Token: 0x040047A5 RID: 18341
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x040047A6 RID: 18342
		public AudioClip snapAudio;

		// Token: 0x040047A7 RID: 18343
		public GameObject shatterVFX;

		// Token: 0x040047A8 RID: 18344
		private DecorativeItem.DecorativeItemState previousItemState = DecorativeItem.DecorativeItemState.dropped;

		// Token: 0x02000B01 RID: 2817
		private enum DecorativeItemState
		{
			// Token: 0x040047AA RID: 18346
			isHeld = 1,
			// Token: 0x040047AB RID: 18347
			dropped,
			// Token: 0x040047AC RID: 18348
			snapped = 4,
			// Token: 0x040047AD RID: 18349
			respawn = 8,
			// Token: 0x040047AE RID: 18350
			none = 16
		}
	}
}
