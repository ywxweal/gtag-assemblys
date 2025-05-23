using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000B0C RID: 2828
	public class GameObjectManagerWithId : MonoBehaviour
	{
		// Token: 0x0600457C RID: 17788 RVA: 0x0014A6E8 File Offset: 0x001488E8
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

		// Token: 0x0600457D RID: 17789 RVA: 0x0014A74E File Offset: 0x0014894E
		private void OnDestroy()
		{
			this.objectData.Clear();
		}

		// Token: 0x0600457E RID: 17790 RVA: 0x0014A75C File Offset: 0x0014895C
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

		// Token: 0x0600457F RID: 17791 RVA: 0x0014A7C4 File Offset: 0x001489C4
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

		// Token: 0x04004811 RID: 18449
		public GameObject objectsContainer;

		// Token: 0x04004812 RID: 18450
		public GTZone zone;

		// Token: 0x04004813 RID: 18451
		private readonly List<GameObjectManagerWithId.gameObjectData> objectData = new List<GameObjectManagerWithId.gameObjectData>();

		// Token: 0x02000B0D RID: 2829
		private class gameObjectData
		{
			// Token: 0x04004814 RID: 18452
			public Transform transform;

			// Token: 0x04004815 RID: 18453
			public Transform followTransform;

			// Token: 0x04004816 RID: 18454
			public string id;

			// Token: 0x04004817 RID: 18455
			public bool isMatched;
		}
	}
}
