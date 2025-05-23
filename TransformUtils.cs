using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020009D1 RID: 2513
public static class TransformUtils
{
	// Token: 0x06003C29 RID: 15401 RVA: 0x0011FD10 File Offset: 0x0011DF10
	public static int ComputePathHashByInstance(Transform t)
	{
		if (t == null)
		{
			return 0;
		}
		int num = 0;
		Transform transform = t;
		while (transform != null)
		{
			num = StaticHash.Compute(num, transform.GetHashCode());
			transform = transform.parent;
		}
		return num;
	}

	// Token: 0x06003C2A RID: 15402 RVA: 0x0011FD4C File Offset: 0x0011DF4C
	public static Hash128 ComputePathHash(Transform t)
	{
		if (t == null)
		{
			return default(Hash128);
		}
		Hash128 hash = default(Hash128);
		Transform transform = t;
		while (transform != null)
		{
			Hash128 hash2 = Hash128.Compute(transform.name);
			HashUtilities.AppendHash(ref hash2, ref hash);
			transform = transform.parent;
		}
		return hash;
	}

	// Token: 0x06003C2B RID: 15403 RVA: 0x0011FDA0 File Offset: 0x0011DFA0
	public static string GetScenePath(Transform t)
	{
		if (t == null)
		{
			return null;
		}
		string text = t.name;
		Transform transform = t.parent;
		while (transform != null)
		{
			text = transform.name + "/" + text;
			transform = transform.parent;
		}
		return text;
	}

	// Token: 0x06003C2C RID: 15404 RVA: 0x0011FDEC File Offset: 0x0011DFEC
	public static string GetScenePathReverse(Transform t)
	{
		if (t == null)
		{
			return null;
		}
		string text = t.name;
		Transform transform = t.parent;
		Queue<string> queue = new Queue<string>(16);
		while (transform != null)
		{
			queue.Enqueue(transform.name);
			transform = transform.parent;
		}
		while (queue.Count > 0)
		{
			text = text + "/" + queue.Dequeue();
		}
		return text;
	}

	// Token: 0x0400404E RID: 16462
	private const string kFwdSlash = "/";
}
