﻿using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000386 RID: 902
public class UiAxis2dInspector : MonoBehaviour
{
	// Token: 0x060014D9 RID: 5337 RVA: 0x000659AB File Offset: 0x00063BAB
	public void SetExtents(Vector2 xExtent, Vector2 yExtent)
	{
		this.m_xExtent = xExtent;
		this.m_yExtent = yExtent;
	}

	// Token: 0x060014DA RID: 5338 RVA: 0x000659BB File Offset: 0x00063BBB
	public void SetName(string name)
	{
		this.m_nameLabel.text = name;
	}

	// Token: 0x060014DB RID: 5339 RVA: 0x000659CC File Offset: 0x00063BCC
	public void SetValue(bool isTouching, Vector2 value)
	{
		this.m_handle.color = (isTouching ? Color.white : new Color(0.2f, 0.2f, 0.2f));
		Vector2 vector = new Vector2(Mathf.Clamp(value.x, this.m_xExtent.x, this.m_xExtent.y), Mathf.Clamp(value.y, this.m_yExtent.x, this.m_yExtent.y));
		this.m_valueLabel.text = string.Concat(new string[]
		{
			"[",
			vector.x.ToString("f2"),
			", ",
			vector.y.ToString("f2"),
			"]"
		});
		RectTransform component = this.m_handle.transform.parent.GetComponent<RectTransform>();
		Vector2 vector2 = ((component != null) ? new Vector2(Mathf.Abs(component.sizeDelta.x), Mathf.Abs(component.sizeDelta.y)) : new Vector2(Mathf.Abs(this.m_xExtent.y - this.m_xExtent.x), Mathf.Abs(this.m_yExtent.y - this.m_yExtent.x)));
		this.m_handle.transform.localPosition = new Vector3(vector.x * vector2.x * 0.5f, vector.y * vector2.y * 0.5f, 0f);
	}

	// Token: 0x0400172F RID: 5935
	[Header("Settings")]
	[SerializeField]
	private Vector2 m_xExtent = new Vector2(-1f, 1f);

	// Token: 0x04001730 RID: 5936
	[SerializeField]
	private Vector2 m_yExtent = new Vector2(-1f, 1f);

	// Token: 0x04001731 RID: 5937
	[Header("Components")]
	[SerializeField]
	private TextMeshProUGUI m_nameLabel;

	// Token: 0x04001732 RID: 5938
	[SerializeField]
	private TextMeshProUGUI m_valueLabel;

	// Token: 0x04001733 RID: 5939
	[SerializeField]
	private Image m_handle;
}
