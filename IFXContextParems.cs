﻿using System;

// Token: 0x02000929 RID: 2345
public interface IFXContextParems<T> where T : FXSArgs
{
	// Token: 0x17000599 RID: 1433
	// (get) Token: 0x06003914 RID: 14612
	FXSystemSettings settings { get; }

	// Token: 0x06003915 RID: 14613
	void OnPlayFX(T parems);
}
