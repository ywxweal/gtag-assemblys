using System;
using System.Reflection;
using UnityEngine;

// Token: 0x020009AD RID: 2477
public class OnEnterPlay_Clear : OnEnterPlay_Attribute
{
	// Token: 0x06003B5B RID: 15195 RVA: 0x0011B628 File Offset: 0x00119828
	public override void OnEnterPlay(FieldInfo field)
	{
		if (!field.IsStatic)
		{
			Debug.LogError(string.Format("Can't Clear non-static field {0}.{1}", field.DeclaringType, field.Name));
			return;
		}
		field.FieldType.GetMethod("Clear").Invoke(field.GetValue(null), new object[0]);
	}
}
