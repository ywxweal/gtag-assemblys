using System;

namespace OculusSampleFramework
{
	// Token: 0x02000BE4 RID: 3044
	public class InteractableCollisionInfo
	{
		// Token: 0x06004B1D RID: 19229 RVA: 0x00164DD5 File Offset: 0x00162FD5
		public InteractableCollisionInfo(ColliderZone collider, InteractableCollisionDepth collisionDepth, InteractableTool collidingTool)
		{
			this.InteractableCollider = collider;
			this.CollisionDepth = collisionDepth;
			this.CollidingTool = collidingTool;
		}

		// Token: 0x04004DC9 RID: 19913
		public ColliderZone InteractableCollider;

		// Token: 0x04004DCA RID: 19914
		public InteractableCollisionDepth CollisionDepth;

		// Token: 0x04004DCB RID: 19915
		public InteractableTool CollidingTool;
	}
}
