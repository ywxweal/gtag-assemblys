using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using GorillaExtensions;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Scripting;

namespace GorillaTagScripts
{
	// Token: 0x02000AC7 RID: 2759
	[NetworkBehaviourWeaved(210)]
	public class WhackAMole : NetworkComponent
	{
		// Token: 0x06004296 RID: 17046 RVA: 0x0013379C File Offset: 0x0013199C
		private void UpdateMeshRendererList()
		{
			List<MeshRenderer> list = new List<MeshRenderer>();
			ZoneBasedObject[] array = this.zoneBasedVisuals;
			for (int i = 0; i < array.Length; i++)
			{
				foreach (MeshRenderer meshRenderer in array[i].GetComponentsInChildren<MeshRenderer>(true))
				{
					if (meshRenderer.enabled)
					{
						list.Add(meshRenderer);
					}
				}
			}
			this.zoneBasedMeshRenderers = list.ToArray();
		}

		// Token: 0x06004297 RID: 17047 RVA: 0x00133804 File Offset: 0x00131A04
		protected override void Awake()
		{
			base.Awake();
			if (this.molesContainerRight != null)
			{
				this.rightMolesList = new List<Mole>(this.molesContainerRight.GetComponentsInChildren<Mole>());
				if (this.rightMolesList.Count > 0)
				{
					this.molesList.AddRange(this.rightMolesList);
				}
			}
			if (this.molesContainerLeft != null)
			{
				this.leftMolesList = new List<Mole>(this.molesContainerLeft.GetComponentsInChildren<Mole>());
				if (this.leftMolesList.Count > 0)
				{
					this.molesList.AddRange(this.leftMolesList);
					foreach (Mole mole in this.leftMolesList)
					{
						mole.IsLeftSideMole = true;
					}
				}
			}
			this.currentLevelIndex = -1;
			foreach (Mole mole2 in this.molesList)
			{
				mole2.OnTapped += this.OnMoleTapped;
			}
			List<Mole> list = this.leftMolesList;
			bool flag;
			if (list != null && list.Count > 0)
			{
				list = this.rightMolesList;
				flag = list != null && list.Count > 0;
			}
			else
			{
				flag = false;
			}
			this.isMultiplayer = flag;
			this.welcomeUI.SetActive(false);
			this.ongoingGameUI.SetActive(false);
			this.levelEndedUI.SetActive(false);
			this.ContinuePressedUI.SetActive(false);
			this.multiplyareScoresUI.SetActive(false);
			this.bestScore = 0;
			this.bestScoreText.text = string.Empty;
			this.highScorePlayerName = string.Empty;
			this.victoryParticles = this.victoryFX.GetComponentsInChildren<ParticleSystem>();
		}

		// Token: 0x06004298 RID: 17048 RVA: 0x001339D4 File Offset: 0x00131BD4
		protected override void Start()
		{
			base.Start();
			this.SwitchState(WhackAMole.GameState.Off);
			if (WhackAMoleManager.instance)
			{
				WhackAMoleManager.instance.Register(this);
			}
		}

		// Token: 0x06004299 RID: 17049 RVA: 0x001339FC File Offset: 0x00131BFC
		private void OnDestroy()
		{
			NetworkBehaviourUtils.InternalOnDestroy(this);
			foreach (Mole mole in this.molesList)
			{
				mole.OnTapped -= this.OnMoleTapped;
			}
			if (WhackAMoleManager.instance)
			{
				WhackAMoleManager.instance.Unregister(this);
			}
			this.molesList.Clear();
		}

		// Token: 0x0600429A RID: 17050 RVA: 0x00133A80 File Offset: 0x00131C80
		public void InvokeUpdate()
		{
			bool isMasterClient = NetworkSystem.Instance.IsMasterClient;
			bool flag = this.zoneBasedVisuals[0].IsLocalPlayerInZone();
			if (isMasterClient != this.wasMasterClient || flag != this.wasLocalPlayerInZone)
			{
				MeshRenderer[] array = this.zoneBasedMeshRenderers;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].enabled = flag;
				}
				bool flag2 = isMasterClient || flag;
				ZoneBasedObject[] array2 = this.zoneBasedVisuals;
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i].gameObject.SetActive(flag2);
				}
				this.wasMasterClient = isMasterClient;
				this.wasLocalPlayerInZone = flag;
			}
			if (!NetworkSystem.Instance.InRoom)
			{
				return;
			}
			foreach (Mole mole in this.molesList)
			{
				mole.InvokeUpdate();
			}
			switch (this.currentState)
			{
			case WhackAMole.GameState.ContinuePressed:
				this.counterText.text = (this.countdownDuration - (int)(Time.time - this.continuePressedTime)).ToString();
				if (this.currentLevel != null)
				{
					this.UpdateLevelUI(this.currentLevel.levelNumber);
				}
				if (base.IsMine && Time.time - this.continuePressedTime > (float)this.countdownDuration)
				{
					this.SwitchState(WhackAMole.GameState.LevelStarted);
				}
				break;
			case WhackAMole.GameState.Ongoing:
				if (!this.audioSource.isPlaying && this.backgroundLoop)
				{
					this.audioSource.GTPlayOneShot(this.backgroundLoop, 1f);
				}
				if (base.IsMine)
				{
					this.UpdateTimerUI((int)this.timer.GetRemainingTime());
					if (Time.time - this.curentTime >= this.currentLevel.pickNextMoleTime)
					{
						this.SwitchState(WhackAMole.GameState.PickMoles);
					}
				}
				break;
			case WhackAMole.GameState.PickMoles:
				if (base.IsMine && this.PickMoles())
				{
					this.SwitchState(WhackAMole.GameState.Ongoing);
				}
				break;
			case WhackAMole.GameState.TimesUp:
				switch (this.curentGameResult)
				{
				case WhackAMole.GameResult.GameOver:
					if (base.IsMine && Time.time - this.gameEndedTime > 10f)
					{
						this.SwitchState(WhackAMole.GameState.Off);
					}
					break;
				case WhackAMole.GameResult.Win:
					if (base.IsMine && Time.time - this.gameEndedTime > 10f)
					{
						this.SwitchState(WhackAMole.GameState.Off);
					}
					break;
				case WhackAMole.GameResult.LevelComplete:
					if (Time.time - this.gameEndedTime > (float)this.betweenLevelPauseDuration && base.IsMine)
					{
						this.SwitchState(WhackAMole.GameState.ContinuePressed);
					}
					break;
				}
				break;
			case WhackAMole.GameState.LevelStarted:
				if (base.IsMine)
				{
					this.SwitchState(WhackAMole.GameState.Ongoing);
				}
				break;
			}
			if (this.arrowRotationNeedsUpdate)
			{
				this.UpdateArrowRotation();
			}
		}

		// Token: 0x0600429B RID: 17051 RVA: 0x00133D5C File Offset: 0x00131F5C
		private void SwitchState(WhackAMole.GameState state)
		{
			this.lastState = this.currentState;
			this.currentState = state;
			switch (this.currentState)
			{
			case WhackAMole.GameState.Off:
				this.ResetGame();
				this.currentLevelIndex = -1;
				this.currentLevel = null;
				this.UpdateLevelUI(1);
				break;
			case WhackAMole.GameState.ContinuePressed:
				this.continuePressedTime = Time.time;
				this.audioSource.GTStop();
				this.audioSource.GTPlayOneShot(this.counterClip, 1f);
				if (base.IsMine)
				{
					this.pickedMolesIndex.Clear();
				}
				this.ResetGame();
				if (base.IsMine)
				{
					this.LoadNextLevel();
				}
				break;
			case WhackAMole.GameState.Ongoing:
				this.UpdateScoreUI(this.currentScore, this.leftPlayerScore, this.rightPlayerScore);
				break;
			case WhackAMole.GameState.TimesUp:
				if (this.currentLevel != null)
				{
					foreach (Mole mole in this.molesList)
					{
						mole.HideMole(false);
					}
					this.curentGameResult = this.GetGameResult();
					this.UpdateResultUI(this.curentGameResult);
					this.levelEndedTotalScoreText.text = "SCORE " + this.totalScore.ToString();
					this.levelEndedCurrentScoreText.text = string.Format("{0}/{1}", this.currentScore, this.currentLevel.GetMinScore(this.isMultiplayer));
					if (this.totalScore > this.bestScore)
					{
						this.bestScore = this.totalScore;
						this.highScorePlayerName = this.playerName;
					}
					this.bestScoreText.text = (this.isMultiplayer ? this.bestScore.ToString() : (this.highScorePlayerName + "  " + this.bestScore.ToString()));
					this.audioSource.GTStop();
					if (this.curentGameResult == WhackAMole.GameResult.LevelComplete)
					{
						this.audioSource.GTPlayOneShot(this.levelCompleteClip, 1f);
						if (NetworkSystem.Instance.LocalPlayer.UserId == this.playerId)
						{
							PlayerGameEvents.MiscEvent("WhackComplete" + this.currentLevel.levelNumber.ToString());
						}
					}
					else if (this.curentGameResult == WhackAMole.GameResult.GameOver)
					{
						this.audioSource.GTPlayOneShot(this.gameOverClip, 1f);
					}
					else if (this.curentGameResult == WhackAMole.GameResult.Win)
					{
						this.audioSource.GTPlayOneShot(this.winClip, 1f);
						if (this.victoryFX)
						{
							ParticleSystem[] array = this.victoryParticles;
							for (int i = 0; i < array.Length; i++)
							{
								array[i].Play();
							}
						}
						if (NetworkSystem.Instance.LocalPlayer.UserId == this.playerId)
						{
							PlayerGameEvents.MiscEvent("WhackComplete" + this.currentLevel.levelNumber.ToString());
						}
					}
					int minScore = this.currentLevel.GetMinScore(this.isMultiplayer);
					if (this.levelGoodMolesPicked < minScore)
					{
						GTDev.LogError<string>(string.Format("[WAM] Lvl:{0} Only Picked {1}/{2} good moles!", this.currentLevel.levelNumber, this.levelGoodMolesPicked, minScore), null);
					}
					if (base.IsMine)
					{
						GorillaTelemetry.WamLevelEnd(this.playerId, this.gameId, this.machineId, this.currentLevel.levelNumber, this.levelGoodMolesPicked, this.levelHazardMolesPicked, minScore, this.currentScore, this.levelHazardMolesHit, this.curentGameResult.ToString());
					}
				}
				break;
			}
			this.UpdateScreenData();
		}

		// Token: 0x0600429C RID: 17052 RVA: 0x00134124 File Offset: 0x00132324
		private void UpdateScreenData()
		{
			switch (this.currentState)
			{
			case WhackAMole.GameState.Off:
				this.welcomeUI.SetActive(true);
				this.ContinuePressedUI.SetActive(false);
				this.ongoingGameUI.SetActive(false);
				this.levelEndedUI.SetActive(false);
				this.multiplyareScoresUI.SetActive(false);
				return;
			case WhackAMole.GameState.ContinuePressed:
				this.levelEndedUI.SetActive(false);
				this.welcomeUI.SetActive(false);
				this.ongoingGameUI.SetActive(false);
				this.multiplyareScoresUI.SetActive(false);
				this.ContinuePressedUI.SetActive(true);
				break;
			case WhackAMole.GameState.Ongoing:
				this.ContinuePressedUI.SetActive(false);
				this.welcomeUI.SetActive(false);
				this.ongoingGameUI.SetActive(true);
				this.levelEndedUI.SetActive(false);
				if (this.isMultiplayer)
				{
					this.multiplyareScoresUI.SetActive(true);
					return;
				}
				break;
			case WhackAMole.GameState.PickMoles:
				break;
			case WhackAMole.GameState.TimesUp:
				this.welcomeUI.SetActive(false);
				this.ongoingGameUI.SetActive(false);
				this.ContinuePressedUI.SetActive(false);
				if (this.isMultiplayer)
				{
					this.multiplyareScoresUI.SetActive(true);
				}
				this.levelEndedUI.SetActive(true);
				return;
			default:
				return;
			}
		}

		// Token: 0x0600429D RID: 17053 RVA: 0x0013425C File Offset: 0x0013245C
		public static int CreateNewGameID()
		{
			int num = (int)((DateTime.Now - WhackAMole.epoch).TotalSeconds * 8.0 % 2147483646.0) + 1;
			if (num <= WhackAMole.lastAssignedID)
			{
				WhackAMole.lastAssignedID++;
				return WhackAMole.lastAssignedID;
			}
			WhackAMole.lastAssignedID = num;
			return num;
		}

		// Token: 0x0600429E RID: 17054 RVA: 0x001342BC File Offset: 0x001324BC
		private void OnMoleTapped(MoleTypes moleType, Vector3 position, bool isLocalTap, bool isLeftHand)
		{
			WhackAMole.GameState gameState = this.currentState;
			if (gameState == WhackAMole.GameState.Off || gameState == WhackAMole.GameState.TimesUp)
			{
				return;
			}
			AudioClip audioClip = (moleType.isHazard ? this.whackHazardClips[Random.Range(0, this.whackHazardClips.Length)] : this.whackMonkeClips[Random.Range(0, this.whackMonkeClips.Length)]);
			if (moleType.isHazard)
			{
				this.audioSource.GTPlayOneShot(audioClip, 1f);
				this.levelHazardMolesHit++;
			}
			else
			{
				this.audioSource.GTPlayOneShot(audioClip, 1f);
			}
			if (moleType.monkeMoleHitMaterial != null)
			{
				moleType.MeshRenderer.material = moleType.monkeMoleHitMaterial;
			}
			this.currentScore += moleType.scorePoint;
			this.totalScore += moleType.scorePoint;
			if (moleType.IsLeftSideMoleType)
			{
				this.leftPlayerScore += moleType.scorePoint;
			}
			else
			{
				this.rightPlayerScore += moleType.scorePoint;
			}
			this.UpdateScoreUI(this.currentScore, this.leftPlayerScore, this.rightPlayerScore);
			moleType.MoleContainerParent.HideMole(true);
		}

		// Token: 0x0600429F RID: 17055 RVA: 0x001343E0 File Offset: 0x001325E0
		public void HandleOnTimerStopped()
		{
			this.gameEndedTime = Time.time;
			this.SwitchState(WhackAMole.GameState.TimesUp);
		}

		// Token: 0x060042A0 RID: 17056 RVA: 0x001343F4 File Offset: 0x001325F4
		private IEnumerator PlayHazardAudio(AudioClip clip)
		{
			this.audioSource.clip = clip;
			this.audioSource.GTPlay();
			yield return new WaitForSeconds(this.audioSource.clip.length);
			this.audioSource.clip = this.errorClip;
			this.audioSource.GTPlay();
			yield break;
		}

		// Token: 0x060042A1 RID: 17057 RVA: 0x0013440C File Offset: 0x0013260C
		private bool PickMoles()
		{
			WhackAMole.<>c__DisplayClass85_0 CS$<>8__locals1;
			CS$<>8__locals1.<>4__this = this;
			this.pickedMolesIndex.Clear();
			float passedTime = this.timer.GetPassedTime();
			if (passedTime > this.currentLevel.levelDuration - this.currentLevel.showMoleDuration)
			{
				return true;
			}
			float num = passedTime / this.currentLevel.levelDuration;
			CS$<>8__locals1.minMoleCount = Mathf.Lerp(this.currentLevel.minimumMoleCount.x, this.currentLevel.minimumMoleCount.y, num);
			CS$<>8__locals1.maxMoleCount = Mathf.Lerp(this.currentLevel.maximumMoleCount.x, this.currentLevel.maximumMoleCount.y, num);
			this.curentTime = Time.time;
			CS$<>8__locals1.hazardMoleChance = Mathf.Lerp(this.currentLevel.hazardMoleChance.x, this.currentLevel.hazardMoleChance.y, num);
			if (this.isMultiplayer)
			{
				this.<PickMoles>g__PickMolesFrom|85_0(this.rightMolesList, ref CS$<>8__locals1);
				this.<PickMoles>g__PickMolesFrom|85_0(this.leftMolesList, ref CS$<>8__locals1);
			}
			else
			{
				this.<PickMoles>g__PickMolesFrom|85_0(this.molesList, ref CS$<>8__locals1);
			}
			return this.pickedMolesIndex.Count != 0;
		}

		// Token: 0x060042A2 RID: 17058 RVA: 0x00134538 File Offset: 0x00132738
		private void LoadNextLevel()
		{
			if (this.currentLevel != null)
			{
				this.resetToFirstLevel = this.currentScore < this.currentLevel.GetMinScore(this.isMultiplayer);
				if (this.resetToFirstLevel)
				{
					this.currentLevelIndex = 0;
				}
				else
				{
					this.currentLevelIndex++;
				}
				if (this.currentLevelIndex >= this.allLevels.Length)
				{
					this.currentLevelIndex = 0;
				}
			}
			else
			{
				this.currentLevelIndex++;
			}
			this.currentLevel = this.allLevels[this.currentLevelIndex];
			this.timer.SetTimerDuration(this.currentLevel.levelDuration);
			this.timer.RestartTimer();
			this.curentTime = Time.time;
			this.currentScore = 0;
			this.leftPlayerScore = 0;
			this.rightPlayerScore = 0;
			this.levelGoodMolesPicked = (this.levelHazardMolesPicked = 0);
			this.levelHazardMolesHit = 0;
			if (this.currentLevelIndex == 0)
			{
				this.totalScore = 0;
			}
			if (this.currentLevelIndex == 0 && base.IsMine)
			{
				this.gameId = WhackAMole.CreateNewGameID();
				Debug.LogWarning("GAME ID" + this.gameId.ToString());
			}
		}

		// Token: 0x060042A3 RID: 17059 RVA: 0x00134668 File Offset: 0x00132868
		private bool PickSingleMole(int randomMoleIndex, float hazardMoleChance)
		{
			bool flag = hazardMoleChance > 0f && Random.value <= hazardMoleChance;
			int moleTypeIndex = this.molesList[randomMoleIndex].GetMoleTypeIndex(flag);
			this.molesList[randomMoleIndex].ShowMole(this.currentLevel.showMoleDuration, moleTypeIndex);
			this.pickedMolesIndex.Add(randomMoleIndex, moleTypeIndex);
			if (flag)
			{
				this.levelHazardMolesPicked++;
			}
			else
			{
				this.levelGoodMolesPicked++;
			}
			return flag;
		}

		// Token: 0x060042A4 RID: 17060 RVA: 0x001346EC File Offset: 0x001328EC
		private void ResetGame()
		{
			foreach (Mole mole in this.molesList)
			{
				mole.ResetPosition();
			}
		}

		// Token: 0x060042A5 RID: 17061 RVA: 0x0013473C File Offset: 0x0013293C
		private void UpdateScoreUI(int totalScore, int _leftPlayerScore, int _rightPlayerScore)
		{
			if (this.currentLevel != null)
			{
				this.scoreText.text = string.Format("SCORE\n{0}/{1}", totalScore, this.currentLevel.GetMinScore(this.isMultiplayer));
				this.leftPlayerScoreText.text = _leftPlayerScore.ToString();
				this.rightPlayerScoreText.text = _rightPlayerScore.ToString();
			}
		}

		// Token: 0x060042A6 RID: 17062 RVA: 0x001347AC File Offset: 0x001329AC
		private void UpdateLevelUI(int levelNumber)
		{
			this.arrowTargetRotation = Quaternion.Euler(0f, 0f, (float)(18 * (levelNumber - 1)));
			this.arrowRotationNeedsUpdate = true;
		}

		// Token: 0x060042A7 RID: 17063 RVA: 0x001347D4 File Offset: 0x001329D4
		private void UpdateArrowRotation()
		{
			Quaternion quaternion = Quaternion.Slerp(this.levelArrow.transform.localRotation, this.arrowTargetRotation, Time.deltaTime * 5f);
			if (Quaternion.Angle(quaternion, this.arrowTargetRotation) < 0.1f)
			{
				quaternion = this.arrowTargetRotation;
				this.arrowRotationNeedsUpdate = false;
			}
			this.levelArrow.transform.localRotation = quaternion;
		}

		// Token: 0x060042A8 RID: 17064 RVA: 0x0013483A File Offset: 0x00132A3A
		private void UpdateTimerUI(int time)
		{
			if (time == this.previousTime)
			{
				return;
			}
			this.timeText.text = "TIME " + time.ToString();
			this.previousTime = time;
		}

		// Token: 0x060042A9 RID: 17065 RVA: 0x00134869 File Offset: 0x00132A69
		private void UpdateResultUI(WhackAMole.GameResult gameResult)
		{
			if (gameResult == WhackAMole.GameResult.LevelComplete)
			{
				this.resultText.text = "LEVEL COMPLETE";
				return;
			}
			if (gameResult == WhackAMole.GameResult.Win)
			{
				this.resultText.text = "YOU WIN!";
				return;
			}
			if (gameResult == WhackAMole.GameResult.GameOver)
			{
				this.resultText.text = "GAME OVER";
			}
		}

		// Token: 0x060042AA RID: 17066 RVA: 0x001348A8 File Offset: 0x00132AA8
		public void OnStartButtonPressed()
		{
			WhackAMole.GameState gameState = this.currentState;
			if (gameState == WhackAMole.GameState.TimesUp || gameState == WhackAMole.GameState.Off)
			{
				base.GetView.RPC("WhackAMoleButtonPressed", RpcTarget.All, Array.Empty<object>());
			}
		}

		// Token: 0x060042AB RID: 17067 RVA: 0x001348D9 File Offset: 0x00132AD9
		[PunRPC]
		private void WhackAMoleButtonPressed(PhotonMessageInfo info)
		{
			this.WhackAMoleButtonPressedShared(info);
		}

		// Token: 0x060042AC RID: 17068 RVA: 0x001348E8 File Offset: 0x00132AE8
		[Rpc]
		private unsafe void RPC_WhackAMoleButtonPressed(RpcInfo info = default(RpcInfo))
		{
			if (!this.InvokeRpc)
			{
				NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
				if (base.Runner.Stage != SimulationStages.Resimulate)
				{
					int localAuthorityMask = base.Object.GetLocalAuthorityMask();
					if ((localAuthorityMask & 7) == 0)
					{
						NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GorillaTagScripts.WhackAMole::RPC_WhackAMoleButtonPressed(Fusion.RpcInfo)", base.Object, 7);
					}
					else
					{
						if (base.Runner.HasAnyActiveConnections())
						{
							int num = 8;
							SimulationMessage* ptr = SimulationMessage.Allocate(base.Runner.Simulation, num);
							byte* data = SimulationMessage.GetData(ptr);
							int num2 = RpcHeader.Write(RpcHeader.Create(base.Object.Id, this.ObjectIndex, 1), data);
							ptr->Offset = num2 * 8;
							base.Runner.SendRpc(ptr);
						}
						if ((localAuthorityMask & 7) != 0)
						{
							info = RpcInfo.FromLocal(base.Runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
							goto IL_0012;
						}
					}
				}
				return;
			}
			this.InvokeRpc = false;
			IL_0012:
			this.WhackAMoleButtonPressedShared(info);
		}

		// Token: 0x060042AD RID: 17069 RVA: 0x00134A08 File Offset: 0x00132C08
		private void WhackAMoleButtonPressedShared(PhotonMessageInfoWrapped info)
		{
			GorillaNot.IncrementRPCCall(info, "WhackAMoleButtonPressedShared");
			VRRig vrrig = GorillaGameManager.StaticFindRigForPlayer(info.Sender);
			if (vrrig)
			{
				this.playerName = vrrig.playerNameVisible;
				if (this.currentState == WhackAMole.GameState.Off)
				{
					this.playerId = info.Sender.UserId;
					if (NetworkSystem.Instance.LocalPlayer.UserId == this.playerId)
					{
						PlayerGameEvents.MiscEvent("PlayArcadeGame");
					}
				}
			}
			this.SwitchState(WhackAMole.GameState.ContinuePressed);
		}

		// Token: 0x060042AE RID: 17070 RVA: 0x00134A88 File Offset: 0x00132C88
		private WhackAMole.GameResult GetGameResult()
		{
			if (this.currentScore < this.currentLevel.GetMinScore(this.isMultiplayer))
			{
				return WhackAMole.GameResult.GameOver;
			}
			if (this.currentLevelIndex >= this.allLevels.Length - 1)
			{
				return WhackAMole.GameResult.Win;
			}
			return WhackAMole.GameResult.LevelComplete;
		}

		// Token: 0x060042AF RID: 17071 RVA: 0x00134ABA File Offset: 0x00132CBA
		public int GetCurrentLevel()
		{
			if (this.currentLevel != null)
			{
				return this.currentLevel.levelNumber;
			}
			return 0;
		}

		// Token: 0x060042B0 RID: 17072 RVA: 0x00134AD7 File Offset: 0x00132CD7
		public int GetTotalLevelNumbers()
		{
			if (this.allLevels != null)
			{
				return this.allLevels.Length;
			}
			return 0;
		}

		// Token: 0x170006A3 RID: 1699
		// (get) Token: 0x060042B1 RID: 17073 RVA: 0x00134AEB File Offset: 0x00132CEB
		// (set) Token: 0x060042B2 RID: 17074 RVA: 0x00134B15 File Offset: 0x00132D15
		[Networked]
		[NetworkedWeaved(0, 210)]
		public unsafe WhackAMole.WhackAMoleData Data
		{
			get
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing WhackAMole.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				return *(WhackAMole.WhackAMoleData*)(this.Ptr + 0);
			}
			set
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing WhackAMole.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				*(WhackAMole.WhackAMoleData*)(this.Ptr + 0) = value;
			}
		}

		// Token: 0x060042B3 RID: 17075 RVA: 0x00134B40 File Offset: 0x00132D40
		public override void WriteDataFusion()
		{
			this.Data = new WhackAMole.WhackAMoleData(this.currentState, this.currentLevelIndex, this.currentScore, this.totalScore, this.bestScore, this.rightPlayerScore, this.highScorePlayerName, this.timer.GetRemainingTime(), this.gameEndedTime, this.gameId, this.pickedMolesIndex);
			this.pickedMolesIndex.Clear();
		}

		// Token: 0x060042B4 RID: 17076 RVA: 0x00134BAC File Offset: 0x00132DAC
		public override void ReadDataFusion()
		{
			this.ReadDataShared(this.Data.CurrentState, this.Data.CurrentLevelIndex, this.Data.CurrentScore, this.Data.TotalScore, this.Data.BestScore, this.Data.RightPlayerScore, this.Data.HighScorePlayerName.Value, this.Data.RemainingTime, this.Data.GameEndedTime, this.Data.GameId);
			for (int i = 0; i < this.Data.PickedMolesIndexCount; i++)
			{
				int num = this.Data.PickedMolesIndex[i];
				if (i >= 0 && i < this.molesList.Count && this.currentLevel)
				{
					this.molesList[i].ShowMole(this.currentLevel.showMoleDuration, num);
				}
			}
		}

		// Token: 0x060042B5 RID: 17077 RVA: 0x00134CC4 File Offset: 0x00132EC4
		protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			if (info.Sender != PhotonNetwork.MasterClient)
			{
				return;
			}
			stream.SendNext(this.currentState);
			stream.SendNext(this.currentLevelIndex);
			stream.SendNext(this.currentScore);
			stream.SendNext(this.totalScore);
			stream.SendNext(this.bestScore);
			stream.SendNext(this.rightPlayerScore);
			stream.SendNext(this.highScorePlayerName);
			stream.SendNext(this.timer.GetRemainingTime());
			stream.SendNext(this.gameEndedTime);
			stream.SendNext(this.gameId);
			stream.SendNext(this.pickedMolesIndex.Count);
			foreach (KeyValuePair<int, int> keyValuePair in this.pickedMolesIndex)
			{
				stream.SendNext(keyValuePair.Key);
				stream.SendNext(keyValuePair.Value);
			}
			this.pickedMolesIndex.Clear();
		}

		// Token: 0x060042B6 RID: 17078 RVA: 0x00134E10 File Offset: 0x00133010
		protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			if (info.Sender != PhotonNetwork.MasterClient)
			{
				return;
			}
			WhackAMole.GameState gameState = (WhackAMole.GameState)stream.ReceiveNext();
			int num = (int)stream.ReceiveNext();
			int num2 = (int)stream.ReceiveNext();
			int num3 = (int)stream.ReceiveNext();
			int num4 = (int)stream.ReceiveNext();
			int num5 = (int)stream.ReceiveNext();
			string text = (string)stream.ReceiveNext();
			float num6 = (float)stream.ReceiveNext();
			float num7 = (float)stream.ReceiveNext();
			int num8 = (int)stream.ReceiveNext();
			int num9 = (int)stream.ReceiveNext();
			this.ReadDataShared(gameState, num, num2, num3, num4, num5, text, num6, num7, num8);
			for (int i = 0; i < num9; i++)
			{
				int num10 = (int)stream.ReceiveNext();
				int num11 = (int)stream.ReceiveNext();
				if (num10 >= 0 && num10 < this.molesList.Count && this.currentLevel)
				{
					this.molesList[num10].ShowMole(this.currentLevel.showMoleDuration, num11);
				}
			}
		}

		// Token: 0x060042B7 RID: 17079 RVA: 0x00134F38 File Offset: 0x00133138
		private void ReadDataShared(WhackAMole.GameState _currentState, int _currentLevelIndex, int cScore, int tScore, int bScore, int rPScore, string hScorePName, float _remainingTime, float endedTime, int _gameId)
		{
			WhackAMole.GameState gameState = this.currentState;
			if (_currentState != gameState)
			{
				this.SwitchState(_currentState);
			}
			this.currentLevelIndex = _currentLevelIndex;
			if (this.currentLevelIndex >= 0 && this.currentLevelIndex < this.allLevels.Length)
			{
				this.currentLevel = this.allLevels[this.currentLevelIndex];
				this.UpdateLevelUI(this.currentLevel.levelNumber);
			}
			this.currentScore = cScore;
			this.totalScore = tScore;
			this.bestScore = bScore;
			this.rightPlayerScore = rPScore;
			this.leftPlayerScore = this.currentScore - this.rightPlayerScore;
			this.highScorePlayerName = hScorePName;
			this.bestScoreText.text = (this.isMultiplayer ? this.bestScore.ToString() : (this.highScorePlayerName + "  " + this.bestScore.ToString()));
			this.remainingTime = _remainingTime;
			if (float.IsFinite(this.remainingTime) && this.currentLevel)
			{
				this.remainingTime = this.remainingTime.ClampSafe(0f, this.currentLevel.levelDuration);
				this.UpdateTimerUI((int)this.remainingTime);
			}
			if (float.IsFinite(endedTime))
			{
				this.gameEndedTime = endedTime.ClampSafe(0f, Time.time);
			}
			this.gameId = _gameId;
		}

		// Token: 0x060042B8 RID: 17080 RVA: 0x0013508C File Offset: 0x0013328C
		protected override void OnOwnerSwitched(NetPlayer newOwningPlayer)
		{
			base.OnOwnerSwitched(newOwningPlayer);
			if (NetworkSystem.Instance.IsMasterClient)
			{
				this.timer.RestartTimer();
				this.timer.SetTimerDuration(this.remainingTime);
				this.curentTime = Time.time;
				if (this.currentLevelIndex >= 0 && this.currentLevelIndex < this.allLevels.Length)
				{
					this.currentLevel = this.allLevels[this.currentLevelIndex];
				}
				this.SwitchState(this.currentState);
			}
		}

		// Token: 0x060042BB RID: 17083 RVA: 0x00135190 File Offset: 0x00133390
		[CompilerGenerated]
		private void <PickMoles>g__PickMolesFrom|85_0(List<Mole> moles, ref WhackAMole.<>c__DisplayClass85_0 A_2)
		{
			int num = Mathf.RoundToInt(Random.Range(A_2.minMoleCount, A_2.maxMoleCount));
			this.potentialMoles.Clear();
			foreach (Mole mole in moles)
			{
				if (mole.CanPickMole())
				{
					this.potentialMoles.Add(mole);
				}
			}
			int num2 = Mathf.Min(num, this.potentialMoles.Count);
			int num3 = Mathf.CeilToInt((float)num2 * A_2.hazardMoleChance);
			int num4 = 0;
			for (int i = 0; i < num2; i++)
			{
				int num5 = Random.Range(0, this.potentialMoles.Count);
				if (this.PickSingleMole(this.molesList.IndexOf(this.potentialMoles[num5]), (num4 < num3) ? A_2.hazardMoleChance : 0f))
				{
					num4++;
				}
				this.potentialMoles.RemoveAt(num5);
			}
		}

		// Token: 0x060042BC RID: 17084 RVA: 0x0013529C File Offset: 0x0013349C
		[WeaverGenerated]
		public override void CopyBackingFieldsToState(bool A_1)
		{
			base.CopyBackingFieldsToState(A_1);
			this.Data = this._Data;
		}

		// Token: 0x060042BD RID: 17085 RVA: 0x001352B4 File Offset: 0x001334B4
		[WeaverGenerated]
		public override void CopyStateToBackingFields()
		{
			base.CopyStateToBackingFields();
			this._Data = this.Data;
		}

		// Token: 0x060042BE RID: 17086 RVA: 0x001352C8 File Offset: 0x001334C8
		[NetworkRpcWeavedInvoker(1, 7, 7)]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RPC_WhackAMoleButtonPressed@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
		{
			byte* data = SimulationMessage.GetData(message);
			int num = (RpcHeader.ReadSize(data) + 3) & -4;
			RpcInfo rpcInfo = RpcInfo.FromMessage(behaviour.Runner, message, RpcHostMode.SourceIsServer);
			behaviour.InvokeRpc = true;
			((WhackAMole)behaviour).RPC_WhackAMoleButtonPressed(rpcInfo);
		}

		// Token: 0x040044FF RID: 17663
		public string machineId = "default";

		// Token: 0x04004500 RID: 17664
		public GameObject molesContainerRight;

		// Token: 0x04004501 RID: 17665
		[Tooltip("Only for co-op version")]
		public GameObject molesContainerLeft;

		// Token: 0x04004502 RID: 17666
		public int betweenLevelPauseDuration = 3;

		// Token: 0x04004503 RID: 17667
		public int countdownDuration = 5;

		// Token: 0x04004504 RID: 17668
		public WhackAMoleLevelSO[] allLevels;

		// Token: 0x04004505 RID: 17669
		[SerializeField]
		private GorillaTimer timer;

		// Token: 0x04004506 RID: 17670
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x04004507 RID: 17671
		public GameObject levelArrow;

		// Token: 0x04004508 RID: 17672
		public GameObject victoryFX;

		// Token: 0x04004509 RID: 17673
		public ZoneBasedObject[] zoneBasedVisuals;

		// Token: 0x0400450A RID: 17674
		[SerializeField]
		private MeshRenderer[] zoneBasedMeshRenderers;

		// Token: 0x0400450B RID: 17675
		[Space]
		public AudioClip backgroundLoop;

		// Token: 0x0400450C RID: 17676
		public AudioClip errorClip;

		// Token: 0x0400450D RID: 17677
		public AudioClip counterClip;

		// Token: 0x0400450E RID: 17678
		public AudioClip levelCompleteClip;

		// Token: 0x0400450F RID: 17679
		public AudioClip winClip;

		// Token: 0x04004510 RID: 17680
		public AudioClip gameOverClip;

		// Token: 0x04004511 RID: 17681
		public AudioClip[] whackHazardClips;

		// Token: 0x04004512 RID: 17682
		public AudioClip[] whackMonkeClips;

		// Token: 0x04004513 RID: 17683
		[Space]
		public GameObject welcomeUI;

		// Token: 0x04004514 RID: 17684
		public GameObject ongoingGameUI;

		// Token: 0x04004515 RID: 17685
		public GameObject levelEndedUI;

		// Token: 0x04004516 RID: 17686
		public GameObject ContinuePressedUI;

		// Token: 0x04004517 RID: 17687
		public GameObject multiplyareScoresUI;

		// Token: 0x04004518 RID: 17688
		[Space]
		public TextMeshPro scoreText;

		// Token: 0x04004519 RID: 17689
		public TextMeshPro bestScoreText;

		// Token: 0x0400451A RID: 17690
		[Tooltip("Only for co-op version")]
		public TextMeshPro rightPlayerScoreText;

		// Token: 0x0400451B RID: 17691
		[Tooltip("Only for co-op version")]
		public TextMeshPro leftPlayerScoreText;

		// Token: 0x0400451C RID: 17692
		public TextMeshPro timeText;

		// Token: 0x0400451D RID: 17693
		public TextMeshPro counterText;

		// Token: 0x0400451E RID: 17694
		public TextMeshPro resultText;

		// Token: 0x0400451F RID: 17695
		public TextMeshPro levelEndedOptionsText;

		// Token: 0x04004520 RID: 17696
		public TextMeshPro levelEndedCountdownText;

		// Token: 0x04004521 RID: 17697
		public TextMeshPro levelEndedTotalScoreText;

		// Token: 0x04004522 RID: 17698
		public TextMeshPro levelEndedCurrentScoreText;

		// Token: 0x04004523 RID: 17699
		private List<Mole> rightMolesList;

		// Token: 0x04004524 RID: 17700
		private List<Mole> leftMolesList;

		// Token: 0x04004525 RID: 17701
		private List<Mole> molesList = new List<Mole>();

		// Token: 0x04004526 RID: 17702
		private WhackAMoleLevelSO currentLevel;

		// Token: 0x04004527 RID: 17703
		private int currentScore;

		// Token: 0x04004528 RID: 17704
		private int totalScore;

		// Token: 0x04004529 RID: 17705
		private int leftPlayerScore;

		// Token: 0x0400452A RID: 17706
		private int rightPlayerScore;

		// Token: 0x0400452B RID: 17707
		private int bestScore;

		// Token: 0x0400452C RID: 17708
		private float curentTime;

		// Token: 0x0400452D RID: 17709
		private int currentLevelIndex;

		// Token: 0x0400452E RID: 17710
		private float continuePressedTime;

		// Token: 0x0400452F RID: 17711
		private bool resetToFirstLevel;

		// Token: 0x04004530 RID: 17712
		private Quaternion arrowTargetRotation;

		// Token: 0x04004531 RID: 17713
		private bool arrowRotationNeedsUpdate;

		// Token: 0x04004532 RID: 17714
		private List<Mole> potentialMoles = new List<Mole>();

		// Token: 0x04004533 RID: 17715
		private Dictionary<int, int> pickedMolesIndex = new Dictionary<int, int>();

		// Token: 0x04004534 RID: 17716
		private WhackAMole.GameState currentState;

		// Token: 0x04004535 RID: 17717
		private WhackAMole.GameState lastState;

		// Token: 0x04004536 RID: 17718
		private float remainingTime;

		// Token: 0x04004537 RID: 17719
		private int previousTime = -1;

		// Token: 0x04004538 RID: 17720
		private bool isMultiplayer;

		// Token: 0x04004539 RID: 17721
		private float gameEndedTime;

		// Token: 0x0400453A RID: 17722
		private WhackAMole.GameResult curentGameResult;

		// Token: 0x0400453B RID: 17723
		private string playerName = string.Empty;

		// Token: 0x0400453C RID: 17724
		private string highScorePlayerName = string.Empty;

		// Token: 0x0400453D RID: 17725
		private ParticleSystem[] victoryParticles;

		// Token: 0x0400453E RID: 17726
		private int levelHazardMolesPicked;

		// Token: 0x0400453F RID: 17727
		private int levelGoodMolesPicked;

		// Token: 0x04004540 RID: 17728
		private string playerId;

		// Token: 0x04004541 RID: 17729
		private int gameId;

		// Token: 0x04004542 RID: 17730
		private int levelHazardMolesHit;

		// Token: 0x04004543 RID: 17731
		private static DateTime epoch = new DateTime(2024, 1, 1);

		// Token: 0x04004544 RID: 17732
		private static int lastAssignedID;

		// Token: 0x04004545 RID: 17733
		private bool wasMasterClient;

		// Token: 0x04004546 RID: 17734
		private bool wasLocalPlayerInZone = true;

		// Token: 0x04004547 RID: 17735
		[WeaverGenerated]
		[SerializeField]
		[DefaultForProperty("Data", 0, 210)]
		[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
		private WhackAMole.WhackAMoleData _Data;

		// Token: 0x02000AC8 RID: 2760
		public enum GameState
		{
			// Token: 0x04004549 RID: 17737
			Off,
			// Token: 0x0400454A RID: 17738
			ContinuePressed,
			// Token: 0x0400454B RID: 17739
			Ongoing,
			// Token: 0x0400454C RID: 17740
			PickMoles,
			// Token: 0x0400454D RID: 17741
			TimesUp,
			// Token: 0x0400454E RID: 17742
			LevelStarted
		}

		// Token: 0x02000AC9 RID: 2761
		private enum GameResult
		{
			// Token: 0x04004550 RID: 17744
			GameOver,
			// Token: 0x04004551 RID: 17745
			Win,
			// Token: 0x04004552 RID: 17746
			LevelComplete,
			// Token: 0x04004553 RID: 17747
			Unknown
		}

		// Token: 0x02000ACA RID: 2762
		[NetworkStructWeaved(210)]
		[StructLayout(LayoutKind.Explicit, Size = 840)]
		public struct WhackAMoleData : INetworkStruct
		{
			// Token: 0x170006A4 RID: 1700
			// (get) Token: 0x060042BF RID: 17087 RVA: 0x0013531B File Offset: 0x0013351B
			// (set) Token: 0x060042C0 RID: 17088 RVA: 0x00135323 File Offset: 0x00133523
			public WhackAMole.GameState CurrentState { readonly get; set; }

			// Token: 0x170006A5 RID: 1701
			// (get) Token: 0x060042C1 RID: 17089 RVA: 0x0013532C File Offset: 0x0013352C
			// (set) Token: 0x060042C2 RID: 17090 RVA: 0x00135334 File Offset: 0x00133534
			public int CurrentLevelIndex { readonly get; set; }

			// Token: 0x170006A6 RID: 1702
			// (get) Token: 0x060042C3 RID: 17091 RVA: 0x0013533D File Offset: 0x0013353D
			// (set) Token: 0x060042C4 RID: 17092 RVA: 0x00135345 File Offset: 0x00133545
			public int CurrentScore { readonly get; set; }

			// Token: 0x170006A7 RID: 1703
			// (get) Token: 0x060042C5 RID: 17093 RVA: 0x0013534E File Offset: 0x0013354E
			// (set) Token: 0x060042C6 RID: 17094 RVA: 0x00135356 File Offset: 0x00133556
			public int TotalScore { readonly get; set; }

			// Token: 0x170006A8 RID: 1704
			// (get) Token: 0x060042C7 RID: 17095 RVA: 0x0013535F File Offset: 0x0013355F
			// (set) Token: 0x060042C8 RID: 17096 RVA: 0x00135367 File Offset: 0x00133567
			public int BestScore { readonly get; set; }

			// Token: 0x170006A9 RID: 1705
			// (get) Token: 0x060042C9 RID: 17097 RVA: 0x00135370 File Offset: 0x00133570
			// (set) Token: 0x060042CA RID: 17098 RVA: 0x00135378 File Offset: 0x00133578
			public int RightPlayerScore { readonly get; set; }

			// Token: 0x170006AA RID: 1706
			// (get) Token: 0x060042CB RID: 17099 RVA: 0x00135381 File Offset: 0x00133581
			// (set) Token: 0x060042CC RID: 17100 RVA: 0x00135393 File Offset: 0x00133593
			[Networked]
			public unsafe NetworkString<_128> HighScorePlayerName
			{
				readonly get
				{
					return *(NetworkString<_128>*)Native.ReferenceToPointer<FixedStorage@129>(ref this._HighScorePlayerName);
				}
				set
				{
					*(NetworkString<_128>*)Native.ReferenceToPointer<FixedStorage@129>(ref this._HighScorePlayerName) = value;
				}
			}

			// Token: 0x170006AB RID: 1707
			// (get) Token: 0x060042CD RID: 17101 RVA: 0x001353A6 File Offset: 0x001335A6
			// (set) Token: 0x060042CE RID: 17102 RVA: 0x001353AE File Offset: 0x001335AE
			public float RemainingTime { readonly get; set; }

			// Token: 0x170006AC RID: 1708
			// (get) Token: 0x060042CF RID: 17103 RVA: 0x001353B7 File Offset: 0x001335B7
			// (set) Token: 0x060042D0 RID: 17104 RVA: 0x001353BF File Offset: 0x001335BF
			public float GameEndedTime { readonly get; set; }

			// Token: 0x170006AD RID: 1709
			// (get) Token: 0x060042D1 RID: 17105 RVA: 0x001353C8 File Offset: 0x001335C8
			// (set) Token: 0x060042D2 RID: 17106 RVA: 0x001353D0 File Offset: 0x001335D0
			public int GameId { readonly get; set; }

			// Token: 0x170006AE RID: 1710
			// (get) Token: 0x060042D3 RID: 17107 RVA: 0x001353D9 File Offset: 0x001335D9
			// (set) Token: 0x060042D4 RID: 17108 RVA: 0x001353E1 File Offset: 0x001335E1
			public int PickedMolesIndexCount { readonly get; set; }

			// Token: 0x170006AF RID: 1711
			// (get) Token: 0x060042D5 RID: 17109 RVA: 0x001353EC File Offset: 0x001335EC
			[Networked]
			[Capacity(10)]
			public unsafe NetworkDictionary<int, int> PickedMolesIndex
			{
				get
				{
					return new NetworkDictionary<int, int>((int*)Native.ReferenceToPointer<FixedStorage@71>(ref this._PickedMolesIndex), 17, ReaderWriter@System_Int32.GetInstance(), ReaderWriter@System_Int32.GetInstance());
				}
			}

			// Token: 0x060042D6 RID: 17110 RVA: 0x00135418 File Offset: 0x00133618
			public WhackAMoleData(WhackAMole.GameState state, int currentLevelIndex, int cScore, int tScore, int bScore, int rPScore, string hScorePName, float remainingTime, float endedTime, int gameId, Dictionary<int, int> moleIndexs)
			{
				this.CurrentState = state;
				this.CurrentLevelIndex = currentLevelIndex;
				this.CurrentScore = cScore;
				this.TotalScore = tScore;
				this.BestScore = bScore;
				this.RightPlayerScore = rPScore;
				this.HighScorePlayerName = hScorePName;
				this.RemainingTime = remainingTime;
				this.GameEndedTime = endedTime;
				this.GameId = gameId;
				this.PickedMolesIndexCount = moleIndexs.Count;
				foreach (KeyValuePair<int, int> keyValuePair in moleIndexs)
				{
					this.PickedMolesIndex.Set(keyValuePair.Key, keyValuePair.Value);
				}
			}

			// Token: 0x0400455A RID: 17754
			[FixedBufferProperty(typeof(NetworkString<_128>), typeof(UnityValueSurrogate@ReaderWriter@Fusion_NetworkString), 0, order = -2147483647)]
			[WeaverGenerated]
			[SerializeField]
			[FieldOffset(24)]
			private FixedStorage@129 _HighScorePlayerName;

			// Token: 0x0400455F RID: 17759
			[FixedBufferProperty(typeof(NetworkDictionary<int, int>), typeof(UnityDictionarySurrogate@ReaderWriter@System_Int32@ReaderWriter@System_Int32), 17, order = -2147483647)]
			[WeaverGenerated]
			[SerializeField]
			[FieldOffset(556)]
			private FixedStorage@71 _PickedMolesIndex;
		}
	}
}
