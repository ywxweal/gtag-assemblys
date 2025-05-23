using System;
using Fusion;
using Photon.Pun;

// Token: 0x020005E1 RID: 1505
internal interface IWrappedSerializable : INetworkStruct
{
	// Token: 0x060024D1 RID: 9425
	void OnSerializeRead(object newData);

	// Token: 0x060024D2 RID: 9426
	void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info);

	// Token: 0x060024D3 RID: 9427
	object OnSerializeWrite();

	// Token: 0x060024D4 RID: 9428
	void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info);
}
