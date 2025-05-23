using System;
using Fusion;
using Photon.Pun;

// Token: 0x020005DC RID: 1500
[NetworkBehaviourWeaved(0)]
internal abstract class GorillaSerializerMasterOnly : GorillaWrappedSerializer
{
	// Token: 0x060024A2 RID: 9378 RVA: 0x000B8393 File Offset: 0x000B6593
	protected override bool ValidOnSerialize(PhotonStream stream, in PhotonMessageInfo info)
	{
		return info.Sender == PhotonNetwork.MasterClient;
	}

	// Token: 0x060024A4 RID: 9380 RVA: 0x000B83AD File Offset: 0x000B65AD
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x060024A5 RID: 9381 RVA: 0x000B83B9 File Offset: 0x000B65B9
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}
}
