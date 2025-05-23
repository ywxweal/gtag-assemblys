using System;
using System.Runtime.InteropServices;
using Fusion;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x020005D9 RID: 1497
[NetworkBehaviourWeaved(15)]
internal class GorillaNetworkTransform : NetworkComponent
{
	// Token: 0x17000370 RID: 880
	// (get) Token: 0x06002489 RID: 9353 RVA: 0x000B7AD3 File Offset: 0x000B5CD3
	public bool RespectOwnership
	{
		get
		{
			return this.respectOwnership;
		}
	}

	// Token: 0x17000371 RID: 881
	// (get) Token: 0x0600248A RID: 9354 RVA: 0x000B7ADB File Offset: 0x000B5CDB
	// (set) Token: 0x0600248B RID: 9355 RVA: 0x000B7B05 File Offset: 0x000B5D05
	[Networked]
	[NetworkedWeaved(0, 15)]
	private unsafe GorillaNetworkTransform.NetTransformData data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GorillaNetworkTransform.data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(GorillaNetworkTransform.NetTransformData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GorillaNetworkTransform.data. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(GorillaNetworkTransform.NetTransformData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x0600248C RID: 9356 RVA: 0x000B7B30 File Offset: 0x000B5D30
	public new void Awake()
	{
		this.m_StoredPosition = base.transform.localPosition;
		this.m_NetworkPosition = Vector3.zero;
		this.m_NetworkScale = Vector3.zero;
		this.m_NetworkRotation = Quaternion.identity;
		this.maxDistanceSquare = this.maxDistance * this.maxDistance;
	}

	// Token: 0x0600248D RID: 9357 RVA: 0x000B7B82 File Offset: 0x000B5D82
	private new void OnEnable()
	{
		NetworkBehaviourUtils.InternalOnEnable(this);
		this.m_firstTake = true;
		if (this.clampToSpawn)
		{
			this.clampOriginPoint = (this.m_UseLocal ? base.transform.localPosition : base.transform.position);
		}
	}

	// Token: 0x0600248E RID: 9358 RVA: 0x000B7BC0 File Offset: 0x000B5DC0
	public void Update()
	{
		if (!base.IsLocallyOwned)
		{
			if (this.m_UseLocal)
			{
				base.transform.localPosition = Vector3.MoveTowards(base.transform.localPosition, this.m_NetworkPosition, this.m_Distance * Time.deltaTime * (float)NetworkSystem.Instance.TickRate);
				base.transform.localRotation = Quaternion.RotateTowards(base.transform.localRotation, this.m_NetworkRotation, this.m_Angle * Time.deltaTime * (float)NetworkSystem.Instance.TickRate);
				return;
			}
			base.transform.position = Vector3.MoveTowards(base.transform.position, this.m_NetworkPosition, this.m_Distance * Time.deltaTime * (float)NetworkSystem.Instance.TickRate);
			base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, this.m_NetworkRotation, this.m_Angle * Time.deltaTime * (float)NetworkSystem.Instance.TickRate);
		}
	}

	// Token: 0x0600248F RID: 9359 RVA: 0x000B7CC8 File Offset: 0x000B5EC8
	public override void WriteDataFusion()
	{
		GorillaNetworkTransform.NetTransformData netTransformData = this.SharedWrite();
		double num = NetworkSystem.Instance.SimTick / 1000.0;
		netTransformData.SentTime = num;
		this.data = netTransformData;
	}

	// Token: 0x06002490 RID: 9360 RVA: 0x000B7D02 File Offset: 0x000B5F02
	public override void ReadDataFusion()
	{
		this.SharedRead(this.data);
	}

	// Token: 0x06002491 RID: 9361 RVA: 0x000B7D10 File Offset: 0x000B5F10
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
		if (this.respectOwnership && player != base.Owner)
		{
			return;
		}
		GorillaNetworkTransform.NetTransformData netTransformData = this.SharedWrite();
		if (this.m_SynchronizePosition)
		{
			stream.SendNext(netTransformData.position);
			stream.SendNext(netTransformData.velocity);
		}
		if (this.m_SynchronizeRotation)
		{
			stream.SendNext(netTransformData.rotation);
		}
		if (this.m_SynchronizeScale)
		{
			stream.SendNext(netTransformData.scale);
		}
	}

	// Token: 0x06002492 RID: 9362 RVA: 0x000B7DA4 File Offset: 0x000B5FA4
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
		if (this.respectOwnership && player != base.Owner)
		{
			return;
		}
		GorillaNetworkTransform.NetTransformData netTransformData = default(GorillaNetworkTransform.NetTransformData);
		if (this.m_SynchronizePosition)
		{
			netTransformData.position = (Vector3)stream.ReceiveNext();
			netTransformData.velocity = (Vector3)stream.ReceiveNext();
		}
		if (this.m_SynchronizeRotation)
		{
			netTransformData.rotation = (Quaternion)stream.ReceiveNext();
		}
		if (this.m_SynchronizeScale)
		{
			netTransformData.scale = (Vector3)stream.ReceiveNext();
		}
		netTransformData.SentTime = (double)((float)info.SentServerTime);
		this.SharedRead(netTransformData);
	}

	// Token: 0x06002493 RID: 9363 RVA: 0x000B7E54 File Offset: 0x000B6054
	private void SharedRead(GorillaNetworkTransform.NetTransformData data)
	{
		if (this.m_SynchronizePosition)
		{
			(ref this.m_NetworkPosition).SetValueSafe(in data.position);
			(ref this.m_Velocity).SetValueSafe(in data.velocity);
			if (this.clampDistanceFromSpawn && Vector3.SqrMagnitude(this.clampOriginPoint - this.m_NetworkPosition) > this.maxDistanceSquare)
			{
				this.m_NetworkPosition = this.clampOriginPoint + this.m_Velocity.normalized * this.maxDistance;
				this.m_Velocity = Vector3.zero;
			}
			if (this.m_firstTake)
			{
				if (this.m_UseLocal)
				{
					base.transform.localPosition = this.m_NetworkPosition;
				}
				else
				{
					base.transform.position = this.m_NetworkPosition;
				}
				this.m_Distance = 0f;
			}
			else
			{
				float num = Mathf.Abs((float)(NetworkSystem.Instance.SimTime - data.SentTime));
				this.m_NetworkPosition += this.m_Velocity * num;
				if (this.m_UseLocal)
				{
					this.m_Distance = Vector3.Distance(base.transform.localPosition, this.m_NetworkPosition);
				}
				else
				{
					this.m_Distance = Vector3.Distance(base.transform.position, this.m_NetworkPosition);
				}
			}
		}
		if (this.m_SynchronizeRotation)
		{
			(ref this.m_NetworkRotation).SetValueSafe(in data.rotation);
			if (this.m_firstTake)
			{
				this.m_Angle = 0f;
				if (this.m_UseLocal)
				{
					base.transform.localRotation = this.m_NetworkRotation;
				}
				else
				{
					base.transform.rotation = this.m_NetworkRotation;
				}
			}
			else if (this.m_UseLocal)
			{
				this.m_Angle = Quaternion.Angle(base.transform.localRotation, this.m_NetworkRotation);
			}
			else
			{
				this.m_Angle = Quaternion.Angle(base.transform.rotation, this.m_NetworkRotation);
			}
		}
		if (this.m_SynchronizeScale)
		{
			(ref this.m_NetworkScale).SetValueSafe(in data.scale);
			base.transform.localScale = this.m_NetworkScale;
		}
		if (this.m_firstTake)
		{
			this.m_firstTake = false;
		}
	}

	// Token: 0x06002494 RID: 9364 RVA: 0x000B807C File Offset: 0x000B627C
	private GorillaNetworkTransform.NetTransformData SharedWrite()
	{
		GorillaNetworkTransform.NetTransformData netTransformData = default(GorillaNetworkTransform.NetTransformData);
		if (this.m_SynchronizePosition)
		{
			if (this.m_UseLocal)
			{
				this.m_Velocity = base.transform.localPosition - this.m_StoredPosition;
				this.m_StoredPosition = base.transform.localPosition;
				netTransformData.position = base.transform.localPosition;
				netTransformData.velocity = this.m_Velocity;
			}
			else
			{
				this.m_Velocity = base.transform.position - this.m_StoredPosition;
				this.m_StoredPosition = base.transform.position;
				netTransformData.position = base.transform.position;
				netTransformData.velocity = this.m_Velocity;
			}
		}
		if (this.m_SynchronizeRotation)
		{
			if (this.m_UseLocal)
			{
				netTransformData.rotation = base.transform.localRotation;
			}
			else
			{
				netTransformData.rotation = base.transform.rotation;
			}
		}
		if (this.m_SynchronizeScale)
		{
			netTransformData.scale = base.transform.localScale;
		}
		return netTransformData;
	}

	// Token: 0x06002495 RID: 9365 RVA: 0x000B818F File Offset: 0x000B638F
	public void GTAddition_DoTeleport()
	{
		this.m_firstTake = true;
	}

	// Token: 0x06002497 RID: 9367 RVA: 0x000B81C7 File Offset: 0x000B63C7
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.data = this._data;
	}

	// Token: 0x06002498 RID: 9368 RVA: 0x000B81DF File Offset: 0x000B63DF
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._data = this.data;
	}

	// Token: 0x04002997 RID: 10647
	[Tooltip("Indicates if localPosition and localRotation should be used. Scale ignores this setting, and always uses localScale to avoid issues with lossyScale.")]
	public bool m_UseLocal;

	// Token: 0x04002998 RID: 10648
	[SerializeField]
	private bool respectOwnership;

	// Token: 0x04002999 RID: 10649
	[SerializeField]
	private bool clampDistanceFromSpawn = true;

	// Token: 0x0400299A RID: 10650
	[SerializeField]
	private float maxDistance = 100f;

	// Token: 0x0400299B RID: 10651
	private float maxDistanceSquare;

	// Token: 0x0400299C RID: 10652
	[SerializeField]
	private bool clampToSpawn = true;

	// Token: 0x0400299D RID: 10653
	[Tooltip("Use this if clampToSpawn is false, to set the center point to check the synced position against")]
	[SerializeField]
	private Vector3 clampOriginPoint;

	// Token: 0x0400299E RID: 10654
	public bool m_SynchronizePosition = true;

	// Token: 0x0400299F RID: 10655
	public bool m_SynchronizeRotation = true;

	// Token: 0x040029A0 RID: 10656
	public bool m_SynchronizeScale;

	// Token: 0x040029A1 RID: 10657
	private float m_Distance;

	// Token: 0x040029A2 RID: 10658
	private float m_Angle;

	// Token: 0x040029A3 RID: 10659
	private Vector3 m_Velocity;

	// Token: 0x040029A4 RID: 10660
	private Vector3 m_NetworkPosition;

	// Token: 0x040029A5 RID: 10661
	private Vector3 m_StoredPosition;

	// Token: 0x040029A6 RID: 10662
	private Vector3 m_NetworkScale;

	// Token: 0x040029A7 RID: 10663
	private Quaternion m_NetworkRotation;

	// Token: 0x040029A8 RID: 10664
	private bool m_firstTake;

	// Token: 0x040029A9 RID: 10665
	[WeaverGenerated]
	[DefaultForProperty("data", 0, 15)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private GorillaNetworkTransform.NetTransformData _data;

	// Token: 0x020005DA RID: 1498
	[NetworkStructWeaved(15)]
	[StructLayout(LayoutKind.Explicit, Size = 60)]
	private struct NetTransformData : INetworkStruct
	{
		// Token: 0x040029AA RID: 10666
		[FieldOffset(0)]
		public Vector3 position;

		// Token: 0x040029AB RID: 10667
		[FieldOffset(12)]
		public Vector3 velocity;

		// Token: 0x040029AC RID: 10668
		[FieldOffset(24)]
		public Quaternion rotation;

		// Token: 0x040029AD RID: 10669
		[FieldOffset(40)]
		public Vector3 scale;

		// Token: 0x040029AE RID: 10670
		[FieldOffset(52)]
		public double SentTime;
	}
}
