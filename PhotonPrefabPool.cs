﻿using System;
using System.Collections.Generic;
using GorillaExtensions;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.Unity;
using UnityEngine;

// Token: 0x020005E3 RID: 1507
public class PhotonPrefabPool : MonoBehaviour, IPunPrefabPoolVerify, IPunPrefabPool, ITickSystemPre
{
	// Token: 0x17000377 RID: 887
	// (get) Token: 0x060024D5 RID: 9429 RVA: 0x000B87DA File Offset: 0x000B69DA
	// (set) Token: 0x060024D6 RID: 9430 RVA: 0x000B87E2 File Offset: 0x000B69E2
	bool ITickSystemPre.PreTickRunning { get; set; }

	// Token: 0x060024D7 RID: 9431 RVA: 0x000B87EB File Offset: 0x000B69EB
	private void Awake()
	{
		RoomSystem.LeftRoomEvent = (Action)Delegate.Combine(RoomSystem.LeftRoomEvent, new Action(this.OnLeftRoom));
	}

	// Token: 0x060024D8 RID: 9432 RVA: 0x000B8810 File Offset: 0x000B6A10
	private void Start()
	{
		PhotonNetwork.PrefabPool = this;
		NetworkSystem.Instance.AddRemoteVoiceAddedCallback(new Action<RemoteVoiceLink>(this.CheckVOIPSettings));
		for (int i = 0; i < this.networkPrefabsData.Length; i++)
		{
			ref PrefabType ptr = ref this.networkPrefabsData[i];
			if (ptr.prefab)
			{
				if (string.IsNullOrEmpty(ptr.prefabName))
				{
					ptr.prefabName = ptr.prefab.name;
				}
				int num = ptr.prefab.GetComponentsInChildren<PhotonView>().Length;
				ptr.photonViewCount = num;
				this.networkPrefabs.Add(ptr.prefabName, ptr);
			}
		}
	}

	// Token: 0x060024D9 RID: 9433 RVA: 0x000B88B0 File Offset: 0x000B6AB0
	bool IPunPrefabPoolVerify.VerifyInstantiation(Player sender, string prefabName, Vector3 position, Quaternion rotation, int[] viewIDs, out GameObject prefab)
	{
		prefab = null;
		if (viewIDs != null)
		{
			float num = 10000f;
			PrefabType prefabType;
			if ((in position).IsValid(in num) && (in rotation).IsValid() && this.networkPrefabs.TryGetValue(prefabName, out prefabType) && viewIDs.Length == prefabType.photonViewCount)
			{
				int num2 = ((sender != null) ? sender.ActorNumber : 0);
				int num3 = viewIDs[0] / PhotonNetwork.MAX_VIEW_IDS;
				for (int i = 0; i < viewIDs.Length; i++)
				{
					int num4 = viewIDs[i];
					if (PhotonNetwork.ViewIDExists(num4))
					{
						return false;
					}
					for (int j = 0; j < viewIDs.Length; j++)
					{
						if (j != i && viewIDs[j] == num4)
						{
							return false;
						}
					}
					int num5 = num4 / PhotonNetwork.MAX_VIEW_IDS;
					if (num5 != num3)
					{
						return false;
					}
					if (num5 == 0)
					{
						if (!prefabType.roomObject)
						{
							return false;
						}
					}
					else if (num5 != num2)
					{
						return false;
					}
				}
				prefab = prefabType.prefab;
				return true;
			}
		}
		return false;
	}

	// Token: 0x060024DA RID: 9434 RVA: 0x000B8990 File Offset: 0x000B6B90
	GameObject IPunPrefabPoolVerify.Instantiate(GameObject prefabInstance, Vector3 position, Quaternion rotation)
	{
		bool activeSelf = prefabInstance.activeSelf;
		if (activeSelf)
		{
			prefabInstance.SetActive(false);
		}
		GameObject gameObject = Object.Instantiate<GameObject>(prefabInstance, position, rotation);
		this.netInstantiedObjects.Add(gameObject);
		if (activeSelf)
		{
			prefabInstance.SetActive(true);
		}
		return gameObject;
	}

	// Token: 0x060024DB RID: 9435 RVA: 0x000B89D0 File Offset: 0x000B6BD0
	GameObject IPunPrefabPool.Instantiate(string prefabId, Vector3 position, Quaternion rotation)
	{
		PrefabType prefabType;
		if (!this.networkPrefabs.TryGetValue(prefabId, out prefabType))
		{
			return null;
		}
		return Object.Instantiate<GameObject>(prefabType.prefab, position, rotation);
	}

	// Token: 0x060024DC RID: 9436 RVA: 0x000B89FC File Offset: 0x000B6BFC
	void IPunPrefabPool.Destroy(GameObject netObj)
	{
		if (netObj.IsNull())
		{
			return;
		}
		if (this.netInstantiedObjects.Remove(netObj))
		{
			PhotonViewCache photonViewCache;
			if (this.m_invalidCreatePool.Count < 200 && netObj.TryGetComponent<PhotonViewCache>(out photonViewCache) && !photonViewCache.Initialized)
			{
				if (this.m_m_invalidCreatePoolLookup.Add(netObj))
				{
					this.m_invalidCreatePool.Add(netObj);
				}
				return;
			}
			Object.Destroy(netObj);
			return;
		}
		else
		{
			PhotonView photonView;
			if (!netObj.TryGetComponent<PhotonView>(out photonView) || photonView.isRuntimeInstantiated)
			{
				Object.Destroy(netObj);
				return;
			}
			if (!this.objectsQueued.Contains(netObj))
			{
				this.objectsWaiting.Enqueue(netObj);
				this.objectsQueued.Add(netObj);
			}
			if (!this.waiting)
			{
				this.waiting = true;
				TickSystem<object>.AddPreTickCallback(this);
			}
			return;
		}
	}

	// Token: 0x060024DD RID: 9437 RVA: 0x000B8ABC File Offset: 0x000B6CBC
	void ITickSystemPre.PreTick()
	{
		if (this.waiting)
		{
			this.waiting = false;
			return;
		}
		Queue<GameObject> queue = this.queueBeingProcssed;
		Queue<GameObject> queue2 = this.objectsWaiting;
		this.objectsWaiting = queue;
		this.queueBeingProcssed = queue2;
		while (this.queueBeingProcssed.Count > 0)
		{
			GameObject gameObject = this.queueBeingProcssed.Dequeue();
			this.objectsQueued.Remove(gameObject);
			if (!gameObject.IsNull())
			{
				gameObject.SetActive(true);
				gameObject.GetComponents<PhotonView>(this.tempViews);
				for (int i = 0; i < this.tempViews.Count; i++)
				{
					PhotonNetwork.RegisterPhotonView(this.tempViews[i]);
				}
			}
		}
		if (this.objectsQueued.Count < 1)
		{
			TickSystem<object>.RemovePreTickCallback(this);
			return;
		}
		this.waiting = true;
	}

	// Token: 0x060024DE RID: 9438 RVA: 0x000B8B80 File Offset: 0x000B6D80
	private void OnLeftRoom()
	{
		foreach (GameObject gameObject in this.m_invalidCreatePool)
		{
			if (!gameObject.IsNull())
			{
				Object.Destroy(gameObject);
			}
		}
		this.m_invalidCreatePool.Clear();
		this.m_m_invalidCreatePoolLookup.Clear();
	}

	// Token: 0x060024DF RID: 9439 RVA: 0x000B8BF0 File Offset: 0x000B6DF0
	private void CheckVOIPSettings(RemoteVoiceLink voiceLink)
	{
		try
		{
			NetPlayer netPlayer = null;
			if (voiceLink.Info.UserData != null)
			{
				int num;
				if (int.TryParse(voiceLink.Info.UserData.ToString(), out num))
				{
					netPlayer = NetworkSystem.Instance.GetPlayer(num / PhotonNetwork.MAX_VIEW_IDS);
				}
			}
			else
			{
				netPlayer = NetworkSystem.Instance.GetPlayer(voiceLink.PlayerId);
			}
			if (netPlayer != null)
			{
				RigContainer rigContainer;
				if ((voiceLink.Info.Bitrate > 20000 || voiceLink.Info.SamplingRate > 16000) && VRRigCache.Instance.TryGetVrrig(netPlayer, out rigContainer))
				{
					rigContainer.ForceMute = true;
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogError(ex.ToString());
		}
	}

	// Token: 0x040029C3 RID: 10691
	[SerializeField]
	private PrefabType[] networkPrefabsData;

	// Token: 0x040029C4 RID: 10692
	public Dictionary<string, PrefabType> networkPrefabs = new Dictionary<string, PrefabType>();

	// Token: 0x040029C5 RID: 10693
	private Queue<GameObject> objectsWaiting = new Queue<GameObject>(20);

	// Token: 0x040029C6 RID: 10694
	private Queue<GameObject> queueBeingProcssed = new Queue<GameObject>(20);

	// Token: 0x040029C7 RID: 10695
	private HashSet<GameObject> objectsQueued = new HashSet<GameObject>();

	// Token: 0x040029C8 RID: 10696
	private HashSet<GameObject> netInstantiedObjects = new HashSet<GameObject>();

	// Token: 0x040029C9 RID: 10697
	private List<PhotonView> tempViews = new List<PhotonView>(5);

	// Token: 0x040029CA RID: 10698
	private List<GameObject> m_invalidCreatePool = new List<GameObject>(100);

	// Token: 0x040029CB RID: 10699
	private HashSet<GameObject> m_m_invalidCreatePoolLookup = new HashSet<GameObject>(100);

	// Token: 0x040029CC RID: 10700
	private bool waiting;
}
