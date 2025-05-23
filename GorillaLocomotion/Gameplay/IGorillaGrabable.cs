using System;
using UnityEngine;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x02000CE2 RID: 3298
	internal interface IGorillaGrabable
	{
		// Token: 0x17000847 RID: 2119
		// (get) Token: 0x060051CB RID: 20939
		string name { get; }

		// Token: 0x060051CC RID: 20940
		bool MomentaryGrabOnly();

		// Token: 0x060051CD RID: 20941
		bool CanBeGrabbed(GorillaGrabber grabber);

		// Token: 0x060051CE RID: 20942
		void OnGrabbed(GorillaGrabber grabber, out Transform grabbedTransform, out Vector3 localGrabbedPosition);

		// Token: 0x060051CF RID: 20943
		void OnGrabReleased(GorillaGrabber grabber);
	}
}
