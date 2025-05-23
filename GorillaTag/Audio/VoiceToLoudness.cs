using System;
using Photon.Voice;
using Photon.Voice.Unity;
using UnityEngine;

namespace GorillaTag.Audio
{
	// Token: 0x02000D8C RID: 3468
	[RequireComponent(typeof(Recorder))]
	public class VoiceToLoudness : MonoBehaviour
	{
		// Token: 0x0600561B RID: 22043 RVA: 0x001A31BD File Offset: 0x001A13BD
		protected void Awake()
		{
			this._recorder = base.GetComponent<Recorder>();
		}

		// Token: 0x0600561C RID: 22044 RVA: 0x001A31CC File Offset: 0x001A13CC
		protected void PhotonVoiceCreated(PhotonVoiceCreatedParams photonVoiceCreatedParams)
		{
			VoiceInfo info = photonVoiceCreatedParams.Voice.Info;
			LocalVoiceAudioFloat localVoiceAudioFloat = photonVoiceCreatedParams.Voice as LocalVoiceAudioFloat;
			if (localVoiceAudioFloat != null)
			{
				localVoiceAudioFloat.AddPostProcessor(new IProcessor<float>[]
				{
					new ProcessVoiceDataToLoudness(this)
				});
			}
		}

		// Token: 0x040059EC RID: 23020
		[NonSerialized]
		public float loudness;

		// Token: 0x040059ED RID: 23021
		private Recorder _recorder;
	}
}
