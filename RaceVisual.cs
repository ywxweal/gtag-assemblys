using System;
using TMPro;
using UnityEngine;

// Token: 0x020001FB RID: 507
public class RaceVisual : MonoBehaviour
{
	// Token: 0x17000129 RID: 297
	// (get) Token: 0x06000BB7 RID: 2999 RVA: 0x0003E543 File Offset: 0x0003C743
	// (set) Token: 0x06000BB8 RID: 3000 RVA: 0x0003E54B File Offset: 0x0003C74B
	public int raceId { get; private set; }

	// Token: 0x1700012A RID: 298
	// (get) Token: 0x06000BB9 RID: 3001 RVA: 0x0003E554 File Offset: 0x0003C754
	// (set) Token: 0x06000BBA RID: 3002 RVA: 0x0003E55C File Offset: 0x0003C75C
	public bool TickRunning { get; set; }

	// Token: 0x06000BBB RID: 3003 RVA: 0x0003E565 File Offset: 0x0003C765
	private void Awake()
	{
		this.checkpoints = base.GetComponent<RaceCheckpointManager>();
		this.finishLineText.text = "";
		this.SetScoreboardText("", "");
		this.SetRaceStartScoreboardText("", "");
	}

	// Token: 0x06000BBC RID: 3004 RVA: 0x0003E5A3 File Offset: 0x0003C7A3
	private void OnEnable()
	{
		RacingManager.instance.RegisterVisual(this);
	}

	// Token: 0x06000BBD RID: 3005 RVA: 0x0003E5B0 File Offset: 0x0003C7B0
	public void Button_StartRace(int laps)
	{
		RacingManager.instance.Button_StartRace(this.raceId, laps);
	}

	// Token: 0x06000BBE RID: 3006 RVA: 0x0003E5C3 File Offset: 0x0003C7C3
	public void ShowFinishLineText(string text)
	{
		this.finishLineText.text = text;
	}

	// Token: 0x06000BBF RID: 3007 RVA: 0x0003E5D1 File Offset: 0x0003C7D1
	public void UpdateCountdown(int timeRemaining)
	{
		if (timeRemaining != this.lastDisplayedCountdown)
		{
			this.countdownText.text = timeRemaining.ToString();
			this.finishLineText.text = "";
			this.lastDisplayedCountdown = timeRemaining;
		}
	}

	// Token: 0x06000BC0 RID: 3008 RVA: 0x0003E608 File Offset: 0x0003C808
	public void SetScoreboardText(string mainText, string timesText)
	{
		foreach (RacingScoreboard racingScoreboard in this.raceScoreboards)
		{
			racingScoreboard.mainDisplay.text = mainText;
			racingScoreboard.timesDisplay.text = timesText;
		}
	}

	// Token: 0x06000BC1 RID: 3009 RVA: 0x0003E644 File Offset: 0x0003C844
	public void SetRaceStartScoreboardText(string mainText, string timesText)
	{
		this.raceStartScoreboard.mainDisplay.text = mainText;
		this.raceStartScoreboard.timesDisplay.text = timesText;
	}

	// Token: 0x06000BC2 RID: 3010 RVA: 0x0003E668 File Offset: 0x0003C868
	public void ActivateStartingWall(bool enable)
	{
		this.startingWall.SetActive(enable);
	}

	// Token: 0x06000BC3 RID: 3011 RVA: 0x0003E676 File Offset: 0x0003C876
	public bool IsPlayerNearCheckpoint(VRRig player, int checkpoint)
	{
		return this.checkpoints.IsPlayerNearCheckpoint(player, checkpoint);
	}

	// Token: 0x06000BC4 RID: 3012 RVA: 0x0003E685 File Offset: 0x0003C885
	public void OnCountdownStart(int laps, float goAfterInterval)
	{
		this.raceConsoleVisual.ShowRaceInProgress(laps);
		this.countdownSoundPlayer.Play();
		this.countdownSoundPlayer.time = this.countdownSoundGoTime - goAfterInterval;
	}

	// Token: 0x06000BC5 RID: 3013 RVA: 0x0003E6B1 File Offset: 0x0003C8B1
	public void OnRaceStart()
	{
		this.finishLineText.text = "GO!";
		this.checkpoints.OnRaceStart();
		this.lastDisplayedCountdown = 0;
		this.startingWall.SetActive(false);
		this.isRaceEndSoundEnabled = false;
	}

	// Token: 0x06000BC6 RID: 3014 RVA: 0x0003E6E8 File Offset: 0x0003C8E8
	public void OnRaceEnded()
	{
		this.finishLineText.text = "";
		this.lastDisplayedCountdown = 0;
		this.checkpoints.OnRaceEnd();
	}

	// Token: 0x06000BC7 RID: 3015 RVA: 0x0003E70C File Offset: 0x0003C90C
	public void OnRaceReset()
	{
		this.raceConsoleVisual.ShowCanStartRace();
	}

	// Token: 0x06000BC8 RID: 3016 RVA: 0x0003E719 File Offset: 0x0003C919
	public void EnableRaceEndSound()
	{
		this.isRaceEndSoundEnabled = true;
	}

	// Token: 0x06000BC9 RID: 3017 RVA: 0x0003E722 File Offset: 0x0003C922
	public void OnCheckpointPassed(int index, SoundBankPlayer checkpointSound)
	{
		if (index == 0 && this.isRaceEndSoundEnabled)
		{
			this.countdownSoundPlayer.PlayOneShot(this.raceEndSound);
		}
		else
		{
			checkpointSound.Play();
		}
		RacingManager.instance.OnCheckpointPassed(this.raceId, index);
	}

	// Token: 0x04000E4A RID: 3658
	[SerializeField]
	private TextMeshPro finishLineText;

	// Token: 0x04000E4B RID: 3659
	[SerializeField]
	private TextMeshPro countdownText;

	// Token: 0x04000E4C RID: 3660
	[SerializeField]
	private RacingScoreboard[] raceScoreboards;

	// Token: 0x04000E4D RID: 3661
	[SerializeField]
	private RacingScoreboard raceStartScoreboard;

	// Token: 0x04000E4E RID: 3662
	[SerializeField]
	private RaceConsoleVisual raceConsoleVisual;

	// Token: 0x04000E4F RID: 3663
	private float nextVisualRefreshTimestamp;

	// Token: 0x04000E50 RID: 3664
	private RaceCheckpointManager checkpoints;

	// Token: 0x04000E51 RID: 3665
	[SerializeField]
	private AudioClip raceEndSound;

	// Token: 0x04000E52 RID: 3666
	[SerializeField]
	private float countdownSoundGoTime;

	// Token: 0x04000E53 RID: 3667
	[SerializeField]
	private AudioSource countdownSoundPlayer;

	// Token: 0x04000E54 RID: 3668
	[SerializeField]
	private GameObject startingWall;

	// Token: 0x04000E55 RID: 3669
	private int lastDisplayedCountdown;

	// Token: 0x04000E56 RID: 3670
	private bool isRaceEndSoundEnabled;
}
