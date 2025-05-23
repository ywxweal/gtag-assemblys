using System;
using JetBrains.Annotations;
using UnityEngine;

// Token: 0x020003C0 RID: 960
public class WorldTargetItem
{
	// Token: 0x0600164C RID: 5708 RVA: 0x0006C3FC File Offset: 0x0006A5FC
	public bool IsValid()
	{
		return this.itemIdx != -1 && this.owner != null;
	}

	// Token: 0x0600164D RID: 5709 RVA: 0x0006C414 File Offset: 0x0006A614
	[CanBeNull]
	public static WorldTargetItem GenerateTargetFromPlayerAndID(NetPlayer owner, int itemIdx)
	{
		VRRig vrrig = GorillaGameManager.StaticFindRigForPlayer(owner);
		if (vrrig == null)
		{
			Debug.LogError("Tried to setup a sharable object but the target rig is null...");
			return null;
		}
		Transform component = vrrig.myBodyDockPositions.TransferrableItem(itemIdx).gameObject.GetComponent<Transform>();
		return new WorldTargetItem(owner, itemIdx, component);
	}

	// Token: 0x0600164E RID: 5710 RVA: 0x0006C45C File Offset: 0x0006A65C
	public static WorldTargetItem GenerateTargetFromWorldSharableItem(NetPlayer owner, int itemIdx, Transform transform)
	{
		return new WorldTargetItem(owner, itemIdx, transform);
	}

	// Token: 0x0600164F RID: 5711 RVA: 0x0006C466 File Offset: 0x0006A666
	private WorldTargetItem(NetPlayer owner, int itemIdx, Transform transform)
	{
		this.owner = owner;
		this.itemIdx = itemIdx;
		this.targetObject = transform;
		this.transferrableObject = transform.GetComponent<TransferrableObject>();
	}

	// Token: 0x06001650 RID: 5712 RVA: 0x0006C48F File Offset: 0x0006A68F
	public override string ToString()
	{
		return string.Format("Id: {0} ({1})", this.itemIdx, this.owner);
	}

	// Token: 0x040018D4 RID: 6356
	public readonly NetPlayer owner;

	// Token: 0x040018D5 RID: 6357
	public readonly int itemIdx;

	// Token: 0x040018D6 RID: 6358
	public readonly Transform targetObject;

	// Token: 0x040018D7 RID: 6359
	public readonly TransferrableObject transferrableObject;
}
