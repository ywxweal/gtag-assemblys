using System;
using GorillaExtensions;
using GorillaNetworking;
using Photon.Realtime;
using Photon.Voice.Unity;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020006E1 RID: 1761
public class GorillaPlayerScoreboardLine : MonoBehaviour
{
	// Token: 0x06002BC8 RID: 11208 RVA: 0x000D7A56 File Offset: 0x000D5C56
	public void Start()
	{
		this.emptyRigCount = 0;
		this.reportedCheating = false;
		this.reportedHateSpeech = false;
		this.reportedToxicity = false;
	}

	// Token: 0x06002BC9 RID: 11209 RVA: 0x000D7A74 File Offset: 0x000D5C74
	public void InitializeLine()
	{
		this.currentNickname = string.Empty;
		this.UpdatePlayerText();
		if (this.linePlayer == NetworkSystem.Instance.LocalPlayer)
		{
			this.muteButton.gameObject.SetActive(false);
			this.reportButton.gameObject.SetActive(false);
			this.hateSpeechButton.SetActive(false);
			this.toxicityButton.SetActive(false);
			this.cheatingButton.SetActive(false);
			this.cancelButton.SetActive(false);
			return;
		}
		this.muteButton.gameObject.SetActive(true);
		if (GorillaScoreboardTotalUpdater.instance != null && GorillaScoreboardTotalUpdater.instance.reportDict.ContainsKey(this.playerActorNumber))
		{
			GorillaScoreboardTotalUpdater.PlayerReports playerReports = GorillaScoreboardTotalUpdater.instance.reportDict[this.playerActorNumber];
			this.reportedCheating = playerReports.cheating;
			this.reportedHateSpeech = playerReports.hateSpeech;
			this.reportedToxicity = playerReports.toxicity;
			this.reportInProgress = playerReports.pressedReport;
		}
		else
		{
			this.reportedCheating = false;
			this.reportedHateSpeech = false;
			this.reportedToxicity = false;
			this.reportInProgress = false;
		}
		this.reportButton.isOn = this.reportedCheating || this.reportedHateSpeech || this.reportedToxicity;
		this.reportButton.UpdateColor();
		this.SwapToReportState(this.reportInProgress);
		this.muteButton.gameObject.SetActive(true);
		this.isMuteManual = PlayerPrefs.HasKey(this.linePlayer.UserId);
		this.mute = PlayerPrefs.GetInt(this.linePlayer.UserId, 0);
		this.muteButton.isOn = this.mute != 0;
		this.muteButton.isAutoOn = false;
		this.muteButton.UpdateColor();
		if (this.rigContainer != null)
		{
			this.rigContainer.hasManualMute = this.isMuteManual;
			this.rigContainer.Muted = this.mute != 0;
		}
	}

	// Token: 0x06002BCA RID: 11210 RVA: 0x000D7C70 File Offset: 0x000D5E70
	public void SetLineData(NetPlayer netPlayer)
	{
		if (!netPlayer.InRoom || netPlayer == this.linePlayer)
		{
			return;
		}
		if (this.playerActorNumber != netPlayer.ActorNumber)
		{
			this.initTime = Time.time;
		}
		this.playerActorNumber = netPlayer.ActorNumber;
		this.linePlayer = netPlayer;
		this.playerNameValue = netPlayer.NickName ?? "";
		RigContainer rigContainer;
		if (VRRigCache.Instance.TryGetVrrig(netPlayer, out rigContainer))
		{
			this.rigContainer = rigContainer;
			this.playerVRRig = rigContainer.Rig;
		}
		this.InitializeLine();
	}

	// Token: 0x06002BCB RID: 11211 RVA: 0x000D7CF8 File Offset: 0x000D5EF8
	public void UpdateLine()
	{
		if (this.linePlayer != null)
		{
			if (this.playerNameVisible != this.playerVRRig.playerNameVisible)
			{
				this.UpdatePlayerText();
				this.parentScoreboard.IsDirty = true;
			}
			if (this.rigContainer != null)
			{
				if (Time.time > this.initTime + this.emptyRigCooldown)
				{
					if (this.playerVRRig.netView != null)
					{
						this.emptyRigCount = 0;
					}
					else
					{
						this.emptyRigCount++;
						if (this.emptyRigCount > 30)
						{
							GorillaNot.instance.SendReport("empty rig", this.linePlayer.UserId, this.linePlayer.NickName);
						}
					}
				}
				Material material;
				if (this.playerVRRig.setMatIndex == 0)
				{
					material = this.playerVRRig.scoreboardMaterial;
				}
				else
				{
					material = this.playerVRRig.materialsToChangeTo[this.playerVRRig.setMatIndex];
				}
				if (this.playerSwatch.material != material)
				{
					this.playerSwatch.material = material;
				}
				if (this.playerSwatch.color != this.playerVRRig.materialsToChangeTo[0].color)
				{
					this.playerSwatch.color = this.playerVRRig.materialsToChangeTo[0].color;
				}
				if (this.myRecorder == null)
				{
					this.myRecorder = NetworkSystem.Instance.LocalRecorder;
				}
				if (this.playerVRRig != null)
				{
					if (this.playerVRRig.remoteUseReplacementVoice || this.playerVRRig.localUseReplacementVoice || GorillaComputer.instance.voiceChatOn == "FALSE")
					{
						if (this.playerVRRig.SpeakingLoudness > this.playerVRRig.replacementVoiceLoudnessThreshold && !this.rigContainer.ForceMute && !this.rigContainer.Muted)
						{
							this.speakerIcon.enabled = true;
						}
						else
						{
							this.speakerIcon.enabled = false;
						}
					}
					else if ((this.rigContainer.Voice != null && this.rigContainer.Voice.IsSpeaking) || (this.playerVRRig.rigSerializer != null && this.playerVRRig.rigSerializer.IsLocallyOwned && this.myRecorder != null && this.myRecorder.IsCurrentlyTransmitting))
					{
						this.speakerIcon.enabled = true;
					}
					else
					{
						this.speakerIcon.enabled = false;
					}
				}
				else
				{
					this.speakerIcon.enabled = false;
				}
				if (!this.isMuteManual)
				{
					bool isPlayerAutoMuted = this.rigContainer.GetIsPlayerAutoMuted();
					if (this.muteButton.isAutoOn != isPlayerAutoMuted)
					{
						this.muteButton.isAutoOn = isPlayerAutoMuted;
						this.muteButton.UpdateColor();
					}
				}
			}
		}
	}

	// Token: 0x06002BCC RID: 11212 RVA: 0x000D7FC8 File Offset: 0x000D61C8
	private void UpdatePlayerText()
	{
		try
		{
			if (this.rigContainer.IsNull() || this.playerVRRig.IsNull())
			{
				this.playerNameVisible = this.NormalizeName(this.linePlayer.NickName != this.currentNickname, this.linePlayer.NickName);
				this.currentNickname = this.linePlayer.NickName;
			}
			else if (this.rigContainer.Initialized)
			{
				this.playerNameVisible = this.playerVRRig.playerNameVisible;
			}
			else if (this.currentNickname.IsNullOrEmpty() || GorillaComputer.instance.friendJoinCollider.playerIDsCurrentlyTouching.Contains(this.linePlayer.UserId))
			{
				this.playerNameVisible = this.NormalizeName(this.linePlayer.NickName != this.currentNickname, this.linePlayer.NickName);
			}
			bool flag = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Custom_Nametags);
			this.currentNickname = this.linePlayer.NickName;
			this.playerName.text = (flag ? this.playerNameVisible : this.linePlayer.DefaultName);
		}
		catch (Exception)
		{
			this.playerNameVisible = this.linePlayer.DefaultName;
			GorillaNot.instance.SendReport("NmError", this.linePlayer.UserId, this.linePlayer.NickName);
		}
	}

	// Token: 0x06002BCD RID: 11213 RVA: 0x000D8140 File Offset: 0x000D6340
	public void PressButton(bool isOn, GorillaPlayerLineButton.ButtonType buttonType)
	{
		if (buttonType != GorillaPlayerLineButton.ButtonType.Mute)
		{
			if (buttonType == GorillaPlayerLineButton.ButtonType.Report)
			{
				this.SetReportState(true, buttonType);
				return;
			}
			this.SetReportState(false, buttonType);
		}
		else if (this.linePlayer != null && this.playerVRRig != null)
		{
			this.isMuteManual = true;
			this.muteButton.isAutoOn = false;
			this.mute = (isOn ? 1 : 0);
			PlayerPrefs.SetInt(this.linePlayer.UserId, this.mute);
			if (this.rigContainer != null)
			{
				this.rigContainer.hasManualMute = this.isMuteManual;
				this.rigContainer.Muted = this.mute != 0;
			}
			PlayerPrefs.Save();
			this.muteButton.UpdateColor();
			GorillaScoreboardTotalUpdater.ReportMute(this.linePlayer, this.mute);
			return;
		}
	}

	// Token: 0x06002BCE RID: 11214 RVA: 0x000D8218 File Offset: 0x000D6418
	public void SetReportState(bool reportState, GorillaPlayerLineButton.ButtonType buttonType)
	{
		this.canPressNextReportButton = buttonType != GorillaPlayerLineButton.ButtonType.Toxicity && buttonType != GorillaPlayerLineButton.ButtonType.Report;
		this.reportInProgress = reportState;
		if (reportState)
		{
			this.SwapToReportState(true);
		}
		else
		{
			this.SwapToReportState(false);
			if (this.linePlayer != null && buttonType != GorillaPlayerLineButton.ButtonType.Cancel)
			{
				if ((!this.reportedHateSpeech && buttonType == GorillaPlayerLineButton.ButtonType.HateSpeech) || (!this.reportedToxicity && buttonType == GorillaPlayerLineButton.ButtonType.Toxicity) || (!this.reportedCheating && buttonType == GorillaPlayerLineButton.ButtonType.Cheating))
				{
					GorillaPlayerScoreboardLine.ReportPlayer(this.linePlayer.UserId, buttonType, this.playerNameVisible);
					this.doneReporting = true;
				}
				this.reportedCheating = this.reportedCheating || buttonType == GorillaPlayerLineButton.ButtonType.Cheating;
				this.reportedToxicity = this.reportedToxicity || buttonType == GorillaPlayerLineButton.ButtonType.Toxicity;
				this.reportedHateSpeech = this.reportedHateSpeech || buttonType == GorillaPlayerLineButton.ButtonType.HateSpeech;
				this.reportButton.isOn = true;
				this.reportButton.UpdateColor();
			}
		}
		if (GorillaScoreboardTotalUpdater.instance != null)
		{
			GorillaScoreboardTotalUpdater.instance.UpdateLineState(this);
		}
		this.parentScoreboard.RedrawPlayerLines();
	}

	// Token: 0x06002BCF RID: 11215 RVA: 0x000D8324 File Offset: 0x000D6524
	public static void ReportPlayer(string PlayerID, GorillaPlayerLineButton.ButtonType buttonType, string OtherPlayerNickName)
	{
		if (OtherPlayerNickName.Length > 12)
		{
			OtherPlayerNickName.Remove(12);
		}
		WebFlags webFlags = new WebFlags(1);
		NetEventOptions netEventOptions = new NetEventOptions
		{
			Flags = webFlags,
			TargetActors = GorillaPlayerScoreboardLine.targetActors
		};
		byte b = 50;
		object[] array = new object[]
		{
			PlayerID,
			buttonType,
			OtherPlayerNickName,
			NetworkSystem.Instance.LocalPlayer.NickName,
			!NetworkSystem.Instance.SessionIsPrivate,
			NetworkSystem.Instance.RoomStringStripped()
		};
		NetworkSystemRaiseEvent.RaiseEvent(b, array, netEventOptions, true);
	}

	// Token: 0x06002BD0 RID: 11216 RVA: 0x000D83BC File Offset: 0x000D65BC
	public static void MutePlayer(string PlayerID, string OtherPlayerNickName, int muting)
	{
		if (OtherPlayerNickName.Length > 12)
		{
			OtherPlayerNickName.Remove(12);
		}
		WebFlags webFlags = new WebFlags(1);
		NetEventOptions netEventOptions = new NetEventOptions
		{
			Flags = webFlags,
			TargetActors = GorillaPlayerScoreboardLine.targetActors
		};
		byte b = 51;
		object[] array = new object[]
		{
			PlayerID,
			muting,
			OtherPlayerNickName,
			NetworkSystem.Instance.LocalPlayer.NickName,
			!NetworkSystem.Instance.SessionIsPrivate,
			NetworkSystem.Instance.RoomStringStripped()
		};
		NetworkSystemRaiseEvent.RaiseEvent(b, array, netEventOptions, true);
	}

	// Token: 0x06002BD1 RID: 11217 RVA: 0x000D8454 File Offset: 0x000D6654
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
				GorillaNot.instance.SendReport("evading the name ban", this.linePlayer.UserId, this.linePlayer.NickName);
			}
		}
		return text;
	}

	// Token: 0x06002BD2 RID: 11218 RVA: 0x000D84F5 File Offset: 0x000D66F5
	public void ResetData()
	{
		this.emptyRigCount = 0;
		this.playerActorNumber = -1;
		this.linePlayer = null;
		this.playerNameValue = string.Empty;
		this.currentNickname = string.Empty;
	}

	// Token: 0x06002BD3 RID: 11219 RVA: 0x000D8522 File Offset: 0x000D6722
	private void OnEnable()
	{
		GorillaScoreboardTotalUpdater.RegisterSL(this);
	}

	// Token: 0x06002BD4 RID: 11220 RVA: 0x000D852A File Offset: 0x000D672A
	private void OnDisable()
	{
		GorillaScoreboardTotalUpdater.UnregisterSL(this);
	}

	// Token: 0x06002BD5 RID: 11221 RVA: 0x000D8534 File Offset: 0x000D6734
	private void SwapToReportState(bool reportInProgress)
	{
		this.reportButton.gameObject.SetActive(!reportInProgress);
		this.hateSpeechButton.SetActive(reportInProgress);
		this.toxicityButton.SetActive(reportInProgress);
		this.cheatingButton.SetActive(reportInProgress);
		this.cancelButton.SetActive(reportInProgress);
	}

	// Token: 0x040031E0 RID: 12768
	private static int[] targetActors = new int[] { -1 };

	// Token: 0x040031E1 RID: 12769
	public Text playerName;

	// Token: 0x040031E2 RID: 12770
	public Text playerLevel;

	// Token: 0x040031E3 RID: 12771
	public Text playerMMR;

	// Token: 0x040031E4 RID: 12772
	public Image playerSwatch;

	// Token: 0x040031E5 RID: 12773
	public Texture infectedTexture;

	// Token: 0x040031E6 RID: 12774
	public NetPlayer linePlayer;

	// Token: 0x040031E7 RID: 12775
	public VRRig playerVRRig;

	// Token: 0x040031E8 RID: 12776
	public string playerLevelValue;

	// Token: 0x040031E9 RID: 12777
	public string playerMMRValue;

	// Token: 0x040031EA RID: 12778
	public string playerNameValue;

	// Token: 0x040031EB RID: 12779
	public string playerNameVisible;

	// Token: 0x040031EC RID: 12780
	public int playerActorNumber;

	// Token: 0x040031ED RID: 12781
	public GorillaPlayerLineButton muteButton;

	// Token: 0x040031EE RID: 12782
	public GorillaPlayerLineButton reportButton;

	// Token: 0x040031EF RID: 12783
	public GameObject hateSpeechButton;

	// Token: 0x040031F0 RID: 12784
	public GameObject toxicityButton;

	// Token: 0x040031F1 RID: 12785
	public GameObject cheatingButton;

	// Token: 0x040031F2 RID: 12786
	public GameObject cancelButton;

	// Token: 0x040031F3 RID: 12787
	public SpriteRenderer speakerIcon;

	// Token: 0x040031F4 RID: 12788
	public bool canPressNextReportButton = true;

	// Token: 0x040031F5 RID: 12789
	public Text[] texts;

	// Token: 0x040031F6 RID: 12790
	public SpriteRenderer[] sprites;

	// Token: 0x040031F7 RID: 12791
	public MeshRenderer[] meshes;

	// Token: 0x040031F8 RID: 12792
	public Image[] images;

	// Token: 0x040031F9 RID: 12793
	private Recorder myRecorder;

	// Token: 0x040031FA RID: 12794
	private bool isMuteManual;

	// Token: 0x040031FB RID: 12795
	private int mute;

	// Token: 0x040031FC RID: 12796
	private int emptyRigCount;

	// Token: 0x040031FD RID: 12797
	public GameObject myRig;

	// Token: 0x040031FE RID: 12798
	public bool reportedCheating;

	// Token: 0x040031FF RID: 12799
	public bool reportedToxicity;

	// Token: 0x04003200 RID: 12800
	public bool reportedHateSpeech;

	// Token: 0x04003201 RID: 12801
	public bool reportInProgress;

	// Token: 0x04003202 RID: 12802
	private string currentNickname;

	// Token: 0x04003203 RID: 12803
	public bool doneReporting;

	// Token: 0x04003204 RID: 12804
	public bool lastVisible = true;

	// Token: 0x04003205 RID: 12805
	public GorillaScoreBoard parentScoreboard;

	// Token: 0x04003206 RID: 12806
	public float initTime;

	// Token: 0x04003207 RID: 12807
	public float emptyRigCooldown = 10f;

	// Token: 0x04003208 RID: 12808
	internal RigContainer rigContainer;
}
