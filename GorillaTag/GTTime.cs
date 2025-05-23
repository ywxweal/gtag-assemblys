using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using GorillaNetworking;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000D0A RID: 3338
	public class GTTime
	{
		// Token: 0x1700085C RID: 2140
		// (get) Token: 0x060053C0 RID: 21440 RVA: 0x001965E4 File Offset: 0x001947E4
		// (set) Token: 0x060053C1 RID: 21441 RVA: 0x001965EB File Offset: 0x001947EB
		public static bool usingServerTime { get; private set; }

		// Token: 0x060053C2 RID: 21442 RVA: 0x001965F3 File Offset: 0x001947F3
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static long GetServerStartupTimeAsMilliseconds()
		{
			return GorillaComputer.instance.startupMillis;
		}

		// Token: 0x060053C3 RID: 21443 RVA: 0x00196604 File Offset: 0x00194804
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static long GetDeviceStartupTimeAsMilliseconds()
		{
			return (long)(TimeSpan.FromTicks(DateTime.UtcNow.Ticks).TotalMilliseconds - Time.realtimeSinceStartupAsDouble * 1000.0);
		}

		// Token: 0x060053C4 RID: 21444 RVA: 0x0019663C File Offset: 0x0019483C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long GetStartupTimeAsMilliseconds()
		{
			GTTime.usingServerTime = true;
			long num = 0L;
			if (GorillaComputer.instance != null)
			{
				num = GTTime.GetServerStartupTimeAsMilliseconds();
			}
			if (num == 0L)
			{
				GTTime.usingServerTime = false;
				num = GTTime.GetDeviceStartupTimeAsMilliseconds();
			}
			return num;
		}

		// Token: 0x060053C5 RID: 21445 RVA: 0x00196677 File Offset: 0x00194877
		public static long TimeAsMilliseconds()
		{
			return GTTime.GetStartupTimeAsMilliseconds() + (long)(Time.realtimeSinceStartupAsDouble * 1000.0);
		}

		// Token: 0x060053C6 RID: 21446 RVA: 0x0019668F File Offset: 0x0019488F
		public static double TimeAsDouble()
		{
			return (double)GTTime.GetStartupTimeAsMilliseconds() / 1000.0 + Time.realtimeSinceStartupAsDouble;
		}

		// Token: 0x060053C7 RID: 21447 RVA: 0x001966A8 File Offset: 0x001948A8
		public static DateTime GetAAxiomDateTime()
		{
			DateTime dateTime;
			try
			{
				TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
				dateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo);
			}
			catch
			{
				try
				{
					TimeZoneInfo timeZoneInfo2 = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
					dateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo2);
				}
				catch
				{
					dateTime = DateTime.UtcNow;
				}
			}
			return dateTime;
		}

		// Token: 0x060053C8 RID: 21448 RVA: 0x00196710 File Offset: 0x00194910
		public static string GetAAxiomDateTimeAsStringForDisplay()
		{
			return GTTime.GetAAxiomDateTime().ToString("yyyy-MM-dd HH:mm:ss.fff");
		}

		// Token: 0x060053C9 RID: 21449 RVA: 0x00196730 File Offset: 0x00194930
		public static string GetAAxiomDateTimeAsStringForFilename()
		{
			return GTTime.GetAAxiomDateTime().ToString("yyyy-MM-dd_HH-mm-ss-fff");
		}

		// Token: 0x060053CA RID: 21450 RVA: 0x00196750 File Offset: 0x00194950
		public static long GetAAxiomDateTimeAsHumanReadableLong()
		{
			return long.Parse(GTTime.GetAAxiomDateTime().ToString("yyyyMMddHHmmssfff00"));
		}

		// Token: 0x060053CB RID: 21451 RVA: 0x00196774 File Offset: 0x00194974
		public static DateTime ConvertDateTimeHumanReadableLongToDateTime(long humanReadableLong)
		{
			return DateTime.ParseExact(humanReadableLong.ToString(), "yyyyMMddHHmmssfff'00'", CultureInfo.InvariantCulture);
		}
	}
}
