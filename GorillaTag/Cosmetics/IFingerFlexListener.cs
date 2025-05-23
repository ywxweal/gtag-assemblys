using System;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000DE5 RID: 3557
	public interface IFingerFlexListener
	{
		// Token: 0x0600581E RID: 22558 RVA: 0x00047642 File Offset: 0x00045842
		bool FingerFlexValidation(bool isLeftHand)
		{
			return true;
		}

		// Token: 0x0600581F RID: 22559
		void OnButtonPressed(bool isLeftHand, float value);

		// Token: 0x06005820 RID: 22560
		void OnButtonReleased(bool isLeftHand, float value);

		// Token: 0x06005821 RID: 22561
		void OnButtonPressStayed(bool isLeftHand, float value);

		// Token: 0x02000DE6 RID: 3558
		public enum ComponentActivator
		{
			// Token: 0x04005D59 RID: 23897
			FingerReleased,
			// Token: 0x04005D5A RID: 23898
			FingerFlexed,
			// Token: 0x04005D5B RID: 23899
			FingerStayed
		}
	}
}
