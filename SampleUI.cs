using System;
using UnityEngine;

// Token: 0x02000323 RID: 803
public class SampleUI : MonoBehaviour
{
	// Token: 0x06001309 RID: 4873 RVA: 0x0005A588 File Offset: 0x00058788
	private void Start()
	{
		DebugUIBuilder.instance.AddLabel("Enable Firebase in your project before running this sample", 1);
		DebugUIBuilder.instance.Show();
		this.inMenu = true;
	}

	// Token: 0x0600130A RID: 4874 RVA: 0x0005A5AC File Offset: 0x000587AC
	private void Update()
	{
		if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.Active) || OVRInput.GetDown(OVRInput.Button.Start, OVRInput.Controller.Active))
		{
			if (this.inMenu)
			{
				DebugUIBuilder.instance.Hide();
			}
			else
			{
				DebugUIBuilder.instance.Show();
			}
			this.inMenu = !this.inMenu;
		}
	}

	// Token: 0x0400152B RID: 5419
	private RectTransform collectionButton;

	// Token: 0x0400152C RID: 5420
	private RectTransform inputText;

	// Token: 0x0400152D RID: 5421
	private RectTransform valueText;

	// Token: 0x0400152E RID: 5422
	private bool inMenu;
}
