using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GorillaExtensions;
using GorillaLocomotion.Swimming;
using GorillaNetworking;
using GorillaNetworking.Store;
using GorillaTag.Rendering;
using GorillaTagScripts.CustomMapSupport;
using GorillaTagScripts.ModIO;
using GorillaTagScripts.VirtualStumpCustomMaps;
using GT_CustomMapSupportRuntime;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.ProBuilder;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.Video;

// Token: 0x020006FE RID: 1790
public class CustomMapLoader : MonoBehaviour
{
	// Token: 0x1700045B RID: 1115
	// (get) Token: 0x06002C87 RID: 11399 RVA: 0x000DB7F6 File Offset: 0x000D99F6
	// (set) Token: 0x06002C86 RID: 11398 RVA: 0x000DB7DF File Offset: 0x000D99DF
	public static string LoadedMapLevelName
	{
		get
		{
			return CustomMapLoader.loadedMapLevelName;
		}
		set
		{
			CustomMapLoader.loadedMapLevelName = value.Replace(" ", "");
		}
	}

	// Token: 0x1700045C RID: 1116
	// (get) Token: 0x06002C88 RID: 11400 RVA: 0x000DB7FD File Offset: 0x000D99FD
	public static long LoadedMapModId
	{
		get
		{
			return CustomMapLoader.loadedMapModId;
		}
	}

	// Token: 0x1700045D RID: 1117
	// (get) Token: 0x06002C89 RID: 11401 RVA: 0x000DB804 File Offset: 0x000D9A04
	public static MapDescriptor LoadedMapDescriptor
	{
		get
		{
			return CustomMapLoader.loadedMapDescriptor;
		}
	}

	// Token: 0x1700045E RID: 1118
	// (get) Token: 0x06002C8A RID: 11402 RVA: 0x000DB80B File Offset: 0x000D9A0B
	public static long LoadingMapModId
	{
		get
		{
			return CustomMapLoader.attemptedLoadID;
		}
	}

	// Token: 0x1700045F RID: 1119
	// (get) Token: 0x06002C8B RID: 11403 RVA: 0x000DB812 File Offset: 0x000D9A12
	public static bool IsLoading
	{
		get
		{
			return CustomMapLoader.isLoading;
		}
	}

	// Token: 0x06002C8C RID: 11404 RVA: 0x000DB819 File Offset: 0x000D9A19
	public static bool IsCustomScene(string sceneName)
	{
		return CustomMapLoader.loadedSceneNames.Contains(sceneName);
	}

	// Token: 0x06002C8D RID: 11405 RVA: 0x000DB826 File Offset: 0x000D9A26
	private void Awake()
	{
		if (CustomMapLoader.instance == null)
		{
			CustomMapLoader.instance = this;
			CustomMapLoader.hasInstance = true;
			return;
		}
		if (CustomMapLoader.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06002C8E RID: 11406 RVA: 0x000DB860 File Offset: 0x000D9A60
	private void Start()
	{
		byte[] array = new byte[]
		{
			Convert.ToByte(68),
			Convert.ToByte(111),
			Convert.ToByte(110),
			Convert.ToByte(116),
			Convert.ToByte(68),
			Convert.ToByte(101),
			Convert.ToByte(115),
			Convert.ToByte(116),
			Convert.ToByte(114),
			Convert.ToByte(111),
			Convert.ToByte(121),
			Convert.ToByte(79),
			Convert.ToByte(110),
			Convert.ToByte(76),
			Convert.ToByte(111),
			Convert.ToByte(97),
			Convert.ToByte(100)
		};
		this.dontDestroyOnLoadSceneName = Encoding.ASCII.GetString(array);
		if (this.networkTrigger != null)
		{
			this.networkTrigger.SetActive(false);
		}
	}

	// Token: 0x06002C8F RID: 11407 RVA: 0x000DB954 File Offset: 0x000D9B54
	public static void LoadMap(long mapModId, string mapFilePath, Action<bool> onLoadComplete, Action<MapLoadStatus, int, string> progressCallback, Action<string> onSceneLoaded)
	{
		if (!CustomMapLoader.hasInstance)
		{
			return;
		}
		if (CustomMapLoader.isLoading)
		{
			return;
		}
		if (CustomMapLoader.isUnloading)
		{
			if (onLoadComplete != null)
			{
				onLoadComplete(false);
			}
			return;
		}
		if (CustomMapLoader.IsModLoaded(mapModId))
		{
			if (onLoadComplete != null)
			{
				onLoadComplete(true);
			}
			return;
		}
		GorillaNetworkJoinTrigger.DisableTriggerJoins();
		CustomMapLoader.modLoadProgressCallback = progressCallback;
		CustomMapLoader.modLoadedCallback = onLoadComplete;
		CustomMapLoader.sceneLoadedCallback = onSceneLoaded;
		CustomMapLoader.instance.StartCoroutine(CustomMapLoader.LoadAssetBundle(mapModId, mapFilePath, new Action<bool, bool>(CustomMapLoader.OnAssetBundleLoaded)));
	}

	// Token: 0x06002C90 RID: 11408 RVA: 0x000DB9D0 File Offset: 0x000D9BD0
	public static void ResetToInitialZone(Action<string> onSceneLoaded, Action<string> onSceneUnloaded)
	{
		int[] array = new int[] { CustomMapLoader.initialSceneIndex };
		List<int> list = CustomMapLoader.loadedSceneIndexes;
		list.Remove(CustomMapLoader.initialSceneIndex);
		if (CustomMapLoader.sceneLoadingCoroutine != null)
		{
			CustomMapLoader.LoadZoneRequest loadZoneRequest = new CustomMapLoader.LoadZoneRequest
			{
				sceneIndexesToLoad = array,
				sceneIndexesToUnload = list.ToArray(),
				onSceneLoadedCallback = onSceneLoaded,
				onSceneUnloadedCallback = onSceneUnloaded
			};
			CustomMapLoader.queuedLoadZoneRequests.Add(loadZoneRequest);
			return;
		}
		CustomMapLoader.sceneLoadedCallback = onSceneLoaded;
		CustomMapLoader.sceneUnloadedCallback = onSceneUnloaded;
		CustomMapLoader.sceneLoadingCoroutine = CustomMapLoader.instance.StartCoroutine(CustomMapLoader.LoadZoneCoroutine(array, list.ToArray()));
	}

	// Token: 0x06002C91 RID: 11409 RVA: 0x000DBA6C File Offset: 0x000D9C6C
	public static void LoadZoneTriggered(int[] loadSceneIndexes, int[] unloadSceneIndexes, Action<string> onSceneLoaded, Action<string> onSceneUnloaded)
	{
		string text = "";
		for (int i = 0; i < loadSceneIndexes.Length; i++)
		{
			text += loadSceneIndexes[i].ToString();
			if (i != loadSceneIndexes.Length - 1)
			{
				text += ", ";
			}
		}
		string text2 = "";
		for (int j = 0; j < unloadSceneIndexes.Length; j++)
		{
			text2 += unloadSceneIndexes[j].ToString();
			if (j != unloadSceneIndexes.Length - 1)
			{
				text2 += ", ";
			}
		}
		if (CustomMapLoader.sceneLoadingCoroutine != null)
		{
			CustomMapLoader.LoadZoneRequest loadZoneRequest = new CustomMapLoader.LoadZoneRequest
			{
				sceneIndexesToLoad = loadSceneIndexes,
				sceneIndexesToUnload = unloadSceneIndexes,
				onSceneLoadedCallback = onSceneLoaded,
				onSceneUnloadedCallback = onSceneUnloaded
			};
			CustomMapLoader.queuedLoadZoneRequests.Add(loadZoneRequest);
			return;
		}
		CustomMapLoader.sceneLoadedCallback = onSceneLoaded;
		CustomMapLoader.sceneUnloadedCallback = onSceneUnloaded;
		CustomMapLoader.sceneLoadingCoroutine = CustomMapLoader.instance.StartCoroutine(CustomMapLoader.LoadZoneCoroutine(loadSceneIndexes, unloadSceneIndexes));
	}

	// Token: 0x06002C92 RID: 11410 RVA: 0x000DBB53 File Offset: 0x000D9D53
	private static IEnumerator LoadZoneCoroutine(int[] loadScenes, int[] unloadScenes)
	{
		if (!unloadScenes.IsNullOrEmpty<int>())
		{
			yield return CustomMapLoader.UnloadScenesCoroutine(unloadScenes);
		}
		if (!loadScenes.IsNullOrEmpty<int>())
		{
			yield return CustomMapLoader.LoadScenesCoroutine(loadScenes);
		}
		if (CustomMapLoader.sceneLoadingCoroutine != null)
		{
			CustomMapLoader.instance.StopCoroutine(CustomMapLoader.sceneLoadingCoroutine);
			CustomMapLoader.sceneLoadingCoroutine = null;
		}
		if (CustomMapLoader.queuedLoadZoneRequests.Count > 0)
		{
			CustomMapLoader.LoadZoneRequest loadZoneRequest = CustomMapLoader.queuedLoadZoneRequests[0];
			CustomMapLoader.queuedLoadZoneRequests.RemoveAt(0);
			CustomMapLoader.LoadZoneTriggered(loadZoneRequest.sceneIndexesToLoad, loadZoneRequest.sceneIndexesToUnload, loadZoneRequest.onSceneLoadedCallback, loadZoneRequest.onSceneUnloadedCallback);
		}
		yield break;
	}

	// Token: 0x06002C93 RID: 11411 RVA: 0x000DBB69 File Offset: 0x000D9D69
	private static IEnumerator LoadScenesCoroutine(int[] sceneIndexes)
	{
		int num;
		for (int i = 0; i < sceneIndexes.Length; i = num + 1)
		{
			if (!CustomMapLoader.loadedSceneFilePaths.Contains(CustomMapLoader.assetBundleSceneFilePaths[sceneIndexes[i]]))
			{
				yield return CustomMapLoader.LoadSceneFromAssetBundle(sceneIndexes[i], false, new Action<bool, bool, string>(CustomMapLoader.OnIncrementalLoadComplete));
			}
			num = i;
		}
		yield break;
	}

	// Token: 0x06002C94 RID: 11412 RVA: 0x000DBB78 File Offset: 0x000D9D78
	private static IEnumerator UnloadScenesCoroutine(int[] sceneIndexes)
	{
		int num;
		for (int i = 0; i < sceneIndexes.Length; i = num + 1)
		{
			yield return CustomMapLoader.UnloadSceneCoroutine(sceneIndexes[i], null);
			num = i;
		}
		yield break;
	}

	// Token: 0x06002C95 RID: 11413 RVA: 0x000DBB87 File Offset: 0x000D9D87
	private static IEnumerator LoadAssetBundle(long mapModID, string packageInfoFilePath, Action<bool, bool> OnLoadComplete)
	{
		if (CustomMapLoader.isLoading)
		{
			if (OnLoadComplete != null)
			{
				OnLoadComplete(false, false);
			}
			yield break;
		}
		yield return CustomMapLoader.CloseDoorAndUnloadModCoroutine();
		if (CustomMapLoader.shouldAbortSceneLoad)
		{
			yield return CustomMapLoader.AbortSceneLoad(-1);
			OnLoadComplete(false, true);
			yield break;
		}
		CustomMapLoader.isLoading = true;
		CustomMapLoader.attemptedLoadID = mapModID;
		Action<MapLoadStatus, int, string> action = CustomMapLoader.modLoadProgressCallback;
		if (action != null)
		{
			action(MapLoadStatus.Loading, 1, "GRABBING LIGHTMAP DATA");
		}
		CustomMapLoader.lightmaps = new LightmapData[LightmapSettings.lightmaps.Length];
		if (CustomMapLoader.lightmapsToKeep.Count > 0)
		{
			CustomMapLoader.lightmapsToKeep.Clear();
		}
		CustomMapLoader.lightmapsToKeep = new List<Texture2D>(LightmapSettings.lightmaps.Length * 2);
		for (int i = 0; i < LightmapSettings.lightmaps.Length; i++)
		{
			CustomMapLoader.lightmaps[i] = LightmapSettings.lightmaps[i];
			if (LightmapSettings.lightmaps[i].lightmapColor != null)
			{
				CustomMapLoader.lightmapsToKeep.Add(LightmapSettings.lightmaps[i].lightmapColor);
			}
			if (LightmapSettings.lightmaps[i].lightmapDir != null)
			{
				CustomMapLoader.lightmapsToKeep.Add(LightmapSettings.lightmaps[i].lightmapDir);
			}
		}
		Action<MapLoadStatus, int, string> action2 = CustomMapLoader.modLoadProgressCallback;
		if (action2 != null)
		{
			action2(MapLoadStatus.Loading, 2, "LOADING PACKAGE INFO");
		}
		MapPackageInfo packageInfo;
		try
		{
			packageInfo = CustomMapLoader.GetPackageInfo(packageInfoFilePath);
		}
		catch (Exception ex)
		{
			Action<MapLoadStatus, int, string> action3 = CustomMapLoader.modLoadProgressCallback;
			if (action3 != null)
			{
				action3(MapLoadStatus.Error, 0, ex.Message);
			}
			yield break;
		}
		if (packageInfo == null)
		{
			Action<MapLoadStatus, int, string> action4 = CustomMapLoader.modLoadProgressCallback;
			if (action4 != null)
			{
				action4(MapLoadStatus.Error, 0, "FAILED TO READ FILE AT " + packageInfoFilePath);
			}
			OnLoadComplete(false, false);
			yield break;
		}
		CustomMapLoader.initialSceneName = packageInfo.initialScene;
		Action<MapLoadStatus, int, string> action5 = CustomMapLoader.modLoadProgressCallback;
		if (action5 != null)
		{
			action5(MapLoadStatus.Loading, 3, "PACKAGE INFO LOADED");
		}
		string text = Path.GetDirectoryName(packageInfoFilePath) + "/" + packageInfo.pcFileName;
		Action<MapLoadStatus, int, string> action6 = CustomMapLoader.modLoadProgressCallback;
		if (action6 != null)
		{
			action6(MapLoadStatus.Loading, 12, "LOADING MAP ASSET BUNDLE");
		}
		AssetBundleCreateRequest loadBundleRequest = AssetBundle.LoadFromFileAsync(text);
		yield return loadBundleRequest;
		CustomMapLoader.mapBundle = loadBundleRequest.assetBundle;
		if (CustomMapLoader.shouldAbortSceneLoad)
		{
			yield return CustomMapLoader.AbortSceneLoad(-1);
			OnLoadComplete(false, true);
			yield break;
		}
		if (CustomMapLoader.mapBundle == null)
		{
			Action<MapLoadStatus, int, string> action7 = CustomMapLoader.modLoadProgressCallback;
			if (action7 != null)
			{
				action7(MapLoadStatus.Error, 0, "CUSTOM MAP ASSET BUNDLE FAILED TO LOAD");
			}
			OnLoadComplete(false, false);
			yield break;
		}
		if (!CustomMapLoader.mapBundle.isStreamedSceneAssetBundle)
		{
			CustomMapLoader.mapBundle.Unload(true);
			Action<MapLoadStatus, int, string> action8 = CustomMapLoader.modLoadProgressCallback;
			if (action8 != null)
			{
				action8(MapLoadStatus.Error, 0, "AssetBundle does not contain a Unity Scene file");
			}
			OnLoadComplete(false, false);
			yield break;
		}
		Action<MapLoadStatus, int, string> action9 = CustomMapLoader.modLoadProgressCallback;
		if (action9 != null)
		{
			action9(MapLoadStatus.Loading, 20, "MAP ASSET BUNDLE LOADED");
		}
		CustomMapLoader.mapBundle.GetAllAssetNames();
		CustomMapLoader.assetBundleSceneFilePaths = CustomMapLoader.mapBundle.GetAllScenePaths();
		if (CustomMapLoader.assetBundleSceneFilePaths.Length == 0)
		{
			CustomMapLoader.mapBundle.Unload(true);
			Action<MapLoadStatus, int, string> action10 = CustomMapLoader.modLoadProgressCallback;
			if (action10 != null)
			{
				action10(MapLoadStatus.Error, 0, "AssetBundle does not contain a Unity Scene file");
			}
			OnLoadComplete(false, false);
			yield break;
		}
		foreach (string text2 in CustomMapLoader.assetBundleSceneFilePaths)
		{
			if (text2.Equals(CustomMapLoader.instance.dontDestroyOnLoadSceneName, StringComparison.OrdinalIgnoreCase))
			{
				CustomMapLoader.mapBundle.Unload(true);
				Action<MapLoadStatus, int, string> action11 = CustomMapLoader.modLoadProgressCallback;
				if (action11 != null)
				{
					action11(MapLoadStatus.Error, 0, "Map name is " + text2 + " this is an invalid name");
				}
				OnLoadComplete(false, false);
				yield break;
			}
		}
		OnLoadComplete(true, false);
		yield break;
	}

	// Token: 0x06002C96 RID: 11414 RVA: 0x000DBBA4 File Offset: 0x000D9DA4
	private static void RequestAbortModLoad(Action callback = null)
	{
		CustomMapLoader.abortModLoadCallback = callback;
		CustomMapLoader.shouldAbortSceneLoad = true;
		CustomMapLoader.shouldUnloadMod = true;
	}

	// Token: 0x06002C97 RID: 11415 RVA: 0x000DBBB8 File Offset: 0x000D9DB8
	private static IEnumerator AbortSceneLoad(int sceneIndex)
	{
		if (sceneIndex == -1)
		{
			CustomMapLoader.shouldUnloadMod = true;
		}
		CustomMapLoader.isLoading = false;
		if (CustomMapLoader.shouldUnloadMod)
		{
			if (CustomMapLoader.sceneLoadingCoroutine != null)
			{
				CustomMapLoader.instance.StopCoroutine(CustomMapLoader.sceneLoadingCoroutine);
				CustomMapLoader.sceneLoadingCoroutine = null;
			}
			yield return CustomMapLoader.UnloadAllScenesCoroutine();
			if (CustomMapLoader.mapBundle != null)
			{
				CustomMapLoader.mapBundle.Unload(true);
			}
			CustomMapLoader.mapBundle = null;
			Action action = CustomMapLoader.abortModLoadCallback;
			if (action != null)
			{
				action();
			}
		}
		else
		{
			yield return CustomMapLoader.UnloadSceneCoroutine(sceneIndex, null);
		}
		CustomMapLoader.abortModLoadCallback = null;
		CustomMapLoader.shouldAbortSceneLoad = false;
		CustomMapLoader.shouldUnloadMod = false;
		yield break;
	}

	// Token: 0x06002C98 RID: 11416 RVA: 0x000DBBC8 File Offset: 0x000D9DC8
	private static int GetSceneIndex(string sceneName)
	{
		int num = -1;
		if (CustomMapLoader.assetBundleSceneFilePaths.Length == 1)
		{
			return 0;
		}
		for (int i = 0; i < CustomMapLoader.assetBundleSceneFilePaths.Length; i++)
		{
			string sceneNameFromFilePath = CustomMapLoader.GetSceneNameFromFilePath(CustomMapLoader.assetBundleSceneFilePaths[i]);
			if (sceneNameFromFilePath != null && sceneNameFromFilePath.Equals(sceneName))
			{
				num = i;
				break;
			}
		}
		return num;
	}

	// Token: 0x06002C99 RID: 11417 RVA: 0x000DBC17 File Offset: 0x000D9E17
	private static IEnumerator LoadSceneFromAssetBundle(int sceneIndex, bool useProgressCallback, Action<bool, bool, string> OnLoadComplete)
	{
		LoadSceneParameters loadSceneParameters = new LoadSceneParameters
		{
			loadSceneMode = LoadSceneMode.Additive,
			localPhysicsMode = LocalPhysicsMode.None
		};
		if (CustomMapLoader.shouldAbortSceneLoad)
		{
			yield return CustomMapLoader.AbortSceneLoad(sceneIndex);
			OnLoadComplete(false, true, "");
			yield break;
		}
		CustomMapLoader.runningAsyncLoad = true;
		if (useProgressCallback)
		{
			Action<MapLoadStatus, int, string> action = CustomMapLoader.modLoadProgressCallback;
			if (action != null)
			{
				action(MapLoadStatus.Loading, 30, "LOADING MAP SCENE");
			}
		}
		CustomMapLoader.attemptedSceneToLoad = CustomMapLoader.assetBundleSceneFilePaths[sceneIndex];
		string sceneName = CustomMapLoader.GetSceneNameFromFilePath(CustomMapLoader.attemptedSceneToLoad);
		AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(CustomMapLoader.attemptedSceneToLoad, loadSceneParameters);
		yield return asyncOperation;
		CustomMapLoader.runningAsyncLoad = false;
		if (CustomMapLoader.shouldAbortSceneLoad)
		{
			yield return CustomMapLoader.AbortSceneLoad(sceneIndex);
			OnLoadComplete(false, true, "");
			yield break;
		}
		if (useProgressCallback)
		{
			Action<MapLoadStatus, int, string> action2 = CustomMapLoader.modLoadProgressCallback;
			if (action2 != null)
			{
				action2(MapLoadStatus.Loading, 50, "SANITIZING MAP");
			}
		}
		GameObject[] rootGameObjects = SceneManager.GetSceneByName(sceneName).GetRootGameObjects();
		List<MapDescriptor> list = new List<MapDescriptor>();
		for (int i = 0; i < rootGameObjects.Length; i++)
		{
			list.AddRange(rootGameObjects[i].GetComponentsInChildren<MapDescriptor>());
		}
		MapDescriptor mapDescriptor = null;
		bool flag = false;
		foreach (MapDescriptor mapDescriptor2 in list)
		{
			if (!mapDescriptor2.IsNull())
			{
				if (!mapDescriptor.IsNull())
				{
					flag = true;
					break;
				}
				mapDescriptor = mapDescriptor2;
			}
		}
		if (flag)
		{
			yield return CustomMapLoader.AbortSceneLoad(sceneIndex);
			if (useProgressCallback)
			{
				Action<MapLoadStatus, int, string> action3 = CustomMapLoader.modLoadProgressCallback;
				if (action3 != null)
				{
					action3(MapLoadStatus.Error, 0, "MAP CONTAINS MULTIPLE MAP DESCRIPTOR OBJECTS");
				}
			}
			OnLoadComplete(false, false, "");
			yield break;
		}
		if (mapDescriptor.IsNull())
		{
			yield return CustomMapLoader.AbortSceneLoad(sceneIndex);
			if (useProgressCallback)
			{
				Action<MapLoadStatus, int, string> action4 = CustomMapLoader.modLoadProgressCallback;
				if (action4 != null)
				{
					action4(MapLoadStatus.Error, 0, "MAP SCENE DOES NOT CONTAIN A MAP DESCRIPTOR");
				}
			}
			OnLoadComplete(false, false, "");
			yield break;
		}
		GameObject gameObject = mapDescriptor.gameObject;
		if (!CustomMapLoader.SanitizeObject(gameObject, gameObject))
		{
			yield return CustomMapLoader.AbortSceneLoad(sceneIndex);
			if (useProgressCallback)
			{
				Action<MapLoadStatus, int, string> action5 = CustomMapLoader.modLoadProgressCallback;
				if (action5 != null)
				{
					action5(MapLoadStatus.Error, 0, "MAP DESCRIPTOR HAS UNAPPROVED COMPONENTS ON IT");
				}
			}
			OnLoadComplete(false, false, "");
			yield break;
		}
		CustomMapLoader.totalObjectsInLoadingScene = 0;
		for (int j = 0; j < rootGameObjects.Length; j++)
		{
			CustomMapLoader.SanitizeObjectRecursive(rootGameObjects[j], gameObject);
		}
		CustomMapLoader.CheckVirtualStumpOverlap(sceneName);
		if (useProgressCallback)
		{
			Action<MapLoadStatus, int, string> action6 = CustomMapLoader.modLoadProgressCallback;
			if (action6 != null)
			{
				action6(MapLoadStatus.Loading, 70, "MAP SCENE LOADED");
			}
		}
		CustomMapLoader.leafGliderIndex = 0;
		yield return CustomMapLoader.ProcessAndInstantiateMap(gameObject, useProgressCallback);
		yield return null;
		if (CustomMapLoader.shouldAbortSceneLoad)
		{
			yield return CustomMapLoader.AbortSceneLoad(sceneIndex);
			OnLoadComplete(false, true, "");
			if (CustomMapLoader.cachedExceptionMessage.Length > 0 && useProgressCallback)
			{
				Action<MapLoadStatus, int, string> action7 = CustomMapLoader.modLoadProgressCallback;
				if (action7 != null)
				{
					action7(MapLoadStatus.Error, 0, CustomMapLoader.cachedExceptionMessage);
				}
			}
			yield break;
		}
		if (useProgressCallback)
		{
			Action<MapLoadStatus, int, string> action8 = CustomMapLoader.modLoadProgressCallback;
			if (action8 != null)
			{
				action8(MapLoadStatus.Loading, 99, "FINALIZING MAP");
			}
		}
		CustomMapLoader.loadedSceneFilePaths.AddIfNew(CustomMapLoader.attemptedSceneToLoad);
		CustomMapLoader.loadedSceneNames.AddIfNew(sceneName);
		CustomMapLoader.loadedSceneIndexes.AddIfNew(sceneIndex);
		OnLoadComplete(true, false, sceneName);
		yield break;
	}

	// Token: 0x06002C9A RID: 11418 RVA: 0x000DBC34 File Offset: 0x000D9E34
	public static void CloseDoorAndUnloadMod(Action unloadFinishedCallback = null)
	{
		if (!CustomMapLoader.IsModLoaded(0L) && !CustomMapLoader.isLoading)
		{
			return;
		}
		CustomMapLoader.unloadModCallback = unloadFinishedCallback;
		if (CustomMapLoader.isLoading)
		{
			CustomMapLoader.RequestAbortModLoad(null);
			return;
		}
		if (CustomMapLoader.instance.accessDoor != null)
		{
			CustomMapLoader.instance.accessDoor.CloseDoor();
		}
		CustomMapLoader.shouldUnloadMod = true;
		if (CustomMapLoader.mapBundle != null)
		{
			CustomMapLoader.mapBundle.Unload(true);
		}
		CustomMapLoader.mapBundle = null;
		CustomMapTelemetry.EndMapTracking();
		CMSSerializer.ResetSyncedMapObjects();
		CustomMapLoader.instance.StartCoroutine(CustomMapLoader.UnloadAllScenesCoroutine());
	}

	// Token: 0x06002C9B RID: 11419 RVA: 0x000DBCCB File Offset: 0x000D9ECB
	private static IEnumerator CloseDoorAndUnloadModCoroutine()
	{
		if (!CustomMapLoader.IsModLoaded(0L) || CustomMapLoader.IsLoading)
		{
			yield break;
		}
		if (CustomMapLoader.instance.accessDoor != null)
		{
			CustomMapLoader.instance.accessDoor.CloseDoor();
		}
		CustomMapLoader.shouldUnloadMod = true;
		if (CustomMapLoader.mapBundle != null)
		{
			CustomMapLoader.mapBundle.Unload(true);
		}
		CustomMapLoader.mapBundle = null;
		CustomMapTelemetry.EndMapTracking();
		CMSSerializer.ResetSyncedMapObjects();
		yield return CustomMapLoader.UnloadAllScenesCoroutine();
		yield break;
	}

	// Token: 0x06002C9C RID: 11420 RVA: 0x000DBCD3 File Offset: 0x000D9ED3
	private static IEnumerator UnloadAllScenesCoroutine()
	{
		CustomMapLoader.isLoading = false;
		CustomMapLoader.isUnloading = true;
		ZoneShaderSettings.ActivateDefaultSettings();
		CustomMapLoader.RemoveCustomMapATM();
		if (!CustomMapLoader.assetBundleSceneFilePaths.IsNullOrEmpty<string>())
		{
			int num;
			for (int sceneIndex = 0; sceneIndex < CustomMapLoader.assetBundleSceneFilePaths.Length; sceneIndex = num + 1)
			{
				yield return CustomMapLoader.UnloadSceneCoroutine(sceneIndex, null);
				num = sceneIndex;
			}
		}
		CustomMapLoader.loadedMapDescriptor = null;
		CustomMapLoader.loadedSceneFilePaths.Clear();
		CustomMapLoader.loadedSceneNames.Clear();
		CustomMapLoader.loadedSceneIndexes.Clear();
		for (int i = 0; i < CustomMapLoader.instance.leafGliders.Length; i++)
		{
			CustomMapLoader.instance.leafGliders[i].CustomMapUnload();
			CustomMapLoader.instance.leafGliders[i].enabled = false;
			CustomMapLoader.instance.leafGliders[CustomMapLoader.leafGliderIndex].transform.GetChild(0).gameObject.SetActive(false);
		}
		GorillaNetworkJoinTrigger.EnableTriggerJoins();
		LightmapSettings.lightmaps = CustomMapLoader.lightmaps;
		CustomMapLoader.UnloadLightmaps();
		Resources.UnloadUnusedAssets();
		CustomMapLoader.isUnloading = false;
		if (CustomMapLoader.shouldUnloadMod)
		{
			yield return CustomMapLoader.ResetLightmaps();
			CustomMapLoader.assetBundleSceneFilePaths = new string[] { "" };
			CustomMapLoader.loadedMapModId = 0L;
			CustomMapLoader.initialSceneIndex = 0;
			CustomMapLoader.initialSceneName = "";
			Action action = CustomMapLoader.unloadModCallback;
			if (action != null)
			{
				action();
			}
			CustomMapLoader.unloadModCallback = null;
			CustomMapLoader.shouldUnloadMod = false;
		}
		yield break;
	}

	// Token: 0x06002C9D RID: 11421 RVA: 0x000DBCDB File Offset: 0x000D9EDB
	private static IEnumerator UnloadSceneCoroutine(int sceneIndex, Action OnUnloadComplete = null)
	{
		if (!CustomMapLoader.hasInstance)
		{
			yield break;
		}
		if (sceneIndex < 0 || sceneIndex >= CustomMapLoader.assetBundleSceneFilePaths.Length)
		{
			Debug.LogError(string.Format("[CustomMapLoader::UnloadSceneCoroutine] SceneIndex of {0} is invalid! ", sceneIndex) + string.Format("The currently loaded AssetBundle contains {0} scenes.", CustomMapLoader.assetBundleSceneFilePaths.Length));
			yield break;
		}
		while (CustomMapLoader.runningAsyncLoad)
		{
			yield return null;
		}
		UnloadSceneOptions unloadSceneOptions = UnloadSceneOptions.UnloadAllEmbeddedSceneObjects;
		string scenePathWithExtension = CustomMapLoader.assetBundleSceneFilePaths[sceneIndex];
		string[] array = scenePathWithExtension.Split(".", StringSplitOptions.None);
		string text = "";
		string sceneName = "";
		if (!array.IsNullOrEmpty<string>())
		{
			text = array[0];
			if (text.Length > 0)
			{
				sceneName = Path.GetFileName(text);
			}
		}
		Scene sceneByName = SceneManager.GetSceneByName(text);
		if (sceneByName.IsValid())
		{
			if (CustomMapLoader.customMapATM.IsNotNull() && CustomMapLoader.customMapATM.gameObject.scene.Equals(sceneByName))
			{
				CustomMapLoader.RemoveCustomMapATM();
			}
			CustomMapLoader.RemoveUnloadingHoverboardAreas(sceneByName);
			AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(scenePathWithExtension, unloadSceneOptions);
			yield return asyncOperation;
			CustomMapLoader.loadedSceneFilePaths.Remove(scenePathWithExtension);
			CustomMapLoader.loadedSceneNames.Remove(sceneName);
			CustomMapLoader.loadedSceneIndexes.Remove(sceneIndex);
			Action<string> action = CustomMapLoader.sceneUnloadedCallback;
			if (action != null)
			{
				action(sceneName);
			}
			if (OnUnloadComplete != null)
			{
				OnUnloadComplete();
			}
			yield break;
		}
		yield break;
	}

	// Token: 0x06002C9E RID: 11422 RVA: 0x000023F4 File Offset: 0x000005F4
	private static void RemoveUnloadingHoverboardAreas(Scene unloadingScene)
	{
	}

	// Token: 0x06002C9F RID: 11423 RVA: 0x000DBCF1 File Offset: 0x000D9EF1
	private static void RemoveCustomMapATM()
	{
		if (ATM_Manager.instance.IsNotNull())
		{
			ATM_Manager.instance.RemoveATM(CustomMapLoader.customMapATM);
			ATM_Manager.instance.ResetTemporaryCreatorCode();
			CustomMapLoader.customMapATM = null;
		}
	}

	// Token: 0x06002CA0 RID: 11424 RVA: 0x000DBD24 File Offset: 0x000D9F24
	private static IEnumerator ResetLightmaps()
	{
		CustomMapLoader.instance.dayNightManager.RequestRepopulateLightmaps();
		LoadSceneParameters loadSceneParameters = new LoadSceneParameters
		{
			loadSceneMode = LoadSceneMode.Additive,
			localPhysicsMode = LocalPhysicsMode.None
		};
		AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(10, loadSceneParameters);
		yield return asyncOperation;
		asyncOperation = SceneManager.UnloadSceneAsync(10);
		yield return asyncOperation;
		yield break;
	}

	// Token: 0x06002CA1 RID: 11425 RVA: 0x000DBD2C File Offset: 0x000D9F2C
	private static void LoadLightmaps(Texture2D[] colorMaps, Texture2D[] dirMaps)
	{
		if (colorMaps.Length == 0)
		{
			return;
		}
		CustomMapLoader.UnloadLightmaps();
		List<LightmapData> list = new List<LightmapData>(LightmapSettings.lightmaps);
		for (int i = 0; i < colorMaps.Length; i++)
		{
			bool flag = false;
			LightmapData lightmapData = new LightmapData();
			if (colorMaps[i] != null)
			{
				lightmapData.lightmapColor = colorMaps[i];
				flag = true;
				if (i < dirMaps.Length && dirMaps[i] != null)
				{
					lightmapData.lightmapDir = dirMaps[i];
				}
			}
			if (flag)
			{
				list.Add(lightmapData);
			}
		}
		LightmapSettings.lightmaps = list.ToArray();
	}

	// Token: 0x06002CA2 RID: 11426 RVA: 0x000DBDAC File Offset: 0x000D9FAC
	private static void UnloadLightmaps()
	{
		foreach (LightmapData lightmapData in LightmapSettings.lightmaps)
		{
			if (lightmapData.lightmapColor != null && !CustomMapLoader.lightmapsToKeep.Contains(lightmapData.lightmapColor))
			{
				Resources.UnloadAsset(lightmapData.lightmapColor);
			}
			if (lightmapData.lightmapDir != null && !CustomMapLoader.lightmapsToKeep.Contains(lightmapData.lightmapDir))
			{
				Resources.UnloadAsset(lightmapData.lightmapDir);
			}
		}
	}

	// Token: 0x06002CA3 RID: 11427 RVA: 0x000DBE28 File Offset: 0x000DA028
	private static bool SanitizeObject(GameObject gameObject, GameObject mapRoot)
	{
		if (gameObject == null)
		{
			return false;
		}
		if (!CustomMapLoader.APPROVED_LAYERS.Contains(gameObject.layer))
		{
			gameObject.layer = 0;
		}
		foreach (Component component in gameObject.GetComponents<Component>())
		{
			if (component == null)
			{
				Object.DestroyImmediate(gameObject, true);
				return false;
			}
			bool flag = true;
			foreach (Type type in CustomMapLoader.componentAllowlist)
			{
				if (component.GetType() == typeof(Camera))
				{
					Camera camera = (Camera)component;
					if (camera.IsNotNull() && camera.targetTexture.IsNull())
					{
						break;
					}
				}
				if (component.GetType() == type)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				foreach (string text in CustomMapLoader.componentTypeStringAllowList)
				{
					if (component.GetType().ToString().Contains(text))
					{
						flag = false;
						break;
					}
				}
			}
			if (flag)
			{
				Object.DestroyImmediate(gameObject, true);
				return false;
			}
		}
		if (gameObject.transform.parent.IsNull() && gameObject.transform != mapRoot.transform)
		{
			gameObject.transform.SetParent(mapRoot.transform);
		}
		return true;
	}

	// Token: 0x06002CA4 RID: 11428 RVA: 0x000DBFB4 File Offset: 0x000DA1B4
	private static void SanitizeObjectRecursive(GameObject rootObject, GameObject mapRoot)
	{
		if (!CustomMapLoader.SanitizeObject(rootObject, mapRoot))
		{
			return;
		}
		CustomMapLoader.totalObjectsInLoadingScene++;
		for (int i = 0; i < rootObject.transform.childCount; i++)
		{
			GameObject gameObject = rootObject.transform.GetChild(i).gameObject;
			if (gameObject.IsNotNull())
			{
				CustomMapLoader.SanitizeObjectRecursive(gameObject, mapRoot);
			}
		}
	}

	// Token: 0x06002CA5 RID: 11429 RVA: 0x000DC00E File Offset: 0x000DA20E
	private static IEnumerator ProcessAndInstantiateMap(GameObject map, bool useProgressCallback)
	{
		if (map.IsNull() || !CustomMapLoader.hasInstance)
		{
			yield break;
		}
		if (useProgressCallback)
		{
			Action<MapLoadStatus, int, string> action = CustomMapLoader.modLoadProgressCallback;
			if (action != null)
			{
				action(MapLoadStatus.Loading, 73, "PROCESSING ROOT MAP OBJECT");
			}
		}
		CustomMapLoader.loadedMapDescriptor = map.GetComponent<MapDescriptor>();
		if (CustomMapLoader.loadedMapDescriptor.IsNull())
		{
			yield break;
		}
		CustomMapLoader.objectsProcessedForLoadingScene = 0;
		CustomMapLoader.objectsProcessedThisFrame = 0;
		if (useProgressCallback)
		{
			Action<MapLoadStatus, int, string> action2 = CustomMapLoader.modLoadProgressCallback;
			if (action2 != null)
			{
				action2(MapLoadStatus.Loading, 75, "PROCESSING CHILD OBJECTS");
			}
		}
		CustomMapLoader.initializePhaseTwoComponents.Clear();
		yield return CustomMapLoader.ProcessChildObjects(map, useProgressCallback);
		if (CustomMapLoader.shouldAbortSceneLoad)
		{
			yield break;
		}
		CustomMapLoader.InitializeComponentsPhaseTwo();
		CustomMapLoader.placeholderReplacements.Clear();
		if (useProgressCallback)
		{
			Action<MapLoadStatus, int, string> action3 = CustomMapLoader.modLoadProgressCallback;
			if (action3 != null)
			{
				action3(MapLoadStatus.Loading, 95, "PROCESSING COMPLETE");
			}
		}
		if (CustomMapLoader.loadedMapDescriptor.IsInitialScene)
		{
			VirtualStumpReturnWatch.SetWatchProperties(CustomMapLoader.loadedMapDescriptor.GetReturnToVStumpWatchProps());
		}
		yield break;
	}

	// Token: 0x06002CA6 RID: 11430 RVA: 0x000DC024 File Offset: 0x000DA224
	private static IEnumerator ProcessChildObjects(GameObject parent, bool useProgressCallback)
	{
		if (parent == null || CustomMapLoader.placeholderReplacements.Contains(parent))
		{
			yield break;
		}
		int num3;
		for (int i = 0; i < parent.transform.childCount; i = num3 + 1)
		{
			Transform child = parent.transform.GetChild(i);
			if (!(child == null))
			{
				GameObject gameObject = child.gameObject;
				if (!(gameObject == null) && !CustomMapLoader.placeholderReplacements.Contains(gameObject))
				{
					try
					{
						CustomMapLoader.SetupCollisions(gameObject);
						CustomMapLoader.ReplaceDataOnlyScripts(gameObject);
						CustomMapLoader.ReplacePlaceholders(gameObject);
						CustomMapLoader.InitializeComponentsPhaseOne(gameObject);
					}
					catch (Exception ex)
					{
						CustomMapLoader.shouldAbortSceneLoad = true;
						CustomMapLoader.cachedExceptionMessage = ex.Message;
						yield break;
					}
					if (gameObject.transform.childCount > 0)
					{
						yield return CustomMapLoader.ProcessChildObjects(gameObject, useProgressCallback);
						if (CustomMapLoader.shouldAbortSceneLoad)
						{
							yield break;
						}
					}
					if (CustomMapLoader.shouldAbortSceneLoad)
					{
						yield break;
					}
					CustomMapLoader.objectsProcessedForLoadingScene++;
					CustomMapLoader.objectsProcessedThisFrame++;
					if (CustomMapLoader.objectsProcessedThisFrame >= CustomMapLoader.numObjectsToProcessPerFrame)
					{
						CustomMapLoader.objectsProcessedThisFrame = 0;
						if (useProgressCallback)
						{
							float num = (float)CustomMapLoader.objectsProcessedForLoadingScene / (float)CustomMapLoader.totalObjectsInLoadingScene;
							int num2 = Mathf.FloorToInt(20f * num);
							Action<MapLoadStatus, int, string> action = CustomMapLoader.modLoadProgressCallback;
							if (action != null)
							{
								action(MapLoadStatus.Loading, 75 + num2, "PROCESSING CHILD OBJECTS");
							}
						}
						yield return null;
					}
				}
			}
			num3 = i;
		}
		yield break;
	}

	// Token: 0x06002CA7 RID: 11431 RVA: 0x000DC03C File Offset: 0x000DA23C
	private static void CheckVirtualStumpOverlap(string sceneName)
	{
		Vector3 vector = new Vector3(5.15f, 0.72f, 5.15f);
		Vector3 vector2 = new Vector3(0f, 0.73f, 0f);
		float num = vector.x * 0.5f + 2f;
		GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
		gameObject.transform.position = CustomMapLoader.instance.virtualStumpMesh.transform.position + vector2;
		gameObject.transform.localScale = vector;
		Collider[] array = Physics.OverlapSphere(gameObject.transform.position, num);
		if (array == null || array.Length == 0)
		{
			Object.DestroyImmediate(gameObject);
			return;
		}
		MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
		meshCollider.convex = true;
		foreach (Collider collider in array)
		{
			Vector3 vector3;
			float num2;
			if (!(collider == null) && !(collider.gameObject == gameObject) && !(collider.gameObject.scene.name != sceneName) && Physics.ComputePenetration(meshCollider, gameObject.transform.position, gameObject.transform.rotation, collider, collider.transform.position, collider.transform.rotation, out vector3, out num2) && !collider.isTrigger)
			{
				GTDev.Log<string>("[CustomMapLoader::CheckVirtualStumpOverlap] Gameobject " + collider.name + " has a collider overlapping with the virtual stump. Collider will be removed", null);
				Object.DestroyImmediate(collider);
			}
		}
		Object.DestroyImmediate(gameObject);
	}

	// Token: 0x06002CA8 RID: 11432 RVA: 0x000DC1C4 File Offset: 0x000DA3C4
	private static void SetupCollisions(GameObject gameObject)
	{
		if (gameObject == null || CustomMapLoader.placeholderReplacements.Contains(gameObject))
		{
			return;
		}
		Collider[] components = gameObject.GetComponents<Collider>();
		if (components == null)
		{
			return;
		}
		bool flag = true;
		bool flag2 = false;
		foreach (Collider collider in components)
		{
			if (!(collider == null))
			{
				if (collider.isTrigger)
				{
					flag2 = true;
					if (gameObject.layer != UnityLayer.GorillaInteractable.ToLayerIndex())
					{
						gameObject.layer = UnityLayer.GorillaTrigger.ToLayerIndex();
						break;
					}
				}
				else
				{
					flag = false;
					if (!flag2 && gameObject.layer == UnityLayer.Default.ToLayerIndex())
					{
						gameObject.layer = UnityLayer.GorillaObject.ToLayerIndex();
					}
				}
			}
		}
		if (!flag)
		{
			SurfaceOverrideSettings component = gameObject.GetComponent<SurfaceOverrideSettings>();
			GorillaSurfaceOverride gorillaSurfaceOverride = gameObject.AddComponent<GorillaSurfaceOverride>();
			if (component == null)
			{
				gorillaSurfaceOverride.overrideIndex = 0;
				return;
			}
			gorillaSurfaceOverride.overrideIndex = (int)component.soundOverride;
			gorillaSurfaceOverride.extraVelMultiplier = component.extraVelMultiplier;
			gorillaSurfaceOverride.extraVelMaxMultiplier = component.extraVelMaxMultiplier;
			gorillaSurfaceOverride.slidePercentageOverride = component.slidePercentage;
			gorillaSurfaceOverride.disablePushBackEffect = component.disablePushBackEffect;
			Object.Destroy(component);
		}
	}

	// Token: 0x06002CA9 RID: 11433 RVA: 0x000DC2D8 File Offset: 0x000DA4D8
	private static void ReplaceDataOnlyScripts(GameObject gameObject)
	{
		MapBoundarySettings component = gameObject.GetComponent<MapBoundarySettings>();
		if (component != null)
		{
			CMSMapBoundary cmsmapBoundary = gameObject.AddComponent<CMSMapBoundary>();
			if (cmsmapBoundary != null)
			{
				cmsmapBoundary.CopyTriggerSettings(component);
			}
			Object.Destroy(component);
		}
		TagZoneSettings component2 = gameObject.GetComponent<TagZoneSettings>();
		if (component2 != null)
		{
			CMSTagZone cmstagZone = gameObject.AddComponent<CMSTagZone>();
			if (cmstagZone != null)
			{
				cmstagZone.CopyTriggerSettings(component2);
			}
			Object.Destroy(component2);
		}
		TeleporterSettings component3 = gameObject.GetComponent<TeleporterSettings>();
		if (component3 != null)
		{
			CMSTeleporter cmsteleporter = gameObject.AddComponent<CMSTeleporter>();
			if (cmsteleporter != null)
			{
				cmsteleporter.CopyTriggerSettings(component3);
			}
			Object.Destroy(component3);
		}
		ObjectActivationTriggerSettings component4 = gameObject.GetComponent<ObjectActivationTriggerSettings>();
		if (component4 != null)
		{
			CMSObjectActivationTrigger cmsobjectActivationTrigger = gameObject.AddComponent<CMSObjectActivationTrigger>();
			if (cmsobjectActivationTrigger != null)
			{
				cmsobjectActivationTrigger.CopyTriggerSettings(component4);
			}
			Object.Destroy(component4);
		}
		LoadZoneSettings component5 = gameObject.GetComponent<LoadZoneSettings>();
		if (component5 != null)
		{
			CMSLoadingZone cmsloadingZone = gameObject.AddComponent<CMSLoadingZone>();
			if (cmsloadingZone != null)
			{
				cmsloadingZone.SetupLoadingZone(component5, in CustomMapLoader.assetBundleSceneFilePaths);
			}
			Object.Destroy(component5);
		}
		CMSZoneShaderSettings component6 = gameObject.GetComponent<CMSZoneShaderSettings>();
		if (component6.IsNotNull())
		{
			ZoneShaderSettings zoneShaderSettings = gameObject.AddComponent<ZoneShaderSettings>();
			zoneShaderSettings.CopySettings(component6, false, false);
			if (component6.isDefaultValues)
			{
				CustomMapManager.SetDefaultZoneShaderSettings(zoneShaderSettings, component6.GetProperties());
			}
			CustomMapManager.AddZoneShaderSettings(zoneShaderSettings);
			Object.Destroy(component6);
		}
		ZoneShaderTriggerSettings component7 = gameObject.GetComponent<ZoneShaderTriggerSettings>();
		if (component7.IsNotNull())
		{
			gameObject.AddComponent<CMSZoneShaderSettingsTrigger>().CopySettings(component7);
			Object.Destroy(component7);
		}
		HandHoldSettings component8 = gameObject.GetComponent<HandHoldSettings>();
		if (component8.IsNotNull())
		{
			gameObject.AddComponent<HandHold>().CopyProperties(component8);
			Object.Destroy(component8);
		}
		CustomMapEjectButtonSettings component9 = gameObject.GetComponent<CustomMapEjectButtonSettings>();
		if (component9.IsNotNull())
		{
			CustomMapEjectButton customMapEjectButton = gameObject.AddComponent<CustomMapEjectButton>();
			customMapEjectButton.gameObject.layer = UnityLayer.GorillaInteractable.ToLayerIndex();
			customMapEjectButton.CopySettings(component9);
			Object.Destroy(component9);
		}
	}

	// Token: 0x06002CAA RID: 11434 RVA: 0x000DC4A8 File Offset: 0x000DA6A8
	private static void ReplacePlaceholders(GameObject placeholderGameObject)
	{
		if (placeholderGameObject.IsNull())
		{
			return;
		}
		GTObjectPlaceholder component = placeholderGameObject.GetComponent<GTObjectPlaceholder>();
		if (component.IsNull())
		{
			return;
		}
		switch (component.PlaceholderObject)
		{
		case GTObject.LeafGlider:
			if (CustomMapLoader.leafGliderIndex < CustomMapLoader.instance.leafGliders.Length)
			{
				CustomMapLoader.instance.leafGliders[CustomMapLoader.leafGliderIndex].enabled = true;
				CustomMapLoader.instance.leafGliders[CustomMapLoader.leafGliderIndex].CustomMapLoad(component.transform, component.maxDistanceBeforeRespawn);
				CustomMapLoader.instance.leafGliders[CustomMapLoader.leafGliderIndex].transform.GetChild(0).gameObject.SetActive(true);
				CustomMapLoader.leafGliderIndex++;
				return;
			}
			break;
		case GTObject.GliderWindVolume:
		{
			List<Collider> list = new List<Collider>(component.GetComponents<Collider>());
			if (component.useDefaultPlaceholder || list.Count == 0)
			{
				GameObject gameObject = Object.Instantiate<GameObject>(CustomMapLoader.instance.gliderWindVolume, placeholderGameObject.transform.position, placeholderGameObject.transform.rotation);
				if (gameObject != null)
				{
					CustomMapLoader.placeholderReplacements.Add(gameObject);
					gameObject.transform.localScale = placeholderGameObject.transform.localScale;
					placeholderGameObject.transform.localScale = Vector3.one;
					gameObject.transform.SetParent(placeholderGameObject.transform);
					GliderWindVolume component2 = gameObject.GetComponent<GliderWindVolume>();
					if (component2 == null)
					{
						return;
					}
					component2.SetProperties(component.maxSpeed, component.maxAccel, component.SpeedVSAccelCurve, component.localWindDirection);
					return;
				}
			}
			else
			{
				placeholderGameObject.layer = UnityLayer.GorillaTrigger.ToLayerIndex();
				GliderWindVolume gliderWindVolume = placeholderGameObject.AddComponent<GliderWindVolume>();
				if (gliderWindVolume.IsNotNull())
				{
					gliderWindVolume.SetProperties(component.maxSpeed, component.maxAccel, component.SpeedVSAccelCurve, component.localWindDirection);
					return;
				}
			}
			break;
		}
		case GTObject.WaterVolume:
		{
			List<Collider> list = new List<Collider>(component.GetComponents<Collider>());
			if (component.useDefaultPlaceholder || list.Count == 0)
			{
				GameObject gameObject2 = Object.Instantiate<GameObject>(CustomMapLoader.instance.waterVolumePrefab, placeholderGameObject.transform.position, placeholderGameObject.transform.rotation);
				if (gameObject2 != null)
				{
					CustomMapLoader.placeholderReplacements.Add(gameObject2);
					gameObject2.layer = UnityLayer.Water.ToLayerIndex();
					gameObject2.transform.localScale = placeholderGameObject.transform.localScale;
					placeholderGameObject.transform.localScale = Vector3.one;
					gameObject2.transform.SetParent(placeholderGameObject.transform);
					MeshRenderer component3 = gameObject2.GetComponent<MeshRenderer>();
					if (component3.IsNull())
					{
						return;
					}
					if (!component.useWaterMesh)
					{
						component3.enabled = false;
						return;
					}
					component3.enabled = true;
					WaterSurfaceMaterialController component4 = gameObject2.GetComponent<WaterSurfaceMaterialController>();
					if (component4.IsNull())
					{
						return;
					}
					component4.ScrollX = component.scrollTextureX;
					component4.ScrollY = component.scrollTextureY;
					component4.Scale = component.scaleTexture;
					return;
				}
			}
			else
			{
				placeholderGameObject.layer = UnityLayer.Water.ToLayerIndex();
				WaterVolume waterVolume = placeholderGameObject.AddComponent<WaterVolume>();
				if (waterVolume.IsNotNull())
				{
					WaterParameters waterParameters = null;
					CMSZoneShaderSettings.EZoneLiquidType liquidType = component.liquidType;
					if (liquidType != CMSZoneShaderSettings.EZoneLiquidType.Water)
					{
						if (liquidType == CMSZoneShaderSettings.EZoneLiquidType.Lava)
						{
							waterParameters = CustomMapLoader.instance.defaultLavaParameters;
						}
					}
					else
					{
						waterParameters = CustomMapLoader.instance.defaultWaterParameters;
					}
					waterVolume.SetPropertiesFromPlaceholder(component.GetWaterVolumeProperties(), list, waterParameters);
					waterVolume.RefreshColliders();
					return;
				}
			}
			break;
		}
		case GTObject.ForceVolume:
		{
			List<Collider> list = new List<Collider>(component.GetComponents<Collider>());
			if (component.useDefaultPlaceholder || list.Count == 0)
			{
				GameObject gameObject3 = Object.Instantiate<GameObject>(CustomMapLoader.instance.forceVolumePrefab, placeholderGameObject.transform.position, placeholderGameObject.transform.rotation);
				if (gameObject3.IsNotNull())
				{
					CustomMapLoader.placeholderReplacements.Add(gameObject3);
					gameObject3.transform.localScale = placeholderGameObject.transform.localScale;
					placeholderGameObject.transform.localScale = Vector3.one;
					gameObject3.transform.SetParent(placeholderGameObject.transform);
					ForceVolume component5 = gameObject3.GetComponent<ForceVolume>();
					if (component5.IsNull())
					{
						return;
					}
					component5.SetPropertiesFromPlaceholder(component.GetForceVolumeProperties(), null, null);
					return;
				}
			}
			else
			{
				ForceVolume forceVolume = placeholderGameObject.AddComponent<ForceVolume>();
				if (forceVolume.IsNotNull())
				{
					AudioSource audioSource = placeholderGameObject.GetComponent<AudioSource>();
					if (audioSource.IsNull())
					{
						audioSource = placeholderGameObject.AddComponent<AudioSource>();
						audioSource.spatialize = true;
						audioSource.playOnAwake = false;
						audioSource.priority = 128;
						audioSource.volume = 0.522f;
						audioSource.pitch = 1f;
						audioSource.panStereo = 0f;
						audioSource.spatialBlend = 1f;
						audioSource.reverbZoneMix = 1f;
						audioSource.dopplerLevel = 1f;
						audioSource.spread = 0f;
						audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
						audioSource.minDistance = 8.2f;
						audioSource.maxDistance = 43.94f;
						audioSource.enabled = true;
					}
					audioSource.outputAudioMixerGroup = CustomMapLoader.instance.masterAudioMixer;
					for (int i = list.Count - 1; i >= 0; i--)
					{
						if (i == 0)
						{
							list[i].isTrigger = true;
						}
						else
						{
							Object.Destroy(list[i]);
						}
					}
					placeholderGameObject.layer = UnityLayer.GorillaBoundary.ToLayerIndex();
					forceVolume.SetPropertiesFromPlaceholder(component.GetForceVolumeProperties(), audioSource, component.GetComponent<Collider>());
					return;
				}
				Debug.LogError("[CustomMapLoader::ReplacePlaceholders] Failed to add ForceVolume component to Placeholder!");
				return;
			}
			break;
		}
		case GTObject.ATM:
		{
			if (CustomMapLoader.customMapATM.IsNotNull())
			{
				Debug.LogError("[CustomMapLoader::ReplacePlaceholders] Map already contains an ATM, maps are only allowed 1 ATM! Removing placeholder and not instantiating...");
				return;
			}
			if (CustomMapLoader.instance.atmPrefab.IsNull())
			{
				return;
			}
			GameObject gameObject4 = Object.Instantiate<GameObject>(CustomMapLoader.instance.atmPrefab, placeholderGameObject.transform.position, placeholderGameObject.transform.rotation);
			if (gameObject4.IsNotNull())
			{
				CustomMapLoader.placeholderReplacements.Add(gameObject4);
				gameObject4.transform.SetParent(placeholderGameObject.transform);
				ATM_UI componentInChildren = gameObject4.GetComponentInChildren<ATM_UI>();
				if (componentInChildren.IsNotNull() && ATM_Manager.instance.IsNotNull())
				{
					CustomMapLoader.customMapATM = componentInChildren;
					ATM_Manager.instance.AddATM(componentInChildren);
					if (!component.defaultCreatorCode.IsNullOrEmpty())
					{
						ATM_Manager.instance.SetTemporaryCreatorCode(component.defaultCreatorCode, true, null);
						return;
					}
				}
			}
			break;
		}
		case GTObject.HoverboardArea:
			if (component.AddComponent<HoverboardAreaTrigger>().IsNotNull())
			{
				component.gameObject.layer = UnityLayer.GorillaBoundary.ToLayerIndex();
				List<Collider> list = new List<Collider>(component.GetComponents<Collider>());
				if (list.Count != 0)
				{
					for (int j = list.Count - 1; j >= 0; j--)
					{
						if (j == 0)
						{
							list[j].isTrigger = true;
						}
						else
						{
							Object.Destroy(list[j]);
						}
					}
					return;
				}
				BoxCollider boxCollider = component.AddComponent<BoxCollider>();
				if (boxCollider.IsNotNull())
				{
					boxCollider.isTrigger = true;
					return;
				}
			}
			break;
		case GTObject.HoverboardDispenser:
		{
			if (CustomMapLoader.instance.hoverboardDispenserPrefab.IsNull())
			{
				Debug.LogError("[CustomMapLoader::ReplacePlaceholders] hoverboardDispenserPrefab is NULL!");
				return;
			}
			GameObject gameObject5 = Object.Instantiate<GameObject>(CustomMapLoader.instance.hoverboardDispenserPrefab, placeholderGameObject.transform.position, placeholderGameObject.transform.rotation);
			if (gameObject5.IsNotNull())
			{
				CustomMapLoader.placeholderReplacements.Add(gameObject5);
				gameObject5.transform.SetParent(placeholderGameObject.transform);
			}
			break;
		}
		default:
			return;
		}
	}

	// Token: 0x06002CAB RID: 11435 RVA: 0x000023F4 File Offset: 0x000005F4
	private static void InitializeComponentsPhaseOne(GameObject childGameObject)
	{
	}

	// Token: 0x06002CAC RID: 11436 RVA: 0x000DCBDC File Offset: 0x000DADDC
	private static void InitializeComponentsPhaseTwo()
	{
		for (int i = 0; i < CustomMapLoader.initializePhaseTwoComponents.Count; i++)
		{
		}
		CustomMapLoader.initializePhaseTwoComponents.Clear();
	}

	// Token: 0x06002CAD RID: 11437 RVA: 0x000DCC08 File Offset: 0x000DAE08
	public static bool OpenDoorToMap()
	{
		if (!CustomMapLoader.hasInstance)
		{
			return false;
		}
		if (CustomMapLoader.instance.accessDoor != null)
		{
			CustomMapLoader.instance.accessDoor.OpenDoor();
			return true;
		}
		return false;
	}

	// Token: 0x06002CAE RID: 11438 RVA: 0x000DCC3C File Offset: 0x000DAE3C
	private static void OnAssetBundleLoaded(bool loadSucceeded, bool loadAborted)
	{
		if (loadAborted)
		{
			return;
		}
		if (loadSucceeded)
		{
			CustomMapLoader.loadedMapModId = CustomMapLoader.attemptedLoadID;
			if (CustomMapLoader.initialSceneName != string.Empty)
			{
				CustomMapLoader.initialSceneIndex = CustomMapLoader.GetSceneIndex(CustomMapLoader.initialSceneName);
			}
			if (CustomMapLoader.initialSceneIndex == -1 && CustomMapLoader.mapBundle != null)
			{
				CustomMapLoader.mapBundle.Unload(true);
				CustomMapLoader.mapBundle = null;
				Action<MapLoadStatus, int, string> action = CustomMapLoader.modLoadProgressCallback;
				if (action != null)
				{
					action(MapLoadStatus.Error, 0, "ASSET BUNDLE CONTAINS MULTIPLE SCENES, BUT NONE ARE SET AS INITIAL SCENE.");
				}
				CustomMapLoader.OnLoadComplete(false, true, "");
			}
			CustomMapLoader.instance.StartCoroutine(CustomMapLoader.LoadSceneFromAssetBundle(CustomMapLoader.initialSceneIndex, true, new Action<bool, bool, string>(CustomMapLoader.OnLoadComplete)));
		}
	}

	// Token: 0x06002CAF RID: 11439 RVA: 0x000DCCEA File Offset: 0x000DAEEA
	private static void OnIncrementalLoadComplete(bool loadSucceeded, bool loadAborted, string loadedScene)
	{
		if (loadSucceeded)
		{
			CustomMapLoader.sceneLoadedCallback(loadedScene);
			return;
		}
		CustomMapLoader.instance.StopAllCoroutines();
		CustomMapLoader.isLoading = false;
	}

	// Token: 0x06002CB0 RID: 11440 RVA: 0x000DCD10 File Offset: 0x000DAF10
	private static void OnLoadComplete(bool loadSucceeded, bool loadAborted, string loadedScene)
	{
		CustomMapLoader.isLoading = false;
		GorillaNetworkJoinTrigger.EnableTriggerJoins();
		if (loadSucceeded)
		{
			Action<MapLoadStatus, int, string> action = CustomMapLoader.modLoadProgressCallback;
			if (action != null)
			{
				action(MapLoadStatus.Loading, 100, "Load Complete");
			}
			if (CustomMapLoader.instance.networkTrigger != null)
			{
				CustomMapLoader.instance.networkTrigger.SetActive(true);
			}
		}
		else
		{
			CustomMapLoader.loadedMapDescriptor = null;
		}
		if (!loadAborted)
		{
			Action<bool> action2 = CustomMapLoader.modLoadedCallback;
			if (action2 != null)
			{
				action2(loadSucceeded);
			}
			if (loadSucceeded)
			{
				Action<string> action3 = CustomMapLoader.sceneLoadedCallback;
				if (action3 == null)
				{
					return;
				}
				action3(loadedScene);
			}
		}
	}

	// Token: 0x06002CB1 RID: 11441 RVA: 0x000DCD98 File Offset: 0x000DAF98
	private static string GetSceneNameFromFilePath(string filePath)
	{
		string[] array = filePath.Split("/", StringSplitOptions.None);
		return array[array.Length - 1].Split(".", StringSplitOptions.None)[0];
	}

	// Token: 0x06002CB2 RID: 11442 RVA: 0x000DCDBC File Offset: 0x000DAFBC
	public static MapPackageInfo GetPackageInfo(string packageInfoFilePath)
	{
		MapPackageInfo mapPackageInfo;
		using (StreamReader streamReader = new StreamReader(File.OpenRead(packageInfoFilePath), Encoding.Default))
		{
			mapPackageInfo = JsonConvert.DeserializeObject<MapPackageInfo>(streamReader.ReadToEnd());
		}
		return mapPackageInfo;
	}

	// Token: 0x06002CB3 RID: 11443 RVA: 0x000DCE04 File Offset: 0x000DB004
	public static Transform GetCustomMapsDefaultSpawnLocation()
	{
		if (CustomMapLoader.hasInstance)
		{
			return CustomMapLoader.instance.CustomMapsDefaultSpawnLocation;
		}
		return null;
	}

	// Token: 0x06002CB4 RID: 11444 RVA: 0x000DCE1B File Offset: 0x000DB01B
	public static bool IsModLoaded(long mapModId = 0L)
	{
		if (mapModId != 0L)
		{
			return !CustomMapLoader.IsLoading && CustomMapLoader.LoadedMapModId == mapModId;
		}
		return !CustomMapLoader.IsLoading && CustomMapLoader.LoadedMapModId != 0L;
	}

	// Token: 0x040032CD RID: 13005
	[OnEnterPlay_SetNull]
	private static volatile CustomMapLoader instance;

	// Token: 0x040032CE RID: 13006
	[OnEnterPlay_Set(false)]
	private static bool hasInstance;

	// Token: 0x040032CF RID: 13007
	public Transform CustomMapsDefaultSpawnLocation;

	// Token: 0x040032D0 RID: 13008
	public CustomMapAccessDoor accessDoor;

	// Token: 0x040032D1 RID: 13009
	public GameObject networkTrigger;

	// Token: 0x040032D2 RID: 13010
	[SerializeField]
	private BetterDayNightManager dayNightManager;

	// Token: 0x040032D3 RID: 13011
	[SerializeField]
	private GameObject placeholderParent;

	// Token: 0x040032D4 RID: 13012
	[SerializeField]
	private GliderHoldable[] leafGliders;

	// Token: 0x040032D5 RID: 13013
	[SerializeField]
	private GameObject leafGlider;

	// Token: 0x040032D6 RID: 13014
	[SerializeField]
	private GameObject gliderWindVolume;

	// Token: 0x040032D7 RID: 13015
	[FormerlySerializedAs("waterVolume")]
	[SerializeField]
	private GameObject waterVolumePrefab;

	// Token: 0x040032D8 RID: 13016
	[SerializeField]
	private WaterParameters defaultWaterParameters;

	// Token: 0x040032D9 RID: 13017
	[SerializeField]
	private WaterParameters defaultLavaParameters;

	// Token: 0x040032DA RID: 13018
	[FormerlySerializedAs("forceVolume")]
	[SerializeField]
	private GameObject forceVolumePrefab;

	// Token: 0x040032DB RID: 13019
	[SerializeField]
	private GameObject atmPrefab;

	// Token: 0x040032DC RID: 13020
	[SerializeField]
	private GameObject hoverboardDispenserPrefab;

	// Token: 0x040032DD RID: 13021
	[SerializeField]
	private GameObject zoneShaderSettingsTrigger;

	// Token: 0x040032DE RID: 13022
	[SerializeField]
	private AudioMixerGroup masterAudioMixer;

	// Token: 0x040032DF RID: 13023
	[SerializeField]
	private ZoneShaderSettings customMapZoneShaderSettings;

	// Token: 0x040032E0 RID: 13024
	[SerializeField]
	private GameObject virtualStumpMesh;

	// Token: 0x040032E1 RID: 13025
	private static readonly int numObjectsToProcessPerFrame = 5;

	// Token: 0x040032E2 RID: 13026
	private static readonly List<int> APPROVED_LAYERS = new List<int>
	{
		0, 1, 2, 4, 5, 9, 11, 18, 22, 27,
		30
	};

	// Token: 0x040032E3 RID: 13027
	private static bool isLoading;

	// Token: 0x040032E4 RID: 13028
	private static bool isUnloading;

	// Token: 0x040032E5 RID: 13029
	private static bool runningAsyncLoad = false;

	// Token: 0x040032E6 RID: 13030
	private static long attemptedLoadID = 0L;

	// Token: 0x040032E7 RID: 13031
	private static string attemptedSceneToLoad;

	// Token: 0x040032E8 RID: 13032
	private static bool shouldUnloadMod = false;

	// Token: 0x040032E9 RID: 13033
	private static AssetBundle mapBundle;

	// Token: 0x040032EA RID: 13034
	private static string initialSceneName = string.Empty;

	// Token: 0x040032EB RID: 13035
	private static int initialSceneIndex = 0;

	// Token: 0x040032EC RID: 13036
	private static string loadedMapLevelName;

	// Token: 0x040032ED RID: 13037
	private static long loadedMapModId;

	// Token: 0x040032EE RID: 13038
	private static MapDescriptor loadedMapDescriptor;

	// Token: 0x040032EF RID: 13039
	private static Action<MapLoadStatus, int, string> modLoadProgressCallback;

	// Token: 0x040032F0 RID: 13040
	private static Action<bool> modLoadedCallback;

	// Token: 0x040032F1 RID: 13041
	private static Coroutine sceneLoadingCoroutine;

	// Token: 0x040032F2 RID: 13042
	private static Action<string> sceneLoadedCallback;

	// Token: 0x040032F3 RID: 13043
	private static Action<string> sceneUnloadedCallback;

	// Token: 0x040032F4 RID: 13044
	private static List<CustomMapLoader.LoadZoneRequest> queuedLoadZoneRequests = new List<CustomMapLoader.LoadZoneRequest>();

	// Token: 0x040032F5 RID: 13045
	private static string[] assetBundleSceneFilePaths;

	// Token: 0x040032F6 RID: 13046
	private static List<string> loadedSceneFilePaths = new List<string>();

	// Token: 0x040032F7 RID: 13047
	private static List<string> loadedSceneNames = new List<string>();

	// Token: 0x040032F8 RID: 13048
	private static List<int> loadedSceneIndexes = new List<int>();

	// Token: 0x040032F9 RID: 13049
	private static int leafGliderIndex;

	// Token: 0x040032FA RID: 13050
	private static int totalObjectsInLoadingScene = 0;

	// Token: 0x040032FB RID: 13051
	private static int objectsProcessedForLoadingScene = 0;

	// Token: 0x040032FC RID: 13052
	private static int objectsProcessedThisFrame = 0;

	// Token: 0x040032FD RID: 13053
	private static List<Component> initializePhaseTwoComponents = new List<Component>();

	// Token: 0x040032FE RID: 13054
	private static bool shouldAbortSceneLoad = false;

	// Token: 0x040032FF RID: 13055
	private static Action abortModLoadCallback;

	// Token: 0x04003300 RID: 13056
	private static Action unloadModCallback;

	// Token: 0x04003301 RID: 13057
	private static string cachedExceptionMessage = "";

	// Token: 0x04003302 RID: 13058
	private static LightmapData[] lightmaps;

	// Token: 0x04003303 RID: 13059
	private static List<Texture2D> lightmapsToKeep = new List<Texture2D>();

	// Token: 0x04003304 RID: 13060
	private static List<GameObject> placeholderReplacements = new List<GameObject>();

	// Token: 0x04003305 RID: 13061
	private static ATM_UI customMapATM = null;

	// Token: 0x04003306 RID: 13062
	private string dontDestroyOnLoadSceneName = "";

	// Token: 0x04003307 RID: 13063
	private static List<Type> componentAllowlist = new List<Type>
	{
		typeof(MeshRenderer),
		typeof(Transform),
		typeof(MeshFilter),
		typeof(MeshRenderer),
		typeof(Collider),
		typeof(BoxCollider),
		typeof(SphereCollider),
		typeof(CapsuleCollider),
		typeof(MeshCollider),
		typeof(Light),
		typeof(ReflectionProbe),
		typeof(AudioSource),
		typeof(Animator),
		typeof(SkinnedMeshRenderer),
		typeof(TextMesh),
		typeof(ParticleSystem),
		typeof(ParticleSystemRenderer),
		typeof(RectTransform),
		typeof(SpriteRenderer),
		typeof(BillboardRenderer),
		typeof(Canvas),
		typeof(CanvasRenderer),
		typeof(CanvasScaler),
		typeof(GraphicRaycaster),
		typeof(Rigidbody),
		typeof(TrailRenderer),
		typeof(LineRenderer),
		typeof(LensFlareComponentSRP),
		typeof(Camera),
		typeof(UniversalAdditionalCameraData),
		typeof(MapDescriptor),
		typeof(AccessDoorPlaceholder),
		typeof(MapOrientationPoint),
		typeof(SurfaceOverrideSettings),
		typeof(TeleporterSettings),
		typeof(TagZoneSettings),
		typeof(MapBoundarySettings),
		typeof(ObjectActivationTriggerSettings),
		typeof(LoadZoneSettings),
		typeof(GTObjectPlaceholder),
		typeof(CMSZoneShaderSettings),
		typeof(ZoneShaderTriggerSettings),
		typeof(MultiPartFire),
		typeof(HandHoldSettings),
		typeof(CustomMapEjectButtonSettings),
		typeof(ProBuilderMesh),
		typeof(TMP_Text),
		typeof(TextMeshPro),
		typeof(TextMeshProUGUI),
		typeof(UniversalAdditionalLightData),
		typeof(BakerySkyLight),
		typeof(BakeryDirectLight),
		typeof(BakeryPointLight),
		typeof(ftLightmapsStorage)
	};

	// Token: 0x04003308 RID: 13064
	private static readonly List<string> componentTypeStringAllowList = new List<string> { "UnityEngine.Halo" };

	// Token: 0x04003309 RID: 13065
	private static Type[] badComponents = new Type[]
	{
		typeof(EventTrigger),
		typeof(UIBehaviour),
		typeof(GorillaPressableButton),
		typeof(GorillaPressableDelayButton),
		typeof(Camera),
		typeof(AudioListener),
		typeof(VideoPlayer)
	};

	// Token: 0x020006FF RID: 1791
	private struct LoadZoneRequest
	{
		// Token: 0x0400330A RID: 13066
		public int[] sceneIndexesToLoad;

		// Token: 0x0400330B RID: 13067
		public int[] sceneIndexesToUnload;

		// Token: 0x0400330C RID: 13068
		public Action<string> onSceneLoadedCallback;

		// Token: 0x0400330D RID: 13069
		public Action<string> onSceneUnloadedCallback;
	}
}
