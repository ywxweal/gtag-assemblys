using System;

namespace OculusSampleFramework
{
	// Token: 0x02000BD1 RID: 3025
	public class ColliderZoneArgs : EventArgs
	{
		// Token: 0x06004AAF RID: 19119 RVA: 0x00163AF7 File Offset: 0x00161CF7
		public ColliderZoneArgs(ColliderZone collider, float frameTime, InteractableTool collidingTool, InteractionType interactionType)
		{
			this.Collider = collider;
			this.FrameTime = frameTime;
			this.CollidingTool = collidingTool;
			this.InteractionT = interactionType;
		}

		// Token: 0x04004D66 RID: 19814
		public readonly ColliderZone Collider;

		// Token: 0x04004D67 RID: 19815
		public readonly float FrameTime;

		// Token: 0x04004D68 RID: 19816
		public readonly InteractableTool CollidingTool;

		// Token: 0x04004D69 RID: 19817
		public readonly InteractionType InteractionT;
	}
}
