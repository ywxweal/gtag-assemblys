using System;
using UnityEngine;

// Token: 0x020002E3 RID: 739
public class CharacterCameraConstraint : MonoBehaviour
{
	// Token: 0x060011B1 RID: 4529 RVA: 0x000026E9 File Offset: 0x000008E9
	private CharacterCameraConstraint()
	{
	}

	// Token: 0x060011B2 RID: 4530 RVA: 0x00054E3B File Offset: 0x0005303B
	private void Awake()
	{
		this._character = base.GetComponent<CapsuleCollider>();
		this._simplePlayerController = base.GetComponent<SimpleCapsuleWithStickMovement>();
	}

	// Token: 0x060011B3 RID: 4531 RVA: 0x00054E55 File Offset: 0x00053055
	private void OnEnable()
	{
		this._simplePlayerController.CameraUpdated += this.CameraUpdate;
	}

	// Token: 0x060011B4 RID: 4532 RVA: 0x00054E6E File Offset: 0x0005306E
	private void OnDisable()
	{
		this._simplePlayerController.CameraUpdated -= this.CameraUpdate;
	}

	// Token: 0x060011B5 RID: 4533 RVA: 0x00054E88 File Offset: 0x00053088
	private void CameraUpdate()
	{
		float num = 0f;
		if (this.CheckCameraOverlapped())
		{
			OVRScreenFade.instance.SetExplicitFade(1f);
		}
		else if (this.CheckCameraNearClipping(out num))
		{
			float num2 = Mathf.InverseLerp(0f, 0.1f, num);
			float num3 = Mathf.Lerp(0f, 1f, num2);
			OVRScreenFade.instance.SetExplicitFade(num3);
		}
		else
		{
			OVRScreenFade.instance.SetExplicitFade(0f);
		}
		float num4 = 0.25f;
		float num5 = this.CameraRig.centerEyeAnchor.localPosition.y + this.HeightOffset + num4;
		float num6 = this.MinimumHeight;
		num6 = Mathf.Min(this._character.height, num6);
		float num7 = this.MaximumHeight;
		RaycastHit raycastHit;
		if (Physics.SphereCast(this._character.transform.position, this._character.radius * 0.2f, Vector3.up, out raycastHit, this.MaximumHeight - this._character.transform.position.y, this.CollideLayers, QueryTriggerInteraction.Ignore))
		{
			num7 = raycastHit.point.y;
		}
		num7 = Mathf.Max(this._character.height, num7);
		this._character.height = Mathf.Clamp(num5, num6, num7);
		float num8 = this.HeightOffset - this._character.height * 0.5f - num4;
		this.CameraRig.transform.localPosition = new Vector3(0f, num8, 0f);
	}

	// Token: 0x060011B6 RID: 4534 RVA: 0x00055010 File Offset: 0x00053210
	private bool CheckCameraOverlapped()
	{
		Camera component = this.CameraRig.centerEyeAnchor.GetComponent<Camera>();
		Vector3 position = this._character.transform.position;
		float num = Mathf.Max(0f, this._character.height * 0.5f - component.nearClipPlane - 0.01f);
		position.y = Mathf.Clamp(this.CameraRig.centerEyeAnchor.position.y, this._character.transform.position.y - num, this._character.transform.position.y + num);
		Vector3 vector = this.CameraRig.centerEyeAnchor.position - position;
		float magnitude = vector.magnitude;
		Vector3 vector2 = vector / magnitude;
		RaycastHit raycastHit;
		return Physics.SphereCast(position, component.nearClipPlane, vector2, out raycastHit, magnitude, this.CollideLayers, QueryTriggerInteraction.Ignore);
	}

	// Token: 0x060011B7 RID: 4535 RVA: 0x00055100 File Offset: 0x00053300
	private bool CheckCameraNearClipping(out float result)
	{
		Camera component = this.CameraRig.centerEyeAnchor.GetComponent<Camera>();
		Vector3[] array = new Vector3[4];
		component.CalculateFrustumCorners(new Rect(0f, 0f, 1f, 1f), component.nearClipPlane, Camera.MonoOrStereoscopicEye.Mono, array);
		Vector3 vector = this.CameraRig.centerEyeAnchor.position + Vector3.Normalize(this.CameraRig.centerEyeAnchor.TransformVector(array[0])) * 0.25f;
		Vector3 vector2 = this.CameraRig.centerEyeAnchor.position + Vector3.Normalize(this.CameraRig.centerEyeAnchor.TransformVector(array[1])) * 0.25f;
		Vector3 vector3 = this.CameraRig.centerEyeAnchor.position + Vector3.Normalize(this.CameraRig.centerEyeAnchor.TransformVector(array[2])) * 0.25f;
		Vector3 vector4 = this.CameraRig.centerEyeAnchor.position + Vector3.Normalize(this.CameraRig.centerEyeAnchor.TransformVector(array[3])) * 0.25f;
		Vector3 vector5 = (vector2 + vector4) / 2f;
		bool flag = false;
		result = 0f;
		foreach (Vector3 vector6 in new Vector3[] { vector, vector2, vector3, vector4, vector5 })
		{
			RaycastHit raycastHit;
			if (Physics.Linecast(this.CameraRig.centerEyeAnchor.position, vector6, out raycastHit, this.CollideLayers, QueryTriggerInteraction.Ignore))
			{
				flag = true;
				result = Mathf.Max(result, Vector3.Distance(raycastHit.point, vector6));
			}
		}
		return flag;
	}

	// Token: 0x040013E1 RID: 5089
	private const float FADE_RAY_LENGTH = 0.25f;

	// Token: 0x040013E2 RID: 5090
	private const float FADE_OVERLAP_MAXIMUM = 0.1f;

	// Token: 0x040013E3 RID: 5091
	private const float FADE_AMOUNT_MAXIMUM = 1f;

	// Token: 0x040013E4 RID: 5092
	[Tooltip("This should be a reference to the OVRCameraRig that is usually a child of the PlayerController.")]
	public OVRCameraRig CameraRig;

	// Token: 0x040013E5 RID: 5093
	[Tooltip("Collision layers to be used for the purposes of fading out the screen when the HMD is inside world geometry and adjusting the capsule height.")]
	public LayerMask CollideLayers;

	// Token: 0x040013E6 RID: 5094
	[Tooltip("Offset is added to camera's real world height, effectively treating it as though the player was taller/standing higher.")]
	public float HeightOffset;

	// Token: 0x040013E7 RID: 5095
	[Tooltip("Minimum height that the character capsule can shrink to.  To disable, set to capsule's height.")]
	public float MinimumHeight;

	// Token: 0x040013E8 RID: 5096
	[Tooltip("Maximum height that the character capsule can grow to.  To disable, set to capsule's height.")]
	public float MaximumHeight;

	// Token: 0x040013E9 RID: 5097
	private CapsuleCollider _character;

	// Token: 0x040013EA RID: 5098
	private SimpleCapsuleWithStickMovement _simplePlayerController;
}
