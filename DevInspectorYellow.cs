using System;

// Token: 0x020001BD RID: 445
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class DevInspectorYellow : DevInspectorColor
{
	// Token: 0x06000AA0 RID: 2720 RVA: 0x0003A2EB File Offset: 0x000384EB
	public DevInspectorYellow()
		: base("#ff5")
	{
	}
}
