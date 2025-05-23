using System;
using GorillaLocomotion.Climbing;
using Unity.Mathematics;
using UnityEngine;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x02000CDF RID: 3295
	public class GorillaZipline : MonoBehaviour
	{
		// Token: 0x17000846 RID: 2118
		// (get) Token: 0x060051BD RID: 20925 RVA: 0x0018CC8A File Offset: 0x0018AE8A
		// (set) Token: 0x060051BE RID: 20926 RVA: 0x0018CC92 File Offset: 0x0018AE92
		public float currentSpeed { get; private set; }

		// Token: 0x060051BF RID: 20927 RVA: 0x0018CC9C File Offset: 0x0018AE9C
		private void FindTFromDistance(ref float t, float distance, int steps = 1000)
		{
			float num = distance / (float)steps;
			Vector3 vector = this.spline.GetPointLocal(t);
			float num2 = 0f;
			for (int i = 0; i < 1000; i++)
			{
				t += num;
				if (t >= 1f || t <= 0f)
				{
					break;
				}
				Vector3 pointLocal = this.spline.GetPointLocal(t);
				num2 += Vector3.Distance(pointLocal, vector);
				if (num2 >= Mathf.Abs(distance))
				{
					break;
				}
				vector = pointLocal;
			}
		}

		// Token: 0x060051C0 RID: 20928 RVA: 0x0018CD10 File Offset: 0x0018AF10
		private float FindSlideHelperSpot(Vector3 grabPoint)
		{
			int i = 0;
			int num = 200;
			float num2 = 0.001f;
			float num3 = 1f / (float)num;
			float3 @float = base.transform.InverseTransformPoint(grabPoint);
			float num4 = 0f;
			float num5 = float.PositiveInfinity;
			while (i < num)
			{
				float num6 = math.distancesq(this.spline.GetPointLocal(num2), @float);
				if (num6 < num5)
				{
					num5 = num6;
					num4 = num2;
				}
				num2 += num3;
				i++;
			}
			return num4;
		}

		// Token: 0x060051C1 RID: 20929 RVA: 0x0018CD8C File Offset: 0x0018AF8C
		private void Start()
		{
			this.spline = base.GetComponent<BezierSpline>();
			GorillaClimbable gorillaClimbable = this.slideHelper;
			gorillaClimbable.onBeforeClimb = (Action<GorillaHandClimber, GorillaClimbableRef>)Delegate.Combine(gorillaClimbable.onBeforeClimb, new Action<GorillaHandClimber, GorillaClimbableRef>(this.OnBeforeClimb));
		}

		// Token: 0x060051C2 RID: 20930 RVA: 0x0018CDC1 File Offset: 0x0018AFC1
		private void OnDestroy()
		{
			GorillaClimbable gorillaClimbable = this.slideHelper;
			gorillaClimbable.onBeforeClimb = (Action<GorillaHandClimber, GorillaClimbableRef>)Delegate.Remove(gorillaClimbable.onBeforeClimb, new Action<GorillaHandClimber, GorillaClimbableRef>(this.OnBeforeClimb));
		}

		// Token: 0x060051C3 RID: 20931 RVA: 0x0018CDEA File Offset: 0x0018AFEA
		public Vector3 GetCurrentDirection()
		{
			return this.spline.GetDirection(this.currentT);
		}

		// Token: 0x060051C4 RID: 20932 RVA: 0x0018CE00 File Offset: 0x0018B000
		private void OnBeforeClimb(GorillaHandClimber hand, GorillaClimbableRef climbRef)
		{
			bool flag = this.currentClimber == null;
			this.currentClimber = hand;
			if (climbRef)
			{
				this.climbOffsetHelper.SetParent(climbRef.transform);
				this.climbOffsetHelper.position = hand.transform.position;
				this.climbOffsetHelper.localPosition = new Vector3(0f, 0f, this.climbOffsetHelper.localPosition.z);
			}
			this.currentT = this.FindSlideHelperSpot(this.climbOffsetHelper.position);
			this.slideHelper.transform.localPosition = this.spline.GetPointLocal(this.currentT);
			if (flag)
			{
				Vector3 averagedVelocity = GTPlayer.Instance.AveragedVelocity;
				float num = Vector3.Dot(averagedVelocity.normalized, this.spline.GetDirection(this.currentT));
				this.currentSpeed = averagedVelocity.magnitude * num * this.currentInheritVelocityMulti;
			}
		}

		// Token: 0x060051C5 RID: 20933 RVA: 0x0018CEF4 File Offset: 0x0018B0F4
		private void Update()
		{
			if (this.currentClimber)
			{
				Vector3 direction = this.spline.GetDirection(this.currentT);
				float num = Physics.gravity.y * direction.y * this.settings.gravityMulti;
				this.currentSpeed = Mathf.MoveTowards(this.currentSpeed, this.settings.maxSpeed, num * Time.deltaTime);
				float num2 = MathUtils.Linear(this.currentSpeed, 0f, this.settings.maxFrictionSpeed, this.settings.friction, this.settings.maxFriction);
				this.currentSpeed = Mathf.MoveTowards(this.currentSpeed, 0f, num2 * Time.deltaTime);
				this.currentSpeed = Mathf.Min(this.currentSpeed, this.settings.maxSpeed);
				this.currentSpeed = Mathf.Max(this.currentSpeed, -this.settings.maxSpeed);
				float num3 = Mathf.Abs(this.currentSpeed);
				this.FindTFromDistance(ref this.currentT, this.currentSpeed * Time.deltaTime, 1000);
				this.slideHelper.transform.localPosition = this.spline.GetPointLocal(this.currentT);
				if (!this.audioSlide.gameObject.activeSelf)
				{
					this.audioSlide.gameObject.SetActive(true);
				}
				this.audioSlide.volume = MathUtils.Linear(num3, 0f, this.settings.maxSpeed, this.settings.minSlideVolume, this.settings.maxSlideVolume);
				this.audioSlide.pitch = MathUtils.Linear(num3, 0f, this.settings.maxSpeed, this.settings.minSlidePitch, this.settings.maxSlidePitch);
				if (!this.audioSlide.isPlaying)
				{
					this.audioSlide.GTPlay();
				}
				float num4 = MathUtils.Linear(num3, 0f, this.settings.maxSpeed, -0.1f, 0.75f);
				if (num4 > 0f)
				{
					GorillaTagger.Instance.DoVibration(this.currentClimber.xrNode, num4, Time.deltaTime);
				}
				if (!this.spline.Loop)
				{
					if (this.currentT >= 1f || this.currentT <= 0f)
					{
						this.currentClimber.ForceStopClimbing(false, true);
					}
				}
				else if (this.currentT >= 1f)
				{
					this.currentT = 0f;
				}
				else if (this.currentT <= 0f)
				{
					this.currentT = 1f;
				}
				if (!this.slideHelper.isBeingClimbed)
				{
					this.Stop();
				}
			}
			if (this.currentInheritVelocityMulti < 1f)
			{
				this.currentInheritVelocityMulti += Time.deltaTime * 0.2f;
				this.currentInheritVelocityMulti = Mathf.Min(this.currentInheritVelocityMulti, 1f);
			}
		}

		// Token: 0x060051C6 RID: 20934 RVA: 0x0018D1E2 File Offset: 0x0018B3E2
		private void Stop()
		{
			this.currentClimber = null;
			this.audioSlide.GTStop();
			this.audioSlide.gameObject.SetActive(false);
			this.currentInheritVelocityMulti = 0.55f;
			this.currentSpeed = 0f;
		}

		// Token: 0x040055DF RID: 21983
		[SerializeField]
		private Transform segmentsRoot;

		// Token: 0x040055E0 RID: 21984
		[SerializeField]
		private GameObject segmentPrefab;

		// Token: 0x040055E1 RID: 21985
		[SerializeField]
		private GorillaClimbable slideHelper;

		// Token: 0x040055E2 RID: 21986
		[SerializeField]
		private AudioSource audioSlide;

		// Token: 0x040055E3 RID: 21987
		private BezierSpline spline;

		// Token: 0x040055E4 RID: 21988
		[SerializeField]
		private Transform climbOffsetHelper;

		// Token: 0x040055E5 RID: 21989
		[SerializeField]
		private GorillaZiplineSettings settings;

		// Token: 0x040055E7 RID: 21991
		[SerializeField]
		private float ziplineDistance = 15f;

		// Token: 0x040055E8 RID: 21992
		[SerializeField]
		private float segmentDistance = 0.9f;

		// Token: 0x040055E9 RID: 21993
		private GorillaHandClimber currentClimber;

		// Token: 0x040055EA RID: 21994
		private float currentT;

		// Token: 0x040055EB RID: 21995
		private const float inheritVelocityRechargeRate = 0.2f;

		// Token: 0x040055EC RID: 21996
		private const float inheritVelocityValueOnRelease = 0.55f;

		// Token: 0x040055ED RID: 21997
		private float currentInheritVelocityMulti = 1f;
	}
}
