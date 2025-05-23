using System;
using System.Runtime.InteropServices;
using Fusion;

// Token: 0x020000FF RID: 255
[NetworkStructWeaved(3)]
[StructLayout(LayoutKind.Explicit, Size = 12)]
public struct BeeSwarmData : INetworkStruct
{
	// Token: 0x1700007C RID: 124
	// (get) Token: 0x0600066D RID: 1645 RVA: 0x00025018 File Offset: 0x00023218
	// (set) Token: 0x0600066E RID: 1646 RVA: 0x00025020 File Offset: 0x00023220
	public int TargetActorNumber { readonly get; set; }

	// Token: 0x1700007D RID: 125
	// (get) Token: 0x0600066F RID: 1647 RVA: 0x00025029 File Offset: 0x00023229
	// (set) Token: 0x06000670 RID: 1648 RVA: 0x00025031 File Offset: 0x00023231
	public int CurrentState { readonly get; set; }

	// Token: 0x1700007E RID: 126
	// (get) Token: 0x06000671 RID: 1649 RVA: 0x0002503A File Offset: 0x0002323A
	// (set) Token: 0x06000672 RID: 1650 RVA: 0x00025042 File Offset: 0x00023242
	public float CurrentSpeed { readonly get; set; }

	// Token: 0x06000673 RID: 1651 RVA: 0x0002504B File Offset: 0x0002324B
	public BeeSwarmData(int actorNr, int state, float speed)
	{
		this.TargetActorNumber = actorNr;
		this.CurrentState = state;
		this.CurrentSpeed = speed;
	}
}
