using System;
using UnityEngine;

// Token: 0x0200066A RID: 1642
public class HoverboardAudio : MonoBehaviour
{
	// Token: 0x0600290A RID: 10506 RVA: 0x000CC747 File Offset: 0x000CA947
	private void Start()
	{
		this.Stop();
	}

	// Token: 0x0600290B RID: 10507 RVA: 0x000CC74F File Offset: 0x000CA94F
	public void PlayTurnSound(float angle)
	{
		if (Time.time > this.turnSoundCooldownUntilTimestamp && angle > this.minAngleDeltaForTurnSound)
		{
			this.turnSoundCooldownUntilTimestamp = Time.time + this.turnSoundCooldownDuration;
			this.turnSounds.Play();
		}
	}

	// Token: 0x0600290C RID: 10508 RVA: 0x000CC784 File Offset: 0x000CA984
	public void UpdateAudioLoop(float speed, float airspeed, float strainLevel, float grindLevel)
	{
		this.motorAnimator.UpdateValue(speed, false);
		this.windRushAnimator.UpdateValue(airspeed, false);
		if (grindLevel > 0f)
		{
			this.grindAnimator.UpdatePitchAndVolume(speed, grindLevel + 0.5f, false);
		}
		else
		{
			this.grindAnimator.UpdatePitchAndVolume(0f, 0f, false);
		}
		strainLevel = Mathf.Clamp01(strainLevel * 10f);
		if (!this.didInitHum1BaseVolume)
		{
			this.hum1BaseVolume = this.hum1.volume;
			this.didInitHum1BaseVolume = true;
		}
		this.hum1.volume = Mathf.MoveTowards(this.hum1.volume, this.hum1BaseVolume * strainLevel, this.fadeSpeed * Time.deltaTime);
	}

	// Token: 0x0600290D RID: 10509 RVA: 0x000CC840 File Offset: 0x000CAA40
	public void Stop()
	{
		if (!this.didInitHum1BaseVolume)
		{
			this.hum1BaseVolume = this.hum1.volume;
			this.didInitHum1BaseVolume = true;
		}
		this.hum1.volume = 0f;
		this.windRushAnimator.UpdateValue(0f, true);
		this.motorAnimator.UpdateValue(0f, true);
		this.grindAnimator.UpdateValue(0f, true);
	}

	// Token: 0x04002E15 RID: 11797
	[SerializeField]
	private AudioSource hum1;

	// Token: 0x04002E16 RID: 11798
	[SerializeField]
	private SoundBankPlayer turnSounds;

	// Token: 0x04002E17 RID: 11799
	private bool didInitHum1BaseVolume;

	// Token: 0x04002E18 RID: 11800
	private float hum1BaseVolume;

	// Token: 0x04002E19 RID: 11801
	[SerializeField]
	private float fadeSpeed;

	// Token: 0x04002E1A RID: 11802
	[SerializeField]
	private AudioAnimator windRushAnimator;

	// Token: 0x04002E1B RID: 11803
	[SerializeField]
	private AudioAnimator motorAnimator;

	// Token: 0x04002E1C RID: 11804
	[SerializeField]
	private AudioAnimator grindAnimator;

	// Token: 0x04002E1D RID: 11805
	[SerializeField]
	private float turnSoundCooldownDuration;

	// Token: 0x04002E1E RID: 11806
	[SerializeField]
	private float minAngleDeltaForTurnSound;

	// Token: 0x04002E1F RID: 11807
	private float turnSoundCooldownUntilTimestamp;
}
