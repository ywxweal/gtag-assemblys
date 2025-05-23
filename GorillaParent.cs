using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;

// Token: 0x02000632 RID: 1586
public class GorillaParent : MonoBehaviour
{
	// Token: 0x06002776 RID: 10102 RVA: 0x000C39D8 File Offset: 0x000C1BD8
	public void Awake()
	{
		if (GorillaParent.instance == null)
		{
			GorillaParent.instance = this;
			GorillaParent.hasInstance = true;
			return;
		}
		if (GorillaParent.instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
	}

	// Token: 0x06002777 RID: 10103 RVA: 0x000C3A13 File Offset: 0x000C1C13
	protected void OnDestroy()
	{
		if (GorillaParent.instance == this)
		{
			GorillaParent.hasInstance = false;
			GorillaParent.instance = null;
		}
	}

	// Token: 0x06002778 RID: 10104 RVA: 0x000C3A32 File Offset: 0x000C1C32
	public void LateUpdate()
	{
		if (RoomSystem.JoinedRoom && GorillaTagger.Instance.myVRRig.IsNull())
		{
			VRRigCache.Instance.InstantiateNetworkObject();
		}
	}

	// Token: 0x06002779 RID: 10105 RVA: 0x000C3A56 File Offset: 0x000C1C56
	public static void ReplicatedClientReady()
	{
		GorillaParent.replicatedClientReady = true;
		Action action = GorillaParent.onReplicatedClientReady;
		if (action == null)
		{
			return;
		}
		action();
	}

	// Token: 0x0600277A RID: 10106 RVA: 0x000C3A6D File Offset: 0x000C1C6D
	public static void OnReplicatedClientReady(Action action)
	{
		if (GorillaParent.replicatedClientReady)
		{
			action();
			return;
		}
		GorillaParent.onReplicatedClientReady = (Action)Delegate.Combine(GorillaParent.onReplicatedClientReady, action);
	}

	// Token: 0x04002BEE RID: 11246
	public GameObject tagUI;

	// Token: 0x04002BEF RID: 11247
	public GameObject playerParent;

	// Token: 0x04002BF0 RID: 11248
	public GameObject vrrigParent;

	// Token: 0x04002BF1 RID: 11249
	[OnEnterPlay_SetNull]
	public static volatile GorillaParent instance;

	// Token: 0x04002BF2 RID: 11250
	[OnEnterPlay_Set(false)]
	public static bool hasInstance;

	// Token: 0x04002BF3 RID: 11251
	public List<VRRig> vrrigs;

	// Token: 0x04002BF4 RID: 11252
	public Dictionary<NetPlayer, VRRig> vrrigDict = new Dictionary<NetPlayer, VRRig>();

	// Token: 0x04002BF5 RID: 11253
	private int i;

	// Token: 0x04002BF6 RID: 11254
	private static bool replicatedClientReady;

	// Token: 0x04002BF7 RID: 11255
	private static Action onReplicatedClientReady;
}
