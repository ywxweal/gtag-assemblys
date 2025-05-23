using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020006C9 RID: 1737
[Serializable]
public class UberShaderProperty
{
	// Token: 0x06002B4C RID: 11084 RVA: 0x000D53CC File Offset: 0x000D35CC
	public T GetValue<T>(Material target)
	{
		switch (this.type)
		{
		case ShaderPropertyType.Color:
			return UberShaderProperty.ValueAs<Color, T>(target.GetColor(this.nameID));
		case ShaderPropertyType.Vector:
			return UberShaderProperty.ValueAs<Vector4, T>(target.GetVector(this.nameID));
		case ShaderPropertyType.Float:
		case ShaderPropertyType.Range:
			return UberShaderProperty.ValueAs<float, T>(target.GetFloat(this.nameID));
		case ShaderPropertyType.Texture:
			return UberShaderProperty.ValueAs<Texture, T>(target.GetTexture(this.nameID));
		case ShaderPropertyType.Int:
			return UberShaderProperty.ValueAs<int, T>(target.GetInt(this.nameID));
		default:
			return default(T);
		}
	}

	// Token: 0x06002B4D RID: 11085 RVA: 0x000D5464 File Offset: 0x000D3664
	public void SetValue<T>(Material target, T value)
	{
		switch (this.type)
		{
		case ShaderPropertyType.Color:
			target.SetColor(this.nameID, UberShaderProperty.ValueAs<T, Color>(value));
			break;
		case ShaderPropertyType.Vector:
			target.SetVector(this.nameID, UberShaderProperty.ValueAs<T, Vector4>(value));
			break;
		case ShaderPropertyType.Float:
		case ShaderPropertyType.Range:
			target.SetFloat(this.nameID, UberShaderProperty.ValueAs<T, float>(value));
			break;
		case ShaderPropertyType.Texture:
			target.SetTexture(this.nameID, UberShaderProperty.ValueAs<T, Texture>(value));
			break;
		case ShaderPropertyType.Int:
			target.SetInt(this.nameID, UberShaderProperty.ValueAs<T, int>(value));
			break;
		}
		if (!this.isKeywordToggle)
		{
			return;
		}
		bool flag = false;
		ShaderPropertyType shaderPropertyType = this.type;
		if (shaderPropertyType != ShaderPropertyType.Float)
		{
			if (shaderPropertyType == ShaderPropertyType.Int)
			{
				flag = UberShaderProperty.ValueAs<T, int>(value) >= 1;
			}
		}
		else
		{
			flag = UberShaderProperty.ValueAs<T, float>(value) >= 0.5f;
		}
		if (flag)
		{
			target.EnableKeyword(this.keyword);
			return;
		}
		target.DisableKeyword(this.keyword);
	}

	// Token: 0x06002B4E RID: 11086 RVA: 0x000D5550 File Offset: 0x000D3750
	public void Enable(Material target)
	{
		ShaderPropertyType shaderPropertyType = this.type;
		if (shaderPropertyType != ShaderPropertyType.Float)
		{
			if (shaderPropertyType == ShaderPropertyType.Int)
			{
				target.SetInt(this.nameID, 1);
			}
		}
		else
		{
			target.SetFloat(this.nameID, 1f);
		}
		if (this.isKeywordToggle)
		{
			target.EnableKeyword(this.keyword);
		}
	}

	// Token: 0x06002B4F RID: 11087 RVA: 0x000D55A0 File Offset: 0x000D37A0
	public void Disable(Material target)
	{
		ShaderPropertyType shaderPropertyType = this.type;
		if (shaderPropertyType != ShaderPropertyType.Float)
		{
			if (shaderPropertyType == ShaderPropertyType.Int)
			{
				target.SetInt(this.nameID, 0);
			}
		}
		else
		{
			target.SetFloat(this.nameID, 0f);
		}
		if (this.isKeywordToggle)
		{
			target.DisableKeyword(this.keyword);
		}
	}

	// Token: 0x06002B50 RID: 11088 RVA: 0x000D55F0 File Offset: 0x000D37F0
	public bool TryGetKeywordState(Material target, out bool enabled)
	{
		enabled = false;
		if (!this.isKeywordToggle)
		{
			return false;
		}
		enabled = target.IsKeywordEnabled(this.keyword);
		return true;
	}

	// Token: 0x06002B51 RID: 11089 RVA: 0x000D560E File Offset: 0x000D380E
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe static TOut ValueAs<TIn, TOut>(TIn value)
	{
		return *Unsafe.As<TIn, TOut>(ref value);
	}

	// Token: 0x040030F9 RID: 12537
	public int index;

	// Token: 0x040030FA RID: 12538
	public int nameID;

	// Token: 0x040030FB RID: 12539
	public string name;

	// Token: 0x040030FC RID: 12540
	public ShaderPropertyType type;

	// Token: 0x040030FD RID: 12541
	public ShaderPropertyFlags flags;

	// Token: 0x040030FE RID: 12542
	public Vector2 rangeLimits;

	// Token: 0x040030FF RID: 12543
	public string[] attributes;

	// Token: 0x04003100 RID: 12544
	public bool isKeywordToggle;

	// Token: 0x04003101 RID: 12545
	public string keyword;
}
