using System;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000DE7 RID: 3559
	public interface IProjectile
	{
		// Token: 0x06005821 RID: 22561
		void Launch(Vector3 startPosition, Quaternion startRotation, Vector3 velocity, float scale);
	}
}
