using System;

namespace Utilities
{
	// Token: 0x02000AB6 RID: 2742
	public static class PathUtils
	{
		// Token: 0x06004223 RID: 16931 RVA: 0x0013196C File Offset: 0x0012FB6C
		public static string Resolve(params string[] subPaths)
		{
			if (subPaths == null || subPaths.Length == 0)
			{
				return null;
			}
			string[] array = string.Concat(subPaths).Split(PathUtils.kPathSeps, StringSplitOptions.RemoveEmptyEntries);
			return Uri.UnescapeDataString(new Uri(string.Join("/", array)).AbsolutePath);
		}

		// Token: 0x0400448F RID: 17551
		private static readonly char[] kPathSeps = new char[] { '\\', '/' };

		// Token: 0x04004490 RID: 17552
		private const string kFwdSlash = "/";
	}
}
