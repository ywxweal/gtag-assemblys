using System;
using System.Collections.Generic;
using Photon.Voice.Unity;
using UnityEngine;

namespace GorillaTag.Audio
{
	// Token: 0x02000D95 RID: 3477
	public class LoudSpeakerNetwork : MonoBehaviour
	{
		// Token: 0x170008A4 RID: 2212
		// (get) Token: 0x0600564D RID: 22093 RVA: 0x001A4394 File Offset: 0x001A2594
		public AudioSource[] SpeakerSources
		{
			get
			{
				return this._speakerSources;
			}
		}

		// Token: 0x0600564E RID: 22094 RVA: 0x001A439C File Offset: 0x001A259C
		private void Awake()
		{
			if (this._speakerSources == null || this._speakerSources.Length == 0)
			{
				this._speakerSources = base.transform.GetComponentsInChildren<AudioSource>();
			}
			this._currentSpeakers = new List<Speaker>();
		}

		// Token: 0x0600564F RID: 22095 RVA: 0x001A43CC File Offset: 0x001A25CC
		private void Start()
		{
			RigContainer rigContainer;
			if (this.GetParentRigContainer(out rigContainer) && rigContainer.Voice != null)
			{
				GTSpeaker gtspeaker = (GTSpeaker)rigContainer.Voice.SpeakerInUse;
				if (gtspeaker != null)
				{
					gtspeaker.AddExternalAudioSources(this._speakerSources);
				}
			}
		}

		// Token: 0x06005650 RID: 22096 RVA: 0x001A4417 File Offset: 0x001A2617
		private bool GetParentRigContainer(out RigContainer rigContainer)
		{
			if (this._rigContainer == null)
			{
				this._rigContainer = base.transform.GetComponentInParent<RigContainer>();
			}
			rigContainer = this._rigContainer;
			return rigContainer != null;
		}

		// Token: 0x06005651 RID: 22097 RVA: 0x001A4448 File Offset: 0x001A2648
		private void OnEnable()
		{
			RigContainer rigContainer;
			if (this.GetParentRigContainer(out rigContainer))
			{
				rigContainer.AddLoudSpeakerNetwork(this);
			}
		}

		// Token: 0x06005652 RID: 22098 RVA: 0x001A4468 File Offset: 0x001A2668
		private void OnDisable()
		{
			RigContainer rigContainer;
			if (this.GetParentRigContainer(out rigContainer))
			{
				rigContainer.RemoveLoudSpeakerNetwork(this);
			}
		}

		// Token: 0x06005653 RID: 22099 RVA: 0x001A4486 File Offset: 0x001A2686
		public void AddSpeaker(Speaker speaker)
		{
			if (this._currentSpeakers.Contains(speaker))
			{
				return;
			}
			this._currentSpeakers.Add(speaker);
		}

		// Token: 0x06005654 RID: 22100 RVA: 0x001A44A3 File Offset: 0x001A26A3
		public void RemoveSpeaker(Speaker speaker)
		{
			this._currentSpeakers.Remove(speaker);
		}

		// Token: 0x06005655 RID: 22101 RVA: 0x001A44B2 File Offset: 0x001A26B2
		public void StartBroadcastSpeakerOutput(VRRig player)
		{
			GorillaTagger.Instance.rigSerializer.BroadcastLoudSpeakerNetwork(true, player.OwningNetPlayer.ActorNumber);
		}

		// Token: 0x06005656 RID: 22102 RVA: 0x001A44D0 File Offset: 0x001A26D0
		public void BroadcastLoudSpeakerNetwork(int actorNumber, bool isLocal = false)
		{
			if (isLocal)
			{
				if (this._localRecorder == null)
				{
					this._localRecorder = (GTRecorder)NetworkSystem.Instance.LocalRecorder;
				}
				if (this._localRecorder != null)
				{
					this._localRecorder.DebugEchoMode = true;
					if (this.ReparentLocalSpeaker)
					{
						Transform transform = this._rigContainer.Voice.SpeakerInUse.transform;
						transform.transform.SetParent(base.transform, false);
						transform.localPosition = Vector3.zero;
					}
				}
				return;
			}
			using (List<Speaker>.Enumerator enumerator = this._currentSpeakers.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					GTSpeaker gtspeaker = (GTSpeaker)enumerator.Current;
					gtspeaker.ToggleAudioSource(true);
					gtspeaker.BroadcastExternal = true;
					RigContainer rigContainer;
					if (VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(actorNumber), out rigContainer))
					{
						Transform transform2 = rigContainer.Voice.SpeakerInUse.transform;
						transform2.SetParent(base.transform, false);
						transform2.localPosition = Vector3.zero;
					}
				}
			}
			this._currentSpeakerActor = actorNumber;
		}

		// Token: 0x06005657 RID: 22103 RVA: 0x001A45F0 File Offset: 0x001A27F0
		public void StopBroadcastSpeakerOutput(VRRig player)
		{
			GorillaTagger.Instance.rigSerializer.BroadcastLoudSpeakerNetwork(false, player.OwningNetPlayer.ActorNumber);
		}

		// Token: 0x06005658 RID: 22104 RVA: 0x001A4610 File Offset: 0x001A2810
		public void StopBroadcastLoudSpeakerNetwork(int actorNumber, bool isLocal = false)
		{
			if (isLocal)
			{
				if (this._localRecorder == null)
				{
					this._localRecorder = (GTRecorder)NetworkSystem.Instance.LocalRecorder;
				}
				if (this._localRecorder != null)
				{
					this._localRecorder.DebugEchoMode = false;
					RigContainer rigContainer;
					if (this.ReparentLocalSpeaker && this.GetParentRigContainer(out rigContainer))
					{
						Transform transform = rigContainer.Voice.SpeakerInUse.transform;
						transform.SetParent(rigContainer.SpeakerHead, false);
						transform.localPosition = Vector3.zero;
					}
				}
				return;
			}
			if (actorNumber == this._currentSpeakerActor)
			{
				using (List<Speaker>.Enumerator enumerator = this._currentSpeakers.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						GTSpeaker gtspeaker = (GTSpeaker)enumerator.Current;
						gtspeaker.ToggleAudioSource(false);
						gtspeaker.BroadcastExternal = false;
						RigContainer rigContainer2;
						if (VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(actorNumber), out rigContainer2))
						{
							Transform transform2 = rigContainer2.Voice.SpeakerInUse.transform;
							transform2.SetParent(rigContainer2.SpeakerHead, false);
							transform2.localPosition = Vector3.zero;
						}
					}
				}
				this._currentSpeakerActor = -1;
			}
		}

		// Token: 0x04005A1E RID: 23070
		[SerializeField]
		private AudioSource[] _speakerSources;

		// Token: 0x04005A1F RID: 23071
		[SerializeField]
		private List<Speaker> _currentSpeakers;

		// Token: 0x04005A20 RID: 23072
		[SerializeField]
		private int _currentSpeakerActor = -1;

		// Token: 0x04005A21 RID: 23073
		public bool ReparentLocalSpeaker = true;

		// Token: 0x04005A22 RID: 23074
		private RigContainer _rigContainer;

		// Token: 0x04005A23 RID: 23075
		private GTRecorder _localRecorder;
	}
}
