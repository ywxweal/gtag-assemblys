using System;
using System.Runtime.InteropServices;
using AOT;
using LitJson;
using Viveport.Core;
using Viveport.Internal.Arcade;

namespace Viveport.Arcade
{
	// Token: 0x02000A84 RID: 2692
	internal class Session
	{
		// Token: 0x0600404F RID: 16463 RVA: 0x0012B1BD File Offset: 0x001293BD
		[MonoPInvokeCallback(typeof(SessionCallback))]
		private static void IsReadyIl2cppCallback(int errorCode, string message)
		{
			Session.isReadyIl2cppCallback(errorCode, message);
		}

		// Token: 0x06004050 RID: 16464 RVA: 0x0012B1CB File Offset: 0x001293CB
		public static void IsReady(Session.SessionListener listener)
		{
			Session.isReadyIl2cppCallback = new Session.SessionHandler(listener).getIsReadyHandler();
			if (IntPtr.Size == 8)
			{
				Session.IsReady_64(new SessionCallback(Session.IsReadyIl2cppCallback));
				return;
			}
			Session.IsReady(new SessionCallback(Session.IsReadyIl2cppCallback));
		}

		// Token: 0x06004051 RID: 16465 RVA: 0x0012B208 File Offset: 0x00129408
		[MonoPInvokeCallback(typeof(SessionCallback))]
		private static void StartIl2cppCallback(int errorCode, string message)
		{
			Session.startIl2cppCallback(errorCode, message);
		}

		// Token: 0x06004052 RID: 16466 RVA: 0x0012B216 File Offset: 0x00129416
		public static void Start(Session.SessionListener listener)
		{
			Session.startIl2cppCallback = new Session.SessionHandler(listener).getStartHandler();
			if (IntPtr.Size == 8)
			{
				Session.Start_64(new SessionCallback(Session.StartIl2cppCallback));
				return;
			}
			Session.Start(new SessionCallback(Session.StartIl2cppCallback));
		}

		// Token: 0x06004053 RID: 16467 RVA: 0x0012B253 File Offset: 0x00129453
		[MonoPInvokeCallback(typeof(SessionCallback))]
		private static void StopIl2cppCallback(int errorCode, string message)
		{
			Session.stopIl2cppCallback(errorCode, message);
		}

		// Token: 0x06004054 RID: 16468 RVA: 0x0012B261 File Offset: 0x00129461
		public static void Stop(Session.SessionListener listener)
		{
			Session.stopIl2cppCallback = new Session.SessionHandler(listener).getStopHandler();
			if (IntPtr.Size == 8)
			{
				Session.Stop_64(new SessionCallback(Session.StopIl2cppCallback));
				return;
			}
			Session.Stop(new SessionCallback(Session.StopIl2cppCallback));
		}

		// Token: 0x040043BE RID: 17342
		private static SessionCallback isReadyIl2cppCallback;

		// Token: 0x040043BF RID: 17343
		private static SessionCallback startIl2cppCallback;

		// Token: 0x040043C0 RID: 17344
		private static SessionCallback stopIl2cppCallback;

		// Token: 0x02000A85 RID: 2693
		private class SessionHandler : Session.BaseHandler
		{
			// Token: 0x06004056 RID: 16470 RVA: 0x0012B29E File Offset: 0x0012949E
			public SessionHandler(Session.SessionListener cb)
			{
				Session.SessionHandler.listener = cb;
			}

			// Token: 0x06004057 RID: 16471 RVA: 0x0012B2AC File Offset: 0x001294AC
			public SessionCallback getIsReadyHandler()
			{
				return new SessionCallback(this.IsReadyHandler);
			}

			// Token: 0x06004058 RID: 16472 RVA: 0x0012B2BC File Offset: 0x001294BC
			protected override void IsReadyHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[Session IsReadyHandler] message=" + message + ",code=" + code.ToString());
				JsonData jsonData = null;
				try
				{
					jsonData = JsonMapper.ToObject(message);
				}
				catch (Exception ex)
				{
					string text = "[Session IsReadyHandler] exception=";
					Exception ex2 = ex;
					Logger.Log(text + ((ex2 != null) ? ex2.ToString() : null));
				}
				int num = -1;
				string text2 = "";
				string text3 = "";
				if (code == 0 && jsonData != null)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text2 = (string)jsonData["message"];
					}
					catch (Exception ex3)
					{
						string text4 = "[IsReadyHandler] statusCode, message ex=";
						Exception ex4 = ex3;
						Logger.Log(text4 + ((ex4 != null) ? ex4.ToString() : null));
					}
					Logger.Log("[IsReadyHandler] statusCode =" + num.ToString() + ",errMessage=" + text2);
					if (num == 0)
					{
						try
						{
							text3 = (string)jsonData["appID"];
						}
						catch (Exception ex5)
						{
							string text5 = "[IsReadyHandler] appID ex=";
							Exception ex6 = ex5;
							Logger.Log(text5 + ((ex6 != null) ? ex6.ToString() : null));
						}
						Logger.Log("[IsReadyHandler] appID=" + text3);
					}
				}
				if (Session.SessionHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							Session.SessionHandler.listener.OnSuccess(text3);
							return;
						}
						Session.SessionHandler.listener.OnFailure(num, text2);
						return;
					}
					else
					{
						Session.SessionHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x06004059 RID: 16473 RVA: 0x0012B430 File Offset: 0x00129630
			public SessionCallback getStartHandler()
			{
				return new SessionCallback(this.StartHandler);
			}

			// Token: 0x0600405A RID: 16474 RVA: 0x0012B440 File Offset: 0x00129640
			protected override void StartHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[Session StartHandler] message=" + message + ",code=" + code.ToString());
				JsonData jsonData = null;
				try
				{
					jsonData = JsonMapper.ToObject(message);
				}
				catch (Exception ex)
				{
					string text = "[Session StartHandler] exception=";
					Exception ex2 = ex;
					Logger.Log(text + ((ex2 != null) ? ex2.ToString() : null));
				}
				int num = -1;
				string text2 = "";
				string text3 = "";
				string text4 = "";
				if (code == 0 && jsonData != null)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text2 = (string)jsonData["message"];
					}
					catch (Exception ex3)
					{
						string text5 = "[StartHandler] statusCode, message ex=";
						Exception ex4 = ex3;
						Logger.Log(text5 + ((ex4 != null) ? ex4.ToString() : null));
					}
					Logger.Log("[StartHandler] statusCode =" + num.ToString() + ",errMessage=" + text2);
					if (num == 0)
					{
						try
						{
							text3 = (string)jsonData["appID"];
							text4 = (string)jsonData["Guid"];
						}
						catch (Exception ex5)
						{
							string text6 = "[StartHandler] appID, Guid ex=";
							Exception ex6 = ex5;
							Logger.Log(text6 + ((ex6 != null) ? ex6.ToString() : null));
						}
						Logger.Log("[StartHandler] appID=" + text3 + ",Guid=" + text4);
					}
				}
				if (Session.SessionHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							Session.SessionHandler.listener.OnStartSuccess(text3, text4);
							return;
						}
						Session.SessionHandler.listener.OnFailure(num, text2);
						return;
					}
					else
					{
						Session.SessionHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x0600405B RID: 16475 RVA: 0x0012B5D4 File Offset: 0x001297D4
			public SessionCallback getStopHandler()
			{
				return new SessionCallback(this.StopHandler);
			}

			// Token: 0x0600405C RID: 16476 RVA: 0x0012B5E4 File Offset: 0x001297E4
			protected override void StopHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[Session StopHandler] message=" + message + ",code=" + code.ToString());
				JsonData jsonData = null;
				try
				{
					jsonData = JsonMapper.ToObject(message);
				}
				catch (Exception ex)
				{
					string text = "[Session StopHandler] exception=";
					Exception ex2 = ex;
					Logger.Log(text + ((ex2 != null) ? ex2.ToString() : null));
				}
				int num = -1;
				string text2 = "";
				string text3 = "";
				string text4 = "";
				if (code == 0 && jsonData != null)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text2 = (string)jsonData["message"];
					}
					catch (Exception ex3)
					{
						string text5 = "[StopHandler] statusCode, message ex=";
						Exception ex4 = ex3;
						Logger.Log(text5 + ((ex4 != null) ? ex4.ToString() : null));
					}
					Logger.Log("[StopHandler] statusCode =" + num.ToString() + ",errMessage=" + text2);
					if (num == 0)
					{
						try
						{
							text3 = (string)jsonData["appID"];
							text4 = (string)jsonData["Guid"];
						}
						catch (Exception ex5)
						{
							string text6 = "[StopHandler] appID, Guid ex=";
							Exception ex6 = ex5;
							Logger.Log(text6 + ((ex6 != null) ? ex6.ToString() : null));
						}
						Logger.Log("[StopHandler] appID=" + text3 + ",Guid=" + text4);
					}
				}
				if (Session.SessionHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							Session.SessionHandler.listener.OnStopSuccess(text3, text4);
							return;
						}
						Session.SessionHandler.listener.OnFailure(num, text2);
						return;
					}
					else
					{
						Session.SessionHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x040043C1 RID: 17345
			private static Session.SessionListener listener;
		}

		// Token: 0x02000A86 RID: 2694
		private abstract class BaseHandler
		{
			// Token: 0x0600405D RID: 16477
			protected abstract void IsReadyHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x0600405E RID: 16478
			protected abstract void StartHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x0600405F RID: 16479
			protected abstract void StopHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);
		}

		// Token: 0x02000A87 RID: 2695
		public class SessionListener
		{
			// Token: 0x06004061 RID: 16481 RVA: 0x000023F4 File Offset: 0x000005F4
			public virtual void OnSuccess(string pchAppID)
			{
			}

			// Token: 0x06004062 RID: 16482 RVA: 0x000023F4 File Offset: 0x000005F4
			public virtual void OnStartSuccess(string pchAppID, string pchGuid)
			{
			}

			// Token: 0x06004063 RID: 16483 RVA: 0x000023F4 File Offset: 0x000005F4
			public virtual void OnStopSuccess(string pchAppID, string pchGuid)
			{
			}

			// Token: 0x06004064 RID: 16484 RVA: 0x000023F4 File Offset: 0x000005F4
			public virtual void OnFailure(int nCode, string pchMessage)
			{
			}
		}
	}
}
