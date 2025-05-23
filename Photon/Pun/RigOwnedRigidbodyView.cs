using System;
using GorillaExtensions;
using UnityEngine;

namespace Photon.Pun
{
	// Token: 0x02000AC0 RID: 2752
	[RequireComponent(typeof(Rigidbody))]
	public class RigOwnedRigidbodyView : MonoBehaviourPun, IPunObservable
	{
		// Token: 0x1700069E RID: 1694
		// (get) Token: 0x06004265 RID: 16997 RVA: 0x001328A0 File Offset: 0x00130AA0
		// (set) Token: 0x06004266 RID: 16998 RVA: 0x001328A8 File Offset: 0x00130AA8
		public bool IsMine { get; private set; }

		// Token: 0x06004267 RID: 16999 RVA: 0x001328B1 File Offset: 0x00130AB1
		public void SetIsMine(bool isMine)
		{
			this.IsMine = isMine;
		}

		// Token: 0x06004268 RID: 17000 RVA: 0x001328BA File Offset: 0x00130ABA
		public void Awake()
		{
			this.m_Body = base.GetComponent<Rigidbody>();
			this.m_NetworkPosition = default(Vector3);
			this.m_NetworkRotation = default(Quaternion);
		}

		// Token: 0x06004269 RID: 17001 RVA: 0x001328E0 File Offset: 0x00130AE0
		public void FixedUpdate()
		{
			if (!this.IsMine)
			{
				this.m_Body.position = Vector3.MoveTowards(this.m_Body.position, this.m_NetworkPosition, this.m_Distance * (1f / (float)PhotonNetwork.SerializationRate));
				this.m_Body.rotation = Quaternion.RotateTowards(this.m_Body.rotation, this.m_NetworkRotation, this.m_Angle * (1f / (float)PhotonNetwork.SerializationRate));
			}
		}

		// Token: 0x0600426A RID: 17002 RVA: 0x00132960 File Offset: 0x00130B60
		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			if (info.Sender != info.photonView.Owner)
			{
				return;
			}
			if (stream.IsWriting)
			{
				stream.SendNext(this.m_Body.position);
				stream.SendNext(this.m_Body.rotation);
				if (this.m_SynchronizeVelocity)
				{
					stream.SendNext(this.m_Body.velocity);
				}
				if (this.m_SynchronizeAngularVelocity)
				{
					stream.SendNext(this.m_Body.angularVelocity);
				}
				stream.SendNext(this.m_Body.IsSleeping());
				return;
			}
			Vector3 vector = (Vector3)stream.ReceiveNext();
			(ref this.m_NetworkPosition).SetValueSafe(in vector);
			Quaternion quaternion = (Quaternion)stream.ReceiveNext();
			(ref this.m_NetworkRotation).SetValueSafe(in quaternion);
			if (this.m_TeleportEnabled && Vector3.Distance(this.m_Body.position, this.m_NetworkPosition) > this.m_TeleportIfDistanceGreaterThan)
			{
				this.m_Body.position = this.m_NetworkPosition;
			}
			if (this.m_SynchronizeVelocity || this.m_SynchronizeAngularVelocity)
			{
				float num = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
				if (this.m_SynchronizeVelocity)
				{
					Vector3 vector2 = (Vector3)stream.ReceiveNext();
					float num2 = 10000f;
					if (!(in vector2).IsValid(in num2))
					{
						vector2 = Vector3.zero;
					}
					if (!this.m_Body.isKinematic)
					{
						this.m_Body.velocity = vector2;
					}
					this.m_NetworkPosition += this.m_Body.velocity * num;
					this.m_Distance = Vector3.Distance(this.m_Body.position, this.m_NetworkPosition);
				}
				if (this.m_SynchronizeAngularVelocity)
				{
					Vector3 vector3 = (Vector3)stream.ReceiveNext();
					float num2 = 10000f;
					if (!(in vector3).IsValid(in num2))
					{
						vector3 = Vector3.zero;
					}
					this.m_Body.angularVelocity = vector3;
					this.m_NetworkRotation = Quaternion.Euler(this.m_Body.angularVelocity * num) * this.m_NetworkRotation;
					this.m_Angle = Quaternion.Angle(this.m_Body.rotation, this.m_NetworkRotation);
				}
			}
			if ((bool)stream.ReceiveNext())
			{
				this.m_Body.Sleep();
			}
		}

		// Token: 0x040044BE RID: 17598
		private float m_Distance;

		// Token: 0x040044BF RID: 17599
		private float m_Angle;

		// Token: 0x040044C0 RID: 17600
		private Rigidbody m_Body;

		// Token: 0x040044C1 RID: 17601
		private Vector3 m_NetworkPosition;

		// Token: 0x040044C2 RID: 17602
		private Quaternion m_NetworkRotation;

		// Token: 0x040044C3 RID: 17603
		public bool m_SynchronizeVelocity = true;

		// Token: 0x040044C4 RID: 17604
		public bool m_SynchronizeAngularVelocity;

		// Token: 0x040044C5 RID: 17605
		public bool m_TeleportEnabled;

		// Token: 0x040044C6 RID: 17606
		public float m_TeleportIfDistanceGreaterThan = 3f;
	}
}
