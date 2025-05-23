using System;
using UnityEngine;

// Token: 0x0200015C RID: 348
public class AudioAnimator : MonoBehaviour
{
	// Token: 0x060008DC RID: 2268 RVA: 0x0002FF3E File Offset: 0x0002E13E
	private void Start()
	{
		if (!this.didInitBaseVolume)
		{
			this.InitBaseVolume();
		}
	}

	// Token: 0x060008DD RID: 2269 RVA: 0x0002FF50 File Offset: 0x0002E150
	private void InitBaseVolume()
	{
		for (int i = 0; i < this.targets.Length; i++)
		{
			this.targets[i].baseVolume = this.targets[i].audioSource.volume;
		}
		this.didInitBaseVolume = true;
	}

	// Token: 0x060008DE RID: 2270 RVA: 0x0002FF9E File Offset: 0x0002E19E
	public void UpdateValue(float value, bool ignoreSmoothing = false)
	{
		this.UpdatePitchAndVolume(value, value, ignoreSmoothing);
	}

	// Token: 0x060008DF RID: 2271 RVA: 0x0002FFAC File Offset: 0x0002E1AC
	public void UpdatePitchAndVolume(float pitchValue, float volumeValue, bool ignoreSmoothing = false)
	{
		if (!this.didInitBaseVolume)
		{
			this.InitBaseVolume();
		}
		for (int i = 0; i < this.targets.Length; i++)
		{
			AudioAnimator.AudioTarget audioTarget = this.targets[i];
			float num = audioTarget.pitchCurve.Evaluate(pitchValue);
			float num2 = Mathf.Pow(1.05946f, num);
			audioTarget.audioSource.pitch = num2;
			float num3 = audioTarget.volumeCurve.Evaluate(volumeValue);
			float volume = audioTarget.audioSource.volume;
			float num4 = audioTarget.baseVolume * num3;
			if (ignoreSmoothing)
			{
				audioTarget.audioSource.volume = num4;
			}
			else if (volume > num4)
			{
				audioTarget.audioSource.volume = Mathf.MoveTowards(audioTarget.audioSource.volume, audioTarget.baseVolume * num3, (1f - audioTarget.lowerSmoothing) * audioTarget.baseVolume * Time.deltaTime * 90f);
			}
			else
			{
				audioTarget.audioSource.volume = Mathf.MoveTowards(audioTarget.audioSource.volume, audioTarget.baseVolume * num3, (1f - audioTarget.riseSmoothing) * audioTarget.baseVolume * Time.deltaTime * 90f);
			}
		}
	}

	// Token: 0x04000A6C RID: 2668
	private bool didInitBaseVolume;

	// Token: 0x04000A6D RID: 2669
	[SerializeField]
	private AudioAnimator.AudioTarget[] targets;

	// Token: 0x0200015D RID: 349
	[Serializable]
	private struct AudioTarget
	{
		// Token: 0x04000A6E RID: 2670
		public AudioSource audioSource;

		// Token: 0x04000A6F RID: 2671
		public AnimationCurve pitchCurve;

		// Token: 0x04000A70 RID: 2672
		public AnimationCurve volumeCurve;

		// Token: 0x04000A71 RID: 2673
		[NonSerialized]
		public float baseVolume;

		// Token: 0x04000A72 RID: 2674
		public float riseSmoothing;

		// Token: 0x04000A73 RID: 2675
		public float lowerSmoothing;
	}
}
