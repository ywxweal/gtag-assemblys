using System;
using System.Runtime.InteropServices;
using Fusion;

// Token: 0x0200029F RID: 671
[NetworkStructWeaved(1)]
[Serializable]
[StructLayout(LayoutKind.Explicit, Size = 4)]
public struct HitTargetStruct : INetworkStruct
{
	// Token: 0x06000F9D RID: 3997 RVA: 0x0004ED74 File Offset: 0x0004CF74
	public HitTargetStruct(int v)
	{
		this.Score = v;
	}

	// Token: 0x04001295 RID: 4757
	[FieldOffset(0)]
	public int Score;
}
