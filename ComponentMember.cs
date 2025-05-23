using System;

// Token: 0x020001BF RID: 447
public class ComponentMember
{
	// Token: 0x17000112 RID: 274
	// (get) Token: 0x06000AA2 RID: 2722 RVA: 0x0003A305 File Offset: 0x00038505
	public string Name { get; }

	// Token: 0x17000113 RID: 275
	// (get) Token: 0x06000AA3 RID: 2723 RVA: 0x0003A30D File Offset: 0x0003850D
	public string Value
	{
		get
		{
			return this.getValue();
		}
	}

	// Token: 0x17000114 RID: 276
	// (get) Token: 0x06000AA4 RID: 2724 RVA: 0x0003A31A File Offset: 0x0003851A
	public bool IsStarred { get; }

	// Token: 0x17000115 RID: 277
	// (get) Token: 0x06000AA5 RID: 2725 RVA: 0x0003A322 File Offset: 0x00038522
	public string Color { get; }

	// Token: 0x06000AA6 RID: 2726 RVA: 0x0003A32A File Offset: 0x0003852A
	public ComponentMember(string name, Func<string> getValue, bool isStarred, string color)
	{
		this.Name = name;
		this.getValue = getValue;
		this.IsStarred = isStarred;
		this.Color = color;
	}

	// Token: 0x04000D23 RID: 3363
	private Func<string> getValue;

	// Token: 0x04000D24 RID: 3364
	public string computedPrefix;

	// Token: 0x04000D25 RID: 3365
	public string computedSuffix;
}
