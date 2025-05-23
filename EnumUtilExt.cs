using System;

// Token: 0x02000757 RID: 1879
public static class EnumUtilExt
{
	// Token: 0x06002EED RID: 12013 RVA: 0x000EAE07 File Offset: 0x000E9007
	public static string GetName<TEnum>(this TEnum e) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.EnumToName[e];
	}

	// Token: 0x06002EEE RID: 12014 RVA: 0x000EAE2B File Offset: 0x000E902B
	public static int GetIndex<TEnum>(this TEnum e) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.EnumToIndex[e];
	}

	// Token: 0x06002EEF RID: 12015 RVA: 0x000EAE4F File Offset: 0x000E904F
	public static long GetLongValue<TEnum>(this TEnum e) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.EnumToLong[e];
	}

	// Token: 0x06002EF0 RID: 12016 RVA: 0x000EAF50 File Offset: 0x000E9150
	public static TEnum GetNextValue<TEnum>(this TEnum e) where TEnum : struct, Enum
	{
		EnumData<TEnum> shared = EnumData<TEnum>.Shared;
		return shared.Values[shared.EnumToIndex[e] + 1 % shared.Values.Length];
	}
}
