using System;

namespace emotitron.Compression
{
	// Token: 0x02000E04 RID: 3588
	public static class ArraySegmentExt
	{
		// Token: 0x060058DF RID: 22751 RVA: 0x001B4F9B File Offset: 0x001B319B
		public static ArraySegment<byte> ExtractArraySegment(byte[] buffer, ref int bitposition)
		{
			return new ArraySegment<byte>(buffer, 0, bitposition + 7 >> 3);
		}

		// Token: 0x060058E0 RID: 22752 RVA: 0x001B4FAA File Offset: 0x001B31AA
		public static ArraySegment<ushort> ExtractArraySegment(ushort[] buffer, ref int bitposition)
		{
			return new ArraySegment<ushort>(buffer, 0, bitposition + 15 >> 4);
		}

		// Token: 0x060058E1 RID: 22753 RVA: 0x001B4FBA File Offset: 0x001B31BA
		public static ArraySegment<uint> ExtractArraySegment(uint[] buffer, ref int bitposition)
		{
			return new ArraySegment<uint>(buffer, 0, bitposition + 31 >> 5);
		}

		// Token: 0x060058E2 RID: 22754 RVA: 0x001B4FCA File Offset: 0x001B31CA
		public static ArraySegment<ulong> ExtractArraySegment(ulong[] buffer, ref int bitposition)
		{
			return new ArraySegment<ulong>(buffer, 0, bitposition + 63 >> 6);
		}

		// Token: 0x060058E3 RID: 22755 RVA: 0x001B4FDC File Offset: 0x001B31DC
		public static void Append(this ArraySegment<byte> buffer, ulong value, ref int bitposition, int bits)
		{
			int num = buffer.Offset << 3;
			bitposition += num;
			buffer.Array.Append(value, ref bitposition, bits);
			bitposition -= num;
		}

		// Token: 0x060058E4 RID: 22756 RVA: 0x001B5010 File Offset: 0x001B3210
		public static void Append(this ArraySegment<uint> buffer, ulong value, ref int bitposition, int bits)
		{
			int num = buffer.Offset << 5;
			bitposition += num;
			buffer.Array.Append(value, ref bitposition, bits);
			bitposition -= num;
		}

		// Token: 0x060058E5 RID: 22757 RVA: 0x001B5044 File Offset: 0x001B3244
		public static void Append(this ArraySegment<ulong> buffer, ulong value, ref int bitposition, int bits)
		{
			int num = buffer.Offset << 6;
			bitposition += num;
			buffer.Array.Append(value, ref bitposition, bits);
			bitposition -= num;
		}

		// Token: 0x060058E6 RID: 22758 RVA: 0x001B5078 File Offset: 0x001B3278
		public static void Write(this ArraySegment<byte> buffer, ulong value, ref int bitposition, int bits)
		{
			int num = buffer.Offset << 3;
			bitposition += num;
			buffer.Array.Write(value, ref bitposition, bits);
			bitposition -= num;
		}

		// Token: 0x060058E7 RID: 22759 RVA: 0x001B50AC File Offset: 0x001B32AC
		public static void Write(this ArraySegment<uint> buffer, ulong value, ref int bitposition, int bits)
		{
			int num = buffer.Offset << 5;
			bitposition += num;
			buffer.Array.Write(value, ref bitposition, bits);
			bitposition -= num;
		}

		// Token: 0x060058E8 RID: 22760 RVA: 0x001B50E0 File Offset: 0x001B32E0
		public static void Write(this ArraySegment<ulong> buffer, ulong value, ref int bitposition, int bits)
		{
			int num = buffer.Offset << 6;
			bitposition += num;
			buffer.Array.Write(value, ref bitposition, bits);
			bitposition -= num;
		}

		// Token: 0x060058E9 RID: 22761 RVA: 0x001B5114 File Offset: 0x001B3314
		public static ulong Read(this ArraySegment<byte> buffer, ref int bitposition, int bits)
		{
			int num = buffer.Offset << 3;
			bitposition += num;
			ulong num2 = buffer.Array.Read(ref bitposition, bits);
			bitposition -= num;
			return num2;
		}

		// Token: 0x060058EA RID: 22762 RVA: 0x001B5148 File Offset: 0x001B3348
		public static ulong Read(this ArraySegment<uint> buffer, ref int bitposition, int bits)
		{
			int num = buffer.Offset << 5;
			bitposition += num;
			ulong num2 = buffer.Array.Read(ref bitposition, bits);
			bitposition -= num;
			return num2;
		}

		// Token: 0x060058EB RID: 22763 RVA: 0x001B517C File Offset: 0x001B337C
		public static ulong Read(this ArraySegment<ulong> buffer, ref int bitposition, int bits)
		{
			int num = buffer.Offset << 6;
			bitposition += num;
			ulong num2 = buffer.Array.Read(ref bitposition, bits);
			bitposition -= num;
			return num2;
		}

		// Token: 0x060058EC RID: 22764 RVA: 0x001B51B0 File Offset: 0x001B33B0
		public static void ReadOutSafe(this ArraySegment<byte> source, int srcStartPos, byte[] target, ref int bitposition, int bits)
		{
			int num = source.Offset << 3;
			srcStartPos += num;
			source.Array.ReadOutSafe(srcStartPos, target, ref bitposition, bits);
		}

		// Token: 0x060058ED RID: 22765 RVA: 0x001B51E0 File Offset: 0x001B33E0
		public static void ReadOutSafe(this ArraySegment<byte> source, int srcStartPos, ulong[] target, ref int bitposition, int bits)
		{
			int num = source.Offset << 3;
			srcStartPos += num;
			source.Array.ReadOutSafe(srcStartPos, target, ref bitposition, bits);
		}

		// Token: 0x060058EE RID: 22766 RVA: 0x001B5210 File Offset: 0x001B3410
		public static void ReadOutSafe(this ArraySegment<ulong> source, int srcStartPos, byte[] target, ref int bitposition, int bits)
		{
			int num = source.Offset << 6;
			srcStartPos += num;
			source.Array.ReadOutSafe(srcStartPos, target, ref bitposition, bits);
		}

		// Token: 0x060058EF RID: 22767 RVA: 0x001B5240 File Offset: 0x001B3440
		public static void ReadOutSafe(this ArraySegment<ulong> source, int srcStartPos, ulong[] target, ref int bitposition, int bits)
		{
			int num = source.Offset << 6;
			srcStartPos += num;
			source.Array.ReadOutSafe(srcStartPos, target, ref bitposition, bits);
		}
	}
}
