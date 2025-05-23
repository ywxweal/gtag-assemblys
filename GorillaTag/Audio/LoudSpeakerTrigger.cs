using System;
using UnityEngine;

namespace GorillaTag.Audio
{
	// Token: 0x02000D96 RID: 3478
	public class LoudSpeakerTrigger : MonoBehaviour
	{
		// Token: 0x0600565A RID: 22106 RVA: 0x001A4752 File Offset: 0x001A2952
		public void SetRecorder(GTRecorder recorder)
		{
			this._recorder = recorder;
		}

		// Token: 0x0600565B RID: 22107 RVA: 0x001A475C File Offset: 0x001A295C
		public void OnPlayerEnter(VRRig player)
		{
			if (this._recorder != null && this._network != null)
			{
				this._recorder.AllowPitchAdjustment = true;
				this._recorder.PitchAdjustment = this.PitchAdjustment;
				this._network.StartBroadcastSpeakerOutput(player);
			}
		}

		// Token: 0x0600565C RID: 22108 RVA: 0x001A47B0 File Offset: 0x001A29B0
		public void OnPlayerExit(VRRig player)
		{
			if (this._recorder != null && this._network != null)
			{
				this._recorder.AllowPitchAdjustment = false;
				this._recorder.PitchAdjustment = 1f;
				this._network.StopBroadcastSpeakerOutput(player);
			}
		}

		// Token: 0x04005A24 RID: 23076
		public float PitchAdjustment = 1f;

		// Token: 0x04005A25 RID: 23077
		[SerializeField]
		private LoudSpeakerNetwork _network;

		// Token: 0x04005A26 RID: 23078
		[SerializeField]
		private GTRecorder _recorder;
	}
}
