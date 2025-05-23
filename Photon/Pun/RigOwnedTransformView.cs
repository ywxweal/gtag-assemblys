using System;
using GorillaExtensions;
using UnityEngine;

namespace Photon.Pun
{
	// Token: 0x02000AC1 RID: 2753
	[HelpURL("https://doc.photonengine.com/en-us/pun/v2/gameplay/synchronization-and-state")]
	public class RigOwnedTransformView : MonoBehaviourPun, IPunObservable
	{
		// Token: 0x1700069F RID: 1695
		// (get) Token: 0x0600426C RID: 17004 RVA: 0x00132BCB File Offset: 0x00130DCB
		// (set) Token: 0x0600426D RID: 17005 RVA: 0x00132BD3 File Offset: 0x00130DD3
		public bool IsMine { get; private set; }

		// Token: 0x0600426E RID: 17006 RVA: 0x00132BDC File Offset: 0x00130DDC
		public void SetIsMine(bool isMine)
		{
			this.IsMine = isMine;
		}

		// Token: 0x0600426F RID: 17007 RVA: 0x00132BE5 File Offset: 0x00130DE5
		public void Awake()
		{
			this.m_StoredPosition = base.transform.localPosition;
			this.m_NetworkPosition = Vector3.zero;
			this.m_networkScale = Vector3.one;
			this.m_NetworkRotation = Quaternion.identity;
		}

		// Token: 0x06004270 RID: 17008 RVA: 0x00132C19 File Offset: 0x00130E19
		private void Reset()
		{
			this.m_UseLocal = true;
		}

		// Token: 0x06004271 RID: 17009 RVA: 0x00132C22 File Offset: 0x00130E22
		private void OnEnable()
		{
			this.m_firstTake = true;
		}

		// Token: 0x06004272 RID: 17010 RVA: 0x00132C2C File Offset: 0x00130E2C
		public void Update()
		{
			Transform transform = base.transform;
			if (!this.IsMine && this.IsValid(this.m_NetworkPosition) && this.IsValid(this.m_NetworkRotation))
			{
				if (this.m_UseLocal)
				{
					transform.localPosition = Vector3.MoveTowards(transform.localPosition, this.m_NetworkPosition, this.m_Distance * Time.deltaTime * (float)PhotonNetwork.SerializationRate);
					transform.localRotation = Quaternion.RotateTowards(transform.localRotation, this.m_NetworkRotation, this.m_Angle * Time.deltaTime * (float)PhotonNetwork.SerializationRate);
					return;
				}
				transform.position = Vector3.MoveTowards(transform.position, this.m_NetworkPosition, this.m_Distance * Time.deltaTime * (float)PhotonNetwork.SerializationRate);
				transform.rotation = Quaternion.RotateTowards(transform.rotation, this.m_NetworkRotation, this.m_Angle * Time.deltaTime * (float)PhotonNetwork.SerializationRate);
			}
		}

		// Token: 0x06004273 RID: 17011 RVA: 0x00132D20 File Offset: 0x00130F20
		private bool IsValid(Vector3 v)
		{
			return !float.IsNaN(v.x) && !float.IsNaN(v.y) && !float.IsNaN(v.z) && !float.IsInfinity(v.x) && !float.IsInfinity(v.y) && !float.IsInfinity(v.z);
		}

		// Token: 0x06004274 RID: 17012 RVA: 0x00132D80 File Offset: 0x00130F80
		private bool IsValid(Quaternion q)
		{
			return !float.IsNaN(q.x) && !float.IsNaN(q.y) && !float.IsNaN(q.z) && !float.IsNaN(q.w) && !float.IsInfinity(q.x) && !float.IsInfinity(q.y) && !float.IsInfinity(q.z) && !float.IsInfinity(q.w);
		}

		// Token: 0x06004275 RID: 17013 RVA: 0x00132DF8 File Offset: 0x00130FF8
		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			if (info.Sender != info.photonView.Owner)
			{
				return;
			}
			Transform transform = base.transform;
			if (stream.IsWriting)
			{
				if (this.m_SynchronizePosition)
				{
					if (this.m_UseLocal)
					{
						this.m_Direction = transform.localPosition - this.m_StoredPosition;
						this.m_StoredPosition = transform.localPosition;
						stream.SendNext(transform.localPosition);
						stream.SendNext(this.m_Direction);
					}
					else
					{
						this.m_Direction = transform.position - this.m_StoredPosition;
						this.m_StoredPosition = transform.position;
						stream.SendNext(transform.position);
						stream.SendNext(this.m_Direction);
					}
				}
				if (this.m_SynchronizeRotation)
				{
					if (this.m_UseLocal)
					{
						stream.SendNext(transform.localRotation);
					}
					else
					{
						stream.SendNext(transform.rotation);
					}
				}
				if (this.m_SynchronizeScale)
				{
					stream.SendNext(transform.localScale);
					return;
				}
			}
			else
			{
				if (this.m_SynchronizePosition)
				{
					Vector3 vector = (Vector3)stream.ReceiveNext();
					(ref this.m_NetworkPosition).SetValueSafe(in vector);
					vector = (Vector3)stream.ReceiveNext();
					(ref this.m_Direction).SetValueSafe(in vector);
					if (this.m_firstTake)
					{
						if (this.m_UseLocal)
						{
							transform.localPosition = this.m_NetworkPosition;
						}
						else
						{
							transform.position = this.m_NetworkPosition;
						}
						this.m_Distance = 0f;
					}
					else
					{
						float num = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
						this.m_NetworkPosition += this.m_Direction * num;
						if (this.m_UseLocal)
						{
							this.m_Distance = Vector3.Distance(transform.localPosition, this.m_NetworkPosition);
						}
						else
						{
							this.m_Distance = Vector3.Distance(transform.position, this.m_NetworkPosition);
						}
					}
				}
				if (this.m_SynchronizeRotation)
				{
					Quaternion quaternion = (Quaternion)stream.ReceiveNext();
					(ref this.m_NetworkRotation).SetValueSafe(in quaternion);
					if (this.m_firstTake)
					{
						this.m_Angle = 0f;
						if (this.m_UseLocal)
						{
							transform.localRotation = this.m_NetworkRotation;
						}
						else
						{
							transform.rotation = this.m_NetworkRotation;
						}
					}
					else if (this.m_UseLocal)
					{
						this.m_Angle = Quaternion.Angle(transform.localRotation, this.m_NetworkRotation);
					}
					else
					{
						this.m_Angle = Quaternion.Angle(transform.rotation, this.m_NetworkRotation);
					}
				}
				if (this.m_SynchronizeScale)
				{
					Vector3 vector = (Vector3)stream.ReceiveNext();
					(ref this.m_networkScale).SetValueSafe(in vector);
					transform.localScale = this.m_networkScale;
				}
				if (this.m_firstTake)
				{
					this.m_firstTake = false;
				}
			}
		}

		// Token: 0x06004276 RID: 17014 RVA: 0x00132C22 File Offset: 0x00130E22
		public void GTAddition_DoTeleport()
		{
			this.m_firstTake = true;
		}

		// Token: 0x040044C8 RID: 17608
		private float m_Distance;

		// Token: 0x040044C9 RID: 17609
		private float m_Angle;

		// Token: 0x040044CA RID: 17610
		private Vector3 m_Direction;

		// Token: 0x040044CB RID: 17611
		private Vector3 m_NetworkPosition;

		// Token: 0x040044CC RID: 17612
		private Vector3 m_StoredPosition;

		// Token: 0x040044CD RID: 17613
		private Vector3 m_networkScale;

		// Token: 0x040044CE RID: 17614
		private Quaternion m_NetworkRotation;

		// Token: 0x040044CF RID: 17615
		public bool m_SynchronizePosition = true;

		// Token: 0x040044D0 RID: 17616
		public bool m_SynchronizeRotation = true;

		// Token: 0x040044D1 RID: 17617
		public bool m_SynchronizeScale;

		// Token: 0x040044D2 RID: 17618
		[Tooltip("Indicates if localPosition and localRotation should be used. Scale ignores this setting, and always uses localScale to avoid issues with lossyScale.")]
		public bool m_UseLocal;

		// Token: 0x040044D3 RID: 17619
		private bool m_firstTake;
	}
}
