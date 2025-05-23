using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020008EA RID: 2282
internal class OwnershipGaurd : MonoBehaviour
{
	// Token: 0x06003770 RID: 14192 RVA: 0x0010BE7F File Offset: 0x0010A07F
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

	// Token: 0x06003771 RID: 14193 RVA: 0x0010BEA9 File Offset: 0x0010A0A9
	private void OnDestroy()
	{
		if (this.NetViews == null)
		{
			return;
		}
		OwnershipGaurdHandler.RemoveViews(this.NetViews);
	}

	// Token: 0x04003CFF RID: 15615
	[SerializeField]
	private PhotonView[] NetViews;

	// Token: 0x04003D00 RID: 15616
	[SerializeField]
	private bool autoRegisterAll = true;
}
