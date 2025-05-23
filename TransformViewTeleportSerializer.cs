using System;
using Fusion;
using Photon.Pun;
using UnityEngine;

// Token: 0x020008F6 RID: 2294
[NetworkBehaviourWeaved(1)]
public class TransformViewTeleportSerializer : NetworkComponent
{
	// Token: 0x06003799 RID: 14233 RVA: 0x0010C309 File Offset: 0x0010A509
	protected override void Start()
	{
		base.Start();
		this.transformView = base.GetComponent<GorillaNetworkTransform>();
	}

	// Token: 0x0600379A RID: 14234 RVA: 0x0010C31D File Offset: 0x0010A51D
	public void SetWillTeleport()
	{
		this.willTeleport = true;
	}

	// Token: 0x1700057E RID: 1406
	// (get) Token: 0x0600379B RID: 14235 RVA: 0x0010C326 File Offset: 0x0010A526
	// (set) Token: 0x0600379C RID: 14236 RVA: 0x0010C350 File Offset: 0x0010A550
	[Networked]
	[NetworkedWeaved(0, 1)]
	public unsafe NetworkBool Data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing TransformViewTeleportSerializer.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(NetworkBool*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing TransformViewTeleportSerializer.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(NetworkBool*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x0600379D RID: 14237 RVA: 0x0010C37B File Offset: 0x0010A57B
	public override void WriteDataFusion()
	{
		this.Data = this.willTeleport;
		this.willTeleport = false;
	}

	// Token: 0x0600379E RID: 14238 RVA: 0x0010C395 File Offset: 0x0010A595
	public override void ReadDataFusion()
	{
		if (this.Data)
		{
			this.transformView.GTAddition_DoTeleport();
		}
	}

	// Token: 0x0600379F RID: 14239 RVA: 0x0010C3AF File Offset: 0x0010A5AF
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (this.transformView.RespectOwnership && info.Sender != info.photonView.Owner)
		{
			return;
		}
		stream.SendNext(this.willTeleport);
		this.willTeleport = false;
	}

	// Token: 0x060037A0 RID: 14240 RVA: 0x0010C3EA File Offset: 0x0010A5EA
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (this.transformView.RespectOwnership && info.Sender != info.photonView.Owner)
		{
			return;
		}
		if ((bool)stream.ReceiveNext())
		{
			this.transformView.GTAddition_DoTeleport();
		}
	}

	// Token: 0x060037A2 RID: 14242 RVA: 0x0010C425 File Offset: 0x0010A625
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Data = this._Data;
	}

	// Token: 0x060037A3 RID: 14243 RVA: 0x0010C43D File Offset: 0x0010A63D
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Data = this.Data;
	}

	// Token: 0x04003D24 RID: 15652
	private bool willTeleport;

	// Token: 0x04003D25 RID: 15653
	private GorillaNetworkTransform transformView;

	// Token: 0x04003D26 RID: 15654
	[WeaverGenerated]
	[SerializeField]
	[DefaultForProperty("Data", 0, 1)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private NetworkBool _Data;
}
