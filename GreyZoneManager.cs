using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000114 RID: 276
public class GreyZoneManager : MonoBehaviourPun, IPunObservable, IInRoomCallbacks
{
	// Token: 0x170000A2 RID: 162
	// (get) Token: 0x06000704 RID: 1796 RVA: 0x000282D6 File Offset: 0x000264D6
	public bool GreyZoneActive
	{
		get
		{
			return this.greyZoneActive;
		}
	}

	// Token: 0x170000A3 RID: 163
	// (get) Token: 0x06000705 RID: 1797 RVA: 0x000282E0 File Offset: 0x000264E0
	public bool GreyZoneAvailable
	{
		get
		{
			bool flag = false;
			if (GorillaComputer.instance != null)
			{
				flag = GorillaComputer.instance.GetServerTime().DayOfYear >= this.greyZoneAvailableDayOfYear;
			}
			return flag;
		}
	}

	// Token: 0x170000A4 RID: 164
	// (get) Token: 0x06000706 RID: 1798 RVA: 0x0002831F File Offset: 0x0002651F
	public int GravityFactorSelection
	{
		get
		{
			return this.gravityFactorOptionSelection;
		}
	}

	// Token: 0x170000A5 RID: 165
	// (get) Token: 0x06000707 RID: 1799 RVA: 0x00028327 File Offset: 0x00026527
	// (set) Token: 0x06000708 RID: 1800 RVA: 0x0002832F File Offset: 0x0002652F
	public bool TickRunning
	{
		get
		{
			return this._tickRunning;
		}
		set
		{
			this._tickRunning = value;
		}
	}

	// Token: 0x170000A6 RID: 166
	// (get) Token: 0x06000709 RID: 1801 RVA: 0x00028338 File Offset: 0x00026538
	public bool HasAuthority
	{
		get
		{
			return !PhotonNetwork.InRoom || base.photonView.IsMine;
		}
	}

	// Token: 0x170000A7 RID: 167
	// (get) Token: 0x0600070A RID: 1802 RVA: 0x0002834E File Offset: 0x0002654E
	public float SummoningProgress
	{
		get
		{
			return this.summoningProgress;
		}
	}

	// Token: 0x0600070B RID: 1803 RVA: 0x00028356 File Offset: 0x00026556
	public void RegisterSummoner(GreyZoneSummoner summoner)
	{
		if (!this.activeSummoners.Contains(summoner))
		{
			this.activeSummoners.Add(summoner);
		}
	}

	// Token: 0x0600070C RID: 1804 RVA: 0x00028372 File Offset: 0x00026572
	public void DeregisterSummoner(GreyZoneSummoner summoner)
	{
		if (this.activeSummoners.Contains(summoner))
		{
			this.activeSummoners.Remove(summoner);
		}
	}

	// Token: 0x0600070D RID: 1805 RVA: 0x0002838F File Offset: 0x0002658F
	public void RegisterMoon(MoonController moon)
	{
		this.moonController = moon;
	}

	// Token: 0x0600070E RID: 1806 RVA: 0x00028398 File Offset: 0x00026598
	public void UnregisterMoon(MoonController moon)
	{
		if (this.moonController == moon)
		{
			this.moonController = null;
		}
	}

	// Token: 0x0600070F RID: 1807 RVA: 0x000283AF File Offset: 0x000265AF
	public void ActivateGreyZoneAuthority()
	{
		this.greyZoneActive = true;
		this.photonConnectedDuringActivation = PhotonNetwork.InRoom;
		this.greyZoneActivationTime = (this.photonConnectedDuringActivation ? PhotonNetwork.Time : ((double)Time.time));
		this.ActivateGreyZoneLocal();
	}

	// Token: 0x06000710 RID: 1808 RVA: 0x000283E4 File Offset: 0x000265E4
	private void ActivateGreyZoneLocal()
	{
		Shader.SetGlobalInt(this._GreyZoneActive, 1);
		GTPlayer instance = GTPlayer.Instance;
		if (instance != null)
		{
			instance.SetGravityOverride(this, new Action<GTPlayer>(this.GravityOverrideFunction));
			this.gravityOverrideSet = true;
		}
		if (MusicManager.Instance != null)
		{
			MusicManager.Instance.FadeOutMusic(2f);
		}
		if (this.audioFadeCoroutine != null)
		{
			base.StopCoroutine(this.audioFadeCoroutine);
		}
		this.audioFadeCoroutine = base.StartCoroutine(this.FadeAudioIn(this.greyZoneAmbience, this.greyZoneAmbienceVolume, this.ambienceFadeTime));
		if (this.greyZoneAmbience != null)
		{
			this.greyZoneAmbience.Play();
		}
		this.greyZoneParticles.gameObject.SetActive(true);
		this.summoningProgress = 1f;
		this.UpdateSummonerVisuals();
		for (int i = 0; i < this.activeSummoners.Count; i++)
		{
			this.activeSummoners[i].OnGreyZoneActivated();
		}
		if (this.OnGreyZoneActivated != null)
		{
			this.OnGreyZoneActivated();
		}
	}

	// Token: 0x06000711 RID: 1809 RVA: 0x000284F8 File Offset: 0x000266F8
	public void DeactivateGreyZoneAuthority()
	{
		this.greyZoneActive = false;
		foreach (KeyValuePair<int, ValueTuple<VRRig, GreyZoneSummoner>> keyValuePair in this.summoningPlayers)
		{
			this.summoningPlayerProgress[keyValuePair.Key] = 0f;
		}
		this.DeactivateGreyZoneLocal();
	}

	// Token: 0x06000712 RID: 1810 RVA: 0x00028568 File Offset: 0x00026768
	private void DeactivateGreyZoneLocal()
	{
		Shader.SetGlobalInt(this._GreyZoneActive, 0);
		if (MusicManager.Instance != null)
		{
			MusicManager.Instance.FadeInMusic(4f);
		}
		if (this.audioFadeCoroutine != null)
		{
			base.StopCoroutine(this.audioFadeCoroutine);
		}
		this.audioFadeCoroutine = base.StartCoroutine(this.FadeAudioOut(this.greyZoneAmbience, this.ambienceFadeTime));
		this.greyZoneParticles.gameObject.SetActive(false);
		this.summoningProgress = 0f;
		this.UpdateSummonerVisuals();
		if (this.OnGreyZoneDeactivated != null)
		{
			this.OnGreyZoneDeactivated();
		}
	}

	// Token: 0x06000713 RID: 1811 RVA: 0x00028610 File Offset: 0x00026810
	public void ForceStopGreyZone()
	{
		this.greyZoneActive = false;
		Shader.SetGlobalInt(this._GreyZoneActive, 0);
		GTPlayer instance = GTPlayer.Instance;
		if (instance != null)
		{
			instance.UnsetGravityOverride(this);
		}
		this.gravityOverrideSet = false;
		if (this.moonController != null)
		{
			this.moonController.UpdateDistance(1f);
		}
		if (MusicManager.Instance != null)
		{
			MusicManager.Instance.FadeInMusic(0f);
		}
		if (this.greyZoneAmbience != null)
		{
			this.greyZoneAmbience.volume = 0f;
			this.greyZoneAmbience.GTStop();
		}
		this.greyZoneParticles.gameObject.SetActive(false);
		this.summoningProgress = 0f;
		this.UpdateSummonerVisuals();
		if (this.OnGreyZoneDeactivated != null)
		{
			this.OnGreyZoneDeactivated();
		}
	}

	// Token: 0x06000714 RID: 1812 RVA: 0x000286F0 File Offset: 0x000268F0
	public void GravityOverrideFunction(GTPlayer player)
	{
		this.gravityReductionAmount = 0f;
		if (this.moonController != null)
		{
			this.gravityReductionAmount = Mathf.InverseLerp(1f - this.skyMonsterDistGravityRampBuffer, this.skyMonsterDistGravityRampBuffer, this.moonController.Distance);
		}
		float num = Mathf.Lerp(1f, this.gravityFactorOptions[this.gravityFactorOptionSelection], this.gravityReductionAmount);
		player.AddForce(Physics.gravity * num * player.scale, ForceMode.Acceleration);
	}

	// Token: 0x06000715 RID: 1813 RVA: 0x00028779 File Offset: 0x00026979
	private IEnumerator FadeAudioIn(AudioSource source, float maxVolume, float duration)
	{
		if (source != null)
		{
			float startingVolume = source.volume;
			float startTime = Time.time;
			source.GTPlay();
			for (float num = 0f; num < 1f; num = (Time.time - startTime) / duration)
			{
				source.volume = Mathf.Lerp(startingVolume, maxVolume, num);
				yield return null;
			}
			source.volume = maxVolume;
		}
		yield break;
	}

	// Token: 0x06000716 RID: 1814 RVA: 0x00028796 File Offset: 0x00026996
	private IEnumerator FadeAudioOut(AudioSource source, float duration)
	{
		if (source != null)
		{
			float startingVolume = source.volume;
			float startTime = Time.time;
			for (float num = 0f; num < 1f; num = (Time.time - startTime) / duration)
			{
				source.volume = Mathf.Lerp(startingVolume, 0f, num);
				yield return null;
			}
			source.volume = 0f;
			source.Stop();
		}
		yield break;
	}

	// Token: 0x06000717 RID: 1815 RVA: 0x000287AC File Offset: 0x000269AC
	public void VRRigEnteredSummonerProximity(VRRig rig, GreyZoneSummoner summoner)
	{
		if (!this.summoningPlayers.ContainsKey(rig.Creator.ActorNumber))
		{
			this.summoningPlayers.Add(rig.Creator.ActorNumber, new ValueTuple<VRRig, GreyZoneSummoner>(rig, summoner));
			this.summoningPlayerProgress.Add(rig.Creator.ActorNumber, 0f);
		}
	}

	// Token: 0x06000718 RID: 1816 RVA: 0x0002880C File Offset: 0x00026A0C
	public void VRRigExitedSummonerProximity(VRRig rig, GreyZoneSummoner summoner)
	{
		if (this.summoningPlayers.ContainsKey(rig.Creator.ActorNumber))
		{
			this.summoningPlayers.Remove(rig.Creator.ActorNumber);
			this.summoningPlayerProgress.Remove(rig.Creator.ActorNumber);
		}
	}

	// Token: 0x06000719 RID: 1817 RVA: 0x00028860 File Offset: 0x00026A60
	private void UpdateSummonerVisuals()
	{
		bool greyZoneAvailable = this.GreyZoneAvailable;
		for (int i = 0; i < this.activeSummoners.Count; i++)
		{
			this.activeSummoners[i].UpdateProgressFeedback(greyZoneAvailable);
		}
	}

	// Token: 0x0600071A RID: 1818 RVA: 0x0002889C File Offset: 0x00026A9C
	private void ValidateSummoningPlayers()
	{
		this.invalidSummoners.Clear();
		foreach (KeyValuePair<int, ValueTuple<VRRig, GreyZoneSummoner>> keyValuePair in this.summoningPlayers)
		{
			VRRig item = keyValuePair.Value.Item1;
			GreyZoneSummoner item2 = keyValuePair.Value.Item2;
			if (item.Creator.ActorNumber != keyValuePair.Key || (item.head.rigTarget.position - item2.SummoningFocusPoint).sqrMagnitude > item2.SummonerMaxDistance * item2.SummonerMaxDistance)
			{
				this.invalidSummoners.Add(keyValuePair.Key);
			}
		}
		foreach (int num in this.invalidSummoners)
		{
			this.summoningPlayers.Remove(num);
			this.summoningPlayerProgress.Remove(num);
		}
	}

	// Token: 0x0600071B RID: 1819 RVA: 0x000289C4 File Offset: 0x00026BC4
	private int DayNightOverrideFunction(int inputIndex)
	{
		int num = 0;
		int num2 = 8;
		int num3 = inputIndex - num;
		int num4 = num2 - inputIndex;
		if (num3 <= 0 || num4 <= 0)
		{
			return inputIndex;
		}
		if (num4 > num3)
		{
			return num2;
		}
		return num;
	}

	// Token: 0x0600071C RID: 1820 RVA: 0x000289EE File Offset: 0x00026BEE
	private void Awake()
	{
		if (GreyZoneManager.Instance == null)
		{
			GreyZoneManager.Instance = this;
			this.greyZoneAmbienceVolume = this.greyZoneAmbience.volume;
			return;
		}
		Object.Destroy(this);
	}

	// Token: 0x0600071D RID: 1821 RVA: 0x00028A24 File Offset: 0x00026C24
	private void OnEnable()
	{
		if (this.forceTimeOfDayToNight)
		{
			BetterDayNightManager instance = BetterDayNightManager.instance;
			if (instance != null)
			{
				instance.SetTimeIndexOverrideFunction(new Func<int, int>(this.DayNightOverrideFunction));
			}
		}
	}

	// Token: 0x0600071E RID: 1822 RVA: 0x00028A5C File Offset: 0x00026C5C
	private void OnDisable()
	{
		this.ForceStopGreyZone();
		if (this.forceTimeOfDayToNight)
		{
			BetterDayNightManager instance = BetterDayNightManager.instance;
			if (instance != null)
			{
				instance.UnsetTimeIndexOverrideFunction();
			}
		}
	}

	// Token: 0x0600071F RID: 1823 RVA: 0x00028A8E File Offset: 0x00026C8E
	private void Update()
	{
		if (this.HasAuthority)
		{
			this.AuthorityUpdate();
		}
		this.SharedUpdate();
	}

	// Token: 0x06000720 RID: 1824 RVA: 0x00028AA4 File Offset: 0x00026CA4
	private void AuthorityUpdate()
	{
		float deltaTime = Time.deltaTime;
		if (this.greyZoneActive)
		{
			this.summoningProgress = 1f;
			double num;
			if (this.photonConnectedDuringActivation && PhotonNetwork.InRoom)
			{
				num = PhotonNetwork.Time;
			}
			else if (!this.photonConnectedDuringActivation && !PhotonNetwork.InRoom)
			{
				num = (double)Time.time;
			}
			else
			{
				num = -100.0;
			}
			if (num > this.greyZoneActivationTime + (double)this.greyZoneActiveDuration || num < this.greyZoneActivationTime - 10.0)
			{
				this.DeactivateGreyZoneAuthority();
				return;
			}
		}
		else if (this.GreyZoneAvailable)
		{
			this.roomPlayerList = PhotonNetwork.PlayerList;
			int num2 = 1;
			if (this.roomPlayerList != null && this.roomPlayerList.Length != 0)
			{
				num2 = Mathf.Max((this.roomPlayerList.Length + 1) / 2, 1);
			}
			float num3 = 0f;
			float num4 = 1f / this.summoningActivationTime;
			foreach (KeyValuePair<int, ValueTuple<VRRig, GreyZoneSummoner>> keyValuePair in this.summoningPlayers)
			{
				VRRig item = keyValuePair.Value.Item1;
				GreyZoneSummoner item2 = keyValuePair.Value.Item2;
				float num5 = this.summoningPlayerProgress[keyValuePair.Key];
				Vector3 vector = item2.SummoningFocusPoint - item.leftHand.rigTarget.position;
				Vector3 vector2 = -item.leftHand.rigTarget.right;
				bool flag = Vector3.Dot(vector, vector2) > 0f;
				Vector3 vector3 = item2.SummoningFocusPoint - item.rightHand.rigTarget.position;
				Vector3 right = item.rightHand.rigTarget.right;
				bool flag2 = Vector3.Dot(vector3, right) > 0f;
				if (flag && flag2)
				{
					num5 = Mathf.MoveTowards(num5, 1f, num4 * deltaTime);
				}
				else
				{
					num5 = Mathf.MoveTowards(num5, 0f, num4 * deltaTime);
				}
				num3 += num5;
				this.summoningPlayerProgress[keyValuePair.Key] = num5;
			}
			float num6 = 0.95f;
			this.summoningProgress = Mathf.Clamp01(num3 / num6 / (float)num2);
			this.UpdateSummonerVisuals();
			if (this.summoningProgress > 0.99f)
			{
				this.ActivateGreyZoneAuthority();
			}
		}
	}

	// Token: 0x06000721 RID: 1825 RVA: 0x00028CFC File Offset: 0x00026EFC
	private void SharedUpdate()
	{
		GTPlayer instance = GTPlayer.Instance;
		if (this.greyZoneActive)
		{
			Vector3 vector = Vector3.ClampMagnitude(instance.InstantaneousVelocity * this.particlePredictiveSpawnVelocityFactor, this.particlePredictiveSpawnMaxDist);
			this.greyZoneParticles.transform.position = instance.HeadCenterPosition + Vector3.down * 0.5f + vector;
		}
		else if (this.gravityOverrideSet && this.gravityReductionAmount < 0.01f)
		{
			instance.UnsetGravityOverride(this);
			this.gravityOverrideSet = false;
		}
		float num = (this.greyZoneActive ? 0f : 1f);
		float num2 = (this.greyZoneActive ? this.skyMonsterMovementEnterTime : this.skyMonsterMovementExitTime);
		if (this.moonController != null && this.moonController.Distance != num)
		{
			float num3 = Mathf.SmoothDamp(this.moonController.Distance, num, ref this.skyMonsterMovementVelocity, num2);
			if ((double)Mathf.Abs(num3 - num) < 0.001)
			{
				num3 = num;
			}
			this.moonController.UpdateDistance(num3);
		}
	}

	// Token: 0x06000722 RID: 1826 RVA: 0x00028E10 File Offset: 0x00027010
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(this.greyZoneActive);
			stream.SendNext(this.greyZoneActivationTime);
			stream.SendNext(this.photonConnectedDuringActivation);
			stream.SendNext(this.gravityFactorOptionSelection);
			stream.SendNext(this.summoningProgress);
			return;
		}
		if (stream.IsReading && info.Sender.IsMasterClient)
		{
			bool flag = this.greyZoneActive;
			this.greyZoneActive = (bool)stream.ReceiveNext();
			this.greyZoneActivationTime = ((double)stream.ReceiveNext()).GetFinite();
			this.photonConnectedDuringActivation = (bool)stream.ReceiveNext();
			this.gravityFactorOptionSelection = (int)stream.ReceiveNext();
			this.summoningProgress = ((float)stream.ReceiveNext()).ClampSafe(0f, 1f);
			this.UpdateSummonerVisuals();
			if (this.greyZoneActive && !flag)
			{
				this.ActivateGreyZoneLocal();
				return;
			}
			if (!this.greyZoneActive && flag)
			{
				this.DeactivateGreyZoneLocal();
			}
		}
	}

	// Token: 0x06000723 RID: 1827 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnPlayerEnteredRoom(Player newPlayer)
	{
	}

	// Token: 0x06000724 RID: 1828 RVA: 0x00028F31 File Offset: 0x00027131
	public void OnPlayerLeftRoom(Player otherPlayer)
	{
		this.ValidateSummoningPlayers();
	}

	// Token: 0x06000725 RID: 1829 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
	{
	}

	// Token: 0x06000726 RID: 1830 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
	{
	}

	// Token: 0x06000727 RID: 1831 RVA: 0x00028F31 File Offset: 0x00027131
	public void OnMasterClientSwitched(Player newMasterClient)
	{
		this.ValidateSummoningPlayers();
	}

	// Token: 0x0400087E RID: 2174
	[OnEnterPlay_SetNull]
	public static volatile GreyZoneManager Instance;

	// Token: 0x0400087F RID: 2175
	[SerializeField]
	private float greyZoneActiveDuration = 90f;

	// Token: 0x04000880 RID: 2176
	[SerializeField]
	private float[] gravityFactorOptions = new float[] { 0.25f, 0.5f, 0.75f };

	// Token: 0x04000881 RID: 2177
	[SerializeField]
	private int gravityFactorOptionSelection = 1;

	// Token: 0x04000882 RID: 2178
	[SerializeField]
	private float summoningActivationTime = 3f;

	// Token: 0x04000883 RID: 2179
	[SerializeField]
	private AudioSource greyZoneAmbience;

	// Token: 0x04000884 RID: 2180
	[SerializeField]
	private float ambienceFadeTime = 4f;

	// Token: 0x04000885 RID: 2181
	[SerializeField]
	private bool forceTimeOfDayToNight;

	// Token: 0x04000886 RID: 2182
	[SerializeField]
	private float skyMonsterMovementEnterTime = 4.5f;

	// Token: 0x04000887 RID: 2183
	[SerializeField]
	private float skyMonsterMovementExitTime = 3.2f;

	// Token: 0x04000888 RID: 2184
	[SerializeField]
	private float skyMonsterDistGravityRampBuffer = 0.15f;

	// Token: 0x04000889 RID: 2185
	[SerializeField]
	[Range(0f, 1f)]
	private float gravityReductionAmount = 1f;

	// Token: 0x0400088A RID: 2186
	[SerializeField]
	private ParticleSystem greyZoneParticles;

	// Token: 0x0400088B RID: 2187
	[SerializeField]
	private float particlePredictiveSpawnMaxDist = 4f;

	// Token: 0x0400088C RID: 2188
	[SerializeField]
	private float particlePredictiveSpawnVelocityFactor = 0.5f;

	// Token: 0x0400088D RID: 2189
	private bool photonConnectedDuringActivation;

	// Token: 0x0400088E RID: 2190
	private double greyZoneActivationTime;

	// Token: 0x0400088F RID: 2191
	private bool greyZoneActive;

	// Token: 0x04000890 RID: 2192
	private bool _tickRunning;

	// Token: 0x04000891 RID: 2193
	private float summoningProgress;

	// Token: 0x04000892 RID: 2194
	private List<GreyZoneSummoner> activeSummoners = new List<GreyZoneSummoner>();

	// Token: 0x04000893 RID: 2195
	private Dictionary<int, ValueTuple<VRRig, GreyZoneSummoner>> summoningPlayers = new Dictionary<int, ValueTuple<VRRig, GreyZoneSummoner>>();

	// Token: 0x04000894 RID: 2196
	private Dictionary<int, float> summoningPlayerProgress = new Dictionary<int, float>();

	// Token: 0x04000895 RID: 2197
	private HashSet<int> invalidSummoners = new HashSet<int>();

	// Token: 0x04000896 RID: 2198
	private Coroutine audioFadeCoroutine;

	// Token: 0x04000897 RID: 2199
	private Player[] roomPlayerList;

	// Token: 0x04000898 RID: 2200
	private ShaderHashId _GreyZoneActive = new ShaderHashId("_GreyZoneActive");

	// Token: 0x04000899 RID: 2201
	private MoonController moonController;

	// Token: 0x0400089A RID: 2202
	private float skyMonsterMovementVelocity;

	// Token: 0x0400089B RID: 2203
	private bool gravityOverrideSet;

	// Token: 0x0400089C RID: 2204
	private float greyZoneAmbienceVolume = 0.15f;

	// Token: 0x0400089D RID: 2205
	private int greyZoneAvailableDayOfYear = new DateTime(2024, 10, 25).DayOfYear;

	// Token: 0x0400089E RID: 2206
	public Action OnGreyZoneActivated;

	// Token: 0x0400089F RID: 2207
	public Action OnGreyZoneDeactivated;
}
