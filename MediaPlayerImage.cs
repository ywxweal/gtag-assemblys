using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000315 RID: 789
public class MediaPlayerImage : Image
{
	// Token: 0x17000223 RID: 547
	// (get) Token: 0x060012CA RID: 4810 RVA: 0x0005853E File Offset: 0x0005673E
	// (set) Token: 0x060012CB RID: 4811 RVA: 0x00058546 File Offset: 0x00056746
	public MediaPlayerImage.ButtonType buttonType
	{
		get
		{
			return this.m_ButtonType;
		}
		set
		{
			if (this.m_ButtonType != value)
			{
				this.m_ButtonType = value;
				this.SetAllDirty();
			}
		}
	}

	// Token: 0x060012CC RID: 4812 RVA: 0x00058560 File Offset: 0x00056760
	protected override void OnPopulateMesh(VertexHelper toFill)
	{
		Rect pixelAdjustedRect = base.GetPixelAdjustedRect();
		Vector4 vector = new Vector4(pixelAdjustedRect.x, pixelAdjustedRect.y, pixelAdjustedRect.x + pixelAdjustedRect.width, pixelAdjustedRect.y + pixelAdjustedRect.height);
		Color32 color = this.color;
		toFill.Clear();
		switch (this.m_ButtonType)
		{
		case MediaPlayerImage.ButtonType.Play:
			toFill.AddVert(new Vector3(vector.x, vector.y), color, new Vector2(0f, 0f));
			toFill.AddVert(new Vector3(vector.x, vector.w), color, new Vector2(0f, 1f));
			toFill.AddVert(new Vector3(vector.z, Mathf.Lerp(vector.y, vector.w, 0.5f)), color, new Vector2(1f, 0.5f));
			toFill.AddTriangle(0, 1, 2);
			return;
		case MediaPlayerImage.ButtonType.Pause:
			toFill.AddVert(new Vector3(vector.x, vector.y), color, new Vector2(0f, 0f));
			toFill.AddVert(new Vector3(vector.x, vector.w), color, new Vector2(0f, 1f));
			toFill.AddVert(new Vector3(Mathf.Lerp(vector.x, vector.z, 0.35f), vector.w), color, new Vector2(0.35f, 1f));
			toFill.AddVert(new Vector3(Mathf.Lerp(vector.x, vector.z, 0.35f), vector.y), color, new Vector2(0.35f, 0f));
			toFill.AddVert(new Vector3(Mathf.Lerp(vector.x, vector.z, 0.65f), vector.y), color, new Vector2(0.65f, 0f));
			toFill.AddVert(new Vector3(Mathf.Lerp(vector.x, vector.z, 0.65f), vector.w), color, new Vector2(0.65f, 1f));
			toFill.AddVert(new Vector3(vector.z, vector.w), color, new Vector2(1f, 1f));
			toFill.AddVert(new Vector3(vector.z, vector.y), color, new Vector2(1f, 0f));
			toFill.AddTriangle(0, 1, 2);
			toFill.AddTriangle(2, 3, 0);
			toFill.AddTriangle(4, 5, 6);
			toFill.AddTriangle(6, 7, 4);
			return;
		case MediaPlayerImage.ButtonType.FastForward:
			toFill.AddVert(new Vector3(vector.x, vector.y), color, new Vector2(0f, 0f));
			toFill.AddVert(new Vector3(vector.x, vector.w), color, new Vector2(0f, 1f));
			toFill.AddVert(new Vector3(Mathf.Lerp(vector.x, vector.z, 0.5f), Mathf.Lerp(vector.y, vector.w, 0.5f)), color, new Vector2(0.5f, 0.5f));
			toFill.AddVert(new Vector3(Mathf.Lerp(vector.x, vector.z, 0.5f), vector.y), color, new Vector2(0.5f, 0f));
			toFill.AddVert(new Vector3(Mathf.Lerp(vector.x, vector.z, 0.5f), vector.w), color, new Vector2(0.5f, 1f));
			toFill.AddVert(new Vector3(vector.z, Mathf.Lerp(vector.y, vector.w, 0.5f)), color, new Vector2(1f, 0.5f));
			toFill.AddTriangle(0, 1, 2);
			toFill.AddTriangle(3, 4, 5);
			return;
		case MediaPlayerImage.ButtonType.Rewind:
			toFill.AddVert(new Vector3(vector.x, Mathf.Lerp(vector.y, vector.w, 0.5f)), color, new Vector2(0f, 0.5f));
			toFill.AddVert(new Vector3(Mathf.Lerp(vector.x, vector.z, 0.5f), vector.w), color, new Vector2(0.5f, 1f));
			toFill.AddVert(new Vector3(Mathf.Lerp(vector.x, vector.z, 0.5f), vector.y), color, new Vector2(0.5f, 0f));
			toFill.AddVert(new Vector3(Mathf.Lerp(vector.x, vector.z, 0.5f), Mathf.Lerp(vector.y, vector.w, 0.5f)), color, new Vector2(0.5f, 0.5f));
			toFill.AddVert(new Vector3(vector.z, vector.w), color, new Vector2(1f, 1f));
			toFill.AddVert(new Vector3(vector.z, vector.y), color, new Vector2(1f, 0f));
			toFill.AddTriangle(0, 1, 2);
			toFill.AddTriangle(3, 4, 5);
			return;
		case MediaPlayerImage.ButtonType.SkipForward:
			toFill.AddVert(new Vector3(vector.x, vector.y), color, new Vector2(0f, 0f));
			toFill.AddVert(new Vector3(vector.x, vector.w), color, new Vector2(0f, 1f));
			toFill.AddVert(new Vector3(Mathf.Lerp(vector.x, vector.z, 0.4375f), Mathf.Lerp(vector.y, vector.w, 0.5f)), color, new Vector2(0.4375f, 0.5f));
			toFill.AddVert(new Vector3(Mathf.Lerp(vector.x, vector.z, 0.4375f), vector.y), color, new Vector2(0.4375f, 0f));
			toFill.AddVert(new Vector3(Mathf.Lerp(vector.x, vector.z, 0.4375f), vector.w), color, new Vector2(0.4375f, 1f));
			toFill.AddVert(new Vector3(Mathf.Lerp(vector.x, vector.z, 0.875f), Mathf.Lerp(vector.y, vector.w, 0.5f)), color, new Vector2(0.875f, 0.5f));
			toFill.AddVert(new Vector3(Mathf.Lerp(vector.x, vector.z, 0.875f), vector.y), color, new Vector2(0.875f, 0f));
			toFill.AddVert(new Vector3(Mathf.Lerp(vector.x, vector.z, 0.875f), vector.w), color, new Vector2(0.875f, 1f));
			toFill.AddVert(new Vector3(vector.z, vector.w), color, new Vector2(1f, 1f));
			toFill.AddVert(new Vector3(vector.z, vector.y), color, new Vector2(1f, 0f));
			toFill.AddTriangle(0, 1, 2);
			toFill.AddTriangle(3, 4, 5);
			toFill.AddTriangle(6, 7, 8);
			toFill.AddTriangle(8, 9, 6);
			return;
		case MediaPlayerImage.ButtonType.SkipBack:
			toFill.AddVert(new Vector3(vector.x, vector.y), color, new Vector2(0f, 0f));
			toFill.AddVert(new Vector3(vector.x, vector.w), color, new Vector2(0f, 1f));
			toFill.AddVert(new Vector3(Mathf.Lerp(vector.x, vector.z, 0.125f), vector.w), color, new Vector2(0.125f, 1f));
			toFill.AddVert(new Vector3(Mathf.Lerp(vector.x, vector.z, 0.125f), vector.y), color, new Vector2(0.125f, 0f));
			toFill.AddVert(new Vector3(Mathf.Lerp(vector.x, vector.z, 0.125f), Mathf.Lerp(vector.y, vector.w, 0.5f)), color, new Vector2(0.125f, 0.5f));
			toFill.AddVert(new Vector3(Mathf.Lerp(vector.x, vector.z, 0.5625f), vector.w), color, new Vector2(0.5625f, 1f));
			toFill.AddVert(new Vector3(Mathf.Lerp(vector.x, vector.z, 0.5625f), vector.y), color, new Vector2(0.5625f, 0f));
			toFill.AddVert(new Vector3(Mathf.Lerp(vector.x, vector.z, 0.5625f), Mathf.Lerp(vector.y, vector.w, 0.5f)), color, new Vector2(0.5625f, 0.5f));
			toFill.AddVert(new Vector3(vector.z, vector.w), color, new Vector2(1f, 1f));
			toFill.AddVert(new Vector3(vector.z, vector.y), color, new Vector2(1f, 0f));
			toFill.AddTriangle(0, 1, 2);
			toFill.AddTriangle(2, 3, 0);
			toFill.AddTriangle(4, 5, 6);
			toFill.AddTriangle(7, 8, 9);
			return;
		}
		toFill.AddVert(new Vector3(vector.x, vector.y), color, new Vector2(0f, 0f));
		toFill.AddVert(new Vector3(vector.x, vector.w), color, new Vector2(0f, 1f));
		toFill.AddVert(new Vector3(vector.z, vector.w), color, new Vector2(1f, 1f));
		toFill.AddVert(new Vector3(vector.z, vector.y), color, new Vector2(1f, 0f));
		toFill.AddTriangle(0, 1, 2);
		toFill.AddTriangle(2, 3, 0);
	}

	// Token: 0x040014D9 RID: 5337
	[SerializeField]
	private MediaPlayerImage.ButtonType m_ButtonType;

	// Token: 0x02000316 RID: 790
	public enum ButtonType
	{
		// Token: 0x040014DB RID: 5339
		Play,
		// Token: 0x040014DC RID: 5340
		Pause,
		// Token: 0x040014DD RID: 5341
		FastForward,
		// Token: 0x040014DE RID: 5342
		Rewind,
		// Token: 0x040014DF RID: 5343
		SkipForward,
		// Token: 0x040014E0 RID: 5344
		SkipBack,
		// Token: 0x040014E1 RID: 5345
		Stop
	}
}
