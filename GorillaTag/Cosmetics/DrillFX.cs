using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000DCE RID: 3534
	public class DrillFX : MonoBehaviour
	{
		// Token: 0x060057A7 RID: 22439 RVA: 0x001AEA34 File Offset: 0x001ACC34
		protected void Awake()
		{
			if (!DrillFX.appIsQuittingHandlerIsSubscribed)
			{
				DrillFX.appIsQuittingHandlerIsSubscribed = true;
				Application.quitting += DrillFX.HandleApplicationQuitting;
			}
			this.hasFX = this.fx != null;
			if (this.hasFX)
			{
				this.fxEmissionModule = this.fx.emission;
				this.fxEmissionMaxRate = this.fxEmissionModule.rateOverTimeMultiplier;
				this.fxShapeModule = this.fx.shape;
				this.fxShapeMaxRadius = this.fxShapeModule.radius;
			}
			this.hasAudio = this.loopAudio != null;
			if (this.hasAudio)
			{
				this.audioMaxVolume = this.loopAudio.volume;
				this.loopAudio.volume = 0f;
				this.loopAudio.loop = true;
				this.loopAudio.GTPlay();
			}
		}

		// Token: 0x060057A8 RID: 22440 RVA: 0x001AEB10 File Offset: 0x001ACD10
		protected void OnEnable()
		{
			if (DrillFX.appIsQuitting)
			{
				return;
			}
			if (this.hasFX)
			{
				this.fxEmissionModule.rateOverTimeMultiplier = 0f;
			}
			if (this.hasAudio)
			{
				this.loopAudio.volume = 0f;
				this.loopAudio.loop = true;
				this.loopAudio.GTPlay();
			}
			this.ValidateLineCastPositions();
		}

		// Token: 0x060057A9 RID: 22441 RVA: 0x001AEB74 File Offset: 0x001ACD74
		protected void OnDisable()
		{
			if (DrillFX.appIsQuitting)
			{
				return;
			}
			if (this.hasFX)
			{
				this.fxEmissionModule.rateOverTimeMultiplier = 0f;
			}
			if (this.hasAudio)
			{
				this.loopAudio.volume = 0f;
				this.loopAudio.GTStop();
			}
		}

		// Token: 0x060057AA RID: 22442 RVA: 0x001AEBC4 File Offset: 0x001ACDC4
		protected void LateUpdate()
		{
			if (DrillFX.appIsQuitting)
			{
				return;
			}
			Transform transform = base.transform;
			RaycastHit raycastHit;
			Vector3 vector = (Physics.Linecast(transform.TransformPoint(this.lineCastStart), transform.TransformPoint(this.lineCastEnd), out raycastHit, this.lineCastLayerMask, QueryTriggerInteraction.Ignore) ? raycastHit.point : this.lineCastEnd);
			Vector3 vector2 = transform.InverseTransformPoint(vector);
			float num = Mathf.Clamp01(Vector3.Distance(this.lineCastStart, vector2) / this.maxDepth);
			if (this.hasFX)
			{
				this.fxEmissionModule.rateOverTimeMultiplier = this.fxEmissionMaxRate * this.fxEmissionCurve.Evaluate(num);
				this.fxShapeModule.position = vector2;
				this.fxShapeModule.radius = Mathf.Lerp(this.fxShapeMaxRadius, this.fxMinRadiusScale * this.fxShapeMaxRadius, num);
			}
			if (this.hasAudio)
			{
				this.loopAudio.volume = Mathf.MoveTowards(this.loopAudio.volume, this.audioMaxVolume * this.loopAudioVolumeCurve.Evaluate(num), this.loopAudioVolumeTransitionSpeed * Time.deltaTime);
			}
		}

		// Token: 0x060057AB RID: 22443 RVA: 0x001AECDA File Offset: 0x001ACEDA
		private static void HandleApplicationQuitting()
		{
			DrillFX.appIsQuitting = true;
		}

		// Token: 0x060057AC RID: 22444 RVA: 0x001AECE4 File Offset: 0x001ACEE4
		private bool ValidateLineCastPositions()
		{
			this.maxDepth = Vector3.Distance(this.lineCastStart, this.lineCastEnd);
			if (this.maxDepth > 1E-45f)
			{
				return true;
			}
			if (Application.isPlaying)
			{
				Debug.Log("DrillFX: lineCastStart and End are too close together. Disabling component.", this);
				base.enabled = false;
			}
			return false;
		}

		// Token: 0x04005C41 RID: 23617
		[SerializeField]
		private ParticleSystem fx;

		// Token: 0x04005C42 RID: 23618
		[SerializeField]
		private AnimationCurve fxEmissionCurve;

		// Token: 0x04005C43 RID: 23619
		[SerializeField]
		private float fxMinRadiusScale = 0.01f;

		// Token: 0x04005C44 RID: 23620
		[Tooltip("Right click menu has custom menu items. Anything starting with \"- \" is custom.")]
		[SerializeField]
		private AudioSource loopAudio;

		// Token: 0x04005C45 RID: 23621
		[SerializeField]
		private AnimationCurve loopAudioVolumeCurve;

		// Token: 0x04005C46 RID: 23622
		[Tooltip("Higher value makes it reach the target volume faster.")]
		[SerializeField]
		private float loopAudioVolumeTransitionSpeed = 3f;

		// Token: 0x04005C47 RID: 23623
		[FormerlySerializedAs("layerMask")]
		[Tooltip("The collision layers the line cast should intersect with")]
		[SerializeField]
		private LayerMask lineCastLayerMask;

		// Token: 0x04005C48 RID: 23624
		[Tooltip("The position in local space that the line cast starts.")]
		[SerializeField]
		private Vector3 lineCastStart = Vector3.zero;

		// Token: 0x04005C49 RID: 23625
		[Tooltip("The position in local space that the line cast ends.")]
		[SerializeField]
		private Vector3 lineCastEnd = Vector3.forward;

		// Token: 0x04005C4A RID: 23626
		private static bool appIsQuitting;

		// Token: 0x04005C4B RID: 23627
		private static bool appIsQuittingHandlerIsSubscribed;

		// Token: 0x04005C4C RID: 23628
		private float maxDepth;

		// Token: 0x04005C4D RID: 23629
		private bool hasFX;

		// Token: 0x04005C4E RID: 23630
		private ParticleSystem.EmissionModule fxEmissionModule;

		// Token: 0x04005C4F RID: 23631
		private float fxEmissionMaxRate;

		// Token: 0x04005C50 RID: 23632
		private ParticleSystem.ShapeModule fxShapeModule;

		// Token: 0x04005C51 RID: 23633
		private float fxShapeMaxRadius;

		// Token: 0x04005C52 RID: 23634
		private bool hasAudio;

		// Token: 0x04005C53 RID: 23635
		private float audioMaxVolume;
	}
}
