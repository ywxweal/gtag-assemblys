using System;
using System.Collections.Generic;
using System.Text;

namespace GorillaTag.Scripts.Utilities
{
	// Token: 0x02000D49 RID: 3401
	public static class GTStr
	{
		// Token: 0x06005522 RID: 21794 RVA: 0x0019E6AC File Offset: 0x0019C8AC
		public static void Bullet(StringBuilder builder, IList<string> strings, string bulletStr = "- ")
		{
			for (int i = 0; i < strings.Count; i++)
			{
				builder.Append(bulletStr).Append(strings[i]).Append("\n");
			}
		}

		// Token: 0x06005523 RID: 21795 RVA: 0x0019E6E8 File Offset: 0x0019C8E8
		public static string Bullet(IList<string> strings, string bulletStr = "- ")
		{
			if (strings == null || strings.Count == 0)
			{
				return string.Empty;
			}
			int num = strings.Count * (bulletStr.Length + 1);
			for (int i = 0; i < strings.Count; i++)
			{
				num += strings[i].Length;
			}
			StringBuilder stringBuilder = new StringBuilder(num);
			GTStr.Bullet(stringBuilder, strings, bulletStr);
			return stringBuilder.ToString();
		}
	}
}
