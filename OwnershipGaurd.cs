using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020008EA RID: 2282
internal class OwnershipGaurd : MonoBehaviour
{
	// Token: 0x06003771 RID: 14193 RVA: 0x0010BF57 File Offset: 0x0010A157
	private void Start()
	{
		if (this.autoRegisterAll)
		{
			this.NetViews = base.GetComponents<PhotonView>();
		}
		if (this.NetViews == null)
		{
			return;
		}
		OwnershipGaurdHandler.RegisterViews(this.NetViews);
	}

	// Token: 0x06003772 RID: 14194 RVA: 0x0010BF81 File Offset: 0x0010A181
	private void OnDestroy()
	{
		if (this.NetViews == null)
		{
			return;
		}
		OwnershipGaurdHandler.RemoveViews(this.NetViews);
	}

	// Token: 0x04003D00 RID: 15616
	[SerializeField]
	private PhotonView[] NetViews;

	// Token: 0x04003D01 RID: 15617
	[SerializeField]
	private bool autoRegisterAll = true;
}
