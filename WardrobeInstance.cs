using System;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000456 RID: 1110
public class WardrobeInstance : MonoBehaviour
{
	// Token: 0x06001B5A RID: 7002 RVA: 0x00086ACF File Offset: 0x00084CCF
	public void Start()
	{
		CosmeticsController.instance.AddWardrobeInstance(this);
	}

	// Token: 0x06001B5B RID: 7003 RVA: 0x00086ADE File Offset: 0x00084CDE
	public void OnDestroy()
	{
		CosmeticsController.instance.RemoveWardrobeInstance(this);
	}

	// Token: 0x04001E52 RID: 7762
	public WardrobeItemButton[] wardrobeItemButtons;

	// Token: 0x04001E53 RID: 7763
	public HeadModel selfDoll;
}
