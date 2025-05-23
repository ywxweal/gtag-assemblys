using System;
using Photon.Pun;

// Token: 0x020006F3 RID: 1779
public class GorillaThrowingRock : GorillaThrowable, IPunInstantiateMagicCallback
{
	// Token: 0x06002C55 RID: 11349 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnPhotonInstantiate(PhotonMessageInfo info)
	{
	}

	// Token: 0x0400329F RID: 12959
	public float bonkSpeedMin = 1f;

	// Token: 0x040032A0 RID: 12960
	public float bonkSpeedMax = 5f;

	// Token: 0x040032A1 RID: 12961
	public VRRig hitRig;
}
