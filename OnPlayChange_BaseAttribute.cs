using System;
using System.Reflection;

// Token: 0x020009A9 RID: 2473
public class OnPlayChange_BaseAttribute : Attribute
{
	// Token: 0x06003B51 RID: 15185 RVA: 0x000023F4 File Offset: 0x000005F4
	public virtual void OnEnterPlay(FieldInfo field)
	{
	}

	// Token: 0x06003B52 RID: 15186 RVA: 0x000023F4 File Offset: 0x000005F4
	public virtual void OnEnterPlay(MethodInfo method)
	{
	}
}
