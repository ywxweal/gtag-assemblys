using System;
using System.Collections.Generic;
using GorillaTagScripts.ModIO;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.CustomMapSupport
{
	// Token: 0x02000B33 RID: 2867
	internal class CMSSerializer : GorillaSerializer
	{
		// Token: 0x0600468D RID: 18061 RVA: 0x0014F6A9 File Offset: 0x0014D8A9
		public void Awake()
		{
			if (CMSSerializer.instance != null)
			{
				Object.Destroy(this);
			}
			CMSSerializer.instance = this;
			CMSSerializer.hasInstance = true;
		}

		// Token: 0x0600468E RID: 18062 RVA: 0x0014F6CE File Offset: 0x0014D8CE
		public void OnEnable()
		{
			CustomMapManager.OnMapLoadComplete.RemoveListener(new UnityAction<bool>(this.OnCustomMapLoaded));
			CustomMapManager.OnMapLoadComplete.AddListener(new UnityAction<bool>(this.OnCustomMapLoaded));
		}

		// Token: 0x0600468F RID: 18063 RVA: 0x0014F6FC File Offset: 0x0014D8FC
		public void OnDisable()
		{
			CustomMapManager.OnMapLoadComplete.RemoveListener(new UnityAction<bool>(this.OnCustomMapLoaded));
		}

		// Token: 0x06004690 RID: 18064 RVA: 0x0014F714 File Offset: 0x0014D914
		private void OnCustomMapLoaded(bool success)
		{
			if (success)
			{
				CMSSerializer.RequestSyncTriggerHistory();
			}
		}

		// Token: 0x06004691 RID: 18065 RVA: 0x0014F71E File Offset: 0x0014D91E
		public static void ResetSyncedMapObjects()
		{
			CMSSerializer.triggerHistory.Clear();
			CMSSerializer.triggerCounts.Clear();
			CMSSerializer.registeredTriggersPerScene.Clear();
			CMSSerializer.waitingForTriggerHistory = false;
			CMSSerializer.waitingForTriggerCounts = false;
		}

		// Token: 0x06004692 RID: 18066 RVA: 0x0014F74C File Offset: 0x0014D94C
		public static void RegisterTrigger(string sceneName, CMSTrigger trigger)
		{
			Dictionary<byte, CMSTrigger> dictionary;
			if (CMSSerializer.registeredTriggersPerScene.TryGetValue(sceneName, out dictionary))
			{
				if (!dictionary.ContainsKey(trigger.GetID()))
				{
					dictionary.Add(trigger.GetID(), trigger);
					return;
				}
			}
			else
			{
				CMSSerializer.registeredTriggersPerScene.Add(sceneName, new Dictionary<byte, CMSTrigger> { 
				{
					trigger.GetID(),
					trigger
				} });
			}
		}

		// Token: 0x06004693 RID: 18067 RVA: 0x0014F7A4 File Offset: 0x0014D9A4
		private static bool TryGetRegisteredTrigger(byte triggerID, out CMSTrigger trigger)
		{
			trigger = null;
			foreach (KeyValuePair<string, Dictionary<byte, CMSTrigger>> keyValuePair in CMSSerializer.registeredTriggersPerScene)
			{
				if (keyValuePair.Value.TryGetValue(triggerID, out trigger))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06004694 RID: 18068 RVA: 0x0014F80C File Offset: 0x0014DA0C
		public static void UnregisterTriggers(string forScene)
		{
			CMSSerializer.registeredTriggersPerScene.Remove(forScene);
		}

		// Token: 0x06004695 RID: 18069 RVA: 0x0014F81A File Offset: 0x0014DA1A
		public static void ResetTrigger(byte triggerID)
		{
			CMSSerializer.triggerCounts.Remove(triggerID);
		}

		// Token: 0x06004696 RID: 18070 RVA: 0x0014F828 File Offset: 0x0014DA28
		private static void RequestSyncTriggerHistory()
		{
			if (!CMSSerializer.hasInstance || !NetworkSystem.Instance.InRoom || NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			CMSSerializer.waitingForTriggerHistory = true;
			CMSSerializer.waitingForTriggerCounts = true;
			CMSSerializer.instance.SendRPC("RequestSyncTriggerHistory_RPC", false, Array.Empty<object>());
		}

		// Token: 0x06004697 RID: 18071 RVA: 0x0014F878 File Offset: 0x0014DA78
		[PunRPC]
		private void RequestSyncTriggerHistory_RPC(PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "RequestSyncTriggerHistory_RPC");
			if (!NetworkSystem.Instance.InRoom || !NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
			if (player.CheckSingleCallRPC(NetPlayer.SingleCallRPC.CMS_RequestTriggerHistory))
			{
				return;
			}
			player.ReceivedSingleCallRPC(NetPlayer.SingleCallRPC.CMS_RequestTriggerHistory);
			byte[] array = CMSSerializer.triggerHistory.ToArray();
			base.SendRPC("SyncTriggerHistory_RPC", info.Sender, new object[] { array });
			base.SendRPC("SyncTriggerCounts_RPC", info.Sender, new object[] { CMSSerializer.triggerCounts });
		}

		// Token: 0x06004698 RID: 18072 RVA: 0x0014F910 File Offset: 0x0014DB10
		[PunRPC]
		private void SyncTriggerHistory_RPC(byte[] syncedTriggerHistory, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "SyncTriggerHistory_RPC");
			if (!NetworkSystem.Instance.InRoom || !info.Sender.IsMasterClient)
			{
				return;
			}
			NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
			if (player.CheckSingleCallRPC(NetPlayer.SingleCallRPC.CMS_SyncTriggerHistory))
			{
				return;
			}
			player.ReceivedSingleCallRPC(NetPlayer.SingleCallRPC.CMS_SyncTriggerHistory);
			if (!CMSSerializer.waitingForTriggerHistory)
			{
				return;
			}
			CMSSerializer.triggerHistory.Clear();
			if (!syncedTriggerHistory.IsNullOrEmpty<byte>())
			{
				CMSSerializer.triggerHistory.AddRange(syncedTriggerHistory);
			}
			CMSSerializer.waitingForTriggerHistory = false;
			foreach (string text in CMSSerializer.scenesWaitingForTriggerHistory)
			{
				CMSSerializer.ProcessTriggerHistory(text);
			}
			CMSSerializer.scenesWaitingForTriggerHistory.Clear();
		}

		// Token: 0x06004699 RID: 18073 RVA: 0x0014F9DC File Offset: 0x0014DBDC
		[PunRPC]
		private void SyncTriggerCounts_RPC(Dictionary<byte, byte> syncedTriggerCounts, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "SyncTriggerCounts_RPC");
			if (!NetworkSystem.Instance.InRoom || !info.Sender.IsMasterClient)
			{
				return;
			}
			NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
			if (player.CheckSingleCallRPC(NetPlayer.SingleCallRPC.CMS_SyncTriggerCounts))
			{
				return;
			}
			player.ReceivedSingleCallRPC(NetPlayer.SingleCallRPC.CMS_SyncTriggerCounts);
			if (!CMSSerializer.waitingForTriggerCounts)
			{
				return;
			}
			CMSSerializer.triggerCounts.Clear();
			if (syncedTriggerCounts != null && syncedTriggerCounts.Count > 0)
			{
				CMSSerializer.triggerCounts = syncedTriggerCounts;
			}
			CMSSerializer.waitingForTriggerCounts = false;
			foreach (string text in CMSSerializer.scenesWaitingForTriggerCounts)
			{
				CMSSerializer.ProcessTriggerCounts(text);
			}
			CMSSerializer.scenesWaitingForTriggerCounts.Clear();
		}

		// Token: 0x0600469A RID: 18074 RVA: 0x0014FAA8 File Offset: 0x0014DCA8
		public static void ProcessSceneLoad(string sceneName)
		{
			if (CMSSerializer.waitingForTriggerHistory)
			{
				CMSSerializer.scenesWaitingForTriggerHistory.Add(sceneName);
			}
			else
			{
				CMSSerializer.ProcessTriggerHistory(sceneName);
			}
			if (CMSSerializer.waitingForTriggerCounts)
			{
				CMSSerializer.scenesWaitingForTriggerCounts.Add(sceneName);
				return;
			}
			CMSSerializer.ProcessTriggerCounts(sceneName);
		}

		// Token: 0x0600469B RID: 18075 RVA: 0x0014FAE0 File Offset: 0x0014DCE0
		private static void ProcessTriggerHistory(string forScene)
		{
			Dictionary<byte, CMSTrigger> dictionary;
			if (CMSSerializer.registeredTriggersPerScene.TryGetValue(forScene, out dictionary))
			{
				foreach (byte b in CMSSerializer.triggerHistory)
				{
					CMSTrigger cmstrigger;
					if (dictionary.TryGetValue(b, out cmstrigger))
					{
						cmstrigger.Trigger(1.0, false, true);
					}
				}
			}
			UnityEvent<string> onTriggerHistoryProcessedForScene = CMSSerializer.OnTriggerHistoryProcessedForScene;
			if (onTriggerHistoryProcessedForScene == null)
			{
				return;
			}
			onTriggerHistoryProcessedForScene.Invoke(forScene);
		}

		// Token: 0x0600469C RID: 18076 RVA: 0x0014FB68 File Offset: 0x0014DD68
		private static void ProcessTriggerCounts(string forScene)
		{
			Dictionary<byte, CMSTrigger> dictionary;
			if (CMSSerializer.registeredTriggersPerScene.TryGetValue(forScene, out dictionary))
			{
				List<byte> list = new List<byte>();
				foreach (KeyValuePair<byte, byte> keyValuePair in CMSSerializer.triggerCounts)
				{
					CMSTrigger cmstrigger;
					if (dictionary.TryGetValue(keyValuePair.Key, out cmstrigger))
					{
						if (cmstrigger.numAllowedTriggers > 0)
						{
							cmstrigger.SetTriggerCount(keyValuePair.Value);
						}
						else
						{
							list.Add(keyValuePair.Key);
						}
					}
				}
				foreach (byte b in list)
				{
					CMSSerializer.triggerCounts.Remove(b);
				}
			}
		}

		// Token: 0x0600469D RID: 18077 RVA: 0x0014FC48 File Offset: 0x0014DE48
		public static void RequestTrigger(byte triggerID)
		{
			if (!CMSSerializer.hasInstance)
			{
				return;
			}
			if (!NetworkSystem.Instance.InRoom || NetworkSystem.Instance.IsMasterClient)
			{
				double num = (double)Time.time;
				if (NetworkSystem.Instance.InRoom)
				{
					num = PhotonNetwork.Time;
					CMSSerializer.instance.SendRPC("ActivateTrigger_RPC", true, new object[]
					{
						triggerID,
						NetworkSystem.Instance.LocalPlayer.ActorNumber
					});
				}
				CMSSerializer.instance.ActivateTrigger(triggerID, num, true);
				return;
			}
			CMSSerializer.instance.SendRPC("RequestTrigger_RPC", false, new object[] { triggerID });
		}

		// Token: 0x0600469E RID: 18078 RVA: 0x0014FCF8 File Offset: 0x0014DEF8
		[PunRPC]
		private void RequestTrigger_RPC(byte triggerID, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "RequestTrigger_RPC");
			if (!NetworkSystem.Instance.InRoom || !NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
			RigContainer rigContainer;
			if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer) || !rigContainer.Rig.fxSettings.callSettings[11].CallLimitSettings.CheckCallTime(Time.unscaledTime))
			{
				return;
			}
			CMSTrigger cmstrigger;
			if (CMSSerializer.TryGetRegisteredTrigger(triggerID, out cmstrigger))
			{
				if (!cmstrigger.CanTrigger())
				{
					return;
				}
				Vector3 position = cmstrigger.gameObject.transform.position;
				RigContainer rigContainer2;
				if (!VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer2))
				{
					return;
				}
				if ((rigContainer2.Rig.bodyTransform.position - position).sqrMagnitude > cmstrigger.validationDistanceSquared)
				{
					return;
				}
			}
			base.SendRPC("ActivateTrigger_RPC", true, new object[]
			{
				triggerID,
				info.Sender.ActorNumber
			});
			this.ActivateTrigger(triggerID, info.SentServerTime, false);
		}

		// Token: 0x0600469F RID: 18079 RVA: 0x0014FE10 File Offset: 0x0014E010
		[PunRPC]
		private void ActivateTrigger_RPC(byte triggerID, int originatingPlayer, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "ActivateTrigger_RPC");
			if (!NetworkSystem.Instance.InRoom || !info.Sender.IsMasterClient)
			{
				return;
			}
			if (info.SentServerTime < 0.0 || info.SentServerTime > 4294967.295)
			{
				return;
			}
			double num = (double)PhotonNetwork.GetPing() / 1000.0;
			if (!Utils.ValidateServerTime(info.SentServerTime, Math.Max(10.0, num * 2.0)))
			{
				return;
			}
			if (!CMSSerializer.ActivateTriggerCallLimiter.CheckCallTime(Time.unscaledTime))
			{
				return;
			}
			this.ActivateTrigger(triggerID, info.SentServerTime, NetworkSystem.Instance.LocalPlayer.ActorNumber == originatingPlayer);
		}

		// Token: 0x060046A0 RID: 18080 RVA: 0x0014FED4 File Offset: 0x0014E0D4
		private void ActivateTrigger(byte triggerID, double triggerTime = -1.0, bool originatedLocally = false)
		{
			CMSTrigger cmstrigger;
			bool flag = CMSSerializer.TryGetRegisteredTrigger(triggerID, out cmstrigger);
			if (!double.IsFinite(triggerTime))
			{
				triggerTime = -1.0;
			}
			byte b;
			bool flag2 = CMSSerializer.triggerCounts.TryGetValue(triggerID, out b);
			bool flag3 = !flag || cmstrigger.numAllowedTriggers > 0;
			if (flag2)
			{
				CMSSerializer.triggerCounts[triggerID] = ((b == byte.MaxValue) ? byte.MaxValue : (b += 1));
			}
			else if (flag3)
			{
				CMSSerializer.triggerCounts.Add(triggerID, 1);
			}
			CMSSerializer.triggerHistory.Remove(triggerID);
			CMSSerializer.triggerHistory.Add(triggerID);
			if (flag)
			{
				cmstrigger.Trigger(triggerTime, originatedLocally, false);
			}
		}

		// Token: 0x0400491D RID: 18717
		[OnEnterPlay_SetNull]
		private static volatile CMSSerializer instance;

		// Token: 0x0400491E RID: 18718
		[OnEnterPlay_Set(false)]
		private static bool hasInstance;

		// Token: 0x0400491F RID: 18719
		private static Dictionary<string, Dictionary<byte, CMSTrigger>> registeredTriggersPerScene = new Dictionary<string, Dictionary<byte, CMSTrigger>>();

		// Token: 0x04004920 RID: 18720
		private static List<byte> triggerHistory = new List<byte>();

		// Token: 0x04004921 RID: 18721
		private static Dictionary<byte, byte> triggerCounts = new Dictionary<byte, byte>();

		// Token: 0x04004922 RID: 18722
		private static bool waitingForTriggerHistory;

		// Token: 0x04004923 RID: 18723
		private static List<string> scenesWaitingForTriggerHistory = new List<string>();

		// Token: 0x04004924 RID: 18724
		private static bool waitingForTriggerCounts;

		// Token: 0x04004925 RID: 18725
		private static List<string> scenesWaitingForTriggerCounts = new List<string>();

		// Token: 0x04004926 RID: 18726
		private static CallLimiter ActivateTriggerCallLimiter = new CallLimiter(50, 1f, 0.5f);

		// Token: 0x04004927 RID: 18727
		public static UnityEvent<string> OnTriggerHistoryProcessedForScene = new UnityEvent<string>();
	}
}
