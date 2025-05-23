using System;
using System.Diagnostics;

// Token: 0x02000159 RID: 345
[Conditional("UNITY_EDITOR")]
[AttributeUsage(AttributeTargets.All)]
public class InlineAttribute : Attribute
{
	// Token: 0x060008D9 RID: 2265 RVA: 0x0002FF28 File Offset: 0x0002E128
	public InlineAttribute(bool keepLabel = false, bool asGroup = false)
	{
		this.keepLabel = keepLabel;
		this.asGroup = asGroup;
	}

	// Token: 0x04000A6A RID: 2666
	public readonly bool keepLabel;

	// Token: 0x04000A6B RID: 2667
	public readonly bool asGroup;
}
