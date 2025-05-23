using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x0200023C RID: 572
public static class SceneIndexExtensions
{
	// Token: 0x06000D26 RID: 3366 RVA: 0x000452AF File Offset: 0x000434AF
	public static SceneIndex GetSceneIndex(this Scene scene)
	{
		return (SceneIndex)scene.buildIndex;
	}

	// Token: 0x06000D27 RID: 3367 RVA: 0x000452B8 File Offset: 0x000434B8
	public static SceneIndex GetSceneIndex(this GameObject obj)
	{
		return (SceneIndex)obj.scene.buildIndex;
	}

	// Token: 0x06000D28 RID: 3368 RVA: 0x000452D4 File Offset: 0x000434D4
	public static SceneIndex GetSceneIndex(this Component cmp)
	{
		return (SceneIndex)cmp.gameObject.scene.buildIndex;
	}

	// Token: 0x06000D29 RID: 3369 RVA: 0x000452F4 File Offset: 0x000434F4
	public static void AddCallbackOnSceneLoad(this SceneIndex scene, Action callback)
	{
		if (SceneIndexExtensions.onSceneLoadCallbacks == null)
		{
			SceneIndexExtensions.onSceneLoadCallbacks = new List<Action>[17];
			for (int i = 0; i < SceneIndexExtensions.onSceneLoadCallbacks.Length; i++)
			{
				SceneIndexExtensions.onSceneLoadCallbacks[i] = new List<Action>();
			}
			SceneManager.sceneLoaded += SceneIndexExtensions.OnSceneLoad;
		}
		SceneIndexExtensions.onSceneLoadCallbacks[(int)scene].Add(callback);
	}

	// Token: 0x06000D2A RID: 3370 RVA: 0x00045350 File Offset: 0x00043550
	public static void RemoveCallbackOnSceneLoad(this SceneIndex scene, Action callback)
	{
		SceneIndexExtensions.onSceneLoadCallbacks[(int)scene].Remove(callback);
	}

	// Token: 0x06000D2B RID: 3371 RVA: 0x00045360 File Offset: 0x00043560
	public static void OnSceneLoad(Scene scene, LoadSceneMode mode)
	{
		if (scene.buildIndex != -1)
		{
			foreach (Action action in SceneIndexExtensions.onSceneLoadCallbacks[scene.buildIndex])
			{
				action();
			}
		}
	}

	// Token: 0x06000D2C RID: 3372 RVA: 0x000453C4 File Offset: 0x000435C4
	public static void AddCallbackOnSceneUnload(this SceneIndex scene, Action callback)
	{
		if (SceneIndexExtensions.onSceneUnloadCallbacks == null)
		{
			SceneIndexExtensions.onSceneUnloadCallbacks = new List<Action>[17];
			for (int i = 0; i < SceneIndexExtensions.onSceneUnloadCallbacks.Length; i++)
			{
				SceneIndexExtensions.onSceneUnloadCallbacks[i] = new List<Action>();
			}
			SceneManager.sceneUnloaded += SceneIndexExtensions.OnSceneUnload;
		}
		SceneIndexExtensions.onSceneUnloadCallbacks[(int)scene].Add(callback);
	}

	// Token: 0x06000D2D RID: 3373 RVA: 0x00045420 File Offset: 0x00043620
	public static void RemoveCallbackOnSceneUnload(this SceneIndex scene, Action callback)
	{
		SceneIndexExtensions.onSceneUnloadCallbacks[(int)scene].Remove(callback);
	}

	// Token: 0x06000D2E RID: 3374 RVA: 0x00045430 File Offset: 0x00043630
	public static void OnSceneUnload(Scene scene)
	{
		if (scene.buildIndex != -1)
		{
			foreach (Action action in SceneIndexExtensions.onSceneUnloadCallbacks[scene.buildIndex])
			{
				action();
			}
		}
	}

	// Token: 0x06000D2F RID: 3375 RVA: 0x00045494 File Offset: 0x00043694
	[OnEnterPlay_Run]
	private static void Reset()
	{
		if (SceneIndexExtensions.onSceneLoadCallbacks != null)
		{
			SceneIndexExtensions.onSceneLoadCallbacks = null;
			SceneManager.sceneLoaded -= SceneIndexExtensions.OnSceneLoad;
		}
		if (SceneIndexExtensions.onSceneUnloadCallbacks != null)
		{
			SceneIndexExtensions.onSceneUnloadCallbacks = null;
			SceneManager.sceneUnloaded -= SceneIndexExtensions.OnSceneUnload;
		}
	}

	// Token: 0x040010C3 RID: 4291
	private const int SceneIndex_COUNT = 17;

	// Token: 0x040010C4 RID: 4292
	[OnEnterPlay_SetNull]
	private static List<Action>[] onSceneLoadCallbacks;

	// Token: 0x040010C5 RID: 4293
	[OnEnterPlay_SetNull]
	private static List<Action>[] onSceneUnloadCallbacks;
}
