using System;
using AOT;
using Viveport.Internal;

namespace Viveport
{
	// Token: 0x02000A51 RID: 2641
	public class UserStats
	{
		// Token: 0x06003EBA RID: 16058 RVA: 0x00128B20 File Offset: 0x00126D20
		[MonoPInvokeCallback(typeof(StatusCallback))]
		private static void IsReadyIl2cppCallback(int errorCode)
		{
			UserStats.isReadyIl2cppCallback(errorCode);
		}

		// Token: 0x06003EBB RID: 16059 RVA: 0x00128B30 File Offset: 0x00126D30
		public static int IsReady(StatusCallback callback)
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null");
			}
			UserStats.isReadyIl2cppCallback = new StatusCallback(callback.Invoke);
			Api.InternalStatusCallbacks.Add(new StatusCallback(UserStats.IsReadyIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				return UserStats.IsReady_64(new StatusCallback(UserStats.IsReadyIl2cppCallback));
			}
			return UserStats.IsReady(new StatusCallback(UserStats.IsReadyIl2cppCallback));
		}

		// Token: 0x06003EBC RID: 16060 RVA: 0x00128B9D File Offset: 0x00126D9D
		[MonoPInvokeCallback(typeof(StatusCallback))]
		private static void DownloadStatsIl2cppCallback(int errorCode)
		{
			UserStats.downloadStatsIl2cppCallback(errorCode);
		}

		// Token: 0x06003EBD RID: 16061 RVA: 0x00128BAC File Offset: 0x00126DAC
		public static int DownloadStats(StatusCallback callback)
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null");
			}
			UserStats.downloadStatsIl2cppCallback = new StatusCallback(callback.Invoke);
			Api.InternalStatusCallbacks.Add(new StatusCallback(UserStats.DownloadStatsIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				return UserStats.DownloadStats_64(new StatusCallback(UserStats.DownloadStatsIl2cppCallback));
			}
			return UserStats.DownloadStats(new StatusCallback(UserStats.DownloadStatsIl2cppCallback));
		}

		// Token: 0x06003EBE RID: 16062 RVA: 0x00128C1C File Offset: 0x00126E1C
		public static int GetStat(string name, int defaultValue)
		{
			int num = defaultValue;
			if (IntPtr.Size == 8)
			{
				UserStats.GetStat_64(name, ref num);
			}
			else
			{
				UserStats.GetStat(name, ref num);
			}
			return num;
		}

		// Token: 0x06003EBF RID: 16063 RVA: 0x00128C48 File Offset: 0x00126E48
		public static float GetStat(string name, float defaultValue)
		{
			float num = defaultValue;
			if (IntPtr.Size == 8)
			{
				UserStats.GetStat_64(name, ref num);
			}
			else
			{
				UserStats.GetStat(name, ref num);
			}
			return num;
		}

		// Token: 0x06003EC0 RID: 16064 RVA: 0x00128C74 File Offset: 0x00126E74
		public static void SetStat(string name, int value)
		{
			if (IntPtr.Size == 8)
			{
				UserStats.SetStat_64(name, value);
				return;
			}
			UserStats.SetStat(name, value);
		}

		// Token: 0x06003EC1 RID: 16065 RVA: 0x00128C8F File Offset: 0x00126E8F
		public static void SetStat(string name, float value)
		{
			if (IntPtr.Size == 8)
			{
				UserStats.SetStat_64(name, value);
				return;
			}
			UserStats.SetStat(name, value);
		}

		// Token: 0x06003EC2 RID: 16066 RVA: 0x00128CAA File Offset: 0x00126EAA
		[MonoPInvokeCallback(typeof(StatusCallback))]
		private static void UploadStatsIl2cppCallback(int errorCode)
		{
			UserStats.uploadStatsIl2cppCallback(errorCode);
		}

		// Token: 0x06003EC3 RID: 16067 RVA: 0x00128CB8 File Offset: 0x00126EB8
		public static int UploadStats(StatusCallback callback)
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null");
			}
			UserStats.uploadStatsIl2cppCallback = new StatusCallback(callback.Invoke);
			Api.InternalStatusCallbacks.Add(new StatusCallback(UserStats.UploadStatsIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				return UserStats.UploadStats_64(new StatusCallback(UserStats.UploadStatsIl2cppCallback));
			}
			return UserStats.UploadStats(new StatusCallback(UserStats.UploadStatsIl2cppCallback));
		}

		// Token: 0x06003EC4 RID: 16068 RVA: 0x00128D28 File Offset: 0x00126F28
		public static bool GetAchievement(string pchName)
		{
			int num = 0;
			if (IntPtr.Size == 8)
			{
				UserStats.GetAchievement_64(pchName, ref num);
			}
			else
			{
				UserStats.GetAchievement(pchName, ref num);
			}
			return num == 1;
		}

		// Token: 0x06003EC5 RID: 16069 RVA: 0x00128D58 File Offset: 0x00126F58
		public static int GetAchievementUnlockTime(string pchName)
		{
			int num = 0;
			if (IntPtr.Size == 8)
			{
				UserStats.GetAchievementUnlockTime_64(pchName, ref num);
			}
			else
			{
				UserStats.GetAchievementUnlockTime(pchName, ref num);
			}
			return num;
		}

		// Token: 0x06003EC6 RID: 16070 RVA: 0x00128D84 File Offset: 0x00126F84
		public static string GetAchievementIcon(string pchName)
		{
			return "";
		}

		// Token: 0x06003EC7 RID: 16071 RVA: 0x00128D84 File Offset: 0x00126F84
		public static string GetAchievementDisplayAttribute(string pchName, UserStats.AchievementDisplayAttribute attr)
		{
			return "";
		}

		// Token: 0x06003EC8 RID: 16072 RVA: 0x00128D84 File Offset: 0x00126F84
		public static string GetAchievementDisplayAttribute(string pchName, UserStats.AchievementDisplayAttribute attr, Locale locale)
		{
			return "";
		}

		// Token: 0x06003EC9 RID: 16073 RVA: 0x00128D8B File Offset: 0x00126F8B
		public static int SetAchievement(string pchName)
		{
			if (IntPtr.Size == 8)
			{
				return UserStats.SetAchievement_64(pchName);
			}
			return UserStats.SetAchievement(pchName);
		}

		// Token: 0x06003ECA RID: 16074 RVA: 0x00128DA2 File Offset: 0x00126FA2
		public static int ClearAchievement(string pchName)
		{
			if (IntPtr.Size == 8)
			{
				return UserStats.ClearAchievement_64(pchName);
			}
			return UserStats.ClearAchievement(pchName);
		}

		// Token: 0x06003ECB RID: 16075 RVA: 0x00128DB9 File Offset: 0x00126FB9
		[MonoPInvokeCallback(typeof(StatusCallback))]
		private static void DownloadLeaderboardScoresIl2cppCallback(int errorCode)
		{
			UserStats.downloadLeaderboardScoresIl2cppCallback(errorCode);
		}

		// Token: 0x06003ECC RID: 16076 RVA: 0x00128DC8 File Offset: 0x00126FC8
		public static int DownloadLeaderboardScores(StatusCallback callback, string pchLeaderboardName, UserStats.LeaderBoardRequestType eLeaderboardDataRequest, UserStats.LeaderBoardTimeRange eLeaderboardDataTimeRange, int nRangeStart, int nRangeEnd)
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null");
			}
			UserStats.downloadLeaderboardScoresIl2cppCallback = new StatusCallback(callback.Invoke);
			Api.InternalStatusCallbacks.Add(new StatusCallback(UserStats.DownloadLeaderboardScoresIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				return UserStats.DownloadLeaderboardScores_64(new StatusCallback(UserStats.DownloadLeaderboardScoresIl2cppCallback), pchLeaderboardName, (ELeaderboardDataRequest)eLeaderboardDataRequest, (ELeaderboardDataTimeRange)eLeaderboardDataTimeRange, nRangeStart, nRangeEnd);
			}
			return UserStats.DownloadLeaderboardScores(new StatusCallback(UserStats.DownloadLeaderboardScoresIl2cppCallback), pchLeaderboardName, (ELeaderboardDataRequest)eLeaderboardDataRequest, (ELeaderboardDataTimeRange)eLeaderboardDataTimeRange, nRangeStart, nRangeEnd);
		}

		// Token: 0x06003ECD RID: 16077 RVA: 0x00128E43 File Offset: 0x00127043
		[MonoPInvokeCallback(typeof(StatusCallback))]
		private static void UploadLeaderboardScoreIl2cppCallback(int errorCode)
		{
			UserStats.uploadLeaderboardScoreIl2cppCallback(errorCode);
		}

		// Token: 0x06003ECE RID: 16078 RVA: 0x00128E50 File Offset: 0x00127050
		public static int UploadLeaderboardScore(StatusCallback callback, string pchLeaderboardName, int nScore)
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null");
			}
			UserStats.uploadLeaderboardScoreIl2cppCallback = new StatusCallback(callback.Invoke);
			Api.InternalStatusCallbacks.Add(new StatusCallback(UserStats.UploadLeaderboardScoreIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				return UserStats.UploadLeaderboardScore_64(new StatusCallback(UserStats.UploadLeaderboardScoreIl2cppCallback), pchLeaderboardName, nScore);
			}
			return UserStats.UploadLeaderboardScore(new StatusCallback(UserStats.UploadLeaderboardScoreIl2cppCallback), pchLeaderboardName, nScore);
		}

		// Token: 0x06003ECF RID: 16079 RVA: 0x00128EC4 File Offset: 0x001270C4
		public static Leaderboard GetLeaderboardScore(int index)
		{
			LeaderboardEntry_t leaderboardEntry_t;
			leaderboardEntry_t.m_nGlobalRank = 0;
			leaderboardEntry_t.m_nScore = 0;
			leaderboardEntry_t.m_pUserName = "";
			if (IntPtr.Size == 8)
			{
				UserStats.GetLeaderboardScore_64(index, ref leaderboardEntry_t);
			}
			else
			{
				UserStats.GetLeaderboardScore(index, ref leaderboardEntry_t);
			}
			return new Leaderboard
			{
				Rank = leaderboardEntry_t.m_nGlobalRank,
				Score = leaderboardEntry_t.m_nScore,
				UserName = leaderboardEntry_t.m_pUserName
			};
		}

		// Token: 0x06003ED0 RID: 16080 RVA: 0x00128F32 File Offset: 0x00127132
		public static int GetLeaderboardScoreCount()
		{
			if (IntPtr.Size == 8)
			{
				return UserStats.GetLeaderboardScoreCount_64();
			}
			return UserStats.GetLeaderboardScoreCount();
		}

		// Token: 0x06003ED1 RID: 16081 RVA: 0x00128F47 File Offset: 0x00127147
		public static UserStats.LeaderBoardSortMethod GetLeaderboardSortMethod()
		{
			if (IntPtr.Size == 8)
			{
				return (UserStats.LeaderBoardSortMethod)UserStats.GetLeaderboardSortMethod_64();
			}
			return (UserStats.LeaderBoardSortMethod)UserStats.GetLeaderboardSortMethod();
		}

		// Token: 0x06003ED2 RID: 16082 RVA: 0x00128F5C File Offset: 0x0012715C
		public static UserStats.LeaderBoardDiaplayType GetLeaderboardDisplayType()
		{
			if (IntPtr.Size == 8)
			{
				return (UserStats.LeaderBoardDiaplayType)UserStats.GetLeaderboardDisplayType_64();
			}
			return (UserStats.LeaderBoardDiaplayType)UserStats.GetLeaderboardDisplayType();
		}

		// Token: 0x04004329 RID: 17193
		private static StatusCallback isReadyIl2cppCallback;

		// Token: 0x0400432A RID: 17194
		private static StatusCallback downloadStatsIl2cppCallback;

		// Token: 0x0400432B RID: 17195
		private static StatusCallback uploadStatsIl2cppCallback;

		// Token: 0x0400432C RID: 17196
		private static StatusCallback downloadLeaderboardScoresIl2cppCallback;

		// Token: 0x0400432D RID: 17197
		private static StatusCallback uploadLeaderboardScoreIl2cppCallback;

		// Token: 0x02000A52 RID: 2642
		public enum LeaderBoardRequestType
		{
			// Token: 0x0400432F RID: 17199
			GlobalData,
			// Token: 0x04004330 RID: 17200
			GlobalDataAroundUser,
			// Token: 0x04004331 RID: 17201
			LocalData,
			// Token: 0x04004332 RID: 17202
			LocalDataAroundUser
		}

		// Token: 0x02000A53 RID: 2643
		public enum LeaderBoardTimeRange
		{
			// Token: 0x04004334 RID: 17204
			AllTime,
			// Token: 0x04004335 RID: 17205
			Daily,
			// Token: 0x04004336 RID: 17206
			Weekly,
			// Token: 0x04004337 RID: 17207
			Monthly
		}

		// Token: 0x02000A54 RID: 2644
		public enum LeaderBoardSortMethod
		{
			// Token: 0x04004339 RID: 17209
			None,
			// Token: 0x0400433A RID: 17210
			Ascending,
			// Token: 0x0400433B RID: 17211
			Descending
		}

		// Token: 0x02000A55 RID: 2645
		public enum LeaderBoardDiaplayType
		{
			// Token: 0x0400433D RID: 17213
			None,
			// Token: 0x0400433E RID: 17214
			Numeric,
			// Token: 0x0400433F RID: 17215
			TimeSeconds,
			// Token: 0x04004340 RID: 17216
			TimeMilliSeconds
		}

		// Token: 0x02000A56 RID: 2646
		public enum LeaderBoardScoreMethod
		{
			// Token: 0x04004342 RID: 17218
			None,
			// Token: 0x04004343 RID: 17219
			KeepBest,
			// Token: 0x04004344 RID: 17220
			ForceUpdate
		}

		// Token: 0x02000A57 RID: 2647
		public enum AchievementDisplayAttribute
		{
			// Token: 0x04004346 RID: 17222
			Name,
			// Token: 0x04004347 RID: 17223
			Desc,
			// Token: 0x04004348 RID: 17224
			Hidden
		}
	}
}
