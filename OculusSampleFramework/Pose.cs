using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000BFE RID: 3070
	public class Pose
	{
		// Token: 0x06004BE0 RID: 19424 RVA: 0x00167A36 File Offset: 0x00165C36
		public Pose()
		{
			this.Position = Vector3.zero;
			this.Rotation = Quaternion.identity;
		}

		// Token: 0x06004BE1 RID: 19425 RVA: 0x00167A54 File Offset: 0x00165C54
		public Pose(Vector3 position, Quaternion rotation)
		{
			this.Position = position;
			this.Rotation = rotation;
		}

		// Token: 0x04004E94 RID: 20116
		public Vector3 Position;

		// Token: 0x04004E95 RID: 20117
		public Quaternion Rotation;
	}
}
