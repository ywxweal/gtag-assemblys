using System;
using UnityEngine;

// Token: 0x020001F6 RID: 502
[CreateAssetMenu(fileName = "PlatformTagJoin", menuName = "ScriptableObjects/PlatformTagJoin", order = 0)]
public class PlatformTagJoin : ScriptableObject
{
	// Token: 0x06000BA6 RID: 2982 RVA: 0x0003E1FA File Offset: 0x0003C3FA
	public override string ToString()
	{
		return this.PlatformTag;
	}

	// Token: 0x04000E35 RID: 3637
	public string PlatformTag = " ";
}
