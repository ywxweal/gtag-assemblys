using System;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x020006A0 RID: 1696
public class PlayerAudioManager : MonoBehaviour
{
	// Token: 0x06002A75 RID: 10869 RVA: 0x000D13C6 File Offset: 0x000CF5C6
	public void SetMixerSnapshot(AudioMixerSnapshot snapshot, float transitionTime = 0.1f)
	{
		snapshot.TransitionTo(transitionTime);
	}

	// Token: 0x06002A76 RID: 10870 RVA: 0x000D13CF File Offset: 0x000CF5CF
	public void UnsetMixerSnapshot(float transitionTime = 0.1f)
	{
		this.defaultSnapshot.TransitionTo(transitionTime);
	}

	// Token: 0x04002F5A RID: 12122
	public AudioMixerSnapshot defaultSnapshot;

	// Token: 0x04002F5B RID: 12123
	public AudioMixerSnapshot underwaterSnapshot;
}
