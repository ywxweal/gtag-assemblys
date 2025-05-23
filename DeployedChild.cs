using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200016C RID: 364
public class DeployedChild : MonoBehaviour
{
	// Token: 0x06000929 RID: 2345 RVA: 0x00031A1C File Offset: 0x0002FC1C
	public void Deploy(DeployableObject parent, Vector3 launchPos, Quaternion launchRot, Vector3 releaseVel, bool isRemote = false)
	{
		this._parent = parent;
		this._parent.DeployChild();
		Transform transform = base.transform;
		transform.position = launchPos;
		transform.rotation = launchRot;
		transform.localScale = this._parent.transform.lossyScale;
		this._rigidbody.velocity = releaseVel;
		this._isRemote = isRemote;
	}

	// Token: 0x0600092A RID: 2346 RVA: 0x00031A79 File Offset: 0x0002FC79
	public void ReturnToParent(float delay)
	{
		if (delay > 0f)
		{
			base.StartCoroutine(this.ReturnToParentDelayed(delay));
			return;
		}
		if (this._parent != null)
		{
			this._parent.ReturnChild();
		}
	}

	// Token: 0x0600092B RID: 2347 RVA: 0x00031AAB File Offset: 0x0002FCAB
	private IEnumerator ReturnToParentDelayed(float delay)
	{
		float start = Time.time;
		while (Time.time < start + delay)
		{
			yield return null;
		}
		if (this._parent != null)
		{
			this._parent.ReturnChild();
		}
		yield break;
	}

	// Token: 0x04000AF5 RID: 2805
	[SerializeField]
	private Rigidbody _rigidbody;

	// Token: 0x04000AF6 RID: 2806
	[SerializeReference]
	private DeployableObject _parent;

	// Token: 0x04000AF7 RID: 2807
	private bool _isRemote;
}
