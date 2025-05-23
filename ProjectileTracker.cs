using System;
using System.Collections.Generic;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000946 RID: 2374
internal static class ProjectileTracker
{
	// Token: 0x060039A5 RID: 14757 RVA: 0x00115D78 File Offset: 0x00113F78
	static ProjectileTracker()
	{
		RoomSystem.LeftRoomEvent = (Action)Delegate.Combine(RoomSystem.LeftRoomEvent, new Action(ProjectileTracker.ClearProjectiles));
		RoomSystem.PlayerLeftEvent = (Action<NetPlayer>)Delegate.Combine(RoomSystem.PlayerLeftEvent, new Action<NetPlayer>(ProjectileTracker.RemovePlayerProjectiles));
	}

	// Token: 0x060039A6 RID: 14758 RVA: 0x00115DEC File Offset: 0x00113FEC
	public static void RemovePlayerProjectiles(NetPlayer player)
	{
		LoopingArray<ProjectileTracker.ProjectileInfo> loopingArray;
		if (ProjectileTracker.m_playerProjectiles.TryGetValue(player, out loopingArray))
		{
			ProjectileTracker.ResetPlayerProjectiles(loopingArray);
			ProjectileTracker.m_playerProjectiles.Remove(player);
			ProjectileTracker.m_projectileInfoPool.Return(loopingArray);
		}
	}

	// Token: 0x060039A7 RID: 14759 RVA: 0x00115E28 File Offset: 0x00114028
	private static void ClearProjectiles()
	{
		foreach (LoopingArray<ProjectileTracker.ProjectileInfo> loopingArray in ProjectileTracker.m_playerProjectiles.Values)
		{
			ProjectileTracker.ResetPlayerProjectiles(loopingArray);
			ProjectileTracker.m_projectileInfoPool.Return(loopingArray);
		}
		ProjectileTracker.m_playerProjectiles.Clear();
	}

	// Token: 0x060039A8 RID: 14760 RVA: 0x00115E94 File Offset: 0x00114094
	private static void ResetPlayerProjectiles(LoopingArray<ProjectileTracker.ProjectileInfo> projectiles)
	{
		for (int i = 0; i < projectiles.Length; i++)
		{
			SlingshotProjectile projectileInstance = projectiles[i].projectileInstance;
			if (!projectileInstance.IsNull() && projectileInstance.projectileOwner != NetworkSystem.Instance.LocalPlayer && projectileInstance.gameObject.activeSelf)
			{
				projectileInstance.Deactivate();
			}
		}
	}

	// Token: 0x060039A9 RID: 14761 RVA: 0x00115EEC File Offset: 0x001140EC
	public static int AddAndIncrementLocalProjectile(SlingshotProjectile projectile, Vector3 intialVelocity, Vector3 initialPosition, float scale)
	{
		SlingshotProjectile projectileInstance = ProjectileTracker.m_localProjectiles[ProjectileTracker.m_localProjectiles.CurrentIndex].projectileInstance;
		if (projectileInstance.IsNotNull() && projectileInstance.projectileOwner == NetworkSystem.Instance.LocalPlayer && projectileInstance.gameObject.activeSelf)
		{
			projectileInstance.Deactivate();
		}
		ProjectileTracker.ProjectileInfo projectileInfo = new ProjectileTracker.ProjectileInfo(PhotonNetwork.Time, intialVelocity, initialPosition, scale, projectile);
		return ProjectileTracker.m_localProjectiles.AddAndIncrement(in projectileInfo);
	}

	// Token: 0x060039AA RID: 14762 RVA: 0x00115F5C File Offset: 0x0011415C
	public static void AddRemotePlayerProjectile(NetPlayer player, SlingshotProjectile projectile, int projectileIndex, double timeShot, Vector3 intialVelocity, Vector3 initialPosition, float scale)
	{
		LoopingArray<ProjectileTracker.ProjectileInfo> loopingArray;
		if (!ProjectileTracker.m_playerProjectiles.ContainsKey(player))
		{
			loopingArray = ProjectileTracker.m_projectileInfoPool.Take();
			ProjectileTracker.m_playerProjectiles[player] = loopingArray;
		}
		else
		{
			loopingArray = ProjectileTracker.m_playerProjectiles[player];
		}
		if (projectileIndex < 0 || projectileIndex >= loopingArray.Length)
		{
			GorillaNot.instance.SendReport("invlProj", player.UserId, player.NickName);
			return;
		}
		SlingshotProjectile projectileInstance = loopingArray[projectileIndex].projectileInstance;
		if (projectileInstance.IsNotNull() && projectileInstance.projectileOwner == player && projectileInstance.gameObject.activeSelf)
		{
			projectileInstance.Deactivate();
		}
		ProjectileTracker.ProjectileInfo projectileInfo = new ProjectileTracker.ProjectileInfo(timeShot, intialVelocity, initialPosition, scale, projectile);
		loopingArray[projectileIndex] = projectileInfo;
	}

	// Token: 0x060039AB RID: 14763 RVA: 0x0011600E File Offset: 0x0011420E
	public static ProjectileTracker.ProjectileInfo GetLocalProjectile(int index)
	{
		return ProjectileTracker.m_localProjectiles[index];
	}

	// Token: 0x060039AC RID: 14764 RVA: 0x0011601C File Offset: 0x0011421C
	public static ValueTuple<bool, ProjectileTracker.ProjectileInfo> GetAndRemoveRemotePlayerProjectile(NetPlayer player, int index)
	{
		ValueTuple<bool, ProjectileTracker.ProjectileInfo> valueTuple = new ValueTuple<bool, ProjectileTracker.ProjectileInfo>(false, default(ProjectileTracker.ProjectileInfo));
		LoopingArray<ProjectileTracker.ProjectileInfo> loopingArray;
		if (index < 0 || index >= ProjectileTracker.m_localProjectiles.Length || !ProjectileTracker.m_playerProjectiles.TryGetValue(player, out loopingArray))
		{
			return valueTuple;
		}
		ProjectileTracker.ProjectileInfo projectileInfo = loopingArray[index];
		if (projectileInfo.projectileInstance.IsNotNull())
		{
			valueTuple.Item1 = true;
			valueTuple.Item2 = projectileInfo;
			loopingArray[index] = default(ProjectileTracker.ProjectileInfo);
		}
		return valueTuple;
	}

	// Token: 0x04003ECC RID: 16076
	private static LoopingArray<ProjectileTracker.ProjectileInfo>.Pool m_projectileInfoPool = new LoopingArray<ProjectileTracker.ProjectileInfo>.Pool(50, 9);

	// Token: 0x04003ECD RID: 16077
	private static LoopingArray<ProjectileTracker.ProjectileInfo> m_localProjectiles = new LoopingArray<ProjectileTracker.ProjectileInfo>(50);

	// Token: 0x04003ECE RID: 16078
	public static readonly Dictionary<NetPlayer, LoopingArray<ProjectileTracker.ProjectileInfo>> m_playerProjectiles = new Dictionary<NetPlayer, LoopingArray<ProjectileTracker.ProjectileInfo>>(9);

	// Token: 0x02000947 RID: 2375
	public struct ProjectileInfo
	{
		// Token: 0x060039AD RID: 14765 RVA: 0x00116092 File Offset: 0x00114292
		public ProjectileInfo(double newTime, Vector3 newVel, Vector3 origin, float newScale, SlingshotProjectile projectile)
		{
			this.timeLaunched = newTime;
			this.shotVelocity = newVel;
			this.launchOrigin = origin;
			this.scale = newScale;
			this.projectileInstance = projectile;
			this.hasImpactOverride = projectile.playerImpactEffectPrefab.IsNotNull();
		}

		// Token: 0x04003ECF RID: 16079
		public double timeLaunched;

		// Token: 0x04003ED0 RID: 16080
		public Vector3 shotVelocity;

		// Token: 0x04003ED1 RID: 16081
		public Vector3 launchOrigin;

		// Token: 0x04003ED2 RID: 16082
		public float scale;

		// Token: 0x04003ED3 RID: 16083
		public SlingshotProjectile projectileInstance;

		// Token: 0x04003ED4 RID: 16084
		public bool hasImpactOverride;
	}
}
