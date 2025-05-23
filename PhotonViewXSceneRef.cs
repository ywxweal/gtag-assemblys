using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200023A RID: 570
public class PhotonViewXSceneRef : MonoBehaviour
{
	// Token: 0x17000145 RID: 325
	// (get) Token: 0x06000D24 RID: 3364 RVA: 0x00045290 File Offset: 0x00043490
	public PhotonView photonView
	{
		get
		{
			PhotonView photonView;
			if (this.reference.TryResolve<PhotonView>(out photonView))
			{
				return photonView;
			}
			return null;
		}
	}

	// Token: 0x040010B0 RID: 4272
	[SerializeField]
	private XSceneRef reference;
}
