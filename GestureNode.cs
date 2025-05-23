using System;
using UnityEngine;

// Token: 0x0200017A RID: 378
[Serializable]
public class GestureNode
{
	// Token: 0x04000B52 RID: 2898
	public bool track;

	// Token: 0x04000B53 RID: 2899
	public GestureHandState state;

	// Token: 0x04000B54 RID: 2900
	public GestureDigitFlexion flexion;

	// Token: 0x04000B55 RID: 2901
	public GestureAlignment alignment;

	// Token: 0x04000B56 RID: 2902
	[Space]
	public GestureNodeFlags flags;
}
