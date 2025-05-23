using System;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000DD4 RID: 3540
	public class CloserCosmetic : MonoBehaviour, ITickSystemTick
	{
		// Token: 0x170008BF RID: 2239
		// (get) Token: 0x060057C7 RID: 22471 RVA: 0x001AF98F File Offset: 0x001ADB8F
		// (set) Token: 0x060057C8 RID: 22472 RVA: 0x001AF997 File Offset: 0x001ADB97
		public bool TickRunning { get; set; }

		// Token: 0x060057C9 RID: 22473 RVA: 0x001AF9A0 File Offset: 0x001ADBA0
		private void OnEnable()
		{
			TickSystem<object>.AddCallbackTarget(this);
			this.localRotA = this.sideA.transform.localRotation;
			this.localRotB = this.sideB.transform.localRotation;
			this.fingerValue = 0f;
			this.UpdateState(CloserCosmetic.State.Opening);
		}

		// Token: 0x060057CA RID: 22474 RVA: 0x0001C957 File Offset: 0x0001AB57
		private void OnDisable()
		{
			TickSystem<object>.RemoveCallbackTarget(this);
		}

		// Token: 0x060057CB RID: 22475 RVA: 0x001AF9F4 File Offset: 0x001ADBF4
		public void Tick()
		{
			switch (this.currentState)
			{
			case CloserCosmetic.State.Closing:
				this.Closing();
				return;
			case CloserCosmetic.State.Opening:
				this.Opening();
				break;
			case CloserCosmetic.State.None:
				break;
			default:
				return;
			}
		}

		// Token: 0x060057CC RID: 22476 RVA: 0x001AFA28 File Offset: 0x001ADC28
		public void Close(bool leftHand, float fingerFlexValue)
		{
			this.UpdateState(CloserCosmetic.State.Closing);
			this.fingerValue = fingerFlexValue;
		}

		// Token: 0x060057CD RID: 22477 RVA: 0x001AFA38 File Offset: 0x001ADC38
		public void Open(bool leftHand, float fingerFlexValue)
		{
			this.UpdateState(CloserCosmetic.State.Opening);
			this.fingerValue = fingerFlexValue;
		}

		// Token: 0x060057CE RID: 22478 RVA: 0x001AFA48 File Offset: 0x001ADC48
		private void Closing()
		{
			float num = (this.useFingerFlexValueAsStrength ? Mathf.Clamp01(this.fingerValue) : 1f);
			Quaternion quaternion = Quaternion.Euler(this.maxRotationB);
			Quaternion quaternion2 = Quaternion.Slerp(this.localRotB, quaternion, num);
			this.sideB.transform.localRotation = quaternion2;
			Quaternion quaternion3 = Quaternion.Euler(this.maxRotationA);
			Quaternion quaternion4 = Quaternion.Slerp(this.localRotA, quaternion3, num);
			this.sideA.transform.localRotation = quaternion4;
			if (Quaternion.Angle(this.sideB.transform.localRotation, quaternion2) < 0.1f && Quaternion.Angle(this.sideA.transform.localRotation, quaternion4) < 0.1f)
			{
				this.UpdateState(CloserCosmetic.State.None);
			}
		}

		// Token: 0x060057CF RID: 22479 RVA: 0x001AFB0C File Offset: 0x001ADD0C
		private void Opening()
		{
			float num = (this.useFingerFlexValueAsStrength ? Mathf.Clamp01(this.fingerValue) : 1f);
			Quaternion quaternion = Quaternion.Slerp(this.sideB.transform.localRotation, this.localRotB, num);
			this.sideB.transform.localRotation = quaternion;
			Quaternion quaternion2 = Quaternion.Slerp(this.sideA.transform.localRotation, this.localRotA, num);
			this.sideA.transform.localRotation = quaternion2;
			if (Quaternion.Angle(this.sideB.transform.localRotation, quaternion) < 0.1f && Quaternion.Angle(this.sideA.transform.localRotation, quaternion2) < 0.1f)
			{
				this.UpdateState(CloserCosmetic.State.None);
			}
		}

		// Token: 0x060057D0 RID: 22480 RVA: 0x001AFBD1 File Offset: 0x001ADDD1
		private void UpdateState(CloserCosmetic.State newState)
		{
			this.currentState = newState;
		}

		// Token: 0x04005C90 RID: 23696
		[SerializeField]
		private GameObject sideA;

		// Token: 0x04005C91 RID: 23697
		[SerializeField]
		private GameObject sideB;

		// Token: 0x04005C92 RID: 23698
		[SerializeField]
		private Vector3 maxRotationA;

		// Token: 0x04005C93 RID: 23699
		[SerializeField]
		private Vector3 maxRotationB;

		// Token: 0x04005C94 RID: 23700
		[SerializeField]
		private bool useFingerFlexValueAsStrength;

		// Token: 0x04005C95 RID: 23701
		private Quaternion localRotA;

		// Token: 0x04005C96 RID: 23702
		private Quaternion localRotB;

		// Token: 0x04005C97 RID: 23703
		private CloserCosmetic.State currentState;

		// Token: 0x04005C98 RID: 23704
		private float fingerValue;

		// Token: 0x02000DD5 RID: 3541
		private enum State
		{
			// Token: 0x04005C9B RID: 23707
			Closing,
			// Token: 0x04005C9C RID: 23708
			Opening,
			// Token: 0x04005C9D RID: 23709
			None
		}
	}
}
