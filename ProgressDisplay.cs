using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000134 RID: 308
public class ProgressDisplay : MonoBehaviour
{
	// Token: 0x06000828 RID: 2088 RVA: 0x0002CDB0 File Offset: 0x0002AFB0
	private void Reset()
	{
		this.root = base.gameObject;
	}

	// Token: 0x06000829 RID: 2089 RVA: 0x0002CDBE File Offset: 0x0002AFBE
	public void SetVisible(bool visible)
	{
		this.root.SetActive(visible);
	}

	// Token: 0x0600082A RID: 2090 RVA: 0x0002CDCC File Offset: 0x0002AFCC
	public void SetProgress(int progress, int total)
	{
		if (this.text)
		{
			if (total < this.largestNumberToShow)
			{
				this.text.text = ((progress >= total) ? string.Format("{0}", total) : string.Format("{0}/{1}", progress, total));
				this.SetTextVisible(true);
			}
			else
			{
				this.SetTextVisible(false);
			}
		}
		this.progressImage.fillAmount = (float)progress / (float)total;
	}

	// Token: 0x0600082B RID: 2091 RVA: 0x0002CE46 File Offset: 0x0002B046
	public void SetProgress(float progress)
	{
		this.progressImage.fillAmount = progress;
	}

	// Token: 0x0600082C RID: 2092 RVA: 0x0002CE54 File Offset: 0x0002B054
	private void SetTextVisible(bool visible)
	{
		if (this.text.gameObject.activeSelf == visible)
		{
			return;
		}
		this.text.gameObject.SetActive(visible);
	}

	// Token: 0x040009A7 RID: 2471
	[SerializeField]
	private GameObject root;

	// Token: 0x040009A8 RID: 2472
	[SerializeField]
	private TMP_Text text;

	// Token: 0x040009A9 RID: 2473
	[SerializeField]
	private Image progressImage;

	// Token: 0x040009AA RID: 2474
	[SerializeField]
	private int largestNumberToShow = 99;
}
