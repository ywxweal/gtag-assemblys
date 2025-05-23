using System;
using UnityEngine;

// Token: 0x02000129 RID: 297
public class RockPiles : MonoBehaviour
{
	// Token: 0x060007CD RID: 1997 RVA: 0x0002B9F4 File Offset: 0x00029BF4
	public void Show(int visiblePercentage)
	{
		if (visiblePercentage <= 0)
		{
			this.ShowRock(-1);
			return;
		}
		int num = -1;
		int num2 = -1;
		for (int i = 0; i < this._rocks.Length; i++)
		{
			RockPiles.RockPile rockPile = this._rocks[i];
			if (visiblePercentage >= rockPile.threshold && num2 < rockPile.threshold)
			{
				num = i;
				num2 = rockPile.threshold;
			}
		}
		this.ShowRock(num);
	}

	// Token: 0x060007CE RID: 1998 RVA: 0x0002BA54 File Offset: 0x00029C54
	private void ShowRock(int rockToShow)
	{
		for (int i = 0; i < this._rocks.Length; i++)
		{
			this._rocks[i].visual.SetActive(i == rockToShow);
		}
	}

	// Token: 0x04000955 RID: 2389
	[SerializeField]
	private RockPiles.RockPile[] _rocks;

	// Token: 0x0200012A RID: 298
	[Serializable]
	public struct RockPile
	{
		// Token: 0x04000956 RID: 2390
		public GameObject visual;

		// Token: 0x04000957 RID: 2391
		public int threshold;
	}
}
