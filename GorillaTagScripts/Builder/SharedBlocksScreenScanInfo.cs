using System;
using TMPro;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000B90 RID: 2960
	public class SharedBlocksScreenScanInfo : SharedBlocksScreen
	{
		// Token: 0x06004945 RID: 18757 RVA: 0x000023F4 File Offset: 0x000005F4
		public override void OnUpPressed()
		{
		}

		// Token: 0x06004946 RID: 18758 RVA: 0x000023F4 File Offset: 0x000005F4
		public override void OnDownPressed()
		{
		}

		// Token: 0x06004947 RID: 18759 RVA: 0x0015E180 File Offset: 0x0015C380
		public override void OnSelectPressed()
		{
			this.terminal.OnLoadMapPressed();
		}

		// Token: 0x06004948 RID: 18760 RVA: 0x0015E18D File Offset: 0x0015C38D
		public override void Show()
		{
			base.Show();
			this.DrawScreen();
		}

		// Token: 0x06004949 RID: 18761 RVA: 0x0015E19C File Offset: 0x0015C39C
		private void DrawScreen()
		{
			if (this.terminal.SelectedMap == null)
			{
				this.mapIDText.text = "MAP ID: NONE";
				return;
			}
			this.mapIDText.text = "MAP ID: " + SharedBlocksTerminal.MapIDToDisplayedString(this.terminal.SelectedMap.MapID);
		}

		// Token: 0x04004C23 RID: 19491
		[SerializeField]
		private TMP_Text mapIDText;
	}
}
