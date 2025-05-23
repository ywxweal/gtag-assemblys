using System;
using UnityEngine;

// Token: 0x0200096A RID: 2410
[Serializable]
public struct AnimHashId
{
	// Token: 0x170005C1 RID: 1473
	// (get) Token: 0x06003A1D RID: 14877 RVA: 0x00116E01 File Offset: 0x00115001
	public string text
	{
		get
		{
			return this._text;
		}
	}

	// Token: 0x170005C2 RID: 1474
	// (get) Token: 0x06003A1E RID: 14878 RVA: 0x00116E09 File Offset: 0x00115009
	public int hash
	{
		get
		{
			return this._hash;
		}
	}

	// Token: 0x06003A1F RID: 14879 RVA: 0x00116E11 File Offset: 0x00115011
	public AnimHashId(string text)
	{
		this._text = text;
		this._hash = Animator.StringToHash(text);
	}

	// Token: 0x06003A20 RID: 14880 RVA: 0x00116E01 File Offset: 0x00115001
	public override string ToString()
	{
		return this._text;
	}

	// Token: 0x06003A21 RID: 14881 RVA: 0x00116E09 File Offset: 0x00115009
	public override int GetHashCode()
	{
		return this._hash;
	}

	// Token: 0x06003A22 RID: 14882 RVA: 0x00116E09 File Offset: 0x00115009
	public static implicit operator int(AnimHashId h)
	{
		return h._hash;
	}

	// Token: 0x06003A23 RID: 14883 RVA: 0x00116E26 File Offset: 0x00115026
	public static implicit operator AnimHashId(string s)
	{
		return new AnimHashId(s);
	}

	// Token: 0x04003F35 RID: 16181
	[SerializeField]
	private string _text;

	// Token: 0x04003F36 RID: 16182
	[NonSerialized]
	private int _hash;
}
