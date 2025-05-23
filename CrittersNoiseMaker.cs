using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000060 RID: 96
public class CrittersNoiseMaker : CrittersToolThrowable
{
	// Token: 0x06000222 RID: 546 RVA: 0x0000D82E File Offset: 0x0000BA2E
	protected override void OnImpact(Vector3 hitPosition, Vector3 hitNormal)
	{
		if (CrittersManager.instance.LocalAuthority())
		{
			if (this.destroyOnImpact || this.playOnce)
			{
				this.PlaySingleNoise();
				return;
			}
			this.StartPlayingRepeatNoise();
		}
	}

	// Token: 0x06000223 RID: 547 RVA: 0x0000D85B File Offset: 0x0000BA5B
	protected override void OnImpactCritter(CrittersPawn impactedCritter)
	{
		this.OnImpact(impactedCritter.transform.position, impactedCritter.transform.up);
	}

	// Token: 0x06000224 RID: 548 RVA: 0x0000D879 File Offset: 0x0000BA79
	protected override void OnPickedUp()
	{
		this.StopPlayRepeatNoise();
	}

	// Token: 0x06000225 RID: 549 RVA: 0x0000D884 File Offset: 0x0000BA84
	private void PlaySingleNoise()
	{
		CrittersLoudNoise crittersLoudNoise = (CrittersLoudNoise)CrittersManager.instance.SpawnActor(CrittersActor.CrittersActorType.LoudNoise, this.soundSubIndex);
		if (crittersLoudNoise == null)
		{
			return;
		}
		crittersLoudNoise.MoveActor(base.transform.position, base.transform.rotation, false, true, true);
		crittersLoudNoise.SetImpulseVelocity(Vector3.zero, Vector3.zero);
		CrittersManager.instance.TriggerEvent(CrittersManager.CritterEvent.NoiseMakerTriggered, this.actorId, base.transform.position);
	}

	// Token: 0x06000226 RID: 550 RVA: 0x0000D901 File Offset: 0x0000BB01
	private void StartPlayingRepeatNoise()
	{
		this.StopPlayRepeatNoise();
		this.repeatPlayNoise = base.StartCoroutine(this.PlayRepeatNoise());
	}

	// Token: 0x06000227 RID: 551 RVA: 0x0000D91B File Offset: 0x0000BB1B
	private void StopPlayRepeatNoise()
	{
		if (this.repeatPlayNoise != null)
		{
			base.StopCoroutine(this.repeatPlayNoise);
			this.repeatPlayNoise = null;
		}
	}

	// Token: 0x06000228 RID: 552 RVA: 0x0000D938 File Offset: 0x0000BB38
	private IEnumerator PlayRepeatNoise()
	{
		int num = Mathf.FloorToInt(this.repeatNoiseDuration / this.repeatNoiseRate);
		int num2;
		for (int i = num; i > 0; i = num2 - 1)
		{
			this.PlaySingleNoise();
			yield return new WaitForSeconds(this.repeatNoiseRate);
			num2 = i;
		}
		if (this.destroyAfterPlayingRepeatNoise)
		{
			this.shouldDisable = true;
		}
		yield break;
	}

	// Token: 0x04000282 RID: 642
	[Header("Noise Maker")]
	public int soundSubIndex = 3;

	// Token: 0x04000283 RID: 643
	public bool playOnce = true;

	// Token: 0x04000284 RID: 644
	public float repeatNoiseDuration;

	// Token: 0x04000285 RID: 645
	public float repeatNoiseRate;

	// Token: 0x04000286 RID: 646
	public bool destroyAfterPlayingRepeatNoise = true;

	// Token: 0x04000287 RID: 647
	private Coroutine repeatPlayNoise;
}
