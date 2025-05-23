using System;
using System.Reflection;
using UnityEngine;

// Token: 0x020009AB RID: 2475
public class OnEnterPlay_Set : OnEnterPlay_Attribute
{
	// Token: 0x06003B56 RID: 15190 RVA: 0x0011B4B6 File Offset: 0x001196B6
	public OnEnterPlay_Set(object value)
	{
		this.value = value;
	}

	// Token: 0x06003B57 RID: 15191 RVA: 0x0011B4C5 File Offset: 0x001196C5
	public override void OnEnterPlay(FieldInfo field)
	{
		if (!field.IsStatic)
		{
			Debug.LogError(string.Format("Can't Set non-static field {0}.{1}", field.DeclaringType, field.Name));
			return;
		}
		field.SetValue(null, this.value);
	}

	// Token: 0x04003FD8 RID: 16344
	private object value;
}
