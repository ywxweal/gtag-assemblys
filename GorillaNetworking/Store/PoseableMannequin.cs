using System;
using System.Collections;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaNetworking.Store
{
	// Token: 0x02000C90 RID: 3216
	public class PoseableMannequin : MonoBehaviour
	{
		// Token: 0x06004FAF RID: 20399 RVA: 0x0017BF25 File Offset: 0x0017A125
		public void Start()
		{
			this.skinnedMeshRenderer.gameObject.SetActive(false);
			this.staticGorillaMesh.gameObject.SetActive(true);
		}

		// Token: 0x06004FB0 RID: 20400 RVA: 0x00128D84 File Offset: 0x00126F84
		private string GetPrefabPathFromCurrentPrefabStage()
		{
			return "";
		}

		// Token: 0x06004FB1 RID: 20401 RVA: 0x00128D84 File Offset: 0x00126F84
		private string GetMeshPathFromPrefabPath(string prefabPath)
		{
			return "";
		}

		// Token: 0x06004FB2 RID: 20402 RVA: 0x0017BF49 File Offset: 0x0017A149
		public void BakeSkinnedMesh()
		{
			this.BakeAndSaveMeshInPath(this.GetMeshPathFromPrefabPath(this.GetPrefabPathFromCurrentPrefabStage()));
		}

		// Token: 0x06004FB3 RID: 20403 RVA: 0x000023F4 File Offset: 0x000005F4
		public void BakeAndSaveMeshInPath(string meshPath)
		{
		}

		// Token: 0x06004FB4 RID: 20404 RVA: 0x0017BF5D File Offset: 0x0017A15D
		private void UpdateStaticMeshMannequin()
		{
			this.staticGorillaMesh.sharedMesh = this.BakedColliderMesh;
			this.staticGorillaMeshRenderer.sharedMaterials = this.skinnedMeshRenderer.sharedMaterials;
			this.staticGorillaMeshCollider.sharedMesh = this.BakedColliderMesh;
		}

		// Token: 0x06004FB5 RID: 20405 RVA: 0x0017BF97 File Offset: 0x0017A197
		private void UpdateSkinnedMeshCollider()
		{
			this.skinnedMeshCollider.sharedMesh = this.BakedColliderMesh;
		}

		// Token: 0x06004FB6 RID: 20406 RVA: 0x0017BFAC File Offset: 0x0017A1AC
		public void UpdateGTPosRotConstraints()
		{
			GTPosRotConstraints[] array = this.cosmeticConstraints;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].constraints.ForEach(delegate(GorillaPosRotConstraint c)
				{
					c.follower.rotation = c.source.rotation;
					c.follower.position = c.source.position;
				});
			}
		}

		// Token: 0x06004FB7 RID: 20407 RVA: 0x0017BFFC File Offset: 0x0017A1FC
		private void HookupCosmeticConstraints()
		{
			this.cosmeticConstraints = base.GetComponentsInChildren<GTPosRotConstraints>();
			foreach (GTPosRotConstraints gtposRotConstraints in this.cosmeticConstraints)
			{
				for (int j = 0; j < gtposRotConstraints.constraints.Length; j++)
				{
					gtposRotConstraints.constraints[j].source = this.FindBone(gtposRotConstraints.constraints[j].follower.name);
				}
			}
		}

		// Token: 0x06004FB8 RID: 20408 RVA: 0x0017C070 File Offset: 0x0017A270
		private Transform FindBone(string boneName)
		{
			foreach (Transform transform in this.skinnedMeshRenderer.bones)
			{
				if (transform.name == boneName)
				{
					return transform;
				}
			}
			return null;
		}

		// Token: 0x06004FB9 RID: 20409 RVA: 0x000023F4 File Offset: 0x000005F4
		public void CreasteTestClip()
		{
		}

		// Token: 0x06004FBA RID: 20410 RVA: 0x0017C0AC File Offset: 0x0017A2AC
		public void SerializeVRRig()
		{
			base.StartCoroutine(this.SaveLocalPlayerPose());
		}

		// Token: 0x06004FBB RID: 20411 RVA: 0x0017C0BB File Offset: 0x0017A2BB
		public IEnumerator SaveLocalPlayerPose()
		{
			yield return null;
			yield break;
		}

		// Token: 0x06004FBC RID: 20412 RVA: 0x000023F4 File Offset: 0x000005F4
		public void SerializeOutBonesFromSkinnedMesh(SkinnedMeshRenderer paramSkinnedMeshRenderer)
		{
		}

		// Token: 0x06004FBD RID: 20413 RVA: 0x0017C0C4 File Offset: 0x0017A2C4
		public void SetCurvesForBone(SkinnedMeshRenderer paramSkinnedMeshRenderer, AnimationClip clip, Transform bone)
		{
			Keyframe[] array = new Keyframe[]
			{
				new Keyframe(0f, bone.parent.localRotation.x)
			};
			Keyframe[] array2 = new Keyframe[]
			{
				new Keyframe(0f, bone.parent.localRotation.y)
			};
			Keyframe[] array3 = new Keyframe[]
			{
				new Keyframe(0f, bone.parent.localRotation.z)
			};
			Keyframe[] array4 = new Keyframe[]
			{
				new Keyframe(0f, bone.parent.localRotation.w)
			};
			AnimationCurve animationCurve = new AnimationCurve(array);
			AnimationCurve animationCurve2 = new AnimationCurve(array2);
			AnimationCurve animationCurve3 = new AnimationCurve(array3);
			AnimationCurve animationCurve4 = new AnimationCurve(array4);
			string text = "";
			string text2 = bone.name.Replace("_new", "");
			foreach (Transform transform in this.skinnedMeshRenderer.bones)
			{
				if (transform.name == text2)
				{
					text = transform.GetPath(this.skinnedMeshRenderer.transform.parent).TrimStart('/');
					break;
				}
			}
			clip.SetCurve(text, typeof(Transform), "m_LocalRotation.x", animationCurve);
			clip.SetCurve(text, typeof(Transform), "m_LocalRotation.y", animationCurve2);
			clip.SetCurve(text, typeof(Transform), "m_LocalRotation.z", animationCurve3);
			clip.SetCurve(text, typeof(Transform), "m_LocalRotation.w", animationCurve4);
		}

		// Token: 0x06004FBE RID: 20414 RVA: 0x000023F4 File Offset: 0x000005F4
		public void UpdatePrefabWithAnimationClip(string AnimationFileName)
		{
		}

		// Token: 0x06004FBF RID: 20415 RVA: 0x000023F4 File Offset: 0x000005F4
		public void LoadPoseOntoMannequin(AnimationClip clip, float frameTime = 0f)
		{
		}

		// Token: 0x06004FC0 RID: 20416 RVA: 0x000023F4 File Offset: 0x000005F4
		public void OnValidate()
		{
		}

		// Token: 0x040052C5 RID: 21189
		public SkinnedMeshRenderer skinnedMeshRenderer;

		// Token: 0x040052C6 RID: 21190
		[FormerlySerializedAs("meshCollider")]
		public MeshCollider skinnedMeshCollider;

		// Token: 0x040052C7 RID: 21191
		public GTPosRotConstraints[] cosmeticConstraints;

		// Token: 0x040052C8 RID: 21192
		public Mesh BakedColliderMesh;

		// Token: 0x040052C9 RID: 21193
		[SerializeField]
		[FormerlySerializedAs("liveAssetPath")]
		protected string prefabAssetPath;

		// Token: 0x040052CA RID: 21194
		[SerializeField]
		protected string prefabFolderPath;

		// Token: 0x040052CB RID: 21195
		[SerializeField]
		protected string prefabAssetName;

		// Token: 0x040052CC RID: 21196
		public MeshFilter staticGorillaMesh;

		// Token: 0x040052CD RID: 21197
		public MeshCollider staticGorillaMeshCollider;

		// Token: 0x040052CE RID: 21198
		public MeshRenderer staticGorillaMeshRenderer;
	}
}
