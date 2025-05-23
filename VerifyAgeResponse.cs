using System;
using System.Runtime.CompilerServices;
using KID.Model;

// Token: 0x020007AC RID: 1964
public class VerifyAgeResponse
{
	// Token: 0x170004EE RID: 1262
	// (get) Token: 0x060030B1 RID: 12465 RVA: 0x000EF5F8 File Offset: 0x000ED7F8
	// (set) Token: 0x060030B2 RID: 12466 RVA: 0x000EF600 File Offset: 0x000ED800
	public SessionStatus Status { get; set; }

	// Token: 0x170004EF RID: 1263
	// (get) Token: 0x060030B3 RID: 12467 RVA: 0x000EF609 File Offset: 0x000ED809
	// (set) Token: 0x060030B4 RID: 12468 RVA: 0x000EF611 File Offset: 0x000ED811
	[Nullable(2)]
	public Session Session
	{
		[NullableContext(2)]
		get;
		[NullableContext(2)]
		set;
	}

	// Token: 0x170004F0 RID: 1264
	// (get) Token: 0x060030B5 RID: 12469 RVA: 0x000EF61A File Offset: 0x000ED81A
	// (set) Token: 0x060030B6 RID: 12470 RVA: 0x000EF622 File Offset: 0x000ED822
	public KIDDefaultSession DefaultSession { get; set; }
}
