using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x02000198 RID: 408
public abstract class ProjectileWeapon : TransferrableObject
{
	// Token: 0x06000A0D RID: 2573
	protected abstract Vector3 GetLaunchPosition();

	// Token: 0x06000A0E RID: 2574
	protected abstract Vector3 GetLaunchVelocity();

	// Token: 0x06000A0F RID: 2575 RVA: 0x00034F57 File Offset: 0x00033157
	internal override void OnEnable()
	{
		base.OnEnable();
		if (base.myOnlineRig != null)
		{
			base.myOnlineRig.projectileWeapon = this;
		}
		if (base.myRig != null)
		{
			base.myRig.projectileWeapon = this;
		}
	}

	// Token: 0x06000A10 RID: 2576 RVA: 0x00034F94 File Offset: 0x00033194
	protected void LaunchProjectile()
	{
		int num = PoolUtils.GameObjHashCode(this.projectilePrefab);
		int num2 = PoolUtils.GameObjHashCode(this.projectileTrail);
		GameObject gameObject = ObjectPools.instance.Instantiate(num, true);
		float num3 = Mathf.Abs(base.transform.lossyScale.x);
		gameObject.transform.localScale = Vector3.one * num3;
		Vector3 launchPosition = this.GetLaunchPosition();
		Vector3 launchVelocity = this.GetLaunchVelocity();
		bool flag;
		bool flag2;
		this.GetIsOnTeams(out flag, out flag2);
		this.AttachTrail(num2, gameObject, launchPosition, flag, flag2);
		SlingshotProjectile component = gameObject.GetComponent<SlingshotProjectile>();
		if (NetworkSystem.Instance.InRoom)
		{
			int num4 = ProjectileTracker.AddAndIncrementLocalProjectile(component, launchVelocity, launchPosition, num3);
			component.Launch(launchPosition, launchVelocity, NetworkSystem.Instance.LocalPlayer, flag, flag2, num4, num3, false, default(Color));
			TransferrableObject.PositionState currentState = this.currentState;
			RoomSystem.SendLaunchProjectile(launchPosition, launchVelocity, RoomSystem.ProjectileSource.ProjectileWeapon, num4, false, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
			this.PlayLaunchSfx();
		}
		else
		{
			component.Launch(launchPosition, launchVelocity, NetworkSystem.Instance.LocalPlayer, flag, flag2, 0, num3, false, default(Color));
			this.PlayLaunchSfx();
		}
		PlayerGameEvents.LaunchedProjectile(this.projectilePrefab.name);
	}

	// Token: 0x06000A11 RID: 2577 RVA: 0x000350D0 File Offset: 0x000332D0
	internal virtual SlingshotProjectile LaunchNetworkedProjectile(Vector3 location, Vector3 velocity, RoomSystem.ProjectileSource projectileSource, int projectileCounter, float scale, bool shouldOverrideColor, Color color, PhotonMessageInfoWrapped info)
	{
		GameObject gameObject = null;
		SlingshotProjectile slingshotProjectile = null;
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		try
		{
			int num = -1;
			int num2 = -1;
			if (projectileSource == RoomSystem.ProjectileSource.ProjectileWeapon)
			{
				if (this.currentState == TransferrableObject.PositionState.OnChest || this.currentState == TransferrableObject.PositionState.None)
				{
					return null;
				}
				num = PoolUtils.GameObjHashCode(this.projectilePrefab);
				num2 = PoolUtils.GameObjHashCode(this.projectileTrail);
			}
			gameObject = ObjectPools.instance.Instantiate(num, true);
			slingshotProjectile = gameObject.GetComponent<SlingshotProjectile>();
			bool flag;
			bool flag2;
			this.GetIsOnTeams(out flag, out flag2);
			if (num2 != -1)
			{
				this.AttachTrail(num2, slingshotProjectile.gameObject, location, flag, flag2);
			}
			slingshotProjectile.Launch(location, velocity, player, flag, flag2, projectileCounter, scale, shouldOverrideColor, color);
			this.PlayLaunchSfx();
		}
		catch
		{
			GorillaNot.instance.SendReport("projectile error", player.UserId, player.NickName);
			if (slingshotProjectile != null && slingshotProjectile)
			{
				slingshotProjectile.transform.position = Vector3.zero;
				slingshotProjectile.Deactivate();
				slingshotProjectile = null;
			}
			else if (gameObject.IsNotNull())
			{
				ObjectPools.instance.Destroy(gameObject);
			}
		}
		return slingshotProjectile;
	}

	// Token: 0x06000A12 RID: 2578 RVA: 0x000351F0 File Offset: 0x000333F0
	protected void GetIsOnTeams(out bool blueTeam, out bool orangeTeam)
	{
		NetPlayer netPlayer = base.OwningPlayer();
		blueTeam = false;
		orangeTeam = false;
		if (GorillaGameManager.instance != null)
		{
			GorillaPaintbrawlManager component = GorillaGameManager.instance.GetComponent<GorillaPaintbrawlManager>();
			if (component != null)
			{
				blueTeam = component.OnBlueTeam(netPlayer);
				orangeTeam = component.OnRedTeam(netPlayer);
			}
		}
	}

	// Token: 0x06000A13 RID: 2579 RVA: 0x00035240 File Offset: 0x00033440
	private void AttachTrail(int trailHash, GameObject newProjectile, Vector3 location, bool blueTeam, bool orangeTeam)
	{
		GameObject gameObject = ObjectPools.instance.Instantiate(trailHash, true);
		SlingshotProjectileTrail component = gameObject.GetComponent<SlingshotProjectileTrail>();
		if (component.IsNull())
		{
			ObjectPools.instance.Destroy(gameObject);
		}
		newProjectile.transform.position = location;
		component.AttachTrail(newProjectile, blueTeam, orangeTeam);
	}

	// Token: 0x06000A14 RID: 2580 RVA: 0x0003528C File Offset: 0x0003348C
	private void PlayLaunchSfx()
	{
		if (this.shootSfx != null && this.shootSfxClips != null && this.shootSfxClips.Length != 0)
		{
			this.shootSfx.GTPlayOneShot(this.shootSfxClips[Random.Range(0, this.shootSfxClips.Length)], 1f);
		}
	}

	// Token: 0x04000C2E RID: 3118
	[SerializeField]
	protected GameObject projectilePrefab;

	// Token: 0x04000C2F RID: 3119
	[SerializeField]
	private GameObject projectileTrail;

	// Token: 0x04000C30 RID: 3120
	public AudioClip[] shootSfxClips;

	// Token: 0x04000C31 RID: 3121
	public AudioSource shootSfx;
}
