using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000BCB RID: 3019
	public class PauseOnInputLoss : MonoBehaviour
	{
		// Token: 0x06004A8E RID: 19086 RVA: 0x001633CB File Offset: 0x001615CB
		private void Start()
		{
			OVRManager.InputFocusAcquired += this.OnInputFocusAcquired;
			OVRManager.InputFocusLost += this.OnInputFocusLost;
		}

		// Token: 0x06004A8F RID: 19087 RVA: 0x001633EF File Offset: 0x001615EF
		private void OnInputFocusLost()
		{
			Time.timeScale = 0f;
		}

		// Token: 0x06004A90 RID: 19088 RVA: 0x001633FB File Offset: 0x001615FB
		private void OnInputFocusAcquired()
		{
			Time.timeScale = 1f;
		}
	}
}
