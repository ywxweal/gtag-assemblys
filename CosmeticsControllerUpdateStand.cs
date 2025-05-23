using System;
using System.Collections;
using GorillaNetworking;
using UnityEngine;

// Token: 0x020003E9 RID: 1001
public class CosmeticsControllerUpdateStand : MonoBehaviour
{
	// Token: 0x06001813 RID: 6163 RVA: 0x0007542C File Offset: 0x0007362C
	public GameObject ReturnChildWithCosmeticNameMatch(Transform parentTransform)
	{
		GameObject gameObject = null;
		using (IEnumerator enumerator = parentTransform.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				Transform child = (Transform)enumerator.Current;
				if (child.gameObject.activeInHierarchy && this.cosmeticsController.allCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => child.name == x.itemName) > -1)
				{
					return child.gameObject;
				}
				gameObject = this.ReturnChildWithCosmeticNameMatch(child);
				if (gameObject != null)
				{
					return gameObject;
				}
			}
		}
		return gameObject;
	}

	// Token: 0x04001AF3 RID: 6899
	public CosmeticsController cosmeticsController;

	// Token: 0x04001AF4 RID: 6900
	public bool FailEntitlement;

	// Token: 0x04001AF5 RID: 6901
	public bool PlayerUnlocked;

	// Token: 0x04001AF6 RID: 6902
	public bool ItemNotGrantedYet;

	// Token: 0x04001AF7 RID: 6903
	public bool ItemSuccessfullyGranted;

	// Token: 0x04001AF8 RID: 6904
	public bool AttemptToConsumeEntitlement;

	// Token: 0x04001AF9 RID: 6905
	public bool EntitlementSuccessfullyConsumed;

	// Token: 0x04001AFA RID: 6906
	public bool LockSuccessfullyCleared;

	// Token: 0x04001AFB RID: 6907
	public bool RunDebug;

	// Token: 0x04001AFC RID: 6908
	public Transform textParent;

	// Token: 0x04001AFD RID: 6909
	private CosmeticsController.CosmeticItem outItem;

	// Token: 0x04001AFE RID: 6910
	public HeadModel[] inventoryHeadModels;

	// Token: 0x04001AFF RID: 6911
	public string headModelsPrefabPath;
}
