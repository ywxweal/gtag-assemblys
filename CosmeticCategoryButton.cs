using System;
using UnityEngine;

// Token: 0x0200044C RID: 1100
public class CosmeticCategoryButton : CosmeticButton
{
	// Token: 0x06001B28 RID: 6952 RVA: 0x0008556E File Offset: 0x0008376E
	public void SetIcon(Sprite sprite)
	{
		this.equippedLeftIcon.enabled = false;
		this.equippedRightIcon.enabled = false;
		this.equippedIcon.enabled = sprite != null;
		this.equippedIcon.sprite = sprite;
	}

	// Token: 0x06001B29 RID: 6953 RVA: 0x000855A8 File Offset: 0x000837A8
	public void SetDualIcon(Sprite leftSprite, Sprite rightSprite)
	{
		this.equippedLeftIcon.enabled = leftSprite != null;
		this.equippedRightIcon.enabled = rightSprite != null;
		this.equippedIcon.enabled = false;
		this.equippedLeftIcon.sprite = leftSprite;
		this.equippedRightIcon.sprite = rightSprite;
	}

	// Token: 0x06001B2A RID: 6954 RVA: 0x00085600 File Offset: 0x00083800
	public override void UpdatePosition()
	{
		base.UpdatePosition();
		if (this.equippedIcon != null)
		{
			this.equippedIcon.transform.position += this.posOffset;
		}
		if (this.equippedLeftIcon != null)
		{
			this.equippedLeftIcon.transform.position += this.posOffset;
		}
		if (this.equippedRightIcon != null)
		{
			this.equippedRightIcon.transform.position += this.posOffset;
		}
	}

	// Token: 0x04001E1C RID: 7708
	[SerializeField]
	private SpriteRenderer equippedIcon;

	// Token: 0x04001E1D RID: 7709
	[SerializeField]
	private SpriteRenderer equippedLeftIcon;

	// Token: 0x04001E1E RID: 7710
	[SerializeField]
	private SpriteRenderer equippedRightIcon;
}
