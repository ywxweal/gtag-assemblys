using System;
using Newtonsoft.Json;

// Token: 0x0200079C RID: 1948
[Serializable]
public class TMPPermission
{
	// Token: 0x170004DB RID: 1243
	// (get) Token: 0x0600307D RID: 12413 RVA: 0x000EF551 File Offset: 0x000ED751
	// (set) Token: 0x0600307E RID: 12414 RVA: 0x000EF559 File Offset: 0x000ED759
	[JsonProperty("name")]
	public string Name { get; set; }

	// Token: 0x170004DC RID: 1244
	// (get) Token: 0x0600307F RID: 12415 RVA: 0x000EF562 File Offset: 0x000ED762
	// (set) Token: 0x06003080 RID: 12416 RVA: 0x000EF56A File Offset: 0x000ED76A
	[JsonProperty("enabled")]
	public bool Enabled { get; set; }

	// Token: 0x170004DD RID: 1245
	// (get) Token: 0x06003081 RID: 12417 RVA: 0x000EF573 File Offset: 0x000ED773
	// (set) Token: 0x06003082 RID: 12418 RVA: 0x000EF57B File Offset: 0x000ED77B
	[JsonProperty("managedBy")]
	public ManagedBy ManagedBy { get; set; }
}
