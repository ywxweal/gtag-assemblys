using System;
using Photon.Pun;

// Token: 0x020005DF RID: 1503
public interface IGorillaSerializeable
{
	// Token: 0x060024CC RID: 9420
	void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info);

	// Token: 0x060024CD RID: 9421
	void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info);
}
