using System;
using UnityEngine;

// Token: 0x020000AD RID: 173
public class ShadeRevealer : ProjectileWeapon
{
	// Token: 0x0600043F RID: 1087 RVA: 0x00018BA0 File Offset: 0x00016DA0
	private float GetDistanceToBeamRay(Vector3 toPosition)
	{
		return Vector3.Cross(this.beamForward.forward, toPosition).magnitude;
	}

	// Token: 0x06000440 RID: 1088 RVA: 0x00018BC8 File Offset: 0x00016DC8
	public ShadeRevealer.State GetBeamStateForPosition(Vector3 toPosition, float tolerance)
	{
		if (toPosition.magnitude <= this.beamLength + tolerance && Vector3.Dot(toPosition.normalized, this.beamForward.forward) > 0f)
		{
			float num = this.GetDistanceToBeamRay(toPosition) - tolerance;
			if (num <= this.lockThreshold)
			{
				return ShadeRevealer.State.LOCKED;
			}
			if (num <= this.trackThreshold)
			{
				return ShadeRevealer.State.TRACKING;
			}
		}
		return ShadeRevealer.State.SCANNING;
	}

	// Token: 0x06000441 RID: 1089 RVA: 0x00018C25 File Offset: 0x00016E25
	public ShadeRevealer.State GetBeamStateForCritter(CosmeticCritter critter, float tolerance)
	{
		return this.GetBeamStateForPosition(critter.transform.position - this.beamForward.position, tolerance);
	}

	// Token: 0x06000442 RID: 1090 RVA: 0x00018C49 File Offset: 0x00016E49
	public bool CritterWithinBeamThreshold(CosmeticCritter critter, ShadeRevealer.State criteria, float tolerance)
	{
		return this.GetBeamStateForCritter(critter, tolerance) >= criteria;
	}

	// Token: 0x06000443 RID: 1091 RVA: 0x00018C59 File Offset: 0x00016E59
	public void SetBestBeamState(ShadeRevealer.State state)
	{
		if (state > this.pendingBeamState)
		{
			this.pendingBeamState = state;
		}
	}

	// Token: 0x06000444 RID: 1092 RVA: 0x00018C6C File Offset: 0x00016E6C
	private void SetObjectsEnabledFromState(ShadeRevealer.State state)
	{
		for (int i = 0; i < this.enableWhenScanning.Length; i++)
		{
			this.enableWhenScanning[i].SetActive(false);
		}
		for (int j = 0; j < this.enableWhenTracking.Length; j++)
		{
			this.enableWhenTracking[j].SetActive(false);
		}
		for (int k = 0; k < this.enableWhenLocked.Length; k++)
		{
			this.enableWhenLocked[k].SetActive(false);
		}
		for (int l = 0; l < this.enableWhenPrimed.Length; l++)
		{
			this.enableWhenPrimed[l].SetActive(false);
		}
		GameObject[] array;
		switch (state)
		{
		case ShadeRevealer.State.SCANNING:
			array = this.enableWhenScanning;
			break;
		case ShadeRevealer.State.TRACKING:
			array = this.enableWhenTracking;
			break;
		case ShadeRevealer.State.LOCKED:
			array = this.enableWhenLocked;
			break;
		case ShadeRevealer.State.PRIMED:
			array = this.enableWhenPrimed;
			break;
		default:
			return;
		}
		for (int m = 0; m < array.Length; m++)
		{
			array[m].SetActive(true);
		}
	}

	// Token: 0x06000445 RID: 1093 RVA: 0x00018D59 File Offset: 0x00016F59
	protected override Vector3 GetLaunchPosition()
	{
		return this.beamForward.position;
	}

	// Token: 0x06000446 RID: 1094 RVA: 0x00018D66 File Offset: 0x00016F66
	protected override Vector3 GetLaunchVelocity()
	{
		return this.beamForward.forward * this.shootVelocity;
	}

	// Token: 0x06000447 RID: 1095 RVA: 0x00018D80 File Offset: 0x00016F80
	internal override SlingshotProjectile LaunchNetworkedProjectile(Vector3 location, Vector3 velocity, RoomSystem.ProjectileSource projectileSource, int projectileCounter, float scale, bool shouldOverrideColor, Color color, PhotonMessageInfoWrapped info)
	{
		if (this.currentBeamState == ShadeRevealer.State.PRIMED)
		{
			return base.LaunchNetworkedProjectile(location, velocity, projectileSource, projectileCounter, scale, shouldOverrideColor, color, info);
		}
		return null;
	}

	// Token: 0x06000448 RID: 1096 RVA: 0x00018DAC File Offset: 0x00016FAC
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		if (this.currentBeamState != this.pendingBeamState)
		{
			this.currentBeamState = this.pendingBeamState;
			this.SetObjectsEnabledFromState(this.currentBeamState);
		}
		this.beamSFX.pitch = 1f + this.shadeCatcher.GetActionTimeFrac() * 2f;
		if (this.isScanning)
		{
			this.pendingBeamState = ShadeRevealer.State.SCANNING;
		}
	}

	// Token: 0x06000449 RID: 1097 RVA: 0x00018E16 File Offset: 0x00017016
	public void StartScanning()
	{
		this.shadeCatcher.enabled = true;
		this.initialActivationSFX.Play();
		this.beamSFX.Play();
		this.isScanning = true;
		this.currentBeamState = ShadeRevealer.State.OFF;
		this.pendingBeamState = ShadeRevealer.State.SCANNING;
	}

	// Token: 0x0600044A RID: 1098 RVA: 0x00018E50 File Offset: 0x00017050
	public void StopScanning()
	{
		if (this.currentBeamState == ShadeRevealer.State.PRIMED)
		{
			base.LaunchProjectile();
			this.shootFX.Play();
		}
		this.shadeCatcher.enabled = false;
		this.initialActivationSFX.Stop();
		this.beamSFX.Stop();
		this.isScanning = false;
		this.currentBeamState = ShadeRevealer.State.OFF;
		this.pendingBeamState = ShadeRevealer.State.OFF;
		this.SetObjectsEnabledFromState(ShadeRevealer.State.OFF);
	}

	// Token: 0x0600044B RID: 1099 RVA: 0x00018EB8 File Offset: 0x000170B8
	public void ShadeCaught()
	{
		this.shadeCatcher.enabled = false;
		this.beamSFX.Stop();
		this.catchSFX.Play();
		this.catchFX.Play();
		this.isScanning = false;
		this.currentBeamState = ShadeRevealer.State.OFF;
		this.pendingBeamState = ShadeRevealer.State.PRIMED;
	}

	// Token: 0x040004CC RID: 1228
	[SerializeField]
	private AudioSource initialActivationSFX;

	// Token: 0x040004CD RID: 1229
	[SerializeField]
	private AudioSource beamSFX;

	// Token: 0x040004CE RID: 1230
	[SerializeField]
	private AudioSource catchSFX;

	// Token: 0x040004CF RID: 1231
	[SerializeField]
	private ParticleSystem catchFX;

	// Token: 0x040004D0 RID: 1232
	[SerializeField]
	private ParticleSystem shootFX;

	// Token: 0x040004D1 RID: 1233
	[Space]
	[SerializeField]
	private CosmeticCritterCatcherShade shadeCatcher;

	// Token: 0x040004D2 RID: 1234
	[Space]
	[Tooltip("The transform that represents the origin of the revealer beam.")]
	[SerializeField]
	private Transform beamForward;

	// Token: 0x040004D3 RID: 1235
	[Tooltip("The maximum length of the beam.")]
	[SerializeField]
	private float beamLength;

	// Token: 0x040004D4 RID: 1236
	[Tooltip("If the Shade is this close to the beam, set it to flee and have all Revealers enter Tracking mode.")]
	[SerializeField]
	private float trackThreshold;

	// Token: 0x040004D5 RID: 1237
	[Tooltip("If the Shade is this close to the beam, slow it down.")]
	[SerializeField]
	private float lockThreshold;

	// Token: 0x040004D6 RID: 1238
	[Tooltip("Editor-only object to help test the thresholds.")]
	[SerializeField]
	private Transform thresholdTester;

	// Token: 0x040004D7 RID: 1239
	[Tooltip("Whether to draw the tester or not.")]
	[SerializeField]
	private bool drawThresholdTesterInEditor = true;

	// Token: 0x040004D8 RID: 1240
	[Tooltip("The velocity to shoot a captured Shade with.")]
	[SerializeField]
	private float shootVelocity = 5f;

	// Token: 0x040004D9 RID: 1241
	[Space]
	[Tooltip("Enable these objects while the beam is in Scanning mode.")]
	[SerializeField]
	private GameObject[] enableWhenScanning;

	// Token: 0x040004DA RID: 1242
	[Tooltip("Enable these objects while the beam is in Tracking mode.")]
	[SerializeField]
	private GameObject[] enableWhenTracking;

	// Token: 0x040004DB RID: 1243
	[Tooltip("Enable these objects while the beam is in Locked mode.")]
	[SerializeField]
	private GameObject[] enableWhenLocked;

	// Token: 0x040004DC RID: 1244
	[Tooltip("Enable these objects while ready to fire.")]
	[SerializeField]
	private GameObject[] enableWhenPrimed;

	// Token: 0x040004DD RID: 1245
	private bool isScanning;

	// Token: 0x040004DE RID: 1246
	private ShadeRevealer.State currentBeamState;

	// Token: 0x040004DF RID: 1247
	private ShadeRevealer.State pendingBeamState;

	// Token: 0x020000AE RID: 174
	public enum State
	{
		// Token: 0x040004E1 RID: 1249
		OFF,
		// Token: 0x040004E2 RID: 1250
		SCANNING,
		// Token: 0x040004E3 RID: 1251
		TRACKING,
		// Token: 0x040004E4 RID: 1252
		LOCKED,
		// Token: 0x040004E5 RID: 1253
		PRIMED
	}
}
