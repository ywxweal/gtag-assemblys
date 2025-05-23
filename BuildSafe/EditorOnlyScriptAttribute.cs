using System;
using System.Diagnostics;

namespace BuildSafe
{
	// Token: 0x02000BAD RID: 2989
	[Conditional("UNITY_EDITOR")]
	[AttributeUsage(AttributeTargets.Class)]
	public class EditorOnlyScriptAttribute : Attribute
	{
	}
}
