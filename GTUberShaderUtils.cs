using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000A26 RID: 2598
public static class GTUberShaderUtils
{
	// Token: 0x06003DDE RID: 15838 RVA: 0x0012552E File Offset: 0x0012372E
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void SetStencilComparison(this Material m, GTShaderStencilCompare cmp)
	{
		m.SetFloat(GTUberShaderUtils._StencilComparison, (float)cmp);
	}

	// Token: 0x06003DDF RID: 15839 RVA: 0x00125542 File Offset: 0x00123742
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void SetStencilPassFrontOp(this Material m, GTShaderStencilOp op)
	{
		m.SetFloat(GTUberShaderUtils._StencilPassFront, (float)op);
	}

	// Token: 0x06003DE0 RID: 15840 RVA: 0x00125556 File Offset: 0x00123756
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void SetStencilReferenceValue(this Material m, int value)
	{
		m.SetFloat(GTUberShaderUtils._StencilReference, (float)value);
	}

	// Token: 0x06003DE1 RID: 15841 RVA: 0x0012556C File Offset: 0x0012376C
	public static void SetVisibleToXRay(this Material m, bool visible, bool saveToDisk = false)
	{
		GTShaderStencilCompare gtshaderStencilCompare = (visible ? GTShaderStencilCompare.Equal : GTShaderStencilCompare.NotEqual);
		GTShaderStencilOp gtshaderStencilOp = (visible ? GTShaderStencilOp.Replace : GTShaderStencilOp.Keep);
		m.SetStencilComparison(gtshaderStencilCompare);
		m.SetStencilPassFrontOp(gtshaderStencilOp);
		m.SetStencilReferenceValue(7);
	}

	// Token: 0x06003DE2 RID: 15842 RVA: 0x001255A0 File Offset: 0x001237A0
	public static void SetRevealsXRay(this Material m, bool reveals, bool changeQueue = true, bool saveToDisk = false)
	{
		m.SetFloat(GTUberShaderUtils._ZWrite, (float)(reveals ? 0 : 1));
		m.SetFloat(GTUberShaderUtils._ColorMask_, (float)(reveals ? 0 : 14));
		m.SetStencilComparison(GTShaderStencilCompare.Disabled);
		m.SetStencilPassFrontOp(reveals ? GTShaderStencilOp.Replace : GTShaderStencilOp.Keep);
		m.SetStencilReferenceValue(reveals ? 7 : 0);
		if (changeQueue)
		{
			int renderQueue = m.renderQueue;
			m.renderQueue = renderQueue + (reveals ? (-1) : 1);
		}
	}

	// Token: 0x06003DE3 RID: 15843 RVA: 0x00125618 File Offset: 0x00123818
	public static int GetNearestRenderQueue(this Material m, out RenderQueue queue)
	{
		int renderQueue = m.renderQueue;
		int num = -1;
		int num2 = int.MaxValue;
		for (int i = 0; i < GTUberShaderUtils.kRenderQueueInts.Length; i++)
		{
			int num3 = GTUberShaderUtils.kRenderQueueInts[i];
			int num4 = Math.Abs(num3 - renderQueue);
			if (num2 > num4)
			{
				num = num3;
				num2 = num4;
			}
		}
		queue = (RenderQueue)num;
		return num;
	}

	// Token: 0x06003DE4 RID: 15844 RVA: 0x00125669 File Offset: 0x00123869
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void InitOnLoad()
	{
		GTUberShaderUtils.kUberShader = Shader.Find("GorillaTag/UberShader");
	}

	// Token: 0x040041FE RID: 16894
	private static Shader kUberShader;

	// Token: 0x040041FF RID: 16895
	private static readonly ShaderHashId _StencilComparison = "_StencilComparison";

	// Token: 0x04004200 RID: 16896
	private static readonly ShaderHashId _StencilPassFront = "_StencilPassFront";

	// Token: 0x04004201 RID: 16897
	private static readonly ShaderHashId _StencilReference = "_StencilReference";

	// Token: 0x04004202 RID: 16898
	private static readonly ShaderHashId _ColorMask_ = "_ColorMask_";

	// Token: 0x04004203 RID: 16899
	private static readonly ShaderHashId _ManualZWrite = "_ManualZWrite";

	// Token: 0x04004204 RID: 16900
	private static readonly ShaderHashId _ZWrite = "_ZWrite";

	// Token: 0x04004205 RID: 16901
	private static readonly int[] kRenderQueueInts = new int[] { 1000, 2000, 2450, 2500, 3000, 4000 };
}
