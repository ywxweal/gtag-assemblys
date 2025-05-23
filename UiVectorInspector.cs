using System;
using TMPro;
using UnityEngine;

// Token: 0x0200038A RID: 906
public class UiVectorInspector : MonoBehaviour
{
	// Token: 0x060014F1 RID: 5361 RVA: 0x00065FDD File Offset: 0x000641DD
	public void SetName(string name)
	{
		this.m_nameLabel.text = name;
	}

	// Token: 0x060014F2 RID: 5362 RVA: 0x00065FEB File Offset: 0x000641EB
	public void SetValue(bool value)
	{
		this.m_valueLabel.text = string.Format("[{0}]", value);
	}

	// Token: 0x0400174C RID: 5964
	[Header("Components")]
	[SerializeField]
	private TextMeshProUGUI m_nameLabel;

	// Token: 0x0400174D RID: 5965
	[SerializeField]
	private TextMeshProUGUI m_valueLabel;
}
