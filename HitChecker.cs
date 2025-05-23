using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000404 RID: 1028
public class HitChecker : MonoBehaviour
{
	// Token: 0x060018E4 RID: 6372 RVA: 0x00078CF0 File Offset: 0x00076EF0
	public static void CheckHandHit(ref int collidersHitCount, LayerMask layerMask, float sphereRadius, ref RaycastHit nullHit, ref RaycastHit[] raycastHits, ref List<RaycastHit> raycastHitList, ref Vector3 spherecastSweep, ref GorillaTriggerColliderHandIndicator handIndicator)
	{
		spherecastSweep = handIndicator.transform.position - handIndicator.lastPosition;
		if (spherecastSweep.magnitude < 0.0001f)
		{
			spherecastSweep = Vector3.up * 0.0001f;
		}
		for (int i = 0; i < raycastHits.Length; i++)
		{
			raycastHits[i] = nullHit;
		}
		collidersHitCount = Physics.SphereCastNonAlloc(handIndicator.lastPosition, sphereRadius, spherecastSweep.normalized, raycastHits, spherecastSweep.magnitude, layerMask, QueryTriggerInteraction.Collide);
		if (collidersHitCount > 0)
		{
			raycastHitList.Clear();
			for (int j = 0; j < raycastHits.Length; j++)
			{
				if (raycastHits[j].collider != null)
				{
					raycastHitList.Add(raycastHits[j]);
				}
			}
		}
	}

	// Token: 0x060018E5 RID: 6373 RVA: 0x00078DD0 File Offset: 0x00076FD0
	public static bool CheckHandIn(ref bool anyHit, ref Collider[] colliderHit, float sphereRadius, int layerMask, ref GorillaTriggerColliderHandIndicator handIndicator, ref List<Collider> collidersToBeIn)
	{
		anyHit = Physics.OverlapSphereNonAlloc(handIndicator.transform.position, sphereRadius, colliderHit, layerMask, QueryTriggerInteraction.Collide) > 0;
		if (anyHit)
		{
			anyHit = false;
			for (int i = 0; i < colliderHit.Length; i++)
			{
				if (collidersToBeIn.Contains(colliderHit[i]))
				{
					anyHit = true;
					break;
				}
			}
		}
		return anyHit;
	}

	// Token: 0x060018E6 RID: 6374 RVA: 0x00078E24 File Offset: 0x00077024
	public static int RayCastHitCompare(RaycastHit a, RaycastHit b)
	{
		if (a.distance < b.distance)
		{
			return -1;
		}
		if (a.distance == b.distance)
		{
			return 0;
		}
		return 1;
	}
}
