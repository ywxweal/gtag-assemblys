using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x020003CA RID: 970
public class BitmapFont : ScriptableObject
{
	// Token: 0x060016A6 RID: 5798 RVA: 0x0006CFD0 File Offset: 0x0006B1D0
	private void OnEnable()
	{
		this._charToSymbol = this.symbols.ToDictionary((BitmapFont.SymbolData s) => s.character, (BitmapFont.SymbolData s) => s);
	}

	// Token: 0x060016A7 RID: 5799 RVA: 0x0006D02C File Offset: 0x0006B22C
	public void RenderToTexture(Texture2D target, string text)
	{
		if (text == null)
		{
			text = string.Empty;
		}
		int num = target.width * target.height;
		if (this._empty.Length != num)
		{
			this._empty = new Color[num];
			for (int i = 0; i < this._empty.Length; i++)
			{
				this._empty[i] = Color.black;
			}
		}
		target.SetPixels(this._empty);
		int length = text.Length;
		int num2 = 1;
		int width = this.fontImage.width;
		int height = this.fontImage.height;
		for (int j = 0; j < length; j++)
		{
			char c = text[j];
			BitmapFont.SymbolData symbolData = this._charToSymbol[c];
			int width2 = symbolData.width;
			int height2 = symbolData.height;
			int x = symbolData.x;
			int y = symbolData.y;
			Graphics.CopyTexture(this.fontImage, 0, 0, x, height - (y + height2), width2, height2, target, 0, 0, num2, 2 + symbolData.yoffset);
			num2 += width2 + 1;
		}
		target.Apply(false);
	}

	// Token: 0x040018FD RID: 6397
	public Texture2D fontImage;

	// Token: 0x040018FE RID: 6398
	public TextAsset fontJson;

	// Token: 0x040018FF RID: 6399
	public int symbolPixelsPerUnit = 1;

	// Token: 0x04001900 RID: 6400
	public string characterMap;

	// Token: 0x04001901 RID: 6401
	[Space]
	public BitmapFont.SymbolData[] symbols = new BitmapFont.SymbolData[0];

	// Token: 0x04001902 RID: 6402
	private Dictionary<char, BitmapFont.SymbolData> _charToSymbol;

	// Token: 0x04001903 RID: 6403
	private Color[] _empty = new Color[0];

	// Token: 0x020003CB RID: 971
	[Serializable]
	public struct SymbolData
	{
		// Token: 0x04001904 RID: 6404
		public char character;

		// Token: 0x04001905 RID: 6405
		[Space]
		public int id;

		// Token: 0x04001906 RID: 6406
		public int width;

		// Token: 0x04001907 RID: 6407
		public int height;

		// Token: 0x04001908 RID: 6408
		public int x;

		// Token: 0x04001909 RID: 6409
		public int y;

		// Token: 0x0400190A RID: 6410
		public int xadvance;

		// Token: 0x0400190B RID: 6411
		public int yoffset;
	}
}
