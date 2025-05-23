using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000BC7 RID: 3015
	[RequireComponent(typeof(Rigidbody))]
	public class DistanceGrabber : OVRGrabber
	{
		// Token: 0x17000733 RID: 1843
		// (get) Token: 0x06004A7A RID: 19066 RVA: 0x001629BA File Offset: 0x00160BBA
		// (set) Token: 0x06004A7B RID: 19067 RVA: 0x001629C2 File Offset: 0x00160BC2
		public bool UseSpherecast
		{
			get
			{
				return this.m_useSpherecast;
			}
			set
			{
				this.m_useSpherecast = value;
				this.GrabVolumeEnable(!this.m_useSpherecast);
			}
		}

		// Token: 0x06004A7C RID: 19068 RVA: 0x001629DC File Offset: 0x00160BDC
		protected override void Start()
		{
			base.Start();
			Collider componentInChildren = this.m_player.GetComponentInChildren<Collider>();
			if (componentInChildren != null)
			{
				this.m_maxGrabDistance = componentInChildren.bounds.size.z * 0.5f + 3f;
			}
			else
			{
				this.m_maxGrabDistance = 12f;
			}
			if (this.m_parentHeldObject)
			{
				Debug.LogError("m_parentHeldObject incompatible with DistanceGrabber. Setting to false.");
				this.m_parentHeldObject = false;
			}
			DistanceGrabber[] array = Object.FindObjectsOfType<DistanceGrabber>();
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] != this)
				{
					this.m_otherHand = array[i];
				}
			}
		}

		// Token: 0x06004A7D RID: 19069 RVA: 0x00162A78 File Offset: 0x00160C78
		public override void Update()
		{
			base.Update();
			Debug.DrawRay(base.transform.position, base.transform.forward, Color.red, 0.1f);
			DistanceGrabbable distanceGrabbable;
			Collider collider;
			this.FindTarget(out distanceGrabbable, out collider);
			if (distanceGrabbable != this.m_target)
			{
				if (this.m_target != null)
				{
					this.m_target.Targeted = this.m_otherHand.m_target == this.m_target;
				}
				this.m_target = distanceGrabbable;
				this.m_targetCollider = collider;
				if (this.m_target != null)
				{
					this.m_target.Targeted = true;
				}
			}
		}

		// Token: 0x06004A7E RID: 19070 RVA: 0x00162B20 File Offset: 0x00160D20
		protected override void GrabBegin()
		{
			DistanceGrabbable target = this.m_target;
			Collider targetCollider = this.m_targetCollider;
			this.GrabVolumeEnable(false);
			if (target != null)
			{
				if (target.isGrabbed)
				{
					((DistanceGrabber)target.grabbedBy).OffhandGrabbed(target);
				}
				this.m_grabbedObj = target;
				this.m_grabbedObj.GrabBegin(this, targetCollider);
				base.SetPlayerIgnoreCollision(this.m_grabbedObj.gameObject, true);
				this.m_movingObjectToHand = true;
				this.m_lastPos = base.transform.position;
				this.m_lastRot = base.transform.rotation;
				Vector3 vector = targetCollider.ClosestPointOnBounds(this.m_gripTransform.position);
				if (!this.m_grabbedObj.snapPosition && !this.m_grabbedObj.snapOrientation && this.m_noSnapThreshhold > 0f && (vector - this.m_gripTransform.position).magnitude < this.m_noSnapThreshhold)
				{
					Vector3 vector2 = this.m_grabbedObj.transform.position - base.transform.position;
					this.m_movingObjectToHand = false;
					vector2 = Quaternion.Inverse(base.transform.rotation) * vector2;
					this.m_grabbedObjectPosOff = vector2;
					Quaternion quaternion = Quaternion.Inverse(base.transform.rotation) * this.m_grabbedObj.transform.rotation;
					this.m_grabbedObjectRotOff = quaternion;
					return;
				}
				this.m_grabbedObjectPosOff = this.m_gripTransform.localPosition;
				if (this.m_grabbedObj.snapOffset)
				{
					Vector3 position = this.m_grabbedObj.snapOffset.position;
					if (this.m_controller == OVRInput.Controller.LTouch)
					{
						position.x = -position.x;
					}
					this.m_grabbedObjectPosOff += position;
				}
				this.m_grabbedObjectRotOff = this.m_gripTransform.localRotation;
				if (this.m_grabbedObj.snapOffset)
				{
					this.m_grabbedObjectRotOff = this.m_grabbedObj.snapOffset.rotation * this.m_grabbedObjectRotOff;
				}
			}
		}

		// Token: 0x06004A7F RID: 19071 RVA: 0x00162D38 File Offset: 0x00160F38
		protected override void MoveGrabbedObject(Vector3 pos, Quaternion rot, bool forceTeleport = false)
		{
			if (this.m_grabbedObj == null)
			{
				return;
			}
			Rigidbody grabbedRigidbody = this.m_grabbedObj.grabbedRigidbody;
			Vector3 vector = pos + rot * this.m_grabbedObjectPosOff;
			Quaternion quaternion = rot * this.m_grabbedObjectRotOff;
			if (this.m_movingObjectToHand)
			{
				float num = this.m_objectPullVelocity * Time.deltaTime;
				Vector3 vector2 = vector - this.m_grabbedObj.transform.position;
				if (num * num * 1.1f > vector2.sqrMagnitude)
				{
					this.m_movingObjectToHand = false;
				}
				else
				{
					vector2.Normalize();
					vector = this.m_grabbedObj.transform.position + vector2 * num;
					quaternion = Quaternion.RotateTowards(this.m_grabbedObj.transform.rotation, quaternion, this.m_objectPullMaxRotationRate * Time.deltaTime);
				}
			}
			grabbedRigidbody.MovePosition(vector);
			grabbedRigidbody.MoveRotation(quaternion);
		}

		// Token: 0x06004A80 RID: 19072 RVA: 0x00162E20 File Offset: 0x00161020
		private static DistanceGrabbable HitInfoToGrabbable(RaycastHit hitInfo)
		{
			if (hitInfo.collider != null)
			{
				GameObject gameObject = hitInfo.collider.gameObject;
				return gameObject.GetComponent<DistanceGrabbable>() ?? gameObject.GetComponentInParent<DistanceGrabbable>();
			}
			return null;
		}

		// Token: 0x06004A81 RID: 19073 RVA: 0x00162E5C File Offset: 0x0016105C
		protected bool FindTarget(out DistanceGrabbable dgOut, out Collider collOut)
		{
			dgOut = null;
			collOut = null;
			float num = float.MaxValue;
			foreach (OVRGrabbable ovrgrabbable in this.m_grabCandidates.Keys)
			{
				DistanceGrabbable distanceGrabbable = ovrgrabbable as DistanceGrabbable;
				bool flag = distanceGrabbable != null && distanceGrabbable.InRange && (!distanceGrabbable.isGrabbed || distanceGrabbable.allowOffhandGrab);
				if (flag && this.m_grabObjectsInLayer >= 0)
				{
					flag = distanceGrabbable.gameObject.layer == this.m_grabObjectsInLayer;
				}
				if (flag)
				{
					for (int i = 0; i < distanceGrabbable.grabPoints.Length; i++)
					{
						Collider collider = distanceGrabbable.grabPoints[i];
						Vector3 vector = collider.ClosestPointOnBounds(this.m_gripTransform.position);
						float sqrMagnitude = (this.m_gripTransform.position - vector).sqrMagnitude;
						if (sqrMagnitude < num)
						{
							bool flag2 = true;
							if (this.m_preventGrabThroughWalls)
							{
								Ray ray = default(Ray);
								ray.direction = distanceGrabbable.transform.position - this.m_gripTransform.position;
								ray.origin = this.m_gripTransform.position;
								Debug.DrawRay(ray.origin, ray.direction, Color.red, 0.1f);
								RaycastHit raycastHit;
								if (Physics.Raycast(ray, out raycastHit, this.m_maxGrabDistance, 1 << this.m_obstructionLayer, QueryTriggerInteraction.Ignore) && (double)(collider.ClosestPointOnBounds(this.m_gripTransform.position) - this.m_gripTransform.position).magnitude > (double)raycastHit.distance * 1.1)
								{
									flag2 = false;
								}
							}
							if (flag2)
							{
								num = sqrMagnitude;
								dgOut = distanceGrabbable;
								collOut = collider;
							}
						}
					}
				}
			}
			if (dgOut == null && this.m_useSpherecast)
			{
				return this.FindTargetWithSpherecast(out dgOut, out collOut);
			}
			return dgOut != null;
		}

		// Token: 0x06004A82 RID: 19074 RVA: 0x00163078 File Offset: 0x00161278
		protected bool FindTargetWithSpherecast(out DistanceGrabbable dgOut, out Collider collOut)
		{
			dgOut = null;
			collOut = null;
			Ray ray = new Ray(this.m_gripTransform.position, this.m_gripTransform.forward);
			int num = ((this.m_grabObjectsInLayer == -1) ? (-1) : (1 << this.m_grabObjectsInLayer));
			RaycastHit raycastHit;
			if (Physics.SphereCast(ray, this.m_spherecastRadius, out raycastHit, this.m_maxGrabDistance, num))
			{
				DistanceGrabbable distanceGrabbable = null;
				Collider collider = null;
				if (raycastHit.collider != null)
				{
					distanceGrabbable = raycastHit.collider.gameObject.GetComponentInParent<DistanceGrabbable>();
					collider = ((distanceGrabbable == null) ? null : raycastHit.collider);
					if (distanceGrabbable)
					{
						dgOut = distanceGrabbable;
						collOut = collider;
					}
				}
				if (distanceGrabbable != null && this.m_preventGrabThroughWalls)
				{
					ray.direction = raycastHit.point - this.m_gripTransform.position;
					dgOut = distanceGrabbable;
					collOut = collider;
					RaycastHit raycastHit2;
					if (Physics.Raycast(ray, out raycastHit2, this.m_maxGrabDistance, 1 << this.m_obstructionLayer, QueryTriggerInteraction.Ignore))
					{
						DistanceGrabbable distanceGrabbable2 = null;
						if (raycastHit.collider != null)
						{
							distanceGrabbable2 = raycastHit2.collider.gameObject.GetComponentInParent<DistanceGrabbable>();
						}
						if (distanceGrabbable2 != distanceGrabbable && raycastHit2.distance < raycastHit.distance)
						{
							dgOut = null;
							collOut = null;
						}
					}
				}
			}
			return dgOut != null;
		}

		// Token: 0x06004A83 RID: 19075 RVA: 0x001631C9 File Offset: 0x001613C9
		protected override void GrabVolumeEnable(bool enabled)
		{
			if (this.m_useSpherecast)
			{
				enabled = false;
			}
			base.GrabVolumeEnable(enabled);
		}

		// Token: 0x06004A84 RID: 19076 RVA: 0x001631DD File Offset: 0x001613DD
		protected override void OffhandGrabbed(OVRGrabbable grabbable)
		{
			base.OffhandGrabbed(grabbable);
		}

		// Token: 0x04004D35 RID: 19765
		[SerializeField]
		private float m_spherecastRadius;

		// Token: 0x04004D36 RID: 19766
		[SerializeField]
		private float m_noSnapThreshhold = 0.05f;

		// Token: 0x04004D37 RID: 19767
		[SerializeField]
		private bool m_useSpherecast;

		// Token: 0x04004D38 RID: 19768
		[SerializeField]
		public bool m_preventGrabThroughWalls;

		// Token: 0x04004D39 RID: 19769
		[SerializeField]
		private float m_objectPullVelocity = 10f;

		// Token: 0x04004D3A RID: 19770
		private float m_objectPullMaxRotationRate = 360f;

		// Token: 0x04004D3B RID: 19771
		private bool m_movingObjectToHand;

		// Token: 0x04004D3C RID: 19772
		[SerializeField]
		private float m_maxGrabDistance;

		// Token: 0x04004D3D RID: 19773
		[SerializeField]
		private int m_grabObjectsInLayer;

		// Token: 0x04004D3E RID: 19774
		[SerializeField]
		private int m_obstructionLayer;

		// Token: 0x04004D3F RID: 19775
		private DistanceGrabber m_otherHand;

		// Token: 0x04004D40 RID: 19776
		protected DistanceGrabbable m_target;

		// Token: 0x04004D41 RID: 19777
		protected Collider m_targetCollider;
	}
}
