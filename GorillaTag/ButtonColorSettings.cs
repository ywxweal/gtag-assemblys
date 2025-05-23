using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000D39 RID: 3385
	[CreateAssetMenu(fileName = "GorillaButtonColorSettings", menuName = "ScriptableObjects/GorillaButtonColorSettings", order = 0)]
	public class ButtonColorSettings : ScriptableObject
	{
		// Token: 0x0400583A RID: 22586
		public Color UnpressedColor;

		// Token: 0x0400583B RID: 22587
		public Color PressedColor;

		// Token: 0x0400583C RID: 22588
		[Tooltip("Optional\nThe time the change will be in effect")]
		public float PressedTime;
	}
}
