using System;
using GorillaNetworking;
using KID.Model;
using UnityEngine;

// Token: 0x020007F4 RID: 2036
internal class UGCPermissionManager : MonoBehaviour
{
	// Token: 0x060031F5 RID: 12789 RVA: 0x000F7222 File Offset: 0x000F5422
	public static void UsePlayFabSafety()
	{
		UGCPermissionManager.permissions = new UGCPermissionManager.PlayFabPermissions(new Action<bool>(UGCPermissionManager.SetUGCEnabled));
		UGCPermissionManager.permissions.Initialize();
	}

	// Token: 0x060031F6 RID: 12790 RVA: 0x000F7244 File Offset: 0x000F5444
	public static void UseKID()
	{
		UGCPermissionManager.permissions = new UGCPermissionManager.KIDPermissions(new Action<bool>(UGCPermissionManager.SetUGCEnabled));
		UGCPermissionManager.permissions.Initialize();
	}

	// Token: 0x17000514 RID: 1300
	// (get) Token: 0x060031F7 RID: 12791 RVA: 0x000F7266 File Offset: 0x000F5466
	public static bool IsUGCDisabled
	{
		get
		{
			return !UGCPermissionManager.isUGCEnabled.GetValueOrDefault();
		}
	}

	// Token: 0x060031F8 RID: 12792 RVA: 0x000F7275 File Offset: 0x000F5475
	public static void CheckPermissions()
	{
		UGCPermissionManager.IUGCPermissions iugcpermissions = UGCPermissionManager.permissions;
		if (iugcpermissions == null)
		{
			return;
		}
		iugcpermissions.CheckPermissions();
	}

	// Token: 0x060031F9 RID: 12793 RVA: 0x000F7286 File Offset: 0x000F5486
	public static void SubscribeToUGCEnabled(Action callback)
	{
		UGCPermissionManager.onUGCEnabled = (Action)Delegate.Combine(UGCPermissionManager.onUGCEnabled, callback);
	}

	// Token: 0x060031FA RID: 12794 RVA: 0x000F729D File Offset: 0x000F549D
	public static void UnsubscribeFromUGCEnabled(Action callback)
	{
		UGCPermissionManager.onUGCEnabled = (Action)Delegate.Remove(UGCPermissionManager.onUGCEnabled, callback);
	}

	// Token: 0x060031FB RID: 12795 RVA: 0x000F72B4 File Offset: 0x000F54B4
	public static void SubscribeToUGCDisabled(Action callback)
	{
		UGCPermissionManager.onUGCDisabled = (Action)Delegate.Combine(UGCPermissionManager.onUGCDisabled, callback);
	}

	// Token: 0x060031FC RID: 12796 RVA: 0x000F72CB File Offset: 0x000F54CB
	public static void UnsubscribeFromUGCDisabled(Action callback)
	{
		UGCPermissionManager.onUGCDisabled = (Action)Delegate.Remove(UGCPermissionManager.onUGCDisabled, callback);
	}

	// Token: 0x060031FD RID: 12797 RVA: 0x000F72E4 File Offset: 0x000F54E4
	private static void SetUGCEnabled(bool enabled)
	{
		bool? flag = UGCPermissionManager.isUGCEnabled;
		if (!((enabled == flag.GetValueOrDefault()) & (flag != null)))
		{
			Debug.LogFormat("[UGCPermissionManager][KID] UGC state changed: [{0}]", new object[] { enabled ? "ENABLED" : "DISABLED" });
			UGCPermissionManager.isUGCEnabled = new bool?(enabled);
			if (enabled)
			{
				Debug.Log("[UGCPermissionManager][KID] Invoking onUGCEnabled");
				Action action = UGCPermissionManager.onUGCEnabled;
				if (action == null)
				{
					return;
				}
				action();
				return;
			}
			else
			{
				Debug.Log("[UGCPermissionManager][KID] Invoking onUGCDisabled");
				Action action2 = UGCPermissionManager.onUGCDisabled;
				if (action2 == null)
				{
					return;
				}
				action2();
			}
		}
	}

	// Token: 0x040038B8 RID: 14520
	[OnEnterPlay_SetNull]
	private static UGCPermissionManager.IUGCPermissions permissions;

	// Token: 0x040038B9 RID: 14521
	[OnEnterPlay_SetNull]
	private static Action onUGCEnabled;

	// Token: 0x040038BA RID: 14522
	[OnEnterPlay_SetNull]
	private static Action onUGCDisabled;

	// Token: 0x040038BB RID: 14523
	private static bool? isUGCEnabled;

	// Token: 0x020007F5 RID: 2037
	private interface IUGCPermissions
	{
		// Token: 0x060031FF RID: 12799
		void Initialize();

		// Token: 0x06003200 RID: 12800
		void CheckPermissions();
	}

	// Token: 0x020007F6 RID: 2038
	private class PlayFabPermissions : UGCPermissionManager.IUGCPermissions
	{
		// Token: 0x06003201 RID: 12801 RVA: 0x000F736E File Offset: 0x000F556E
		public PlayFabPermissions(Action<bool> setUGCEnabled)
		{
			this.setUGCEnabled = setUGCEnabled;
		}

		// Token: 0x06003202 RID: 12802 RVA: 0x000F7380 File Offset: 0x000F5580
		public void Initialize()
		{
			bool safety = PlayFabAuthenticator.instance.GetSafety();
			Debug.LogFormat("[UGCPermissionManager][KID] UGC initialized from Playfab: [{0}]", new object[] { safety ? "DISABLED" : "ENABLED" });
			Action<bool> action = this.setUGCEnabled;
			if (action == null)
			{
				return;
			}
			action(!safety);
		}

		// Token: 0x06003203 RID: 12803 RVA: 0x000023F4 File Offset: 0x000005F4
		public void CheckPermissions()
		{
		}

		// Token: 0x040038BC RID: 14524
		private Action<bool> setUGCEnabled;
	}

	// Token: 0x020007F7 RID: 2039
	private class KIDPermissions : UGCPermissionManager.IUGCPermissions
	{
		// Token: 0x06003204 RID: 12804 RVA: 0x000F73D0 File Offset: 0x000F55D0
		public KIDPermissions(Action<bool> setUGCEnabled)
		{
			this.setUGCEnabled = setUGCEnabled;
		}

		// Token: 0x06003205 RID: 12805 RVA: 0x000F73DF File Offset: 0x000F55DF
		private void SetUGCEnabled(bool enabled)
		{
			Action<bool> action = this.setUGCEnabled;
			if (action == null)
			{
				return;
			}
			action(enabled);
		}

		// Token: 0x06003206 RID: 12806 RVA: 0x000F73F2 File Offset: 0x000F55F2
		public void Initialize()
		{
			Debug.Log("[UGCPermissionManager][KID] Initializing with KID");
			this.CheckPermissions();
			KIDManager.RegisterSessionUpdatedCallback_UGC(new Action<bool, Permission.ManagedByEnum>(this.OnKIDSessionUpdate));
		}

		// Token: 0x06003207 RID: 12807 RVA: 0x000F7415 File Offset: 0x000F5615
		public bool MeetsModIOMinimumAge()
		{
			if (KIDManager.CurrentSession == null)
			{
				Debug.LogError("[UGCPermissionManager][KID] No KID session. Assuming player does not meet ModIO minimum age.");
				return false;
			}
			return KIDManager.CurrentSession.Age >= 13;
		}

		// Token: 0x06003208 RID: 12808 RVA: 0x000F743C File Offset: 0x000F563C
		public void CheckPermissions()
		{
			Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Mods);
			bool item = KIDManager.CheckFeatureOptIn(EKIDFeatures.Mods, null).Item2;
			this.ProcessPermissionKID(item, permissionDataByFeature.Enabled, permissionDataByFeature.ManagedBy);
		}

		// Token: 0x06003209 RID: 12809 RVA: 0x000F7470 File Offset: 0x000F5670
		private void OnKIDSessionUpdate(bool isEnabled, Permission.ManagedByEnum managedBy)
		{
			Debug.Log("[UGCPermissionManager][KID] KID session update.");
			bool item = KIDManager.CheckFeatureOptIn(EKIDFeatures.Mods, null).Item2;
			this.ProcessPermissionKID(item, isEnabled, managedBy);
		}

		// Token: 0x0600320A RID: 12810 RVA: 0x000F74A0 File Offset: 0x000F56A0
		private void ProcessPermissionKID(bool hasOptedIn, bool isEnabled, Permission.ManagedByEnum managedBy)
		{
			Debug.LogFormat("[UGCPermissionManager][KID] Process KID permissions - opted in: [{0}], enabled: [{1}], managedBy: [{2}].", new object[] { hasOptedIn, isEnabled, managedBy });
			if (!this.MeetsModIOMinimumAge())
			{
				Debug.Log("[UGCPermissionManager][KID] KID player age is lower than the ModIO minimum age. Disabling UGC.");
				this.SetUGCEnabled(false);
				return;
			}
			if (managedBy == Permission.ManagedByEnum.PROHIBITED)
			{
				Debug.Log("[UGCPermissionManager][KID] KID UGC prohibited.");
				this.SetUGCEnabled(false);
				return;
			}
			if (managedBy != Permission.ManagedByEnum.PLAYER)
			{
				if (managedBy == Permission.ManagedByEnum.GUARDIAN)
				{
					Debug.LogFormat("[UGCPermissionManager][KID] KID UGC managed by guardian. (opted in: [{0}], enabled: [{1}])", new object[] { hasOptedIn, isEnabled });
					this.SetUGCEnabled(isEnabled);
				}
				return;
			}
			if (isEnabled)
			{
				Debug.Log("[UGCPermissionManager][KID] KID UGC managed by player and enabled - opting in and enabling UGC.");
				if (!hasOptedIn)
				{
					KIDManager.SetFeatureOptIn(EKIDFeatures.Mods, true);
				}
				this.SetUGCEnabled(true);
				return;
			}
			Debug.LogFormat("[UGCPermissionManager][KID] KID UGC managed by player and disabled by default - using opt in status. (opted in: [{0}])", new object[] { hasOptedIn });
			this.SetUGCEnabled(hasOptedIn);
		}

		// Token: 0x040038BD RID: 14525
		private Action<bool> setUGCEnabled;
	}
}
