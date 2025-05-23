using System;
using GorillaExtensions;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000ADD RID: 2781
	public class BuilderItemReliableState : MonoBehaviour, IPunObservable
	{
		// Token: 0x06004360 RID: 17248 RVA: 0x001379B4 File Offset: 0x00135BB4
		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			if (stream.IsWriting)
			{
				stream.SendNext(this.rightHandAttachPos);
				stream.SendNext(this.rightHandAttachRot);
				stream.SendNext(this.leftHandAttachPos);
				stream.SendNext(this.leftHandAttachRot);
				return;
			}
			this.rightHandAttachPos = (Vector3)stream.ReceiveNext();
			this.rightHandAttachRot = (Quaternion)stream.ReceiveNext();
			this.leftHandAttachPos = (Vector3)stream.ReceiveNext();
			this.leftHandAttachRot = (Quaternion)stream.ReceiveNext();
			float num = 10000f;
			if (!(in this.rightHandAttachPos).IsValid(in num))
			{
				this.rightHandAttachPos = Vector3.zero;
			}
			if (!(in this.rightHandAttachRot).IsValid())
			{
				this.rightHandAttachRot = quaternion.identity;
			}
			num = 10000f;
			if (!(in this.leftHandAttachPos).IsValid(in num))
			{
				this.leftHandAttachPos = Vector3.zero;
			}
			if (!(in this.leftHandAttachRot).IsValid())
			{
				this.leftHandAttachRot = quaternion.identity;
			}
			this.dirty = true;
		}

		// Token: 0x040045E5 RID: 17893
		public Vector3 rightHandAttachPos = Vector3.zero;

		// Token: 0x040045E6 RID: 17894
		public Quaternion rightHandAttachRot = Quaternion.identity;

		// Token: 0x040045E7 RID: 17895
		public Vector3 leftHandAttachPos = Vector3.zero;

		// Token: 0x040045E8 RID: 17896
		public Quaternion leftHandAttachRot = Quaternion.identity;

		// Token: 0x040045E9 RID: 17897
		public bool dirty;
	}
}
