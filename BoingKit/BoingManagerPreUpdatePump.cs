using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02000E6B RID: 3691
	public class BoingManagerPreUpdatePump : MonoBehaviour
	{
		// Token: 0x06005C59 RID: 23641 RVA: 0x001C512A File Offset: 0x001C332A
		private void FixedUpdate()
		{
			this.TryPump();
		}

		// Token: 0x06005C5A RID: 23642 RVA: 0x001C512A File Offset: 0x001C332A
		private void Update()
		{
			this.TryPump();
		}

		// Token: 0x06005C5B RID: 23643 RVA: 0x001C5132 File Offset: 0x001C3332
		private void TryPump()
		{
			if (this.m_lastPumpedFrame >= Time.frameCount)
			{
				return;
			}
			if (this.m_lastPumpedFrame >= 0)
			{
				this.DoPump();
			}
			this.m_lastPumpedFrame = Time.frameCount;
		}

		// Token: 0x06005C5C RID: 23644 RVA: 0x001C515C File Offset: 0x001C335C
		private void DoPump()
		{
			BoingManager.RestoreBehaviors();
			BoingManager.RestoreReactors();
			BoingManager.RestoreBones();
			BoingManager.DispatchReactorFieldCompute();
		}

		// Token: 0x0400603E RID: 24638
		private int m_lastPumpedFrame = -1;
	}
}
