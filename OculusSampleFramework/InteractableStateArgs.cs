using System;

namespace OculusSampleFramework
{
	// Token: 0x02000BDA RID: 3034
	public class InteractableStateArgs : EventArgs
	{
		// Token: 0x06004AE6 RID: 19174 RVA: 0x001642CF File Offset: 0x001624CF
		public InteractableStateArgs(Interactable interactable, InteractableTool tool, InteractableState newInteractableState, InteractableState oldState, ColliderZoneArgs colliderArgs)
		{
			this.Interactable = interactable;
			this.Tool = tool;
			this.NewInteractableState = newInteractableState;
			this.OldInteractableState = oldState;
			this.ColliderArgs = colliderArgs;
		}

		// Token: 0x04004D96 RID: 19862
		public readonly Interactable Interactable;

		// Token: 0x04004D97 RID: 19863
		public readonly InteractableTool Tool;

		// Token: 0x04004D98 RID: 19864
		public readonly InteractableState OldInteractableState;

		// Token: 0x04004D99 RID: 19865
		public readonly InteractableState NewInteractableState;

		// Token: 0x04004D9A RID: 19866
		public readonly ColliderZoneArgs ColliderArgs;
	}
}
