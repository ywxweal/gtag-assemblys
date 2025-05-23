using System;
using System.Collections.Generic;
using Newtonsoft.Json;

// Token: 0x020007A7 RID: 1959
[Serializable]
public class GetRequirementsResponse
{
	// Token: 0x170004E7 RID: 1255
	// (get) Token: 0x0600309F RID: 12447 RVA: 0x000EF625 File Offset: 0x000ED825
	// (set) Token: 0x060030A0 RID: 12448 RVA: 0x000EF62D File Offset: 0x000ED82D
	[JsonProperty("age")]
	public int? Age { get; set; }

	// Token: 0x170004E8 RID: 1256
	// (get) Token: 0x060030A1 RID: 12449 RVA: 0x000EF636 File Offset: 0x000ED836
	// (set) Token: 0x060030A2 RID: 12450 RVA: 0x000EF63E File Offset: 0x000ED83E
	public int? PlatformMinimumAge { get; set; }

	// Token: 0x170004E9 RID: 1257
	// (get) Token: 0x060030A3 RID: 12451 RVA: 0x000EF647 File Offset: 0x000ED847
	// (set) Token: 0x060030A4 RID: 12452 RVA: 0x000EF64F File Offset: 0x000ED84F
	[JsonProperty("ageStatus")]
	public SessionStatus AgeStatus { get; set; }

	// Token: 0x170004EA RID: 1258
	// (get) Token: 0x060030A5 RID: 12453 RVA: 0x000EF658 File Offset: 0x000ED858
	// (set) Token: 0x060030A6 RID: 12454 RVA: 0x000EF660 File Offset: 0x000ED860
	[JsonProperty("digitalContentAge")]
	public int DigitalConsentAge { get; set; }

	// Token: 0x170004EB RID: 1259
	// (get) Token: 0x060030A7 RID: 12455 RVA: 0x000EF669 File Offset: 0x000ED869
	// (set) Token: 0x060030A8 RID: 12456 RVA: 0x000EF671 File Offset: 0x000ED871
	[JsonProperty("minimumAge")]
	public int MinimumAge { get; set; }

	// Token: 0x170004EC RID: 1260
	// (get) Token: 0x060030A9 RID: 12457 RVA: 0x000EF67A File Offset: 0x000ED87A
	// (set) Token: 0x060030AA RID: 12458 RVA: 0x000EF682 File Offset: 0x000ED882
	[JsonProperty("civilAge")]
	public int CivilAge { get; set; }

	// Token: 0x170004ED RID: 1261
	// (get) Token: 0x060030AB RID: 12459 RVA: 0x000EF68B File Offset: 0x000ED88B
	// (set) Token: 0x060030AC RID: 12460 RVA: 0x000EF693 File Offset: 0x000ED893
	[JsonProperty("approvedAgeCollectionMethods")]
	public List<ApprovedAgeCollectionMethods> ApprovedAgeCollectionMethods { get; set; }
}
