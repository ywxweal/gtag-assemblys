using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x020001F0 RID: 496
public class PerSceneRenderData : MonoBehaviour
{
	// Token: 0x06000B7B RID: 2939 RVA: 0x0003D538 File Offset: 0x0003B738
	private void RefreshRenderer()
	{
		int sceneIndex = this.sceneIndex;
		new List<Renderer>();
		foreach (Renderer renderer in Object.FindObjectsByType<Renderer>(FindObjectsSortMode.None))
		{
			if (renderer.gameObject.scene.buildIndex == sceneIndex)
			{
				this.representativeRenderer = renderer;
				return;
			}
		}
	}

	// Token: 0x17000124 RID: 292
	// (get) Token: 0x06000B7C RID: 2940 RVA: 0x0003D58C File Offset: 0x0003B78C
	public string sceneName
	{
		get
		{
			return base.gameObject.scene.name;
		}
	}

	// Token: 0x17000125 RID: 293
	// (get) Token: 0x06000B7D RID: 2941 RVA: 0x0003D5AC File Offset: 0x0003B7AC
	public int sceneIndex
	{
		get
		{
			return base.gameObject.scene.buildIndex;
		}
	}

	// Token: 0x06000B7E RID: 2942 RVA: 0x0003D5CC File Offset: 0x0003B7CC
	private void Awake()
	{
		for (int i = 0; i < this.mRendererIndex; i++)
		{
			this.mRenderers[i] = this.gO[i].GetComponent<MeshRenderer>();
		}
	}

	// Token: 0x06000B7F RID: 2943 RVA: 0x0003D5FF File Offset: 0x0003B7FF
	private void OnEnable()
	{
		BetterDayNightManager.Register(this);
	}

	// Token: 0x06000B80 RID: 2944 RVA: 0x0003D607 File Offset: 0x0003B807
	private void OnDisable()
	{
		BetterDayNightManager.Unregister(this);
	}

	// Token: 0x06000B81 RID: 2945 RVA: 0x0003D610 File Offset: 0x0003B810
	public void AddMeshToList(GameObject _gO, MeshRenderer mR)
	{
		try
		{
			if (mR.lightmapIndex != -1)
			{
				this.gO[this.mRendererIndex] = _gO;
				this.mRendererIndex++;
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
	}

	// Token: 0x06000B82 RID: 2946 RVA: 0x0003D65C File Offset: 0x0003B85C
	public bool CheckShouldRepopulate()
	{
		return this.representativeRenderer.lightmapIndex != this.lastLightmapIndex;
	}

	// Token: 0x17000126 RID: 294
	// (get) Token: 0x06000B83 RID: 2947 RVA: 0x0003D674 File Offset: 0x0003B874
	public bool IsLoadingLightmaps
	{
		get
		{
			return this.resourceRequests.Count != 0;
		}
	}

	// Token: 0x17000127 RID: 295
	// (get) Token: 0x06000B84 RID: 2948 RVA: 0x0003D684 File Offset: 0x0003B884
	public int LoadingLightmapsCount
	{
		get
		{
			return this.resourceRequests.Count;
		}
	}

	// Token: 0x06000B85 RID: 2949 RVA: 0x0003D694 File Offset: 0x0003B894
	private Texture2D GetLightmap(string timeOfDay)
	{
		if (this.singleLightmap != null)
		{
			return this.singleLightmap;
		}
		Texture2D texture2D;
		if (!this.lightmapsCache.TryGetValue(timeOfDay, out texture2D))
		{
			ResourceRequest request;
			if (this.resourceRequests.TryGetValue(timeOfDay, out request))
			{
				return null;
			}
			request = Resources.LoadAsync<Texture2D>(Path.Combine(this.lightmapsResourcePath, timeOfDay));
			this.resourceRequests.Add(timeOfDay, request);
			request.completed += delegate(AsyncOperation ao)
			{
				if (this == null)
				{
					return;
				}
				this.lightmapsCache.Add(timeOfDay, (Texture2D)request.asset);
				this.resourceRequests.Remove(timeOfDay);
				if (BetterDayNightManager.instance != null)
				{
					BetterDayNightManager.instance.RequestRepopulateLightmaps();
				}
			};
		}
		return texture2D;
	}

	// Token: 0x06000B86 RID: 2950 RVA: 0x0003D748 File Offset: 0x0003B948
	public void PopulateLightmaps(string fromTimeOfDay, string toTimeOfDay, LightmapData[] lightmaps)
	{
		LightmapData lightmapData = new LightmapData();
		lightmapData.lightmapColor = this.GetLightmap(fromTimeOfDay);
		lightmapData.lightmapDir = this.GetLightmap(toTimeOfDay);
		if (lightmapData.lightmapColor != null && lightmapData.lightmapDir != null && this.representativeRenderer.lightmapIndex < lightmaps.Length)
		{
			lightmaps[this.representativeRenderer.lightmapIndex] = lightmapData;
		}
		this.lastLightmapIndex = this.representativeRenderer.lightmapIndex;
		for (int i = 0; i < this.mRendererIndex; i++)
		{
			if (i < this.mRenderers.Length && this.mRenderers[i] != null)
			{
				this.mRenderers[i].lightmapIndex = this.lastLightmapIndex;
			}
		}
	}

	// Token: 0x06000B87 RID: 2951 RVA: 0x0003D800 File Offset: 0x0003BA00
	public void ReleaseLightmap(string oldTimeOfDay)
	{
		Texture2D texture2D;
		if (this.lightmapsCache.Remove(oldTimeOfDay, out texture2D))
		{
			Resources.UnloadAsset(texture2D);
		}
	}

	// Token: 0x06000B88 RID: 2952 RVA: 0x0003D824 File Offset: 0x0003BA24
	private void TryGetLightmapOrAsyncLoad(string momentName, Action<Texture2D> callback)
	{
		if (this.singleLightmap != null)
		{
			callback(this.singleLightmap);
		}
		Texture2D texture2D;
		if (this.lightmapsCache.TryGetValue(momentName, out texture2D))
		{
			callback(texture2D);
		}
		List<Action<Texture2D>> callbacks;
		if (!this._momentName_to_callbacks.TryGetValue(momentName, out callbacks))
		{
			callbacks = new List<Action<Texture2D>>(8);
			this._momentName_to_callbacks[momentName] = callbacks;
		}
		if (!callbacks.Contains(callback))
		{
			callbacks.Add(callback);
		}
		ResourceRequest request;
		if (this.resourceRequests.TryGetValue(momentName, out request))
		{
			return;
		}
		request = Resources.LoadAsync<Texture2D>(Path.Combine(this.lightmapsResourcePath, momentName));
		this.resourceRequests.Add(momentName, request);
		request.completed += delegate(AsyncOperation ao)
		{
			if (this == null || ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			Texture2D texture2D2 = (Texture2D)request.asset;
			this.lightmapsCache.Add(momentName, texture2D2);
			this.resourceRequests.Remove(momentName);
			foreach (Action<Texture2D> action in callbacks)
			{
				if (action != null)
				{
					action(texture2D2);
				}
			}
			callbacks.Clear();
		};
	}

	// Token: 0x06000B89 RID: 2953 RVA: 0x0003D938 File Offset: 0x0003BB38
	public bool IsLightmapWithNameLoaded(string lightmapName)
	{
		if (this.singleLightmap != null)
		{
			return true;
		}
		string text;
		string text2;
		this.GetFromAndToLightmapNames(out text, out text2);
		return !string.IsNullOrEmpty(lightmapName) && ((!string.IsNullOrEmpty(text) && text == lightmapName) || (!string.IsNullOrEmpty(text2) && text2 == lightmapName));
	}

	// Token: 0x06000B8A RID: 2954 RVA: 0x0003D990 File Offset: 0x0003BB90
	public bool IsLightmapsWithNamesLoaded(string fromLightmapName, string toLightmapName)
	{
		if (this.singleLightmap != null)
		{
			return true;
		}
		string text;
		string text2;
		this.GetFromAndToLightmapNames(out text, out text2);
		return !string.IsNullOrEmpty(fromLightmapName) && !string.IsNullOrEmpty(toLightmapName) && !string.IsNullOrEmpty(text) && text == fromLightmapName && !string.IsNullOrEmpty(text2) && text2 == toLightmapName;
	}

	// Token: 0x06000B8B RID: 2955 RVA: 0x0003D9EC File Offset: 0x0003BBEC
	public void GetFromAndToLightmapNames(out string fromLightmapName, out string toLightmapName)
	{
		if (this.singleLightmap != null)
		{
			fromLightmapName = null;
			toLightmapName = null;
			return;
		}
		LightmapData[] lightmaps = LightmapSettings.lightmaps;
		if (this.representativeRenderer.lightmapIndex < 0 || this.representativeRenderer.lightmapIndex >= lightmaps.Length)
		{
			fromLightmapName = null;
			toLightmapName = null;
			return;
		}
		Texture2D lightmapColor = lightmaps[this.representativeRenderer.lightmapIndex].lightmapColor;
		Texture2D lightmapDir = lightmaps[this.representativeRenderer.lightmapIndex].lightmapDir;
		fromLightmapName = ((lightmapColor != null) ? lightmapColor.name : null);
		toLightmapName = ((lightmapDir != null) ? lightmapDir.name : null);
	}

	// Token: 0x06000B8C RID: 2956 RVA: 0x0003DA88 File Offset: 0x0003BC88
	public static void g_StartAllScenesPopulateLightmaps(string fromLightmapName, string toLightmapName)
	{
		PerSceneRenderData._g_allScenesPopulateLightmaps_renderDatasHashSet.Clear();
		PerSceneRenderData[] array = Object.FindObjectsByType<PerSceneRenderData>(FindObjectsSortMode.None);
		PerSceneRenderData._g_allScenesPopulateLightmaps_renderDatasHashSet.UnionWith(array);
		foreach (PerSceneRenderData perSceneRenderData in array)
		{
			perSceneRenderData.StartPopulateLightmaps(fromLightmapName, toLightmapName);
			perSceneRenderData.OnPopulateToAndFromLightmapsCompleted = (Action<PerSceneRenderData>)Delegate.Combine(perSceneRenderData.OnPopulateToAndFromLightmapsCompleted, new Action<PerSceneRenderData>(PerSceneRenderData._g_AllScenesPopulateLightmaps_OnOneCompleted));
		}
	}

	// Token: 0x06000B8D RID: 2957 RVA: 0x0003DAF0 File Offset: 0x0003BCF0
	private static void _g_AllScenesPopulateLightmaps_OnOneCompleted(PerSceneRenderData perSceneRenderData)
	{
		int count = PerSceneRenderData._g_allScenesPopulateLightmaps_renderDatasHashSet.Count;
		PerSceneRenderData._g_allScenesPopulateLightmaps_renderDatasHashSet.Remove(perSceneRenderData);
		int count2 = PerSceneRenderData._g_allScenesPopulateLightmaps_renderDatasHashSet.Count;
		if (count2 == 0 && count2 != count)
		{
			Action action = PerSceneRenderData.g_OnAllScenesPopulateLightmapsCompleted;
			if (action == null)
			{
				return;
			}
			action();
		}
	}

	// Token: 0x17000128 RID: 296
	// (get) Token: 0x06000B8E RID: 2958 RVA: 0x0003DB35 File Offset: 0x0003BD35
	public static int g_AllScenesPopulatingLightmapsLoadCount
	{
		get
		{
			return PerSceneRenderData._g_allScenesPopulateLightmaps_renderDatasHashSet.Count;
		}
	}

	// Token: 0x06000B8F RID: 2959 RVA: 0x0003DB44 File Offset: 0x0003BD44
	public void StartPopulateLightmaps(string fromMomentName, string toMomentName)
	{
		PerSceneRenderData._g_allScenesPopulateLightmaps_renderDatasHashSet.Clear();
		this._populateLightmaps_fromMomentLightmap = null;
		this._populateLightmaps_toMomentLightmap = null;
		this._populateLightmaps_fromMomentName = fromMomentName;
		this._populateLightmaps_toMomentName = toMomentName;
		this.TryGetLightmapOrAsyncLoad(fromMomentName, new Action<Texture2D>(this._PopulateLightmaps_OnLoadLightmap));
		this.TryGetLightmapOrAsyncLoad(toMomentName, new Action<Texture2D>(this._PopulateLightmaps_OnLoadLightmap));
	}

	// Token: 0x06000B90 RID: 2960 RVA: 0x0003DBA0 File Offset: 0x0003BDA0
	private void _PopulateLightmaps_OnLoadLightmap(Texture2D lightmapTex)
	{
		if (this == null || ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		if (this._populateLightmaps_fromMomentName != lightmapTex.name)
		{
			this._populateLightmaps_fromMomentLightmap = lightmapTex;
		}
		if (this._populateLightmaps_toMomentName != lightmapTex.name)
		{
			this._populateLightmaps_toMomentLightmap = lightmapTex;
		}
		if (this._populateLightmaps_fromMomentLightmap != null && this._populateLightmaps_toMomentLightmap != null)
		{
			LightmapData[] lightmaps = LightmapSettings.lightmaps;
			LightmapData lightmapData = new LightmapData
			{
				lightmapColor = this._populateLightmaps_fromMomentLightmap,
				lightmapDir = this._populateLightmaps_toMomentLightmap
			};
			if (this.representativeRenderer.lightmapIndex >= 0 && this.representativeRenderer.lightmapIndex < lightmaps.Length)
			{
				lightmaps[this.representativeRenderer.lightmapIndex] = lightmapData;
			}
			LightmapSettings.lightmaps = lightmaps;
			this.lastLightmapIndex = this.representativeRenderer.lightmapIndex;
			for (int i = 0; i < this.mRendererIndex; i++)
			{
				if (i < this.mRenderers.Length && this.mRenderers[i] != null)
				{
					this.mRenderers[i].lightmapIndex = this.lastLightmapIndex;
				}
			}
			Action<PerSceneRenderData> onPopulateToAndFromLightmapsCompleted = this.OnPopulateToAndFromLightmapsCompleted;
			if (onPopulateToAndFromLightmapsCompleted == null)
			{
				return;
			}
			onPopulateToAndFromLightmapsCompleted(this);
		}
	}

	// Token: 0x04000E14 RID: 3604
	public Renderer representativeRenderer;

	// Token: 0x04000E15 RID: 3605
	public string lightmapsResourcePath;

	// Token: 0x04000E16 RID: 3606
	public Texture2D singleLightmap;

	// Token: 0x04000E17 RID: 3607
	private int lastLightmapIndex = -1;

	// Token: 0x04000E18 RID: 3608
	public GameObject[] gO = new GameObject[5000];

	// Token: 0x04000E19 RID: 3609
	public MeshRenderer[] mRenderers = new MeshRenderer[5000];

	// Token: 0x04000E1A RID: 3610
	public int mRendererIndex;

	// Token: 0x04000E1B RID: 3611
	private readonly Dictionary<string, ResourceRequest> resourceRequests = new Dictionary<string, ResourceRequest>(8);

	// Token: 0x04000E1C RID: 3612
	private readonly Dictionary<string, Texture2D> lightmapsCache = new Dictionary<string, Texture2D>(8);

	// Token: 0x04000E1D RID: 3613
	private Dictionary<string, List<Action<Texture2D>>> _momentName_to_callbacks = new Dictionary<string, List<Action<Texture2D>>>(8);

	// Token: 0x04000E1E RID: 3614
	private static readonly HashSet<PerSceneRenderData> _g_allScenesPopulateLightmaps_renderDatasHashSet = new HashSet<PerSceneRenderData>(32);

	// Token: 0x04000E1F RID: 3615
	public static Action g_OnAllScenesPopulateLightmapsCompleted;

	// Token: 0x04000E20 RID: 3616
	private string _populateLightmaps_fromMomentName;

	// Token: 0x04000E21 RID: 3617
	private string _populateLightmaps_toMomentName;

	// Token: 0x04000E22 RID: 3618
	private Texture2D _populateLightmaps_fromMomentLightmap;

	// Token: 0x04000E23 RID: 3619
	private Texture2D _populateLightmaps_toMomentLightmap;

	// Token: 0x04000E24 RID: 3620
	public Action<PerSceneRenderData> OnPopulateToAndFromLightmapsCompleted;
}
