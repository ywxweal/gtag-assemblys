using System;
using UnityEngine;

// Token: 0x020000F8 RID: 248
public class VacuumHoldable : TransferrableObject
{
	// Token: 0x06000631 RID: 1585 RVA: 0x000239DC File Offset: 0x00021BDC
	public override void OnSpawn(VRRig rig)
	{
		base.OnSpawn(rig);
		this.itemState = TransferrableObject.ItemStates.State0;
	}

	// Token: 0x06000632 RID: 1586 RVA: 0x000239EC File Offset: 0x00021BEC
	internal override void OnEnable()
	{
		base.OnEnable();
		this.itemState = TransferrableObject.ItemStates.State0;
		this.hasAudioSource = this.audioSource != null && this.audioSource.clip != null;
	}

	// Token: 0x06000633 RID: 1587 RVA: 0x00023A24 File Offset: 0x00021C24
	internal override void OnDisable()
	{
		base.OnDisable();
		this.itemState = TransferrableObject.ItemStates.State0;
		if (this.particleFX.isPlaying)
		{
			this.particleFX.Stop();
		}
		if (this.hasAudioSource && this.audioSource.isPlaying)
		{
			this.audioSource.GTStop();
		}
	}

	// Token: 0x06000634 RID: 1588 RVA: 0x00023A78 File Offset: 0x00021C78
	private void InitToDefault()
	{
		this.itemState = TransferrableObject.ItemStates.State0;
		if (this.particleFX.isPlaying)
		{
			this.particleFX.Stop();
		}
		if (this.hasAudioSource && this.audioSource.isPlaying)
		{
			this.audioSource.GTStop();
		}
	}

	// Token: 0x06000635 RID: 1589 RVA: 0x00023AC4 File Offset: 0x00021CC4
	public override void ResetToDefaultState()
	{
		base.ResetToDefaultState();
		this.InitToDefault();
	}

	// Token: 0x06000636 RID: 1590 RVA: 0x00023AD4 File Offset: 0x00021CD4
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		if (!this.IsMyItem() && base.myOnlineRig != null && base.myOnlineRig.muted)
		{
			this.itemState = TransferrableObject.ItemStates.State0;
		}
		if (this.itemState == TransferrableObject.ItemStates.State0)
		{
			if (this.particleFX.isPlaying)
			{
				this.particleFX.Stop();
			}
			if (this.hasAudioSource && this.audioSource.isPlaying)
			{
				this.audioSource.GTStop();
				return;
			}
		}
		else
		{
			if (!this.particleFX.isEmitting)
			{
				this.particleFX.Play();
			}
			if (this.hasAudioSource && !this.audioSource.isPlaying)
			{
				this.audioSource.GTPlay();
			}
			if (this.IsMyItem() && Time.time > this.activationStartTime + this.activationVibrationStartDuration)
			{
				GorillaTagger.Instance.StartVibration(this.currentState == TransferrableObject.PositionState.InLeftHand, this.activationVibrationLoopStrength, Time.deltaTime);
			}
		}
	}

	// Token: 0x06000637 RID: 1591 RVA: 0x00023BC8 File Offset: 0x00021DC8
	public override void OnActivate()
	{
		base.OnActivate();
		this.itemState = TransferrableObject.ItemStates.State1;
		if (this.IsMyItem())
		{
			this.activationStartTime = Time.time;
			GorillaTagger.Instance.StartVibration(this.currentState == TransferrableObject.PositionState.InLeftHand, this.activationVibrationStartStrength, this.activationVibrationStartDuration);
		}
	}

	// Token: 0x06000638 RID: 1592 RVA: 0x00023C14 File Offset: 0x00021E14
	public override void OnDeactivate()
	{
		base.OnDeactivate();
		this.itemState = TransferrableObject.ItemStates.State0;
	}

	// Token: 0x04000759 RID: 1881
	[Tooltip("Emission rate will be increase when the trigger button is pressed.")]
	public ParticleSystem particleFX;

	// Token: 0x0400075A RID: 1882
	[Tooltip("Sound will loop and fade in/out volume when trigger pressed.")]
	public AudioSource audioSource;

	// Token: 0x0400075B RID: 1883
	private float activationVibrationStartStrength = 0.8f;

	// Token: 0x0400075C RID: 1884
	private float activationVibrationStartDuration = 0.05f;

	// Token: 0x0400075D RID: 1885
	private float activationVibrationLoopStrength = 0.005f;

	// Token: 0x0400075E RID: 1886
	private float activationStartTime;

	// Token: 0x0400075F RID: 1887
	private bool hasAudioSource;

	// Token: 0x020000F9 RID: 249
	private enum VacuumState
	{
		// Token: 0x04000761 RID: 1889
		None = 1,
		// Token: 0x04000762 RID: 1890
		Active
	}
}
