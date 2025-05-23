using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CjLib;
using ExitGames.Client.Photon;
using GorillaExtensions;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using GorillaNetworking;
using GorillaTag.Cosmetics;
using GorillaTag.GuidedRefs;
using GorillaTagScripts;
using GorillaTagScripts.Builder;
using Photon.Pun;
using Photon.Voice.Unity;
using Steamworks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.XR;

// Token: 0x02000639 RID: 1593
public class GorillaTagger : MonoBehaviour, IGuidedRefReceiverMono, IGuidedRefMonoBehaviour, IGuidedRefObject
{
	// Token: 0x170003CB RID: 971
	// (get) Token: 0x060027B7 RID: 10167 RVA: 0x000C4CDC File Offset: 0x000C2EDC
	public static GorillaTagger Instance
	{
		get
		{
			return GorillaTagger._instance;
		}
	}

	// Token: 0x170003CC RID: 972
	// (get) Token: 0x060027B8 RID: 10168 RVA: 0x000C4CE3 File Offset: 0x000C2EE3
	public NetworkView myVRRig
	{
		get
		{
			return this.offlineVRRig.netView;
		}
	}

	// Token: 0x170003CD RID: 973
	// (get) Token: 0x060027B9 RID: 10169 RVA: 0x000C4CF0 File Offset: 0x000C2EF0
	internal VRRigSerializer rigSerializer
	{
		get
		{
			return this.offlineVRRig.rigSerializer;
		}
	}

	// Token: 0x170003CE RID: 974
	// (get) Token: 0x060027BA RID: 10170 RVA: 0x000C4CFD File Offset: 0x000C2EFD
	// (set) Token: 0x060027BB RID: 10171 RVA: 0x000C4D05 File Offset: 0x000C2F05
	public Rigidbody rigidbody { get; private set; }

	// Token: 0x170003CF RID: 975
	// (get) Token: 0x060027BC RID: 10172 RVA: 0x000C4D0E File Offset: 0x000C2F0E
	public float DefaultHandTapVolume
	{
		get
		{
			return this.cacheHandTapVolume;
		}
	}

	// Token: 0x170003D0 RID: 976
	// (get) Token: 0x060027BD RID: 10173 RVA: 0x000C4D16 File Offset: 0x000C2F16
	// (set) Token: 0x060027BE RID: 10174 RVA: 0x000C4D1E File Offset: 0x000C2F1E
	public Recorder myRecorder { get; private set; }

	// Token: 0x170003D1 RID: 977
	// (get) Token: 0x060027BF RID: 10175 RVA: 0x000C4D27 File Offset: 0x000C2F27
	public float sphereCastRadius
	{
		get
		{
			if (this.tagRadiusOverride == null)
			{
				return 0.03f;
			}
			return this.tagRadiusOverride.Value;
		}
	}

	// Token: 0x060027C0 RID: 10176 RVA: 0x000C4D47 File Offset: 0x000C2F47
	public void SetTagRadiusOverrideThisFrame(float radius)
	{
		this.tagRadiusOverride = new float?(radius);
		this.tagRadiusOverrideFrame = Time.frameCount;
	}

	// Token: 0x060027C1 RID: 10177 RVA: 0x000C4D60 File Offset: 0x000C2F60
	protected void Awake()
	{
		this.GuidedRefInitialize();
		this.RecoverMissingRefs();
		this.MirrorCameraCullingMask = new Watchable<int>(this.BaseMirrorCameraCullingMask);
		if (GorillaTagger._instance != null && GorillaTagger._instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			GorillaTagger._instance = this;
			GorillaTagger.hasInstance = true;
			Action action = GorillaTagger.onPlayerSpawnedRootCallback;
			if (action != null)
			{
				action();
			}
		}
		GRFirstTimeUserExperience grfirstTimeUserExperience = Object.FindObjectOfType<GRFirstTimeUserExperience>(true);
		GameObject gameObject = ((grfirstTimeUserExperience != null) ? grfirstTimeUserExperience.gameObject : null);
		if (!this.disableTutorial && (this.testTutorial || (PlayerPrefs.GetString("tutorial") != "done" && PlayerPrefs.GetString("didTutorial") != "done" && NetworkSystemConfig.AppVersion != "dev")))
		{
			base.transform.parent.position = new Vector3(-140f, 28f, -102f);
			base.transform.parent.eulerAngles = new Vector3(0f, 180f, 0f);
			GTPlayer.Instance.InitializeValues();
			PlayerPrefs.SetFloat("redValue", Random.value);
			PlayerPrefs.SetFloat("greenValue", Random.value);
			PlayerPrefs.SetFloat("blueValue", Random.value);
			PlayerPrefs.Save();
		}
		else
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Add("didTutorial", true);
			PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable, null, null);
			PlayerPrefs.SetString("didTutorial", "done");
			PlayerPrefs.Save();
			bool flag = true;
			if (gameObject != null && PlayerPrefs.GetString("spawnInWrongStump") == "flagged" && flag)
			{
				gameObject.SetActive(true);
				GRFirstTimeUserExperience grfirstTimeUserExperience2;
				if (gameObject.TryGetComponent<GRFirstTimeUserExperience>(out grfirstTimeUserExperience2) && grfirstTimeUserExperience2.spawnPoint != null)
				{
					GTPlayer.Instance.TeleportTo(grfirstTimeUserExperience2.spawnPoint.position, grfirstTimeUserExperience2.spawnPoint.rotation);
					GTPlayer.Instance.InitializeValues();
					PlayerPrefs.DeleteKey("spawnInWrongStump");
					PlayerPrefs.Save();
				}
			}
		}
		this.thirdPersonCamera.SetActive(Application.platform != RuntimePlatform.Android);
		this.inputDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
		this.wasInOverlay = false;
		this.baseSlideControl = GTPlayer.Instance.slideControl;
		this.gorillaTagColliderLayerMask = LayerMask.GetMask(new string[] { "Gorilla Tag Collider" });
		this.rigidbody = base.GetComponent<Rigidbody>();
		this.cacheHandTapVolume = this.handTapVolume;
		OVRManager.foveatedRenderingLevel = OVRManager.FoveatedRenderingLevel.Medium;
	}

	// Token: 0x060027C2 RID: 10178 RVA: 0x000C4FF9 File Offset: 0x000C31F9
	protected void OnDestroy()
	{
		if (GorillaTagger._instance == this)
		{
			GorillaTagger._instance = null;
			GorillaTagger.hasInstance = false;
		}
	}

	// Token: 0x060027C3 RID: 10179 RVA: 0x000C5014 File Offset: 0x000C3214
	private void IsXRSubsystemActive()
	{
		this.loadedDeviceName = XRSettings.loadedDeviceName;
		List<XRDisplaySubsystem> list = new List<XRDisplaySubsystem>();
		SubsystemManager.GetInstances<XRDisplaySubsystem>(list);
		using (List<XRDisplaySubsystem>.Enumerator enumerator = list.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.running)
				{
					this.xrSubsystemIsActive = true;
					return;
				}
			}
		}
		this.xrSubsystemIsActive = false;
	}

	// Token: 0x060027C4 RID: 10180 RVA: 0x000C5088 File Offset: 0x000C3288
	protected void Start()
	{
		this.IsXRSubsystemActive();
		if (this.loadedDeviceName == "OpenVR Display")
		{
			GTPlayer.Instance.leftHandOffset = new Vector3(-0.02f, 0f, -0.07f);
			GTPlayer.Instance.rightHandOffset = new Vector3(0.02f, 0f, -0.07f);
			Quaternion quaternion = Quaternion.Euler(new Vector3(-90f, 180f, -20f));
			Quaternion quaternion2 = Quaternion.Euler(new Vector3(-90f, 180f, 20f));
			Quaternion quaternion3 = Quaternion.Euler(new Vector3(-141f, 204f, -27f));
			Quaternion quaternion4 = Quaternion.Euler(new Vector3(-141f, 156f, 27f));
			GTPlayer.Instance.leftHandRotOffset = quaternion3 * Quaternion.Inverse(quaternion);
			GTPlayer.Instance.rightHandRotOffset = quaternion4 * Quaternion.Inverse(quaternion2);
		}
		this.bodyVector = new Vector3(0f, this.bodyCollider.height / 2f - this.bodyCollider.radius, 0f);
		if (SteamManager.Initialized)
		{
			this.gameOverlayActivatedCb = Callback<GameOverlayActivated_t>.Create(new Callback<GameOverlayActivated_t>.DispatchDelegate(this.OnGameOverlayActivated));
		}
	}

	// Token: 0x060027C5 RID: 10181 RVA: 0x000C51D0 File Offset: 0x000C33D0
	private void OnGameOverlayActivated(GameOverlayActivated_t pCallback)
	{
		this.isGameOverlayActive = pCallback.m_bActive > 0;
	}

	// Token: 0x060027C6 RID: 10182 RVA: 0x000C51E4 File Offset: 0x000C33E4
	protected void LateUpdate()
	{
		GorillaTagger.<>c__DisplayClass112_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		if (this.isGameOverlayActive)
		{
			if (this.leftHandTriggerCollider.activeSelf)
			{
				this.leftHandTriggerCollider.SetActive(false);
				this.rightHandTriggerCollider.SetActive(true);
			}
			GTPlayer.Instance.inOverlay = true;
		}
		else
		{
			if (!this.leftHandTriggerCollider.activeSelf)
			{
				this.leftHandTriggerCollider.SetActive(true);
				this.rightHandTriggerCollider.SetActive(true);
			}
			GTPlayer.Instance.inOverlay = false;
		}
		if (this.xrSubsystemIsActive && Application.platform != RuntimePlatform.Android)
		{
			if (Mathf.Abs(Time.fixedDeltaTime - 1f / XRDevice.refreshRate) > 0.0001f)
			{
				Debug.Log(" =========== adjusting refresh size =========");
				Debug.Log(" fixedDeltaTime before:\t" + Time.fixedDeltaTime.ToString());
				Debug.Log(" refresh rate         :\t" + XRDevice.refreshRate.ToString());
				Time.fixedDeltaTime = 1f / XRDevice.refreshRate;
				Debug.Log(" fixedDeltaTime after :\t" + Time.fixedDeltaTime.ToString());
				Debug.Log(" history size before  :\t" + GTPlayer.Instance.velocityHistorySize.ToString());
				GTPlayer.Instance.velocityHistorySize = Mathf.Max(Mathf.Min(Mathf.FloorToInt(XRDevice.refreshRate * 0.083333336f), 10), 6);
				if (GTPlayer.Instance.velocityHistorySize > 9)
				{
					GTPlayer.Instance.velocityHistorySize--;
				}
				Debug.Log("new history size: " + GTPlayer.Instance.velocityHistorySize.ToString());
				Debug.Log(" ============================================");
				GTPlayer.Instance.slideControl = 1f - this.CalcSlideControl(XRDevice.refreshRate);
				GTPlayer.Instance.InitializeValues();
			}
		}
		else if (Application.platform != RuntimePlatform.Android && OVRManager.instance != null && OVRManager.OVRManagerinitialized && OVRManager.instance.gameObject != null && OVRManager.instance.gameObject.activeSelf)
		{
			Object.Destroy(OVRManager.instance.gameObject);
		}
		if (!this.frameRateUpdated && Application.platform == RuntimePlatform.Android && OVRManager.instance.gameObject.activeSelf)
		{
			InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsManually;
			int num = OVRManager.display.displayFrequenciesAvailable.Length - 1;
			float num2 = OVRManager.display.displayFrequenciesAvailable[num];
			float systemDisplayFrequency = OVRPlugin.systemDisplayFrequency;
			if (systemDisplayFrequency != 60f)
			{
				if (systemDisplayFrequency == 71f)
				{
					num2 = 72f;
				}
			}
			else
			{
				num2 = 60f;
			}
			while (num2 > 90f)
			{
				num--;
				if (num < 0)
				{
					break;
				}
				num2 = OVRManager.display.displayFrequenciesAvailable[num];
			}
			float num3 = 1f;
			if (Mathf.Abs(Time.fixedDeltaTime - 1f / num2 * num3) > 0.0001f)
			{
				float num4 = Time.fixedDeltaTime - 1f / num2 * num3;
				Debug.Log(" =========== adjusting refresh size =========");
				Debug.Log("!!!!Time.fixedDeltaTime - (1f / newRefreshRate) * " + num3.ToString() + ")" + num4.ToString());
				Debug.Log("Old Refresh rate: " + systemDisplayFrequency.ToString());
				Debug.Log("New Refresh rate: " + num2.ToString());
				Debug.Log(" fixedDeltaTime before:\t" + Time.fixedDeltaTime.ToString());
				Debug.Log(" fixedDeltaTime after :\t" + (1f / num2).ToString());
				Time.fixedDeltaTime = 1f / num2 * num3;
				OVRPlugin.systemDisplayFrequency = num2;
				GTPlayer.Instance.velocityHistorySize = Mathf.FloorToInt(num2 * 0.083333336f);
				if (GTPlayer.Instance.velocityHistorySize > 9)
				{
					GTPlayer.Instance.velocityHistorySize--;
				}
				Debug.Log(" fixedDeltaTime after :\t" + Time.fixedDeltaTime.ToString());
				Debug.Log(" history size before  :\t" + GTPlayer.Instance.velocityHistorySize.ToString());
				Debug.Log("new history size: " + GTPlayer.Instance.velocityHistorySize.ToString());
				Debug.Log(" ============================================");
				GTPlayer.Instance.slideControl = 1f - this.CalcSlideControl(XRDevice.refreshRate);
				GTPlayer.Instance.InitializeValues();
				OVRManager.instance.gameObject.SetActive(false);
				this.frameRateUpdated = true;
			}
		}
		if (!this.xrSubsystemIsActive && Application.platform != RuntimePlatform.Android && Mathf.Abs(Time.fixedDeltaTime - 0.0069444445f) > 0.0001f)
		{
			Debug.Log("updating delta time. was: " + Time.fixedDeltaTime.ToString() + ". now it's " + 0.0069444445f.ToString());
			Application.targetFrameRate = 144;
			Time.fixedDeltaTime = 0.0069444445f;
			GTPlayer.Instance.velocityHistorySize = Mathf.Min(Mathf.FloorToInt(12f), 10);
			if (GTPlayer.Instance.velocityHistorySize > 9)
			{
				GTPlayer.Instance.velocityHistorySize--;
			}
			Debug.Log("new history size: " + GTPlayer.Instance.velocityHistorySize.ToString());
			GTPlayer.Instance.slideControl = 1f - this.CalcSlideControl(144f);
			GTPlayer.Instance.InitializeValues();
		}
		this.leftRaycastSweep = this.leftHandTransform.position - this.lastLeftHandPositionForTag;
		this.leftHeadRaycastSweep = this.leftHandTransform.position - this.headCollider.transform.position;
		this.rightRaycastSweep = this.rightHandTransform.position - this.lastRightHandPositionForTag;
		this.rightHeadRaycastSweep = this.rightHandTransform.position - this.headCollider.transform.position;
		this.headRaycastSweep = this.headCollider.transform.position - this.lastHeadPositionForTag;
		this.bodyRaycastSweep = this.bodyCollider.transform.position - this.lastBodyPositionForTag;
		this.otherPlayer = null;
		this.touchedPlayer = null;
		CS$<>8__locals1.otherTouchedPlayer = null;
		if (this.tagRadiusOverrideFrame < Time.frameCount)
		{
			this.tagRadiusOverride = null;
		}
		float num5 = this.sphereCastRadius * GTPlayer.Instance.scale;
		CS$<>8__locals1.bodyHit = false;
		CS$<>8__locals1.leftHandHit = false;
		this.nonAllocHits = Physics.SphereCastNonAlloc(this.lastLeftHandPositionForTag, num5, this.leftRaycastSweep.normalized, this.nonAllocRaycastHits, Mathf.Max(this.leftRaycastSweep.magnitude, num5), this.gorillaTagColliderLayerMask, QueryTriggerInteraction.Collide);
		this.<LateUpdate>g__TryTaggingAllHits|112_0(false, true, ref CS$<>8__locals1);
		this.nonAllocHits = Physics.SphereCastNonAlloc(this.headCollider.transform.position, num5, this.leftHeadRaycastSweep.normalized, this.nonAllocRaycastHits, Mathf.Max(this.leftHeadRaycastSweep.magnitude, num5), this.gorillaTagColliderLayerMask, QueryTriggerInteraction.Collide);
		this.<LateUpdate>g__TryTaggingAllHits|112_0(false, true, ref CS$<>8__locals1);
		this.nonAllocHits = Physics.SphereCastNonAlloc(this.lastRightHandPositionForTag, num5, this.rightRaycastSweep.normalized, this.nonAllocRaycastHits, Mathf.Max(this.rightRaycastSweep.magnitude, num5), this.gorillaTagColliderLayerMask, QueryTriggerInteraction.Collide);
		this.<LateUpdate>g__TryTaggingAllHits|112_0(false, false, ref CS$<>8__locals1);
		this.nonAllocHits = Physics.SphereCastNonAlloc(this.headCollider.transform.position, num5, this.rightHeadRaycastSweep.normalized, this.nonAllocRaycastHits, Mathf.Max(this.rightHeadRaycastSweep.magnitude, num5), this.gorillaTagColliderLayerMask, QueryTriggerInteraction.Collide);
		this.<LateUpdate>g__TryTaggingAllHits|112_0(false, false, ref CS$<>8__locals1);
		this.nonAllocHits = Physics.SphereCastNonAlloc(this.headCollider.transform.position, this.headCollider.radius * this.headCollider.transform.localScale.x * GTPlayer.Instance.scale, this.headRaycastSweep.normalized, this.nonAllocRaycastHits, Mathf.Max(this.headRaycastSweep.magnitude, num5), this.gorillaTagColliderLayerMask, QueryTriggerInteraction.Collide);
		this.<LateUpdate>g__TryTaggingAllHits|112_0(true, false, ref CS$<>8__locals1);
		this.topVector = this.lastBodyPositionForTag + this.bodyVector;
		this.bottomVector = this.lastBodyPositionForTag - this.bodyVector;
		this.nonAllocHits = Physics.CapsuleCastNonAlloc(this.topVector, this.bottomVector, this.bodyCollider.radius * 2f * GTPlayer.Instance.scale, this.bodyRaycastSweep.normalized, this.nonAllocRaycastHits, Mathf.Max(this.bodyRaycastSweep.magnitude, num5), this.gorillaTagColliderLayerMask, QueryTriggerInteraction.Collide);
		this.<LateUpdate>g__TryTaggingAllHits|112_0(true, false, ref CS$<>8__locals1);
		if (this.otherPlayer != null)
		{
			GameMode.ActiveGameMode.LocalTag(this.otherPlayer, NetworkSystem.Instance.LocalPlayer, CS$<>8__locals1.bodyHit, CS$<>8__locals1.leftHandHit);
			GameMode.ReportTag(this.otherPlayer);
		}
		if (CS$<>8__locals1.otherTouchedPlayer != null && GorillaGameManager.instance != null)
		{
			CustomGameMode.TouchPlayer(CS$<>8__locals1.otherTouchedPlayer);
		}
		if (CS$<>8__locals1.otherTouchedPlayer != null)
		{
			this.HitWithKnockBack(CS$<>8__locals1.otherTouchedPlayer, NetworkSystem.Instance.LocalPlayer, CS$<>8__locals1.leftHandHit);
		}
		GTPlayer instance = GTPlayer.Instance;
		bool flag = true;
		this.ProcessHandTapping(in flag, ref this.lastLeftTap, ref this.leftHandTouching, in instance.leftHandMaterialTouchIndex, in instance.leftHandSurfaceOverride, in instance.leftHandHitInfo, in instance.leftHandFollower, in this.leftHandSlideSource, in instance.leftHandCenterVelocityTracker);
		flag = false;
		this.ProcessHandTapping(in flag, ref this.lastRightTap, ref this.rightHandTouching, in instance.rightHandMaterialTouchIndex, in instance.rightHandSurfaceOverride, in instance.rightHandHitInfo, in instance.rightHandFollower, in this.rightHandSlideSource, in instance.rightHandCenterVelocityTracker);
		this.CheckEndStatusEffect();
		this.lastLeftHandPositionForTag = this.leftHandTransform.position;
		this.lastRightHandPositionForTag = this.rightHandTransform.position;
		this.lastBodyPositionForTag = this.bodyCollider.transform.position;
		this.lastHeadPositionForTag = this.headCollider.transform.position;
		if (GTPlayer.Instance.IsBodySliding && (double)GTPlayer.Instance.RigidbodyVelocity.magnitude >= 0.15)
		{
			if (!this.bodySlideSource.isPlaying)
			{
				this.bodySlideSource.Play();
			}
		}
		else
		{
			this.bodySlideSource.Stop();
		}
		if (GorillaComputer.instance == null || NetworkSystem.Instance.LocalRecorder == null)
		{
			return;
		}
		if (float.IsFinite(GorillaTagger.moderationMutedTime) && GorillaTagger.moderationMutedTime >= 0f)
		{
			GorillaTagger.moderationMutedTime -= Time.deltaTime;
		}
		if (GorillaComputer.instance.voiceChatOn == "TRUE")
		{
			this.myRecorder = NetworkSystem.Instance.LocalRecorder;
			if (this.offlineVRRig.remoteUseReplacementVoice)
			{
				this.offlineVRRig.remoteUseReplacementVoice = false;
			}
			if (GorillaTagger.moderationMutedTime > 0f)
			{
				this.myRecorder.TransmitEnabled = false;
			}
			if (GorillaComputer.instance.pttType != "ALL CHAT")
			{
				this.primaryButtonPressRight = false;
				this.secondaryButtonPressRight = false;
				this.primaryButtonPressLeft = false;
				this.secondaryButtonPressLeft = false;
				this.primaryButtonPressRight = ControllerInputPoller.PrimaryButtonPress(XRNode.RightHand);
				this.secondaryButtonPressRight = ControllerInputPoller.SecondaryButtonPress(XRNode.RightHand);
				this.primaryButtonPressLeft = ControllerInputPoller.PrimaryButtonPress(XRNode.LeftHand);
				this.secondaryButtonPressLeft = ControllerInputPoller.SecondaryButtonPress(XRNode.LeftHand);
				if (this.primaryButtonPressRight || this.secondaryButtonPressRight || this.primaryButtonPressLeft || this.secondaryButtonPressLeft)
				{
					if (GorillaComputer.instance.pttType == "PUSH TO MUTE")
					{
						this.offlineVRRig.shouldSendSpeakingLoudness = false;
						bool transmitEnabled = this.myRecorder.TransmitEnabled;
						this.myRecorder.TransmitEnabled = false;
						return;
					}
					if (GorillaComputer.instance.pttType == "PUSH TO TALK")
					{
						this.offlineVRRig.shouldSendSpeakingLoudness = true;
						if (GorillaTagger.moderationMutedTime <= 0f && !this.myRecorder.TransmitEnabled)
						{
							this.myRecorder.TransmitEnabled = true;
							return;
						}
					}
				}
				else if (GorillaComputer.instance.pttType == "PUSH TO MUTE")
				{
					this.offlineVRRig.shouldSendSpeakingLoudness = true;
					if (GorillaTagger.moderationMutedTime <= 0f && !this.myRecorder.TransmitEnabled)
					{
						this.myRecorder.TransmitEnabled = true;
						return;
					}
				}
				else if (GorillaComputer.instance.pttType == "PUSH TO TALK")
				{
					this.offlineVRRig.shouldSendSpeakingLoudness = false;
					bool transmitEnabled2 = this.myRecorder.TransmitEnabled;
					this.myRecorder.TransmitEnabled = false;
					return;
				}
			}
			else
			{
				if (GorillaTagger.moderationMutedTime <= 0f && !this.myRecorder.TransmitEnabled)
				{
					this.myRecorder.TransmitEnabled = true;
				}
				if (!this.offlineVRRig.shouldSendSpeakingLoudness)
				{
					this.offlineVRRig.shouldSendSpeakingLoudness = true;
					return;
				}
			}
		}
		else if (GorillaComputer.instance.voiceChatOn == "FALSE")
		{
			this.myRecorder = NetworkSystem.Instance.LocalRecorder;
			if (!this.offlineVRRig.remoteUseReplacementVoice)
			{
				this.offlineVRRig.remoteUseReplacementVoice = true;
			}
			if (this.myRecorder.TransmitEnabled)
			{
				this.myRecorder.TransmitEnabled = false;
			}
			if (GorillaComputer.instance.pttType != "ALL CHAT")
			{
				this.primaryButtonPressRight = false;
				this.secondaryButtonPressRight = false;
				this.primaryButtonPressLeft = false;
				this.secondaryButtonPressLeft = false;
				this.primaryButtonPressRight = ControllerInputPoller.PrimaryButtonPress(XRNode.RightHand);
				this.secondaryButtonPressRight = ControllerInputPoller.SecondaryButtonPress(XRNode.RightHand);
				this.primaryButtonPressLeft = ControllerInputPoller.PrimaryButtonPress(XRNode.LeftHand);
				this.secondaryButtonPressLeft = ControllerInputPoller.SecondaryButtonPress(XRNode.LeftHand);
				if (this.primaryButtonPressRight || this.secondaryButtonPressRight || this.primaryButtonPressLeft || this.secondaryButtonPressLeft)
				{
					if (GorillaComputer.instance.pttType == "PUSH TO MUTE")
					{
						this.offlineVRRig.shouldSendSpeakingLoudness = false;
						return;
					}
					if (GorillaComputer.instance.pttType == "PUSH TO TALK")
					{
						this.offlineVRRig.shouldSendSpeakingLoudness = true;
						return;
					}
				}
				else
				{
					if (GorillaComputer.instance.pttType == "PUSH TO MUTE")
					{
						this.offlineVRRig.shouldSendSpeakingLoudness = true;
						return;
					}
					if (GorillaComputer.instance.pttType == "PUSH TO TALK")
					{
						this.offlineVRRig.shouldSendSpeakingLoudness = false;
						return;
					}
				}
			}
			else if (!this.offlineVRRig.shouldSendSpeakingLoudness)
			{
				this.offlineVRRig.shouldSendSpeakingLoudness = true;
				return;
			}
		}
		else
		{
			this.myRecorder = NetworkSystem.Instance.LocalRecorder;
			if (this.offlineVRRig.remoteUseReplacementVoice)
			{
				this.offlineVRRig.remoteUseReplacementVoice = false;
			}
			if (this.offlineVRRig.shouldSendSpeakingLoudness)
			{
				this.offlineVRRig.shouldSendSpeakingLoudness = false;
			}
			if (this.myRecorder.TransmitEnabled)
			{
				this.myRecorder.TransmitEnabled = false;
			}
		}
	}

	// Token: 0x060027C7 RID: 10183 RVA: 0x000C60C8 File Offset: 0x000C42C8
	private bool TryToTag(RaycastHit hitInfo, bool isBodyTag, out NetPlayer taggedPlayer, out NetPlayer touchedPlayer)
	{
		taggedPlayer = null;
		touchedPlayer = null;
		if (NetworkSystem.Instance.InRoom)
		{
			VRRig componentInParent = hitInfo.collider.GetComponentInParent<VRRig>();
			this.tempCreator = ((componentInParent != null) ? componentInParent.creator : null);
			if (this.tempCreator != null && NetworkSystem.Instance.LocalPlayer != this.tempCreator)
			{
				touchedPlayer = this.tempCreator;
				if (GorillaGameManager.instance != null && Time.time > this.taggedTime + this.tagCooldown && GorillaGameManager.instance.LocalCanTag(NetworkSystem.Instance.LocalPlayer, this.tempCreator))
				{
					if (!isBodyTag)
					{
						this.StartVibration((this.leftHandTransform.position - hitInfo.collider.transform.position).magnitude < (this.rightHandTransform.position - hitInfo.collider.transform.position).magnitude, this.tagHapticStrength, this.tagHapticDuration);
					}
					else
					{
						this.StartVibration(true, this.tagHapticStrength, this.tagHapticDuration);
						this.StartVibration(false, this.tagHapticStrength, this.tagHapticDuration);
					}
					taggedPlayer = this.tempCreator;
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x060027C8 RID: 10184 RVA: 0x000C6218 File Offset: 0x000C4418
	private void HitWithKnockBack(NetPlayer taggedPlayer, NetPlayer taggingPlayer, bool leftHand)
	{
		Vector3 averageVelocity = (leftHand ? GTPlayer.Instance.leftHandCenterVelocityTracker : GTPlayer.Instance.rightHandCenterVelocityTracker).GetAverageVelocity(true, 0.15f, false);
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(taggingPlayer, out rigContainer))
		{
			return;
		}
		VRMap vrmap = (leftHand ? rigContainer.Rig.leftHand : rigContainer.Rig.rightHand);
		Vector3 vector = (leftHand ? (-vrmap.rigTarget.right) : vrmap.rigTarget.right);
		RigContainer rigContainer2;
		CosmeticEffectsOnPlayers.CosmeticEffect cosmeticEffect;
		if (VRRigCache.Instance.TryGetVrrig(taggedPlayer, out rigContainer2) && rigContainer2.Rig.TemporaryCosmeticEffects.TryGetValue(CosmeticEffectsOnPlayers.EFFECTTYPE.KNOCKBACK, out cosmeticEffect))
		{
			RoomSystem.HitPlayer(taggedPlayer, vector, averageVelocity.magnitude * cosmeticEffect.knockbackStrengthMultiplier);
		}
	}

	// Token: 0x060027C9 RID: 10185 RVA: 0x000C62D3 File Offset: 0x000C44D3
	public void StartVibration(bool forLeftController, float amplitude, float duration)
	{
		base.StartCoroutine(this.HapticPulses(forLeftController, amplitude, duration));
	}

	// Token: 0x060027CA RID: 10186 RVA: 0x000C62E5 File Offset: 0x000C44E5
	private IEnumerator HapticPulses(bool forLeftController, float amplitude, float duration)
	{
		float startTime = Time.time;
		uint channel = 0U;
		global::UnityEngine.XR.InputDevice device;
		if (forLeftController)
		{
			device = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
		}
		else
		{
			device = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
		}
		while (Time.time < startTime + duration)
		{
			device.SendHapticImpulse(channel, amplitude, this.hapticWaitSeconds);
			yield return new WaitForSeconds(this.hapticWaitSeconds * 0.9f);
		}
		yield break;
	}

	// Token: 0x060027CB RID: 10187 RVA: 0x000C630C File Offset: 0x000C450C
	public void PlayHapticClip(bool forLeftController, AudioClip clip, float strength)
	{
		if (forLeftController)
		{
			if (this.leftHapticsRoutine != null)
			{
				base.StopCoroutine(this.leftHapticsRoutine);
			}
			this.leftHapticsRoutine = base.StartCoroutine(this.AudioClipHapticPulses(forLeftController, clip, strength));
			return;
		}
		if (this.rightHapticsRoutine != null)
		{
			base.StopCoroutine(this.rightHapticsRoutine);
		}
		this.rightHapticsRoutine = base.StartCoroutine(this.AudioClipHapticPulses(forLeftController, clip, strength));
	}

	// Token: 0x060027CC RID: 10188 RVA: 0x000C636F File Offset: 0x000C456F
	public void StopHapticClip(bool forLeftController)
	{
		if (forLeftController)
		{
			if (this.leftHapticsRoutine != null)
			{
				base.StopCoroutine(this.leftHapticsRoutine);
				this.leftHapticsRoutine = null;
				return;
			}
		}
		else if (this.rightHapticsRoutine != null)
		{
			base.StopCoroutine(this.rightHapticsRoutine);
			this.rightHapticsRoutine = null;
		}
	}

	// Token: 0x060027CD RID: 10189 RVA: 0x000C63AB File Offset: 0x000C45AB
	private IEnumerator AudioClipHapticPulses(bool forLeftController, AudioClip clip, float strength)
	{
		uint channel = 0U;
		int bufferSize = 8192;
		int sampleWindowSize = 256;
		float[] audioData;
		global::UnityEngine.XR.InputDevice device;
		if (forLeftController)
		{
			float[] array;
			if ((array = this.leftHapticsBuffer) == null)
			{
				array = (this.leftHapticsBuffer = new float[bufferSize]);
			}
			audioData = array;
			device = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
		}
		else
		{
			float[] array2;
			if ((array2 = this.rightHapticsBuffer) == null)
			{
				array2 = (this.rightHapticsBuffer = new float[bufferSize]);
			}
			audioData = array2;
			device = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
		}
		int sampleOffset = -bufferSize;
		float startTime = Time.time;
		float length = clip.length;
		float endTime = Time.time + length;
		float sampleRate = (float)clip.samples;
		while (Time.time <= endTime)
		{
			float num = (Time.time - startTime) / length;
			int num2 = (int)(sampleRate * num);
			if (Mathf.Max(num2 + sampleWindowSize - 1, audioData.Length - 1) >= sampleOffset + bufferSize)
			{
				clip.GetData(audioData, num2);
				sampleOffset = num2;
			}
			float num3 = 0f;
			int num4 = Mathf.Min(clip.samples - num2, sampleWindowSize);
			for (int i = 0; i < num4; i++)
			{
				float num5 = audioData[num2 - sampleOffset + i];
				num3 += num5 * num5;
			}
			float num6 = Mathf.Clamp01(((num4 > 0) ? Mathf.Sqrt(num3 / (float)num4) : 0f) * strength);
			device.SendHapticImpulse(channel, num6, Time.fixedDeltaTime);
			yield return null;
		}
		if (forLeftController)
		{
			this.leftHapticsRoutine = null;
		}
		else
		{
			this.rightHapticsRoutine = null;
		}
		yield break;
	}

	// Token: 0x060027CE RID: 10190 RVA: 0x000C63D0 File Offset: 0x000C45D0
	public void DoVibration(XRNode node, float amplitude, float duration)
	{
		global::UnityEngine.XR.InputDevice deviceAtXRNode = InputDevices.GetDeviceAtXRNode(node);
		if (deviceAtXRNode.isValid)
		{
			deviceAtXRNode.SendHapticImpulse(0U, amplitude, duration);
		}
	}

	// Token: 0x060027CF RID: 10191 RVA: 0x000C63F8 File Offset: 0x000C45F8
	public void UpdateColor(float red, float green, float blue)
	{
		this.offlineVRRig.InitializeNoobMaterialLocal(red, green, blue);
		if (NetworkSystem.Instance != null && !NetworkSystem.Instance.InRoom)
		{
			this.offlineVRRig.mainSkin.sharedMaterial = this.offlineVRRig.materialsToChangeTo[0];
		}
	}

	// Token: 0x060027D0 RID: 10192 RVA: 0x000C644C File Offset: 0x000C464C
	protected void OnTriggerEnter(Collider other)
	{
		GorillaTriggerBox gorillaTriggerBox;
		if (other.TryGetComponent<GorillaTriggerBox>(out gorillaTriggerBox))
		{
			gorillaTriggerBox.OnBoxTriggered();
		}
	}

	// Token: 0x060027D1 RID: 10193 RVA: 0x000C646C File Offset: 0x000C466C
	protected void OnTriggerExit(Collider other)
	{
		GorillaTriggerBox gorillaTriggerBox;
		if (other.TryGetComponent<GorillaTriggerBox>(out gorillaTriggerBox))
		{
			gorillaTriggerBox.OnBoxExited();
		}
	}

	// Token: 0x060027D2 RID: 10194 RVA: 0x000C648C File Offset: 0x000C468C
	public void ShowCosmeticParticles(bool showParticles)
	{
		if (showParticles)
		{
			this.mainCamera.GetComponent<Camera>().cullingMask |= UnityLayer.GorillaCosmeticParticle.ToLayerMask();
			this.MirrorCameraCullingMask.value |= UnityLayer.GorillaCosmeticParticle.ToLayerMask();
			return;
		}
		this.mainCamera.GetComponent<Camera>().cullingMask &= ~UnityLayer.GorillaCosmeticParticle.ToLayerMask();
		this.MirrorCameraCullingMask.value &= ~UnityLayer.GorillaCosmeticParticle.ToLayerMask();
	}

	// Token: 0x060027D3 RID: 10195 RVA: 0x000C650D File Offset: 0x000C470D
	public void ApplyStatusEffect(GorillaTagger.StatusEffect newStatus, float duration)
	{
		this.EndStatusEffect(this.currentStatus);
		this.currentStatus = newStatus;
		this.statusEndTime = Time.time + duration;
		switch (newStatus)
		{
		case GorillaTagger.StatusEffect.None:
		case GorillaTagger.StatusEffect.Slowed:
			break;
		case GorillaTagger.StatusEffect.Frozen:
			GTPlayer.Instance.disableMovement = true;
			break;
		default:
			return;
		}
	}

	// Token: 0x060027D4 RID: 10196 RVA: 0x000C654D File Offset: 0x000C474D
	private void CheckEndStatusEffect()
	{
		if (Time.time > this.statusEndTime)
		{
			this.EndStatusEffect(this.currentStatus);
		}
	}

	// Token: 0x060027D5 RID: 10197 RVA: 0x000C6568 File Offset: 0x000C4768
	private void EndStatusEffect(GorillaTagger.StatusEffect effectToEnd)
	{
		switch (effectToEnd)
		{
		case GorillaTagger.StatusEffect.None:
			break;
		case GorillaTagger.StatusEffect.Frozen:
			GTPlayer.Instance.disableMovement = false;
			this.currentStatus = GorillaTagger.StatusEffect.None;
			return;
		case GorillaTagger.StatusEffect.Slowed:
			this.currentStatus = GorillaTagger.StatusEffect.None;
			break;
		default:
			return;
		}
	}

	// Token: 0x060027D6 RID: 10198 RVA: 0x000C6597 File Offset: 0x000C4797
	private float CalcSlideControl(float fps)
	{
		return Mathf.Pow(Mathf.Pow(1f - this.baseSlideControl, 120f), 1f / fps);
	}

	// Token: 0x060027D7 RID: 10199 RVA: 0x000C65BB File Offset: 0x000C47BB
	public static void OnPlayerSpawned(Action action)
	{
		if (GorillaTagger._instance)
		{
			action();
			return;
		}
		GorillaTagger.onPlayerSpawnedRootCallback = (Action)Delegate.Combine(GorillaTagger.onPlayerSpawnedRootCallback, action);
	}

	// Token: 0x060027D8 RID: 10200 RVA: 0x000C65E8 File Offset: 0x000C47E8
	private void ProcessHandTapping(in bool leftHand, ref float lastTapTime, ref bool wasHandTouching, in int handMatIndex, in GorillaSurfaceOverride surfaceOverride, in RaycastHit handHitInfo, in Transform handFollower, in AudioSource handSlideSource, in GorillaVelocityTracker handVelocityTracker)
	{
		bool flag = GTPlayer.Instance.IsHandTouching(leftHand);
		if (GTPlayer.Instance.inOverlay)
		{
			wasHandTouching = flag;
			handSlideSource.GTStop();
			return;
		}
		if (GTPlayer.Instance.IsHandSliding(leftHand))
		{
			this.StartVibration(leftHand, this.tapHapticStrength / 5f, Time.fixedDeltaTime);
			if (!handSlideSource.isPlaying)
			{
				handSlideSource.GTPlay();
			}
		}
		else
		{
			handSlideSource.GTStop();
			if ((!wasHandTouching && flag) || (wasHandTouching && !flag))
			{
				bool flag2 = Time.time > lastTapTime + this.tapCoolDown;
				Tappable tappable = null;
				bool flag3 = surfaceOverride && surfaceOverride.TryGetComponent<Tappable>(out tappable);
				if (flag3 && tappable.overrideTapCooldown)
				{
					flag2 = tappable.CanTap(leftHand);
				}
				if (flag2)
				{
					lastTapTime = Time.time;
					GorillaAmbushManager gorillaAmbushManager = GameMode.ActiveGameMode as GorillaAmbushManager;
					if (gorillaAmbushManager != null && gorillaAmbushManager.IsInfected(NetworkSystem.Instance.LocalPlayer))
					{
						float sqrMagnitude = (handVelocityTracker.GetAverageVelocity(true, 0.03f, false) / GTPlayer.Instance.scale).sqrMagnitude;
						float sqrMagnitude2 = handVelocityTracker.GetAverageVelocity(false, 0.03f, false).sqrMagnitude;
						this.handTapVolume = ((sqrMagnitude > sqrMagnitude2) ? Mathf.Sqrt(sqrMagnitude) : Mathf.Sqrt(sqrMagnitude2));
						this.handTapVolume = Mathf.Clamp(this.handTapVolume, 0f, gorillaAmbushManager.crawlingSpeedForMaxVolume);
					}
					else
					{
						this.handTapVolume = this.cacheHandTapVolume;
					}
					if (surfaceOverride != null)
					{
						this.tempInt = surfaceOverride.overrideIndex;
						if (surfaceOverride.sendOnTapEvent)
						{
							BuilderPieceTappable builderPieceTappable;
							if (flag3)
							{
								tappable.OnTap(this.handTapVolume);
							}
							else if (surfaceOverride.TryGetComponent<BuilderPieceTappable>(out builderPieceTappable))
							{
								builderPieceTappable.OnTapLocal(this.handTapVolume);
							}
						}
						PlayerGameEvents.TapObject(surfaceOverride.name);
					}
					else
					{
						this.tempInt = handMatIndex;
					}
					GorillaFreezeTagManager gorillaFreezeTagManager = GameMode.ActiveGameMode as GorillaFreezeTagManager;
					if (gorillaFreezeTagManager != null && gorillaFreezeTagManager.IsFrozen(NetworkSystem.Instance.LocalPlayer))
					{
						this.tempInt = gorillaFreezeTagManager.GetFrozenHandTapAudioIndex();
					}
					this.StartVibration(leftHand, this.tapHapticStrength, this.tapHapticDuration);
					RaycastHit raycastHit = handHitInfo;
					this.tempHitDir = Vector3.Normalize(raycastHit.point - handFollower.position);
					this.offlineVRRig.OnHandTap(this.tempInt, leftHand, this.handTapVolume, this.tempHitDir);
					if (GameMode.ActiveGameMode != null)
					{
						GorillaGameManager activeGameMode = GameMode.ActiveGameMode;
						NetPlayer localPlayer = NetworkSystem.Instance.LocalPlayer;
						Tappable tappable2 = tappable;
						bool flag4 = leftHand;
						Vector3 averageVelocity = handVelocityTracker.GetAverageVelocity(true, 0.03f, false);
						raycastHit = handHitInfo;
						activeGameMode.HandleHandTap(localPlayer, tappable2, flag4, averageVelocity, raycastHit.normal);
					}
					if (NetworkSystem.Instance.InRoom && this.myVRRig.IsNotNull() && this.myVRRig != null)
					{
						this.myVRRig.GetView.RPC("OnHandTapRPC", RpcTarget.Others, new object[]
						{
							this.tempInt,
							leftHand,
							this.handTapVolume,
							Utils.PackVector3ToLong(this.tempHitDir)
						});
					}
				}
			}
		}
		wasHandTouching = flag;
	}

	// Token: 0x060027D9 RID: 10201 RVA: 0x000C6928 File Offset: 0x000C4B28
	public void DebugDrawTagCasts(Color color)
	{
		float num = this.sphereCastRadius * GTPlayer.Instance.scale;
		this.DrawSphereCast(this.lastLeftHandPositionForTag, this.leftRaycastSweep.normalized, num, Mathf.Max(this.leftRaycastSweep.magnitude, num), color);
		this.DrawSphereCast(this.headCollider.transform.position, this.leftHeadRaycastSweep.normalized, num, Mathf.Max(this.leftHeadRaycastSweep.magnitude, num), color);
		this.DrawSphereCast(this.lastRightHandPositionForTag, this.rightRaycastSweep.normalized, num, Mathf.Max(this.rightRaycastSweep.magnitude, num), color);
		this.DrawSphereCast(this.headCollider.transform.position, this.rightHeadRaycastSweep.normalized, num, Mathf.Max(this.rightHeadRaycastSweep.magnitude, num), color);
	}

	// Token: 0x060027DA RID: 10202 RVA: 0x000C6A03 File Offset: 0x000C4C03
	private void DrawSphereCast(Vector3 start, Vector3 dir, float radius, float dist, Color color)
	{
		DebugUtil.DrawCapsule(start, start + dir * dist, radius, 16, 16, color, true, DebugUtil.Style.Wireframe);
	}

	// Token: 0x060027DB RID: 10203 RVA: 0x000C6A22 File Offset: 0x000C4C22
	private void RecoverMissingRefs()
	{
		if (!this.offlineVRRig)
		{
			this.RecoverMissingRefs_Asdf<AudioSource>(ref this.leftHandSlideSource, "leftHandSlideSource", "./**/Left Arm IK/SlideAudio");
			this.RecoverMissingRefs_Asdf<AudioSource>(ref this.rightHandSlideSource, "rightHandSlideSource", "./**/Right Arm IK/SlideAudio");
		}
	}

	// Token: 0x060027DC RID: 10204 RVA: 0x000C6A60 File Offset: 0x000C4C60
	private void RecoverMissingRefs_Asdf<T>(ref T objRef, string objFieldName, string recoveryPath) where T : Object
	{
		if (objRef)
		{
			return;
		}
		Transform transform;
		if (!this.offlineVRRig.transform.TryFindByPath(recoveryPath, out transform, false))
		{
			Debug.LogError(string.Concat(new string[] { "`", objFieldName, "` reference missing and could not find by path: \"", recoveryPath, "\"" }), this);
		}
		objRef = transform.GetComponentInChildren<T>();
		if (!objRef)
		{
			Debug.LogError(string.Concat(new string[] { "`", objFieldName, "` reference is missing. Found transform with recover path, but did not find the component. Recover path: \"", recoveryPath, "\"" }), this);
		}
	}

	// Token: 0x060027DD RID: 10205 RVA: 0x000C6B16 File Offset: 0x000C4D16
	public void GuidedRefInitialize()
	{
		GuidedRefHub.RegisterReceiverField<GorillaTagger>(this, "offlineVRRig", ref this.offlineVRRig_gRef);
		GuidedRefHub.ReceiverFullyRegistered<GorillaTagger>(this);
	}

	// Token: 0x170003D2 RID: 978
	// (get) Token: 0x060027DE RID: 10206 RVA: 0x000C6B2F File Offset: 0x000C4D2F
	// (set) Token: 0x060027DF RID: 10207 RVA: 0x000C6B37 File Offset: 0x000C4D37
	int IGuidedRefReceiverMono.GuidedRefsWaitingToResolveCount { get; set; }

	// Token: 0x060027E0 RID: 10208 RVA: 0x000C6B40 File Offset: 0x000C4D40
	bool IGuidedRefReceiverMono.GuidedRefTryResolveReference(GuidedRefTryResolveInfo target)
	{
		if (this.offlineVRRig_gRef.fieldId == target.fieldId && this.offlineVRRig == null)
		{
			this.offlineVRRig = target.targetMono.GuidedRefTargetObject as VRRig;
			return this.offlineVRRig != null;
		}
		return false;
	}

	// Token: 0x060027E1 RID: 10209 RVA: 0x000023F4 File Offset: 0x000005F4
	void IGuidedRefReceiverMono.OnAllGuidedRefsResolved()
	{
	}

	// Token: 0x060027E2 RID: 10210 RVA: 0x000023F4 File Offset: 0x000005F4
	void IGuidedRefReceiverMono.OnGuidedRefTargetDestroyed(int fieldId)
	{
	}

	// Token: 0x060027E5 RID: 10213 RVA: 0x00045F89 File Offset: 0x00044189
	Transform IGuidedRefMonoBehaviour.get_transform()
	{
		return base.transform;
	}

	// Token: 0x060027E6 RID: 10214 RVA: 0x00017401 File Offset: 0x00015601
	int IGuidedRefObject.GetInstanceID()
	{
		return base.GetInstanceID();
	}

	// Token: 0x060027E7 RID: 10215 RVA: 0x000C6C4C File Offset: 0x000C4E4C
	[CompilerGenerated]
	private void <LateUpdate>g__TryTaggingAllHits|112_0(bool isBodyTag, bool isLeftHand, ref GorillaTagger.<>c__DisplayClass112_0 A_3)
	{
		for (int i = 0; i < this.nonAllocHits; i++)
		{
			if (this.nonAllocRaycastHits[i].collider.gameObject.activeSelf)
			{
				if (this.TryToTag(this.nonAllocRaycastHits[i], isBodyTag, out this.tryPlayer, out this.touchedPlayer))
				{
					this.otherPlayer = this.tryPlayer;
					A_3.bodyHit = isBodyTag;
					A_3.leftHandHit = isLeftHand;
					return;
				}
				if (this.touchedPlayer != null)
				{
					A_3.otherTouchedPlayer = this.touchedPlayer;
				}
			}
		}
	}

	// Token: 0x04002C3E RID: 11326
	[OnEnterPlay_SetNull]
	private static GorillaTagger _instance;

	// Token: 0x04002C3F RID: 11327
	[OnEnterPlay_Set(false)]
	public static bool hasInstance;

	// Token: 0x04002C40 RID: 11328
	public static float moderationMutedTime = -1f;

	// Token: 0x04002C41 RID: 11329
	public bool inCosmeticsRoom;

	// Token: 0x04002C42 RID: 11330
	public SphereCollider headCollider;

	// Token: 0x04002C43 RID: 11331
	public CapsuleCollider bodyCollider;

	// Token: 0x04002C44 RID: 11332
	private Vector3 lastLeftHandPositionForTag;

	// Token: 0x04002C45 RID: 11333
	private Vector3 lastRightHandPositionForTag;

	// Token: 0x04002C46 RID: 11334
	private Vector3 lastBodyPositionForTag;

	// Token: 0x04002C47 RID: 11335
	private Vector3 lastHeadPositionForTag;

	// Token: 0x04002C48 RID: 11336
	public Transform rightHandTransform;

	// Token: 0x04002C49 RID: 11337
	public Transform leftHandTransform;

	// Token: 0x04002C4A RID: 11338
	public float hapticWaitSeconds = 0.05f;

	// Token: 0x04002C4B RID: 11339
	public float handTapVolume = 0.1f;

	// Token: 0x04002C4C RID: 11340
	public float tapCoolDown = 0.15f;

	// Token: 0x04002C4D RID: 11341
	public float lastLeftTap;

	// Token: 0x04002C4E RID: 11342
	public float lastRightTap;

	// Token: 0x04002C4F RID: 11343
	public float tapHapticDuration = 0.05f;

	// Token: 0x04002C50 RID: 11344
	public float tapHapticStrength = 0.5f;

	// Token: 0x04002C51 RID: 11345
	public float tagHapticDuration = 0.15f;

	// Token: 0x04002C52 RID: 11346
	public float tagHapticStrength = 1f;

	// Token: 0x04002C53 RID: 11347
	public float taggedHapticDuration = 0.35f;

	// Token: 0x04002C54 RID: 11348
	public float taggedHapticStrength = 1f;

	// Token: 0x04002C55 RID: 11349
	private bool leftHandTouching;

	// Token: 0x04002C56 RID: 11350
	private bool rightHandTouching;

	// Token: 0x04002C57 RID: 11351
	public float taggedTime;

	// Token: 0x04002C58 RID: 11352
	public float tagCooldown;

	// Token: 0x04002C59 RID: 11353
	public float slowCooldown = 3f;

	// Token: 0x04002C5A RID: 11354
	public VRRig offlineVRRig;

	// Token: 0x04002C5B RID: 11355
	[FormerlySerializedAs("offlineVRRig_guidedRef")]
	public GuidedRefReceiverFieldInfo offlineVRRig_gRef = new GuidedRefReceiverFieldInfo(false);

	// Token: 0x04002C5C RID: 11356
	public GameObject thirdPersonCamera;

	// Token: 0x04002C5D RID: 11357
	public GameObject mainCamera;

	// Token: 0x04002C5E RID: 11358
	public bool testTutorial;

	// Token: 0x04002C5F RID: 11359
	public bool disableTutorial;

	// Token: 0x04002C60 RID: 11360
	public bool frameRateUpdated;

	// Token: 0x04002C61 RID: 11361
	public GameObject leftHandTriggerCollider;

	// Token: 0x04002C62 RID: 11362
	public GameObject rightHandTriggerCollider;

	// Token: 0x04002C63 RID: 11363
	public AudioSource leftHandSlideSource;

	// Token: 0x04002C64 RID: 11364
	public AudioSource rightHandSlideSource;

	// Token: 0x04002C65 RID: 11365
	public AudioSource bodySlideSource;

	// Token: 0x04002C66 RID: 11366
	public bool overrideNotInFocus;

	// Token: 0x04002C68 RID: 11368
	private Vector3 leftRaycastSweep;

	// Token: 0x04002C69 RID: 11369
	private Vector3 leftHeadRaycastSweep;

	// Token: 0x04002C6A RID: 11370
	private Vector3 rightRaycastSweep;

	// Token: 0x04002C6B RID: 11371
	private Vector3 rightHeadRaycastSweep;

	// Token: 0x04002C6C RID: 11372
	private Vector3 headRaycastSweep;

	// Token: 0x04002C6D RID: 11373
	private Vector3 bodyRaycastSweep;

	// Token: 0x04002C6E RID: 11374
	private global::UnityEngine.XR.InputDevice rightDevice;

	// Token: 0x04002C6F RID: 11375
	private global::UnityEngine.XR.InputDevice leftDevice;

	// Token: 0x04002C70 RID: 11376
	private bool primaryButtonPressRight;

	// Token: 0x04002C71 RID: 11377
	private bool secondaryButtonPressRight;

	// Token: 0x04002C72 RID: 11378
	private bool primaryButtonPressLeft;

	// Token: 0x04002C73 RID: 11379
	private bool secondaryButtonPressLeft;

	// Token: 0x04002C74 RID: 11380
	private RaycastHit hitInfo;

	// Token: 0x04002C75 RID: 11381
	public NetPlayer otherPlayer;

	// Token: 0x04002C76 RID: 11382
	private NetPlayer tryPlayer;

	// Token: 0x04002C77 RID: 11383
	private NetPlayer touchedPlayer;

	// Token: 0x04002C78 RID: 11384
	private Vector3 topVector;

	// Token: 0x04002C79 RID: 11385
	private Vector3 bottomVector;

	// Token: 0x04002C7A RID: 11386
	private Vector3 bodyVector;

	// Token: 0x04002C7B RID: 11387
	private Vector3 tempHitDir;

	// Token: 0x04002C7C RID: 11388
	private int tempInt;

	// Token: 0x04002C7D RID: 11389
	private global::UnityEngine.XR.InputDevice inputDevice;

	// Token: 0x04002C7E RID: 11390
	private bool wasInOverlay;

	// Token: 0x04002C7F RID: 11391
	private PhotonView tempView;

	// Token: 0x04002C80 RID: 11392
	private NetPlayer tempCreator;

	// Token: 0x04002C81 RID: 11393
	private float cacheHandTapVolume;

	// Token: 0x04002C82 RID: 11394
	public GorillaTagger.StatusEffect currentStatus;

	// Token: 0x04002C83 RID: 11395
	public float statusStartTime;

	// Token: 0x04002C84 RID: 11396
	public float statusEndTime;

	// Token: 0x04002C85 RID: 11397
	private float refreshRate;

	// Token: 0x04002C86 RID: 11398
	private float baseSlideControl;

	// Token: 0x04002C87 RID: 11399
	private int gorillaTagColliderLayerMask;

	// Token: 0x04002C88 RID: 11400
	private RaycastHit[] nonAllocRaycastHits = new RaycastHit[30];

	// Token: 0x04002C89 RID: 11401
	private int nonAllocHits;

	// Token: 0x04002C8B RID: 11403
	private bool xrSubsystemIsActive;

	// Token: 0x04002C8C RID: 11404
	public string loadedDeviceName = "";

	// Token: 0x04002C8D RID: 11405
	[SerializeField]
	private LayerMask BaseMirrorCameraCullingMask;

	// Token: 0x04002C8E RID: 11406
	public Watchable<int> MirrorCameraCullingMask;

	// Token: 0x04002C8F RID: 11407
	private float[] leftHapticsBuffer;

	// Token: 0x04002C90 RID: 11408
	private float[] rightHapticsBuffer;

	// Token: 0x04002C91 RID: 11409
	private Coroutine leftHapticsRoutine;

	// Token: 0x04002C92 RID: 11410
	private Coroutine rightHapticsRoutine;

	// Token: 0x04002C93 RID: 11411
	private Callback<GameOverlayActivated_t> gameOverlayActivatedCb;

	// Token: 0x04002C94 RID: 11412
	private bool isGameOverlayActive;

	// Token: 0x04002C95 RID: 11413
	private float? tagRadiusOverride;

	// Token: 0x04002C96 RID: 11414
	private int tagRadiusOverrideFrame = -1;

	// Token: 0x04002C97 RID: 11415
	private static Action onPlayerSpawnedRootCallback;

	// Token: 0x0200063A RID: 1594
	public enum StatusEffect
	{
		// Token: 0x04002C9A RID: 11418
		None,
		// Token: 0x04002C9B RID: 11419
		Frozen,
		// Token: 0x04002C9C RID: 11420
		Slowed,
		// Token: 0x04002C9D RID: 11421
		Dead,
		// Token: 0x04002C9E RID: 11422
		Infected,
		// Token: 0x04002C9F RID: 11423
		It
	}
}
