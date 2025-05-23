using System;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaNetworking;
using UnityEngine;

// Token: 0x020005AC RID: 1452
public class GRFirstTimeUserExperience : MonoBehaviour
{
	// Token: 0x06002384 RID: 9092 RVA: 0x000AF9E6 File Offset: 0x000ADBE6
	[ContextMenu("Set Player Pref")]
	private void RemovePlayerPref()
	{
		PlayerPrefs.SetString("spawnInWrongStump", "flagged");
		PlayerPrefs.Save();
	}

	// Token: 0x06002385 RID: 9093 RVA: 0x000B2A84 File Offset: 0x000B0C84
	private void OnEnable()
	{
		this.audioSource = base.GetComponent<AudioSource>();
		this.flickerSphere.SetActive(false);
		this.logoQuad.SetActive(false);
		this.flickerSphereOrigParent = this.flickerSphere.transform.parent;
		GameLightingManager.instance.SetCustomDynamicLightingEnabled(true);
		this.playerLight = GorillaTagger.Instance.mainCamera.GetComponentInChildren<GameLight>(true);
		this.playerLight.gameObject.SetActive(true);
		this.ChangeState(GRFirstTimeUserExperience.TransitionState.Waiting);
	}

	// Token: 0x06002386 RID: 9094 RVA: 0x000B2B08 File Offset: 0x000B0D08
	public void ChangeState(GRFirstTimeUserExperience.TransitionState state)
	{
		this.transitionState = state;
		switch (state)
		{
		case GRFirstTimeUserExperience.TransitionState.Waiting:
			this.stateStartTime = Time.time;
			return;
		case GRFirstTimeUserExperience.TransitionState.Flicker:
			this.transitionState = GRFirstTimeUserExperience.TransitionState.Flicker;
			this.flickerSphere.transform.SetParent(GTPlayer.Instance.headCollider.transform, false);
			this.flickerSphere.SetActive(true);
			this.logoQuad.SetActive(false);
			this.stateStartTime = Time.time;
			return;
		case GRFirstTimeUserExperience.TransitionState.Logo:
			this.stateStartTime = Time.time;
			this.flickerSphere.SetActive(true);
			this.logoQuad.SetActive(true);
			return;
		case GRFirstTimeUserExperience.TransitionState.ZoneLoad:
		{
			ZoneManagement instance = ZoneManagement.instance;
			instance.OnSceneLoadsCompleted = (Action)Delegate.Combine(instance.OnSceneLoadsCompleted, new Action(this.OnZoneLoadComplete));
			ZoneManagement.SetActiveZone(this.teleportZone);
			return;
		}
		case GRFirstTimeUserExperience.TransitionState.Teleport:
			PhotonNetworkController.Instance.AttemptToJoinPublicRoom(this.joinRoomTrigger, JoinType.Solo);
			GTPlayer.Instance.TeleportTo(this.teleportLocation.position, this.teleportLocation.rotation);
			GTPlayer.Instance.InitializeValues();
			this.stateStartTime = Time.time;
			return;
		case GRFirstTimeUserExperience.TransitionState.Exit:
			this.flickerSphere.transform.SetParent(this.flickerSphereOrigParent, false);
			this.flickerSphere.SetActive(false);
			this.logoQuad.SetActive(false);
			this.rootObject.SetActive(false);
			GorillaTagger.Instance.mainCamera.GetComponentInChildren<GameLight>(true).gameObject.SetActive(false);
			return;
		default:
			return;
		}
	}

	// Token: 0x06002387 RID: 9095 RVA: 0x000B2C87 File Offset: 0x000B0E87
	private void OnZoneLoadComplete()
	{
		ZoneManagement instance = ZoneManagement.instance;
		instance.OnSceneLoadsCompleted = (Action)Delegate.Remove(instance.OnSceneLoadsCompleted, new Action(this.OnZoneLoadComplete));
		this.ChangeState(GRFirstTimeUserExperience.TransitionState.Teleport);
	}

	// Token: 0x06002388 RID: 9096 RVA: 0x000B2CB8 File Offset: 0x000B0EB8
	public void InterruptWaitingTimer()
	{
		this.stateStartTime = -1f;
		for (int i = 0; i < this.delayObjects.Count; i++)
		{
			this.delayObjects[i].enabledTime = this.stateStartTime;
		}
	}

	// Token: 0x06002389 RID: 9097 RVA: 0x000B2D00 File Offset: 0x000B0F00
	private void Update()
	{
		switch (this.transitionState)
		{
		case GRFirstTimeUserExperience.TransitionState.Waiting:
			if (PrivateUIRoom.GetInOverlay())
			{
				if (this.stateStartTime >= 0f)
				{
					this.InterruptWaitingTimer();
				}
			}
			else if (this.stateStartTime < 0f)
			{
				this.stateStartTime = Time.time;
			}
			if (this.stateStartTime >= 0f && Time.time - this.stateStartTime >= this.transitionDelay)
			{
				this.ChangeState(GRFirstTimeUserExperience.TransitionState.Flicker);
				return;
			}
			break;
		case GRFirstTimeUserExperience.TransitionState.Flicker:
		{
			float num = Time.time - this.stateStartTime;
			if (this.stateStartTime >= 0f && num >= this.flickerDuration)
			{
				this.ChangeState(GRFirstTimeUserExperience.TransitionState.Logo);
				return;
			}
			bool flag = this.flickerTimeline.Evaluate(num / this.flickerDuration) < 0f;
			this.flickerSphere.SetActive(flag);
			if (flag && !this.flickerLightWasOff)
			{
				if (this.audioSource != null && this.flickerAudioCount < this.flickerAudio.Count && this.flickerAudio[this.flickerAudioCount] != null)
				{
					this.audioSource.PlayOneShot(this.flickerAudio[this.flickerAudioCount]);
				}
				this.flickerAudioCount++;
			}
			this.flickerLightWasOff = flag;
			return;
		}
		case GRFirstTimeUserExperience.TransitionState.Logo:
			if (this.stateStartTime >= 0f && Time.time - this.stateStartTime >= this.logoDisplayTime)
			{
				this.ChangeState(GRFirstTimeUserExperience.TransitionState.ZoneLoad);
				return;
			}
			break;
		case GRFirstTimeUserExperience.TransitionState.ZoneLoad:
			break;
		case GRFirstTimeUserExperience.TransitionState.Teleport:
			if (this.stateStartTime >= 0f && Time.time - this.stateStartTime >= this.teleportSettleTime)
			{
				this.ChangeState(GRFirstTimeUserExperience.TransitionState.Exit);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x04002822 RID: 10274
	public Transform spawnPoint;

	// Token: 0x04002823 RID: 10275
	public GameObject rootObject;

	// Token: 0x04002824 RID: 10276
	public GameObject flickerSphere;

	// Token: 0x04002825 RID: 10277
	public GameObject logoQuad;

	// Token: 0x04002826 RID: 10278
	public AnimationCurve flickerTimeline;

	// Token: 0x04002827 RID: 10279
	public float flickerDuration = 3f;

	// Token: 0x04002828 RID: 10280
	public GTZone teleportZone = GTZone.none;

	// Token: 0x04002829 RID: 10281
	public Transform teleportLocation;

	// Token: 0x0400282A RID: 10282
	public float transitionDelay = 60f;

	// Token: 0x0400282B RID: 10283
	public float logoDisplayTime = 4f;

	// Token: 0x0400282C RID: 10284
	public float teleportSettleTime = 1f;

	// Token: 0x0400282D RID: 10285
	public GorillaNetworkJoinTrigger joinRoomTrigger;

	// Token: 0x0400282E RID: 10286
	public List<AudioClip> flickerAudio = new List<AudioClip>();

	// Token: 0x0400282F RID: 10287
	public List<DisableGameObjectDelayed> delayObjects;

	// Token: 0x04002830 RID: 10288
	private Transform flickerSphereOrigParent;

	// Token: 0x04002831 RID: 10289
	private float stateStartTime = -1f;

	// Token: 0x04002832 RID: 10290
	private bool flickerLightWasOff;

	// Token: 0x04002833 RID: 10291
	private int flickerAudioCount;

	// Token: 0x04002834 RID: 10292
	private AudioSource audioSource;

	// Token: 0x04002835 RID: 10293
	private GRFirstTimeUserExperience.TransitionState transitionState;

	// Token: 0x04002836 RID: 10294
	public GameLight playerLight;

	// Token: 0x020005AD RID: 1453
	public enum TransitionState
	{
		// Token: 0x04002838 RID: 10296
		Waiting,
		// Token: 0x04002839 RID: 10297
		Flicker,
		// Token: 0x0400283A RID: 10298
		Logo,
		// Token: 0x0400283B RID: 10299
		ZoneLoad,
		// Token: 0x0400283C RID: 10300
		Teleport,
		// Token: 0x0400283D RID: 10301
		Exit
	}
}
