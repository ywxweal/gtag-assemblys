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
		// (get) Token: 0x060053C1 RID: 21441 RVA: 0x001966BC File Offset: 0x001948BC
		// (set) Token: 0x060053C2 RID: 21442 RVA: 0x001966C3 File Offset: 0x001948C3
		public static bool usingServerTime { get; private set; }

		// Token: 0x060053C3 RID: 21443 RVA: 0x001966CB File Offset: 0x001948CB
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static long GetServerStartupTimeAsMilliseconds()
		{
			return GorillaComputer.instance.startupMillis;
		}

		// Token: 0x060053C4 RID: 21444 RVA: 0x001966DC File Offset: 0x001948DC
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static long GetDeviceStartupTimeAsMilliseconds()
		{
			return (long)(TimeSpan.FromTicks(DateTime.UtcNow.Ticks).TotalMilliseconds - Time.realtimeSinceStartupAsDouble * 1000.0);
		}

		// Token: 0x060053C5 RID: 21445 RVA: 0x00196714 File Offset: 0x00194914
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

		// Token: 0x060053C6 RID: 21446 RVA: 0x0019674F File Offset: 0x0019494F
		public static long TimeAsMilliseconds()
		{
			return GTTime.GetStartupTimeAsMilliseconds() + (long)(Time.realtimeSinceStartupAsDouble * 1000.0);
		}

		// Token: 0x060053C7 RID: 21447 RVA: 0x00196767 File Offset: 0x00194967
		public static double TimeAsDouble()
		{
			return (double)GTTime.GetStartupTimeAsMilliseconds() / 1000.0 + Time.realtimeSinceStartupAsDouble;
		}

		// Token: 0x060053C8 RID: 21448 RVA: 0x00196780 File Offset: 0x00194980
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

		// Token: 0x060053C9 RID: 21449 RVA: 0x001967E8 File Offset: 0x001949E8
		public static string GetAAxiomDateTimeAsStringForDisplay()
		{
			return GTTime.GetAAxiomDateTime().ToString("yyyy-MM-dd HH:mm:ss.fff");
		}

		// Token: 0x060053CA RID: 21450 RVA: 0x00196808 File Offset: 0x00194A08
		public static string GetAAxiomDateTimeAsStringForFilename()
		{
			return GTTime.GetAAxiomDateTime().ToString("yyyy-MM-dd_HH-mm-ss-fff");
		}

		// Token: 0x060053CB RID: 21451 RVA: 0x00196828 File Offset: 0x00194A28
		public static long GetAAxiomDateTimeAsHumanReadableLong()
		{
			return long.Parse(GTTime.GetAAxiomDateTime().ToString("yyyyMMddHHmmssfff00"));
		}

		// Token: 0x060053CC RID: 21452 RVA: 0x0019684C File Offset: 0x00194A4C
		public static DateTime ConvertDateTimeHumanReadableLongToDateTime(long humanReadableLong)
		{
			return DateTime.ParseExact(humanReadableLong.ToString(), "yyyyMMddHHmmssfff'00'", CultureInfo.InvariantCulture);
		}
	}
}
