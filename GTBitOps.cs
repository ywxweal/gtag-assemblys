using System;
using System.Runtime.CompilerServices;

// Token: 0x020001D3 RID: 467
public static class GTBitOps
{
	// Token: 0x06000AE0 RID: 2784 RVA: 0x0003AA26 File Offset: 0x00038C26
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetValueMask(int count)
	{
		return (1 << count) - 1;
	}

	// Token: 0x06000AE1 RID: 2785 RVA: 0x0003AA30 File Offset: 0x00038C30
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetClearMask(int index, int valueMask)
	{
		return ~(valueMask << index);
	}

	// Token: 0x06000AE2 RID: 2786 RVA: 0x0003AA39 File Offset: 0x00038C39
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetClearMaskByCount(int index, int count)
	{
		return ~((1 << count) - 1 << index);
	}

	// Token: 0x06000AE3 RID: 2787 RVA: 0x0003AA49 File Offset: 0x00038C49
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int ReadBits(int bits, int index, int valueMask)
	{
		return (bits >> index) & valueMask;
	}

	// Token: 0x06000AE4 RID: 2788 RVA: 0x0003AA53 File Offset: 0x00038C53
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int ReadBits(int bits, GTBitOps.BitWriteInfo info)
	{
		return (bits >> info.index) & info.valueMask;
	}

	// Token: 0x06000AE5 RID: 2789 RVA: 0x0003AA67 File Offset: 0x00038C67
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int ReadBitsByCount(int bits, int index, int count)
	{
		return (bits >> index) & ((1 << count) - 1);
	}

	// Token: 0x06000AE6 RID: 2790 RVA: 0x0003AA78 File Offset: 0x00038C78
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool ReadBit(int bits, int index)
	{
		return ((bits >> index) & 1) == 1;
	}

	// Token: 0x06000AE7 RID: 2791 RVA: 0x0003AA85 File Offset: 0x00038C85
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteBits(ref int bits, GTBitOps.BitWriteInfo info, int value)
	{
		bits = (bits & info.clearMask) | ((value & info.valueMask) << info.index);
	}

	// Token: 0x06000AE8 RID: 2792 RVA: 0x0003AAA5 File Offset: 0x00038CA5
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int WriteBits(int bits, GTBitOps.BitWriteInfo info, int value)
	{
		GTBitOps.WriteBits(ref bits, info, value);
		return bits;
	}

	// Token: 0x06000AE9 RID: 2793 RVA: 0x0003AAB1 File Offset: 0x00038CB1
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteBits(ref int bits, int index, int valueMask, int clearMask, int value)
	{
		bits = (bits & clearMask) | ((value & valueMask) << index);
	}

	// Token: 0x06000AEA RID: 2794 RVA: 0x0003AAC3 File Offset: 0x00038CC3
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int WriteBits(int bits, int index, int valueMask, int clearMask, int value)
	{
		GTBitOps.WriteBits(ref bits, index, valueMask, clearMask, value);
		return bits;
	}

	// Token: 0x06000AEB RID: 2795 RVA: 0x0003AAD2 File Offset: 0x00038CD2
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteBitsByCount(ref int bits, int index, int count, int value)
	{
		bits = (bits & ~((1 << count) - 1 << index)) | ((value & ((1 << count) - 1)) << index);
	}

	// Token: 0x06000AEC RID: 2796 RVA: 0x0003AAF7 File Offset: 0x00038CF7
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int WriteBitsByCount(int bits, int index, int count, int value)
	{
		GTBitOps.WriteBitsByCount(ref bits, index, count, value);
		return bits;
	}

	// Token: 0x06000AED RID: 2797 RVA: 0x0003AB04 File Offset: 0x00038D04
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteBit(ref int bits, int index, bool value)
	{
		bits = (bits & ~(1 << index)) | ((value ? 1 : 0) << index);
	}

	// Token: 0x06000AEE RID: 2798 RVA: 0x0003AB1F File Offset: 0x00038D1F
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int WriteBit(int bits, int index, bool value)
	{
		GTBitOps.WriteBit(ref bits, index, value);
		return bits;
	}

	// Token: 0x06000AEF RID: 2799 RVA: 0x0003AB2B File Offset: 0x00038D2B
	public static string ToBinaryString(int number)
	{
		return Convert.ToString(number, 2).PadLeft(32, '0');
	}

	// Token: 0x020001D4 RID: 468
	public readonly struct BitWriteInfo
	{
		// Token: 0x06000AF0 RID: 2800 RVA: 0x0003AB3D File Offset: 0x00038D3D
		public BitWriteInfo(int index, int count)
		{
			this.index = index;
			this.valueMask = GTBitOps.GetValueMask(count);
			this.clearMask = GTBitOps.GetClearMask(index, this.valueMask);
		}

		// Token: 0x04000D69 RID: 3433
		public readonly int index;

		// Token: 0x04000D6A RID: 3434
		public readonly int valueMask;

		// Token: 0x04000D6B RID: 3435
		public readonly int clearMask;
	}
}
