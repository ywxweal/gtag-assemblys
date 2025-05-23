using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020005D2 RID: 1490
public class GRUIStationEmployeeBadges : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06002452 RID: 9298 RVA: 0x000B698C File Offset: 0x000B4B8C
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		this.registeredBadges = new List<GRBadge>();
		for (int i = 0; i < this.badgeDispensers.Count; i++)
		{
			this.badgeDispensers[i].index = i;
			this.badgeDispensers[i].actorNr = -1;
		}
		this.dispenserForActorNr = new Dictionary<int, int>();
		VRRigCache.OnRigActivated += this.UpdateRigs;
		VRRigCache.OnRigDeactivated += this.UpdateRigs;
		RoomSystem.JoinedRoomEvent = (Action)Delegate.Combine(RoomSystem.JoinedRoomEvent, new Action(this.UpdateRigs));
		this.UpdateRigs();
	}

	// Token: 0x06002453 RID: 9299 RVA: 0x000B6A38 File Offset: 0x000B4C38
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		VRRigCache.OnRigActivated -= this.UpdateRigs;
		VRRigCache.OnRigDeactivated -= this.UpdateRigs;
		RoomSystem.JoinedRoomEvent = (Action)Delegate.Remove(RoomSystem.JoinedRoomEvent, new Action(this.UpdateRigs));
	}

	// Token: 0x06002454 RID: 9300 RVA: 0x000B6A8E File Offset: 0x000B4C8E
	public void UpdateRigs(RigContainer container)
	{
		this.UpdateRigs();
	}

	// Token: 0x06002455 RID: 9301 RVA: 0x000B6A96 File Offset: 0x000B4C96
	public void UpdateRigs()
	{
		GRUIStationEmployeeBadges.tempRigs.Clear();
		GRUIStationEmployeeBadges.tempRigs.Add(VRRig.LocalRig);
		VRRigCache.Instance.GetAllUsedRigs(GRUIStationEmployeeBadges.tempRigs);
	}

	// Token: 0x06002456 RID: 9302 RVA: 0x000B6AC0 File Offset: 0x000B4CC0
	public void RefreshBadgesAuthority()
	{
		for (int i = 0; i < GRUIStationEmployeeBadges.tempRigs.Count; i++)
		{
			NetPlayer netPlayer = (GRUIStationEmployeeBadges.tempRigs[i].isOfflineVRRig ? NetworkSystem.Instance.LocalPlayer : GRUIStationEmployeeBadges.tempRigs[i].OwningNetPlayer);
			int num;
			if (netPlayer != null && netPlayer.ActorNumber != -1 && !this.dispenserForActorNr.TryGetValue(netPlayer.ActorNumber, out num))
			{
				for (int j = 0; j < this.badgeDispensers.Count; j++)
				{
					if (this.badgeDispensers[j].actorNr == -1)
					{
						this.badgeDispensers[j].CreateBadge(netPlayer);
						break;
					}
				}
			}
		}
		for (int k = this.registeredBadges.Count - 1; k >= 0; k--)
		{
			int num2;
			if (NetworkSystem.Instance.GetNetPlayerByID(this.registeredBadges[k].actorNr) == null || !this.dispenserForActorNr.TryGetValue(this.registeredBadges[k].actorNr, out num2) || num2 != this.registeredBadges[k].dispenserIndex)
			{
				GameEntityManager.instance.RequestDestroyItem(this.registeredBadges[k].GetComponent<GameEntity>().id);
			}
		}
	}

	// Token: 0x06002457 RID: 9303 RVA: 0x000B6C10 File Offset: 0x000B4E10
	public void SliceUpdate()
	{
		if (GameEntityManager.instance.IsAuthority())
		{
			this.RefreshBadgesAuthority();
		}
		for (int i = 0; i < this.badgeDispensers.Count; i++)
		{
			this.badgeDispensers[i].Refresh();
		}
	}

	// Token: 0x06002458 RID: 9304 RVA: 0x000B6C58 File Offset: 0x000B4E58
	public void RemoveBadge(GRBadge badge)
	{
		if (this.registeredBadges.Contains(badge))
		{
			this.registeredBadges.Remove(badge);
		}
		if (this.badgeDispensers[badge.dispenserIndex].idBadge == badge)
		{
			this.dispenserForActorNr.Remove(badge.actorNr);
			this.badgeDispensers[badge.dispenserIndex].ClearBadge();
		}
	}

	// Token: 0x06002459 RID: 9305 RVA: 0x000B6CC8 File Offset: 0x000B4EC8
	public void LinkBadgeToDispenser(GRBadge badge, long createData)
	{
		if (!this.registeredBadges.Contains(badge))
		{
			this.registeredBadges.Add(badge);
		}
		int num = (int)(createData % 100L);
		if (num > this.badgeDispensers.Count)
		{
			return;
		}
		NetPlayer netPlayerByID = NetworkSystem.Instance.GetNetPlayerByID((int)(createData / 100L));
		if (netPlayerByID != null)
		{
			this.dispenserForActorNr[netPlayerByID.ActorNumber] = num;
			this.badgeDispensers[num].AttachIDBadge(badge, netPlayerByID);
		}
	}

	// Token: 0x0600245C RID: 9308 RVA: 0x00011040 File Offset: 0x0000F240
	bool IGorillaSliceableSimple.get_isActiveAndEnabled()
	{
		return base.isActiveAndEnabled;
	}

	// Token: 0x0400296C RID: 10604
	[SerializeField]
	public List<GRUIEmployeeBadgeDispenser> badgeDispensers;

	// Token: 0x0400296D RID: 10605
	private static List<VRRig> tempRigs = new List<VRRig>(16);

	// Token: 0x0400296E RID: 10606
	public Dictionary<int, int> dispenserForActorNr;

	// Token: 0x0400296F RID: 10607
	public List<GRBadge> registeredBadges;
}
