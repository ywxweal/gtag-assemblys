using System;
using System.Linq;
using UnityEngine;

// Token: 0x020009F0 RID: 2544
public class Firework : MonoBehaviour
{
	// Token: 0x06003CEA RID: 15594 RVA: 0x001217A2 File Offset: 0x0011F9A2
	private void Launch()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (this._controller)
		{
			this._controller.Launch(this);
		}
	}

	// Token: 0x06003CEB RID: 15595 RVA: 0x001217C8 File Offset: 0x0011F9C8
	private void OnValidate()
	{
		if (!this._controller)
		{
			this._controller = base.GetComponentInParent<FireworksController>();
		}
		if (!this._controller)
		{
			return;
		}
		Firework[] array = this._controller.fireworks;
		if (array.Contains(this))
		{
			return;
		}
		array = (from x in array.Concat(new Firework[] { this })
			where x != null
			select x).ToArray<Firework>();
		this._controller.fireworks = array;
	}

	// Token: 0x06003CEC RID: 15596 RVA: 0x00121858 File Offset: 0x0011FA58
	private void OnDrawGizmos()
	{
		if (!this._controller)
		{
			return;
		}
		this._controller.RenderGizmo(this, Color.cyan);
	}

	// Token: 0x06003CED RID: 15597 RVA: 0x00121879 File Offset: 0x0011FA79
	private void OnDrawGizmosSelected()
	{
		if (!this._controller)
		{
			return;
		}
		this._controller.RenderGizmo(this, Color.yellow);
	}

	// Token: 0x04004095 RID: 16533
	[SerializeField]
	private FireworksController _controller;

	// Token: 0x04004096 RID: 16534
	[Space]
	public Transform origin;

	// Token: 0x04004097 RID: 16535
	public Transform target;

	// Token: 0x04004098 RID: 16536
	[Space]
	public Color colorOrigin = Color.cyan;

	// Token: 0x04004099 RID: 16537
	public Color colorTarget = Color.magenta;

	// Token: 0x0400409A RID: 16538
	[Space]
	public AudioSource sourceOrigin;

	// Token: 0x0400409B RID: 16539
	public AudioSource sourceTarget;

	// Token: 0x0400409C RID: 16540
	[Space]
	public ParticleSystem trail;

	// Token: 0x0400409D RID: 16541
	[Space]
	public ParticleSystem[] explosions;

	// Token: 0x0400409E RID: 16542
	[Space]
	public bool doTrail = true;

	// Token: 0x0400409F RID: 16543
	public bool doTrailAudio = true;

	// Token: 0x040040A0 RID: 16544
	public bool doExplosion = true;
}
