using System;
using System.Collections.Generic;

// Token: 0x0200023E RID: 574
public static class XSceneRefGlobalHub
{
	// Token: 0x06000D37 RID: 3383 RVA: 0x000455EC File Offset: 0x000437EC
	public static void Register(int ID, XSceneRefTarget obj)
	{
		if (ID > 0)
		{
			int sceneIndex = (int)obj.GetSceneIndex();
			if (sceneIndex >= 0)
			{
				XSceneRefGlobalHub.registry[sceneIndex][ID] = obj;
			}
		}
	}

	// Token: 0x06000D38 RID: 3384 RVA: 0x0004561C File Offset: 0x0004381C
	public static void Unregister(int ID, XSceneRefTarget obj)
	{
		int sceneIndex = (int)obj.GetSceneIndex();
		if (ID > 0 && sceneIndex >= 0)
		{
			XSceneRefGlobalHub.registry[sceneIndex].Remove(ID);
		}
	}

	// Token: 0x06000D39 RID: 3385 RVA: 0x0004564A File Offset: 0x0004384A
	public static bool TryResolve(SceneIndex sceneIndex, int ID, out XSceneRefTarget result)
	{
		return XSceneRefGlobalHub.registry[(int)sceneIndex].TryGetValue(ID, out result);
	}

	// Token: 0x040010CA RID: 4298
	private static List<Dictionary<int, XSceneRefTarget>> registry = new List<Dictionary<int, XSceneRefTarget>>
	{
		new Dictionary<int, XSceneRefTarget> { { 0, null } },
		new Dictionary<int, XSceneRefTarget> { { 0, null } },
		new Dictionary<int, XSceneRefTarget> { { 0, null } },
		new Dictionary<int, XSceneRefTarget> { { 0, null } },
		new Dictionary<int, XSceneRefTarget> { { 0, null } },
		new Dictionary<int, XSceneRefTarget> { { 0, null } },
		new Dictionary<int, XSceneRefTarget> { { 0, null } },
		new Dictionary<int, XSceneRefTarget> { { 0, null } },
		new Dictionary<int, XSceneRefTarget> { { 0, null } },
		new Dictionary<int, XSceneRefTarget> { { 0, null } },
		new Dictionary<int, XSceneRefTarget> { { 0, null } },
		new Dictionary<int, XSceneRefTarget> { { 0, null } },
		new Dictionary<int, XSceneRefTarget> { { 0, null } },
		new Dictionary<int, XSceneRefTarget> { { 0, null } },
		new Dictionary<int, XSceneRefTarget> { { 0, null } },
		new Dictionary<int, XSceneRefTarget> { { 0, null } }
	};
}
