using System;
using UnityEngine;

// Token: 0x0200077D RID: 1917
[Serializable]
public struct StringEnum<TEnum> where TEnum : struct, Enum
{
	// Token: 0x170004C5 RID: 1221
	// (get) Token: 0x06002FF5 RID: 12277 RVA: 0x000EDCE0 File Offset: 0x000EBEE0
	public TEnum Value
	{
		get
		{
			return this.m_EnumValue;
		}
	}

	// Token: 0x06002FF6 RID: 12278 RVA: 0x000EDCE8 File Offset: 0x000EBEE8
	public static implicit operator StringEnum<TEnum>(TEnum e)
	{
		return new StringEnum<TEnum>
		{
			m_EnumValue = e
		};
	}

	// Token: 0x06002FF7 RID: 12279 RVA: 0x000EDCE0 File Offset: 0x000EBEE0
	public static implicit operator TEnum(StringEnum<TEnum> se)
	{
		return se.m_EnumValue;
	}

	// Token: 0x06002FF8 RID: 12280 RVA: 0x000EDD06 File Offset: 0x000EBF06
	public static bool operator ==(StringEnum<TEnum> left, StringEnum<TEnum> right)
	{
		return left.m_EnumValue.Equals(right.m_EnumValue);
	}

	// Token: 0x06002FF9 RID: 12281 RVA: 0x000EDD25 File Offset: 0x000EBF25
	public static bool operator !=(StringEnum<TEnum> left, StringEnum<TEnum> right)
	{
		return !(left == right);
	}

	// Token: 0x06002FFA RID: 12282 RVA: 0x000EDD34 File Offset: 0x000EBF34
	public override bool Equals(object obj)
	{
		if (obj is StringEnum<TEnum>)
		{
			StringEnum<TEnum> stringEnum = (StringEnum<TEnum>)obj;
			return this.m_EnumValue.Equals(stringEnum.m_EnumValue);
		}
		return false;
	}

	// Token: 0x06002FFB RID: 12283 RVA: 0x000EDD6E File Offset: 0x000EBF6E
	public override int GetHashCode()
	{
		return this.m_EnumValue.GetHashCode();
	}

	// Token: 0x04003638 RID: 13880
	[SerializeField]
	private TEnum m_EnumValue;
}
