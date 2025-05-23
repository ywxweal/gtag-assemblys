using System;
using UnityEngine;

// Token: 0x0200077D RID: 1917
[Serializable]
public struct StringEnum<TEnum> where TEnum : struct, Enum
{
	// Token: 0x170004C5 RID: 1221
	// (get) Token: 0x06002FF4 RID: 12276 RVA: 0x000EDC3C File Offset: 0x000EBE3C
	public TEnum Value
	{
		get
		{
			return this.m_EnumValue;
		}
	}

	// Token: 0x06002FF5 RID: 12277 RVA: 0x000EDC44 File Offset: 0x000EBE44
	public static implicit operator StringEnum<TEnum>(TEnum e)
	{
		return new StringEnum<TEnum>
		{
			m_EnumValue = e
		};
	}

	// Token: 0x06002FF6 RID: 12278 RVA: 0x000EDC3C File Offset: 0x000EBE3C
	public static implicit operator TEnum(StringEnum<TEnum> se)
	{
		return se.m_EnumValue;
	}

	// Token: 0x06002FF7 RID: 12279 RVA: 0x000EDC62 File Offset: 0x000EBE62
	public static bool operator ==(StringEnum<TEnum> left, StringEnum<TEnum> right)
	{
		return left.m_EnumValue.Equals(right.m_EnumValue);
	}

	// Token: 0x06002FF8 RID: 12280 RVA: 0x000EDC81 File Offset: 0x000EBE81
	public static bool operator !=(StringEnum<TEnum> left, StringEnum<TEnum> right)
	{
		return !(left == right);
	}

	// Token: 0x06002FF9 RID: 12281 RVA: 0x000EDC90 File Offset: 0x000EBE90
	public override bool Equals(object obj)
	{
		if (obj is StringEnum<TEnum>)
		{
			StringEnum<TEnum> stringEnum = (StringEnum<TEnum>)obj;
			return this.m_EnumValue.Equals(stringEnum.m_EnumValue);
		}
		return false;
	}

	// Token: 0x06002FFA RID: 12282 RVA: 0x000EDCCA File Offset: 0x000EBECA
	public override int GetHashCode()
	{
		return this.m_EnumValue.GetHashCode();
	}

	// Token: 0x04003636 RID: 13878
	[SerializeField]
	private TEnum m_EnumValue;
}
