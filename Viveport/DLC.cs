using System;
using System.Text;
using AOT;
using Viveport.Internal;

namespace Viveport
{
	// Token: 0x02000A66 RID: 2662
	public class DLC
	{
		// Token: 0x06003F86 RID: 16262 RVA: 0x0012AB20 File Offset: 0x00128D20
		[MonoPInvokeCallback(typeof(StatusCallback))]
		private static void IsDlcReadyIl2cppCallback(int errorCode)
		{
			DLC.isDlcReadyIl2cppCallback(errorCode);
		}

		// Token: 0x06003F87 RID: 16263 RVA: 0x0012AB30 File Offset: 0x00128D30
		public static int IsDlcReady(StatusCallback callback)
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null");
			}
			DLC.isDlcReadyIl2cppCallback = new StatusCallback(callback.Invoke);
			Api.InternalStatusCallbacks.Add(new StatusCallback(DLC.IsDlcReadyIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				return DLC.IsReady_64(new StatusCallback(DLC.IsDlcReadyIl2cppCallback));
			}
			return DLC.IsReady(new StatusCallback(DLC.IsDlcReadyIl2cppCallback));
		}

		// Token: 0x06003F88 RID: 16264 RVA: 0x0012AB9D File Offset: 0x00128D9D
		public static int GetCount()
		{
			if (IntPtr.Size == 8)
			{
				return DLC.GetCount_64();
			}
			return DLC.GetCount();
		}

		// Token: 0x06003F89 RID: 16265 RVA: 0x0012ABB4 File Offset: 0x00128DB4
		public static bool GetIsAvailable(int index, out string appId, out bool isAvailable)
		{
			StringBuilder stringBuilder = new StringBuilder(37);
			bool flag;
			if (IntPtr.Size == 8)
			{
				flag = DLC.GetIsAvailable_64(index, stringBuilder, out isAvailable);
			}
			else
			{
				flag = DLC.GetIsAvailable(index, stringBuilder, out isAvailable);
			}
			appId = stringBuilder.ToString();
			return flag;
		}

		// Token: 0x06003F8A RID: 16266 RVA: 0x0012ABF0 File Offset: 0x00128DF0
		public static int IsSubscribed()
		{
			if (IntPtr.Size == 8)
			{
				return DLC.IsSubscribed_64();
			}
			return DLC.IsSubscribed();
		}

		// Token: 0x04004385 RID: 17285
		private static StatusCallback isDlcReadyIl2cppCallback;

		// Token: 0x04004386 RID: 17286
		private const int AppIdLength = 37;
	}
}
