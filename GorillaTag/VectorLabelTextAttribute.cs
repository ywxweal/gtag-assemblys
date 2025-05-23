using System;
using System.Diagnostics;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000D02 RID: 3330
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class VectorLabelTextAttribute : PropertyAttribute
	{
		// Token: 0x0600538D RID: 21389 RVA: 0x00195926 File Offset: 0x00193B26
		public VectorLabelTextAttribute(params string[] labels)
			: this(-1, labels)
		{
		}

		// Token: 0x0600538E RID: 21390 RVA: 0x0001218B File Offset: 0x0001038B
		public VectorLabelTextAttribute(int width, params string[] labels)
		{
		}
	}
}
