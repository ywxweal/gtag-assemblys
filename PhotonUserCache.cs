using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Realtime;

// Token: 0x020008EE RID: 2286
public class PhotonUserCache : IInRoomCallbacks, IMatchmakingCallbacks
{
	// Token: 0x0600377A RID: 14202 RVA: 0x000023F4 File Offset: 0x000005F4
	void IInRoomCallbacks.OnPlayerEnteredRoom(Player newPlayer)
	{
	}

	// Token: 0x0600377B RID: 14203 RVA: 0x000023F4 File Offset: 0x000005F4
	void IMatchmakingCallbacks.OnJoinedRoom()
	{
	}

	// Token: 0x0600377C RID: 14204 RVA: 0x000023F4 File Offset: 0x000005F4
	void IMatchmakingCallbacks.OnLeftRoom()
	{
	}

	// Token: 0x0600377D RID: 14205 RVA: 0x000023F4 File Offset: 0x000005F4
	void IInRoomCallbacks.OnPlayerLeftRoom(Player player)
	{
	}

	// Token: 0x0600377E RID: 14206 RVA: 0x000023F4 File Offset: 0x000005F4
	void IMatchmakingCallbacks.OnCreateRoomFailed(short returnCode, string message)
	{
	}

	// Token: 0x0600377F RID: 14207 RVA: 0x000023F4 File Offset: 0x000005F4
	void IMatchmakingCallbacks.OnJoinRoomFailed(short returnCode, string message)
	{
	}

	// Token: 0x06003780 RID: 14208 RVA: 0x000023F4 File Offset: 0x000005F4
	void IMatchmakingCallbacks.OnCreatedRoom()
	{
	}

	// Token: 0x06003781 RID: 14209 RVA: 0x000023F4 File Offset: 0x000005F4
	void IMatchmakingCallbacks.OnPreLeavingRoom()
	{
	}

	// Token: 0x06003782 RID: 14210 RVA: 0x000023F4 File Offset: 0x000005F4
	void IMatchmakingCallbacks.OnJoinRandomFailed(short returnCode, string message)
	{
	}

	// Token: 0x06003783 RID: 14211 RVA: 0x000023F4 File Offset: 0x000005F4
	void IMatchmakingCallbacks.OnFriendListUpdate(List<FriendInfo> friendList)
	{
	}

	// Token: 0x06003784 RID: 14212 RVA: 0x000023F4 File Offset: 0x000005F4
	void IInRoomCallbacks.OnRoomPropertiesUpdate(Hashtable changedProperties)
	{
	}

	// Token: 0x06003785 RID: 14213 RVA: 0x000023F4 File Offset: 0x000005F4
	void IInRoomCallbacks.OnPlayerPropertiesUpdate(Player player, Hashtable changedProperties)
	{
	}

	// Token: 0x06003786 RID: 14214 RVA: 0x000023F4 File Offset: 0x000005F4
	void IInRoomCallbacks.OnMasterClientSwitched(Player newMasterClient)
	{
	}
}
