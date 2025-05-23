using System;
using System.Collections.Generic;
using Fusion;
using GorillaExtensions;
using Photon.Realtime;
using UnityEngine;

namespace GorillaGameModes
{
	// Token: 0x02000AB8 RID: 2744
	public class GameMode : MonoBehaviour
	{
		// Token: 0x06004225 RID: 16933 RVA: 0x001319C8 File Offset: 0x0012FBC8
		private void Awake()
		{
			if (GameMode.instance.IsNull())
			{
				GameMode.instance = this;
				foreach (GorillaGameManager gorillaGameManager in base.gameObject.GetComponentsInChildren<GorillaGameManager>(true))
				{
					int num = (int)gorillaGameManager.GameType();
					string text = gorillaGameManager.GameTypeName();
					if (GameMode.gameModeTable.ContainsKey(num))
					{
						Debug.LogWarning("Duplicate gamemode type, skipping this instance", gorillaGameManager);
					}
					else
					{
						GameMode.gameModeTable.Add((int)gorillaGameManager.GameType(), gorillaGameManager);
						GameMode.gameModeKeyByName.Add(text, num);
						GameMode.gameModes.Add(gorillaGameManager);
						GameMode.gameModeNames.Add(text);
					}
				}
				return;
			}
			Object.Destroy(this);
		}

		// Token: 0x06004226 RID: 16934 RVA: 0x00131A6D File Offset: 0x0012FC6D
		private void OnDestroy()
		{
			if (GameMode.instance == this)
			{
				GameMode.instance = null;
			}
		}

		// Token: 0x14000078 RID: 120
		// (add) Token: 0x06004227 RID: 16935 RVA: 0x00131A84 File Offset: 0x0012FC84
		// (remove) Token: 0x06004228 RID: 16936 RVA: 0x00131AB8 File Offset: 0x0012FCB8
		public static event GameMode.OnStartGameModeAction OnStartGameMode;

		// Token: 0x1700069A RID: 1690
		// (get) Token: 0x06004229 RID: 16937 RVA: 0x00131AEB File Offset: 0x0012FCEB
		public static GorillaGameManager ActiveGameMode
		{
			get
			{
				return GameMode.activeGameMode;
			}
		}

		// Token: 0x1700069B RID: 1691
		// (get) Token: 0x0600422A RID: 16938 RVA: 0x00131AF2 File Offset: 0x0012FCF2
		internal static GameModeSerializer ActiveNetworkHandler
		{
			get
			{
				return GameMode.activeNetworkHandler;
			}
		}

		// Token: 0x1700069C RID: 1692
		// (get) Token: 0x0600422B RID: 16939 RVA: 0x00131AF9 File Offset: 0x0012FCF9
		public static GameModeZoneMapping GameModeZoneMapping
		{
			get
			{
				return GameMode.instance.gameModeZoneMapping;
			}
		}

		// Token: 0x0600422C RID: 16940 RVA: 0x00131B08 File Offset: 0x0012FD08
		static GameMode()
		{
			GameMode.StaticLoad();
		}

		// Token: 0x0600422D RID: 16941 RVA: 0x00131B8C File Offset: 0x0012FD8C
		[OnEnterPlay_Run]
		private static void StaticLoad()
		{
			RoomSystem.LeftRoomEvent = (Action)Delegate.Combine(RoomSystem.LeftRoomEvent, new Action(GameMode.ResetGameModes));
			RoomSystem.JoinedRoomEvent = (Action)Delegate.Combine(RoomSystem.JoinedRoomEvent, new Action(GameMode.RefreshPlayers));
			RoomSystem.PlayersChangedEvent = (Action)Delegate.Combine(RoomSystem.PlayersChangedEvent, new Action(GameMode.RefreshPlayers));
		}

		// Token: 0x0600422E RID: 16942 RVA: 0x00131BF9 File Offset: 0x0012FDF9
		internal static bool LoadGameModeFromProperty()
		{
			return GameMode.LoadGameMode(GameMode.FindGameModeFromRoomProperty());
		}

		// Token: 0x0600422F RID: 16943 RVA: 0x00131C05 File Offset: 0x0012FE05
		internal static bool ChangeGameFromProperty()
		{
			return GameMode.ChangeGameMode(GameMode.FindGameModeFromRoomProperty());
		}

		// Token: 0x06004230 RID: 16944 RVA: 0x00131C11 File Offset: 0x0012FE11
		internal static bool LoadGameModeFromProperty(string prop)
		{
			return GameMode.LoadGameMode(GameMode.FindGameModeInString(prop));
		}

		// Token: 0x06004231 RID: 16945 RVA: 0x00131C1E File Offset: 0x0012FE1E
		internal static bool ChangeGameFromProperty(string prop)
		{
			return GameMode.ChangeGameMode(GameMode.FindGameModeInString(prop));
		}

		// Token: 0x06004232 RID: 16946 RVA: 0x00131C2C File Offset: 0x0012FE2C
		public static int GetGameModeKeyFromRoomProp()
		{
			string text = GameMode.FindGameModeFromRoomProperty();
			int num;
			if (string.IsNullOrEmpty(text) || !GameMode.gameModeKeyByName.TryGetValue(text, out num))
			{
				GTDev.LogWarning<string>("Unable to find game mode key for " + text, null);
				return -1;
			}
			return num;
		}

		// Token: 0x06004233 RID: 16947 RVA: 0x00131C6A File Offset: 0x0012FE6A
		private static string FindGameModeFromRoomProperty()
		{
			if (!NetworkSystem.Instance.InRoom || string.IsNullOrEmpty(NetworkSystem.Instance.GameModeString))
			{
				return null;
			}
			return GameMode.FindGameModeInString(NetworkSystem.Instance.GameModeString);
		}

		// Token: 0x06004234 RID: 16948 RVA: 0x00131C9A File Offset: 0x0012FE9A
		public static bool IsValidGameMode(string gameMode)
		{
			return !string.IsNullOrEmpty(gameMode) && GameMode.gameModeKeyByName.ContainsKey(gameMode);
		}

		// Token: 0x06004235 RID: 16949 RVA: 0x00131CB4 File Offset: 0x0012FEB4
		private static string FindGameModeInString(string gmString)
		{
			for (int i = 0; i < GameMode.gameModes.Count; i++)
			{
				string text = GameMode.gameModes[i].GameTypeName();
				if (gmString.EndsWith(text))
				{
					return text;
				}
			}
			return null;
		}

		// Token: 0x06004236 RID: 16950 RVA: 0x00131CF4 File Offset: 0x0012FEF4
		public static bool LoadGameMode(string gameMode)
		{
			if (gameMode == null)
			{
				Debug.LogError("GAME MODE NULL");
				return false;
			}
			int num;
			if (!GameMode.gameModeKeyByName.TryGetValue(gameMode, out num))
			{
				Debug.LogWarning("Unable to find game mode key for " + gameMode);
				return false;
			}
			return GameMode.LoadGameMode(num);
		}

		// Token: 0x06004237 RID: 16951 RVA: 0x00131D38 File Offset: 0x0012FF38
		public static bool LoadGameMode(int key)
		{
			foreach (KeyValuePair<int, GorillaGameManager> keyValuePair in GameMode.gameModeTable)
			{
			}
			if (!GameMode.gameModeTable.ContainsKey(key))
			{
				Debug.LogWarning("Missing game mode for key " + key.ToString());
				return false;
			}
			PrefabType prefabType;
			VRRigCache.Instance.GetComponent<PhotonPrefabPool>().networkPrefabs.TryGetValue("GameMode", out prefabType);
			GameObject prefab = prefabType.prefab;
			if (prefab == null)
			{
				GTDev.LogError<string>("Unable to find game mode prefab to spawn", null);
				return false;
			}
			if (NetworkSystem.Instance.NetInstantiate(prefab, Vector3.zero, Quaternion.identity, true, 0, new object[] { key }, delegate(NetworkRunner runner, NetworkObject no)
			{
				no.GetComponent<GameModeSerializer>().Init(key);
			}).IsNull())
			{
				GTDev.LogWarning<string>("Unable to create GameManager with key " + key.ToString(), null);
				return false;
			}
			return true;
		}

		// Token: 0x06004238 RID: 16952 RVA: 0x00131E54 File Offset: 0x00130054
		internal static bool ChangeGameMode(string gameMode)
		{
			if (gameMode == null)
			{
				return false;
			}
			int num;
			if (!GameMode.gameModeKeyByName.TryGetValue(gameMode, out num))
			{
				Debug.LogWarning("Unable to find game mode key for " + gameMode);
				return false;
			}
			return GameMode.ChangeGameMode(num);
		}

		// Token: 0x06004239 RID: 16953 RVA: 0x00131E90 File Offset: 0x00130090
		internal static bool ChangeGameMode(int key)
		{
			GorillaGameManager gorillaGameManager;
			if (!NetworkSystem.Instance.IsMasterClient || !GameMode.gameModeTable.TryGetValue(key, out gorillaGameManager) || gorillaGameManager == GameMode.activeGameMode)
			{
				return false;
			}
			if (GameMode.activeNetworkHandler.IsNotNull())
			{
				NetworkSystem.Instance.NetDestroy(GameMode.activeNetworkHandler.gameObject);
			}
			GameMode.StopGameModeSafe(GameMode.activeGameMode);
			GameMode.activeGameMode = null;
			GameMode.activeNetworkHandler = null;
			return GameMode.LoadGameMode(key);
		}

		// Token: 0x0600423A RID: 16954 RVA: 0x00131F04 File Offset: 0x00130104
		internal static void SetupGameModeRemote(GameModeSerializer networkSerializer)
		{
			GorillaGameManager gameModeInstance = networkSerializer.GameModeInstance;
			if (GameMode.activeGameMode.IsNotNull() && gameModeInstance.IsNotNull() && gameModeInstance != GameMode.activeGameMode)
			{
				GameMode.StopGameModeSafe(GameMode.activeGameMode);
			}
			GameMode.activeNetworkHandler = networkSerializer;
			GameMode.activeGameMode = gameModeInstance;
			GameMode.activeGameMode.NetworkLinkSetup(networkSerializer);
			GameMode.StartGameModeSafe(GameMode.activeGameMode);
			if (GameMode.OnStartGameMode != null)
			{
				GameMode.OnStartGameMode(GameMode.activeGameMode.GameType());
			}
			if (!GameMode.activatedGameModes.Contains(GameMode.activeGameMode))
			{
				GameMode.activatedGameModes.Add(GameMode.activeGameMode);
			}
		}

		// Token: 0x0600423B RID: 16955 RVA: 0x00131F9F File Offset: 0x0013019F
		internal static void RemoveNetworkLink(GameModeSerializer networkSerializer)
		{
			if (GameMode.activeGameMode.IsNotNull() && networkSerializer == GameMode.activeNetworkHandler)
			{
				GameMode.activeGameMode.NetworkLinkDestroyed(networkSerializer);
				GameMode.activeNetworkHandler = null;
				return;
			}
		}

		// Token: 0x0600423C RID: 16956 RVA: 0x00131FCC File Offset: 0x001301CC
		public static GorillaGameManager GetGameModeInstance(GameModeType type)
		{
			return GameMode.GetGameModeInstance((int)type);
		}

		// Token: 0x0600423D RID: 16957 RVA: 0x00131FD4 File Offset: 0x001301D4
		public static GorillaGameManager GetGameModeInstance(int type)
		{
			GorillaGameManager gorillaGameManager;
			if (GameMode.gameModeTable.TryGetValue(type, out gorillaGameManager))
			{
				if (gorillaGameManager == null)
				{
					Debug.LogError("Couldnt get mode from table");
					foreach (KeyValuePair<int, GorillaGameManager> keyValuePair in GameMode.gameModeTable)
					{
					}
				}
				return gorillaGameManager;
			}
			return null;
		}

		// Token: 0x0600423E RID: 16958 RVA: 0x00132044 File Offset: 0x00130244
		public static T GetGameModeInstance<T>(GameModeType type) where T : GorillaGameManager
		{
			return GameMode.GetGameModeInstance<T>((int)type);
		}

		// Token: 0x0600423F RID: 16959 RVA: 0x0013204C File Offset: 0x0013024C
		public static T GetGameModeInstance<T>(int type) where T : GorillaGameManager
		{
			T t = GameMode.GetGameModeInstance(type) as T;
			if (t != null)
			{
				return t;
			}
			return default(T);
		}

		// Token: 0x06004240 RID: 16960 RVA: 0x00132080 File Offset: 0x00130280
		public static void ResetGameModes()
		{
			GameMode.activeGameMode = null;
			GameMode.activeNetworkHandler = null;
			GameMode.optOutPlayers.Clear();
			GameMode.ParticipatingPlayers.Clear();
			for (int i = 0; i < GameMode.activatedGameModes.Count; i++)
			{
				GorillaGameManager gorillaGameManager = GameMode.activatedGameModes[i];
				GameMode.StopGameModeSafe(gorillaGameManager);
				GameMode.ResetGameModeSafe(gorillaGameManager);
			}
			GameMode.activatedGameModes.Clear();
		}

		// Token: 0x06004241 RID: 16961 RVA: 0x001320E4 File Offset: 0x001302E4
		private static void StartGameModeSafe(GorillaGameManager gameMode)
		{
			try
			{
				gameMode.StartPlaying();
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x06004242 RID: 16962 RVA: 0x0013210C File Offset: 0x0013030C
		private static void StopGameModeSafe(GorillaGameManager gameMode)
		{
			try
			{
				gameMode.StopPlaying();
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x06004243 RID: 16963 RVA: 0x00132134 File Offset: 0x00130334
		private static void ResetGameModeSafe(GorillaGameManager gameMode)
		{
			try
			{
				gameMode.Reset();
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x06004244 RID: 16964 RVA: 0x0013215C File Offset: 0x0013035C
		public static void ReportTag(NetPlayer player)
		{
			if (NetworkSystem.Instance.InRoom && GameMode.activeNetworkHandler.IsNotNull())
			{
				GameMode.activeNetworkHandler.SendRPC("RPC_ReportTag", false, new object[] { player.ActorNumber });
			}
		}

		// Token: 0x06004245 RID: 16965 RVA: 0x0013219C File Offset: 0x0013039C
		public static void ReportHit()
		{
			if (GorillaGameManager.instance.GameType() == GameModeType.Custom)
			{
				CustomGameMode.TaggedByEnvironment();
			}
			if (NetworkSystem.Instance.InRoom && GameMode.activeNetworkHandler.IsNotNull())
			{
				GameMode.activeNetworkHandler.SendRPC("RPC_ReportHit", false, Array.Empty<object>());
			}
		}

		// Token: 0x06004246 RID: 16966 RVA: 0x001321E8 File Offset: 0x001303E8
		public static void BroadcastRoundComplete()
		{
			if (NetworkSystem.Instance.IsMasterClient && NetworkSystem.Instance.InRoom && GameMode.activeNetworkHandler.IsNotNull())
			{
				GameMode.activeNetworkHandler.SendRPC("RPC_BroadcastRoundComplete", true, Array.Empty<object>());
			}
		}

		// Token: 0x06004247 RID: 16967 RVA: 0x00132224 File Offset: 0x00130424
		public static void RefreshPlayers()
		{
			List<NetPlayer> playersInRoom = RoomSystem.PlayersInRoom;
			int num = Mathf.Min(playersInRoom.Count, 10);
			GameMode.ParticipatingPlayers.Clear();
			for (int i = 0; i < num; i++)
			{
				if (GameMode.CanParticipate(playersInRoom[i]))
				{
					GameMode.ParticipatingPlayers.Add(playersInRoom[i]);
				}
			}
		}

		// Token: 0x06004248 RID: 16968 RVA: 0x0013227A File Offset: 0x0013047A
		public static void OptOut(VRRig rig)
		{
			GameMode.OptOut(rig.creator.ActorNumber);
		}

		// Token: 0x06004249 RID: 16969 RVA: 0x0013228C File Offset: 0x0013048C
		public static void OptOut(NetPlayer player)
		{
			GameMode.OptOut(player.ActorNumber);
		}

		// Token: 0x0600424A RID: 16970 RVA: 0x00132299 File Offset: 0x00130499
		public static void OptOut(int playerActorNumber)
		{
			if (GameMode.optOutPlayers.Add(playerActorNumber))
			{
				GameMode.RefreshPlayers();
			}
		}

		// Token: 0x0600424B RID: 16971 RVA: 0x001322AD File Offset: 0x001304AD
		public static void OptIn(VRRig rig)
		{
			GameMode.OptIn(rig.creator.ActorNumber);
		}

		// Token: 0x0600424C RID: 16972 RVA: 0x001322BF File Offset: 0x001304BF
		public static void OptIn(NetPlayer player)
		{
			GameMode.OptIn(player.ActorNumber);
		}

		// Token: 0x0600424D RID: 16973 RVA: 0x001322CC File Offset: 0x001304CC
		public static void OptIn(int playerActorNumber)
		{
			if (GameMode.optOutPlayers.Remove(playerActorNumber))
			{
				GameMode.RefreshPlayers();
			}
		}

		// Token: 0x0600424E RID: 16974 RVA: 0x001322E0 File Offset: 0x001304E0
		private static bool CanParticipate(NetPlayer player)
		{
			return player.InRoom() && !GameMode.optOutPlayers.Contains(player.ActorNumber) && NetworkSystem.Instance.GetPlayerTutorialCompletion(player.ActorNumber);
		}

		// Token: 0x0400449C RID: 17564
		[SerializeField]
		private GameModeZoneMapping gameModeZoneMapping;

		// Token: 0x0400449E RID: 17566
		[OnEnterPlay_SetNull]
		private static GameMode instance;

		// Token: 0x0400449F RID: 17567
		[OnEnterPlay_Clear]
		private static Dictionary<int, GorillaGameManager> gameModeTable = new Dictionary<int, GorillaGameManager>();

		// Token: 0x040044A0 RID: 17568
		[OnEnterPlay_Clear]
		private static Dictionary<string, int> gameModeKeyByName = new Dictionary<string, int>();

		// Token: 0x040044A1 RID: 17569
		[OnEnterPlay_Clear]
		private static Dictionary<int, FusionGameModeData> fusionTypeTable = new Dictionary<int, FusionGameModeData>();

		// Token: 0x040044A2 RID: 17570
		[OnEnterPlay_Clear]
		private static List<GorillaGameManager> gameModes = new List<GorillaGameManager>(10);

		// Token: 0x040044A3 RID: 17571
		[OnEnterPlay_Clear]
		public static readonly List<string> gameModeNames = new List<string>(10);

		// Token: 0x040044A4 RID: 17572
		[OnEnterPlay_Clear]
		private static readonly List<GorillaGameManager> activatedGameModes = new List<GorillaGameManager>(9);

		// Token: 0x040044A5 RID: 17573
		[OnEnterPlay_SetNull]
		private static GorillaGameManager activeGameMode = null;

		// Token: 0x040044A6 RID: 17574
		[OnEnterPlay_SetNull]
		private static GameModeSerializer activeNetworkHandler = null;

		// Token: 0x040044A7 RID: 17575
		private static List<Player> participatingPlayers = new List<Player>(10);

		// Token: 0x040044A8 RID: 17576
		[OnEnterPlay_Clear]
		private static readonly HashSet<int> optOutPlayers = new HashSet<int>(10);

		// Token: 0x040044A9 RID: 17577
		[OnEnterPlay_Clear]
		public static readonly List<NetPlayer> ParticipatingPlayers = new List<NetPlayer>(10);

		// Token: 0x02000AB9 RID: 2745
		// (Invoke) Token: 0x06004251 RID: 16977
		public delegate void OnStartGameModeAction(GameModeType newGameModeType);
	}
}
