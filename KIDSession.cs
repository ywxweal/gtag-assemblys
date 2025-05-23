using System;
using System.Collections.Generic;
using KID.Model;

// Token: 0x0200079E RID: 1950
[Serializable]
public class KIDSession
{
	// Token: 0x170004DE RID: 1246
	// (get) Token: 0x06003084 RID: 12420 RVA: 0x000EF4E0 File Offset: 0x000ED6E0
	// (set) Token: 0x06003085 RID: 12421 RVA: 0x000EF4E8 File Offset: 0x000ED6E8
	public SessionStatus SessionStatus { get; set; }

	// Token: 0x170004DF RID: 1247
	// (get) Token: 0x06003086 RID: 12422 RVA: 0x000EF4F1 File Offset: 0x000ED6F1
	// (set) Token: 0x06003087 RID: 12423 RVA: 0x000EF4F9 File Offset: 0x000ED6F9
	public GTAgeStatusType AgeStatus { get; set; }

	// Token: 0x170004E0 RID: 1248
	// (get) Token: 0x06003088 RID: 12424 RVA: 0x000EF502 File Offset: 0x000ED702
	// (set) Token: 0x06003089 RID: 12425 RVA: 0x000EF50A File Offset: 0x000ED70A
	public Guid SessionId { get; set; }

	// Token: 0x170004E1 RID: 1249
	// (get) Token: 0x0600308A RID: 12426 RVA: 0x000EF513 File Offset: 0x000ED713
	// (set) Token: 0x0600308B RID: 12427 RVA: 0x000EF51B File Offset: 0x000ED71B
	public string KUID { get; set; }

	// Token: 0x170004E2 RID: 1250
	// (get) Token: 0x0600308C RID: 12428 RVA: 0x000EF524 File Offset: 0x000ED724
	// (set) Token: 0x0600308D RID: 12429 RVA: 0x000EF52C File Offset: 0x000ED72C
	public string etag { get; set; }

	// Token: 0x170004E3 RID: 1251
	// (get) Token: 0x0600308E RID: 12430 RVA: 0x000EF535 File Offset: 0x000ED735
	// (set) Token: 0x0600308F RID: 12431 RVA: 0x000EF53D File Offset: 0x000ED73D
	public List<Permission> Permissions { get; set; }

	// Token: 0x170004E4 RID: 1252
	// (get) Token: 0x06003090 RID: 12432 RVA: 0x000EF546 File Offset: 0x000ED746
	// (set) Token: 0x06003091 RID: 12433 RVA: 0x000EF54E File Offset: 0x000ED74E
	public DateTime DateOfBirth { get; set; }

	// Token: 0x170004E5 RID: 1253
	// (get) Token: 0x06003092 RID: 12434 RVA: 0x000EF557 File Offset: 0x000ED757
	// (set) Token: 0x06003093 RID: 12435 RVA: 0x000EF55F File Offset: 0x000ED75F
	public string Jurisdiction { get; set; }
}
