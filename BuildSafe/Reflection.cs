using System;
using System.Linq;
using System.Reflection;

namespace BuildSafe
{
	// Token: 0x02000BB1 RID: 2993
	public static class Reflection<T>
	{
		// Token: 0x17000722 RID: 1826
		// (get) Token: 0x06004A1F RID: 18975 RVA: 0x00161AF8 File Offset: 0x0015FCF8
		public static Type Type { get; } = typeof(T);

		// Token: 0x17000723 RID: 1827
		// (get) Token: 0x06004A20 RID: 18976 RVA: 0x00161AFF File Offset: 0x0015FCFF
		public static EventInfo[] Events
		{
			get
			{
				return Reflection<T>.PreFetchEvents();
			}
		}

		// Token: 0x17000724 RID: 1828
		// (get) Token: 0x06004A21 RID: 18977 RVA: 0x00161B06 File Offset: 0x0015FD06
		public static MethodInfo[] Methods
		{
			get
			{
				return Reflection<T>.PreFetchMethods();
			}
		}

		// Token: 0x17000725 RID: 1829
		// (get) Token: 0x06004A22 RID: 18978 RVA: 0x00161B0D File Offset: 0x0015FD0D
		public static FieldInfo[] Fields
		{
			get
			{
				return Reflection<T>.PreFetchFields();
			}
		}

		// Token: 0x17000726 RID: 1830
		// (get) Token: 0x06004A23 RID: 18979 RVA: 0x00161B14 File Offset: 0x0015FD14
		public static PropertyInfo[] Properties
		{
			get
			{
				return Reflection<T>.PreFetchProperties();
			}
		}

		// Token: 0x06004A24 RID: 18980 RVA: 0x00161B1B File Offset: 0x0015FD1B
		private static EventInfo[] PreFetchEvents()
		{
			if (Reflection<T>.gEventsCache != null)
			{
				return Reflection<T>.gEventsCache;
			}
			return Reflection<T>.gEventsCache = Reflection<T>.Type.GetRuntimeEvents().ToArray<EventInfo>();
		}

		// Token: 0x06004A25 RID: 18981 RVA: 0x00161B3F File Offset: 0x0015FD3F
		private static PropertyInfo[] PreFetchProperties()
		{
			if (Reflection<T>.gPropertiesCache != null)
			{
				return Reflection<T>.gPropertiesCache;
			}
			return Reflection<T>.gPropertiesCache = Reflection<T>.Type.GetRuntimeProperties().ToArray<PropertyInfo>();
		}

		// Token: 0x06004A26 RID: 18982 RVA: 0x00161B63 File Offset: 0x0015FD63
		private static MethodInfo[] PreFetchMethods()
		{
			if (Reflection<T>.gMethodsCache != null)
			{
				return Reflection<T>.gMethodsCache;
			}
			return Reflection<T>.gMethodsCache = Reflection<T>.Type.GetRuntimeMethods().ToArray<MethodInfo>();
		}

		// Token: 0x06004A27 RID: 18983 RVA: 0x00161B87 File Offset: 0x0015FD87
		private static FieldInfo[] PreFetchFields()
		{
			if (Reflection<T>.gFieldsCache != null)
			{
				return Reflection<T>.gFieldsCache;
			}
			return Reflection<T>.gFieldsCache = Reflection<T>.Type.GetRuntimeFields().ToArray<FieldInfo>();
		}

		// Token: 0x04004CFF RID: 19711
		private static Type gCachedType;

		// Token: 0x04004D00 RID: 19712
		private static MethodInfo[] gMethodsCache;

		// Token: 0x04004D01 RID: 19713
		private static FieldInfo[] gFieldsCache;

		// Token: 0x04004D02 RID: 19714
		private static PropertyInfo[] gPropertiesCache;

		// Token: 0x04004D03 RID: 19715
		private static EventInfo[] gEventsCache;
	}
}
