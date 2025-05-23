using System;
using Fusion;
using Photon.Pun;

// Token: 0x020005DC RID: 1500
[NetworkBehaviourWeaved(0)]
internal abstract class GorillaSerializerMasterOnly : GorillaWrappedSerializer
{
	// Token: 0x060024A2 RID: 9378 RVA: 0x000B8373 File Offset: 0x000B6573
	protected override bool ValidOnSerialize(PhotonStream stream, in PhotonMessageInfo info)
	{
		return info.Sender == PhotonNetwork.MasterClient;
	}

	// Token: 0x060024A4 RID: 9380 RVA: 0x000B838D File Offset: 0x000B658D
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x060024A5 RID: 9381 RVA: 0x000B8399 File Offset: 0x000B6599
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}
}
