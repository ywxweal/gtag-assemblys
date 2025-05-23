using System;
using GameObjectScheduling;
using GorillaNetworking;
using TMPro;
using UnityEngine;

// Token: 0x02000695 RID: 1685
public class ModeSelectButton : GorillaPressableButton
{
	// Token: 0x17000411 RID: 1041
	// (get) Token: 0x06002A2E RID: 10798 RVA: 0x000D07FE File Offset: 0x000CE9FE
	// (set) Token: 0x06002A2F RID: 10799 RVA: 0x000D0806 File Offset: 0x000CEA06
	public PartyGameModeWarning WarningScreen
	{
		get
		{
			return this.warningScreen;
		}
		set
		{
			this.warningScreen = value;
		}
	}

	// Token: 0x06002A30 RID: 10800 RVA: 0x000D080F File Offset: 0x000CEA0F
	public override void Start()
	{
		base.Start();
		GorillaComputer.instance.currentGameMode.AddCallback(new Action<string>(this.OnGameModeChanged), true);
	}

	// Token: 0x06002A31 RID: 10801 RVA: 0x000D0835 File Offset: 0x000CEA35
	private void OnDestroy()
	{
		if (!ApplicationQuittingState.IsQuitting)
		{
			GorillaComputer.instance.currentGameMode.RemoveCallback(new Action<string>(this.OnGameModeChanged));
		}
	}

	// Token: 0x06002A32 RID: 10802 RVA: 0x000D085B File Offset: 0x000CEA5B
	public override void ButtonActivationWithHand(bool isLeftHand)
	{
		base.ButtonActivationWithHand(isLeftHand);
		if (this.warningScreen.ShouldShowWarning)
		{
			this.warningScreen.Show();
			return;
		}
		GorillaComputer.instance.OnModeSelectButtonPress(this.gameMode, isLeftHand);
	}

	// Token: 0x06002A33 RID: 10803 RVA: 0x000D0890 File Offset: 0x000CEA90
	public void OnGameModeChanged(string newGameMode)
	{
		this.buttonRenderer.material = ((newGameMode.ToLower() == this.gameMode.ToLower()) ? this.pressedMaterial : this.unpressedMaterial);
	}

	// Token: 0x06002A34 RID: 10804 RVA: 0x000D08C3 File Offset: 0x000CEAC3
	public void SetInfo(ModeSelectButtonInfoData info)
	{
		this.SetInfo(info.Mode, info.ModeTitle, info.NewMode, info.CountdownTo);
	}

	// Token: 0x06002A35 RID: 10805 RVA: 0x000D08E4 File Offset: 0x000CEAE4
	public void SetInfo(string Mode, string ModeTitle, bool NewMode, CountdownTextDate CountdownTo)
	{
		this.gameModeTitle.text = ModeTitle;
		this.gameMode = Mode;
		this.newModeSplash.SetActive(NewMode);
		this.limitedCountdown.gameObject.SetActive(false);
		if (CountdownTo == null)
		{
			return;
		}
		this.limitedCountdown.Countdown = CountdownTo;
		this.limitedCountdown.gameObject.SetActive(true);
	}

	// Token: 0x06002A36 RID: 10806 RVA: 0x000D094A File Offset: 0x000CEB4A
	public void HideNewAndLimitedTimeInfo()
	{
		this.limitedCountdown.gameObject.SetActive(false);
		this.newModeSplash.SetActive(false);
	}

	// Token: 0x04002F33 RID: 12083
	[SerializeField]
	public string gameMode;

	// Token: 0x04002F34 RID: 12084
	[SerializeField]
	private PartyGameModeWarning warningScreen;

	// Token: 0x04002F35 RID: 12085
	[SerializeField]
	private TMP_Text gameModeTitle;

	// Token: 0x04002F36 RID: 12086
	[SerializeField]
	private GameObject newModeSplash;

	// Token: 0x04002F37 RID: 12087
	[SerializeField]
	private CountdownText limitedCountdown;
}
