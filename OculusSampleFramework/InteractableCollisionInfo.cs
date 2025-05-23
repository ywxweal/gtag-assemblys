using System;

namespace OculusSampleFramework
{
	// Token: 0x02000BE4 RID: 3044
	public class InteractableCollisionInfo
	{
		// Token: 0x06004B1E RID: 19230 RVA: 0x00164EAD File Offset: 0x001630AD
		public InteractableCollisionInfo(ColliderZone collider, InteractableCollisionDepth collisionDepth, InteractableTool collidingTool)
		{
			this.InteractableCollider = collider;
			this.CollisionDepth = collisionDepth;
			this.CollidingTool = collidingTool;
		}

		// Token: 0x04004DCA RID: 19914
		public ColliderZone InteractableCollider;

		// Token: 0x04004DCB RID: 19915
		public InteractableCollisionDepth CollisionDepth;

		// Token: 0x04004DCC RID: 19916
		public InteractableTool CollidingTool;
	}
}
