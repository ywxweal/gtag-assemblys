using System;
using Cinemachine.Utility;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x02000190 RID: 400
public class LookDirectionStabilizer : MonoBehaviour, ISpawnable
{
	// Token: 0x170000F6 RID: 246
	// (get) Token: 0x060009DC RID: 2524 RVA: 0x00034842 File Offset: 0x00032A42
	// (set) Token: 0x060009DD RID: 2525 RVA: 0x0003484A File Offset: 0x00032A4A
	public bool IsSpawned { get; set; }

	// Token: 0x170000F7 RID: 247
	// (get) Token: 0x060009DE RID: 2526 RVA: 0x00034853 File Offset: 0x00032A53
	// (set) Token: 0x060009DF RID: 2527 RVA: 0x0003485B File Offset: 0x00032A5B
	public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

	// Token: 0x060009E0 RID: 2528 RVA: 0x00034864 File Offset: 0x00032A64
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this.myRig = rig;
	}

	// Token: 0x060009E1 RID: 2529 RVA: 0x000023F4 File Offset: 0x000005F4
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x060009E2 RID: 2530 RVA: 0x00034870 File Offset: 0x00032A70
	private void Update()
	{
		Transform rigTarget = this.myRig.head.rigTarget;
		if (rigTarget.forward.y < 0f)
		{
			Quaternion quaternion = Quaternion.LookRotation(rigTarget.up.ProjectOntoPlane(Vector3.up));
			Quaternion rotation = base.transform.parent.rotation;
			float num = Vector3.Dot(rigTarget.up, Vector3.up);
			base.transform.rotation = Quaternion.Lerp(rotation, quaternion, Mathf.InverseLerp(1f, 0.7f, num));
			return;
		}
		base.transform.localRotation = Quaternion.identity;
	}

	// Token: 0x04000C0B RID: 3083
	private VRRig myRig;
}
