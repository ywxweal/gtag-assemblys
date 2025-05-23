using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x02000C0C RID: 3084
	public class CosmeticItemRegistry
	{
		// Token: 0x06004C2B RID: 19499 RVA: 0x00168DB0 File Offset: 0x00166FB0
		public void Initialize(GameObject[] cosmeticGObjs)
		{
			if (this._isInitialized)
			{
				return;
			}
			this._isInitialized = true;
			foreach (GameObject gameObject in cosmeticGObjs)
			{
				string text = gameObject.name.Replace("LEFT.", "").Replace("RIGHT.", "").TrimEnd();
				CosmeticItemInstance cosmeticItemInstance;
				if (this.nameToCosmeticMap.ContainsKey(text))
				{
					cosmeticItemInstance = this.nameToCosmeticMap[text];
				}
				else
				{
					cosmeticItemInstance = new CosmeticItemInstance();
					this.nameToCosmeticMap.Add(text, cosmeticItemInstance);
				}
				bool flag = gameObject.name.Contains("LEFT.");
				bool flag2 = gameObject.name.Contains("RIGHT.");
				if (flag)
				{
					cosmeticItemInstance.leftObjects.Add(gameObject);
				}
				else if (flag2)
				{
					cosmeticItemInstance.rightObjects.Add(gameObject);
				}
				else
				{
					cosmeticItemInstance.objects.Add(gameObject);
				}
			}
		}

		// Token: 0x06004C2C RID: 19500 RVA: 0x00168E98 File Offset: 0x00167098
		public CosmeticItemInstance Cosmetic(string itemName)
		{
			if (!this._isInitialized)
			{
				Debug.LogError("Tried to use CosmeticItemRegistry before it was initialized!");
				return null;
			}
			if (string.IsNullOrEmpty(itemName) || itemName == "NOTHING")
			{
				return null;
			}
			CosmeticItemInstance cosmeticItemInstance;
			if (!this.nameToCosmeticMap.TryGetValue(itemName, out cosmeticItemInstance))
			{
				return null;
			}
			return cosmeticItemInstance;
		}

		// Token: 0x04004F17 RID: 20247
		private bool _isInitialized;

		// Token: 0x04004F18 RID: 20248
		private Dictionary<string, CosmeticItemInstance> nameToCosmeticMap = new Dictionary<string, CosmeticItemInstance>();

		// Token: 0x04004F19 RID: 20249
		private GameObject nullItem;
	}
}
