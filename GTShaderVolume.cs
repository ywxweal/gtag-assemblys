using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200024D RID: 589
[ExecuteAlways]
public class GTShaderVolume : MonoBehaviour
{
	// Token: 0x06000D5F RID: 3423 RVA: 0x00045D80 File Offset: 0x00043F80
	private void OnEnable()
	{
		if (GTShaderVolume.gVolumes.Count > 16)
		{
			return;
		}
		if (!GTShaderVolume.gVolumes.Contains(this))
		{
			GTShaderVolume.gVolumes.Add(this);
		}
	}

	// Token: 0x06000D60 RID: 3424 RVA: 0x00045DA9 File Offset: 0x00043FA9
	private void OnDisable()
	{
		GTShaderVolume.gVolumes.Remove(this);
	}

	// Token: 0x06000D61 RID: 3425 RVA: 0x00045DB8 File Offset: 0x00043FB8
	public static void SyncVolumeData()
	{
		m4x4 m4x = default(m4x4);
		int count = GTShaderVolume.gVolumes.Count;
		for (int i = 0; i < 16; i++)
		{
			if (i >= count)
			{
				MatrixUtils.Clear(ref GTShaderVolume.ShaderData[i]);
			}
			else
			{
				GTShaderVolume gtshaderVolume = GTShaderVolume.gVolumes[i];
				if (!gtshaderVolume)
				{
					MatrixUtils.Clear(ref GTShaderVolume.ShaderData[i]);
				}
				else
				{
					Transform transform = gtshaderVolume.transform;
					Vector4 vector = transform.position;
					Vector4 vector2 = transform.rotation.ToVector();
					Vector4 vector3 = transform.localScale;
					m4x.SetRow0(ref vector);
					m4x.SetRow1(ref vector2);
					m4x.SetRow2(ref vector3);
					m4x.Push(ref GTShaderVolume.ShaderData[i]);
				}
			}
		}
		Shader.SetGlobalInteger(GTShaderVolume._GT_ShaderVolumesActive, count);
		Shader.SetGlobalMatrixArray(GTShaderVolume._GT_ShaderVolumes, GTShaderVolume.ShaderData);
	}

	// Token: 0x040010F7 RID: 4343
	public const int MAX_VOLUMES = 16;

	// Token: 0x040010F8 RID: 4344
	private static Matrix4x4[] ShaderData = new Matrix4x4[16];

	// Token: 0x040010F9 RID: 4345
	[Space]
	private static List<GTShaderVolume> gVolumes = new List<GTShaderVolume>(16);

	// Token: 0x040010FA RID: 4346
	private static ShaderHashId _GT_ShaderVolumes = "_GT_ShaderVolumes";

	// Token: 0x040010FB RID: 4347
	private static ShaderHashId _GT_ShaderVolumesActive = "_GT_ShaderVolumesActive";
}
