using System;
using UnityEngine;

// Token: 0x0200044B RID: 1099
public class CosmeticButton : GorillaPressableButton
{
	// Token: 0x06001B24 RID: 6948 RVA: 0x0008535F File Offset: 0x0008355F
	public void Awake()
	{
		this.startingPos = base.transform.localPosition;
	}

	// Token: 0x06001B25 RID: 6949 RVA: 0x00085374 File Offset: 0x00083574
	public override void UpdateColor()
	{
		if (!base.enabled)
		{
			this.buttonRenderer.material = this.disabledMaterial;
			if (this.myText != null)
			{
				this.myText.text = this.offText;
			}
		}
		else if (this.isOn)
		{
			this.buttonRenderer.material = this.pressedMaterial;
			if (this.myText != null)
			{
				this.myText.text = this.onText;
			}
		}
		else
		{
			this.buttonRenderer.material = this.unpressedMaterial;
			if (this.myText != null)
			{
				this.myText.text = this.offText;
			}
		}
		this.UpdatePosition();
	}

	// Token: 0x06001B26 RID: 6950 RVA: 0x0008542C File Offset: 0x0008362C
	public virtual void UpdatePosition()
	{
		Vector3 vector = this.startingPos;
		if (!base.enabled)
		{
			vector += this.disabledOffset;
		}
		else if (this.isOn)
		{
			vector += this.pressedOffset;
		}
		this.posOffset = base.transform.position;
		base.transform.localPosition = vector;
		this.posOffset = base.transform.position - this.posOffset;
		if (this.myText != null)
		{
			this.myText.transform.position += this.posOffset;
		}
		if (this.myTmpText != null)
		{
			this.myTmpText.transform.position += this.posOffset;
		}
		if (this.myTmpText2 != null)
		{
			this.myTmpText2.transform.position += this.posOffset;
		}
	}

	// Token: 0x04001E17 RID: 7703
	[SerializeField]
	private Vector3 pressedOffset = new Vector3(0f, 0f, 0.1f);

	// Token: 0x04001E18 RID: 7704
	[SerializeField]
	private Material disabledMaterial;

	// Token: 0x04001E19 RID: 7705
	[SerializeField]
	private Vector3 disabledOffset = new Vector3(0f, 0f, 0.1f);

	// Token: 0x04001E1A RID: 7706
	private Vector3 startingPos;

	// Token: 0x04001E1B RID: 7707
	protected Vector3 posOffset;
}
