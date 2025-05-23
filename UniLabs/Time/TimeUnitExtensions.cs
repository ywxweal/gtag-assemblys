using System;

namespace UniLabs.Time
{
	// Token: 0x02000AAA RID: 2730
	public static class TimeUnitExtensions
	{
		// Token: 0x060041B4 RID: 16820 RVA: 0x001302B8 File Offset: 0x0012E4B8
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

		// Token: 0x060041B5 RID: 16821 RVA: 0x00130328 File Offset: 0x0012E528
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

		// Token: 0x060041B6 RID: 16822 RVA: 0x00130398 File Offset: 0x0012E598
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

		// Token: 0x060041B7 RID: 16823 RVA: 0x00130410 File Offset: 0x0012E610
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

		// Token: 0x060041B8 RID: 16824 RVA: 0x001304D4 File Offset: 0x0012E6D4
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

		// Token: 0x060041B9 RID: 16825 RVA: 0x001305B0 File Offset: 0x0012E7B0
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

		// Token: 0x060041BA RID: 16826 RVA: 0x0013069C File Offset: 0x0012E89C
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

		// Token: 0x060041BB RID: 16827 RVA: 0x00130778 File Offset: 0x0012E978
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

		// Token: 0x060041BC RID: 16828 RVA: 0x00130878 File Offset: 0x0012EA78
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

		// Token: 0x060041BD RID: 16829 RVA: 0x001308E8 File Offset: 0x0012EAE8
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

		// Token: 0x060041BE RID: 16830 RVA: 0x00130960 File Offset: 0x0012EB60
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
		// (Invoke) Token: 0x060041C0 RID: 16832
		public delegate TimeSpan WithUnitValueDelegate(TimeSpan timeSpan, TimeUnit timeUnit, double value);

		// Token: 0x02000AAC RID: 2732
		// (Invoke) Token: 0x060041C4 RID: 16836
		public delegate double GetUnitValueDelegate(TimeSpan timeSpan, TimeUnit timeUnit);
	}
}
