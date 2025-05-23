using System;
using UnityEngine;

// Token: 0x020000E8 RID: 232
public class SnowballGrabZone : HoldableObject
{
	// Token: 0x060005D4 RID: 1492 RVA: 0x000023F4 File Offset: 0x000005F4
	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
	}

	// Token: 0x060005D5 RID: 1493 RVA: 0x000023F4 File Offset: 0x000005F4
	public override void DropItemCleanup()
	{
	}

	// Token: 0x060005D6 RID: 1494 RVA: 0x00021D1C File Offset: 0x0001FF1C
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		SnowballThrowable snowballThrowable;
		((grabbingHand == EquipmentInteractor.instance.leftHand) ? SnowballMaker.leftHandInstance : SnowballMaker.rightHandInstance).TryCreateSnowball(this.materialIndex, out snowballThrowable);
	}

	// Token: 0x040006D9 RID: 1753
	[GorillaSoundLookup]
	public int materialIndex;
}
