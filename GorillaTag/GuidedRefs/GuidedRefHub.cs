using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaTag.GuidedRefs.Internal;
using UnityEngine;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x02000D5D RID: 3421
	[DefaultExecutionOrder(-2147483648)]
	public class GuidedRefHub : MonoBehaviour, IGuidedRefMonoBehaviour, IGuidedRefObject
	{
		// Token: 0x06005578 RID: 21880 RVA: 0x001A03C3 File Offset: 0x0019E5C3
		protected void Awake()
		{
			this.GuidedRefInitialize();
		}

		// Token: 0x06005579 RID: 21881 RVA: 0x001A03CC File Offset: 0x0019E5CC
		protected void OnDestroy()
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			if (this.isRootInstance)
			{
				GuidedRefHub.hasRootInstance = false;
				GuidedRefHub.rootInstance = null;
			}
			List<int> list;
			if (GuidedRefHub.globalLookupRefInstIDsByHub.TryGetValue(this, out list))
			{
				foreach (int num in list)
				{
					GuidedRefHub.globalLookupHubsThatHaveRegisteredInstId[num].Remove(this);
				}
				GuidedRefHub.globalLookupRefInstIDsByHub.Remove(this);
			}
		}

		// Token: 0x0600557A RID: 21882 RVA: 0x001A045C File Offset: 0x0019E65C
		public void GuidedRefInitialize()
		{
			if (this.isRootInstance)
			{
				if (GuidedRefHub.hasRootInstance)
				{
					Debug.LogError(string.Concat(new string[]
					{
						"GuidedRefHub: Attempted to assign global instance when one was already assigned:\n- This path: ",
						base.transform.GetPath(),
						"\n- Global instance: ",
						GuidedRefHub.rootInstance.transform.GetPath(),
						"\n"
					}), this);
					Object.Destroy(this);
					return;
				}
				GuidedRefHub.hasRootInstance = true;
				GuidedRefHub.rootInstance = this;
			}
			GuidedRefHub.globalLookupRefInstIDsByHub[this] = new List<int>(2);
		}

		// Token: 0x0600557B RID: 21883 RVA: 0x001A04E5 File Offset: 0x0019E6E5
		public static bool IsInstanceIDRegisteredWithAnyHub(int instanceID)
		{
			return GuidedRefHub.globalLookupHubsThatHaveRegisteredInstId.ContainsKey(instanceID);
		}

		// Token: 0x0600557C RID: 21884 RVA: 0x001A04F4 File Offset: 0x0019E6F4
		private void RegisterTarget_Internal<TIGuidedRefTargetMono>(TIGuidedRefTargetMono targetMono) where TIGuidedRefTargetMono : IGuidedRefTargetMono
		{
			RelayInfo orAddRelayInfoByTargetId = this.GetOrAddRelayInfoByTargetId(targetMono.GRefTargetInfo.targetId);
			if (orAddRelayInfoByTargetId == null)
			{
				return;
			}
			IGuidedRefTargetMono guidedRefTargetMono = targetMono;
			if (orAddRelayInfoByTargetId.targetMono != null && orAddRelayInfoByTargetId.targetMono != guidedRefTargetMono)
			{
				if (targetMono.GRefTargetInfo.hackIgnoreDuplicateRegistration)
				{
					return;
				}
				Debug.LogError(string.Concat(new string[]
				{
					"GuidedRefHub: Multiple targets registering with the same Hub. Maybe look at the HubID you are using:- hub=\"",
					base.transform.GetPath(),
					"\"\n- target1=\"",
					orAddRelayInfoByTargetId.targetMono.transform.GetPath(),
					"\",\n- target2=\"",
					targetMono.transform.GetPath(),
					"\""
				}), this);
				return;
			}
			else
			{
				int instanceID = targetMono.GetInstanceID();
				GuidedRefHub.GetHubsThatHaveRegisteredInstId(instanceID).Add(this);
				List<int> list;
				if (!GuidedRefHub.globalLookupRefInstIDsByHub.TryGetValue(this, out list))
				{
					Debug.LogError(string.Concat(new string[]
					{
						"GuidedRefHub: It appears hub was not registered before `RegisterTarget` was called on it: - hub: \"",
						base.transform.GetPath(),
						"\"\n- target: \"",
						targetMono.transform.GetPath(),
						"\""
					}), this);
					return;
				}
				list.Add(instanceID);
				orAddRelayInfoByTargetId.targetMono = targetMono;
				GuidedRefHub.ResolveReferences(orAddRelayInfoByTargetId);
				return;
			}
		}

		// Token: 0x0600557D RID: 21885 RVA: 0x001A0648 File Offset: 0x0019E848
		public static void RegisterTarget<TIGuidedRefTargetMono>(TIGuidedRefTargetMono targetMono, GuidedRefHubIdSO[] hubIds = null, Component debugCaller = null) where TIGuidedRefTargetMono : IGuidedRefTargetMono
		{
			if (targetMono == null)
			{
				string text = ((debugCaller == null) ? "UNSUPPLIED_CALLER_NAME" : debugCaller.name);
				Debug.LogError("GuidedRefHub: Cannot register null target from \"" + text + "\".", debugCaller);
				return;
			}
			if (targetMono.GRefTargetInfo.targetId == null)
			{
				return;
			}
			GuidedRefHub.globalHubsTransientList.Clear();
			targetMono.transform.GetComponentsInParent<GuidedRefHub>(true, GuidedRefHub.globalHubsTransientList);
			if (GuidedRefHub.hasRootInstance)
			{
				GuidedRefHub.globalHubsTransientList.Add(GuidedRefHub.rootInstance);
			}
			bool flag = false;
			foreach (GuidedRefHub guidedRefHub in GuidedRefHub.globalHubsTransientList)
			{
				if (hubIds == null || hubIds.Length <= 0 || Array.IndexOf<GuidedRefHubIdSO>(hubIds, guidedRefHub.hubId) != -1)
				{
					flag = true;
					guidedRefHub.RegisterTarget_Internal<TIGuidedRefTargetMono>(targetMono);
				}
			}
			if (!flag && Application.isPlaying)
			{
				Debug.LogError("GuidedRefHub: Could not find hub for target: \"" + targetMono.transform.GetPath() + "\"", targetMono.transform);
			}
		}

		// Token: 0x0600557E RID: 21886 RVA: 0x001A077C File Offset: 0x0019E97C
		public static void UnregisterTarget<TIGuidedRefTargetMono>(TIGuidedRefTargetMono targetMono, bool destroyed = true) where TIGuidedRefTargetMono : IGuidedRefTargetMono
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			if (targetMono == null)
			{
				Debug.LogError("GuidedRefHub: Cannot unregister null target.");
				return;
			}
			int instanceID = targetMono.GetInstanceID();
			List<GuidedRefHub> list;
			if (!GuidedRefHub.globalLookupHubsThatHaveRegisteredInstId.TryGetValue(instanceID, out list))
			{
				return;
			}
			using (List<GuidedRefHub>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					RelayInfo relayInfo;
					if (enumerator.Current.lookupRelayInfoByTargetId.TryGetValue(targetMono.GRefTargetInfo.targetId, out relayInfo))
					{
						foreach (RegisteredReceiverFieldInfo registeredReceiverFieldInfo in relayInfo.registeredFields)
						{
							if (registeredReceiverFieldInfo.receiverMono != null)
							{
								registeredReceiverFieldInfo.receiverMono.OnGuidedRefTargetDestroyed(registeredReceiverFieldInfo.fieldId);
								GuidedRefHub.kReceiversWaitingToFullyResolve.Remove(registeredReceiverFieldInfo.receiverMono);
								relayInfo.resolvedFields.Remove(registeredReceiverFieldInfo);
								relayInfo.registeredFields.Add(registeredReceiverFieldInfo);
								IGuidedRefReceiverMono receiverMono = registeredReceiverFieldInfo.receiverMono;
								int guidedRefsWaitingToResolveCount = receiverMono.GuidedRefsWaitingToResolveCount;
								receiverMono.GuidedRefsWaitingToResolveCount = guidedRefsWaitingToResolveCount + 1;
							}
						}
					}
				}
			}
			foreach (GuidedRefHub guidedRefHub in list)
			{
				RelayInfo relayInfo2;
				if (guidedRefHub.lookupRelayInfoByTargetId.TryGetValue(targetMono.GRefTargetInfo.targetId, out relayInfo2))
				{
					relayInfo2.targetMono = null;
				}
				GuidedRefHub.globalLookupRefInstIDsByHub[guidedRefHub].Remove(instanceID);
			}
			GuidedRefHub.globalLookupHubsThatHaveRegisteredInstId.Remove(instanceID);
		}

		// Token: 0x0600557F RID: 21887 RVA: 0x001A0944 File Offset: 0x0019EB44
		public static void ReceiverFullyRegistered<TIGuidedRefReceiverMono>(TIGuidedRefReceiverMono receiverMono) where TIGuidedRefReceiverMono : IGuidedRefReceiverMono
		{
			GuidedRefHub.kReceiversFullyRegistered.Add(receiverMono);
			GuidedRefHub.kReceiversWaitingToFullyResolve.Add(receiverMono);
			GuidedRefHub.CheckAndNotifyIfReceiverFullyResolved<TIGuidedRefReceiverMono>(receiverMono);
		}

		// Token: 0x06005580 RID: 21888 RVA: 0x001A0970 File Offset: 0x0019EB70
		private static void CheckAndNotifyIfReceiverFullyResolved<TIGuidedRefReceiverMono>(TIGuidedRefReceiverMono receiverMono) where TIGuidedRefReceiverMono : IGuidedRefReceiverMono
		{
			if (receiverMono.GuidedRefsWaitingToResolveCount == 0 && GuidedRefHub.kReceiversFullyRegistered.Contains(receiverMono))
			{
				GuidedRefHub.kReceiversWaitingToFullyResolve.Remove(receiverMono);
				receiverMono.OnAllGuidedRefsResolved();
			}
		}

		// Token: 0x06005581 RID: 21889 RVA: 0x001A09BC File Offset: 0x0019EBBC
		private void RegisterReceiverField(RegisteredReceiverFieldInfo registeredReceiverFieldInfo, GuidedRefTargetIdSO targetId)
		{
			GuidedRefHub.globalLookupRefInstIDsByHub[this].Add(registeredReceiverFieldInfo.receiverMono.GetInstanceID());
			GuidedRefHub.GetHubsThatHaveRegisteredInstId(registeredReceiverFieldInfo.receiverMono.GetInstanceID()).Add(this);
			RelayInfo orAddRelayInfoByTargetId = this.GetOrAddRelayInfoByTargetId(targetId);
			orAddRelayInfoByTargetId.registeredFields.Add(registeredReceiverFieldInfo);
			GuidedRefHub.ResolveReferences(orAddRelayInfoByTargetId);
		}

		// Token: 0x06005582 RID: 21890 RVA: 0x001A0A14 File Offset: 0x0019EC14
		private static void RegisterReceiverField_Internal<TIGuidedRefReceiverMono>(GuidedRefHubIdSO hubId, TIGuidedRefReceiverMono receiverMono, int fieldId, GuidedRefTargetIdSO targetId, int index) where TIGuidedRefReceiverMono : IGuidedRefReceiverMono
		{
			if (receiverMono == null)
			{
				Debug.LogError("GuidedRefHub: Cannot register null receiver.");
				return;
			}
			GuidedRefHub.globalHubsTransientList.Clear();
			receiverMono.transform.GetComponentsInParent<GuidedRefHub>(true, GuidedRefHub.globalHubsTransientList);
			if (GuidedRefHub.hasRootInstance)
			{
				GuidedRefHub.globalHubsTransientList.Add(GuidedRefHub.rootInstance);
			}
			RegisteredReceiverFieldInfo registeredReceiverFieldInfo = new RegisteredReceiverFieldInfo
			{
				receiverMono = receiverMono,
				fieldId = fieldId,
				index = index
			};
			bool flag = false;
			foreach (GuidedRefHub guidedRefHub in GuidedRefHub.globalHubsTransientList)
			{
				if (!(hubId != null) || !(guidedRefHub.hubId != hubId))
				{
					flag = true;
					guidedRefHub.RegisterReceiverField(registeredReceiverFieldInfo, targetId);
					break;
				}
			}
			if (flag)
			{
				int guidedRefsWaitingToResolveCount = receiverMono.GuidedRefsWaitingToResolveCount;
				receiverMono.GuidedRefsWaitingToResolveCount = guidedRefsWaitingToResolveCount + 1;
				return;
			}
			Debug.LogError("Could not find matching GuidedRefHub to register with for receiver at: " + receiverMono.transform.GetPath(), receiverMono.transform);
		}

		// Token: 0x06005583 RID: 21891 RVA: 0x001A0B4C File Offset: 0x0019ED4C
		public static void RegisterReceiverField<TIGuidedRefReceiverMono>(TIGuidedRefReceiverMono receiverMono, string fieldIdName, ref GuidedRefReceiverFieldInfo fieldInfo) where TIGuidedRefReceiverMono : IGuidedRefReceiverMono
		{
			if (!GRef.ShouldResolveNow(fieldInfo.resolveModes))
			{
				return;
			}
			fieldInfo.fieldId = Shader.PropertyToID(fieldIdName);
			GuidedRefHub.RegisterReceiverField_Internal<TIGuidedRefReceiverMono>(fieldInfo.hubId, receiverMono, fieldInfo.fieldId, fieldInfo.targetId, -1);
		}

		// Token: 0x06005584 RID: 21892 RVA: 0x001A0B84 File Offset: 0x0019ED84
		public static void RegisterReceiverArray<TIGuidedRefReceiverMono, T>(TIGuidedRefReceiverMono receiverMono, string fieldIdName, ref T[] receiverArray, ref GuidedRefReceiverArrayInfo arrayInfo) where TIGuidedRefReceiverMono : IGuidedRefReceiverMono where T : Object
		{
			if (!GRef.ShouldResolveNow(arrayInfo.resolveModes))
			{
				return;
			}
			if (receiverArray == null)
			{
				receiverArray = new T[arrayInfo.targets.Length];
			}
			else if (receiverArray.Length != arrayInfo.targets.Length)
			{
				Array.Resize<T>(ref receiverArray, arrayInfo.targets.Length);
			}
			arrayInfo.fieldId = Shader.PropertyToID(fieldIdName);
			for (int i = 0; i < arrayInfo.targets.Length; i++)
			{
				GuidedRefHub.RegisterReceiverField_Internal<TIGuidedRefReceiverMono>(arrayInfo.hubId, receiverMono, arrayInfo.fieldId, arrayInfo.targets[i], i);
			}
		}

		// Token: 0x06005585 RID: 21893 RVA: 0x001A0C0C File Offset: 0x0019EE0C
		public static void UnregisterReceiver<TIGuidedRefReceiverMono>(TIGuidedRefReceiverMono receiverMono) where TIGuidedRefReceiverMono : IGuidedRefReceiverMono
		{
			if (receiverMono == null)
			{
				Debug.LogError("GuidedRefHub: Cannot unregister null receiver.");
				return;
			}
			int instanceID = receiverMono.GetInstanceID();
			List<GuidedRefHub> list;
			if (!GuidedRefHub.globalLookupHubsThatHaveRegisteredInstId.TryGetValue(instanceID, out list))
			{
				Debug.LogError("Tried to unregister a receiver before it was registered.");
				return;
			}
			IGuidedRefReceiverMono iReceiverMono = receiverMono;
			Predicate<RegisteredReceiverFieldInfo> <>9__0;
			foreach (GuidedRefHub guidedRefHub in list)
			{
				foreach (RelayInfo relayInfo in guidedRefHub.lookupRelayInfoByTargetId.Values)
				{
					List<RegisteredReceiverFieldInfo> registeredFields = relayInfo.registeredFields;
					Predicate<RegisteredReceiverFieldInfo> predicate;
					if ((predicate = <>9__0) == null)
					{
						predicate = (<>9__0 = (RegisteredReceiverFieldInfo fieldInfo) => fieldInfo.receiverMono == iReceiverMono);
					}
					registeredFields.RemoveAll(predicate);
				}
				GuidedRefHub.globalLookupRefInstIDsByHub[guidedRefHub].Remove(instanceID);
			}
			GuidedRefHub.globalLookupHubsThatHaveRegisteredInstId.Remove(instanceID);
			receiverMono.GuidedRefsWaitingToResolveCount = 0;
		}

		// Token: 0x06005586 RID: 21894 RVA: 0x001A0D40 File Offset: 0x0019EF40
		private RelayInfo GetOrAddRelayInfoByTargetId(GuidedRefTargetIdSO targetId)
		{
			if (targetId == null)
			{
				Debug.LogError("GetOrAddRelayInfoByTargetId cannot register null target id");
				return null;
			}
			RelayInfo relayInfo;
			if (!this.lookupRelayInfoByTargetId.TryGetValue(targetId, out relayInfo))
			{
				relayInfo = new RelayInfo
				{
					targetMono = null,
					registeredFields = new List<RegisteredReceiverFieldInfo>(1),
					resolvedFields = new List<RegisteredReceiverFieldInfo>(1)
				};
				this.lookupRelayInfoByTargetId[targetId] = relayInfo;
				GuidedRefHub.static_relayInfo_to_targetId[relayInfo] = targetId;
			}
			return relayInfo;
		}

		// Token: 0x06005587 RID: 21895 RVA: 0x001A0DB4 File Offset: 0x0019EFB4
		public static List<GuidedRefHub> GetHubsThatHaveRegisteredInstId(int instanceId)
		{
			List<GuidedRefHub> list;
			if (!GuidedRefHub.globalLookupHubsThatHaveRegisteredInstId.TryGetValue(instanceId, out list))
			{
				list = new List<GuidedRefHub>(1);
				GuidedRefHub.globalLookupHubsThatHaveRegisteredInstId[instanceId] = list;
			}
			return list;
		}

		// Token: 0x06005588 RID: 21896 RVA: 0x001A0DE4 File Offset: 0x0019EFE4
		private static void ResolveReferences(RelayInfo relayInfo)
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			if (relayInfo == null)
			{
				Debug.LogError("GuidedRefHub.ResolveReferences: (this should never happen) relayInfo is null.");
				return;
			}
			if (relayInfo.registeredFields == null)
			{
				GuidedRefTargetIdSO guidedRefTargetIdSO = GuidedRefHub.static_relayInfo_to_targetId[relayInfo];
				string text = ((guidedRefTargetIdSO != null) ? guidedRefTargetIdSO.name : "NULL");
				Debug.LogError("GuidedRefHub.ResolveReferences: (this should never happen) \"" + text + "\"relayInfo.registeredFields is null.");
				return;
			}
			if (relayInfo.targetMono == null)
			{
				return;
			}
			for (int i = relayInfo.registeredFields.Count - 1; i >= 0; i--)
			{
				RegisteredReceiverFieldInfo registeredReceiverFieldInfo = relayInfo.registeredFields[i];
				if (registeredReceiverFieldInfo.receiverMono.GuidedRefTryResolveReference(new GuidedRefTryResolveInfo
				{
					fieldId = registeredReceiverFieldInfo.fieldId,
					index = registeredReceiverFieldInfo.index,
					targetMono = relayInfo.targetMono
				}))
				{
					relayInfo.registeredFields.RemoveAt(i);
					GuidedRefHub.CheckAndNotifyIfReceiverFullyResolved<IGuidedRefReceiverMono>(registeredReceiverFieldInfo.receiverMono);
					relayInfo.resolvedFields.Add(registeredReceiverFieldInfo);
				}
			}
		}

		// Token: 0x06005589 RID: 21897 RVA: 0x001A0ED8 File Offset: 0x0019F0D8
		public static bool TryResolveField<TIGuidedRefReceiverMono, T>(TIGuidedRefReceiverMono receiverMono, ref T refReceiverObj, GuidedRefReceiverFieldInfo receiverFieldInfo, GuidedRefTryResolveInfo tryResolveInfo) where TIGuidedRefReceiverMono : IGuidedRefReceiverMono where T : Object
		{
			if (tryResolveInfo.index > -1 || tryResolveInfo.fieldId != receiverFieldInfo.fieldId || refReceiverObj != null)
			{
				return false;
			}
			bool flag = tryResolveInfo.targetMono != null && tryResolveInfo.targetMono.GuidedRefTargetObject != null;
			T t = (flag ? (tryResolveInfo.targetMono.GuidedRefTargetObject as T) : default(T));
			if (!flag)
			{
				string fieldNameByID = GuidedRefHub.GetFieldNameByID(tryResolveInfo.fieldId);
				Debug.LogError(string.Concat(new string[]
				{
					"TryResolveField: Receiver \"",
					receiverMono.transform.name,
					"\" with field \"",
					fieldNameByID,
					"\": was already assigned to something other than matching target id! Assigning to found target anyway. Make the receiving field null before attempting to resolve to prevent this message. ",
					string.Format("fieldId={0}, receiver path=\"{1}\"", tryResolveInfo.fieldId, receiverMono.transform.GetPath())
				}));
			}
			else if (refReceiverObj != null && refReceiverObj != t)
			{
				Debug.LogError("was assigned didn't match assigning anyway");
				string fieldNameByID2 = GuidedRefHub.GetFieldNameByID(tryResolveInfo.fieldId);
				Debug.LogError(string.Concat(new string[]
				{
					"TryResolveField: Receiver \"",
					receiverMono.transform.name,
					"\" with field \"",
					fieldNameByID2,
					"\" was already assigned to something other than matching target id! Assigning to found target anyway. Make the receiving field null before attempting to resolve to prevent this message. ",
					string.Format("fieldId={0}, receiver path=\"{1}\"", tryResolveInfo.fieldId, receiverMono.transform.GetPath())
				}));
			}
			refReceiverObj = t;
			int guidedRefsWaitingToResolveCount = receiverMono.GuidedRefsWaitingToResolveCount;
			receiverMono.GuidedRefsWaitingToResolveCount = guidedRefsWaitingToResolveCount - 1;
			return true;
		}

		// Token: 0x0600558A RID: 21898 RVA: 0x001A10AC File Offset: 0x0019F2AC
		public static bool TryResolveArrayItem<TIGuidedRefReceiverMono, T>(TIGuidedRefReceiverMono receiverMono, IList<T> receivingArray, GuidedRefReceiverArrayInfo receiverArrayInfo, GuidedRefTryResolveInfo tryResolveInfo) where TIGuidedRefReceiverMono : IGuidedRefReceiverMono where T : Object
		{
			bool flag;
			return GuidedRefHub.TryResolveArrayItem<TIGuidedRefReceiverMono, T>(receiverMono, receivingArray, receiverArrayInfo, tryResolveInfo, out flag);
		}

		// Token: 0x0600558B RID: 21899 RVA: 0x001A10C4 File Offset: 0x0019F2C4
		public static bool TryResolveArrayItem<TIGuidedRefReceiverMono, T>(TIGuidedRefReceiverMono receiverMono, IList<T> receivingArray, GuidedRefReceiverArrayInfo receiverArrayInfo, GuidedRefTryResolveInfo tryResolveInfo, out bool arrayResolved) where TIGuidedRefReceiverMono : IGuidedRefReceiverMono where T : Object
		{
			arrayResolved = false;
			if (tryResolveInfo.index <= -1 && receiverArrayInfo.fieldId != tryResolveInfo.fieldId)
			{
				return false;
			}
			if (receivingArray == null)
			{
				string fieldNameByID = GuidedRefHub.GetFieldNameByID(tryResolveInfo.fieldId);
				Debug.LogError(string.Concat(new string[]
				{
					"TryResolveArrayItem: Receiver \"",
					receiverMono.transform.name,
					"\" with array \"",
					fieldNameByID,
					"\": Receiving array cannot be null!",
					string.Format("fieldId={0}, receiver path=\"{1}\"", tryResolveInfo.fieldId, receiverMono.transform.GetPath())
				}));
				return false;
			}
			if (receiverArrayInfo.targets == null)
			{
				string fieldNameByID2 = GuidedRefHub.GetFieldNameByID(tryResolveInfo.fieldId);
				Debug.LogError(string.Concat(new string[]
				{
					"TryResolveArrayItem: Receiver component \"",
					receiverMono.transform.name,
					"\" with array \"",
					fieldNameByID2,
					"\": Targets array is null! It must have been set to null after registering. If this intentional than the you need to unregister first.",
					string.Format("fieldId={0}, receiver path=\"{1}\"", tryResolveInfo.fieldId, receiverMono.transform.GetPath())
				}));
				return false;
			}
			int num = receiverArrayInfo.targets.Length;
			if (num <= receiverArrayInfo.resolveCount)
			{
				string fieldNameByID3 = GuidedRefHub.GetFieldNameByID(tryResolveInfo.fieldId);
				Debug.LogError(string.Concat(new string[]
				{
					"TryResolveArrayItem: Receiver component \"",
					receiverMono.transform.name,
					"\" with array \"",
					fieldNameByID3,
					"\": Targets array size is equal or smaller than resolve count. Did you change the size of the array before it finished resolving or before unregistering?",
					string.Format("fieldId={0}, receiver path=\"{1}\"", tryResolveInfo.fieldId, receiverMono.transform.GetPath())
				}));
				return false;
			}
			if (num != receivingArray.Count)
			{
				string fieldNameByID4 = GuidedRefHub.GetFieldNameByID(tryResolveInfo.fieldId);
				Debug.LogError(string.Concat(new string[]
				{
					"TryResolveArrayItem: Receiver component \"",
					receiverMono.transform.name,
					"\" with array \"",
					fieldNameByID4,
					"\": The sizes of `receivingList` and `receiverArrayInfo.fieldInfos` are not equal. They must be the same length before calling.",
					string.Format("fieldId={0}, receiver path=\"{1}\"", tryResolveInfo.fieldId, receiverMono.transform.GetPath())
				}));
				return false;
			}
			T t = tryResolveInfo.targetMono.GuidedRefTargetObject as T;
			if (t == null)
			{
				string fieldNameByID5 = GuidedRefHub.GetFieldNameByID(tryResolveInfo.fieldId);
				Debug.LogError(string.Concat(new string[]
				{
					"TryResolveArrayItem: Receiver \"",
					receiverMono.transform.name,
					"\" with field \"",
					fieldNameByID5,
					"\" found a matching target id but target object was null! Was it destroyed without unregistering? ",
					string.Format("fieldId={0}, receiver path=\"{1}\"", tryResolveInfo.fieldId, receiverMono.transform.GetPath())
				}));
			}
			if (receivingArray[tryResolveInfo.index] != null && receivingArray[tryResolveInfo.index] != t)
			{
				string fieldNameByID6 = GuidedRefHub.GetFieldNameByID(tryResolveInfo.fieldId);
				Debug.LogError(string.Concat(new string[]
				{
					"TryResolveArrayItem: Receiver \"",
					receiverMono.transform.name,
					"\" with array \"",
					fieldNameByID6,
					"\" ",
					string.Format("at index {0}: Already assigned to something other than matching target id! ", tryResolveInfo.index),
					"Assigning to found target anyway. Make the receiving field null before attempting to resolve to prevent this message. ",
					string.Format("fieldId={0}, receiver path=\"{1}\"", tryResolveInfo.fieldId, receiverMono.transform.GetPath())
				}));
			}
			int num2 = receiverArrayInfo.resolveCount + 1;
			receiverArrayInfo.resolveCount = num2;
			arrayResolved = num2 >= num;
			num2 = receiverMono.GuidedRefsWaitingToResolveCount;
			receiverMono.GuidedRefsWaitingToResolveCount = num2 - 1;
			receivingArray[tryResolveInfo.index] = t;
			return true;
		}

		// Token: 0x0600558C RID: 21900 RVA: 0x001A14B7 File Offset: 0x0019F6B7
		public static string GetFieldNameByID(int fieldId)
		{
			return "FieldNameOnlyAvailableInEditor";
		}

		// Token: 0x0600558F RID: 21903 RVA: 0x00045F89 File Offset: 0x00044189
		Transform IGuidedRefMonoBehaviour.get_transform()
		{
			return base.transform;
		}

		// Token: 0x06005590 RID: 21904 RVA: 0x00017401 File Offset: 0x00015601
		int IGuidedRefObject.GetInstanceID()
		{
			return base.GetInstanceID();
		}

		// Token: 0x040058E4 RID: 22756
		[SerializeField]
		private bool isRootInstance;

		// Token: 0x040058E5 RID: 22757
		public GuidedRefHubIdSO hubId;

		// Token: 0x040058E6 RID: 22758
		[OnEnterPlay_SetNull]
		[NonSerialized]
		public static GuidedRefHub rootInstance;

		// Token: 0x040058E7 RID: 22759
		[OnEnterPlay_Set(false)]
		[NonSerialized]
		public static bool hasRootInstance;

		// Token: 0x040058E8 RID: 22760
		[DebugReadout]
		private readonly Dictionary<GuidedRefTargetIdSO, RelayInfo> lookupRelayInfoByTargetId = new Dictionary<GuidedRefTargetIdSO, RelayInfo>(256);

		// Token: 0x040058E9 RID: 22761
		private static readonly Dictionary<RelayInfo, GuidedRefTargetIdSO> static_relayInfo_to_targetId = new Dictionary<RelayInfo, GuidedRefTargetIdSO>(256);

		// Token: 0x040058EA RID: 22762
		[OnEnterPlay_Clear]
		private static readonly Dictionary<int, List<GuidedRefHub>> globalLookupHubsThatHaveRegisteredInstId = new Dictionary<int, List<GuidedRefHub>>(256);

		// Token: 0x040058EB RID: 22763
		[OnEnterPlay_Clear]
		private static readonly Dictionary<GuidedRefHub, List<int>> globalLookupRefInstIDsByHub = new Dictionary<GuidedRefHub, List<int>>(256);

		// Token: 0x040058EC RID: 22764
		[OnEnterPlay_Clear]
		private static readonly List<GuidedRefHub> globalHubsTransientList = new List<GuidedRefHub>(32);

		// Token: 0x040058ED RID: 22765
		private const string kUnsuppliedCallerName = "UNSUPPLIED_CALLER_NAME";

		// Token: 0x040058EE RID: 22766
		[DebugReadout]
		[OnEnterPlay_Clear]
		internal static readonly HashSet<IGuidedRefReceiverMono> kReceiversWaitingToFullyResolve = new HashSet<IGuidedRefReceiverMono>(256);

		// Token: 0x040058EF RID: 22767
		[DebugReadout]
		[OnEnterPlay_Clear]
		internal static readonly HashSet<IGuidedRefReceiverMono> kReceiversFullyRegistered = new HashSet<IGuidedRefReceiverMono>(256);
	}
}
