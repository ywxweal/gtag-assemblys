using System;
using System.Runtime.CompilerServices;
using KID.Model;

// Token: 0x020007AC RID: 1964
public class VerifyAgeResponse
{
	// Token: 0x170004EE RID: 1262
	// (get) Token: 0x060030B2 RID: 12466 RVA: 0x000EF69C File Offset: 0x000ED89C
	// (set) Token: 0x060030B3 RID: 12467 RVA: 0x000EF6A4 File Offset: 0x000ED8A4
	public SessionStatus Status { get; set; }

	// Token: 0x170004EF RID: 1263
	// (get) Token: 0x060030B4 RID: 12468 RVA: 0x000EF6AD File Offset: 0x000ED8AD
	// (set) Token: 0x060030B5 RID: 12469 RVA: 0x000EF6B5 File Offset: 0x000ED8B5
	[Nullable(2)]
	public Session Session
	{
		[NullableContext(2)]
		get;
		[NullableContext(2)]
		set;
	}

	// Token: 0x170004F0 RID: 1264
	// (get) Token: 0x060030B6 RID: 12470 RVA: 0x000EF6BE File Offset: 0x000ED8BE
	// (set) Token: 0x060030B7 RID: 12471 RVA: 0x000EF6C6 File Offset: 0x000ED8C6
	public KIDDefaultSession DefaultSession { get; set; }
}
