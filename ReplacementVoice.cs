using System;
using GorillaTag.Cosmetics;
using UnityEngine;

// Token: 0x020006A7 RID: 1703
public class ReplacementVoice : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06002A9B RID: 10907 RVA: 0x00010F2B File Offset: 0x0000F12B
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x06002A9C RID: 10908 RVA: 0x00010F34 File Offset: 0x0000F134
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x06002A9D RID: 10909 RVA: 0x000D1AEC File Offset: 0x000CFCEC
	public void SliceUpdate()
	{
		if (!this.replacementVoiceSource.isPlaying && this.myVRRig.ShouldPlayReplacementVoice())
		{
			if (!Mathf.Approximately(this.myVRRig.voiceAudio.pitch, this.replacementVoiceSource.pitch))
			{
				this.replacementVoiceSource.pitch = this.myVRRig.voiceAudio.pitch;
			}
			if (this.myVRRig.SpeakingLoudness < this.loudReplacementVoiceThreshold)
			{
				this.replacementVoiceSource.clip = this.replacementVoiceClips[Random.Range(0, this.replacementVoiceClips.Length - 1)];
				this.replacementVoiceSource.volume = this.normalVolume;
			}
			else
			{
				this.replacementVoiceSource.clip = this.replacementVoiceClipsLoud[Random.Range(0, this.replacementVoiceClipsLoud.Length - 1)];
				this.replacementVoiceSource.volume = this.loudVolume;
			}
			this.replacementVoiceSource.GTPlay();
			return;
		}
		CosmeticEffectsOnPlayers.CosmeticEffect cosmeticEffect;
		if (!this.replacementVoiceSource.isPlaying && this.myVRRig.TryGetCosmeticVoiceOverride(CosmeticEffectsOnPlayers.EFFECTTYPE.VOICEOVERRIDE, out cosmeticEffect))
		{
			if (this.myVRRig.SpeakingLoudness < this.myVRRig.replacementVoiceLoudnessThreshold)
			{
				return;
			}
			if (!Mathf.Approximately(this.myVRRig.voiceAudio.pitch, this.replacementVoiceSource.pitch))
			{
				this.replacementVoiceSource.pitch = this.myVRRig.voiceAudio.pitch;
			}
			if (this.myVRRig.SpeakingLoudness < cosmeticEffect.voiceOverrideLoudThreshold)
			{
				this.replacementVoiceSource.clip = cosmeticEffect.voiceOverrideNormalClips[Random.Range(0, cosmeticEffect.voiceOverrideNormalClips.Length - 1)];
				this.replacementVoiceSource.volume = cosmeticEffect.voiceOverrideNormalVolume;
			}
			else
			{
				this.replacementVoiceSource.clip = cosmeticEffect.voiceOverrideLoudClips[Random.Range(0, cosmeticEffect.voiceOverrideLoudClips.Length - 1)];
				this.replacementVoiceSource.volume = cosmeticEffect.voiceOverrideLoudVolume;
			}
			this.replacementVoiceSource.GTPlay();
		}
	}

	// Token: 0x06002A9F RID: 10911 RVA: 0x00011040 File Offset: 0x0000F240
	bool IGorillaSliceableSimple.get_isActiveAndEnabled()
	{
		return base.isActiveAndEnabled;
	}

	// Token: 0x04002F7A RID: 12154
	public AudioSource replacementVoiceSource;

	// Token: 0x04002F7B RID: 12155
	public AudioClip[] replacementVoiceClips;

	// Token: 0x04002F7C RID: 12156
	public AudioClip[] replacementVoiceClipsLoud;

	// Token: 0x04002F7D RID: 12157
	public float loudReplacementVoiceThreshold = 0.1f;

	// Token: 0x04002F7E RID: 12158
	public VRRig myVRRig;

	// Token: 0x04002F7F RID: 12159
	public float normalVolume = 0.5f;

	// Token: 0x04002F80 RID: 12160
	public float loudVolume = 0.8f;
}
