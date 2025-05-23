using System;

// Token: 0x020002D1 RID: 721
public class StaticRPCEntry
{
	// Token: 0x0600116B RID: 4459 RVA: 0x00053E9F File Offset: 0x0005209F
	public StaticRPCEntry(NetworkSystem.StaticRPCPlaceholder placeholder, byte code, NetworkSystem.StaticRPC lookupMethod)
	{
		this.placeholder = placeholder;
		this.code = code;
		this.lookupMethod = lookupMethod;
	}

	// Token: 0x0400139E RID: 5022
	public NetworkSystem.StaticRPCPlaceholder placeholder;

	// Token: 0x0400139F RID: 5023
	public byte code;

	// Token: 0x040013A0 RID: 5024
	public NetworkSystem.StaticRPC lookupMethod;
}
