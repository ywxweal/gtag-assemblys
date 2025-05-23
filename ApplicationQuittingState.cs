using System;
using UnityEngine;

// Token: 0x02000152 RID: 338
public static class ApplicationQuittingState
{
	// Token: 0x170000D5 RID: 213
	// (get) Token: 0x060008C8 RID: 2248 RVA: 0x0002FB43 File Offset: 0x0002DD43
	// (set) Token: 0x060008C9 RID: 2249 RVA: 0x0002FB4A File Offset: 0x0002DD4A
	public static bool IsQuitting { get; private set; }

	// Token: 0x060008CA RID: 2250 RVA: 0x0002FB52 File Offset: 0x0002DD52
	[RuntimeInitializeOnLoadMethod]
	private static void Init()
	{
		Application.quitting += ApplicationQuittingState.HandleApplicationQuitting;
	}

	// Token: 0x060008CB RID: 2251 RVA: 0x0002FB65 File Offset: 0x0002DD65
	private static void HandleApplicationQuitting()
	{
		ApplicationQuittingState.IsQuitting = true;
	}
}
