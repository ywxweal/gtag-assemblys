using System;
using System.Collections.Generic;

// Token: 0x02000243 RID: 579
[Serializable]
public class SerializablePerformanceReport<T>
{
	// Token: 0x040010D9 RID: 4313
	public string reportDate;

	// Token: 0x040010DA RID: 4314
	public string version;

	// Token: 0x040010DB RID: 4315
	public List<T> results;
}
