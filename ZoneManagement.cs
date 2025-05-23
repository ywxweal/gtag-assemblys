using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02000236 RID: 566
public class ZoneManagement : MonoBehaviour
{
	// Token: 0x14000022 RID: 34
	// (add) Token: 0x06000D06 RID: 3334 RVA: 0x00044A9C File Offset: 0x00042C9C
	// (remove) Token: 0x06000D07 RID: 3335 RVA: 0x00044AD0 File Offset: 0x00042CD0
	public static event ZoneManagement.ZoneChangeEvent OnZoneChange;

	// Token: 0x17000144 RID: 324
	// (get) Token: 0x06000D08 RID: 3336 RVA: 0x00044B03 File Offset: 0x00042D03
	// (set) Token: 0x06000D09 RID: 3337 RVA: 0x00044B0B File Offset: 0x00042D0B
	public bool hasInstance { get; private set; }

	// Token: 0x06000D0A RID: 3338 RVA: 0x00044B14 File Offset: 0x00042D14
	private void Awake()
	{
		if (ZoneManagement.instance == null)
		{
			this.Initialize();
			return;
		}
		if (ZoneManagement.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06000D0B RID: 3339 RVA: 0x00044B42 File Offset: 0x00042D42
	public static void SetActiveZone(GTZone zone)
	{
		ZoneManagement.SetActiveZones(new GTZone[] { zone });
	}

	// Token: 0x06000D0C RID: 3340 RVA: 0x00044B54 File Offset: 0x00042D54
	public static void SetActiveZones(GTZone[] zones)
	{
		if (ZoneManagement.instance == null)
		{
			ZoneManagement.FindInstance();
		}
		if (zones == null || zones.Length == 0)
		{
			return;
		}
		ZoneManagement.instance.SetZones(zones);
		Action action = ZoneManagement.instance.onZoneChanged;
		if (action != null)
		{
			action();
		}
		if (ZoneManagement.OnZoneChange != null)
		{
			ZoneManagement.OnZoneChange(ZoneManagement.instance.zones);
		}
	}

	// Token: 0x06000D0D RID: 3341 RVA: 0x00044BB8 File Offset: 0x00042DB8
	public static bool IsInZone(GTZone zone)
	{
		if (ZoneManagement.instance == null)
		{
			ZoneManagement.FindInstance();
		}
		ZoneData zoneData = ZoneManagement.instance.GetZoneData(zone);
		return zoneData != null && zoneData.active;
	}

	// Token: 0x06000D0E RID: 3342 RVA: 0x00044BEE File Offset: 0x00042DEE
	public GameObject GetPrimaryGameObject(GTZone zone)
	{
		return this.GetZoneData(zone).rootGameObjects[0];
	}

	// Token: 0x06000D0F RID: 3343 RVA: 0x00044BFE File Offset: 0x00042DFE
	public static void AddSceneToForceStayLoaded(string sceneName)
	{
		if (ZoneManagement.instance == null)
		{
			ZoneManagement.FindInstance();
		}
		ZoneManagement.instance.sceneForceStayLoaded.Add(sceneName);
	}

	// Token: 0x06000D10 RID: 3344 RVA: 0x00044C23 File Offset: 0x00042E23
	public static void RemoveSceneFromForceStayLoaded(string sceneName)
	{
		if (ZoneManagement.instance == null)
		{
			ZoneManagement.FindInstance();
		}
		ZoneManagement.instance.sceneForceStayLoaded.Remove(sceneName);
	}

	// Token: 0x06000D11 RID: 3345 RVA: 0x00044C48 File Offset: 0x00042E48
	public static void FindInstance()
	{
		ZoneManagement zoneManagement = Object.FindObjectOfType<ZoneManagement>();
		if (zoneManagement == null)
		{
			throw new NullReferenceException("Unable to find ZoneManagement object in scene.");
		}
		Debug.LogWarning("ZoneManagement accessed before MonoBehaviour awake function called; consider delaying zone management functions to avoid FindObject lookup.");
		zoneManagement.Initialize();
	}

	// Token: 0x06000D12 RID: 3346 RVA: 0x00044C74 File Offset: 0x00042E74
	public bool IsSceneLoaded(GTZone gtZone)
	{
		foreach (ZoneData zoneData in this.zones)
		{
			if (zoneData.zone == gtZone && this.scenesLoaded.Contains(zoneData.sceneName))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000D13 RID: 3347 RVA: 0x00044CBC File Offset: 0x00042EBC
	public bool IsZoneActive(GTZone zone)
	{
		ZoneData zoneData = this.GetZoneData(zone);
		return zoneData != null && zoneData.active;
	}

	// Token: 0x06000D14 RID: 3348 RVA: 0x00044CDC File Offset: 0x00042EDC
	public HashSet<string> GetAllLoadedScenes()
	{
		return this.scenesLoaded;
	}

	// Token: 0x06000D15 RID: 3349 RVA: 0x00044CE4 File Offset: 0x00042EE4
	public bool IsSceneLoaded(string sceneName)
	{
		return this.scenesLoaded.Contains(sceneName);
	}

	// Token: 0x06000D16 RID: 3350 RVA: 0x00044CF4 File Offset: 0x00042EF4
	private void Initialize()
	{
		ZoneManagement.instance = this;
		this.hasInstance = true;
		HashSet<GameObject> hashSet = new HashSet<GameObject>();
		List<GameObject> list = new List<GameObject>(8);
		for (int i = 0; i < this.zones.Length; i++)
		{
			list.Clear();
			ZoneData zoneData = this.zones[i];
			if (zoneData != null && zoneData.rootGameObjects != null)
			{
				hashSet.UnionWith(zoneData.rootGameObjects);
				for (int j = 0; j < zoneData.rootGameObjects.Length; j++)
				{
					GameObject gameObject = zoneData.rootGameObjects[j];
					if (!(gameObject == null))
					{
						list.Add(gameObject);
					}
				}
				hashSet.UnionWith(list);
			}
		}
		this.allObjects = hashSet.ToArray<GameObject>();
		this.objectActivationState = new bool[this.allObjects.Length];
	}

	// Token: 0x06000D17 RID: 3351 RVA: 0x00044DB0 File Offset: 0x00042FB0
	private void SetZones(GTZone[] newActiveZones)
	{
		for (int i = 0; i < this.objectActivationState.Length; i++)
		{
			this.objectActivationState[i] = false;
		}
		this.activeZones.Clear();
		for (int j = 0; j < newActiveZones.Length; j++)
		{
			this.activeZones.Add(newActiveZones[j]);
		}
		this.scenesRequested.Clear();
		this.scenesRequested.Add("GorillaTag");
		float num = 0f;
		for (int k = 0; k < this.zones.Length; k++)
		{
			ZoneData zoneData = this.zones[k];
			if (zoneData == null || zoneData.rootGameObjects == null || !newActiveZones.Contains(zoneData.zone))
			{
				zoneData.active = false;
			}
			else
			{
				zoneData.active = true;
				num = Mathf.Max(num, zoneData.CameraFarClipPlane);
				if (!string.IsNullOrEmpty(zoneData.sceneName))
				{
					this.scenesRequested.Add(zoneData.sceneName);
				}
				foreach (GameObject gameObject in zoneData.rootGameObjects)
				{
					if (!(gameObject == null))
					{
						for (int m = 0; m < this.allObjects.Length; m++)
						{
							if (gameObject == this.allObjects[m])
							{
								this.objectActivationState[m] = true;
								break;
							}
						}
					}
				}
			}
		}
		if (this.mainCamera == null)
		{
			this.mainCamera = Camera.main;
		}
		this.mainCamera.farClipPlane = num;
		int loadedSceneCount = SceneManager.loadedSceneCount;
		for (int n = 0; n < loadedSceneCount; n++)
		{
			this.scenesLoaded.Add(SceneManager.GetSceneAt(n).name);
		}
		foreach (string text in this.scenesRequested)
		{
			if (this.scenesLoaded.Add(text))
			{
				AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(text, LoadSceneMode.Additive);
				this._scenes_to_loadOps[text] = asyncOperation;
				asyncOperation.completed += this.HandleOnSceneLoadCompleted;
			}
		}
		this.scenesToUnload.Clear();
		foreach (string text2 in this.scenesLoaded)
		{
			if (!this.scenesRequested.Contains(text2) && !this.sceneForceStayLoaded.Contains(text2))
			{
				this.scenesToUnload.Add(text2);
			}
		}
		foreach (string text3 in this.scenesToUnload)
		{
			this.scenesLoaded.Remove(text3);
			AsyncOperation asyncOperation2 = SceneManager.UnloadSceneAsync(text3);
			this._scenes_to_unloadOps[text3] = asyncOperation2;
		}
		for (int num2 = 0; num2 < this.objectActivationState.Length; num2++)
		{
			if (!(this.allObjects[num2] == null))
			{
				this.allObjects[num2].SetActive(this.objectActivationState[num2]);
			}
		}
	}

	// Token: 0x06000D18 RID: 3352 RVA: 0x000450F4 File Offset: 0x000432F4
	private void HandleOnSceneLoadCompleted(AsyncOperation thisLoadOp)
	{
		using (Dictionary<string, AsyncOperation>.ValueCollection.Enumerator enumerator = this._scenes_to_loadOps.Values.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (!enumerator.Current.isDone)
				{
					return;
				}
			}
		}
		using (Dictionary<string, AsyncOperation>.ValueCollection.Enumerator enumerator = this._scenes_to_unloadOps.Values.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (!enumerator.Current.isDone)
				{
					return;
				}
			}
		}
		Action onSceneLoadsCompleted = this.OnSceneLoadsCompleted;
		if (onSceneLoadsCompleted == null)
		{
			return;
		}
		onSceneLoadsCompleted();
	}

	// Token: 0x06000D19 RID: 3353 RVA: 0x000451A8 File Offset: 0x000433A8
	private ZoneData GetZoneData(GTZone zone)
	{
		for (int i = 0; i < this.zones.Length; i++)
		{
			if (this.zones[i].zone == zone)
			{
				return this.zones[i];
			}
		}
		return null;
	}

	// Token: 0x06000D1A RID: 3354 RVA: 0x000451E2 File Offset: 0x000433E2
	public static bool IsValidZoneInt(int zoneInt)
	{
		return zoneInt >= 11 && zoneInt <= 24;
	}

	// Token: 0x0400109B RID: 4251
	public static ZoneManagement instance;

	// Token: 0x0400109D RID: 4253
	[SerializeField]
	private ZoneData[] zones;

	// Token: 0x0400109E RID: 4254
	private GameObject[] allObjects;

	// Token: 0x0400109F RID: 4255
	private bool[] objectActivationState;

	// Token: 0x040010A0 RID: 4256
	public Action onZoneChanged;

	// Token: 0x040010A1 RID: 4257
	public Action OnSceneLoadsCompleted;

	// Token: 0x040010A2 RID: 4258
	public List<GTZone> activeZones = new List<GTZone>(20);

	// Token: 0x040010A3 RID: 4259
	private HashSet<string> scenesLoaded = new HashSet<string>();

	// Token: 0x040010A4 RID: 4260
	private HashSet<string> scenesRequested = new HashSet<string>();

	// Token: 0x040010A5 RID: 4261
	private HashSet<string> sceneForceStayLoaded = new HashSet<string>(8);

	// Token: 0x040010A6 RID: 4262
	private List<string> scenesToUnload = new List<string>();

	// Token: 0x040010A7 RID: 4263
	private Dictionary<string, AsyncOperation> _scenes_to_loadOps = new Dictionary<string, AsyncOperation>(32);

	// Token: 0x040010A8 RID: 4264
	private Dictionary<string, AsyncOperation> _scenes_to_unloadOps = new Dictionary<string, AsyncOperation>(32);

	// Token: 0x040010A9 RID: 4265
	private Camera mainCamera;

	// Token: 0x02000237 RID: 567
	// (Invoke) Token: 0x06000D1D RID: 3357
	public delegate void ZoneChangeEvent(ZoneData[] zones);
}
