using System;
using System.Diagnostics;

namespace UniLabs.Time
{
	// Token: 0x02000AA7 RID: 2727
	[Conditional("UNITY_EDITOR")]
	public class TimeSpanDrawerSettingsAttribute : Attribute
	{
		// Token: 0x060041AF RID: 16815 RVA: 0x001302BF File Offset: 0x0012E4BF
		public TimeSpanDrawerSettingsAttribute()
		{
		}

		// Token: 0x060041B0 RID: 16816 RVA: 0x001302D5 File Offset: 0x0012E4D5
		public TimeSpanDrawerSettingsAttribute(TimeUnit highestUnit, TimeUnit lowestUnit)
		{
			this.HighestUnit = highestUnit;
			this.LowestUnit = lowestUnit;
		}

		// Token: 0x060041B1 RID: 16817 RVA: 0x001302F9 File Offset: 0x0012E4F9
		public TimeSpanDrawerSettingsAttribute(TimeUnit highestUnit, bool drawMilliseconds = false)
		{
			this.HighestUnit = highestUnit;
			this.LowestUnit = (drawMilliseconds ? TimeUnit.Milliseconds : TimeUnit.Seconds);
		}

		// Token: 0x060041B2 RID: 16818 RVA: 0x00130323 File Offset: 0x0012E523
		public TimeSpanDrawerSettingsAttribute(bool drawMilliseconds)
		{
			this.HighestUnit = TimeUnit.Days;
			this.LowestUnit = (drawMilliseconds ? TimeUnit.Milliseconds : TimeUnit.Seconds);
		}

		// Token: 0x04004477 RID: 17527
		public TimeUnit HighestUnit = TimeUnit.Days;

		// Token: 0x04004478 RID: 17528
		public TimeUnit LowestUnit = TimeUnit.Seconds;
	}
}
