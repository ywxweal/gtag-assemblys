using System;
using System.Diagnostics;

// Token: 0x02000157 RID: 343
[Conditional("UNITY_EDITOR")]
public class DarkBoxAttribute : Attribute
{
	// Token: 0x060008D6 RID: 2262 RVA: 0x0000224E File Offset: 0x0000044E
	public DarkBoxAttribute()
	{
	}

	// Token: 0x060008D7 RID: 2263 RVA: 0x0002FF19 File Offset: 0x0002E119
	public DarkBoxAttribute(bool withBorders)
	{
		this.withBorders = withBorders;
	}

	// Token: 0x04000A69 RID: 2665
	public readonly bool withBorders;
}
