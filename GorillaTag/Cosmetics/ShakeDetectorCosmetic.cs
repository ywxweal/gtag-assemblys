using System;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000DF4 RID: 3572
	public class ShakeDetectorCosmetic : MonoBehaviour
	{
		// Token: 0x170008CD RID: 2253
		// (get) Token: 0x0600586C RID: 22636 RVA: 0x001B307C File Offset: 0x001B127C
		// (set) Token: 0x0600586D RID: 22637 RVA: 0x001B3084 File Offset: 0x001B1284
		public Vector3 HandVelocity { get; private set; }

		// Token: 0x0600586E RID: 22638 RVA: 0x001B308D File Offset: 0x001B128D
		private void Awake()
		{
			this.HandVelocity = Vector3.zero;
			this.shakeEndTime = 0f;
		}

		// Token: 0x0600586F RID: 22639 RVA: 0x001B30A8 File Offset: 0x001B12A8
		private void UpdateShakeVelocity()
		{
			if (!this.parentTransferrable)
			{
				return;
			}
			if (!this.parentTransferrable.InHand())
			{
				this.HandVelocity = Vector3.zero;
				return;
			}
			if (!this.parentTransferrable.IsMyItem())
			{
				return;
			}
			this.isLeftHand = this.parentTransferrable.InLeftHand();
			this.HandVelocity = (this.isLeftHand ? GTPlayer.Instance.leftInteractPointVelocityTracker.GetAverageVelocity(true, 0.15f, false) : GTPlayer.Instance.rightInteractPointVelocityTracker.GetAverageVelocity(true, 0.15f, false));
			this.HandVelocity = Vector3.ClampMagnitude(this.HandVelocity, this.maxHandVelocity);
		}

		// Token: 0x06005870 RID: 22640 RVA: 0x001B3150 File Offset: 0x001B1350
		public void Update()
		{
			this.UpdateShakeVelocity();
			if (Time.time - this.shakeEndTime > this.cooldown && !this.isShaking && this.HandVelocity.magnitude >= this.shakeStartVelocityThreshold)
			{
				UnityEvent<bool, float> unityEvent = this.onShakeStartLocal;
				if (unityEvent != null)
				{
					unityEvent.Invoke(this.isLeftHand, this.HandVelocity.magnitude);
				}
				this.isShaking = true;
			}
			if (this.isShaking && this.HandVelocity.magnitude < this.shakeEndVelocityThreshold)
			{
				UnityEvent<bool, float> unityEvent2 = this.onShakeEndLocal;
				if (unityEvent2 != null)
				{
					unityEvent2.Invoke(this.isLeftHand, this.HandVelocity.magnitude);
				}
				this.isShaking = false;
				this.shakeEndTime = Time.time;
			}
		}

		// Token: 0x04005DAE RID: 23982
		[SerializeField]
		private TransferrableObject parentTransferrable;

		// Token: 0x04005DAF RID: 23983
		[Tooltip("for velocity equal or above this, we fire a Shake Start event")]
		[SerializeField]
		private float shakeStartVelocityThreshold;

		// Token: 0x04005DB0 RID: 23984
		[Tooltip("for velocity under this, we fire a Shake End event")]
		[SerializeField]
		private float shakeEndVelocityThreshold;

		// Token: 0x04005DB1 RID: 23985
		[Tooltip("cooldown starts when shaking ends")]
		[SerializeField]
		private float cooldown;

		// Token: 0x04005DB2 RID: 23986
		[Tooltip("Use for clamping hand velocity value")]
		[SerializeField]
		private float maxHandVelocity = 20f;

		// Token: 0x04005DB3 RID: 23987
		[FormerlySerializedAs("onShakeStart")]
		public UnityEvent<bool, float> onShakeStartLocal;

		// Token: 0x04005DB4 RID: 23988
		[FormerlySerializedAs("onShakeEnd")]
		public UnityEvent<bool, float> onShakeEndLocal;

		// Token: 0x04005DB6 RID: 23990
		private bool isShaking;

		// Token: 0x04005DB7 RID: 23991
		private float shakeEndTime;

		// Token: 0x04005DB8 RID: 23992
		private bool isLeftHand;
	}
}
