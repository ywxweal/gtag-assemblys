using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020008CF RID: 2255
public class FriendSystem : MonoBehaviour
{
	// Token: 0x17000576 RID: 1398
	// (get) Token: 0x060036F6 RID: 14070 RVA: 0x00109D52 File Offset: 0x00107F52
	public FriendSystem.PlayerPrivacy LocalPlayerPrivacy
	{
		get
		{
			return this.localPlayerPrivacy;
		}
	}

	// Token: 0x14000062 RID: 98
	// (add) Token: 0x060036F7 RID: 14071 RVA: 0x00109D5C File Offset: 0x00107F5C
	// (remove) Token: 0x060036F8 RID: 14072 RVA: 0x00109D94 File Offset: 0x00107F94
	public event Action<List<FriendBackendController.Friend>> OnFriendListRefresh;

	// Token: 0x060036F9 RID: 14073 RVA: 0x00109DCC File Offset: 0x00107FCC
	public void SetLocalPlayerPrivacy(FriendSystem.PlayerPrivacy privacyState)
	{
		this.localPlayerPrivacy = privacyState;
		FriendBackendController.PrivacyState privacyState2;
		switch (privacyState)
		{
		default:
			privacyState2 = FriendBackendController.PrivacyState.VISIBLE;
			break;
		case FriendSystem.PlayerPrivacy.PublicOnly:
			privacyState2 = FriendBackendController.PrivacyState.PUBLIC_ONLY;
			break;
		case FriendSystem.PlayerPrivacy.Hidden:
			privacyState2 = FriendBackendController.PrivacyState.HIDDEN;
			break;
		}
		FriendBackendController.Instance.SetPrivacyState(privacyState2);
	}

	// Token: 0x060036FA RID: 14074 RVA: 0x00109E09 File Offset: 0x00108009
	public void RefreshFriendsList()
	{
		FriendBackendController.Instance.GetFriends();
	}

	// Token: 0x060036FB RID: 14075 RVA: 0x00109E18 File Offset: 0x00108018
	public void SendFriendRequest(NetPlayer targetPlayer, GTZone stationZone, FriendSystem.FriendRequestCallback callback)
	{
		FriendSystem.FriendRequestData friendRequestData = new FriendSystem.FriendRequestData
		{
			completionCallback = callback,
			sendingPlayerId = NetworkSystem.Instance.LocalPlayer.UserId.GetHashCode(),
			targetPlayerId = targetPlayer.UserId.GetHashCode(),
			localTimeSent = Time.time,
			zone = stationZone
		};
		this.pendingFriendRequests.Add(friendRequestData);
		FriendBackendController.Instance.AddFriend(targetPlayer);
	}

	// Token: 0x060036FC RID: 14076 RVA: 0x00109E94 File Offset: 0x00108094
	public void RemoveFriend(FriendBackendController.Friend friend, FriendSystem.FriendRemovalCallback callback = null)
	{
		this.pendingFriendRemovals.Add(new FriendSystem.FriendRemovalData
		{
			completionCallback = callback,
			targetPlayerId = friend.Presence.FriendLinkId.GetHashCode(),
			localTimeSent = Time.time
		});
		FriendBackendController.Instance.RemoveFriend(friend);
	}

	// Token: 0x060036FD RID: 14077 RVA: 0x00109EF0 File Offset: 0x001080F0
	public bool HasPendingFriendRequest(GTZone zone, int senderId)
	{
		for (int i = 0; i < this.pendingFriendRequests.Count; i++)
		{
			if (this.pendingFriendRequests[i].zone == zone && this.pendingFriendRequests[i].sendingPlayerId == senderId)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060036FE RID: 14078 RVA: 0x00109F40 File Offset: 0x00108140
	public bool CheckFriendshipWithPlayer(int targetActorNumber)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(targetActorNumber);
		if (player != null)
		{
			int hashCode = player.UserId.GetHashCode();
			List<FriendBackendController.Friend> friendsList = FriendBackendController.Instance.FriendsList;
			for (int i = 0; i < friendsList.Count; i++)
			{
				if (friendsList[i] != null && friendsList[i].Presence != null && friendsList[i].Presence.FriendLinkId.GetHashCode() == hashCode)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x060036FF RID: 14079 RVA: 0x00109FB9 File Offset: 0x001081B9
	private void Awake()
	{
		if (FriendSystem.Instance == null)
		{
			FriendSystem.Instance = this;
			return;
		}
		Object.Destroy(this);
	}

	// Token: 0x06003700 RID: 14080 RVA: 0x00109FDC File Offset: 0x001081DC
	private void Start()
	{
		FriendBackendController.Instance.OnGetFriendsComplete += this.OnGetFriendsReturned;
		FriendBackendController.Instance.OnAddFriendComplete += this.OnAddFriendReturned;
		FriendBackendController.Instance.OnRemoveFriendComplete += this.OnRemoveFriendReturned;
	}

	// Token: 0x06003701 RID: 14081 RVA: 0x0010A034 File Offset: 0x00108234
	private void OnDestroy()
	{
		if (FriendBackendController.Instance != null)
		{
			FriendBackendController.Instance.OnGetFriendsComplete -= this.OnGetFriendsReturned;
			FriendBackendController.Instance.OnAddFriendComplete -= this.OnAddFriendReturned;
			FriendBackendController.Instance.OnRemoveFriendComplete -= this.OnRemoveFriendReturned;
		}
	}

	// Token: 0x06003702 RID: 14082 RVA: 0x0010A098 File Offset: 0x00108298
	private void OnGetFriendsReturned(bool succeeded)
	{
		if (succeeded)
		{
			this.lastFriendsListRefresh = Time.time;
			switch (FriendBackendController.Instance.MyPrivacyState)
			{
			default:
				this.localPlayerPrivacy = FriendSystem.PlayerPrivacy.Visible;
				break;
			case FriendBackendController.PrivacyState.PUBLIC_ONLY:
				this.localPlayerPrivacy = FriendSystem.PlayerPrivacy.PublicOnly;
				break;
			case FriendBackendController.PrivacyState.HIDDEN:
				this.localPlayerPrivacy = FriendSystem.PlayerPrivacy.Hidden;
				break;
			}
			Action<List<FriendBackendController.Friend>> onFriendListRefresh = this.OnFriendListRefresh;
			if (onFriendListRefresh == null)
			{
				return;
			}
			onFriendListRefresh(FriendBackendController.Instance.FriendsList);
		}
	}

	// Token: 0x06003703 RID: 14083 RVA: 0x0010A108 File Offset: 0x00108308
	private void OnAddFriendReturned(NetPlayer targetPlayer, bool succeeded)
	{
		int hashCode = targetPlayer.UserId.GetHashCode();
		this.indexesToRemove.Clear();
		for (int i = 0; i < this.pendingFriendRequests.Count; i++)
		{
			if (this.pendingFriendRequests[i].targetPlayerId == hashCode)
			{
				FriendSystem.FriendRequestCallback completionCallback = this.pendingFriendRequests[i].completionCallback;
				if (completionCallback != null)
				{
					completionCallback(this.pendingFriendRequests[i].zone, this.pendingFriendRequests[i].sendingPlayerId, this.pendingFriendRequests[i].targetPlayerId, succeeded);
				}
				this.indexesToRemove.Add(i);
			}
			else if (this.pendingFriendRequests[i].localTimeSent + this.friendRequestExpirationTime < Time.time)
			{
				this.indexesToRemove.Add(i);
			}
		}
		for (int j = this.indexesToRemove.Count - 1; j >= 0; j--)
		{
			this.pendingFriendRequests.RemoveAt(this.indexesToRemove[j]);
		}
	}

	// Token: 0x06003704 RID: 14084 RVA: 0x0010A214 File Offset: 0x00108414
	private void OnRemoveFriendReturned(FriendBackendController.Friend friend, bool succeeded)
	{
		if (friend != null && friend.Presence != null)
		{
			int hashCode = friend.Presence.FriendLinkId.GetHashCode();
			this.indexesToRemove.Clear();
			for (int i = 0; i < this.pendingFriendRemovals.Count; i++)
			{
				if (this.pendingFriendRemovals[i].targetPlayerId == hashCode)
				{
					FriendSystem.FriendRemovalCallback completionCallback = this.pendingFriendRemovals[i].completionCallback;
					if (completionCallback != null)
					{
						completionCallback(hashCode, succeeded);
					}
					this.indexesToRemove.Add(i);
				}
				else if (this.pendingFriendRemovals[i].localTimeSent + this.friendRequestExpirationTime < Time.time)
				{
					this.indexesToRemove.Add(i);
				}
			}
			for (int j = this.indexesToRemove.Count - 1; j >= 0; j--)
			{
				this.pendingFriendRemovals.RemoveAt(this.indexesToRemove[j]);
			}
		}
	}

	// Token: 0x04003C7F RID: 15487
	[OnEnterPlay_SetNull]
	public static volatile FriendSystem Instance;

	// Token: 0x04003C80 RID: 15488
	[SerializeField]
	private float friendRequestExpirationTime = 10f;

	// Token: 0x04003C81 RID: 15489
	private FriendSystem.PlayerPrivacy localPlayerPrivacy;

	// Token: 0x04003C82 RID: 15490
	private List<FriendSystem.FriendRequestData> pendingFriendRequests = new List<FriendSystem.FriendRequestData>();

	// Token: 0x04003C83 RID: 15491
	private List<FriendSystem.FriendRemovalData> pendingFriendRemovals = new List<FriendSystem.FriendRemovalData>();

	// Token: 0x04003C84 RID: 15492
	private List<int> indexesToRemove = new List<int>();

	// Token: 0x04003C86 RID: 15494
	private float lastFriendsListRefresh;

	// Token: 0x020008D0 RID: 2256
	// (Invoke) Token: 0x06003707 RID: 14087
	public delegate void FriendRequestCallback(GTZone zone, int localId, int friendId, bool success);

	// Token: 0x020008D1 RID: 2257
	private struct FriendRequestData
	{
		// Token: 0x04003C87 RID: 15495
		public GTZone zone;

		// Token: 0x04003C88 RID: 15496
		public int sendingPlayerId;

		// Token: 0x04003C89 RID: 15497
		public int targetPlayerId;

		// Token: 0x04003C8A RID: 15498
		public float localTimeSent;

		// Token: 0x04003C8B RID: 15499
		public FriendSystem.FriendRequestCallback completionCallback;
	}

	// Token: 0x020008D2 RID: 2258
	// (Invoke) Token: 0x0600370B RID: 14091
	public delegate void FriendRemovalCallback(int friendId, bool success);

	// Token: 0x020008D3 RID: 2259
	private struct FriendRemovalData
	{
		// Token: 0x04003C8C RID: 15500
		public int targetPlayerId;

		// Token: 0x04003C8D RID: 15501
		public float localTimeSent;

		// Token: 0x04003C8E RID: 15502
		public FriendSystem.FriendRemovalCallback completionCallback;
	}

	// Token: 0x020008D4 RID: 2260
	private enum FriendRequestStatus
	{
		// Token: 0x04003C90 RID: 15504
		Pending,
		// Token: 0x04003C91 RID: 15505
		Succeeded,
		// Token: 0x04003C92 RID: 15506
		Failed
	}

	// Token: 0x020008D5 RID: 2261
	public enum PlayerPrivacy
	{
		// Token: 0x04003C94 RID: 15508
		Visible,
		// Token: 0x04003C95 RID: 15509
		PublicOnly,
		// Token: 0x04003C96 RID: 15510
		Hidden
	}
}
