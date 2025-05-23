using System;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000B69 RID: 2921
	public class BuilderScaleParticles : MonoBehaviour
	{
		// Token: 0x0600484A RID: 18506 RVA: 0x00158BDC File Offset: 0x00156DDC
		private void OnEnable()
		{
			if (this.useLossyScale)
			{
				this.setScaleNextFrame = true;
				this.enableFrame = Time.frameCount;
			}
		}

		// Token: 0x0600484B RID: 18507 RVA: 0x00158BF8 File Offset: 0x00156DF8
		private void LateUpdate()
		{
			if (this.setScaleNextFrame && Time.frameCount > this.enableFrame)
			{
				if (this.useLossyScale)
				{
					this.SetScale(base.transform.lossyScale.x);
				}
				this.setScaleNextFrame = false;
			}
		}

		// Token: 0x0600484C RID: 18508 RVA: 0x00158C34 File Offset: 0x00156E34
		private void OnDisable()
		{
			if (this.useLossyScale)
			{
				this.RevertScale();
			}
		}

		// Token: 0x0600484D RID: 18509 RVA: 0x00158C44 File Offset: 0x00156E44
		public void SetScale(float inScale)
		{
			bool isPlaying = this.system.isPlaying;
			if (isPlaying)
			{
				this.system.Stop();
				this.system.Clear();
			}
			if (Mathf.Approximately(inScale, this.scale))
			{
				if (this.autoPlay || isPlaying)
				{
					this.system.Play(true);
				}
				return;
			}
			this.scale = inScale;
			this.RevertScale();
			if (Mathf.Approximately(this.scale, 1f))
			{
				if (this.autoPlay || isPlaying)
				{
					this.system.Play(true);
				}
				return;
			}
			ParticleSystem.MainModule main = this.system.main;
			this.gravityMod = main.gravityModifierMultiplier;
			main.gravityModifierMultiplier = this.gravityMod * this.scale;
			if (main.startSize3D)
			{
				ParticleSystem.MinMaxCurve startSizeX = main.startSizeX;
				this.sizeCurveXCache = main.startSizeX;
				this.ScaleCurve(ref startSizeX, this.scale);
				main.startSizeX = startSizeX;
				ParticleSystem.MinMaxCurve startSizeY = main.startSizeY;
				this.sizeCurveYCache = main.startSizeY;
				this.ScaleCurve(ref startSizeY, this.scale);
				main.startSizeY = startSizeY;
				ParticleSystem.MinMaxCurve startSizeZ = main.startSizeZ;
				this.sizeCurveZCache = main.startSizeZ;
				this.ScaleCurve(ref startSizeZ, this.scale);
				main.startSizeZ = startSizeZ;
			}
			else
			{
				ParticleSystem.MinMaxCurve startSize = main.startSize;
				this.sizeCurveCache = main.startSize;
				this.ScaleCurve(ref startSize, this.scale);
				main.startSize = startSize;
			}
			ParticleSystem.MinMaxCurve startSpeed = main.startSpeed;
			this.speedCurveCache = main.startSpeed;
			this.ScaleCurve(ref startSpeed, this.scale);
			main.startSpeed = startSpeed;
			if (this.scaleShape)
			{
				ParticleSystem.ShapeModule shape = this.system.shape;
				this.shapeScale = shape.scale;
				shape.scale = this.shapeScale * this.scale;
			}
			if (this.scaleVelocityLifetime)
			{
				ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime = this.system.velocityOverLifetime;
				this.lifetimeVelocityX = velocityOverLifetime.x;
				this.lifetimeVelocityY = velocityOverLifetime.y;
				this.lifetimeVelocityZ = velocityOverLifetime.z;
				ParticleSystem.MinMaxCurve minMaxCurve = velocityOverLifetime.x;
				this.ScaleCurve(ref minMaxCurve, this.scale);
				velocityOverLifetime.x = minMaxCurve;
				minMaxCurve = velocityOverLifetime.y;
				this.ScaleCurve(ref minMaxCurve, this.scale);
				velocityOverLifetime.y = minMaxCurve;
				minMaxCurve = velocityOverLifetime.z;
				this.ScaleCurve(ref minMaxCurve, this.scale);
				velocityOverLifetime.z = minMaxCurve;
			}
			if (this.scaleVelocityLimitLifetime)
			{
				ParticleSystem.LimitVelocityOverLifetimeModule limitVelocityOverLifetime = this.system.limitVelocityOverLifetime;
				this.limitMultiplier = limitVelocityOverLifetime.limitMultiplier;
				limitVelocityOverLifetime.limitMultiplier = this.limitMultiplier * this.scale;
			}
			if (this.scaleForceOverLife)
			{
				ParticleSystem.ForceOverLifetimeModule forceOverLifetime = this.system.forceOverLifetime;
				this.forceX = forceOverLifetime.x;
				this.forceY = forceOverLifetime.y;
				this.forceZ = forceOverLifetime.z;
				ParticleSystem.MinMaxCurve minMaxCurve2 = forceOverLifetime.x;
				this.ScaleCurve(ref minMaxCurve2, this.scale);
				forceOverLifetime.x = minMaxCurve2;
				minMaxCurve2 = forceOverLifetime.y;
				this.ScaleCurve(ref minMaxCurve2, this.scale);
				forceOverLifetime.y = minMaxCurve2;
				minMaxCurve2 = forceOverLifetime.z;
				this.ScaleCurve(ref minMaxCurve2, this.scale);
				forceOverLifetime.z = minMaxCurve2;
			}
			if (this.autoPlay || isPlaying)
			{
				this.system.Play(true);
			}
			this.shouldRevert = true;
		}

		// Token: 0x0600484E RID: 18510 RVA: 0x00158FB4 File Offset: 0x001571B4
		private void ScaleCurve(ref ParticleSystem.MinMaxCurve curve, float scale)
		{
			switch (curve.mode)
			{
			case ParticleSystemCurveMode.Constant:
				curve.constant *= scale;
				return;
			case ParticleSystemCurveMode.Curve:
			case ParticleSystemCurveMode.TwoCurves:
				curve.curveMultiplier *= scale;
				return;
			case ParticleSystemCurveMode.TwoConstants:
				curve.constantMin *= scale;
				curve.constantMax *= scale;
				return;
			default:
				return;
			}
		}

		// Token: 0x0600484F RID: 18511 RVA: 0x0015901C File Offset: 0x0015721C
		public void RevertScale()
		{
			if (!this.shouldRevert)
			{
				return;
			}
			ParticleSystem.MainModule main = this.system.main;
			main.gravityModifierMultiplier = this.gravityMod;
			main.startSpeed = this.speedCurveCache;
			if (main.startSize3D)
			{
				main.startSizeX = this.sizeCurveXCache;
				main.startSizeY = this.sizeCurveYCache;
				main.startSizeZ = this.sizeCurveZCache;
			}
			else
			{
				main.startSize = this.sizeCurveCache;
			}
			if (this.scaleShape)
			{
				this.system.shape.scale = this.shapeScale;
			}
			if (this.scaleVelocityLifetime)
			{
				ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime = this.system.velocityOverLifetime;
				velocityOverLifetime.x = this.lifetimeVelocityX;
				velocityOverLifetime.y = this.lifetimeVelocityY;
				velocityOverLifetime.z = this.lifetimeVelocityZ;
			}
			if (this.scaleVelocityLimitLifetime)
			{
				this.system.limitVelocityOverLifetime.limitMultiplier = this.limitMultiplier;
			}
			if (this.scaleForceOverLife)
			{
				ParticleSystem.ForceOverLifetimeModule forceOverLifetime = this.system.forceOverLifetime;
				forceOverLifetime.x = this.forceX;
				forceOverLifetime.y = this.forceY;
				forceOverLifetime.z = this.forceZ;
			}
			this.scale = 1f;
			this.shouldRevert = false;
		}

		// Token: 0x04004AC8 RID: 19144
		private float scale = 1f;

		// Token: 0x04004AC9 RID: 19145
		[Tooltip("Scale particles on enable using lossy scale")]
		[SerializeField]
		private bool useLossyScale;

		// Token: 0x04004ACA RID: 19146
		[Tooltip("Play particles after scaling")]
		[SerializeField]
		private bool autoPlay;

		// Token: 0x04004ACB RID: 19147
		[SerializeField]
		private ParticleSystem system;

		// Token: 0x04004ACC RID: 19148
		[SerializeField]
		private bool scaleShape;

		// Token: 0x04004ACD RID: 19149
		[SerializeField]
		private bool scaleVelocityLifetime;

		// Token: 0x04004ACE RID: 19150
		[SerializeField]
		private bool scaleVelocityLimitLifetime;

		// Token: 0x04004ACF RID: 19151
		[SerializeField]
		private bool scaleForceOverLife;

		// Token: 0x04004AD0 RID: 19152
		private float gravityMod = 1f;

		// Token: 0x04004AD1 RID: 19153
		private ParticleSystem.MinMaxCurve speedCurveCache;

		// Token: 0x04004AD2 RID: 19154
		private ParticleSystem.MinMaxCurve sizeCurveCache;

		// Token: 0x04004AD3 RID: 19155
		private ParticleSystem.MinMaxCurve sizeCurveXCache;

		// Token: 0x04004AD4 RID: 19156
		private ParticleSystem.MinMaxCurve sizeCurveYCache;

		// Token: 0x04004AD5 RID: 19157
		private ParticleSystem.MinMaxCurve sizeCurveZCache;

		// Token: 0x04004AD6 RID: 19158
		private ParticleSystem.MinMaxCurve forceX;

		// Token: 0x04004AD7 RID: 19159
		private ParticleSystem.MinMaxCurve forceY;

		// Token: 0x04004AD8 RID: 19160
		private ParticleSystem.MinMaxCurve forceZ;

		// Token: 0x04004AD9 RID: 19161
		private Vector3 shapeScale = Vector3.one;

		// Token: 0x04004ADA RID: 19162
		private ParticleSystem.MinMaxCurve lifetimeVelocityX;

		// Token: 0x04004ADB RID: 19163
		private ParticleSystem.MinMaxCurve lifetimeVelocityY;

		// Token: 0x04004ADC RID: 19164
		private ParticleSystem.MinMaxCurve lifetimeVelocityZ;

		// Token: 0x04004ADD RID: 19165
		private float limitMultiplier = 1f;

		// Token: 0x04004ADE RID: 19166
		private bool shouldRevert;

		// Token: 0x04004ADF RID: 19167
		private bool setScaleNextFrame;

		// Token: 0x04004AE0 RID: 19168
		private int enableFrame;
	}
}
