using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000320 RID: 800
public class CustomDebugUI : MonoBehaviour
{
	// Token: 0x060012F9 RID: 4857 RVA: 0x0005A1DA File Offset: 0x000583DA
	private void Awake()
	{
		CustomDebugUI.instance = this;
	}

	// Token: 0x060012FA RID: 4858 RVA: 0x0005A1E4 File Offset: 0x000583E4
	public RectTransform AddTextField(string label, int targetCanvas = 0)
	{
		RectTransform component = Object.Instantiate<RectTransform>(this.textPrefab).GetComponent<RectTransform>();
		component.GetComponentInChildren<InputField>().text = label;
		DebugUIBuilder debugUIBuilder = DebugUIBuilder.instance;
		typeof(DebugUIBuilder).GetMethod("AddRect", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(debugUIBuilder, new object[] { component, targetCanvas });
		return component;
	}

	// Token: 0x060012FB RID: 4859 RVA: 0x0005A248 File Offset: 0x00058448
	public void RemoveFromCanvas(RectTransform element, int targetCanvas = 0)
	{
		DebugUIBuilder debugUIBuilder = DebugUIBuilder.instance;
		FieldInfo field = typeof(DebugUIBuilder).GetField("insertedElements", BindingFlags.Instance | BindingFlags.NonPublic);
		MethodInfo method = typeof(DebugUIBuilder).GetMethod("Relayout", BindingFlags.Instance | BindingFlags.NonPublic);
		List<RectTransform>[] array = (List<RectTransform>[])field.GetValue(debugUIBuilder);
		if (targetCanvas > -1 && targetCanvas < array.Length - 1)
		{
			array[targetCanvas].Remove(element);
			element.SetParent(null);
			method.Invoke(debugUIBuilder, new object[0]);
		}
	}

	// Token: 0x04001526 RID: 5414
	[SerializeField]
	private RectTransform textPrefab;

	// Token: 0x04001527 RID: 5415
	public static CustomDebugUI instance;

	// Token: 0x04001528 RID: 5416
	private const BindingFlags privateFlags = BindingFlags.Instance | BindingFlags.NonPublic;
}
