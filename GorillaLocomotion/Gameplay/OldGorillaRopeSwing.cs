using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x02000CE4 RID: 3300
	public class OldGorillaRopeSwing : MonoBehaviourPun
	{
		// Token: 0x17000848 RID: 2120
		// (get) Token: 0x060051D9 RID: 20953 RVA: 0x0018D5CC File Offset: 0x0018B7CC
		// (set) Token: 0x060051DA RID: 20954 RVA: 0x0018D5D4 File Offset: 0x0018B7D4
		public bool isIdle { get; private set; }

		// Token: 0x060051DB RID: 20955 RVA: 0x0018D5DD File Offset: 0x0018B7DD
		private void Awake()
		{
			this.SetIsIdle(true);
		}

		// Token: 0x060051DC RID: 20956 RVA: 0x0018D5E6 File Offset: 0x0018B7E6
		private void OnDisable()
		{
			if (!this.isIdle)
			{
				this.SetIsIdle(true);
			}
		}

		// Token: 0x060051DD RID: 20957 RVA: 0x0018D5F8 File Offset: 0x0018B7F8
		private void Update()
		{
			if (this.localPlayerOn && this.localGrabbedRigid)
			{
				float magnitude = this.localGrabbedRigid.velocity.magnitude;
				if (magnitude > 2.5f && !this.ropeCreakSFX.isPlaying && Mathf.RoundToInt(Time.time) % 5 == 0)
				{
					this.ropeCreakSFX.GTPlay();
				}
				float num = MathUtils.Linear(magnitude, 0f, 10f, -0.07f, 0.5f);
				if (num > 0f)
				{
					GorillaTagger.Instance.DoVibration(this.localPlayerXRNode, num, Time.deltaTime);
				}
			}
			if (!this.isIdle)
			{
				if (!this.localPlayerOn && this.remotePlayers.Count == 0)
				{
					foreach (Rigidbody rigidbody in this.bones)
					{
						float magnitude2 = rigidbody.velocity.magnitude;
						float num2 = Time.deltaTime * this.settings.frictionWhenNotHeld;
						if (num2 < magnitude2 - 0.1f)
						{
							rigidbody.velocity = Vector3.MoveTowards(rigidbody.velocity, Vector3.zero, num2);
						}
					}
				}
				bool flag = false;
				for (int j = 0; j < this.bones.Length; j++)
				{
					if (this.bones[j].velocity.magnitude > 0.1f)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					this.potentialIdleTimer += Time.deltaTime;
				}
				else
				{
					this.potentialIdleTimer = 0f;
				}
				if (this.potentialIdleTimer >= 2f)
				{
					this.SetIsIdle(true);
					this.potentialIdleTimer = 0f;
				}
			}
		}

		// Token: 0x060051DE RID: 20958 RVA: 0x0018D7A4 File Offset: 0x0018B9A4
		private void SetIsIdle(bool idle)
		{
			this.isIdle = idle;
			this.ToggleIsKinematic(idle);
			if (idle)
			{
				for (int i = 0; i < this.bones.Length; i++)
				{
					this.bones[i].velocity = Vector3.zero;
					this.bones[i].angularVelocity = Vector3.zero;
					this.bones[i].transform.localRotation = Quaternion.identity;
				}
			}
		}

		// Token: 0x060051DF RID: 20959 RVA: 0x0018D810 File Offset: 0x0018BA10
		private void ToggleIsKinematic(bool kinematic)
		{
			for (int i = 0; i < this.bones.Length; i++)
			{
				this.bones[i].isKinematic = kinematic;
				if (kinematic)
				{
					this.bones[i].interpolation = RigidbodyInterpolation.None;
				}
				else
				{
					this.bones[i].interpolation = RigidbodyInterpolation.Interpolate;
				}
			}
		}

		// Token: 0x060051E0 RID: 20960 RVA: 0x0018D85F File Offset: 0x0018BA5F
		public Rigidbody GetBone(int index)
		{
			if (index >= this.bones.Length)
			{
				return this.bones.Last<Rigidbody>();
			}
			return this.bones[index];
		}

		// Token: 0x060051E1 RID: 20961 RVA: 0x0018D880 File Offset: 0x0018BA80
		public int GetBoneIndex(Rigidbody r)
		{
			for (int i = 0; i < this.bones.Length; i++)
			{
				if (this.bones[i] == r)
				{
					return i;
				}
			}
			return this.bones.Length - 1;
		}

		// Token: 0x060051E2 RID: 20962 RVA: 0x0018D8BC File Offset: 0x0018BABC
		public void AttachLocalPlayer(XRNode xrNode, Rigidbody rigid, Vector3 offset, Vector3 velocity)
		{
			int boneIndex = this.GetBoneIndex(rigid);
			velocity *= this.settings.inheritVelocityMultiplier;
			if (GorillaTagger.hasInstance && GorillaTagger.Instance.offlineVRRig)
			{
				GorillaTagger.Instance.offlineVRRig.grabbedRopeIndex = base.photonView.ViewID;
				GorillaTagger.Instance.offlineVRRig.grabbedRopeBoneIndex = boneIndex;
				GorillaTagger.Instance.offlineVRRig.grabbedRopeIsLeft = xrNode == XRNode.LeftHand;
				GorillaTagger.Instance.offlineVRRig.grabbedRopeOffset = offset;
			}
			List<Vector3> list = new List<Vector3>();
			List<Vector3> list2 = new List<Vector3>();
			if (this.remotePlayers.Count <= 0)
			{
				foreach (Rigidbody rigidbody in this.bones)
				{
					list.Add(rigidbody.transform.localEulerAngles);
					list2.Add(rigidbody.velocity);
				}
			}
			if (Time.time - this.lastGrabTime > 1f && (this.remotePlayers.Count == 0 || velocity.magnitude > 2f))
			{
				this.SetVelocity_RPC(boneIndex, velocity, true, list.ToArray(), list2.ToArray());
			}
			this.lastGrabTime = Time.time;
			this.ropeCreakSFX.transform.parent = this.GetBone(Math.Max(0, boneIndex - 2)).transform;
			this.ropeCreakSFX.transform.localPosition = Vector3.zero;
			this.localPlayerOn = true;
			this.localPlayerXRNode = xrNode;
			this.localGrabbedRigid = rigid;
		}

		// Token: 0x060051E3 RID: 20963 RVA: 0x0018DA3F File Offset: 0x0018BC3F
		public void DetachLocalPlayer()
		{
			if (GorillaTagger.hasInstance && GorillaTagger.Instance.offlineVRRig)
			{
				GorillaTagger.Instance.offlineVRRig.grabbedRopeIndex = -1;
			}
			this.localPlayerOn = false;
			this.localGrabbedRigid = null;
		}

		// Token: 0x060051E4 RID: 20964 RVA: 0x0018DA78 File Offset: 0x0018BC78
		public bool AttachRemotePlayer(int playerId, int boneIndex, Transform offsetTransform, Vector3 offset)
		{
			Rigidbody bone = this.GetBone(boneIndex);
			if (bone == null)
			{
				return false;
			}
			offsetTransform.SetParent(bone.transform);
			offsetTransform.localPosition = offset;
			offsetTransform.localRotation = Quaternion.identity;
			if (this.remotePlayers.ContainsKey(playerId))
			{
				Debug.LogError("already on the list!");
				return false;
			}
			this.remotePlayers.Add(playerId, boneIndex);
			return true;
		}

		// Token: 0x060051E5 RID: 20965 RVA: 0x0018DADF File Offset: 0x0018BCDF
		public void DetachRemotePlayer(int playerId)
		{
			this.remotePlayers.Remove(playerId);
		}

		// Token: 0x060051E6 RID: 20966 RVA: 0x0018DAF0 File Offset: 0x0018BCF0
		public void SetVelocity_RPC(int boneIndex, Vector3 velocity, bool wholeRope = true, Vector3[] ropeRotations = null, Vector3[] ropeVelocities = null)
		{
			if (NetworkSystem.Instance.InRoom)
			{
				base.photonView.RPC("SetVelocity", RpcTarget.All, new object[] { boneIndex, velocity, wholeRope, ropeRotations, ropeVelocities });
				return;
			}
			this.SetVelocity(boneIndex, velocity, wholeRope, ropeRotations, ropeVelocities);
		}

		// Token: 0x060051E7 RID: 20967 RVA: 0x0018DB54 File Offset: 0x0018BD54
		[PunRPC]
		public void SetVelocity(int boneIndex, Vector3 velocity, bool wholeRope = true, Vector3[] ropeRotations = null, Vector3[] ropeVelocities = null)
		{
			this.SetIsIdle(false);
			if (ropeRotations != null && ropeVelocities != null && ropeRotations.Length != 0)
			{
				this.ToggleIsKinematic(true);
				for (int i = 0; i < ropeRotations.Length; i++)
				{
					if (i != 0)
					{
						this.bones[i].transform.localRotation = Quaternion.Euler(ropeRotations[i]);
						this.bones[i].velocity = ropeVelocities[i];
					}
				}
				this.ToggleIsKinematic(false);
			}
			Rigidbody bone = this.GetBone(boneIndex);
			if (bone)
			{
				if (wholeRope)
				{
					int num = 0;
					float num2 = Mathf.Min(velocity.magnitude, 15f);
					foreach (Rigidbody rigidbody in this.bones)
					{
						Vector3 vector = velocity / (float)boneIndex * (float)num;
						vector = Vector3.ClampMagnitude(vector, num2);
						rigidbody.velocity = vector;
						num++;
					}
					return;
				}
				bone.velocity = velocity;
			}
		}

		// Token: 0x04005606 RID: 22022
		public const float kPlayerMass = 0.8f;

		// Token: 0x04005607 RID: 22023
		public const float ropeBitGenOffset = 1f;

		// Token: 0x04005608 RID: 22024
		public const float MAX_ROPE_SPEED = 15f;

		// Token: 0x04005609 RID: 22025
		[SerializeField]
		private GameObject prefabRopeBit;

		// Token: 0x0400560A RID: 22026
		public Rigidbody[] bones = Array.Empty<Rigidbody>();

		// Token: 0x0400560B RID: 22027
		private Dictionary<int, int> remotePlayers = new Dictionary<int, int>();

		// Token: 0x0400560C RID: 22028
		[NonSerialized]
		public float lastGrabTime;

		// Token: 0x0400560D RID: 22029
		[SerializeField]
		private AudioSource ropeCreakSFX;

		// Token: 0x0400560E RID: 22030
		private bool localPlayerOn;

		// Token: 0x0400560F RID: 22031
		private XRNode localPlayerXRNode;

		// Token: 0x04005610 RID: 22032
		private Rigidbody localGrabbedRigid;

		// Token: 0x04005611 RID: 22033
		private const float MAX_VELOCITY_FOR_IDLE = 0.1f;

		// Token: 0x04005612 RID: 22034
		private const float TIME_FOR_IDLE = 2f;

		// Token: 0x04005614 RID: 22036
		private float potentialIdleTimer;

		// Token: 0x04005615 RID: 22037
		[Header("Config")]
		[SerializeField]
		private int ropeLength = 8;

		// Token: 0x04005616 RID: 22038
		[SerializeField]
		private GorillaRopeSwingSettings settings;
	}
}
