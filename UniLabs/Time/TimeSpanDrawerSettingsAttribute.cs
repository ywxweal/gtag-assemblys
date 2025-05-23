using System;
using System.Diagnostics;

namespace UniLabs.Time
{
	// Token: 0x02000AA7 RID: 2727
	[Conditional("UNITY_EDITOR")]
	public class TimeSpanDrawerSettingsAttribute : Attribute
	{
		// Token: 0x060041AE RID: 16814 RVA: 0x001301E7 File Offset: 0x0012E3E7
		public TimeSpanDrawerSettingsAttribute()
		{
		}

		// Token: 0x060041AF RID: 16815 RVA: 0x001301FD File Offset: 0x0012E3FD
		public TimeSpanDrawerSettingsAttribute(TimeUnit highestUnit, TimeUnit lowestUnit)
		{
			this.HighestUnit = highestUnit;
			this.LowestUnit = lowestUnit;
		}

		// Token: 0x060041B0 RID: 16816 RVA: 0x00130221 File Offset: 0x0012E421
		public TimeSpanDrawerSettingsAttribute(TimeUnit highestUnit, bool drawMilliseconds = false)
		{
			this.HighestUnit = highestUnit;
			this.LowestUnit = (drawMilliseconds ? TimeUnit.Milliseconds : TimeUnit.Seconds);
		}

		// Token: 0x060041B1 RID: 16817 RVA: 0x0013024B File Offset: 0x0012E44B
		public TimeSpanDrawerSettingsAttribute(bool drawMilliseconds)
		{
			this.HighestUnit = TimeUnit.Days;
			this.LowestUnit = (drawMilliseconds ? TimeUnit.Milliseconds : TimeUnit.Seconds);
		}

		// Token: 0x04004476 RID: 17526
		public TimeUnit HighestUnit = TimeUnit.Days;

		// Token: 0x04004477 RID: 17527
		public TimeUnit LowestUnit = TimeUnit.Seconds;
	}
}
