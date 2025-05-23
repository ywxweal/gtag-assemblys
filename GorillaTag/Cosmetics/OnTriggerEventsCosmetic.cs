using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000DED RID: 3565
	public class OnTriggerEventsCosmetic : MonoBehaviour
	{
		// Token: 0x0600584B RID: 22603 RVA: 0x001B2923 File Offset: 0x001B0B23
		private bool IsMyItem()
		{
			return this._rig != null && this._rig.isOfflineVRRig;
		}

		// Token: 0x0600584C RID: 22604 RVA: 0x001B2940 File Offset: 0x001B0B40
		private void Awake()
		{
			this._rig = base.GetComponentInParent<VRRig>();
			this.parentTransferable = base.GetComponentInParent<TransferrableObject>();
		}

		// Token: 0x0600584D RID: 22605 RVA: 0x001B295C File Offset: 0x001B0B5C
		private void FireEvents(Collider other, OnTriggerEventsCosmetic.Listener listener)
		{
			if (!listener.syncForEveryoneInRoom && !this.IsMyItem())
			{
				return;
			}
			if (this.parentTransferable && listener.fireOnlyWhileHeld && !this.parentTransferable.InHand())
			{
				return;
			}
			if (listener.triggerTagsList.Count > 0 && !this.IsTagValid(other.gameObject, listener))
			{
				return;
			}
			if (((1 << other.gameObject.layer) & listener.triggerLayerMask) == 0)
			{
				return;
			}
			bool flag = this.parentTransferable && this.parentTransferable.InLeftHand();
			UnityEvent<bool, Collider> listenerComponent = listener.listenerComponent;
			if (listenerComponent != null)
			{
				listenerComponent.Invoke(flag, other);
			}
			VRRig componentInParent = other.GetComponentInParent<VRRig>();
			if (componentInParent != null)
			{
				UnityEvent<VRRig> onTriggeredVRRig = listener.onTriggeredVRRig;
				if (onTriggeredVRRig == null)
				{
					return;
				}
				onTriggeredVRRig.Invoke(componentInParent);
			}
		}

		// Token: 0x0600584E RID: 22606 RVA: 0x001B2A2A File Offset: 0x001B0C2A
		private bool IsTagValid(GameObject obj, OnTriggerEventsCosmetic.Listener listener)
		{
			return listener.triggerTagsList.Contains(obj.tag);
		}

		// Token: 0x0600584F RID: 22607 RVA: 0x001B2A40 File Offset: 0x001B0C40
		private void OnTriggerEnter(Collider other)
		{
			for (int i = 0; i < this.eventListeners.Length; i++)
			{
				OnTriggerEventsCosmetic.Listener listener = this.eventListeners[i];
				if (listener.eventType == OnTriggerEventsCosmetic.EventType.TriggerEnter)
				{
					this.FireEvents(other, listener);
				}
			}
		}

		// Token: 0x06005850 RID: 22608 RVA: 0x001B2A7C File Offset: 0x001B0C7C
		private void OnTriggerExit(Collider other)
		{
			for (int i = 0; i < this.eventListeners.Length; i++)
			{
				OnTriggerEventsCosmetic.Listener listener = this.eventListeners[i];
				if (listener.eventType == OnTriggerEventsCosmetic.EventType.TriggerExit)
				{
					this.FireEvents(other, listener);
				}
			}
		}

		// Token: 0x06005851 RID: 22609 RVA: 0x001B2AB8 File Offset: 0x001B0CB8
		private void OnTriggerStay(Collider other)
		{
			for (int i = 0; i < this.eventListeners.Length; i++)
			{
				OnTriggerEventsCosmetic.Listener listener = this.eventListeners[i];
				if (listener.eventType == OnTriggerEventsCosmetic.EventType.TriggerStay)
				{
					this.FireEvents(other, listener);
				}
			}
		}

		// Token: 0x04005D7D RID: 23933
		public OnTriggerEventsCosmetic.Listener[] eventListeners = new OnTriggerEventsCosmetic.Listener[0];

		// Token: 0x04005D7E RID: 23934
		private VRRig _rig;

		// Token: 0x04005D7F RID: 23935
		private TransferrableObject parentTransferable;

		// Token: 0x02000DEE RID: 3566
		[Serializable]
		public class Listener
		{
			// Token: 0x04005D80 RID: 23936
			public LayerMask triggerLayerMask;

			// Token: 0x04005D81 RID: 23937
			public List<string> triggerTagsList = new List<string>();

			// Token: 0x04005D82 RID: 23938
			public OnTriggerEventsCosmetic.EventType eventType;

			// Token: 0x04005D83 RID: 23939
			public UnityEvent<bool, Collider> listenerComponent;

			// Token: 0x04005D84 RID: 23940
			public UnityEvent<VRRig> onTriggeredVRRig;

			// Token: 0x04005D85 RID: 23941
			public bool syncForEveryoneInRoom = true;

			// Token: 0x04005D86 RID: 23942
			[Tooltip("Fire these events only when the item is held in hand, only works if there is a transferable component somewhere on the object or its parent.")]
			public bool fireOnlyWhileHeld = true;
		}

		// Token: 0x02000DEF RID: 3567
		public enum EventType
		{
			// Token: 0x04005D88 RID: 23944
			TriggerEnter,
			// Token: 0x04005D89 RID: 23945
			TriggerStay,
			// Token: 0x04005D8A RID: 23946
			TriggerExit
		}
	}
}
