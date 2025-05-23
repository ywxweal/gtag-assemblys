using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusion;
using GorillaExtensions;
using GorillaGameModes;
using GorillaNetworking;
using GorillaTag;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Scripting;

namespace GorillaTagScripts
{
	// Token: 0x02000B0A RID: 2826
	public class FriendshipGroupDetection : NetworkSceneObject
	{
		// Token: 0x170006BF RID: 1727
		// (get) Token: 0x06004537 RID: 17719 RVA: 0x00147DF0 File Offset: 0x00145FF0
		// (set) Token: 0x06004538 RID: 17720 RVA: 0x00147DF7 File Offset: 0x00145FF7
		public static FriendshipGroupDetection Instance { get; private set; }

		// Token: 0x170006C0 RID: 1728
		// (get) Token: 0x06004539 RID: 17721 RVA: 0x00147DFF File Offset: 0x00145FFF
		// (set) Token: 0x0600453A RID: 17722 RVA: 0x00147E07 File Offset: 0x00146007
		public List<Color> myBeadColors { get; private set; } = new List<Color>();

		// Token: 0x170006C1 RID: 1729
		// (get) Token: 0x0600453B RID: 17723 RVA: 0x00147E10 File Offset: 0x00146010
		// (set) Token: 0x0600453C RID: 17724 RVA: 0x00147E18 File Offset: 0x00146018
		public Color myBraceletColor { get; private set; }

		// Token: 0x170006C2 RID: 1730
		// (get) Token: 0x0600453D RID: 17725 RVA: 0x00147E21 File Offset: 0x00146021
		// (set) Token: 0x0600453E RID: 17726 RVA: 0x00147E29 File Offset: 0x00146029
		public int MyBraceletSelfIndex { get; private set; }

		// Token: 0x170006C3 RID: 1731
		// (get) Token: 0x0600453F RID: 17727 RVA: 0x00147E32 File Offset: 0x00146032
		public List<string> PartyMemberIDs
		{
			get
			{
				return this.myPartyMemberIDs;
			}
		}

		// Token: 0x170006C4 RID: 1732
		// (get) Token: 0x06004540 RID: 17728 RVA: 0x00147E3A File Offset: 0x0014603A
		public bool IsInParty
		{
			get
			{
				return this.myPartyMemberIDs != null;
			}
		}

		// Token: 0x170006C5 RID: 1733
		// (get) Token: 0x06004541 RID: 17729 RVA: 0x00147E45 File Offset: 0x00146045
		// (set) Token: 0x06004542 RID: 17730 RVA: 0x00147E4D File Offset: 0x0014604D
		public GroupJoinZoneAB partyZone { get; private set; }

		// Token: 0x06004543 RID: 17731 RVA: 0x00147E56 File Offset: 0x00146056
		private void Awake()
		{
			FriendshipGroupDetection.Instance = this;
			if (this.friendshipBubble)
			{
				this.particleSystem = this.friendshipBubble.GetComponent<ParticleSystem>();
				this.audioSource = this.friendshipBubble.GetComponent<AudioSource>();
			}
		}

		// Token: 0x06004544 RID: 17732 RVA: 0x00147E8D File Offset: 0x0014608D
		public void AddGroupZoneCallback(Action<GroupJoinZoneAB> callback)
		{
			this.groupZoneCallbacks.Add(callback);
		}

		// Token: 0x06004545 RID: 17733 RVA: 0x00147E9B File Offset: 0x0014609B
		public void RemoveGroupZoneCallback(Action<GroupJoinZoneAB> callback)
		{
			this.groupZoneCallbacks.Remove(callback);
		}

		// Token: 0x06004546 RID: 17734 RVA: 0x00147EAA File Offset: 0x001460AA
		public bool IsInMyGroup(string userID)
		{
			return this.myPartyMemberIDs != null && this.myPartyMemberIDs.Contains(userID);
		}

		// Token: 0x06004547 RID: 17735 RVA: 0x00147EC4 File Offset: 0x001460C4
		public bool AnyPartyMembersOutsideFriendCollider()
		{
			if (!this.IsInParty)
			{
				return false;
			}
			foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
			{
				if (vrrig.IsLocalPartyMember && !GorillaComputer.instance.friendJoinCollider.playerIDsCurrentlyTouching.Contains(vrrig.creator.UserId))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x170006C6 RID: 1734
		// (get) Token: 0x06004548 RID: 17736 RVA: 0x00147F54 File Offset: 0x00146154
		// (set) Token: 0x06004549 RID: 17737 RVA: 0x00147F5C File Offset: 0x0014615C
		public bool DidJoinLeftHanded { get; private set; }

		// Token: 0x0600454A RID: 17738 RVA: 0x00147F68 File Offset: 0x00146168
		private void Update()
		{
			List<int> list = this.playersInProvisionalGroup;
			List<int> list2 = this.playersInProvisionalGroup;
			List<int> list3 = this.tempIntList;
			this.tempIntList = list2;
			this.playersInProvisionalGroup = list3;
			Vector3 vector;
			this.UpdateProvisionalGroup(out vector);
			if (this.playersInProvisionalGroup.Count > 0)
			{
				this.friendshipBubble.transform.position = vector;
			}
			bool flag = false;
			if (list.Count == this.playersInProvisionalGroup.Count)
			{
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i] != this.playersInProvisionalGroup[i])
					{
						flag = true;
						break;
					}
				}
			}
			else
			{
				flag = true;
			}
			if (flag)
			{
				this.groupCreateAfterTimestamp = Time.time + this.groupTime;
				this.amFirstProvisionalPlayer = this.playersInProvisionalGroup.Count > 0 && this.playersInProvisionalGroup[0] == NetworkSystem.Instance.LocalPlayer.ActorNumber;
				if (this.playersInProvisionalGroup.Count > 0 && !this.amFirstProvisionalPlayer)
				{
					List<int> list4 = this.tempIntList;
					list4.Clear();
					NetPlayer netPlayer = null;
					foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
					{
						if (vrrig.creator.ActorNumber == this.playersInProvisionalGroup[0])
						{
							netPlayer = vrrig.creator;
							if (vrrig.IsLocalPartyMember)
							{
								list4.Clear();
								break;
							}
						}
						else if (vrrig.IsLocalPartyMember)
						{
							list4.Add(vrrig.creator.ActorNumber);
						}
					}
					if (list4.Count > 0)
					{
						this.photonView.RPC("NotifyPartyMerging", netPlayer.GetPlayerRef(), new object[] { list4.ToArray() });
					}
					else
					{
						this.photonView.RPC("NotifyNoPartyToMerge", netPlayer.GetPlayerRef(), Array.Empty<object>());
					}
				}
				if (this.playersInProvisionalGroup.Count == 0)
				{
					if (Time.time > this.suppressPartyCreationUntilTimestamp && this.playEffectsAfterTimestamp == 0f)
					{
						this.audioSource.GTStop();
						this.audioSource.GTPlayOneShot(this.fistBumpInterruptedAudio, 1f);
					}
					this.particleSystem.Stop();
					this.playEffectsAfterTimestamp = 0f;
				}
				else
				{
					this.playEffectsAfterTimestamp = Time.time + this.playEffectsDelay;
				}
			}
			else if (this.playEffectsAfterTimestamp > 0f && Time.time > this.playEffectsAfterTimestamp)
			{
				this.audioSource.time = 0f;
				this.audioSource.GTPlay();
				this.particleSystem.Play();
				this.playEffectsAfterTimestamp = 0f;
			}
			else if (this.playersInProvisionalGroup.Count > 0 && Time.time > this.groupCreateAfterTimestamp && this.amFirstProvisionalPlayer)
			{
				List<int> list5 = this.tempIntList;
				list5.Clear();
				list5.AddRange(this.playersInProvisionalGroup);
				int num = 0;
				if (this.IsInParty)
				{
					foreach (VRRig vrrig2 in GorillaParent.instance.vrrigs)
					{
						if (vrrig2.IsLocalPartyMember)
						{
							list5.Add(vrrig2.creator.ActorNumber);
							num++;
						}
					}
				}
				int num2 = 0;
				foreach (int num3 in this.playersInProvisionalGroup)
				{
					int[] array;
					if (this.partyMergeIDs.TryGetValue(num3, out array))
					{
						list5.AddRange(array);
						num2++;
					}
				}
				list5.Sort();
				int[] array2 = list5.Distinct<int>().ToArray<int>();
				this.myBraceletColor = GTColor.RandomHSV(this.braceletRandomColorHSVRanges);
				this.SendPartyFormedRPC(FriendshipGroupDetection.PackColor(this.myBraceletColor), array2, false);
				this.groupCreateAfterTimestamp = Time.time + this.cooldownAfterCreatingGroup;
			}
			if (this.myPartyMemberIDs != null)
			{
				this.UpdateWarningSigns();
			}
		}

		// Token: 0x0600454B RID: 17739 RVA: 0x001483A8 File Offset: 0x001465A8
		private void UpdateProvisionalGroup(out Vector3 midpoint)
		{
			this.playersInProvisionalGroup.Clear();
			bool flag;
			VRMap makingFist = GorillaTagger.Instance.offlineVRRig.GetMakingFist(this.debug, out flag);
			if (makingFist == null || !NetworkSystem.Instance.InRoom || GorillaParent.instance.vrrigs.Count == 0 || Time.time < this.suppressPartyCreationUntilTimestamp || (global::GorillaGameModes.GameMode.ActiveGameMode != null && !global::GorillaGameModes.GameMode.ActiveGameMode.CanJoinFrienship(NetworkSystem.Instance.LocalPlayer)))
			{
				midpoint = Vector3.zero;
				return;
			}
			this.WillJoinLeftHanded = flag;
			this.playersToPropagateFrom.Clear();
			this.provisionalGroupUsingLeftHands.Clear();
			this.playersMakingFists.Clear();
			int actorNumber = NetworkSystem.Instance.LocalPlayer.ActorNumber;
			int num = -1;
			foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
			{
				bool flag2;
				VRMap makingFist2 = vrrig.GetMakingFist(this.debug, out flag2);
				if (makingFist2 != null && (!(global::GorillaGameModes.GameMode.ActiveGameMode != null) || global::GorillaGameModes.GameMode.ActiveGameMode.CanJoinFrienship(vrrig.OwningNetPlayer)))
				{
					FriendshipGroupDetection.PlayerFist playerFist = new FriendshipGroupDetection.PlayerFist
					{
						actorNumber = vrrig.creator.ActorNumber,
						position = makingFist2.rigTarget.position,
						isLeftHand = flag2
					};
					if (vrrig.isOfflineVRRig)
					{
						num = this.playersMakingFists.Count;
					}
					this.playersMakingFists.Add(playerFist);
				}
			}
			if (this.playersMakingFists.Count <= 1)
			{
				midpoint = Vector3.zero;
				return;
			}
			this.playersToPropagateFrom.Enqueue(this.playersMakingFists[num]);
			this.playersInProvisionalGroup.Add(actorNumber);
			midpoint = makingFist.rigTarget.position;
			int num2 = 1 << num;
			FriendshipGroupDetection.PlayerFist playerFist2;
			while (this.playersToPropagateFrom.TryDequeue(out playerFist2))
			{
				for (int i = 0; i < this.playersMakingFists.Count; i++)
				{
					if ((num2 & (1 << i)) == 0)
					{
						FriendshipGroupDetection.PlayerFist playerFist3 = this.playersMakingFists[i];
						if ((playerFist2.position - playerFist3.position).IsShorterThan(this.detectionRadius))
						{
							int num3 = ~this.playersInProvisionalGroup.BinarySearch(playerFist3.actorNumber);
							num2 |= 1 << i;
							this.playersInProvisionalGroup.Insert(num3, playerFist3.actorNumber);
							if (playerFist3.isLeftHand)
							{
								this.provisionalGroupUsingLeftHands.Add(playerFist3.actorNumber);
							}
							this.playersToPropagateFrom.Enqueue(playerFist3);
							midpoint += playerFist3.position;
						}
					}
				}
			}
			if (this.playersInProvisionalGroup.Count == 1)
			{
				this.playersInProvisionalGroup.Clear();
			}
			if (this.playersInProvisionalGroup.Count > 0)
			{
				midpoint /= (float)this.playersInProvisionalGroup.Count;
			}
		}

		// Token: 0x0600454C RID: 17740 RVA: 0x001486D4 File Offset: 0x001468D4
		private void UpdateWarningSigns()
		{
			ZoneEntity zoneEntity = GorillaTagger.Instance.offlineVRRig.zoneEntity;
			GTZone currentRoomZone = PhotonNetworkController.Instance.CurrentRoomZone;
			GroupJoinZoneAB groupJoinZoneAB = 0;
			if (this.myPartyMemberIDs != null)
			{
				foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
				{
					if (vrrig.IsLocalPartyMember && !vrrig.isOfflineVRRig)
					{
						groupJoinZoneAB |= vrrig.zoneEntity.GroupZone;
					}
				}
			}
			if (groupJoinZoneAB != this.partyZone)
			{
				this.debugStr.Clear();
				foreach (VRRig vrrig2 in GorillaParent.instance.vrrigs)
				{
					if (vrrig2.IsLocalPartyMember && !vrrig2.isOfflineVRRig)
					{
						this.debugStr.Append(string.Format("{0} in {1};", vrrig2.playerNameVisible, vrrig2.zoneEntity.GroupZone));
					}
				}
				this.partyZone = groupJoinZoneAB;
				foreach (Action<GroupJoinZoneAB> action in this.groupZoneCallbacks)
				{
					action(this.partyZone);
				}
			}
		}

		// Token: 0x0600454D RID: 17741 RVA: 0x0014885C File Offset: 0x00146A5C
		[PunRPC]
		private void NotifyNoPartyToMerge(PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "NotifyNoPartyToMerge");
			if (info.Sender == null || this.partyMergeIDs == null)
			{
				return;
			}
			this.partyMergeIDs.Remove(info.Sender.ActorNumber);
		}

		// Token: 0x0600454E RID: 17742 RVA: 0x00148894 File Offset: 0x00146A94
		[Rpc]
		private unsafe static void RPC_NotifyNoPartyToMerge(NetworkRunner runner, RpcInfo info = default(RpcInfo))
		{
			if (NetworkBehaviourUtils.InvokeRpc)
			{
				NetworkBehaviourUtils.InvokeRpc = false;
			}
			else
			{
				if (runner == null)
				{
					throw new ArgumentNullException("runner");
				}
				if (runner.Stage == SimulationStages.Resimulate)
				{
					return;
				}
				if (runner.HasAnyActiveConnections())
				{
					int num = 8;
					SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
					byte* data = SimulationMessage.GetData(ptr);
					int num2 = RpcHeader.Write(RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_NotifyNoPartyToMerge(Fusion.NetworkRunner,Fusion.RpcInfo)")), data);
					ptr->Offset = num2 * 8;
					ptr->SetStatic();
					runner.SendRpc(ptr);
				}
				info = RpcInfo.FromLocal(runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
			}
			FriendshipGroupDetection.Instance.partyMergeIDs.Remove(info.Source.PlayerId);
		}

		// Token: 0x0600454F RID: 17743 RVA: 0x00148974 File Offset: 0x00146B74
		[PunRPC]
		private void NotifyPartyMerging(int[] memberIDs, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "NotifyPartyMerging");
			if (memberIDs.Length > 10)
			{
				return;
			}
			this.partyMergeIDs[info.Sender.ActorNumber] = memberIDs;
		}

		// Token: 0x06004550 RID: 17744 RVA: 0x001489A0 File Offset: 0x00146BA0
		[Rpc]
		private unsafe static void RPC_NotifyPartyMerging(NetworkRunner runner, [RpcTarget] PlayerRef playerRef, int[] memberIDs, RpcInfo info = default(RpcInfo))
		{
			if (NetworkBehaviourUtils.InvokeRpc)
			{
				NetworkBehaviourUtils.InvokeRpc = false;
			}
			else
			{
				if (runner == null)
				{
					throw new ArgumentNullException("runner");
				}
				if (runner.Stage != SimulationStages.Resimulate)
				{
					RpcTargetStatus rpcTargetStatus = runner.GetRpcTargetStatus(playerRef);
					if (rpcTargetStatus == RpcTargetStatus.Unreachable)
					{
						NetworkBehaviourUtils.NotifyRpcTargetUnreachable(playerRef, "System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_NotifyPartyMerging(Fusion.NetworkRunner,Fusion.PlayerRef,System.Int32[],Fusion.RpcInfo)");
					}
					else
					{
						if (rpcTargetStatus == RpcTargetStatus.Self)
						{
							info = RpcInfo.FromLocal(runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
							goto IL_0010;
						}
						int num = 8;
						num += (memberIDs.Length * 4 + 4 + 3) & -4;
						SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
						byte* data = SimulationMessage.GetData(ptr);
						int num2 = RpcHeader.Write(RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_NotifyPartyMerging(Fusion.NetworkRunner,Fusion.PlayerRef,System.Int32[],Fusion.RpcInfo)")), data);
						*(int*)(data + num2) = memberIDs.Length;
						num2 += 4;
						num2 = ((Native.CopyFromArray<int>((void*)(data + num2), memberIDs) + 3) & -4) + num2;
						ptr->Offset = num2 * 8;
						ptr->SetTarget(playerRef);
						ptr->SetStatic();
						runner.SendRpc(ptr);
					}
				}
				return;
			}
			IL_0010:
			if (memberIDs.Length > 10)
			{
				return;
			}
			FriendshipGroupDetection.Instance.partyMergeIDs[info.Source.PlayerId] = memberIDs;
		}

		// Token: 0x06004551 RID: 17745 RVA: 0x00148B0C File Offset: 0x00146D0C
		public void SendAboutToGroupJoin()
		{
			foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
			{
				Debug.Log(string.Concat(new string[]
				{
					"Sending group join to ",
					GorillaParent.instance.vrrigs.Count.ToString(),
					" players. Party member:",
					vrrig.OwningNetPlayer.NickName,
					"Is offline rig",
					vrrig.isOfflineVRRig.ToString()
				}));
				if (vrrig.IsLocalPartyMember && !vrrig.isOfflineVRRig)
				{
					this.photonView.RPC("PartyMemberIsAboutToGroupJoin", vrrig.Creator.GetPlayerRef(), Array.Empty<object>());
				}
			}
		}

		// Token: 0x06004552 RID: 17746 RVA: 0x00148BF4 File Offset: 0x00146DF4
		[PunRPC]
		private void PartyMemberIsAboutToGroupJoin(PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "PartyMemberIsAboutToGroupJoin");
			this.PartMemberIsAboutToGroupJoinWrapped(new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x06004553 RID: 17747 RVA: 0x00148C10 File Offset: 0x00146E10
		[Rpc]
		private unsafe static void RPC_PartyMemberIsAboutToGroupJoin(NetworkRunner runner, [RpcTarget] PlayerRef targetPlayer, RpcInfo info = default(RpcInfo))
		{
			if (NetworkBehaviourUtils.InvokeRpc)
			{
				NetworkBehaviourUtils.InvokeRpc = false;
			}
			else
			{
				if (runner == null)
				{
					throw new ArgumentNullException("runner");
				}
				if (runner.Stage != SimulationStages.Resimulate)
				{
					RpcTargetStatus rpcTargetStatus = runner.GetRpcTargetStatus(targetPlayer);
					if (rpcTargetStatus == RpcTargetStatus.Unreachable)
					{
						NetworkBehaviourUtils.NotifyRpcTargetUnreachable(targetPlayer, "System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_PartyMemberIsAboutToGroupJoin(Fusion.NetworkRunner,Fusion.PlayerRef,Fusion.RpcInfo)");
					}
					else
					{
						if (rpcTargetStatus == RpcTargetStatus.Self)
						{
							info = RpcInfo.FromLocal(runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
							goto IL_0010;
						}
						int num = 8;
						SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
						byte* data = SimulationMessage.GetData(ptr);
						int num2 = RpcHeader.Write(RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_PartyMemberIsAboutToGroupJoin(Fusion.NetworkRunner,Fusion.PlayerRef,Fusion.RpcInfo)")), data);
						ptr->Offset = num2 * 8;
						ptr->SetTarget(targetPlayer);
						ptr->SetStatic();
						runner.SendRpc(ptr);
					}
				}
				return;
			}
			IL_0010:
			FriendshipGroupDetection.Instance.PartMemberIsAboutToGroupJoinWrapped(new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x06004554 RID: 17748 RVA: 0x00148D10 File Offset: 0x00146F10
		private void PartMemberIsAboutToGroupJoinWrapped(PhotonMessageInfoWrapped wrappedInfo)
		{
			float time = Time.time;
			float num = this.aboutToGroupJoin_CooldownUntilTimestamp;
			if (wrappedInfo.senderID < NetworkSystem.Instance.LocalPlayer.ActorNumber)
			{
				this.aboutToGroupJoin_CooldownUntilTimestamp = Time.time + 5f;
				if (this.myPartyMembersHash.Contains(wrappedInfo.Sender.UserId))
				{
					PhotonNetworkController.Instance.DeferJoining(2f);
				}
			}
		}

		// Token: 0x06004555 RID: 17749 RVA: 0x00148D7C File Offset: 0x00146F7C
		private void SendPartyFormedRPC(short braceletColor, int[] memberIDs, bool forceDebug)
		{
			string text = Enum.Parse<GameModeType>(GorillaComputer.instance.currentGameMode.Value, true).ToString();
			foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
			{
				if (this.playersInProvisionalGroup.BinarySearch(vrrig.creator.ActorNumber) >= 0)
				{
					this.photonView.RPC("PartyFormedSuccessfully", vrrig.Creator.GetPlayerRef(), new object[] { text, braceletColor, memberIDs, forceDebug });
				}
			}
		}

		// Token: 0x06004556 RID: 17750 RVA: 0x00148E48 File Offset: 0x00147048
		[Rpc]
		private unsafe static void RPC_PartyFormedSuccessfully(NetworkRunner runner, [RpcTarget] PlayerRef targetPlayer, string partyGameMode, short braceletColor, int[] memberIDs, bool forceDebug, RpcInfo info = default(RpcInfo))
		{
			if (NetworkBehaviourUtils.InvokeRpc)
			{
				NetworkBehaviourUtils.InvokeRpc = false;
			}
			else
			{
				if (runner == null)
				{
					throw new ArgumentNullException("runner");
				}
				if (runner.Stage != SimulationStages.Resimulate)
				{
					RpcTargetStatus rpcTargetStatus = runner.GetRpcTargetStatus(targetPlayer);
					if (rpcTargetStatus == RpcTargetStatus.Unreachable)
					{
						NetworkBehaviourUtils.NotifyRpcTargetUnreachable(targetPlayer, "System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_PartyFormedSuccessfully(Fusion.NetworkRunner,Fusion.PlayerRef,System.String,System.Int16,System.Int32[],System.Boolean,Fusion.RpcInfo)");
					}
					else
					{
						if (rpcTargetStatus == RpcTargetStatus.Self)
						{
							info = RpcInfo.FromLocal(runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
							goto IL_0010;
						}
						int num = 8;
						num += (ReadWriteUtilsForWeaver.GetByteCountUtf8NoHash(partyGameMode) + 3) & -4;
						num += 4;
						num += (memberIDs.Length * 4 + 4 + 3) & -4;
						num += 4;
						SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
						byte* data = SimulationMessage.GetData(ptr);
						int num2 = RpcHeader.Write(RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_PartyFormedSuccessfully(Fusion.NetworkRunner,Fusion.PlayerRef,System.String,System.Int16,System.Int32[],System.Boolean,Fusion.RpcInfo)")), data);
						num2 = ((ReadWriteUtilsForWeaver.WriteStringUtf8NoHash((void*)(data + num2), partyGameMode) + 3) & -4) + num2;
						*(short*)(data + num2) = braceletColor;
						num2 += (2 + 3) & -4;
						*(int*)(data + num2) = memberIDs.Length;
						num2 += 4;
						num2 = ((Native.CopyFromArray<int>((void*)(data + num2), memberIDs) + 3) & -4) + num2;
						ReadWriteUtilsForWeaver.WriteBoolean((int*)(data + num2), forceDebug);
						num2 += 4;
						ptr->Offset = num2 * 8;
						ptr->SetTarget(targetPlayer);
						ptr->SetStatic();
						runner.SendRpc(ptr);
					}
				}
				return;
			}
			IL_0010:
			GorillaNot.IncrementRPCCall(info, "PartyFormedSuccessfully");
			FriendshipGroupDetection.Instance.PartyFormedSuccesfullyWrapped(partyGameMode, braceletColor, memberIDs, forceDebug, new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x06004557 RID: 17751 RVA: 0x00149044 File Offset: 0x00147244
		[PunRPC]
		private void PartyFormedSuccessfully(string partyGameMode, short braceletColor, int[] memberIDs, bool forceDebug, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "PartyFormedSuccessfully");
			this.PartyFormedSuccesfullyWrapped(partyGameMode, braceletColor, memberIDs, forceDebug, new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x06004558 RID: 17752 RVA: 0x00149064 File Offset: 0x00147264
		private void PartyFormedSuccesfullyWrapped(string partyGameMode, short braceletColor, int[] memberIDs, bool forceDebug, PhotonMessageInfoWrapped info)
		{
			if (memberIDs == null || memberIDs.Length > 10 || !memberIDs.Contains(info.Sender.ActorNumber) || this.playersInProvisionalGroup.IndexOf(info.Sender.ActorNumber) != 0 || Mathf.Abs(this.groupCreateAfterTimestamp - Time.time) > this.m_maxGroupJoinTimeDifference || !global::GorillaGameModes.GameMode.IsValidGameMode(partyGameMode))
			{
				return;
			}
			if (this.IsInParty)
			{
				string text = Enum.Parse<GameModeType>(GorillaComputer.instance.currentGameMode.Value, true).ToString();
				foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
				{
					if (vrrig.IsLocalPartyMember && !vrrig.isOfflineVRRig)
					{
						this.photonView.RPC("AddPartyMembers", vrrig.Creator.GetPlayerRef(), new object[] { text, braceletColor, memberIDs });
					}
				}
			}
			this.suppressPartyCreationUntilTimestamp = Time.time + this.cooldownAfterCreatingGroup;
			this.DidJoinLeftHanded = this.WillJoinLeftHanded;
			this.SetNewParty(partyGameMode, braceletColor, memberIDs);
		}

		// Token: 0x06004559 RID: 17753 RVA: 0x001491AC File Offset: 0x001473AC
		[PunRPC]
		private void AddPartyMembers(string partyGameMode, short braceletColor, int[] memberIDs, PhotonMessageInfo info)
		{
			this.AddPartyMembersWrapped(partyGameMode, braceletColor, memberIDs, new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x0600455A RID: 17754 RVA: 0x001491C0 File Offset: 0x001473C0
		[Rpc]
		private unsafe static void RPC_AddPartyMembers(NetworkRunner runner, [RpcTarget] PlayerRef rpcTarget, string partyGameMode, short braceletColor, int[] memberIDs, RpcInfo info = default(RpcInfo))
		{
			if (NetworkBehaviourUtils.InvokeRpc)
			{
				NetworkBehaviourUtils.InvokeRpc = false;
			}
			else
			{
				if (runner == null)
				{
					throw new ArgumentNullException("runner");
				}
				if (runner.Stage != SimulationStages.Resimulate)
				{
					RpcTargetStatus rpcTargetStatus = runner.GetRpcTargetStatus(rpcTarget);
					if (rpcTargetStatus == RpcTargetStatus.Unreachable)
					{
						NetworkBehaviourUtils.NotifyRpcTargetUnreachable(rpcTarget, "System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_AddPartyMembers(Fusion.NetworkRunner,Fusion.PlayerRef,System.String,System.Int16,System.Int32[],Fusion.RpcInfo)");
					}
					else
					{
						if (rpcTargetStatus == RpcTargetStatus.Self)
						{
							info = RpcInfo.FromLocal(runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
							goto IL_0010;
						}
						int num = 8;
						num += (ReadWriteUtilsForWeaver.GetByteCountUtf8NoHash(partyGameMode) + 3) & -4;
						num += 4;
						num += (memberIDs.Length * 4 + 4 + 3) & -4;
						SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
						byte* data = SimulationMessage.GetData(ptr);
						int num2 = RpcHeader.Write(RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_AddPartyMembers(Fusion.NetworkRunner,Fusion.PlayerRef,System.String,System.Int16,System.Int32[],Fusion.RpcInfo)")), data);
						num2 = ((ReadWriteUtilsForWeaver.WriteStringUtf8NoHash((void*)(data + num2), partyGameMode) + 3) & -4) + num2;
						*(short*)(data + num2) = braceletColor;
						num2 += (2 + 3) & -4;
						*(int*)(data + num2) = memberIDs.Length;
						num2 += 4;
						num2 = ((Native.CopyFromArray<int>((void*)(data + num2), memberIDs) + 3) & -4) + num2;
						ptr->Offset = num2 * 8;
						ptr->SetTarget(rpcTarget);
						ptr->SetStatic();
						runner.SendRpc(ptr);
					}
				}
				return;
			}
			IL_0010:
			FriendshipGroupDetection.Instance.AddPartyMembersWrapped(partyGameMode, braceletColor, memberIDs, new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x0600455B RID: 17755 RVA: 0x00149384 File Offset: 0x00147584
		private void AddPartyMembersWrapped(string partyGameMode, short braceletColor, int[] memberIDs, PhotonMessageInfoWrapped infoWrapped)
		{
			GorillaNot.IncrementRPCCall(infoWrapped, "AddPartyMembersWrapped");
			if (memberIDs.Length > 10 || !this.IsInParty || !this.myPartyMembersHash.Contains(NetworkSystem.Instance.GetUserID(infoWrapped.senderID)) || !global::GorillaGameModes.GameMode.IsValidGameMode(partyGameMode))
			{
				return;
			}
			Debug.Log("Adding party members: [" + string.Join<int>(",", memberIDs) + "]");
			this.SetNewParty(partyGameMode, braceletColor, memberIDs);
		}

		// Token: 0x0600455C RID: 17756 RVA: 0x001493FC File Offset: 0x001475FC
		private void SetNewParty(string partyGameMode, short braceletColor, int[] memberIDs)
		{
			GorillaComputer.instance.SetGameModeWithoutButton(partyGameMode);
			this.myPartyMemberIDs = new List<string>();
			FriendshipGroupDetection.userIdLookup.Clear();
			foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
			{
				FriendshipGroupDetection.userIdLookup.Add(vrrig.creator.ActorNumber, vrrig.creator.UserId);
			}
			foreach (int num in memberIDs)
			{
				string text;
				if (FriendshipGroupDetection.userIdLookup.TryGetValue(num, out text))
				{
					this.myPartyMemberIDs.Add(text);
				}
			}
			this.myBraceletColor = FriendshipGroupDetection.UnpackColor(braceletColor);
			GorillaTagger.Instance.StartVibration(this.DidJoinLeftHanded, this.hapticStrength, this.hapticDuration);
			this.OnPartyMembershipChanged();
			PlayerGameEvents.MiscEvent("FriendshipGroupJoined");
		}

		// Token: 0x0600455D RID: 17757 RVA: 0x001494FC File Offset: 0x001476FC
		public void LeaveParty()
		{
			if (this.myPartyMemberIDs == null)
			{
				return;
			}
			foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
			{
				if (vrrig.IsLocalPartyMember && !vrrig.isOfflineVRRig)
				{
					this.photonView.RPC("PlayerLeftParty", vrrig.Creator.GetPlayerRef(), Array.Empty<object>());
				}
			}
			this.myPartyMemberIDs = null;
			this.OnPartyMembershipChanged();
			PhotonNetworkController.Instance.ClearDeferredJoin();
			GorillaTagger.Instance.StartVibration(false, this.hapticStrength, this.hapticDuration);
		}

		// Token: 0x0600455E RID: 17758 RVA: 0x001495B8 File Offset: 0x001477B8
		[Rpc]
		private unsafe static void RPC_PlayerLeftParty(NetworkRunner runner, [RpcTarget] PlayerRef player, RpcInfo info = default(RpcInfo))
		{
			if (NetworkBehaviourUtils.InvokeRpc)
			{
				NetworkBehaviourUtils.InvokeRpc = false;
			}
			else
			{
				if (runner == null)
				{
					throw new ArgumentNullException("runner");
				}
				if (runner.Stage != SimulationStages.Resimulate)
				{
					RpcTargetStatus rpcTargetStatus = runner.GetRpcTargetStatus(player);
					if (rpcTargetStatus == RpcTargetStatus.Unreachable)
					{
						NetworkBehaviourUtils.NotifyRpcTargetUnreachable(player, "System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_PlayerLeftParty(Fusion.NetworkRunner,Fusion.PlayerRef,Fusion.RpcInfo)");
					}
					else
					{
						if (rpcTargetStatus == RpcTargetStatus.Self)
						{
							info = RpcInfo.FromLocal(runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
							goto IL_0010;
						}
						int num = 8;
						SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
						byte* data = SimulationMessage.GetData(ptr);
						int num2 = RpcHeader.Write(RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_PlayerLeftParty(Fusion.NetworkRunner,Fusion.PlayerRef,Fusion.RpcInfo)")), data);
						ptr->Offset = num2 * 8;
						ptr->SetTarget(player);
						ptr->SetStatic();
						runner.SendRpc(ptr);
					}
				}
				return;
			}
			IL_0010:
			GorillaNot.IncrementRPCCall(info, "PlayerLeftParty");
			FriendshipGroupDetection.Instance.PlayerLeftPartyWrapped(new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x0600455F RID: 17759 RVA: 0x001496C7 File Offset: 0x001478C7
		[PunRPC]
		private void PlayerLeftParty(PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "PlayerLeftParty");
			this.PlayerLeftPartyWrapped(new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x06004560 RID: 17760 RVA: 0x001496E0 File Offset: 0x001478E0
		private void PlayerLeftPartyWrapped(PhotonMessageInfoWrapped infoWrapped)
		{
			if (this.myPartyMemberIDs == null)
			{
				return;
			}
			if (!this.myPartyMemberIDs.Remove(infoWrapped.Sender.UserId))
			{
				return;
			}
			if (this.myPartyMemberIDs.Count <= 1)
			{
				this.myPartyMemberIDs = null;
			}
			this.OnPartyMembershipChanged();
			GorillaTagger.Instance.StartVibration(this.DidJoinLeftHanded, this.hapticStrength, this.hapticDuration);
		}

		// Token: 0x06004561 RID: 17761 RVA: 0x00149747 File Offset: 0x00147947
		public void SendVerifyPartyMember(NetPlayer player)
		{
			this.photonView.RPC("VerifyPartyMember", player.GetPlayerRef(), Array.Empty<object>());
		}

		// Token: 0x06004562 RID: 17762 RVA: 0x00149764 File Offset: 0x00147964
		[PunRPC]
		private void VerifyPartyMember(PhotonMessageInfo info)
		{
			this.VerifyPartyMemberWrapped(new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x06004563 RID: 17763 RVA: 0x00149774 File Offset: 0x00147974
		[Rpc]
		private unsafe static void RPC_VerifyPartyMember(NetworkRunner runner, [RpcTarget] PlayerRef rpcTarget, RpcInfo info = default(RpcInfo))
		{
			if (NetworkBehaviourUtils.InvokeRpc)
			{
				NetworkBehaviourUtils.InvokeRpc = false;
			}
			else
			{
				if (runner == null)
				{
					throw new ArgumentNullException("runner");
				}
				if (runner.Stage != SimulationStages.Resimulate)
				{
					RpcTargetStatus rpcTargetStatus = runner.GetRpcTargetStatus(rpcTarget);
					if (rpcTargetStatus == RpcTargetStatus.Unreachable)
					{
						NetworkBehaviourUtils.NotifyRpcTargetUnreachable(rpcTarget, "System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_VerifyPartyMember(Fusion.NetworkRunner,Fusion.PlayerRef,Fusion.RpcInfo)");
					}
					else
					{
						if (rpcTargetStatus == RpcTargetStatus.Self)
						{
							info = RpcInfo.FromLocal(runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
							goto IL_0010;
						}
						int num = 8;
						SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
						byte* data = SimulationMessage.GetData(ptr);
						int num2 = RpcHeader.Write(RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_VerifyPartyMember(Fusion.NetworkRunner,Fusion.PlayerRef,Fusion.RpcInfo)")), data);
						ptr->Offset = num2 * 8;
						ptr->SetTarget(rpcTarget);
						ptr->SetStatic();
						runner.SendRpc(ptr);
					}
				}
				return;
			}
			IL_0010:
			FriendshipGroupDetection.Instance.VerifyPartyMemberWrapped(new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x06004564 RID: 17764 RVA: 0x00149874 File Offset: 0x00147A74
		private void VerifyPartyMemberWrapped(PhotonMessageInfoWrapped infoWrapped)
		{
			GorillaNot.IncrementRPCCall(infoWrapped, "VerifyPartyMemberWrapped");
			RigContainer rigContainer;
			if (!VRRigCache.Instance.TryGetVrrig(infoWrapped.Sender, out rigContainer) || !FXSystem.CheckCallSpam(rigContainer.Rig.fxSettings, 15, infoWrapped.SentServerTime))
			{
				return;
			}
			if (this.myPartyMemberIDs == null || !this.myPartyMemberIDs.Contains(NetworkSystem.Instance.GetUserID(infoWrapped.senderID)))
			{
				this.photonView.RPC("PlayerLeftParty", infoWrapped.Sender.GetPlayerRef(), Array.Empty<object>());
			}
		}

		// Token: 0x06004565 RID: 17765 RVA: 0x00149904 File Offset: 0x00147B04
		public void SendRequestPartyGameMode(string gameMode)
		{
			int num = int.MaxValue;
			NetPlayer netPlayer = null;
			foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
			{
				if (vrrig.IsLocalPartyMember && vrrig.creator.ActorNumber < num)
				{
					netPlayer = vrrig.creator;
					num = vrrig.creator.ActorNumber;
				}
			}
			if (netPlayer != null)
			{
				this.photonView.RPC("RequestPartyGameMode", netPlayer.GetPlayerRef(), new object[] { gameMode });
			}
		}

		// Token: 0x06004566 RID: 17766 RVA: 0x001499AC File Offset: 0x00147BAC
		[Rpc]
		private unsafe static void RPC_RequestPartyGameMode(NetworkRunner runner, [RpcTarget] PlayerRef targetPlayer, string gameMode, RpcInfo info = default(RpcInfo))
		{
			if (NetworkBehaviourUtils.InvokeRpc)
			{
				NetworkBehaviourUtils.InvokeRpc = false;
			}
			else
			{
				if (runner == null)
				{
					throw new ArgumentNullException("runner");
				}
				if (runner.Stage != SimulationStages.Resimulate)
				{
					RpcTargetStatus rpcTargetStatus = runner.GetRpcTargetStatus(targetPlayer);
					if (rpcTargetStatus == RpcTargetStatus.Unreachable)
					{
						NetworkBehaviourUtils.NotifyRpcTargetUnreachable(targetPlayer, "System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_RequestPartyGameMode(Fusion.NetworkRunner,Fusion.PlayerRef,System.String,Fusion.RpcInfo)");
					}
					else
					{
						if (rpcTargetStatus == RpcTargetStatus.Self)
						{
							info = RpcInfo.FromLocal(runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
							goto IL_0010;
						}
						int num = 8;
						num += (ReadWriteUtilsForWeaver.GetByteCountUtf8NoHash(gameMode) + 3) & -4;
						SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
						byte* data = SimulationMessage.GetData(ptr);
						int num2 = RpcHeader.Write(RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_RequestPartyGameMode(Fusion.NetworkRunner,Fusion.PlayerRef,System.String,Fusion.RpcInfo)")), data);
						num2 = ((ReadWriteUtilsForWeaver.WriteStringUtf8NoHash((void*)(data + num2), gameMode) + 3) & -4) + num2;
						ptr->Offset = num2 * 8;
						ptr->SetTarget(targetPlayer);
						ptr->SetStatic();
						runner.SendRpc(ptr);
					}
				}
				return;
			}
			IL_0010:
			FriendshipGroupDetection.Instance.RequestPartyGameModeWrapped(gameMode, new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x06004567 RID: 17767 RVA: 0x00149AE9 File Offset: 0x00147CE9
		[PunRPC]
		private void RequestPartyGameMode(string gameMode, PhotonMessageInfo info)
		{
			this.RequestPartyGameModeWrapped(gameMode, new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x06004568 RID: 17768 RVA: 0x00149AF8 File Offset: 0x00147CF8
		private void RequestPartyGameModeWrapped(string gameMode, PhotonMessageInfoWrapped info)
		{
			GorillaNot.IncrementRPCCall(info, "RequestPartyGameModeWrapped");
			if (!this.IsInParty || !this.IsInMyGroup(info.Sender.UserId) || !global::GorillaGameModes.GameMode.IsValidGameMode(gameMode))
			{
				return;
			}
			foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
			{
				if (vrrig.IsLocalPartyMember)
				{
					this.photonView.RPC("NotifyPartyGameModeChanged", vrrig.creator.GetPlayerRef(), new object[] { gameMode });
				}
			}
		}

		// Token: 0x06004569 RID: 17769 RVA: 0x00149BA8 File Offset: 0x00147DA8
		[Rpc]
		private unsafe static void RPC_NotifyPartyGameModeChanged(NetworkRunner runner, [RpcTarget] PlayerRef targetPlayer, string gameMode, RpcInfo info = default(RpcInfo))
		{
			if (NetworkBehaviourUtils.InvokeRpc)
			{
				NetworkBehaviourUtils.InvokeRpc = false;
			}
			else
			{
				if (runner == null)
				{
					throw new ArgumentNullException("runner");
				}
				if (runner.Stage != SimulationStages.Resimulate)
				{
					RpcTargetStatus rpcTargetStatus = runner.GetRpcTargetStatus(targetPlayer);
					if (rpcTargetStatus == RpcTargetStatus.Unreachable)
					{
						NetworkBehaviourUtils.NotifyRpcTargetUnreachable(targetPlayer, "System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_NotifyPartyGameModeChanged(Fusion.NetworkRunner,Fusion.PlayerRef,System.String,Fusion.RpcInfo)");
					}
					else
					{
						if (rpcTargetStatus == RpcTargetStatus.Self)
						{
							info = RpcInfo.FromLocal(runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
							goto IL_0010;
						}
						int num = 8;
						num += (ReadWriteUtilsForWeaver.GetByteCountUtf8NoHash(gameMode) + 3) & -4;
						SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
						byte* data = SimulationMessage.GetData(ptr);
						int num2 = RpcHeader.Write(RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_NotifyPartyGameModeChanged(Fusion.NetworkRunner,Fusion.PlayerRef,System.String,Fusion.RpcInfo)")), data);
						num2 = ((ReadWriteUtilsForWeaver.WriteStringUtf8NoHash((void*)(data + num2), gameMode) + 3) & -4) + num2;
						ptr->Offset = num2 * 8;
						ptr->SetTarget(targetPlayer);
						ptr->SetStatic();
						runner.SendRpc(ptr);
					}
				}
				return;
			}
			IL_0010:
			FriendshipGroupDetection.Instance.NotifyPartyGameModeChangedWrapped(gameMode, new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x0600456A RID: 17770 RVA: 0x00149CE5 File Offset: 0x00147EE5
		[PunRPC]
		private void NotifyPartyGameModeChanged(string gameMode, PhotonMessageInfo info)
		{
			this.NotifyPartyGameModeChangedWrapped(gameMode, new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x0600456B RID: 17771 RVA: 0x00149CF4 File Offset: 0x00147EF4
		private void NotifyPartyGameModeChangedWrapped(string gameMode, PhotonMessageInfoWrapped info)
		{
			GorillaNot.IncrementRPCCall(info, "NotifyPartyGameModeChangedWrapped");
			if (!this.IsInParty || !this.IsInMyGroup(info.Sender.UserId) || !global::GorillaGameModes.GameMode.IsValidGameMode(gameMode))
			{
				return;
			}
			GorillaComputer.instance.SetGameModeWithoutButton(gameMode);
		}

		// Token: 0x0600456C RID: 17772 RVA: 0x00149D34 File Offset: 0x00147F34
		private void OnPartyMembershipChanged()
		{
			this.myPartyMembersHash.Clear();
			if (this.myPartyMemberIDs != null)
			{
				foreach (string text in this.myPartyMemberIDs)
				{
					this.myPartyMembersHash.Add(text);
				}
			}
			this.myBeadColors.Clear();
			FriendshipGroupDetection.tempColorLookup.Clear();
			foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
			{
				vrrig.ClearPartyMemberStatus();
				if (vrrig.IsLocalPartyMember)
				{
					FriendshipGroupDetection.tempColorLookup.Add(vrrig.Creator.UserId, vrrig.playerColor);
				}
			}
			this.MyBraceletSelfIndex = 0;
			if (this.myPartyMemberIDs != null)
			{
				using (List<string>.Enumerator enumerator = this.myPartyMemberIDs.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string text2 = enumerator.Current;
						Color color;
						if (FriendshipGroupDetection.tempColorLookup.TryGetValue(text2, out color))
						{
							if (text2 == PhotonNetwork.LocalPlayer.UserId)
							{
								this.MyBraceletSelfIndex = this.myBeadColors.Count;
							}
							this.myBeadColors.Add(color);
						}
					}
					goto IL_015A;
				}
			}
			GorillaComputer.instance.SetGameModeWithoutButton(GorillaComputer.instance.lastPressedGameMode);
			IL_015A:
			this.myBeadColors.Add(this.myBraceletColor);
			GorillaTagger.Instance.offlineVRRig.UpdateFriendshipBracelet();
			this.UpdateWarningSigns();
		}

		// Token: 0x0600456D RID: 17773 RVA: 0x00149EEC File Offset: 0x001480EC
		public bool IsPartyWithinCollider(GorillaFriendCollider friendCollider)
		{
			foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
			{
				if (vrrig.IsLocalPartyMember && !vrrig.isOfflineVRRig && !friendCollider.playerIDsCurrentlyTouching.Contains(vrrig.Creator.UserId))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600456E RID: 17774 RVA: 0x00149F70 File Offset: 0x00148170
		public static short PackColor(Color col)
		{
			return (short)(Mathf.RoundToInt(col.r * 9f) + Mathf.RoundToInt(col.g * 9f) * 10 + Mathf.RoundToInt(col.b * 9f) * 100);
		}

		// Token: 0x0600456F RID: 17775 RVA: 0x00149FB0 File Offset: 0x001481B0
		public static Color UnpackColor(short data)
		{
			return new Color
			{
				r = (float)(data % 10) / 9f,
				g = (float)(data / 10 % 10) / 9f,
				b = (float)(data / 100 % 10) / 9f
			};
		}

		// Token: 0x06004572 RID: 17778 RVA: 0x0014A0EC File Offset: 0x001482EC
		[NetworkRpcStaticWeavedInvoker("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_NotifyNoPartyToMerge(Fusion.NetworkRunner,Fusion.RpcInfo)")]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RPC_NotifyNoPartyToMerge@Invoker(NetworkRunner runner, SimulationMessage* message)
		{
			byte* data = SimulationMessage.GetData(message);
			int num = (RpcHeader.ReadSize(data) + 3) & -4;
			RpcInfo rpcInfo = RpcInfo.FromMessage(runner, message, RpcHostMode.SourceIsServer);
			NetworkBehaviourUtils.InvokeRpc = true;
			FriendshipGroupDetection.RPC_NotifyNoPartyToMerge(runner, rpcInfo);
		}

		// Token: 0x06004573 RID: 17779 RVA: 0x0014A13C File Offset: 0x0014833C
		[NetworkRpcStaticWeavedInvoker("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_NotifyPartyMerging(Fusion.NetworkRunner,Fusion.PlayerRef,System.Int32[],Fusion.RpcInfo)")]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RPC_NotifyPartyMerging@Invoker(NetworkRunner runner, SimulationMessage* message)
		{
			byte* data = SimulationMessage.GetData(message);
			int num = (RpcHeader.ReadSize(data) + 3) & -4;
			PlayerRef target = message->Target;
			int[] array = new int[*(int*)(data + num)];
			num += 4;
			num = ((Native.CopyToArray<int>(array, (void*)(data + num)) + 3) & -4) + num;
			RpcInfo rpcInfo = RpcInfo.FromMessage(runner, message, RpcHostMode.SourceIsServer);
			NetworkBehaviourUtils.InvokeRpc = true;
			FriendshipGroupDetection.RPC_NotifyPartyMerging(runner, target, array, rpcInfo);
		}

		// Token: 0x06004574 RID: 17780 RVA: 0x0014A1E0 File Offset: 0x001483E0
		[NetworkRpcStaticWeavedInvoker("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_PartyMemberIsAboutToGroupJoin(Fusion.NetworkRunner,Fusion.PlayerRef,Fusion.RpcInfo)")]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RPC_PartyMemberIsAboutToGroupJoin@Invoker(NetworkRunner runner, SimulationMessage* message)
		{
			byte* data = SimulationMessage.GetData(message);
			int num = (RpcHeader.ReadSize(data) + 3) & -4;
			PlayerRef target = message->Target;
			RpcInfo rpcInfo = RpcInfo.FromMessage(runner, message, RpcHostMode.SourceIsServer);
			NetworkBehaviourUtils.InvokeRpc = true;
			FriendshipGroupDetection.RPC_PartyMemberIsAboutToGroupJoin(runner, target, rpcInfo);
		}

		// Token: 0x06004575 RID: 17781 RVA: 0x0014A240 File Offset: 0x00148440
		[NetworkRpcStaticWeavedInvoker("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_PartyFormedSuccessfully(Fusion.NetworkRunner,Fusion.PlayerRef,System.String,System.Int16,System.Int32[],System.Boolean,Fusion.RpcInfo)")]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RPC_PartyFormedSuccessfully@Invoker(NetworkRunner runner, SimulationMessage* message)
		{
			byte* data = SimulationMessage.GetData(message);
			int num = (RpcHeader.ReadSize(data) + 3) & -4;
			PlayerRef target = message->Target;
			string text;
			num = ((ReadWriteUtilsForWeaver.ReadStringUtf8NoHash((void*)(data + num), out text) + 3) & -4) + num;
			short num2 = *(short*)(data + num);
			num += (2 + 3) & -4;
			short num3 = num2;
			int[] array = new int[*(int*)(data + num)];
			num += 4;
			num = ((Native.CopyToArray<int>(array, (void*)(data + num)) + 3) & -4) + num;
			bool flag = ReadWriteUtilsForWeaver.ReadBoolean((int*)(data + num));
			num += 4;
			bool flag2 = flag;
			RpcInfo rpcInfo = RpcInfo.FromMessage(runner, message, RpcHostMode.SourceIsServer);
			NetworkBehaviourUtils.InvokeRpc = true;
			FriendshipGroupDetection.RPC_PartyFormedSuccessfully(runner, target, text, num3, array, flag2, rpcInfo);
		}

		// Token: 0x06004576 RID: 17782 RVA: 0x0014A350 File Offset: 0x00148550
		[NetworkRpcStaticWeavedInvoker("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_AddPartyMembers(Fusion.NetworkRunner,Fusion.PlayerRef,System.String,System.Int16,System.Int32[],Fusion.RpcInfo)")]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RPC_AddPartyMembers@Invoker(NetworkRunner runner, SimulationMessage* message)
		{
			byte* data = SimulationMessage.GetData(message);
			int num = (RpcHeader.ReadSize(data) + 3) & -4;
			PlayerRef target = message->Target;
			string text;
			num = ((ReadWriteUtilsForWeaver.ReadStringUtf8NoHash((void*)(data + num), out text) + 3) & -4) + num;
			short num2 = *(short*)(data + num);
			num += (2 + 3) & -4;
			short num3 = num2;
			int[] array = new int[*(int*)(data + num)];
			num += 4;
			num = ((Native.CopyToArray<int>(array, (void*)(data + num)) + 3) & -4) + num;
			RpcInfo rpcInfo = RpcInfo.FromMessage(runner, message, RpcHostMode.SourceIsServer);
			NetworkBehaviourUtils.InvokeRpc = true;
			FriendshipGroupDetection.RPC_AddPartyMembers(runner, target, text, num3, array, rpcInfo);
		}

		// Token: 0x06004577 RID: 17783 RVA: 0x0014A440 File Offset: 0x00148640
		[NetworkRpcStaticWeavedInvoker("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_PlayerLeftParty(Fusion.NetworkRunner,Fusion.PlayerRef,Fusion.RpcInfo)")]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RPC_PlayerLeftParty@Invoker(NetworkRunner runner, SimulationMessage* message)
		{
			byte* data = SimulationMessage.GetData(message);
			int num = (RpcHeader.ReadSize(data) + 3) & -4;
			PlayerRef target = message->Target;
			RpcInfo rpcInfo = RpcInfo.FromMessage(runner, message, RpcHostMode.SourceIsServer);
			NetworkBehaviourUtils.InvokeRpc = true;
			FriendshipGroupDetection.RPC_PlayerLeftParty(runner, target, rpcInfo);
		}

		// Token: 0x06004578 RID: 17784 RVA: 0x0014A4A0 File Offset: 0x001486A0
		[NetworkRpcStaticWeavedInvoker("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_VerifyPartyMember(Fusion.NetworkRunner,Fusion.PlayerRef,Fusion.RpcInfo)")]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RPC_VerifyPartyMember@Invoker(NetworkRunner runner, SimulationMessage* message)
		{
			byte* data = SimulationMessage.GetData(message);
			int num = (RpcHeader.ReadSize(data) + 3) & -4;
			PlayerRef target = message->Target;
			RpcInfo rpcInfo = RpcInfo.FromMessage(runner, message, RpcHostMode.SourceIsServer);
			NetworkBehaviourUtils.InvokeRpc = true;
			FriendshipGroupDetection.RPC_VerifyPartyMember(runner, target, rpcInfo);
		}

		// Token: 0x06004579 RID: 17785 RVA: 0x0014A500 File Offset: 0x00148700
		[NetworkRpcStaticWeavedInvoker("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_RequestPartyGameMode(Fusion.NetworkRunner,Fusion.PlayerRef,System.String,Fusion.RpcInfo)")]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RPC_RequestPartyGameMode@Invoker(NetworkRunner runner, SimulationMessage* message)
		{
			byte* data = SimulationMessage.GetData(message);
			int num = (RpcHeader.ReadSize(data) + 3) & -4;
			PlayerRef target = message->Target;
			string text;
			num = ((ReadWriteUtilsForWeaver.ReadStringUtf8NoHash((void*)(data + num), out text) + 3) & -4) + num;
			RpcInfo rpcInfo = RpcInfo.FromMessage(runner, message, RpcHostMode.SourceIsServer);
			NetworkBehaviourUtils.InvokeRpc = true;
			FriendshipGroupDetection.RPC_RequestPartyGameMode(runner, target, text, rpcInfo);
		}

		// Token: 0x0600457A RID: 17786 RVA: 0x0014A588 File Offset: 0x00148788
		[NetworkRpcStaticWeavedInvoker("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_NotifyPartyGameModeChanged(Fusion.NetworkRunner,Fusion.PlayerRef,System.String,Fusion.RpcInfo)")]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RPC_NotifyPartyGameModeChanged@Invoker(NetworkRunner runner, SimulationMessage* message)
		{
			byte* data = SimulationMessage.GetData(message);
			int num = (RpcHeader.ReadSize(data) + 3) & -4;
			PlayerRef target = message->Target;
			string text;
			num = ((ReadWriteUtilsForWeaver.ReadStringUtf8NoHash((void*)(data + num), out text) + 3) & -4) + num;
			RpcInfo rpcInfo = RpcInfo.FromMessage(runner, message, RpcHostMode.SourceIsServer);
			NetworkBehaviourUtils.InvokeRpc = true;
			FriendshipGroupDetection.RPC_NotifyPartyGameModeChanged(runner, target, text, rpcInfo);
		}

		// Token: 0x040047E8 RID: 18408
		[SerializeField]
		private float detectionRadius = 0.5f;

		// Token: 0x040047E9 RID: 18409
		[SerializeField]
		private float groupTime = 5f;

		// Token: 0x040047EA RID: 18410
		[SerializeField]
		private float cooldownAfterCreatingGroup = 5f;

		// Token: 0x040047EB RID: 18411
		[SerializeField]
		private float hapticStrength = 1.5f;

		// Token: 0x040047EC RID: 18412
		[SerializeField]
		private float hapticDuration = 2f;

		// Token: 0x040047ED RID: 18413
		public bool debug;

		// Token: 0x040047EE RID: 18414
		public double offset = 0.5;

		// Token: 0x040047EF RID: 18415
		[SerializeField]
		private float m_maxGroupJoinTimeDifference = 1f;

		// Token: 0x040047F0 RID: 18416
		private List<string> myPartyMemberIDs;

		// Token: 0x040047F1 RID: 18417
		private HashSet<string> myPartyMembersHash = new HashSet<string>();

		// Token: 0x040047F6 RID: 18422
		private List<Action<GroupJoinZoneAB>> groupZoneCallbacks = new List<Action<GroupJoinZoneAB>>();

		// Token: 0x040047F7 RID: 18423
		[SerializeField]
		private GTColor.HSVRanges braceletRandomColorHSVRanges;

		// Token: 0x040047F8 RID: 18424
		public GameObject friendshipBubble;

		// Token: 0x040047F9 RID: 18425
		public AudioClip fistBumpInterruptedAudio;

		// Token: 0x040047FA RID: 18426
		private ParticleSystem particleSystem;

		// Token: 0x040047FB RID: 18427
		private AudioSource audioSource;

		// Token: 0x040047FC RID: 18428
		private Queue<FriendshipGroupDetection.PlayerFist> playersToPropagateFrom = new Queue<FriendshipGroupDetection.PlayerFist>();

		// Token: 0x040047FD RID: 18429
		private List<int> playersInProvisionalGroup = new List<int>();

		// Token: 0x040047FE RID: 18430
		private List<int> provisionalGroupUsingLeftHands = new List<int>();

		// Token: 0x040047FF RID: 18431
		private List<int> tempIntList = new List<int>();

		// Token: 0x04004800 RID: 18432
		private bool amFirstProvisionalPlayer;

		// Token: 0x04004801 RID: 18433
		private Dictionary<int, int[]> partyMergeIDs = new Dictionary<int, int[]>();

		// Token: 0x04004802 RID: 18434
		private float groupCreateAfterTimestamp;

		// Token: 0x04004803 RID: 18435
		private float playEffectsAfterTimestamp;

		// Token: 0x04004804 RID: 18436
		[SerializeField]
		private float playEffectsDelay;

		// Token: 0x04004805 RID: 18437
		private float suppressPartyCreationUntilTimestamp;

		// Token: 0x04004807 RID: 18439
		private bool WillJoinLeftHanded;

		// Token: 0x04004808 RID: 18440
		private List<FriendshipGroupDetection.PlayerFist> playersMakingFists = new List<FriendshipGroupDetection.PlayerFist>();

		// Token: 0x04004809 RID: 18441
		private StringBuilder debugStr = new StringBuilder();

		// Token: 0x0400480A RID: 18442
		private float aboutToGroupJoin_CooldownUntilTimestamp;

		// Token: 0x0400480B RID: 18443
		private static Dictionary<int, string> userIdLookup = new Dictionary<int, string>();

		// Token: 0x0400480C RID: 18444
		private static Dictionary<string, Color> tempColorLookup = new Dictionary<string, Color>();

		// Token: 0x02000B0B RID: 2827
		private struct PlayerFist
		{
			// Token: 0x0400480D RID: 18445
			public int actorNumber;

			// Token: 0x0400480E RID: 18446
			public Vector3 position;

			// Token: 0x0400480F RID: 18447
			public bool isLeftHand;
		}
	}
}
