using System;
using UnityEngine;

// Token: 0x02000969 RID: 2409
public static class AnimatorUtils
{
	// Token: 0x06003A1B RID: 14875 RVA: 0x00116D0C File Offset: 0x00114F0C
	public static void ResetToEntryState(this Animator a)
	{
		if (a == null)
		{
			return;
		}
		a.Rebind();
		a.Update(0f);
	}
}
