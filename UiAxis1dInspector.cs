using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000385 RID: 901
public class UiAxis1dInspector : MonoBehaviour
{
	// Token: 0x060014D5 RID: 5333 RVA: 0x00065910 File Offset: 0x00063B10
	public void SetExtents(float minExtent, float maxExtent)
	{
		this.m_minExtent = minExtent;
		this.m_maxExtent = maxExtent;
	}

	// Token: 0x060014D6 RID: 5334 RVA: 0x00065920 File Offset: 0x00063B20
	public void SetName(string name)
	{
		this.m_nameLabel.text = name;
	}

	// Token: 0x060014D7 RID: 5335 RVA: 0x00065930 File Offset: 0x00063B30
	public void SetValue(float value)
	{
		this.m_valueLabel.text = string.Format("[{0}]", value.ToString("f2"));
		this.m_slider.minValue = Mathf.Min(value, this.m_minExtent);
		this.m_slider.maxValue = Mathf.Max(value, this.m_maxExtent);
		this.m_slider.value = value;
	}

	// Token: 0x0400172A RID: 5930
	[Header("Settings")]
	[SerializeField]
	private float m_minExtent;

	// Token: 0x0400172B RID: 5931
	[SerializeField]
	private float m_maxExtent = 1f;

	// Token: 0x0400172C RID: 5932
	[Header("Components")]
	[SerializeField]
	private TextMeshProUGUI m_nameLabel;

	// Token: 0x0400172D RID: 5933
	[SerializeField]
	private TextMeshProUGUI m_valueLabel;

	// Token: 0x0400172E RID: 5934
	[SerializeField]
	private Slider m_slider;
}
