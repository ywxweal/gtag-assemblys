using System;
using System.Collections.Generic;
using System.Linq;
using GorillaExtensions;
using GorillaLocomotion.Climbing;
using UnityEngine;
using UnityEngine.XR;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x02000CDC RID: 3292
	public class GorillaRopeSwing : MonoBehaviour, IBuilderPieceComponent
	{
		// Token: 0x06005196 RID: 20886 RVA: 0x0018C0B4 File Offset: 0x0018A2B4
		private void EdRecalculateId()
		{
			this.CalculateId(true);
		}

		// Token: 0x17000842 RID: 2114
		// (get) Token: 0x06005197 RID: 20887 RVA: 0x0018C0BD File Offset: 0x0018A2BD
		// (set) Token: 0x06005198 RID: 20888 RVA: 0x0018C0C5 File Offset: 0x0018A2C5
		public bool isIdle { get; private set; }

		// Token: 0x17000843 RID: 2115
		// (get) Token: 0x06005199 RID: 20889 RVA: 0x0018C0CE File Offset: 0x0018A2CE
		// (set) Token: 0x0600519A RID: 20890 RVA: 0x0018C0D6 File Offset: 0x0018A2D6
		public bool isFullyIdle { get; private set; }

		// Token: 0x17000844 RID: 2116
		// (get) Token: 0x0600519B RID: 20891 RVA: 0x0018C0DF File Offset: 0x0018A2DF
		public bool SupportsMovingAtRuntime
		{
			get
			{
				return this.supportMovingAtRuntime;
			}
		}

		// Token: 0x17000845 RID: 2117
		// (get) Token: 0x0600519C RID: 20892 RVA: 0x0018C0E7 File Offset: 0x0018A2E7
		public bool hasPlayers
		{
			get
			{
				return this.localPlayerOn || this.remotePlayers.Count > 0;
			}
		}

		// Token: 0x0600519D RID: 20893 RVA: 0x0018C104 File Offset: 0x0018A304
		private void Awake()
		{
			base.transform.rotation = Quaternion.identity;
			this.scaleFactor = (base.transform.lossyScale.x + base.transform.lossyScale.y + base.transform.lossyScale.z) / 3f;
			this.SetIsIdle(true, false);
		}

		// Token: 0x0600519E RID: 20894 RVA: 0x0018C167 File Offset: 0x0018A367
		private void Start()
		{
			if (!this.useStaticId)
			{
				this.CalculateId(false);
			}
			RopeSwingManager.Register(this);
			this.started = true;
		}

		// Token: 0x0600519F RID: 20895 RVA: 0x0018C185 File Offset: 0x0018A385
		private void OnDestroy()
		{
			if (RopeSwingManager.instance != null)
			{
				RopeSwingManager.Unregister(this);
			}
		}

		// Token: 0x060051A0 RID: 20896 RVA: 0x0018C19C File Offset: 0x0018A39C
		private void OnEnable()
		{
			base.transform.rotation = Quaternion.identity;
			this.scaleFactor = (base.transform.lossyScale.x + base.transform.lossyScale.y + base.transform.lossyScale.z) / 3f;
			this.SetIsIdle(true, true);
			VectorizedCustomRopeSimulation.Register(this);
			GorillaRopeSwingUpdateManager.RegisterRopeSwing(this);
		}

		// Token: 0x060051A1 RID: 20897 RVA: 0x0018C20B File Offset: 0x0018A40B
		private void OnDisable()
		{
			if (!this.isIdle)
			{
				this.SetIsIdle(true, true);
			}
			VectorizedCustomRopeSimulation.Unregister(this);
			GorillaRopeSwingUpdateManager.UnregisterRopeSwing(this);
		}

		// Token: 0x060051A2 RID: 20898 RVA: 0x0018C22C File Offset: 0x0018A42C
		internal void CalculateId(bool force = false)
		{
			Transform transform = base.transform;
			int staticHash = TransformUtils.GetScenePath(transform).GetStaticHash();
			int staticHash2 = base.GetType().Name.GetStaticHash();
			int num = StaticHash.Compute(staticHash, staticHash2);
			if (this.useStaticId)
			{
				if (string.IsNullOrEmpty(this.staticId) || force)
				{
					Vector3 position = transform.position;
					int num2 = StaticHash.Compute(position.x, position.y, position.z);
					int instanceID = transform.GetInstanceID();
					int num3 = StaticHash.Compute(num, num2, instanceID);
					this.staticId = string.Format("#ID_{0:X8}", num3);
				}
				this.ropeId = this.staticId.GetStaticHash();
				return;
			}
			this.ropeId = (Application.isPlaying ? num : 0);
		}

		// Token: 0x060051A3 RID: 20899 RVA: 0x0018C2E8 File Offset: 0x0018A4E8
		public void InvokeUpdate()
		{
			if (this.isIdle)
			{
				this.isFullyIdle = true;
			}
			if (!this.isIdle)
			{
				int num = -1;
				if (this.localPlayerOn)
				{
					num = this.localPlayerBoneIndex;
				}
				else if (this.remotePlayers.Count > 0)
				{
					num = this.remotePlayers.First<KeyValuePair<int, int>>().Value;
				}
				if (num >= 0 && VectorizedCustomRopeSimulation.instance.GetNodeVelocity(this, num).magnitude > 2f && !this.ropeCreakSFX.isPlaying && Mathf.RoundToInt(Time.time) % 5 == 0)
				{
					this.ropeCreakSFX.GTPlay();
				}
				if (this.localPlayerOn)
				{
					float num2 = MathUtils.Linear(this.velocityTracker.GetLatestVelocity(true).magnitude / this.scaleFactor, 0f, 10f, -0.07f, 0.5f);
					if (num2 > 0f)
					{
						GorillaTagger.Instance.DoVibration(this.localPlayerXRNode, num2, Time.deltaTime);
					}
				}
				Transform bone = this.GetBone(this.lastNodeCheckIndex);
				Vector3 nodeVelocity = VectorizedCustomRopeSimulation.instance.GetNodeVelocity(this, this.lastNodeCheckIndex);
				if (Physics.SphereCastNonAlloc(bone.position, 0.2f * this.scaleFactor, nodeVelocity.normalized, this.nodeHits, 0.4f * this.scaleFactor, this.wallLayerMask, QueryTriggerInteraction.Ignore) > 0)
				{
					this.SetVelocity(this.lastNodeCheckIndex, Vector3.zero, false, default(PhotonMessageInfoWrapped));
				}
				if (nodeVelocity.magnitude <= 0.35f)
				{
					this.potentialIdleTimer += Time.deltaTime;
				}
				else
				{
					this.potentialIdleTimer = 0f;
				}
				if (this.potentialIdleTimer >= 2f)
				{
					this.SetIsIdle(true, false);
					this.potentialIdleTimer = 0f;
				}
				this.lastNodeCheckIndex++;
				if (this.lastNodeCheckIndex > this.nodes.Length)
				{
					this.lastNodeCheckIndex = 2;
				}
			}
			if (this.hasMonkeBlockParent && this.supportMovingAtRuntime)
			{
				base.transform.rotation = Quaternion.Euler(0f, base.transform.parent.rotation.eulerAngles.y, 0f);
			}
		}

		// Token: 0x060051A4 RID: 20900 RVA: 0x0018C51C File Offset: 0x0018A71C
		private void SetIsIdle(bool idle, bool resetPos = false)
		{
			this.isIdle = idle;
			this.ropeCreakSFX.gameObject.SetActive(!idle);
			if (idle)
			{
				this.ToggleVelocityTracker(false, 0, default(Vector3));
				if (resetPos)
				{
					Vector3 vector = Vector3.zero;
					for (int i = 0; i < this.nodes.Length; i++)
					{
						this.nodes[i].transform.localRotation = Quaternion.identity;
						this.nodes[i].transform.localPosition = vector;
						vector += new Vector3(0f, -1f, 0f);
					}
					return;
				}
			}
			else
			{
				this.isFullyIdle = false;
			}
		}

		// Token: 0x060051A5 RID: 20901 RVA: 0x0018C5C1 File Offset: 0x0018A7C1
		public Transform GetBone(int index)
		{
			if (index >= this.nodes.Length)
			{
				return this.nodes.Last<Transform>();
			}
			return this.nodes[index];
		}

		// Token: 0x060051A6 RID: 20902 RVA: 0x0018C5E4 File Offset: 0x0018A7E4
		public int GetBoneIndex(Transform r)
		{
			for (int i = 0; i < this.nodes.Length; i++)
			{
				if (this.nodes[i] == r)
				{
					return i;
				}
			}
			return this.nodes.Length - 1;
		}

		// Token: 0x060051A7 RID: 20903 RVA: 0x0018C620 File Offset: 0x0018A820
		public void AttachLocalPlayer(XRNode xrNode, Transform grabbedBone, Vector3 offset, Vector3 velocity)
		{
			int boneIndex = this.GetBoneIndex(grabbedBone);
			this.localPlayerBoneIndex = boneIndex;
			velocity /= this.scaleFactor;
			velocity *= this.settings.inheritVelocityMultiplier;
			if (GorillaTagger.hasInstance && GorillaTagger.Instance.offlineVRRig)
			{
				GorillaTagger.Instance.offlineVRRig.grabbedRopeIndex = this.ropeId;
				GorillaTagger.Instance.offlineVRRig.grabbedRopeBoneIndex = boneIndex;
				GorillaTagger.Instance.offlineVRRig.grabbedRopeIsLeft = xrNode == XRNode.LeftHand;
				GorillaTagger.Instance.offlineVRRig.grabbedRopeOffset = offset;
				GorillaTagger.Instance.offlineVRRig.grabbedRopeIsPhotonView = false;
			}
			this.RefreshAllBonesMass();
			List<Vector3> list = new List<Vector3>();
			if (this.remotePlayers.Count <= 0)
			{
				foreach (Transform transform in this.nodes)
				{
					list.Add(transform.position);
				}
			}
			velocity.y = 0f;
			if (Time.time - this.lastGrabTime > 1f && (this.remotePlayers.Count == 0 || velocity.magnitude > 2.5f))
			{
				RopeSwingManager.instance.SendSetVelocity_RPC(this.ropeId, boneIndex, velocity, true);
			}
			this.lastGrabTime = Time.time;
			this.ropeCreakSFX.transform.parent = this.GetBone(Math.Max(0, boneIndex - 3)).transform;
			this.ropeCreakSFX.transform.localPosition = Vector3.zero;
			this.localPlayerOn = true;
			this.localPlayerXRNode = xrNode;
			this.ToggleVelocityTracker(true, boneIndex, offset);
		}

		// Token: 0x060051A8 RID: 20904 RVA: 0x0018C7B9 File Offset: 0x0018A9B9
		public void DetachLocalPlayer()
		{
			if (GorillaTagger.hasInstance && GorillaTagger.Instance.offlineVRRig)
			{
				GorillaTagger.Instance.offlineVRRig.grabbedRopeIndex = -1;
			}
			this.localPlayerOn = false;
			this.localPlayerBoneIndex = 0;
			this.RefreshAllBonesMass();
		}

		// Token: 0x060051A9 RID: 20905 RVA: 0x0018C7F8 File Offset: 0x0018A9F8
		private void ToggleVelocityTracker(bool enable, int boneIndex = 0, Vector3 offset = default(Vector3))
		{
			if (enable)
			{
				this.velocityTracker.transform.SetParent(this.GetBone(boneIndex));
				this.velocityTracker.transform.localPosition = offset;
				this.velocityTracker.ResetState();
			}
			this.velocityTracker.gameObject.SetActive(enable);
			if (enable)
			{
				this.velocityTracker.Tick();
			}
		}

		// Token: 0x060051AA RID: 20906 RVA: 0x0018C85C File Offset: 0x0018AA5C
		private void RefreshAllBonesMass()
		{
			int num = 0;
			foreach (KeyValuePair<int, int> keyValuePair in this.remotePlayers)
			{
				if (keyValuePair.Value > num)
				{
					num = keyValuePair.Value;
				}
			}
			if (this.localPlayerBoneIndex > num)
			{
				num = this.localPlayerBoneIndex;
			}
			VectorizedCustomRopeSimulation.instance.SetMassForPlayers(this, this.hasPlayers, num);
		}

		// Token: 0x060051AB RID: 20907 RVA: 0x0018C8E0 File Offset: 0x0018AAE0
		public bool AttachRemotePlayer(int playerId, int boneIndex, Transform offsetTransform, Vector3 offset)
		{
			Transform bone = this.GetBone(boneIndex);
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
			this.RefreshAllBonesMass();
			return true;
		}

		// Token: 0x060051AC RID: 20908 RVA: 0x0018C94D File Offset: 0x0018AB4D
		public void DetachRemotePlayer(int playerId)
		{
			this.remotePlayers.Remove(playerId);
			this.RefreshAllBonesMass();
		}

		// Token: 0x060051AD RID: 20909 RVA: 0x0018C964 File Offset: 0x0018AB64
		public void SetVelocity(int boneIndex, Vector3 velocity, bool wholeRope, PhotonMessageInfoWrapped info)
		{
			if (!base.isActiveAndEnabled)
			{
				return;
			}
			float num = 10000f;
			if (!(in velocity).IsValid(in num))
			{
				return;
			}
			velocity.x = Mathf.Clamp(velocity.x, -100f, 100f);
			velocity.y = Mathf.Clamp(velocity.y, -100f, 100f);
			velocity.z = Mathf.Clamp(velocity.z, -100f, 100f);
			boneIndex = Mathf.Clamp(boneIndex, 0, this.nodes.Length);
			Transform bone = this.GetBone(boneIndex);
			if (!bone)
			{
				return;
			}
			if (info.Sender != null && !info.Sender.IsLocal)
			{
				VRRig vrrig = GorillaGameManager.StaticFindRigForPlayer(info.Sender);
				if (!vrrig || Vector3.Distance(bone.position, vrrig.transform.position) > 5f)
				{
					return;
				}
			}
			this.SetIsIdle(false, false);
			if (bone)
			{
				VectorizedCustomRopeSimulation.instance.SetVelocity(this, velocity, wholeRope, boneIndex);
			}
		}

		// Token: 0x060051AE RID: 20910 RVA: 0x0018CA6C File Offset: 0x0018AC6C
		public void OnPieceCreate(int pieceType, int pieceId)
		{
			this.monkeBlockParent = base.GetComponentInParent<BuilderPiece>();
			this.hasMonkeBlockParent = this.monkeBlockParent != null;
			int num = StaticHash.Compute(pieceType, pieceId);
			this.staticId = string.Format("#ID_{0:X8}", num);
			this.ropeId = this.staticId.GetStaticHash();
			GorillaRopeSwing gorillaRopeSwing;
			if (this.started && !RopeSwingManager.instance.TryGetRope(this.ropeId, out gorillaRopeSwing))
			{
				RopeSwingManager.Register(this);
			}
		}

		// Token: 0x060051AF RID: 20911 RVA: 0x0018CAE8 File Offset: 0x0018ACE8
		public void OnPieceDestroy()
		{
			RopeSwingManager.Unregister(this);
		}

		// Token: 0x060051B0 RID: 20912 RVA: 0x0018CAF0 File Offset: 0x0018ACF0
		public void OnPiecePlacementDeserialized()
		{
			VectorizedCustomRopeSimulation.Unregister(this);
			base.transform.rotation = Quaternion.identity;
			this.scaleFactor = (base.transform.lossyScale.x + base.transform.lossyScale.y + base.transform.lossyScale.z) / 3f;
			this.SetIsIdle(true, true);
			VectorizedCustomRopeSimulation.Register(this);
			if (this.monkeBlockParent != null)
			{
				this.supportMovingAtRuntime = this.IsAttachedToMovingPiece();
			}
		}

		// Token: 0x060051B1 RID: 20913 RVA: 0x0018CB79 File Offset: 0x0018AD79
		public void OnPieceActivate()
		{
			if (this.monkeBlockParent != null)
			{
				this.supportMovingAtRuntime = this.IsAttachedToMovingPiece();
			}
		}

		// Token: 0x060051B2 RID: 20914 RVA: 0x0018CB98 File Offset: 0x0018AD98
		private bool IsAttachedToMovingPiece()
		{
			return this.monkeBlockParent.attachIndex >= 0 && this.monkeBlockParent.attachIndex < this.monkeBlockParent.gridPlanes.Count && this.monkeBlockParent.gridPlanes[this.monkeBlockParent.attachIndex].GetMovingParentGrid() != null;
		}

		// Token: 0x060051B3 RID: 20915 RVA: 0x0018CBF8 File Offset: 0x0018ADF8
		public void OnPieceDeactivate()
		{
			this.supportMovingAtRuntime = false;
		}

		// Token: 0x040055BD RID: 21949
		public int ropeId;

		// Token: 0x040055BE RID: 21950
		public string staticId;

		// Token: 0x040055BF RID: 21951
		public bool useStaticId;

		// Token: 0x040055C0 RID: 21952
		public const float ropeBitGenOffset = 1f;

		// Token: 0x040055C1 RID: 21953
		[SerializeField]
		private GameObject prefabRopeBit;

		// Token: 0x040055C2 RID: 21954
		[SerializeField]
		private bool supportMovingAtRuntime;

		// Token: 0x040055C3 RID: 21955
		public Transform[] nodes = Array.Empty<Transform>();

		// Token: 0x040055C4 RID: 21956
		private Dictionary<int, int> remotePlayers = new Dictionary<int, int>();

		// Token: 0x040055C5 RID: 21957
		[NonSerialized]
		public float lastGrabTime;

		// Token: 0x040055C6 RID: 21958
		[SerializeField]
		private AudioSource ropeCreakSFX;

		// Token: 0x040055C7 RID: 21959
		public GorillaVelocityTracker velocityTracker;

		// Token: 0x040055C8 RID: 21960
		private bool localPlayerOn;

		// Token: 0x040055C9 RID: 21961
		private int localPlayerBoneIndex;

		// Token: 0x040055CA RID: 21962
		private XRNode localPlayerXRNode;

		// Token: 0x040055CB RID: 21963
		private const float MAX_VELOCITY_FOR_IDLE = 0.5f;

		// Token: 0x040055CC RID: 21964
		private const float TIME_FOR_IDLE = 2f;

		// Token: 0x040055CF RID: 21967
		private float potentialIdleTimer;

		// Token: 0x040055D0 RID: 21968
		[SerializeField]
		private int ropeLength = 8;

		// Token: 0x040055D1 RID: 21969
		[SerializeField]
		private GorillaRopeSwingSettings settings;

		// Token: 0x040055D2 RID: 21970
		private bool hasMonkeBlockParent;

		// Token: 0x040055D3 RID: 21971
		private BuilderPiece monkeBlockParent;

		// Token: 0x040055D4 RID: 21972
		[NonSerialized]
		public int ropeDataStartIndex;

		// Token: 0x040055D5 RID: 21973
		[NonSerialized]
		public int ropeDataIndexOffset;

		// Token: 0x040055D6 RID: 21974
		[SerializeField]
		private LayerMask wallLayerMask;

		// Token: 0x040055D7 RID: 21975
		private RaycastHit[] nodeHits = new RaycastHit[1];

		// Token: 0x040055D8 RID: 21976
		private float scaleFactor = 1f;

		// Token: 0x040055D9 RID: 21977
		private bool started;

		// Token: 0x040055DA RID: 21978
		private int lastNodeCheckIndex = 2;
	}
}
