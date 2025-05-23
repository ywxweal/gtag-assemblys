using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000DE0 RID: 3552
	public class FingerFlexEvent : MonoBehaviour
	{
		// Token: 0x0600580A RID: 22538 RVA: 0x001B1B4B File Offset: 0x001AFD4B
		private void Awake()
		{
			this._rig = base.GetComponentInParent<VRRig>();
			this.parentTransferable = base.GetComponentInParent<TransferrableObject>();
		}

		// Token: 0x0600580B RID: 22539 RVA: 0x001B1B65 File Offset: 0x001AFD65
		private bool IsMyItem()
		{
			return this._rig != null && this._rig.isOfflineVRRig;
		}

		// Token: 0x0600580C RID: 22540 RVA: 0x001B1B84 File Offset: 0x001AFD84
		private void Update()
		{
			for (int i = 0; i < this.eventListeners.Length; i++)
			{
				FingerFlexEvent.Listener listener = this.eventListeners[i];
				this.FireEvents(listener);
			}
		}

		// Token: 0x0600580D RID: 22541 RVA: 0x001B1BB4 File Offset: 0x001AFDB4
		private void FireEvents(FingerFlexEvent.Listener listener)
		{
			if (!listener.syncForEveryoneInRoom && !this.IsMyItem())
			{
				return;
			}
			if (this.parentTransferable && !this.parentTransferable.InHand() && listener.eventType == FingerFlexEvent.EventType.OnFingerReleased)
			{
				if (listener.fingerRightLastValue > listener.fingerReleaseValue)
				{
					UnityEvent<bool, float> listenerComponent = listener.listenerComponent;
					if (listenerComponent != null)
					{
						listenerComponent.Invoke(false, 0f);
					}
					listener.fingerRightLastValue = 0f;
				}
				if (listener.fingerLeftLastValue > listener.fingerReleaseValue)
				{
					UnityEvent<bool, float> listenerComponent2 = listener.listenerComponent;
					if (listenerComponent2 != null)
					{
						listenerComponent2.Invoke(true, 0f);
					}
					listener.fingerLeftLastValue = 0f;
				}
			}
			if (this.parentTransferable && listener.fireOnlyWhileHeld && !this.parentTransferable.InHand())
			{
				return;
			}
			switch (this.fingerType)
			{
			case FingerFlexEvent.FingerType.Thumb:
			{
				float calcT = this._rig.leftThumb.calcT;
				float calcT2 = this._rig.rightThumb.calcT;
				this.FireEvents(listener, calcT, calcT2);
				return;
			}
			case FingerFlexEvent.FingerType.Index:
			{
				float calcT3 = this._rig.leftIndex.calcT;
				float calcT4 = this._rig.rightIndex.calcT;
				this.FireEvents(listener, calcT3, calcT4);
				return;
			}
			case FingerFlexEvent.FingerType.Middle:
			{
				float calcT5 = this._rig.leftMiddle.calcT;
				float calcT6 = this._rig.rightMiddle.calcT;
				this.FireEvents(listener, calcT5, calcT6);
				return;
			}
			default:
				return;
			}
		}

		// Token: 0x0600580E RID: 22542 RVA: 0x001B1D1C File Offset: 0x001AFF1C
		private void FireEvents(FingerFlexEvent.Listener listener, float leftFinger, float rightFinger)
		{
			if (this.parentTransferable && this.FingerFlexValidation(true))
			{
				this.CheckFingerValue(listener, leftFinger, true, ref listener.fingerLeftLastValue);
				return;
			}
			if (this.parentTransferable && this.FingerFlexValidation(false))
			{
				this.CheckFingerValue(listener, rightFinger, false, ref listener.fingerRightLastValue);
				return;
			}
			this.CheckFingerValue(listener, leftFinger, true, ref listener.fingerLeftLastValue);
			this.CheckFingerValue(listener, rightFinger, false, ref listener.fingerRightLastValue);
		}

		// Token: 0x0600580F RID: 22543 RVA: 0x001B1D94 File Offset: 0x001AFF94
		private void CheckFingerValue(FingerFlexEvent.Listener listener, float fingerValue, bool isLeft, ref float lastValue)
		{
			if (fingerValue > listener.fingerFlexValue)
			{
				listener.frameCounter++;
			}
			switch (listener.eventType)
			{
			case FingerFlexEvent.EventType.OnFingerFlexed:
				if (fingerValue > listener.fingerFlexValue && lastValue < listener.fingerFlexValue)
				{
					UnityEvent<bool, float> listenerComponent = listener.listenerComponent;
					if (listenerComponent != null)
					{
						listenerComponent.Invoke(isLeft, fingerValue);
					}
				}
				break;
			case FingerFlexEvent.EventType.OnFingerReleased:
				if (fingerValue <= listener.fingerReleaseValue && lastValue > listener.fingerReleaseValue)
				{
					UnityEvent<bool, float> listenerComponent2 = listener.listenerComponent;
					if (listenerComponent2 != null)
					{
						listenerComponent2.Invoke(isLeft, fingerValue);
					}
					listener.frameCounter = 0;
				}
				break;
			case FingerFlexEvent.EventType.OnFingerFlexStayed:
				if (fingerValue > listener.fingerFlexValue && lastValue >= listener.fingerFlexValue && listener.frameCounter % listener.frameInterval == 0)
				{
					UnityEvent<bool, float> listenerComponent3 = listener.listenerComponent;
					if (listenerComponent3 != null)
					{
						listenerComponent3.Invoke(isLeft, fingerValue);
					}
					listener.frameCounter = 0;
				}
				break;
			}
			lastValue = fingerValue;
		}

		// Token: 0x06005810 RID: 22544 RVA: 0x001B1E76 File Offset: 0x001B0076
		private bool FingerFlexValidation(bool isLeftHand)
		{
			return (!this.parentTransferable.InLeftHand() || isLeftHand) && (this.parentTransferable.InLeftHand() || !isLeftHand);
		}

		// Token: 0x04005D35 RID: 23861
		[SerializeField]
		private FingerFlexEvent.FingerType fingerType = FingerFlexEvent.FingerType.Index;

		// Token: 0x04005D36 RID: 23862
		public FingerFlexEvent.Listener[] eventListeners = new FingerFlexEvent.Listener[0];

		// Token: 0x04005D37 RID: 23863
		private VRRig _rig;

		// Token: 0x04005D38 RID: 23864
		private TransferrableObject parentTransferable;

		// Token: 0x02000DE1 RID: 3553
		[Serializable]
		public class Listener
		{
			// Token: 0x04005D39 RID: 23865
			public FingerFlexEvent.EventType eventType;

			// Token: 0x04005D3A RID: 23866
			public UnityEvent<bool, float> listenerComponent;

			// Token: 0x04005D3B RID: 23867
			public float fingerFlexValue = 0.75f;

			// Token: 0x04005D3C RID: 23868
			public float fingerReleaseValue = 0.01f;

			// Token: 0x04005D3D RID: 23869
			[Tooltip("How many frames should pass to fire a finger flex stayed event")]
			public int frameInterval = 20;

			// Token: 0x04005D3E RID: 23870
			[Tooltip("This event will be fired for everyone in the room (synced) by default unless you uncheck this box so that it will be fired only for the local player.")]
			public bool syncForEveryoneInRoom = true;

			// Token: 0x04005D3F RID: 23871
			[Tooltip("Fire these events only when the item is held in hand, only works if there is a transferable component somewhere on the object or its parent.")]
			public bool fireOnlyWhileHeld = true;

			// Token: 0x04005D40 RID: 23872
			internal int frameCounter;

			// Token: 0x04005D41 RID: 23873
			internal float fingerRightLastValue;

			// Token: 0x04005D42 RID: 23874
			internal float fingerLeftLastValue;
		}

		// Token: 0x02000DE2 RID: 3554
		public enum EventType
		{
			// Token: 0x04005D44 RID: 23876
			OnFingerFlexed,
			// Token: 0x04005D45 RID: 23877
			OnFingerReleased,
			// Token: 0x04005D46 RID: 23878
			OnFingerFlexStayed
		}

		// Token: 0x02000DE3 RID: 3555
		private enum FingerType
		{
			// Token: 0x04005D48 RID: 23880
			Thumb,
			// Token: 0x04005D49 RID: 23881
			Index,
			// Token: 0x04005D4A RID: 23882
			Middle
		}
	}
}
