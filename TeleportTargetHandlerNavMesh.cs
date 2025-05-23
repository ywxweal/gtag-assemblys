using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000308 RID: 776
public class TeleportTargetHandlerNavMesh : TeleportTargetHandler
{
	// Token: 0x06001292 RID: 4754 RVA: 0x000578FF File Offset: 0x00055AFF
	private void Awake()
	{
		this._path = new NavMeshPath();
	}

	// Token: 0x06001293 RID: 4755 RVA: 0x0005790C File Offset: 0x00055B0C
	protected override bool ConsiderTeleport(Vector3 start, ref Vector3 end)
	{
		if (base.LocomotionTeleport.AimCollisionTest(start, end, this.AimCollisionLayerMask, out this.AimData.TargetHitInfo))
		{
			Vector3 normalized = (end - start).normalized;
			end = start + normalized * this.AimData.TargetHitInfo.distance;
			return true;
		}
		return false;
	}

	// Token: 0x06001294 RID: 4756 RVA: 0x00057978 File Offset: 0x00055B78
	public override Vector3? ConsiderDestination(Vector3 location)
	{
		Vector3? vector = base.ConsiderDestination(location);
		if (vector != null)
		{
			Vector3 characterPosition = base.LocomotionTeleport.GetCharacterPosition();
			Vector3 valueOrDefault = vector.GetValueOrDefault();
			NavMesh.CalculatePath(characterPosition, valueOrDefault, this.NavMeshAreaMask, this._path);
			if (this._path.status == NavMeshPathStatus.PathComplete)
			{
				return vector;
			}
		}
		return null;
	}

	// Token: 0x06001295 RID: 4757 RVA: 0x000023F4 File Offset: 0x000005F4
	[Conditional("SHOW_PATH_RESULT")]
	private void OnDrawGizmos()
	{
	}

	// Token: 0x040014A1 RID: 5281
	public int NavMeshAreaMask = -1;

	// Token: 0x040014A2 RID: 5282
	private NavMeshPath _path;
}
