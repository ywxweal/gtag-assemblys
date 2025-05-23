using System;
using UnityEngine;

// Token: 0x02000407 RID: 1031
public interface IHoldableObject
{
	// Token: 0x170002C5 RID: 709
	// (get) Token: 0x060018F4 RID: 6388
	GameObject gameObject { get; }

	// Token: 0x170002C6 RID: 710
	// (get) Token: 0x060018F5 RID: 6389
	// (set) Token: 0x060018F6 RID: 6390
	string name { get; set; }

	// Token: 0x170002C7 RID: 711
	// (get) Token: 0x060018F7 RID: 6391
	bool TwoHanded { get; }

	// Token: 0x060018F8 RID: 6392
	void OnHover(InteractionPoint pointHovered, GameObject hoveringHand);

	// Token: 0x060018F9 RID: 6393
	void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand);

	// Token: 0x060018FA RID: 6394
	bool OnRelease(DropZone zoneReleased, GameObject releasingHand);

	// Token: 0x060018FB RID: 6395
	void DropItemCleanup();
}
