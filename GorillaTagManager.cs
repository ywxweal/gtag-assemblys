using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using GorillaGameModes;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x0200063E RID: 1598
public class GorillaTagManager : GorillaGameManager
{
	// Token: 0x060027F4 RID: 10228 RVA: 0x000C7000 File Offset: 0x000C5200
	public override void StartPlaying()
	{
		base.StartPlaying();
		if (NetworkSystem.Instance.IsMasterClient)
		{
			for (int i = 0; i < this.currentInfected.Count; i++)
			{
				this.tempPlayer = this.currentInfected[i];
				if (this.tempPlayer == null || !this.tempPlayer.InRoom())
				{
					this.currentInfected.RemoveAt(i);
					i--;
				}
			}
			if (this.currentIt != null && !this.currentIt.InRoom())
			{
				this.currentIt = null;
			}
			if (this.lastInfectedPlayer != null && !this.lastInfectedPlayer.InRoom())
			{
				this.lastInfectedPlayer = null;
			}
			this.UpdateState();
		}
	}

	// Token: 0x060027F5 RID: 10229 RVA: 0x000C70AD File Offset: 0x000C52AD
	public override void StopPlaying()
	{
		base.StopPlaying();
		base.StopAllCoroutines();
		this.lastTaggedActorNr.Clear();
	}

	// Token: 0x060027F6 RID: 10230 RVA: 0x000C70C8 File Offset: 0x000C52C8
	public override void Reset()
	{
		base.Reset();
		for (int i = 0; i < this.currentInfectedArray.Length; i++)
		{
			this.currentInfectedArray[i] = -1;
		}
		this.currentInfected.Clear();
		this.lastTag = 0.0;
		this.timeInfectedGameEnded = 0.0;
		this.allInfected = false;
		this.isCurrentlyTag = false;
		this.waitingToStartNextInfectionGame = false;
		this.currentIt = null;
		this.lastInfectedPlayer = null;
	}

	// Token: 0x060027F7 RID: 10231 RVA: 0x000C7144 File Offset: 0x000C5344
	public virtual void UpdateState()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			if (global::GorillaGameModes.GameMode.ParticipatingPlayers.Count < 1)
			{
				this.isCurrentlyTag = true;
				this.ClearInfectionState();
				this.lastInfectedPlayer = null;
				this.currentIt = null;
				return;
			}
			if (this.isCurrentlyTag && this.currentIt == null)
			{
				int num = Random.Range(0, global::GorillaGameModes.GameMode.ParticipatingPlayers.Count);
				this.ChangeCurrentIt(global::GorillaGameModes.GameMode.ParticipatingPlayers[num], false);
				return;
			}
			if (this.isCurrentlyTag && global::GorillaGameModes.GameMode.ParticipatingPlayers.Count >= this.infectedModeThreshold)
			{
				this.SetisCurrentlyTag(false);
				this.ClearInfectionState();
				int num2 = Random.Range(0, global::GorillaGameModes.GameMode.ParticipatingPlayers.Count);
				this.AddInfectedPlayer(global::GorillaGameModes.GameMode.ParticipatingPlayers[num2], true);
				this.lastInfectedPlayer = global::GorillaGameModes.GameMode.ParticipatingPlayers[num2];
				return;
			}
			if (!this.isCurrentlyTag && global::GorillaGameModes.GameMode.ParticipatingPlayers.Count < this.infectedModeThreshold)
			{
				this.ClearInfectionState();
				this.lastInfectedPlayer = null;
				this.SetisCurrentlyTag(true);
				int num3 = Random.Range(0, global::GorillaGameModes.GameMode.ParticipatingPlayers.Count);
				this.ChangeCurrentIt(global::GorillaGameModes.GameMode.ParticipatingPlayers[num3], false);
				return;
			}
			if (!this.isCurrentlyTag && this.currentInfected.Count == 0)
			{
				int num4 = Random.Range(0, global::GorillaGameModes.GameMode.ParticipatingPlayers.Count);
				this.AddInfectedPlayer(global::GorillaGameModes.GameMode.ParticipatingPlayers[num4], true);
				return;
			}
			if (!this.isCurrentlyTag)
			{
				this.UpdateInfectionState();
			}
		}
	}

	// Token: 0x060027F8 RID: 10232 RVA: 0x000C72B2 File Offset: 0x000C54B2
	public override void InfrequentUpdate()
	{
		base.InfrequentUpdate();
		if (NetworkSystem.Instance.IsMasterClient)
		{
			this.UpdateState();
		}
		this.inspectorLocalPlayerSpeed = this.LocalPlayerSpeed();
	}

	// Token: 0x060027F9 RID: 10233 RVA: 0x000C72D8 File Offset: 0x000C54D8
	protected virtual IEnumerator InfectionEnd()
	{
		while ((double)Time.time < this.timeInfectedGameEnded + (double)this.tagCoolDown)
		{
			yield return new WaitForSeconds(0.1f);
		}
		if (!this.isCurrentlyTag && this.waitingToStartNextInfectionGame)
		{
			this.ClearInfectionState();
			global::GorillaGameModes.GameMode.RefreshPlayers();
			List<NetPlayer> participatingPlayers = global::GorillaGameModes.GameMode.ParticipatingPlayers;
			if (participatingPlayers.Count > 0)
			{
				int num = Random.Range(0, participatingPlayers.Count);
				int num2 = 0;
				while (num2 < 10 && participatingPlayers[num] == this.lastInfectedPlayer)
				{
					num = Random.Range(0, participatingPlayers.Count);
					num2++;
				}
				this.AddInfectedPlayer(participatingPlayers[num], true);
				this.lastInfectedPlayer = participatingPlayers[num];
				this.lastTag = (double)Time.time;
			}
		}
		yield return null;
		yield break;
	}

	// Token: 0x060027FA RID: 10234 RVA: 0x000C72E8 File Offset: 0x000C54E8
	public void UpdateInfectionState()
	{
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		this.allInfected = true;
		foreach (NetPlayer netPlayer in global::GorillaGameModes.GameMode.ParticipatingPlayers)
		{
			if (!this.currentInfected.Contains(netPlayer))
			{
				this.allInfected = false;
				break;
			}
		}
		if (!this.isCurrentlyTag && !this.waitingToStartNextInfectionGame && this.allInfected)
		{
			this.EndInfectionGame();
		}
	}

	// Token: 0x060027FB RID: 10235 RVA: 0x000C737C File Offset: 0x000C557C
	public void UpdateTagState(bool withTagFreeze = true)
	{
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		foreach (NetPlayer netPlayer in global::GorillaGameModes.GameMode.ParticipatingPlayers)
		{
			if (this.currentIt == netPlayer)
			{
				if (withTagFreeze)
				{
					RoomSystem.SendStatusEffectToPlayer(RoomSystem.StatusEffects.TaggedTime, netPlayer);
				}
				else
				{
					RoomSystem.SendStatusEffectToPlayer(RoomSystem.StatusEffects.JoinedTaggedTime, netPlayer);
				}
				RoomSystem.SendSoundEffectOnOther(0, 0.25f, netPlayer, false);
				break;
			}
		}
	}

	// Token: 0x060027FC RID: 10236 RVA: 0x000C7400 File Offset: 0x000C5600
	protected void EndInfectionGame()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			foreach (NetPlayer netPlayer in global::GorillaGameModes.GameMode.ParticipatingPlayers)
			{
				RoomSystem.SendSoundEffectToPlayer(2, 0.25f, netPlayer, true);
			}
			PlayerGameEvents.GameModeCompleteRound();
			global::GorillaGameModes.GameMode.BroadcastRoundComplete();
			this.lastTaggedActorNr.Clear();
			this.waitingToStartNextInfectionGame = true;
			this.timeInfectedGameEnded = (double)Time.time;
			base.StartCoroutine(this.InfectionEnd());
		}
	}

	// Token: 0x060027FD RID: 10237 RVA: 0x000C749C File Offset: 0x000C569C
	public override bool LocalCanTag(NetPlayer myPlayer, NetPlayer otherPlayer)
	{
		if (this.isCurrentlyTag)
		{
			return myPlayer == this.currentIt && myPlayer != otherPlayer;
		}
		return this.currentInfected.Contains(myPlayer) && !this.currentInfected.Contains(otherPlayer);
	}

	// Token: 0x060027FE RID: 10238 RVA: 0x000C74D8 File Offset: 0x000C56D8
	public override void LocalTag(NetPlayer taggedPlayer, NetPlayer taggingPlayer, bool bodyHit, bool leftHand)
	{
		if (this.LocalCanTag(NetworkSystem.Instance.LocalPlayer, taggedPlayer) && (double)Time.time > this.lastQuestTagTime + (double)this.tagCoolDown)
		{
			PlayerGameEvents.MiscEvent("GameModeTag");
			this.lastQuestTagTime = (double)Time.time;
			if (!this.isCurrentlyTag)
			{
				PlayerGameEvents.GameModeObjectiveTriggered();
			}
		}
	}

	// Token: 0x060027FF RID: 10239 RVA: 0x000C7534 File Offset: 0x000C5734
	protected float InterpolatedInfectedJumpMultiplier(int infectedCount)
	{
		if (global::GorillaGameModes.GameMode.ParticipatingPlayers.Count < 2)
		{
			return this.fastJumpMultiplier;
		}
		return (this.fastJumpMultiplier - this.slowJumpMultiplier) / (float)(global::GorillaGameModes.GameMode.ParticipatingPlayers.Count - 1) * (float)(global::GorillaGameModes.GameMode.ParticipatingPlayers.Count - infectedCount) + this.slowJumpMultiplier;
	}

	// Token: 0x06002800 RID: 10240 RVA: 0x000C7588 File Offset: 0x000C5788
	protected float InterpolatedInfectedJumpSpeed(int infectedCount)
	{
		if (global::GorillaGameModes.GameMode.ParticipatingPlayers.Count < 2)
		{
			return this.fastJumpLimit;
		}
		return (this.fastJumpLimit - this.slowJumpLimit) / (float)(global::GorillaGameModes.GameMode.ParticipatingPlayers.Count - 1) * (float)(global::GorillaGameModes.GameMode.ParticipatingPlayers.Count - infectedCount) + this.slowJumpLimit;
	}

	// Token: 0x06002801 RID: 10241 RVA: 0x000C75DC File Offset: 0x000C57DC
	protected float InterpolatedNoobJumpMultiplier(int infectedCount)
	{
		if (global::GorillaGameModes.GameMode.ParticipatingPlayers.Count < 2)
		{
			return this.slowJumpMultiplier;
		}
		return (this.fastJumpMultiplier - this.slowJumpMultiplier) / (float)(global::GorillaGameModes.GameMode.ParticipatingPlayers.Count - 1) * (float)(infectedCount - 1) * 0.9f + this.slowJumpMultiplier;
	}

	// Token: 0x06002802 RID: 10242 RVA: 0x000C762C File Offset: 0x000C582C
	protected float InterpolatedNoobJumpSpeed(int infectedCount)
	{
		if (global::GorillaGameModes.GameMode.ParticipatingPlayers.Count < 2)
		{
			return this.slowJumpLimit;
		}
		return (this.fastJumpLimit - this.fastJumpLimit) / (float)(global::GorillaGameModes.GameMode.ParticipatingPlayers.Count - 1) * (float)(infectedCount - 1) * 0.9f + this.slowJumpLimit;
	}

	// Token: 0x06002803 RID: 10243 RVA: 0x000C767C File Offset: 0x000C587C
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
			this.taggedRig.SetTaggedBy(this.taggingRig);
			if (this.isCurrentlyTag)
			{
				if (taggingPlayer == this.currentIt && taggingPlayer != taggedPlayer && (double)Time.time > this.lastTag + (double)this.tagCoolDown)
				{
					base.AddLastTagged(taggedPlayer, taggingPlayer);
					this.ChangeCurrentIt(taggedPlayer, true);
					this.lastTag = (double)Time.time;
					return;
				}
			}
			else if (this.currentInfected.Contains(taggingPlayer) && !this.currentInfected.Contains(taggedPlayer) && (double)Time.time > this.lastTag + (double)this.tagCoolDown)
			{
				if (!this.taggingRig.IsPositionInRange(this.taggedRig.transform.position, 6f) && !this.taggingRig.CheckTagDistanceRollback(this.taggedRig, 6f, 0.2f))
				{
					GorillaNot.instance.SendReport("extremely far tag", taggingPlayer.UserId, taggingPlayer.NickName);
					return;
				}
				base.AddLastTagged(taggedPlayer, taggingPlayer);
				this.AddInfectedPlayer(taggedPlayer, true);
				int count = this.currentInfected.Count;
			}
		}
	}

	// Token: 0x06002804 RID: 10244 RVA: 0x000C77E4 File Offset: 0x000C59E4
	public override void HitPlayer(NetPlayer taggedPlayer)
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			this.taggedRig = this.FindPlayerVRRig(taggedPlayer);
			if (this.taggedRig == null || this.waitingToStartNextInfectionGame || (double)Time.time < this.timeInfectedGameEnded + (double)(2f * this.tagCoolDown))
			{
				return;
			}
			if (this.isCurrentlyTag)
			{
				base.AddLastTagged(taggedPlayer, taggedPlayer);
				this.ChangeCurrentIt(taggedPlayer, false);
				return;
			}
			if (!this.currentInfected.Contains(taggedPlayer))
			{
				base.AddLastTagged(taggedPlayer, taggedPlayer);
				this.AddInfectedPlayer(taggedPlayer, false);
				int count = this.currentInfected.Count;
			}
		}
	}

	// Token: 0x06002805 RID: 10245 RVA: 0x000C7884 File Offset: 0x000C5A84
	public override bool CanAffectPlayer(NetPlayer player, bool thisFrame)
	{
		if (this.isCurrentlyTag)
		{
			return this.currentIt != player && thisFrame;
		}
		return !this.waitingToStartNextInfectionGame && (double)Time.time >= this.timeInfectedGameEnded + (double)(2f * this.tagCoolDown) && !this.currentInfected.Contains(player);
	}

	// Token: 0x06002806 RID: 10246 RVA: 0x000C78DD File Offset: 0x000C5ADD
	public bool IsInfected(NetPlayer player)
	{
		if (this.isCurrentlyTag)
		{
			return this.currentIt == player;
		}
		return this.currentInfected.Contains(player);
	}

	// Token: 0x06002807 RID: 10247 RVA: 0x000C78FD File Offset: 0x000C5AFD
	public override void NewVRRig(NetPlayer player, int vrrigPhotonViewID, bool didTutorial)
	{
		base.NewVRRig(player, vrrigPhotonViewID, didTutorial);
		if (NetworkSystem.Instance.IsMasterClient)
		{
			bool flag = this.isCurrentlyTag;
			this.UpdateState();
			if (!flag && !this.isCurrentlyTag)
			{
				if (didTutorial)
				{
					this.AddInfectedPlayer(player, false);
				}
				this.UpdateInfectionState();
			}
		}
	}

	// Token: 0x06002808 RID: 10248 RVA: 0x000C793C File Offset: 0x000C5B3C
	public override void OnPlayerLeftRoom(NetPlayer otherPlayer)
	{
		base.OnPlayerLeftRoom(otherPlayer);
		if (NetworkSystem.Instance.IsMasterClient)
		{
			if (this.isCurrentlyTag && ((otherPlayer != null && otherPlayer == this.currentIt) || this.currentIt.ActorNumber == otherPlayer.ActorNumber))
			{
				if (global::GorillaGameModes.GameMode.ParticipatingPlayers.Count > 0)
				{
					int num = Random.Range(0, global::GorillaGameModes.GameMode.ParticipatingPlayers.Count);
					this.ChangeCurrentIt(global::GorillaGameModes.GameMode.ParticipatingPlayers[num], false);
				}
			}
			else if (!this.isCurrentlyTag && global::GorillaGameModes.GameMode.ParticipatingPlayers.Count >= this.infectedModeThreshold)
			{
				while (this.currentInfected.Contains(otherPlayer))
				{
					this.currentInfected.Remove(otherPlayer);
				}
				this.UpdateInfectionState();
			}
			this.UpdateState();
		}
	}

	// Token: 0x06002809 RID: 10249 RVA: 0x000C79FC File Offset: 0x000C5BFC
	private void CopyInfectedListToArray()
	{
		this.iterator1 = 0;
		while (this.iterator1 < this.currentInfectedArray.Length)
		{
			this.currentInfectedArray[this.iterator1] = -1;
			this.iterator1++;
		}
		this.iterator1 = this.currentInfected.Count - 1;
		while (this.iterator1 >= 0)
		{
			if (this.currentInfected[this.iterator1] == null)
			{
				this.currentInfected.RemoveAt(this.iterator1);
			}
			this.iterator1--;
		}
		this.iterator1 = 0;
		while (this.iterator1 < this.currentInfected.Count)
		{
			this.currentInfectedArray[this.iterator1] = this.currentInfected[this.iterator1].ActorNumber;
			this.iterator1++;
		}
	}

	// Token: 0x0600280A RID: 10250 RVA: 0x000C7ADC File Offset: 0x000C5CDC
	private void CopyInfectedArrayToList()
	{
		this.currentInfected.Clear();
		this.iterator1 = 0;
		while (this.iterator1 < this.currentInfectedArray.Length)
		{
			if (this.currentInfectedArray[this.iterator1] != -1)
			{
				this.tempPlayer = NetworkSystem.Instance.GetPlayer(this.currentInfectedArray[this.iterator1]);
				if (this.tempPlayer != null)
				{
					this.currentInfected.Add(this.tempPlayer);
				}
			}
			this.iterator1++;
		}
	}

	// Token: 0x0600280B RID: 10251 RVA: 0x000C7B61 File Offset: 0x000C5D61
	public void ChangeCurrentIt(NetPlayer newCurrentIt, bool withTagFreeze = true)
	{
		this.lastTag = (double)Time.time;
		this.currentIt = newCurrentIt;
		this.UpdateTagState(withTagFreeze);
	}

	// Token: 0x0600280C RID: 10252 RVA: 0x000C7B7D File Offset: 0x000C5D7D
	public void SetisCurrentlyTag(bool newTagSetting)
	{
		if (newTagSetting)
		{
			this.isCurrentlyTag = true;
		}
		else
		{
			this.isCurrentlyTag = false;
		}
		RoomSystem.SendSoundEffectAll(2, 0.25f, false);
	}

	// Token: 0x0600280D RID: 10253 RVA: 0x000C7B9E File Offset: 0x000C5D9E
	public void AddInfectedPlayer(NetPlayer infectedPlayer, bool withTagStop = true)
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
			this.UpdateInfectionState();
		}
	}

	// Token: 0x0600280E RID: 10254 RVA: 0x000C7BDE File Offset: 0x000C5DDE
	public void ClearInfectionState()
	{
		this.currentInfected.Clear();
		this.waitingToStartNextInfectionGame = false;
	}

	// Token: 0x0600280F RID: 10255 RVA: 0x000C7BF2 File Offset: 0x000C5DF2
	public override void OnMasterClientSwitched(Player newMasterClient)
	{
		base.OnMasterClientSwitched(newMasterClient);
		if (NetworkSystem.Instance.IsMasterClient)
		{
			this.CopyRoomDataToLocalData();
			this.UpdateState();
		}
	}

	// Token: 0x06002810 RID: 10256 RVA: 0x000C7C13 File Offset: 0x000C5E13
	public void CopyRoomDataToLocalData()
	{
		this.lastTag = 0.0;
		this.timeInfectedGameEnded = 0.0;
		this.waitingToStartNextInfectionGame = false;
		if (this.isCurrentlyTag)
		{
			this.UpdateTagState(true);
			return;
		}
		this.UpdateInfectionState();
	}

	// Token: 0x06002811 RID: 10257 RVA: 0x000C7C50 File Offset: 0x000C5E50
	public override void OnSerializeRead(object newData)
	{
		TagData tagData = (TagData)newData;
		this.isCurrentlyTag = tagData.isCurrentlyTag;
		this.tempItInt = tagData.currentItID;
		this.currentIt = ((this.tempItInt != -1) ? NetworkSystem.Instance.GetPlayer(this.tempItInt) : null);
		tagData.infectedPlayerList.CopyTo(this.currentInfectedArray, true);
		this.CopyInfectedArrayToList();
	}

	// Token: 0x06002812 RID: 10258 RVA: 0x000C7CC0 File Offset: 0x000C5EC0
	public override object OnSerializeWrite()
	{
		this.CopyInfectedListToArray();
		TagData tagData = default(TagData);
		tagData.isCurrentlyTag = this.isCurrentlyTag;
		tagData.currentItID = ((this.currentIt != null) ? this.currentIt.ActorNumber : (-1));
		tagData.infectedPlayerList.CopyFrom(this.currentInfectedArray, 0, this.currentInfectedArray.Length);
		return tagData;
	}

	// Token: 0x06002813 RID: 10259 RVA: 0x000C7D30 File Offset: 0x000C5F30
	public override void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
	{
		this.CopyInfectedListToArray();
		stream.SendNext(this.isCurrentlyTag);
		stream.SendNext((this.currentIt != null) ? this.currentIt.ActorNumber : (-1));
		stream.SendNext(this.currentInfectedArray[0]);
		stream.SendNext(this.currentInfectedArray[1]);
		stream.SendNext(this.currentInfectedArray[2]);
		stream.SendNext(this.currentInfectedArray[3]);
		stream.SendNext(this.currentInfectedArray[4]);
		stream.SendNext(this.currentInfectedArray[5]);
		stream.SendNext(this.currentInfectedArray[6]);
		stream.SendNext(this.currentInfectedArray[7]);
		stream.SendNext(this.currentInfectedArray[8]);
		stream.SendNext(this.currentInfectedArray[9]);
		base.WriteLastTagged(stream);
	}

	// Token: 0x06002814 RID: 10260 RVA: 0x000C7E3C File Offset: 0x000C603C
	public override void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
	{
		NetworkSystem.Instance.GetPlayer(info.Sender);
		this.isCurrentlyTag = (bool)stream.ReceiveNext();
		this.tempItInt = (int)stream.ReceiveNext();
		this.currentIt = ((this.tempItInt != -1) ? NetworkSystem.Instance.GetPlayer(this.tempItInt) : null);
		this.currentInfectedArray[0] = (int)stream.ReceiveNext();
		this.currentInfectedArray[1] = (int)stream.ReceiveNext();
		this.currentInfectedArray[2] = (int)stream.ReceiveNext();
		this.currentInfectedArray[3] = (int)stream.ReceiveNext();
		this.currentInfectedArray[4] = (int)stream.ReceiveNext();
		this.currentInfectedArray[5] = (int)stream.ReceiveNext();
		this.currentInfectedArray[6] = (int)stream.ReceiveNext();
		this.currentInfectedArray[7] = (int)stream.ReceiveNext();
		this.currentInfectedArray[8] = (int)stream.ReceiveNext();
		this.currentInfectedArray[9] = (int)stream.ReceiveNext();
		base.ReadLastTagged(stream);
		this.CopyInfectedArrayToList();
	}

	// Token: 0x06002815 RID: 10261 RVA: 0x00047642 File Offset: 0x00045842
	public override GameModeType GameType()
	{
		return GameModeType.Infection;
	}

	// Token: 0x06002816 RID: 10262 RVA: 0x000C7F6A File Offset: 0x000C616A
	public override void AddFusionDataBehaviour(NetworkObject netObject)
	{
		netObject.AddBehaviour<TagGameModeData>();
	}

	// Token: 0x06002817 RID: 10263 RVA: 0x000C7F73 File Offset: 0x000C6173
	public override string GameModeName()
	{
		return "INFECTION";
	}

	// Token: 0x06002818 RID: 10264 RVA: 0x000C7F7A File Offset: 0x000C617A
	public override int MyMatIndex(NetPlayer forPlayer)
	{
		if (this.isCurrentlyTag && forPlayer == this.currentIt)
		{
			return 1;
		}
		if (this.currentInfected.Contains(forPlayer))
		{
			return 2;
		}
		return 0;
	}

	// Token: 0x06002819 RID: 10265 RVA: 0x000C7FA0 File Offset: 0x000C61A0
	public override float[] LocalPlayerSpeed()
	{
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
			if (this.currentInfected.Contains(NetworkSystem.Instance.LocalPlayer))
			{
				this.playerSpeed[0] = this.InterpolatedInfectedJumpSpeed(this.currentInfected.Count);
				this.playerSpeed[1] = this.InterpolatedInfectedJumpMultiplier(this.currentInfected.Count);
				return this.playerSpeed;
			}
			this.playerSpeed[0] = this.InterpolatedNoobJumpSpeed(this.currentInfected.Count);
			this.playerSpeed[1] = this.InterpolatedNoobJumpMultiplier(this.currentInfected.Count);
			return this.playerSpeed;
		}
	}

	// Token: 0x04002CBD RID: 11453
	public float tagCoolDown = 5f;

	// Token: 0x04002CBE RID: 11454
	public int infectedModeThreshold = 4;

	// Token: 0x04002CBF RID: 11455
	public const byte ReportTagEvent = 1;

	// Token: 0x04002CC0 RID: 11456
	public const byte ReportInfectionTagEvent = 2;

	// Token: 0x04002CC1 RID: 11457
	public List<NetPlayer> currentInfected = new List<NetPlayer>(10);

	// Token: 0x04002CC2 RID: 11458
	public int[] currentInfectedArray = new int[] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

	// Token: 0x04002CC3 RID: 11459
	public NetPlayer currentIt;

	// Token: 0x04002CC4 RID: 11460
	public NetPlayer lastInfectedPlayer;

	// Token: 0x04002CC5 RID: 11461
	public double lastTag;

	// Token: 0x04002CC6 RID: 11462
	public double timeInfectedGameEnded;

	// Token: 0x04002CC7 RID: 11463
	public bool waitingToStartNextInfectionGame;

	// Token: 0x04002CC8 RID: 11464
	public bool isCurrentlyTag;

	// Token: 0x04002CC9 RID: 11465
	private int tempItInt;

	// Token: 0x04002CCA RID: 11466
	private int iterator1;

	// Token: 0x04002CCB RID: 11467
	private NetPlayer tempPlayer;

	// Token: 0x04002CCC RID: 11468
	private bool allInfected;

	// Token: 0x04002CCD RID: 11469
	public float[] inspectorLocalPlayerSpeed;

	// Token: 0x04002CCE RID: 11470
	private protected VRRig taggingRig;

	// Token: 0x04002CCF RID: 11471
	private protected VRRig taggedRig;

	// Token: 0x04002CD0 RID: 11472
	private NetPlayer lastTaggedPlayer;

	// Token: 0x04002CD1 RID: 11473
	private double lastQuestTagTime;
}
