using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Fusion;
using Photon.Realtime;

// Token: 0x020002B9 RID: 697
public class RoomConfig
{
	// Token: 0x170001D7 RID: 471
	// (get) Token: 0x060010BE RID: 4286 RVA: 0x000503FF File Offset: 0x0004E5FF
	public bool IsJoiningWithFriends
	{
		get
		{
			return this.joinFriendIDs != null && this.joinFriendIDs.Length != 0;
		}
	}

	// Token: 0x060010BF RID: 4287 RVA: 0x00050418 File Offset: 0x0004E618
	public void SetFriendIDs(List<string> friendIDs)
	{
		for (int i = 0; i < friendIDs.Count; i++)
		{
			if (friendIDs[i] == NetworkSystem.Instance.GetMyNickName())
			{
				friendIDs.RemoveAt(i);
				i--;
			}
		}
		this.joinFriendIDs = new string[friendIDs.Count];
		for (int j = 0; j < friendIDs.Count; j++)
		{
			this.joinFriendIDs[j] = friendIDs[j];
		}
	}

	// Token: 0x060010C0 RID: 4288 RVA: 0x0005048A File Offset: 0x0004E68A
	public void ClearExpectedUsers()
	{
		if (this.joinFriendIDs == null || this.joinFriendIDs.Length == 0)
		{
			return;
		}
		this.joinFriendIDs = new string[0];
	}

	// Token: 0x060010C1 RID: 4289 RVA: 0x000504AC File Offset: 0x0004E6AC
	public RoomOptions ToPUNOpts()
	{
		return new RoomOptions
		{
			IsVisible = this.isPublic,
			IsOpen = this.isJoinable,
			MaxPlayers = this.MaxPlayers,
			CustomRoomProperties = this.CustomProps,
			PublishUserId = true,
			CustomRoomPropertiesForLobby = this.AutoCustomLobbyProps()
		};
	}

	// Token: 0x060010C2 RID: 4290 RVA: 0x00050501 File Offset: 0x0004E701
	public void SetFusionOpts(NetworkRunner runnerInst)
	{
		runnerInst.SessionInfo.IsVisible = this.isPublic;
		runnerInst.SessionInfo.IsOpen = this.isJoinable;
	}

	// Token: 0x060010C3 RID: 4291 RVA: 0x00050525 File Offset: 0x0004E725
	public static RoomConfig SPConfig()
	{
		return new RoomConfig
		{
			isPublic = false,
			isJoinable = false,
			MaxPlayers = 1
		};
	}

	// Token: 0x060010C4 RID: 4292 RVA: 0x00050541 File Offset: 0x0004E741
	public static RoomConfig AnyPublicConfig()
	{
		return new RoomConfig
		{
			isPublic = true,
			isJoinable = true,
			createIfMissing = true,
			MaxPlayers = 10
		};
	}

	// Token: 0x060010C5 RID: 4293 RVA: 0x00050568 File Offset: 0x0004E768
	private string[] AutoCustomLobbyProps()
	{
		string[] array = new string[this.CustomProps.Count];
		int num = 0;
		foreach (DictionaryEntry dictionaryEntry in this.CustomProps)
		{
			array[num] = (string)dictionaryEntry.Key;
			num++;
		}
		return array;
	}

	// Token: 0x04001302 RID: 4866
	public const string Room_GameModePropKey = "gameMode";

	// Token: 0x04001303 RID: 4867
	public const string Room_PlatformPropKey = "platform";

	// Token: 0x04001304 RID: 4868
	public bool isPublic;

	// Token: 0x04001305 RID: 4869
	public bool isJoinable;

	// Token: 0x04001306 RID: 4870
	public byte MaxPlayers;

	// Token: 0x04001307 RID: 4871
	public Hashtable CustomProps = new Hashtable();

	// Token: 0x04001308 RID: 4872
	public bool createIfMissing;

	// Token: 0x04001309 RID: 4873
	public string[] joinFriendIDs;
}
