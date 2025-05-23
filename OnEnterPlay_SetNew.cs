using System;
using System.Reflection;
using UnityEngine;

// Token: 0x020009AC RID: 2476
public class OnEnterPlay_SetNew : OnEnterPlay_Attribute
{
	// Token: 0x06003B59 RID: 15193 RVA: 0x0011B5D0 File Offset: 0x001197D0
	public override void OnEnterPlay(FieldInfo field)
	{
		if (!field.IsStatic)
		{
			Debug.LogError(string.Format("Can't SetNew non-static field {0}.{1}", field.DeclaringType, field.Name));
			return;
		}
		object obj = field.FieldType.GetConstructor(new Type[0]).Invoke(new object[0]);
		field.SetValue(null, obj);
	}
}
