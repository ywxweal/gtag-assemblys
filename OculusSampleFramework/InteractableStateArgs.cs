using System;

namespace OculusSampleFramework
{
	// Token: 0x02000BDA RID: 3034
	public class InteractableStateArgs : EventArgs
	{
		// Token: 0x06004AE5 RID: 19173 RVA: 0x001641F7 File Offset: 0x001623F7
		public InteractableStateArgs(Interactable interactable, InteractableTool tool, InteractableState newInteractableState, InteractableState oldState, ColliderZoneArgs colliderArgs)
		{
			this.Interactable = interactable;
			this.Tool = tool;
			this.NewInteractableState = newInteractableState;
			this.OldInteractableState = oldState;
			this.ColliderArgs = colliderArgs;
		}

		// Token: 0x04004D95 RID: 19861
		public readonly Interactable Interactable;

		// Token: 0x04004D96 RID: 19862
		public readonly InteractableTool Tool;

		// Token: 0x04004D97 RID: 19863
		public readonly InteractableState OldInteractableState;

		// Token: 0x04004D98 RID: 19864
		public readonly InteractableState NewInteractableState;

		// Token: 0x04004D99 RID: 19865
		public readonly ColliderZoneArgs ColliderArgs;
	}
}
