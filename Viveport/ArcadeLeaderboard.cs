using System;
using AOT;
using Viveport.Internal;

namespace Viveport
{
	// Token: 0x02000A58 RID: 2648
	public class ArcadeLeaderboard
	{
		// Token: 0x06003ED5 RID: 16085 RVA: 0x00129049 File Offset: 0x00127249
		[MonoPInvokeCallback(typeof(StatusCallback))]
		private static void IsReadyIl2cppCallback(int errorCode)
		{
			ArcadeLeaderboard.isReadyIl2cppCallback(errorCode);
		}

		// Token: 0x06003ED6 RID: 16086 RVA: 0x00129058 File Offset: 0x00127258
		public static void IsReady(StatusCallback callback)
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null");
			}
			ArcadeLeaderboard.isReadyIl2cppCallback = new StatusCallback(callback.Invoke);
			Api.InternalStatusCallbacks.Add(new StatusCallback(ArcadeLeaderboard.IsReadyIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				ArcadeLeaderboard.IsReady_64(new StatusCallback(ArcadeLeaderboard.IsReadyIl2cppCallback));
				return;
			}
			ArcadeLeaderboard.IsReady(new StatusCallback(ArcadeLeaderboard.IsReadyIl2cppCallback));
		}

		// Token: 0x06003ED7 RID: 16087 RVA: 0x001290C5 File Offset: 0x001272C5
		[MonoPInvokeCallback(typeof(StatusCallback))]
		private static void DownloadLeaderboardScoresIl2cppCallback(int errorCode)
		{
			ArcadeLeaderboard.downloadLeaderboardScoresIl2cppCallback(errorCode);
		}

		// Token: 0x06003ED8 RID: 16088 RVA: 0x001290D4 File Offset: 0x001272D4
		public static void DownloadLeaderboardScores(StatusCallback callback, string pchLeaderboardName, ArcadeLeaderboard.LeaderboardTimeRange eLeaderboardDataTimeRange, int nCount)
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null");
			}
			ArcadeLeaderboard.downloadLeaderboardScoresIl2cppCallback = new StatusCallback(callback.Invoke);
			Api.InternalStatusCallbacks.Add(new StatusCallback(ArcadeLeaderboard.DownloadLeaderboardScoresIl2cppCallback));
			eLeaderboardDataTimeRange = ArcadeLeaderboard.LeaderboardTimeRange.AllTime;
			if (IntPtr.Size == 8)
			{
				ArcadeLeaderboard.DownloadLeaderboardScores_64(new StatusCallback(ArcadeLeaderboard.DownloadLeaderboardScoresIl2cppCallback), pchLeaderboardName, (ELeaderboardDataTimeRange)eLeaderboardDataTimeRange, nCount);
				return;
			}
			ArcadeLeaderboard.DownloadLeaderboardScores(new StatusCallback(ArcadeLeaderboard.DownloadLeaderboardScoresIl2cppCallback), pchLeaderboardName, (ELeaderboardDataTimeRange)eLeaderboardDataTimeRange, nCount);
		}

		// Token: 0x06003ED9 RID: 16089 RVA: 0x0012914A File Offset: 0x0012734A
		[MonoPInvokeCallback(typeof(StatusCallback))]
		private static void UploadLeaderboardScoreIl2cppCallback(int errorCode)
		{
			ArcadeLeaderboard.uploadLeaderboardScoreIl2cppCallback(errorCode);
		}

		// Token: 0x06003EDA RID: 16090 RVA: 0x00129158 File Offset: 0x00127358
		public static void UploadLeaderboardScore(StatusCallback callback, string pchLeaderboardName, string pchUserName, int nScore)
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null");
			}
			ArcadeLeaderboard.uploadLeaderboardScoreIl2cppCallback = new StatusCallback(callback.Invoke);
			Api.InternalStatusCallbacks.Add(new StatusCallback(ArcadeLeaderboard.UploadLeaderboardScoreIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				ArcadeLeaderboard.UploadLeaderboardScore_64(new StatusCallback(ArcadeLeaderboard.UploadLeaderboardScoreIl2cppCallback), pchLeaderboardName, pchUserName, nScore);
				return;
			}
			ArcadeLeaderboard.UploadLeaderboardScore(new StatusCallback(ArcadeLeaderboard.UploadLeaderboardScoreIl2cppCallback), pchLeaderboardName, pchUserName, nScore);
		}

		// Token: 0x06003EDB RID: 16091 RVA: 0x001291CC File Offset: 0x001273CC
		public static Leaderboard GetLeaderboardScore(int index)
		{
			LeaderboardEntry_t leaderboardEntry_t;
			leaderboardEntry_t.m_nGlobalRank = 0;
			leaderboardEntry_t.m_nScore = 0;
			leaderboardEntry_t.m_pUserName = "";
			if (IntPtr.Size == 8)
			{
				ArcadeLeaderboard.GetLeaderboardScore_64(index, ref leaderboardEntry_t);
			}
			else
			{
				ArcadeLeaderboard.GetLeaderboardScore(index, ref leaderboardEntry_t);
			}
			return new Leaderboard
			{
				Rank = leaderboardEntry_t.m_nGlobalRank,
				Score = leaderboardEntry_t.m_nScore,
				UserName = leaderboardEntry_t.m_pUserName
			};
		}

		// Token: 0x06003EDC RID: 16092 RVA: 0x00129238 File Offset: 0x00127438
		public static int GetLeaderboardScoreCount()
		{
			if (IntPtr.Size == 8)
			{
				return ArcadeLeaderboard.GetLeaderboardScoreCount_64();
			}
			return ArcadeLeaderboard.GetLeaderboardScoreCount();
		}

		// Token: 0x06003EDD RID: 16093 RVA: 0x0012924D File Offset: 0x0012744D
		public static int GetLeaderboardUserRank()
		{
			if (IntPtr.Size == 8)
			{
				return ArcadeLeaderboard.GetLeaderboardUserRank_64();
			}
			return ArcadeLeaderboard.GetLeaderboardUserRank();
		}

		// Token: 0x06003EDE RID: 16094 RVA: 0x00129262 File Offset: 0x00127462
		public static int GetLeaderboardUserScore()
		{
			if (IntPtr.Size == 8)
			{
				return ArcadeLeaderboard.GetLeaderboardUserScore_64();
			}
			return ArcadeLeaderboard.GetLeaderboardUserScore();
		}

		// Token: 0x0400434A RID: 17226
		private static StatusCallback isReadyIl2cppCallback;

		// Token: 0x0400434B RID: 17227
		private static StatusCallback downloadLeaderboardScoresIl2cppCallback;

		// Token: 0x0400434C RID: 17228
		private static StatusCallback uploadLeaderboardScoreIl2cppCallback;

		// Token: 0x02000A59 RID: 2649
		public enum LeaderboardTimeRange
		{
			// Token: 0x0400434E RID: 17230
			AllTime
		}
	}
}
