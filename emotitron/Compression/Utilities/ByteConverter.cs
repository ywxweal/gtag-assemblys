using System;
using System.Runtime.InteropServices;

namespace emotitron.Compression.Utilities
{
	// Token: 0x02000E14 RID: 3604
	[StructLayout(LayoutKind.Explicit)]
	public struct ByteConverter
	{
		// Token: 0x170008D1 RID: 2257
		public byte this[int index]
		{
			get
			{
				switch (index)
				{
				case 0:
					return this.byte0;
				case 1:
					return this.byte1;
				case 2:
					return this.byte2;
				case 3:
					return this.byte3;
				case 4:
					return this.byte4;
				case 5:
					return this.byte5;
				case 6:
					return this.byte6;
				case 7:
					return this.byte7;
				default:
					return 0;
				}
			}
		}

		// Token: 0x06005A12 RID: 23058 RVA: 0x001B7B80 File Offset: 0x001B5D80
		public static implicit operator ByteConverter(byte[] bytes)
		{
			ByteConverter byteConverter = default(ByteConverter);
			int num = bytes.Length;
			byteConverter.byte0 = bytes[0];
			if (num > 0)
			{
				byteConverter.byte1 = bytes[1];
			}
			if (num > 1)
			{
				byteConverter.byte2 = bytes[2];
			}
			if (num > 2)
			{
				byteConverter.byte3 = bytes[3];
			}
			if (num > 3)
			{
				byteConverter.byte4 = bytes[4];
			}
			if (num > 4)
			{
				byteConverter.byte5 = bytes[5];
			}
			if (num > 5)
			{
				byteConverter.byte6 = bytes[3];
			}
			if (num > 6)
			{
				byteConverter.byte7 = bytes[7];
			}
			return byteConverter;
		}

		// Token: 0x06005A13 RID: 23059 RVA: 0x001B7C04 File Offset: 0x001B5E04
		public static implicit operator ByteConverter(byte val)
		{
			return new ByteConverter
			{
				byte0 = val
			};
		}

		// Token: 0x06005A14 RID: 23060 RVA: 0x001B7C24 File Offset: 0x001B5E24
		public static implicit operator ByteConverter(sbyte val)
		{
			return new ByteConverter
			{
				int8 = val
			};
		}

		// Token: 0x06005A15 RID: 23061 RVA: 0x001B7C44 File Offset: 0x001B5E44
		public static implicit operator ByteConverter(char val)
		{
			return new ByteConverter
			{
				character = val
			};
		}

		// Token: 0x06005A16 RID: 23062 RVA: 0x001B7C64 File Offset: 0x001B5E64
		public static implicit operator ByteConverter(uint val)
		{
			return new ByteConverter
			{
				uint32 = val
			};
		}

		// Token: 0x06005A17 RID: 23063 RVA: 0x001B7C84 File Offset: 0x001B5E84
		public static implicit operator ByteConverter(int val)
		{
			return new ByteConverter
			{
				int32 = val
			};
		}

		// Token: 0x06005A18 RID: 23064 RVA: 0x001B7CA4 File Offset: 0x001B5EA4
		public static implicit operator ByteConverter(ulong val)
		{
			return new ByteConverter
			{
				uint64 = val
			};
		}

		// Token: 0x06005A19 RID: 23065 RVA: 0x001B7CC4 File Offset: 0x001B5EC4
		public static implicit operator ByteConverter(long val)
		{
			return new ByteConverter
			{
				int64 = val
			};
		}

		// Token: 0x06005A1A RID: 23066 RVA: 0x001B7CE4 File Offset: 0x001B5EE4
		public static implicit operator ByteConverter(float val)
		{
			return new ByteConverter
			{
				float32 = val
			};
		}

		// Token: 0x06005A1B RID: 23067 RVA: 0x001B7D04 File Offset: 0x001B5F04
		public static implicit operator ByteConverter(double val)
		{
			return new ByteConverter
			{
				float64 = val
			};
		}

		// Token: 0x06005A1C RID: 23068 RVA: 0x001B7D24 File Offset: 0x001B5F24
		public static implicit operator ByteConverter(bool val)
		{
			return new ByteConverter
			{
				int32 = (val ? 1 : 0)
			};
		}

		// Token: 0x06005A1D RID: 23069 RVA: 0x001B7D48 File Offset: 0x001B5F48
		public void ExtractByteArray(byte[] targetArray)
		{
			int num = targetArray.Length;
			targetArray[0] = this.byte0;
			if (num > 0)
			{
				targetArray[1] = this.byte1;
			}
			if (num > 1)
			{
				targetArray[2] = this.byte2;
			}
			if (num > 2)
			{
				targetArray[3] = this.byte3;
			}
			if (num > 3)
			{
				targetArray[4] = this.byte4;
			}
			if (num > 4)
			{
				targetArray[5] = this.byte5;
			}
			if (num > 5)
			{
				targetArray[6] = this.byte6;
			}
			if (num > 6)
			{
				targetArray[7] = this.byte7;
			}
		}

		// Token: 0x06005A1E RID: 23070 RVA: 0x001B7DBB File Offset: 0x001B5FBB
		public static implicit operator byte(ByteConverter bc)
		{
			return bc.byte0;
		}

		// Token: 0x06005A1F RID: 23071 RVA: 0x001B7DC3 File Offset: 0x001B5FC3
		public static implicit operator sbyte(ByteConverter bc)
		{
			return bc.int8;
		}

		// Token: 0x06005A20 RID: 23072 RVA: 0x001B7DCB File Offset: 0x001B5FCB
		public static implicit operator char(ByteConverter bc)
		{
			return bc.character;
		}

		// Token: 0x06005A21 RID: 23073 RVA: 0x001B7DD3 File Offset: 0x001B5FD3
		public static implicit operator ushort(ByteConverter bc)
		{
			return bc.uint16;
		}

		// Token: 0x06005A22 RID: 23074 RVA: 0x001B7DDB File Offset: 0x001B5FDB
		public static implicit operator short(ByteConverter bc)
		{
			return bc.int16;
		}

		// Token: 0x06005A23 RID: 23075 RVA: 0x001B7DE3 File Offset: 0x001B5FE3
		public static implicit operator uint(ByteConverter bc)
		{
			return bc.uint32;
		}

		// Token: 0x06005A24 RID: 23076 RVA: 0x001B7DEB File Offset: 0x001B5FEB
		public static implicit operator int(ByteConverter bc)
		{
			return bc.int32;
		}

		// Token: 0x06005A25 RID: 23077 RVA: 0x001B7DF3 File Offset: 0x001B5FF3
		public static implicit operator ulong(ByteConverter bc)
		{
			return bc.uint64;
		}

		// Token: 0x06005A26 RID: 23078 RVA: 0x001B7DFB File Offset: 0x001B5FFB
		public static implicit operator long(ByteConverter bc)
		{
			return bc.int64;
		}

		// Token: 0x06005A27 RID: 23079 RVA: 0x001B7E03 File Offset: 0x001B6003
		public static implicit operator float(ByteConverter bc)
		{
			return bc.float32;
		}

		// Token: 0x06005A28 RID: 23080 RVA: 0x001B7E0B File Offset: 0x001B600B
		public static implicit operator double(ByteConverter bc)
		{
			return bc.float64;
		}

		// Token: 0x06005A29 RID: 23081 RVA: 0x001B7E13 File Offset: 0x001B6013
		public static implicit operator bool(ByteConverter bc)
		{
			return bc.int32 != 0;
		}

		// Token: 0x04005E5A RID: 24154
		[FieldOffset(0)]
		public float float32;

		// Token: 0x04005E5B RID: 24155
		[FieldOffset(0)]
		public double float64;

		// Token: 0x04005E5C RID: 24156
		[FieldOffset(0)]
		public sbyte int8;

		// Token: 0x04005E5D RID: 24157
		[FieldOffset(0)]
		public short int16;

		// Token: 0x04005E5E RID: 24158
		[FieldOffset(0)]
		public ushort uint16;

		// Token: 0x04005E5F RID: 24159
		[FieldOffset(0)]
		public char character;

		// Token: 0x04005E60 RID: 24160
		[FieldOffset(0)]
		public int int32;

		// Token: 0x04005E61 RID: 24161
		[FieldOffset(0)]
		public uint uint32;

		// Token: 0x04005E62 RID: 24162
		[FieldOffset(0)]
		public long int64;

		// Token: 0x04005E63 RID: 24163
		[FieldOffset(0)]
		public ulong uint64;

		// Token: 0x04005E64 RID: 24164
		[FieldOffset(0)]
		public byte byte0;

		// Token: 0x04005E65 RID: 24165
		[FieldOffset(1)]
		public byte byte1;

		// Token: 0x04005E66 RID: 24166
		[FieldOffset(2)]
		public byte byte2;

		// Token: 0x04005E67 RID: 24167
		[FieldOffset(3)]
		public byte byte3;

		// Token: 0x04005E68 RID: 24168
		[FieldOffset(4)]
		public byte byte4;

		// Token: 0x04005E69 RID: 24169
		[FieldOffset(5)]
		public byte byte5;

		// Token: 0x04005E6A RID: 24170
		[FieldOffset(6)]
		public byte byte6;

		// Token: 0x04005E6B RID: 24171
		[FieldOffset(7)]
		public byte byte7;

		// Token: 0x04005E6C RID: 24172
		[FieldOffset(4)]
		public uint uint16_B;
	}
}
