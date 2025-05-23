using System;
using System.Collections;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000BF8 RID: 3064
	public class TrainCrossingController : MonoBehaviour
	{
		// Token: 0x06004BB6 RID: 19382 RVA: 0x00166FFA File Offset: 0x001651FA
		private void Awake()
		{
			this._lightsSide1Mat = this._lightSide1Renderer.material;
			this._lightsSide2Mat = this._lightSide2Renderer.material;
		}

		// Token: 0x06004BB7 RID: 19383 RVA: 0x0016701E File Offset: 0x0016521E
		private void OnDestroy()
		{
			if (this._lightsSide1Mat != null)
			{
				Object.Destroy(this._lightsSide1Mat);
			}
			if (this._lightsSide2Mat != null)
			{
				Object.Destroy(this._lightsSide2Mat);
			}
		}

		// Token: 0x06004BB8 RID: 19384 RVA: 0x00167052 File Offset: 0x00165252
		public void CrossingButtonStateChanged(InteractableStateArgs obj)
		{
			if (obj.NewInteractableState == InteractableState.ActionState)
			{
				this.ActivateTrainCrossing();
			}
			this._toolInteractingWithMe = ((obj.NewInteractableState > InteractableState.Default) ? obj.Tool : null);
		}

		// Token: 0x06004BB9 RID: 19385 RVA: 0x00167080 File Offset: 0x00165280
		private void Update()
		{
			if (this._toolInteractingWithMe == null)
			{
				this._selectionCylinder.CurrSelectionState = SelectionCylinder.SelectionState.Off;
				return;
			}
			this._selectionCylinder.CurrSelectionState = ((this._toolInteractingWithMe.ToolInputState == ToolInputState.PrimaryInputDown || this._toolInteractingWithMe.ToolInputState == ToolInputState.PrimaryInputDownStay) ? SelectionCylinder.SelectionState.Highlighted : SelectionCylinder.SelectionState.Selected);
		}

		// Token: 0x06004BBA RID: 19386 RVA: 0x001670D4 File Offset: 0x001652D4
		private void ActivateTrainCrossing()
		{
			int num = this._crossingSounds.Length - 1;
			AudioClip audioClip = this._crossingSounds[(int)(Random.value * (float)num)];
			this._audioSource.clip = audioClip;
			this._audioSource.timeSamples = 0;
			this._audioSource.Play();
			if (this._xingAnimationCr != null)
			{
				base.StopCoroutine(this._xingAnimationCr);
			}
			this._xingAnimationCr = base.StartCoroutine(this.AnimateCrossing(audioClip.length * 0.75f));
		}

		// Token: 0x06004BBB RID: 19387 RVA: 0x00167152 File Offset: 0x00165352
		private IEnumerator AnimateCrossing(float animationLength)
		{
			this.ToggleLightObjects(true);
			float animationEndTime = Time.time + animationLength;
			float lightBlinkDuration = animationLength * 0.1f;
			float lightBlinkStartTime = Time.time;
			float lightBlinkEndTime = Time.time + lightBlinkDuration;
			Material lightToBlinkOn = this._lightsSide1Mat;
			Material lightToBlinkOff = this._lightsSide2Mat;
			Color onColor = new Color(1f, 1f, 1f, 1f);
			Color offColor = new Color(1f, 1f, 1f, 0f);
			while (Time.time < animationEndTime)
			{
				float num = (Time.time - lightBlinkStartTime) / lightBlinkDuration;
				lightToBlinkOn.SetColor(this._colorId, Color.Lerp(offColor, onColor, num));
				lightToBlinkOff.SetColor(this._colorId, Color.Lerp(onColor, offColor, num));
				if (Time.time > lightBlinkEndTime)
				{
					Material material = lightToBlinkOn;
					lightToBlinkOn = lightToBlinkOff;
					lightToBlinkOff = material;
					lightBlinkStartTime = Time.time;
					lightBlinkEndTime = Time.time + lightBlinkDuration;
				}
				yield return null;
			}
			this.ToggleLightObjects(false);
			yield break;
		}

		// Token: 0x06004BBC RID: 19388 RVA: 0x00167168 File Offset: 0x00165368
		private void AffectMaterials(Material[] materials, Color newColor)
		{
			for (int i = 0; i < materials.Length; i++)
			{
				materials[i].SetColor(this._colorId, newColor);
			}
		}

		// Token: 0x06004BBD RID: 19389 RVA: 0x00167194 File Offset: 0x00165394
		private void ToggleLightObjects(bool enableState)
		{
			this._lightSide1Renderer.gameObject.SetActive(enableState);
			this._lightSide2Renderer.gameObject.SetActive(enableState);
		}

		// Token: 0x04004E4E RID: 20046
		[SerializeField]
		private AudioSource _audioSource;

		// Token: 0x04004E4F RID: 20047
		[SerializeField]
		private AudioClip[] _crossingSounds;

		// Token: 0x04004E50 RID: 20048
		[SerializeField]
		private MeshRenderer _lightSide1Renderer;

		// Token: 0x04004E51 RID: 20049
		[SerializeField]
		private MeshRenderer _lightSide2Renderer;

		// Token: 0x04004E52 RID: 20050
		[SerializeField]
		private SelectionCylinder _selectionCylinder;

		// Token: 0x04004E53 RID: 20051
		private Material _lightsSide1Mat;

		// Token: 0x04004E54 RID: 20052
		private Material _lightsSide2Mat;

		// Token: 0x04004E55 RID: 20053
		private int _colorId = Shader.PropertyToID("_Color");

		// Token: 0x04004E56 RID: 20054
		private Coroutine _xingAnimationCr;

		// Token: 0x04004E57 RID: 20055
		private InteractableTool _toolInteractingWithMe;
	}
}
