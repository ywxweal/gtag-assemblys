using System;
using GorillaLocomotion;
using Liv.Lck.GorillaTag;
using UnityEngine;

// Token: 0x02000265 RID: 613
public class LckTabletSizeManager : MonoBehaviour
{
	// Token: 0x06000E0D RID: 3597 RVA: 0x00047D30 File Offset: 0x00045F30
	private void Update()
	{
		if (!GTPlayer.Instance.IsDefaultScale && this.lckDirectGrabbable.isGrabbed)
		{
			if (this.tabletFollower.GetPlayerSizeModifier() != this._shrinkSize)
			{
				this.tabletFollower.SetPlayerSizeModifier(false, this._shrinkSize);
			}
		}
		else if (GTPlayer.Instance.IsDefaultScale && this.lckDirectGrabbable.isGrabbed && this.tabletFollower.GetPlayerSizeModifier() != 1f)
		{
			this.tabletFollower.SetPlayerSizeModifier(true, 1f);
		}
		if (!GTPlayer.Instance.IsDefaultScale && !this.lckDirectGrabbable.isGrabbed)
		{
			if (base.transform.localScale != this._shrinkVector)
			{
				this.tabletFollower.SetPlayerSizeModifier(false, this._shrinkSize);
				base.transform.localScale = this._shrinkVector;
				GameObject gameObject = Camera.main.transform.Find("LCKBodyCameraSpawner(Clone)").gameObject;
				if (gameObject != null)
				{
					gameObject.GetComponent<LckBodyCameraSpawner>().ManuallySetCameraOnNeck();
					return;
				}
			}
		}
		else if (GTPlayer.Instance.IsDefaultScale && !this.lckDirectGrabbable.isGrabbed && base.transform.localScale != Vector3.one)
		{
			this.tabletFollower.SetPlayerSizeModifier(true, 1f);
			base.transform.localScale = Vector3.one;
			GameObject gameObject2 = Camera.main.transform.Find("LCKBodyCameraSpawner(Clone)").gameObject;
			if (gameObject2 != null)
			{
				gameObject2.GetComponent<LckBodyCameraSpawner>().ManuallySetCameraOnNeck();
			}
		}
	}

	// Token: 0x04001171 RID: 4465
	[SerializeField]
	private LckDirectGrabbable lckDirectGrabbable;

	// Token: 0x04001172 RID: 4466
	[SerializeField]
	private GtTabletFollower tabletFollower;

	// Token: 0x04001173 RID: 4467
	private float _shrinkSize = 0.06f;

	// Token: 0x04001174 RID: 4468
	private Vector3 _shrinkVector = new Vector3(0.06f, 0.06f, 0.06f);
}
