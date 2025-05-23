using System;
using UnityEngine;

// Token: 0x02000274 RID: 628
[Serializable]
public class NativeSizeChangerSettings
{
	// Token: 0x1700016C RID: 364
	// (get) Token: 0x06000E7C RID: 3708 RVA: 0x00049145 File Offset: 0x00047345
	// (set) Token: 0x06000E7D RID: 3709 RVA: 0x0004914D File Offset: 0x0004734D
	public Vector3 WorldPosition
	{
		get
		{
			return this.worldPosition;
		}
		set
		{
			this.worldPosition = value;
		}
	}

	// Token: 0x1700016D RID: 365
	// (get) Token: 0x06000E7E RID: 3710 RVA: 0x00049156 File Offset: 0x00047356
	// (set) Token: 0x06000E7F RID: 3711 RVA: 0x0004915E File Offset: 0x0004735E
	public float ActivationTime
	{
		get
		{
			return this.activationTime;
		}
		set
		{
			this.activationTime = value;
		}
	}

	// Token: 0x040011AF RID: 4527
	public const float MinAllowedSize = 0.1f;

	// Token: 0x040011B0 RID: 4528
	public const float MaxAllowedSize = 10f;

	// Token: 0x040011B1 RID: 4529
	private Vector3 worldPosition;

	// Token: 0x040011B2 RID: 4530
	private float activationTime;

	// Token: 0x040011B3 RID: 4531
	[Range(0.1f, 10f)]
	public float playerSizeScale = 1f;

	// Token: 0x040011B4 RID: 4532
	public bool ExpireOnRoomJoin = true;

	// Token: 0x040011B5 RID: 4533
	public bool ExpireInWater = true;

	// Token: 0x040011B6 RID: 4534
	public float ExpireAfterSeconds;

	// Token: 0x040011B7 RID: 4535
	public float ExpireOnDistance;
}
