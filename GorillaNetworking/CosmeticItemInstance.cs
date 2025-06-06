﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x02000C0D RID: 3085
	public class CosmeticItemInstance
	{
		// Token: 0x06004C2F RID: 19503 RVA: 0x00168FD0 File Offset: 0x001671D0
		private void EnableItem(GameObject obj, bool enable)
		{
			CosmeticAnchors component = obj.GetComponent<CosmeticAnchors>();
			try
			{
				if (component && !enable)
				{
					component.EnableAnchor(false);
				}
				obj.SetActive(enable);
				if (component && enable)
				{
					component.EnableAnchor(true);
				}
			}
			catch (Exception ex)
			{
				Debug.LogError(string.Format("Exception while enabling cosmetic: {0}", ex));
			}
		}

		// Token: 0x06004C30 RID: 19504 RVA: 0x00169034 File Offset: 0x00167234
		public void DisableItem(CosmeticsController.CosmeticSlots cosmeticSlot)
		{
			bool flag = CosmeticsController.CosmeticSet.IsSlotLeftHanded(cosmeticSlot);
			bool flag2 = CosmeticsController.CosmeticSet.IsSlotRightHanded(cosmeticSlot);
			foreach (GameObject gameObject in this.objects)
			{
				this.EnableItem(gameObject, false);
			}
			if (flag)
			{
				foreach (GameObject gameObject2 in this.leftObjects)
				{
					this.EnableItem(gameObject2, false);
				}
			}
			if (flag2)
			{
				foreach (GameObject gameObject3 in this.rightObjects)
				{
					this.EnableItem(gameObject3, false);
				}
			}
		}

		// Token: 0x06004C31 RID: 19505 RVA: 0x00169128 File Offset: 0x00167328
		public void EnableItem(CosmeticsController.CosmeticSlots cosmeticSlot)
		{
			bool flag = CosmeticsController.CosmeticSet.IsSlotLeftHanded(cosmeticSlot);
			bool flag2 = CosmeticsController.CosmeticSet.IsSlotRightHanded(cosmeticSlot);
			foreach (GameObject gameObject in this.objects)
			{
				this.EnableItem(gameObject, true);
				if (cosmeticSlot == CosmeticsController.CosmeticSlots.Badge)
				{
					GorillaTagger.Instance.offlineVRRig.GetComponent<VRRigAnchorOverrides>().CurrentBadgeTransform = gameObject.transform;
				}
			}
			if (flag)
			{
				foreach (GameObject gameObject2 in this.leftObjects)
				{
					this.EnableItem(gameObject2, true);
				}
			}
			if (flag2)
			{
				foreach (GameObject gameObject3 in this.rightObjects)
				{
					this.EnableItem(gameObject3, true);
				}
			}
		}

		// Token: 0x04004F1B RID: 20251
		public List<GameObject> leftObjects = new List<GameObject>();

		// Token: 0x04004F1C RID: 20252
		public List<GameObject> rightObjects = new List<GameObject>();

		// Token: 0x04004F1D RID: 20253
		public List<GameObject> objects = new List<GameObject>();
	}
}
