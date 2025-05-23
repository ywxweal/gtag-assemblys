using System;

// Token: 0x020006D0 RID: 1744
public interface ITimeOfDaySystem
{
	// Token: 0x17000448 RID: 1096
	// (get) Token: 0x06002B83 RID: 11139
	double currentTimeInSeconds { get; }

	// Token: 0x17000449 RID: 1097
	// (get) Token: 0x06002B84 RID: 11140
	double totalTimeInSeconds { get; }
}
