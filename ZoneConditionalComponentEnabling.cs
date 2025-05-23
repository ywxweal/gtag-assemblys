using System;
using UnityEngine;

// Token: 0x02000240 RID: 576
public class ZoneConditionalComponentEnabling : MonoBehaviour
{
	// Token: 0x06000D44 RID: 3396 RVA: 0x000458BF File Offset: 0x00043ABF
	private void Start()
	{
		this.OnZoneChanged();
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
	}

	// Token: 0x06000D45 RID: 3397 RVA: 0x000458ED File Offset: 0x00043AED
	private void OnDestroy()
	{
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.OnZoneChanged));
	}

	// Token: 0x06000D46 RID: 3398 RVA: 0x00045918 File Offset: 0x00043B18
	private void OnZoneChanged()
	{
		bool flag = ZoneManagement.IsInZone(this.zone);
		bool flag2 = (this.invisibleWhileLoaded ? (!flag) : flag);
		if (this.components != null)
		{
			for (int i = 0; i < this.components.Length; i++)
			{
				if (this.components[i] != null)
				{
					this.components[i].enabled = flag2;
				}
			}
		}
		if (this.m_renderers != null)
		{
			for (int j = 0; j < this.m_renderers.Length; j++)
			{
				if (this.m_renderers[j] != null)
				{
					this.m_renderers[j].enabled = flag2;
				}
			}
		}
		if (this.m_colliders != null)
		{
			for (int k = 0; k < this.m_colliders.Length; k++)
			{
				if (this.m_colliders[k] != null)
				{
					this.m_colliders[k].enabled = flag2;
				}
			}
		}
	}

	// Token: 0x040010CF RID: 4303
	[SerializeField]
	private GTZone zone;

	// Token: 0x040010D0 RID: 4304
	[SerializeField]
	private bool invisibleWhileLoaded;

	// Token: 0x040010D1 RID: 4305
	[SerializeField]
	private Behaviour[] components;

	// Token: 0x040010D2 RID: 4306
	[SerializeField]
	private Renderer[] m_renderers;

	// Token: 0x040010D3 RID: 4307
	[SerializeField]
	private Collider[] m_colliders;
}
