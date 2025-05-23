using System;

// Token: 0x020001BE RID: 446
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class DevInspectorCyan : DevInspectorColor
{
	// Token: 0x06000AA1 RID: 2721 RVA: 0x0003A2F8 File Offset: 0x000384F8
	public DevInspectorCyan()
		: base("#5ff")
	{
	}
}
