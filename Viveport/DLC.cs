using System;
using System.Text;
using AOT;
using Viveport.Internal;

namespace Viveport
{
	// Token: 0x02000A66 RID: 2662
	public class DLC
	{
		// Token: 0x06003F87 RID: 16263 RVA: 0x0012ABF8 File Offset: 0x00128DF8
		[MonoPInvokeCallback(typeof(StatusCallback))]
		private static void IsDlcReadyIl2cppCallback(int errorCode)
		{
			DLC.isDlcReadyIl2cppCallback(errorCode);
		}

		// Token: 0x06003F88 RID: 16264 RVA: 0x0012AC08 File Offset: 0x00128E08
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

		// Token: 0x06003F89 RID: 16265 RVA: 0x0012AC75 File Offset: 0x00128E75
		public static int GetCount()
		{
			if (IntPtr.Size == 8)
			{
				return DLC.GetCount_64();
			}
			return DLC.GetCount();
		}

		// Token: 0x06003F8A RID: 16266 RVA: 0x0012AC8C File Offset: 0x00128E8C
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

		// Token: 0x06003F8B RID: 16267 RVA: 0x0012ACC8 File Offset: 0x00128EC8
		public static int IsSubscribed()
		{
			if (IntPtr.Size == 8)
			{
				return DLC.IsSubscribed_64();
			}
			return DLC.IsSubscribed();
		}

		// Token: 0x04004386 RID: 17286
		private static StatusCallback isDlcReadyIl2cppCallback;

		// Token: 0x04004387 RID: 17287
		private const int AppIdLength = 37;
	}
}
