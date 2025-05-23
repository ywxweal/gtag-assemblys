using System;
using UnityEngine;

// Token: 0x02000969 RID: 2409
public static class AnimatorUtils
{
	// Token: 0x06003A1C RID: 14876 RVA: 0x00116DE4 File Offset: 0x00114FE4
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
