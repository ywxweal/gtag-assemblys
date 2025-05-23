using System;
using UnityEngine;

// Token: 0x02000826 RID: 2086
public class KIDUI_DebugScreen : MonoBehaviour
{
	// Token: 0x06003304 RID: 13060 RVA: 0x000FBB6A File Offset: 0x000F9D6A
	private void Awake()
	{
		Object.DestroyImmediate(base.gameObject);
	}

	// Token: 0x06003305 RID: 13061 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnResetUserAndQuit()
	{
	}

	// Token: 0x06003306 RID: 13062 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnClose()
	{
	}

	// Token: 0x06003307 RID: 13063 RVA: 0x00045F91 File Offset: 0x00044191
	public static string GetOrCreateUsername()
	{
		return null;
	}

	// Token: 0x06003308 RID: 13064 RVA: 0x000023F4 File Offset: 0x000005F4
	public void ResetAll()
	{
	}
}
