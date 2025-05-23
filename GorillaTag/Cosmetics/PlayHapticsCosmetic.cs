using System;
using GorillaLocomotion;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000DF0 RID: 3568
	public class PlayHapticsCosmetic : MonoBehaviour
	{
		// Token: 0x06005855 RID: 22613 RVA: 0x001B2BFF File Offset: 0x001B0DFF
		public void PlayHaptics(bool isLeftHand)
		{
			GorillaTagger.Instance.StartVibration(isLeftHand, this.hapticStrength, this.hapticDuration);
		}

		// Token: 0x06005856 RID: 22614 RVA: 0x001B2BFF File Offset: 0x001B0DFF
		public void PlayHaptics(bool isLeftHand, float value)
		{
			GorillaTagger.Instance.StartVibration(isLeftHand, this.hapticStrength, this.hapticDuration);
		}

		// Token: 0x06005857 RID: 22615 RVA: 0x001B2BFF File Offset: 0x001B0DFF
		public void PlayHaptics(bool isLeftHand, Collider other)
		{
			GorillaTagger.Instance.StartVibration(isLeftHand, this.hapticStrength, this.hapticDuration);
		}

		// Token: 0x06005858 RID: 22616 RVA: 0x001B2BFF File Offset: 0x001B0DFF
		public void PlayHaptics(bool isLeftHand, Collision other)
		{
			GorillaTagger.Instance.StartVibration(isLeftHand, this.hapticStrength, this.hapticDuration);
		}

		// Token: 0x06005859 RID: 22617 RVA: 0x001B2C18 File Offset: 0x001B0E18
		public void PlayHapticsByButtonValue(bool isLeftHand, float strength)
		{
			float num = Mathf.InverseLerp(this.minHapticStrengthThreshold, this.maxHapticStrengthThreshold, strength);
			GorillaTagger.Instance.StartVibration(isLeftHand, num, this.hapticDuration);
		}

		// Token: 0x0600585A RID: 22618 RVA: 0x001B2C4C File Offset: 0x001B0E4C
		public void PlayHapticsByVelocity(bool isLeftHand, float velocity)
		{
			float num = (isLeftHand ? GTPlayer.Instance.leftInteractPointVelocityTracker.GetAverageVelocity(true, 0.15f, false) : GTPlayer.Instance.rightInteractPointVelocityTracker.GetAverageVelocity(true, 0.15f, false)).magnitude;
			num = Mathf.InverseLerp(this.minHapticStrengthThreshold, this.maxHapticStrengthThreshold, num);
			GorillaTagger.Instance.StartVibration(isLeftHand, num, this.hapticDuration);
		}

		// Token: 0x04005D8C RID: 23948
		[SerializeField]
		private float hapticDuration;

		// Token: 0x04005D8D RID: 23949
		[SerializeField]
		private float hapticStrength;

		// Token: 0x04005D8E RID: 23950
		[SerializeField]
		private float minHapticStrengthThreshold;

		// Token: 0x04005D8F RID: 23951
		[SerializeField]
		private float maxHapticStrengthThreshold;
	}
}
