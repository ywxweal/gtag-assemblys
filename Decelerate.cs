using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200053C RID: 1340
public class Decelerate : MonoBehaviour
{
	// Token: 0x06002088 RID: 8328 RVA: 0x000A32B0 File Offset: 0x000A14B0
	public void Restart()
	{
		base.enabled = true;
	}

	// Token: 0x06002089 RID: 8329 RVA: 0x000A32BC File Offset: 0x000A14BC
	private void Update()
	{
		if (!this._rigidbody)
		{
			return;
		}
		Vector3 vector = this._rigidbody.velocity;
		vector *= this._friction;
		if (vector.Approx0(0.001f))
		{
			this._rigidbody.velocity = Vector3.zero;
			UnityEvent unityEvent = this.onStop;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			base.enabled = false;
		}
		else
		{
			this._rigidbody.velocity = vector;
		}
		if (this._resetOrientationOnRelease && !this._rigidbody.rotation.Approx(Quaternion.identity, 1E-06f))
		{
			this._rigidbody.rotation = Quaternion.identity;
		}
	}

	// Token: 0x04002475 RID: 9333
	[SerializeField]
	private Rigidbody _rigidbody;

	// Token: 0x04002476 RID: 9334
	[SerializeField]
	private float _friction = 0.875f;

	// Token: 0x04002477 RID: 9335
	[SerializeField]
	private bool _resetOrientationOnRelease;

	// Token: 0x04002478 RID: 9336
	public UnityEvent onStop;
}
