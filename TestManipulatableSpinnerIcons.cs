using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200040F RID: 1039
public class TestManipulatableSpinnerIcons : MonoBehaviour
{
	// Token: 0x0600194A RID: 6474 RVA: 0x0007A6B6 File Offset: 0x000788B6
	private void Awake()
	{
		this.GenerateRollers();
	}

	// Token: 0x0600194B RID: 6475 RVA: 0x0007A6BE File Offset: 0x000788BE
	private void LateUpdate()
	{
		this.currentRotation = this.spinner.angle * this.rotationScale;
		this.UpdateSelectedIndex();
		this.UpdateRollers();
	}

	// Token: 0x0600194C RID: 6476 RVA: 0x0007A6E4 File Offset: 0x000788E4
	private void GenerateRollers()
	{
		for (int i = 0; i < this.rollerElementCount; i++)
		{
			float num = this.rollerElementAngle * (float)i + this.rollerElementAngle * 0.5f;
			Object.Instantiate<GameObject>(this.rollerElementTemplate, base.transform).transform.localRotation = Quaternion.Euler(num, 0f, 0f);
			GameObject gameObject = Object.Instantiate<GameObject>(this.iconElementTemplate, this.iconCanvas.transform);
			gameObject.transform.localRotation = Quaternion.Euler(num, 0f, 0f);
			this.visibleIcons.Add(gameObject.GetComponentInChildren<Text>());
		}
		this.rollerElementTemplate.SetActive(false);
		this.iconElementTemplate.SetActive(false);
		this.UpdateRollers();
	}

	// Token: 0x0600194D RID: 6477 RVA: 0x0007A7AC File Offset: 0x000789AC
	private void UpdateSelectedIndex()
	{
		float num = this.currentRotation / this.rollerElementAngle;
		if (this.rollerElementCount % 2 == 1)
		{
			num += 0.5f;
		}
		this.selectedIndex = Mathf.FloorToInt(num);
		this.selectedIndex %= this.scrollableCount;
		if (this.selectedIndex < 0)
		{
			this.selectedIndex = this.scrollableCount + this.selectedIndex;
		}
	}

	// Token: 0x0600194E RID: 6478 RVA: 0x0007A818 File Offset: 0x00078A18
	private void UpdateRollers()
	{
		float num = this.currentRotation;
		if (Mathf.Abs(num) > this.rollerElementAngle / 2f)
		{
			if (num > 0f)
			{
				num += this.rollerElementAngle / 2f;
				num %= this.rollerElementAngle;
				num -= this.rollerElementAngle / 2f;
			}
			else
			{
				num -= this.rollerElementAngle / 2f;
				num %= this.rollerElementAngle;
				num += this.rollerElementAngle / 2f;
			}
		}
		num -= (float)this.rollerElementCount / 2f * this.rollerElementAngle;
		base.transform.localRotation = Quaternion.Euler(num, 0f, 0f);
		this.iconCanvas.transform.localRotation = Quaternion.Euler(num, 0f, 0f);
		int num2 = this.rollerElementCount / 2;
		for (int i = 0; i < this.visibleIcons.Count; i++)
		{
			int num3 = this.selectedIndex - i + num2;
			if (num3 < 0)
			{
				num3 += this.scrollableCount;
			}
			else
			{
				num3 %= this.scrollableCount;
			}
			this.visibleIcons[i].text = string.Format("{0}", num3 + 1);
		}
	}

	// Token: 0x04001C14 RID: 7188
	public ManipulatableSpinner spinner;

	// Token: 0x04001C15 RID: 7189
	public float rotationScale = 1f;

	// Token: 0x04001C16 RID: 7190
	public int rollerElementCount = 5;

	// Token: 0x04001C17 RID: 7191
	public GameObject rollerElementTemplate;

	// Token: 0x04001C18 RID: 7192
	public GameObject iconCanvas;

	// Token: 0x04001C19 RID: 7193
	public GameObject iconElementTemplate;

	// Token: 0x04001C1A RID: 7194
	public float iconOffset = 1f;

	// Token: 0x04001C1B RID: 7195
	public float rollerElementAngle = 15f;

	// Token: 0x04001C1C RID: 7196
	private List<Text> visibleIcons = new List<Text>();

	// Token: 0x04001C1D RID: 7197
	private float currentRotation;

	// Token: 0x04001C1E RID: 7198
	public int scrollableCount = 50;

	// Token: 0x04001C1F RID: 7199
	public int selectedIndex;
}
