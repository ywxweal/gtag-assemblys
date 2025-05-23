using System;
using UnityEngine;

// Token: 0x020001D0 RID: 464
public class GTShaderGlobals : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x17000117 RID: 279
	// (get) Token: 0x06000ACD RID: 2765 RVA: 0x0003A7E4 File Offset: 0x000389E4
	public static Vector3 WorldSpaceCameraPos
	{
		get
		{
			return GTShaderGlobals.gMainCameraWorldPos;
		}
	}

	// Token: 0x17000118 RID: 280
	// (get) Token: 0x06000ACE RID: 2766 RVA: 0x0003A7EB File Offset: 0x000389EB
	public static float Time
	{
		get
		{
			return GTShaderGlobals.gTime;
		}
	}

	// Token: 0x17000119 RID: 281
	// (get) Token: 0x06000ACF RID: 2767 RVA: 0x0003A7F2 File Offset: 0x000389F2
	public static int Frame
	{
		get
		{
			return GTShaderGlobals.gIFrame;
		}
	}

	// Token: 0x06000AD0 RID: 2768 RVA: 0x0003A7F9 File Offset: 0x000389F9
	private void Awake()
	{
		GTShaderGlobals.gMainCamera = Camera.main;
		if (GTShaderGlobals.gMainCamera)
		{
			GTShaderGlobals.gMainCameraXform = GTShaderGlobals.gMainCamera.transform;
			GTShaderGlobals.gMainCameraWorldPos = GTShaderGlobals.gMainCameraXform.position;
		}
		this.SliceUpdate();
	}

	// Token: 0x06000AD1 RID: 2769 RVA: 0x0003A835 File Offset: 0x00038A35
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void Initialize()
	{
		GTShaderGlobals.InitBlueNoiseTex();
	}

	// Token: 0x06000AD2 RID: 2770 RVA: 0x00017251 File Offset: 0x00015451
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06000AD3 RID: 2771 RVA: 0x0001725A File Offset: 0x0001545A
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06000AD4 RID: 2772 RVA: 0x0003A83C File Offset: 0x00038A3C
	public void SliceUpdate()
	{
		GTShaderGlobals.UpdateTime();
		GTShaderGlobals.UpdateFrame();
		GTShaderGlobals.UpdateCamera();
	}

	// Token: 0x06000AD5 RID: 2773 RVA: 0x0003A84D File Offset: 0x00038A4D
	private static void UpdateFrame()
	{
		GTShaderGlobals.gIFrame = global::UnityEngine.Time.frameCount;
		Shader.SetGlobalInteger(GTShaderGlobals._GT_iFrame, GTShaderGlobals.gIFrame);
	}

	// Token: 0x06000AD6 RID: 2774 RVA: 0x0003A86D File Offset: 0x00038A6D
	private static void UpdateCamera()
	{
		if (!GTShaderGlobals.gMainCameraXform)
		{
			return;
		}
		GTShaderGlobals.gMainCameraWorldPos = GTShaderGlobals.gMainCameraXform.position;
		Shader.SetGlobalVector(GTShaderGlobals._GT_WorldSpaceCameraPos, GTShaderGlobals.gMainCameraWorldPos);
	}

	// Token: 0x06000AD7 RID: 2775 RVA: 0x0003A8A4 File Offset: 0x00038AA4
	private static void UpdateTime()
	{
		GTShaderGlobals.gTime = (float)(DateTime.UtcNow - GTShaderGlobals.gStartTime).TotalSeconds;
		Shader.SetGlobalFloat(GTShaderGlobals._GT_Time, GTShaderGlobals.gTime);
	}

	// Token: 0x06000AD8 RID: 2776 RVA: 0x0003A8E2 File Offset: 0x00038AE2
	private static void UpdatePawns()
	{
		GTShaderGlobals.gActivePawns = GorillaPawn.ActiveCount;
		GorillaPawn.SyncPawnData();
		Shader.SetGlobalMatrixArray(GTShaderGlobals._GT_PawnData, GTShaderGlobals.gPawnData);
		Shader.SetGlobalInteger(GTShaderGlobals._GT_PawnActiveCount, GTShaderGlobals.gActivePawns);
	}

	// Token: 0x06000AD9 RID: 2777 RVA: 0x0003A91C File Offset: 0x00038B1C
	private static void InitBlueNoiseTex()
	{
		GTShaderGlobals.gBlueNoiseTex = Resources.Load<Texture2D>("Graphics/Textures/noise_blue_rgba_128");
		GTShaderGlobals.gBlueNoiseTexWH = GTShaderGlobals.gBlueNoiseTex.GetTexelSize();
		Shader.SetGlobalTexture(GTShaderGlobals._GT_BlueNoiseTex, GTShaderGlobals.gBlueNoiseTex);
		Shader.SetGlobalVector(GTShaderGlobals._GT_BlueNoiseTex_WH, GTShaderGlobals.gBlueNoiseTexWH);
	}

	// Token: 0x06000ADC RID: 2780 RVA: 0x00011040 File Offset: 0x0000F240
	bool IGorillaSliceableSimple.get_isActiveAndEnabled()
	{
		return base.isActiveAndEnabled;
	}

	// Token: 0x04000D57 RID: 3415
	private static Camera gMainCamera;

	// Token: 0x04000D58 RID: 3416
	private static Transform gMainCameraXform;

	// Token: 0x04000D59 RID: 3417
	private static Vector3 gMainCameraWorldPos;

	// Token: 0x04000D5A RID: 3418
	[Space]
	private static int gIFrame;

	// Token: 0x04000D5B RID: 3419
	private static float gTime;

	// Token: 0x04000D5C RID: 3420
	[Space]
	private static Texture2D gBlueNoiseTex;

	// Token: 0x04000D5D RID: 3421
	private static Vector4 gBlueNoiseTexWH;

	// Token: 0x04000D5E RID: 3422
	[Space]
	private static int gActivePawns;

	// Token: 0x04000D5F RID: 3423
	[Space]
	private static DateTime gStartTime = DateTime.Today.AddDays(-1.0).ToUniversalTime();

	// Token: 0x04000D60 RID: 3424
	private static Matrix4x4[] gPawnData = GorillaPawn.ShaderData;

	// Token: 0x04000D61 RID: 3425
	private static ShaderHashId _GT_WorldSpaceCameraPos = "_GT_WorldSpaceCameraPos";

	// Token: 0x04000D62 RID: 3426
	private static ShaderHashId _GT_BlueNoiseTex = "_GT_BlueNoiseTex";

	// Token: 0x04000D63 RID: 3427
	private static ShaderHashId _GT_BlueNoiseTex_WH = "_GT_BlueNoiseTex_WH";

	// Token: 0x04000D64 RID: 3428
	private static ShaderHashId _GT_iFrame = "_GT_iFrame";

	// Token: 0x04000D65 RID: 3429
	private static ShaderHashId _GT_Time = "_GT_Time";

	// Token: 0x04000D66 RID: 3430
	private static ShaderHashId _GT_PawnData = "_GT_PawnData";

	// Token: 0x04000D67 RID: 3431
	private static ShaderHashId _GT_PawnActiveCount = "_GT_PawnActiveCount";
}
