using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02000E6B RID: 3691
	public class BoingManagerPreUpdatePump : MonoBehaviour
	{
		// Token: 0x06005C5A RID: 23642 RVA: 0x001C5202 File Offset: 0x001C3402
		private void FixedUpdate()
		{
			this.TryPump();
		}

		// Token: 0x06005C5B RID: 23643 RVA: 0x001C5202 File Offset: 0x001C3402
		private void Update()
		{
			this.TryPump();
		}

		// Token: 0x06005C5C RID: 23644 RVA: 0x001C520A File Offset: 0x001C340A
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

		// Token: 0x06005C5D RID: 23645 RVA: 0x001C5234 File Offset: 0x001C3434
		private void DoPump()
		{
			BoingManager.RestoreBehaviors();
			BoingManager.RestoreReactors();
			BoingManager.RestoreBones();
			BoingManager.DispatchReactorFieldCompute();
		}

		// Token: 0x0400603F RID: 24639
		private int m_lastPumpedFrame = -1;
	}
}
