using System;
using UnityEngine;

// Token: 0x02000383 RID: 899
public class SceneSettings : MonoBehaviour
{
	// Token: 0x060014CD RID: 5325 RVA: 0x000655A3 File Offset: 0x000637A3
	private void Awake()
	{
		Time.fixedDeltaTime = this.m_fixedTimeStep;
		Physics.gravity = Vector3.down * 9.81f * this.m_gravityScalar;
		Physics.defaultContactOffset = this.m_defaultContactOffset;
	}

	// Token: 0x060014CE RID: 5326 RVA: 0x000655DA File Offset: 0x000637DA
	private void Start()
	{
		SceneSettings.CollidersSetContactOffset(this.m_defaultContactOffset);
	}

	// Token: 0x060014CF RID: 5327 RVA: 0x000655E8 File Offset: 0x000637E8
	private static void CollidersSetContactOffset(float contactOffset)
	{
		Collider[] array = Object.FindObjectsOfType<Collider>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].contactOffset = contactOffset;
		}
	}

	// Token: 0x0400171C RID: 5916
	[Header("Time")]
	[SerializeField]
	private float m_fixedTimeStep = 0.01f;

	// Token: 0x0400171D RID: 5917
	[Header("Physics")]
	[SerializeField]
	private float m_gravityScalar = 0.75f;

	// Token: 0x0400171E RID: 5918
	[SerializeField]
	private float m_defaultContactOffset = 0.001f;
}
