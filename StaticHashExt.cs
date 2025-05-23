using System;

// Token: 0x020009CB RID: 2507
public static class StaticHashExt
{
	// Token: 0x06003C02 RID: 15362 RVA: 0x0011F4BD File Offset: 0x0011D6BD
	public static int GetStaticHash(this int i)
	{
		return StaticHash.Compute(i);
	}

	// Token: 0x06003C03 RID: 15363 RVA: 0x0011F4C5 File Offset: 0x0011D6C5
	public static int GetStaticHash(this uint u)
	{
		return StaticHash.Compute(u);
	}

	// Token: 0x06003C04 RID: 15364 RVA: 0x0011F4CD File Offset: 0x0011D6CD
	public static int GetStaticHash(this float f)
	{
		return StaticHash.Compute(f);
	}

	// Token: 0x06003C05 RID: 15365 RVA: 0x0011F4D5 File Offset: 0x0011D6D5
	public static int GetStaticHash(this long l)
	{
		return StaticHash.Compute(l);
	}

	// Token: 0x06003C06 RID: 15366 RVA: 0x0011F4DD File Offset: 0x0011D6DD
	public static int GetStaticHash(this double d)
	{
		return StaticHash.Compute(d);
	}

	// Token: 0x06003C07 RID: 15367 RVA: 0x0011F4E5 File Offset: 0x0011D6E5
	public static int GetStaticHash(this bool b)
	{
		return StaticHash.Compute(b);
	}

	// Token: 0x06003C08 RID: 15368 RVA: 0x0011F4ED File Offset: 0x0011D6ED
	public static int GetStaticHash(this DateTime dt)
	{
		return StaticHash.Compute(dt);
	}

	// Token: 0x06003C09 RID: 15369 RVA: 0x0011F4F5 File Offset: 0x0011D6F5
	public static int GetStaticHash(this string s)
	{
		return StaticHash.Compute(s);
	}

	// Token: 0x06003C0A RID: 15370 RVA: 0x0011F4FD File Offset: 0x0011D6FD
	public static int GetStaticHash(this byte[] bytes)
	{
		return StaticHash.Compute(bytes);
	}
}
