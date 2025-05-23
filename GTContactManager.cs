using System;
using UnityEngine;

// Token: 0x020001CF RID: 463
public class GTContactManager : MonoBehaviour
{
	// Token: 0x06000AC6 RID: 2758 RVA: 0x000023F4 File Offset: 0x000005F4
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void InitializeOnLoad()
	{
	}

	// Token: 0x06000AC7 RID: 2759 RVA: 0x0003A614 File Offset: 0x00038814
	private static GTContactPoint[] InitContactPoints(int count)
	{
		GTContactPoint[] array = new GTContactPoint[count];
		for (int i = 0; i < count; i++)
		{
			array[i] = new GTContactPoint();
		}
		return array;
	}

	// Token: 0x06000AC8 RID: 2760 RVA: 0x0003A640 File Offset: 0x00038840
	public static void RaiseContact(Vector3 point, Vector3 normal)
	{
		if (GTContactManager.gNextFree == -1)
		{
			return;
		}
		float time = GTShaderGlobals.Time;
		GTContactPoint gtcontactPoint = GTContactManager._gContactPoints[GTContactManager.gNextFree];
		gtcontactPoint.contactPoint = point;
		gtcontactPoint.radius = 0.04f;
		gtcontactPoint.counterVelocity = normal;
		gtcontactPoint.timestamp = time;
		gtcontactPoint.lifetime = 2f;
		gtcontactPoint.color = GTContactManager.gRND.NextColor();
		gtcontactPoint.free = 0U;
	}

	// Token: 0x06000AC9 RID: 2761 RVA: 0x0003A6A8 File Offset: 0x000388A8
	public static void ProcessContacts()
	{
		Matrix4x4[] shaderData = GTContactManager.ShaderData;
		GTContactPoint[] gContactPoints = GTContactManager._gContactPoints;
		int frame = GTShaderGlobals.Frame;
		for (int i = 0; i < 32; i++)
		{
			GTContactManager.Transfer(ref gContactPoints[i].data, ref shaderData[i]);
		}
	}

	// Token: 0x06000ACA RID: 2762 RVA: 0x0003A6E8 File Offset: 0x000388E8
	private static void Transfer(ref Matrix4x4 from, ref Matrix4x4 to)
	{
		to.m00 = from.m00;
		to.m01 = from.m01;
		to.m02 = from.m02;
		to.m03 = from.m03;
		to.m10 = from.m10;
		to.m11 = from.m11;
		to.m12 = from.m12;
		to.m13 = from.m13;
		to.m20 = from.m20;
		to.m21 = from.m21;
		to.m22 = from.m22;
		to.m23 = from.m23;
		to.m30 = from.m30;
		to.m31 = from.m31;
		to.m32 = from.m32;
		to.m33 = from.m33;
	}

	// Token: 0x04000D52 RID: 3410
	public const int MAX_CONTACTS = 32;

	// Token: 0x04000D53 RID: 3411
	public static Matrix4x4[] ShaderData = new Matrix4x4[32];

	// Token: 0x04000D54 RID: 3412
	private static GTContactPoint[] _gContactPoints = GTContactManager.InitContactPoints(32);

	// Token: 0x04000D55 RID: 3413
	private static int gNextFree = 0;

	// Token: 0x04000D56 RID: 3414
	private static SRand gRND = new SRand(DateTime.UtcNow);
}
