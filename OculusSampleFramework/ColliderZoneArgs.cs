using System;

namespace OculusSampleFramework
{
	// Token: 0x02000BD1 RID: 3025
	public class ColliderZoneArgs : EventArgs
	{
		// Token: 0x06004AAE RID: 19118 RVA: 0x00163A1F File Offset: 0x00161C1F
		public ColliderZoneArgs(ColliderZone collider, float frameTime, InteractableTool collidingTool, InteractionType interactionType)
		{
			this.Collider = collider;
			this.FrameTime = frameTime;
			this.CollidingTool = collidingTool;
			this.InteractionT = interactionType;
		}

		// Token: 0x04004D65 RID: 19813
		public readonly ColliderZone Collider;

		// Token: 0x04004D66 RID: 19814
		public readonly float FrameTime;

		// Token: 0x04004D67 RID: 19815
		public readonly InteractableTool CollidingTool;

		// Token: 0x04004D68 RID: 19816
		public readonly InteractionType InteractionT;
	}
}
