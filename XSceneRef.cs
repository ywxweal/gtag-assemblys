using System;
using UnityEngine;

// Token: 0x0200023D RID: 573
[Serializable]
public struct XSceneRef
{
	// Token: 0x06000D30 RID: 3376 RVA: 0x000454D4 File Offset: 0x000436D4
	public bool TryResolve(out XSceneRefTarget result)
	{
		if (this.TargetID == 0)
		{
			result = null;
			return true;
		}
		if (this.didCache && this.cached != null)
		{
			result = this.cached;
			return true;
		}
		XSceneRefTarget xsceneRefTarget;
		if (!XSceneRefGlobalHub.TryResolve(this.TargetScene, this.TargetID, out xsceneRefTarget))
		{
			result = null;
			return false;
		}
		this.cached = xsceneRefTarget;
		this.didCache = true;
		result = xsceneRefTarget;
		return true;
	}

	// Token: 0x06000D31 RID: 3377 RVA: 0x0004553C File Offset: 0x0004373C
	public bool TryResolve(out GameObject result)
	{
		XSceneRefTarget xsceneRefTarget;
		if (this.TryResolve(out xsceneRefTarget))
		{
			result = ((xsceneRefTarget == null) ? null : xsceneRefTarget.gameObject);
			return true;
		}
		result = null;
		return false;
	}

	// Token: 0x06000D32 RID: 3378 RVA: 0x00045570 File Offset: 0x00043770
	public bool TryResolve<T>(out T result) where T : Component
	{
		XSceneRefTarget xsceneRefTarget;
		if (this.TryResolve(out xsceneRefTarget))
		{
			result = ((xsceneRefTarget == null) ? default(T) : xsceneRefTarget.GetComponent<T>());
			return true;
		}
		result = default(T);
		return false;
	}

	// Token: 0x06000D33 RID: 3379 RVA: 0x000455B1 File Offset: 0x000437B1
	public void AddCallbackOnLoad(Action callback)
	{
		this.TargetScene.AddCallbackOnSceneLoad(callback);
	}

	// Token: 0x06000D34 RID: 3380 RVA: 0x000455BF File Offset: 0x000437BF
	public void RemoveCallbackOnLoad(Action callback)
	{
		this.TargetScene.RemoveCallbackOnSceneLoad(callback);
	}

	// Token: 0x06000D35 RID: 3381 RVA: 0x000455CD File Offset: 0x000437CD
	public void AddCallbackOnUnload(Action callback)
	{
		this.TargetScene.AddCallbackOnSceneUnload(callback);
	}

	// Token: 0x06000D36 RID: 3382 RVA: 0x000455DB File Offset: 0x000437DB
	public void RemoveCallbackOnUnload(Action callback)
	{
		this.TargetScene.RemoveCallbackOnSceneUnload(callback);
	}

	// Token: 0x040010C6 RID: 4294
	public SceneIndex TargetScene;

	// Token: 0x040010C7 RID: 4295
	public int TargetID;

	// Token: 0x040010C8 RID: 4296
	private XSceneRefTarget cached;

	// Token: 0x040010C9 RID: 4297
	private bool didCache;
}
