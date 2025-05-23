using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000BDC RID: 3036
	public class InteractableToolsCreator : MonoBehaviour
	{
		// Token: 0x06004AEB RID: 19179 RVA: 0x00164254 File Offset: 0x00162454
		private void Awake()
		{
			if (this.LeftHandTools != null && this.LeftHandTools.Length != 0)
			{
				base.StartCoroutine(this.AttachToolsToHands(this.LeftHandTools, false));
			}
			if (this.RightHandTools != null && this.RightHandTools.Length != 0)
			{
				base.StartCoroutine(this.AttachToolsToHands(this.RightHandTools, true));
			}
		}

		// Token: 0x06004AEC RID: 19180 RVA: 0x001642AB File Offset: 0x001624AB
		private IEnumerator AttachToolsToHands(Transform[] toolObjects, bool isRightHand)
		{
			HandsManager handsManagerObj = null;
			while ((handsManagerObj = HandsManager.Instance) == null || !handsManagerObj.IsInitialized())
			{
				yield return null;
			}
			HashSet<Transform> hashSet = new HashSet<Transform>();
			foreach (Transform transform in toolObjects)
			{
				hashSet.Add(transform.transform);
			}
			foreach (Transform toolObject in hashSet)
			{
				OVRSkeleton handSkeletonToAttachTo = (isRightHand ? handsManagerObj.RightHandSkeleton : handsManagerObj.LeftHandSkeleton);
				while (handSkeletonToAttachTo == null || handSkeletonToAttachTo.Bones == null)
				{
					yield return null;
				}
				this.AttachToolToHandTransform(toolObject, isRightHand);
				handSkeletonToAttachTo = null;
				toolObject = null;
			}
			HashSet<Transform>.Enumerator enumerator = default(HashSet<Transform>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x06004AED RID: 19181 RVA: 0x001642C8 File Offset: 0x001624C8
		private void AttachToolToHandTransform(Transform tool, bool isRightHanded)
		{
			Transform transform = Object.Instantiate<Transform>(tool).transform;
			transform.localPosition = Vector3.zero;
			InteractableTool component = transform.GetComponent<InteractableTool>();
			component.IsRightHandedTool = isRightHanded;
			component.Initialize();
		}

		// Token: 0x04004D9B RID: 19867
		[SerializeField]
		private Transform[] LeftHandTools;

		// Token: 0x04004D9C RID: 19868
		[SerializeField]
		private Transform[] RightHandTools;
	}
}
