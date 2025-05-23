using System;

// Token: 0x020009EC RID: 2540
public class InvalidType : ProxyType
{
	// Token: 0x170005ED RID: 1517
	// (get) Token: 0x06003CB6 RID: 15542 RVA: 0x001214A1 File Offset: 0x0011F6A1
	public override string Name
	{
		get
		{
			return this._self.Name;
		}
	}

	// Token: 0x170005EE RID: 1518
	// (get) Token: 0x06003CB7 RID: 15543 RVA: 0x001214AE File Offset: 0x0011F6AE
	public override string FullName
	{
		get
		{
			return this._self.FullName;
		}
	}

	// Token: 0x170005EF RID: 1519
	// (get) Token: 0x06003CB8 RID: 15544 RVA: 0x001214BB File Offset: 0x0011F6BB
	public override string AssemblyQualifiedName
	{
		get
		{
			return this._self.AssemblyQualifiedName;
		}
	}

	// Token: 0x0400408A RID: 16522
	private Type _self = typeof(InvalidType);
}
