using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000B0C RID: 2828
	public class GameObjectManagerWithId : MonoBehaviour
	{
		// Token: 0x0600457B RID: 17787 RVA: 0x0014A610 File Offset: 0x00148810
		private void Awake()
		{
			Transform[] componentsInChildren = this.objectsContainer.GetComponentsInChildren<Transform>(false);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				GameObjectManagerWithId.gameObjectData gameObjectData = new GameObjectManagerWithId.gameObjectData();
				gameObjectData.transform = componentsInChildren[i];
				gameObjectData.id = this.zone.ToString() + i.ToString();
				this.objectData.Add(gameObjectData);
			}
		}

		// Token: 0x0600457C RID: 17788 RVA: 0x0014A676 File Offset: 0x00148876
		private void OnDestroy()
		{
			this.objectData.Clear();
		}

		// Token: 0x0600457D RID: 17789 RVA: 0x0014A684 File Offset: 0x00148884
		public void ReceiveEvent(string id, Transform _transform)
		{
			foreach (GameObjectManagerWithId.gameObjectData gameObjectData in this.objectData)
			{
				if (gameObjectData.id == id)
				{
					gameObjectData.isMatched = true;
					gameObjectData.followTransform = _transform;
				}
			}
		}

		// Token: 0x0600457E RID: 17790 RVA: 0x0014A6EC File Offset: 0x001488EC
		private void Update()
		{
			foreach (GameObjectManagerWithId.gameObjectData gameObjectData in this.objectData)
			{
				if (gameObjectData.isMatched)
				{
					gameObjectData.transform.transform.position = gameObjectData.followTransform.position;
					gameObjectData.transform.transform.rotation = gameObjectData.followTransform.rotation;
				}
			}
		}

		// Token: 0x04004810 RID: 18448
		public GameObject objectsContainer;

		// Token: 0x04004811 RID: 18449
		public GTZone zone;

		// Token: 0x04004812 RID: 18450
		private readonly List<GameObjectManagerWithId.gameObjectData> objectData = new List<GameObjectManagerWithId.gameObjectData>();

		// Token: 0x02000B0D RID: 2829
		private class gameObjectData
		{
			// Token: 0x04004813 RID: 18451
			public Transform transform;

			// Token: 0x04004814 RID: 18452
			public Transform followTransform;

			// Token: 0x04004815 RID: 18453
			public string id;

			// Token: 0x04004816 RID: 18454
			public bool isMatched;
		}
	}
}
