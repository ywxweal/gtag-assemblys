using System;
using System.Text;
using AOT;
using Viveport.Internal;

namespace Viveport
{
	// Token: 0x02000A68 RID: 2664
	public class Deeplink
	{
		// Token: 0x06003F91 RID: 16273 RVA: 0x0012AE61 File Offset: 0x00129061
		[MonoPInvokeCallback(typeof(StatusCallback))]
		private static void IsReadyIl2cppCallback(int errorCode)
		{
			Deeplink.isReadyIl2cppCallback(errorCode);
		}

		// Token: 0x06003F92 RID: 16274 RVA: 0x0012AE70 File Offset: 0x00129070
		public static void IsReady(StatusCallback callback)
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null");
			}
			Deeplink.isReadyIl2cppCallback = new StatusCallback(callback.Invoke);
			Api.InternalStatusCallbacks.Add(new StatusCallback(Deeplink.IsReadyIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				Deeplink.IsReady_64(new StatusCallback(Deeplink.IsReadyIl2cppCallback));
				return;
			}
			Deeplink.IsReady(new StatusCallback(Deeplink.IsReadyIl2cppCallback));
		}

		// Token: 0x06003F93 RID: 16275 RVA: 0x0012AEDD File Offset: 0x001290DD
		[MonoPInvokeCallback(typeof(StatusCallback2))]
		private static void GoToAppIl2cppCallback(int errorCode, string message)
		{
			Deeplink.goToAppIl2cppCallback(errorCode, message);
		}

		// Token: 0x06003F94 RID: 16276 RVA: 0x0012AEEC File Offset: 0x001290EC
		public static void GoToApp(StatusCallback2 callback, string viveportId, string launchData)
		{
			if (callback == null || string.IsNullOrEmpty(viveportId))
			{
				throw new InvalidOperationException("callback == null || string.IsNullOrEmpty(viveportId)");
			}
			Deeplink.goToAppIl2cppCallback = new StatusCallback2(callback.Invoke);
			Api.InternalStatusCallback2s.Add(new StatusCallback2(Deeplink.GoToAppIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				Deeplink.GoToApp_64(new StatusCallback2(Deeplink.GoToAppIl2cppCallback), viveportId, launchData);
				return;
			}
			Deeplink.GoToApp(new StatusCallback2(Deeplink.GoToAppIl2cppCallback), viveportId, launchData);
		}

		// Token: 0x06003F95 RID: 16277 RVA: 0x0012AF65 File Offset: 0x00129165
		[MonoPInvokeCallback(typeof(StatusCallback2))]
		private static void GoToAppWithBranchNameIl2cppCallback(int errorCode, string message)
		{
			Deeplink.goToAppWithBranchNameIl2cppCallback(errorCode, message);
		}

		// Token: 0x06003F96 RID: 16278 RVA: 0x0012AF74 File Offset: 0x00129174
		public static void GoToApp(StatusCallback2 callback, string viveportId, string launchData, string branchName)
		{
			if (callback == null || string.IsNullOrEmpty(viveportId))
			{
				throw new InvalidOperationException("callback == null || string.IsNullOrEmpty(viveportId)");
			}
			Deeplink.goToAppWithBranchNameIl2cppCallback = new StatusCallback2(callback.Invoke);
			Api.InternalStatusCallback2s.Add(new StatusCallback2(Deeplink.GoToAppWithBranchNameIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				Deeplink.GoToApp_64(new StatusCallback2(Deeplink.GoToAppWithBranchNameIl2cppCallback), viveportId, launchData, branchName);
				return;
			}
			Deeplink.GoToApp(new StatusCallback2(Deeplink.GoToAppWithBranchNameIl2cppCallback), viveportId, launchData, branchName);
		}

		// Token: 0x06003F97 RID: 16279 RVA: 0x0012AFEF File Offset: 0x001291EF
		[MonoPInvokeCallback(typeof(StatusCallback2))]
		private static void GoToStoreIl2cppCallback(int errorCode, string message)
		{
			Deeplink.goToStoreIl2cppCallback(errorCode, message);
		}

		// Token: 0x06003F98 RID: 16280 RVA: 0x0012B000 File Offset: 0x00129200
		public static void GoToStore(StatusCallback2 callback, string viveportId = "")
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null || string.IsNullOrEmpty(viveportId)");
			}
			Deeplink.goToStoreIl2cppCallback = new StatusCallback2(callback.Invoke);
			Api.InternalStatusCallback2s.Add(new StatusCallback2(Deeplink.GoToStoreIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				Deeplink.GoToStore_64(new StatusCallback2(Deeplink.GoToStoreIl2cppCallback), viveportId);
				return;
			}
			Deeplink.GoToStore(new StatusCallback2(Deeplink.GoToStoreIl2cppCallback), viveportId);
		}

		// Token: 0x06003F99 RID: 16281 RVA: 0x0012B06F File Offset: 0x0012926F
		[MonoPInvokeCallback(typeof(StatusCallback2))]
		private static void GoToAppOrGoToStoreIl2cppCallback(int errorCode, string message)
		{
			Deeplink.goToAppOrGoToStoreIl2cppCallback(errorCode, message);
		}

		// Token: 0x06003F9A RID: 16282 RVA: 0x0012B080 File Offset: 0x00129280
		public static void GoToAppOrGoToStore(StatusCallback2 callback, string viveportId, string launchData)
		{
			if (callback == null || string.IsNullOrEmpty(viveportId))
			{
				throw new InvalidOperationException("callback == null || string.IsNullOrEmpty(viveportId)");
			}
			Deeplink.goToAppOrGoToStoreIl2cppCallback = new StatusCallback2(callback.Invoke);
			Api.InternalStatusCallback2s.Add(new StatusCallback2(Deeplink.GoToAppOrGoToStoreIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				Deeplink.GoToAppOrGoToStore_64(new StatusCallback2(Deeplink.GoToAppOrGoToStoreIl2cppCallback), viveportId, launchData);
				return;
			}
			Deeplink.GoToAppOrGoToStore(new StatusCallback2(Deeplink.GoToAppOrGoToStoreIl2cppCallback), viveportId, launchData);
		}

		// Token: 0x06003F9B RID: 16283 RVA: 0x0012B0FC File Offset: 0x001292FC
		public static string GetAppLaunchData()
		{
			StringBuilder stringBuilder = new StringBuilder(256);
			if (IntPtr.Size == 8)
			{
				Deeplink.GetAppLaunchData_64(stringBuilder, 256);
			}
			else
			{
				Deeplink.GetAppLaunchData(stringBuilder, 256);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x04004389 RID: 17289
		private const int MaxIdLength = 256;

		// Token: 0x0400438A RID: 17290
		private static StatusCallback isReadyIl2cppCallback;

		// Token: 0x0400438B RID: 17291
		private static StatusCallback2 goToAppIl2cppCallback;

		// Token: 0x0400438C RID: 17292
		private static StatusCallback2 goToAppWithBranchNameIl2cppCallback;

		// Token: 0x0400438D RID: 17293
		private static StatusCallback2 goToStoreIl2cppCallback;

		// Token: 0x0400438E RID: 17294
		private static StatusCallback2 goToAppOrGoToStoreIl2cppCallback;
	}
}
