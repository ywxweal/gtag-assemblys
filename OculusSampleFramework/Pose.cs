using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000BFE RID: 3070
	public class Pose
	{
		// Token: 0x06004BE1 RID: 19425 RVA: 0x00167B0E File Offset: 0x00165D0E
		public Pose()
		{
			this.Position = Vector3.zero;
			this.Rotation = Quaternion.identity;
		}

		// Token: 0x06004BE2 RID: 19426 RVA: 0x00167B2C File Offset: 0x00165D2C
		public Pose(Vector3 position, Quaternion rotation)
		{
			this.Position = position;
			this.Rotation = rotation;
		}

		// Token: 0x04004E95 RID: 20117
		public Vector3 Position;

		// Token: 0x04004E96 RID: 20118
		public Quaternion Rotation;
	}
}
