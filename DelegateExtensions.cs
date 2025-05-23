using System;
using System.Collections.Generic;

// Token: 0x020001C4 RID: 452
public static class DelegateExtensions
{
	// Token: 0x06000AB0 RID: 2736 RVA: 0x0003A42C File Offset: 0x0003862C
	public static List<string> ToStringList(this Delegate[] invocationList)
	{
		List<string> list = new List<string>();
		if (invocationList != null)
		{
			foreach (Delegate @delegate in invocationList)
			{
				string name = @delegate.Method.Name;
				string text = ((@delegate.Target != null) ? @delegate.Target.GetType().FullName : "Static Method");
				list.Add(text + "." + name);
			}
		}
		return list;
	}

	// Token: 0x06000AB1 RID: 2737 RVA: 0x0003A499 File Offset: 0x00038699
	public static string ToText(this Delegate[] invocationList)
	{
		return string.Join(", ", invocationList.ToStringList());
	}
}
