using System;
using System.Collections.Generic;
using KID.Model;

// Token: 0x0200079E RID: 1950
[Serializable]
public class KIDSession
{
	// Token: 0x170004DE RID: 1246
	// (get) Token: 0x06003085 RID: 12421 RVA: 0x000EF584 File Offset: 0x000ED784
	// (set) Token: 0x06003086 RID: 12422 RVA: 0x000EF58C File Offset: 0x000ED78C
	public SessionStatus SessionStatus { get; set; }

	// Token: 0x170004DF RID: 1247
	// (get) Token: 0x06003087 RID: 12423 RVA: 0x000EF595 File Offset: 0x000ED795
	// (set) Token: 0x06003088 RID: 12424 RVA: 0x000EF59D File Offset: 0x000ED79D
	public GTAgeStatusType AgeStatus { get; set; }

	// Token: 0x170004E0 RID: 1248
	// (get) Token: 0x06003089 RID: 12425 RVA: 0x000EF5A6 File Offset: 0x000ED7A6
	// (set) Token: 0x0600308A RID: 12426 RVA: 0x000EF5AE File Offset: 0x000ED7AE
	public Guid SessionId { get; set; }

	// Token: 0x170004E1 RID: 1249
	// (get) Token: 0x0600308B RID: 12427 RVA: 0x000EF5B7 File Offset: 0x000ED7B7
	// (set) Token: 0x0600308C RID: 12428 RVA: 0x000EF5BF File Offset: 0x000ED7BF
	public string KUID { get; set; }

	// Token: 0x170004E2 RID: 1250
	// (get) Token: 0x0600308D RID: 12429 RVA: 0x000EF5C8 File Offset: 0x000ED7C8
	// (set) Token: 0x0600308E RID: 12430 RVA: 0x000EF5D0 File Offset: 0x000ED7D0
	public string etag { get; set; }

	// Token: 0x170004E3 RID: 1251
	// (get) Token: 0x0600308F RID: 12431 RVA: 0x000EF5D9 File Offset: 0x000ED7D9
	// (set) Token: 0x06003090 RID: 12432 RVA: 0x000EF5E1 File Offset: 0x000ED7E1
	public List<Permission> Permissions { get; set; }

	// Token: 0x170004E4 RID: 1252
	// (get) Token: 0x06003091 RID: 12433 RVA: 0x000EF5EA File Offset: 0x000ED7EA
	// (set) Token: 0x06003092 RID: 12434 RVA: 0x000EF5F2 File Offset: 0x000ED7F2
	public DateTime DateOfBirth { get; set; }

	// Token: 0x170004E5 RID: 1253
	// (get) Token: 0x06003093 RID: 12435 RVA: 0x000EF5FB File Offset: 0x000ED7FB
	// (set) Token: 0x06003094 RID: 12436 RVA: 0x000EF603 File Offset: 0x000ED803
	public string Jurisdiction { get; set; }
}
