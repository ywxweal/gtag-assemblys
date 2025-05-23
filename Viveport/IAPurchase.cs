using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using LitJson;
using Viveport.Core;
using Viveport.Internal;

namespace Viveport
{
	// Token: 0x02000A5A RID: 2650
	public class IAPurchase
	{
		// Token: 0x06003EDF RID: 16095 RVA: 0x0012919F File Offset: 0x0012739F
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void IsReadyIl2cppCallback(int errorCode, string message)
		{
			IAPurchase.isReadyIl2cppCallback(errorCode, message);
		}

		// Token: 0x06003EE0 RID: 16096 RVA: 0x001291AD File Offset: 0x001273AD
		public static void IsReady(IAPurchase.IAPurchaseListener listener, string pchAppKey)
		{
			IAPurchase.isReadyIl2cppCallback = new IAPurchase.IAPHandler(listener).getIsReadyHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.IsReady_64(new IAPurchaseCallback(IAPurchase.IsReadyIl2cppCallback), pchAppKey);
				return;
			}
			IAPurchase.IsReady(new IAPurchaseCallback(IAPurchase.IsReadyIl2cppCallback), pchAppKey);
		}

		// Token: 0x06003EE1 RID: 16097 RVA: 0x001291EC File Offset: 0x001273EC
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void Request01Il2cppCallback(int errorCode, string message)
		{
			IAPurchase.request01Il2cppCallback(errorCode, message);
		}

		// Token: 0x06003EE2 RID: 16098 RVA: 0x001291FA File Offset: 0x001273FA
		public static void Request(IAPurchase.IAPurchaseListener listener, string pchPrice)
		{
			IAPurchase.request01Il2cppCallback = new IAPurchase.IAPHandler(listener).getRequestHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.Request_64(new IAPurchaseCallback(IAPurchase.Request01Il2cppCallback), pchPrice);
				return;
			}
			IAPurchase.Request(new IAPurchaseCallback(IAPurchase.Request01Il2cppCallback), pchPrice);
		}

		// Token: 0x06003EE3 RID: 16099 RVA: 0x00129239 File Offset: 0x00127439
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void Request02Il2cppCallback(int errorCode, string message)
		{
			IAPurchase.request02Il2cppCallback(errorCode, message);
		}

		// Token: 0x06003EE4 RID: 16100 RVA: 0x00129248 File Offset: 0x00127448
		public static void Request(IAPurchase.IAPurchaseListener listener, string pchPrice, string pchUserData)
		{
			IAPurchase.request02Il2cppCallback = new IAPurchase.IAPHandler(listener).getRequestHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.Request_64(new IAPurchaseCallback(IAPurchase.Request02Il2cppCallback), pchPrice, pchUserData);
				return;
			}
			IAPurchase.Request(new IAPurchaseCallback(IAPurchase.Request02Il2cppCallback), pchPrice, pchUserData);
		}

		// Token: 0x06003EE5 RID: 16101 RVA: 0x00129294 File Offset: 0x00127494
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void PurchaseIl2cppCallback(int errorCode, string message)
		{
			IAPurchase.purchaseIl2cppCallback(errorCode, message);
		}

		// Token: 0x06003EE6 RID: 16102 RVA: 0x001292A2 File Offset: 0x001274A2
		public static void Purchase(IAPurchase.IAPurchaseListener listener, string pchPurchaseId)
		{
			IAPurchase.purchaseIl2cppCallback = new IAPurchase.IAPHandler(listener).getPurchaseHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.Purchase_64(new IAPurchaseCallback(IAPurchase.PurchaseIl2cppCallback), pchPurchaseId);
				return;
			}
			IAPurchase.Purchase(new IAPurchaseCallback(IAPurchase.PurchaseIl2cppCallback), pchPurchaseId);
		}

		// Token: 0x06003EE7 RID: 16103 RVA: 0x001292E1 File Offset: 0x001274E1
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void Query01Il2cppCallback(int errorCode, string message)
		{
			IAPurchase.query01Il2cppCallback(errorCode, message);
		}

		// Token: 0x06003EE8 RID: 16104 RVA: 0x001292EF File Offset: 0x001274EF
		public static void Query(IAPurchase.IAPurchaseListener listener, string pchPurchaseId)
		{
			IAPurchase.query01Il2cppCallback = new IAPurchase.IAPHandler(listener).getQueryHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.Query_64(new IAPurchaseCallback(IAPurchase.Query01Il2cppCallback), pchPurchaseId);
				return;
			}
			IAPurchase.Query(new IAPurchaseCallback(IAPurchase.Query01Il2cppCallback), pchPurchaseId);
		}

		// Token: 0x06003EE9 RID: 16105 RVA: 0x0012932E File Offset: 0x0012752E
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void Query02Il2cppCallback(int errorCode, string message)
		{
			IAPurchase.query02Il2cppCallback(errorCode, message);
		}

		// Token: 0x06003EEA RID: 16106 RVA: 0x0012933C File Offset: 0x0012753C
		public static void Query(IAPurchase.IAPurchaseListener listener)
		{
			IAPurchase.query02Il2cppCallback = new IAPurchase.IAPHandler(listener).getQueryListHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.Query_64(new IAPurchaseCallback(IAPurchase.Query02Il2cppCallback));
				return;
			}
			IAPurchase.Query(new IAPurchaseCallback(IAPurchase.Query02Il2cppCallback));
		}

		// Token: 0x06003EEB RID: 16107 RVA: 0x00129379 File Offset: 0x00127579
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void GetBalanceIl2cppCallback(int errorCode, string message)
		{
			IAPurchase.getBalanceIl2cppCallback(errorCode, message);
		}

		// Token: 0x06003EEC RID: 16108 RVA: 0x00129387 File Offset: 0x00127587
		public static void GetBalance(IAPurchase.IAPurchaseListener listener)
		{
			IAPurchase.getBalanceIl2cppCallback = new IAPurchase.IAPHandler(listener).getBalanceHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.GetBalance_64(new IAPurchaseCallback(IAPurchase.GetBalanceIl2cppCallback));
				return;
			}
			IAPurchase.GetBalance(new IAPurchaseCallback(IAPurchase.GetBalanceIl2cppCallback));
		}

		// Token: 0x06003EED RID: 16109 RVA: 0x001293C4 File Offset: 0x001275C4
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void RequestSubscriptionIl2cppCallback(int errorCode, string message)
		{
			IAPurchase.requestSubscriptionIl2cppCallback(errorCode, message);
		}

		// Token: 0x06003EEE RID: 16110 RVA: 0x001293D4 File Offset: 0x001275D4
		public static void RequestSubscription(IAPurchase.IAPurchaseListener listener, string pchPrice, string pchFreeTrialType, int nFreeTrialValue, string pchChargePeriodType, int nChargePeriodValue, int nNumberOfChargePeriod, string pchPlanId)
		{
			IAPurchase.requestSubscriptionIl2cppCallback = new IAPurchase.IAPHandler(listener).getRequestSubscriptionHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.RequestSubscription_64(new IAPurchaseCallback(IAPurchase.RequestSubscriptionIl2cppCallback), pchPrice, pchFreeTrialType, nFreeTrialValue, pchChargePeriodType, nChargePeriodValue, nNumberOfChargePeriod, pchPlanId);
				return;
			}
			IAPurchase.RequestSubscription(new IAPurchaseCallback(IAPurchase.RequestSubscriptionIl2cppCallback), pchPrice, pchFreeTrialType, nFreeTrialValue, pchChargePeriodType, nChargePeriodValue, nNumberOfChargePeriod, pchPlanId);
		}

		// Token: 0x06003EEF RID: 16111 RVA: 0x00129432 File Offset: 0x00127632
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void RequestSubscriptionWithPlanIDIl2cppCallback(int errorCode, string message)
		{
			IAPurchase.requestSubscriptionWithPlanIDIl2cppCallback(errorCode, message);
		}

		// Token: 0x06003EF0 RID: 16112 RVA: 0x00129440 File Offset: 0x00127640
		public static void RequestSubscriptionWithPlanID(IAPurchase.IAPurchaseListener listener, string pchPlanId)
		{
			IAPurchase.requestSubscriptionWithPlanIDIl2cppCallback = new IAPurchase.IAPHandler(listener).getRequestSubscriptionWithPlanIDHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.RequestSubscriptionWithPlanID_64(new IAPurchaseCallback(IAPurchase.RequestSubscriptionWithPlanIDIl2cppCallback), pchPlanId);
				return;
			}
			IAPurchase.RequestSubscriptionWithPlanID(new IAPurchaseCallback(IAPurchase.RequestSubscriptionWithPlanIDIl2cppCallback), pchPlanId);
		}

		// Token: 0x06003EF1 RID: 16113 RVA: 0x0012947F File Offset: 0x0012767F
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void SubscribeIl2cppCallback(int errorCode, string message)
		{
			IAPurchase.subscribeIl2cppCallback(errorCode, message);
		}

		// Token: 0x06003EF2 RID: 16114 RVA: 0x0012948D File Offset: 0x0012768D
		public static void Subscribe(IAPurchase.IAPurchaseListener listener, string pchSubscriptionId)
		{
			IAPurchase.subscribeIl2cppCallback = new IAPurchase.IAPHandler(listener).getSubscribeHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.Subscribe_64(new IAPurchaseCallback(IAPurchase.SubscribeIl2cppCallback), pchSubscriptionId);
				return;
			}
			IAPurchase.Subscribe(new IAPurchaseCallback(IAPurchase.SubscribeIl2cppCallback), pchSubscriptionId);
		}

		// Token: 0x06003EF3 RID: 16115 RVA: 0x001294CC File Offset: 0x001276CC
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void QuerySubscriptionIl2cppCallback(int errorCode, string message)
		{
			IAPurchase.querySubscriptionIl2cppCallback(errorCode, message);
		}

		// Token: 0x06003EF4 RID: 16116 RVA: 0x001294DA File Offset: 0x001276DA
		public static void QuerySubscription(IAPurchase.IAPurchaseListener listener, string pchSubscriptionId)
		{
			IAPurchase.querySubscriptionIl2cppCallback = new IAPurchase.IAPHandler(listener).getQuerySubscriptionHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.QuerySubscription_64(new IAPurchaseCallback(IAPurchase.QuerySubscriptionIl2cppCallback), pchSubscriptionId);
				return;
			}
			IAPurchase.QuerySubscription(new IAPurchaseCallback(IAPurchase.QuerySubscriptionIl2cppCallback), pchSubscriptionId);
		}

		// Token: 0x06003EF5 RID: 16117 RVA: 0x00129519 File Offset: 0x00127719
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void QuerySubscriptionListIl2cppCallback(int errorCode, string message)
		{
			IAPurchase.querySubscriptionListIl2cppCallback(errorCode, message);
		}

		// Token: 0x06003EF6 RID: 16118 RVA: 0x00129527 File Offset: 0x00127727
		public static void QuerySubscriptionList(IAPurchase.IAPurchaseListener listener)
		{
			IAPurchase.querySubscriptionListIl2cppCallback = new IAPurchase.IAPHandler(listener).getQuerySubscriptionListHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.QuerySubscriptionList_64(new IAPurchaseCallback(IAPurchase.QuerySubscriptionListIl2cppCallback));
				return;
			}
			IAPurchase.QuerySubscriptionList(new IAPurchaseCallback(IAPurchase.QuerySubscriptionListIl2cppCallback));
		}

		// Token: 0x06003EF7 RID: 16119 RVA: 0x00129564 File Offset: 0x00127764
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void CancelSubscriptionIl2cppCallback(int errorCode, string message)
		{
			IAPurchase.cancelSubscriptionIl2cppCallback(errorCode, message);
		}

		// Token: 0x06003EF8 RID: 16120 RVA: 0x00129572 File Offset: 0x00127772
		public static void CancelSubscription(IAPurchase.IAPurchaseListener listener, string pchSubscriptionId)
		{
			IAPurchase.cancelSubscriptionIl2cppCallback = new IAPurchase.IAPHandler(listener).getCancelSubscriptionHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.CancelSubscription_64(new IAPurchaseCallback(IAPurchase.CancelSubscriptionIl2cppCallback), pchSubscriptionId);
				return;
			}
			IAPurchase.CancelSubscription(new IAPurchaseCallback(IAPurchase.CancelSubscriptionIl2cppCallback), pchSubscriptionId);
		}

		// Token: 0x0400434E RID: 17230
		private static IAPurchaseCallback isReadyIl2cppCallback;

		// Token: 0x0400434F RID: 17231
		private static IAPurchaseCallback request01Il2cppCallback;

		// Token: 0x04004350 RID: 17232
		private static IAPurchaseCallback request02Il2cppCallback;

		// Token: 0x04004351 RID: 17233
		private static IAPurchaseCallback purchaseIl2cppCallback;

		// Token: 0x04004352 RID: 17234
		private static IAPurchaseCallback query01Il2cppCallback;

		// Token: 0x04004353 RID: 17235
		private static IAPurchaseCallback query02Il2cppCallback;

		// Token: 0x04004354 RID: 17236
		private static IAPurchaseCallback getBalanceIl2cppCallback;

		// Token: 0x04004355 RID: 17237
		private static IAPurchaseCallback requestSubscriptionIl2cppCallback;

		// Token: 0x04004356 RID: 17238
		private static IAPurchaseCallback requestSubscriptionWithPlanIDIl2cppCallback;

		// Token: 0x04004357 RID: 17239
		private static IAPurchaseCallback subscribeIl2cppCallback;

		// Token: 0x04004358 RID: 17240
		private static IAPurchaseCallback querySubscriptionIl2cppCallback;

		// Token: 0x04004359 RID: 17241
		private static IAPurchaseCallback querySubscriptionListIl2cppCallback;

		// Token: 0x0400435A RID: 17242
		private static IAPurchaseCallback cancelSubscriptionIl2cppCallback;

		// Token: 0x02000A5B RID: 2651
		private class IAPHandler : IAPurchase.BaseHandler
		{
			// Token: 0x06003EFA RID: 16122 RVA: 0x001295B1 File Offset: 0x001277B1
			public IAPHandler(IAPurchase.IAPurchaseListener cb)
			{
				IAPurchase.IAPHandler.listener = cb;
			}

			// Token: 0x06003EFB RID: 16123 RVA: 0x001295BF File Offset: 0x001277BF
			public IAPurchaseCallback getIsReadyHandler()
			{
				return new IAPurchaseCallback(this.IsReadyHandler);
			}

			// Token: 0x06003EFC RID: 16124 RVA: 0x001295D0 File Offset: 0x001277D0
			protected override void IsReadyHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[IsReadyHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				string text = "";
				string text2 = "";
				if (code == 0)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text2 = (string)jsonData["message"];
					}
					catch (Exception ex)
					{
						string text3 = "[IsReadyHandler] statusCode, message ex=";
						Exception ex2 = ex;
						Logger.Log(text3 + ((ex2 != null) ? ex2.ToString() : null));
					}
					Logger.Log("[IsReadyHandler] statusCode =" + num.ToString() + ",errMessage=" + text2);
					if (num == 0)
					{
						try
						{
							text = (string)jsonData["currencyName"];
						}
						catch (Exception ex3)
						{
							string text4 = "[IsReadyHandler] currencyName ex=";
							Exception ex4 = ex3;
							Logger.Log(text4 + ((ex4 != null) ? ex4.ToString() : null));
						}
						Logger.Log("[IsReadyHandler] currencyName=" + text);
					}
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							IAPurchase.IAPHandler.listener.OnSuccess(text);
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text2);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x06003EFD RID: 16125 RVA: 0x00129700 File Offset: 0x00127900
			public IAPurchaseCallback getRequestHandler()
			{
				return new IAPurchaseCallback(this.RequestHandler);
			}

			// Token: 0x06003EFE RID: 16126 RVA: 0x00129710 File Offset: 0x00127910
			protected override void RequestHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[RequestHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				string text = "";
				string text2 = "";
				if (code == 0)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text2 = (string)jsonData["message"];
					}
					catch (Exception ex)
					{
						string text3 = "[RequestHandler] statusCode, message ex=";
						Exception ex2 = ex;
						Logger.Log(text3 + ((ex2 != null) ? ex2.ToString() : null));
					}
					Logger.Log("[RequestHandler] statusCode =" + num.ToString() + ",errMessage=" + text2);
					if (num == 0)
					{
						try
						{
							text = (string)jsonData["purchase_id"];
						}
						catch (Exception ex3)
						{
							string text4 = "[RequestHandler] purchase_id ex=";
							Exception ex4 = ex3;
							Logger.Log(text4 + ((ex4 != null) ? ex4.ToString() : null));
						}
						Logger.Log("[RequestHandler] purchaseId =" + text);
					}
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							IAPurchase.IAPHandler.listener.OnRequestSuccess(text);
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text2);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x06003EFF RID: 16127 RVA: 0x00129840 File Offset: 0x00127A40
			public IAPurchaseCallback getPurchaseHandler()
			{
				return new IAPurchaseCallback(this.PurchaseHandler);
			}

			// Token: 0x06003F00 RID: 16128 RVA: 0x00129850 File Offset: 0x00127A50
			protected override void PurchaseHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[PurchaseHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				string text = "";
				string text2 = "";
				long num2 = 0L;
				if (code == 0)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text2 = (string)jsonData["message"];
					}
					catch (Exception ex)
					{
						string text3 = "[PurchaseHandler] statusCode, message ex=";
						Exception ex2 = ex;
						Logger.Log(text3 + ((ex2 != null) ? ex2.ToString() : null));
					}
					Logger.Log("[PurchaseHandler] statusCode =" + num.ToString() + ",errMessage=" + text2);
					if (num == 0)
					{
						try
						{
							text = (string)jsonData["purchase_id"];
							num2 = (long)jsonData["paid_timestamp"];
						}
						catch (Exception ex3)
						{
							string text4 = "[PurchaseHandler] purchase_id,paid_timestamp ex=";
							Exception ex4 = ex3;
							Logger.Log(text4 + ((ex4 != null) ? ex4.ToString() : null));
						}
						Logger.Log("[PurchaseHandler] purchaseId =" + text + ",paid_timestamp=" + num2.ToString());
					}
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							IAPurchase.IAPHandler.listener.OnPurchaseSuccess(text);
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text2);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x06003F01 RID: 16129 RVA: 0x001299A0 File Offset: 0x00127BA0
			public IAPurchaseCallback getQueryHandler()
			{
				return new IAPurchaseCallback(this.QueryHandler);
			}

			// Token: 0x06003F02 RID: 16130 RVA: 0x001299B0 File Offset: 0x00127BB0
			protected override void QueryHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[QueryHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				string text = "";
				string text2 = "";
				string text3 = "";
				string text4 = "";
				string text5 = "";
				string text6 = "";
				long num2 = 0L;
				if (code == 0)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text2 = (string)jsonData["message"];
					}
					catch (Exception ex)
					{
						string text7 = "[QueryHandler] statusCode, message ex=";
						Exception ex2 = ex;
						Logger.Log(text7 + ((ex2 != null) ? ex2.ToString() : null));
					}
					Logger.Log("[QueryHandler] statusCode =" + num.ToString() + ",errMessage=" + text2);
					if (num == 0)
					{
						try
						{
							text = (string)jsonData["purchase_id"];
							text3 = (string)jsonData["order_id"];
							text4 = (string)jsonData["status"];
							text5 = (string)jsonData["price"];
							text6 = (string)jsonData["currency"];
							num2 = (long)jsonData["paid_timestamp"];
						}
						catch (Exception ex3)
						{
							string text8 = "[QueryHandler] purchase_id, order_id ex=";
							Exception ex4 = ex3;
							Logger.Log(text8 + ((ex4 != null) ? ex4.ToString() : null));
						}
						Logger.Log(string.Concat(new string[] { "[QueryHandler] status =", text4, ",price=", text5, ",currency=", text6 }));
						Logger.Log(string.Concat(new string[]
						{
							"[QueryHandler] purchaseId =",
							text,
							",order_id=",
							text3,
							",paid_timestamp=",
							num2.ToString()
						}));
					}
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							IAPurchase.QueryResponse queryResponse = new IAPurchase.QueryResponse();
							queryResponse.purchase_id = text;
							queryResponse.order_id = text3;
							queryResponse.price = text5;
							queryResponse.currency = text6;
							queryResponse.paid_timestamp = num2;
							queryResponse.status = text4;
							IAPurchase.IAPHandler.listener.OnQuerySuccess(queryResponse);
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text2);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x06003F03 RID: 16131 RVA: 0x00129BFC File Offset: 0x00127DFC
			public IAPurchaseCallback getQueryListHandler()
			{
				return new IAPurchaseCallback(this.QueryListHandler);
			}

			// Token: 0x06003F04 RID: 16132 RVA: 0x00129C0C File Offset: 0x00127E0C
			protected override void QueryListHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[QueryListHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				int num2 = 0;
				int num3 = 0;
				int num4 = 0;
				List<IAPurchase.QueryResponse2> list = new List<IAPurchase.QueryResponse2>();
				string text = "";
				if (code == 0)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text = (string)jsonData["message"];
					}
					catch (Exception ex)
					{
						string text2 = "[QueryListHandler] statusCode, message ex=";
						Exception ex2 = ex;
						Logger.Log(text2 + ((ex2 != null) ? ex2.ToString() : null));
					}
					Logger.Log("[QueryListHandler] statusCode =" + num.ToString() + ",errMessage=" + text);
					if (num == 0)
					{
						try
						{
							JsonData jsonData2 = JsonMapper.ToObject(text);
							num2 = (int)jsonData2["total"];
							num3 = (int)jsonData2["from"];
							num4 = (int)jsonData2["to"];
							JsonData jsonData3 = jsonData2["purchases"];
							bool isArray = jsonData3.IsArray;
							foreach (object obj in ((IEnumerable)jsonData3))
							{
								JsonData jsonData4 = (JsonData)obj;
								IAPurchase.QueryResponse2 queryResponse = new IAPurchase.QueryResponse2();
								IDictionary dictionary = jsonData4;
								queryResponse.app_id = (dictionary.Contains("app_id") ? ((string)jsonData4["app_id"]) : "");
								queryResponse.currency = (dictionary.Contains("currency") ? ((string)jsonData4["currency"]) : "");
								queryResponse.purchase_id = (dictionary.Contains("purchase_id") ? ((string)jsonData4["purchase_id"]) : "");
								queryResponse.order_id = (dictionary.Contains("order_id") ? ((string)jsonData4["order_id"]) : "");
								queryResponse.price = (dictionary.Contains("price") ? ((string)jsonData4["price"]) : "");
								queryResponse.user_data = (dictionary.Contains("user_data") ? ((string)jsonData4["user_data"]) : "");
								if (dictionary.Contains("paid_timestamp"))
								{
									if (jsonData4["paid_timestamp"].IsLong)
									{
										queryResponse.paid_timestamp = (long)jsonData4["paid_timestamp"];
									}
									else if (jsonData4["paid_timestamp"].IsInt)
									{
										queryResponse.paid_timestamp = (long)(int)jsonData4["paid_timestamp"];
									}
								}
								list.Add(queryResponse);
							}
						}
						catch (Exception ex3)
						{
							string text3 = "[QueryListHandler] purchase_id, order_id ex=";
							Exception ex4 = ex3;
							Logger.Log(text3 + ((ex4 != null) ? ex4.ToString() : null));
						}
					}
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							IAPurchase.QueryListResponse queryListResponse = new IAPurchase.QueryListResponse();
							queryListResponse.total = num2;
							queryListResponse.from = num3;
							queryListResponse.to = num4;
							queryListResponse.purchaseList = list;
							IAPurchase.IAPHandler.listener.OnQuerySuccess(queryListResponse);
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x06003F05 RID: 16133 RVA: 0x00129F94 File Offset: 0x00128194
			public IAPurchaseCallback getBalanceHandler()
			{
				return new IAPurchaseCallback(this.BalanceHandler);
			}

			// Token: 0x06003F06 RID: 16134 RVA: 0x00129FA4 File Offset: 0x001281A4
			protected override void BalanceHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[BalanceHandler] code=" + code.ToString() + ",message= " + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				string text = "";
				string text2 = "";
				string text3 = "";
				if (code == 0)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text3 = (string)jsonData["message"];
					}
					catch (Exception ex)
					{
						string text4 = "[BalanceHandler] statusCode, message ex=";
						Exception ex2 = ex;
						Logger.Log(text4 + ((ex2 != null) ? ex2.ToString() : null));
					}
					Logger.Log("[BalanceHandler] statusCode =" + num.ToString() + ",errMessage=" + text3);
					if (num == 0)
					{
						try
						{
							text = (string)jsonData["currencyName"];
							text2 = (string)jsonData["balance"];
						}
						catch (Exception ex3)
						{
							string text5 = "[BalanceHandler] currencyName, balance ex=";
							Exception ex4 = ex3;
							Logger.Log(text5 + ((ex4 != null) ? ex4.ToString() : null));
						}
						Logger.Log("[BalanceHandler] currencyName=" + text + ",balance=" + text2);
					}
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							IAPurchase.IAPHandler.listener.OnBalanceSuccess(text2);
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text3);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x06003F07 RID: 16135 RVA: 0x0012A100 File Offset: 0x00128300
			public IAPurchaseCallback getRequestSubscriptionHandler()
			{
				return new IAPurchaseCallback(this.RequestSubscriptionHandler);
			}

			// Token: 0x06003F08 RID: 16136 RVA: 0x0012A110 File Offset: 0x00128310
			protected override void RequestSubscriptionHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[RequestSubscriptionHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				string text = "";
				string text2 = "";
				try
				{
					num = (int)jsonData["statusCode"];
					text2 = (string)jsonData["message"];
				}
				catch (Exception ex)
				{
					string text3 = "[RequestSubscriptionHandler] statusCode, message ex=";
					Exception ex2 = ex;
					Logger.Log(text3 + ((ex2 != null) ? ex2.ToString() : null));
				}
				Logger.Log("[RequestSubscriptionHandler] statusCode =" + num.ToString() + ",errMessage=" + text2);
				if (num == 0)
				{
					try
					{
						text = (string)jsonData["subscription_id"];
					}
					catch (Exception ex3)
					{
						string text4 = "[RequestSubscriptionHandler] subscription_id ex=";
						Exception ex4 = ex3;
						Logger.Log(text4 + ((ex4 != null) ? ex4.ToString() : null));
					}
					Logger.Log("[RequestSubscriptionHandler] subscription_id =" + text);
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							IAPurchase.IAPHandler.listener.OnRequestSubscriptionSuccess(text);
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text2);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x06003F09 RID: 16137 RVA: 0x0012A238 File Offset: 0x00128438
			public IAPurchaseCallback getRequestSubscriptionWithPlanIDHandler()
			{
				return new IAPurchaseCallback(this.RequestSubscriptionWithPlanIDHandler);
			}

			// Token: 0x06003F0A RID: 16138 RVA: 0x0012A248 File Offset: 0x00128448
			protected override void RequestSubscriptionWithPlanIDHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[RequestSubscriptionWithPlanIDHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				string text = "";
				string text2 = "";
				try
				{
					num = (int)jsonData["statusCode"];
					text2 = (string)jsonData["message"];
				}
				catch (Exception ex)
				{
					string text3 = "[RequestSubscriptionWithPlanIDHandler] statusCode, message ex=";
					Exception ex2 = ex;
					Logger.Log(text3 + ((ex2 != null) ? ex2.ToString() : null));
				}
				Logger.Log("[RequestSubscriptionWithPlanIDHandler] statusCode =" + num.ToString() + ",errMessage=" + text2);
				if (num == 0)
				{
					try
					{
						text = (string)jsonData["subscription_id"];
					}
					catch (Exception ex3)
					{
						string text4 = "[RequestSubscriptionWithPlanIDHandler] subscription_id ex=";
						Exception ex4 = ex3;
						Logger.Log(text4 + ((ex4 != null) ? ex4.ToString() : null));
					}
					Logger.Log("[RequestSubscriptionWithPlanIDHandler] subscription_id =" + text);
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							IAPurchase.IAPHandler.listener.OnRequestSubscriptionWithPlanIDSuccess(text);
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text2);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x06003F0B RID: 16139 RVA: 0x0012A370 File Offset: 0x00128570
			public IAPurchaseCallback getSubscribeHandler()
			{
				return new IAPurchaseCallback(this.SubscribeHandler);
			}

			// Token: 0x06003F0C RID: 16140 RVA: 0x0012A380 File Offset: 0x00128580
			protected override void SubscribeHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[SubscribeHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				string text = "";
				string text2 = "";
				string text3 = "";
				long num2 = 0L;
				try
				{
					num = (int)jsonData["statusCode"];
					text2 = (string)jsonData["message"];
				}
				catch (Exception ex)
				{
					string text4 = "[SubscribeHandler] statusCode, message ex=";
					Exception ex2 = ex;
					Logger.Log(text4 + ((ex2 != null) ? ex2.ToString() : null));
				}
				Logger.Log("[SubscribeHandler] statusCode =" + num.ToString() + ",errMessage=" + text2);
				if (num == 0)
				{
					try
					{
						text = (string)jsonData["subscription_id"];
						text3 = (string)jsonData["plan_id"];
						num2 = (long)jsonData["subscribed_timestamp"];
					}
					catch (Exception ex3)
					{
						string text5 = "[SubscribeHandler] subscription_id, plan_id ex=";
						Exception ex4 = ex3;
						Logger.Log(text5 + ((ex4 != null) ? ex4.ToString() : null));
					}
					Logger.Log(string.Concat(new string[]
					{
						"[SubscribeHandler] subscription_id =",
						text,
						", plan_id=",
						text3,
						", timestamp=",
						num2.ToString()
					}));
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							IAPurchase.IAPHandler.listener.OnSubscribeSuccess(text);
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text2);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x06003F0D RID: 16141 RVA: 0x0012A508 File Offset: 0x00128708
			public IAPurchaseCallback getQuerySubscriptionHandler()
			{
				return new IAPurchaseCallback(this.QuerySubscriptionHandler);
			}

			// Token: 0x06003F0E RID: 16142 RVA: 0x0012A518 File Offset: 0x00128718
			protected override void QuerySubscriptionHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[QuerySubscriptionHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				string text = "";
				List<IAPurchase.Subscription> list = null;
				if (code == 0)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text = (string)jsonData["message"];
					}
					catch (Exception ex)
					{
						string text2 = "[QuerySubscriptionHandler] statusCode, message ex=";
						Exception ex2 = ex;
						Logger.Log(text2 + ((ex2 != null) ? ex2.ToString() : null));
					}
					Logger.Log("[QuerySubscriptionHandler] statusCode =" + num.ToString() + ",errMessage=" + text);
					if (num == 0)
					{
						try
						{
							list = JsonMapper.ToObject<IAPurchase.QuerySubscritionResponse>(message).subscriptions;
						}
						catch (Exception ex3)
						{
							string text3 = "[QuerySubscriptionHandler] ex =";
							Exception ex4 = ex3;
							Logger.Log(text3 + ((ex4 != null) ? ex4.ToString() : null));
						}
					}
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0 && list != null && list.Count > 0)
						{
							IAPurchase.IAPHandler.listener.OnQuerySubscriptionSuccess(list.ToArray());
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x06003F0F RID: 16143 RVA: 0x0012A640 File Offset: 0x00128840
			public IAPurchaseCallback getQuerySubscriptionListHandler()
			{
				return new IAPurchaseCallback(this.QuerySubscriptionListHandler);
			}

			// Token: 0x06003F10 RID: 16144 RVA: 0x0012A650 File Offset: 0x00128850
			protected override void QuerySubscriptionListHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[QuerySubscriptionListHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				string text = "";
				List<IAPurchase.Subscription> list = null;
				if (code == 0)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text = (string)jsonData["message"];
					}
					catch (Exception ex)
					{
						string text2 = "[QuerySubscriptionListHandler] statusCode, message ex=";
						Exception ex2 = ex;
						Logger.Log(text2 + ((ex2 != null) ? ex2.ToString() : null));
					}
					Logger.Log("[QuerySubscriptionListHandler] statusCode =" + num.ToString() + ",errMessage=" + text);
					if (num == 0)
					{
						try
						{
							list = JsonMapper.ToObject<IAPurchase.QuerySubscritionResponse>(message).subscriptions;
						}
						catch (Exception ex3)
						{
							string text3 = "[QuerySubscriptionListHandler] ex =";
							Exception ex4 = ex3;
							Logger.Log(text3 + ((ex4 != null) ? ex4.ToString() : null));
						}
					}
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0 && list != null && list.Count > 0)
						{
							IAPurchase.IAPHandler.listener.OnQuerySubscriptionListSuccess(list.ToArray());
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x06003F11 RID: 16145 RVA: 0x0012A778 File Offset: 0x00128978
			public IAPurchaseCallback getCancelSubscriptionHandler()
			{
				return new IAPurchaseCallback(this.CancelSubscriptionHandler);
			}

			// Token: 0x06003F12 RID: 16146 RVA: 0x0012A788 File Offset: 0x00128988
			protected override void CancelSubscriptionHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[CancelSubscriptionHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				bool flag = false;
				string text = "";
				if (code == 0)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text = (string)jsonData["message"];
					}
					catch (Exception ex)
					{
						string text2 = "[CancelSubscriptionHandler] statusCode, message ex=";
						Exception ex2 = ex;
						Logger.Log(text2 + ((ex2 != null) ? ex2.ToString() : null));
					}
					Logger.Log("[CancelSubscriptionHandler] statusCode =" + num.ToString() + ",errMessage=" + text);
					if (num == 0)
					{
						flag = true;
						Logger.Log("[CancelSubscriptionHandler] isCanceled = " + flag.ToString());
					}
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							IAPurchase.IAPHandler.listener.OnCancelSubscriptionSuccess(flag);
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x0400435B RID: 17243
			private static IAPurchase.IAPurchaseListener listener;
		}

		// Token: 0x02000A5C RID: 2652
		private abstract class BaseHandler
		{
			// Token: 0x06003F13 RID: 16147
			protected abstract void IsReadyHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x06003F14 RID: 16148
			protected abstract void RequestHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x06003F15 RID: 16149
			protected abstract void PurchaseHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x06003F16 RID: 16150
			protected abstract void QueryHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x06003F17 RID: 16151
			protected abstract void QueryListHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x06003F18 RID: 16152
			protected abstract void BalanceHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x06003F19 RID: 16153
			protected abstract void RequestSubscriptionHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x06003F1A RID: 16154
			protected abstract void RequestSubscriptionWithPlanIDHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x06003F1B RID: 16155
			protected abstract void SubscribeHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x06003F1C RID: 16156
			protected abstract void QuerySubscriptionHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x06003F1D RID: 16157
			protected abstract void QuerySubscriptionListHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x06003F1E RID: 16158
			protected abstract void CancelSubscriptionHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);
		}

		// Token: 0x02000A5D RID: 2653
		public class IAPurchaseListener
		{
			// Token: 0x06003F20 RID: 16160 RVA: 0x000023F4 File Offset: 0x000005F4
			public virtual void OnSuccess(string pchCurrencyName)
			{
			}

			// Token: 0x06003F21 RID: 16161 RVA: 0x000023F4 File Offset: 0x000005F4
			public virtual void OnRequestSuccess(string pchPurchaseId)
			{
			}

			// Token: 0x06003F22 RID: 16162 RVA: 0x000023F4 File Offset: 0x000005F4
			public virtual void OnPurchaseSuccess(string pchPurchaseId)
			{
			}

			// Token: 0x06003F23 RID: 16163 RVA: 0x000023F4 File Offset: 0x000005F4
			public virtual void OnQuerySuccess(IAPurchase.QueryResponse response)
			{
			}

			// Token: 0x06003F24 RID: 16164 RVA: 0x000023F4 File Offset: 0x000005F4
			public virtual void OnQuerySuccess(IAPurchase.QueryListResponse response)
			{
			}

			// Token: 0x06003F25 RID: 16165 RVA: 0x000023F4 File Offset: 0x000005F4
			public virtual void OnBalanceSuccess(string pchBalance)
			{
			}

			// Token: 0x06003F26 RID: 16166 RVA: 0x000023F4 File Offset: 0x000005F4
			public virtual void OnFailure(int nCode, string pchMessage)
			{
			}

			// Token: 0x06003F27 RID: 16167 RVA: 0x000023F4 File Offset: 0x000005F4
			public virtual void OnRequestSubscriptionSuccess(string pchSubscriptionId)
			{
			}

			// Token: 0x06003F28 RID: 16168 RVA: 0x000023F4 File Offset: 0x000005F4
			public virtual void OnRequestSubscriptionWithPlanIDSuccess(string pchSubscriptionId)
			{
			}

			// Token: 0x06003F29 RID: 16169 RVA: 0x000023F4 File Offset: 0x000005F4
			public virtual void OnSubscribeSuccess(string pchSubscriptionId)
			{
			}

			// Token: 0x06003F2A RID: 16170 RVA: 0x000023F4 File Offset: 0x000005F4
			public virtual void OnQuerySubscriptionSuccess(IAPurchase.Subscription[] subscriptionlist)
			{
			}

			// Token: 0x06003F2B RID: 16171 RVA: 0x000023F4 File Offset: 0x000005F4
			public virtual void OnQuerySubscriptionListSuccess(IAPurchase.Subscription[] subscriptionlist)
			{
			}

			// Token: 0x06003F2C RID: 16172 RVA: 0x000023F4 File Offset: 0x000005F4
			public virtual void OnCancelSubscriptionSuccess(bool bCanceled)
			{
			}
		}

		// Token: 0x02000A5E RID: 2654
		public class QueryResponse
		{
			// Token: 0x1700062F RID: 1583
			// (get) Token: 0x06003F2E RID: 16174 RVA: 0x0012A878 File Offset: 0x00128A78
			// (set) Token: 0x06003F2F RID: 16175 RVA: 0x0012A880 File Offset: 0x00128A80
			public string order_id { get; set; }

			// Token: 0x17000630 RID: 1584
			// (get) Token: 0x06003F30 RID: 16176 RVA: 0x0012A889 File Offset: 0x00128A89
			// (set) Token: 0x06003F31 RID: 16177 RVA: 0x0012A891 File Offset: 0x00128A91
			public string purchase_id { get; set; }

			// Token: 0x17000631 RID: 1585
			// (get) Token: 0x06003F32 RID: 16178 RVA: 0x0012A89A File Offset: 0x00128A9A
			// (set) Token: 0x06003F33 RID: 16179 RVA: 0x0012A8A2 File Offset: 0x00128AA2
			public string status { get; set; }

			// Token: 0x17000632 RID: 1586
			// (get) Token: 0x06003F34 RID: 16180 RVA: 0x0012A8AB File Offset: 0x00128AAB
			// (set) Token: 0x06003F35 RID: 16181 RVA: 0x0012A8B3 File Offset: 0x00128AB3
			public string price { get; set; }

			// Token: 0x17000633 RID: 1587
			// (get) Token: 0x06003F36 RID: 16182 RVA: 0x0012A8BC File Offset: 0x00128ABC
			// (set) Token: 0x06003F37 RID: 16183 RVA: 0x0012A8C4 File Offset: 0x00128AC4
			public string currency { get; set; }

			// Token: 0x17000634 RID: 1588
			// (get) Token: 0x06003F38 RID: 16184 RVA: 0x0012A8CD File Offset: 0x00128ACD
			// (set) Token: 0x06003F39 RID: 16185 RVA: 0x0012A8D5 File Offset: 0x00128AD5
			public long paid_timestamp { get; set; }
		}

		// Token: 0x02000A5F RID: 2655
		public class QueryResponse2
		{
			// Token: 0x17000635 RID: 1589
			// (get) Token: 0x06003F3B RID: 16187 RVA: 0x0012A8DE File Offset: 0x00128ADE
			// (set) Token: 0x06003F3C RID: 16188 RVA: 0x0012A8E6 File Offset: 0x00128AE6
			public string order_id { get; set; }

			// Token: 0x17000636 RID: 1590
			// (get) Token: 0x06003F3D RID: 16189 RVA: 0x0012A8EF File Offset: 0x00128AEF
			// (set) Token: 0x06003F3E RID: 16190 RVA: 0x0012A8F7 File Offset: 0x00128AF7
			public string app_id { get; set; }

			// Token: 0x17000637 RID: 1591
			// (get) Token: 0x06003F3F RID: 16191 RVA: 0x0012A900 File Offset: 0x00128B00
			// (set) Token: 0x06003F40 RID: 16192 RVA: 0x0012A908 File Offset: 0x00128B08
			public string purchase_id { get; set; }

			// Token: 0x17000638 RID: 1592
			// (get) Token: 0x06003F41 RID: 16193 RVA: 0x0012A911 File Offset: 0x00128B11
			// (set) Token: 0x06003F42 RID: 16194 RVA: 0x0012A919 File Offset: 0x00128B19
			public string user_data { get; set; }

			// Token: 0x17000639 RID: 1593
			// (get) Token: 0x06003F43 RID: 16195 RVA: 0x0012A922 File Offset: 0x00128B22
			// (set) Token: 0x06003F44 RID: 16196 RVA: 0x0012A92A File Offset: 0x00128B2A
			public string price { get; set; }

			// Token: 0x1700063A RID: 1594
			// (get) Token: 0x06003F45 RID: 16197 RVA: 0x0012A933 File Offset: 0x00128B33
			// (set) Token: 0x06003F46 RID: 16198 RVA: 0x0012A93B File Offset: 0x00128B3B
			public string currency { get; set; }

			// Token: 0x1700063B RID: 1595
			// (get) Token: 0x06003F47 RID: 16199 RVA: 0x0012A944 File Offset: 0x00128B44
			// (set) Token: 0x06003F48 RID: 16200 RVA: 0x0012A94C File Offset: 0x00128B4C
			public long paid_timestamp { get; set; }
		}

		// Token: 0x02000A60 RID: 2656
		public class QueryListResponse
		{
			// Token: 0x1700063C RID: 1596
			// (get) Token: 0x06003F4A RID: 16202 RVA: 0x0012A955 File Offset: 0x00128B55
			// (set) Token: 0x06003F4B RID: 16203 RVA: 0x0012A95D File Offset: 0x00128B5D
			public int total { get; set; }

			// Token: 0x1700063D RID: 1597
			// (get) Token: 0x06003F4C RID: 16204 RVA: 0x0012A966 File Offset: 0x00128B66
			// (set) Token: 0x06003F4D RID: 16205 RVA: 0x0012A96E File Offset: 0x00128B6E
			public int from { get; set; }

			// Token: 0x1700063E RID: 1598
			// (get) Token: 0x06003F4E RID: 16206 RVA: 0x0012A977 File Offset: 0x00128B77
			// (set) Token: 0x06003F4F RID: 16207 RVA: 0x0012A97F File Offset: 0x00128B7F
			public int to { get; set; }

			// Token: 0x0400436C RID: 17260
			public List<IAPurchase.QueryResponse2> purchaseList;
		}

		// Token: 0x02000A61 RID: 2657
		public class StatusDetailTransaction
		{
			// Token: 0x1700063F RID: 1599
			// (get) Token: 0x06003F51 RID: 16209 RVA: 0x0012A988 File Offset: 0x00128B88
			// (set) Token: 0x06003F52 RID: 16210 RVA: 0x0012A990 File Offset: 0x00128B90
			public long create_time { get; set; }

			// Token: 0x17000640 RID: 1600
			// (get) Token: 0x06003F53 RID: 16211 RVA: 0x0012A999 File Offset: 0x00128B99
			// (set) Token: 0x06003F54 RID: 16212 RVA: 0x0012A9A1 File Offset: 0x00128BA1
			public string payment_method { get; set; }

			// Token: 0x17000641 RID: 1601
			// (get) Token: 0x06003F55 RID: 16213 RVA: 0x0012A9AA File Offset: 0x00128BAA
			// (set) Token: 0x06003F56 RID: 16214 RVA: 0x0012A9B2 File Offset: 0x00128BB2
			public string status { get; set; }
		}

		// Token: 0x02000A62 RID: 2658
		public class StatusDetail
		{
			// Token: 0x17000642 RID: 1602
			// (get) Token: 0x06003F58 RID: 16216 RVA: 0x0012A9BB File Offset: 0x00128BBB
			// (set) Token: 0x06003F59 RID: 16217 RVA: 0x0012A9C3 File Offset: 0x00128BC3
			public long date_next_charge { get; set; }

			// Token: 0x17000643 RID: 1603
			// (get) Token: 0x06003F5A RID: 16218 RVA: 0x0012A9CC File Offset: 0x00128BCC
			// (set) Token: 0x06003F5B RID: 16219 RVA: 0x0012A9D4 File Offset: 0x00128BD4
			public IAPurchase.StatusDetailTransaction[] transactions { get; set; }

			// Token: 0x17000644 RID: 1604
			// (get) Token: 0x06003F5C RID: 16220 RVA: 0x0012A9DD File Offset: 0x00128BDD
			// (set) Token: 0x06003F5D RID: 16221 RVA: 0x0012A9E5 File Offset: 0x00128BE5
			public string cancel_reason { get; set; }
		}

		// Token: 0x02000A63 RID: 2659
		public class TimePeriod
		{
			// Token: 0x17000645 RID: 1605
			// (get) Token: 0x06003F5F RID: 16223 RVA: 0x0012A9EE File Offset: 0x00128BEE
			// (set) Token: 0x06003F60 RID: 16224 RVA: 0x0012A9F6 File Offset: 0x00128BF6
			public string time_type { get; set; }

			// Token: 0x17000646 RID: 1606
			// (get) Token: 0x06003F61 RID: 16225 RVA: 0x0012A9FF File Offset: 0x00128BFF
			// (set) Token: 0x06003F62 RID: 16226 RVA: 0x0012AA07 File Offset: 0x00128C07
			public int value { get; set; }
		}

		// Token: 0x02000A64 RID: 2660
		public class Subscription
		{
			// Token: 0x17000647 RID: 1607
			// (get) Token: 0x06003F64 RID: 16228 RVA: 0x0012AA10 File Offset: 0x00128C10
			// (set) Token: 0x06003F65 RID: 16229 RVA: 0x0012AA18 File Offset: 0x00128C18
			public string app_id { get; set; }

			// Token: 0x17000648 RID: 1608
			// (get) Token: 0x06003F66 RID: 16230 RVA: 0x0012AA21 File Offset: 0x00128C21
			// (set) Token: 0x06003F67 RID: 16231 RVA: 0x0012AA29 File Offset: 0x00128C29
			public string order_id { get; set; }

			// Token: 0x17000649 RID: 1609
			// (get) Token: 0x06003F68 RID: 16232 RVA: 0x0012AA32 File Offset: 0x00128C32
			// (set) Token: 0x06003F69 RID: 16233 RVA: 0x0012AA3A File Offset: 0x00128C3A
			public string subscription_id { get; set; }

			// Token: 0x1700064A RID: 1610
			// (get) Token: 0x06003F6A RID: 16234 RVA: 0x0012AA43 File Offset: 0x00128C43
			// (set) Token: 0x06003F6B RID: 16235 RVA: 0x0012AA4B File Offset: 0x00128C4B
			public string price { get; set; }

			// Token: 0x1700064B RID: 1611
			// (get) Token: 0x06003F6C RID: 16236 RVA: 0x0012AA54 File Offset: 0x00128C54
			// (set) Token: 0x06003F6D RID: 16237 RVA: 0x0012AA5C File Offset: 0x00128C5C
			public string currency { get; set; }

			// Token: 0x1700064C RID: 1612
			// (get) Token: 0x06003F6E RID: 16238 RVA: 0x0012AA65 File Offset: 0x00128C65
			// (set) Token: 0x06003F6F RID: 16239 RVA: 0x0012AA6D File Offset: 0x00128C6D
			public long subscribed_timestamp { get; set; }

			// Token: 0x1700064D RID: 1613
			// (get) Token: 0x06003F70 RID: 16240 RVA: 0x0012AA76 File Offset: 0x00128C76
			// (set) Token: 0x06003F71 RID: 16241 RVA: 0x0012AA7E File Offset: 0x00128C7E
			public IAPurchase.TimePeriod free_trial_period { get; set; }

			// Token: 0x1700064E RID: 1614
			// (get) Token: 0x06003F72 RID: 16242 RVA: 0x0012AA87 File Offset: 0x00128C87
			// (set) Token: 0x06003F73 RID: 16243 RVA: 0x0012AA8F File Offset: 0x00128C8F
			public IAPurchase.TimePeriod charge_period { get; set; }

			// Token: 0x1700064F RID: 1615
			// (get) Token: 0x06003F74 RID: 16244 RVA: 0x0012AA98 File Offset: 0x00128C98
			// (set) Token: 0x06003F75 RID: 16245 RVA: 0x0012AAA0 File Offset: 0x00128CA0
			public int number_of_charge_period { get; set; }

			// Token: 0x17000650 RID: 1616
			// (get) Token: 0x06003F76 RID: 16246 RVA: 0x0012AAA9 File Offset: 0x00128CA9
			// (set) Token: 0x06003F77 RID: 16247 RVA: 0x0012AAB1 File Offset: 0x00128CB1
			public string plan_id { get; set; }

			// Token: 0x17000651 RID: 1617
			// (get) Token: 0x06003F78 RID: 16248 RVA: 0x0012AABA File Offset: 0x00128CBA
			// (set) Token: 0x06003F79 RID: 16249 RVA: 0x0012AAC2 File Offset: 0x00128CC2
			public string plan_name { get; set; }

			// Token: 0x17000652 RID: 1618
			// (get) Token: 0x06003F7A RID: 16250 RVA: 0x0012AACB File Offset: 0x00128CCB
			// (set) Token: 0x06003F7B RID: 16251 RVA: 0x0012AAD3 File Offset: 0x00128CD3
			public string status { get; set; }

			// Token: 0x17000653 RID: 1619
			// (get) Token: 0x06003F7C RID: 16252 RVA: 0x0012AADC File Offset: 0x00128CDC
			// (set) Token: 0x06003F7D RID: 16253 RVA: 0x0012AAE4 File Offset: 0x00128CE4
			public IAPurchase.StatusDetail status_detail { get; set; }
		}

		// Token: 0x02000A65 RID: 2661
		public class QuerySubscritionResponse
		{
			// Token: 0x17000654 RID: 1620
			// (get) Token: 0x06003F7F RID: 16255 RVA: 0x0012AAED File Offset: 0x00128CED
			// (set) Token: 0x06003F80 RID: 16256 RVA: 0x0012AAF5 File Offset: 0x00128CF5
			public int statusCode { get; set; }

			// Token: 0x17000655 RID: 1621
			// (get) Token: 0x06003F81 RID: 16257 RVA: 0x0012AAFE File Offset: 0x00128CFE
			// (set) Token: 0x06003F82 RID: 16258 RVA: 0x0012AB06 File Offset: 0x00128D06
			public string message { get; set; }

			// Token: 0x17000656 RID: 1622
			// (get) Token: 0x06003F83 RID: 16259 RVA: 0x0012AB0F File Offset: 0x00128D0F
			// (set) Token: 0x06003F84 RID: 16260 RVA: 0x0012AB17 File Offset: 0x00128D17
			public List<IAPurchase.Subscription> subscriptions { get; set; }
		}
	}
}
