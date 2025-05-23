using System;
using System.Collections.Generic;

// Token: 0x020002D2 RID: 722
public class StaticRPCLookup
{
	// Token: 0x0600116C RID: 4460 RVA: 0x00053EBC File Offset: 0x000520BC
	public void Add(NetworkSystem.StaticRPCPlaceholder placeholder, byte code, NetworkSystem.StaticRPC lookupMethod)
	{
		int count = this.entries.Count;
		this.entries.Add(new StaticRPCEntry(placeholder, code, lookupMethod));
		this.eventCodeEntryLookup.Add(code, count);
		this.placeholderEntryLookup.Add(placeholder, count);
	}

	// Token: 0x0600116D RID: 4461 RVA: 0x00053F02 File Offset: 0x00052102
	public NetworkSystem.StaticRPC CodeToMethod(byte code)
	{
		return this.entries[this.eventCodeEntryLookup[code]].lookupMethod;
	}

	// Token: 0x0600116E RID: 4462 RVA: 0x00053F20 File Offset: 0x00052120
	public byte PlaceholderToCode(NetworkSystem.StaticRPCPlaceholder placeholder)
	{
		return this.entries[this.placeholderEntryLookup[placeholder]].code;
	}

	// Token: 0x040013A1 RID: 5025
	public List<StaticRPCEntry> entries = new List<StaticRPCEntry>();

	// Token: 0x040013A2 RID: 5026
	private Dictionary<byte, int> eventCodeEntryLookup = new Dictionary<byte, int>();

	// Token: 0x040013A3 RID: 5027
	private Dictionary<NetworkSystem.StaticRPCPlaceholder, int> placeholderEntryLookup = new Dictionary<NetworkSystem.StaticRPCPlaceholder, int>();
}
