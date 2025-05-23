using System;
using System.ComponentModel;
using UnityEngine;

// Token: 0x0200022A RID: 554
public static class UnityTagsExt
{
	// Token: 0x06000CD3 RID: 3283 RVA: 0x000430C8 File Offset: 0x000412C8
	public static UnityTag ToTag(this string s)
	{
		if (string.IsNullOrWhiteSpace(s))
		{
			return UnityTag.Invalid;
		}
		UnityTag unityTag;
		if (!UnityTags.StringToTag.TryGetValue(s, out unityTag))
		{
			return UnityTag.Invalid;
		}
		return unityTag;
	}

	// Token: 0x06000CD4 RID: 3284 RVA: 0x000430F1 File Offset: 0x000412F1
	public static void SetTag(this Component c, UnityTag tag)
	{
		if (c == null)
		{
			return;
		}
		if (tag == UnityTag.Invalid)
		{
			throw new InvalidEnumArgumentException("tag");
		}
		c.tag = UnityTags.StringValues[(int)tag];
	}

	// Token: 0x06000CD5 RID: 3285 RVA: 0x00043119 File Offset: 0x00041319
	public static void SetTag(this GameObject g, UnityTag tag)
	{
		if (g == null)
		{
			return;
		}
		if (tag == UnityTag.Invalid)
		{
			throw new InvalidEnumArgumentException("tag");
		}
		g.tag = UnityTags.StringValues[(int)tag];
	}

	// Token: 0x06000CD6 RID: 3286 RVA: 0x00043141 File Offset: 0x00041341
	public static bool TryGetTag(this GameObject g, out UnityTag tag)
	{
		tag = UnityTag.Invalid;
		return !(g == null) && UnityTags.StringToTag.TryGetValue(g.tag, out tag);
	}

	// Token: 0x06000CD7 RID: 3287 RVA: 0x00043162 File Offset: 0x00041362
	public static bool TryGetTag(this Component c, out UnityTag tag)
	{
		tag = UnityTag.Invalid;
		return !(c == null) && UnityTags.StringToTag.TryGetValue(c.tag, out tag);
	}

	// Token: 0x06000CD8 RID: 3288 RVA: 0x00043183 File Offset: 0x00041383
	public static bool CompareTag(this GameObject g, UnityTag tag)
	{
		if (g == null)
		{
			return false;
		}
		if (tag == UnityTag.Invalid)
		{
			throw new InvalidEnumArgumentException("tag");
		}
		return g.CompareTag(UnityTags.StringValues[(int)tag]);
	}

	// Token: 0x06000CD9 RID: 3289 RVA: 0x000431AC File Offset: 0x000413AC
	public static bool CompareTag(this Component c, UnityTag tag)
	{
		if (c == null)
		{
			return false;
		}
		if (tag == UnityTag.Invalid)
		{
			throw new InvalidEnumArgumentException("tag");
		}
		return c.CompareTag(UnityTags.StringValues[(int)tag]);
	}
}
