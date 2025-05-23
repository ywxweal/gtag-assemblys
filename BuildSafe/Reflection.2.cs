using System;
using System.Linq;
using System.Reflection;

namespace BuildSafe
{
	// Token: 0x02000BB2 RID: 2994
	public static class Reflection
	{
		// Token: 0x17000727 RID: 1831
		// (get) Token: 0x06004A29 RID: 18985 RVA: 0x00161BBC File Offset: 0x0015FDBC
		public static Assembly[] AllAssemblies
		{
			get
			{
				return Reflection.PreFetchAllAssemblies();
			}
		}

		// Token: 0x17000728 RID: 1832
		// (get) Token: 0x06004A2A RID: 18986 RVA: 0x00161BC3 File Offset: 0x0015FDC3
		public static Type[] AllTypes
		{
			get
			{
				return Reflection.PreFetchAllTypes();
			}
		}

		// Token: 0x06004A2B RID: 18987 RVA: 0x00161BCA File Offset: 0x0015FDCA
		static Reflection()
		{
			Reflection.PreFetchAllAssemblies();
			Reflection.PreFetchAllTypes();
		}

		// Token: 0x06004A2C RID: 18988 RVA: 0x00161BD8 File Offset: 0x0015FDD8
		private static Assembly[] PreFetchAllAssemblies()
		{
			if (Reflection.gAssemblyCache != null)
			{
				return Reflection.gAssemblyCache;
			}
			return Reflection.gAssemblyCache = (from a in AppDomain.CurrentDomain.GetAssemblies()
				where a != null
				select a).ToArray<Assembly>();
		}

		// Token: 0x06004A2D RID: 18989 RVA: 0x00161C2C File Offset: 0x0015FE2C
		private static Type[] PreFetchAllTypes()
		{
			if (Reflection.gTypeCache != null)
			{
				return Reflection.gTypeCache;
			}
			return Reflection.gTypeCache = (from t in Reflection.PreFetchAllAssemblies().SelectMany((Assembly a) => a.GetTypes())
				where t != null
				select t).ToArray<Type>();
		}

		// Token: 0x06004A2E RID: 18990 RVA: 0x00161CA0 File Offset: 0x0015FEA0
		public static MethodInfo[] GetMethodsWithAttribute<T>() where T : Attribute
		{
			return (from m in Reflection.AllTypes.SelectMany((Type t) => t.GetRuntimeMethods())
				where m.GetCustomAttributes(typeof(T), false).Length != 0
				select m).ToArray<MethodInfo>();
		}

		// Token: 0x04004D05 RID: 19717
		private static Assembly[] gAssemblyCache;

		// Token: 0x04004D06 RID: 19718
		private static Type[] gTypeCache;
	}
}
