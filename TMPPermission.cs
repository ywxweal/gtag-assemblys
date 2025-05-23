using System;
using Newtonsoft.Json;

// Token: 0x0200079C RID: 1948
[Serializable]
public class TMPPermission
{
	// Token: 0x170004DB RID: 1243
	// (get) Token: 0x0600307C RID: 12412 RVA: 0x000EF4AD File Offset: 0x000ED6AD
	// (set) Token: 0x0600307D RID: 12413 RVA: 0x000EF4B5 File Offset: 0x000ED6B5
	[JsonProperty("name")]
	public string Name { get; set; }

	// Token: 0x170004DC RID: 1244
	// (get) Token: 0x0600307E RID: 12414 RVA: 0x000EF4BE File Offset: 0x000ED6BE
	// (set) Token: 0x0600307F RID: 12415 RVA: 0x000EF4C6 File Offset: 0x000ED6C6
	[JsonProperty("enabled")]
	public bool Enabled { get; set; }

	// Token: 0x170004DD RID: 1245
	// (get) Token: 0x06003080 RID: 12416 RVA: 0x000EF4CF File Offset: 0x000ED6CF
	// (set) Token: 0x06003081 RID: 12417 RVA: 0x000EF4D7 File Offset: 0x000ED6D7
	[JsonProperty("managedBy")]
	public ManagedBy ManagedBy { get; set; }
}
