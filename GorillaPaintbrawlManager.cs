using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using GorillaGameModes;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x0200062D RID: 1581
public class GorillaPaintbrawlManager : GorillaGameManager
{
	// Token: 0x06002734 RID: 10036 RVA: 0x000C23B8 File Offset: 0x000C05B8
	private void ActivatePaintbrawlBalloons(bool enable)
	{
		if (GorillaTagger.Instance.offlineVRRig != null)
		{
			GorillaTagger.Instance.offlineVRRig.paintbrawlBalloons.gameObject.SetActive(enable);
		}
	}

	// Token: 0x06002735 RID: 10037 RVA: 0x000C23E6 File Offset: 0x000C05E6
	private bool HasFlag(GorillaPaintbrawlManager.PaintbrawlStatus state, GorillaPaintbrawlManager.PaintbrawlStatus statusFlag)
	{
		return (state & statusFlag) > GorillaPaintbrawlManager.PaintbrawlStatus.None;
	}

	// Token: 0x06002736 RID: 10038 RVA: 0x000C23EE File Offset: 0x000C05EE
	public override GameModeType GameType()
	{
		return GameModeType.Paintbrawl;
	}

	// Token: 0x06002737 RID: 10039 RVA: 0x000C23F1 File Offset: 0x000C05F1
	public override void AddFusionDataBehaviour(NetworkObject behaviour)
	{
		behaviour.AddBehaviour<BattleGameModeData>();
	}

	// Token: 0x06002738 RID: 10040 RVA: 0x000C23FA File Offset: 0x000C05FA
	public override string GameModeName()
	{
		return "PAINTBRAWL";
	}

	// Token: 0x06002739 RID: 10041 RVA: 0x000C2404 File Offset: 0x000C0604
	private void ActivateDefaultSlingShot()
	{
		VRRig offlineVRRig = GorillaTagger.Instance.offlineVRRig;
		if (offlineVRRig != null && !Slingshot.IsSlingShotEnabled())
		{
			CosmeticsController instance = CosmeticsController.instance;
			CosmeticsController.CosmeticItem itemFromDict = instance.GetItemFromDict("Slingshot");
			instance.ApplyCosmeticItemToSet(offlineVRRig.cosmeticSet, itemFromDict, true, false);
		}
	}

	// Token: 0x0600273A RID: 10042 RVA: 0x000C244D File Offset: 0x000C064D
	public override void Awake()
	{
		base.Awake();
		this.coroutineRunning = false;
		this.currentState = GorillaPaintbrawlManager.PaintbrawlState.NotEnoughPlayers;
	}

	// Token: 0x0600273B RID: 10043 RVA: 0x000C2464 File Offset: 0x000C0664
	public override void StartPlaying()
	{
		base.StartPlaying();
		this.ActivatePaintbrawlBalloons(true);
		this.VerifyPlayersInDict<int>(this.playerLives);
		this.VerifyPlayersInDict<GorillaPaintbrawlManager.PaintbrawlStatus>(this.playerStatusDict);
		this.VerifyPlayersInDict<float>(this.playerHitTimes);
		this.VerifyPlayersInDict<float>(this.playerStunTimes);
		this.CopyBattleDictToArray();
		this.UpdateBattleState();
	}

	// Token: 0x0600273C RID: 10044 RVA: 0x000C24BC File Offset: 0x000C06BC
	public override void StopPlaying()
	{
		base.StopPlaying();
		if (Slingshot.IsSlingShotEnabled())
		{
			CosmeticsController instance = CosmeticsController.instance;
			VRRig offlineVRRig = GorillaTagger.Instance.offlineVRRig;
			CosmeticsController.CosmeticItem itemFromDict = instance.GetItemFromDict("Slingshot");
			if (offlineVRRig.cosmeticSet.HasItem("Slingshot"))
			{
				instance.ApplyCosmeticItemToSet(offlineVRRig.cosmeticSet, itemFromDict, true, false);
			}
		}
		this.ActivatePaintbrawlBalloons(false);
		base.StopAllCoroutines();
		this.coroutineRunning = false;
	}

	// Token: 0x0600273D RID: 10045 RVA: 0x000C252C File Offset: 0x000C072C
	public override void Reset()
	{
		base.Reset();
		this.playerLives.Clear();
		this.playerStatusDict.Clear();
		this.playerHitTimes.Clear();
		this.playerStunTimes.Clear();
		for (int i = 0; i < this.playerActorNumberArray.Length; i++)
		{
			this.playerLivesArray[i] = 0;
			this.playerActorNumberArray[i] = -1;
			this.playerStatusArray[i] = GorillaPaintbrawlManager.PaintbrawlStatus.None;
		}
		this.currentState = GorillaPaintbrawlManager.PaintbrawlState.NotEnoughPlayers;
	}

	// Token: 0x0600273E RID: 10046 RVA: 0x000C25A0 File Offset: 0x000C07A0
	private void VerifyPlayersInDict<T>(Dictionary<int, T> dict)
	{
		if (dict.Count < 1)
		{
			return;
		}
		int[] array = dict.Keys.ToArray<int>();
		for (int i = 0; i < array.Length; i++)
		{
			if (!Utils.PlayerInRoom(array[i]))
			{
				dict.Remove(array[i]);
			}
		}
	}

	// Token: 0x0600273F RID: 10047 RVA: 0x000C25E5 File Offset: 0x000C07E5
	internal override void NetworkLinkSetup(GameModeSerializer netSerializer)
	{
		base.NetworkLinkSetup(netSerializer);
		netSerializer.AddRPCComponent<PaintbrawlRPCs>();
	}

	// Token: 0x06002740 RID: 10048 RVA: 0x000C25F5 File Offset: 0x000C07F5
	private void Transition(GorillaPaintbrawlManager.PaintbrawlState newState)
	{
		this.currentState = newState;
		Debug.Log("current state is: " + this.currentState.ToString());
	}

	// Token: 0x06002741 RID: 10049 RVA: 0x000C2620 File Offset: 0x000C0820
	public void UpdateBattleState()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			switch (this.currentState)
			{
			case GorillaPaintbrawlManager.PaintbrawlState.NotEnoughPlayers:
				if ((float)RoomSystem.PlayersInRoom.Count >= this.playerMin)
				{
					this.Transition(GorillaPaintbrawlManager.PaintbrawlState.StartCountdown);
				}
				break;
			case GorillaPaintbrawlManager.PaintbrawlState.GameEnd:
				if (this.EndBattleGame())
				{
					this.Transition(GorillaPaintbrawlManager.PaintbrawlState.GameEndWaiting);
				}
				break;
			case GorillaPaintbrawlManager.PaintbrawlState.GameEndWaiting:
				if (this.BattleEnd())
				{
					this.Transition(GorillaPaintbrawlManager.PaintbrawlState.StartCountdown);
				}
				break;
			case GorillaPaintbrawlManager.PaintbrawlState.StartCountdown:
				this.RandomizeTeams();
				this.ActivatePaintbrawlBalloons(true);
				base.StartCoroutine(this.StartBattleCountdown());
				this.Transition(GorillaPaintbrawlManager.PaintbrawlState.CountingDownToStart);
				break;
			case GorillaPaintbrawlManager.PaintbrawlState.CountingDownToStart:
				if (!this.coroutineRunning)
				{
					this.Transition(GorillaPaintbrawlManager.PaintbrawlState.StartCountdown);
				}
				break;
			case GorillaPaintbrawlManager.PaintbrawlState.GameStart:
				this.StartBattle();
				this.Transition(GorillaPaintbrawlManager.PaintbrawlState.GameRunning);
				break;
			case GorillaPaintbrawlManager.PaintbrawlState.GameRunning:
				if (this.CheckForGameEnd())
				{
					this.Transition(GorillaPaintbrawlManager.PaintbrawlState.GameEnd);
					PlayerGameEvents.GameModeCompleteRound();
					global::GorillaGameModes.GameMode.BroadcastRoundComplete();
				}
				if ((float)RoomSystem.PlayersInRoom.Count < this.playerMin)
				{
					this.InitializePlayerStatus();
					this.ActivatePaintbrawlBalloons(false);
					this.Transition(GorillaPaintbrawlManager.PaintbrawlState.NotEnoughPlayers);
				}
				break;
			}
			this.UpdatePlayerStatus();
		}
	}

	// Token: 0x06002742 RID: 10050 RVA: 0x000C2738 File Offset: 0x000C0938
	private bool CheckForGameEnd()
	{
		this.bcount = 0;
		this.rcount = 0;
		foreach (NetPlayer netPlayer in RoomSystem.PlayersInRoom)
		{
			if (this.playerLives.TryGetValue(netPlayer.ActorNumber, out this.lives))
			{
				if (this.lives > 0 && this.playerStatusDict.TryGetValue(netPlayer.ActorNumber, out this.tempStatus))
				{
					if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.RedTeam))
					{
						this.rcount++;
					}
					else if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.BlueTeam))
					{
						this.bcount++;
					}
				}
			}
			else
			{
				this.playerLives.Add(netPlayer.ActorNumber, 0);
			}
		}
		return this.bcount == 0 || this.rcount == 0;
	}

	// Token: 0x06002743 RID: 10051 RVA: 0x000C2838 File Offset: 0x000C0A38
	public IEnumerator StartBattleCountdown()
	{
		this.coroutineRunning = true;
		this.countDownTime = 5;
		while (this.countDownTime > 0)
		{
			try
			{
				RoomSystem.SendSoundEffectAll(6, 0.25f, false);
				foreach (NetPlayer netPlayer in RoomSystem.PlayersInRoom)
				{
					this.playerLives[netPlayer.ActorNumber] = 3;
				}
			}
			catch
			{
			}
			yield return new WaitForSeconds(1f);
			this.countDownTime--;
		}
		this.coroutineRunning = false;
		this.currentState = GorillaPaintbrawlManager.PaintbrawlState.GameStart;
		yield return null;
		yield break;
	}

	// Token: 0x06002744 RID: 10052 RVA: 0x000C2848 File Offset: 0x000C0A48
	public void StartBattle()
	{
		RoomSystem.SendSoundEffectAll(7, 0.5f, false);
		foreach (NetPlayer netPlayer in RoomSystem.PlayersInRoom)
		{
			this.playerLives[netPlayer.ActorNumber] = 3;
		}
	}

	// Token: 0x06002745 RID: 10053 RVA: 0x000C28B4 File Offset: 0x000C0AB4
	private bool EndBattleGame()
	{
		if ((float)RoomSystem.PlayersInRoom.Count >= this.playerMin)
		{
			RoomSystem.SendStatusEffectAll(RoomSystem.StatusEffects.TaggedTime);
			RoomSystem.SendSoundEffectAll(2, 0.25f, false);
			this.timeBattleEnded = Time.time;
			return true;
		}
		return false;
	}

	// Token: 0x06002746 RID: 10054 RVA: 0x000C28E9 File Offset: 0x000C0AE9
	public bool BattleEnd()
	{
		return Time.time > this.timeBattleEnded + this.tagCoolDown;
	}

	// Token: 0x06002747 RID: 10055 RVA: 0x000C28FF File Offset: 0x000C0AFF
	public bool SlingshotHit(NetPlayer myPlayer, Player otherPlayer)
	{
		return this.playerLives.TryGetValue(otherPlayer.ActorNumber, out this.lives) && this.lives > 0;
	}

	// Token: 0x06002748 RID: 10056 RVA: 0x000C2928 File Offset: 0x000C0B28
	public void ReportSlingshotHit(NetPlayer taggedPlayer, Vector3 hitLocation, int projectileCount, PhotonMessageInfoWrapped info)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		if (this.currentState != GorillaPaintbrawlManager.PaintbrawlState.GameRunning)
		{
			return;
		}
		if (this.OnSameTeam(taggedPlayer, player))
		{
			return;
		}
		if (this.GetPlayerLives(taggedPlayer) > 0 && this.GetPlayerLives(player) > 0 && !this.PlayerInHitCooldown(taggedPlayer))
		{
			if (!this.playerHitTimes.TryGetValue(taggedPlayer.ActorNumber, out this.outHitTime))
			{
				this.playerHitTimes.Add(taggedPlayer.ActorNumber, Time.time);
			}
			else
			{
				this.playerHitTimes[taggedPlayer.ActorNumber] = Time.time;
			}
			Dictionary<int, int> dictionary = this.playerLives;
			int actorNumber = taggedPlayer.ActorNumber;
			int num = dictionary[actorNumber];
			dictionary[actorNumber] = num - 1;
			RoomSystem.SendSoundEffectOnOther(0, 0.25f, taggedPlayer, false);
			return;
		}
		if (this.GetPlayerLives(player) == 0 && this.GetPlayerLives(taggedPlayer) > 0)
		{
			this.tempStatus = this.GetPlayerStatus(taggedPlayer);
			if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Normal) && !this.PlayerInHitCooldown(taggedPlayer) && !this.PlayerInStunCooldown(taggedPlayer))
			{
				if (!this.playerStunTimes.TryGetValue(taggedPlayer.ActorNumber, out this.outHitTime))
				{
					this.playerStunTimes.Add(taggedPlayer.ActorNumber, Time.time);
				}
				else
				{
					this.playerStunTimes[taggedPlayer.ActorNumber] = Time.time;
				}
				RoomSystem.SendStatusEffectToPlayer(RoomSystem.StatusEffects.SetSlowedTime, taggedPlayer);
				RoomSystem.SendSoundEffectOnOther(5, 0.125f, taggedPlayer, false);
				RigContainer rigContainer;
				if (VRRigCache.Instance.TryGetVrrig(taggedPlayer, out rigContainer))
				{
					this.tempView = rigContainer.Rig.netView;
				}
			}
		}
	}

	// Token: 0x06002749 RID: 10057 RVA: 0x000C2AC4 File Offset: 0x000C0CC4
	public override void HitPlayer(NetPlayer player)
	{
		if (!NetworkSystem.Instance.IsMasterClient || this.currentState != GorillaPaintbrawlManager.PaintbrawlState.GameRunning)
		{
			return;
		}
		if (this.GetPlayerLives(player) > 0)
		{
			this.playerLives[player.ActorNumber] = 0;
			RoomSystem.SendSoundEffectOnOther(0, 0.25f, player, false);
		}
	}

	// Token: 0x0600274A RID: 10058 RVA: 0x000C2B10 File Offset: 0x000C0D10
	public override bool CanAffectPlayer(NetPlayer player, bool thisFrame)
	{
		return this.playerLives.TryGetValue(player.ActorNumber, out this.lives) && this.lives > 0;
	}

	// Token: 0x0600274B RID: 10059 RVA: 0x000C2B38 File Offset: 0x000C0D38
	public override void OnPlayerEnteredRoom(NetPlayer newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		if (NetworkSystem.Instance.IsMasterClient)
		{
			if (this.currentState == GorillaPaintbrawlManager.PaintbrawlState.GameRunning)
			{
				this.playerLives.Add(newPlayer.ActorNumber, 0);
			}
			else
			{
				this.playerLives.Add(newPlayer.ActorNumber, 3);
			}
			this.playerStatusDict.Add(newPlayer.ActorNumber, GorillaPaintbrawlManager.PaintbrawlStatus.None);
			this.CopyBattleDictToArray();
			this.AddPlayerToCorrectTeam(newPlayer);
		}
	}

	// Token: 0x0600274C RID: 10060 RVA: 0x000C2BA8 File Offset: 0x000C0DA8
	public override void OnPlayerLeftRoom(NetPlayer otherPlayer)
	{
		base.OnPlayerLeftRoom(otherPlayer);
		if (this.playerLives.ContainsKey(otherPlayer.ActorNumber))
		{
			this.playerLives.Remove(otherPlayer.ActorNumber);
		}
		if (this.playerStatusDict.ContainsKey(otherPlayer.ActorNumber))
		{
			this.playerStatusDict.Remove(otherPlayer.ActorNumber);
		}
	}

	// Token: 0x0600274D RID: 10061 RVA: 0x000C2C08 File Offset: 0x000C0E08
	public override void OnSerializeRead(object newData)
	{
		PaintbrawlData paintbrawlData = (PaintbrawlData)newData;
		paintbrawlData.playerActorNumberArray.CopyTo(this.playerActorNumberArray, true);
		paintbrawlData.playerLivesArray.CopyTo(this.playerLivesArray, true);
		paintbrawlData.playerStatusArray.CopyTo(this.playerStatusArray, true);
		this.currentState = paintbrawlData.currentPaintbrawlState;
		this.CopyArrayToBattleDict();
	}

	// Token: 0x0600274E RID: 10062 RVA: 0x000C2C70 File Offset: 0x000C0E70
	public override object OnSerializeWrite()
	{
		this.CopyBattleDictToArray();
		PaintbrawlData paintbrawlData = default(PaintbrawlData);
		paintbrawlData.playerActorNumberArray.CopyFrom(this.playerActorNumberArray, 0, this.playerActorNumberArray.Length);
		paintbrawlData.playerLivesArray.CopyFrom(this.playerLivesArray, 0, this.playerLivesArray.Length);
		paintbrawlData.playerStatusArray.CopyFrom(this.playerStatusArray, 0, this.playerStatusArray.Length);
		paintbrawlData.currentPaintbrawlState = this.currentState;
		return paintbrawlData;
	}

	// Token: 0x0600274F RID: 10063 RVA: 0x000C2CF8 File Offset: 0x000C0EF8
	public override void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
	{
		this.CopyBattleDictToArray();
		for (int i = 0; i < this.playerLivesArray.Length; i++)
		{
			stream.SendNext(this.playerActorNumberArray[i]);
			stream.SendNext(this.playerLivesArray[i]);
			stream.SendNext(this.playerStatusArray[i]);
		}
		stream.SendNext((int)this.currentState);
	}

	// Token: 0x06002750 RID: 10064 RVA: 0x000C2D68 File Offset: 0x000C0F68
	public override void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
	{
		NetworkSystem.Instance.GetPlayer(info.Sender);
		for (int i = 0; i < this.playerLivesArray.Length; i++)
		{
			this.playerActorNumberArray[i] = (int)stream.ReceiveNext();
			this.playerLivesArray[i] = (int)stream.ReceiveNext();
			this.playerStatusArray[i] = (GorillaPaintbrawlManager.PaintbrawlStatus)stream.ReceiveNext();
		}
		this.currentState = (GorillaPaintbrawlManager.PaintbrawlState)stream.ReceiveNext();
		this.CopyArrayToBattleDict();
	}

	// Token: 0x06002751 RID: 10065 RVA: 0x000C2DEC File Offset: 0x000C0FEC
	public override int MyMatIndex(NetPlayer forPlayer)
	{
		this.tempStatus = this.GetPlayerStatus(forPlayer);
		if (this.tempStatus != GorillaPaintbrawlManager.PaintbrawlStatus.None)
		{
			if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.RedTeam))
			{
				if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Normal))
				{
					return 8;
				}
				if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Hit))
				{
					return 9;
				}
				if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Stunned))
				{
					return 10;
				}
				if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Grace))
				{
					return 10;
				}
				if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Eliminated))
				{
					return 11;
				}
			}
			else
			{
				if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Normal))
				{
					return 4;
				}
				if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Hit))
				{
					return 5;
				}
				if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Stunned))
				{
					return 6;
				}
				if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Grace))
				{
					return 6;
				}
				if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Eliminated))
				{
					return 7;
				}
			}
		}
		return 0;
	}

	// Token: 0x06002752 RID: 10066 RVA: 0x000C2ED8 File Offset: 0x000C10D8
	public override float[] LocalPlayerSpeed()
	{
		if (this.playerStatusDict.TryGetValue(NetworkSystem.Instance.LocalPlayerID, out this.tempStatus))
		{
			if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Normal))
			{
				this.playerSpeed[0] = 6.5f;
				this.playerSpeed[1] = 1.1f;
				return this.playerSpeed;
			}
			if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Stunned))
			{
				this.playerSpeed[0] = 2f;
				this.playerSpeed[1] = 0.5f;
				return this.playerSpeed;
			}
			if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Eliminated))
			{
				this.playerSpeed[0] = this.fastJumpLimit;
				this.playerSpeed[1] = this.fastJumpMultiplier;
				return this.playerSpeed;
			}
		}
		this.playerSpeed[0] = 6.5f;
		this.playerSpeed[1] = 1.1f;
		return this.playerSpeed;
	}

	// Token: 0x06002753 RID: 10067 RVA: 0x000C2FB9 File Offset: 0x000C11B9
	public override void Tick()
	{
		base.Tick();
		if (NetworkSystem.Instance.IsMasterClient)
		{
			this.UpdateBattleState();
		}
		this.ActivateDefaultSlingShot();
	}

	// Token: 0x06002754 RID: 10068 RVA: 0x000C2FDC File Offset: 0x000C11DC
	public override void InfrequentUpdate()
	{
		base.InfrequentUpdate();
		foreach (int num in this.playerLives.Keys)
		{
			this.playerInList = false;
			using (List<NetPlayer>.Enumerator enumerator2 = RoomSystem.PlayersInRoom.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					if (enumerator2.Current.ActorNumber == num)
					{
						this.playerInList = true;
					}
				}
			}
			if (!this.playerInList)
			{
				this.playerLives.Remove(num);
			}
		}
	}

	// Token: 0x06002755 RID: 10069 RVA: 0x000C3098 File Offset: 0x000C1298
	public int GetPlayerLives(NetPlayer player)
	{
		if (player == null)
		{
			return 0;
		}
		if (this.playerLives.TryGetValue(player.ActorNumber, out this.outLives))
		{
			return this.outLives;
		}
		return 0;
	}

	// Token: 0x06002756 RID: 10070 RVA: 0x000C30C0 File Offset: 0x000C12C0
	public bool PlayerInHitCooldown(NetPlayer player)
	{
		float num;
		return this.playerHitTimes.TryGetValue(player.ActorNumber, out num) && num + this.hitCooldown > Time.time;
	}

	// Token: 0x06002757 RID: 10071 RVA: 0x000C30F4 File Offset: 0x000C12F4
	public bool PlayerInStunCooldown(NetPlayer player)
	{
		float num;
		return this.playerStunTimes.TryGetValue(player.ActorNumber, out num) && num + this.hitCooldown + this.stunGracePeriod > Time.time;
	}

	// Token: 0x06002758 RID: 10072 RVA: 0x000C312E File Offset: 0x000C132E
	public GorillaPaintbrawlManager.PaintbrawlStatus GetPlayerStatus(NetPlayer player)
	{
		if (this.playerStatusDict.TryGetValue(player.ActorNumber, out this.tempStatus))
		{
			return this.tempStatus;
		}
		return GorillaPaintbrawlManager.PaintbrawlStatus.None;
	}

	// Token: 0x06002759 RID: 10073 RVA: 0x000C3151 File Offset: 0x000C1351
	public bool OnRedTeam(GorillaPaintbrawlManager.PaintbrawlStatus status)
	{
		return this.HasFlag(status, GorillaPaintbrawlManager.PaintbrawlStatus.RedTeam);
	}

	// Token: 0x0600275A RID: 10074 RVA: 0x000C315C File Offset: 0x000C135C
	public bool OnRedTeam(NetPlayer player)
	{
		GorillaPaintbrawlManager.PaintbrawlStatus playerStatus = this.GetPlayerStatus(player);
		return this.OnRedTeam(playerStatus);
	}

	// Token: 0x0600275B RID: 10075 RVA: 0x000C3178 File Offset: 0x000C1378
	public bool OnBlueTeam(GorillaPaintbrawlManager.PaintbrawlStatus status)
	{
		return this.HasFlag(status, GorillaPaintbrawlManager.PaintbrawlStatus.BlueTeam);
	}

	// Token: 0x0600275C RID: 10076 RVA: 0x000C3184 File Offset: 0x000C1384
	public bool OnBlueTeam(NetPlayer player)
	{
		GorillaPaintbrawlManager.PaintbrawlStatus playerStatus = this.GetPlayerStatus(player);
		return this.OnBlueTeam(playerStatus);
	}

	// Token: 0x0600275D RID: 10077 RVA: 0x000C31A0 File Offset: 0x000C13A0
	public bool OnNoTeam(GorillaPaintbrawlManager.PaintbrawlStatus status)
	{
		return !this.OnRedTeam(status) && !this.OnBlueTeam(status);
	}

	// Token: 0x0600275E RID: 10078 RVA: 0x000C31B8 File Offset: 0x000C13B8
	public bool OnNoTeam(NetPlayer player)
	{
		GorillaPaintbrawlManager.PaintbrawlStatus playerStatus = this.GetPlayerStatus(player);
		return this.OnNoTeam(playerStatus);
	}

	// Token: 0x0600275F RID: 10079 RVA: 0x00002076 File Offset: 0x00000276
	public override bool LocalCanTag(NetPlayer myPlayer, NetPlayer otherPlayer)
	{
		return false;
	}

	// Token: 0x06002760 RID: 10080 RVA: 0x000C31D4 File Offset: 0x000C13D4
	public bool OnSameTeam(GorillaPaintbrawlManager.PaintbrawlStatus playerA, GorillaPaintbrawlManager.PaintbrawlStatus playerB)
	{
		bool flag = this.OnRedTeam(playerA) && this.OnRedTeam(playerB);
		bool flag2 = this.OnBlueTeam(playerA) && this.OnBlueTeam(playerB);
		return flag || flag2;
	}

	// Token: 0x06002761 RID: 10081 RVA: 0x000C320C File Offset: 0x000C140C
	public bool OnSameTeam(NetPlayer myPlayer, NetPlayer otherPlayer)
	{
		GorillaPaintbrawlManager.PaintbrawlStatus playerStatus = this.GetPlayerStatus(myPlayer);
		GorillaPaintbrawlManager.PaintbrawlStatus playerStatus2 = this.GetPlayerStatus(otherPlayer);
		return this.OnSameTeam(playerStatus, playerStatus2);
	}

	// Token: 0x06002762 RID: 10082 RVA: 0x000C3234 File Offset: 0x000C1434
	public bool LocalCanHit(NetPlayer myPlayer, NetPlayer otherPlayer)
	{
		bool flag = !this.OnSameTeam(myPlayer, otherPlayer);
		bool flag2 = this.GetPlayerLives(otherPlayer) != 0;
		return flag && flag2;
	}

	// Token: 0x06002763 RID: 10083 RVA: 0x000C325C File Offset: 0x000C145C
	private void CopyBattleDictToArray()
	{
		for (int i = 0; i < this.playerLivesArray.Length; i++)
		{
			this.playerLivesArray[i] = 0;
			this.playerActorNumberArray[i] = -1;
		}
		this.keyValuePairs = this.playerLives.ToArray<KeyValuePair<int, int>>();
		int num = 0;
		while (num < this.playerLivesArray.Length && num < this.keyValuePairs.Length)
		{
			this.playerActorNumberArray[num] = this.keyValuePairs[num].Key;
			this.playerLivesArray[num] = this.keyValuePairs[num].Value;
			this.playerStatusArray[num] = this.GetPlayerStatus(NetworkSystem.Instance.GetPlayer(this.keyValuePairs[num].Key));
			num++;
		}
	}

	// Token: 0x06002764 RID: 10084 RVA: 0x000C3318 File Offset: 0x000C1518
	private void CopyArrayToBattleDict()
	{
		for (int i = 0; i < this.playerLivesArray.Length; i++)
		{
			if (this.playerActorNumberArray[i] != -1 && Utils.PlayerInRoom(this.playerActorNumberArray[i]))
			{
				if (this.playerLives.TryGetValue(this.playerActorNumberArray[i], out this.outLives))
				{
					this.playerLives[this.playerActorNumberArray[i]] = this.playerLivesArray[i];
				}
				else
				{
					this.playerLives.Add(this.playerActorNumberArray[i], this.playerLivesArray[i]);
				}
				if (this.playerStatusDict.ContainsKey(this.playerActorNumberArray[i]))
				{
					this.playerStatusDict[this.playerActorNumberArray[i]] = this.playerStatusArray[i];
				}
				else
				{
					this.playerStatusDict.Add(this.playerActorNumberArray[i], this.playerStatusArray[i]);
				}
			}
		}
	}

	// Token: 0x06002765 RID: 10085 RVA: 0x000C33FE File Offset: 0x000C15FE
	private GorillaPaintbrawlManager.PaintbrawlStatus SetFlag(GorillaPaintbrawlManager.PaintbrawlStatus currState, GorillaPaintbrawlManager.PaintbrawlStatus flag)
	{
		return currState | flag;
	}

	// Token: 0x06002766 RID: 10086 RVA: 0x000C3403 File Offset: 0x000C1603
	private GorillaPaintbrawlManager.PaintbrawlStatus SetFlagExclusive(GorillaPaintbrawlManager.PaintbrawlStatus currState, GorillaPaintbrawlManager.PaintbrawlStatus flag)
	{
		return flag;
	}

	// Token: 0x06002767 RID: 10087 RVA: 0x000C3406 File Offset: 0x000C1606
	private GorillaPaintbrawlManager.PaintbrawlStatus ClearFlag(GorillaPaintbrawlManager.PaintbrawlStatus currState, GorillaPaintbrawlManager.PaintbrawlStatus flag)
	{
		return currState & ~flag;
	}

	// Token: 0x06002768 RID: 10088 RVA: 0x000C23E6 File Offset: 0x000C05E6
	private bool FlagIsSet(GorillaPaintbrawlManager.PaintbrawlStatus currState, GorillaPaintbrawlManager.PaintbrawlStatus flag)
	{
		return (currState & flag) > GorillaPaintbrawlManager.PaintbrawlStatus.None;
	}

	// Token: 0x06002769 RID: 10089 RVA: 0x000C340C File Offset: 0x000C160C
	public void RandomizeTeams()
	{
		int[] array = new int[RoomSystem.PlayersInRoom.Count];
		for (int i = 0; i < RoomSystem.PlayersInRoom.Count; i++)
		{
			array[i] = i;
		}
		Random rand = new Random();
		int[] array2 = array.OrderBy((int x) => rand.Next()).ToArray<int>();
		GorillaPaintbrawlManager.PaintbrawlStatus paintbrawlStatus = ((rand.Next(0, 2) == 0) ? GorillaPaintbrawlManager.PaintbrawlStatus.RedTeam : GorillaPaintbrawlManager.PaintbrawlStatus.BlueTeam);
		GorillaPaintbrawlManager.PaintbrawlStatus paintbrawlStatus2 = ((paintbrawlStatus == GorillaPaintbrawlManager.PaintbrawlStatus.RedTeam) ? GorillaPaintbrawlManager.PaintbrawlStatus.BlueTeam : GorillaPaintbrawlManager.PaintbrawlStatus.RedTeam);
		for (int j = 0; j < RoomSystem.PlayersInRoom.Count; j++)
		{
			GorillaPaintbrawlManager.PaintbrawlStatus paintbrawlStatus3 = ((array2[j] % 2 == 0) ? paintbrawlStatus2 : paintbrawlStatus);
			this.playerStatusDict[RoomSystem.PlayersInRoom[j].ActorNumber] = paintbrawlStatus3;
		}
	}

	// Token: 0x0600276A RID: 10090 RVA: 0x000C34D8 File Offset: 0x000C16D8
	public void AddPlayerToCorrectTeam(NetPlayer newPlayer)
	{
		this.rcount = 0;
		for (int i = 0; i < RoomSystem.PlayersInRoom.Count; i++)
		{
			if (this.playerStatusDict.ContainsKey(RoomSystem.PlayersInRoom[i].ActorNumber))
			{
				GorillaPaintbrawlManager.PaintbrawlStatus paintbrawlStatus = this.playerStatusDict[RoomSystem.PlayersInRoom[i].ActorNumber];
				this.rcount = (this.HasFlag(paintbrawlStatus, GorillaPaintbrawlManager.PaintbrawlStatus.RedTeam) ? (this.rcount + 1) : this.rcount);
			}
		}
		if ((RoomSystem.PlayersInRoom.Count - 1) / 2 == this.rcount)
		{
			this.playerStatusDict[newPlayer.ActorNumber] = ((Random.Range(0, 2) == 0) ? this.SetFlag(this.playerStatusDict[newPlayer.ActorNumber], GorillaPaintbrawlManager.PaintbrawlStatus.RedTeam) : this.SetFlag(this.playerStatusDict[newPlayer.ActorNumber], GorillaPaintbrawlManager.PaintbrawlStatus.BlueTeam));
			return;
		}
		if (this.rcount <= (RoomSystem.PlayersInRoom.Count - 1) / 2)
		{
			this.playerStatusDict[newPlayer.ActorNumber] = this.SetFlag(this.playerStatusDict[newPlayer.ActorNumber], GorillaPaintbrawlManager.PaintbrawlStatus.RedTeam);
		}
	}

	// Token: 0x0600276B RID: 10091 RVA: 0x000C35FC File Offset: 0x000C17FC
	private void InitializePlayerStatus()
	{
		this.keyValuePairsStatus = this.playerStatusDict.ToArray<KeyValuePair<int, GorillaPaintbrawlManager.PaintbrawlStatus>>();
		foreach (KeyValuePair<int, GorillaPaintbrawlManager.PaintbrawlStatus> keyValuePair in this.keyValuePairsStatus)
		{
			this.playerStatusDict[keyValuePair.Key] = GorillaPaintbrawlManager.PaintbrawlStatus.Normal;
		}
	}

	// Token: 0x0600276C RID: 10092 RVA: 0x000C364C File Offset: 0x000C184C
	private void UpdatePlayerStatus()
	{
		this.keyValuePairsStatus = this.playerStatusDict.ToArray<KeyValuePair<int, GorillaPaintbrawlManager.PaintbrawlStatus>>();
		foreach (KeyValuePair<int, GorillaPaintbrawlManager.PaintbrawlStatus> keyValuePair in this.keyValuePairsStatus)
		{
			GorillaPaintbrawlManager.PaintbrawlStatus paintbrawlStatus = (this.HasFlag(this.playerStatusDict[keyValuePair.Key], GorillaPaintbrawlManager.PaintbrawlStatus.RedTeam) ? GorillaPaintbrawlManager.PaintbrawlStatus.RedTeam : GorillaPaintbrawlManager.PaintbrawlStatus.BlueTeam);
			if (this.playerLives.TryGetValue(keyValuePair.Key, out this.outLives) && this.outLives == 0)
			{
				this.playerStatusDict[keyValuePair.Key] = paintbrawlStatus | GorillaPaintbrawlManager.PaintbrawlStatus.Eliminated;
			}
			else if (this.playerHitTimes.TryGetValue(keyValuePair.Key, out this.outHitTime) && this.outHitTime + this.hitCooldown > Time.time)
			{
				this.playerStatusDict[keyValuePair.Key] = paintbrawlStatus | GorillaPaintbrawlManager.PaintbrawlStatus.Hit;
			}
			else if (this.playerStunTimes.TryGetValue(keyValuePair.Key, out this.outHitTime))
			{
				if (this.outHitTime + this.hitCooldown > Time.time)
				{
					this.playerStatusDict[keyValuePair.Key] = paintbrawlStatus | GorillaPaintbrawlManager.PaintbrawlStatus.Stunned;
				}
				else if (this.outHitTime + this.hitCooldown + this.stunGracePeriod > Time.time)
				{
					this.playerStatusDict[keyValuePair.Key] = paintbrawlStatus | GorillaPaintbrawlManager.PaintbrawlStatus.Grace;
				}
				else
				{
					this.playerStatusDict[keyValuePair.Key] = paintbrawlStatus | GorillaPaintbrawlManager.PaintbrawlStatus.Normal;
				}
			}
			else
			{
				this.playerStatusDict[keyValuePair.Key] = paintbrawlStatus | GorillaPaintbrawlManager.PaintbrawlStatus.Normal;
			}
		}
	}

	// Token: 0x04002BBD RID: 11197
	private float playerMin = 2f;

	// Token: 0x04002BBE RID: 11198
	public float tagCoolDown = 5f;

	// Token: 0x04002BBF RID: 11199
	public Dictionary<int, int> playerLives = new Dictionary<int, int>();

	// Token: 0x04002BC0 RID: 11200
	public Dictionary<int, GorillaPaintbrawlManager.PaintbrawlStatus> playerStatusDict = new Dictionary<int, GorillaPaintbrawlManager.PaintbrawlStatus>();

	// Token: 0x04002BC1 RID: 11201
	public Dictionary<int, float> playerHitTimes = new Dictionary<int, float>();

	// Token: 0x04002BC2 RID: 11202
	public Dictionary<int, float> playerStunTimes = new Dictionary<int, float>();

	// Token: 0x04002BC3 RID: 11203
	public int[] playerActorNumberArray = new int[] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

	// Token: 0x04002BC4 RID: 11204
	public int[] playerLivesArray = new int[10];

	// Token: 0x04002BC5 RID: 11205
	public GorillaPaintbrawlManager.PaintbrawlStatus[] playerStatusArray = new GorillaPaintbrawlManager.PaintbrawlStatus[10];

	// Token: 0x04002BC6 RID: 11206
	public bool teamBattle = true;

	// Token: 0x04002BC7 RID: 11207
	public int countDownTime;

	// Token: 0x04002BC8 RID: 11208
	private float timeBattleEnded;

	// Token: 0x04002BC9 RID: 11209
	public float hitCooldown = 3f;

	// Token: 0x04002BCA RID: 11210
	public float stunGracePeriod = 2f;

	// Token: 0x04002BCB RID: 11211
	public object objRef;

	// Token: 0x04002BCC RID: 11212
	private bool playerInList;

	// Token: 0x04002BCD RID: 11213
	private bool coroutineRunning;

	// Token: 0x04002BCE RID: 11214
	private int lives;

	// Token: 0x04002BCF RID: 11215
	private int outLives;

	// Token: 0x04002BD0 RID: 11216
	private int bcount;

	// Token: 0x04002BD1 RID: 11217
	private int rcount;

	// Token: 0x04002BD2 RID: 11218
	private int randInt;

	// Token: 0x04002BD3 RID: 11219
	private float outHitTime;

	// Token: 0x04002BD4 RID: 11220
	private NetworkView tempView;

	// Token: 0x04002BD5 RID: 11221
	private KeyValuePair<int, int>[] keyValuePairs;

	// Token: 0x04002BD6 RID: 11222
	private KeyValuePair<int, GorillaPaintbrawlManager.PaintbrawlStatus>[] keyValuePairsStatus;

	// Token: 0x04002BD7 RID: 11223
	private GorillaPaintbrawlManager.PaintbrawlStatus tempStatus;

	// Token: 0x04002BD8 RID: 11224
	private GorillaPaintbrawlManager.PaintbrawlState currentState;

	// Token: 0x0200062E RID: 1582
	public enum PaintbrawlStatus
	{
		// Token: 0x04002BDA RID: 11226
		RedTeam = 1,
		// Token: 0x04002BDB RID: 11227
		BlueTeam,
		// Token: 0x04002BDC RID: 11228
		Normal = 4,
		// Token: 0x04002BDD RID: 11229
		Hit = 8,
		// Token: 0x04002BDE RID: 11230
		Stunned = 16,
		// Token: 0x04002BDF RID: 11231
		Grace = 32,
		// Token: 0x04002BE0 RID: 11232
		Eliminated = 64,
		// Token: 0x04002BE1 RID: 11233
		None = 0
	}

	// Token: 0x0200062F RID: 1583
	public enum PaintbrawlState
	{
		// Token: 0x04002BE3 RID: 11235
		NotEnoughPlayers,
		// Token: 0x04002BE4 RID: 11236
		GameEnd,
		// Token: 0x04002BE5 RID: 11237
		GameEndWaiting,
		// Token: 0x04002BE6 RID: 11238
		StartCountdown,
		// Token: 0x04002BE7 RID: 11239
		CountingDownToStart,
		// Token: 0x04002BE8 RID: 11240
		GameStart,
		// Token: 0x04002BE9 RID: 11241
		GameRunning
	}
}
