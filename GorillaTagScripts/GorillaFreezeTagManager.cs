using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GorillaGameModes;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000B12 RID: 2834
	public class GorillaFreezeTagManager : GorillaTagManager
	{
		// Token: 0x06004598 RID: 17816 RVA: 0x000A7C5D File Offset: 0x000A5E5D
		public override GameModeType GameType()
		{
			return GameModeType.FreezeTag;
		}

		// Token: 0x06004599 RID: 17817 RVA: 0x0014AA62 File Offset: 0x00148C62
		public override string GameModeName()
		{
			return "FREEZE TAG";
		}

		// Token: 0x0600459A RID: 17818 RVA: 0x0014AA69 File Offset: 0x00148C69
		public override void Awake()
		{
			base.Awake();
			this.fastJumpLimitCached = this.fastJumpLimit;
			this.fastJumpMultiplierCached = this.fastJumpMultiplier;
			this.slowJumpLimitCached = this.slowJumpLimit;
			this.slowJumpMultiplierCached = this.slowJumpMultiplier;
		}

		// Token: 0x0600459B RID: 17819 RVA: 0x0014AAA4 File Offset: 0x00148CA4
		public override void UpdateState()
		{
			if (NetworkSystem.Instance.IsMasterClient)
			{
				foreach (KeyValuePair<NetPlayer, float> keyValuePair in this.currentFrozen.ToList<KeyValuePair<NetPlayer, float>>())
				{
					if (Time.time - keyValuePair.Value >= this.freezeDuration)
					{
						this.currentFrozen.Remove(keyValuePair.Key);
						this.AddInfectedPlayer(keyValuePair.Key, false);
						RoomSystem.SendSoundEffectAll(11, 0.25f, false);
					}
				}
				if (GameMode.ParticipatingPlayers.Count < 1)
				{
					this.Reset();
					base.SetisCurrentlyTag(true);
					return;
				}
				if (this.isCurrentlyTag && this.currentIt == null)
				{
					int num = Random.Range(0, GameMode.ParticipatingPlayers.Count);
					base.ChangeCurrentIt(GameMode.ParticipatingPlayers[num], false);
				}
				else if (this.isCurrentlyTag && GameMode.ParticipatingPlayers.Count >= this.infectedModeThreshold)
				{
					this.Reset();
					int num2 = Random.Range(0, GameMode.ParticipatingPlayers.Count);
					this.AddInfectedPlayer(GameMode.ParticipatingPlayers[num2], true);
				}
				else if (!this.isCurrentlyTag && GameMode.ParticipatingPlayers.Count < this.infectedModeThreshold)
				{
					this.Reset();
					base.SetisCurrentlyTag(true);
					int num3 = Random.Range(0, GameMode.ParticipatingPlayers.Count);
					base.ChangeCurrentIt(GameMode.ParticipatingPlayers[num3], false);
				}
				else if (!this.isCurrentlyTag && this.currentInfected.Count == 0)
				{
					int num4 = Random.Range(0, GameMode.ParticipatingPlayers.Count);
					this.AddInfectedPlayer(GameMode.ParticipatingPlayers[num4], true);
				}
				bool flag = true;
				foreach (NetPlayer netPlayer in GameMode.ParticipatingPlayers)
				{
					if (!this.IsFrozen(netPlayer) && !base.IsInfected(netPlayer))
					{
						flag = false;
						break;
					}
				}
				if (flag && !this.isCurrentlyTag)
				{
					base.EndInfectionGame();
				}
			}
		}

		// Token: 0x0600459C RID: 17820 RVA: 0x0014ACD4 File Offset: 0x00148ED4
		public override void Tick()
		{
			base.Tick();
			if (this.localVRRig)
			{
				this.localVRRig.IsFrozen = this.IsFrozen(NetworkSystem.Instance.LocalPlayer);
			}
		}

		// Token: 0x0600459D RID: 17821 RVA: 0x0014AD04 File Offset: 0x00148F04
		public override void StartPlaying()
		{
			base.StartPlaying();
			this.localVRRig = this.FindPlayerVRRig(NetworkSystem.Instance.LocalPlayer);
			if (NetworkSystem.Instance.IsMasterClient)
			{
				foreach (NetPlayer netPlayer in this.lastRoundInfectedPlayers.ToArray())
				{
					if (netPlayer != null && !netPlayer.InRoom)
					{
						this.lastRoundInfectedPlayers.Remove(netPlayer);
					}
				}
				foreach (NetPlayer netPlayer2 in this.currentRoundInfectedPlayers.ToArray())
				{
					if (netPlayer2 != null && !netPlayer2.InRoom)
					{
						this.currentRoundInfectedPlayers.Remove(netPlayer2);
					}
				}
			}
		}

		// Token: 0x0600459E RID: 17822 RVA: 0x0014ADA8 File Offset: 0x00148FA8
		public override void ReportTag(NetPlayer taggedPlayer, NetPlayer taggingPlayer)
		{
			if (NetworkSystem.Instance.IsMasterClient)
			{
				this.taggingRig = this.FindPlayerVRRig(taggingPlayer);
				this.taggedRig = this.FindPlayerVRRig(taggedPlayer);
				if (this.taggingRig == null || this.taggedRig == null)
				{
					return;
				}
				Debug.LogWarning("Report TAG - tagged " + this.taggedRig.playerNameVisible + ", tagging " + this.taggingRig.playerNameVisible);
				if (this.isCurrentlyTag)
				{
					if (taggingPlayer == this.currentIt && taggingPlayer != taggedPlayer && (double)Time.time > this.lastTag + (double)this.tagCoolDown)
					{
						base.AddLastTagged(taggedPlayer, taggingPlayer);
						base.ChangeCurrentIt(taggedPlayer, false);
						this.lastTag = (double)Time.time;
						return;
					}
				}
				else if (this.currentInfected.Contains(taggingPlayer) && !this.currentInfected.Contains(taggedPlayer) && !this.currentFrozen.ContainsKey(taggedPlayer) && (double)Time.time > this.lastTag + (double)this.tagCoolDown)
				{
					if (!this.taggingRig.IsPositionInRange(this.taggedRig.transform.position, 6f) && !this.taggingRig.CheckTagDistanceRollback(this.taggedRig, 6f, 0.2f))
					{
						GorillaNot.instance.SendReport("extremely far tag", taggingPlayer.UserId, taggingPlayer.NickName);
						return;
					}
					base.AddLastTagged(taggedPlayer, taggingPlayer);
					this.AddFrozenPlayer(taggedPlayer);
					return;
				}
				else if (!this.currentInfected.Contains(taggingPlayer) && !this.currentInfected.Contains(taggedPlayer) && this.currentFrozen.ContainsKey(taggedPlayer) && (double)Time.time > this.lastTag + (double)this.tagCoolDown)
				{
					if (!this.taggingRig.IsPositionInRange(this.taggedRig.transform.position, 6f) && !this.taggingRig.CheckTagDistanceRollback(this.taggedRig, 6f, 0.2f))
					{
						GorillaNot.instance.SendReport("extremely far tag", taggingPlayer.UserId, taggingPlayer.NickName);
						return;
					}
					this.UnfreezePlayer(taggedPlayer);
				}
			}
		}

		// Token: 0x0600459F RID: 17823 RVA: 0x0014AFD8 File Offset: 0x001491D8
		public override bool LocalCanTag(NetPlayer myPlayer, NetPlayer otherPlayer)
		{
			if (this.isCurrentlyTag)
			{
				return myPlayer == this.currentIt && myPlayer != otherPlayer;
			}
			return (this.currentInfected.Contains(myPlayer) && !this.currentFrozen.ContainsKey(otherPlayer) && !this.currentInfected.Contains(otherPlayer)) || (!this.currentInfected.Contains(myPlayer) && !this.currentFrozen.ContainsKey(myPlayer) && (this.currentInfected.Contains(otherPlayer) || this.currentFrozen.ContainsKey(otherPlayer)));
		}

		// Token: 0x060045A0 RID: 17824 RVA: 0x0014B067 File Offset: 0x00149267
		public override void NewVRRig(NetPlayer player, int vrrigPhotonViewID, bool didTutorial)
		{
			if (NetworkSystem.Instance.IsMasterClient)
			{
				GameMode.RefreshPlayers();
				if (!this.isCurrentlyTag && !base.IsInfected(player))
				{
					this.AddInfectedPlayer(player, true);
					this.currentRoundInfectedPlayers.Add(player);
				}
				base.UpdateInfectionState();
			}
		}

		// Token: 0x060045A1 RID: 17825 RVA: 0x0014B0A5 File Offset: 0x001492A5
		protected override IEnumerator InfectionEnd()
		{
			while ((double)Time.time < this.timeInfectedGameEnded + (double)this.tagCoolDown)
			{
				yield return new WaitForSeconds(0.1f);
			}
			if (!this.isCurrentlyTag && this.waitingToStartNextInfectionGame)
			{
				base.ClearInfectionState();
				this.currentFrozen.Clear();
				GameMode.RefreshPlayers();
				this.lastRoundInfectedPlayers.Clear();
				this.lastRoundInfectedPlayers.AddRange(this.currentRoundInfectedPlayers);
				this.currentRoundInfectedPlayers.Clear();
				List<NetPlayer> participatingPlayers = GameMode.ParticipatingPlayers;
				int num = 0;
				if (participatingPlayers.Count > 0 && participatingPlayers.Count < this.infectMorePlayerLowerThreshold)
				{
					num = 1;
				}
				else if (participatingPlayers.Count >= this.infectMorePlayerLowerThreshold && participatingPlayers.Count < this.infectMorePlayerUpperThreshold)
				{
					num = 2;
				}
				else if (participatingPlayers.Count >= this.infectMorePlayerUpperThreshold)
				{
					num = 3;
				}
				for (int i = 0; i < num; i++)
				{
					this.TryAddNewInfectedPlayer();
				}
				this.lastTag = (double)Time.time;
			}
			yield return null;
			yield break;
		}

		// Token: 0x060045A2 RID: 17826 RVA: 0x0014B0B4 File Offset: 0x001492B4
		public override void Reset()
		{
			base.Reset();
			this.currentFrozen.Clear();
			this.currentRoundInfectedPlayers.Clear();
			this.lastRoundInfectedPlayers.Clear();
		}

		// Token: 0x060045A3 RID: 17827 RVA: 0x000C7B7E File Offset: 0x000C5D7E
		private new void AddInfectedPlayer(NetPlayer infectedPlayer, bool withTagStop = true)
		{
			if (NetworkSystem.Instance.IsMasterClient)
			{
				this.currentInfected.Add(infectedPlayer);
				if (!withTagStop)
				{
					RoomSystem.SendStatusEffectToPlayer(RoomSystem.StatusEffects.JoinedTaggedTime, infectedPlayer);
				}
				else
				{
					RoomSystem.SendStatusEffectToPlayer(RoomSystem.StatusEffects.TaggedTime, infectedPlayer);
				}
				RoomSystem.SendSoundEffectOnOther(0, 0.25f, infectedPlayer, false);
				base.UpdateInfectionState();
			}
		}

		// Token: 0x060045A4 RID: 17828 RVA: 0x0014B0E0 File Offset: 0x001492E0
		private void TryAddNewInfectedPlayer()
		{
			List<NetPlayer> participatingPlayers = GameMode.ParticipatingPlayers;
			int num = Random.Range(0, participatingPlayers.Count);
			int num2 = 0;
			while (num2 < 10 && this.lastRoundInfectedPlayers.Contains(participatingPlayers[num]))
			{
				num = Random.Range(0, participatingPlayers.Count);
				num2++;
			}
			this.AddInfectedPlayer(participatingPlayers[num], true);
			this.currentRoundInfectedPlayers.Add(participatingPlayers[num]);
		}

		// Token: 0x060045A5 RID: 17829 RVA: 0x0014B14E File Offset: 0x0014934E
		public override int MyMatIndex(NetPlayer forPlayer)
		{
			if (this.isCurrentlyTag && forPlayer == this.currentIt)
			{
				return 14;
			}
			if (this.currentInfected.Contains(forPlayer))
			{
				return 14;
			}
			return 0;
		}

		// Token: 0x060045A6 RID: 17830 RVA: 0x0014B178 File Offset: 0x00149378
		public override void UpdatePlayerAppearance(VRRig rig)
		{
			NetPlayer netPlayer = (rig.isOfflineVRRig ? NetworkSystem.Instance.LocalPlayer : rig.creator);
			rig.UpdateFrozenEffect(this.IsFrozen(netPlayer));
			int num = this.MyMatIndex(netPlayer);
			rig.ChangeMaterialLocal(num);
		}

		// Token: 0x060045A7 RID: 17831 RVA: 0x0014B1BC File Offset: 0x001493BC
		private void UnfreezePlayer(NetPlayer taggedPlayer)
		{
			if (NetworkSystem.Instance.IsMasterClient && this.currentFrozen.ContainsKey(taggedPlayer))
			{
				this.currentFrozen.Remove(taggedPlayer);
				RoomSystem.SendStatusEffectToPlayer(RoomSystem.StatusEffects.UnTagged, taggedPlayer);
				RoomSystem.SendSoundEffectAll(10, 0.25f, true);
			}
		}

		// Token: 0x060045A8 RID: 17832 RVA: 0x0014B1FC File Offset: 0x001493FC
		private void AddFrozenPlayer(NetPlayer taggedPlayer)
		{
			if (NetworkSystem.Instance.IsMasterClient && !this.currentFrozen.ContainsKey(taggedPlayer))
			{
				this.currentFrozen.Add(taggedPlayer, Time.time);
				RoomSystem.SendStatusEffectToPlayer(RoomSystem.StatusEffects.FrozenTime, taggedPlayer);
				RoomSystem.SendSoundEffectAll(9, 0.25f, false);
				RoomSystem.SendSoundEffectToPlayer(12, 0.05f, taggedPlayer, false);
			}
		}

		// Token: 0x060045A9 RID: 17833 RVA: 0x0014B256 File Offset: 0x00149456
		public bool IsFrozen(NetPlayer player)
		{
			return this.currentFrozen.ContainsKey(player);
		}

		// Token: 0x060045AA RID: 17834 RVA: 0x0014B264 File Offset: 0x00149464
		public override float[] LocalPlayerSpeed()
		{
			this.fastJumpLimit = this.fastJumpLimitCached;
			this.fastJumpMultiplier = this.fastJumpMultiplierCached;
			this.slowJumpLimit = this.slowJumpLimitCached;
			this.slowJumpMultiplier = this.slowJumpMultiplierCached;
			if (this.isCurrentlyTag)
			{
				if (NetworkSystem.Instance.LocalPlayer == this.currentIt)
				{
					this.playerSpeed[0] = this.fastJumpLimit;
					this.playerSpeed[1] = this.fastJumpMultiplier;
					return this.playerSpeed;
				}
				this.playerSpeed[0] = this.slowJumpLimit;
				this.playerSpeed[1] = this.slowJumpMultiplier;
				return this.playerSpeed;
			}
			else
			{
				if (!this.currentInfected.Contains(NetworkSystem.Instance.LocalPlayer) && !this.currentFrozen.ContainsKey(NetworkSystem.Instance.LocalPlayer))
				{
					this.playerSpeed[0] = base.InterpolatedNoobJumpSpeed(this.currentInfected.Count);
					this.playerSpeed[1] = base.InterpolatedNoobJumpMultiplier(this.currentInfected.Count);
					return this.playerSpeed;
				}
				if (this.currentFrozen.ContainsKey(NetworkSystem.Instance.LocalPlayer))
				{
					this.fastJumpLimit = this.frozenPlayerFastJumpLimit;
					this.fastJumpMultiplier = this.frozenPlayerFastJumpMultiplier;
					this.slowJumpLimit = this.frozenPlayerSlowJumpLimit;
					this.slowJumpMultiplier = this.frozenPlayerSlowJumpMultiplier;
				}
				this.playerSpeed[0] = base.InterpolatedInfectedJumpSpeed(this.currentInfected.Count);
				this.playerSpeed[1] = base.InterpolatedInfectedJumpMultiplier(this.currentInfected.Count);
				return this.playerSpeed;
			}
		}

		// Token: 0x060045AB RID: 17835 RVA: 0x0014B3E8 File Offset: 0x001495E8
		public int GetFrozenHandTapAudioIndex()
		{
			int num = Random.Range(0, this.frozenHandTapIndices.Length);
			return this.frozenHandTapIndices[num];
		}

		// Token: 0x060045AC RID: 17836 RVA: 0x0014B40C File Offset: 0x0014960C
		public override void OnPlayerLeftRoom(NetPlayer otherPlayer)
		{
			base.OnPlayerLeftRoom(otherPlayer);
			if (NetworkSystem.Instance.IsMasterClient)
			{
				if (this.isCurrentlyTag && ((otherPlayer != null && otherPlayer == this.currentIt) || this.currentIt.ActorNumber == otherPlayer.ActorNumber) && GameMode.ParticipatingPlayers.Count > 0)
				{
					int num = Random.Range(0, GameMode.ParticipatingPlayers.Count);
					base.ChangeCurrentIt(GameMode.ParticipatingPlayers[num], false);
				}
				if (this.currentInfected.Contains(otherPlayer))
				{
					this.currentInfected.Remove(otherPlayer);
				}
				if (this.currentFrozen.ContainsKey(otherPlayer))
				{
					this.currentFrozen.Remove(otherPlayer);
				}
				this.UpdateState();
			}
		}

		// Token: 0x060045AD RID: 17837 RVA: 0x0014B4C4 File Offset: 0x001496C4
		public override void StopPlaying()
		{
			base.StopPlaying();
			foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
			{
				vrrig.ForceResetFrozenEffect();
			}
		}

		// Token: 0x060045AE RID: 17838 RVA: 0x0014B520 File Offset: 0x00149720
		public override void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
		{
			base.OnSerializeRead(stream, info);
			this.currentFrozen.Clear();
			int num = (int)stream.ReceiveNext();
			for (int i = 0; i < num; i++)
			{
				int num2 = (int)stream.ReceiveNext();
				float num3 = (float)stream.ReceiveNext();
				NetPlayer player = NetworkSystem.Instance.GetPlayer(num2);
				this.currentFrozen.Add(player, num3);
			}
		}

		// Token: 0x060045AF RID: 17839 RVA: 0x0014B58C File Offset: 0x0014978C
		public override void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
		{
			base.OnSerializeWrite(stream, info);
			stream.SendNext(this.currentFrozen.Count);
			foreach (KeyValuePair<NetPlayer, float> keyValuePair in this.currentFrozen)
			{
				stream.SendNext(keyValuePair.Key.ActorNumber);
				stream.SendNext(keyValuePair.Value);
			}
		}

		// Token: 0x0400483F RID: 18495
		public Dictionary<NetPlayer, float> currentFrozen = new Dictionary<NetPlayer, float>(10);

		// Token: 0x04004840 RID: 18496
		public float freezeDuration;

		// Token: 0x04004841 RID: 18497
		public int infectMorePlayerLowerThreshold = 6;

		// Token: 0x04004842 RID: 18498
		public int infectMorePlayerUpperThreshold = 10;

		// Token: 0x04004843 RID: 18499
		[Space]
		[Header("Frozen player jump settings")]
		public float frozenPlayerFastJumpLimit;

		// Token: 0x04004844 RID: 18500
		public float frozenPlayerFastJumpMultiplier;

		// Token: 0x04004845 RID: 18501
		public float frozenPlayerSlowJumpLimit;

		// Token: 0x04004846 RID: 18502
		public float frozenPlayerSlowJumpMultiplier;

		// Token: 0x04004847 RID: 18503
		[GorillaSoundLookup]
		public int[] frozenHandTapIndices;

		// Token: 0x04004848 RID: 18504
		private float fastJumpLimitCached;

		// Token: 0x04004849 RID: 18505
		private float fastJumpMultiplierCached;

		// Token: 0x0400484A RID: 18506
		private float slowJumpLimitCached;

		// Token: 0x0400484B RID: 18507
		private float slowJumpMultiplierCached;

		// Token: 0x0400484C RID: 18508
		private VRRig localVRRig;

		// Token: 0x0400484D RID: 18509
		private int hapticStrength;

		// Token: 0x0400484E RID: 18510
		private List<NetPlayer> currentRoundInfectedPlayers = new List<NetPlayer>(10);

		// Token: 0x0400484F RID: 18511
		private List<NetPlayer> lastRoundInfectedPlayers = new List<NetPlayer>(10);
	}
}
