using System;
using Photon.Pun;
using TMPro;
using UnityEngine;

// Token: 0x020004C8 RID: 1224
public class MonkeBallScoreboard : MonoBehaviour
{
	// Token: 0x06001DB4 RID: 7604 RVA: 0x00090A26 File Offset: 0x0008EC26
	public void Setup(MonkeBallGame game)
	{
		this.game = game;
	}

	// Token: 0x06001DB5 RID: 7605 RVA: 0x00090A30 File Offset: 0x0008EC30
	public void RefreshScore()
	{
		for (int i = 0; i < this.game.team.Count; i++)
		{
			this.teamDisplays[i].scoreLabel.text = this.game.team[i].score.ToString();
		}
	}

	// Token: 0x06001DB6 RID: 7606 RVA: 0x00090A85 File Offset: 0x0008EC85
	public void RefreshTeamPlayers(int teamId, int numPlayers)
	{
		this.teamDisplays[teamId].playersLabel.text = string.Format("PLAYERS: {0}", Mathf.Clamp(numPlayers, 0, 99));
	}

	// Token: 0x06001DB7 RID: 7607 RVA: 0x00090AB1 File Offset: 0x0008ECB1
	public void PlayScoreFx()
	{
		this.PlayFX(this.scoreSound, this.scoreSoundVolume);
	}

	// Token: 0x06001DB8 RID: 7608 RVA: 0x00090AC5 File Offset: 0x0008ECC5
	public void PlayPlayerJoinFx()
	{
		this.PlayFX(this.playerJoinSound, 0.5f);
	}

	// Token: 0x06001DB9 RID: 7609 RVA: 0x00090AD8 File Offset: 0x0008ECD8
	public void PlayPlayerLeaveFx()
	{
		this.PlayFX(this.playerLeaveSound, 0.5f);
	}

	// Token: 0x06001DBA RID: 7610 RVA: 0x00090AEB File Offset: 0x0008ECEB
	public void PlayGameStartFx()
	{
		this.PlayFX(this.gameStartSound, this.gameStartVolume);
	}

	// Token: 0x06001DBB RID: 7611 RVA: 0x00090AFF File Offset: 0x0008ECFF
	public void PlayGameEndFx()
	{
		this.PlayFX(this.gameEndSound, this.gameEndVolume);
	}

	// Token: 0x06001DBC RID: 7612 RVA: 0x00090B13 File Offset: 0x0008ED13
	private void PlayFX(AudioClip clip, float volume)
	{
		if (this.audioSource != null)
		{
			this.audioSource.clip = clip;
			this.audioSource.volume = volume;
			this.audioSource.Play();
		}
	}

	// Token: 0x06001DBD RID: 7613 RVA: 0x00090B48 File Offset: 0x0008ED48
	public void RefreshTime()
	{
		float num = (float)(this.game.gameEndTime - PhotonNetwork.Time);
		if (this.game.gameEndTime < 0.0)
		{
			num = 0f;
		}
		num = Mathf.Max(num, 0f);
		this.timeRemainingLabel.text = num.ToString("#00.00");
	}

	// Token: 0x040020D1 RID: 8401
	private MonkeBallGame game;

	// Token: 0x040020D2 RID: 8402
	public MonkeBallScoreboard.TeamDisplay[] teamDisplays;

	// Token: 0x040020D3 RID: 8403
	public TextMeshPro timeRemainingLabel;

	// Token: 0x040020D4 RID: 8404
	public AudioSource audioSource;

	// Token: 0x040020D5 RID: 8405
	public AudioClip scoreSound;

	// Token: 0x040020D6 RID: 8406
	public float scoreSoundVolume;

	// Token: 0x040020D7 RID: 8407
	public AudioClip playerJoinSound;

	// Token: 0x040020D8 RID: 8408
	public AudioClip playerLeaveSound;

	// Token: 0x040020D9 RID: 8409
	public AudioClip gameStartSound;

	// Token: 0x040020DA RID: 8410
	public float gameStartVolume;

	// Token: 0x040020DB RID: 8411
	public AudioClip gameEndSound;

	// Token: 0x040020DC RID: 8412
	public float gameEndVolume;

	// Token: 0x020004C9 RID: 1225
	[Serializable]
	public class TeamDisplay
	{
		// Token: 0x040020DD RID: 8413
		public TextMeshPro nameLabel;

		// Token: 0x040020DE RID: 8414
		public TextMeshPro scoreLabel;

		// Token: 0x040020DF RID: 8415
		public TextMeshPro playersLabel;
	}
}
