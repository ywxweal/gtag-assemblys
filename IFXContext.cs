using System;

// Token: 0x02000927 RID: 2343
public interface IFXContext
{
	// Token: 0x17000598 RID: 1432
	// (get) Token: 0x06003910 RID: 14608
	FXSystemSettings settings { get; }

	// Token: 0x06003911 RID: 14609
	void OnPlayFX();
}
