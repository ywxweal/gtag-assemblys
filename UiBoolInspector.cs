using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000387 RID: 903
public class UiBoolInspector : MonoBehaviour
{
	// Token: 0x060014DD RID: 5341 RVA: 0x00065B97 File Offset: 0x00063D97
	public void SetName(string name)
	{
		this.m_nameLabel.text = name;
	}

	// Token: 0x060014DE RID: 5342 RVA: 0x00065BA5 File Offset: 0x00063DA5
	public void SetValue(bool value)
	{
		this.m_toggle.isOn = value;
	}

	// Token: 0x04001734 RID: 5940
	[Header("Components")]
	[SerializeField]
	private TextMeshProUGUI m_nameLabel;

	// Token: 0x04001735 RID: 5941
	[SerializeField]
	private Toggle m_toggle;
}
