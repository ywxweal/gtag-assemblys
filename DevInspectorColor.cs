using System;

// Token: 0x020001BA RID: 442
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class DevInspectorColor : Attribute
{
	// Token: 0x17000111 RID: 273
	// (get) Token: 0x06000A9C RID: 2716 RVA: 0x0003A2D4 File Offset: 0x000384D4
	public string Color { get; }

	// Token: 0x06000A9D RID: 2717 RVA: 0x0003A2DC File Offset: 0x000384DC
	public DevInspectorColor(string color)
	{
		this.Color = color;
	}
}
