using System;
using System.Globalization;
using System.Reflection;

// Token: 0x020009ED RID: 2541
public class ProxyType : Type
{
	// Token: 0x06003CB9 RID: 15545 RVA: 0x00121408 File Offset: 0x0011F608
	public ProxyType()
	{
	}

	// Token: 0x06003CBA RID: 15546 RVA: 0x00121420 File Offset: 0x0011F620
	public ProxyType(string typeName)
	{
		this._typeName = typeName;
	}

	// Token: 0x170005F0 RID: 1520
	// (get) Token: 0x06003CBB RID: 15547 RVA: 0x0012143F File Offset: 0x0011F63F
	public override string Name
	{
		get
		{
			return this._typeName;
		}
	}

	// Token: 0x170005F1 RID: 1521
	// (get) Token: 0x06003CBC RID: 15548 RVA: 0x00121447 File Offset: 0x0011F647
	public override string FullName
	{
		get
		{
			return ProxyType.kPrefix + this._typeName;
		}
	}

	// Token: 0x06003CBD RID: 15549 RVA: 0x0012145C File Offset: 0x0011F65C
	public static ProxyType Parse(string input)
	{
		if (string.IsNullOrWhiteSpace(input))
		{
			throw new ArgumentNullException("input");
		}
		input = input.Trim();
		if (!input.Contains(ProxyType.kPrefix, StringComparison.InvariantCultureIgnoreCase))
		{
			return ProxyType.kInvalidType;
		}
		if (!input.StartsWith(ProxyType.kPrefix, StringComparison.InvariantCultureIgnoreCase))
		{
			return ProxyType.kInvalidType;
		}
		if (input.Contains(','))
		{
			input = input.Split(',', StringSplitOptions.None)[0];
		}
		string text = input.Split('.', StringSplitOptions.None)[1].Trim();
		if (string.IsNullOrWhiteSpace(text))
		{
			return ProxyType.kInvalidType;
		}
		return new ProxyType(text);
	}

	// Token: 0x06003CBE RID: 15550 RVA: 0x001214E8 File Offset: 0x0011F6E8
	public override string ToString()
	{
		return base.ToString() + "." + this._typeName;
	}

	// Token: 0x06003CBF RID: 15551 RVA: 0x00121500 File Offset: 0x0011F700
	public override object[] GetCustomAttributes(bool inherit)
	{
		return this._self.GetCustomAttributes(inherit);
	}

	// Token: 0x06003CC0 RID: 15552 RVA: 0x0012150E File Offset: 0x0011F70E
	public override object[] GetCustomAttributes(Type attributeType, bool inherit)
	{
		return this._self.GetCustomAttributes(attributeType, inherit);
	}

	// Token: 0x06003CC1 RID: 15553 RVA: 0x0012151D File Offset: 0x0011F71D
	public override bool IsDefined(Type attributeType, bool inherit)
	{
		return this._self.IsDefined(attributeType, inherit);
	}

	// Token: 0x170005F2 RID: 1522
	// (get) Token: 0x06003CC2 RID: 15554 RVA: 0x0012152C File Offset: 0x0011F72C
	public override Module Module
	{
		get
		{
			return this._self.Module;
		}
	}

	// Token: 0x170005F3 RID: 1523
	// (get) Token: 0x06003CC3 RID: 15555 RVA: 0x00121539 File Offset: 0x0011F739
	public override string Namespace
	{
		get
		{
			return this._self.Namespace;
		}
	}

	// Token: 0x06003CC4 RID: 15556 RVA: 0x00002076 File Offset: 0x00000276
	protected override TypeAttributes GetAttributeFlagsImpl()
	{
		return TypeAttributes.NotPublic;
	}

	// Token: 0x06003CC5 RID: 15557 RVA: 0x00045F91 File Offset: 0x00044191
	protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
	{
		return null;
	}

	// Token: 0x06003CC6 RID: 15558 RVA: 0x00121546 File Offset: 0x0011F746
	public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
	{
		return this._self.GetConstructors(bindingAttr);
	}

	// Token: 0x06003CC7 RID: 15559 RVA: 0x00121554 File Offset: 0x0011F754
	public override Type GetElementType()
	{
		return this._self.GetElementType();
	}

	// Token: 0x06003CC8 RID: 15560 RVA: 0x00121561 File Offset: 0x0011F761
	public override EventInfo GetEvent(string name, BindingFlags bindingAttr)
	{
		return this._self.GetEvent(name, bindingAttr);
	}

	// Token: 0x06003CC9 RID: 15561 RVA: 0x00121570 File Offset: 0x0011F770
	public override EventInfo[] GetEvents(BindingFlags bindingAttr)
	{
		return this._self.GetEvents(bindingAttr);
	}

	// Token: 0x06003CCA RID: 15562 RVA: 0x0012157E File Offset: 0x0011F77E
	public override FieldInfo GetField(string name, BindingFlags bindingAttr)
	{
		return this._self.GetField(name, bindingAttr);
	}

	// Token: 0x06003CCB RID: 15563 RVA: 0x0012158D File Offset: 0x0011F78D
	public override FieldInfo[] GetFields(BindingFlags bindingAttr)
	{
		return this._self.GetFields(bindingAttr);
	}

	// Token: 0x06003CCC RID: 15564 RVA: 0x0012159B File Offset: 0x0011F79B
	public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
	{
		return this._self.GetMembers(bindingAttr);
	}

	// Token: 0x06003CCD RID: 15565 RVA: 0x00045F91 File Offset: 0x00044191
	protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
	{
		return null;
	}

	// Token: 0x06003CCE RID: 15566 RVA: 0x001215A9 File Offset: 0x0011F7A9
	public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
	{
		return this._self.GetMethods(bindingAttr);
	}

	// Token: 0x06003CCF RID: 15567 RVA: 0x001215B7 File Offset: 0x0011F7B7
	public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
	{
		return this._self.GetProperties(bindingAttr);
	}

	// Token: 0x06003CD0 RID: 15568 RVA: 0x001215C8 File Offset: 0x0011F7C8
	public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
	{
		return this._self.InvokeMember(name, invokeAttr, binder, target, args, modifiers, culture, namedParameters);
	}

	// Token: 0x170005F4 RID: 1524
	// (get) Token: 0x06003CD1 RID: 15569 RVA: 0x001215ED File Offset: 0x0011F7ED
	public override Type UnderlyingSystemType
	{
		get
		{
			return this._self.UnderlyingSystemType;
		}
	}

	// Token: 0x06003CD2 RID: 15570 RVA: 0x00002076 File Offset: 0x00000276
	protected override bool IsArrayImpl()
	{
		return false;
	}

	// Token: 0x06003CD3 RID: 15571 RVA: 0x00002076 File Offset: 0x00000276
	protected override bool IsByRefImpl()
	{
		return false;
	}

	// Token: 0x06003CD4 RID: 15572 RVA: 0x00002076 File Offset: 0x00000276
	protected override bool IsCOMObjectImpl()
	{
		return false;
	}

	// Token: 0x06003CD5 RID: 15573 RVA: 0x00002076 File Offset: 0x00000276
	protected override bool IsPointerImpl()
	{
		return false;
	}

	// Token: 0x06003CD6 RID: 15574 RVA: 0x00002076 File Offset: 0x00000276
	protected override bool IsPrimitiveImpl()
	{
		return false;
	}

	// Token: 0x170005F5 RID: 1525
	// (get) Token: 0x06003CD7 RID: 15575 RVA: 0x001215FA File Offset: 0x0011F7FA
	public override Assembly Assembly
	{
		get
		{
			return this._self.Assembly;
		}
	}

	// Token: 0x170005F6 RID: 1526
	// (get) Token: 0x06003CD8 RID: 15576 RVA: 0x00121607 File Offset: 0x0011F807
	public override string AssemblyQualifiedName
	{
		get
		{
			return this._self.AssemblyQualifiedName.Replace("ProxyType", this.FullName);
		}
	}

	// Token: 0x170005F7 RID: 1527
	// (get) Token: 0x06003CD9 RID: 15577 RVA: 0x00121624 File Offset: 0x0011F824
	public override Type BaseType
	{
		get
		{
			return this._self.BaseType;
		}
	}

	// Token: 0x170005F8 RID: 1528
	// (get) Token: 0x06003CDA RID: 15578 RVA: 0x00121631 File Offset: 0x0011F831
	public override Guid GUID
	{
		get
		{
			return this._self.GUID;
		}
	}

	// Token: 0x06003CDB RID: 15579 RVA: 0x00045F91 File Offset: 0x00044191
	protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
	{
		return null;
	}

	// Token: 0x06003CDC RID: 15580 RVA: 0x00002076 File Offset: 0x00000276
	protected override bool HasElementTypeImpl()
	{
		return false;
	}

	// Token: 0x06003CDD RID: 15581 RVA: 0x0012163E File Offset: 0x0011F83E
	public override Type GetNestedType(string name, BindingFlags bindingAttr)
	{
		return this._self.GetNestedType(name, bindingAttr);
	}

	// Token: 0x06003CDE RID: 15582 RVA: 0x0012164D File Offset: 0x0011F84D
	public override Type[] GetNestedTypes(BindingFlags bindingAttr)
	{
		return this._self.GetNestedTypes(bindingAttr);
	}

	// Token: 0x06003CDF RID: 15583 RVA: 0x0012165B File Offset: 0x0011F85B
	public override Type GetInterface(string name, bool ignoreCase)
	{
		return this._self.GetInterface(name, ignoreCase);
	}

	// Token: 0x06003CE0 RID: 15584 RVA: 0x0012166A File Offset: 0x0011F86A
	public override Type[] GetInterfaces()
	{
		return this._self.GetInterfaces();
	}

	// Token: 0x0400408A RID: 16522
	private Type _self = typeof(ProxyType);

	// Token: 0x0400408B RID: 16523
	private readonly string _typeName;

	// Token: 0x0400408C RID: 16524
	private static readonly string kPrefix = "ProxyType.";

	// Token: 0x0400408D RID: 16525
	private static InvalidType kInvalidType = new InvalidType();
}
