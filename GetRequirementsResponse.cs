using System;
using System.Collections.Generic;
using Newtonsoft.Json;

// Token: 0x020007A7 RID: 1959
[Serializable]
public class GetRequirementsResponse
{
	// Token: 0x170004E7 RID: 1255
	// (get) Token: 0x0600309E RID: 12446 RVA: 0x000EF581 File Offset: 0x000ED781
	// (set) Token: 0x0600309F RID: 12447 RVA: 0x000EF589 File Offset: 0x000ED789
	[JsonProperty("age")]
	public int? Age { get; set; }

	// Token: 0x170004E8 RID: 1256
	// (get) Token: 0x060030A0 RID: 12448 RVA: 0x000EF592 File Offset: 0x000ED792
	// (set) Token: 0x060030A1 RID: 12449 RVA: 0x000EF59A File Offset: 0x000ED79A
	public int? PlatformMinimumAge { get; set; }

	// Token: 0x170004E9 RID: 1257
	// (get) Token: 0x060030A2 RID: 12450 RVA: 0x000EF5A3 File Offset: 0x000ED7A3
	// (set) Token: 0x060030A3 RID: 12451 RVA: 0x000EF5AB File Offset: 0x000ED7AB
	[JsonProperty("ageStatus")]
	public SessionStatus AgeStatus { get; set; }

	// Token: 0x170004EA RID: 1258
	// (get) Token: 0x060030A4 RID: 12452 RVA: 0x000EF5B4 File Offset: 0x000ED7B4
	// (set) Token: 0x060030A5 RID: 12453 RVA: 0x000EF5BC File Offset: 0x000ED7BC
	[JsonProperty("digitalContentAge")]
	public int DigitalConsentAge { get; set; }

	// Token: 0x170004EB RID: 1259
	// (get) Token: 0x060030A6 RID: 12454 RVA: 0x000EF5C5 File Offset: 0x000ED7C5
	// (set) Token: 0x060030A7 RID: 12455 RVA: 0x000EF5CD File Offset: 0x000ED7CD
	[JsonProperty("minimumAge")]
	public int MinimumAge { get; set; }

	// Token: 0x170004EC RID: 1260
	// (get) Token: 0x060030A8 RID: 12456 RVA: 0x000EF5D6 File Offset: 0x000ED7D6
	// (set) Token: 0x060030A9 RID: 12457 RVA: 0x000EF5DE File Offset: 0x000ED7DE
	[JsonProperty("civilAge")]
	public int CivilAge { get; set; }

	// Token: 0x170004ED RID: 1261
	// (get) Token: 0x060030AA RID: 12458 RVA: 0x000EF5E7 File Offset: 0x000ED7E7
	// (set) Token: 0x060030AB RID: 12459 RVA: 0x000EF5EF File Offset: 0x000ED7EF
	[JsonProperty("approvedAgeCollectionMethods")]
	public List<ApprovedAgeCollectionMethods> ApprovedAgeCollectionMethods { get; set; }
}
