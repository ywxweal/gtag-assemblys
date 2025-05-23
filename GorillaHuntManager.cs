using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using GorillaGameModes;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000613 RID: 1555
public class GorillaHuntManager : GorillaGameManager
{
	// Token: 0x06002696 RID: 9878 RVA: 0x000121FB File Offset: 0x000103FB
	public override GameModeType GameType()
	{
		return GameModeType.Hunt;
	}

	// Token: 0x06002697 RID: 9879 RVA: 0x000BF0E3 File Offset: 0x000BD2E3
	public override string GameModeName()
	{
		return "HUNT";
	}

	// Token: 0x06002698 RID: 9880 RVA: 0x000BF0EA File Offset: 0x000BD2EA
	public override void AddFusionDataBehaviour(NetworkObject behaviour)
	{
		behaviour.AddBehaviour<HuntGameModeData>();
	}

	// Token: 0x06002699 RID: 9881 RVA: 0x000BF0F4 File Offset: 0x000BD2F4
	public override void StartPlaying()
	{
		base.StartPlaying();
		GorillaTagger.Instance.offlineVRRig.huntComputer.SetActive(true);
		if (NetworkSystem.Instance.IsMasterClient)
		{
			for (int i = 0; i < this.currentHunted.Count; i++)
			{
				this.tempPlayer = this.currentHunted[i];
				if (this.tempPlayer == null || !this.tempPlayer.InRoom())
				{
					this.currentHunted.RemoveAt(i);
					i--;
				}
			}
			for (int i = 0; i < this.currentTarget.Count; i++)
			{
				this.tempPlayer = this.currentTarget[i];
				if (this.tempPlayer == null || !this.tempPlayer.InRoom())
				{
					this.currentTarget.RemoveAt(i);
					i--;
				}
			}
			this.UpdateState();
		}
	}

	// Token: 0x0600269A RID: 9882 RVA: 0x000BF1CB File Offset: 0x000BD3CB
	public override void StopPlaying()
	{
		base.StopPlaying();
		GorillaTagger.Instance.offlineVRRig.huntComputer.SetActive(false);
		base.StopAllCoroutines();
	}

	// Token: 0x0600269B RID: 9883 RVA: 0x000BF1F0 File Offset: 0x000BD3F0
	public override void Reset()
	{
		base.Reset();
		this.currentHunted.Clear();
		this.currentTarget.Clear();
		for (int i = 0; i < this.currentHuntedArray.Length; i++)
		{
			this.currentHuntedArray[i] = -1;
			this.currentTargetArray[i] = -1;
		}
		this.huntStarted = false;
		this.waitingToStartNextHuntGame = false;
		this.inStartCountdown = false;
		this.timeHuntGameEnded = 0.0;
		this.countDownTime = 0;
		this.timeLastSlowTagged = 0f;
	}

	// Token: 0x0600269C RID: 9884 RVA: 0x000BF274 File Offset: 0x000BD474
	public void UpdateState()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			if (NetworkSystem.Instance.RoomPlayerCount <= 3)
			{
				this.CleanUpHunt();
				this.huntStarted = false;
				this.waitingToStartNextHuntGame = false;
				this.iterator1 = 0;
				while (this.iterator1 < RoomSystem.PlayersInRoom.Count)
				{
					RoomSystem.SendSoundEffectToPlayer(0, 0.25f, RoomSystem.PlayersInRoom[this.iterator1], false);
					this.iterator1++;
				}
				return;
			}
			if (NetworkSystem.Instance.RoomPlayerCount > 3 && !this.huntStarted && !this.waitingToStartNextHuntGame && !this.inStartCountdown)
			{
				Utils.Log("<color=red> there are enough players</color>", this);
				base.StartCoroutine(this.StartHuntCountdown());
				return;
			}
			this.UpdateHuntState();
		}
	}

	// Token: 0x0600269D RID: 9885 RVA: 0x000BF33B File Offset: 0x000BD53B
	public void CleanUpHunt()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			this.currentHunted.Clear();
			this.currentTarget.Clear();
		}
	}

	// Token: 0x0600269E RID: 9886 RVA: 0x000BF35F File Offset: 0x000BD55F
	public IEnumerator StartHuntCountdown()
	{
		if (NetworkSystem.Instance.IsMasterClient && !this.inStartCountdown)
		{
			this.inStartCountdown = true;
			this.countDownTime = 5;
			this.CleanUpHunt();
			while (this.countDownTime > 0)
			{
				yield return new WaitForSeconds(1f);
				this.countDownTime--;
			}
			this.StartHunt();
		}
		yield return null;
		yield break;
	}

	// Token: 0x0600269F RID: 9887 RVA: 0x000BF370 File Offset: 0x000BD570
	public void StartHunt()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			this.huntStarted = true;
			this.waitingToStartNextHuntGame = false;
			this.countDownTime = 0;
			this.inStartCountdown = false;
			this.CleanUpHunt();
			this.iterator1 = 0;
			while (this.iterator1 < NetworkSystem.Instance.AllNetPlayers.Count<NetPlayer>())
			{
				if (this.currentTarget.Count < 10)
				{
					this.currentTarget.Add(NetworkSystem.Instance.AllNetPlayers[this.iterator1]);
					RoomSystem.SendSoundEffectToPlayer(0, 0.25f, NetworkSystem.Instance.AllNetPlayers[this.iterator1], false);
				}
				this.iterator1++;
			}
			this.RandomizePlayerList(ref this.currentTarget);
		}
	}

	// Token: 0x060026A0 RID: 9888 RVA: 0x000BF430 File Offset: 0x000BD630
	public void RandomizePlayerList(ref List<NetPlayer> listToRandomize)
	{
		for (int i = 0; i < listToRandomize.Count - 1; i++)
		{
			this.tempRandIndex = Random.Range(i, listToRandomize.Count);
			this.tempRandPlayer = listToRandomize[i];
			listToRandomize[i] = listToRandomize[this.tempRandIndex];
			listToRandomize[this.tempRandIndex] = this.tempRandPlayer;
		}
	}

	// Token: 0x060026A1 RID: 9889 RVA: 0x000BF49A File Offset: 0x000BD69A
	public IEnumerator HuntEnd()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			while ((double)Time.time < this.timeHuntGameEnded + (double)this.tagCoolDown)
			{
				yield return new WaitForSeconds(0.1f);
			}
			if (this.waitingToStartNextHuntGame)
			{
				base.StartCoroutine(this.StartHuntCountdown());
			}
			yield return null;
		}
		yield return null;
		yield break;
	}

	// Token: 0x060026A2 RID: 9890 RVA: 0x000BF4AC File Offset: 0x000BD6AC
	public void UpdateHuntState()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			this.notHuntedCount = 0;
			foreach (NetPlayer netPlayer in RoomSystem.PlayersInRoom)
			{
				if (this.currentTarget.Contains(netPlayer) && !this.currentHunted.Contains(netPlayer))
				{
					this.notHuntedCount++;
				}
			}
			if (this.notHuntedCount <= 2 && this.huntStarted)
			{
				this.EndHuntGame();
			}
		}
	}

	// Token: 0x060026A3 RID: 9891 RVA: 0x000BF54C File Offset: 0x000BD74C
	private void EndHuntGame()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			foreach (NetPlayer netPlayer in RoomSystem.PlayersInRoom)
			{
				RoomSystem.SendStatusEffectToPlayer(RoomSystem.StatusEffects.TaggedTime, netPlayer);
				RoomSystem.SendSoundEffectToPlayer(2, 0.25f, netPlayer, false);
			}
			this.huntStarted = false;
			this.timeHuntGameEnded = (double)Time.time;
			this.waitingToStartNextHuntGame = true;
			base.StartCoroutine(this.HuntEnd());
		}
	}

	// Token: 0x060026A4 RID: 9892 RVA: 0x000BF5E0 File Offset: 0x000BD7E0
	public override bool LocalCanTag(NetPlayer myPlayer, NetPlayer otherPlayer)
	{
		if (this.waitingToStartNextHuntGame || this.countDownTime > 0 || GorillaTagger.Instance.currentStatus == GorillaTagger.StatusEffect.Frozen)
		{
			return false;
		}
		if (this.currentHunted.Contains(myPlayer) && !this.currentHunted.Contains(otherPlayer) && Time.time > this.timeLastSlowTagged + 1f)
		{
			this.timeLastSlowTagged = Time.time;
			return true;
		}
		return this.IsTargetOf(myPlayer, otherPlayer);
	}

	// Token: 0x060026A5 RID: 9893 RVA: 0x000BF658 File Offset: 0x000BD858
	public override void ReportTag(NetPlayer taggedPlayer, NetPlayer taggingPlayer)
	{
		if (NetworkSystem.Instance.IsMasterClient && !this.waitingToStartNextHuntGame)
		{
			if ((this.currentHunted.Contains(taggingPlayer) || !this.currentTarget.Contains(taggingPlayer)) && !this.currentHunted.Contains(taggedPlayer) && this.currentTarget.Contains(taggedPlayer))
			{
				RoomSystem.SendStatusEffectToPlayer(RoomSystem.StatusEffects.SetSlowedTime, taggedPlayer);
				RoomSystem.SendSoundEffectOnOther(5, 0.125f, taggedPlayer, false);
				return;
			}
			if (this.IsTargetOf(taggingPlayer, taggedPlayer))
			{
				RoomSystem.SendStatusEffectToPlayer(RoomSystem.StatusEffects.TaggedTime, taggedPlayer);
				RoomSystem.SendSoundEffectOnOther(0, 0.25f, taggedPlayer, false);
				this.currentHunted.Add(taggedPlayer);
				this.UpdateHuntState();
			}
		}
	}

	// Token: 0x060026A6 RID: 9894 RVA: 0x000BF6FC File Offset: 0x000BD8FC
	public bool IsTargetOf(NetPlayer huntingPlayer, NetPlayer huntedPlayer)
	{
		return !this.currentHunted.Contains(huntingPlayer) && !this.currentHunted.Contains(huntedPlayer) && this.currentTarget.Contains(huntingPlayer) && this.currentTarget.Contains(huntedPlayer) && huntedPlayer == this.GetTargetOf(huntingPlayer);
	}

	// Token: 0x060026A7 RID: 9895 RVA: 0x000BF750 File Offset: 0x000BD950
	public NetPlayer GetTargetOf(NetPlayer netPlayer)
	{
		if (this.currentHunted.Contains(netPlayer) || !this.currentTarget.Contains(netPlayer))
		{
			return null;
		}
		this.tempTargetIndex = this.currentTarget.IndexOf(netPlayer);
		for (int num = (this.tempTargetIndex + 1) % this.currentTarget.Count; num != this.tempTargetIndex; num = (num + 1) % this.currentTarget.Count)
		{
			if (this.currentTarget[num] == netPlayer)
			{
				return null;
			}
			if (!this.currentHunted.Contains(this.currentTarget[num]) && this.currentTarget[num] != null)
			{
				return this.currentTarget[num];
			}
		}
		return null;
	}

	// Token: 0x060026A8 RID: 9896 RVA: 0x000BF804 File Offset: 0x000BDA04
	public override void HitPlayer(NetPlayer taggedPlayer)
	{
		if (NetworkSystem.Instance.IsMasterClient && !this.waitingToStartNextHuntGame && !this.currentHunted.Contains(taggedPlayer) && this.currentTarget.Contains(taggedPlayer))
		{
			RoomSystem.SendStatusEffectToPlayer(RoomSystem.StatusEffects.TaggedTime, taggedPlayer);
			RoomSystem.SendSoundEffectOnOther(0, 0.25f, taggedPlayer, false);
			this.currentHunted.Add(taggedPlayer);
			this.UpdateHuntState();
		}
	}

	// Token: 0x060026A9 RID: 9897 RVA: 0x000BF867 File Offset: 0x000BDA67
	public override bool CanAffectPlayer(NetPlayer player, bool thisFrame)
	{
		return !this.waitingToStartNextHuntGame && !this.currentHunted.Contains(player) && this.currentTarget.Contains(player);
	}

	// Token: 0x060026AA RID: 9898 RVA: 0x000BF88D File Offset: 0x000BDA8D
	public override void OnPlayerEnteredRoom(NetPlayer newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		bool isMasterClient = NetworkSystem.Instance.IsMasterClient;
	}

	// Token: 0x060026AB RID: 9899 RVA: 0x000BF8A1 File Offset: 0x000BDAA1
	public override void NewVRRig(NetPlayer player, int vrrigPhotonViewID, bool didTutorial)
	{
		base.NewVRRig(player, vrrigPhotonViewID, didTutorial);
		if (NetworkSystem.Instance.IsMasterClient)
		{
			if (!this.waitingToStartNextHuntGame && this.huntStarted)
			{
				this.currentHunted.Add(player);
			}
			this.UpdateState();
		}
	}

	// Token: 0x060026AC RID: 9900 RVA: 0x000BF8DC File Offset: 0x000BDADC
	public override void OnPlayerLeftRoom(NetPlayer otherPlayer)
	{
		base.OnPlayerLeftRoom(otherPlayer);
		if (NetworkSystem.Instance.IsMasterClient)
		{
			if (this.currentTarget.Contains(otherPlayer))
			{
				this.currentTarget.Remove(otherPlayer);
			}
			if (this.currentHunted.Contains(otherPlayer))
			{
				this.currentHunted.Remove(otherPlayer);
			}
			this.UpdateState();
		}
	}

	// Token: 0x060026AD RID: 9901 RVA: 0x000BF938 File Offset: 0x000BDB38
	private void CopyHuntDataListToArray()
	{
		this.copyListToArrayIndex = 0;
		while (this.copyListToArrayIndex < 10)
		{
			this.currentHuntedArray[this.copyListToArrayIndex] = -1;
			this.currentTargetArray[this.copyListToArrayIndex] = -1;
			this.copyListToArrayIndex++;
		}
		this.copyListToArrayIndex = this.currentHunted.Count - 1;
		while (this.copyListToArrayIndex >= 0)
		{
			if (this.currentHunted[this.copyListToArrayIndex] == null)
			{
				this.currentHunted.RemoveAt(this.copyListToArrayIndex);
			}
			this.copyListToArrayIndex--;
		}
		this.copyListToArrayIndex = this.currentTarget.Count - 1;
		while (this.copyListToArrayIndex >= 0)
		{
			if (this.currentTarget[this.copyListToArrayIndex] == null)
			{
				this.currentTarget.RemoveAt(this.copyListToArrayIndex);
			}
			this.copyListToArrayIndex--;
		}
		this.copyListToArrayIndex = 0;
		while (this.copyListToArrayIndex < this.currentHunted.Count)
		{
			this.currentHuntedArray[this.copyListToArrayIndex] = this.currentHunted[this.copyListToArrayIndex].ActorNumber;
			this.copyListToArrayIndex++;
		}
		this.copyListToArrayIndex = 0;
		while (this.copyListToArrayIndex < this.currentTarget.Count)
		{
			this.currentTargetArray[this.copyListToArrayIndex] = this.currentTarget[this.copyListToArrayIndex].ActorNumber;
			this.copyListToArrayIndex++;
		}
	}

	// Token: 0x060026AE RID: 9902 RVA: 0x000BFABC File Offset: 0x000BDCBC
	private void CopyHuntDataArrayToList()
	{
		this.currentTarget.Clear();
		this.copyArrayToListIndex = 0;
		while (this.copyArrayToListIndex < this.currentTargetArray.Length)
		{
			if (this.currentTargetArray[this.copyArrayToListIndex] != -1)
			{
				this.tempPlayer = NetworkSystem.Instance.GetPlayer(this.currentTargetArray[this.copyArrayToListIndex]);
				if (this.tempPlayer != null)
				{
					this.currentTarget.Add(this.tempPlayer);
				}
			}
			this.copyArrayToListIndex++;
		}
		this.currentHunted.Clear();
		this.copyArrayToListIndex = 0;
		while (this.copyArrayToListIndex < this.currentHuntedArray.Length)
		{
			if (this.currentHuntedArray[this.copyArrayToListIndex] != -1)
			{
				this.tempPlayer = NetworkSystem.Instance.GetPlayer(this.currentHuntedArray[this.copyArrayToListIndex]);
				if (this.tempPlayer != null)
				{
					this.currentHunted.Add(this.tempPlayer);
				}
			}
			this.copyArrayToListIndex++;
		}
	}

	// Token: 0x060026AF RID: 9903 RVA: 0x000BFBB9 File Offset: 0x000BDDB9
	public override void OnMasterClientSwitched(Player newMasterClient)
	{
		base.OnMasterClientSwitched(newMasterClient);
		if (NetworkSystem.Instance.IsMasterClient)
		{
			this.CopyRoomDataToLocalData();
			this.UpdateState();
		}
	}

	// Token: 0x060026B0 RID: 9904 RVA: 0x000BFBDA File Offset: 0x000BDDDA
	public void CopyRoomDataToLocalData()
	{
		this.waitingToStartNextHuntGame = false;
		this.UpdateHuntState();
	}

	// Token: 0x060026B1 RID: 9905 RVA: 0x000BFBEC File Offset: 0x000BDDEC
	public override void OnSerializeRead(object newData)
	{
		HuntData huntData = (HuntData)newData;
		huntData.currentHuntedArray.CopyTo(this.currentHuntedArray, true);
		huntData.currentTargetArray.CopyTo(this.currentTargetArray, true);
		this.huntStarted = huntData.huntStarted;
		this.waitingToStartNextHuntGame = huntData.waitingToStartNextHuntGame;
		this.countDownTime = huntData.countDownTime;
		this.CopyHuntDataArrayToList();
	}

	// Token: 0x060026B2 RID: 9906 RVA: 0x000BFC60 File Offset: 0x000BDE60
	public override object OnSerializeWrite()
	{
		this.CopyHuntDataListToArray();
		HuntData huntData = default(HuntData);
		huntData.currentHuntedArray.CopyFrom(this.currentHuntedArray, 0, this.currentHuntedArray.Length);
		huntData.currentTargetArray.CopyFrom(this.currentTargetArray, 0, this.currentTargetArray.Length);
		huntData.huntStarted = this.huntStarted;
		huntData.waitingToStartNextHuntGame = this.waitingToStartNextHuntGame;
		huntData.countDownTime = this.countDownTime;
		return huntData;
	}

	// Token: 0x060026B3 RID: 9907 RVA: 0x000BFCF0 File Offset: 0x000BDEF0
	public override void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
	{
		this.CopyHuntDataListToArray();
		stream.SendNext(this.currentHuntedArray[0]);
		stream.SendNext(this.currentHuntedArray[1]);
		stream.SendNext(this.currentHuntedArray[2]);
		stream.SendNext(this.currentHuntedArray[3]);
		stream.SendNext(this.currentHuntedArray[4]);
		stream.SendNext(this.currentHuntedArray[5]);
		stream.SendNext(this.currentHuntedArray[6]);
		stream.SendNext(this.currentHuntedArray[7]);
		stream.SendNext(this.currentHuntedArray[8]);
		stream.SendNext(this.currentHuntedArray[9]);
		stream.SendNext(this.currentTargetArray[0]);
		stream.SendNext(this.currentTargetArray[1]);
		stream.SendNext(this.currentTargetArray[2]);
		stream.SendNext(this.currentTargetArray[3]);
		stream.SendNext(this.currentTargetArray[4]);
		stream.SendNext(this.currentTargetArray[5]);
		stream.SendNext(this.currentTargetArray[6]);
		stream.SendNext(this.currentTargetArray[7]);
		stream.SendNext(this.currentTargetArray[8]);
		stream.SendNext(this.currentTargetArray[9]);
		stream.SendNext(this.huntStarted);
		stream.SendNext(this.waitingToStartNextHuntGame);
		stream.SendNext(this.countDownTime);
	}

	// Token: 0x060026B4 RID: 9908 RVA: 0x000BFEB4 File Offset: 0x000BE0B4
	public override void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
	{
		this.currentHuntedArray[0] = (int)stream.ReceiveNext();
		this.currentHuntedArray[1] = (int)stream.ReceiveNext();
		this.currentHuntedArray[2] = (int)stream.ReceiveNext();
		this.currentHuntedArray[3] = (int)stream.ReceiveNext();
		this.currentHuntedArray[4] = (int)stream.ReceiveNext();
		this.currentHuntedArray[5] = (int)stream.ReceiveNext();
		this.currentHuntedArray[6] = (int)stream.ReceiveNext();
		this.currentHuntedArray[7] = (int)stream.ReceiveNext();
		this.currentHuntedArray[8] = (int)stream.ReceiveNext();
		this.currentHuntedArray[9] = (int)stream.ReceiveNext();
		this.currentTargetArray[0] = (int)stream.ReceiveNext();
		this.currentTargetArray[1] = (int)stream.ReceiveNext();
		this.currentTargetArray[2] = (int)stream.ReceiveNext();
		this.currentTargetArray[3] = (int)stream.ReceiveNext();
		this.currentTargetArray[4] = (int)stream.ReceiveNext();
		this.currentTargetArray[5] = (int)stream.ReceiveNext();
		this.currentTargetArray[6] = (int)stream.ReceiveNext();
		this.currentTargetArray[7] = (int)stream.ReceiveNext();
		this.currentTargetArray[8] = (int)stream.ReceiveNext();
		this.currentTargetArray[9] = (int)stream.ReceiveNext();
		this.huntStarted = (bool)stream.ReceiveNext();
		this.waitingToStartNextHuntGame = (bool)stream.ReceiveNext();
		this.countDownTime = (int)stream.ReceiveNext();
		this.CopyHuntDataArrayToList();
	}

	// Token: 0x060026B5 RID: 9909 RVA: 0x000C0078 File Offset: 0x000BE278
	public override int MyMatIndex(NetPlayer forPlayer)
	{
		NetPlayer targetOf = this.GetTargetOf(forPlayer);
		if (this.currentHunted.Contains(forPlayer) || (this.huntStarted && targetOf == null))
		{
			return 3;
		}
		return 0;
	}

	// Token: 0x060026B6 RID: 9910 RVA: 0x000C00AC File Offset: 0x000BE2AC
	public override float[] LocalPlayerSpeed()
	{
		if (this.currentHunted.Contains(NetworkSystem.Instance.LocalPlayer) || (this.huntStarted && this.GetTargetOf(NetworkSystem.Instance.LocalPlayer) == null))
		{
			return new float[] { 8.5f, 1.3f };
		}
		if (GorillaTagger.Instance.currentStatus == GorillaTagger.StatusEffect.Slowed)
		{
			return new float[] { 5.5f, 0.9f };
		}
		return new float[] { 6.5f, 1.1f };
	}

	// Token: 0x060026B7 RID: 9911 RVA: 0x000C013B File Offset: 0x000BE33B
	public override void InfrequentUpdate()
	{
		base.InfrequentUpdate();
	}

	// Token: 0x04002B0B RID: 11019
	public float tagCoolDown = 5f;

	// Token: 0x04002B0C RID: 11020
	public int[] currentHuntedArray = new int[] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

	// Token: 0x04002B0D RID: 11021
	public List<NetPlayer> currentHunted = new List<NetPlayer>(10);

	// Token: 0x04002B0E RID: 11022
	public int[] currentTargetArray = new int[] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

	// Token: 0x04002B0F RID: 11023
	public List<NetPlayer> currentTarget = new List<NetPlayer>(10);

	// Token: 0x04002B10 RID: 11024
	public bool huntStarted;

	// Token: 0x04002B11 RID: 11025
	public bool waitingToStartNextHuntGame;

	// Token: 0x04002B12 RID: 11026
	public bool inStartCountdown;

	// Token: 0x04002B13 RID: 11027
	public int countDownTime;

	// Token: 0x04002B14 RID: 11028
	public double timeHuntGameEnded;

	// Token: 0x04002B15 RID: 11029
	public float timeLastSlowTagged;

	// Token: 0x04002B16 RID: 11030
	public object objRef;

	// Token: 0x04002B17 RID: 11031
	private int iterator1;

	// Token: 0x04002B18 RID: 11032
	private NetPlayer tempRandPlayer;

	// Token: 0x04002B19 RID: 11033
	private int tempRandIndex;

	// Token: 0x04002B1A RID: 11034
	private int notHuntedCount;

	// Token: 0x04002B1B RID: 11035
	private int tempTargetIndex;

	// Token: 0x04002B1C RID: 11036
	private NetPlayer tempPlayer;

	// Token: 0x04002B1D RID: 11037
	private int copyListToArrayIndex;

	// Token: 0x04002B1E RID: 11038
	private int copyArrayToListIndex;
}
