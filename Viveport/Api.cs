using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using AOT;
using LitJson;
using PublicKeyConvert;
using Viveport.Core;
using Viveport.Internal;

namespace Viveport
{
	// Token: 0x02000A4E RID: 2638
	public class Api
	{
		// Token: 0x06003EA5 RID: 16037 RVA: 0x0012841C File Offset: 0x0012661C
		public static void GetLicense(Api.LicenseChecker checker, string appId, string appKey)
		{
			if (checker == null || string.IsNullOrEmpty(appId) || string.IsNullOrEmpty(appKey))
			{
				throw new InvalidOperationException("checker == null || string.IsNullOrEmpty(appId) || string.IsNullOrEmpty(appKey)");
			}
			Api._appId = appId;
			Api._appKey = appKey;
			Api.InternalLicenseCheckers.Add(checker);
			if (IntPtr.Size == 8)
			{
				Api.GetLicense_64(new GetLicenseCallback(Api.GetLicenseHandler), Api._appId, Api._appKey);
				return;
			}
			Api.GetLicense(new GetLicenseCallback(Api.GetLicenseHandler), Api._appId, Api._appKey);
		}

		// Token: 0x06003EA6 RID: 16038 RVA: 0x0012849D File Offset: 0x0012669D
		[MonoPInvokeCallback(typeof(StatusCallback))]
		private static void InitIl2cppCallback(int errorCode)
		{
			Api.initIl2cppCallback(errorCode);
		}

		// Token: 0x06003EA7 RID: 16039 RVA: 0x001284AC File Offset: 0x001266AC
		public static int Init(StatusCallback callback, string appId)
		{
			if (callback == null || string.IsNullOrEmpty(appId))
			{
				throw new InvalidOperationException("callback == null || string.IsNullOrEmpty(appId)");
			}
			Api.initIl2cppCallback = new StatusCallback(callback.Invoke);
			Api.InternalStatusCallbacks.Add(new StatusCallback(Api.InitIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				return Api.Init_64(new StatusCallback(Api.InitIl2cppCallback), appId);
			}
			return Api.Init(new StatusCallback(Api.InitIl2cppCallback), appId);
		}

		// Token: 0x06003EA8 RID: 16040 RVA: 0x00128523 File Offset: 0x00126723
		[MonoPInvokeCallback(typeof(StatusCallback))]
		private static void ShutdownIl2cppCallback(int errorCode)
		{
			Api.shutdownIl2cppCallback(errorCode);
		}

		// Token: 0x06003EA9 RID: 16041 RVA: 0x00128530 File Offset: 0x00126730
		public static int Shutdown(StatusCallback callback)
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null");
			}
			Api.shutdownIl2cppCallback = new StatusCallback(callback.Invoke);
			Api.InternalStatusCallbacks.Add(new StatusCallback(Api.ShutdownIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				return Api.Shutdown_64(new StatusCallback(Api.ShutdownIl2cppCallback));
			}
			return Api.Shutdown(new StatusCallback(Api.ShutdownIl2cppCallback));
		}

		// Token: 0x06003EAA RID: 16042 RVA: 0x001285A0 File Offset: 0x001267A0
		public static string Version()
		{
			string text = "";
			try
			{
				if (IntPtr.Size == 8)
				{
					text += Marshal.PtrToStringAnsi(Api.Version_64());
				}
				else
				{
					text += Marshal.PtrToStringAnsi(Api.Version());
				}
			}
			catch (Exception)
			{
				Logger.Log("Can not load version from native library");
			}
			return "C# version: " + Api.VERSION + ", Native version: " + text;
		}

		// Token: 0x06003EAB RID: 16043 RVA: 0x00128614 File Offset: 0x00126814
		[MonoPInvokeCallback(typeof(QueryRuntimeModeCallback))]
		private static void QueryRuntimeModeIl2cppCallback(int errorCode, int mode)
		{
			Api.queryRuntimeModeIl2cppCallback(errorCode, mode);
		}

		// Token: 0x06003EAC RID: 16044 RVA: 0x00128624 File Offset: 0x00126824
		public static void QueryRuntimeMode(QueryRuntimeModeCallback callback)
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null");
			}
			Api.queryRuntimeModeIl2cppCallback = new QueryRuntimeModeCallback(callback.Invoke);
			Api.InternalQueryRunTimeCallbacks.Add(new QueryRuntimeModeCallback(Api.QueryRuntimeModeIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				Api.QueryRuntimeMode_64(new QueryRuntimeModeCallback(Api.QueryRuntimeModeIl2cppCallback));
				return;
			}
			Api.QueryRuntimeMode(new QueryRuntimeModeCallback(Api.QueryRuntimeModeIl2cppCallback));
		}

		// Token: 0x06003EAD RID: 16045 RVA: 0x00128694 File Offset: 0x00126894
		[MonoPInvokeCallback(typeof(GetLicenseCallback))]
		private static void GetLicenseHandler([MarshalAs(UnmanagedType.LPStr)] string message, [MarshalAs(UnmanagedType.LPStr)] string signature)
		{
			if (string.IsNullOrEmpty(message))
			{
				for (int i = Api.InternalLicenseCheckers.Count - 1; i >= 0; i--)
				{
					Api.LicenseChecker licenseChecker = Api.InternalLicenseCheckers[i];
					licenseChecker.OnFailure(90003, "License message is empty");
					Api.InternalLicenseCheckers.Remove(licenseChecker);
				}
				return;
			}
			if (string.IsNullOrEmpty(signature))
			{
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = 99999;
				string text = "";
				try
				{
					num = int.Parse((string)jsonData["code"]);
				}
				catch
				{
				}
				try
				{
					text = (string)jsonData["message"];
				}
				catch
				{
				}
				for (int j = Api.InternalLicenseCheckers.Count - 1; j >= 0; j--)
				{
					Api.LicenseChecker licenseChecker2 = Api.InternalLicenseCheckers[j];
					licenseChecker2.OnFailure(num, text);
					Api.InternalLicenseCheckers.Remove(licenseChecker2);
				}
				return;
			}
			if (!Api.VerifyMessage(Api._appId, Api._appKey, message, signature))
			{
				for (int k = Api.InternalLicenseCheckers.Count - 1; k >= 0; k--)
				{
					Api.LicenseChecker licenseChecker3 = Api.InternalLicenseCheckers[k];
					licenseChecker3.OnFailure(90001, "License verification failed");
					Api.InternalLicenseCheckers.Remove(licenseChecker3);
				}
				return;
			}
			string @string = Encoding.UTF8.GetString(Convert.FromBase64String(message.Substring(message.IndexOf("\n", StringComparison.Ordinal) + 1)));
			JsonData jsonData2 = JsonMapper.ToObject(@string);
			Logger.Log("License: " + @string);
			long num2 = -1L;
			long num3 = -1L;
			int num4 = -1;
			bool flag = false;
			try
			{
				num2 = (long)jsonData2["issueTime"];
			}
			catch
			{
			}
			try
			{
				num3 = (long)jsonData2["expirationTime"];
			}
			catch
			{
			}
			try
			{
				num4 = (int)jsonData2["latestVersion"];
			}
			catch
			{
			}
			try
			{
				flag = (bool)jsonData2["updateRequired"];
			}
			catch
			{
			}
			for (int l = Api.InternalLicenseCheckers.Count - 1; l >= 0; l--)
			{
				Api.LicenseChecker licenseChecker4 = Api.InternalLicenseCheckers[l];
				licenseChecker4.OnSuccess(num2, num3, num4, flag);
				Api.InternalLicenseCheckers.Remove(licenseChecker4);
			}
		}

		// Token: 0x06003EAE RID: 16046 RVA: 0x00128920 File Offset: 0x00126B20
		private static bool VerifyMessage(string appId, string appKey, string message, string signature)
		{
			try
			{
				RSACryptoServiceProvider rsacryptoServiceProvider = PEMKeyLoader.CryptoServiceProviderFromPublicKeyInfo(appKey);
				byte[] array = Convert.FromBase64String(signature);
				SHA1Managed sha1Managed = new SHA1Managed();
				byte[] bytes = Encoding.UTF8.GetBytes(appId + "\n" + message);
				return rsacryptoServiceProvider.VerifyData(bytes, sha1Managed, array);
			}
			catch (Exception ex)
			{
				Logger.Log(ex.ToString());
			}
			return false;
		}

		// Token: 0x0400431A RID: 17178
		internal static readonly List<GetLicenseCallback> InternalGetLicenseCallbacks = new List<GetLicenseCallback>();

		// Token: 0x0400431B RID: 17179
		internal static readonly List<StatusCallback> InternalStatusCallbacks = new List<StatusCallback>();

		// Token: 0x0400431C RID: 17180
		internal static readonly List<QueryRuntimeModeCallback> InternalQueryRunTimeCallbacks = new List<QueryRuntimeModeCallback>();

		// Token: 0x0400431D RID: 17181
		internal static readonly List<StatusCallback2> InternalStatusCallback2s = new List<StatusCallback2>();

		// Token: 0x0400431E RID: 17182
		internal static readonly List<Api.LicenseChecker> InternalLicenseCheckers = new List<Api.LicenseChecker>();

		// Token: 0x0400431F RID: 17183
		private static StatusCallback initIl2cppCallback;

		// Token: 0x04004320 RID: 17184
		private static StatusCallback shutdownIl2cppCallback;

		// Token: 0x04004321 RID: 17185
		private static QueryRuntimeModeCallback queryRuntimeModeIl2cppCallback;

		// Token: 0x04004322 RID: 17186
		private static readonly string VERSION = "1.7.2.30";

		// Token: 0x04004323 RID: 17187
		private static string _appId = "";

		// Token: 0x04004324 RID: 17188
		private static string _appKey = "";

		// Token: 0x02000A4F RID: 2639
		public abstract class LicenseChecker
		{
			// Token: 0x06003EB1 RID: 16049
			public abstract void OnSuccess(long issueTime, long expirationTime, int latestVersion, bool updateRequired);

			// Token: 0x06003EB2 RID: 16050
			public abstract void OnFailure(int errorCode, string errorMessage);
		}
	}
}
