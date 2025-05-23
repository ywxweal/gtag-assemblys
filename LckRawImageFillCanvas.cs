using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x0200025E RID: 606
[ExecuteInEditMode]
public class LckRawImageFillCanvas : UIBehaviour
{
	// Token: 0x06000DDF RID: 3551 RVA: 0x00047663 File Offset: 0x00045863
	private new void OnEnable()
	{
		this.UpdateSizeDelta();
	}

	// Token: 0x06000DE0 RID: 3552 RVA: 0x00047663 File Offset: 0x00045863
	private void Update()
	{
		this.UpdateSizeDelta();
	}

	// Token: 0x06000DE1 RID: 3553 RVA: 0x0004766C File Offset: 0x0004586C
	private void UpdateSizeDelta()
	{
		if (this._rawImage == null || this._rawImage.texture == null)
		{
			return;
		}
		RectTransform rectTransform = this._rawImage.rectTransform;
		Vector2 sizeDelta = ((RectTransform)rectTransform.parent).sizeDelta;
		Vector2 vector = new Vector2((float)this._rawImage.texture.width, (float)this._rawImage.texture.height);
		float num = sizeDelta.x / sizeDelta.y;
		float num2 = vector.x / vector.y;
		float num3 = num / num2;
		Vector2 vector2 = new Vector2(sizeDelta.x, sizeDelta.x / num2);
		Vector2 vector3 = new Vector2(sizeDelta.y * num2, sizeDelta.y);
		switch (this._scaleType)
		{
		case LckRawImageFillCanvas.ScaleType.Fill:
			rectTransform.sizeDelta = ((num3 > 1f) ? vector2 : vector3);
			return;
		case LckRawImageFillCanvas.ScaleType.Inset:
			rectTransform.sizeDelta = ((num3 < 1f) ? vector2 : vector3);
			return;
		case LckRawImageFillCanvas.ScaleType.Stretch:
			rectTransform.sizeDelta = sizeDelta;
			return;
		default:
			return;
		}
	}

	// Token: 0x04001154 RID: 4436
	[SerializeField]
	private RawImage _rawImage;

	// Token: 0x04001155 RID: 4437
	[SerializeField]
	private LckRawImageFillCanvas.ScaleType _scaleType;

	// Token: 0x0200025F RID: 607
	private enum ScaleType
	{
		// Token: 0x04001157 RID: 4439
		Fill,
		// Token: 0x04001158 RID: 4440
		Inset,
		// Token: 0x04001159 RID: 4441
		Stretch
	}
}
