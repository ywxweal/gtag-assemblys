using System;

// Token: 0x02000757 RID: 1879
public static class EnumUtilExt
{
	// Token: 0x06002EEE RID: 12014 RVA: 0x000EAEAB File Offset: 0x000E90AB
	public static string GetName<TEnum>(this TEnum e) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.EnumToName[e];
	}

	// Token: 0x06002EEF RID: 12015 RVA: 0x000EAECF File Offset: 0x000E90CF
	public static int GetIndex<TEnum>(this TEnum e) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.EnumToIndex[e];
	}

	// Token: 0x06002EF0 RID: 12016 RVA: 0x000EAEF3 File Offset: 0x000E90F3
	public static long GetLongValue<TEnum>(this TEnum e) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.EnumToLong[e];
	}

	// Token: 0x06002EF1 RID: 12017 RVA: 0x000EAFF4 File Offset: 0x000E91F4
	public static TEnum GetNextValue<TEnum>(this TEnum e) where TEnum : struct, Enum
	{
		EnumData<TEnum> shared = EnumData<TEnum>.Shared;
		return shared.Values[shared.EnumToIndex[e] + 1 % shared.Values.Length];
	}
}
