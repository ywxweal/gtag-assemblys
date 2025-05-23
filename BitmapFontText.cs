using System;
using UnityEngine;

// Token: 0x020003CD RID: 973
public class BitmapFontText : MonoBehaviour
{
	// Token: 0x060016AD RID: 5805 RVA: 0x0006D19E File Offset: 0x0006B39E
	private void Awake()
	{
		this.Init();
		this.Render();
	}

	// Token: 0x060016AE RID: 5806 RVA: 0x0006D1AC File Offset: 0x0006B3AC
	public void Render()
	{
		this.font.RenderToTexture(this.texture, this.uppercaseOnly ? this.text.ToUpperInvariant() : this.text);
	}

	// Token: 0x060016AF RID: 5807 RVA: 0x0006D1DC File Offset: 0x0006B3DC
	public void Init()
	{
		this.texture = new Texture2D(this.textArea.x, this.textArea.y, this.font.fontImage.format, false);
		this.texture.filterMode = FilterMode.Point;
		this.material = new Material(this.renderer.sharedMaterial);
		this.material.mainTexture = this.texture;
		this.renderer.sharedMaterial = this.material;
	}

	// Token: 0x0400190F RID: 6415
	public string text;

	// Token: 0x04001910 RID: 6416
	public bool uppercaseOnly;

	// Token: 0x04001911 RID: 6417
	public Vector2Int textArea;

	// Token: 0x04001912 RID: 6418
	[Space]
	public Renderer renderer;

	// Token: 0x04001913 RID: 6419
	public Texture2D texture;

	// Token: 0x04001914 RID: 6420
	public Material material;

	// Token: 0x04001915 RID: 6421
	public BitmapFont font;
}
