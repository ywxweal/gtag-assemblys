using System;
using Fusion;
using Photon.Pun;
using UnityEngine;

// Token: 0x020008F6 RID: 2294
[NetworkBehaviourWeaved(1)]
public class TransformViewTeleportSerializer : NetworkComponent
{
	// Token: 0x0600379A RID: 14234 RVA: 0x0010C3E1 File Offset: 0x0010A5E1
	protected override void Start()
	{
		base.Start();
		this.transformView = base.GetComponent<GorillaNetworkTransform>();
	}

	// Token: 0x0600379B RID: 14235 RVA: 0x0010C3F5 File Offset: 0x0010A5F5
	public void SetWillTeleport()
	{
		this.willTeleport = true;
	}

	// Token: 0x1700057E RID: 1406
	// (get) Token: 0x0600379C RID: 14236 RVA: 0x0010C3FE File Offset: 0x0010A5FE
	// (set) Token: 0x0600379D RID: 14237 RVA: 0x0010C428 File Offset: 0x0010A628
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

	// Token: 0x0600379E RID: 14238 RVA: 0x0010C453 File Offset: 0x0010A653
	public override void WriteDataFusion()
	{
		this.Data = this.willTeleport;
		this.willTeleport = false;
	}

	// Token: 0x0600379F RID: 14239 RVA: 0x0010C46D File Offset: 0x0010A66D
	public override void ReadDataFusion()
	{
		if (this.Data)
		{
			this.transformView.GTAddition_DoTeleport();
		}
	}

	// Token: 0x060037A0 RID: 14240 RVA: 0x0010C487 File Offset: 0x0010A687
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (this.transformView.RespectOwnership && info.Sender != info.photonView.Owner)
		{
			return;
		}
		stream.SendNext(this.willTeleport);
		this.willTeleport = false;
	}

	// Token: 0x060037A1 RID: 14241 RVA: 0x0010C4C2 File Offset: 0x0010A6C2
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

	// Token: 0x060037A3 RID: 14243 RVA: 0x0010C4FD File Offset: 0x0010A6FD
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Data = this._Data;
	}

	// Token: 0x060037A4 RID: 14244 RVA: 0x0010C515 File Offset: 0x0010A715
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Data = this.Data;
	}

	// Token: 0x04003D25 RID: 15653
	private bool willTeleport;

	// Token: 0x04003D26 RID: 15654
	private GorillaNetworkTransform transformView;

	// Token: 0x04003D27 RID: 15655
	[WeaverGenerated]
	[SerializeField]
	[DefaultForProperty("Data", 0, 1)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private NetworkBool _Data;
}
