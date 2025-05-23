using System;
using System.Collections.Generic;
using GorillaNetworking;
using Photon.Pun;
using UnityEngine;

// Token: 0x020008D6 RID: 2262
public class MockFriendServer : MonoBehaviourPun
{
	// Token: 0x17000577 RID: 1399
	// (get) Token: 0x0600370D RID: 14093 RVA: 0x0010A25A File Offset: 0x0010845A
	public int LocalPlayerId
	{
		get
		{
			return PhotonNetwork.LocalPlayer.UserId.GetHashCode();
		}
	}

	// Token: 0x0600370E RID: 14094 RVA: 0x0010A26B File Offset: 0x0010846B
	private void Awake()
	{
		if (MockFriendServer.Instance == null)
		{
			MockFriendServer.Instance = this;
			PhotonNetwork.AddCallbackTarget(this);
			NetworkSystem.Instance.OnMultiplayerStarted += this.OnMultiplayerStarted;
			return;
		}
		Object.Destroy(this);
	}

	// Token: 0x0600370F RID: 14095 RVA: 0x0010A2A9 File Offset: 0x001084A9
	private void OnMultiplayerStarted()
	{
		this.RegisterLocalPlayer(this.LocalPlayerId);
	}

	// Token: 0x06003710 RID: 14096 RVA: 0x0010A2B8 File Offset: 0x001084B8
	private void Update()
	{
		if (PhotonNetwork.InRoom && base.photonView.IsMine)
		{
			this.indexesToRemove.Clear();
			for (int i = 0; i < this.friendRequests.Count; i++)
			{
				if (this.friendRequests[i].requestTime + this.friendRequestExpirationTime < Time.time)
				{
					this.indexesToRemove.Add(i);
				}
			}
			for (int j = 0; j < this.indexesToRemove.Count; j++)
			{
				this.friendRequests.RemoveAt(this.indexesToRemove[j]);
			}
			this.indexesToRemove.Clear();
			for (int k = 0; k < this.friendRequests.Count; k++)
			{
				if (this.friendRequests[k].requestTime + this.friendRequestExpirationTime < Time.time)
				{
					this.indexesToRemove.Add(k);
				}
				else if (this.friendRequests[k].completionTime < Time.time)
				{
					for (int l = k + 1; l < this.friendRequests.Count; l++)
					{
						int num;
						int num2;
						if (this.friendRequests[l].completionTime < Time.time && this.friendRequests[k].requestorPublicId == this.friendRequests[l].requesteePublicId && this.friendRequests[k].requesteePublicId == this.friendRequests[l].requestorPublicId && this.TryLookupPrivateId(this.friendRequests[k].requestorPublicId, out num) && this.TryLookupPrivateId(this.friendRequests[k].requesteePublicId, out num2))
						{
							this.AddFriend(this.friendRequests[k].requestorPublicId, this.friendRequests[k].requesteePublicId, num, num2);
							this.indexesToRemove.Add(l);
							this.indexesToRemove.Add(k);
							base.photonView.RPC("AddFriendPairRPC", RpcTarget.Others, new object[]
							{
								this.friendRequests[k].requestorPublicId,
								this.friendRequests[k].requesteePublicId,
								num,
								num2
							});
							break;
						}
					}
				}
			}
			for (int m = 0; m < this.indexesToRemove.Count; m++)
			{
				this.friendRequests.RemoveAt(this.indexesToRemove[m]);
			}
		}
	}

	// Token: 0x06003711 RID: 14097 RVA: 0x0010A564 File Offset: 0x00108764
	public void RegisterLocalPlayer(int localPlayerPublicId)
	{
		int hashCode = PlayFabAuthenticator.instance.GetPlayFabPlayerId().GetHashCode();
		if (base.photonView.IsMine)
		{
			this.RegisterLocalPlayerInternal(localPlayerPublicId, hashCode);
			return;
		}
		base.photonView.RPC("RegisterLocalPlayerRPC", RpcTarget.MasterClient, new object[] { localPlayerPublicId, hashCode });
	}

	// Token: 0x06003712 RID: 14098 RVA: 0x0010A5C4 File Offset: 0x001087C4
	public void RequestAddFriend(int targetPlayerId)
	{
		if (base.photonView.IsMine)
		{
			this.RequestAddFriendInternal(this.LocalPlayerId, targetPlayerId);
			return;
		}
		base.photonView.RPC("RequestAddFriendRPC", RpcTarget.MasterClient, new object[] { this.LocalPlayerId, targetPlayerId });
	}

	// Token: 0x06003713 RID: 14099 RVA: 0x0010A61C File Offset: 0x0010881C
	public void RequestRemoveFriend(int targetPlayerId)
	{
		if (base.photonView.IsMine)
		{
			this.RequestRemoveFriendInternal(this.LocalPlayerId, targetPlayerId);
			return;
		}
		base.photonView.RPC("RequestRemoveFriendRPC", RpcTarget.MasterClient, new object[] { this.LocalPlayerId, targetPlayerId });
	}

	// Token: 0x06003714 RID: 14100 RVA: 0x0010A674 File Offset: 0x00108874
	public void GetFriendList(List<int> friendListResult)
	{
		int localPlayerId = this.LocalPlayerId;
		friendListResult.Clear();
		for (int i = 0; i < this.friendPairList.Count; i++)
		{
			if (this.friendPairList[i].publicIdPlayerA == localPlayerId)
			{
				friendListResult.Add(this.friendPairList[i].publicIdPlayerB);
			}
			else if (this.friendPairList[i].publicIdPlayerB == localPlayerId)
			{
				friendListResult.Add(this.friendPairList[i].publicIdPlayerA);
			}
		}
	}

	// Token: 0x06003715 RID: 14101 RVA: 0x0010A6FC File Offset: 0x001088FC
	private void RequestAddFriendInternal(int localPlayerPublicId, int otherPlayerPublicId)
	{
		if (base.photonView.IsMine)
		{
			bool flag = false;
			for (int i = 0; i < this.friendRequests.Count; i++)
			{
				if (this.friendRequests[i].requestorPublicId == localPlayerPublicId && this.friendRequests[i].requesteePublicId == otherPlayerPublicId)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				float time = Time.time;
				float num = Random.Range(this.friendRequestCompletionDelayRange.x, this.friendRequestCompletionDelayRange.y);
				this.friendRequests.Add(new MockFriendServer.FriendRequest
				{
					requestorPublicId = localPlayerPublicId,
					requesteePublicId = otherPlayerPublicId,
					requestTime = time,
					completionTime = time + num
				});
			}
		}
	}

	// Token: 0x06003716 RID: 14102 RVA: 0x0010A7B9 File Offset: 0x001089B9
	[PunRPC]
	public void RequestAddFriendRPC(int localPlayerPublicId, int otherPlayerPublicId, PhotonMessageInfo info)
	{
		this.RequestAddFriendInternal(localPlayerPublicId, otherPlayerPublicId);
	}

	// Token: 0x06003717 RID: 14103 RVA: 0x0010A7C4 File Offset: 0x001089C4
	private void RequestRemoveFriendInternal(int localPlayerPublicId, int otherPlayerPublicId)
	{
		int num;
		int num2;
		if (base.photonView.IsMine && this.TryLookupPrivateId(localPlayerPublicId, out num) && this.TryLookupPrivateId(otherPlayerPublicId, out num2))
		{
			this.RemoveFriend(num, num2);
		}
	}

	// Token: 0x06003718 RID: 14104 RVA: 0x0010A7FC File Offset: 0x001089FC
	[PunRPC]
	public void RequestRemoveFriendRPC(int localPlayerPublicId, int otherPlayerPublicId, PhotonMessageInfo info)
	{
		this.RequestRemoveFriendInternal(localPlayerPublicId, otherPlayerPublicId);
	}

	// Token: 0x06003719 RID: 14105 RVA: 0x0010A808 File Offset: 0x00108A08
	private void RegisterLocalPlayerInternal(int publicId, int privateId)
	{
		if (base.photonView.IsMine)
		{
			bool flag = false;
			for (int i = 0; i < this.privateIdLookup.Count; i++)
			{
				if (publicId == this.privateIdLookup[i].playerPublicId || privateId == this.privateIdLookup[i].playerPrivateId)
				{
					MockFriendServer.PrivateIdEncryptionPlaceholder privateIdEncryptionPlaceholder = this.privateIdLookup[i];
					privateIdEncryptionPlaceholder.playerPublicId = publicId;
					privateIdEncryptionPlaceholder.playerPrivateId = privateId;
					this.privateIdLookup[i] = privateIdEncryptionPlaceholder;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				this.privateIdLookup.Add(new MockFriendServer.PrivateIdEncryptionPlaceholder
				{
					playerPublicId = publicId,
					playerPrivateId = privateId
				});
			}
		}
	}

	// Token: 0x0600371A RID: 14106 RVA: 0x0010A8BA File Offset: 0x00108ABA
	[PunRPC]
	public void RegisterLocalPlayerRPC(int playerPublicId, int playerPrivateId, PhotonMessageInfo info)
	{
		this.RegisterLocalPlayerInternal(playerPublicId, playerPrivateId);
	}

	// Token: 0x0600371B RID: 14107 RVA: 0x0010A8C4 File Offset: 0x00108AC4
	[PunRPC]
	public void AddFriendPairRPC(int publicIdA, int publicIdB, int privateIdA, int privateIdB, PhotonMessageInfo info)
	{
		this.AddFriend(publicIdA, publicIdB, privateIdA, privateIdB);
	}

	// Token: 0x0600371C RID: 14108 RVA: 0x0010A8D4 File Offset: 0x00108AD4
	private void AddFriend(int publicIdA, int publicIdB, int privateIdA, int privateIdB)
	{
		for (int i = 0; i < this.friendPairList.Count; i++)
		{
			if ((this.friendPairList[i].privateIdPlayerA == privateIdA && this.friendPairList[i].privateIdPlayerB == privateIdB) || (this.friendPairList[i].privateIdPlayerA == privateIdB && this.friendPairList[i].privateIdPlayerB == privateIdA))
			{
				return;
			}
		}
		this.friendPairList.Add(new MockFriendServer.FriendPair
		{
			publicIdPlayerA = publicIdA,
			publicIdPlayerB = publicIdB,
			privateIdPlayerA = privateIdA,
			privateIdPlayerB = privateIdB
		});
	}

	// Token: 0x0600371D RID: 14109 RVA: 0x0010A980 File Offset: 0x00108B80
	private void RemoveFriend(int privateIdA, int privateIdB)
	{
		this.indexesToRemove.Clear();
		for (int i = 0; i < this.friendPairList.Count; i++)
		{
			if ((this.friendPairList[i].privateIdPlayerA == privateIdA && this.friendPairList[i].privateIdPlayerB == privateIdB) || (this.friendPairList[i].privateIdPlayerA == privateIdB && this.friendPairList[i].privateIdPlayerB == privateIdA))
			{
				this.indexesToRemove.Add(i);
			}
		}
		for (int j = 0; j < this.friendPairList.Count; j++)
		{
			this.friendPairList.RemoveAt(this.indexesToRemove[j]);
		}
	}

	// Token: 0x0600371E RID: 14110 RVA: 0x0010AA38 File Offset: 0x00108C38
	private bool TryLookupPrivateId(int publicId, out int privateId)
	{
		for (int i = 0; i < this.privateIdLookup.Count; i++)
		{
			if (this.privateIdLookup[i].playerPublicId == publicId)
			{
				privateId = this.privateIdLookup[i].playerPrivateId;
				return true;
			}
		}
		privateId = -1;
		return false;
	}

	// Token: 0x04003C96 RID: 15510
	[OnEnterPlay_SetNull]
	public static volatile MockFriendServer Instance;

	// Token: 0x04003C97 RID: 15511
	[SerializeField]
	private Vector2 friendRequestCompletionDelayRange = new Vector2(0.5f, 1f);

	// Token: 0x04003C98 RID: 15512
	[SerializeField]
	private float friendRequestExpirationTime = 10f;

	// Token: 0x04003C99 RID: 15513
	private List<MockFriendServer.FriendPair> friendPairList = new List<MockFriendServer.FriendPair>();

	// Token: 0x04003C9A RID: 15514
	private List<MockFriendServer.PrivateIdEncryptionPlaceholder> privateIdLookup = new List<MockFriendServer.PrivateIdEncryptionPlaceholder>();

	// Token: 0x04003C9B RID: 15515
	private List<MockFriendServer.FriendRequest> friendRequests = new List<MockFriendServer.FriendRequest>();

	// Token: 0x04003C9C RID: 15516
	private List<int> indexesToRemove = new List<int>();

	// Token: 0x020008D7 RID: 2263
	public struct FriendPair
	{
		// Token: 0x04003C9D RID: 15517
		public int publicIdPlayerA;

		// Token: 0x04003C9E RID: 15518
		public int publicIdPlayerB;

		// Token: 0x04003C9F RID: 15519
		public int privateIdPlayerA;

		// Token: 0x04003CA0 RID: 15520
		public int privateIdPlayerB;
	}

	// Token: 0x020008D8 RID: 2264
	public struct PrivateIdEncryptionPlaceholder
	{
		// Token: 0x04003CA1 RID: 15521
		public int playerPublicId;

		// Token: 0x04003CA2 RID: 15522
		public int playerPrivateId;
	}

	// Token: 0x020008D9 RID: 2265
	public struct FriendRequest
	{
		// Token: 0x04003CA3 RID: 15523
		public int requestorPublicId;

		// Token: 0x04003CA4 RID: 15524
		public int requesteePublicId;

		// Token: 0x04003CA5 RID: 15525
		public float requestTime;

		// Token: 0x04003CA6 RID: 15526
		public float completionTime;
	}
}
