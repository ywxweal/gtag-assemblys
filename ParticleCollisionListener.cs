using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000778 RID: 1912
public class ParticleCollisionListener : MonoBehaviour
{
	// Token: 0x06002F9C RID: 12188 RVA: 0x000ED0D4 File Offset: 0x000EB2D4
	private void Awake()
	{
		this._events = new List<ParticleCollisionEvent>();
	}

	// Token: 0x06002F9D RID: 12189 RVA: 0x000023F4 File Offset: 0x000005F4
	protected virtual void OnCollisionEvent(ParticleCollisionEvent ev)
	{
	}

	// Token: 0x06002F9E RID: 12190 RVA: 0x000ED0E4 File Offset: 0x000EB2E4
	public void OnParticleCollision(GameObject other)
	{
		int collisionEvents = this.target.GetCollisionEvents(other, this._events);
		for (int i = 0; i < collisionEvents; i++)
		{
			this.OnCollisionEvent(this._events[i]);
		}
	}

	// Token: 0x04003621 RID: 13857
	public ParticleSystem target;

	// Token: 0x04003622 RID: 13858
	[SerializeReference]
	private List<ParticleCollisionEvent> _events = new List<ParticleCollisionEvent>();
}
