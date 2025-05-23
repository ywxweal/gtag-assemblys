using System;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000A12 RID: 2578
[NetworkBehaviourWeaved(3)]
public class ThrowableBugReliableState : NetworkComponent, IRequestableOwnershipGuardCallbacks
{
	// Token: 0x17000603 RID: 1539
	// (get) Token: 0x06003D8E RID: 15758 RVA: 0x00124235 File Offset: 0x00122435
	// (set) Token: 0x06003D8F RID: 15759 RVA: 0x0012425F File Offset: 0x0012245F
	[Networked]
	[NetworkedWeaved(0, 3)]
	public unsafe ThrowableBugReliableState.BugData Data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing ThrowableBugReliableState.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(ThrowableBugReliableState.BugData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing ThrowableBugReliableState.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(ThrowableBugReliableState.BugData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x06003D90 RID: 15760 RVA: 0x0012428A File Offset: 0x0012248A
	public override void WriteDataFusion()
	{
		this.Data = new ThrowableBugReliableState.BugData(this.travelingDirection);
	}

	// Token: 0x06003D91 RID: 15761 RVA: 0x001242A0 File Offset: 0x001224A0
	public override void ReadDataFusion()
	{
		this.travelingDirection = this.Data.tDirection;
	}

	// Token: 0x06003D92 RID: 15762 RVA: 0x001242C1 File Offset: 0x001224C1
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		stream.SendNext(this.travelingDirection);
	}

	// Token: 0x06003D93 RID: 15763 RVA: 0x001242D4 File Offset: 0x001224D4
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		Vector3 vector = (Vector3)stream.ReceiveNext();
		(ref this.travelingDirection).SetValueSafe(in vector);
	}

	// Token: 0x06003D94 RID: 15764 RVA: 0x00002628 File Offset: 0x00000828
	public void OnOwnershipTransferred(NetPlayer toPlayer, NetPlayer fromPlayer)
	{
		throw new NotImplementedException();
	}

	// Token: 0x06003D95 RID: 15765 RVA: 0x00002628 File Offset: 0x00000828
	public bool OnOwnershipRequest(NetPlayer fromPlayer)
	{
		throw new NotImplementedException();
	}

	// Token: 0x06003D96 RID: 15766 RVA: 0x00002628 File Offset: 0x00000828
	public void OnMyOwnerLeft()
	{
		throw new NotImplementedException();
	}

	// Token: 0x06003D97 RID: 15767 RVA: 0x00002628 File Offset: 0x00000828
	public bool OnMasterClientAssistedTakeoverRequest(NetPlayer fromPlayer, NetPlayer toPlayer)
	{
		throw new NotImplementedException();
	}

	// Token: 0x06003D98 RID: 15768 RVA: 0x00002628 File Offset: 0x00000828
	public void OnMyCreatorLeft()
	{
		throw new NotImplementedException();
	}

	// Token: 0x06003D9A RID: 15770 RVA: 0x0012430D File Offset: 0x0012250D
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Data = this._Data;
	}

	// Token: 0x06003D9B RID: 15771 RVA: 0x00124325 File Offset: 0x00122525
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Data = this.Data;
	}

	// Token: 0x04004168 RID: 16744
	public Vector3 travelingDirection = Vector3.zero;

	// Token: 0x04004169 RID: 16745
	[WeaverGenerated]
	[SerializeField]
	[DefaultForProperty("Data", 0, 3)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private ThrowableBugReliableState.BugData _Data;

	// Token: 0x02000A13 RID: 2579
	[NetworkStructWeaved(3)]
	[StructLayout(LayoutKind.Explicit, Size = 12)]
	public struct BugData : INetworkStruct
	{
		// Token: 0x17000604 RID: 1540
		// (get) Token: 0x06003D9C RID: 15772 RVA: 0x00124339 File Offset: 0x00122539
		// (set) Token: 0x06003D9D RID: 15773 RVA: 0x0012434B File Offset: 0x0012254B
		[Networked]
		public unsafe Vector3 tDirection
		{
			readonly get
			{
				return *(Vector3*)Native.ReferenceToPointer<FixedStorage@3>(ref this._tDirection);
			}
			set
			{
				*(Vector3*)Native.ReferenceToPointer<FixedStorage@3>(ref this._tDirection) = value;
			}
		}

		// Token: 0x06003D9E RID: 15774 RVA: 0x0012435E File Offset: 0x0012255E
		public BugData(Vector3 dir)
		{
			this.tDirection = dir;
		}

		// Token: 0x0400416A RID: 16746
		[FixedBufferProperty(typeof(Vector3), typeof(UnityValueSurrogate@ReaderWriter@UnityEngine_Vector3), 0, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(0)]
		private FixedStorage@3 _tDirection;
	}
}
