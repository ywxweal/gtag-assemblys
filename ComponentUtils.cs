using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000984 RID: 2436
public static class ComponentUtils
{
	// Token: 0x06003A8B RID: 14987 RVA: 0x00118518 File Offset: 0x00116718
	public static T EnsureComponent<T>(this Component ctx, ref T target) where T : Component
	{
		if (ctx.AsNull<Component>() == null)
		{
			return default(T);
		}
		if (target.AsNull<T>() != null)
		{
			return target;
		}
		return target = ctx.GetComponent<T>();
	}

	// Token: 0x06003A8C RID: 14988 RVA: 0x0011856B File Offset: 0x0011676B
	public static bool TryEnsureComponent<T>(this Component ctx, ref T target) where T : Component
	{
		if (ctx.AsNull<Component>() == null)
		{
			return false;
		}
		if (target.AsNull<T>() != null)
		{
			return true;
		}
		target = ctx.GetComponent<T>();
		return true;
	}

	// Token: 0x06003A8D RID: 14989 RVA: 0x001185A4 File Offset: 0x001167A4
	public static T AddComponent<T>(this Component c) where T : Component
	{
		return c.gameObject.AddComponent<T>();
	}

	// Token: 0x06003A8E RID: 14990 RVA: 0x001185B1 File Offset: 0x001167B1
	public static void GetOrAddComponent<T>(this Component c, out T result) where T : Component
	{
		if (!c.TryGetComponent<T>(out result))
		{
			result = c.gameObject.AddComponent<T>();
		}
	}

	// Token: 0x06003A8F RID: 14991 RVA: 0x001185CD File Offset: 0x001167CD
	public static bool GetComponentAndSetFieldIfNullElseLogAndDisable<T>(this Behaviour c, ref T fieldRef, string fieldName, string fieldTypeName, string msgSuffix = "Disabling.", [CallerMemberName] string caller = "__UNKNOWN_CALLER__") where T : Component
	{
		if (c.GetComponentAndSetFieldIfNullElseLog(ref fieldRef, fieldName, fieldTypeName, msgSuffix, caller))
		{
			return true;
		}
		c.enabled = false;
		return false;
	}

	// Token: 0x06003A90 RID: 14992 RVA: 0x001185E8 File Offset: 0x001167E8
	public static bool GetComponentAndSetFieldIfNullElseLog<T>(this Behaviour c, ref T fieldRef, string fieldName, string fieldTypeName, string msgSuffix = "", [CallerMemberName] string caller = "__UNKNOWN_CALLER__") where T : Component
	{
		if (fieldRef != null)
		{
			return true;
		}
		fieldRef = c.GetComponent<T>();
		if (fieldRef != null)
		{
			return true;
		}
		Debug.LogError(string.Concat(new string[] { caller, ": Could not find ", fieldTypeName, " \"", fieldName, "\" on \"", c.name, "\". ", msgSuffix }), c);
		return false;
	}

	// Token: 0x06003A91 RID: 14993 RVA: 0x00118679 File Offset: 0x00116879
	public static bool DisableIfNull<T>(this Behaviour c, T fieldRef, string fieldName, string fieldTypeName, [CallerMemberName] string caller = "__UNKNOWN_CALLER__") where T : Object
	{
		if (fieldRef != null)
		{
			return true;
		}
		c.enabled = false;
		return false;
	}

	// Token: 0x06003A92 RID: 14994 RVA: 0x00118693 File Offset: 0x00116893
	public static Hash128 ComputeStaticHash128(Component c, string k)
	{
		return ComponentUtils.ComputeStaticHash128(c, StaticHash.Compute(k));
	}

	// Token: 0x06003A93 RID: 14995 RVA: 0x001186A4 File Offset: 0x001168A4
	public static Hash128 ComputeStaticHash128(Component c, int k = 0)
	{
		if (c == null)
		{
			return default(Hash128);
		}
		Transform transform = c.transform;
		Component[] components = c.gameObject.GetComponents(typeof(Component));
		uint[] array = ComponentUtils.kHashBits;
		int siblingIndex = transform.GetSiblingIndex();
		int num = components.Length;
		int num2 = 0;
		while (num2 < num && c != components[num2])
		{
			num2++;
		}
		int num3 = StaticHash.Compute(k + 2, 1);
		int num4 = StaticHash.Compute(siblingIndex + 4, num3);
		int num5 = StaticHash.Compute(num + 8, num4);
		int num6 = StaticHash.Compute(num2 + 16, num5);
		array[0] = (uint)num3;
		array[1] = (uint)num4;
		array[2] = (uint)num5;
		array[3] = (uint)num6;
		SRand srand = new SRand(StaticHash.Compute(num3, num4, num5, num6));
		srand.Shuffle<uint>(array);
		Hash128 hash = new Hash128(array[0], array[1], array[2], array[3]);
		Hash128 hash2 = Hash128.Compute(c.GetType().FullName);
		Hash128 hash3 = TransformUtils.ComputePathHash(transform);
		Hash128 hash4 = transform.localToWorldMatrix.QuantizedHash128();
		HashUtilities.AppendHash(ref hash2, ref hash);
		HashUtilities.AppendHash(ref hash3, ref hash);
		HashUtilities.AppendHash(ref hash4, ref hash);
		return hash;
	}

	// Token: 0x04003F75 RID: 16245
	private static readonly uint[] kHashBits = new uint[4];
}
