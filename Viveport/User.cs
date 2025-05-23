using System;
using System.Text;
using AOT;
using Viveport.Internal;

namespace Viveport
{
	// Token: 0x02000A50 RID: 2640
	public class User
	{
		// Token: 0x06003EB4 RID: 16052 RVA: 0x001289E1 File Offset: 0x00126BE1
		[MonoPInvokeCallback(typeof(StatusCallback))]
		private static void IsReadyIl2cppCallback(int errorCode)
		{
			User.isReadyIl2cppCallback(errorCode);
		}

		// Token: 0x06003EB5 RID: 16053 RVA: 0x001289F0 File Offset: 0x00126BF0
		public static int IsReady(StatusCallback callback)
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null");
			}
			User.isReadyIl2cppCallback = new StatusCallback(callback.Invoke);
			Api.InternalStatusCallbacks.Add(new StatusCallback(User.IsReadyIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				return User.IsReady_64(new StatusCallback(User.IsReadyIl2cppCallback));
			}
			return User.IsReady(new StatusCallback(User.IsReadyIl2cppCallback));
		}

		// Token: 0x06003EB6 RID: 16054 RVA: 0x00128A60 File Offset: 0x00126C60
		public static string GetUserId()
		{
			StringBuilder stringBuilder = new StringBuilder(256);
			if (IntPtr.Size == 8)
			{
				User.GetUserID_64(stringBuilder, 256);
			}
			else
			{
				User.GetUserID(stringBuilder, 256);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06003EB7 RID: 16055 RVA: 0x00128AA0 File Offset: 0x00126CA0
		public static string GetUserName()
		{
			StringBuilder stringBuilder = new StringBuilder(256);
			if (IntPtr.Size == 8)
			{
				User.GetUserName_64(stringBuilder, 256);
			}
			else
			{
				User.GetUserName(stringBuilder, 256);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06003EB8 RID: 16056 RVA: 0x00128AE0 File Offset: 0x00126CE0
		public static string GetUserAvatarUrl()
		{
			StringBuilder stringBuilder = new StringBuilder(512);
			if (IntPtr.Size == 8)
			{
				User.GetUserAvatarUrl_64(stringBuilder, 512);
			}
			else
			{
				User.GetUserAvatarUrl(stringBuilder, 512);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x04004325 RID: 17189
		private static StatusCallback isReadyIl2cppCallback;

		// Token: 0x04004326 RID: 17190
		private const int MaxIdLength = 256;

		// Token: 0x04004327 RID: 17191
		private const int MaxNameLength = 256;

		// Token: 0x04004328 RID: 17192
		private const int MaxUrlLength = 512;
	}
}
