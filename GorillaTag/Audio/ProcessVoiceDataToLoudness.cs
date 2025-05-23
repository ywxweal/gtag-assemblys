using System;
using Photon.Voice;
using UnityEngine;

namespace GorillaTag.Audio
{
	// Token: 0x02000D8D RID: 3469
	internal class ProcessVoiceDataToLoudness : IProcessor<float>, IDisposable
	{
		// Token: 0x0600561E RID: 22046 RVA: 0x001A3209 File Offset: 0x001A1409
		public ProcessVoiceDataToLoudness(VoiceToLoudness voiceToLoudness)
		{
			this._voiceToLoudness = voiceToLoudness;
		}

		// Token: 0x0600561F RID: 22047 RVA: 0x001A3218 File Offset: 0x001A1418
		public float[] Process(float[] buf)
		{
			float num = 0f;
			for (int i = 0; i < buf.Length; i++)
			{
				num += Mathf.Abs(buf[i]);
			}
			this._voiceToLoudness.loudness = num / (float)buf.Length;
			return buf;
		}

		// Token: 0x06005620 RID: 22048 RVA: 0x000023F4 File Offset: 0x000005F4
		public void Dispose()
		{
		}

		// Token: 0x040059EE RID: 23022
		private VoiceToLoudness _voiceToLoudness;
	}
}
