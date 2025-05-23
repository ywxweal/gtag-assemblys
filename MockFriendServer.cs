using System;
using System.Collections.Generic;
using GorillaNetworking;
using Photon.Pun;
using UnityEngine;

// Token: 0x020008D6 RID: 2262
public class MockFriendServer : MonoBehaviourPun
{
	// Token: 0x17000577 RID: 1399
	// (get) Token: 0x0600370E RID: 14094 RVA: 0x0010A332 File Offset: 0x00108532
	public int LocalPlayerId
	{
		get
		{
			return PhotonNetwork.LocalPlayer.UserId.GetHashCode();
		}
	}

	// Token: 0x0600370F RID: 14095 RVA: 0x0010A343 File Offset: 0x00108543
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

	// Token: 0x06003710 RID: 14096 RVA: 0x0010A381 File Offset: 0x00108581
	private void OnMultiplayerStarted()
	{
		this.RegisterLocalPlayer(this.LocalPlayerId);
	}

	// Token: 0x06003711 RID: 14097 RVA: 0x0010A390 File Offset: 0x00108590
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

	// Token: 0x06003712 RID: 14098 RVA: 0x0010A63C File Offset: 0x0010883C
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

	// Token: 0x06003713 RID: 14099 RVA: 0x0010A69C File Offset: 0x0010889C
	public void RequestAddFriend(int targetPlayerId)
	{
		if (base.photonView.IsMine)
		{
			this.RequestAddFriendInternal(this.LocalPlayerId, targetPlayerId);
			return;
		}
		base.photonView.RPC("RequestAddFriendRPC", RpcTarget.MasterClient, new object[] { this.LocalPlayerId, targetPlayerId });
	}

	// Token: 0x06003714 RID: 14100 RVA: 0x0010A6F4 File Offset: 0x001088F4
	public void RequestRemoveFriend(int targetPlayerId)
	{
		if (base.photonView.IsMine)
		{
			this.RequestRemoveFriendInternal(this.LocalPlayerId, targetPlayerId);
			return;
		}
		base.photonView.RPC("RequestRemoveFriendRPC", RpcTarget.MasterClient, new object[] { this.LocalPlayerId, targetPlayerId });
	}

	// Token: 0x06003715 RID: 14101 RVA: 0x0010A74C File Offset: 0x0010894C
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

	// Token: 0x06003716 RID: 14102 RVA: 0x0010A7D4 File Offset: 0x001089D4
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

	// Token: 0x06003717 RID: 14103 RVA: 0x0010A891 File Offset: 0x00108A91
	[PunRPC]
	public void RequestAddFriendRPC(int localPlayerPublicId, int otherPlayerPublicId, PhotonMessageInfo info)
	{
		this.RequestAddFriendInternal(localPlayerPublicId, otherPlayerPublicId);
	}

	// Token: 0x06003718 RID: 14104 RVA: 0x0010A89C File Offset: 0x00108A9C
	private void RequestRemoveFriendInternal(int localPlayerPublicId, int otherPlayerPublicId)
	{
		int num;
		int num2;
		if (base.photonView.IsMine && this.TryLookupPrivateId(localPlayerPublicId, out num) && this.TryLookupPrivateId(otherPlayerPublicId, out num2))
		{
			this.RemoveFriend(num, num2);
		}
	}

	// Token: 0x06003719 RID: 14105 RVA: 0x0010A8D4 File Offset: 0x00108AD4
	[PunRPC]
	public void RequestRemoveFriendRPC(int localPlayerPublicId, int otherPlayerPublicId, PhotonMessageInfo info)
	{
		this.RequestRemoveFriendInternal(localPlayerPublicId, otherPlayerPublicId);
	}

	// Token: 0x0600371A RID: 14106 RVA: 0x0010A8E0 File Offset: 0x00108AE0
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

	// Token: 0x0600371B RID: 14107 RVA: 0x0010A992 File Offset: 0x00108B92
	[PunRPC]
	public void RegisterLocalPlayerRPC(int playerPublicId, int playerPrivateId, PhotonMessageInfo info)
	{
		this.RegisterLocalPlayerInternal(playerPublicId, playerPrivateId);
	}

	// Token: 0x0600371C RID: 14108 RVA: 0x0010A99C File Offset: 0x00108B9C
	[PunRPC]
	public void AddFriendPairRPC(int publicIdA, int publicIdB, int privateIdA, int privateIdB, PhotonMessageInfo info)
	{
		this.AddFriend(publicIdA, publicIdB, privateIdA, privateIdB);
	}

	// Token: 0x0600371D RID: 14109 RVA: 0x0010A9AC File Offset: 0x00108BAC
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

	// Token: 0x0600371E RID: 14110 RVA: 0x0010AA58 File Offset: 0x00108C58
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

	// Token: 0x0600371F RID: 14111 RVA: 0x0010AB10 File Offset: 0x00108D10
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

	// Token: 0x04003C97 RID: 15511
	[OnEnterPlay_SetNull]
	public static volatile MockFriendServer Instance;

	// Token: 0x04003C98 RID: 15512
	[SerializeField]
	private Vector2 friendRequestCompletionDelayRange = new Vector2(0.5f, 1f);

	// Token: 0x04003C99 RID: 15513
	[SerializeField]
	private float friendRequestExpirationTime = 10f;

	// Token: 0x04003C9A RID: 15514
	private List<MockFriendServer.FriendPair> friendPairList = new List<MockFriendServer.FriendPair>();

	// Token: 0x04003C9B RID: 15515
	private List<MockFriendServer.PrivateIdEncryptionPlaceholder> privateIdLookup = new List<MockFriendServer.PrivateIdEncryptionPlaceholder>();

	// Token: 0x04003C9C RID: 15516
	private List<MockFriendServer.FriendRequest> friendRequests = new List<MockFriendServer.FriendRequest>();

	// Token: 0x04003C9D RID: 15517
	private List<int> indexesToRemove = new List<int>();

	// Token: 0x020008D7 RID: 2263
	public struct FriendPair
	{
		// Token: 0x04003C9E RID: 15518
		public int publicIdPlayerA;

		// Token: 0x04003C9F RID: 15519
		public int publicIdPlayerB;

		// Token: 0x04003CA0 RID: 15520
		public int privateIdPlayerA;

		// Token: 0x04003CA1 RID: 15521
		public int privateIdPlayerB;
	}

	// Token: 0x020008D8 RID: 2264
	public struct PrivateIdEncryptionPlaceholder
	{
		// Token: 0x04003CA2 RID: 15522
		public int playerPublicId;

		// Token: 0x04003CA3 RID: 15523
		public int playerPrivateId;
	}

	// Token: 0x020008D9 RID: 2265
	public struct FriendRequest
	{
		// Token: 0x04003CA4 RID: 15524
		public int requestorPublicId;

		// Token: 0x04003CA5 RID: 15525
		public int requesteePublicId;

		// Token: 0x04003CA6 RID: 15526
		public float requestTime;

		// Token: 0x04003CA7 RID: 15527
		public float completionTime;
	}
}
