using System;
using GorillaTag;
using UnityEngine;

// Token: 0x0200018A RID: 394
public class HandTapEffect : MonoBehaviour
{
	// Token: 0x060009BB RID: 2491 RVA: 0x00033D32 File Offset: 0x00031F32
	private void Awake()
	{
		this.rig = base.GetComponentInParent<VRRig>();
		this.rightHandEffects = this.rig.RightHandEffect;
		this.leftHandEffects = this.rig.LeftHandEffect;
	}

	// Token: 0x060009BC RID: 2492 RVA: 0x00033D62 File Offset: 0x00031F62
	private void OnEnable()
	{
		this.rightHandEffects.PrefabPoolIds[2] = in this.rightHandPrefab;
		this.leftHandEffects.PrefabPoolIds[2] = in this.leftHandPrefab;
	}

	// Token: 0x060009BD RID: 2493 RVA: 0x00033D94 File Offset: 0x00031F94
	private void OnDisable()
	{
		if (this.rightHandEffects.PrefabPoolIds[2] == (in this.rightHandPrefab))
		{
			this.rightHandEffects.PrefabPoolIds[2] = -1;
		}
		if (this.leftHandEffects.PrefabPoolIds[2] == (in this.leftHandPrefab))
		{
			this.leftHandEffects.PrefabPoolIds[2] = -1;
		}
	}

	// Token: 0x04000BD1 RID: 3025
	[Tooltip("Must be in the global object pool and have a tag.")]
	[SerializeField]
	private HashWrapper rightHandPrefab;

	// Token: 0x04000BD2 RID: 3026
	[Tooltip("Must be in the global object pool and have a tag.")]
	[SerializeField]
	private HashWrapper leftHandPrefab;

	// Token: 0x04000BD3 RID: 3027
	private VRRig rig;

	// Token: 0x04000BD4 RID: 3028
	private HandEffectContext rightHandEffects;

	// Token: 0x04000BD5 RID: 3029
	private HandEffectContext leftHandEffects;
}
