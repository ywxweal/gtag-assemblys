using System;
using Fusion;
using GorillaExtensions;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaTag.Audio;
using GorillaTagScripts;
using Photon.Pun;
using Photon.Voice.Fusion;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine;

// Token: 0x020005F0 RID: 1520
[NetworkBehaviourWeaved(35)]
internal class VRRigSerializer : GorillaWrappedSerializer, IFXContextParems<HandTapArgs>, IFXContextParems<GeoSoundArg>
{
	// Token: 0x17000391 RID: 913
	// (get) Token: 0x06002559 RID: 9561 RVA: 0x000BA54D File Offset: 0x000B874D
	// (set) Token: 0x0600255A RID: 9562 RVA: 0x000BA577 File Offset: 0x000B8777
	[Networked]
	[NetworkedWeaved(0, 17)]
	public unsafe NetworkString<_16> nickName
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing VRRigSerializer.nickName. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(NetworkString<_16>*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing VRRigSerializer.nickName. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(NetworkString<_16>*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x17000392 RID: 914
	// (get) Token: 0x0600255B RID: 9563 RVA: 0x000BA5A2 File Offset: 0x000B87A2
	// (set) Token: 0x0600255C RID: 9564 RVA: 0x000BA5D0 File Offset: 0x000B87D0
	[Networked]
	[NetworkedWeaved(17, 17)]
	public unsafe NetworkString<_16> defaultName
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing VRRigSerializer.defaultName. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(NetworkString<_16>*)(this.Ptr + 17);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing VRRigSerializer.defaultName. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(NetworkString<_16>*)(this.Ptr + 17) = value;
		}
	}

	// Token: 0x17000393 RID: 915
	// (get) Token: 0x0600255D RID: 9565 RVA: 0x000BA5FF File Offset: 0x000B87FF
	// (set) Token: 0x0600255E RID: 9566 RVA: 0x000BA62D File Offset: 0x000B882D
	[Networked]
	[NetworkedWeaved(34, 1)]
	public bool tutorialComplete
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing VRRigSerializer.tutorialComplete. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ReadWriteUtilsForWeaver.ReadBoolean(this.Ptr + 34);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing VRRigSerializer.tutorialComplete. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.WriteBoolean(this.Ptr + 34, value);
		}
	}

	// Token: 0x17000394 RID: 916
	// (get) Token: 0x0600255F RID: 9567 RVA: 0x000BA65C File Offset: 0x000B885C
	private PhotonVoiceView Voice
	{
		get
		{
			return this.voiceView;
		}
	}

	// Token: 0x17000395 RID: 917
	// (get) Token: 0x06002560 RID: 9568 RVA: 0x000BA664 File Offset: 0x000B8864
	public VRRig VRRig
	{
		get
		{
			return this.vrrig;
		}
	}

	// Token: 0x17000396 RID: 918
	// (get) Token: 0x06002561 RID: 9569 RVA: 0x000BA66C File Offset: 0x000B886C
	public FXSystemSettings settings
	{
		get
		{
			return this.vrrig.fxSettings;
		}
	}

	// Token: 0x06002562 RID: 9570 RVA: 0x000BA67C File Offset: 0x000B887C
	protected override bool OnSpawnSetupCheck(PhotonMessageInfoWrapped wrappedInfo, out GameObject outTargetObject, out Type outTargetType)
	{
		outTargetObject = null;
		outTargetType = null;
		NetPlayer player = NetworkSystem.Instance.GetPlayer(wrappedInfo.senderID);
		if (this.netView.IsRoomView)
		{
			if (player != null)
			{
				GorillaNot.instance.SendReport("creating rigs as room objects", player.UserId, player.NickName);
			}
			return false;
		}
		if (NetworkSystem.Instance.IsObjectRoomObject(base.gameObject))
		{
			NetPlayer player2 = NetworkSystem.Instance.GetPlayer(wrappedInfo.senderID);
			if (player2 != null)
			{
				GorillaNot.instance.SendReport("creating rigs as room objects", player2.UserId, player2.NickName);
			}
			return false;
		}
		if (player != this.netView.Owner)
		{
			GorillaNot.instance.SendReport("creating rigs for someone else", player.UserId, player.NickName);
			return false;
		}
		if (VRRigCache.Instance.TryGetVrrig(player, out this.rigContainer))
		{
			outTargetObject = this.rigContainer.gameObject;
			outTargetType = typeof(VRRig);
			this.vrrig = this.rigContainer.Rig;
			return true;
		}
		return false;
	}

	// Token: 0x06002563 RID: 9571 RVA: 0x000BA784 File Offset: 0x000B8984
	protected override void OnSuccesfullySpawned(PhotonMessageInfoWrapped info)
	{
		bool initialized = this.rigContainer.Initialized;
		this.rigContainer.InitializeNetwork(this.netView, this.Voice, this);
		this.networkSpeaker.SetParent(this.rigContainer.SpeakerHead, false);
		base.transform.SetParent(VRRigCache.Instance.NetworkParent, true);
		this.SetupLoudSpeakerNetwork(this.rigContainer);
		this.netView.GetView.AddCallbackTarget(this);
		if (!initialized)
		{
			object[] instantiationData = info.punInfo.photonView.InstantiationData;
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			if (instantiationData != null && instantiationData.Length == 3)
			{
				object obj = instantiationData[0];
				if (obj is float)
				{
					float num4 = (float)obj;
					obj = instantiationData[1];
					if (obj is float)
					{
						float num5 = (float)obj;
						obj = instantiationData[2];
						if (obj is float)
						{
							float num6 = (float)obj;
							num = num4.ClampSafe(0f, 1f);
							num2 = num5.ClampSafe(0f, 1f);
							num3 = num6.ClampSafe(0f, 1f);
						}
					}
				}
			}
			this.vrrig.InitializeNoobMaterialLocal(num, num2, num3);
		}
		NetworkSystem.Instance.IsObjectLocallyOwned(base.gameObject);
	}

	// Token: 0x06002564 RID: 9572 RVA: 0x000023F4 File Offset: 0x000005F4
	protected override void OnFailedSpawn()
	{
	}

	// Token: 0x06002565 RID: 9573 RVA: 0x000BA8CC File Offset: 0x000B8ACC
	protected override void OnBeforeDespawn()
	{
		this.CleanUp(true);
	}

	// Token: 0x06002566 RID: 9574 RVA: 0x000BA8D8 File Offset: 0x000B8AD8
	private void CleanUp(bool netDestroy)
	{
		if (!this.successfullInstantiate)
		{
			return;
		}
		this.successfullInstantiate = false;
		if (this.vrrig != null)
		{
			if (!NetworkSystem.Instance.InRoom)
			{
				if (this.vrrig.isOfflineVRRig)
				{
					this.vrrig.ChangeMaterialLocal(0);
				}
			}
			else if (this.vrrig.isOfflineVRRig)
			{
				NetworkSystem.Instance.NetDestroy(base.gameObject);
			}
			if (this.vrrig.netView == this.netView)
			{
				this.vrrig.netView = null;
			}
			if (this.vrrig.rigSerializer == this)
			{
				this.vrrig.rigSerializer = null;
			}
		}
		if (this.networkSpeaker != null)
		{
			this.CleanupLoudSpeakerNetwork();
			if (netDestroy)
			{
				this.networkSpeaker.SetParent(base.transform, false);
			}
			else
			{
				this.networkSpeaker.SetParent(null);
			}
			this.networkSpeaker.gameObject.SetActive(false);
		}
		this.vrrig = null;
	}

	// Token: 0x06002567 RID: 9575 RVA: 0x000BA9CF File Offset: 0x000B8BCF
	private void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
		this.CleanUp(false);
	}

	// Token: 0x06002568 RID: 9576 RVA: 0x000BA9DE File Offset: 0x000B8BDE
	private void OnDestroy()
	{
		NetworkBehaviourUtils.InternalOnDestroy(this);
		if (this.networkSpeaker != null && this.networkSpeaker.parent != base.transform)
		{
			global::UnityEngine.Object.Destroy(this.networkSpeaker.gameObject);
		}
	}

	// Token: 0x06002569 RID: 9577 RVA: 0x000BAA1C File Offset: 0x000B8C1C
	[PunRPC]
	public void RPC_InitializeNoobMaterial(float red, float green, float blue, PhotonMessageInfo info)
	{
		this.InitializeNoobMaterialShared(red, green, blue, info);
	}

	// Token: 0x0600256A RID: 9578 RVA: 0x000BAA2E File Offset: 0x000B8C2E
	[PunRPC]
	public void RPC_RequestCosmetics(PhotonMessageInfo info)
	{
		this.RequestCosmeticsShared(info);
	}

	// Token: 0x0600256B RID: 9579 RVA: 0x000BAA3C File Offset: 0x000B8C3C
	[PunRPC]
	public void RPC_PlayDrum(int drumIndex, float drumVolume, PhotonMessageInfo info)
	{
		this.PlayDrumShared(drumIndex, drumVolume, info);
	}

	// Token: 0x0600256C RID: 9580 RVA: 0x000BAA4C File Offset: 0x000B8C4C
	[PunRPC]
	public void RPC_PlaySelfOnlyInstrument(int selfOnlyIndex, int noteIndex, float instrumentVol, PhotonMessageInfo info)
	{
		this.PlaySelfOnlyInstrumentShared(selfOnlyIndex, noteIndex, instrumentVol, info);
	}

	// Token: 0x0600256D RID: 9581 RVA: 0x000BAA5E File Offset: 0x000B8C5E
	[PunRPC]
	public void RPC_PlayHandTap(int soundIndex, bool isLeftHand, float tapVolume, PhotonMessageInfo info = default(PhotonMessageInfo))
	{
		this.PlayHandTapShared(soundIndex, isLeftHand, tapVolume, info);
	}

	// Token: 0x0600256E RID: 9582 RVA: 0x000BAA70 File Offset: 0x000B8C70
	public void RPC_UpdateNativeSize(float value, PhotonMessageInfo info = default(PhotonMessageInfo))
	{
		this.UpdateNativeSizeShared(value, info);
	}

	// Token: 0x0600256F RID: 9583 RVA: 0x000023F4 File Offset: 0x000005F4
	public void RPC_UpdateCosmetics(string[] currentItems, PhotonMessageInfo info)
	{
	}

	// Token: 0x06002570 RID: 9584 RVA: 0x000023F4 File Offset: 0x000005F4
	public void RPC_UpdateCosmeticsWithTryon(string[] currentItems, string[] tryOnItems, PhotonMessageInfo info)
	{
	}

	// Token: 0x06002571 RID: 9585 RVA: 0x000BAA7F File Offset: 0x000B8C7F
	[PunRPC]
	public void RPC_UpdateCosmeticsWithTryonPacked(int[] currentItemsPacked, int[] tryOnItemsPacked, PhotonMessageInfo info)
	{
		this.UpdateCosmeticsWithTryonShared(currentItemsPacked, tryOnItemsPacked, info);
	}

	// Token: 0x06002572 RID: 9586 RVA: 0x000BAA8F File Offset: 0x000B8C8F
	[PunRPC]
	public void RPC_HideAllCosmetics(PhotonMessageInfo info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.HideAllCosmetics(info);
	}

	// Token: 0x06002573 RID: 9587 RVA: 0x000BAAA2 File Offset: 0x000B8CA2
	[PunRPC]
	public void RPC_PlaySplashEffect(Vector3 splashPosition, Quaternion splashRotation, float splashScale, float boundingRadius, bool bigSplash, bool enteringWater, PhotonMessageInfo info)
	{
		this.PlaySplashEffectShared(splashPosition, splashRotation, splashScale, boundingRadius, bigSplash, enteringWater, info);
	}

	// Token: 0x06002574 RID: 9588 RVA: 0x000BAABA File Offset: 0x000B8CBA
	[PunRPC]
	public void RPC_PlayGeodeEffect(Vector3 hitPosition, PhotonMessageInfo info)
	{
		this.PlayGeodeEffectShared(hitPosition, info);
	}

	// Token: 0x06002575 RID: 9589 RVA: 0x000BAAC9 File Offset: 0x000B8CC9
	[PunRPC]
	public void EnableNonCosmeticHandItemRPC(bool enable, bool isLeftHand, PhotonMessageInfo info)
	{
		this.EnableNonCosmeticHandItemShared(enable, isLeftHand, info);
	}

	// Token: 0x06002576 RID: 9590 RVA: 0x000BAAD9 File Offset: 0x000B8CD9
	[PunRPC]
	public void OnHandTapRPC(int surfaceIndex, bool leftHanded, float handSpeed, long packedDir, PhotonMessageInfo info)
	{
		this.OnHandTapRPCShared(surfaceIndex, leftHanded, handSpeed, packedDir, info);
	}

	// Token: 0x06002577 RID: 9591 RVA: 0x000BAAED File Offset: 0x000B8CED
	[PunRPC]
	public void RPC_UpdateQuestScore(int score, PhotonMessageInfo info)
	{
		this.UpdateQuestScore(score, info);
	}

	// Token: 0x06002578 RID: 9592 RVA: 0x000BAAFC File Offset: 0x000B8CFC
	[PunRPC]
	public void RPC_RequestQuestScore(PhotonMessageInfo info)
	{
		this.RequestQuestScore(info);
	}

	// Token: 0x06002579 RID: 9593 RVA: 0x000BAB0C File Offset: 0x000B8D0C
	private void SetupLoudSpeakerNetwork(RigContainer rigContainer)
	{
		if (this.networkSpeaker == null)
		{
			return;
		}
		Speaker component = this.networkSpeaker.GetComponent<Speaker>();
		if (component == null)
		{
			return;
		}
		foreach (LoudSpeakerNetwork loudSpeakerNetwork in rigContainer.LoudSpeakerNetworks)
		{
			loudSpeakerNetwork.AddSpeaker(component);
		}
	}

	// Token: 0x0600257A RID: 9594 RVA: 0x000BAB84 File Offset: 0x000B8D84
	private void CleanupLoudSpeakerNetwork()
	{
		if (this.networkSpeaker == null)
		{
			return;
		}
		Speaker component = this.networkSpeaker.GetComponent<Speaker>();
		if (component == null)
		{
			return;
		}
		foreach (LoudSpeakerNetwork loudSpeakerNetwork in this.rigContainer.LoudSpeakerNetworks)
		{
			loudSpeakerNetwork.RemoveSpeaker(component);
		}
	}

	// Token: 0x0600257B RID: 9595 RVA: 0x000BAC00 File Offset: 0x000B8E00
	public void BroadcastLoudSpeakerNetwork(bool toggleBroadcast, int actorNumber)
	{
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(actorNumber), out rigContainer))
		{
			return;
		}
		bool flag = actorNumber == NetworkSystem.Instance.LocalPlayer.ActorNumber;
		this.BroadcastLoudSpeakerNetworkShared(toggleBroadcast, rigContainer, actorNumber, flag);
	}

	// Token: 0x0600257C RID: 9596 RVA: 0x000BAC44 File Offset: 0x000B8E44
	private void BroadcastLoudSpeakerNetworkShared(bool toggleBroadcast, RigContainer rigContainer, int actorNumber, bool isLocal)
	{
		this.SetupLoudSpeakerNetwork(rigContainer);
		foreach (LoudSpeakerNetwork loudSpeakerNetwork in rigContainer.LoudSpeakerNetworks)
		{
			if (toggleBroadcast)
			{
				loudSpeakerNetwork.BroadcastLoudSpeakerNetwork(actorNumber, isLocal);
			}
			else
			{
				loudSpeakerNetwork.StopBroadcastLoudSpeakerNetwork(actorNumber, isLocal);
			}
		}
	}

	// Token: 0x0600257D RID: 9597 RVA: 0x000BACB0 File Offset: 0x000B8EB0
	[PunRPC]
	public void GrabbedByPlayer(bool grabbedBody, bool grabbedLeftHand, bool grabbedWithLeftHand, PhotonMessageInfo info)
	{
		GorillaGuardianManager gorillaGuardianManager = global::GorillaGameModes.GameMode.ActiveGameMode as GorillaGuardianManager;
		if (gorillaGuardianManager == null || !gorillaGuardianManager.IsPlayerGuardian(info.Sender))
		{
			return;
		}
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer))
		{
			return;
		}
		this.vrrig.GrabbedByPlayer(rigContainer.Rig, grabbedBody, grabbedLeftHand, grabbedWithLeftHand);
	}

	// Token: 0x0600257E RID: 9598 RVA: 0x000BAD0C File Offset: 0x000B8F0C
	[PunRPC]
	public void DroppedByPlayer(Vector3 throwVelocity, PhotonMessageInfo info)
	{
		RigContainer rigContainer;
		if (VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer))
		{
			float num = 10000f;
			if ((in throwVelocity).IsValid(in num))
			{
				this.vrrig.DroppedByPlayer(rigContainer.Rig, throwVelocity);
				return;
			}
		}
	}

	// Token: 0x0600257F RID: 9599 RVA: 0x000BAD51 File Offset: 0x000B8F51
	void IFXContextParems<HandTapArgs>.OnPlayFX(HandTapArgs parems)
	{
		this.vrrig.PlayHandTapLocal(parems.soundIndex, parems.isLeftHand, parems.tapVolume);
	}

	// Token: 0x06002580 RID: 9600 RVA: 0x000BAD70 File Offset: 0x000B8F70
	void IFXContextParems<GeoSoundArg>.OnPlayFX(GeoSoundArg parems)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.PlayGeodeEffect(parems.position);
	}

	// Token: 0x06002581 RID: 9601 RVA: 0x000BAD88 File Offset: 0x000B8F88
	private void OnHandTapRPCShared(int surfaceIndex, bool leftHanded, float handSpeed, long packedDir, PhotonMessageInfoWrapped info)
	{
		GorillaNot.IncrementRPCCall(info, "OnHandTapRPCShared");
		if (info.Sender != this.netView.Owner)
		{
			return;
		}
		if (surfaceIndex < 0 || surfaceIndex > GTPlayer.Instance.materialData.Count - 1)
		{
			return;
		}
		Vector3 vector = Utils.UnpackVector3FromLong(packedDir);
		(ref this.tempVector).SetValueSafe(in vector);
		if (this.tempVector.sqrMagnitude != 1f)
		{
			this.tempVector = this.tempVector.normalized;
		}
		float num = GorillaTagger.Instance.DefaultHandTapVolume;
		GorillaAmbushManager gorillaAmbushManager = global::GorillaGameModes.GameMode.ActiveGameMode as GorillaAmbushManager;
		if (gorillaAmbushManager != null && gorillaAmbushManager.IsInfected(this.rigContainer.Creator))
		{
			num = gorillaAmbushManager.crawlingSpeedForMaxVolume;
		}
		OnHandTapFX onHandTapFX = new OnHandTapFX
		{
			rig = this.vrrig,
			surfaceIndex = surfaceIndex,
			isLeftHand = leftHanded,
			volume = handSpeed.ClampSafe(0f, num),
			tapDir = this.tempVector
		};
		if (leftHanded)
		{
			if (CrittersManager.instance.IsNotNull() && CrittersManager.instance.LocalAuthority() && CrittersManager.instance.rigSetupByRig[this.vrrig].IsNotNull())
			{
				CrittersLoudNoise crittersLoudNoise = (CrittersLoudNoise)CrittersManager.instance.rigSetupByRig[this.vrrig].rigActors[0].actorSet;
				if (crittersLoudNoise.IsNotNull())
				{
					crittersLoudNoise.PlayHandTapRemote(info.SentServerTime, true);
				}
			}
		}
		else if (CrittersManager.instance.IsNotNull() && CrittersManager.instance.LocalAuthority() && CrittersManager.instance.rigSetupByRig[this.vrrig].IsNotNull())
		{
			CrittersLoudNoise crittersLoudNoise2 = (CrittersLoudNoise)CrittersManager.instance.rigSetupByRig[this.vrrig].rigActors[2].actorSet;
			if (crittersLoudNoise2.IsNotNull())
			{
				crittersLoudNoise2.PlayHandTapRemote(info.SentServerTime, false);
			}
		}
		FXSystem.PlayFXForRig<HandEffectContext>(FXType.OnHandTap, onHandTapFX, info);
	}

	// Token: 0x06002582 RID: 9602 RVA: 0x000BAFA8 File Offset: 0x000B91A8
	private void PlayHandTapShared(int soundIndex, bool isLeftHand, float tapVolume, PhotonMessageInfoWrapped info = default(PhotonMessageInfoWrapped))
	{
		GorillaNot.IncrementRPCCall(info, "PlayHandTapShared");
		NetPlayer sender = info.Sender;
		if (info.Sender == this.netView.Owner && float.IsFinite(tapVolume))
		{
			this.handTapArgs.soundIndex = soundIndex;
			this.handTapArgs.isLeftHand = isLeftHand;
			this.handTapArgs.tapVolume = Mathf.Clamp(tapVolume, 0f, 0.1f);
			FXSystem.PlayFX<HandTapArgs>(FXType.PlayHandTap, this, this.handTapArgs, info);
			return;
		}
		GorillaNot.instance.SendReport("inappropriate tag data being sent hand tap", sender.UserId, sender.NickName);
	}

	// Token: 0x06002583 RID: 9603 RVA: 0x000BB048 File Offset: 0x000B9248
	private void UpdateNativeSizeShared(float value, PhotonMessageInfoWrapped info = default(PhotonMessageInfoWrapped))
	{
		GorillaNot.IncrementRPCCall(info, "UpdateNativeSizeShared");
		NetPlayer sender = info.Sender;
		if (info.Sender == this.netView.Owner && RPCUtil.SafeValue(value, 0.1f, 10f) && RPCUtil.NotSpam("UpdateNativeSizeShared", info, 1f))
		{
			if (this.vrrig != null)
			{
				this.vrrig.NativeScale = value;
				return;
			}
		}
		else
		{
			GorillaNot.instance.SendReport("inappropriate tag data being sent native size", sender.UserId, sender.NickName);
		}
	}

	// Token: 0x06002584 RID: 9604 RVA: 0x000BB0D8 File Offset: 0x000B92D8
	private void PlayGeodeEffectShared(Vector3 hitPosition, PhotonMessageInfoWrapped info)
	{
		GorillaNot.IncrementRPCCall(info, "PlayGeodeEffectShared");
		if (info.Sender == this.netView.Owner)
		{
			float num = 10000f;
			if ((in hitPosition).IsValid(in num))
			{
				this.geoSoundArg.position = hitPosition;
				FXSystem.PlayFX<GeoSoundArg>(FXType.PlayHandTap, this, this.geoSoundArg, info);
				return;
			}
		}
		GorillaNot.instance.SendReport("inappropriate tag data being sent geode effect", info.Sender.UserId, info.Sender.NickName);
	}

	// Token: 0x06002585 RID: 9605 RVA: 0x000BB159 File Offset: 0x000B9359
	private void InitializeNoobMaterialShared(float red, float green, float blue, PhotonMessageInfoWrapped info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.InitializeNoobMaterial(red, green, blue, info);
	}

	// Token: 0x06002586 RID: 9606 RVA: 0x000BB170 File Offset: 0x000B9370
	private void RequestMaterialColorShared(int askingPlayerID, PhotonMessageInfoWrapped info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.RequestMaterialColor(askingPlayerID, info);
	}

	// Token: 0x06002587 RID: 9607 RVA: 0x000BB184 File Offset: 0x000B9384
	private void RequestCosmeticsShared(PhotonMessageInfoWrapped info)
	{
		GorillaNot.IncrementRPCCall(info, "RequestCosmetics");
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer) || !rigContainer.Rig.fxSettings.callSettings[9].CallLimitSettings.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.RequestCosmetics(info);
	}

	// Token: 0x06002588 RID: 9608 RVA: 0x000BB1E7 File Offset: 0x000B93E7
	private void PlayDrumShared(int drumIndex, float drumVolume, PhotonMessageInfoWrapped info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.PlayDrum(drumIndex, drumVolume, info);
	}

	// Token: 0x06002589 RID: 9609 RVA: 0x000BB1FC File Offset: 0x000B93FC
	private void PlaySelfOnlyInstrumentShared(int selfOnlyIndex, int noteIndex, float instrumentVol, PhotonMessageInfoWrapped info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.PlaySelfOnlyInstrument(selfOnlyIndex, noteIndex, instrumentVol, info);
	}

	// Token: 0x0600258A RID: 9610 RVA: 0x000BB213 File Offset: 0x000B9413
	private void UpdateCosmeticsWithTryonShared(int[] currentItems, int[] tryOnItems, PhotonMessageInfoWrapped info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.UpdateCosmeticsWithTryon(currentItems, tryOnItems, info);
	}

	// Token: 0x0600258B RID: 9611 RVA: 0x000BB228 File Offset: 0x000B9428
	private void PlaySplashEffectShared(Vector3 splashPosition, Quaternion splashRotation, float splashScale, float boundingRadius, bool bigSplash, bool enteringWater, PhotonMessageInfoWrapped info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.PlaySplashEffect(splashPosition, splashRotation, splashScale, boundingRadius, bigSplash, enteringWater, info);
	}

	// Token: 0x0600258C RID: 9612 RVA: 0x000BB245 File Offset: 0x000B9445
	private void EnableNonCosmeticHandItemShared(bool enable, bool isLeftHand, PhotonMessageInfoWrapped info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.EnableNonCosmeticHandItemRPC(enable, isLeftHand, info);
	}

	// Token: 0x0600258D RID: 9613 RVA: 0x000BB25A File Offset: 0x000B945A
	public void UpdateQuestScore(int score, PhotonMessageInfoWrapped info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.UpdateQuestScore(score, info);
	}

	// Token: 0x0600258E RID: 9614 RVA: 0x000BB26E File Offset: 0x000B946E
	public void RequestQuestScore(PhotonMessageInfoWrapped info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.RequestQuestScore(info);
	}

	// Token: 0x06002590 RID: 9616 RVA: 0x000BB2AA File Offset: 0x000B94AA
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.nickName = this._nickName;
		this.defaultName = this._defaultName;
		this.tutorialComplete = this._tutorialComplete;
	}

	// Token: 0x06002591 RID: 9617 RVA: 0x000BB2DA File Offset: 0x000B94DA
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._nickName = this.nickName;
		this._defaultName = this.defaultName;
		this._tutorialComplete = this.tutorialComplete;
	}

	// Token: 0x04002A09 RID: 10761
	[WeaverGenerated]
	[SerializeField]
	[DefaultForProperty("nickName", 0, 17)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private NetworkString<_16> _nickName;

	// Token: 0x04002A0A RID: 10762
	[WeaverGenerated]
	[SerializeField]
	[DefaultForProperty("defaultName", 17, 17)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private NetworkString<_16> _defaultName;

	// Token: 0x04002A0B RID: 10763
	[WeaverGenerated]
	[SerializeField]
	[DefaultForProperty("tutorialComplete", 34, 1)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private bool _tutorialComplete;

	// Token: 0x04002A0C RID: 10764
	[SerializeField]
	private PhotonVoiceView voiceView;

	// Token: 0x04002A0D RID: 10765
	[SerializeField]
	private VoiceNetworkObject fusionVoiceView;

	// Token: 0x04002A0E RID: 10766
	public Transform networkSpeaker;

	// Token: 0x04002A0F RID: 10767
	[SerializeField]
	private VRRig vrrig;

	// Token: 0x04002A10 RID: 10768
	private RigContainer rigContainer;

	// Token: 0x04002A11 RID: 10769
	private HandTapArgs handTapArgs = new HandTapArgs();

	// Token: 0x04002A12 RID: 10770
	private GeoSoundArg geoSoundArg = new GeoSoundArg();

	// Token: 0x04002A13 RID: 10771
	private Vector3 tempVector = Vector3.zero;
}
