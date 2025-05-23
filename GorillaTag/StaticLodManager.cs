using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GorillaTag
{
	// Token: 0x02000D1D RID: 3357
	[DefaultExecutionOrder(2000)]
	public class StaticLodManager : MonoBehaviour, IGorillaSliceableSimple
	{
		// Token: 0x060053F6 RID: 21494 RVA: 0x00196C51 File Offset: 0x00194E51
		private void Awake()
		{
			StaticLodManager.gorillaInteractableLayer = UnityLayer.GorillaInteractable;
		}

		// Token: 0x060053F7 RID: 21495 RVA: 0x00196C5A File Offset: 0x00194E5A
		public void OnEnable()
		{
			GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
			this.mainCamera = Camera.main;
			this.hasMainCamera = this.mainCamera != null;
		}

		// Token: 0x060053F8 RID: 21496 RVA: 0x00010F34 File Offset: 0x0000F134
		public void OnDisable()
		{
			GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		}

		// Token: 0x060053F9 RID: 21497 RVA: 0x00196C80 File Offset: 0x00194E80
		public static int Register(StaticLodGroup lodGroup)
		{
			StaticLodGroupExcluder componentInParent = lodGroup.GetComponentInParent<StaticLodGroupExcluder>();
			Text[] array = lodGroup.GetComponentsInChildren<Text>(true);
			List<Text> list = new List<Text>(array.Length);
			foreach (Text text in array)
			{
				StaticLodGroupExcluder componentInParent2 = text.GetComponentInParent<StaticLodGroupExcluder>();
				if (!(componentInParent2 != null) || !(componentInParent2 != componentInParent))
				{
					list.Add(text);
				}
			}
			array = list.ToArray();
			TextMeshPro[] array3 = lodGroup.GetComponentsInChildren<TextMeshPro>(true);
			List<TextMeshPro> list2 = new List<TextMeshPro>(array3.Length);
			foreach (TextMeshPro textMeshPro in array3)
			{
				StaticLodGroupExcluder componentInParent3 = textMeshPro.GetComponentInParent<StaticLodGroupExcluder>();
				if (!(componentInParent3 != null) || !(componentInParent3 != componentInParent))
				{
					list2.Add(textMeshPro);
				}
			}
			array3 = list2.ToArray();
			Collider[] componentsInChildren = lodGroup.GetComponentsInChildren<Collider>(true);
			List<Collider> list3 = new List<Collider>(componentsInChildren.Length);
			foreach (Collider collider in componentsInChildren)
			{
				if (collider.gameObject.IsOnLayer(StaticLodManager.gorillaInteractableLayer))
				{
					StaticLodGroupExcluder componentInParent4 = collider.GetComponentInParent<StaticLodGroupExcluder>();
					if (!(componentInParent4 != null) || !(componentInParent4 != componentInParent))
					{
						list3.Add(collider);
					}
				}
			}
			Bounds bounds;
			if (array.Length != 0)
			{
				bounds = new Bounds(array[0].transform.position, Vector3.one * 0.01f);
			}
			else if (array3.Length != 0)
			{
				bounds = new Bounds(array3[0].transform.position, Vector3.one * 0.01f);
			}
			else if (list3.Count > 0)
			{
				bounds = new Bounds(list3[0].bounds.center, list3[0].bounds.size);
			}
			else
			{
				bounds = new Bounds(lodGroup.transform.position, Vector3.one * 0.01f);
			}
			foreach (Text text2 in array)
			{
				bounds.Encapsulate(text2.transform.position);
			}
			foreach (TextMeshPro textMeshPro2 in array3)
			{
				bounds.Encapsulate(textMeshPro2.transform.position);
			}
			foreach (Collider collider2 in list3)
			{
				bounds.Encapsulate(collider2.bounds);
			}
			StaticLodManager.GroupInfo groupInfo = new StaticLodManager.GroupInfo
			{
				isLoaded = true,
				componentEnabled = lodGroup.isActiveAndEnabled,
				center = bounds.center,
				radiusSq = bounds.extents.sqrMagnitude,
				uiEnabled = true,
				uiEnableDistanceSq = lodGroup.uiFadeDistanceMax * lodGroup.uiFadeDistanceMax,
				uiTexts = array,
				uiTMPs = array3,
				collidersEnabled = true,
				collisionEnableDistanceSq = lodGroup.collisionEnableDistance * lodGroup.collisionEnableDistance,
				interactableColliders = list3.ToArray()
			};
			int count;
			if (StaticLodManager.freeSlots.TryPop(out count))
			{
				StaticLodManager.groupMonoBehaviours[count] = lodGroup;
				StaticLodManager.groupInfos[count] = groupInfo;
			}
			else
			{
				count = StaticLodManager.groupMonoBehaviours.Count;
				StaticLodManager.groupMonoBehaviours.Add(lodGroup);
				StaticLodManager.groupInfos.Add(groupInfo);
			}
			return count;
		}

		// Token: 0x060053FA RID: 21498 RVA: 0x00197000 File Offset: 0x00195200
		public static void Unregister(int lodGroupIndex)
		{
			StaticLodManager.groupMonoBehaviours[lodGroupIndex] = null;
			StaticLodManager.groupInfos[lodGroupIndex] = default(StaticLodManager.GroupInfo);
			StaticLodManager.freeSlots.Push(lodGroupIndex);
		}

		// Token: 0x060053FB RID: 21499 RVA: 0x00197038 File Offset: 0x00195238
		public static void SetEnabled(int index, bool enable)
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			StaticLodManager.GroupInfo groupInfo = StaticLodManager.groupInfos[index];
			groupInfo.componentEnabled = enable;
			StaticLodManager.groupInfos[index] = groupInfo;
		}

		// Token: 0x060053FC RID: 21500 RVA: 0x00197070 File Offset: 0x00195270
		public void SliceUpdate()
		{
			if (!this.hasMainCamera)
			{
				return;
			}
			Vector3 position = this.mainCamera.transform.position;
			for (int i = 0; i < StaticLodManager.groupInfos.Count; i++)
			{
				StaticLodManager.GroupInfo groupInfo = StaticLodManager.groupInfos[i];
				if (groupInfo.isLoaded && groupInfo.componentEnabled)
				{
					float num = Mathf.Max(0f, (groupInfo.center - position).sqrMagnitude - groupInfo.radiusSq);
					float num2 = (groupInfo.uiEnabled ? 0.010000001f : 0f);
					bool flag = num < groupInfo.uiEnableDistanceSq + num2;
					if (flag != groupInfo.uiEnabled)
					{
						for (int j = 0; j < groupInfo.uiTexts.Length; j++)
						{
							Text text = groupInfo.uiTexts[j];
							if (!(text == null))
							{
								text.enabled = flag;
							}
						}
						for (int k = 0; k < groupInfo.uiTMPs.Length; k++)
						{
							TextMeshPro textMeshPro = groupInfo.uiTMPs[k];
							if (!(textMeshPro == null))
							{
								textMeshPro.enabled = flag;
							}
						}
					}
					groupInfo.uiEnabled = flag;
					num2 = (groupInfo.collidersEnabled ? 0.010000001f : 0f);
					bool flag2 = num < groupInfo.collisionEnableDistanceSq + num2;
					if (flag2 != groupInfo.collidersEnabled)
					{
						for (int l = 0; l < groupInfo.interactableColliders.Length; l++)
						{
							if (!(groupInfo.interactableColliders[l] == null))
							{
								groupInfo.interactableColliders[l].enabled = flag2;
							}
						}
					}
					groupInfo.collidersEnabled = flag2;
					StaticLodManager.groupInfos[i] = groupInfo;
				}
			}
		}

		// Token: 0x060053FF RID: 21503 RVA: 0x00011040 File Offset: 0x0000F240
		bool IGorillaSliceableSimple.get_isActiveAndEnabled()
		{
			return base.isActiveAndEnabled;
		}

		// Token: 0x040056E4 RID: 22244
		[OnEnterPlay_Clear]
		private static readonly List<StaticLodGroup> groupMonoBehaviours = new List<StaticLodGroup>(32);

		// Token: 0x040056E5 RID: 22245
		[DebugReadout]
		[OnEnterPlay_Clear]
		private static readonly List<StaticLodManager.GroupInfo> groupInfos = new List<StaticLodManager.GroupInfo>(32);

		// Token: 0x040056E6 RID: 22246
		[OnEnterPlay_Clear]
		private static readonly Stack<int> freeSlots = new Stack<int>();

		// Token: 0x040056E7 RID: 22247
		private static UnityLayer gorillaInteractableLayer;

		// Token: 0x040056E8 RID: 22248
		private Camera mainCamera;

		// Token: 0x040056E9 RID: 22249
		private bool hasMainCamera;

		// Token: 0x02000D1E RID: 3358
		private struct GroupInfo
		{
			// Token: 0x040056EA RID: 22250
			public bool isLoaded;

			// Token: 0x040056EB RID: 22251
			public bool componentEnabled;

			// Token: 0x040056EC RID: 22252
			public Vector3 center;

			// Token: 0x040056ED RID: 22253
			public float radiusSq;

			// Token: 0x040056EE RID: 22254
			public bool uiEnabled;

			// Token: 0x040056EF RID: 22255
			public float uiEnableDistanceSq;

			// Token: 0x040056F0 RID: 22256
			public Text[] uiTexts;

			// Token: 0x040056F1 RID: 22257
			public TextMeshPro[] uiTMPs;

			// Token: 0x040056F2 RID: 22258
			public bool collidersEnabled;

			// Token: 0x040056F3 RID: 22259
			public float collisionEnableDistanceSq;

			// Token: 0x040056F4 RID: 22260
			public Collider[] interactableColliders;
		}
	}
}
