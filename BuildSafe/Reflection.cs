using System;
using System.Linq;
using System.Reflection;

namespace BuildSafe
{
	// Token: 0x02000BB1 RID: 2993
	public static class Reflection<T>
	{
		// Token: 0x17000722 RID: 1826
		// (get) Token: 0x06004A1E RID: 18974 RVA: 0x00161A20 File Offset: 0x0015FC20
		public static Type Type { get; } = typeof(T);

		// Token: 0x17000723 RID: 1827
		// (get) Token: 0x06004A1F RID: 18975 RVA: 0x00161A27 File Offset: 0x0015FC27
		public static EventInfo[] Events
		{
			get
			{
				return Reflection<T>.PreFetchEvents();
			}
		}

		// Token: 0x17000724 RID: 1828
		// (get) Token: 0x06004A20 RID: 18976 RVA: 0x00161A2E File Offset: 0x0015FC2E
		public static MethodInfo[] Methods
		{
			get
			{
				return Reflection<T>.PreFetchMethods();
			}
		}

		// Token: 0x17000725 RID: 1829
		// (get) Token: 0x06004A21 RID: 18977 RVA: 0x00161A35 File Offset: 0x0015FC35
		public static FieldInfo[] Fields
		{
			get
			{
				return Reflection<T>.PreFetchFields();
			}
		}

		// Token: 0x17000726 RID: 1830
		// (get) Token: 0x06004A22 RID: 18978 RVA: 0x00161A3C File Offset: 0x0015FC3C
		public static PropertyInfo[] Properties
		{
			get
			{
				return Reflection<T>.PreFetchProperties();
			}
		}

		// Token: 0x06004A23 RID: 18979 RVA: 0x00161A43 File Offset: 0x0015FC43
		private static EventInfo[] PreFetchEvents()
		{
			if (Reflection<T>.gEventsCache != null)
			{
				return Reflection<T>.gEventsCache;
			}
			return Reflection<T>.gEventsCache = Reflection<T>.Type.GetRuntimeEvents().ToArray<EventInfo>();
		}

		// Token: 0x06004A24 RID: 18980 RVA: 0x00161A67 File Offset: 0x0015FC67
		private static PropertyInfo[] PreFetchProperties()
		{
			if (Reflection<T>.gPropertiesCache != null)
			{
				return Reflection<T>.gPropertiesCache;
			}
			return Reflection<T>.gPropertiesCache = Reflection<T>.Type.GetRuntimeProperties().ToArray<PropertyInfo>();
		}

		// Token: 0x06004A25 RID: 18981 RVA: 0x00161A8B File Offset: 0x0015FC8B
		private static MethodInfo[] PreFetchMethods()
		{
			if (Reflection<T>.gMethodsCache != null)
			{
				return Reflection<T>.gMethodsCache;
			}
			return Reflection<T>.gMethodsCache = Reflection<T>.Type.GetRuntimeMethods().ToArray<MethodInfo>();
		}

		// Token: 0x06004A26 RID: 18982 RVA: 0x00161AAF File Offset: 0x0015FCAF
		private static FieldInfo[] PreFetchFields()
		{
			if (Reflection<T>.gFieldsCache != null)
			{
				return Reflection<T>.gFieldsCache;
			}
			return Reflection<T>.gFieldsCache = Reflection<T>.Type.GetRuntimeFields().ToArray<FieldInfo>();
		}

		// Token: 0x04004CFE RID: 19710
		private static Type gCachedType;

		// Token: 0x04004CFF RID: 19711
		private static MethodInfo[] gMethodsCache;

		// Token: 0x04004D00 RID: 19712
		private static FieldInfo[] gFieldsCache;

		// Token: 0x04004D01 RID: 19713
		private static PropertyInfo[] gPropertiesCache;

		// Token: 0x04004D02 RID: 19714
		private static EventInfo[] gEventsCache;
	}
}
