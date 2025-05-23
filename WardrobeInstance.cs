using System;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000456 RID: 1110
public class WardrobeInstance : MonoBehaviour
{
	// Token: 0x06001B5A RID: 7002 RVA: 0x00086AEF File Offset: 0x00084CEF
	public void Start()
	{
		CosmeticsController.instance.AddWardrobeInstance(this);
	}

	// Token: 0x06001B5B RID: 7003 RVA: 0x00086AFE File Offset: 0x00084CFE
	public void OnDestroy()
	{
		CosmeticsController.instance.RemoveWardrobeInstance(this);
	}

	// Token: 0x04001E52 RID: 7762
	public WardrobeItemButton[] wardrobeItemButtons;

	// Token: 0x04001E53 RID: 7763
	public HeadModel selfDoll;
}
