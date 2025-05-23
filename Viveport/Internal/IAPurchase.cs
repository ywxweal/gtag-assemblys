using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal
{
	// Token: 0x02000A7D RID: 2685
	internal class IAPurchase
	{
		// Token: 0x06004002 RID: 16386
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportIAPurchase_IsReady")]
		public static extern void IsReady(IAPurchaseCallback callback, string pchAppKey);

		// Token: 0x06004003 RID: 16387
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportIAPurchase_IsReady")]
		public static extern void IsReady_64(IAPurchaseCallback callback, string pchAppKey);

		// Token: 0x06004004 RID: 16388
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportIAPurchase_Request")]
		public static extern void Request(IAPurchaseCallback callback, string pchPrice);

		// Token: 0x06004005 RID: 16389
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportIAPurchase_Request")]
		public static extern void Request_64(IAPurchaseCallback callback, string pchPrice);

		// Token: 0x06004006 RID: 16390
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportIAPurchase_RequestWithUserData")]
		public static extern void Request(IAPurchaseCallback callback, string pchPrice, string pchUserData);

		// Token: 0x06004007 RID: 16391
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportIAPurchase_RequestWithUserData")]
		public static extern void Request_64(IAPurchaseCallback callback, string pchPrice, string pchUserData);

		// Token: 0x06004008 RID: 16392
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportIAPurchase_Purchase")]
		public static extern void Purchase(IAPurchaseCallback callback, string pchPurchaseId);

		// Token: 0x06004009 RID: 16393
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportIAPurchase_Purchase")]
		public static extern void Purchase_64(IAPurchaseCallback callback, string pchPurchaseId);

		// Token: 0x0600400A RID: 16394
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportIAPurchase_Query")]
		public static extern void Query(IAPurchaseCallback callback, string pchPurchaseId);

		// Token: 0x0600400B RID: 16395
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportIAPurchase_Query")]
		public static extern void Query_64(IAPurchaseCallback callback, string pchPurchaseId);

		// Token: 0x0600400C RID: 16396
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportIAPurchase_QueryList")]
		public static extern void Query(IAPurchaseCallback callback);

		// Token: 0x0600400D RID: 16397
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportIAPurchase_QueryList")]
		public static extern void Query_64(IAPurchaseCallback callback);

		// Token: 0x0600400E RID: 16398
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, EntryPoint = "IViveportIAPurchase_GetBalance")]
		public static extern void GetBalance(IAPurchaseCallback callback);

		// Token: 0x0600400F RID: 16399
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "IViveportIAPurchase_GetBalance")]
		public static extern void GetBalance_64(IAPurchaseCallback callback);

		// Token: 0x06004010 RID: 16400
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportIAPurchase_RequestSubscription")]
		public static extern void RequestSubscription(IAPurchaseCallback callback, string pchPrice, string pchFreeTrialType, int nFreeTrialValue, string pchChargePeriodType, int nChargePeriodValue, int nNumberOfChargePeriod, string pchPlanId);

		// Token: 0x06004011 RID: 16401
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportIAPurchase_RequestSubscription")]
		public static extern void RequestSubscription_64(IAPurchaseCallback callback, string pchPrice, string pchFreeTrialType, int nFreeTrialValue, string pchChargePeriodType, int nChargePeriodValue, int nNumberOfChargePeriod, string pchPlanId);

		// Token: 0x06004012 RID: 16402
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportIAPurchase_RequestSubscriptionWithPlanID")]
		public static extern void RequestSubscriptionWithPlanID(IAPurchaseCallback callback, string pchPlanId);

		// Token: 0x06004013 RID: 16403
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportIAPurchase_RequestSubscriptionWithPlanID")]
		public static extern void RequestSubscriptionWithPlanID_64(IAPurchaseCallback callback, string pchPlanId);

		// Token: 0x06004014 RID: 16404
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportIAPurchase_Subscribe")]
		public static extern void Subscribe(IAPurchaseCallback callback, string pchSubscriptionId);

		// Token: 0x06004015 RID: 16405
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportIAPurchase_Subscribe")]
		public static extern void Subscribe_64(IAPurchaseCallback callback, string pchSubscriptionId);

		// Token: 0x06004016 RID: 16406
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportIAPurchase_QuerySubscription")]
		public static extern void QuerySubscription(IAPurchaseCallback callback, string pchSubscriptionId);

		// Token: 0x06004017 RID: 16407
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportIAPurchase_QuerySubscription")]
		public static extern void QuerySubscription_64(IAPurchaseCallback callback, string pchSubscriptionId);

		// Token: 0x06004018 RID: 16408
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportIAPurchase_QuerySubscriptionList")]
		public static extern void QuerySubscriptionList(IAPurchaseCallback callback);

		// Token: 0x06004019 RID: 16409
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportIAPurchase_QuerySubscriptionList")]
		public static extern void QuerySubscriptionList_64(IAPurchaseCallback callback);

		// Token: 0x0600401A RID: 16410
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportIAPurchase_CancelSubscription")]
		public static extern void CancelSubscription(IAPurchaseCallback callback, string pchSubscriptionId);

		// Token: 0x0600401B RID: 16411
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportIAPurchase_CancelSubscription")]
		public static extern void CancelSubscription_64(IAPurchaseCallback callback, string pchSubscriptionId);
	}
}
