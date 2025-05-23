using System;
using System.Collections.Generic;
using Fusion;
using GorillaGameModes;
using UnityEngine;

// Token: 0x02000278 RID: 632
public class CustomObjectProvider : NetworkObjectProviderDefault
{
	// Token: 0x1700016E RID: 366
	// (get) Token: 0x06000E86 RID: 3718 RVA: 0x0004924C File Offset: 0x0004744C
	private static NetworkObjectBaker Baker
	{
		get
		{
			NetworkObjectBaker networkObjectBaker;
			if ((networkObjectBaker = CustomObjectProvider.baker) == null)
			{
				networkObjectBaker = (CustomObjectProvider.baker = new NetworkObjectBaker());
			}
			return networkObjectBaker;
		}
	}

	// Token: 0x06000E87 RID: 3719 RVA: 0x00049262 File Offset: 0x00047462
	public override NetworkObjectAcquireResult AcquirePrefabInstance(NetworkRunner runner, in NetworkPrefabAcquireContext context, out NetworkObject instance)
	{
		NetworkObjectAcquireResult networkObjectAcquireResult = base.AcquirePrefabInstance(runner, in context, out instance);
		if (networkObjectAcquireResult == NetworkObjectAcquireResult.Success)
		{
			this.IsGameMode(instance);
			return networkObjectAcquireResult;
		}
		instance = null;
		return networkObjectAcquireResult;
	}

	// Token: 0x06000E88 RID: 3720 RVA: 0x0004927C File Offset: 0x0004747C
	private void IsGameMode(NetworkObject instance)
	{
		if (instance.gameObject.GetComponent<GameModeSerializer>() != null)
		{
			global::GorillaGameModes.GameMode.GetGameModeInstance(global::GorillaGameModes.GameMode.GetGameModeKeyFromRoomProp()).AddFusionDataBehaviour(instance);
			CustomObjectProvider.Baker.Bake(instance.gameObject);
		}
	}

	// Token: 0x06000E89 RID: 3721 RVA: 0x000492B2 File Offset: 0x000474B2
	protected override void DestroySceneObject(NetworkRunner runner, NetworkSceneObjectId sceneObjectId, NetworkObject instance)
	{
		if (this.SceneObjects != null && this.SceneObjects.Contains(instance.gameObject))
		{
			return;
		}
		base.DestroySceneObject(runner, sceneObjectId, instance);
	}

	// Token: 0x06000E8A RID: 3722 RVA: 0x000492D9 File Offset: 0x000474D9
	protected override void DestroyPrefabInstance(NetworkRunner runner, NetworkPrefabId prefabId, NetworkObject instance)
	{
		base.DestroyPrefabInstance(runner, prefabId, instance);
	}

	// Token: 0x040011C2 RID: 4546
	public const int GameModeFlag = 1;

	// Token: 0x040011C3 RID: 4547
	public const int PlayerFlag = 2;

	// Token: 0x040011C4 RID: 4548
	private static NetworkObjectBaker baker;

	// Token: 0x040011C5 RID: 4549
	internal List<GameObject> SceneObjects;
}
