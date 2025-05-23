using System;

namespace UniLabs.Time
{
	// Token: 0x02000AAA RID: 2730
	public static class TimeUnitExtensions
	{
		// Token: 0x060041B5 RID: 16821 RVA: 0x00130390 File Offset: 0x0012E590
		public static string ToShortString(this TimeUnit timeUnit)
		{
			string text;
			switch (timeUnit)
			{
			case TimeUnit.None:
				text = "";
				break;
			case TimeUnit.Milliseconds:
				text = "ms";
				break;
			case TimeUnit.Seconds:
				text = "s";
				break;
			case TimeUnit.Minutes:
				text = "m";
				break;
			case TimeUnit.Hours:
				text = "h";
				break;
			case TimeUnit.Days:
				text = "D";
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return text;
		}

		// Token: 0x060041B6 RID: 16822 RVA: 0x00130400 File Offset: 0x0012E600
		public static string ToSeparatorString(this TimeUnit timeUnit)
		{
			string text;
			switch (timeUnit)
			{
			case TimeUnit.None:
				text = "";
				break;
			case TimeUnit.Milliseconds:
				text = "";
				break;
			case TimeUnit.Seconds:
				text = ".";
				break;
			case TimeUnit.Minutes:
				text = ":";
				break;
			case TimeUnit.Hours:
				text = ":";
				break;
			case TimeUnit.Days:
				text = ".";
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return text;
		}

		// Token: 0x060041B7 RID: 16823 RVA: 0x00130470 File Offset: 0x0012E670
		public static double GetUnitValue(this TimeSpan timeSpan, TimeUnit timeUnit)
		{
			int num;
			switch (timeUnit)
			{
			case TimeUnit.None:
				num = 0;
				break;
			case TimeUnit.Milliseconds:
				num = timeSpan.Milliseconds;
				break;
			case TimeUnit.Seconds:
				num = timeSpan.Seconds;
				break;
			case TimeUnit.Minutes:
				num = timeSpan.Minutes;
				break;
			case TimeUnit.Hours:
				num = timeSpan.Hours;
				break;
			case TimeUnit.Days:
				num = timeSpan.Days;
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return (double)num;
		}

		// Token: 0x060041B8 RID: 16824 RVA: 0x001304E8 File Offset: 0x0012E6E8
		public static TimeSpan WithUnitValue(this TimeSpan timeSpan, TimeUnit timeUnit, double value)
		{
			TimeSpan timeSpan2;
			switch (timeUnit)
			{
			case TimeUnit.None:
				timeSpan2 = timeSpan;
				break;
			case TimeUnit.Milliseconds:
				timeSpan2 = timeSpan.Add(TimeSpan.FromMilliseconds(value - (double)timeSpan.Milliseconds));
				break;
			case TimeUnit.Seconds:
				timeSpan2 = timeSpan.Add(TimeSpan.FromSeconds(value - (double)timeSpan.Seconds));
				break;
			case TimeUnit.Minutes:
				timeSpan2 = timeSpan.Add(TimeSpan.FromMinutes(value - (double)timeSpan.Minutes));
				break;
			case TimeUnit.Hours:
				timeSpan2 = timeSpan.Add(TimeSpan.FromHours(value - (double)timeSpan.Hours));
				break;
			case TimeUnit.Days:
				timeSpan2 = timeSpan.Add(TimeSpan.FromDays(value - (double)timeSpan.Days));
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return timeSpan2;
		}

		// Token: 0x060041B9 RID: 16825 RVA: 0x001305AC File Offset: 0x0012E7AC
		public static double GetLowestUnitValue(this TimeSpan timeSpan, TimeUnit timeUnit)
		{
			double num;
			switch (timeUnit)
			{
			case TimeUnit.None:
				num = 0.0;
				break;
			case TimeUnit.Milliseconds:
				num = (double)timeSpan.Milliseconds;
				break;
			case TimeUnit.Seconds:
				num = new TimeSpan(0, 0, 0, timeSpan.Seconds, timeSpan.Milliseconds).TotalSeconds;
				break;
			case TimeUnit.Minutes:
				num = new TimeSpan(0, 0, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds).TotalMinutes;
				break;
			case TimeUnit.Hours:
				num = new TimeSpan(0, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds).TotalHours;
				break;
			case TimeUnit.Days:
				num = timeSpan.TotalDays;
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return num;
		}

		// Token: 0x060041BA RID: 16826 RVA: 0x00130688 File Offset: 0x0012E888
		public static TimeSpan WithLowestUnitValue(this TimeSpan timeSpan, TimeUnit timeUnit, double value)
		{
			TimeSpan timeSpan2;
			switch (timeUnit)
			{
			case TimeUnit.None:
				timeSpan2 = timeSpan;
				break;
			case TimeUnit.Milliseconds:
				timeSpan2 = new TimeSpan(timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, (int)value);
				break;
			case TimeUnit.Seconds:
				timeSpan2 = new TimeSpan(timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, 0).Add(TimeSpan.FromSeconds(value));
				break;
			case TimeUnit.Minutes:
				timeSpan2 = new TimeSpan(timeSpan.Days, timeSpan.Hours, 0, 0).Add(TimeSpan.FromMinutes(value));
				break;
			case TimeUnit.Hours:
				timeSpan2 = new TimeSpan(timeSpan.Days, 0, 0, 0).Add(TimeSpan.FromHours(value));
				break;
			case TimeUnit.Days:
				timeSpan2 = TimeSpan.FromDays(value);
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return timeSpan2;
		}

		// Token: 0x060041BB RID: 16827 RVA: 0x00130774 File Offset: 0x0012E974
		public static double GetHighestUnitValue(this TimeSpan timeSpan, TimeUnit timeUnit)
		{
			double num;
			switch (timeUnit)
			{
			case TimeUnit.None:
				num = 0.0;
				break;
			case TimeUnit.Milliseconds:
				num = timeSpan.TotalMilliseconds;
				break;
			case TimeUnit.Seconds:
				num = new TimeSpan(timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds).TotalSeconds;
				break;
			case TimeUnit.Minutes:
				num = new TimeSpan(timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, 0).TotalMinutes;
				break;
			case TimeUnit.Hours:
				num = new TimeSpan(timeSpan.Days, timeSpan.Hours, 0, 0).TotalHours;
				break;
			case TimeUnit.Days:
				num = (double)timeSpan.Days;
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return num;
		}

		// Token: 0x060041BC RID: 16828 RVA: 0x00130850 File Offset: 0x0012EA50
		public static TimeSpan WithHighestUnitValue(this TimeSpan timeSpan, TimeUnit timeUnit, double value)
		{
			TimeSpan timeSpan2;
			switch (timeUnit)
			{
			case TimeUnit.None:
				timeSpan2 = timeSpan;
				break;
			case TimeUnit.Milliseconds:
				timeSpan2 = TimeSpan.FromMilliseconds(value);
				break;
			case TimeUnit.Seconds:
				timeSpan2 = new TimeSpan(0, 0, 0, 0, timeSpan.Milliseconds).Add(TimeSpan.FromSeconds(value));
				break;
			case TimeUnit.Minutes:
				timeSpan2 = new TimeSpan(0, 0, 0, timeSpan.Seconds, timeSpan.Milliseconds).Add(TimeSpan.FromMinutes(value));
				break;
			case TimeUnit.Hours:
				timeSpan2 = new TimeSpan(0, 0, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds).Add(TimeSpan.FromHours(value));
				break;
			case TimeUnit.Days:
				timeSpan2 = new TimeSpan(0, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds).Add(TimeSpan.FromDays(value));
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return timeSpan2;
		}

		// Token: 0x060041BD RID: 16829 RVA: 0x00130950 File Offset: 0x0012EB50
		public static double GetSingleUnitValue(this TimeSpan timeSpan, TimeUnit timeUnit)
		{
			double num;
			switch (timeUnit)
			{
			case TimeUnit.Milliseconds:
				num = timeSpan.TotalMilliseconds;
				break;
			case TimeUnit.Seconds:
				num = timeSpan.TotalSeconds;
				break;
			case TimeUnit.Minutes:
				num = timeSpan.TotalMinutes;
				break;
			case TimeUnit.Hours:
				num = timeSpan.TotalHours;
				break;
			case TimeUnit.Days:
				num = timeSpan.TotalDays;
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return num;
		}

		// Token: 0x060041BE RID: 16830 RVA: 0x001309C0 File Offset: 0x0012EBC0
		public static TimeSpan FromSingleUnitValue(this TimeSpan timeSpan, TimeUnit timeUnit, double value)
		{
			TimeSpan timeSpan2;
			switch (timeUnit)
			{
			case TimeUnit.None:
				timeSpan2 = TimeSpan.Zero;
				break;
			case TimeUnit.Milliseconds:
				timeSpan2 = TimeSpan.FromMilliseconds(value);
				break;
			case TimeUnit.Seconds:
				timeSpan2 = TimeSpan.FromSeconds(value);
				break;
			case TimeUnit.Minutes:
				timeSpan2 = TimeSpan.FromMinutes(value);
				break;
			case TimeUnit.Hours:
				timeSpan2 = TimeSpan.FromHours(value);
				break;
			case TimeUnit.Days:
				timeSpan2 = TimeSpan.FromDays(value);
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return timeSpan2;
		}

		// Token: 0x060041BF RID: 16831 RVA: 0x00130A38 File Offset: 0x0012EC38
		public static TimeSpan SnapToUnit(this TimeSpan timeSpan, TimeUnit timeUnit)
		{
			TimeSpan timeSpan2;
			switch (timeUnit)
			{
			case TimeUnit.None:
				timeSpan2 = timeSpan;
				break;
			case TimeUnit.Milliseconds:
				timeSpan2 = timeSpan;
				break;
			case TimeUnit.Seconds:
				timeSpan2 = new TimeSpan(timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
				break;
			case TimeUnit.Minutes:
				timeSpan2 = new TimeSpan(timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, 0);
				break;
			case TimeUnit.Hours:
				timeSpan2 = new TimeSpan(timeSpan.Days, timeSpan.Hours, 0, 0);
				break;
			case TimeUnit.Days:
				timeSpan2 = new TimeSpan(timeSpan.Days, 0, 0, 0);
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return timeSpan2;
		}

		// Token: 0x02000AAB RID: 2731
		// (Invoke) Token: 0x060041C1 RID: 16833
		public delegate TimeSpan WithUnitValueDelegate(TimeSpan timeSpan, TimeUnit timeUnit, double value);

		// Token: 0x02000AAC RID: 2732
		// (Invoke) Token: 0x060041C5 RID: 16837
		public delegate double GetUnitValueDelegate(TimeSpan timeSpan, TimeUnit timeUnit);
	}
}
