using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000BCB RID: 3019
	public class PauseOnInputLoss : MonoBehaviour
	{
		// Token: 0x06004A8D RID: 19085 RVA: 0x001632F3 File Offset: 0x001614F3
		private void Start()
		{
			OVRManager.InputFocusAcquired += this.OnInputFocusAcquired;
			OVRManager.InputFocusLost += this.OnInputFocusLost;
		}

		// Token: 0x06004A8E RID: 19086 RVA: 0x00163317 File Offset: 0x00161517
		private void OnInputFocusLost()
		{
			Time.timeScale = 0f;
		}

		// Token: 0x06004A8F RID: 19087 RVA: 0x00163323 File Offset: 0x00161523
		private void OnInputFocusAcquired()
		{
			Time.timeScale = 1f;
		}
	}
}
