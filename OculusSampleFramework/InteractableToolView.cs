using System;

namespace OculusSampleFramework
{
	// Token: 0x02000BE6 RID: 3046
	public interface InteractableToolView
	{
		// Token: 0x17000764 RID: 1892
		// (get) Token: 0x06004B35 RID: 19253
		InteractableTool InteractableTool { get; }

		// Token: 0x06004B36 RID: 19254
		void SetFocusedInteractable(Interactable interactable);

		// Token: 0x17000765 RID: 1893
		// (get) Token: 0x06004B37 RID: 19255
		// (set) Token: 0x06004B38 RID: 19256
		bool EnableState { get; set; }

		// Token: 0x17000766 RID: 1894
		// (get) Token: 0x06004B39 RID: 19257
		// (set) Token: 0x06004B3A RID: 19258
		bool ToolActivateState { get; set; }
	}
}
