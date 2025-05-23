using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x02000C4A RID: 3146
	public class GorillaServer : MonoBehaviour, ISerializationCallbackReceiver
	{
		// Token: 0x170007B7 RID: 1975
		// (get) Token: 0x06004E1D RID: 19997 RVA: 0x0017482F File Offset: 0x00172A2F
		public bool FeatureFlagsReady
		{
			get
			{
				return this.featureFlags.ready;
			}
		}

		// Token: 0x170007B8 RID: 1976
		// (get) Token: 0x06004E1E RID: 19998 RVA: 0x0017483C File Offset: 0x00172A3C
		private global::PlayFab.CloudScriptModels.EntityKey playerEntity
		{
			get
			{
				return new global::PlayFab.CloudScriptModels.EntityKey
				{
					Id = PlayFabSettings.staticPlayer.EntityId,
					Type = PlayFabSettings.staticPlayer.EntityType
				};
			}
		}

		// Token: 0x06004E1F RID: 19999 RVA: 0x00174863 File Offset: 0x00172A63
		public void Start()
		{
			this.featureFlags.FetchFeatureFlags();
		}

		// Token: 0x06004E20 RID: 20000 RVA: 0x00174870 File Offset: 0x00172A70
		private void Awake()
		{
			if (GorillaServer.Instance == null)
			{
				GorillaServer.Instance = this;
				return;
			}
			Object.Destroy(this);
		}

		// Token: 0x06004E21 RID: 20001 RVA: 0x00174890 File Offset: 0x00172A90
		public void ReturnCurrentVersion(ReturnCurrentVersionRequest request, Action<ExecuteFunctionResult> successCallback, Action<PlayFabError> errorCallback)
		{
			successCallback = this.DebugWrapCb<ExecuteFunctionResult>(successCallback, "ReturnCurrentVersion result");
			errorCallback = this.DebugWrapCb<PlayFabError>(errorCallback, "ReturnCurrentVersion error");
			if (this.featureFlags.IsEnabledForUser("2024-05-ReturnCurrentVersionV2"))
			{
				Debug.Log("GorillaServer: ReturnCurrentVersion V2 call");
				PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
				{
					Entity = this.playerEntity,
					FunctionName = "ReturnCurrentVersionV2",
					FunctionParameter = request
				}, successCallback, errorCallback, null, null);
				return;
			}
			Debug.Log("GorillaServer: ReturnCurrentVersion LEGACY call");
			PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest
			{
				FunctionName = "ReturnCurrentVersionNew",
				FunctionParameter = request
			}, delegate(global::PlayFab.ClientModels.ExecuteCloudScriptResult result)
			{
				successCallback(this.toFunctionResult(result));
			}, errorCallback, null, null);
		}

		// Token: 0x06004E22 RID: 20002 RVA: 0x0017495C File Offset: 0x00172B5C
		public void ReturnMyOculusHash(Action<ExecuteFunctionResult> successCallback, Action<PlayFabError> errorCallback)
		{
			successCallback = this.DebugWrapCb<ExecuteFunctionResult>(successCallback, "ReturnMyOculusHash result");
			errorCallback = this.DebugWrapCb<PlayFabError>(errorCallback, "ReturnMyOculusHash error");
			if (this.featureFlags.IsEnabledForUser("2024-05-ReturnMyOculusHashV2"))
			{
				Debug.Log("GorillaServer: ReturnMyOculusHash V2 call");
				PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
				{
					Entity = this.playerEntity,
					FunctionName = "ReturnMyOculusHashV2",
					FunctionParameter = new { }
				}, successCallback, errorCallback, null, null);
				return;
			}
			Debug.Log("GorillaServer: ReturnMyOculusHash LEGACY call");
			PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest
			{
				FunctionName = "ReturnMyOculusHash"
			}, delegate(global::PlayFab.ClientModels.ExecuteCloudScriptResult result)
			{
				successCallback(this.toFunctionResult(result));
			}, errorCallback, null, null);
		}

		// Token: 0x06004E23 RID: 20003 RVA: 0x00174A24 File Offset: 0x00172C24
		public void TryDistributeCurrency(Action<ExecuteFunctionResult> successCallback, Action<PlayFabError> errorCallback)
		{
			successCallback = this.DebugWrapCb<ExecuteFunctionResult>(successCallback, "TryDistributeCurrency result");
			errorCallback = this.DebugWrapCb<PlayFabError>(errorCallback, "TryDistributeCurrency error");
			if (this.featureFlags.IsEnabledForUser("2024-05-TryDistributeCurrencyV2"))
			{
				Debug.Log("GorillaServer: TryDistributeCurrency V2 call");
				PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
				{
					Entity = this.playerEntity,
					FunctionName = "TryDistributeCurrencyV2",
					FunctionParameter = new { }
				}, successCallback, errorCallback, null, null);
				return;
			}
			Debug.Log("GorillaServer: TryDistributeCurrency LEGACY call");
			PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest
			{
				FunctionName = "TryDistributeCurrency",
				FunctionParameter = new { }
			}, delegate(global::PlayFab.ClientModels.ExecuteCloudScriptResult result)
			{
				successCallback(this.toFunctionResult(result));
			}, errorCallback, null, null);
		}

		// Token: 0x06004E24 RID: 20004 RVA: 0x00174AF8 File Offset: 0x00172CF8
		public void AddOrRemoveDLCOwnership(Action<ExecuteFunctionResult> successCallback, Action<PlayFabError> errorCallback)
		{
			successCallback = this.DebugWrapCb<ExecuteFunctionResult>(successCallback, "AddOrRemoveDLCOwnership result");
			errorCallback = this.DebugWrapCb<PlayFabError>(errorCallback, "AddOrRemoveDLCOwnership error");
			if (this.featureFlags.IsEnabledForUser("2024-05-AddOrRemoveDLCOwnershipV2"))
			{
				Debug.Log("GorillaServer: AddOrRemoveDLCOwnership V2 call");
				PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
				{
					Entity = this.playerEntity,
					FunctionName = "AddOrRemoveDLCOwnershipV2",
					FunctionParameter = new { }
				}, successCallback, errorCallback, null, null);
				return;
			}
			Debug.Log("GorillaServer: AddOrRemoveDLCOwnership LEGACY call");
			PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest
			{
				FunctionName = "AddOrRemoveDLCOwnership",
				FunctionParameter = new { }
			}, delegate(global::PlayFab.ClientModels.ExecuteCloudScriptResult result)
			{
				successCallback(this.toFunctionResult(result));
			}, errorCallback, null, null);
		}

		// Token: 0x06004E25 RID: 20005 RVA: 0x00174BCC File Offset: 0x00172DCC
		public void BroadcastMyRoom(BroadcastMyRoomRequest request, Action<ExecuteFunctionResult> successCallback, Action<PlayFabError> errorCallback)
		{
			successCallback = this.DebugWrapCb<ExecuteFunctionResult>(successCallback, "BroadcastMyRoom result");
			errorCallback = this.DebugWrapCb<PlayFabError>(errorCallback, "BroadcastMyRoom error");
			if (this.featureFlags.IsEnabledForUser("2024-05-BroadcastMyRoomV2"))
			{
				Debug.Log(string.Format("GorillaServer: BroadcastMyRoom V2 call ({0})", request));
				PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
				{
					Entity = this.playerEntity,
					FunctionName = "BroadcastMyRoomV2",
					FunctionParameter = request
				}, successCallback, errorCallback, null, null);
				return;
			}
			Debug.Log(string.Format("GorillaServer: BroadcastMyRoom LEGACY call ({0})", request));
			PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest
			{
				FunctionName = "BroadcastMyRoom",
				FunctionParameter = request
			}, delegate(global::PlayFab.ClientModels.ExecuteCloudScriptResult result)
			{
				successCallback(this.toFunctionResult(result));
			}, errorCallback, null, null);
		}

		// Token: 0x06004E26 RID: 20006 RVA: 0x00174CA1 File Offset: 0x00172EA1
		public bool NewCosmeticsPath()
		{
			return this.featureFlags.IsEnabledForUser("2024-06-CosmeticsAuthenticationV2");
		}

		// Token: 0x06004E27 RID: 20007 RVA: 0x00174CB3 File Offset: 0x00172EB3
		public bool NewCosmeticsPathShouldSetSharedGroupData()
		{
			return this.featureFlags.IsEnabledForUser("2025-04-CosmeticsAuthenticationV2-SetData");
		}

		// Token: 0x06004E28 RID: 20008 RVA: 0x00174CC5 File Offset: 0x00172EC5
		public bool NewCosmeticsPathShouldReadSharedGroupData()
		{
			return this.featureFlags.IsEnabledForUser("2025-04-CosmeticsAuthenticationV2-ReadData");
		}

		// Token: 0x06004E29 RID: 20009 RVA: 0x00174CD7 File Offset: 0x00172ED7
		public bool NewCosmeticsPathShouldSetRoomData()
		{
			return this.featureFlags.IsEnabledForUser("2025-04-CosmeticsAuthenticationV2-Compat");
		}

		// Token: 0x06004E2A RID: 20010 RVA: 0x00174CEC File Offset: 0x00172EEC
		public void UpdateUserCosmetics()
		{
			ExecuteFunctionRequest executeFunctionRequest = new ExecuteFunctionRequest();
			executeFunctionRequest.Entity = this.playerEntity;
			executeFunctionRequest.FunctionName = "UpdatePersonalCosmeticsList";
			executeFunctionRequest.FunctionParameter = new { };
			executeFunctionRequest.GeneratePlayStreamEvent = new bool?(false);
			PlayFabCloudScriptAPI.ExecuteFunction(executeFunctionRequest, delegate(ExecuteFunctionResult result)
			{
				if (CosmeticsController.instance != null)
				{
					CosmeticsController.instance.CheckCosmeticsSharedGroup();
				}
			}, delegate(PlayFabError error)
			{
			}, null, null);
		}

		// Token: 0x06004E2B RID: 20011 RVA: 0x00174D74 File Offset: 0x00172F74
		public void GetAcceptedAgreements(GetAcceptedAgreementsRequest request, Action<Dictionary<string, string>> successCallback, Action<PlayFabError> errorCallback)
		{
			successCallback = this.DebugWrapCb<Dictionary<string, string>>(successCallback, "GetAcceptedAgreements result");
			errorCallback = this.DebugWrapCb<PlayFabError>(errorCallback, "GetAcceptedAgreements json error");
			Debug.Log(string.Format("GorillaServer: GetAcceptedAgreements call ({0})", request));
			PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
			{
				Entity = this.playerEntity,
				FunctionName = "GetAcceptedAgreements",
				FunctionParameter = string.Join(",", request.AgreementKeys),
				GeneratePlayStreamEvent = new bool?(false)
			}, delegate(ExecuteFunctionResult result)
			{
				try
				{
					string text = Convert.ToString(result.FunctionResult);
					successCallback(JsonConvert.DeserializeObject<Dictionary<string, string>>(text));
				}
				catch (Exception ex)
				{
					errorCallback(new PlayFabError
					{
						ErrorMessage = string.Format("Invalid format for GetAcceptedAgreements ({0})", ex),
						Error = PlayFabErrorCode.JsonParseError
					});
				}
			}, errorCallback, null, null);
		}

		// Token: 0x06004E2C RID: 20012 RVA: 0x00174E2C File Offset: 0x0017302C
		public void SubmitAcceptedAgreements(SubmitAcceptedAgreementsRequest request, Action<ExecuteFunctionResult> successCallback, Action<PlayFabError> errorCallback)
		{
			successCallback = this.DebugWrapCb<ExecuteFunctionResult>(successCallback, "SubmitAcceptedAgreements result");
			errorCallback = this.DebugWrapCb<PlayFabError>(errorCallback, "SubmitAcceptedAgreements error");
			Debug.Log(string.Format("GorillaServer: SubmitAcceptedAgreements call ({0})", request));
			PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
			{
				Entity = this.playerEntity,
				FunctionName = "SubmitAcceptedAgreements",
				FunctionParameter = request.Agreements,
				GeneratePlayStreamEvent = new bool?(false)
			}, successCallback, errorCallback, null, null);
		}

		// Token: 0x06004E2D RID: 20013 RVA: 0x00174EA4 File Offset: 0x001730A4
		public void GetUserAge(Action<int> successCallback, Action<PlayFabError> errorCallback)
		{
			successCallback = this.DebugWrapCb<int>(successCallback, "GetUserAge result");
			errorCallback = this.DebugWrapCb<PlayFabError>(errorCallback, "GetUserAge json error");
			Debug.Log("GorillaServer: GetUserAge call");
			PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
			{
				Entity = this.playerEntity,
				FunctionName = "GetUserAge",
				GeneratePlayStreamEvent = new bool?(false)
			}, delegate(ExecuteFunctionResult result)
			{
				try
				{
					string text = Convert.ToString(result.FunctionResult);
					successCallback(JsonConvert.DeserializeObject<int>(text));
				}
				catch (Exception ex)
				{
					errorCallback(new PlayFabError
					{
						ErrorMessage = string.Format("Invalid format for GetAcceptedAgreements ({0})", ex),
						Error = PlayFabErrorCode.JsonParseError
					});
				}
			}, errorCallback, null, null);
		}

		// Token: 0x06004E2E RID: 20014 RVA: 0x00174F40 File Offset: 0x00173140
		public void SubmitUserAge(SubmitUserAgeRequest request, Action<ExecuteFunctionResult> successCallback, Action<PlayFabError> errorCallback)
		{
			successCallback = this.DebugWrapCb<ExecuteFunctionResult>(successCallback, "SubmitUserAge result");
			errorCallback = this.DebugWrapCb<PlayFabError>(errorCallback, "SubmitUserAge error");
			Debug.Log(string.Format("GorillaServer: SubmitUserAge call ({0})", request));
			PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
			{
				Entity = this.playerEntity,
				FunctionName = "SubmitUserAge",
				FunctionParameter = request.UserAge,
				GeneratePlayStreamEvent = new bool?(false)
			}, successCallback, errorCallback, null, null);
		}

		// Token: 0x06004E2F RID: 20015 RVA: 0x00174FB8 File Offset: 0x001731B8
		public void UploadGorillanalytics(object uploadData)
		{
			Debug.Log(string.Format("GorillaServer: UploadGorillanalytics call ({0})", uploadData));
			ExecuteFunctionRequest executeFunctionRequest = new ExecuteFunctionRequest();
			executeFunctionRequest.Entity = this.playerEntity;
			executeFunctionRequest.FunctionName = "Gorillanalytics";
			executeFunctionRequest.FunctionParameter = uploadData;
			executeFunctionRequest.GeneratePlayStreamEvent = new bool?(false);
			PlayFabCloudScriptAPI.ExecuteFunction(executeFunctionRequest, delegate(ExecuteFunctionResult result)
			{
				Debug.Log(string.Format("The {0} function took {1} to complete", result.FunctionName, result.ExecutionTimeMilliseconds));
			}, delegate(PlayFabError error)
			{
				Debug.Log("Error uploading Gorillanalytics: " + error.GenerateErrorReport());
			}, null, null);
		}

		// Token: 0x06004E30 RID: 20016 RVA: 0x0017504C File Offset: 0x0017324C
		public void CheckForBadName(CheckForBadNameRequest request, Action<ExecuteFunctionResult> successCallback, Action<PlayFabError> errorCallback)
		{
			successCallback = this.DebugWrapCb<ExecuteFunctionResult>(successCallback, "CheckForBadName result");
			errorCallback = this.DebugWrapCb<PlayFabError>(errorCallback, "CheckForBadName error");
			Debug.Log(string.Format("GorillaServer: CheckForBadName call ({0})", request));
			PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
			{
				Entity = this.playerEntity,
				FunctionName = "CheckForBadName",
				FunctionParameter = new
				{
					name = request.name,
					forRoom = request.forRoom.ToString()
				},
				GeneratePlayStreamEvent = new bool?(false)
			}, successCallback, errorCallback, null, null);
		}

		// Token: 0x06004E31 RID: 20017 RVA: 0x001750D4 File Offset: 0x001732D4
		public void GetRandomName(Action<ExecuteFunctionResult> successCallback, Action<PlayFabError> errorCallback)
		{
			successCallback = this.DebugWrapCb<ExecuteFunctionResult>(successCallback, "GetRandomName result");
			errorCallback = this.DebugWrapCb<PlayFabError>(errorCallback, "GetRandomName error");
			Debug.Log("GorillaServer: GetRandomName call");
			PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
			{
				Entity = this.playerEntity,
				FunctionName = "GetRandomName",
				GeneratePlayStreamEvent = new bool?(false)
			}, successCallback, errorCallback, null, null);
		}

		// Token: 0x06004E32 RID: 20018 RVA: 0x00175138 File Offset: 0x00173338
		public void ReturnQueueStats(ReturnQueueStatsRequest request, Action<ExecuteFunctionResult> successCallback, Action<PlayFabError> errorCallback)
		{
			successCallback = this.DebugWrapCb<ExecuteFunctionResult>(successCallback, "ReturnQueueStats result");
			errorCallback = this.DebugWrapCb<PlayFabError>(errorCallback, "ReturnQueueStats error");
			Debug.Log("GorillaServer: ReturnQueueStats call");
			PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
			{
				Entity = this.playerEntity,
				FunctionName = "ReturnQueueStats",
				FunctionParameter = new
				{
					QueueName = request.queueName
				},
				GeneratePlayStreamEvent = new bool?(false)
			}, successCallback, errorCallback, null, null);
		}

		// Token: 0x06004E33 RID: 20019 RVA: 0x001751AD File Offset: 0x001733AD
		private Action<T> DebugWrapCb<T>(Action<T> cb, string label)
		{
			return delegate(T arg)
			{
				if (this.debug)
				{
					try
					{
						Debug.Log(string.Concat(new string[]
						{
							"GorillaServer: ",
							label,
							" (",
							JsonConvert.SerializeObject(arg, this.serializationSettings),
							")"
						}));
					}
					catch (Exception ex)
					{
						Debug.LogError(string.Format("GorillaServer: {0} Error printing failure log: {1}", label, ex));
					}
				}
				cb(arg);
			};
		}

		// Token: 0x06004E34 RID: 20020 RVA: 0x001751D4 File Offset: 0x001733D4
		private ExecuteFunctionResult toFunctionResult(global::PlayFab.ClientModels.ExecuteCloudScriptResult csResult)
		{
			FunctionExecutionError functionExecutionError = null;
			if (csResult.Error != null)
			{
				functionExecutionError = new FunctionExecutionError
				{
					Error = csResult.Error.Error,
					Message = csResult.Error.Message,
					StackTrace = csResult.Error.StackTrace
				};
			}
			return new ExecuteFunctionResult
			{
				CustomData = csResult.CustomData,
				Error = functionExecutionError,
				ExecutionTimeMilliseconds = Convert.ToInt32(Math.Round(csResult.ExecutionTimeSeconds * 1000.0)),
				FunctionName = csResult.FunctionName,
				FunctionResult = csResult.FunctionResult,
				FunctionResultTooLarge = csResult.FunctionResultTooLarge
			};
		}

		// Token: 0x06004E35 RID: 20021 RVA: 0x00175280 File Offset: 0x00173480
		public void OnBeforeSerialize()
		{
			this.FeatureFlagsTitleDataKey = this.featureFlags.TitleDataKey;
			this.DefaultDeployFeatureFlagsEnabled.Clear();
			foreach (KeyValuePair<string, bool> keyValuePair in this.featureFlags.defaults)
			{
				if (keyValuePair.Value)
				{
					this.DefaultDeployFeatureFlagsEnabled.Add(keyValuePair.Key);
				}
			}
		}

		// Token: 0x06004E36 RID: 20022 RVA: 0x00175308 File Offset: 0x00173508
		public void OnAfterDeserialize()
		{
			this.featureFlags.TitleDataKey = this.FeatureFlagsTitleDataKey;
			foreach (string text in this.DefaultDeployFeatureFlagsEnabled)
			{
				this.featureFlags.defaults.AddOrUpdate(text, true);
			}
		}

		// Token: 0x06004E37 RID: 20023 RVA: 0x00175378 File Offset: 0x00173578
		public bool CheckIsInKIDOptInCohort()
		{
			return this.featureFlags.IsEnabledForUser("2025-04-KIDOptIn");
		}

		// Token: 0x06004E38 RID: 20024 RVA: 0x0017538A File Offset: 0x0017358A
		public bool CheckIsInKIDRequiredCohort()
		{
			return this.featureFlags.IsEnabledForUser("2025-04-KIDRequired");
		}

		// Token: 0x06004E39 RID: 20025 RVA: 0x0017539C File Offset: 0x0017359C
		public bool CheckOptedInKID()
		{
			return KIDManager.HasOptedInToKID;
		}

		// Token: 0x04005134 RID: 20788
		public static volatile GorillaServer Instance;

		// Token: 0x04005135 RID: 20789
		public string FeatureFlagsTitleDataKey = "DeployFeatureFlags";

		// Token: 0x04005136 RID: 20790
		public List<string> DefaultDeployFeatureFlagsEnabled = new List<string>();

		// Token: 0x04005137 RID: 20791
		private TitleDataFeatureFlags featureFlags = new TitleDataFeatureFlags();

		// Token: 0x04005138 RID: 20792
		private bool debug;

		// Token: 0x04005139 RID: 20793
		private JsonSerializerSettings serializationSettings = new JsonSerializerSettings
		{
			NullValueHandling = NullValueHandling.Ignore,
			DefaultValueHandling = DefaultValueHandling.Ignore,
			MissingMemberHandling = MissingMemberHandling.Ignore,
			ObjectCreationHandling = ObjectCreationHandling.Replace,
			ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
			TypeNameHandling = TypeNameHandling.Auto
		};
	}
}
