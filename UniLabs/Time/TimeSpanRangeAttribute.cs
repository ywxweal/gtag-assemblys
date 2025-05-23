using System;
using System.Diagnostics;

namespace UniLabs.Time
{
	// Token: 0x02000AA8 RID: 2728
	[AttributeUsage(AttributeTargets.All)]
	[Conditional("UNITY_EDITOR")]
	public class TimeSpanRangeAttribute : Attribute
	{
		// Token: 0x060041B3 RID: 16819 RVA: 0x0013034D File Offset: 0x0012E54D
		public TimeSpanRangeAttribute(string maxGetter, bool inline = false, TimeUnit snappingUnit = TimeUnit.Seconds)
		{
			this.MaxGetter = maxGetter;
			this.SnappingUnit = snappingUnit;
			this.Inline = inline;
		}

		// Token: 0x060041B4 RID: 16820 RVA: 0x0013036A File Offset: 0x0012E56A
		public TimeSpanRangeAttribute(string minGetter, string maxGetter, bool inline = false, TimeUnit snappingUnit = TimeUnit.Seconds)
		{
			this.MinGetter = minGetter;
			this.MaxGetter = maxGetter;
			this.SnappingUnit = snappingUnit;
			this.Inline = inline;
		}

		// Token: 0x04004479 RID: 17529
		public string MinGetter;

		// Token: 0x0400447A RID: 17530
		public string MaxGetter;

		// Token: 0x0400447B RID: 17531
		public TimeUnit SnappingUnit;

		// Token: 0x0400447C RID: 17532
		public bool Inline;

		// Token: 0x0400447D RID: 17533
		public string DisableMinMaxIf;
	}
}
