using System;
using Photon.Pun;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x02000CE3 RID: 3299
	public class NoncontrollableBroomstick : MonoBehaviour, IGorillaGrabable
	{
		// Token: 0x060051D0 RID: 20944 RVA: 0x0018D38C File Offset: 0x0018B58C
		private void Start()
		{
			this.smoothRotationTrackingRateExp = Mathf.Exp(this.smoothRotationTrackingRate);
			this.progressPerFixedUpdate = Time.fixedDeltaTime / this.duration;
			this.progress = this.SplineProgressOffet;
			this.secondsToCycles = 1.0 / (double)this.duration;
			if (this.unitySpline != null)
			{
				this.nativeSpline = new NativeSpline(this.unitySpline.Spline, this.unitySpline.transform.localToWorldMatrix, Allocator.Persistent);
			}
		}

		// Token: 0x060051D1 RID: 20945 RVA: 0x0018D41C File Offset: 0x0018B61C
		protected virtual void FixedUpdate()
		{
			if (PhotonNetwork.InRoom)
			{
				double num = PhotonNetwork.Time * this.secondsToCycles + (double)this.SplineProgressOffet;
				this.progress = (float)(num % 1.0);
			}
			else
			{
				this.progress = (this.progress + this.progressPerFixedUpdate) % 1f;
			}
			Quaternion quaternion = Quaternion.identity;
			if (this.unitySpline != null)
			{
				float3 @float;
				float3 float2;
				float3 float3;
				this.nativeSpline.Evaluate(this.progress, out @float, out float2, out float3);
				base.transform.position = @float;
				if (this.lookForward)
				{
					quaternion = Quaternion.LookRotation(new Vector3(float2.x, float2.y, float2.z));
				}
			}
			else if (this.spline != null)
			{
				Vector3 point = this.spline.GetPoint(this.progress, this.constantVelocity);
				base.transform.position = point;
				if (this.lookForward)
				{
					quaternion = Quaternion.LookRotation(this.spline.GetDirection(this.progress, this.constantVelocity));
				}
			}
			if (this.lookForward)
			{
				base.transform.rotation = Quaternion.Slerp(quaternion, base.transform.rotation, Mathf.Exp(-this.smoothRotationTrackingRateExp * Time.deltaTime));
			}
		}

		// Token: 0x060051D2 RID: 20946 RVA: 0x00047642 File Offset: 0x00045842
		bool IGorillaGrabable.CanBeGrabbed(GorillaGrabber grabber)
		{
			return true;
		}

		// Token: 0x060051D3 RID: 20947 RVA: 0x0018D565 File Offset: 0x0018B765
		void IGorillaGrabable.OnGrabbed(GorillaGrabber g, out Transform grabbedObject, out Vector3 grabbedLocalPosition)
		{
			grabbedObject = base.transform;
			grabbedLocalPosition = base.transform.InverseTransformPoint(g.transform.position);
		}

		// Token: 0x060051D4 RID: 20948 RVA: 0x000023F4 File Offset: 0x000005F4
		void IGorillaGrabable.OnGrabReleased(GorillaGrabber g)
		{
		}

		// Token: 0x060051D5 RID: 20949 RVA: 0x0018D58B File Offset: 0x0018B78B
		private void OnDestroy()
		{
			this.nativeSpline.Dispose();
		}

		// Token: 0x060051D6 RID: 20950 RVA: 0x0018D598 File Offset: 0x0018B798
		public bool MomentaryGrabOnly()
		{
			return this.momentaryGrabOnly;
		}

		// Token: 0x060051D8 RID: 20952 RVA: 0x0001396B File Offset: 0x00011B6B
		string IGorillaGrabable.get_name()
		{
			return base.name;
		}

		// Token: 0x040055F9 RID: 22009
		public SplineContainer unitySpline;

		// Token: 0x040055FA RID: 22010
		public BezierSpline spline;

		// Token: 0x040055FB RID: 22011
		public float duration = 30f;

		// Token: 0x040055FC RID: 22012
		public float smoothRotationTrackingRate = 0.5f;

		// Token: 0x040055FD RID: 22013
		public bool lookForward = true;

		// Token: 0x040055FE RID: 22014
		[SerializeField]
		private float SplineProgressOffet;

		// Token: 0x040055FF RID: 22015
		private float progress;

		// Token: 0x04005600 RID: 22016
		private float smoothRotationTrackingRateExp;

		// Token: 0x04005601 RID: 22017
		[SerializeField]
		private bool constantVelocity;

		// Token: 0x04005602 RID: 22018
		private float progressPerFixedUpdate;

		// Token: 0x04005603 RID: 22019
		private double secondsToCycles;

		// Token: 0x04005604 RID: 22020
		private NativeSpline nativeSpline;

		// Token: 0x04005605 RID: 22021
		[SerializeField]
		private bool momentaryGrabOnly = true;
	}
}
