using System;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x020006A0 RID: 1696
public class PlayerAudioManager : MonoBehaviour
{
	// Token: 0x06002A74 RID: 10868 RVA: 0x000D1322 File Offset: 0x000CF522
	public void SetMixerSnapshot(AudioMixerSnapshot snapshot, float transitionTime = 0.1f)
	{
		snapshot.TransitionTo(transitionTime);
	}

	// Token: 0x06002A75 RID: 10869 RVA: 0x000D132B File Offset: 0x000CF52B
	public void UnsetMixerSnapshot(float transitionTime = 0.1f)
	{
		this.defaultSnapshot.TransitionTo(transitionTime);
	}

	// Token: 0x04002F58 RID: 12120
	public AudioMixerSnapshot defaultSnapshot;

	// Token: 0x04002F59 RID: 12121
	public AudioMixerSnapshot underwaterSnapshot;
}
