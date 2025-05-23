using System;
using System.Reflection;
using UnityEngine;

// Token: 0x020009AE RID: 2478
public class OnEnterPlay_Run : OnEnterPlay_Attribute
{
	// Token: 0x06003B5D RID: 15197 RVA: 0x0011B67C File Offset: 0x0011987C
	public override void OnEnterPlay(MethodInfo method)
	{
		if (!method.IsStatic)
		{
			Debug.LogError(string.Format("Can't Run non-static method {0}.{1}", method.DeclaringType, method.Name));
			return;
		}
		method.Invoke(null, new object[0]);
	}
}
