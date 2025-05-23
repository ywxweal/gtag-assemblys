using System;
using emotitron.Compression.HalfFloat;
using emotitron.Compression.Utilities;

namespace emotitron.Compression
{
	// Token: 0x02000E0C RID: 3596
	public static class PrimitiveSerializeExt
	{
		// Token: 0x06005983 RID: 22915 RVA: 0x001B6BA6 File Offset: 0x001B4DA6
		public static void Inject(this ByteConverter value, ref ulong buffer, ref int bitposition, int bits)
		{
			value.Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x06005984 RID: 22916 RVA: 0x001B6BB6 File Offset: 0x001B4DB6
		public static void Inject(this ByteConverter value, ref uint buffer, ref int bitposition, int bits)
		{
			value.Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x06005985 RID: 22917 RVA: 0x001B6BC6 File Offset: 0x001B4DC6
		public static void Inject(this ByteConverter value, ref ushort buffer, ref int bitposition, int bits)
		{
			value.Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x06005986 RID: 22918 RVA: 0x001B6BD6 File Offset: 0x001B4DD6
		public static void Inject(this ByteConverter value, ref byte buffer, ref int bitposition, int bits)
		{
			value.Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x06005987 RID: 22919 RVA: 0x001B6BE8 File Offset: 0x001B4DE8
		public static ulong WriteSigned(this ulong buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)((value << 1) ^ (value >> 31));
			return buffer.Write((ulong)num, ref bitposition, bits);
		}

		// Token: 0x06005988 RID: 22920 RVA: 0x001B6C08 File Offset: 0x001B4E08
		public static void InjectSigned(this long value, ref ulong buffer, ref int bitposition, int bits)
		{
			((uint)((value << 1) ^ (value >> 31))).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x06005989 RID: 22921 RVA: 0x001B6C1B File Offset: 0x001B4E1B
		public static void InjectSigned(this int value, ref ulong buffer, ref int bitposition, int bits)
		{
			((uint)((value << 1) ^ (value >> 31))).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x0600598A RID: 22922 RVA: 0x001B6C1B File Offset: 0x001B4E1B
		public static void InjectSigned(this short value, ref ulong buffer, ref int bitposition, int bits)
		{
			((uint)(((int)value << 1) ^ (value >> 31))).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x0600598B RID: 22923 RVA: 0x001B6C1B File Offset: 0x001B4E1B
		public static void InjectSigned(this sbyte value, ref ulong buffer, ref int bitposition, int bits)
		{
			((uint)(((int)value << 1) ^ (value >> 31))).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x0600598C RID: 22924 RVA: 0x001B6C30 File Offset: 0x001B4E30
		public static int ReadSigned(this ulong buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.Read(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x0600598D RID: 22925 RVA: 0x001B6C54 File Offset: 0x001B4E54
		public static uint WriteSigned(this uint buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)((value << 1) ^ (value >> 31));
			return buffer.Write((ulong)num, ref bitposition, bits);
		}

		// Token: 0x0600598E RID: 22926 RVA: 0x001B6C74 File Offset: 0x001B4E74
		public static void InjectSigned(this long value, ref uint buffer, ref int bitposition, int bits)
		{
			((uint)((value << 1) ^ (value >> 31))).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x0600598F RID: 22927 RVA: 0x001B6C87 File Offset: 0x001B4E87
		public static void InjectSigned(this int value, ref uint buffer, ref int bitposition, int bits)
		{
			((uint)((value << 1) ^ (value >> 31))).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x06005990 RID: 22928 RVA: 0x001B6C87 File Offset: 0x001B4E87
		public static void InjectSigned(this short value, ref uint buffer, ref int bitposition, int bits)
		{
			((uint)(((int)value << 1) ^ (value >> 31))).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x06005991 RID: 22929 RVA: 0x001B6C87 File Offset: 0x001B4E87
		public static void InjectSigned(this sbyte value, ref uint buffer, ref int bitposition, int bits)
		{
			((uint)(((int)value << 1) ^ (value >> 31))).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x06005992 RID: 22930 RVA: 0x001B6C9C File Offset: 0x001B4E9C
		public static int ReadSigned(this uint buffer, ref int bitposition, int bits)
		{
			uint num = buffer.Read(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x06005993 RID: 22931 RVA: 0x001B6CC0 File Offset: 0x001B4EC0
		public static ushort WriteSigned(this ushort buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)((value << 1) ^ (value >> 31));
			return buffer.Write((ulong)num, ref bitposition, bits);
		}

		// Token: 0x06005994 RID: 22932 RVA: 0x001B6CE0 File Offset: 0x001B4EE0
		public static void InjectSigned(this long value, ref ushort buffer, ref int bitposition, int bits)
		{
			((uint)((value << 1) ^ (value >> 31))).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x06005995 RID: 22933 RVA: 0x001B6CF3 File Offset: 0x001B4EF3
		public static void InjectSigned(this int value, ref ushort buffer, ref int bitposition, int bits)
		{
			((uint)((value << 1) ^ (value >> 31))).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x06005996 RID: 22934 RVA: 0x001B6CF3 File Offset: 0x001B4EF3
		public static void InjectSigned(this short value, ref ushort buffer, ref int bitposition, int bits)
		{
			((uint)(((int)value << 1) ^ (value >> 31))).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x06005997 RID: 22935 RVA: 0x001B6CF3 File Offset: 0x001B4EF3
		public static void InjectSigned(this sbyte value, ref ushort buffer, ref int bitposition, int bits)
		{
			((uint)(((int)value << 1) ^ (value >> 31))).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x06005998 RID: 22936 RVA: 0x001B6D08 File Offset: 0x001B4F08
		public static int ReadSigned(this ushort buffer, ref int bitposition, int bits)
		{
			uint num = buffer.Read(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x06005999 RID: 22937 RVA: 0x001B6D2C File Offset: 0x001B4F2C
		public static byte WriteSigned(this byte buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)((value << 1) ^ (value >> 31));
			return buffer.Write((ulong)num, ref bitposition, bits);
		}

		// Token: 0x0600599A RID: 22938 RVA: 0x001B6D4C File Offset: 0x001B4F4C
		public static void InjectSigned(this long value, ref byte buffer, ref int bitposition, int bits)
		{
			((uint)((value << 1) ^ (value >> 31))).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x0600599B RID: 22939 RVA: 0x001B6D5F File Offset: 0x001B4F5F
		public static void InjectSigned(this int value, ref byte buffer, ref int bitposition, int bits)
		{
			((uint)((value << 1) ^ (value >> 31))).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x0600599C RID: 22940 RVA: 0x001B6D5F File Offset: 0x001B4F5F
		public static void InjectSigned(this short value, ref byte buffer, ref int bitposition, int bits)
		{
			((uint)(((int)value << 1) ^ (value >> 31))).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x0600599D RID: 22941 RVA: 0x001B6D5F File Offset: 0x001B4F5F
		public static void InjectSigned(this sbyte value, ref byte buffer, ref int bitposition, int bits)
		{
			((uint)(((int)value << 1) ^ (value >> 31))).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x0600599E RID: 22942 RVA: 0x001B6D74 File Offset: 0x001B4F74
		public static int ReadSigned(this byte buffer, ref int bitposition, int bits)
		{
			uint num = buffer.Read(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x0600599F RID: 22943 RVA: 0x001B6D95 File Offset: 0x001B4F95
		public static ulong WritetBool(this ulong buffer, bool value, ref int bitposition)
		{
			return buffer.Write((ulong)(value ? 1L : 0L), ref bitposition, 1);
		}

		// Token: 0x060059A0 RID: 22944 RVA: 0x001B6DA7 File Offset: 0x001B4FA7
		public static uint WritetBool(this uint buffer, bool value, ref int bitposition)
		{
			return buffer.Write((ulong)(value ? 1L : 0L), ref bitposition, 1);
		}

		// Token: 0x060059A1 RID: 22945 RVA: 0x001B6DB9 File Offset: 0x001B4FB9
		public static ushort WritetBool(this ushort buffer, bool value, ref int bitposition)
		{
			return buffer.Write((ulong)(value ? 1L : 0L), ref bitposition, 1);
		}

		// Token: 0x060059A2 RID: 22946 RVA: 0x001B6DCB File Offset: 0x001B4FCB
		public static byte WritetBool(this byte buffer, bool value, ref int bitposition)
		{
			return buffer.Write((ulong)(value ? 1L : 0L), ref bitposition, 1);
		}

		// Token: 0x060059A3 RID: 22947 RVA: 0x001B6DDD File Offset: 0x001B4FDD
		public static void Inject(this bool value, ref ulong buffer, ref int bitposition)
		{
			((ulong)(value ? 1L : 0L)).Inject(ref buffer, ref bitposition, 1);
		}

		// Token: 0x060059A4 RID: 22948 RVA: 0x001B6DEF File Offset: 0x001B4FEF
		public static void Inject(this bool value, ref uint buffer, ref int bitposition)
		{
			((ulong)(value ? 1L : 0L)).Inject(ref buffer, ref bitposition, 1);
		}

		// Token: 0x060059A5 RID: 22949 RVA: 0x001B6E01 File Offset: 0x001B5001
		public static void Inject(this bool value, ref ushort buffer, ref int bitposition)
		{
			((ulong)(value ? 1L : 0L)).Inject(ref buffer, ref bitposition, 1);
		}

		// Token: 0x060059A6 RID: 22950 RVA: 0x001B6E13 File Offset: 0x001B5013
		public static void Inject(this bool value, ref byte buffer, ref int bitposition)
		{
			((ulong)(value ? 1L : 0L)).Inject(ref buffer, ref bitposition, 1);
		}

		// Token: 0x060059A7 RID: 22951 RVA: 0x001B6E25 File Offset: 0x001B5025
		public static bool ReadBool(this ulong buffer, ref int bitposition)
		{
			return buffer.Read(ref bitposition, 1) != 0UL;
		}

		// Token: 0x060059A8 RID: 22952 RVA: 0x001B6E34 File Offset: 0x001B5034
		public static bool ReadtBool(this uint buffer, ref int bitposition)
		{
			return buffer.Read(ref bitposition, 1) != 0U;
		}

		// Token: 0x060059A9 RID: 22953 RVA: 0x001B6E43 File Offset: 0x001B5043
		public static bool ReadBool(this ushort buffer, ref int bitposition)
		{
			return buffer.Read(ref bitposition, 1) != 0U;
		}

		// Token: 0x060059AA RID: 22954 RVA: 0x001B6E52 File Offset: 0x001B5052
		public static bool ReadBool(this byte buffer, ref int bitposition)
		{
			return buffer.Read(ref bitposition, 1) != 0U;
		}

		// Token: 0x060059AB RID: 22955 RVA: 0x001B6E64 File Offset: 0x001B5064
		public static ulong Write(this ulong buffer, ulong value, ref int bitposition, int bits = 64)
		{
			ulong num = value << bitposition;
			ulong num2 = ulong.MaxValue >> 64 - bits << bitposition;
			buffer &= ~num2;
			buffer |= num2 & num;
			bitposition += bits;
			return buffer;
		}

		// Token: 0x060059AC RID: 22956 RVA: 0x001B6EA0 File Offset: 0x001B50A0
		public static uint Write(this uint buffer, ulong value, ref int bitposition, int bits = 64)
		{
			uint num = (uint)value << bitposition;
			uint num2 = uint.MaxValue >> 32 - bits << bitposition;
			buffer &= ~num2;
			buffer |= num2 & num;
			bitposition += bits;
			return buffer;
		}

		// Token: 0x060059AD RID: 22957 RVA: 0x001B6EDC File Offset: 0x001B50DC
		public static ushort Write(this ushort buffer, ulong value, ref int bitposition, int bits = 64)
		{
			uint num = (uint)value << bitposition;
			uint num2 = 65535U >> 16 - bits << bitposition;
			buffer = (ushort)(((uint)buffer & ~num2) | (num2 & num));
			bitposition += bits;
			return buffer;
		}

		// Token: 0x060059AE RID: 22958 RVA: 0x001B6F18 File Offset: 0x001B5118
		public static byte Write(this byte buffer, ulong value, ref int bitposition, int bits = 64)
		{
			uint num = (uint)value << bitposition;
			uint num2 = 255U >> 8 - bits << bitposition;
			buffer = (byte)(((uint)buffer & ~num2) | (num2 & num));
			bitposition += bits;
			return buffer;
		}

		// Token: 0x060059AF RID: 22959 RVA: 0x001B6F53 File Offset: 0x001B5153
		public static void Inject(this ulong value, ref ulong buffer, ref int bitposition, int bits = 64)
		{
			buffer = buffer.Write(value, ref bitposition, bits);
		}

		// Token: 0x060059B0 RID: 22960 RVA: 0x001B6F64 File Offset: 0x001B5164
		public static void Inject(this ulong value, ref ulong buffer, int bitposition, int bits = 64)
		{
			ulong num = value << bitposition;
			ulong num2 = ulong.MaxValue >> 64 - bits << bitposition;
			buffer &= ~num2;
			buffer |= num2 & num;
		}

		// Token: 0x060059B1 RID: 22961 RVA: 0x001B6F97 File Offset: 0x001B5197
		public static void Inject(this uint value, ref ulong buffer, ref int bitposition, int bits = 32)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x060059B2 RID: 22962 RVA: 0x001B6FA8 File Offset: 0x001B51A8
		public static void Inject(this uint value, ref ulong buffer, int bitposition, int bits = 32)
		{
			ulong num = (ulong)value << bitposition;
			ulong num2 = ulong.MaxValue >> 64 - bits << bitposition;
			buffer &= ~num2;
			buffer |= num2 & num;
		}

		// Token: 0x060059B3 RID: 22963 RVA: 0x001B6F97 File Offset: 0x001B5197
		public static void Inject(this ushort value, ref ulong buffer, ref int bitposition, int bits = 16)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x060059B4 RID: 22964 RVA: 0x001B6FDC File Offset: 0x001B51DC
		public static void Inject(this ushort value, ref ulong buffer, int bitposition, int bits = 16)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x060059B5 RID: 22965 RVA: 0x001B6F97 File Offset: 0x001B5197
		public static void Inject(this byte value, ref ulong buffer, ref int bitposition, int bits = 8)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x060059B6 RID: 22966 RVA: 0x001B6FDC File Offset: 0x001B51DC
		public static void Inject(this byte value, ref ulong buffer, int bitposition, int bits = 8)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x060059B7 RID: 22967 RVA: 0x001B6F53 File Offset: 0x001B5153
		public static void InjectUnsigned(this long value, ref ulong buffer, ref int bitposition, int bits = 32)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x060059B8 RID: 22968 RVA: 0x001B6FEC File Offset: 0x001B51EC
		public static void InjectUnsigned(this int value, ref ulong buffer, ref int bitposition, int bits = 32)
		{
			buffer = buffer.Write((ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x060059B9 RID: 22969 RVA: 0x001B6FEC File Offset: 0x001B51EC
		public static void InjectUnsigned(this short value, ref ulong buffer, ref int bitposition, int bits = 32)
		{
			buffer = buffer.Write((ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x060059BA RID: 22970 RVA: 0x001B6FEC File Offset: 0x001B51EC
		public static void InjectUnsigned(this sbyte value, ref ulong buffer, ref int bitposition, int bits = 32)
		{
			buffer = buffer.Write((ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x060059BB RID: 22971 RVA: 0x001B6FFB File Offset: 0x001B51FB
		public static void Inject(this ulong value, ref uint buffer, ref int bitposition, int bits = 64)
		{
			buffer = buffer.Write(value, ref bitposition, bits);
		}

		// Token: 0x060059BC RID: 22972 RVA: 0x001B7009 File Offset: 0x001B5209
		public static void Inject(this ulong value, ref uint buffer, int bitposition, int bits = 64)
		{
			buffer = buffer.Write(value, ref bitposition, bits);
		}

		// Token: 0x060059BD RID: 22973 RVA: 0x001B7018 File Offset: 0x001B5218
		public static void Inject(this uint value, ref uint buffer, ref int bitposition, int bits = 32)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x060059BE RID: 22974 RVA: 0x001B7027 File Offset: 0x001B5227
		public static void Inject(this uint value, ref uint buffer, int bitposition, int bits = 32)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x060059BF RID: 22975 RVA: 0x001B7018 File Offset: 0x001B5218
		public static void Inject(this ushort value, ref uint buffer, ref int bitposition, int bits = 16)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x060059C0 RID: 22976 RVA: 0x001B7027 File Offset: 0x001B5227
		public static void Inject(this ushort value, ref uint buffer, int bitposition, int bits = 16)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x060059C1 RID: 22977 RVA: 0x001B7018 File Offset: 0x001B5218
		public static void Inject(this byte value, ref uint buffer, ref int bitposition, int bits = 8)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x060059C2 RID: 22978 RVA: 0x001B7027 File Offset: 0x001B5227
		public static void Inject(this byte value, ref uint buffer, int bitposition, int bits = 8)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x060059C3 RID: 22979 RVA: 0x001B6FFB File Offset: 0x001B51FB
		public static void InjectUnsigned(this long value, ref uint buffer, ref int bitposition, int bits = 64)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x060059C4 RID: 22980 RVA: 0x001B7037 File Offset: 0x001B5237
		public static void InjectUnsigned(this int value, ref uint buffer, ref int bitposition, int bits = 64)
		{
			buffer = buffer.Write((ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x060059C5 RID: 22981 RVA: 0x001B7037 File Offset: 0x001B5237
		public static void InjectUnsigned(this short value, ref uint buffer, ref int bitposition, int bits = 64)
		{
			buffer = buffer.Write((ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x060059C6 RID: 22982 RVA: 0x001B7037 File Offset: 0x001B5237
		public static void InjectUnsigned(this sbyte value, ref uint buffer, ref int bitposition, int bits = 64)
		{
			buffer = buffer.Write((ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x060059C7 RID: 22983 RVA: 0x001B7046 File Offset: 0x001B5246
		public static void Inject(this ulong value, ref ushort buffer, ref int bitposition, int bits = 16)
		{
			buffer = buffer.Write(value, ref bitposition, bits);
		}

		// Token: 0x060059C8 RID: 22984 RVA: 0x001B7054 File Offset: 0x001B5254
		public static void Inject(this ulong value, ref ushort buffer, int bitposition, int bits = 16)
		{
			buffer = buffer.Write(value, ref bitposition, bits);
		}

		// Token: 0x060059C9 RID: 22985 RVA: 0x001B7063 File Offset: 0x001B5263
		public static void Inject(this uint value, ref ushort buffer, ref int bitposition, int bits = 16)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x060059CA RID: 22986 RVA: 0x001B7072 File Offset: 0x001B5272
		public static void Inject(this uint value, ref ushort buffer, int bitposition, int bits = 16)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x060059CB RID: 22987 RVA: 0x001B7063 File Offset: 0x001B5263
		public static void Inject(this ushort value, ref ushort buffer, ref int bitposition, int bits = 16)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x060059CC RID: 22988 RVA: 0x001B7072 File Offset: 0x001B5272
		public static void Inject(this ushort value, ref ushort buffer, int bitposition, int bits = 16)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x060059CD RID: 22989 RVA: 0x001B7063 File Offset: 0x001B5263
		public static void Inject(this byte value, ref ushort buffer, ref int bitposition, int bits = 8)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x060059CE RID: 22990 RVA: 0x001B7072 File Offset: 0x001B5272
		public static void Inject(this byte value, ref ushort buffer, int bitposition, int bits = 8)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x060059CF RID: 22991 RVA: 0x001B7082 File Offset: 0x001B5282
		public static void Inject(this ulong value, ref byte buffer, ref int bitposition, int bits = 8)
		{
			buffer = buffer.Write(value, ref bitposition, bits);
		}

		// Token: 0x060059D0 RID: 22992 RVA: 0x001B7090 File Offset: 0x001B5290
		public static void Inject(this ulong value, ref byte buffer, int bitposition, int bits = 8)
		{
			buffer = buffer.Write(value, ref bitposition, bits);
		}

		// Token: 0x060059D1 RID: 22993 RVA: 0x001B709F File Offset: 0x001B529F
		public static void Inject(this uint value, ref byte buffer, ref int bitposition, int bits = 8)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x060059D2 RID: 22994 RVA: 0x001B70AE File Offset: 0x001B52AE
		public static void Inject(this uint value, ref byte buffer, int bitposition, int bits = 8)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x060059D3 RID: 22995 RVA: 0x001B709F File Offset: 0x001B529F
		public static void Inject(this ushort value, ref byte buffer, ref int bitposition, int bits = 8)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x060059D4 RID: 22996 RVA: 0x001B70AE File Offset: 0x001B52AE
		public static void Inject(this ushort value, ref byte buffer, int bitposition, int bits = 8)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x060059D5 RID: 22997 RVA: 0x001B709F File Offset: 0x001B529F
		public static void Inject(this byte value, ref byte buffer, ref int bitposition, int bits = 8)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x060059D6 RID: 22998 RVA: 0x001B70AE File Offset: 0x001B52AE
		public static void Inject(this byte value, ref byte buffer, int bitposition, int bits = 8)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x060059D7 RID: 22999 RVA: 0x001B70BE File Offset: 0x001B52BE
		[Obsolete("Argument order changed")]
		public static ulong Extract(this ulong value, int bits, ref int bitposition)
		{
			return value.Extract(bits, ref bitposition);
		}

		// Token: 0x060059D8 RID: 23000 RVA: 0x001B70C8 File Offset: 0x001B52C8
		public static ulong Read(this ulong value, ref int bitposition, int bits)
		{
			ulong num = ulong.MaxValue >> 64 - bits;
			ulong num2 = (value >> bitposition) & num;
			bitposition += bits;
			return num2;
		}

		// Token: 0x060059D9 RID: 23001 RVA: 0x001B70F0 File Offset: 0x001B52F0
		[Obsolete("Use Read instead.")]
		public static ulong Extract(this ulong value, ref int bitposition, int bits)
		{
			ulong num = ulong.MaxValue >> 64 - bits;
			ulong num2 = (value >> bitposition) & num;
			bitposition += bits;
			return num2;
		}

		// Token: 0x060059DA RID: 23002 RVA: 0x001B7118 File Offset: 0x001B5318
		[Obsolete("Always include the [ref int bitposition] argument. Extracting from position 0 would be better handled with a mask operation.")]
		public static ulong Extract(this ulong value, int bits)
		{
			ulong num = ulong.MaxValue >> 64 - bits;
			return value & num;
		}

		// Token: 0x060059DB RID: 23003 RVA: 0x001B7134 File Offset: 0x001B5334
		public static uint Read(this uint value, ref int bitposition, int bits)
		{
			uint num = uint.MaxValue >> 32 - bits;
			uint num2 = (value >> bitposition) & num;
			bitposition += bits;
			return num2;
		}

		// Token: 0x060059DC RID: 23004 RVA: 0x001B715C File Offset: 0x001B535C
		[Obsolete("Use Read instead.")]
		public static uint Extract(this uint value, ref int bitposition, int bits)
		{
			uint num = uint.MaxValue >> 32 - bits;
			uint num2 = (value >> bitposition) & num;
			bitposition += bits;
			return num2;
		}

		// Token: 0x060059DD RID: 23005 RVA: 0x001B7184 File Offset: 0x001B5384
		[Obsolete("Always include the [ref int bitposition] argument. Extracting from position 0 would be better handled with a mask operation.")]
		public static uint Extract(this uint value, int bits)
		{
			uint num = uint.MaxValue >> 32 - bits;
			return value & num;
		}

		// Token: 0x060059DE RID: 23006 RVA: 0x001B71A0 File Offset: 0x001B53A0
		public static uint Read(this ushort value, ref int bitposition, int bits)
		{
			uint num = 65535U >> 16 - bits;
			uint num2 = ((uint)value >> bitposition) & num;
			bitposition += bits;
			return num2;
		}

		// Token: 0x060059DF RID: 23007 RVA: 0x001B71CC File Offset: 0x001B53CC
		[Obsolete("Use Read instead.")]
		public static uint Extract(this ushort value, ref int bitposition, int bits)
		{
			uint num = 65535U >> 16 - bits;
			uint num2 = ((uint)value >> bitposition) & num;
			bitposition += bits;
			return num2;
		}

		// Token: 0x060059E0 RID: 23008 RVA: 0x001B71F8 File Offset: 0x001B53F8
		public static uint Read(this byte value, ref int bitposition, int bits)
		{
			uint num = 255U >> 8 - bits;
			uint num2 = ((uint)value >> bitposition) & num;
			bitposition += bits;
			return num2;
		}

		// Token: 0x060059E1 RID: 23009 RVA: 0x001B7224 File Offset: 0x001B5424
		[Obsolete("Use Read instead.")]
		public static uint Extract(this byte value, ref int bitposition, int bits)
		{
			uint num = 255U >> 8 - bits;
			uint num2 = ((uint)value >> bitposition) & num;
			bitposition += bits;
			return num2;
		}

		// Token: 0x060059E2 RID: 23010 RVA: 0x001B7250 File Offset: 0x001B5450
		[Obsolete("Always include the [ref int bitposition] argument. Extracting from position 0 would be better handled with a mask operation.")]
		public static byte Extract(this byte value, int bits)
		{
			uint num = 255U >> 8 - bits;
			return (byte)((uint)value & num);
		}

		// Token: 0x060059E3 RID: 23011 RVA: 0x001B726E File Offset: 0x001B546E
		public static void Inject(this float f, ref ulong buffer, ref int bitposition)
		{
			buffer = buffer.Write(f, ref bitposition, 32);
		}

		// Token: 0x060059E4 RID: 23012 RVA: 0x001B7287 File Offset: 0x001B5487
		public static float ReadFloat(this ulong buffer, ref int bitposition)
		{
			return buffer.Read(ref bitposition, 32);
		}

		// Token: 0x060059E5 RID: 23013 RVA: 0x001B729C File Offset: 0x001B549C
		[Obsolete("Use Read instead.")]
		public static float ExtractFloat(this ulong buffer, ref int bitposition)
		{
			return buffer.Extract(ref bitposition, 32);
		}

		// Token: 0x060059E6 RID: 23014 RVA: 0x001B72B4 File Offset: 0x001B54B4
		public static ushort InjectAsHalfFloat(this float f, ref ulong buffer, ref int bitposition)
		{
			ushort num = HalfUtilities.Pack(f);
			buffer = buffer.Write((ulong)num, ref bitposition, 16);
			return num;
		}

		// Token: 0x060059E7 RID: 23015 RVA: 0x001B72D8 File Offset: 0x001B54D8
		public static ushort InjectAsHalfFloat(this float f, ref uint buffer, ref int bitposition)
		{
			ushort num = HalfUtilities.Pack(f);
			buffer = buffer.Write((ulong)num, ref bitposition, 16);
			return num;
		}

		// Token: 0x060059E8 RID: 23016 RVA: 0x001B72FB File Offset: 0x001B54FB
		public static float ReadHalfFloat(this ulong buffer, ref int bitposition)
		{
			return HalfUtilities.Unpack((ushort)buffer.Read(ref bitposition, 16));
		}

		// Token: 0x060059E9 RID: 23017 RVA: 0x001B730C File Offset: 0x001B550C
		[Obsolete("Use Read instead.")]
		public static float ExtractHalfFloat(this ulong buffer, ref int bitposition)
		{
			return HalfUtilities.Unpack((ushort)buffer.Extract(ref bitposition, 16));
		}

		// Token: 0x060059EA RID: 23018 RVA: 0x001B731D File Offset: 0x001B551D
		public static float ReadHalfFloat(this uint buffer, ref int bitposition)
		{
			return HalfUtilities.Unpack((ushort)buffer.Read(ref bitposition, 16));
		}

		// Token: 0x060059EB RID: 23019 RVA: 0x001B732E File Offset: 0x001B552E
		[Obsolete("Use Read instead.")]
		public static float ExtractHalfFloat(this uint buffer, ref int bitposition)
		{
			return HalfUtilities.Unpack((ushort)buffer.Extract(ref bitposition, 16));
		}

		// Token: 0x060059EC RID: 23020 RVA: 0x001B733F File Offset: 0x001B553F
		[Obsolete("Argument order changed")]
		public static void Inject(this ulong value, ref uint buffer, int bits, ref int bitposition)
		{
			value.Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x060059ED RID: 23021 RVA: 0x001B734A File Offset: 0x001B554A
		[Obsolete("Argument order changed")]
		public static void Inject(this ulong value, ref ulong buffer, int bits, ref int bitposition)
		{
			value.Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x04005E39 RID: 24121
		private const string overrunerror = "Write buffer overrun. writepos + bits exceeds target length. Data loss will occur.";
	}
}
