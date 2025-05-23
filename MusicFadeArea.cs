using System;
using System.Collections;
using System.Collections.Generic;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000696 RID: 1686
public class MusicFadeArea : MonoBehaviour
{
	// Token: 0x06002A37 RID: 10807 RVA: 0x000D08C8 File Offset: 0x000CEAC8
	private void Awake()
	{
		for (int i = 0; i < this.sourcesToFadeIn.Count; i++)
		{
			this.sourcesToFadeIn[i].audioSource.Stop();
			this.sourcesToFadeIn[i].audioSource.volume = 0f;
		}
	}

	// Token: 0x06002A38 RID: 10808 RVA: 0x000D091C File Offset: 0x000CEB1C
	private void OnTriggerEnter(Collider other)
	{
		if (other == GTPlayer.Instance.headCollider)
		{
			MusicManager.Instance.FadeOutMusic(this.fadeDuration);
			if (this.fadeCoroutine != null)
			{
				base.StopCoroutine(this.fadeCoroutine);
			}
			if (this.sourcesToFadeIn.Count > 0)
			{
				this.fadeCoroutine = base.StartCoroutine(this.FadeInSources());
			}
		}
	}

	// Token: 0x06002A39 RID: 10809 RVA: 0x000D0984 File Offset: 0x000CEB84
	private void OnTriggerExit(Collider other)
	{
		if (other == GTPlayer.Instance.headCollider)
		{
			MusicManager.Instance.FadeInMusic(this.fadeDuration);
			if (this.fadeCoroutine != null)
			{
				base.StopCoroutine(this.fadeCoroutine);
			}
			if (this.sourcesToFadeIn.Count > 0)
			{
				this.fadeCoroutine = base.StartCoroutine(this.FadeOutSources());
			}
		}
	}

	// Token: 0x06002A3A RID: 10810 RVA: 0x000D09E9 File Offset: 0x000CEBE9
	private IEnumerator FadeInSources()
	{
		for (int i = 0; i < this.sourcesToFadeIn.Count; i++)
		{
			this.sourcesToFadeIn[i].audioSource.Play();
			this.sourcesToFadeIn[i].audioSource.volume = this.sourcesToFadeIn[i].maxVolume * this.fadeProgress;
		}
		while (this.fadeProgress < 1f)
		{
			for (int j = 0; j < this.sourcesToFadeIn.Count; j++)
			{
				this.sourcesToFadeIn[j].audioSource.volume = this.sourcesToFadeIn[j].maxVolume * this.fadeProgress;
			}
			yield return null;
			this.fadeProgress = Mathf.MoveTowards(this.fadeProgress, 1f, Time.deltaTime / this.fadeDuration);
		}
		for (int k = 0; k < this.sourcesToFadeIn.Count; k++)
		{
			this.sourcesToFadeIn[k].audioSource.volume = this.sourcesToFadeIn[k].maxVolume;
		}
		yield break;
	}

	// Token: 0x06002A3B RID: 10811 RVA: 0x000D09F8 File Offset: 0x000CEBF8
	private IEnumerator FadeOutSources()
	{
		for (int i = 0; i < this.sourcesToFadeIn.Count; i++)
		{
			this.sourcesToFadeIn[i].audioSource.volume = this.sourcesToFadeIn[i].maxVolume * this.fadeProgress;
		}
		while (this.fadeProgress > 0f)
		{
			for (int j = 0; j < this.sourcesToFadeIn.Count; j++)
			{
				this.sourcesToFadeIn[j].audioSource.volume = this.sourcesToFadeIn[j].maxVolume * this.fadeProgress;
			}
			yield return null;
			this.fadeProgress = Mathf.MoveTowards(this.fadeProgress, 0f, Time.deltaTime / this.fadeDuration);
		}
		for (int k = 0; k < this.sourcesToFadeIn.Count; k++)
		{
			this.sourcesToFadeIn[k].audioSource.Stop();
			this.sourcesToFadeIn[k].audioSource.volume = 0f;
		}
		yield break;
	}

	// Token: 0x04002F36 RID: 12086
	[SerializeField]
	private List<MusicFadeArea.AudioSourceEntry> sourcesToFadeIn = new List<MusicFadeArea.AudioSourceEntry>();

	// Token: 0x04002F37 RID: 12087
	[SerializeField]
	private float fadeDuration = 3f;

	// Token: 0x04002F38 RID: 12088
	private float fadeProgress;

	// Token: 0x04002F39 RID: 12089
	private Coroutine fadeCoroutine;

	// Token: 0x02000697 RID: 1687
	[Serializable]
	public struct AudioSourceEntry
	{
		// Token: 0x04002F3A RID: 12090
		public AudioSource audioSource;

		// Token: 0x04002F3B RID: 12091
		public float maxVolume;
	}
}
