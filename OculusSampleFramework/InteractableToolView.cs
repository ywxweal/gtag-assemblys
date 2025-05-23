using System;

namespace OculusSampleFramework
{
	// Token: 0x02000BE6 RID: 3046
	public interface InteractableToolView
	{
		// Token: 0x17000764 RID: 1892
		// (get) Token: 0x06004B34 RID: 19252
		InteractableTool InteractableTool { get; }

		// Token: 0x06004B35 RID: 19253
		void SetFocusedInteractable(Interactable interactable);

		// Token: 0x17000765 RID: 1893
		// (get) Token: 0x06004B36 RID: 19254
		// (set) Token: 0x06004B37 RID: 19255
		bool EnableState { get; set; }

		// Token: 0x17000766 RID: 1894
		// (get) Token: 0x06004B38 RID: 19256
		// (set) Token: 0x06004B39 RID: 19257
		bool ToolActivateState { get; set; }
	}
}
