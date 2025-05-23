using System;
using UnityEngine;

// Token: 0x020009D4 RID: 2516
public class UIMatchRotation : MonoBehaviour
{
	// Token: 0x06003C37 RID: 15415 RVA: 0x0011FE85 File Offset: 0x0011E085
	private void Start()
	{
		this.referenceTransform = Camera.main.transform;
		base.transform.forward = this.x0z(this.referenceTransform.forward);
	}

	// Token: 0x06003C38 RID: 15416 RVA: 0x0011FEB4 File Offset: 0x0011E0B4
	private void Update()
	{
		Vector3 vector = this.x0z(base.transform.forward);
		Vector3 vector2 = this.x0z(this.referenceTransform.forward);
		float num = Vector3.Dot(vector, vector2);
		UIMatchRotation.State state = this.state;
		if (state != UIMatchRotation.State.Ready)
		{
			if (state != UIMatchRotation.State.Rotating)
			{
				return;
			}
			base.transform.forward = Vector3.Lerp(base.transform.forward, vector2, Time.deltaTime * this.lerpSpeed);
			if (Vector3.Dot(base.transform.forward, vector2) > 0.995f)
			{
				this.state = UIMatchRotation.State.Ready;
			}
		}
		else if (num < 1f - this.threshold)
		{
			this.state = UIMatchRotation.State.Rotating;
			return;
		}
	}

	// Token: 0x06003C39 RID: 15417 RVA: 0x0011FF58 File Offset: 0x0011E158
	private Vector3 x0z(Vector3 vector)
	{
		vector.y = 0f;
		return vector.normalized;
	}

	// Token: 0x04004051 RID: 16465
	[SerializeField]
	private Transform referenceTransform;

	// Token: 0x04004052 RID: 16466
	[SerializeField]
	private float threshold = 0.35f;

	// Token: 0x04004053 RID: 16467
	[SerializeField]
	private float lerpSpeed = 5f;

	// Token: 0x04004054 RID: 16468
	private UIMatchRotation.State state;

	// Token: 0x020009D5 RID: 2517
	private enum State
	{
		// Token: 0x04004056 RID: 16470
		Ready,
		// Token: 0x04004057 RID: 16471
		Rotating
	}
}
