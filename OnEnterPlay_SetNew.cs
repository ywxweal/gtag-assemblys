using System;
using System.Reflection;
using UnityEngine;

// Token: 0x020009AC RID: 2476
public class OnEnterPlay_SetNew : OnEnterPlay_Attribute
{
	// Token: 0x06003B58 RID: 15192 RVA: 0x0011B4F8 File Offset: 0x001196F8
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
