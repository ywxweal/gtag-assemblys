using System;
using GorillaTag.CosmeticSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000DD6 RID: 3542
	public class ControllerButtonEvent : MonoBehaviour, ISpawnable
	{
		// Token: 0x170008C0 RID: 2240
		// (get) Token: 0x060057D2 RID: 22482 RVA: 0x001AFBDA File Offset: 0x001ADDDA
		// (set) Token: 0x060057D3 RID: 22483 RVA: 0x001AFBE2 File Offset: 0x001ADDE2
		public bool IsSpawned { get; set; }

		// Token: 0x170008C1 RID: 2241
		// (get) Token: 0x060057D4 RID: 22484 RVA: 0x001AFBEB File Offset: 0x001ADDEB
		// (set) Token: 0x060057D5 RID: 22485 RVA: 0x001AFBF3 File Offset: 0x001ADDF3
		public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

		// Token: 0x060057D6 RID: 22486 RVA: 0x001AFBFC File Offset: 0x001ADDFC
		public void OnSpawn(VRRig rig)
		{
			this.myRig = rig;
		}

		// Token: 0x060057D7 RID: 22487 RVA: 0x000023F4 File Offset: 0x000005F4
		public void OnDespawn()
		{
		}

		// Token: 0x060057D8 RID: 22488 RVA: 0x001AFC05 File Offset: 0x001ADE05
		private bool IsMyItem()
		{
			return this.myRig != null && this.myRig.isOfflineVRRig;
		}

		// Token: 0x060057D9 RID: 22489 RVA: 0x001AFC22 File Offset: 0x001ADE22
		private void Awake()
		{
			this.triggerLastValue = 0f;
			this.gripLastValue = 0f;
			this.primaryLastValue = false;
			this.secondaryLastValue = false;
			this.frameCounter = 0;
		}

		// Token: 0x060057DA RID: 22490 RVA: 0x001AFC50 File Offset: 0x001ADE50
		public void LateUpdate()
		{
			if (!this.IsMyItem())
			{
				return;
			}
			XRNode xrnode = (this.inLeftHand ? XRNode.LeftHand : XRNode.RightHand);
			switch (this.buttonType)
			{
			case ControllerButtonEvent.ButtonType.trigger:
			{
				float num = ControllerInputPoller.TriggerFloat(xrnode);
				if (num > this.triggerValue)
				{
					this.frameCounter++;
				}
				if (num > this.triggerValue && this.triggerLastValue < this.triggerValue)
				{
					UnityEvent<bool, float> unityEvent = this.onButtonPressed;
					if (unityEvent != null)
					{
						unityEvent.Invoke(this.inLeftHand, num);
					}
				}
				else if (num <= this.triggerReleaseValue && this.triggerLastValue > this.triggerReleaseValue)
				{
					UnityEvent<bool, float> unityEvent2 = this.onButtonReleased;
					if (unityEvent2 != null)
					{
						unityEvent2.Invoke(this.inLeftHand, num);
					}
					this.frameCounter = 0;
				}
				else if (num > this.triggerValue && this.triggerLastValue >= this.triggerValue && this.frameCounter % this.frameInterval == 0)
				{
					UnityEvent<bool, float> unityEvent3 = this.onButtonPressStayed;
					if (unityEvent3 != null)
					{
						unityEvent3.Invoke(this.inLeftHand, num);
					}
					this.frameCounter = 0;
				}
				this.triggerLastValue = num;
				return;
			}
			case ControllerButtonEvent.ButtonType.primary:
			{
				bool flag = ControllerInputPoller.PrimaryButtonPress(xrnode);
				if (flag)
				{
					this.frameCounter++;
				}
				if (flag && !this.primaryLastValue)
				{
					UnityEvent<bool, float> unityEvent4 = this.onButtonPressed;
					if (unityEvent4 != null)
					{
						unityEvent4.Invoke(this.inLeftHand, 1f);
					}
				}
				else if (!flag && this.primaryLastValue)
				{
					UnityEvent<bool, float> unityEvent5 = this.onButtonReleased;
					if (unityEvent5 != null)
					{
						unityEvent5.Invoke(this.inLeftHand, 0f);
					}
					this.frameCounter = 0;
				}
				else if (flag && this.primaryLastValue && this.frameCounter % this.frameInterval == 0)
				{
					UnityEvent<bool, float> unityEvent6 = this.onButtonPressStayed;
					if (unityEvent6 != null)
					{
						unityEvent6.Invoke(this.inLeftHand, 1f);
					}
					this.frameCounter = 0;
				}
				this.primaryLastValue = flag;
				return;
			}
			case ControllerButtonEvent.ButtonType.secondary:
			{
				bool flag2 = ControllerInputPoller.SecondaryButtonPress(xrnode);
				if (flag2)
				{
					this.frameCounter++;
				}
				if (flag2 && !this.secondaryLastValue)
				{
					UnityEvent<bool, float> unityEvent7 = this.onButtonPressed;
					if (unityEvent7 != null)
					{
						unityEvent7.Invoke(this.inLeftHand, 1f);
					}
				}
				else if (!flag2 && this.secondaryLastValue)
				{
					UnityEvent<bool, float> unityEvent8 = this.onButtonReleased;
					if (unityEvent8 != null)
					{
						unityEvent8.Invoke(this.inLeftHand, 0f);
					}
					this.frameCounter = 0;
				}
				else if (flag2 && this.secondaryLastValue && this.frameCounter % this.frameInterval == 0)
				{
					UnityEvent<bool, float> unityEvent9 = this.onButtonPressStayed;
					if (unityEvent9 != null)
					{
						unityEvent9.Invoke(this.inLeftHand, 1f);
					}
					this.frameCounter = 0;
				}
				this.secondaryLastValue = flag2;
				return;
			}
			case ControllerButtonEvent.ButtonType.grip:
			{
				float num2 = ControllerInputPoller.GripFloat(xrnode);
				if (num2 > this.gripValue)
				{
					this.frameCounter++;
				}
				if (num2 > this.gripValue && this.gripLastValue < this.gripValue)
				{
					UnityEvent<bool, float> unityEvent10 = this.onButtonPressed;
					if (unityEvent10 != null)
					{
						unityEvent10.Invoke(this.inLeftHand, num2);
					}
				}
				else if (num2 <= this.gripReleaseValue && this.gripLastValue > this.gripReleaseValue)
				{
					UnityEvent<bool, float> unityEvent11 = this.onButtonReleased;
					if (unityEvent11 != null)
					{
						unityEvent11.Invoke(this.inLeftHand, num2);
					}
					this.frameCounter = 0;
				}
				else if (num2 > this.gripValue && this.gripLastValue >= this.gripValue && this.frameCounter % this.frameInterval == 0)
				{
					UnityEvent<bool, float> unityEvent12 = this.onButtonPressStayed;
					if (unityEvent12 != null)
					{
						unityEvent12.Invoke(this.inLeftHand, num2);
					}
					this.frameCounter = 0;
				}
				this.gripLastValue = num2;
				return;
			}
			default:
				return;
			}
		}

		// Token: 0x04005C9E RID: 23710
		[SerializeField]
		private float gripValue = 0.75f;

		// Token: 0x04005C9F RID: 23711
		[SerializeField]
		private float gripReleaseValue = 0.01f;

		// Token: 0x04005CA0 RID: 23712
		[SerializeField]
		private float triggerValue = 0.75f;

		// Token: 0x04005CA1 RID: 23713
		[SerializeField]
		private float triggerReleaseValue = 0.01f;

		// Token: 0x04005CA2 RID: 23714
		[SerializeField]
		private ControllerButtonEvent.ButtonType buttonType;

		// Token: 0x04005CA3 RID: 23715
		[Tooltip("How many frames should pass to trigger a press stayed button")]
		[SerializeField]
		private int frameInterval = 20;

		// Token: 0x04005CA4 RID: 23716
		public UnityEvent<bool, float> onButtonPressed;

		// Token: 0x04005CA5 RID: 23717
		public UnityEvent<bool, float> onButtonReleased;

		// Token: 0x04005CA6 RID: 23718
		public UnityEvent<bool, float> onButtonPressStayed;

		// Token: 0x04005CA7 RID: 23719
		private float triggerLastValue;

		// Token: 0x04005CA8 RID: 23720
		private float gripLastValue;

		// Token: 0x04005CA9 RID: 23721
		private bool primaryLastValue;

		// Token: 0x04005CAA RID: 23722
		private bool secondaryLastValue;

		// Token: 0x04005CAB RID: 23723
		private int frameCounter;

		// Token: 0x04005CAC RID: 23724
		private bool inLeftHand;

		// Token: 0x04005CAD RID: 23725
		private VRRig myRig;

		// Token: 0x02000DD7 RID: 3543
		private enum ButtonType
		{
			// Token: 0x04005CB1 RID: 23729
			trigger,
			// Token: 0x04005CB2 RID: 23730
			primary,
			// Token: 0x04005CB3 RID: 23731
			secondary,
			// Token: 0x04005CB4 RID: 23732
			grip
		}
	}
}
