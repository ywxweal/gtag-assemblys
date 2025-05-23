using System;
using UnityEngine;

// Token: 0x02000017 RID: 23
public class PlatformerCollectiblesMain : MonoBehaviour
{
	// Token: 0x06000055 RID: 85 RVA: 0x0000330C File Offset: 0x0000150C
	public void Start()
	{
		int num = 0;
		while ((float)num < this.CoinGridCount)
		{
			float num2 = -0.5f * this.CoinGridSize + this.CoinGridSize * (float)num / (this.CoinGridCount - 1f);
			int num3 = 0;
			while ((float)num3 < this.CoinGridCount)
			{
				float num4 = -0.5f * this.CoinGridSize + this.CoinGridSize * (float)num3 / (this.CoinGridCount - 1f);
				Object.Instantiate<GameObject>(this.Coin).transform.position = new Vector3(num2, 0.2f, num4);
				num3++;
			}
			num++;
		}
	}

	// Token: 0x04000044 RID: 68
	public GameObject Coin;

	// Token: 0x04000045 RID: 69
	public float CoinGridCount = 5f;

	// Token: 0x04000046 RID: 70
	public float CoinGridSize = 7f;
}
