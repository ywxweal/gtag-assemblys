using System;
using UnityEngine;

// Token: 0x0200015E RID: 350
public class PlayerSpeedBasedAudio : MonoBehaviour
{
	// Token: 0x060008E1 RID: 2273 RVA: 0x000300DB File Offset: 0x0002E2DB
	private void Start()
	{
		this.fadeRate = 1f / this.fadeTime;
		this.baseVolume = this.audioSource.volume;
		this.localPlayerVelocityEstimator.TryResolve<GorillaVelocityEstimator>(out this.velocityEstimator);
	}

	// Token: 0x060008E2 RID: 2274 RVA: 0x00030114 File Offset: 0x0002E314
	private void Update()
	{
		this.currentFadeLevel = Mathf.MoveTowards(this.currentFadeLevel, Mathf.InverseLerp(this.minVolumeSpeed, this.fullVolumeSpeed, this.velocityEstimator.linearVelocity.magnitude), this.fadeRate * Time.deltaTime);
		if (this.baseVolume == 0f || this.currentFadeLevel == 0f)
		{
			this.audioSource.volume = 0.0001f;
			return;
		}
		this.audioSource.volume = this.baseVolume * this.currentFadeLevel;
	}

	// Token: 0x04000A74 RID: 2676
	[SerializeField]
	private float minVolumeSpeed;

	// Token: 0x04000A75 RID: 2677
	[SerializeField]
	private float fullVolumeSpeed;

	// Token: 0x04000A76 RID: 2678
	[SerializeField]
	private float fadeTime;

	// Token: 0x04000A77 RID: 2679
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04000A78 RID: 2680
	[SerializeField]
	private XSceneRef localPlayerVelocityEstimator;

	// Token: 0x04000A79 RID: 2681
	private GorillaVelocityEstimator velocityEstimator;

	// Token: 0x04000A7A RID: 2682
	private float baseVolume;

	// Token: 0x04000A7B RID: 2683
	private float fadeRate;

	// Token: 0x04000A7C RID: 2684
	private float currentFadeLevel;
}
