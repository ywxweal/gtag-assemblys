using System;
using GorillaNetworking;
using UnityEngine;

// Token: 0x020003E2 RID: 994
public class BetaChecker : MonoBehaviour
{
	// Token: 0x06001800 RID: 6144 RVA: 0x00074F23 File Offset: 0x00073123
	private void Start()
	{
		if (PlayerPrefs.GetString("CheckedBox2") == "true")
		{
			this.doNotEnable = true;
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x06001801 RID: 6145 RVA: 0x00074F50 File Offset: 0x00073150
	private void Update()
	{
		if (!this.doNotEnable)
		{
			if (CosmeticsController.instance.confirmedDidntPlayInBeta)
			{
				PlayerPrefs.SetString("CheckedBox2", "true");
				PlayerPrefs.Save();
				base.gameObject.SetActive(false);
				return;
			}
			if (CosmeticsController.instance.playedInBeta)
			{
				GameObject[] array = this.objectsToEnable;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].SetActive(true);
				}
				this.doNotEnable = true;
			}
		}
	}

	// Token: 0x04001ADF RID: 6879
	public GameObject[] objectsToEnable;

	// Token: 0x04001AE0 RID: 6880
	public bool doNotEnable;
}
