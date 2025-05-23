using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020005E4 RID: 1508
public class PhotonViewCache : MonoBehaviour, IPunInstantiateMagicCallback
{
	// Token: 0x17000378 RID: 888
	// (get) Token: 0x060024E1 RID: 9441 RVA: 0x000B8D1C File Offset: 0x000B6F1C
	// (set) Token: 0x060024E2 RID: 9442 RVA: 0x000B8D24 File Offset: 0x000B6F24
	public bool Initialized { get; private set; }

	// Token: 0x060024E3 RID: 9443 RVA: 0x000023F4 File Offset: 0x000005F4
	void IPunInstantiateMagicCallback.OnPhotonInstantiate(PhotonMessageInfo info)
	{
	}

	// Token: 0x040029CE RID: 10702
	private PhotonView[] m_photonViews;

	// Token: 0x040029CF RID: 10703
	[SerializeField]
	private bool m_isRoomObject;
}
