using System;

// Token: 0x020009EC RID: 2540
public class InvalidType : ProxyType
{
	// Token: 0x170005ED RID: 1517
	// (get) Token: 0x06003CB5 RID: 15541 RVA: 0x001213C9 File Offset: 0x0011F5C9
	public override string Name
	{
		get
		{
			return this._self.Name;
		}
	}

	// Token: 0x170005EE RID: 1518
	// (get) Token: 0x06003CB6 RID: 15542 RVA: 0x001213D6 File Offset: 0x0011F5D6
	public override string FullName
	{
		get
		{
			return this._self.FullName;
		}
	}

	// Token: 0x170005EF RID: 1519
	// (get) Token: 0x06003CB7 RID: 15543 RVA: 0x001213E3 File Offset: 0x0011F5E3
	public override string AssemblyQualifiedName
	{
		get
		{
			return this._self.AssemblyQualifiedName;
		}
	}

	// Token: 0x04004089 RID: 16521
	private Type _self = typeof(InvalidType);
}
