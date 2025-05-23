using System;
using AOT;
using Viveport.Internal;

namespace Viveport
{
	// Token: 0x02000A69 RID: 2665
	internal class Token
	{
		// Token: 0x06003F9C RID: 16284 RVA: 0x0012B064 File Offset: 0x00129264
		[MonoPInvokeCallback(typeof(StatusCallback))]
		private static void IsReadyIl2cppCallback(int errorCode)
		{
			Token.isReadyIl2cppCallback(errorCode);
		}

		// Token: 0x06003F9D RID: 16285 RVA: 0x0012B074 File Offset: 0x00129274
		public static void IsReady(StatusCallback callback)
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null");
			}
			Token.isReadyIl2cppCallback = new StatusCallback(callback.Invoke);
			Api.InternalStatusCallbacks.Add(new StatusCallback(Token.IsReadyIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				Token.IsReady_64(new StatusCallback(Token.IsReadyIl2cppCallback));
				return;
			}
			Token.IsReady(new StatusCallback(Token.IsReadyIl2cppCallback));
		}

		// Token: 0x06003F9E RID: 16286 RVA: 0x0012B0E3 File Offset: 0x001292E3
		[MonoPInvokeCallback(typeof(StatusCallback2))]
		private static void GetSessionTokenIl2cppCallback(int errorCode, string message)
		{
			Token.getSessionTokenIl2cppCallback(errorCode, message);
		}

		// Token: 0x06003F9F RID: 16287 RVA: 0x0012B0F4 File Offset: 0x001292F4
		public static void GetSessionToken(StatusCallback2 callback)
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null");
			}
			Token.getSessionTokenIl2cppCallback = new StatusCallback2(callback.Invoke);
			Api.InternalStatusCallback2s.Add(new StatusCallback2(Token.GetSessionTokenIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				Token.GetSessionToken_64(new StatusCallback2(Token.GetSessionTokenIl2cppCallback));
				return;
			}
			Token.GetSessionToken(new StatusCallback2(Token.GetSessionTokenIl2cppCallback));
		}

		// Token: 0x0400438E RID: 17294
		private static StatusCallback isReadyIl2cppCallback;

		// Token: 0x0400438F RID: 17295
		private static StatusCallback2 getSessionTokenIl2cppCallback;
	}
}
