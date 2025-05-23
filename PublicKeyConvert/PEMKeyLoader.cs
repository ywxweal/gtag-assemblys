﻿using System;
using System.IO;
using System.Security.Cryptography;

namespace PublicKeyConvert
{
	// Token: 0x02000A3E RID: 2622
	public class PEMKeyLoader
	{
		// Token: 0x06003E58 RID: 15960 RVA: 0x00127E00 File Offset: 0x00126000
		private static bool CompareBytearrays(byte[] a, byte[] b)
		{
			if (a.Length != b.Length)
			{
				return false;
			}
			int num = 0;
			for (int i = 0; i < a.Length; i++)
			{
				if (a[i] != b[num])
				{
					return false;
				}
				num++;
			}
			return true;
		}

		// Token: 0x06003E59 RID: 15961 RVA: 0x00127E38 File Offset: 0x00126038
		public static RSACryptoServiceProvider CryptoServiceProviderFromPublicKeyInfo(byte[] x509key)
		{
			new byte[15];
			if (x509key == null || x509key.Length == 0)
			{
				return null;
			}
			BinaryReader binaryReader = new BinaryReader(new MemoryStream(x509key));
			RSACryptoServiceProvider rsacryptoServiceProvider;
			try
			{
				ushort num = binaryReader.ReadUInt16();
				if (num == 33072)
				{
					binaryReader.ReadByte();
				}
				else
				{
					if (num != 33328)
					{
						return null;
					}
					binaryReader.ReadInt16();
				}
				if (!PEMKeyLoader.CompareBytearrays(binaryReader.ReadBytes(15), PEMKeyLoader.SeqOID))
				{
					rsacryptoServiceProvider = null;
				}
				else
				{
					num = binaryReader.ReadUInt16();
					if (num == 33027)
					{
						binaryReader.ReadByte();
					}
					else
					{
						if (num != 33283)
						{
							return null;
						}
						binaryReader.ReadInt16();
					}
					if (binaryReader.ReadByte() != 0)
					{
						rsacryptoServiceProvider = null;
					}
					else
					{
						num = binaryReader.ReadUInt16();
						if (num == 33072)
						{
							binaryReader.ReadByte();
						}
						else
						{
							if (num != 33328)
							{
								return null;
							}
							binaryReader.ReadInt16();
						}
						num = binaryReader.ReadUInt16();
						byte b = 0;
						byte b2;
						if (num == 33026)
						{
							b2 = binaryReader.ReadByte();
						}
						else
						{
							if (num != 33282)
							{
								return null;
							}
							b = binaryReader.ReadByte();
							b2 = binaryReader.ReadByte();
						}
						byte[] array = new byte[4];
						array[0] = b2;
						array[1] = b;
						int num2 = BitConverter.ToInt32(array, 0);
						if (binaryReader.PeekChar() == 0)
						{
							binaryReader.ReadByte();
							num2--;
						}
						byte[] array2 = binaryReader.ReadBytes(num2);
						if (binaryReader.ReadByte() != 2)
						{
							rsacryptoServiceProvider = null;
						}
						else
						{
							int num3 = (int)binaryReader.ReadByte();
							byte[] array3 = binaryReader.ReadBytes(num3);
							RSACryptoServiceProvider rsacryptoServiceProvider2 = new RSACryptoServiceProvider();
							rsacryptoServiceProvider2.ImportParameters(new RSAParameters
							{
								Modulus = array2,
								Exponent = array3
							});
							rsacryptoServiceProvider = rsacryptoServiceProvider2;
						}
					}
				}
			}
			finally
			{
				binaryReader.Close();
			}
			return rsacryptoServiceProvider;
		}

		// Token: 0x06003E5A RID: 15962 RVA: 0x00128004 File Offset: 0x00126204
		public static RSACryptoServiceProvider CryptoServiceProviderFromPublicKeyInfo(string base64EncodedKey)
		{
			try
			{
				return PEMKeyLoader.CryptoServiceProviderFromPublicKeyInfo(Convert.FromBase64String(base64EncodedKey));
			}
			catch (FormatException)
			{
			}
			return null;
		}

		// Token: 0x040042E7 RID: 17127
		private static byte[] SeqOID = new byte[]
		{
			48, 13, 6, 9, 42, 134, 72, 134, 247, 13,
			1, 1, 1, 5, 0
		};
	}
}
