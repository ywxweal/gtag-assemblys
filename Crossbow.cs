using System;
using UnityEngine;

// Token: 0x0200016A RID: 362
public class Crossbow : ProjectileWeapon
{
	// Token: 0x06000914 RID: 2324 RVA: 0x00031318 File Offset: 0x0002F518
	protected override void Awake()
	{
		base.Awake();
		TransferrableObjectHoldablePart_Crank[] array = this.cranks;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetOnCrankedCallback(new Action<float>(this.OnCrank));
		}
		this.SetReloadFraction(0f);
	}

	// Token: 0x06000915 RID: 2325 RVA: 0x00031360 File Offset: 0x0002F560
	public void SetReloadFraction(float newFraction)
	{
		this.loadFraction = Mathf.Clamp01(newFraction);
		this.animator.SetFloat(this.ReloadFractionHashID, this.loadFraction);
		if (this.loadFraction == 1f && !this.dummyProjectile.enabled)
		{
			this.shootSfx.GTPlayOneShot(this.reloadComplete_audioClip, 1f);
			this.dummyProjectile.enabled = true;
			return;
		}
		if (this.loadFraction < 1f && this.dummyProjectile.enabled)
		{
			this.dummyProjectile.enabled = false;
		}
	}

	// Token: 0x06000916 RID: 2326 RVA: 0x000313F8 File Offset: 0x0002F5F8
	private void OnCrank(float degrees)
	{
		if (this.loadFraction == 1f)
		{
			return;
		}
		this.totalCrankDegrees += degrees;
		this.crankSoundDegrees += degrees;
		if (Mathf.Abs(this.crankSoundDegrees) > this.crankSoundDegreesThreshold)
		{
			this.playingCrankSoundUntilTimestamp = Time.time + this.crankSoundContinueDuration;
			this.crankSoundDegrees = 0f;
		}
		if (!this.reloadAudio.isPlaying && Time.time < this.playingCrankSoundUntilTimestamp)
		{
			this.reloadAudio.GTPlay();
		}
		this.SetReloadFraction(Mathf.Abs(this.totalCrankDegrees / this.crankTotalDegreesToReload));
		if (this.loadFraction >= 1f)
		{
			this.totalCrankDegrees = 0f;
		}
	}

	// Token: 0x06000917 RID: 2327 RVA: 0x000314B4 File Offset: 0x0002F6B4
	protected override Vector3 GetLaunchPosition()
	{
		return this.launchPosition.position;
	}

	// Token: 0x06000918 RID: 2328 RVA: 0x000314C1 File Offset: 0x0002F6C1
	protected override Vector3 GetLaunchVelocity()
	{
		return this.launchPosition.forward * this.launchSpeed * base.myRig.scaleFactor;
	}

	// Token: 0x06000919 RID: 2329 RVA: 0x000314EC File Offset: 0x0002F6EC
	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		if (!base.InHand())
		{
			this.wasPressingTrigger = false;
			return;
		}
		if ((base.InLeftHand() ? base.myRig.leftIndex.calcT : base.myRig.rightIndex.calcT) > 0.5f)
		{
			if (this.loadFraction == 1f && !this.wasPressingTrigger)
			{
				this.SetReloadFraction(0f);
				this.animator.SetTrigger(this.FireHashID);
				base.LaunchProjectile();
			}
			this.wasPressingTrigger = true;
		}
		else
		{
			this.wasPressingTrigger = false;
		}
		if (this.itemState.HasFlag(TransferrableObject.ItemStates.State0))
		{
			if (this.loadFraction < 1f)
			{
				this.itemState &= (TransferrableObject.ItemStates)(-2);
				return;
			}
		}
		else if (this.loadFraction == 1f)
		{
			this.itemState |= TransferrableObject.ItemStates.State0;
		}
	}

	// Token: 0x0600091A RID: 2330 RVA: 0x000315DC File Offset: 0x0002F7DC
	protected override void LateUpdateReplicated()
	{
		base.LateUpdateReplicated();
		if (!base.InHand())
		{
			return;
		}
		if (this.itemState.HasFlag(TransferrableObject.ItemStates.State0))
		{
			this.SetReloadFraction(1f);
			return;
		}
		if (this.loadFraction == 1f)
		{
			this.SetReloadFraction(0f);
		}
	}

	// Token: 0x0600091B RID: 2331 RVA: 0x00031634 File Offset: 0x0002F834
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		if (this.reloadAudio.isPlaying && Time.time > this.playingCrankSoundUntilTimestamp)
		{
			this.reloadAudio.GTStop();
		}
	}

	// Token: 0x04000AD9 RID: 2777
	[SerializeField]
	private Transform launchPosition;

	// Token: 0x04000ADA RID: 2778
	[SerializeField]
	private float launchSpeed;

	// Token: 0x04000ADB RID: 2779
	[SerializeField]
	private Animator animator;

	// Token: 0x04000ADC RID: 2780
	[SerializeField]
	private float crankTotalDegreesToReload;

	// Token: 0x04000ADD RID: 2781
	[SerializeField]
	private TransferrableObjectHoldablePart_Crank[] cranks;

	// Token: 0x04000ADE RID: 2782
	[SerializeField]
	private MeshRenderer dummyProjectile;

	// Token: 0x04000ADF RID: 2783
	[SerializeField]
	private AudioSource reloadAudio;

	// Token: 0x04000AE0 RID: 2784
	[SerializeField]
	private AudioClip reloadComplete_audioClip;

	// Token: 0x04000AE1 RID: 2785
	[SerializeField]
	private float crankSoundContinueDuration = 0.1f;

	// Token: 0x04000AE2 RID: 2786
	[SerializeField]
	private float crankSoundDegreesThreshold = 0.1f;

	// Token: 0x04000AE3 RID: 2787
	private AnimHashId FireHashID = "Fire";

	// Token: 0x04000AE4 RID: 2788
	private AnimHashId ReloadFractionHashID = "ReloadFraction";

	// Token: 0x04000AE5 RID: 2789
	private float totalCrankDegrees;

	// Token: 0x04000AE6 RID: 2790
	private float loadFraction;

	// Token: 0x04000AE7 RID: 2791
	private float playingCrankSoundUntilTimestamp;

	// Token: 0x04000AE8 RID: 2792
	private float crankSoundDegrees;

	// Token: 0x04000AE9 RID: 2793
	private bool wasPressingTrigger;
}
