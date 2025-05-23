using System;
using UnityEngine;

// Token: 0x0200069F RID: 1695
public class PitchShiftAudioPlayer : MonoBehaviour
{
	// Token: 0x06002A6E RID: 10862 RVA: 0x000D1262 File Offset: 0x000CF462
	private void Awake()
	{
		if (this._source == null)
		{
			this._source = base.GetComponent<AudioSource>();
		}
		if (this._pitch == null)
		{
			this._pitch = base.GetComponent<RangedFloat>();
		}
	}

	// Token: 0x06002A6F RID: 10863 RVA: 0x000D1298 File Offset: 0x000CF498
	private void OnEnable()
	{
		this._pitchMixVars.Rent(out this._pitchMix);
		this._source.outputAudioMixerGroup = this._pitchMix.group;
	}

	// Token: 0x06002A70 RID: 10864 RVA: 0x000D12C2 File Offset: 0x000CF4C2
	private void OnDisable()
	{
		this._source.Stop();
		this._source.outputAudioMixerGroup = null;
		AudioMixVar pitchMix = this._pitchMix;
		if (pitchMix == null)
		{
			return;
		}
		pitchMix.ReturnToPool();
	}

	// Token: 0x06002A71 RID: 10865 RVA: 0x000D12EB File Offset: 0x000CF4EB
	private void Update()
	{
		if (this.apply)
		{
			this.ApplyPitch();
		}
	}

	// Token: 0x06002A72 RID: 10866 RVA: 0x000D12FB File Offset: 0x000CF4FB
	private void ApplyPitch()
	{
		this._pitchMix.value = this._pitch.curved;
	}

	// Token: 0x04002F53 RID: 12115
	public bool apply = true;

	// Token: 0x04002F54 RID: 12116
	[SerializeField]
	private AudioSource _source;

	// Token: 0x04002F55 RID: 12117
	[SerializeField]
	private AudioMixVarPool _pitchMixVars;

	// Token: 0x04002F56 RID: 12118
	[SerializeReference]
	private AudioMixVar _pitchMix;

	// Token: 0x04002F57 RID: 12119
	[SerializeField]
	private RangedFloat _pitch;
}
