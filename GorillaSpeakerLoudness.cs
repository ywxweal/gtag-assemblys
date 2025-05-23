using System;
using GorillaNetworking;
using GorillaTag;
using GorillaTag.Audio;
using Oculus.VoiceSDK.Utilities;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine;

// Token: 0x02000638 RID: 1592
public class GorillaSpeakerLoudness : MonoBehaviour, IGorillaSliceableSimple, IDynamicFloat
{
	// Token: 0x170003C5 RID: 965
	// (get) Token: 0x060027A8 RID: 10152 RVA: 0x000C48CB File Offset: 0x000C2ACB
	public bool IsSpeaking
	{
		get
		{
			return this.isSpeaking;
		}
	}

	// Token: 0x170003C6 RID: 966
	// (get) Token: 0x060027A9 RID: 10153 RVA: 0x000C48D3 File Offset: 0x000C2AD3
	public float Loudness
	{
		get
		{
			return this.loudness;
		}
	}

	// Token: 0x170003C7 RID: 967
	// (get) Token: 0x060027AA RID: 10154 RVA: 0x000C48DB File Offset: 0x000C2ADB
	public float LoudnessNormalized
	{
		get
		{
			return Mathf.Min(this.loudness / this.normalizedMax, 1f);
		}
	}

	// Token: 0x170003C8 RID: 968
	// (get) Token: 0x060027AB RID: 10155 RVA: 0x000C48F4 File Offset: 0x000C2AF4
	public float floatValue
	{
		get
		{
			return this.LoudnessNormalized;
		}
	}

	// Token: 0x170003C9 RID: 969
	// (get) Token: 0x060027AC RID: 10156 RVA: 0x000C48FC File Offset: 0x000C2AFC
	public bool IsMicEnabled
	{
		get
		{
			return this.isMicEnabled;
		}
	}

	// Token: 0x170003CA RID: 970
	// (get) Token: 0x060027AD RID: 10157 RVA: 0x000C4904 File Offset: 0x000C2B04
	public float SmoothedLoudness
	{
		get
		{
			return this.smoothedLoudness;
		}
	}

	// Token: 0x060027AE RID: 10158 RVA: 0x000C490C File Offset: 0x000C2B0C
	private void Start()
	{
		this.rigContainer = base.GetComponent<RigContainer>();
		this.timeLastUpdated = Time.time;
		this.deltaTime = Time.deltaTime;
	}

	// Token: 0x060027AF RID: 10159 RVA: 0x00010F2B File Offset: 0x0000F12B
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x060027B0 RID: 10160 RVA: 0x00010F34 File Offset: 0x0000F134
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x060027B1 RID: 10161 RVA: 0x000C4930 File Offset: 0x000C2B30
	public void SliceUpdate()
	{
		this.deltaTime = Time.time - this.timeLastUpdated;
		this.timeLastUpdated = Time.time;
		this.UpdateMicEnabled();
		this.UpdateLoudness();
		this.UpdateSmoothedLoudness();
	}

	// Token: 0x060027B2 RID: 10162 RVA: 0x000C4964 File Offset: 0x000C2B64
	private void UpdateMicEnabled()
	{
		if (this.rigContainer == null)
		{
			return;
		}
		VRRig rig = this.rigContainer.Rig;
		if (rig.isOfflineVRRig)
		{
			this.permission = this.permission || MicPermissionsManager.HasMicPermission();
			if (this.permission && !this.micConnected && Microphone.devices != null)
			{
				this.micConnected = Microphone.devices.Length != 0;
			}
			this.isMicEnabled = this.permission && this.micConnected;
			rig.IsMicEnabled = this.isMicEnabled;
			return;
		}
		this.isMicEnabled = rig.IsMicEnabled;
	}

	// Token: 0x060027B3 RID: 10163 RVA: 0x000C4A00 File Offset: 0x000C2C00
	private void UpdateLoudness()
	{
		if (this.rigContainer == null)
		{
			return;
		}
		PhotonVoiceView voice = this.rigContainer.Voice;
		if (voice != null && this.speaker == null)
		{
			this.speaker = voice.SpeakerInUse;
		}
		if (this.recorder == null)
		{
			this.recorder = ((voice != null) ? voice.RecorderInUse : null);
		}
		VRRig rig = this.rigContainer.Rig;
		if ((rig.remoteUseReplacementVoice || rig.localUseReplacementVoice || GorillaComputer.instance.voiceChatOn == "FALSE") && rig.SpeakingLoudness > 0f && !this.rigContainer.ForceMute && !this.rigContainer.Muted)
		{
			this.isSpeaking = true;
			this.loudness = rig.SpeakingLoudness;
			return;
		}
		if (voice != null && voice.IsSpeaking)
		{
			this.isSpeaking = true;
			if (!(this.speaker != null))
			{
				this.loudness = 0f;
				return;
			}
			if (this.speakerVoiceToLoudness == null)
			{
				this.speakerVoiceToLoudness = this.speaker.GetComponent<SpeakerVoiceToLoudness>();
			}
			if (this.speakerVoiceToLoudness != null)
			{
				this.loudness = this.speakerVoiceToLoudness.loudness;
				return;
			}
		}
		else if (voice != null && this.recorder != null && NetworkSystem.Instance.IsObjectLocallyOwned(voice.gameObject) && this.recorder.IsCurrentlyTransmitting)
		{
			if (this.voiceToLoudness == null)
			{
				this.voiceToLoudness = this.recorder.GetComponent<VoiceToLoudness>();
			}
			this.isSpeaking = true;
			if (this.voiceToLoudness != null)
			{
				this.loudness = this.voiceToLoudness.loudness;
				return;
			}
			this.loudness = 0f;
			return;
		}
		else
		{
			this.isSpeaking = false;
			this.loudness = 0f;
		}
	}

	// Token: 0x060027B4 RID: 10164 RVA: 0x000C4BE8 File Offset: 0x000C2DE8
	private void UpdateSmoothedLoudness()
	{
		if (!this.isSpeaking)
		{
			this.smoothedLoudness = 0f;
			return;
		}
		if (!Mathf.Approximately(this.loudness, this.lastLoudness))
		{
			this.timeSinceLoudnessChange = 0f;
			this.smoothedLoudness = Mathf.Lerp(this.smoothedLoudness, this.loudness, Mathf.Clamp01(this.loudnessBlendStrength * this.deltaTime));
			this.lastLoudness = this.loudness;
			return;
		}
		if (this.timeSinceLoudnessChange > this.loudnessUpdateCheckRate)
		{
			this.smoothedLoudness = 0.001f;
			return;
		}
		this.smoothedLoudness = Mathf.Lerp(this.smoothedLoudness, this.loudness, Mathf.Clamp01(this.loudnessBlendStrength * this.deltaTime));
		this.timeSinceLoudnessChange += this.deltaTime;
	}

	// Token: 0x060027B6 RID: 10166 RVA: 0x00011040 File Offset: 0x0000F240
	bool IGorillaSliceableSimple.get_isActiveAndEnabled()
	{
		return base.isActiveAndEnabled;
	}

	// Token: 0x04002C2C RID: 11308
	private bool isSpeaking;

	// Token: 0x04002C2D RID: 11309
	private float loudness;

	// Token: 0x04002C2E RID: 11310
	[SerializeField]
	private float normalizedMax = 0.175f;

	// Token: 0x04002C2F RID: 11311
	private bool isMicEnabled;

	// Token: 0x04002C30 RID: 11312
	private RigContainer rigContainer;

	// Token: 0x04002C31 RID: 11313
	private Speaker speaker;

	// Token: 0x04002C32 RID: 11314
	private SpeakerVoiceToLoudness speakerVoiceToLoudness;

	// Token: 0x04002C33 RID: 11315
	private Recorder recorder;

	// Token: 0x04002C34 RID: 11316
	private VoiceToLoudness voiceToLoudness;

	// Token: 0x04002C35 RID: 11317
	private float smoothedLoudness;

	// Token: 0x04002C36 RID: 11318
	private float lastLoudness;

	// Token: 0x04002C37 RID: 11319
	private float timeSinceLoudnessChange;

	// Token: 0x04002C38 RID: 11320
	private float loudnessUpdateCheckRate = 0.2f;

	// Token: 0x04002C39 RID: 11321
	private float loudnessBlendStrength = 2f;

	// Token: 0x04002C3A RID: 11322
	private bool permission;

	// Token: 0x04002C3B RID: 11323
	private bool micConnected;

	// Token: 0x04002C3C RID: 11324
	private float timeLastUpdated;

	// Token: 0x04002C3D RID: 11325
	private float deltaTime;
}
