using System;
using UnityEngine;

// Token: 0x02000968 RID: 2408
public class AmbientSoundRandomizer : MonoBehaviour
{
	// Token: 0x06003A17 RID: 14871 RVA: 0x00116D11 File Offset: 0x00114F11
	private void Button_Cache()
	{
		this.audioSources = base.GetComponentsInChildren<AudioSource>();
	}

	// Token: 0x06003A18 RID: 14872 RVA: 0x00116D1F File Offset: 0x00114F1F
	private void Awake()
	{
		this.SetTarget();
	}

	// Token: 0x06003A19 RID: 14873 RVA: 0x00116D28 File Offset: 0x00114F28
	private void Update()
	{
		if (this.timer >= this.timerTarget)
		{
			int num = Random.Range(0, this.audioSources.Length);
			int num2 = Random.Range(0, this.audioClips.Length);
			this.audioSources[num].clip = this.audioClips[num2];
			this.audioSources[num].GTPlay();
			this.SetTarget();
			return;
		}
		this.timer += Time.deltaTime;
	}

	// Token: 0x06003A1A RID: 14874 RVA: 0x00116D9C File Offset: 0x00114F9C
	private void SetTarget()
	{
		this.timerTarget = this.baseTime + Random.Range(0f, this.randomModifier);
		this.timer = 0f;
	}

	// Token: 0x04003F2F RID: 16175
	[SerializeField]
	private AudioSource[] audioSources;

	// Token: 0x04003F30 RID: 16176
	[SerializeField]
	private AudioClip[] audioClips;

	// Token: 0x04003F31 RID: 16177
	[SerializeField]
	private float baseTime = 15f;

	// Token: 0x04003F32 RID: 16178
	[SerializeField]
	private float randomModifier = 5f;

	// Token: 0x04003F33 RID: 16179
	private float timer;

	// Token: 0x04003F34 RID: 16180
	private float timerTarget;
}
