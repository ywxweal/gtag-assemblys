using System;
using UnityEngine;

namespace GorillaTag.Audio
{
	// Token: 0x02000D94 RID: 3476
	public class LoudSpeakerActivator : MonoBehaviour
	{
		// Token: 0x06005647 RID: 22087 RVA: 0x001A40F7 File Offset: 0x001A22F7
		private void Awake()
		{
			this._isLocal = this.IsParentedToLocalRig();
			if (!this._isLocal)
			{
				this._nonlocalRig = base.transform.root.GetComponent<VRRig>();
			}
		}

		// Token: 0x06005648 RID: 22088 RVA: 0x001A4124 File Offset: 0x001A2324
		private bool IsParentedToLocalRig()
		{
			if (VRRigCache.Instance.localRig == null)
			{
				return false;
			}
			Transform transform = base.transform.parent;
			while (transform != null)
			{
				if (transform == VRRigCache.Instance.localRig.transform)
				{
					return true;
				}
				transform = transform.parent;
			}
			return false;
		}

		// Token: 0x06005649 RID: 22089 RVA: 0x001A417D File Offset: 0x001A237D
		public void SetRecorder(GTRecorder recorder)
		{
			this._recorder = recorder;
		}

		// Token: 0x0600564A RID: 22090 RVA: 0x001A4188 File Offset: 0x001A2388
		public void StartLocalBroadcast()
		{
			if (!this._isLocal)
			{
				if (this._network != null && this._nonlocalRig != null)
				{
					this._network.StartBroadcastSpeakerOutput(this._nonlocalRig);
				}
				return;
			}
			if (this.IsBroadcasting)
			{
				return;
			}
			if (this._recorder == null && NetworkSystem.Instance.LocalRecorder != null)
			{
				this.SetRecorder((GTRecorder)NetworkSystem.Instance.LocalRecorder);
			}
			if (this._recorder != null && this._network != null)
			{
				this.IsBroadcasting = true;
				this._recorder.AllowPitchAdjustment = true;
				this._recorder.PitchAdjustment = this.PitchAdjustment;
				this._recorder.AllowVolumeAdjustment = true;
				this._recorder.VolumeAdjustment = this.VolumeAdjustment;
				this._network.StartBroadcastSpeakerOutput(VRRigCache.Instance.localRig.Rig);
			}
		}

		// Token: 0x0600564B RID: 22091 RVA: 0x001A4280 File Offset: 0x001A2480
		public void StopLocalBroadcast()
		{
			if (!this._isLocal)
			{
				if (this._network != null && this._nonlocalRig != null)
				{
					this._network.StopBroadcastSpeakerOutput(this._nonlocalRig);
				}
				return;
			}
			if (!this.IsBroadcasting)
			{
				return;
			}
			if (this._recorder == null && NetworkSystem.Instance.LocalRecorder != null)
			{
				this.SetRecorder((GTRecorder)NetworkSystem.Instance.LocalRecorder);
			}
			if (this._recorder != null && this._network != null)
			{
				this.IsBroadcasting = false;
				this._recorder.AllowPitchAdjustment = false;
				this._recorder.PitchAdjustment = 1f;
				this._recorder.AllowVolumeAdjustment = false;
				this._recorder.VolumeAdjustment = 1f;
				this._network.StopBroadcastSpeakerOutput(VRRigCache.Instance.localRig.Rig);
			}
		}

		// Token: 0x04005A17 RID: 23063
		public float PitchAdjustment = 1f;

		// Token: 0x04005A18 RID: 23064
		public float VolumeAdjustment = 2.5f;

		// Token: 0x04005A19 RID: 23065
		public bool IsBroadcasting;

		// Token: 0x04005A1A RID: 23066
		[SerializeField]
		private LoudSpeakerNetwork _network;

		// Token: 0x04005A1B RID: 23067
		[SerializeField]
		private GTRecorder _recorder;

		// Token: 0x04005A1C RID: 23068
		private bool _isLocal;

		// Token: 0x04005A1D RID: 23069
		private VRRig _nonlocalRig;
	}
}
