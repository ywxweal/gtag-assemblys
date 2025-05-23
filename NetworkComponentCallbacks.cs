using System;
using Fusion;
using Photon.Pun;

// Token: 0x020002A4 RID: 676
[NetworkBehaviourWeaved(0)]
public class NetworkComponentCallbacks : NetworkComponent
{
	// Token: 0x06000FDE RID: 4062 RVA: 0x0004F0FC File Offset: 0x0004D2FC
	public override void ReadDataFusion()
	{
		this.ReadData();
	}

	// Token: 0x06000FDF RID: 4063 RVA: 0x0004F109 File Offset: 0x0004D309
	public override void WriteDataFusion()
	{
		this.WriteData();
	}

	// Token: 0x06000FE0 RID: 4064 RVA: 0x0004F116 File Offset: 0x0004D316
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		this.ReadPunData(stream, info);
	}

	// Token: 0x06000FE1 RID: 4065 RVA: 0x0004F125 File Offset: 0x0004D325
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		this.WritePunData(stream, info);
	}

	// Token: 0x06000FE3 RID: 4067 RVA: 0x00002637 File Offset: 0x00000837
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x06000FE4 RID: 4068 RVA: 0x00002643 File Offset: 0x00000843
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}

	// Token: 0x040012A4 RID: 4772
	public Action ReadData;

	// Token: 0x040012A5 RID: 4773
	public Action WriteData;

	// Token: 0x040012A6 RID: 4774
	public Action<PhotonStream, PhotonMessageInfo> ReadPunData;

	// Token: 0x040012A7 RID: 4775
	public Action<PhotonStream, PhotonMessageInfo> WritePunData;
}
