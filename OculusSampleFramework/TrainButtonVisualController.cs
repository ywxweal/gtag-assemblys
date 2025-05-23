using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace OculusSampleFramework
{
	// Token: 0x02000BF4 RID: 3060
	public class TrainButtonVisualController : MonoBehaviour
	{
		// Token: 0x06004B96 RID: 19350 RVA: 0x001668F0 File Offset: 0x00164AF0
		private void Awake()
		{
			this._materialColorId = Shader.PropertyToID("_Color");
			this._buttonMaterial = this._meshRenderer.material;
			this._buttonDefaultColor = this._buttonMaterial.GetColor(this._materialColorId);
			this._oldPosition = base.transform.localPosition;
		}

		// Token: 0x06004B97 RID: 19351 RVA: 0x00166946 File Offset: 0x00164B46
		private void OnDestroy()
		{
			if (this._buttonMaterial != null)
			{
				Object.Destroy(this._buttonMaterial);
			}
		}

		// Token: 0x06004B98 RID: 19352 RVA: 0x00166964 File Offset: 0x00164B64
		private void OnEnable()
		{
			this._buttonController.InteractableStateChanged.AddListener(new UnityAction<InteractableStateArgs>(this.InteractableStateChanged));
			this._buttonController.ContactZoneEvent += this.ActionOrInContactZoneStayEvent;
			this._buttonController.ActionZoneEvent += this.ActionOrInContactZoneStayEvent;
			this._buttonInContactOrActionStates = false;
		}

		// Token: 0x06004B99 RID: 19353 RVA: 0x001669C4 File Offset: 0x00164BC4
		private void OnDisable()
		{
			if (this._buttonController != null)
			{
				this._buttonController.InteractableStateChanged.RemoveListener(new UnityAction<InteractableStateArgs>(this.InteractableStateChanged));
				this._buttonController.ContactZoneEvent -= this.ActionOrInContactZoneStayEvent;
				this._buttonController.ActionZoneEvent -= this.ActionOrInContactZoneStayEvent;
			}
		}

		// Token: 0x06004B9A RID: 19354 RVA: 0x00166A2C File Offset: 0x00164C2C
		private void ActionOrInContactZoneStayEvent(ColliderZoneArgs collisionArgs)
		{
			if (!this._buttonInContactOrActionStates || collisionArgs.CollidingTool.IsFarFieldTool)
			{
				return;
			}
			Vector3 localScale = this._buttonContactTransform.localScale;
			Vector3 interactionPosition = collisionArgs.CollidingTool.InteractionPosition;
			float num = (this._buttonContactTransform.InverseTransformPoint(interactionPosition) - 0.5f * Vector3.one).y * localScale.y;
			if (num > -this._contactMaxDisplacementDistance && num <= 0f)
			{
				base.transform.localPosition = new Vector3(this._oldPosition.x, this._oldPosition.y + num, this._oldPosition.z);
			}
		}

		// Token: 0x06004B9B RID: 19355 RVA: 0x00166ADC File Offset: 0x00164CDC
		private void InteractableStateChanged(InteractableStateArgs obj)
		{
			this._buttonInContactOrActionStates = false;
			this._glowRenderer.gameObject.SetActive(obj.NewInteractableState > InteractableState.Default);
			switch (obj.NewInteractableState)
			{
			case InteractableState.ProximityState:
				this._buttonMaterial.SetColor(this._materialColorId, this._buttonDefaultColor);
				this.LerpToOldPosition();
				return;
			case InteractableState.ContactState:
				this.StopResetLerping();
				this._buttonMaterial.SetColor(this._materialColorId, this._buttonContactColor);
				this._buttonInContactOrActionStates = true;
				return;
			case InteractableState.ActionState:
				this.StopResetLerping();
				this._buttonMaterial.SetColor(this._materialColorId, this._buttonActionColor);
				this.PlaySound(this._actionSoundEffect);
				this._buttonInContactOrActionStates = true;
				return;
			default:
				this._buttonMaterial.SetColor(this._materialColorId, this._buttonDefaultColor);
				this.LerpToOldPosition();
				return;
			}
		}

		// Token: 0x06004B9C RID: 19356 RVA: 0x00166BB7 File Offset: 0x00164DB7
		private void PlaySound(AudioClip clip)
		{
			this._audioSource.timeSamples = 0;
			this._audioSource.clip = clip;
			this._audioSource.Play();
		}

		// Token: 0x06004B9D RID: 19357 RVA: 0x00166BDC File Offset: 0x00164DDC
		private void StopResetLerping()
		{
			if (this._lerpToOldPositionCr != null)
			{
				base.StopCoroutine(this._lerpToOldPositionCr);
			}
		}

		// Token: 0x06004B9E RID: 19358 RVA: 0x00166BF4 File Offset: 0x00164DF4
		private void LerpToOldPosition()
		{
			if ((base.transform.localPosition - this._oldPosition).sqrMagnitude < Mathf.Epsilon)
			{
				return;
			}
			this.StopResetLerping();
			this._lerpToOldPositionCr = base.StartCoroutine(this.ResetPosition());
		}

		// Token: 0x06004B9F RID: 19359 RVA: 0x00166C3F File Offset: 0x00164E3F
		private IEnumerator ResetPosition()
		{
			float startTime = Time.time;
			float endTime = Time.time + 1f;
			while (Time.time < endTime)
			{
				base.transform.localPosition = Vector3.Lerp(base.transform.localPosition, this._oldPosition, (Time.time - startTime) / 1f);
				yield return null;
			}
			base.transform.localPosition = this._oldPosition;
			this._lerpToOldPositionCr = null;
			yield break;
		}

		// Token: 0x04004E2B RID: 20011
		private const float LERP_TO_OLD_POS_DURATION = 1f;

		// Token: 0x04004E2C RID: 20012
		private const float LOCAL_SIZE_HALVED = 0.5f;

		// Token: 0x04004E2D RID: 20013
		[SerializeField]
		private MeshRenderer _meshRenderer;

		// Token: 0x04004E2E RID: 20014
		[SerializeField]
		private MeshRenderer _glowRenderer;

		// Token: 0x04004E2F RID: 20015
		[SerializeField]
		private ButtonController _buttonController;

		// Token: 0x04004E30 RID: 20016
		[SerializeField]
		private Color _buttonContactColor = new Color(0.51f, 0.78f, 0.92f, 1f);

		// Token: 0x04004E31 RID: 20017
		[SerializeField]
		private Color _buttonActionColor = new Color(0.24f, 0.72f, 0.98f, 1f);

		// Token: 0x04004E32 RID: 20018
		[SerializeField]
		private AudioSource _audioSource;

		// Token: 0x04004E33 RID: 20019
		[SerializeField]
		private AudioClip _actionSoundEffect;

		// Token: 0x04004E34 RID: 20020
		[SerializeField]
		private Transform _buttonContactTransform;

		// Token: 0x04004E35 RID: 20021
		[SerializeField]
		private float _contactMaxDisplacementDistance = 0.0141f;

		// Token: 0x04004E36 RID: 20022
		private Material _buttonMaterial;

		// Token: 0x04004E37 RID: 20023
		private Color _buttonDefaultColor;

		// Token: 0x04004E38 RID: 20024
		private int _materialColorId;

		// Token: 0x04004E39 RID: 20025
		private bool _buttonInContactOrActionStates;

		// Token: 0x04004E3A RID: 20026
		private Coroutine _lerpToOldPositionCr;

		// Token: 0x04004E3B RID: 20027
		private Vector3 _oldPosition;
	}
}
