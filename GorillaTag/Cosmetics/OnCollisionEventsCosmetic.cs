using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000DEA RID: 3562
	public class OnCollisionEventsCosmetic : MonoBehaviour
	{
		// Token: 0x06005842 RID: 22594 RVA: 0x001B280F File Offset: 0x001B0A0F
		private bool IsMyItem()
		{
			return this._rig != null && this._rig.isOfflineVRRig;
		}

		// Token: 0x06005843 RID: 22595 RVA: 0x001B282C File Offset: 0x001B0A2C
		private void Awake()
		{
			this._rig = base.GetComponentInParent<VRRig>();
			this.parentTransferable = base.GetComponentInParent<TransferrableObject>();
		}

		// Token: 0x06005844 RID: 22596 RVA: 0x001B2848 File Offset: 0x001B0A48
		private void FireEvents(Collision other, OnCollisionEventsCosmetic.Listener listener)
		{
			if (!listener.syncForEveryoneInRoom && !this.IsMyItem())
			{
				return;
			}
			if (this.parentTransferable && listener.fireOnlyWhileHeld && !this.parentTransferable.InHand())
			{
				return;
			}
			if (listener.collisionTagsList.Count > 0 && !this.IsTagValid(other.gameObject, listener))
			{
				return;
			}
			if (!this.IsInCollisionLayer(other.gameObject, listener))
			{
				return;
			}
			bool flag = this.parentTransferable && this.parentTransferable.InLeftHand();
			UnityEvent<bool, Collision> listenerComponent = listener.listenerComponent;
			if (listenerComponent == null)
			{
				return;
			}
			listenerComponent.Invoke(flag, other);
		}

		// Token: 0x06005845 RID: 22597 RVA: 0x001B28E5 File Offset: 0x001B0AE5
		private bool IsTagValid(GameObject obj, OnCollisionEventsCosmetic.Listener listener)
		{
			return listener.collisionTagsList.Contains(obj.tag);
		}

		// Token: 0x06005846 RID: 22598 RVA: 0x001B28F8 File Offset: 0x001B0AF8
		private bool IsInCollisionLayer(GameObject obj, OnCollisionEventsCosmetic.Listener listener)
		{
			return (listener.collisionLayerMask.value & (1 << obj.layer)) != 0;
		}

		// Token: 0x06005847 RID: 22599 RVA: 0x001B2914 File Offset: 0x001B0B14
		private void OnCollisionEnter(Collision other)
		{
			for (int i = 0; i < this.eventListeners.Length; i++)
			{
				OnCollisionEventsCosmetic.Listener listener = this.eventListeners[i];
				if (listener.eventType == OnCollisionEventsCosmetic.EventType.CollisionEnter)
				{
					this.FireEvents(other, listener);
				}
			}
		}

		// Token: 0x06005848 RID: 22600 RVA: 0x001B2950 File Offset: 0x001B0B50
		private void OnCollisionExit(Collision other)
		{
			for (int i = 0; i < this.eventListeners.Length; i++)
			{
				OnCollisionEventsCosmetic.Listener listener = this.eventListeners[i];
				if (listener.eventType == OnCollisionEventsCosmetic.EventType.CollisionExit)
				{
					this.FireEvents(other, listener);
				}
			}
		}

		// Token: 0x06005849 RID: 22601 RVA: 0x001B298C File Offset: 0x001B0B8C
		private void OnCollisionStay(Collision other)
		{
			for (int i = 0; i < this.eventListeners.Length; i++)
			{
				OnCollisionEventsCosmetic.Listener listener = this.eventListeners[i];
				if (listener.eventType == OnCollisionEventsCosmetic.EventType.CollisionStay)
				{
					this.FireEvents(other, listener);
				}
			}
		}

		// Token: 0x04005D71 RID: 23921
		public OnCollisionEventsCosmetic.Listener[] eventListeners = new OnCollisionEventsCosmetic.Listener[0];

		// Token: 0x04005D72 RID: 23922
		private VRRig _rig;

		// Token: 0x04005D73 RID: 23923
		private TransferrableObject parentTransferable;

		// Token: 0x02000DEB RID: 3563
		[Serializable]
		public class Listener
		{
			// Token: 0x04005D74 RID: 23924
			public LayerMask collisionLayerMask;

			// Token: 0x04005D75 RID: 23925
			public List<string> collisionTagsList = new List<string>();

			// Token: 0x04005D76 RID: 23926
			public OnCollisionEventsCosmetic.EventType eventType;

			// Token: 0x04005D77 RID: 23927
			public UnityEvent<bool, Collision> listenerComponent;

			// Token: 0x04005D78 RID: 23928
			public bool syncForEveryoneInRoom = true;

			// Token: 0x04005D79 RID: 23929
			[Tooltip("Fire these events only when the item is held in hand, only works if there is a transferable component somewhere on the object or its parent.")]
			public bool fireOnlyWhileHeld = true;
		}

		// Token: 0x02000DEC RID: 3564
		public enum EventType
		{
			// Token: 0x04005D7B RID: 23931
			CollisionEnter,
			// Token: 0x04005D7C RID: 23932
			CollisionStay,
			// Token: 0x04005D7D RID: 23933
			CollisionExit
		}
	}
}
