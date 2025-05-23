using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000BE7 RID: 3047
	public class PinchStateModule
	{
		// Token: 0x17000767 RID: 1895
		// (get) Token: 0x06004B3B RID: 19259 RVA: 0x00165249 File Offset: 0x00163449
		public bool PinchUpAndDownOnFocusedObject
		{
			get
			{
				return this._currPinchState == PinchStateModule.PinchState.PinchUp && this._firstFocusedInteractable != null;
			}
		}

		// Token: 0x17000768 RID: 1896
		// (get) Token: 0x06004B3C RID: 19260 RVA: 0x00165262 File Offset: 0x00163462
		public bool PinchSteadyOnFocusedObject
		{
			get
			{
				return this._currPinchState == PinchStateModule.PinchState.PinchStay && this._firstFocusedInteractable != null;
			}
		}

		// Token: 0x17000769 RID: 1897
		// (get) Token: 0x06004B3D RID: 19261 RVA: 0x0016527B File Offset: 0x0016347B
		public bool PinchDownOnFocusedObject
		{
			get
			{
				return this._currPinchState == PinchStateModule.PinchState.PinchDown && this._firstFocusedInteractable != null;
			}
		}

		// Token: 0x06004B3E RID: 19262 RVA: 0x00165294 File Offset: 0x00163494
		public PinchStateModule()
		{
			this._currPinchState = PinchStateModule.PinchState.None;
			this._firstFocusedInteractable = null;
		}

		// Token: 0x06004B3F RID: 19263 RVA: 0x001652AC File Offset: 0x001634AC
		public void UpdateState(OVRHand hand, Interactable currFocusedInteractable)
		{
			float fingerPinchStrength = hand.GetFingerPinchStrength(OVRHand.HandFinger.Index);
			bool flag = Mathf.Abs(1f - fingerPinchStrength) < Mathf.Epsilon;
			switch (this._currPinchState)
			{
			case PinchStateModule.PinchState.PinchDown:
				this._currPinchState = (flag ? PinchStateModule.PinchState.PinchStay : PinchStateModule.PinchState.PinchUp);
				if (this._firstFocusedInteractable != currFocusedInteractable)
				{
					this._firstFocusedInteractable = null;
					return;
				}
				break;
			case PinchStateModule.PinchState.PinchStay:
				if (!flag)
				{
					this._currPinchState = PinchStateModule.PinchState.PinchUp;
				}
				if (currFocusedInteractable != this._firstFocusedInteractable)
				{
					this._firstFocusedInteractable = null;
					return;
				}
				break;
			case PinchStateModule.PinchState.PinchUp:
				if (!flag)
				{
					this._currPinchState = PinchStateModule.PinchState.None;
					this._firstFocusedInteractable = null;
					return;
				}
				this._currPinchState = PinchStateModule.PinchState.PinchDown;
				if (currFocusedInteractable != this._firstFocusedInteractable)
				{
					this._firstFocusedInteractable = null;
					return;
				}
				break;
			default:
				if (flag)
				{
					this._currPinchState = PinchStateModule.PinchState.PinchDown;
					this._firstFocusedInteractable = currFocusedInteractable;
				}
				break;
			}
		}

		// Token: 0x04004DD6 RID: 19926
		private const float PINCH_STRENGTH_THRESHOLD = 1f;

		// Token: 0x04004DD7 RID: 19927
		private PinchStateModule.PinchState _currPinchState;

		// Token: 0x04004DD8 RID: 19928
		private Interactable _firstFocusedInteractable;

		// Token: 0x02000BE8 RID: 3048
		private enum PinchState
		{
			// Token: 0x04004DDA RID: 19930
			None,
			// Token: 0x04004DDB RID: 19931
			PinchDown,
			// Token: 0x04004DDC RID: 19932
			PinchStay,
			// Token: 0x04004DDD RID: 19933
			PinchUp
		}
	}
}
