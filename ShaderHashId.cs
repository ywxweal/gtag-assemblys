using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020009B8 RID: 2488
[Serializable]
public struct ShaderHashId : IEquatable<ShaderHashId>
{
	// Token: 0x170005DB RID: 1499
	// (get) Token: 0x06003B71 RID: 15217 RVA: 0x0011B7D7 File Offset: 0x001199D7
	public string text
	{
		get
		{
			return this._text;
		}
	}

	// Token: 0x170005DC RID: 1500
	// (get) Token: 0x06003B72 RID: 15218 RVA: 0x0011B7DF File Offset: 0x001199DF
	public int hash
	{
		get
		{
			return this._hash;
		}
	}

	// Token: 0x06003B73 RID: 15219 RVA: 0x0011B7E7 File Offset: 0x001199E7
	public ShaderHashId(string text)
	{
		this._text = text;
		this._hash = Shader.PropertyToID(text);
	}

	// Token: 0x06003B74 RID: 15220 RVA: 0x0011B7D7 File Offset: 0x001199D7
	public override string ToString()
	{
		return this._text;
	}

	// Token: 0x06003B75 RID: 15221 RVA: 0x0011B7DF File Offset: 0x001199DF
	public override int GetHashCode()
	{
		return this._hash;
	}

	// Token: 0x06003B76 RID: 15222 RVA: 0x0011B7DF File Offset: 0x001199DF
	public static implicit operator int(ShaderHashId h)
	{
		return h._hash;
	}

	// Token: 0x06003B77 RID: 15223 RVA: 0x0011B7FC File Offset: 0x001199FC
	public static implicit operator ShaderHashId(string s)
	{
		return new ShaderHashId(s);
	}

	// Token: 0x06003B78 RID: 15224 RVA: 0x0011B804 File Offset: 0x00119A04
	public bool Equals(ShaderHashId other)
	{
		return this._hash == other._hash;
	}

	// Token: 0x06003B79 RID: 15225 RVA: 0x0011B814 File Offset: 0x00119A14
	public override bool Equals(object obj)
	{
		if (obj is ShaderHashId)
		{
			ShaderHashId shaderHashId = (ShaderHashId)obj;
			return this.Equals(shaderHashId);
		}
		return false;
	}

	// Token: 0x06003B7A RID: 15226 RVA: 0x0011B839 File Offset: 0x00119A39
	public static bool operator ==(ShaderHashId x, ShaderHashId y)
	{
		return x.Equals(y);
	}

	// Token: 0x06003B7B RID: 15227 RVA: 0x0011B843 File Offset: 0x00119A43
	public static bool operator !=(ShaderHashId x, ShaderHashId y)
	{
		return !x.Equals(y);
	}

	// Token: 0x04003FDE RID: 16350
	[FormerlySerializedAs("_hashText")]
	[SerializeField]
	private string _text;

	// Token: 0x04003FDF RID: 16351
	[NonSerialized]
	private int _hash;
}
