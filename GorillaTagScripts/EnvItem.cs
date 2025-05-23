using System;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000B04 RID: 2820
	public class EnvItem : MonoBehaviour, IPunInstantiateMagicCallback
	{
		// Token: 0x0600450C RID: 17676 RVA: 0x000023F4 File Offset: 0x000005F4
		public void OnEnable()
		{
		}

		// Token: 0x0600450D RID: 17677 RVA: 0x000023F4 File Offset: 0x000005F4
		public void OnDisable()
		{
		}

		// Token: 0x0600450E RID: 17678 RVA: 0x0014733C File Offset: 0x0014553C
		public void OnPhotonInstantiate(PhotonMessageInfo info)
		{
			object[] instantiationData = info.photonView.InstantiationData;
			this.spawnedByPhotonViewId = (int)instantiationData[0];
		}

		// Token: 0x040047C2 RID: 18370
		public int spawnedByPhotonViewId;
	}
}
