using System;

namespace emotitron.Compression
{
	// Token: 0x02000E04 RID: 3588
	public static class ArraySegmentExt
	{
		// Token: 0x060058E0 RID: 22752 RVA: 0x001B5073 File Offset: 0x001B3273
		public static ArraySegment<byte> ExtractArraySegment(byte[] buffer, ref int bitposition)
		{
			return new ArraySegment<byte>(buffer, 0, bitposition + 7 >> 3);
		}

		// Token: 0x060058E1 RID: 22753 RVA: 0x001B5082 File Offset: 0x001B3282
		public static ArraySegment<ushort> ExtractArraySegment(ushort[] buffer, ref int bitposition)
		{
			return new ArraySegment<ushort>(buffer, 0, bitposition + 15 >> 4);
		}

		// Token: 0x060058E2 RID: 22754 RVA: 0x001B5092 File Offset: 0x001B3292
		public static ArraySegment<uint> ExtractArraySegment(uint[] buffer, ref int bitposition)
		{
			return new ArraySegment<uint>(buffer, 0, bitposition + 31 >> 5);
		}

		// Token: 0x060058E3 RID: 22755 RVA: 0x001B50A2 File Offset: 0x001B32A2
		public static ArraySegment<ulong> ExtractArraySegment(ulong[] buffer, ref int bitposition)
		{
			return new ArraySegment<ulong>(buffer, 0, bitposition + 63 >> 6);
		}

		// Token: 0x060058E4 RID: 22756 RVA: 0x001B50B4 File Offset: 0x001B32B4
		public static void Append(this ArraySegment<byte> buffer, ulong value, ref int bitposition, int bits)
		{
			int num = buffer.Offset << 3;
			bitposition += num;
			buffer.Array.Append(value, ref bitposition, bits);
			bitposition -= num;
		}

		// Token: 0x060058E5 RID: 22757 RVA: 0x001B50E8 File Offset: 0x001B32E8
		public static void Append(this ArraySegment<uint> buffer, ulong value, ref int bitposition, int bits)
		{
			int num = buffer.Offset << 5;
			bitposition += num;
			buffer.Array.Append(value, ref bitposition, bits);
			bitposition -= num;
		}

		// Token: 0x060058E6 RID: 22758 RVA: 0x001B511C File Offset: 0x001B331C
		public static void Append(this ArraySegment<ulong> buffer, ulong value, ref int bitposition, int bits)
		{
			int num = buffer.Offset << 6;
			bitposition += num;
			buffer.Array.Append(value, ref bitposition, bits);
			bitposition -= num;
		}

		// Token: 0x060058E7 RID: 22759 RVA: 0x001B5150 File Offset: 0x001B3350
		public static void Write(this ArraySegment<byte> buffer, ulong value, ref int bitposition, int bits)
		{
			int num = buffer.Offset << 3;
			bitposition += num;
			buffer.Array.Write(value, ref bitposition, bits);
			bitposition -= num;
		}

		// Token: 0x060058E8 RID: 22760 RVA: 0x001B5184 File Offset: 0x001B3384
		public static void Write(this ArraySegment<uint> buffer, ulong value, ref int bitposition, int bits)
		{
			int num = buffer.Offset << 5;
			bitposition += num;
			buffer.Array.Write(value, ref bitposition, bits);
			bitposition -= num;
		}

		// Token: 0x060058E9 RID: 22761 RVA: 0x001B51B8 File Offset: 0x001B33B8
		public static void Write(this ArraySegment<ulong> buffer, ulong value, ref int bitposition, int bits)
		{
			int num = buffer.Offset << 6;
			bitposition += num;
			buffer.Array.Write(value, ref bitposition, bits);
			bitposition -= num;
		}

		// Token: 0x060058EA RID: 22762 RVA: 0x001B51EC File Offset: 0x001B33EC
		public static ulong Read(this ArraySegment<byte> buffer, ref int bitposition, int bits)
		{
			int num = buffer.Offset << 3;
			bitposition += num;
			ulong num2 = buffer.Array.Read(ref bitposition, bits);
			bitposition -= num;
			return num2;
		}

		// Token: 0x060058EB RID: 22763 RVA: 0x001B5220 File Offset: 0x001B3420
		public static ulong Read(this ArraySegment<uint> buffer, ref int bitposition, int bits)
		{
			int num = buffer.Offset << 5;
			bitposition += num;
			ulong num2 = buffer.Array.Read(ref bitposition, bits);
			bitposition -= num;
			return num2;
		}

		// Token: 0x060058EC RID: 22764 RVA: 0x001B5254 File Offset: 0x001B3454
		public static ulong Read(this ArraySegment<ulong> buffer, ref int bitposition, int bits)
		{
			int num = buffer.Offset << 6;
			bitposition += num;
			ulong num2 = buffer.Array.Read(ref bitposition, bits);
			bitposition -= num;
			return num2;
		}

		// Token: 0x060058ED RID: 22765 RVA: 0x001B5288 File Offset: 0x001B3488
		public static void ReadOutSafe(this ArraySegment<byte> source, int srcStartPos, byte[] target, ref int bitposition, int bits)
		{
			int num = source.Offset << 3;
			srcStartPos += num;
			source.Array.ReadOutSafe(srcStartPos, target, ref bitposition, bits);
		}

		// Token: 0x060058EE RID: 22766 RVA: 0x001B52B8 File Offset: 0x001B34B8
		public static void ReadOutSafe(this ArraySegment<byte> source, int srcStartPos, ulong[] target, ref int bitposition, int bits)
		{
			int num = source.Offset << 3;
			srcStartPos += num;
			source.Array.ReadOutSafe(srcStartPos, target, ref bitposition, bits);
		}

		// Token: 0x060058EF RID: 22767 RVA: 0x001B52E8 File Offset: 0x001B34E8
		public static void ReadOutSafe(this ArraySegment<ulong> source, int srcStartPos, byte[] target, ref int bitposition, int bits)
		{
			int num = source.Offset << 6;
			srcStartPos += num;
			source.Array.ReadOutSafe(srcStartPos, target, ref bitposition, bits);
		}

		// Token: 0x060058F0 RID: 22768 RVA: 0x001B5318 File Offset: 0x001B3518
		public static void ReadOutSafe(this ArraySegment<ulong> source, int srcStartPos, ulong[] target, ref int bitposition, int bits)
		{
			int num = source.Offset << 6;
			srcStartPos += num;
			source.Array.ReadOutSafe(srcStartPos, target, ref bitposition, bits);
		}
	}
}
