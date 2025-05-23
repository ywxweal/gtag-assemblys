using System;
using System.Collections;
using Photon.Voice.Unity;
using UnityEngine;

namespace GorillaTag.Audio
{
	// Token: 0x02000D90 RID: 3472
	public class GTRecorder : Recorder
	{
		// Token: 0x0600562F RID: 22063 RVA: 0x001A3BFE File Offset: 0x001A1DFE
		protected override MicWrapper CreateMicWrapper(string micDev, int samplingRateInt, VoiceLogger logger)
		{
			this._micWrapper = new GTMicWrapper(micDev, samplingRateInt, this.AllowPitchAdjustment, this.PitchAdjustment, this.AllowVolumeAdjustment, this.VolumeAdjustment, logger);
			return this._micWrapper;
		}

		// Token: 0x06005630 RID: 22064 RVA: 0x001A3C2C File Offset: 0x001A1E2C
		private IEnumerator DoTestEcho()
		{
			base.DebugEchoMode = true;
			yield return new WaitForSeconds(this.DebugEchoLength);
			base.DebugEchoMode = false;
			yield return null;
			this._testEchoCoroutine = null;
			yield break;
		}

		// Token: 0x06005631 RID: 22065 RVA: 0x001A3C3B File Offset: 0x001A1E3B
		public void LateUpdate()
		{
			if (this._micWrapper != null)
			{
				this._micWrapper.UpdateWrapper(this.AllowPitchAdjustment, this.PitchAdjustment, this.AllowVolumeAdjustment, this.VolumeAdjustment);
			}
		}

		// Token: 0x04005A02 RID: 23042
		public bool AllowPitchAdjustment;

		// Token: 0x04005A03 RID: 23043
		public float PitchAdjustment = 1f;

		// Token: 0x04005A04 RID: 23044
		public bool AllowVolumeAdjustment;

		// Token: 0x04005A05 RID: 23045
		public float VolumeAdjustment = 1f;

		// Token: 0x04005A06 RID: 23046
		public float DebugEchoLength = 5f;

		// Token: 0x04005A07 RID: 23047
		private GTMicWrapper _micWrapper;

		// Token: 0x04005A08 RID: 23048
		private Coroutine _testEchoCoroutine;
	}
}
