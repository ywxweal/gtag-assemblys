﻿using System;
using System.Reflection;
using UnityEngine;

// Token: 0x020009B4 RID: 2484
public class OnExitPlay_SetNew : OnExitPlay_Attribute
{
	// Token: 0x06003B69 RID: 15209 RVA: 0x0011B744 File Offset: 0x00119944
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
