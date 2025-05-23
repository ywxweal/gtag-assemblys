using System;
using GorillaExtensions;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000B02 RID: 2818
	public class DecorativeItemReliableState : MonoBehaviour, IPunObservable
	{
		// Token: 0x060044F3 RID: 17651 RVA: 0x001467F0 File Offset: 0x001449F0
		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			if (stream.IsWriting)
			{
				stream.SendNext(this.isSnapped);
				stream.SendNext(this.snapPosition);
				stream.SendNext(this.respawnPosition);
				stream.SendNext(this.respawnRotation);
				return;
			}
			this.isSnapped = (bool)stream.ReceiveNext();
			this.snapPosition = (Vector3)stream.ReceiveNext();
			this.respawnPosition = (Vector3)stream.ReceiveNext();
			this.respawnRotation = (Quaternion)stream.ReceiveNext();
			float num = 10000f;
			if (!(in this.snapPosition).IsValid(in num))
			{
				this.snapPosition = Vector3.zero;
			}
			num = 10000f;
			if (!(in this.respawnPosition).IsValid(in num))
			{
				this.respawnPosition = Vector3.zero;
			}
			if (!(in this.respawnRotation).IsValid())
			{
				this.respawnRotation = quaternion.identity;
			}
		}

		// Token: 0x040047AF RID: 18351
		public bool isSnapped;

		// Token: 0x040047B0 RID: 18352
		public Vector3 snapPosition = Vector3.zero;

		// Token: 0x040047B1 RID: 18353
		public Vector3 respawnPosition = Vector3.zero;

		// Token: 0x040047B2 RID: 18354
		public Quaternion respawnRotation = Quaternion.identity;
	}
}
