using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using GorillaExtensions;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using GorillaLocomotion.Gameplay;
using GorillaNetworking;
using GorillaTag;
using GorillaTag.Cosmetics;
using GorillaTagScripts;
using KID.Model;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using PlayFab;
using PlayFab.ClientModels;
using TagEffects;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

// Token: 0x020003D2 RID: 978
public class VRRig : MonoBehaviour, IWrappedSerializable, INetworkStruct, IPreDisable, IUserCosmeticsCallback, IEyeScannable
{
	// Token: 0x060016C7 RID: 5831 RVA: 0x0006D6AC File Offset: 0x0006B8AC
	private void CosmeticsV2_Awake()
	{
		if (CosmeticsV2Spawner_Dirty.allPartsInstantiated)
		{
			this.Handle_CosmeticsV2_OnPostInstantiateAllPrefabs_DoEnableAllCosmetics();
			return;
		}
		if (!this._isListeningFor_OnPostInstantiateAllPrefabs)
		{
			this._isListeningFor_OnPostInstantiateAllPrefabs = true;
			CosmeticsV2Spawner_Dirty.OnPostInstantiateAllPrefabs = (Action)Delegate.Combine(CosmeticsV2Spawner_Dirty.OnPostInstantiateAllPrefabs, new Action(this.Handle_CosmeticsV2_OnPostInstantiateAllPrefabs_DoEnableAllCosmetics));
		}
	}

	// Token: 0x060016C8 RID: 5832 RVA: 0x0006D6EB File Offset: 0x0006B8EB
	private void CosmeticsV2_OnDestroy()
	{
		if (CosmeticsV2Spawner_Dirty.allPartsInstantiated)
		{
			this.Handle_CosmeticsV2_OnPostInstantiateAllPrefabs_DoEnableAllCosmetics();
			return;
		}
		CosmeticsV2Spawner_Dirty.OnPostInstantiateAllPrefabs = (Action)Delegate.Remove(CosmeticsV2Spawner_Dirty.OnPostInstantiateAllPrefabs, new Action(this.Handle_CosmeticsV2_OnPostInstantiateAllPrefabs_DoEnableAllCosmetics));
	}

	// Token: 0x060016C9 RID: 5833 RVA: 0x0006D71B File Offset: 0x0006B91B
	internal void Handle_CosmeticsV2_OnPostInstantiateAllPrefabs_DoEnableAllCosmetics()
	{
		CosmeticsV2Spawner_Dirty.OnPostInstantiateAllPrefabs = (Action)Delegate.Remove(CosmeticsV2Spawner_Dirty.OnPostInstantiateAllPrefabs, new Action(this.Handle_CosmeticsV2_OnPostInstantiateAllPrefabs_DoEnableAllCosmetics));
		this.CheckForEarlyAccess();
		this.BuildInitialize_AfterCosmeticsV2Instantiated();
		this.SetCosmeticsActive();
	}

	// Token: 0x17000277 RID: 631
	// (get) Token: 0x060016CA RID: 5834 RVA: 0x0006D74F File Offset: 0x0006B94F
	// (set) Token: 0x060016CB RID: 5835 RVA: 0x0006D75C File Offset: 0x0006B95C
	public Vector3 syncPos
	{
		get
		{
			return this.netSyncPos.CurrentSyncTarget;
		}
		set
		{
			this.netSyncPos.SetNewSyncTarget(value);
		}
	}

	// Token: 0x17000278 RID: 632
	// (get) Token: 0x060016CC RID: 5836 RVA: 0x0006D76A File Offset: 0x0006B96A
	// (set) Token: 0x060016CD RID: 5837 RVA: 0x0006D772 File Offset: 0x0006B972
	public GameObject[] cosmetics
	{
		get
		{
			return this._cosmetics;
		}
		set
		{
			this._cosmetics = value;
		}
	}

	// Token: 0x17000279 RID: 633
	// (get) Token: 0x060016CE RID: 5838 RVA: 0x0006D77B File Offset: 0x0006B97B
	// (set) Token: 0x060016CF RID: 5839 RVA: 0x0006D783 File Offset: 0x0006B983
	public GameObject[] overrideCosmetics
	{
		get
		{
			return this._overrideCosmetics;
		}
		set
		{
			this._overrideCosmetics = value;
		}
	}

	// Token: 0x060016D0 RID: 5840 RVA: 0x0006D78C File Offset: 0x0006B98C
	internal void SetTaggedBy(VRRig taggingRig)
	{
		this.taggedById = taggingRig.OwningNetPlayer.ActorNumber;
	}

	// Token: 0x1700027A RID: 634
	// (get) Token: 0x060016D1 RID: 5841 RVA: 0x0006D79F File Offset: 0x0006B99F
	// (set) Token: 0x060016D2 RID: 5842 RVA: 0x0006D7A7 File Offset: 0x0006B9A7
	internal bool InitializedCosmetics
	{
		get
		{
			return this.initializedCosmetics;
		}
		set
		{
			this.initializedCosmetics = value;
		}
	}

	// Token: 0x1700027B RID: 635
	// (get) Token: 0x060016D3 RID: 5843 RVA: 0x0006D7B0 File Offset: 0x0006B9B0
	// (set) Token: 0x060016D4 RID: 5844 RVA: 0x0006D7B8 File Offset: 0x0006B9B8
	public CosmeticRefRegistry cosmeticReferences { get; private set; }

	// Token: 0x1700027C RID: 636
	// (get) Token: 0x060016D5 RID: 5845 RVA: 0x0006D7C1 File Offset: 0x0006B9C1
	public bool HasBracelet
	{
		get
		{
			return this.reliableState.HasBracelet;
		}
	}

	// Token: 0x060016D6 RID: 5846 RVA: 0x0006D7CE File Offset: 0x0006B9CE
	public Vector3 GetMouthPosition()
	{
		return this.MouthPosition.position;
	}

	// Token: 0x1700027D RID: 637
	// (get) Token: 0x060016D7 RID: 5847 RVA: 0x0006D7DB File Offset: 0x0006B9DB
	// (set) Token: 0x060016D8 RID: 5848 RVA: 0x0006D7E3 File Offset: 0x0006B9E3
	public GorillaSkin CurrentCosmeticSkin { get; set; }

	// Token: 0x1700027E RID: 638
	// (get) Token: 0x060016D9 RID: 5849 RVA: 0x0006D7EC File Offset: 0x0006B9EC
	// (set) Token: 0x060016DA RID: 5850 RVA: 0x0006D7F4 File Offset: 0x0006B9F4
	public GorillaSkin CurrentModeSkin { get; set; }

	// Token: 0x1700027F RID: 639
	// (get) Token: 0x060016DB RID: 5851 RVA: 0x0006D7FD File Offset: 0x0006B9FD
	// (set) Token: 0x060016DC RID: 5852 RVA: 0x0006D805 File Offset: 0x0006BA05
	public GorillaSkin TemporaryEffectSkin { get; set; }

	// Token: 0x060016DD RID: 5853 RVA: 0x0006D80E File Offset: 0x0006BA0E
	public VRRig.PartyMemberStatus GetPartyMemberStatus()
	{
		if (this.partyMemberStatus == VRRig.PartyMemberStatus.NeedsUpdate)
		{
			this.partyMemberStatus = (FriendshipGroupDetection.Instance.IsInMyGroup(this.creator.UserId) ? VRRig.PartyMemberStatus.InLocalParty : VRRig.PartyMemberStatus.NotInLocalParty);
		}
		return this.partyMemberStatus;
	}

	// Token: 0x17000280 RID: 640
	// (get) Token: 0x060016DE RID: 5854 RVA: 0x0006D83F File Offset: 0x0006BA3F
	public bool IsLocalPartyMember
	{
		get
		{
			return this.GetPartyMemberStatus() != VRRig.PartyMemberStatus.NotInLocalParty;
		}
	}

	// Token: 0x060016DF RID: 5855 RVA: 0x0006D84D File Offset: 0x0006BA4D
	public void ClearPartyMemberStatus()
	{
		this.partyMemberStatus = VRRig.PartyMemberStatus.NeedsUpdate;
	}

	// Token: 0x060016E0 RID: 5856 RVA: 0x0006D856 File Offset: 0x0006BA56
	public int ActiveTransferrableObjectIndex(int idx)
	{
		return this.reliableState.activeTransferrableObjectIndex[idx];
	}

	// Token: 0x060016E1 RID: 5857 RVA: 0x0006D865 File Offset: 0x0006BA65
	public int ActiveTransferrableObjectIndexLength()
	{
		return this.reliableState.activeTransferrableObjectIndex.Length;
	}

	// Token: 0x060016E2 RID: 5858 RVA: 0x0006D874 File Offset: 0x0006BA74
	public void SetActiveTransferrableObjectIndex(int idx, int v)
	{
		if (this.reliableState.activeTransferrableObjectIndex[idx] != v)
		{
			this.reliableState.activeTransferrableObjectIndex[idx] = v;
			this.reliableState.SetIsDirty();
		}
	}

	// Token: 0x060016E3 RID: 5859 RVA: 0x0006D89F File Offset: 0x0006BA9F
	public TransferrableObject.PositionState TransferrablePosStates(int idx)
	{
		return this.reliableState.transferrablePosStates[idx];
	}

	// Token: 0x060016E4 RID: 5860 RVA: 0x0006D8AE File Offset: 0x0006BAAE
	public void SetTransferrablePosStates(int idx, TransferrableObject.PositionState v)
	{
		if (this.reliableState.transferrablePosStates[idx] != v)
		{
			this.reliableState.transferrablePosStates[idx] = v;
			this.reliableState.SetIsDirty();
		}
	}

	// Token: 0x060016E5 RID: 5861 RVA: 0x0006D8D9 File Offset: 0x0006BAD9
	public TransferrableObject.ItemStates TransferrableItemStates(int idx)
	{
		return this.reliableState.transferrableItemStates[idx];
	}

	// Token: 0x060016E6 RID: 5862 RVA: 0x0006D8E8 File Offset: 0x0006BAE8
	public void SetTransferrableItemStates(int idx, TransferrableObject.ItemStates v)
	{
		if (this.reliableState.transferrableItemStates[idx] != v)
		{
			this.reliableState.transferrableItemStates[idx] = v;
			this.reliableState.SetIsDirty();
		}
	}

	// Token: 0x060016E7 RID: 5863 RVA: 0x0006D913 File Offset: 0x0006BB13
	public void SetTransferrableDockPosition(int idx, BodyDockPositions.DropPositions v)
	{
		if (this.reliableState.transferableDockPositions[idx] != v)
		{
			this.reliableState.transferableDockPositions[idx] = v;
			this.reliableState.SetIsDirty();
		}
	}

	// Token: 0x060016E8 RID: 5864 RVA: 0x0006D93E File Offset: 0x0006BB3E
	public BodyDockPositions.DropPositions TransferrableDockPosition(int idx)
	{
		return this.reliableState.transferableDockPositions[idx];
	}

	// Token: 0x17000281 RID: 641
	// (get) Token: 0x060016E9 RID: 5865 RVA: 0x0006D94D File Offset: 0x0006BB4D
	// (set) Token: 0x060016EA RID: 5866 RVA: 0x0006D95A File Offset: 0x0006BB5A
	public int WearablePackedStates
	{
		get
		{
			return this.reliableState.wearablesPackedStates;
		}
		set
		{
			if (this.reliableState.wearablesPackedStates != value)
			{
				this.reliableState.wearablesPackedStates = value;
				this.reliableState.SetIsDirty();
			}
		}
	}

	// Token: 0x17000282 RID: 642
	// (get) Token: 0x060016EB RID: 5867 RVA: 0x0006D981 File Offset: 0x0006BB81
	// (set) Token: 0x060016EC RID: 5868 RVA: 0x0006D98E File Offset: 0x0006BB8E
	public int LeftThrowableProjectileIndex
	{
		get
		{
			return this.reliableState.lThrowableProjectileIndex;
		}
		set
		{
			if (this.reliableState.lThrowableProjectileIndex != value)
			{
				this.reliableState.lThrowableProjectileIndex = value;
				this.reliableState.SetIsDirty();
			}
		}
	}

	// Token: 0x17000283 RID: 643
	// (get) Token: 0x060016ED RID: 5869 RVA: 0x0006D9B5 File Offset: 0x0006BBB5
	// (set) Token: 0x060016EE RID: 5870 RVA: 0x0006D9C2 File Offset: 0x0006BBC2
	public int RightThrowableProjectileIndex
	{
		get
		{
			return this.reliableState.rThrowableProjectileIndex;
		}
		set
		{
			if (this.reliableState.rThrowableProjectileIndex != value)
			{
				this.reliableState.rThrowableProjectileIndex = value;
				this.reliableState.SetIsDirty();
			}
		}
	}

	// Token: 0x17000284 RID: 644
	// (get) Token: 0x060016EF RID: 5871 RVA: 0x0006D9E9 File Offset: 0x0006BBE9
	// (set) Token: 0x060016F0 RID: 5872 RVA: 0x0006D9F6 File Offset: 0x0006BBF6
	public Color32 LeftThrowableProjectileColor
	{
		get
		{
			return this.reliableState.lThrowableProjectileColor;
		}
		set
		{
			if (!this.reliableState.lThrowableProjectileColor.Equals(value))
			{
				this.reliableState.lThrowableProjectileColor = value;
				this.reliableState.SetIsDirty();
			}
		}
	}

	// Token: 0x17000285 RID: 645
	// (get) Token: 0x060016F1 RID: 5873 RVA: 0x0006DA2D File Offset: 0x0006BC2D
	// (set) Token: 0x060016F2 RID: 5874 RVA: 0x0006DA3A File Offset: 0x0006BC3A
	public Color32 RightThrowableProjectileColor
	{
		get
		{
			return this.reliableState.rThrowableProjectileColor;
		}
		set
		{
			if (!this.reliableState.rThrowableProjectileColor.Equals(value))
			{
				this.reliableState.rThrowableProjectileColor = value;
				this.reliableState.SetIsDirty();
			}
		}
	}

	// Token: 0x060016F3 RID: 5875 RVA: 0x0006DA71 File Offset: 0x0006BC71
	public Color32 GetThrowableProjectileColor(bool isLeftHand)
	{
		if (!isLeftHand)
		{
			return this.RightThrowableProjectileColor;
		}
		return this.LeftThrowableProjectileColor;
	}

	// Token: 0x060016F4 RID: 5876 RVA: 0x0006DA83 File Offset: 0x0006BC83
	public void SetThrowableProjectileColor(bool isLeftHand, Color32 color)
	{
		if (isLeftHand)
		{
			this.LeftThrowableProjectileColor = color;
			return;
		}
		this.RightThrowableProjectileColor = color;
	}

	// Token: 0x060016F5 RID: 5877 RVA: 0x0006DA97 File Offset: 0x0006BC97
	public void SetRandomThrowableModelIndex(int randModelIndex)
	{
		this.RandomThrowableIndex = randModelIndex;
	}

	// Token: 0x060016F6 RID: 5878 RVA: 0x0006DAA0 File Offset: 0x0006BCA0
	public int GetRandomThrowableModelIndex()
	{
		return this.RandomThrowableIndex;
	}

	// Token: 0x17000286 RID: 646
	// (get) Token: 0x060016F7 RID: 5879 RVA: 0x0006DAA8 File Offset: 0x0006BCA8
	// (set) Token: 0x060016F8 RID: 5880 RVA: 0x0006DAB5 File Offset: 0x0006BCB5
	private int RandomThrowableIndex
	{
		get
		{
			return this.reliableState.randomThrowableIndex;
		}
		set
		{
			if (this.reliableState.randomThrowableIndex != value)
			{
				this.reliableState.randomThrowableIndex = value;
				this.reliableState.SetIsDirty();
			}
		}
	}

	// Token: 0x17000287 RID: 647
	// (get) Token: 0x060016F9 RID: 5881 RVA: 0x0006DADC File Offset: 0x0006BCDC
	// (set) Token: 0x060016FA RID: 5882 RVA: 0x0006DAE9 File Offset: 0x0006BCE9
	public bool IsMicEnabled
	{
		get
		{
			return this.reliableState.isMicEnabled;
		}
		set
		{
			if (this.reliableState.isMicEnabled != value)
			{
				this.reliableState.isMicEnabled = value;
				this.reliableState.SetIsDirty();
			}
		}
	}

	// Token: 0x17000288 RID: 648
	// (get) Token: 0x060016FB RID: 5883 RVA: 0x0006DB10 File Offset: 0x0006BD10
	// (set) Token: 0x060016FC RID: 5884 RVA: 0x0006DB1D File Offset: 0x0006BD1D
	public int SizeLayerMask
	{
		get
		{
			return this.reliableState.sizeLayerMask;
		}
		set
		{
			if (this.reliableState.sizeLayerMask != value)
			{
				this.reliableState.sizeLayerMask = value;
				this.reliableState.SetIsDirty();
			}
		}
	}

	// Token: 0x17000289 RID: 649
	// (get) Token: 0x060016FD RID: 5885 RVA: 0x0006DB44 File Offset: 0x0006BD44
	public float scaleFactor
	{
		get
		{
			return this.scaleMultiplier * this.nativeScale;
		}
	}

	// Token: 0x1700028A RID: 650
	// (get) Token: 0x060016FE RID: 5886 RVA: 0x0006DB53 File Offset: 0x0006BD53
	// (set) Token: 0x060016FF RID: 5887 RVA: 0x0006DB5B File Offset: 0x0006BD5B
	public float ScaleMultiplier
	{
		get
		{
			return this.scaleMultiplier;
		}
		set
		{
			this.scaleMultiplier = value;
		}
	}

	// Token: 0x1700028B RID: 651
	// (get) Token: 0x06001700 RID: 5888 RVA: 0x0006DB64 File Offset: 0x0006BD64
	// (set) Token: 0x06001701 RID: 5889 RVA: 0x0006DB6C File Offset: 0x0006BD6C
	public float NativeScale
	{
		get
		{
			return this.nativeScale;
		}
		set
		{
			this.nativeScale = value;
		}
	}

	// Token: 0x1700028C RID: 652
	// (get) Token: 0x06001702 RID: 5890 RVA: 0x0006DB75 File Offset: 0x0006BD75
	public NetPlayer Creator
	{
		get
		{
			return this.creator;
		}
	}

	// Token: 0x1700028D RID: 653
	// (get) Token: 0x06001703 RID: 5891 RVA: 0x0006DB7D File Offset: 0x0006BD7D
	internal bool Initialized
	{
		get
		{
			return this.initialized;
		}
	}

	// Token: 0x1700028E RID: 654
	// (get) Token: 0x06001704 RID: 5892 RVA: 0x0006DB85 File Offset: 0x0006BD85
	// (set) Token: 0x06001705 RID: 5893 RVA: 0x0006DB8D File Offset: 0x0006BD8D
	public float SpeakingLoudness
	{
		get
		{
			return this.speakingLoudness;
		}
		set
		{
			this.speakingLoudness = value;
		}
	}

	// Token: 0x1700028F RID: 655
	// (get) Token: 0x06001706 RID: 5894 RVA: 0x0006DB96 File Offset: 0x0006BD96
	internal HandEffectContext LeftHandEffect
	{
		get
		{
			return this._leftHandEffect;
		}
	}

	// Token: 0x17000290 RID: 656
	// (get) Token: 0x06001707 RID: 5895 RVA: 0x0006DB9E File Offset: 0x0006BD9E
	internal HandEffectContext RightHandEffect
	{
		get
		{
			return this._rightHandEffect;
		}
	}

	// Token: 0x06001708 RID: 5896 RVA: 0x0006DBA8 File Offset: 0x0006BDA8
	public void BuildInitialize()
	{
		this.fxSettings = Object.Instantiate<FXSystemSettings>(this.sharedFXSettings);
		this.fxSettings.forLocalRig = this.isOfflineVRRig;
		this.lastPosition = base.transform.position;
		if (!this.isOfflineVRRig)
		{
			base.transform.parent = null;
		}
		SizeManager component = base.GetComponent<SizeManager>();
		if (component != null)
		{
			component.BuildInitialize();
		}
		this.myMouthFlap = base.GetComponent<GorillaMouthFlap>();
		this.mySpeakerLoudness = base.GetComponent<GorillaSpeakerLoudness>();
		if (this.myReplacementVoice == null)
		{
			this.myReplacementVoice = base.GetComponentInChildren<ReplacementVoice>();
		}
		this.myEyeExpressions = base.GetComponent<GorillaEyeExpressions>();
	}

	// Token: 0x06001709 RID: 5897 RVA: 0x0006DC4C File Offset: 0x0006BE4C
	public void BuildInitialize_AfterCosmeticsV2Instantiated()
	{
		if (!this._rigBuildFullyInitialized)
		{
			Dictionary<string, GameObject> dictionary = new Dictionary<string, GameObject>();
			foreach (GameObject gameObject in this.cosmetics)
			{
				GameObject gameObject2;
				if (!dictionary.TryGetValue(gameObject.name, out gameObject2))
				{
					dictionary.Add(gameObject.name, gameObject);
				}
			}
			foreach (GameObject gameObject3 in this.overrideCosmetics)
			{
				GameObject gameObject2;
				if (dictionary.TryGetValue(gameObject3.name, out gameObject2) && gameObject2.name == gameObject3.name)
				{
					gameObject2.name = "OVERRIDDEN";
				}
			}
			this.cosmetics = this.cosmetics.Concat(this.overrideCosmetics).ToArray<GameObject>();
		}
		this.cosmeticsObjectRegistry.Initialize(this.cosmetics);
		this._rigBuildFullyInitialized = true;
	}

	// Token: 0x0600170A RID: 5898 RVA: 0x0006DD24 File Offset: 0x0006BF24
	private void Awake()
	{
		this.CosmeticsV2_Awake();
		PlayFabAuthenticator instance = PlayFabAuthenticator.instance;
		instance.OnSafetyUpdate = (Action<bool>)Delegate.Combine(instance.OnSafetyUpdate, new Action<bool>(this.UpdateName));
		if (this.isOfflineVRRig)
		{
			this.BuildInitialize();
		}
		this.SharedStart();
	}

	// Token: 0x0600170B RID: 5899 RVA: 0x0006DD73 File Offset: 0x0006BF73
	private void EnsureInstantiatedMaterial()
	{
		if (this.myDefaultSkinMaterialInstance == null)
		{
			this.myDefaultSkinMaterialInstance = Object.Instantiate<Material>(this.materialsToChangeTo[0]);
			this.materialsToChangeTo[0] = this.myDefaultSkinMaterialInstance;
		}
	}

	// Token: 0x0600170C RID: 5900 RVA: 0x0006DDA4 File Offset: 0x0006BFA4
	private void ApplyColorCode()
	{
		float num = 0f;
		float @float = PlayerPrefs.GetFloat("redValue", num);
		float float2 = PlayerPrefs.GetFloat("greenValue", num);
		float float3 = PlayerPrefs.GetFloat("blueValue", num);
		GorillaTagger.Instance.UpdateColor(@float, float2, float3);
	}

	// Token: 0x0600170D RID: 5901 RVA: 0x0006DDE8 File Offset: 0x0006BFE8
	private void SharedStart()
	{
		if (this.isInitialized)
		{
			return;
		}
		this.lastScaleFactor = this.scaleFactor;
		this.isInitialized = true;
		this.myBodyDockPositions = base.GetComponent<BodyDockPositions>();
		this.reliableState.SharedStart(this.isOfflineVRRig, this.myBodyDockPositions);
		this.concatStringOfCosmeticsAllowed = "";
		this.EnsureInstantiatedMaterial();
		this.initialized = false;
		if (this.isOfflineVRRig)
		{
			if (CosmeticsController.hasInstance && CosmeticsController.instance.v2_allCosmeticsInfoAssetRef_isLoaded)
			{
				CosmeticsController.instance.currentWornSet.LoadFromPlayerPreferences(CosmeticsController.instance);
			}
			if (Application.platform == RuntimePlatform.Android && this.spectatorSkin != null)
			{
				Object.Destroy(this.spectatorSkin);
			}
			base.StartCoroutine(this.OccasionalUpdate());
			this.initialized = true;
		}
		else if (!this.isOfflineVRRig)
		{
			if (this.spectatorSkin != null)
			{
				Object.Destroy(this.spectatorSkin);
			}
			this.head.syncPos = -this.headBodyOffset;
		}
		GorillaSkin.ShowActiveSkin(this);
		base.Invoke("ApplyColorCode", 1f);
		List<Material> list = new List<Material>();
		this.mainSkin.GetSharedMaterials(list);
		this.layerChanger = base.GetComponent<LayerChanger>();
		if (this.layerChanger != null)
		{
			this.layerChanger.InitializeLayers(base.transform);
		}
		this.frozenEffectMinY = this.frozenEffect.transform.localScale.y;
		this.frozenEffectMinHorizontalScale = this.frozenEffect.transform.localScale.x;
	}

	// Token: 0x0600170E RID: 5902 RVA: 0x0006DF79 File Offset: 0x0006C179
	private IEnumerator OccasionalUpdate()
	{
		for (;;)
		{
			try
			{
				if (RoomSystem.JoinedRoom && NetworkSystem.Instance.IsMasterClient && global::GorillaGameModes.GameMode.ActiveNetworkHandler.IsNull())
				{
					global::GorillaGameModes.GameMode.LoadGameModeFromProperty();
				}
			}
			catch
			{
			}
			yield return new WaitForSeconds(1f);
		}
		yield break;
	}

	// Token: 0x0600170F RID: 5903 RVA: 0x0006DF84 File Offset: 0x0006C184
	public bool IsItemAllowed(string itemName)
	{
		if (itemName == "Slingshot")
		{
			return NetworkSystem.Instance.InRoom && GorillaGameManager.instance is GorillaPaintbrawlManager;
		}
		if (BuilderSetManager.instance.GetStarterSetsConcat().Contains(itemName))
		{
			return true;
		}
		if (this.concatStringOfCosmeticsAllowed == null)
		{
			return false;
		}
		if (this.concatStringOfCosmeticsAllowed.Contains(itemName))
		{
			return true;
		}
		bool canTryOn = CosmeticsController.instance.GetItemFromDict(itemName).canTryOn;
		return this.inTryOnRoom && canTryOn;
	}

	// Token: 0x06001710 RID: 5904 RVA: 0x0006E00A File Offset: 0x0006C20A
	public void ApplyLocalTrajectoryOverride(Vector3 overrideVelocity)
	{
		this.LocalTrajectoryOverrideBlend = 1f;
		this.LocalTrajectoryOverridePosition = base.transform.position;
		this.LocalTrajectoryOverrideVelocity = overrideVelocity;
	}

	// Token: 0x06001711 RID: 5905 RVA: 0x0006E02F File Offset: 0x0006C22F
	public bool IsLocalTrajectoryOverrideActive()
	{
		return this.LocalTrajectoryOverrideBlend > 0f;
	}

	// Token: 0x06001712 RID: 5906 RVA: 0x0006E03E File Offset: 0x0006C23E
	public void ApplyLocalGrabOverride(bool isBody, bool isLeftHand, Transform grabbingHand)
	{
		this.localOverrideIsBody = isBody;
		this.localOverrideIsLeftHand = isLeftHand;
		this.localOverrideGrabbingHand = grabbingHand;
		this.localGrabOverrideBlend = 1f;
	}

	// Token: 0x06001713 RID: 5907 RVA: 0x0006E060 File Offset: 0x0006C260
	public void ClearLocalGrabOverride()
	{
		this.localGrabOverrideBlend = -1f;
	}

	// Token: 0x06001714 RID: 5908 RVA: 0x0006E070 File Offset: 0x0006C270
	public void RemoteRigUpdate()
	{
		if (this.scaleFactor != this.lastScaleFactor)
		{
			this.ScaleUpdate();
		}
		if (this.voiceAudio != null)
		{
			float num = GorillaTagger.Instance.offlineVRRig.scaleFactor / this.scaleFactor;
			float num2 = this.voicePitchForRelativeScale.Evaluate(num);
			if (float.IsNaN(num2) || num2 <= 0f)
			{
				Debug.LogError("Voice pitch curve is invalid, please fix!");
			}
			float num3 = (this.UsingHauntedRing ? this.HauntedRingVoicePitch : num2);
			num3 = (this.IsHaunted ? this.HauntedVoicePitch : num3);
			if (!Mathf.Approximately(this.voiceAudio.pitch, num3))
			{
				this.voiceAudio.pitch = num3;
			}
		}
		this.jobPos = base.transform.position;
		if (Time.time > this.timeSpawned + this.doNotLerpConstant)
		{
			this.jobPos = Vector3.Lerp(base.transform.position, this.SanitizeVector3(this.syncPos), this.lerpValueBody * 0.66f);
			if (this.currentRopeSwing && this.currentRopeSwingTarget)
			{
				Vector3 vector;
				if (this.grabbedRopeIsLeft)
				{
					vector = this.currentRopeSwingTarget.position - this.leftHandTransform.position;
				}
				else
				{
					vector = this.currentRopeSwingTarget.position - this.rightHandTransform.position;
				}
				if (this.shouldLerpToRope)
				{
					this.jobPos += Vector3.Lerp(Vector3.zero, vector, this.lastRopeGrabTimer * 4f);
					if (this.lastRopeGrabTimer < 1f)
					{
						this.lastRopeGrabTimer += Time.deltaTime;
					}
				}
				else
				{
					this.jobPos += vector;
				}
			}
			else if (this.currentHoldParent)
			{
				Transform transform;
				if (this.grabbedRopeIsBody)
				{
					transform = this.bodyTransform;
				}
				else if (this.grabbedRopeIsLeft)
				{
					transform = this.leftHandTransform;
				}
				else
				{
					transform = this.rightHandTransform;
				}
				this.jobPos += this.currentHoldParent.TransformPoint(this.grabbedRopeOffset) - transform.position;
			}
			else if (this.mountedMonkeBlock || this.mountedMovingSurface)
			{
				Transform transform2 = (this.movingSurfaceIsMonkeBlock ? this.mountedMonkeBlock.transform : this.mountedMovingSurface.transform);
				Vector3 vector2 = Vector3.zero;
				Vector3 vector3 = this.jobPos - base.transform.position;
				Transform transform3;
				if (this.mountedMovingSurfaceIsBody)
				{
					transform3 = this.bodyTransform;
				}
				else if (this.mountedMovingSurfaceIsLeft)
				{
					transform3 = this.leftHandTransform;
				}
				else
				{
					transform3 = this.rightHandTransform;
				}
				vector2 = transform2.TransformPoint(this.mountedMonkeBlockOffset) - (transform3.position + vector3);
				if (this.shouldLerpToMovingSurface)
				{
					this.lastMountedSurfaceTimer += Time.deltaTime;
					this.jobPos += Vector3.Lerp(Vector3.zero, vector2, this.lastMountedSurfaceTimer * 4f);
					if (this.lastMountedSurfaceTimer * 4f >= 1f)
					{
						this.shouldLerpToMovingSurface = false;
					}
				}
				else
				{
					this.jobPos += vector2;
				}
			}
		}
		else
		{
			this.jobPos = this.SanitizeVector3(this.syncPos);
		}
		if (this.LocalTrajectoryOverrideBlend > 0f)
		{
			this.LocalTrajectoryOverrideBlend -= Time.deltaTime / this.LocalTrajectoryOverrideDuration;
			this.LocalTrajectoryOverrideVelocity += Physics.gravity * Time.deltaTime * 0.5f;
			Vector3 vector4;
			Vector3 vector5;
			if (this.LocalTestMovementCollision(this.LocalTrajectoryOverridePosition, this.LocalTrajectoryOverrideVelocity, out vector4, out vector5))
			{
				this.LocalTrajectoryOverrideVelocity = vector4;
				this.LocalTrajectoryOverridePosition = vector5;
			}
			else
			{
				this.LocalTrajectoryOverridePosition += this.LocalTrajectoryOverrideVelocity * Time.deltaTime;
			}
			this.LocalTrajectoryOverrideVelocity += Physics.gravity * Time.deltaTime * 0.5f;
			this.jobPos = Vector3.Lerp(this.jobPos, this.LocalTrajectoryOverridePosition, this.LocalTrajectoryOverrideBlend);
		}
		else if (this.localGrabOverrideBlend > 0f)
		{
			this.localGrabOverrideBlend -= Time.deltaTime / this.LocalGrabOverrideDuration;
			if (this.localOverrideGrabbingHand != null)
			{
				Transform transform4;
				if (this.localOverrideIsBody)
				{
					transform4 = this.bodyTransform;
				}
				else if (this.localOverrideIsLeftHand)
				{
					transform4 = this.leftHandTransform;
				}
				else
				{
					transform4 = this.rightHandTransform;
				}
				this.jobPos += this.localOverrideGrabbingHand.TransformPoint(this.grabbedRopeOffset) - transform4.position;
			}
		}
		this.jobRotation = Quaternion.Lerp(base.transform.rotation, this.SanitizeQuaternion(this.syncRotation), this.lerpValueBody);
		this.head.syncPos = base.transform.rotation * -this.headBodyOffset * this.scaleFactor;
		this.head.MapOther(this.lerpValueBody);
		this.rightHand.MapOther(this.lerpValueBody);
		this.leftHand.MapOther(this.lerpValueBody);
		this.rightIndex.MapOtherFinger((float)(this.handSync % 10) / 10f, this.lerpValueFingers);
		this.rightMiddle.MapOtherFinger((float)(this.handSync % 100) / 100f, this.lerpValueFingers);
		this.rightThumb.MapOtherFinger((float)(this.handSync % 1000) / 1000f, this.lerpValueFingers);
		this.leftIndex.MapOtherFinger((float)(this.handSync % 10000) / 10000f, this.lerpValueFingers);
		this.leftMiddle.MapOtherFinger((float)(this.handSync % 100000) / 100000f, this.lerpValueFingers);
		this.leftThumb.MapOtherFinger((float)(this.handSync % 1000000) / 1000000f, this.lerpValueFingers);
		this.leftHandHoldableStatus = this.handSync % 10000000 / 1000000;
		this.rightHandHoldableStatus = this.handSync % 100000000 / 10000000;
	}

	// Token: 0x06001715 RID: 5909 RVA: 0x0006E6F0 File Offset: 0x0006C8F0
	private void ScaleUpdate()
	{
		this.frameScale = Mathf.MoveTowards(this.lastScaleFactor, this.scaleFactor, Time.deltaTime * 4f);
		base.transform.localScale = Vector3.one * this.frameScale;
		this.lastScaleFactor = this.frameScale;
	}

	// Token: 0x06001716 RID: 5910 RVA: 0x0006E746 File Offset: 0x0006C946
	public void AddLateUpdateCallback(ICallBack action)
	{
		this.lateUpdateCallbacks.Add(action);
	}

	// Token: 0x06001717 RID: 5911 RVA: 0x0006E754 File Offset: 0x0006C954
	public void RemoveLateUpdateCallback(ICallBack action)
	{
		this.lateUpdateCallbacks.Remove(action);
	}

	// Token: 0x06001718 RID: 5912 RVA: 0x0006E764 File Offset: 0x0006C964
	private void LateUpdate()
	{
		if (this.isOfflineVRRig)
		{
			if (GorillaGameManager.instance != null)
			{
				this.speedArray = GorillaGameManager.instance.LocalPlayerSpeed();
				GTPlayer.Instance.jumpMultiplier = this.speedArray[1];
				GTPlayer.Instance.maxJumpSpeed = this.speedArray[0];
			}
			else
			{
				GTPlayer.Instance.jumpMultiplier = 1.1f;
				GTPlayer.Instance.maxJumpSpeed = 6.5f;
			}
			this.nativeScale = GTPlayer.Instance.NativeScale;
			this.scaleMultiplier = GTPlayer.Instance.ScaleMultiplier;
			if (this.scaleFactor != this.lastScaleFactor)
			{
				this.ScaleUpdate();
			}
			base.transform.eulerAngles = new Vector3(0f, this.mainCamera.transform.rotation.eulerAngles.y, 0f);
			this.syncPos = this.mainCamera.transform.position + this.headConstraint.rotation * this.head.trackingPositionOffset * this.lastScaleFactor + base.transform.rotation * this.headBodyOffset * this.lastScaleFactor;
			base.transform.position = this.syncPos;
			this.head.MapMine(this.lastScaleFactor, this.playerOffsetTransform);
			this.rightHand.MapMine(this.lastScaleFactor, this.playerOffsetTransform);
			this.leftHand.MapMine(this.lastScaleFactor, this.playerOffsetTransform);
			this.rightIndex.MapMyFinger(this.lerpValueFingers);
			this.rightMiddle.MapMyFinger(this.lerpValueFingers);
			this.rightThumb.MapMyFinger(this.lerpValueFingers);
			this.leftIndex.MapMyFinger(this.lerpValueFingers);
			this.leftMiddle.MapMyFinger(this.lerpValueFingers);
			this.leftThumb.MapMyFinger(this.lerpValueFingers);
			if (GorillaTagger.Instance.loadedDeviceName == "Oculus")
			{
				this.mainSkin.enabled = OVRManager.hasInputFocus;
			}
			this.bodyRenderer.ActiveBody.enabled = !GTPlayer.Instance.inOverlay;
			int i = this.loudnessCheckFrame - 1;
			this.loudnessCheckFrame = i;
			if (i < 0)
			{
				this.SpeakingLoudness = 0f;
				if (this.shouldSendSpeakingLoudness && this.netView)
				{
					PhotonVoiceView component = this.netView.GetComponent<PhotonVoiceView>();
					if (component && component.RecorderInUse)
					{
						MicWrapper micWrapper = component.RecorderInUse.InputSource as MicWrapper;
						if (micWrapper != null)
						{
							int num = this.replacementVoiceDetectionDelay;
							if (num > this.voiceSampleBuffer.Length)
							{
								Array.Resize<float>(ref this.voiceSampleBuffer, num);
							}
							float[] array = this.voiceSampleBuffer;
							if (micWrapper != null && micWrapper.Mic != null && micWrapper.Mic.samples >= num && micWrapper.Mic.GetData(array, micWrapper.Mic.samples - num))
							{
								float num2 = 0f;
								for (int j = 0; j < num; j++)
								{
									float num3 = Mathf.Sqrt(array[j]);
									if (num3 > num2)
									{
										num2 = num3;
									}
								}
								this.SpeakingLoudness = num2;
							}
						}
					}
				}
				this.loudnessCheckFrame = 10;
			}
		}
		if (this.creator != null)
		{
			if (GorillaGameManager.instance != null)
			{
				GorillaGameManager.instance.UpdatePlayerAppearance(this);
			}
			else if (this.setMatIndex != 0)
			{
				this.ChangeMaterialLocal(0);
				this.ForceResetFrozenEffect();
			}
		}
		if (this.inDuplicationZone)
		{
			this.renderTransform.position = base.transform.position + this.duplicationZone.VisualOffsetForRigs;
		}
		if (this.frozenEffect.activeSelf)
		{
			GorillaFreezeTagManager gorillaFreezeTagManager = GorillaGameManager.instance as GorillaFreezeTagManager;
			if (gorillaFreezeTagManager != null)
			{
				Vector3 localScale = this.frozenEffect.transform.localScale;
				Vector3 vector = localScale;
				vector.y = Mathf.Lerp(this.frozenEffectMinY, this.frozenEffectMaxY, this.frozenTimeElapsed / gorillaFreezeTagManager.freezeDuration);
				localScale = new Vector3(localScale.x, vector.y, localScale.z);
				this.frozenEffect.transform.localScale = localScale;
				this.frozenTimeElapsed += Time.deltaTime;
			}
		}
		if (this.TemporaryCosmeticEffects.Count > 0)
		{
			foreach (KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> keyValuePair in this.TemporaryCosmeticEffects.ToArray<KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect>>())
			{
				if (Time.time - keyValuePair.Value.EffectStartedTime >= keyValuePair.Value.effectDuration)
				{
					this.RemoveTemporaryCosmeticEffects(keyValuePair);
				}
			}
		}
		this.lateUpdateCallbacks.TryRunCallbacks();
	}

	// Token: 0x06001719 RID: 5913 RVA: 0x0006EC40 File Offset: 0x0006CE40
	private void RemoveTemporaryCosmeticEffects(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect)
	{
		if (effect.Key == CosmeticEffectsOnPlayers.EFFECTTYPE.SKIN)
		{
			bool flag;
			if (effect.Value.newSkin != null && GorillaSkin.GetActiveSkin(this, out flag) == effect.Value.newSkin)
			{
				GorillaSkin.ApplyToRig(this, null, GorillaSkin.SkinType.temporaryEffect);
			}
		}
		else if (effect.Key == CosmeticEffectsOnPlayers.EFFECTTYPE.FPV)
		{
			if (this.FPVEffectsParent != null)
			{
				this.SpawnFPVEffects(effect, false);
			}
		}
		else if (effect.Key == CosmeticEffectsOnPlayers.EFFECTTYPE.KNOCKBACK)
		{
			this.DisableHitWithKnockBack(effect);
		}
		this.TemporaryCosmeticEffects.Remove(effect.Key);
	}

	// Token: 0x0600171A RID: 5914 RVA: 0x0006ECD5 File Offset: 0x0006CED5
	public void SpawnSkinEffects(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect)
	{
		GorillaSkin.ApplyToRig(this, effect.Value.newSkin, GorillaSkin.SkinType.temporaryEffect);
		this.TemporaryCosmeticEffects.TryAdd(effect.Key, effect.Value);
	}

	// Token: 0x0600171B RID: 5915 RVA: 0x0006ED04 File Offset: 0x0006CF04
	public void SpawnFPVEffects(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect, bool enable)
	{
		if (this.FPVEffectsParent == null)
		{
			return;
		}
		if (enable)
		{
			GameObject gameObject = ObjectPools.instance.Instantiate(effect.Value.FPVEffect, this.FPVEffectsParent.transform.position, this.FPVEffectsParent.transform.rotation, true);
			if (gameObject != null)
			{
				gameObject.gameObject.transform.SetParent(this.FPVEffectsParent.transform);
				gameObject.gameObject.transform.localPosition = Vector3.zero;
			}
			this.TemporaryCosmeticEffects.TryAdd(effect.Key, effect.Value);
			return;
		}
		foreach (object obj in this.FPVEffectsParent.transform)
		{
			Transform transform = (Transform)obj;
			ObjectPools.instance.Destroy(transform.gameObject);
		}
		if (this.TemporaryCosmeticEffects.ContainsKey(effect.Key))
		{
			this.TemporaryCosmeticEffects.Remove(effect.Key);
		}
	}

	// Token: 0x0600171C RID: 5916 RVA: 0x0006EE34 File Offset: 0x0006D034
	public void EnableHitWithKnockBack(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect)
	{
		this.TemporaryCosmeticEffects.TryAdd(effect.Key, effect.Value);
	}

	// Token: 0x0600171D RID: 5917 RVA: 0x0006EE50 File Offset: 0x0006D050
	private void DisableHitWithKnockBack(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect)
	{
		if (this.TemporaryCosmeticEffects.ContainsKey(effect.Key) && effect.Value.knockbackVFX)
		{
			GameObject gameObject = ObjectPools.instance.Instantiate(effect.Value.knockbackVFX, base.transform.position, true);
			if (gameObject != null)
			{
				gameObject.gameObject.transform.SetParent(base.transform);
				gameObject.gameObject.transform.localPosition = Vector3.zero;
				ParticleSystem componentInChildren = gameObject.GetComponentInChildren<ParticleSystem>();
				if (componentInChildren)
				{
					componentInChildren.Play();
				}
			}
		}
	}

	// Token: 0x0600171E RID: 5918 RVA: 0x0006EEF0 File Offset: 0x0006D0F0
	public void DisableHitWithKnockBack()
	{
		foreach (KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> keyValuePair in this.TemporaryCosmeticEffects.ToArray<KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect>>())
		{
			bool flag;
			if (keyValuePair.Key == CosmeticEffectsOnPlayers.EFFECTTYPE.KNOCKBACK)
			{
				this.DisableHitWithKnockBack(keyValuePair);
				this.TemporaryCosmeticEffects.Remove(keyValuePair.Key);
			}
			else if (keyValuePair.Key == CosmeticEffectsOnPlayers.EFFECTTYPE.SKIN && keyValuePair.Value.newSkin != null && GorillaSkin.GetActiveSkin(this, out flag) == keyValuePair.Value.newSkin)
			{
				GorillaSkin.ApplyToRig(this, null, GorillaSkin.SkinType.temporaryEffect);
				this.TemporaryCosmeticEffects.Remove(keyValuePair.Key);
			}
		}
	}

	// Token: 0x0600171F RID: 5919 RVA: 0x0006EE34 File Offset: 0x0006D034
	public void ActivateVOEffect(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect)
	{
		this.TemporaryCosmeticEffects.TryAdd(effect.Key, effect.Value);
	}

	// Token: 0x06001720 RID: 5920 RVA: 0x0006EF9E File Offset: 0x0006D19E
	public bool TryGetCosmeticVoiceOverride(CosmeticEffectsOnPlayers.EFFECTTYPE key, out CosmeticEffectsOnPlayers.CosmeticEffect value)
	{
		if (this.TemporaryCosmeticEffects == null)
		{
			value = null;
			return false;
		}
		return this.TemporaryCosmeticEffects.TryGetValue(key, out value);
	}

	// Token: 0x17000291 RID: 657
	// (get) Token: 0x06001721 RID: 5921 RVA: 0x0006EFBA File Offset: 0x0006D1BA
	public bool IsPlayerMeshHidden
	{
		get
		{
			return !this.mainSkin.enabled;
		}
	}

	// Token: 0x06001722 RID: 5922 RVA: 0x0006EFCA File Offset: 0x0006D1CA
	public void SetPlayerMeshHidden(bool hide)
	{
		this.mainSkin.enabled = !hide;
		this.faceSkin.enabled = !hide;
		this.nameTagAnchor.SetActive(!hide);
		this.UpdateMatParticles(-1);
	}

	// Token: 0x06001723 RID: 5923 RVA: 0x0006F000 File Offset: 0x0006D200
	public void SetInvisibleToLocalPlayer(bool invisible)
	{
		if (this.IsInvisibleToLocalPlayer == invisible)
		{
			return;
		}
		this.IsInvisibleToLocalPlayer = invisible;
		this.nameTagAnchor.SetActive(!invisible);
		this.UpdateFriendshipBracelet();
	}

	// Token: 0x06001724 RID: 5924 RVA: 0x0006F028 File Offset: 0x0006D228
	public void ChangeLayer(string layerName)
	{
		if (this.layerChanger != null)
		{
			this.layerChanger.ChangeLayer(base.transform.parent, layerName);
		}
		GTPlayer.Instance.ChangeLayer(layerName);
	}

	// Token: 0x06001725 RID: 5925 RVA: 0x0006F05A File Offset: 0x0006D25A
	public void RestoreLayer()
	{
		if (this.layerChanger != null)
		{
			this.layerChanger.RestoreOriginalLayers();
		}
		GTPlayer.Instance.RestoreLayer();
	}

	// Token: 0x06001726 RID: 5926 RVA: 0x000023F4 File Offset: 0x000005F4
	public void SetHeadBodyOffset()
	{
	}

	// Token: 0x06001727 RID: 5927 RVA: 0x0006F07F File Offset: 0x0006D27F
	public void VRRigResize(float ratioVar)
	{
		this.ratio *= ratioVar;
	}

	// Token: 0x06001728 RID: 5928 RVA: 0x0006F090 File Offset: 0x0006D290
	public int ReturnHandPosition()
	{
		return 0 + Mathf.FloorToInt(this.rightIndex.calcT * 9.99f) + Mathf.FloorToInt(this.rightMiddle.calcT * 9.99f) * 10 + Mathf.FloorToInt(this.rightThumb.calcT * 9.99f) * 100 + Mathf.FloorToInt(this.leftIndex.calcT * 9.99f) * 1000 + Mathf.FloorToInt(this.leftMiddle.calcT * 9.99f) * 10000 + Mathf.FloorToInt(this.leftThumb.calcT * 9.99f) * 100000 + this.leftHandHoldableStatus * 1000000 + this.rightHandHoldableStatus * 10000000;
	}

	// Token: 0x06001729 RID: 5929 RVA: 0x0006F15A File Offset: 0x0006D35A
	public void OnDestroy()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		if (this.currentRopeSwingTarget && this.currentRopeSwingTarget.gameObject)
		{
			Object.Destroy(this.currentRopeSwingTarget.gameObject);
		}
		this.ClearRopeData();
	}

	// Token: 0x0600172A RID: 5930 RVA: 0x0006F19C File Offset: 0x0006D39C
	private InputStruct SerializeWriteShared()
	{
		InputStruct inputStruct = default(InputStruct);
		inputStruct.headRotation = BitPackUtils.PackQuaternionForNetwork(this.head.rigTarget.localRotation);
		inputStruct.rightHandLong = BitPackUtils.PackHandPosRotForNetwork(this.rightHand.rigTarget.localPosition, this.rightHand.rigTarget.localRotation);
		inputStruct.leftHandLong = BitPackUtils.PackHandPosRotForNetwork(this.leftHand.rigTarget.localPosition, this.leftHand.rigTarget.localRotation);
		inputStruct.position = BitPackUtils.PackWorldPosForNetwork(base.transform.position);
		inputStruct.handPosition = this.ReturnHandPosition();
		inputStruct.taggedById = (short)this.taggedById;
		int num = Mathf.Clamp(Mathf.RoundToInt(base.transform.rotation.eulerAngles.y + 360f) % 360, 0, 360);
		int num2 = Mathf.RoundToInt(Mathf.Clamp01(this.SpeakingLoudness) * 255f);
		int num3 = num + (this.remoteUseReplacementVoice ? 512 : 0) + ((this.grabbedRopeIndex != -1) ? 1024 : 0) + (this.grabbedRopeIsPhotonView ? 2048 : 0) + (this.hoverboardVisual.IsHeld ? 8192 : 0) + (this.hoverboardVisual.IsLeftHanded ? 16384 : 0) + ((this.mountedMovingSurfaceId != -1) ? 32768 : 0) + (num2 << 16);
		inputStruct.packedFields = num3;
		inputStruct.packedCompetitiveData = this.PackCompetitiveData();
		if (this.grabbedRopeIndex != -1)
		{
			inputStruct.grabbedRopeIndex = this.grabbedRopeIndex;
			inputStruct.ropeBoneIndex = this.grabbedRopeBoneIndex;
			inputStruct.ropeGrabIsLeft = this.grabbedRopeIsLeft;
			inputStruct.ropeGrabIsBody = this.grabbedRopeIsBody;
			inputStruct.ropeGrabOffset = this.grabbedRopeOffset;
		}
		if (this.grabbedRopeIndex == -1 && this.mountedMovingSurfaceId != -1)
		{
			inputStruct.grabbedRopeIndex = this.mountedMovingSurfaceId;
			inputStruct.ropeGrabIsLeft = this.mountedMovingSurfaceIsLeft;
			inputStruct.ropeGrabIsBody = this.mountedMovingSurfaceIsBody;
			inputStruct.ropeGrabOffset = this.mountedMonkeBlockOffset;
		}
		if (this.hoverboardVisual.IsHeld)
		{
			inputStruct.hoverboardPosRot = BitPackUtils.PackHandPosRotForNetwork(this.hoverboardVisual.NominalLocalPosition, this.hoverboardVisual.NominalLocalRotation);
			inputStruct.hoverboardColor = BitPackUtils.PackColorForNetwork(this.hoverboardVisual.boardColor);
		}
		return inputStruct;
	}

	// Token: 0x0600172B RID: 5931 RVA: 0x0006F408 File Offset: 0x0006D608
	private void SerializeReadShared(InputStruct data)
	{
		VRMap vrmap = this.head;
		Quaternion quaternion = BitPackUtils.UnpackQuaternionFromNetwork(data.headRotation);
		(ref vrmap.syncRotation).SetValueSafe(in quaternion);
		BitPackUtils.UnpackHandPosRotFromNetwork(data.rightHandLong, out this.tempVec, out this.tempQuat);
		this.rightHand.syncPos = this.tempVec;
		(ref this.rightHand.syncRotation).SetValueSafe(in this.tempQuat);
		BitPackUtils.UnpackHandPosRotFromNetwork(data.leftHandLong, out this.tempVec, out this.tempQuat);
		this.leftHand.syncPos = this.tempVec;
		(ref this.leftHand.syncRotation).SetValueSafe(in this.tempQuat);
		this.syncPos = BitPackUtils.UnpackWorldPosFromNetwork(data.position);
		this.handSync = data.handPosition;
		int packedFields = data.packedFields;
		int num = packedFields & 511;
		this.syncRotation.eulerAngles = this.SanitizeVector3(new Vector3(0f, (float)num, 0f));
		this.remoteUseReplacementVoice = (packedFields & 512) != 0;
		int num2 = (packedFields >> 16) & 255;
		this.SpeakingLoudness = (float)num2 / 255f;
		this.UpdateReplacementVoice();
		this.UnpackCompetitiveData(data.packedCompetitiveData);
		this.taggedById = (int)data.taggedById;
		bool flag = (packedFields & 1024) != 0;
		this.grabbedRopeIsPhotonView = (packedFields & 2048) != 0;
		if (flag)
		{
			this.grabbedRopeIndex = data.grabbedRopeIndex;
			this.grabbedRopeBoneIndex = data.ropeBoneIndex;
			this.grabbedRopeIsLeft = data.ropeGrabIsLeft;
			this.grabbedRopeIsBody = data.ropeGrabIsBody;
			(ref this.grabbedRopeOffset).SetValueSafe(in data.ropeGrabOffset);
		}
		else
		{
			this.grabbedRopeIndex = -1;
		}
		bool flag2 = (packedFields & 32768) != 0;
		if (!flag && flag2)
		{
			this.mountedMovingSurfaceId = data.grabbedRopeIndex;
			this.mountedMovingSurfaceIsLeft = data.ropeGrabIsLeft;
			this.mountedMovingSurfaceIsBody = data.ropeGrabIsBody;
			(ref this.mountedMonkeBlockOffset).SetValueSafe(in data.ropeGrabOffset);
			this.movingSurfaceIsMonkeBlock = data.movingSurfaceIsMonkeBlock;
		}
		else
		{
			this.mountedMovingSurfaceId = -1;
		}
		bool flag3 = (packedFields & 8192) != 0;
		bool flag4 = (packedFields & 16384) != 0;
		if (flag3)
		{
			Vector3 vector;
			Quaternion quaternion2;
			BitPackUtils.UnpackHandPosRotFromNetwork(data.hoverboardPosRot, out vector, out quaternion2);
			Color color = BitPackUtils.UnpackColorFromNetwork(data.hoverboardColor);
			if ((in quaternion2).IsValid())
			{
				this.hoverboardVisual.SetIsHeld(flag4, vector.ClampMagnitudeSafe(1f), quaternion2, color);
			}
		}
		else if (this.hoverboardVisual.gameObject.activeSelf)
		{
			this.hoverboardVisual.SetNotHeld();
		}
		if (this.grabbedRopeIsPhotonView)
		{
			this.localGrabOverrideBlend = -1f;
		}
		this.UpdateRopeData();
		this.UpdateMovingMonkeBlockData();
		this.AddVelocityToQueue(this.syncPos, data.serverTimeStamp);
	}

	// Token: 0x0600172C RID: 5932 RVA: 0x0006F6B4 File Offset: 0x0006D8B4
	void IWrappedSerializable.OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
	{
		InputStruct inputStruct = this.SerializeWriteShared();
		stream.SendNext(inputStruct.headRotation);
		stream.SendNext(inputStruct.rightHandLong);
		stream.SendNext(inputStruct.leftHandLong);
		stream.SendNext(inputStruct.position);
		stream.SendNext(inputStruct.handPosition);
		stream.SendNext(inputStruct.packedFields);
		stream.SendNext(inputStruct.packedCompetitiveData);
		if (this.grabbedRopeIndex != -1)
		{
			stream.SendNext(inputStruct.grabbedRopeIndex);
			stream.SendNext(inputStruct.ropeBoneIndex);
			stream.SendNext(inputStruct.ropeGrabIsLeft);
			stream.SendNext(inputStruct.ropeGrabIsBody);
			stream.SendNext(inputStruct.ropeGrabOffset);
		}
		else if (this.mountedMovingSurfaceId != -1)
		{
			stream.SendNext(inputStruct.grabbedRopeIndex);
			stream.SendNext(inputStruct.ropeGrabIsLeft);
			stream.SendNext(inputStruct.ropeGrabIsBody);
			stream.SendNext(inputStruct.ropeGrabOffset);
			stream.SendNext(inputStruct.movingSurfaceIsMonkeBlock);
		}
		if ((inputStruct.packedFields & 8192) != 0)
		{
			stream.SendNext(inputStruct.hoverboardPosRot);
			stream.SendNext(inputStruct.hoverboardColor);
		}
	}

	// Token: 0x0600172D RID: 5933 RVA: 0x0006F830 File Offset: 0x0006DA30
	void IWrappedSerializable.OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!Utils.ValidateServerTime(info.SentServerTime, 60.0))
		{
			return;
		}
		InputStruct inputStruct = new InputStruct
		{
			headRotation = (int)stream.ReceiveNext(),
			rightHandLong = (long)stream.ReceiveNext(),
			leftHandLong = (long)stream.ReceiveNext(),
			position = (long)stream.ReceiveNext(),
			handPosition = (int)stream.ReceiveNext(),
			packedFields = (int)stream.ReceiveNext(),
			packedCompetitiveData = (short)stream.ReceiveNext()
		};
		bool flag = (inputStruct.packedFields & 1024) != 0;
		bool flag2 = (inputStruct.packedFields & 32768) != 0;
		if (flag)
		{
			inputStruct.grabbedRopeIndex = (int)stream.ReceiveNext();
			inputStruct.ropeBoneIndex = (int)stream.ReceiveNext();
			inputStruct.ropeGrabIsLeft = (bool)stream.ReceiveNext();
			inputStruct.ropeGrabIsBody = (bool)stream.ReceiveNext();
			inputStruct.ropeGrabOffset = (Vector3)stream.ReceiveNext();
		}
		else if (flag2)
		{
			inputStruct.grabbedRopeIndex = (int)stream.ReceiveNext();
			inputStruct.ropeGrabIsLeft = (bool)stream.ReceiveNext();
			inputStruct.ropeGrabIsBody = (bool)stream.ReceiveNext();
			inputStruct.ropeGrabOffset = (Vector3)stream.ReceiveNext();
		}
		if ((inputStruct.packedFields & 8192) != 0)
		{
			inputStruct.hoverboardPosRot = (long)stream.ReceiveNext();
			inputStruct.hoverboardColor = (short)stream.ReceiveNext();
		}
		inputStruct.serverTimeStamp = info.SentServerTime;
		this.SerializeReadShared(inputStruct);
	}

	// Token: 0x0600172E RID: 5934 RVA: 0x0006F9F0 File Offset: 0x0006DBF0
	public object OnSerializeWrite()
	{
		InputStruct inputStruct = this.SerializeWriteShared();
		double num = NetworkSystem.Instance.SimTick / 1000.0;
		inputStruct.serverTimeStamp = num;
		return inputStruct;
	}

	// Token: 0x0600172F RID: 5935 RVA: 0x0006FA2C File Offset: 0x0006DC2C
	public void OnSerializeRead(object objectData)
	{
		InputStruct inputStruct = (InputStruct)objectData;
		this.SerializeReadShared(inputStruct);
	}

	// Token: 0x06001730 RID: 5936 RVA: 0x0006FA48 File Offset: 0x0006DC48
	private void UpdateExtrapolationTarget()
	{
		float num = (float)(NetworkSystem.Instance.SimTime - this.remoteLatestTimestamp);
		num -= 0.15f;
		num = Mathf.Clamp(num, -0.5f, 0.5f);
		this.syncPos += this.remoteVelocity * num;
		this.remoteCorrectionNeeded = this.syncPos - base.transform.position;
		if (this.remoteCorrectionNeeded.magnitude > 1.5f && this.grabbedRopeIndex <= 0)
		{
			base.transform.position = this.syncPos;
			this.remoteCorrectionNeeded = Vector3.zero;
		}
	}

	// Token: 0x06001731 RID: 5937 RVA: 0x0006FAF4 File Offset: 0x0006DCF4
	private void UpdateRopeData()
	{
		if (this.previousGrabbedRope == this.grabbedRopeIndex && this.previousGrabbedRopeBoneIndex == this.grabbedRopeBoneIndex && this.previousGrabbedRopeWasLeft == this.grabbedRopeIsLeft && this.previousGrabbedRopeWasBody == this.grabbedRopeIsBody)
		{
			return;
		}
		this.ClearRopeData();
		if (this.grabbedRopeIndex != -1)
		{
			GorillaRopeSwing gorillaRopeSwing;
			if (this.grabbedRopeIsPhotonView)
			{
				PhotonView photonView = PhotonView.Find(this.grabbedRopeIndex);
				GorillaClimbable gorillaClimbable;
				HandHoldXSceneRef handHoldXSceneRef;
				VRRigSerializer vrrigSerializer;
				if (photonView.TryGetComponent<GorillaClimbable>(out gorillaClimbable))
				{
					this.currentHoldParent = photonView.transform;
				}
				else if (photonView.TryGetComponent<HandHoldXSceneRef>(out handHoldXSceneRef))
				{
					GameObject targetObject = handHoldXSceneRef.targetObject;
					this.currentHoldParent = ((targetObject != null) ? targetObject.transform : null);
				}
				else if (photonView && photonView.TryGetComponent<VRRigSerializer>(out vrrigSerializer))
				{
					this.currentHoldParent = ((this.grabbedRopeBoneIndex == 1) ? vrrigSerializer.VRRig.leftHandHoldsPlayer.transform : vrrigSerializer.VRRig.rightHandHoldsPlayer.transform);
				}
			}
			else if (RopeSwingManager.instance.TryGetRope(this.grabbedRopeIndex, out gorillaRopeSwing) && gorillaRopeSwing != null)
			{
				if (this.currentRopeSwingTarget == null || this.currentRopeSwingTarget.gameObject == null)
				{
					this.currentRopeSwingTarget = new GameObject("RopeSwingTarget").transform;
				}
				if (gorillaRopeSwing.AttachRemotePlayer(this.creator.ActorNumber, this.grabbedRopeBoneIndex, this.currentRopeSwingTarget, this.grabbedRopeOffset))
				{
					this.currentRopeSwing = gorillaRopeSwing;
				}
				this.lastRopeGrabTimer = 0f;
			}
		}
		else if (this.previousGrabbedRope != -1)
		{
			PhotonView photonView2 = PhotonView.Find(this.previousGrabbedRope);
			VRRigSerializer vrrigSerializer2;
			if (photonView2 && photonView2.TryGetComponent<VRRigSerializer>(out vrrigSerializer2) && vrrigSerializer2.VRRig == VRRig.LocalRig)
			{
				EquipmentInteractor.instance.ForceDropEquipment(this.bodyHolds);
				EquipmentInteractor.instance.ForceDropEquipment(this.leftHolds);
				EquipmentInteractor.instance.ForceDropEquipment(this.rightHolds);
			}
		}
		this.shouldLerpToRope = true;
		this.previousGrabbedRope = this.grabbedRopeIndex;
		this.previousGrabbedRopeBoneIndex = this.grabbedRopeBoneIndex;
		this.previousGrabbedRopeWasLeft = this.grabbedRopeIsLeft;
		this.previousGrabbedRopeWasBody = this.grabbedRopeIsBody;
	}

	// Token: 0x06001732 RID: 5938 RVA: 0x0006FD34 File Offset: 0x0006DF34
	private void UpdateMovingMonkeBlockData()
	{
		if (this.mountedMonkeBlockOffset.sqrMagnitude > 2f)
		{
			this.mountedMovingSurfaceId = -1;
			this.mountedMovingSurfaceIsLeft = false;
			this.mountedMovingSurfaceIsBody = false;
			this.mountedMonkeBlock = null;
			this.mountedMovingSurface = null;
		}
		if (this.prevMovingSurfaceID == this.mountedMovingSurfaceId && this.movingSurfaceWasBody == this.mountedMovingSurfaceIsBody && this.movingSurfaceWasLeft == this.mountedMovingSurfaceIsLeft && this.movingSurfaceWasMonkeBlock == this.movingSurfaceIsMonkeBlock)
		{
			return;
		}
		if (this.mountedMovingSurfaceId == -1)
		{
			this.mountedMovingSurfaceIsLeft = false;
			this.mountedMovingSurfaceIsBody = false;
			this.mountedMonkeBlock = null;
			this.mountedMovingSurface = null;
		}
		else if (this.movingSurfaceIsMonkeBlock)
		{
			this.mountedMonkeBlock = null;
			BuilderTable builderTable;
			if (BuilderTable.TryGetBuilderTableForZone(this.zoneEntity.currentZone, out builderTable))
			{
				this.mountedMonkeBlock = builderTable.GetPiece(this.mountedMovingSurfaceId);
			}
			if (this.mountedMonkeBlock == null)
			{
				this.mountedMovingSurfaceId = -1;
				this.mountedMovingSurfaceIsLeft = false;
				this.mountedMovingSurfaceIsBody = false;
				this.mountedMonkeBlock = null;
				this.mountedMovingSurface = null;
			}
		}
		else if (MovingSurfaceManager.instance == null || !MovingSurfaceManager.instance.TryGetMovingSurface(this.mountedMovingSurfaceId, out this.mountedMovingSurface))
		{
			this.mountedMovingSurfaceId = -1;
			this.mountedMovingSurfaceIsLeft = false;
			this.mountedMovingSurfaceIsBody = false;
			this.mountedMonkeBlock = null;
			this.mountedMovingSurface = null;
		}
		if (this.mountedMovingSurfaceId != -1 && this.prevMovingSurfaceID == -1)
		{
			this.shouldLerpToMovingSurface = true;
			this.lastMountedSurfaceTimer = 0f;
		}
		this.prevMovingSurfaceID = this.mountedMovingSurfaceId;
		this.movingSurfaceWasLeft = this.mountedMovingSurfaceIsLeft;
		this.movingSurfaceWasBody = this.mountedMovingSurfaceIsBody;
		this.movingSurfaceWasMonkeBlock = this.movingSurfaceIsMonkeBlock;
	}

	// Token: 0x06001733 RID: 5939 RVA: 0x0006FEE0 File Offset: 0x0006E0E0
	public static void AttachLocalPlayerToMovingSurface(int blockId, bool isLeft, bool isBody, Vector3 offset, bool isMonkeBlock)
	{
		if (GorillaTagger.hasInstance && GorillaTagger.Instance.offlineVRRig)
		{
			GorillaTagger.Instance.offlineVRRig.mountedMovingSurfaceId = blockId;
			GorillaTagger.Instance.offlineVRRig.mountedMovingSurfaceIsLeft = isLeft;
			GorillaTagger.Instance.offlineVRRig.mountedMovingSurfaceIsBody = isBody;
			GorillaTagger.Instance.offlineVRRig.movingSurfaceIsMonkeBlock = isMonkeBlock;
			GorillaTagger.Instance.offlineVRRig.mountedMonkeBlockOffset = offset;
		}
	}

	// Token: 0x06001734 RID: 5940 RVA: 0x0006FF56 File Offset: 0x0006E156
	public static void DetachLocalPlayerFromMovingSurface()
	{
		if (GorillaTagger.hasInstance && GorillaTagger.Instance.offlineVRRig)
		{
			GorillaTagger.Instance.offlineVRRig.mountedMovingSurfaceId = -1;
		}
	}

	// Token: 0x06001735 RID: 5941 RVA: 0x0006FF80 File Offset: 0x0006E180
	public static void AttachLocalPlayerToPhotonView(PhotonView view, XRNode xrNode, Vector3 offset, Vector3 velocity)
	{
		if (GorillaTagger.hasInstance && GorillaTagger.Instance.offlineVRRig)
		{
			GorillaTagger.Instance.offlineVRRig.grabbedRopeIndex = view.ViewID;
			GorillaTagger.Instance.offlineVRRig.grabbedRopeIsLeft = xrNode == XRNode.LeftHand;
			GorillaTagger.Instance.offlineVRRig.grabbedRopeOffset = offset;
			GorillaTagger.Instance.offlineVRRig.grabbedRopeIsPhotonView = true;
		}
	}

	// Token: 0x06001736 RID: 5942 RVA: 0x0006FFED File Offset: 0x0006E1ED
	public static void DetachLocalPlayerFromPhotonView()
	{
		if (GorillaTagger.hasInstance && GorillaTagger.Instance.offlineVRRig)
		{
			GorillaTagger.Instance.offlineVRRig.grabbedRopeIndex = -1;
		}
	}

	// Token: 0x06001737 RID: 5943 RVA: 0x00070018 File Offset: 0x0006E218
	private void ClearRopeData()
	{
		if (this.currentRopeSwing)
		{
			this.currentRopeSwing.DetachRemotePlayer(this.creator.ActorNumber);
		}
		if (this.currentRopeSwingTarget)
		{
			this.currentRopeSwingTarget.SetParent(null);
		}
		this.currentRopeSwing = null;
		this.currentHoldParent = null;
	}

	// Token: 0x06001738 RID: 5944 RVA: 0x0007006F File Offset: 0x0006E26F
	public void ChangeMaterial(int materialIndex, PhotonMessageInfo info)
	{
		if (info.Sender == PhotonNetwork.MasterClient)
		{
			this.ChangeMaterialLocal(materialIndex);
		}
	}

	// Token: 0x06001739 RID: 5945 RVA: 0x00070088 File Offset: 0x0006E288
	public void UpdateFrozenEffect(bool enable)
	{
		if (this.frozenEffect != null && ((!this.frozenEffect.activeSelf && enable) || (this.frozenEffect.activeSelf && !enable)))
		{
			this.frozenEffect.SetActive(enable);
			if (enable)
			{
				this.frozenTimeElapsed = 0f;
			}
			else
			{
				Vector3 localScale = this.frozenEffect.transform.localScale;
				localScale = new Vector3(localScale.x, this.frozenEffectMinY, localScale.z);
				this.frozenEffect.transform.localScale = localScale;
			}
		}
		if (this.iceCubeLeft != null && ((!this.iceCubeLeft.activeSelf && enable) || (this.iceCubeLeft.activeSelf && !enable)))
		{
			this.iceCubeLeft.SetActive(enable);
		}
		if (this.iceCubeRight != null && ((!this.iceCubeRight.activeSelf && enable) || (this.iceCubeRight.activeSelf && !enable)))
		{
			this.iceCubeRight.SetActive(enable);
		}
	}

	// Token: 0x0600173A RID: 5946 RVA: 0x00070194 File Offset: 0x0006E394
	public void ForceResetFrozenEffect()
	{
		this.frozenEffect.SetActive(false);
		this.iceCubeRight.SetActive(false);
		this.iceCubeLeft.SetActive(false);
	}

	// Token: 0x0600173B RID: 5947 RVA: 0x000701BC File Offset: 0x0006E3BC
	public void ChangeMaterialLocal(int materialIndex)
	{
		if (this.setMatIndex == materialIndex)
		{
			return;
		}
		this.setMatIndex = materialIndex;
		if (this.setMatIndex > -1 && this.setMatIndex < this.materialsToChangeTo.Length)
		{
			this.bodyRenderer.SetMaterialIndex(materialIndex);
		}
		this.UpdateMatParticles(materialIndex);
		if (materialIndex > 0 && VRRig.LocalRig != this)
		{
			this.PlayTaggedEffect();
		}
	}

	// Token: 0x0600173C RID: 5948 RVA: 0x00070220 File Offset: 0x0006E420
	public void PlayTaggedEffect()
	{
		TagEffectPack tagEffectPack = null;
		quaternion quaternion = base.transform.rotation;
		TagEffectsLibrary.EffectType effectType = ((VRRig.LocalRig == this) ? TagEffectsLibrary.EffectType.FIRST_PERSON : TagEffectsLibrary.EffectType.THIRD_PERSON);
		if (GorillaGameManager.instance != null)
		{
			GorillaGameManager.instance.lastTaggedActorNr.TryGetValue(this.OwningNetPlayer.ActorNumber, out this.taggedById);
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(this.taggedById);
		RigContainer rigContainer;
		if (player != null && VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
		{
			tagEffectPack = rigContainer.Rig.CosmeticEffectPack;
			if (tagEffectPack && tagEffectPack.shouldFaceTagger && effectType == TagEffectsLibrary.EffectType.THIRD_PERSON)
			{
				quaternion = Quaternion.LookRotation((rigContainer.Rig.transform.position - base.transform.position).normalized);
			}
		}
		TagEffectsLibrary.PlayEffect(base.transform, false, this.scaleFactor, effectType, this.CosmeticEffectPack, tagEffectPack, quaternion);
	}

	// Token: 0x0600173D RID: 5949 RVA: 0x0007031C File Offset: 0x0006E51C
	public void UpdateMatParticles(int materialIndex)
	{
		if (this.lavaParticleSystem != null)
		{
			if (!this.isOfflineVRRig && materialIndex == 2 && this.lavaParticleSystem.isStopped)
			{
				this.lavaParticleSystem.Play();
			}
			else if (!this.isOfflineVRRig && this.lavaParticleSystem.isPlaying)
			{
				this.lavaParticleSystem.Stop();
			}
		}
		if (this.rockParticleSystem != null)
		{
			if (!this.isOfflineVRRig && materialIndex == 1 && this.rockParticleSystem.isStopped)
			{
				this.rockParticleSystem.Play();
			}
			else if (!this.isOfflineVRRig && this.rockParticleSystem.isPlaying)
			{
				this.rockParticleSystem.Stop();
			}
		}
		if (this.iceParticleSystem != null)
		{
			if (!this.isOfflineVRRig && materialIndex == 3 && this.rockParticleSystem.isStopped)
			{
				this.iceParticleSystem.Play();
			}
			else if (!this.isOfflineVRRig && this.iceParticleSystem.isPlaying)
			{
				this.iceParticleSystem.Stop();
			}
		}
		if (this.snowFlakeParticleSystem != null)
		{
			if (!this.isOfflineVRRig && materialIndex == 14 && this.snowFlakeParticleSystem.isStopped)
			{
				this.snowFlakeParticleSystem.Play();
				return;
			}
			if (!this.isOfflineVRRig && this.snowFlakeParticleSystem.isPlaying)
			{
				this.snowFlakeParticleSystem.Stop();
			}
		}
	}

	// Token: 0x0600173E RID: 5950 RVA: 0x0007047C File Offset: 0x0006E67C
	public void InitializeNoobMaterial(float red, float green, float blue, PhotonMessageInfoWrapped info)
	{
		this.IncrementRPC(info, "RPC_InitializeNoobMaterial");
		NetworkSystem.Instance.GetPlayer(info.senderID);
		string userID = NetworkSystem.Instance.GetUserID(info.senderID);
		if (info.senderID == NetworkSystem.Instance.GetOwningPlayerID(this.rigSerializer.gameObject) && (!this.initialized || (this.initialized && GorillaComputer.instance.friendJoinCollider.playerIDsCurrentlyTouching.Contains(userID)) || (this.initialized && CosmeticWardrobeProximityDetector.IsUserNearWardrobe(userID))))
		{
			this.initialized = true;
			blue = blue.ClampSafe(0f, 1f);
			red = red.ClampSafe(0f, 1f);
			green = green.ClampSafe(0f, 1f);
			this.InitializeNoobMaterialLocal(red, green, blue);
		}
	}

	// Token: 0x0600173F RID: 5951 RVA: 0x00070558 File Offset: 0x0006E758
	public void InitializeNoobMaterialLocal(float red, float green, float blue)
	{
		Color color = new Color(red, green, blue);
		this.EnsureInstantiatedMaterial();
		if (this.myDefaultSkinMaterialInstance != null)
		{
			color.r = Mathf.Clamp(color.r, 0f, 1f);
			color.g = Mathf.Clamp(color.g, 0f, 1f);
			color.b = Mathf.Clamp(color.b, 0f, 1f);
			this.skeleton.UpdateColor(color);
			this.myDefaultSkinMaterialInstance.color = color;
		}
		this.SetColor(color);
		bool flag = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Custom_Nametags);
		this.UpdateName(flag);
	}

	// Token: 0x06001740 RID: 5952 RVA: 0x00070604 File Offset: 0x0006E804
	public void UpdateName(bool isNamePermissionEnabled)
	{
		if (!this.isOfflineVRRig && this.creator != null)
		{
			string text = ((isNamePermissionEnabled && GorillaComputer.instance.NametagsEnabled) ? this.creator.NickName : this.creator.DefaultName);
			this.playerNameVisible = this.NormalizeName(true, text);
		}
		else if (this.showName && NetworkSystem.Instance != null)
		{
			this.playerNameVisible = ((isNamePermissionEnabled && GorillaComputer.instance.NametagsEnabled) ? NetworkSystem.Instance.GetMyNickName() : NetworkSystem.Instance.GetMyDefaultName());
		}
		this.SetNameTagText(this.playerNameVisible);
		if (this.creator != null)
		{
			this.creator.SanitizedNickName = this.playerNameVisible;
		}
		Action onPlayerNameVisibleChanged = this.OnPlayerNameVisibleChanged;
		if (onPlayerNameVisibleChanged == null)
		{
			return;
		}
		onPlayerNameVisibleChanged();
	}

	// Token: 0x06001741 RID: 5953 RVA: 0x000706D2 File Offset: 0x0006E8D2
	public void SetNameTagText(string name)
	{
		this.playerNameVisible = name;
		this.playerText1.text = name;
		this.playerText2.text = name;
		Action<RigContainer> onNameChanged = this.OnNameChanged;
		if (onNameChanged == null)
		{
			return;
		}
		onNameChanged(this.rigContainer);
	}

	// Token: 0x06001742 RID: 5954 RVA: 0x0007070C File Offset: 0x0006E90C
	public void UpdateName()
	{
		Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Custom_Nametags);
		bool flag = (permissionDataByFeature.Enabled || permissionDataByFeature.ManagedBy == Permission.ManagedByEnum.PLAYER) && permissionDataByFeature.ManagedBy != Permission.ManagedByEnum.PROHIBITED;
		this.UpdateName(flag);
	}

	// Token: 0x06001743 RID: 5955 RVA: 0x00070748 File Offset: 0x0006E948
	public string NormalizeName(bool doIt, string text)
	{
		if (doIt)
		{
			if (GorillaComputer.instance.CheckAutoBanListForName(text))
			{
				text = new string(Array.FindAll<char>(text.ToCharArray(), (char c) => Utils.IsASCIILetterOrDigit(c)));
				if (text.Length > 12)
				{
					text = text.Substring(0, 11);
				}
				text = text.ToUpper();
			}
			else
			{
				text = "BADGORILLA";
			}
		}
		return text;
	}

	// Token: 0x06001744 RID: 5956 RVA: 0x000707BF File Offset: 0x0006E9BF
	public void SetJumpLimitLocal(float maxJumpSpeed)
	{
		GTPlayer.Instance.maxJumpSpeed = maxJumpSpeed;
	}

	// Token: 0x06001745 RID: 5957 RVA: 0x000707CC File Offset: 0x0006E9CC
	public void SetJumpMultiplierLocal(float jumpMultiplier)
	{
		GTPlayer.Instance.jumpMultiplier = jumpMultiplier;
	}

	// Token: 0x06001746 RID: 5958 RVA: 0x000707DC File Offset: 0x0006E9DC
	public void RequestMaterialColor(int askingPlayerID, PhotonMessageInfoWrapped info)
	{
		this.IncrementRPC(info, "RequestMaterialColor");
		Player playerRef = ((PunNetPlayer)NetworkSystem.Instance.GetPlayer(info.senderID)).PlayerRef;
		if (this.netView.IsMine)
		{
			this.netView.GetView.RPC("RPC_InitializeNoobMaterial", playerRef, new object[]
			{
				this.myDefaultSkinMaterialInstance.color.r,
				this.myDefaultSkinMaterialInstance.color.g,
				this.myDefaultSkinMaterialInstance.color.b
			});
		}
	}

	// Token: 0x06001747 RID: 5959 RVA: 0x00070884 File Offset: 0x0006EA84
	public void RequestCosmetics(PhotonMessageInfoWrapped info)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		if (this.netView.IsMine && CosmeticsController.hasInstance)
		{
			if (CosmeticsController.instance.isHidingCosmeticsFromRemotePlayers)
			{
				this.netView.SendRPC("RPC_HideAllCosmetics", info.Sender, Array.Empty<object>());
				return;
			}
			int[] array = CosmeticsController.instance.currentWornSet.ToPackedIDArray();
			int[] array2 = CosmeticsController.instance.tryOnSet.ToPackedIDArray();
			this.netView.SendRPC("RPC_UpdateCosmeticsWithTryonPacked", player, new object[] { array, array2 });
		}
	}

	// Token: 0x06001748 RID: 5960 RVA: 0x00070924 File Offset: 0x0006EB24
	public void PlayTagSoundLocal(int soundIndex, float soundVolume, bool stopCurrentAudio)
	{
		if (soundIndex < 0 || soundIndex >= this.clipToPlay.Length)
		{
			return;
		}
		this.tagSound.volume = Mathf.Min(0.25f, soundVolume);
		if (stopCurrentAudio)
		{
			this.tagSound.Stop();
		}
		this.tagSound.GTPlayOneShot(this.clipToPlay[soundIndex], 1f);
	}

	// Token: 0x06001749 RID: 5961 RVA: 0x0007097D File Offset: 0x0006EB7D
	public void AssignDrumToMusicDrums(int drumIndex, AudioSource drum)
	{
		if (drumIndex >= 0 && drumIndex < this.musicDrums.Length && drum != null)
		{
			this.musicDrums[drumIndex] = drum;
		}
	}

	// Token: 0x0600174A RID: 5962 RVA: 0x000709A0 File Offset: 0x0006EBA0
	public void PlayDrum(int drumIndex, float drumVolume, PhotonMessageInfoWrapped info)
	{
		this.IncrementRPC(info, "RPC_PlayDrum");
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		RigContainer rigContainer;
		if (VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
		{
			this.senderRig = rigContainer.Rig;
		}
		if (this.senderRig == null || this.senderRig.muted)
		{
			return;
		}
		if (drumIndex < 0 || drumIndex >= this.musicDrums.Length || (this.senderRig.transform.position - base.transform.position).sqrMagnitude > 9f || !float.IsFinite(drumVolume))
		{
			GorillaNot.instance.SendReport("inappropriate tag data being sent drum", player.UserId, player.NickName);
			return;
		}
		AudioSource audioSource = (this.netView.IsMine ? GorillaTagger.Instance.offlineVRRig.musicDrums[drumIndex] : this.musicDrums[drumIndex]);
		if (!audioSource.gameObject.activeSelf)
		{
			return;
		}
		float instrumentVolume = GorillaComputer.instance.instrumentVolume;
		audioSource.time = 0f;
		audioSource.volume = Mathf.Max(Mathf.Min(instrumentVolume, drumVolume * instrumentVolume), 0f);
		audioSource.GTPlay();
	}

	// Token: 0x0600174B RID: 5963 RVA: 0x00070AD4 File Offset: 0x0006ECD4
	public int AssignInstrumentToInstrumentSelfOnly(TransferrableObject instrument)
	{
		if (instrument == null)
		{
			return -1;
		}
		if (!this.instrumentSelfOnly.Contains(instrument))
		{
			this.instrumentSelfOnly.Add(instrument);
		}
		return this.instrumentSelfOnly.IndexOf(instrument);
	}

	// Token: 0x0600174C RID: 5964 RVA: 0x00070B08 File Offset: 0x0006ED08
	public void PlaySelfOnlyInstrument(int selfOnlyIndex, int noteIndex, float instrumentVol, PhotonMessageInfoWrapped info)
	{
		this.IncrementRPC(info, "RPC_PlaySelfOnlyInstrument");
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		if (player == this.netView.Owner && !this.muted)
		{
			if (selfOnlyIndex >= 0 && selfOnlyIndex < this.instrumentSelfOnly.Count && float.IsFinite(instrumentVol))
			{
				if (this.instrumentSelfOnly[selfOnlyIndex].gameObject.activeSelf)
				{
					this.instrumentSelfOnly[selfOnlyIndex].PlayNote(noteIndex, Mathf.Max(Mathf.Min(GorillaComputer.instance.instrumentVolume, instrumentVol * GorillaComputer.instance.instrumentVolume), 0f) / 2f);
					return;
				}
			}
			else
			{
				GorillaNot.instance.SendReport("inappropriate tag data being sent self only instrument", player.UserId, player.NickName);
			}
		}
	}

	// Token: 0x0600174D RID: 5965 RVA: 0x00070BE4 File Offset: 0x0006EDE4
	public void PlayHandTapLocal(int soundIndex, bool isLeftHand, float tapVolume)
	{
		if (soundIndex > -1 && soundIndex < GTPlayer.Instance.materialData.Count)
		{
			GTPlayer.MaterialData materialData = GTPlayer.Instance.materialData[soundIndex];
			AudioSource audioSource = (isLeftHand ? this.leftHandPlayer : this.rightHandPlayer);
			audioSource.volume = tapVolume;
			AudioClip audioClip = (materialData.overrideAudio ? materialData.audio : GTPlayer.Instance.materialData[0].audio);
			audioSource.GTPlayOneShot(audioClip, 1f);
		}
	}

	// Token: 0x0600174E RID: 5966 RVA: 0x00070C64 File Offset: 0x0006EE64
	internal void OnHandTap(int soundIndex, bool isLeftHand, float handSpeed, Vector3 tapDir)
	{
		if (soundIndex < 0 || soundIndex > GTPlayer.Instance.materialData.Count - 1)
		{
			return;
		}
		if (isLeftHand)
		{
			FXSystem.PlayFX(this.GetLeftHandEffect(soundIndex, handSpeed, tapDir));
			if (CrittersManager.instance.IsNotNull() && CrittersManager.instance.LocalAuthority() && CrittersManager.instance.rigSetupByRig[this].IsNotNull())
			{
				CrittersLoudNoise crittersLoudNoise = (CrittersLoudNoise)CrittersManager.instance.rigSetupByRig[this].rigActors[0].actorSet;
				if (crittersLoudNoise.IsNotNull())
				{
					crittersLoudNoise.PlayHandTapLocal(true);
					return;
				}
			}
		}
		else
		{
			FXSystem.PlayFX(this.GetRightHandEffect(soundIndex, handSpeed, tapDir));
			if (CrittersManager.instance.IsNotNull() && CrittersManager.instance.LocalAuthority() && CrittersManager.instance.rigSetupByRig[this].IsNotNull())
			{
				CrittersLoudNoise crittersLoudNoise2 = (CrittersLoudNoise)CrittersManager.instance.rigSetupByRig[this].rigActors[2].actorSet;
				if (crittersLoudNoise2.IsNotNull())
				{
					crittersLoudNoise2.PlayHandTapLocal(false);
				}
			}
		}
	}

	// Token: 0x0600174F RID: 5967 RVA: 0x00070D93 File Offset: 0x0006EF93
	internal HandEffectContext GetLeftHandEffect(int index, float handSpeed, Vector3 tapDir)
	{
		return this.SetHandEffectData(index, this._leftHandEffect, this.leftHand, handSpeed, tapDir);
	}

	// Token: 0x06001750 RID: 5968 RVA: 0x00070DAA File Offset: 0x0006EFAA
	internal HandEffectContext GetRightHandEffect(int index, float handSpeed, Vector3 tapDir)
	{
		return this.SetHandEffectData(index, this._rightHandEffect, this.rightHand, handSpeed, tapDir);
	}

	// Token: 0x06001751 RID: 5969 RVA: 0x00070DC4 File Offset: 0x0006EFC4
	internal HandEffectContext SetHandEffectData(int index, HandEffectContext effectContext, VRMap targetHand, float handSpeed, Vector3 tapDir)
	{
		GTPlayer.MaterialData handSurfaceData = this.GetHandSurfaceData(index);
		Vector3 vector = tapDir * this.tapPointDistance * this.scaleFactor;
		if (this.isOfflineVRRig)
		{
			Vector3 vector2 = targetHand.rigTarget.rotation * targetHand.trackingPositionOffset * this.scaleFactor;
			effectContext.position = targetHand.rigTarget.position - vector2 + vector;
		}
		else
		{
			Quaternion quaternion = targetHand.rigTarget.parent.rotation * targetHand.syncRotation;
			Vector3 vector3 = this.netSyncPos.GetPredictedFuture() - base.transform.position;
			Vector3 vector2 = quaternion * targetHand.trackingPositionOffset * this.scaleFactor;
			effectContext.position = targetHand.rigTarget.parent.TransformPoint(targetHand.netSyncPos.GetPredictedFuture()) - vector2 + vector + vector3;
		}
		int[] prefabHashes = effectContext.prefabHashes;
		int num = 0;
		HashWrapper hashWrapper = GTPlayer.Instance.materialDatasSO.surfaceEffects[handSurfaceData.surfaceEffectIndex];
		prefabHashes[num] = in hashWrapper;
		effectContext.prefabHashes[1] = ((RoomSystem.JoinedRoom && global::GorillaGameModes.GameMode.ActiveGameMode.IsNotNull()) ? global::GorillaGameModes.GameMode.ActiveGameMode.SpecialHandFX(this.creator, this.rigContainer) : (-1));
		effectContext.soundFX = handSurfaceData.audio;
		effectContext.clipVolume = handSpeed * this.handSpeedToVolumeModifier;
		effectContext.handSpeed = handSpeed;
		return effectContext;
	}

	// Token: 0x06001752 RID: 5970 RVA: 0x00070F44 File Offset: 0x0006F144
	internal GTPlayer.MaterialData GetHandSurfaceData(int index)
	{
		GTPlayer.MaterialData materialData = GTPlayer.Instance.materialData[index];
		if (!materialData.overrideAudio)
		{
			materialData = GTPlayer.Instance.materialData[0];
		}
		return materialData;
	}

	// Token: 0x06001753 RID: 5971 RVA: 0x00070F7C File Offset: 0x0006F17C
	public void PlaySplashEffect(Vector3 splashPosition, Quaternion splashRotation, float splashScale, float boundingRadius, bool bigSplash, bool enteringWater, PhotonMessageInfoWrapped info)
	{
		this.IncrementRPC(info, "RPC_PlaySplashEffect");
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		if (player == this.netView.Owner)
		{
			float num = 10000f;
			if ((in splashPosition).IsValid(in num) && (in splashRotation).IsValid() && float.IsFinite(splashScale) && float.IsFinite(boundingRadius))
			{
				if ((base.transform.position - splashPosition).sqrMagnitude >= 9f)
				{
					return;
				}
				float time = Time.time;
				int num2 = -1;
				float num3 = time + 10f;
				for (int i = 0; i < this.splashEffectTimes.Length; i++)
				{
					if (this.splashEffectTimes[i] < num3)
					{
						num3 = this.splashEffectTimes[i];
						num2 = i;
					}
				}
				if (time - 0.5f > num3)
				{
					this.splashEffectTimes[num2] = time;
					boundingRadius = Mathf.Clamp(boundingRadius, 0.0001f, 0.5f);
					ObjectPools.instance.Instantiate(GTPlayer.Instance.waterParams.rippleEffect, splashPosition, splashRotation, GTPlayer.Instance.waterParams.rippleEffectScale * boundingRadius * 2f, true);
					splashScale = Mathf.Clamp(splashScale, 1E-05f, 1f);
					ObjectPools.instance.Instantiate(GTPlayer.Instance.waterParams.splashEffect, splashPosition, splashRotation, splashScale, true).GetComponent<WaterSplashEffect>().PlayEffect(bigSplash, enteringWater, splashScale, null);
					return;
				}
				return;
			}
		}
		GorillaNot.instance.SendReport("inappropriate tag data being sent splash effect", player.UserId, player.NickName);
	}

	// Token: 0x06001754 RID: 5972 RVA: 0x00071118 File Offset: 0x0006F318
	[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
	public void RPC_EnableNonCosmeticHandItem(bool enable, bool isLeftHand, RpcInfo info = default(RpcInfo))
	{
		PhotonMessageInfoWrapped photonMessageInfoWrapped = new PhotonMessageInfoWrapped(info);
		this.IncrementRPC(photonMessageInfoWrapped, "EnableNonCosmeticHandItem");
		if (photonMessageInfoWrapped.Sender == this.creator)
		{
			this.senderRig = GorillaGameManager.StaticFindRigForPlayer(photonMessageInfoWrapped.Sender);
			if (this.senderRig == null)
			{
				return;
			}
			if (isLeftHand && this.nonCosmeticLeftHandItem)
			{
				this.senderRig.nonCosmeticLeftHandItem.EnableItem(enable);
				return;
			}
			if (!isLeftHand && this.nonCosmeticRightHandItem)
			{
				this.senderRig.nonCosmeticRightHandItem.EnableItem(enable);
				return;
			}
		}
		else
		{
			GorillaNot.instance.SendReport("inappropriate tag data being sent Enable Non Cosmetic Hand Item", photonMessageInfoWrapped.Sender.UserId, photonMessageInfoWrapped.Sender.NickName);
		}
	}

	// Token: 0x06001755 RID: 5973 RVA: 0x000711D8 File Offset: 0x0006F3D8
	[PunRPC]
	public void EnableNonCosmeticHandItemRPC(bool enable, bool isLeftHand, PhotonMessageInfoWrapped info)
	{
		NetPlayer sender = info.Sender;
		this.IncrementRPC(info, "EnableNonCosmeticHandItem");
		if (sender == this.netView.Owner)
		{
			this.senderRig = GorillaGameManager.StaticFindRigForPlayer(sender);
			if (this.senderRig == null)
			{
				return;
			}
			if (isLeftHand && this.nonCosmeticLeftHandItem)
			{
				this.senderRig.nonCosmeticLeftHandItem.EnableItem(enable);
				return;
			}
			if (!isLeftHand && this.nonCosmeticRightHandItem)
			{
				this.senderRig.nonCosmeticRightHandItem.EnableItem(enable);
				return;
			}
		}
		else
		{
			GorillaNot.instance.SendReport("inappropriate tag data being sent Enable Non Cosmetic Hand Item", info.Sender.UserId, info.Sender.NickName);
		}
	}

	// Token: 0x06001756 RID: 5974 RVA: 0x00071290 File Offset: 0x0006F490
	public bool IsMakingFistLeft()
	{
		if (this.isOfflineVRRig)
		{
			return ControllerInputPoller.GripFloat(XRNode.LeftHand) > 0.25f && ControllerInputPoller.TriggerFloat(XRNode.LeftHand) > 0.25f;
		}
		return this.leftIndex.calcT > 0.25f && this.leftMiddle.calcT > 0.25f;
	}

	// Token: 0x06001757 RID: 5975 RVA: 0x000712E8 File Offset: 0x0006F4E8
	public bool IsMakingFistRight()
	{
		if (this.isOfflineVRRig)
		{
			return ControllerInputPoller.GripFloat(XRNode.RightHand) > 0.25f && ControllerInputPoller.TriggerFloat(XRNode.RightHand) > 0.25f;
		}
		return this.rightIndex.calcT > 0.25f && this.rightMiddle.calcT > 0.25f;
	}

	// Token: 0x06001758 RID: 5976 RVA: 0x00071340 File Offset: 0x0006F540
	public bool IsMakingFiveLeft()
	{
		if (this.isOfflineVRRig)
		{
			return ControllerInputPoller.GripFloat(XRNode.LeftHand) < 0.25f && ControllerInputPoller.TriggerFloat(XRNode.LeftHand) < 0.25f;
		}
		return this.leftIndex.calcT < 0.25f && this.leftMiddle.calcT < 0.25f;
	}

	// Token: 0x06001759 RID: 5977 RVA: 0x00071398 File Offset: 0x0006F598
	public bool IsMakingFiveRight()
	{
		if (this.isOfflineVRRig)
		{
			return ControllerInputPoller.GripFloat(XRNode.RightHand) < 0.25f && ControllerInputPoller.TriggerFloat(XRNode.RightHand) < 0.25f;
		}
		return this.rightIndex.calcT < 0.25f && this.rightMiddle.calcT < 0.25f;
	}

	// Token: 0x0600175A RID: 5978 RVA: 0x000713F0 File Offset: 0x0006F5F0
	public VRMap GetMakingFist(bool debug, out bool isLeftHand)
	{
		if (this.IsMakingFistRight())
		{
			isLeftHand = false;
			return this.rightHand;
		}
		if (this.IsMakingFistLeft())
		{
			isLeftHand = true;
			return this.leftHand;
		}
		isLeftHand = false;
		return null;
	}

	// Token: 0x0600175B RID: 5979 RVA: 0x0007141C File Offset: 0x0006F61C
	public void PlayGeodeEffect(Vector3 hitPosition)
	{
		if ((base.transform.position - hitPosition).sqrMagnitude < 9f && this.geodeCrackingSound)
		{
			this.geodeCrackingSound.GTPlay();
		}
	}

	// Token: 0x0600175C RID: 5980 RVA: 0x00071464 File Offset: 0x0006F664
	public void PlayClimbSound(AudioClip clip, bool isLeftHand)
	{
		if (isLeftHand)
		{
			this.leftHandPlayer.volume = 0.1f;
			this.leftHandPlayer.clip = clip;
			this.leftHandPlayer.GTPlayOneShot(this.leftHandPlayer.clip, 1f);
			return;
		}
		this.rightHandPlayer.volume = 0.1f;
		this.rightHandPlayer.clip = clip;
		this.rightHandPlayer.GTPlayOneShot(this.rightHandPlayer.clip, 1f);
	}

	// Token: 0x0600175D RID: 5981 RVA: 0x000714E4 File Offset: 0x0006F6E4
	public void HideAllCosmetics(PhotonMessageInfo info)
	{
		this.IncrementRPC(info, "HideAllCosmetics");
		if (NetworkSystem.Instance.GetPlayer(info.Sender) == this.netView.Owner)
		{
			this.LocalUpdateCosmeticsWithTryon(CosmeticsController.CosmeticSet.EmptySet, CosmeticsController.CosmeticSet.EmptySet);
			return;
		}
		GorillaNot.instance.SendReport("inappropriate tag data being sent update cosmetics", info.Sender.UserId, info.Sender.NickName);
	}

	// Token: 0x0600175E RID: 5982 RVA: 0x00071554 File Offset: 0x0006F754
	public void UpdateCosmetics(string[] currentItems, PhotonMessageInfoWrapped info)
	{
		this.IncrementRPC(info, "RPC_UpdateCosmetics");
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		if (info.Sender == this.netView.Owner && currentItems.Length <= 16)
		{
			CosmeticsController.CosmeticSet cosmeticSet = new CosmeticsController.CosmeticSet(currentItems, CosmeticsController.instance);
			this.LocalUpdateCosmetics(cosmeticSet);
			return;
		}
		GorillaNot.instance.SendReport("inappropriate tag data being sent update cosmetics", player.UserId, player.NickName);
	}

	// Token: 0x0600175F RID: 5983 RVA: 0x000715CC File Offset: 0x0006F7CC
	public void UpdateCosmeticsWithTryon(string[] currentItems, string[] tryOnItems, PhotonMessageInfoWrapped info)
	{
		this.IncrementRPC(info, "RPC_UpdateCosmeticsWithTryon");
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		if (info.Sender == this.netView.Owner && currentItems.Length == 16 && tryOnItems.Length == 16)
		{
			CosmeticsController.CosmeticSet cosmeticSet = new CosmeticsController.CosmeticSet(currentItems, CosmeticsController.instance);
			CosmeticsController.CosmeticSet cosmeticSet2 = new CosmeticsController.CosmeticSet(tryOnItems, CosmeticsController.instance);
			this.LocalUpdateCosmeticsWithTryon(cosmeticSet, cosmeticSet2);
			return;
		}
		GorillaNot.instance.SendReport("inappropriate tag data being sent update cosmetics with tryon", player.UserId, player.NickName);
	}

	// Token: 0x06001760 RID: 5984 RVA: 0x0007165C File Offset: 0x0006F85C
	public void UpdateCosmeticsWithTryon(int[] currentItemsPacked, int[] tryOnItemsPacked, PhotonMessageInfoWrapped info)
	{
		this.IncrementRPC(info, "RPC_UpdateCosmeticsWithTryon");
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		if (info.Sender == this.netView.Owner && CosmeticsController.instance.ValidatePackedItems(currentItemsPacked) && CosmeticsController.instance.ValidatePackedItems(tryOnItemsPacked))
		{
			CosmeticsController.CosmeticSet cosmeticSet = new CosmeticsController.CosmeticSet(currentItemsPacked, CosmeticsController.instance);
			CosmeticsController.CosmeticSet cosmeticSet2 = new CosmeticsController.CosmeticSet(tryOnItemsPacked, CosmeticsController.instance);
			this.LocalUpdateCosmeticsWithTryon(cosmeticSet, cosmeticSet2);
			return;
		}
		GorillaNot.instance.SendReport("inappropriate tag data being sent update cosmetics with tryon", player.UserId, player.NickName);
	}

	// Token: 0x06001761 RID: 5985 RVA: 0x000716FA File Offset: 0x0006F8FA
	public void LocalUpdateCosmetics(CosmeticsController.CosmeticSet newSet)
	{
		this.cosmeticSet = newSet;
		if (this.InitializedCosmetics)
		{
			this.SetCosmeticsActive();
		}
	}

	// Token: 0x06001762 RID: 5986 RVA: 0x00071711 File Offset: 0x0006F911
	public void LocalUpdateCosmeticsWithTryon(CosmeticsController.CosmeticSet newSet, CosmeticsController.CosmeticSet newTryOnSet)
	{
		this.cosmeticSet = newSet;
		this.tryOnSet = newTryOnSet;
		if (this.initializedCosmetics)
		{
			this.SetCosmeticsActive();
		}
	}

	// Token: 0x06001763 RID: 5987 RVA: 0x0007172F File Offset: 0x0006F92F
	private void CheckForEarlyAccess()
	{
		if (this.concatStringOfCosmeticsAllowed.Contains("Early Access Supporter Pack"))
		{
			this.concatStringOfCosmeticsAllowed += "LBAAE.LFAAM.LFAAN.LHAAA.LHAAK.LHAAL.LHAAM.LHAAN.LHAAO.LHAAP.LHABA.LHABB.";
		}
		this.InitializedCosmetics = true;
	}

	// Token: 0x06001764 RID: 5988 RVA: 0x00071760 File Offset: 0x0006F960
	public void SetCosmeticsActive()
	{
		if (CosmeticsController.instance == null || !CosmeticsV2Spawner_Dirty.allPartsInstantiated)
		{
			return;
		}
		this.prevSet.CopyItems(this.mergedSet);
		this.mergedSet.MergeSets(this.inTryOnRoom ? this.tryOnSet : null, this.cosmeticSet);
		BodyDockPositions component = base.GetComponent<BodyDockPositions>();
		this.mergedSet.ActivateCosmetics(this.prevSet, this, component, this.cosmeticsObjectRegistry);
	}

	// Token: 0x06001765 RID: 5989 RVA: 0x000717D8 File Offset: 0x0006F9D8
	public void GetCosmeticsPlayFabCatalogData()
	{
		if (CosmeticsController.instance != null)
		{
			PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), delegate(GetUserInventoryResult result)
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				foreach (ItemInstance itemInstance in result.Inventory)
				{
					if (!dictionary.ContainsKey(itemInstance.ItemId))
					{
						dictionary[itemInstance.ItemId] = itemInstance.ItemId;
						if (itemInstance.CatalogVersion == CosmeticsController.instance.catalog)
						{
							this.concatStringOfCosmeticsAllowed += itemInstance.ItemId;
						}
					}
				}
				if (CosmeticsV2Spawner_Dirty.allPartsInstantiated)
				{
					this.Handle_CosmeticsV2_OnPostInstantiateAllPrefabs_DoEnableAllCosmetics();
				}
			}, delegate(PlayFabError error)
			{
				this.initializedCosmetics = true;
				if (CosmeticsV2Spawner_Dirty.allPartsInstantiated)
				{
					this.SetCosmeticsActive();
				}
			}, null, null);
		}
		this.concatStringOfCosmeticsAllowed += "Slingshot";
		this.concatStringOfCosmeticsAllowed += BuilderSetManager.instance.GetStarterSetsConcat();
	}

	// Token: 0x06001766 RID: 5990 RVA: 0x0007184C File Offset: 0x0006FA4C
	public void GenerateFingerAngleLookupTables()
	{
		this.GenerateTableIndex(ref this.leftIndex);
		this.GenerateTableIndex(ref this.rightIndex);
		this.GenerateTableMiddle(ref this.leftMiddle);
		this.GenerateTableMiddle(ref this.rightMiddle);
		this.GenerateTableThumb(ref this.leftThumb);
		this.GenerateTableThumb(ref this.rightThumb);
	}

	// Token: 0x06001767 RID: 5991 RVA: 0x000718A4 File Offset: 0x0006FAA4
	private void GenerateTableThumb(ref VRMapThumb thumb)
	{
		thumb.angle1Table = new Quaternion[11];
		thumb.angle2Table = new Quaternion[11];
		for (int i = 0; i < thumb.angle1Table.Length; i++)
		{
			Debug.Log((float)i / 10f);
			thumb.angle1Table[i] = Quaternion.Lerp(Quaternion.Euler(thumb.startingAngle1), Quaternion.Euler(thumb.closedAngle1), (float)i / 10f);
			thumb.angle2Table[i] = Quaternion.Lerp(Quaternion.Euler(thumb.startingAngle2), Quaternion.Euler(thumb.closedAngle2), (float)i / 10f);
		}
	}

	// Token: 0x06001768 RID: 5992 RVA: 0x0007195C File Offset: 0x0006FB5C
	private void GenerateTableIndex(ref VRMapIndex index)
	{
		index.angle1Table = new Quaternion[11];
		index.angle2Table = new Quaternion[11];
		index.angle3Table = new Quaternion[11];
		for (int i = 0; i < index.angle1Table.Length; i++)
		{
			index.angle1Table[i] = Quaternion.Lerp(Quaternion.Euler(index.startingAngle1), Quaternion.Euler(index.closedAngle1), (float)i / 10f);
			index.angle2Table[i] = Quaternion.Lerp(Quaternion.Euler(index.startingAngle2), Quaternion.Euler(index.closedAngle2), (float)i / 10f);
			index.angle3Table[i] = Quaternion.Lerp(Quaternion.Euler(index.startingAngle3), Quaternion.Euler(index.closedAngle3), (float)i / 10f);
		}
	}

	// Token: 0x06001769 RID: 5993 RVA: 0x00071A44 File Offset: 0x0006FC44
	private void GenerateTableMiddle(ref VRMapMiddle middle)
	{
		middle.angle1Table = new Quaternion[11];
		middle.angle2Table = new Quaternion[11];
		middle.angle3Table = new Quaternion[11];
		for (int i = 0; i < middle.angle1Table.Length; i++)
		{
			middle.angle1Table[i] = Quaternion.Lerp(Quaternion.Euler(middle.startingAngle1), Quaternion.Euler(middle.closedAngle1), (float)i / 10f);
			middle.angle2Table[i] = Quaternion.Lerp(Quaternion.Euler(middle.startingAngle2), Quaternion.Euler(middle.closedAngle2), (float)i / 10f);
			middle.angle3Table[i] = Quaternion.Lerp(Quaternion.Euler(middle.startingAngle3), Quaternion.Euler(middle.closedAngle3), (float)i / 10f);
		}
	}

	// Token: 0x0600176A RID: 5994 RVA: 0x00071B2C File Offset: 0x0006FD2C
	private Quaternion SanitizeQuaternion(Quaternion quat)
	{
		if (float.IsNaN(quat.w) || float.IsNaN(quat.x) || float.IsNaN(quat.y) || float.IsNaN(quat.z) || float.IsInfinity(quat.w) || float.IsInfinity(quat.x) || float.IsInfinity(quat.y) || float.IsInfinity(quat.z))
		{
			return Quaternion.identity;
		}
		return quat;
	}

	// Token: 0x0600176B RID: 5995 RVA: 0x00071BA8 File Offset: 0x0006FDA8
	private Vector3 SanitizeVector3(Vector3 vec)
	{
		if (float.IsNaN(vec.x) || float.IsNaN(vec.y) || float.IsNaN(vec.z) || float.IsInfinity(vec.x) || float.IsInfinity(vec.y) || float.IsInfinity(vec.z))
		{
			return Vector3.zero;
		}
		return Vector3.ClampMagnitude(vec, 5000f);
	}

	// Token: 0x0600176C RID: 5996 RVA: 0x00071C14 File Offset: 0x0006FE14
	private void IncrementRPC(PhotonMessageInfoWrapped info, string sourceCall)
	{
		if (GorillaGameManager.instance != null)
		{
			GorillaNot.IncrementRPCCall(info, sourceCall);
		}
	}

	// Token: 0x0600176D RID: 5997 RVA: 0x00071C2A File Offset: 0x0006FE2A
	private void IncrementRPC(PhotonMessageInfo info, string sourceCall)
	{
		if (GorillaGameManager.instance != null)
		{
			GorillaNot.IncrementRPCCall(info, sourceCall);
		}
	}

	// Token: 0x0600176E RID: 5998 RVA: 0x00071C40 File Offset: 0x0006FE40
	private void AddVelocityToQueue(Vector3 position, double serverTime)
	{
		Vector3 vector = Vector3.zero;
		if (this.velocityHistoryList.Count > 0)
		{
			double num = Utils.CalculateNetworkDeltaTime(this.velocityHistoryList[0].time, serverTime);
			if (num == 0.0)
			{
				return;
			}
			vector = (position - this.lastPosition) / (float)num;
		}
		this.velocityHistoryList.Add(new VRRig.VelocityTime(vector, serverTime));
		this.lastPosition = position;
	}

	// Token: 0x0600176F RID: 5999 RVA: 0x00071CB4 File Offset: 0x0006FEB4
	private Vector3 ReturnVelocityAtTime(double timeToReturn)
	{
		if (this.velocityHistoryList.Count <= 1)
		{
			return Vector3.zero;
		}
		int num = 0;
		int num2 = this.velocityHistoryList.Count - 1;
		int num3 = 0;
		if (num2 == num)
		{
			return this.velocityHistoryList[num].vel;
		}
		while (num2 - num > 1 && num3 < 1000)
		{
			num3++;
			int num4 = (num2 - num) / 2;
			if (this.velocityHistoryList[num4].time > timeToReturn)
			{
				num2 = num4;
			}
			else
			{
				num = num4;
			}
		}
		float num5 = (float)(this.velocityHistoryList[num].time - timeToReturn);
		double num6 = this.velocityHistoryList[num].time - this.velocityHistoryList[num2].time;
		if (num6 == 0.0)
		{
			num6 = 0.001;
		}
		num5 /= (float)num6;
		num5 = Mathf.Clamp(num5, 0f, 1f);
		return Vector3.Lerp(this.velocityHistoryList[num].vel, this.velocityHistoryList[num2].vel, num5);
	}

	// Token: 0x06001770 RID: 6000 RVA: 0x00071DC6 File Offset: 0x0006FFC6
	public Vector3 LatestVelocity()
	{
		if (this.velocityHistoryList.Count > 0)
		{
			return this.velocityHistoryList[0].vel;
		}
		return Vector3.zero;
	}

	// Token: 0x06001771 RID: 6001 RVA: 0x00071DED File Offset: 0x0006FFED
	public bool IsPositionInRange(Vector3 position, float range)
	{
		return (this.syncPos - position).IsShorterThan(range * this.scaleFactor);
	}

	// Token: 0x06001772 RID: 6002 RVA: 0x00071E08 File Offset: 0x00070008
	public bool CheckTagDistanceRollback(VRRig otherRig, float max, float timeInterval)
	{
		Vector3 vector;
		Vector3 vector2;
		GorillaMath.LineSegClosestPoints(this.syncPos, -this.LatestVelocity() * timeInterval, otherRig.syncPos, -otherRig.LatestVelocity() * timeInterval, out vector, out vector2);
		return Vector3.SqrMagnitude(vector - vector2) < max * max * this.scaleFactor;
	}

	// Token: 0x06001773 RID: 6003 RVA: 0x00071E64 File Offset: 0x00070064
	public Vector3 ClampVelocityRelativeToPlayerSafe(Vector3 inVel, float max)
	{
		max *= this.scaleFactor;
		Vector3 vector = Vector3.zero;
		(ref vector).SetValueSafe(in inVel);
		Vector3 vector2 = ((this.velocityHistoryList.Count > 0) ? this.velocityHistoryList[0].vel : Vector3.zero);
		Vector3 vector3 = vector - vector2;
		vector3 = Vector3.ClampMagnitude(vector3, max);
		vector = vector2 + vector3;
		return vector;
	}

	// Token: 0x14000047 RID: 71
	// (add) Token: 0x06001774 RID: 6004 RVA: 0x00071ECC File Offset: 0x000700CC
	// (remove) Token: 0x06001775 RID: 6005 RVA: 0x00071F04 File Offset: 0x00070104
	public event Action<Color> OnColorChanged;

	// Token: 0x14000048 RID: 72
	// (add) Token: 0x06001776 RID: 6006 RVA: 0x00071F3C File Offset: 0x0007013C
	// (remove) Token: 0x06001777 RID: 6007 RVA: 0x00071F74 File Offset: 0x00070174
	public event Action OnPlayerNameVisibleChanged;

	// Token: 0x06001778 RID: 6008 RVA: 0x00071FAC File Offset: 0x000701AC
	public void SetColor(Color color)
	{
		this.skeleton.UpdateColor(color);
		Action<Color> onColorChanged = this.OnColorChanged;
		if (onColorChanged != null)
		{
			onColorChanged(color);
		}
		Action<Color> action = this.onColorInitialized;
		if (action != null)
		{
			action(color);
		}
		this.onColorInitialized = delegate(Color color1)
		{
		};
		this.colorInitialized = true;
		this.playerColor = color;
		if (this.OnDataChange != null)
		{
			this.OnDataChange();
		}
	}

	// Token: 0x06001779 RID: 6009 RVA: 0x0007202F File Offset: 0x0007022F
	public void OnColorInitialized(Action<Color> action)
	{
		if (this.colorInitialized)
		{
			action(this.playerColor);
			return;
		}
		this.onColorInitialized = (Action<Color>)Delegate.Combine(this.onColorInitialized, action);
	}

	// Token: 0x14000049 RID: 73
	// (add) Token: 0x0600177A RID: 6010 RVA: 0x00072060 File Offset: 0x00070260
	// (remove) Token: 0x0600177B RID: 6011 RVA: 0x00072098 File Offset: 0x00070298
	public event Action<int> OnQuestScoreChanged;

	// Token: 0x0600177C RID: 6012 RVA: 0x000720D0 File Offset: 0x000702D0
	public void SetQuestScore(int score)
	{
		this.SetQuestScoreLocal(score);
		Action<int> onQuestScoreChanged = this.OnQuestScoreChanged;
		if (onQuestScoreChanged != null)
		{
			onQuestScoreChanged(this.currentQuestScore);
		}
		if (this.netView != null)
		{
			this.netView.SendRPC("RPC_UpdateQuestScore", RpcTarget.All, new object[] { this.currentQuestScore });
		}
	}

	// Token: 0x0600177D RID: 6013 RVA: 0x0007212E File Offset: 0x0007032E
	public int GetCurrentQuestScore()
	{
		if (!this._scoreUpdated)
		{
			this.SetQuestScoreLocal(ProgressionController.TotalPoints);
		}
		return this.currentQuestScore;
	}

	// Token: 0x0600177E RID: 6014 RVA: 0x00072149 File Offset: 0x00070349
	private void SetQuestScoreLocal(int score)
	{
		this.currentQuestScore = score;
		this._scoreUpdated = true;
	}

	// Token: 0x0600177F RID: 6015 RVA: 0x0007215C File Offset: 0x0007035C
	public void UpdateQuestScore(int score, PhotonMessageInfoWrapped info)
	{
		this.IncrementRPC(info, "UpdateQuestScore");
		if (NetworkSystem.Instance.GetPlayer(info.senderID) == this.netView.Owner)
		{
			if (!this.updateQuestCallLimit.CheckCallTime(Time.time))
			{
				return;
			}
			if (score < this.currentQuestScore)
			{
				return;
			}
			this.SetQuestScoreLocal(score);
			Action<int> onQuestScoreChanged = this.OnQuestScoreChanged;
			if (onQuestScoreChanged == null)
			{
				return;
			}
			onQuestScoreChanged(this.currentQuestScore);
		}
	}

	// Token: 0x06001780 RID: 6016 RVA: 0x000721CC File Offset: 0x000703CC
	public void RequestQuestScore(PhotonMessageInfoWrapped info)
	{
		this.IncrementRPC(info, "RequestQuestScore");
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		if (player.CheckSingleCallRPC(NetPlayer.SingleCallRPC.RequestQuestScore))
		{
			return;
		}
		player.ReceivedSingleCallRPC(NetPlayer.SingleCallRPC.RequestQuestScore);
		if (this.netView.IsMine)
		{
			this.netView.SendRPC("RPC_UpdateQuestScore", player, new object[] { this.currentQuestScore });
		}
	}

	// Token: 0x06001781 RID: 6017 RVA: 0x0007223C File Offset: 0x0007043C
	public void OnEnable()
	{
		EyeScannerMono.Register(this);
		GorillaComputer.RegisterOnNametagSettingChanged(new Action<bool>(this.UpdateName));
		if (this.currentRopeSwingTarget != null)
		{
			this.currentRopeSwingTarget.SetParent(null);
		}
		if (!this.isOfflineVRRig)
		{
			PlayerCosmeticsSystem.RegisterCosmeticCallback(this.creator.ActorNumber, this);
		}
		this.bodyRenderer.SetDefaults();
		this.SetInvisibleToLocalPlayer(false);
		if (this.isOfflineVRRig)
		{
			HandHold.HandPositionRequestOverride += this.HandHold_HandPositionRequestOverride;
			HandHold.HandPositionReleaseOverride += this.HandHold_HandPositionReleaseOverride;
			return;
		}
		VRRigJobManager.Instance.RegisterVRRig(this);
	}

	// Token: 0x06001782 RID: 6018 RVA: 0x000722DC File Offset: 0x000704DC
	void IPreDisable.PreDisable()
	{
		try
		{
			this.ClearRopeData();
			if (this.currentRopeSwingTarget)
			{
				this.currentRopeSwingTarget.SetParent(base.transform);
			}
			this.EnableHuntWatch(false);
			this.EnablePaintbrawlCosmetics(false);
			this.ClearPartyMemberStatus();
			this.concatStringOfCosmeticsAllowed = "";
			this.rawCosmeticString = "";
			if (this.cosmeticSet != null)
			{
				this.mergedSet.DeactivateAllCosmetcs(this.myBodyDockPositions, CosmeticsController.instance.nullItem, this.cosmeticsObjectRegistry);
				this.mergedSet.ClearSet(CosmeticsController.instance.nullItem);
				this.prevSet.ClearSet(CosmeticsController.instance.nullItem);
				this.tryOnSet.ClearSet(CosmeticsController.instance.nullItem);
				this.cosmeticSet.ClearSet(CosmeticsController.instance.nullItem);
			}
			if (!this.isOfflineVRRig)
			{
				PlayerCosmeticsSystem.RemoveCosmeticCallback(this.creator.ActorNumber);
				this.pendingCosmeticUpdate = true;
			}
		}
		catch (Exception)
		{
		}
	}

	// Token: 0x06001783 RID: 6019 RVA: 0x000723F4 File Offset: 0x000705F4
	public void OnDisable()
	{
		try
		{
			GorillaSkin.ApplyToRig(this, null, GorillaSkin.SkinType.gameMode);
			this.ChangeMaterialLocal(0);
			GorillaComputer.UnregisterOnNametagSettingChanged(new Action<bool>(this.UpdateName));
			this.netView = null;
			this.voiceAudio = null;
			this.muted = false;
			this.initialized = false;
			this.initializedCosmetics = false;
			this.inTryOnRoom = false;
			this.timeSpawned = 0f;
			this.setMatIndex = 0;
			this.currentCosmeticTries = 0;
			this.velocityHistoryList.Clear();
			this.netSyncPos.Reset();
			this.rightHand.netSyncPos.Reset();
			this.leftHand.netSyncPos.Reset();
			this.ForceResetFrozenEffect();
			this.nativeScale = (this.frameScale = (this.lastScaleFactor = 1f));
			base.transform.localScale = Vector3.one;
			this.currentQuestScore = 0;
			this._scoreUpdated = false;
			this.TemporaryCosmeticEffects.Clear();
			try
			{
				CallLimitType<CallLimiter>[] callSettings = this.fxSettings.callSettings;
				for (int i = 0; i < callSettings.Length; i++)
				{
					callSettings[i].CallLimitSettings.Reset();
				}
			}
			catch
			{
				Debug.LogError("fxtype missing in fxSettings, please fix or remove this");
			}
		}
		catch (Exception)
		{
		}
		if (this.isOfflineVRRig)
		{
			HandHold.HandPositionRequestOverride -= this.HandHold_HandPositionRequestOverride;
			HandHold.HandPositionReleaseOverride -= this.HandHold_HandPositionReleaseOverride;
		}
		else
		{
			VRRigJobManager.Instance.DeregisterVRRig(this);
		}
		EyeScannerMono.Unregister(this);
		this.creator = null;
	}

	// Token: 0x06001784 RID: 6020 RVA: 0x0007259C File Offset: 0x0007079C
	private void HandHold_HandPositionReleaseOverride(HandHold hh, bool rightHand)
	{
		if (rightHand)
		{
			this.rightHand.handholdOverrideTarget = null;
			return;
		}
		this.leftHand.handholdOverrideTarget = null;
	}

	// Token: 0x06001785 RID: 6021 RVA: 0x000725BA File Offset: 0x000707BA
	private void HandHold_HandPositionRequestOverride(HandHold hh, bool rightHand, Vector3 pos)
	{
		if (rightHand)
		{
			this.rightHand.handholdOverrideTarget = hh.transform;
			this.rightHand.handholdOverrideTargetOffset = pos;
			return;
		}
		this.leftHand.handholdOverrideTarget = hh.transform;
		this.leftHand.handholdOverrideTargetOffset = pos;
	}

	// Token: 0x06001786 RID: 6022 RVA: 0x000725FC File Offset: 0x000707FC
	public void NetInitialize()
	{
		this.timeSpawned = Time.time;
		if (NetworkSystem.Instance.InRoom)
		{
			GorillaGameManager instance = GorillaGameManager.instance;
			if (instance != null)
			{
				if (instance is GorillaHuntManager || instance.GameModeName() == "HUNT")
				{
					this.EnableHuntWatch(true);
				}
				else if (instance is GorillaPaintbrawlManager || instance.GameModeName() == "PAINTBRAWL")
				{
					this.EnablePaintbrawlCosmetics(true);
				}
			}
			else
			{
				string gameModeString = NetworkSystem.Instance.GameModeString;
				if (!gameModeString.IsNullOrEmpty())
				{
					string text = gameModeString;
					if (text.Contains("HUNT"))
					{
						this.EnableHuntWatch(true);
					}
					else if (text.Contains("PAINTBRAWL"))
					{
						this.EnablePaintbrawlCosmetics(true);
					}
				}
			}
			this.UpdateFriendshipBracelet();
			if (this.IsLocalPartyMember && !this.isOfflineVRRig)
			{
				FriendshipGroupDetection.Instance.SendVerifyPartyMember(this.creator);
			}
		}
		if (this.netView != null)
		{
			base.transform.position = this.netView.gameObject.transform.position;
			base.transform.rotation = this.netView.gameObject.transform.rotation;
		}
		try
		{
			Action action = VRRig.newPlayerJoined;
			if (action != null)
			{
				action();
			}
		}
		catch (Exception ex)
		{
			Debug.LogError(ex);
		}
	}

	// Token: 0x06001787 RID: 6023 RVA: 0x00072758 File Offset: 0x00070958
	public void GrabbedByPlayer(VRRig grabbedByRig, bool grabbedBody, bool grabbedLeftHand, bool grabbedWithLeftHand)
	{
		GorillaClimbable gorillaClimbable = (grabbedWithLeftHand ? grabbedByRig.leftHandHoldsPlayer : grabbedByRig.rightHandHoldsPlayer);
		GorillaHandClimber gorillaHandClimber;
		if (grabbedBody)
		{
			gorillaHandClimber = EquipmentInteractor.instance.BodyClimber;
		}
		else if (grabbedLeftHand)
		{
			gorillaHandClimber = EquipmentInteractor.instance.LeftClimber;
		}
		else
		{
			gorillaHandClimber = EquipmentInteractor.instance.RightClimber;
		}
		gorillaHandClimber.SetCanRelease(false);
		GTPlayer.Instance.BeginClimbing(gorillaClimbable, gorillaHandClimber, null);
		this.grabbedRopeIsBody = grabbedBody;
		this.grabbedRopeIsLeft = grabbedLeftHand;
		this.grabbedRopeIndex = grabbedByRig.netView.ViewID;
		this.grabbedRopeBoneIndex = (grabbedWithLeftHand ? 1 : 0);
		this.grabbedRopeOffset = Vector3.zero;
		this.grabbedRopeIsPhotonView = true;
	}

	// Token: 0x06001788 RID: 6024 RVA: 0x000727FC File Offset: 0x000709FC
	public void DroppedByPlayer(VRRig grabbedByRig, Vector3 throwVelocity)
	{
		throwVelocity = Vector3.ClampMagnitude(throwVelocity, 20f);
		GorillaClimbable currentClimbable = GTPlayer.Instance.CurrentClimbable;
		if (GTPlayer.Instance.isClimbing && (currentClimbable == grabbedByRig.leftHandHoldsPlayer || currentClimbable == grabbedByRig.rightHandHoldsPlayer))
		{
			GorillaHandClimber currentClimber = GTPlayer.Instance.CurrentClimber;
			GTPlayer.Instance.EndClimbing(currentClimber, false, false);
			GTPlayer.Instance.SetVelocity(throwVelocity);
			this.grabbedRopeIsBody = false;
			this.grabbedRopeIsLeft = false;
			this.grabbedRopeIndex = -1;
			this.grabbedRopeBoneIndex = 0;
			this.grabbedRopeOffset = Vector3.zero;
			this.grabbedRopeIsPhotonView = false;
		}
	}

	// Token: 0x06001789 RID: 6025 RVA: 0x0007289C File Offset: 0x00070A9C
	public bool IsOnGround(float headCheckDistance, float handCheckDistance, out Vector3 groundNormal)
	{
		GTPlayer instance = GTPlayer.Instance;
		Vector3 position = base.transform.position;
		Vector3 vector;
		RaycastHit raycastHit;
		if (this.LocalCheckCollision(position, Vector3.down * headCheckDistance * this.scaleFactor, instance.headCollider.radius * this.scaleFactor, out vector, out raycastHit))
		{
			groundNormal = raycastHit.normal;
			return true;
		}
		Vector3 position2 = this.leftHand.rigTarget.position;
		if (this.LocalCheckCollision(position2, Vector3.down * handCheckDistance * this.scaleFactor, instance.minimumRaycastDistance * this.scaleFactor, out vector, out raycastHit))
		{
			groundNormal = raycastHit.normal;
			return true;
		}
		Vector3 position3 = this.rightHand.rigTarget.position;
		if (this.LocalCheckCollision(position3, Vector3.down * handCheckDistance * this.scaleFactor, instance.minimumRaycastDistance * this.scaleFactor, out vector, out raycastHit))
		{
			groundNormal = raycastHit.normal;
			return true;
		}
		groundNormal = Vector3.up;
		return false;
	}

	// Token: 0x0600178A RID: 6026 RVA: 0x000729B0 File Offset: 0x00070BB0
	private bool LocalTestMovementCollision(Vector3 startPosition, Vector3 startVelocity, out Vector3 modifiedVelocity, out Vector3 finalPosition)
	{
		GTPlayer instance = GTPlayer.Instance;
		Vector3 vector = startVelocity * Time.deltaTime;
		finalPosition = startPosition + vector;
		modifiedVelocity = startVelocity;
		Vector3 vector2;
		RaycastHit raycastHit;
		bool flag = this.LocalCheckCollision(startPosition, vector, instance.headCollider.radius * this.scaleFactor, out vector2, out raycastHit);
		if (flag)
		{
			finalPosition = vector2 - vector.normalized * 0.01f;
			modifiedVelocity = startVelocity - raycastHit.normal * Vector3.Dot(raycastHit.normal, startVelocity);
		}
		Vector3 position = this.leftHand.rigTarget.position;
		Vector3 vector3;
		RaycastHit raycastHit2;
		bool flag2 = this.LocalCheckCollision(position, vector, instance.minimumRaycastDistance * this.scaleFactor, out vector3, out raycastHit2);
		if (flag2)
		{
			finalPosition = vector3 - (this.leftHand.rigTarget.position - startPosition) - vector.normalized * 0.01f;
			modifiedVelocity = Vector3.zero;
		}
		Vector3 position2 = this.rightHand.rigTarget.position;
		Vector3 vector4;
		RaycastHit raycastHit3;
		bool flag3 = this.LocalCheckCollision(position2, vector, instance.minimumRaycastDistance * this.scaleFactor, out vector4, out raycastHit3);
		if (flag3)
		{
			finalPosition = vector4 - (this.rightHand.rigTarget.position - startPosition) - vector.normalized * 0.01f;
			modifiedVelocity = Vector3.zero;
		}
		return flag || flag2 || flag3;
	}

	// Token: 0x0600178B RID: 6027 RVA: 0x00072B40 File Offset: 0x00070D40
	private bool LocalCheckCollision(Vector3 startPosition, Vector3 movement, float radius, out Vector3 finalPosition, out RaycastHit hit)
	{
		GTPlayer instance = GTPlayer.Instance;
		finalPosition = startPosition + movement;
		RaycastHit raycastHit = default(RaycastHit);
		bool flag = false;
		int num = Physics.SphereCastNonAlloc(startPosition, radius, movement.normalized, this.rayCastNonAllocColliders, movement.magnitude, instance.locomotionEnabledLayers.value);
		if (num > 0)
		{
			raycastHit = this.rayCastNonAllocColliders[0];
			for (int i = 0; i < num; i++)
			{
				if (raycastHit.distance > 0f && (!flag || this.rayCastNonAllocColliders[i].distance < raycastHit.distance))
				{
					flag = true;
					raycastHit = this.rayCastNonAllocColliders[i];
				}
			}
		}
		hit = raycastHit;
		if (flag)
		{
			finalPosition = raycastHit.point + raycastHit.normal * radius;
			return true;
		}
		return false;
	}

	// Token: 0x0600178C RID: 6028 RVA: 0x00072C20 File Offset: 0x00070E20
	public void UpdateFriendshipBracelet()
	{
		bool flag = false;
		if (this.isOfflineVRRig)
		{
			bool flag2 = false;
			VRRig.PartyMemberStatus partyMemberStatus = this.GetPartyMemberStatus();
			if (partyMemberStatus != VRRig.PartyMemberStatus.InLocalParty)
			{
				if (partyMemberStatus == VRRig.PartyMemberStatus.NotInLocalParty)
				{
					flag2 = false;
					this.reliableState.isBraceletLeftHanded = false;
				}
			}
			else
			{
				flag2 = true;
				this.reliableState.isBraceletLeftHanded = FriendshipGroupDetection.Instance.DidJoinLeftHanded && !this.huntComputer.activeSelf;
			}
			if (this.reliableState.HasBracelet != flag2 || this.reliableState.braceletBeadColors.Count != FriendshipGroupDetection.Instance.myBeadColors.Count)
			{
				this.reliableState.SetIsDirty();
				flag = this.reliableState.HasBracelet == flag2;
			}
			this.reliableState.braceletBeadColors.Clear();
			if (flag2)
			{
				this.reliableState.braceletBeadColors.AddRange(FriendshipGroupDetection.Instance.myBeadColors);
			}
			this.reliableState.braceletSelfIndex = FriendshipGroupDetection.Instance.MyBraceletSelfIndex;
		}
		if (this.nonCosmeticLeftHandItem != null)
		{
			bool flag3 = this.reliableState.HasBracelet && this.reliableState.isBraceletLeftHanded && !this.IsInvisibleToLocalPlayer;
			this.nonCosmeticLeftHandItem.EnableItem(flag3);
			if (flag3)
			{
				this.friendshipBraceletLeftHand.UpdateBeads(this.reliableState.braceletBeadColors, this.reliableState.braceletSelfIndex);
				if (flag)
				{
					this.friendshipBraceletLeftHand.PlayAppearEffects();
				}
			}
		}
		if (this.nonCosmeticRightHandItem != null)
		{
			bool flag4 = this.reliableState.HasBracelet && !this.reliableState.isBraceletLeftHanded && !this.IsInvisibleToLocalPlayer;
			this.nonCosmeticRightHandItem.EnableItem(flag4);
			if (flag4)
			{
				this.friendshipBraceletRightHand.UpdateBeads(this.reliableState.braceletBeadColors, this.reliableState.braceletSelfIndex);
				if (flag)
				{
					this.friendshipBraceletRightHand.PlayAppearEffects();
				}
			}
		}
	}

	// Token: 0x0600178D RID: 6029 RVA: 0x00072DF0 File Offset: 0x00070FF0
	public void EnableHuntWatch(bool on)
	{
		this.huntComputer.SetActive(on);
		if (this.builderResizeWatch != null)
		{
			MeshRenderer component = this.builderResizeWatch.GetComponent<MeshRenderer>();
			if (component != null)
			{
				component.enabled = !on;
			}
		}
	}

	// Token: 0x0600178E RID: 6030 RVA: 0x00072E36 File Offset: 0x00071036
	public void EnablePaintbrawlCosmetics(bool on)
	{
		this.paintbrawlBalloons.gameObject.SetActive(on);
	}

	// Token: 0x0600178F RID: 6031 RVA: 0x00072E4C File Offset: 0x0007104C
	public void EnableBuilderResizeWatch(bool on)
	{
		if (this.builderResizeWatch != null && this.builderResizeWatch.activeSelf != on)
		{
			this.builderResizeWatch.SetActive(on);
			if (this.builderArmShelfLeft != null)
			{
				this.builderArmShelfLeft.gameObject.SetActive(on);
			}
			if (this.builderArmShelfRight != null)
			{
				this.builderArmShelfRight.gameObject.SetActive(on);
			}
		}
		if (this.isOfflineVRRig)
		{
			bool flag = this.reliableState.isBuilderWatchEnabled != on;
			this.reliableState.isBuilderWatchEnabled = on;
			if (flag)
			{
				this.reliableState.SetIsDirty();
			}
		}
	}

	// Token: 0x06001790 RID: 6032 RVA: 0x00072EF1 File Offset: 0x000710F1
	public void EnableGuardianEjectWatch(bool on)
	{
		if (this.guardianEjectWatch != null && this.guardianEjectWatch.activeSelf != on)
		{
			this.guardianEjectWatch.SetActive(on);
		}
	}

	// Token: 0x06001791 RID: 6033 RVA: 0x00072F1B File Offset: 0x0007111B
	public void EnableVStumpReturnWatch(bool on)
	{
		if (this.vStumpReturnWatch != null && this.vStumpReturnWatch.activeSelf != on)
		{
			this.vStumpReturnWatch.SetActive(on);
		}
	}

	// Token: 0x06001792 RID: 6034 RVA: 0x00072F48 File Offset: 0x00071148
	private void UpdateReplacementVoice()
	{
		if (this.remoteUseReplacementVoice || this.localUseReplacementVoice || GorillaComputer.instance.voiceChatOn != "TRUE")
		{
			this.voiceAudio.mute = true;
			return;
		}
		this.voiceAudio.mute = false;
	}

	// Token: 0x06001793 RID: 6035 RVA: 0x00072F98 File Offset: 0x00071198
	public bool ShouldPlayReplacementVoice()
	{
		return this.netView && !this.netView.IsMine && !(GorillaComputer.instance.voiceChatOn == "OFF") && (this.remoteUseReplacementVoice || this.localUseReplacementVoice || GorillaComputer.instance.voiceChatOn == "FALSE") && this.SpeakingLoudness > this.replacementVoiceLoudnessThreshold;
	}

	// Token: 0x06001794 RID: 6036 RVA: 0x00073013 File Offset: 0x00071213
	public void SetDuplicationZone(RigDuplicationZone duplicationZone)
	{
		this.duplicationZone = duplicationZone;
		this.inDuplicationZone = duplicationZone != null;
	}

	// Token: 0x06001795 RID: 6037 RVA: 0x00073029 File Offset: 0x00071229
	public void ClearDuplicationZone(RigDuplicationZone duplicationZone)
	{
		if (this.duplicationZone == duplicationZone)
		{
			this.SetDuplicationZone(null);
			this.renderTransform.localPosition = Vector3.zero;
		}
	}

	// Token: 0x17000292 RID: 658
	// (get) Token: 0x06001796 RID: 6038 RVA: 0x00073050 File Offset: 0x00071250
	// (set) Token: 0x06001797 RID: 6039 RVA: 0x00073058 File Offset: 0x00071258
	bool IUserCosmeticsCallback.PendingUpdate
	{
		get
		{
			return this.pendingCosmeticUpdate;
		}
		set
		{
			this.pendingCosmeticUpdate = value;
		}
	}

	// Token: 0x17000293 RID: 659
	// (get) Token: 0x06001798 RID: 6040 RVA: 0x00073061 File Offset: 0x00071261
	// (set) Token: 0x06001799 RID: 6041 RVA: 0x00073069 File Offset: 0x00071269
	public bool IsFrozen { get; set; }

	// Token: 0x0600179A RID: 6042 RVA: 0x00073074 File Offset: 0x00071274
	bool IUserCosmeticsCallback.OnGetUserCosmetics(string cosmetics)
	{
		if (cosmetics == this.rawCosmeticString && this.currentCosmeticTries < this.cosmeticRetries)
		{
			this.currentCosmeticTries++;
			return false;
		}
		this.rawCosmeticString = cosmetics ?? "";
		this.concatStringOfCosmeticsAllowed = this.rawCosmeticString;
		this.InitializedCosmetics = true;
		this.currentCosmeticTries = 0;
		this.CheckForEarlyAccess();
		this.SetCosmeticsActive();
		this.myBodyDockPositions.RefreshTransferrableItems();
		NetworkView networkView = this.netView;
		if (networkView != null)
		{
			networkView.SendRPC("RPC_RequestCosmetics", this.creator, Array.Empty<object>());
		}
		return true;
	}

	// Token: 0x0600179B RID: 6043 RVA: 0x00073110 File Offset: 0x00071310
	private short PackCompetitiveData()
	{
		if (!this.turningCompInitialized)
		{
			this.GorillaSnapTurningComp = GorillaTagger.Instance.GetComponent<GorillaSnapTurn>();
			this.turningCompInitialized = true;
		}
		this.fps = Mathf.Min(Mathf.RoundToInt(1f / Time.smoothDeltaTime), 255);
		int num = 0;
		if (this.GorillaSnapTurningComp != null)
		{
			this.turnFactor = this.GorillaSnapTurningComp.turnFactor;
			this.turnType = this.GorillaSnapTurningComp.turnType;
			string text = this.turnType;
			if (!(text == "SNAP"))
			{
				if (text == "SMOOTH")
				{
					num = 2;
				}
			}
			else
			{
				num = 1;
			}
			num *= 10;
			num += this.turnFactor;
		}
		return (short)(this.fps + (num << 8));
	}

	// Token: 0x0600179C RID: 6044 RVA: 0x000731D0 File Offset: 0x000713D0
	private void UnpackCompetitiveData(short packed)
	{
		int num = 255;
		this.fps = (int)packed & num;
		int num2 = 31;
		int num3 = (packed >> 8) & num2;
		this.turnFactor = num3 % 10;
		int num4 = num3 / 10;
		if (num4 == 1)
		{
			this.turnType = "SNAP";
			return;
		}
		if (num4 != 2)
		{
			this.turnType = "NONE";
			return;
		}
		this.turnType = "SMOOTH";
	}

	// Token: 0x0600179D RID: 6045 RVA: 0x00073234 File Offset: 0x00071434
	private void OnKIDSessionUpdated(bool showCustomNames, Permission.ManagedByEnum managedBy)
	{
		bool flag = (showCustomNames || managedBy == Permission.ManagedByEnum.PLAYER) && managedBy != Permission.ManagedByEnum.PROHIBITED;
		GorillaComputer.instance.SetComputerSettingsBySafety(!flag, new GorillaComputer.ComputerState[] { GorillaComputer.ComputerState.Name }, false);
		bool flag2 = PlayerPrefs.GetInt("nameTagsOn", -1) > 0;
		switch (managedBy)
		{
		case Permission.ManagedByEnum.PLAYER:
			flag = GorillaComputer.instance.NametagsEnabled;
			break;
		case Permission.ManagedByEnum.GUARDIAN:
			flag = showCustomNames && flag2;
			break;
		case Permission.ManagedByEnum.PROHIBITED:
			flag = false;
			break;
		}
		this.UpdateName(flag);
		Debug.Log("[KID] On Session Update - Custom Names Permission changed - Has enabled customNames? [" + flag.ToString() + "]");
	}

	// Token: 0x0600179E RID: 6046 RVA: 0x000732D0 File Offset: 0x000714D0
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	private static void CacheLocalRig()
	{
		if (VRRig.gLocalRig != null)
		{
			return;
		}
		GameObject gameObject = GameObject.Find("Local Gorilla Player");
		VRRig.gLocalRig = ((gameObject != null) ? gameObject.GetComponentInChildren<VRRig>() : null);
	}

	// Token: 0x17000294 RID: 660
	// (get) Token: 0x0600179F RID: 6047 RVA: 0x000732FB File Offset: 0x000714FB
	public static VRRig LocalRig
	{
		get
		{
			return VRRig.gLocalRig;
		}
	}

	// Token: 0x17000295 RID: 661
	// (get) Token: 0x060017A0 RID: 6048 RVA: 0x00073302 File Offset: 0x00071502
	public bool isLocal
	{
		get
		{
			return VRRig.gLocalRig == this;
		}
	}

	// Token: 0x17000296 RID: 662
	// (get) Token: 0x060017A1 RID: 6049 RVA: 0x0000FE4F File Offset: 0x0000E04F
	int IEyeScannable.scannableId
	{
		get
		{
			return base.gameObject.GetInstanceID();
		}
	}

	// Token: 0x17000297 RID: 663
	// (get) Token: 0x060017A2 RID: 6050 RVA: 0x0007330F File Offset: 0x0007150F
	Vector3 IEyeScannable.Position
	{
		get
		{
			return base.transform.position;
		}
	}

	// Token: 0x17000298 RID: 664
	// (get) Token: 0x060017A3 RID: 6051 RVA: 0x0007331C File Offset: 0x0007151C
	Bounds IEyeScannable.Bounds
	{
		get
		{
			return default(Bounds);
		}
	}

	// Token: 0x17000299 RID: 665
	// (get) Token: 0x060017A4 RID: 6052 RVA: 0x00073332 File Offset: 0x00071532
	IList<KeyValueStringPair> IEyeScannable.Entries
	{
		get
		{
			return this.buildEntries();
		}
	}

	// Token: 0x060017A5 RID: 6053 RVA: 0x0007333C File Offset: 0x0007153C
	private IList<KeyValueStringPair> buildEntries()
	{
		return new KeyValueStringPair[]
		{
			new KeyValueStringPair("Name", this.playerNameVisible),
			new KeyValueStringPair("Color", string.Format("{0}, {1}, {2}", Mathf.RoundToInt(this.playerColor.r * 9f), Mathf.RoundToInt(this.playerColor.g * 9f), Mathf.RoundToInt(this.playerColor.b * 9f)))
		};
	}

	// Token: 0x1400004A RID: 74
	// (add) Token: 0x060017A6 RID: 6054 RVA: 0x000733D4 File Offset: 0x000715D4
	// (remove) Token: 0x060017A7 RID: 6055 RVA: 0x0007340C File Offset: 0x0007160C
	public event Action OnDataChange;

	// Token: 0x0400192E RID: 6446
	private bool _isListeningFor_OnPostInstantiateAllPrefabs;

	// Token: 0x0400192F RID: 6447
	public static Action newPlayerJoined;

	// Token: 0x04001930 RID: 6448
	public VRMap head;

	// Token: 0x04001931 RID: 6449
	public VRMap rightHand;

	// Token: 0x04001932 RID: 6450
	public VRMap leftHand;

	// Token: 0x04001933 RID: 6451
	public VRMapThumb leftThumb;

	// Token: 0x04001934 RID: 6452
	public VRMapIndex leftIndex;

	// Token: 0x04001935 RID: 6453
	public VRMapMiddle leftMiddle;

	// Token: 0x04001936 RID: 6454
	public VRMapThumb rightThumb;

	// Token: 0x04001937 RID: 6455
	public VRMapIndex rightIndex;

	// Token: 0x04001938 RID: 6456
	public VRMapMiddle rightMiddle;

	// Token: 0x04001939 RID: 6457
	public CrittersLoudNoise leftHandNoise;

	// Token: 0x0400193A RID: 6458
	public CrittersLoudNoise rightHandNoise;

	// Token: 0x0400193B RID: 6459
	public CrittersLoudNoise speakingNoise;

	// Token: 0x0400193C RID: 6460
	private int previousGrabbedRope = -1;

	// Token: 0x0400193D RID: 6461
	private int previousGrabbedRopeBoneIndex;

	// Token: 0x0400193E RID: 6462
	private bool previousGrabbedRopeWasLeft;

	// Token: 0x0400193F RID: 6463
	private bool previousGrabbedRopeWasBody;

	// Token: 0x04001940 RID: 6464
	private GorillaRopeSwing currentRopeSwing;

	// Token: 0x04001941 RID: 6465
	private Transform currentHoldParent;

	// Token: 0x04001942 RID: 6466
	private Transform currentRopeSwingTarget;

	// Token: 0x04001943 RID: 6467
	private float lastRopeGrabTimer;

	// Token: 0x04001944 RID: 6468
	private bool shouldLerpToRope;

	// Token: 0x04001945 RID: 6469
	[NonSerialized]
	public int grabbedRopeIndex = -1;

	// Token: 0x04001946 RID: 6470
	[NonSerialized]
	public int grabbedRopeBoneIndex;

	// Token: 0x04001947 RID: 6471
	[NonSerialized]
	public bool grabbedRopeIsLeft;

	// Token: 0x04001948 RID: 6472
	[NonSerialized]
	public bool grabbedRopeIsBody;

	// Token: 0x04001949 RID: 6473
	[NonSerialized]
	public bool grabbedRopeIsPhotonView;

	// Token: 0x0400194A RID: 6474
	[NonSerialized]
	public Vector3 grabbedRopeOffset = Vector3.zero;

	// Token: 0x0400194B RID: 6475
	private int prevMovingSurfaceID = -1;

	// Token: 0x0400194C RID: 6476
	private bool movingSurfaceWasLeft;

	// Token: 0x0400194D RID: 6477
	private bool movingSurfaceWasBody;

	// Token: 0x0400194E RID: 6478
	private bool movingSurfaceWasMonkeBlock;

	// Token: 0x0400194F RID: 6479
	[NonSerialized]
	public int mountedMovingSurfaceId = -1;

	// Token: 0x04001950 RID: 6480
	[NonSerialized]
	private BuilderPiece mountedMonkeBlock;

	// Token: 0x04001951 RID: 6481
	[NonSerialized]
	private MovingSurface mountedMovingSurface;

	// Token: 0x04001952 RID: 6482
	[NonSerialized]
	public bool mountedMovingSurfaceIsLeft;

	// Token: 0x04001953 RID: 6483
	[NonSerialized]
	public bool mountedMovingSurfaceIsBody;

	// Token: 0x04001954 RID: 6484
	[NonSerialized]
	public bool movingSurfaceIsMonkeBlock;

	// Token: 0x04001955 RID: 6485
	[NonSerialized]
	public Vector3 mountedMonkeBlockOffset = Vector3.zero;

	// Token: 0x04001956 RID: 6486
	private float lastMountedSurfaceTimer;

	// Token: 0x04001957 RID: 6487
	private bool shouldLerpToMovingSurface;

	// Token: 0x04001958 RID: 6488
	[Tooltip("- False in 'Gorilla Player Networked.prefab'.\n- True in 'Local VRRig.prefab/Local Gorilla Player'.\n- False in 'Local VRRig.prefab/Actual Gorilla'")]
	public bool isOfflineVRRig;

	// Token: 0x04001959 RID: 6489
	public GameObject mainCamera;

	// Token: 0x0400195A RID: 6490
	public Transform playerOffsetTransform;

	// Token: 0x0400195B RID: 6491
	public int SDKIndex;

	// Token: 0x0400195C RID: 6492
	public bool isMyPlayer;

	// Token: 0x0400195D RID: 6493
	public AudioSource leftHandPlayer;

	// Token: 0x0400195E RID: 6494
	public AudioSource rightHandPlayer;

	// Token: 0x0400195F RID: 6495
	public AudioSource tagSound;

	// Token: 0x04001960 RID: 6496
	[SerializeField]
	private float ratio;

	// Token: 0x04001961 RID: 6497
	public Transform headConstraint;

	// Token: 0x04001962 RID: 6498
	public Vector3 headBodyOffset = Vector3.zero;

	// Token: 0x04001963 RID: 6499
	public GameObject headMesh;

	// Token: 0x04001964 RID: 6500
	private NetworkVector3 netSyncPos = new NetworkVector3();

	// Token: 0x04001965 RID: 6501
	public Vector3 jobPos;

	// Token: 0x04001966 RID: 6502
	public Quaternion syncRotation;

	// Token: 0x04001967 RID: 6503
	public Quaternion jobRotation;

	// Token: 0x04001968 RID: 6504
	public AudioClip[] clipToPlay;

	// Token: 0x04001969 RID: 6505
	public AudioClip[] handTapSound;

	// Token: 0x0400196A RID: 6506
	public int currentMatIndex;

	// Token: 0x0400196B RID: 6507
	public int setMatIndex;

	// Token: 0x0400196C RID: 6508
	public float lerpValueFingers;

	// Token: 0x0400196D RID: 6509
	public float lerpValueBody;

	// Token: 0x0400196E RID: 6510
	public GameObject backpack;

	// Token: 0x0400196F RID: 6511
	public Transform leftHandTransform;

	// Token: 0x04001970 RID: 6512
	public Transform rightHandTransform;

	// Token: 0x04001971 RID: 6513
	public Transform bodyTransform;

	// Token: 0x04001972 RID: 6514
	public SkinnedMeshRenderer mainSkin;

	// Token: 0x04001973 RID: 6515
	public GorillaSkin defaultSkin;

	// Token: 0x04001974 RID: 6516
	public MeshRenderer faceSkin;

	// Token: 0x04001975 RID: 6517
	public XRaySkeleton skeleton;

	// Token: 0x04001976 RID: 6518
	public GorillaBodyRenderer bodyRenderer;

	// Token: 0x04001977 RID: 6519
	public ZoneEntity zoneEntity;

	// Token: 0x04001978 RID: 6520
	public Material myDefaultSkinMaterialInstance;

	// Token: 0x04001979 RID: 6521
	public Material scoreboardMaterial;

	// Token: 0x0400197A RID: 6522
	public GameObject spectatorSkin;

	// Token: 0x0400197B RID: 6523
	public int handSync;

	// Token: 0x0400197C RID: 6524
	public Material[] materialsToChangeTo;

	// Token: 0x0400197D RID: 6525
	public float red;

	// Token: 0x0400197E RID: 6526
	public float green;

	// Token: 0x0400197F RID: 6527
	public float blue;

	// Token: 0x04001980 RID: 6528
	public TextMeshPro playerText1;

	// Token: 0x04001981 RID: 6529
	public TextMeshPro playerText2;

	// Token: 0x04001982 RID: 6530
	public string playerNameVisible;

	// Token: 0x04001983 RID: 6531
	[Tooltip("- True in 'Gorilla Player Networked.prefab'.\n- True in 'Local VRRig.prefab/Local Gorilla Player'.\n- False in 'Local VRRig.prefab/Actual Gorilla'")]
	public bool showName;

	// Token: 0x04001984 RID: 6532
	public CosmeticItemRegistry cosmeticsObjectRegistry = new CosmeticItemRegistry();

	// Token: 0x04001985 RID: 6533
	[FormerlySerializedAs("cosmetics")]
	public GameObject[] _cosmetics;

	// Token: 0x04001986 RID: 6534
	[FormerlySerializedAs("overrideCosmetics")]
	public GameObject[] _overrideCosmetics;

	// Token: 0x04001987 RID: 6535
	private int taggedById;

	// Token: 0x04001988 RID: 6536
	public string concatStringOfCosmeticsAllowed = "";

	// Token: 0x04001989 RID: 6537
	private bool initializedCosmetics;

	// Token: 0x0400198A RID: 6538
	public CosmeticsController.CosmeticSet cosmeticSet;

	// Token: 0x0400198B RID: 6539
	public CosmeticsController.CosmeticSet tryOnSet;

	// Token: 0x0400198C RID: 6540
	public CosmeticsController.CosmeticSet mergedSet;

	// Token: 0x0400198D RID: 6541
	public CosmeticsController.CosmeticSet prevSet;

	// Token: 0x0400198E RID: 6542
	private int cosmeticRetries = 2;

	// Token: 0x0400198F RID: 6543
	private int currentCosmeticTries;

	// Token: 0x04001991 RID: 6545
	public SizeManager sizeManager;

	// Token: 0x04001992 RID: 6546
	public float pitchScale = 0.3f;

	// Token: 0x04001993 RID: 6547
	public float pitchOffset = 1f;

	// Token: 0x04001994 RID: 6548
	[NonSerialized]
	public bool IsHaunted;

	// Token: 0x04001995 RID: 6549
	public float HauntedVoicePitch = 0.5f;

	// Token: 0x04001996 RID: 6550
	public float HauntedHearingVolume = 0.15f;

	// Token: 0x04001997 RID: 6551
	[NonSerialized]
	public bool UsingHauntedRing;

	// Token: 0x04001998 RID: 6552
	[NonSerialized]
	public float HauntedRingVoicePitch;

	// Token: 0x04001999 RID: 6553
	public FriendshipBracelet friendshipBraceletLeftHand;

	// Token: 0x0400199A RID: 6554
	public NonCosmeticHandItem nonCosmeticLeftHandItem;

	// Token: 0x0400199B RID: 6555
	public FriendshipBracelet friendshipBraceletRightHand;

	// Token: 0x0400199C RID: 6556
	public NonCosmeticHandItem nonCosmeticRightHandItem;

	// Token: 0x0400199D RID: 6557
	public HoverboardVisual hoverboardVisual;

	// Token: 0x0400199E RID: 6558
	private int hoverboardEnabledCount;

	// Token: 0x0400199F RID: 6559
	public HoldableHand bodyHolds;

	// Token: 0x040019A0 RID: 6560
	public HoldableHand leftHolds;

	// Token: 0x040019A1 RID: 6561
	public HoldableHand rightHolds;

	// Token: 0x040019A2 RID: 6562
	public GorillaClimbable leftHandHoldsPlayer;

	// Token: 0x040019A3 RID: 6563
	public GorillaClimbable rightHandHoldsPlayer;

	// Token: 0x040019A4 RID: 6564
	public GameObject nameTagAnchor;

	// Token: 0x040019A5 RID: 6565
	public GameObject frozenEffect;

	// Token: 0x040019A6 RID: 6566
	public GameObject iceCubeLeft;

	// Token: 0x040019A7 RID: 6567
	public GameObject iceCubeRight;

	// Token: 0x040019A8 RID: 6568
	public float frozenEffectMaxY;

	// Token: 0x040019A9 RID: 6569
	public float frozenEffectMaxHorizontalScale = 0.8f;

	// Token: 0x040019AA RID: 6570
	public GameObject FPVEffectsParent;

	// Token: 0x040019AB RID: 6571
	public Dictionary<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> TemporaryCosmeticEffects = new Dictionary<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect>();

	// Token: 0x040019AC RID: 6572
	public VRRigReliableState reliableState;

	// Token: 0x040019AD RID: 6573
	[SerializeField]
	private Transform MouthPosition;

	// Token: 0x040019B1 RID: 6577
	internal RigContainer rigContainer;

	// Token: 0x040019B2 RID: 6578
	public Action<RigContainer> OnNameChanged;

	// Token: 0x040019B3 RID: 6579
	private Vector3 remoteVelocity;

	// Token: 0x040019B4 RID: 6580
	private double remoteLatestTimestamp;

	// Token: 0x040019B5 RID: 6581
	private Vector3 remoteCorrectionNeeded;

	// Token: 0x040019B6 RID: 6582
	private const float REMOTE_CORRECTION_RATE = 5f;

	// Token: 0x040019B7 RID: 6583
	private const bool USE_NEW_NETCODE = false;

	// Token: 0x040019B8 RID: 6584
	private float stealthTimer;

	// Token: 0x040019B9 RID: 6585
	private GorillaAmbushManager stealthManager;

	// Token: 0x040019BA RID: 6586
	private LayerChanger layerChanger;

	// Token: 0x040019BB RID: 6587
	private float frozenEffectMinY;

	// Token: 0x040019BC RID: 6588
	private float frozenEffectMinHorizontalScale;

	// Token: 0x040019BD RID: 6589
	private float frozenTimeElapsed;

	// Token: 0x040019BE RID: 6590
	public TagEffectPack CosmeticEffectPack;

	// Token: 0x040019BF RID: 6591
	private GorillaSnapTurn GorillaSnapTurningComp;

	// Token: 0x040019C0 RID: 6592
	private bool turningCompInitialized;

	// Token: 0x040019C1 RID: 6593
	private string turnType = "NONE";

	// Token: 0x040019C2 RID: 6594
	private int turnFactor;

	// Token: 0x040019C3 RID: 6595
	private int fps;

	// Token: 0x040019C4 RID: 6596
	private VRRig.PartyMemberStatus partyMemberStatus;

	// Token: 0x040019C5 RID: 6597
	public static readonly GTBitOps.BitWriteInfo[] WearablePackedStatesBitWriteInfos = new GTBitOps.BitWriteInfo[]
	{
		new GTBitOps.BitWriteInfo(0, 1),
		new GTBitOps.BitWriteInfo(1, 2),
		new GTBitOps.BitWriteInfo(3, 2),
		new GTBitOps.BitWriteInfo(5, 2),
		new GTBitOps.BitWriteInfo(7, 2),
		new GTBitOps.BitWriteInfo(9, 2)
	};

	// Token: 0x040019C6 RID: 6598
	public bool inTryOnRoom;

	// Token: 0x040019C7 RID: 6599
	public bool muted;

	// Token: 0x040019C8 RID: 6600
	private float lastScaleFactor = 1f;

	// Token: 0x040019C9 RID: 6601
	private float scaleMultiplier = 1f;

	// Token: 0x040019CA RID: 6602
	private float nativeScale = 1f;

	// Token: 0x040019CB RID: 6603
	private float timeSpawned;

	// Token: 0x040019CC RID: 6604
	public float doNotLerpConstant = 1f;

	// Token: 0x040019CD RID: 6605
	public string tempString;

	// Token: 0x040019CE RID: 6606
	private Player tempPlayer;

	// Token: 0x040019CF RID: 6607
	internal NetPlayer creator;

	// Token: 0x040019D0 RID: 6608
	private float[] speedArray;

	// Token: 0x040019D1 RID: 6609
	private double handLerpValues;

	// Token: 0x040019D2 RID: 6610
	private bool initialized;

	// Token: 0x040019D3 RID: 6611
	[FormerlySerializedAs("battleBalloons")]
	public PaintbrawlBalloons paintbrawlBalloons;

	// Token: 0x040019D4 RID: 6612
	private int tempInt;

	// Token: 0x040019D5 RID: 6613
	public BodyDockPositions myBodyDockPositions;

	// Token: 0x040019D6 RID: 6614
	public ParticleSystem lavaParticleSystem;

	// Token: 0x040019D7 RID: 6615
	public ParticleSystem rockParticleSystem;

	// Token: 0x040019D8 RID: 6616
	public ParticleSystem iceParticleSystem;

	// Token: 0x040019D9 RID: 6617
	public ParticleSystem snowFlakeParticleSystem;

	// Token: 0x040019DA RID: 6618
	public string tempItemName;

	// Token: 0x040019DB RID: 6619
	public CosmeticsController.CosmeticItem tempItem;

	// Token: 0x040019DC RID: 6620
	public string tempItemId;

	// Token: 0x040019DD RID: 6621
	public int tempItemCost;

	// Token: 0x040019DE RID: 6622
	public int leftHandHoldableStatus;

	// Token: 0x040019DF RID: 6623
	public int rightHandHoldableStatus;

	// Token: 0x040019E0 RID: 6624
	[Tooltip("This has to match the drumsAS array in DrumsItem.cs.")]
	[SerializeReference]
	public AudioSource[] musicDrums;

	// Token: 0x040019E1 RID: 6625
	private List<TransferrableObject> instrumentSelfOnly = new List<TransferrableObject>();

	// Token: 0x040019E2 RID: 6626
	public AudioSource geodeCrackingSound;

	// Token: 0x040019E3 RID: 6627
	public float bonkTime;

	// Token: 0x040019E4 RID: 6628
	public float bonkCooldown = 2f;

	// Token: 0x040019E5 RID: 6629
	private VRRig tempVRRig;

	// Token: 0x040019E6 RID: 6630
	public GameObject huntComputer;

	// Token: 0x040019E7 RID: 6631
	public GameObject builderResizeWatch;

	// Token: 0x040019E8 RID: 6632
	public BuilderArmShelf builderArmShelfLeft;

	// Token: 0x040019E9 RID: 6633
	public BuilderArmShelf builderArmShelfRight;

	// Token: 0x040019EA RID: 6634
	public GameObject guardianEjectWatch;

	// Token: 0x040019EB RID: 6635
	public GameObject vStumpReturnWatch;

	// Token: 0x040019EC RID: 6636
	public ProjectileWeapon projectileWeapon;

	// Token: 0x040019ED RID: 6637
	private PhotonVoiceView myPhotonVoiceView;

	// Token: 0x040019EE RID: 6638
	private VRRig senderRig;

	// Token: 0x040019EF RID: 6639
	private bool isInitialized;

	// Token: 0x040019F0 RID: 6640
	private CircularBuffer<VRRig.VelocityTime> velocityHistoryList = new CircularBuffer<VRRig.VelocityTime>(200);

	// Token: 0x040019F1 RID: 6641
	public int velocityHistoryMaxLength = 200;

	// Token: 0x040019F2 RID: 6642
	private Vector3 lastPosition;

	// Token: 0x040019F3 RID: 6643
	public const int splashLimitCount = 4;

	// Token: 0x040019F4 RID: 6644
	public const float splashLimitCooldown = 0.5f;

	// Token: 0x040019F5 RID: 6645
	private float[] splashEffectTimes = new float[4];

	// Token: 0x040019F6 RID: 6646
	internal AudioSource voiceAudio;

	// Token: 0x040019F7 RID: 6647
	public bool remoteUseReplacementVoice;

	// Token: 0x040019F8 RID: 6648
	public bool localUseReplacementVoice;

	// Token: 0x040019F9 RID: 6649
	private MicWrapper currentMicWrapper;

	// Token: 0x040019FA RID: 6650
	private IAudioDesc audioDesc;

	// Token: 0x040019FB RID: 6651
	private float speakingLoudness;

	// Token: 0x040019FC RID: 6652
	public bool shouldSendSpeakingLoudness = true;

	// Token: 0x040019FD RID: 6653
	public float replacementVoiceLoudnessThreshold = 0.05f;

	// Token: 0x040019FE RID: 6654
	public int replacementVoiceDetectionDelay = 128;

	// Token: 0x040019FF RID: 6655
	private GorillaMouthFlap myMouthFlap;

	// Token: 0x04001A00 RID: 6656
	private GorillaSpeakerLoudness mySpeakerLoudness;

	// Token: 0x04001A01 RID: 6657
	public ReplacementVoice myReplacementVoice;

	// Token: 0x04001A02 RID: 6658
	private GorillaEyeExpressions myEyeExpressions;

	// Token: 0x04001A03 RID: 6659
	[SerializeField]
	internal NetworkView netView;

	// Token: 0x04001A04 RID: 6660
	[SerializeField]
	internal VRRigSerializer rigSerializer;

	// Token: 0x04001A05 RID: 6661
	public NetPlayer OwningNetPlayer;

	// Token: 0x04001A06 RID: 6662
	[SerializeField]
	private FXSystemSettings sharedFXSettings;

	// Token: 0x04001A07 RID: 6663
	[NonSerialized]
	public FXSystemSettings fxSettings;

	// Token: 0x04001A08 RID: 6664
	[SerializeField]
	private float tapPointDistance = 0.035f;

	// Token: 0x04001A09 RID: 6665
	[SerializeField]
	private float handSpeedToVolumeModifier = 0.05f;

	// Token: 0x04001A0A RID: 6666
	[SerializeField]
	private HandEffectContext _leftHandEffect;

	// Token: 0x04001A0B RID: 6667
	[SerializeField]
	private HandEffectContext _rightHandEffect;

	// Token: 0x04001A0C RID: 6668
	private bool _rigBuildFullyInitialized;

	// Token: 0x04001A0D RID: 6669
	[SerializeField]
	private Transform renderTransform;

	// Token: 0x04001A0E RID: 6670
	private bool playerWasHaunted;

	// Token: 0x04001A0F RID: 6671
	private float nonHauntedVolume;

	// Token: 0x04001A10 RID: 6672
	[SerializeField]
	private AnimationCurve voicePitchForRelativeScale;

	// Token: 0x04001A11 RID: 6673
	private Vector3 LocalTrajectoryOverridePosition;

	// Token: 0x04001A12 RID: 6674
	private Vector3 LocalTrajectoryOverrideVelocity;

	// Token: 0x04001A13 RID: 6675
	private float LocalTrajectoryOverrideBlend;

	// Token: 0x04001A14 RID: 6676
	[SerializeField]
	private float LocalTrajectoryOverrideDuration = 1f;

	// Token: 0x04001A15 RID: 6677
	private bool localOverrideIsBody;

	// Token: 0x04001A16 RID: 6678
	private bool localOverrideIsLeftHand;

	// Token: 0x04001A17 RID: 6679
	private Transform localOverrideGrabbingHand;

	// Token: 0x04001A18 RID: 6680
	private float localGrabOverrideBlend;

	// Token: 0x04001A19 RID: 6681
	[SerializeField]
	private float LocalGrabOverrideDuration = 0.25f;

	// Token: 0x04001A1A RID: 6682
	private float[] voiceSampleBuffer = new float[128];

	// Token: 0x04001A1B RID: 6683
	private const int CHECK_LOUDNESS_FREQ_FRAMES = 10;

	// Token: 0x04001A1C RID: 6684
	private CallbackContainer<ICallBack> lateUpdateCallbacks = new CallbackContainer<ICallBack>(5);

	// Token: 0x04001A1D RID: 6685
	private bool IsInvisibleToLocalPlayer;

	// Token: 0x04001A1E RID: 6686
	private const int remoteUseReplacementVoice_BIT = 512;

	// Token: 0x04001A1F RID: 6687
	private const int grabbedRope_BIT = 1024;

	// Token: 0x04001A20 RID: 6688
	private const int grabbedRopeIsPhotonView_BIT = 2048;

	// Token: 0x04001A21 RID: 6689
	private const int isHoldingHoverboard_BIT = 8192;

	// Token: 0x04001A22 RID: 6690
	private const int isHoverboardLeftHanded_BIT = 16384;

	// Token: 0x04001A23 RID: 6691
	private const int isOnMovingSurface_BIT = 32768;

	// Token: 0x04001A24 RID: 6692
	private Vector3 tempVec;

	// Token: 0x04001A25 RID: 6693
	private Quaternion tempQuat;

	// Token: 0x04001A26 RID: 6694
	public Color playerColor;

	// Token: 0x04001A27 RID: 6695
	public bool colorInitialized;

	// Token: 0x04001A28 RID: 6696
	private Action<Color> onColorInitialized;

	// Token: 0x04001A2C RID: 6700
	private int currentQuestScore;

	// Token: 0x04001A2D RID: 6701
	private bool _scoreUpdated;

	// Token: 0x04001A2E RID: 6702
	private CallLimiter updateQuestCallLimit = new CallLimiter(1, 0.5f, 0.5f);

	// Token: 0x04001A2F RID: 6703
	public const float maxThrowVelocity = 20f;

	// Token: 0x04001A30 RID: 6704
	private RaycastHit[] rayCastNonAllocColliders = new RaycastHit[5];

	// Token: 0x04001A31 RID: 6705
	private bool inDuplicationZone;

	// Token: 0x04001A32 RID: 6706
	private RigDuplicationZone duplicationZone;

	// Token: 0x04001A33 RID: 6707
	private bool pendingCosmeticUpdate = true;

	// Token: 0x04001A34 RID: 6708
	private string rawCosmeticString = "";

	// Token: 0x04001A36 RID: 6710
	public List<HandEffectsOverrideCosmetic> CosmeticHandEffectsOverride_Right = new List<HandEffectsOverrideCosmetic>();

	// Token: 0x04001A37 RID: 6711
	public List<HandEffectsOverrideCosmetic> CosmeticHandEffectsOverride_Left = new List<HandEffectsOverrideCosmetic>();

	// Token: 0x04001A38 RID: 6712
	private int loudnessCheckFrame;

	// Token: 0x04001A39 RID: 6713
	private float frameScale;

	// Token: 0x04001A3A RID: 6714
	private const bool SHOW_SCREENS = false;

	// Token: 0x04001A3B RID: 6715
	private static VRRig gLocalRig;

	// Token: 0x020003D3 RID: 979
	public enum PartyMemberStatus
	{
		// Token: 0x04001A3E RID: 6718
		NeedsUpdate,
		// Token: 0x04001A3F RID: 6719
		InLocalParty,
		// Token: 0x04001A40 RID: 6720
		NotInLocalParty
	}

	// Token: 0x020003D4 RID: 980
	public enum WearablePackedStateSlots
	{
		// Token: 0x04001A42 RID: 6722
		Hat,
		// Token: 0x04001A43 RID: 6723
		LeftHand,
		// Token: 0x04001A44 RID: 6724
		RightHand,
		// Token: 0x04001A45 RID: 6725
		Face,
		// Token: 0x04001A46 RID: 6726
		Pants1,
		// Token: 0x04001A47 RID: 6727
		Pants2
	}

	// Token: 0x020003D5 RID: 981
	public struct VelocityTime
	{
		// Token: 0x060017AC RID: 6060 RVA: 0x0007375A File Offset: 0x0007195A
		public VelocityTime(Vector3 velocity, double velTime)
		{
			this.vel = velocity;
			this.time = velTime;
		}

		// Token: 0x04001A48 RID: 6728
		public Vector3 vel;

		// Token: 0x04001A49 RID: 6729
		public double time;
	}
}
