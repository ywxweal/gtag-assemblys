using System;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000B8F RID: 2959
	public class SharedBlocksScreen : MonoBehaviour
	{
		// Token: 0x0600493C RID: 18748 RVA: 0x000023F4 File Offset: 0x000005F4
		public virtual void OnUpPressed()
		{
		}

		// Token: 0x0600493D RID: 18749 RVA: 0x000023F4 File Offset: 0x000005F4
		public virtual void OnDownPressed()
		{
		}

		// Token: 0x0600493E RID: 18750 RVA: 0x000023F4 File Offset: 0x000005F4
		public virtual void OnSelectPressed()
		{
		}

		// Token: 0x0600493F RID: 18751 RVA: 0x000023F4 File Offset: 0x000005F4
		public virtual void OnDeletePressed()
		{
		}

		// Token: 0x06004940 RID: 18752 RVA: 0x000023F4 File Offset: 0x000005F4
		public virtual void OnNumberPressed(int number)
		{
		}

		// Token: 0x06004941 RID: 18753 RVA: 0x000023F4 File Offset: 0x000005F4
		public virtual void OnLetterPressed(string letter)
		{
		}

		// Token: 0x06004942 RID: 18754 RVA: 0x0015E14A File Offset: 0x0015C34A
		public virtual void Show()
		{
			if (!base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(true);
			}
		}

		// Token: 0x06004943 RID: 18755 RVA: 0x0015E165 File Offset: 0x0015C365
		public virtual void Hide()
		{
			if (base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(false);
			}
		}

		// Token: 0x04004C21 RID: 19489
		public SharedBlocksTerminal.ScreenType screenType;

		// Token: 0x04004C22 RID: 19490
		public SharedBlocksTerminal terminal;
	}
}
