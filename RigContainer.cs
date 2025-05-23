using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GorillaNetworking;
using GorillaTag.Audio;
using Newtonsoft.Json;
using Photon.Voice.PUN;
using PlayFab;
using PlayFab.CloudScriptModels;
using UnityEngine;

// Token: 0x020005E5 RID: 1509
[RequireComponent(typeof(VRRig), typeof(VRRigReliableState))]
public class RigContainer : MonoBehaviour
{
	// Token: 0x17000379 RID: 889
	// (get) Token: 0x060024E5 RID: 9445 RVA: 0x000B8D0D File Offset: 0x000B6F0D
	// (set) Token: 0x060024E6 RID: 9446 RVA: 0x000B8D15 File Offset: 0x000B6F15
	public bool Initialized { get; private set; }

	// Token: 0x1700037A RID: 890
	// (get) Token: 0x060024E7 RID: 9447 RVA: 0x000B8D1E File Offset: 0x000B6F1E
	public VRRig Rig
	{
		get
		{
			return this.vrrig;
		}
	}

	// Token: 0x1700037B RID: 891
	// (get) Token: 0x060024E8 RID: 9448 RVA: 0x000B8D26 File Offset: 0x000B6F26
	public VRRigReliableState ReliableState
	{
		get
		{
			return this.reliableState;
		}
	}

	// Token: 0x1700037C RID: 892
	// (get) Token: 0x060024E9 RID: 9449 RVA: 0x000B8D2E File Offset: 0x000B6F2E
	public Transform SpeakerHead
	{
		get
		{
			return this.speakerHead;
		}
	}

	// Token: 0x1700037D RID: 893
	// (get) Token: 0x060024EA RID: 9450 RVA: 0x000B8D36 File Offset: 0x000B6F36
	public AudioSource ReplacementVoiceSource
	{
		get
		{
			return this.replacementVoiceSource;
		}
	}

	// Token: 0x1700037E RID: 894
	// (get) Token: 0x060024EB RID: 9451 RVA: 0x000B8D3E File Offset: 0x000B6F3E
	public List<LoudSpeakerNetwork> LoudSpeakerNetworks
	{
		get
		{
			return this.loudSpeakerNetworks;
		}
	}

	// Token: 0x1700037F RID: 895
	// (get) Token: 0x060024EC RID: 9452 RVA: 0x000B8D46 File Offset: 0x000B6F46
	// (set) Token: 0x060024ED RID: 9453 RVA: 0x000B8D4E File Offset: 0x000B6F4E
	public PhotonVoiceView Voice
	{
		get
		{
			return this.voiceView;
		}
		set
		{
			if (value == this.voiceView)
			{
				return;
			}
			if (this.voiceView != null)
			{
				this.voiceView.SpeakerInUse.enabled = false;
			}
			this.voiceView = value;
			this.RefreshVoiceChat();
		}
	}

	// Token: 0x17000380 RID: 896
	// (get) Token: 0x060024EE RID: 9454 RVA: 0x000B8D8B File Offset: 0x000B6F8B
	public NetworkView netView
	{
		get
		{
			return this.vrrig.netView;
		}
	}

	// Token: 0x17000381 RID: 897
	// (get) Token: 0x060024EF RID: 9455 RVA: 0x000B8D98 File Offset: 0x000B6F98
	public int CachedNetViewID
	{
		get
		{
			return this.m_cachedNetViewID;
		}
	}

	// Token: 0x17000382 RID: 898
	// (get) Token: 0x060024F0 RID: 9456 RVA: 0x000B8DA0 File Offset: 0x000B6FA0
	// (set) Token: 0x060024F1 RID: 9457 RVA: 0x000B8DAB File Offset: 0x000B6FAB
	public bool Muted
	{
		get
		{
			return !this.enableVoice;
		}
		set
		{
			this.enableVoice = !value;
			this.RefreshVoiceChat();
		}
	}

	// Token: 0x17000383 RID: 899
	// (get) Token: 0x060024F2 RID: 9458 RVA: 0x000B8DBD File Offset: 0x000B6FBD
	// (set) Token: 0x060024F3 RID: 9459 RVA: 0x000B8DCC File Offset: 0x000B6FCC
	public NetPlayer Creator
	{
		get
		{
			return this.vrrig.creator;
		}
		set
		{
			if (this.vrrig.isOfflineVRRig || (this.vrrig.creator != null && this.vrrig.creator.InRoom))
			{
				return;
			}
			this.vrrig.creator = value;
			this.vrrig.UpdateName();
		}
	}

	// Token: 0x17000384 RID: 900
	// (get) Token: 0x060024F4 RID: 9460 RVA: 0x000B8E1D File Offset: 0x000B701D
	// (set) Token: 0x060024F5 RID: 9461 RVA: 0x000B8E25 File Offset: 0x000B7025
	public bool ForceMute
	{
		get
		{
			return this.forceMute;
		}
		set
		{
			this.forceMute = value;
			this.RefreshVoiceChat();
		}
	}

	// Token: 0x17000385 RID: 901
	// (get) Token: 0x060024F6 RID: 9462 RVA: 0x000B8E34 File Offset: 0x000B7034
	public SphereCollider HeadCollider
	{
		get
		{
			return this.headCollider;
		}
	}

	// Token: 0x17000386 RID: 902
	// (get) Token: 0x060024F7 RID: 9463 RVA: 0x000B8E3C File Offset: 0x000B703C
	public CapsuleCollider BodyCollider
	{
		get
		{
			return this.bodyCollider;
		}
	}

	// Token: 0x17000387 RID: 903
	// (get) Token: 0x060024F8 RID: 9464 RVA: 0x000B8E44 File Offset: 0x000B7044
	public VRRigEvents RigEvents
	{
		get
		{
			return this.rigEvents;
		}
	}

	// Token: 0x060024F9 RID: 9465 RVA: 0x000B8E4C File Offset: 0x000B704C
	public bool GetIsPlayerAutoMuted()
	{
		return this.bPlayerAutoMuted;
	}

	// Token: 0x060024FA RID: 9466 RVA: 0x000B8E54 File Offset: 0x000B7054
	public void UpdateAutomuteLevel(string autoMuteLevel)
	{
		if (autoMuteLevel.Equals("LOW", StringComparison.OrdinalIgnoreCase))
		{
			this.playerChatQuality = 1;
		}
		else if (autoMuteLevel.Equals("HIGH", StringComparison.OrdinalIgnoreCase))
		{
			this.playerChatQuality = 0;
		}
		else if (autoMuteLevel.Equals("ERROR", StringComparison.OrdinalIgnoreCase))
		{
			this.playerChatQuality = 2;
		}
		else
		{
			this.playerChatQuality = 2;
		}
		this.RefreshVoiceChat();
	}

	// Token: 0x060024FB RID: 9467 RVA: 0x000B8EB3 File Offset: 0x000B70B3
	private void Awake()
	{
		this.loudSpeakerNetworks = new List<LoudSpeakerNetwork>();
	}

	// Token: 0x060024FC RID: 9468 RVA: 0x000B8EC0 File Offset: 0x000B70C0
	private void Start()
	{
		if (this.Rig.isOfflineVRRig)
		{
			this.vrrig.creator = NetworkSystem.Instance.LocalPlayer;
			RoomSystem.JoinedRoomEvent = (Action)Delegate.Combine(RoomSystem.JoinedRoomEvent, new Action(this.OnMultiPlayerStarted));
			RoomSystem.LeftRoomEvent = (Action)Delegate.Combine(RoomSystem.LeftRoomEvent, new Action(this.OnReturnedToSinglePlayer));
		}
		this.Rig.rigContainer = this;
	}

	// Token: 0x060024FD RID: 9469 RVA: 0x000B8F3B File Offset: 0x000B713B
	private void OnMultiPlayerStarted()
	{
		if (this.Rig.isOfflineVRRig)
		{
			this.vrrig.creator = NetworkSystem.Instance.GetLocalPlayer();
		}
	}

	// Token: 0x060024FE RID: 9470 RVA: 0x000B8F5F File Offset: 0x000B715F
	private void OnReturnedToSinglePlayer()
	{
		if (this.Rig.isOfflineVRRig)
		{
			RigContainer.CancelAutomuteRequest();
		}
	}

	// Token: 0x060024FF RID: 9471 RVA: 0x000B8F74 File Offset: 0x000B7174
	private void OnDisable()
	{
		this.Initialized = false;
		this.enableVoice = true;
		this.voiceView = null;
		base.gameObject.transform.localPosition = Vector3.zero;
		base.gameObject.transform.localRotation = Quaternion.identity;
		this.vrrig.syncPos = base.gameObject.transform.position;
		this.vrrig.syncRotation = base.gameObject.transform.rotation;
		this.forceMute = false;
	}

	// Token: 0x06002500 RID: 9472 RVA: 0x000B8FFD File Offset: 0x000B71FD
	internal void InitializeNetwork(NetworkView netView, PhotonVoiceView voiceView, VRRigSerializer vrRigSerializer)
	{
		if (!netView || !voiceView)
		{
			return;
		}
		this.InitializeNetwork_Shared(netView, vrRigSerializer);
		this.Voice = voiceView;
		this.vrrig.voiceAudio = voiceView.SpeakerInUse.GetComponent<AudioSource>();
	}

	// Token: 0x06002501 RID: 9473 RVA: 0x000B9038 File Offset: 0x000B7238
	private void InitializeNetwork_Shared(NetworkView netView, VRRigSerializer vrRigSerializer)
	{
		if (this.vrrig.netView)
		{
			GorillaNot.instance.SendReport("inappropriate tag data being sent creating multiple vrrigs", this.Creator.UserId, this.Creator.NickName);
			if (this.vrrig.netView.IsMine)
			{
				NetworkSystem.Instance.NetDestroy(this.vrrig.gameObject);
			}
			else
			{
				this.vrrig.netView.gameObject.SetActive(false);
			}
		}
		this.vrrig.netView = netView;
		this.vrrig.rigSerializer = vrRigSerializer;
		this.vrrig.OwningNetPlayer = NetworkSystem.Instance.GetPlayer(NetworkSystem.Instance.GetOwningPlayerID(vrRigSerializer.gameObject));
		this.m_cachedNetViewID = netView.ViewID;
		if (!this.Initialized)
		{
			this.vrrig.NetInitialize();
			if (GorillaGameManager.instance != null && NetworkSystem.Instance.IsMasterClient)
			{
				int owningPlayerID = NetworkSystem.Instance.GetOwningPlayerID(vrRigSerializer.gameObject);
				bool playerTutorialCompletion = NetworkSystem.Instance.GetPlayerTutorialCompletion(owningPlayerID);
				GorillaGameManager.instance.NewVRRig(netView.Owner, netView.ViewID, playerTutorialCompletion);
			}
			if (!this.vrrig.OwningNetPlayer.IsLocal)
			{
				netView.SendRPC("RPC_RequestQuestScore", netView.Owner, Array.Empty<object>());
			}
			if (this.vrrig.InitializedCosmetics)
			{
				netView.SendRPC("RPC_RequestCosmetics", netView.Owner, Array.Empty<object>());
			}
		}
		this.Initialized = true;
		if (!this.vrrig.isOfflineVRRig)
		{
			base.StartCoroutine(RigContainer.QueueAutomute(this.Creator));
		}
	}

	// Token: 0x06002502 RID: 9474 RVA: 0x000B91DA File Offset: 0x000B73DA
	private static IEnumerator QueueAutomute(NetPlayer player)
	{
		RigContainer.playersToCheckAutomute.Add(player);
		if (!RigContainer.automuteQueued)
		{
			RigContainer.automuteQueued = true;
			yield return new WaitForSecondsRealtime(1f);
			while (RigContainer.waitingForAutomuteCallback)
			{
				yield return null;
			}
			RigContainer.automuteQueued = false;
			RigContainer.RequestAutomuteSettings();
		}
		yield break;
	}

	// Token: 0x06002503 RID: 9475 RVA: 0x000B91EC File Offset: 0x000B73EC
	private static void RequestAutomuteSettings()
	{
		if (RigContainer.playersToCheckAutomute.Count == 0)
		{
			return;
		}
		RigContainer.waitingForAutomuteCallback = true;
		RigContainer.playersToCheckAutomute.RemoveAll((NetPlayer player) => player == null);
		RigContainer.requestedAutomutePlayers = new List<NetPlayer>(RigContainer.playersToCheckAutomute);
		RigContainer.playersToCheckAutomute.Clear();
		string[] array = RigContainer.requestedAutomutePlayers.Select((NetPlayer x) => x.UserId).ToArray<string>();
		foreach (NetPlayer netPlayer in RigContainer.requestedAutomutePlayers)
		{
		}
		ExecuteFunctionRequest executeFunctionRequest = new ExecuteFunctionRequest();
		executeFunctionRequest.Entity = new EntityKey
		{
			Id = PlayFabSettings.staticPlayer.EntityId,
			Type = PlayFabSettings.staticPlayer.EntityType
		};
		executeFunctionRequest.FunctionName = "ShouldUserAutomutePlayer";
		executeFunctionRequest.FunctionParameter = string.Join(",", array);
		PlayFabCloudScriptAPI.ExecuteFunction(executeFunctionRequest, delegate(ExecuteFunctionResult result)
		{
			Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(result.FunctionResult.ToString());
			if (dictionary == null)
			{
				using (List<NetPlayer>.Enumerator enumerator2 = RigContainer.requestedAutomutePlayers.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						NetPlayer netPlayer2 = enumerator2.Current;
						if (netPlayer2 != null)
						{
							RigContainer.ReceiveAutomuteSettings(netPlayer2, "none");
						}
					}
					goto IL_00A6;
				}
			}
			foreach (NetPlayer netPlayer3 in RigContainer.requestedAutomutePlayers)
			{
				if (netPlayer3 != null)
				{
					string text;
					if (dictionary.TryGetValue(netPlayer3.UserId, out text))
					{
						RigContainer.ReceiveAutomuteSettings(netPlayer3, text);
					}
					else
					{
						RigContainer.ReceiveAutomuteSettings(netPlayer3, "none");
					}
				}
			}
			IL_00A6:
			RigContainer.requestedAutomutePlayers.Clear();
			RigContainer.waitingForAutomuteCallback = false;
		}, delegate(PlayFabError error)
		{
			foreach (NetPlayer netPlayer4 in RigContainer.requestedAutomutePlayers)
			{
				RigContainer.ReceiveAutomuteSettings(netPlayer4, "ERROR");
			}
			RigContainer.requestedAutomutePlayers.Clear();
			RigContainer.waitingForAutomuteCallback = false;
		}, null, null);
	}

	// Token: 0x06002504 RID: 9476 RVA: 0x000B9350 File Offset: 0x000B7550
	private static void CancelAutomuteRequest()
	{
		RigContainer.playersToCheckAutomute.Clear();
		RigContainer.automuteQueued = false;
		if (RigContainer.requestedAutomutePlayers != null)
		{
			RigContainer.requestedAutomutePlayers.Clear();
		}
		RigContainer.waitingForAutomuteCallback = false;
	}

	// Token: 0x06002505 RID: 9477 RVA: 0x000B937C File Offset: 0x000B757C
	private static void ReceiveAutomuteSettings(NetPlayer player, string score)
	{
		RigContainer rigContainer;
		VRRigCache.Instance.TryGetVrrig(player, out rigContainer);
		if (rigContainer != null)
		{
			rigContainer.UpdateAutomuteLevel(score);
		}
	}

	// Token: 0x06002506 RID: 9478 RVA: 0x000B93A8 File Offset: 0x000B75A8
	private void ProcessAutomute()
	{
		int @int = PlayerPrefs.GetInt("autoMute", 1);
		this.bPlayerAutoMuted = !this.hasManualMute && this.playerChatQuality < @int;
	}

	// Token: 0x06002507 RID: 9479 RVA: 0x000B93DC File Offset: 0x000B75DC
	public void RefreshVoiceChat()
	{
		if (this.Voice == null)
		{
			return;
		}
		this.ProcessAutomute();
		this.Voice.SpeakerInUse.enabled = !this.forceMute && this.enableVoice && !this.bPlayerAutoMuted && GorillaComputer.instance.voiceChatOn == "TRUE";
		this.replacementVoiceSource.mute = this.forceMute || !this.enableVoice || this.bPlayerAutoMuted || GorillaComputer.instance.voiceChatOn == "OFF";
	}

	// Token: 0x06002508 RID: 9480 RVA: 0x000B947B File Offset: 0x000B767B
	public void AddLoudSpeakerNetwork(LoudSpeakerNetwork network)
	{
		if (this.loudSpeakerNetworks.Contains(network))
		{
			return;
		}
		this.loudSpeakerNetworks.Add(network);
	}

	// Token: 0x06002509 RID: 9481 RVA: 0x000B9498 File Offset: 0x000B7698
	public void RemoveLoudSpeakerNetwork(LoudSpeakerNetwork network)
	{
		this.loudSpeakerNetworks.Remove(network);
	}

	// Token: 0x0600250A RID: 9482 RVA: 0x000B94A8 File Offset: 0x000B76A8
	public static void RefreshAllRigVoices()
	{
		RigContainer.staticTempRC = null;
		if (!NetworkSystem.Instance.InRoom || VRRigCache.Instance == null)
		{
			return;
		}
		foreach (NetPlayer netPlayer in NetworkSystem.Instance.AllNetPlayers)
		{
			if (VRRigCache.Instance.TryGetVrrig(netPlayer, out RigContainer.staticTempRC))
			{
				RigContainer.staticTempRC.RefreshVoiceChat();
			}
		}
	}

	// Token: 0x040029D1 RID: 10705
	[SerializeField]
	private VRRig vrrig;

	// Token: 0x040029D2 RID: 10706
	[SerializeField]
	private VRRigReliableState reliableState;

	// Token: 0x040029D3 RID: 10707
	[SerializeField]
	private Transform speakerHead;

	// Token: 0x040029D4 RID: 10708
	[SerializeField]
	private AudioSource replacementVoiceSource;

	// Token: 0x040029D5 RID: 10709
	private List<LoudSpeakerNetwork> loudSpeakerNetworks;

	// Token: 0x040029D6 RID: 10710
	private PhotonVoiceView voiceView;

	// Token: 0x040029D7 RID: 10711
	private int m_cachedNetViewID;

	// Token: 0x040029D8 RID: 10712
	private bool enableVoice = true;

	// Token: 0x040029D9 RID: 10713
	private bool forceMute;

	// Token: 0x040029DA RID: 10714
	[SerializeField]
	private SphereCollider headCollider;

	// Token: 0x040029DB RID: 10715
	[SerializeField]
	private CapsuleCollider bodyCollider;

	// Token: 0x040029DC RID: 10716
	[SerializeField]
	private VRRigEvents rigEvents;

	// Token: 0x040029DD RID: 10717
	public bool hasManualMute;

	// Token: 0x040029DE RID: 10718
	private bool bPlayerAutoMuted;

	// Token: 0x040029DF RID: 10719
	public int playerChatQuality = 2;

	// Token: 0x040029E0 RID: 10720
	private static List<NetPlayer> playersToCheckAutomute = new List<NetPlayer>();

	// Token: 0x040029E1 RID: 10721
	private static bool automuteQueued = false;

	// Token: 0x040029E2 RID: 10722
	private static List<NetPlayer> requestedAutomutePlayers;

	// Token: 0x040029E3 RID: 10723
	private static bool waitingForAutomuteCallback = false;

	// Token: 0x040029E4 RID: 10724
	private static RigContainer staticTempRC;
}
