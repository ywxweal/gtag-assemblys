using System;
using GorillaExtensions;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x020001DD RID: 477
public class GTPosRotConstraints : MonoBehaviour, ISpawnable
{
	// Token: 0x1700011E RID: 286
	// (get) Token: 0x06000B28 RID: 2856 RVA: 0x0003BF7C File Offset: 0x0003A17C
	// (set) Token: 0x06000B29 RID: 2857 RVA: 0x0003BF84 File Offset: 0x0003A184
	public bool IsSpawned { get; set; }

	// Token: 0x1700011F RID: 287
	// (get) Token: 0x06000B2A RID: 2858 RVA: 0x0003BF8D File Offset: 0x0003A18D
	// (set) Token: 0x06000B2B RID: 2859 RVA: 0x0003BF95 File Offset: 0x0003A195
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x06000B2C RID: 2860 RVA: 0x0003BFA0 File Offset: 0x0003A1A0
	void ISpawnable.OnSpawn(VRRig rig)
	{
		Transform[] array = Array.Empty<Transform>();
		string text;
		if (rig != null && !GTHardCodedBones.TryGetBoneXforms(rig, out array, out text))
		{
			Debug.LogError("GTPosRotConstraints: Error getting bone Transforms: " + text, this);
			return;
		}
		for (int i = 0; i < this.constraints.Length; i++)
		{
			GorillaPosRotConstraint gorillaPosRotConstraint = this.constraints[i];
			if (Mathf.Approximately(gorillaPosRotConstraint.rotationOffset.x, 0f) && Mathf.Approximately(gorillaPosRotConstraint.rotationOffset.y, 0f) && Mathf.Approximately(gorillaPosRotConstraint.rotationOffset.z, 0f) && Mathf.Approximately(gorillaPosRotConstraint.rotationOffset.w, 0f))
			{
				gorillaPosRotConstraint.rotationOffset = Quaternion.identity;
			}
			if (!gorillaPosRotConstraint.follower)
			{
				Debug.LogError(string.Concat(new string[]
				{
					string.Format("{0}: Disabling component! At index {1}, Transform `follower` is ", "GTPosRotConstraints", i),
					"null. Affected component path: ",
					base.transform.GetPathQ(),
					"\n- Affected component path: ",
					base.transform.GetPathQ()
				}), this);
				base.enabled = false;
				return;
			}
			if (gorillaPosRotConstraint.sourceGorillaBone == GTHardCodedBones.EBone.None)
			{
				if (!gorillaPosRotConstraint.source)
				{
					if (string.IsNullOrEmpty(gorillaPosRotConstraint.sourceRelativePath))
					{
						Debug.LogError(string.Format("{0}: Disabling component! At index {1} Transform `source` is ", "GTPosRotConstraints", i) + "null, not EBone, and `sourceRelativePath` is null or empty.\n- Affected component path: " + base.transform.GetPathQ(), this);
						base.enabled = false;
						return;
					}
					if (!base.transform.TryFindByPath(gorillaPosRotConstraint.sourceRelativePath, out gorillaPosRotConstraint.source, false))
					{
						Debug.LogError(string.Concat(new string[]
						{
							string.Format("{0}: Disabling component! At index {1} Transform `source` is ", "GTPosRotConstraints", i),
							"null, not EBone, and could not find by path: \"",
							gorillaPosRotConstraint.sourceRelativePath,
							"\"\n- Affected component path: ",
							base.transform.GetPathQ()
						}), this);
						base.enabled = false;
						return;
					}
				}
				this.constraints[i] = gorillaPosRotConstraint;
			}
			else
			{
				if (rig == null)
				{
					Debug.LogError("GTPosRotConstraints: Disabling component! `VRRig` could not be found in parents, but " + string.Format("bone at index {0} is set to use EBone `{1}` but without `VRRig` it cannot ", i, gorillaPosRotConstraint.sourceGorillaBone) + "be resolved.\n- Affected component path: " + base.transform.GetPathQ(), this);
					base.enabled = false;
					return;
				}
				int boneIndex = GTHardCodedBones.GetBoneIndex(gorillaPosRotConstraint.sourceGorillaBone);
				if (boneIndex <= 0)
				{
					Debug.LogError(string.Format("{0}: (should never happen) Disabling component! At index {1}, could ", "GTPosRotConstraints", i) + string.Format("not find EBone `{0}`.\n", gorillaPosRotConstraint.sourceGorillaBone) + "- Affected component path: " + base.transform.GetPathQ(), this);
					base.enabled = false;
					return;
				}
				gorillaPosRotConstraint.source = array[boneIndex];
				if (!gorillaPosRotConstraint.source)
				{
					Debug.LogError(string.Concat(new string[]
					{
						string.Format("{0}: Disabling component! At index {1}, bone {2} was ", "GTPosRotConstraints", i, gorillaPosRotConstraint.sourceGorillaBone),
						"not present in `VRRig` path: ",
						rig.transform.GetPathQ(),
						"\n- Affected component path: ",
						base.transform.GetPathQ()
					}), this);
					base.enabled = false;
					return;
				}
				this.constraints[i] = gorillaPosRotConstraint;
			}
		}
		if (base.isActiveAndEnabled)
		{
			GTPosRotConstraintManager.Register(this);
		}
	}

	// Token: 0x06000B2D RID: 2861 RVA: 0x000023F4 File Offset: 0x000005F4
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x06000B2E RID: 2862 RVA: 0x0003C30F File Offset: 0x0003A50F
	protected void OnEnable()
	{
		if (this.IsSpawned)
		{
			GTPosRotConstraintManager.Register(this);
		}
	}

	// Token: 0x06000B2F RID: 2863 RVA: 0x0003C31F File Offset: 0x0003A51F
	protected void OnDisable()
	{
		GTPosRotConstraintManager.Unregister(this);
	}

	// Token: 0x04000DA1 RID: 3489
	public GorillaPosRotConstraint[] constraints;
}
