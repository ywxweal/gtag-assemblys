using System;
using TMPro;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000B90 RID: 2960
	public class SharedBlocksScreenScanInfo : SharedBlocksScreen
	{
		// Token: 0x06004946 RID: 18758 RVA: 0x000023F4 File Offset: 0x000005F4
		public override void OnUpPressed()
		{
		}

		// Token: 0x06004947 RID: 18759 RVA: 0x000023F4 File Offset: 0x000005F4
		public override void OnDownPressed()
		{
		}

		// Token: 0x06004948 RID: 18760 RVA: 0x0015E258 File Offset: 0x0015C458
		public override void OnSelectPressed()
		{
			this.terminal.OnLoadMapPressed();
		}

		// Token: 0x06004949 RID: 18761 RVA: 0x0015E265 File Offset: 0x0015C465
		public override void Show()
		{
			base.Show();
			this.DrawScreen();
		}

		// Token: 0x0600494A RID: 18762 RVA: 0x0015E274 File Offset: 0x0015C474
		private void DrawScreen()
		{
			if (this.terminal.SelectedMap == null)
			{
				this.mapIDText.text = "MAP ID: NONE";
				return;
			}
			this.mapIDText.text = "MAP ID: " + SharedBlocksTerminal.MapIDToDisplayedString(this.terminal.SelectedMap.MapID);
		}

		// Token: 0x04004C24 RID: 19492
		[SerializeField]
		private TMP_Text mapIDText;
	}
}
