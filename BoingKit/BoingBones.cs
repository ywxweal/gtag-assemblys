using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02000E50 RID: 3664
	public class BoingBones : BoingReactor
	{
		// Token: 0x06005BBD RID: 23485 RVA: 0x001C2F33 File Offset: 0x001C1133
		protected override void Register()
		{
			BoingManager.Register(this);
		}

		// Token: 0x06005BBE RID: 23486 RVA: 0x001C2F3B File Offset: 0x001C113B
		protected override void Unregister()
		{
			BoingManager.Unregister(this);
		}

		// Token: 0x06005BBF RID: 23487 RVA: 0x001C2F43 File Offset: 0x001C1143
		protected override void OnUpgrade(Version oldVersion, Version newVersion)
		{
			base.OnUpgrade(oldVersion, newVersion);
			if (oldVersion.Revision < 33)
			{
				this.TwistPropagation = false;
			}
		}

		// Token: 0x06005BC0 RID: 23488 RVA: 0x001C2F5F File Offset: 0x001C115F
		public void OnValidate()
		{
			this.RescanBoneChains();
			this.UpdateCollisionRadius();
		}

		// Token: 0x06005BC1 RID: 23489 RVA: 0x001C2F6D File Offset: 0x001C116D
		public override void OnEnable()
		{
			base.OnEnable();
			this.RescanBoneChains();
			this.Reboot();
		}

		// Token: 0x06005BC2 RID: 23490 RVA: 0x001C2F81 File Offset: 0x001C1181
		public override void OnDisable()
		{
			base.OnDisable();
			this.Restore();
		}

		// Token: 0x06005BC3 RID: 23491 RVA: 0x001C2F90 File Offset: 0x001C1190
		public void RescanBoneChains()
		{
			if (this.BoneChains == null)
			{
				return;
			}
			int num = this.BoneChains.Length;
			if (this.BoneData == null || this.BoneData.Length != num)
			{
				BoingBones.Bone[][] array = new BoingBones.Bone[num][];
				if (this.BoneData != null)
				{
					int i = 0;
					int num2 = Mathf.Min(this.BoneData.Length, num);
					while (i < num2)
					{
						array[i] = this.BoneData[i];
						i++;
					}
				}
				this.BoneData = array;
			}
			Queue<BoingBones.RescanEntry> queue = new Queue<BoingBones.RescanEntry>();
			for (int j = 0; j < num; j++)
			{
				BoingBones.Chain chain = this.BoneChains[j];
				bool flag = false;
				if (this.BoneData[j] == null)
				{
					flag = true;
				}
				if (!flag && chain.m_scannedRoot == null)
				{
					flag = true;
				}
				if (!flag && chain.m_scannedRoot != chain.Root)
				{
					flag = true;
				}
				if (!flag && chain.m_scannedExclusion != null != (chain.Exclusion != null))
				{
					flag = true;
				}
				if (!flag && chain.Exclusion != null)
				{
					if (chain.m_scannedExclusion.Length != chain.Exclusion.Length)
					{
						flag = true;
					}
					else
					{
						for (int k = 0; k < chain.m_scannedExclusion.Length; k++)
						{
							if (!(chain.m_scannedExclusion[k] == chain.Exclusion[k]))
							{
								flag = true;
								break;
							}
						}
					}
				}
				Transform transform = ((chain != null) ? chain.Root : null);
				int num3 = ((transform != null) ? Codec.HashTransformHierarchy(transform) : (-1));
				if (!flag && transform != null && chain.m_hierarchyHash != num3)
				{
					flag = true;
				}
				if (flag)
				{
					if (transform == null)
					{
						this.BoneData[j] = null;
					}
					else
					{
						chain.m_scannedRoot = chain.Root;
						chain.m_scannedExclusion = chain.Exclusion.ToArray<Transform>();
						chain.m_hierarchyHash = num3;
						chain.MaxLengthFromRoot = 0f;
						List<BoingBones.Bone> list = new List<BoingBones.Bone>();
						queue.Enqueue(new BoingBones.RescanEntry(transform, -1, 0f));
						while (queue.Count > 0)
						{
							BoingBones.RescanEntry rescanEntry = queue.Dequeue();
							if (!chain.Exclusion.Contains(rescanEntry.Transform))
							{
								int count = list.Count;
								Transform transform2 = rescanEntry.Transform;
								int[] array2 = new int[transform2.childCount];
								for (int l = 0; l < array2.Length; l++)
								{
									array2[l] = -1;
								}
								int num4 = 0;
								int m = 0;
								int childCount = transform2.childCount;
								while (m < childCount)
								{
									Transform child = transform2.GetChild(m);
									if (!chain.Exclusion.Contains(child))
									{
										float num5 = Vector3.Distance(rescanEntry.Transform.position, child.position);
										float num6 = rescanEntry.LengthFromRoot + num5;
										queue.Enqueue(new BoingBones.RescanEntry(child, count, num6));
										num4++;
									}
									m++;
								}
								chain.MaxLengthFromRoot = Mathf.Max(rescanEntry.LengthFromRoot, chain.MaxLengthFromRoot);
								BoingBones.Bone bone = new BoingBones.Bone(transform2, rescanEntry.ParentIndex, rescanEntry.LengthFromRoot);
								if (num4 > 0)
								{
									bone.ChildIndices = array2;
								}
								list.Add(bone);
							}
						}
						for (int n = 0; n < list.Count; n++)
						{
							BoingBones.Bone bone2 = list[n];
							if (bone2.ParentIndex >= 0)
							{
								BoingBones.Bone bone3 = list[bone2.ParentIndex];
								int num7 = 0;
								while (bone3.ChildIndices[num7] >= 0)
								{
									num7++;
								}
								if (num7 < bone3.ChildIndices.Length)
								{
									bone3.ChildIndices[num7] = n;
								}
							}
						}
						if (list.Count != 0)
						{
							float num8 = MathUtil.InvSafe(chain.MaxLengthFromRoot);
							for (int num9 = 0; num9 < list.Count; num9++)
							{
								BoingBones.Bone bone4 = list[num9];
								float num10 = Mathf.Clamp01(bone4.LengthFromRoot * num8);
								bone4.CollisionRadius = chain.MaxCollisionRadius * BoingBones.Chain.EvaluateCurve(chain.CollisionRadiusCurveType, num10, chain.CollisionRadiusCustomCurve);
							}
							this.BoneData[j] = list.ToArray();
							this.Reboot(j);
						}
					}
				}
			}
		}

		// Token: 0x06005BC4 RID: 23492 RVA: 0x001C33AC File Offset: 0x001C15AC
		private void UpdateCollisionRadius()
		{
			for (int i = 0; i < this.BoneData.Length; i++)
			{
				BoingBones.Chain chain = this.BoneChains[i];
				BoingBones.Bone[] array = this.BoneData[i];
				if (array != null)
				{
					float num = MathUtil.InvSafe(chain.MaxLengthFromRoot);
					foreach (BoingBones.Bone bone in array)
					{
						float num2 = Mathf.Clamp01(bone.LengthFromRoot * num);
						bone.CollisionRadius = chain.MaxCollisionRadius * BoingBones.Chain.EvaluateCurve(chain.CollisionRadiusCurveType, num2, chain.CollisionRadiusCustomCurve);
					}
				}
			}
		}

		// Token: 0x06005BC5 RID: 23493 RVA: 0x001C3434 File Offset: 0x001C1634
		public override void Reboot()
		{
			base.Reboot();
			for (int i = 0; i < this.BoneData.Length; i++)
			{
				this.Reboot(i);
			}
		}

		// Token: 0x06005BC6 RID: 23494 RVA: 0x001C3464 File Offset: 0x001C1664
		public void Reboot(int iChain)
		{
			BoingBones.Bone[] array = this.BoneData[iChain];
			if (array == null)
			{
				return;
			}
			foreach (BoingBones.Bone bone in array)
			{
				bone.Instance.PositionSpring.Reset(bone.Transform.position);
				bone.Instance.RotationSpring.Reset(bone.Transform.rotation);
				bone.CachedPositionWs = bone.Transform.position;
				bone.CachedPositionLs = bone.Transform.localPosition;
				bone.CachedRotationWs = bone.Transform.rotation;
				bone.CachedRotationLs = bone.Transform.localRotation;
				bone.CachedScaleLs = bone.Transform.localScale;
			}
			this.CachedTransformValid = true;
		}

		// Token: 0x170008F2 RID: 2290
		// (get) Token: 0x06005BC7 RID: 23495 RVA: 0x001C3528 File Offset: 0x001C1728
		internal float MinScale
		{
			get
			{
				return this.m_minScale;
			}
		}

		// Token: 0x06005BC8 RID: 23496 RVA: 0x001C3530 File Offset: 0x001C1730
		public override void PrepareExecute()
		{
			base.PrepareExecute();
			this.Params.Bits.SetBit(4, false);
			float fixedDeltaTime = Time.fixedDeltaTime;
			float num = ((this.UpdateMode == BoingManager.UpdateMode.FixedUpdate) ? fixedDeltaTime : Time.deltaTime);
			this.m_minScale = Mathf.Min(base.transform.localScale.x, Mathf.Min(base.transform.localScale.y, base.transform.localScale.z));
			for (int i = 0; i < this.BoneData.Length; i++)
			{
				BoingBones.Chain chain = this.BoneChains[i];
				BoingBones.Bone[] array = this.BoneData[i];
				if (array != null && !(chain.Root == null) && array.Length != 0)
				{
					Vector3 vector = chain.Gravity * num;
					float num2 = 0f;
					foreach (BoingBones.Bone bone in array)
					{
						if (bone.ParentIndex < 0)
						{
							if (!chain.LooseRoot)
							{
								bone.Instance.PositionSpring.Reset(bone.Transform.position);
								bone.Instance.RotationSpring.Reset(bone.Transform.rotation);
							}
							bone.LengthFromRoot = 0f;
						}
						else
						{
							BoingBones.Bone bone2 = array[bone.ParentIndex];
							float num3 = Vector3.Distance(bone.Transform.position, bone2.Transform.position);
							bone.LengthFromRoot = bone2.LengthFromRoot + num3;
							num2 = Mathf.Max(num2, bone.LengthFromRoot);
						}
					}
					float num4 = MathUtil.InvSafe(num2);
					foreach (BoingBones.Bone bone3 in array)
					{
						float num5 = bone3.LengthFromRoot * num4;
						bone3.AnimationBlend = BoingBones.Chain.EvaluateCurve(chain.AnimationBlendCurveType, num5, chain.AnimationBlendCustomCurve);
						bone3.LengthStiffness = BoingBones.Chain.EvaluateCurve(chain.LengthStiffnessCurveType, num5, chain.LengthStiffnessCustomCurve);
						bone3.LengthStiffnessT = 1f - Mathf.Pow(1f - bone3.LengthStiffness, 30f * fixedDeltaTime);
						bone3.FullyStiffToParentLength = ((bone3.ParentIndex >= 0) ? Vector3.Distance(array[bone3.ParentIndex].Transform.position, bone3.Transform.position) : 0f);
						bone3.PoseStiffness = BoingBones.Chain.EvaluateCurve(chain.PoseStiffnessCurveType, num5, chain.PoseStiffnessCustomCurve);
						bone3.BendAngleCap = chain.MaxBendAngleCap * MathUtil.Deg2Rad * BoingBones.Chain.EvaluateCurve(chain.BendAngleCapCurveType, num5, chain.BendAngleCapCustomCurve);
						bone3.CollisionRadius = chain.MaxCollisionRadius * BoingBones.Chain.EvaluateCurve(chain.CollisionRadiusCurveType, num5, chain.CollisionRadiusCustomCurve);
						bone3.SquashAndStretch = BoingBones.Chain.EvaluateCurve(chain.SquashAndStretchCurveType, num5, chain.SquashAndStretchCustomCurve);
					}
					Vector3 position = array[0].Transform.position;
					for (int l = 0; l < array.Length; l++)
					{
						BoingBones.Bone bone4 = array[l];
						float num6 = bone4.LengthFromRoot * num4;
						bone4.AnimationBlend = BoingBones.Chain.EvaluateCurve(chain.AnimationBlendCurveType, num6, chain.AnimationBlendCustomCurve);
						bone4.LengthStiffness = BoingBones.Chain.EvaluateCurve(chain.LengthStiffnessCurveType, num6, chain.LengthStiffnessCustomCurve);
						bone4.PoseStiffness = BoingBones.Chain.EvaluateCurve(chain.PoseStiffnessCurveType, num6, chain.PoseStiffnessCustomCurve);
						bone4.BendAngleCap = chain.MaxBendAngleCap * MathUtil.Deg2Rad * BoingBones.Chain.EvaluateCurve(chain.BendAngleCapCurveType, num6, chain.BendAngleCapCustomCurve);
						bone4.CollisionRadius = chain.MaxCollisionRadius * BoingBones.Chain.EvaluateCurve(chain.CollisionRadiusCurveType, num6, chain.CollisionRadiusCustomCurve);
						bone4.SquashAndStretch = BoingBones.Chain.EvaluateCurve(chain.SquashAndStretchCurveType, num6, chain.SquashAndStretchCustomCurve);
						if (l > 0)
						{
							BoingBones.Bone bone5 = bone4;
							bone5.Instance.PositionSpring.Velocity = bone5.Instance.PositionSpring.Velocity + vector;
						}
						bone4.RotationInverseWs = Quaternion.Inverse(bone4.Transform.rotation);
						bone4.SpringRotationWs = bone4.Instance.RotationSpring.ValueQuat;
						bone4.SpringRotationInverseWs = Quaternion.Inverse(bone4.SpringRotationWs);
						Vector3 vector2 = bone4.Transform.position;
						Quaternion quaternion = bone4.Transform.rotation;
						Vector3 localScale = bone4.Transform.localScale;
						if (bone4.ParentIndex >= 0)
						{
							BoingBones.Bone bone6 = array[bone4.ParentIndex];
							Vector3 position2 = bone6.Transform.position;
							Vector3 value = bone6.Instance.PositionSpring.Value;
							Vector3 vector3 = bone6.SpringRotationInverseWs * (bone4.Instance.PositionSpring.Value - value);
							Quaternion quaternion2 = bone6.SpringRotationInverseWs * bone4.Instance.RotationSpring.ValueQuat;
							Vector3 position3 = bone4.Transform.position;
							Quaternion rotation = bone4.Transform.rotation;
							Vector3 vector4 = bone6.RotationInverseWs * (position3 - position2);
							Quaternion quaternion3 = bone6.RotationInverseWs * rotation;
							float poseStiffness = bone4.PoseStiffness;
							Vector3 vector5 = Vector3.Lerp(vector3, vector4, poseStiffness);
							Quaternion quaternion4 = Quaternion.Slerp(quaternion2, quaternion3, poseStiffness);
							vector2 = value + bone6.SpringRotationWs * vector5;
							quaternion = bone6.SpringRotationWs * quaternion4;
							if (bone4.BendAngleCap < MathUtil.Pi - MathUtil.Epsilon)
							{
								Vector3 vector6 = vector2 - position;
								vector6 = VectorUtil.ClampBend(vector6, position3 - position, bone4.BendAngleCap);
								vector2 = position + vector6;
							}
						}
						if (chain.ParamsOverride == null)
						{
							bone4.Instance.PrepareExecute(ref this.Params, vector2, quaternion, localScale, true);
						}
						else
						{
							bone4.Instance.PrepareExecute(ref chain.ParamsOverride.Params, vector2, quaternion, localScale, true);
						}
					}
				}
			}
		}

		// Token: 0x06005BC9 RID: 23497 RVA: 0x001C3B30 File Offset: 0x001C1D30
		public void AccumulateTarget(ref BoingEffector.Params effector, float dt)
		{
			for (int i = 0; i < this.BoneData.Length; i++)
			{
				BoingBones.Chain chain = this.BoneChains[i];
				BoingBones.Bone[] array = this.BoneData[i];
				if (array != null && chain.EffectorReaction)
				{
					foreach (BoingBones.Bone bone in array)
					{
						if (chain.ParamsOverride == null)
						{
							bone.Instance.AccumulateTarget(ref this.Params, ref effector, dt);
						}
						else
						{
							Bits32 bits = chain.ParamsOverride.Params.Bits;
							chain.ParamsOverride.Params.Bits = this.Params.Bits;
							bone.Instance.AccumulateTarget(ref chain.ParamsOverride.Params, ref effector, dt);
							chain.ParamsOverride.Params.Bits = bits;
						}
					}
				}
			}
		}

		// Token: 0x06005BCA RID: 23498 RVA: 0x001C3C18 File Offset: 0x001C1E18
		public void EndAccumulateTargets()
		{
			for (int i = 0; i < this.BoneData.Length; i++)
			{
				BoingBones.Chain chain = this.BoneChains[i];
				BoingBones.Bone[] array = this.BoneData[i];
				if (array != null)
				{
					foreach (BoingBones.Bone bone in array)
					{
						if (chain.ParamsOverride == null)
						{
							bone.Instance.EndAccumulateTargets(ref this.Params);
						}
						else
						{
							bone.Instance.EndAccumulateTargets(ref chain.ParamsOverride.Params);
						}
					}
				}
			}
		}

		// Token: 0x06005BCB RID: 23499 RVA: 0x001C3C9C File Offset: 0x001C1E9C
		public override void Restore()
		{
			if (!this.CachedTransformValid)
			{
				return;
			}
			for (int i = 0; i < this.BoneData.Length; i++)
			{
				BoingBones.Chain chain = this.BoneChains[i];
				BoingBones.Bone[] array = this.BoneData[i];
				if (array != null)
				{
					for (int j = 0; j < array.Length; j++)
					{
						BoingBones.Bone bone = array[j];
						if (j != 0 || chain.LooseRoot)
						{
							bone.Transform.localPosition = bone.CachedPositionLs;
							bone.Transform.localRotation = bone.CachedRotationLs;
							bone.Transform.localScale = bone.CachedScaleLs;
						}
					}
				}
			}
		}

		// Token: 0x04005FA4 RID: 24484
		[SerializeField]
		internal BoingBones.Bone[][] BoneData;

		// Token: 0x04005FA5 RID: 24485
		public BoingBones.Chain[] BoneChains = new BoingBones.Chain[1];

		// Token: 0x04005FA6 RID: 24486
		public bool TwistPropagation = true;

		// Token: 0x04005FA7 RID: 24487
		[Range(0.1f, 20f)]
		public float MaxCollisionResolutionSpeed = 3f;

		// Token: 0x04005FA8 RID: 24488
		public BoingBoneCollider[] BoingColliders = new BoingBoneCollider[0];

		// Token: 0x04005FA9 RID: 24489
		public Collider[] UnityColliders = new Collider[0];

		// Token: 0x04005FAA RID: 24490
		public bool DebugDrawRawBones;

		// Token: 0x04005FAB RID: 24491
		public bool DebugDrawTargetBones;

		// Token: 0x04005FAC RID: 24492
		public bool DebugDrawBoingBones;

		// Token: 0x04005FAD RID: 24493
		public bool DebugDrawFinalBones;

		// Token: 0x04005FAE RID: 24494
		public bool DebugDrawColliders;

		// Token: 0x04005FAF RID: 24495
		public bool DebugDrawChainBounds;

		// Token: 0x04005FB0 RID: 24496
		public bool DebugDrawBoneNames;

		// Token: 0x04005FB1 RID: 24497
		public bool DebugDrawLengthFromRoot;

		// Token: 0x04005FB2 RID: 24498
		private float m_minScale = 1f;

		// Token: 0x02000E51 RID: 3665
		[Serializable]
		public class Bone
		{
			// Token: 0x06005BCD RID: 23501 RVA: 0x001C3D88 File Offset: 0x001C1F88
			internal void UpdateBounds()
			{
				this.Bounds = new Bounds(this.Instance.PositionSpring.Value, 2f * this.CollisionRadius * Vector3.one);
			}

			// Token: 0x06005BCE RID: 23502 RVA: 0x001C3DBC File Offset: 0x001C1FBC
			internal Bone(Transform transform, int iParent, float lengthFromRoot)
			{
				this.Transform = transform;
				this.RotationInverseWs = Quaternion.identity;
				this.ParentIndex = iParent;
				this.LengthFromRoot = lengthFromRoot;
				this.Instance.Reset();
				this.CachedPositionWs = transform.position;
				this.CachedPositionLs = transform.localPosition;
				this.CachedRotationWs = transform.rotation;
				this.CachedRotationLs = transform.localRotation;
				this.CachedScaleLs = transform.localScale;
				this.AnimationBlend = 0f;
				this.LengthStiffness = 0f;
				this.PoseStiffness = 0f;
				this.BendAngleCap = 180f;
				this.CollisionRadius = 0f;
			}

			// Token: 0x04005FB3 RID: 24499
			internal BoingWork.Params.InstanceData Instance;

			// Token: 0x04005FB4 RID: 24500
			internal Transform Transform;

			// Token: 0x04005FB5 RID: 24501
			internal Vector3 ScaleWs;

			// Token: 0x04005FB6 RID: 24502
			internal Vector3 CachedScaleLs;

			// Token: 0x04005FB7 RID: 24503
			internal Vector3 BlendedPositionWs;

			// Token: 0x04005FB8 RID: 24504
			internal Vector3 BlendedScaleLs;

			// Token: 0x04005FB9 RID: 24505
			internal Vector3 CachedPositionWs;

			// Token: 0x04005FBA RID: 24506
			internal Vector3 CachedPositionLs;

			// Token: 0x04005FBB RID: 24507
			internal Bounds Bounds;

			// Token: 0x04005FBC RID: 24508
			internal Quaternion RotationInverseWs;

			// Token: 0x04005FBD RID: 24509
			internal Quaternion SpringRotationWs;

			// Token: 0x04005FBE RID: 24510
			internal Quaternion SpringRotationInverseWs;

			// Token: 0x04005FBF RID: 24511
			internal Quaternion CachedRotationWs;

			// Token: 0x04005FC0 RID: 24512
			internal Quaternion CachedRotationLs;

			// Token: 0x04005FC1 RID: 24513
			internal Quaternion BlendedRotationWs;

			// Token: 0x04005FC2 RID: 24514
			internal Quaternion RotationBackPropDeltaPs;

			// Token: 0x04005FC3 RID: 24515
			internal int ParentIndex;

			// Token: 0x04005FC4 RID: 24516
			internal int[] ChildIndices;

			// Token: 0x04005FC5 RID: 24517
			internal float LengthFromRoot;

			// Token: 0x04005FC6 RID: 24518
			internal float AnimationBlend;

			// Token: 0x04005FC7 RID: 24519
			internal float LengthStiffness;

			// Token: 0x04005FC8 RID: 24520
			internal float LengthStiffnessT;

			// Token: 0x04005FC9 RID: 24521
			internal float FullyStiffToParentLength;

			// Token: 0x04005FCA RID: 24522
			internal float PoseStiffness;

			// Token: 0x04005FCB RID: 24523
			internal float BendAngleCap;

			// Token: 0x04005FCC RID: 24524
			internal float CollisionRadius;

			// Token: 0x04005FCD RID: 24525
			internal float SquashAndStretch;
		}

		// Token: 0x02000E52 RID: 3666
		[Serializable]
		public class Chain
		{
			// Token: 0x06005BCF RID: 23503 RVA: 0x001C3E70 File Offset: 0x001C2070
			public static float EvaluateCurve(BoingBones.Chain.CurveType type, float t, AnimationCurve curve)
			{
				switch (type)
				{
				case BoingBones.Chain.CurveType.ConstantOne:
					return 1f;
				case BoingBones.Chain.CurveType.ConstantHalf:
					return 0.5f;
				case BoingBones.Chain.CurveType.ConstantZero:
					return 0f;
				case BoingBones.Chain.CurveType.RootOneTailHalf:
					return 1f - 0.5f * Mathf.Clamp01(t);
				case BoingBones.Chain.CurveType.RootOneTailZero:
					return 1f - Mathf.Clamp01(t);
				case BoingBones.Chain.CurveType.RootHalfTailOne:
					return 0.5f + 0.5f * Mathf.Clamp01(t);
				case BoingBones.Chain.CurveType.RootZeroTailOne:
					return Mathf.Clamp01(t);
				case BoingBones.Chain.CurveType.Custom:
					return curve.Evaluate(t);
				default:
					return 0f;
				}
			}

			// Token: 0x04005FCE RID: 24526
			[Tooltip("Root Transform object from which to build a chain (or tree if a bone has multiple children) of bouncy boing bones.")]
			public Transform Root;

			// Token: 0x04005FCF RID: 24527
			[Tooltip("List of Transform objects to exclude from chain building.")]
			public Transform[] Exclusion;

			// Token: 0x04005FD0 RID: 24528
			[Tooltip("Enable to allow reaction to boing effectors.")]
			public bool EffectorReaction = true;

			// Token: 0x04005FD1 RID: 24529
			[Tooltip("Enable to allow root Transform object to be sprung around as well. Otherwise, no effects will be applied to the root Transform object.")]
			public bool LooseRoot;

			// Token: 0x04005FD2 RID: 24530
			[Tooltip("Assign a SharedParamsOverride asset to override the parameters for this chain. Useful for chains using different parameters than that of the BoingBones component.")]
			public SharedBoingParams ParamsOverride;

			// Token: 0x04005FD3 RID: 24531
			[ConditionalField(null, null, null, null, null, null, null, Label = "Animation Blend", Tooltip = "Animation blend determines each bone's final transform between the original raw transform and its corresponding boing bone. 1.0 means 100% contribution from raw (or animated) transform. 0.0 means 100% contribution from boing bone.\n\nEach curve type provides a type of mapping for each bone's percentage down the chain (0.0 at root & 1.0 at maximum chain length) to the bone's animation blend:\n\n - Constant One: 1.0 all the way.\n - Constant Half: 0.5 all the way.\n - Constant Zero: 0.0 all the way.\n - Root One Tail Half: 1.0 at 0% chain length and 0.5 at 100% chain length.\n - Root One Tail Zero: 1.0 at 0% chain length and 0.0 at 100% chain length.\n - Root Half Tail One: 0.5 at 0% chain length and 1.0 at 100% chain length.\n - Root Zero Tail One: 0.0 at 0% chain length and 1.0 at 100% chain length.\n - Custom: Custom curve.")]
			public BoingBones.Chain.CurveType AnimationBlendCurveType = BoingBones.Chain.CurveType.RootOneTailZero;

			// Token: 0x04005FD4 RID: 24532
			[ConditionalField("AnimationBlendCurveType", BoingBones.Chain.CurveType.Custom, null, null, null, null, null, Label = "  Custom Curve")]
			public AnimationCurve AnimationBlendCustomCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

			// Token: 0x04005FD5 RID: 24533
			[ConditionalField(null, null, null, null, null, null, null, Label = "Length Stiffness", Tooltip = "Length stiffness determines how much each target bone (target transform each boing bone is sprung towards) tries to maintain original distance from its parent. 1.0 means 100% distance maintenance. 0.0 means 0% distance maintenance.\n\nEach curve type provides a type of mapping for each bone's percentage down the chain (0.0 at root & 1.0 at maximum chain length) to the bone's length stiffness:\n\n - Constant One: 1.0 all the way.\n - Constant Half: 0.5 all the way.\n - Constant Zero: 0.0 all the way.\n - Root One Tail Half: 1.0 at 0% chain length and 0.5 at 100% chain length.\n - Root One Tail Zero: 1.0 at 0% chain length and 0.0 at 100% chain length.\n - Root Half Tail One: 0.5 at 0% chain length and 1.0 at 100% chain length.\n - Root Zero Tail One: 0.0 at 0% chain length and 1.0 at 100% chain length.\n - Custom: Custom curve.")]
			public BoingBones.Chain.CurveType LengthStiffnessCurveType;

			// Token: 0x04005FD6 RID: 24534
			[ConditionalField("LengthStiffnessCurveType", BoingBones.Chain.CurveType.Custom, null, null, null, null, null, Label = "  Custom Curve")]
			public AnimationCurve LengthStiffnessCustomCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

			// Token: 0x04005FD7 RID: 24535
			[ConditionalField(null, null, null, null, null, null, null, Label = "Pose Stiffness", Tooltip = "Pose stiffness determines how much each target bone (target transform each boing bone is sprung towards) tries to maintain original transform. 1.0 means 100% original transform maintenance. 0.0 means 0% original transform maintenance.\n\nEach curve type provides a type of mapping for each bone's percentage down the chain (0.0 at root & 1.0 at maximum chain length) to the bone's pose stiffness:\n\n - Constant One: 1.0 all the way.\n - Constant Half: 0.5 all the way.\n - Constant Zero: 0.0 all the way.\n - Root One Tail Half: 1.0 at 0% chain length and 0.5 at 100% chain length.\n - Root One Tail Zero: 1.0 at 0% chain length and 0.0 at 100% chain length.\n - Root Half Tail One: 0.5 at 0% chain length and 1.0 at 100% chain length.\n - Root Zero Tail One: 0.0 at 0% chain length and 1.0 at 100% chain length.\n - Custom: Custom curve.")]
			public BoingBones.Chain.CurveType PoseStiffnessCurveType;

			// Token: 0x04005FD8 RID: 24536
			[ConditionalField("PoseStiffnessCurveType", BoingBones.Chain.CurveType.Custom, null, null, null, null, null, Label = "  Custom Curve")]
			public AnimationCurve PoseStiffnessCustomCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

			// Token: 0x04005FD9 RID: 24537
			[ConditionalField(null, null, null, null, null, null, null, Label = "Bend Angle Cap", Tooltip = "Maximum bone bend angle cap.", Min = 0f, Max = 180f)]
			public float MaxBendAngleCap = 180f;

			// Token: 0x04005FDA RID: 24538
			[ConditionalField(null, null, null, null, null, null, null, Label = "  Curve Type", Tooltip = "Percentage(0.0 = 0 %; 1.0 = 100 %) of maximum bone bend angle cap.Bend angle cap limits how much each bone can bend relative to the root (in degrees). 1.0 means 100% maximum bend angle cap. 0.0 means 0% maximum bend angle cap.\n\nEach curve type provides a type of mapping for each bone's percentage down the chain (0.0 at root & 1.0 at maximum chain length) to the bone's pose stiffness:\n\n - Constant One: 1.0 all the way.\n - Constant Half: 0.5 all the way.\n - Constant Zero: 0.0 all the way.\n - Root One Tail Half: 1.0 at 0% chain length and 0.5 at 100% chain length.\n - Root One Tail Zero: 1.0 at 0% chain length and 0.0 at 100% chain length.\n - Root Half Tail One: 0.5 at 0% chain length and 1.0 at 100% chain length.\n - Root Zero Tail One: 0.0 at 0% chain length and 1.0 at 100% chain length.\n - Custom: Custom curve.")]
			public BoingBones.Chain.CurveType BendAngleCapCurveType;

			// Token: 0x04005FDB RID: 24539
			[ConditionalField("BendAngleCapCurveType", BoingBones.Chain.CurveType.Custom, null, null, null, null, null, Label = "    Custom Curve")]
			public AnimationCurve BendAngleCapCustomCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

			// Token: 0x04005FDC RID: 24540
			[ConditionalField(null, null, null, null, null, null, null, Label = "Collision Radius", Tooltip = "Maximum bone collision radius.")]
			public float MaxCollisionRadius = 0.1f;

			// Token: 0x04005FDD RID: 24541
			[ConditionalField(null, null, null, null, null, null, null, Label = "  Curve Type", Tooltip = "Percentage (0.0 = 0%; 1.0 = 100%) of maximum bone collision radius.\n\nEach curve type provides a type of mapping for each bone's percentage down the chain (0.0 at root & 1.0 at maximum chain length) to the bone's collision radius:\n\n - Constant One: 1.0 all the way.\n - Constant Half: 0.5 all the way.\n - Constant Zero: 0.0 all the way.\n - Root One Tail Half: 1.0 at 0% chain length and 0.5 at 100% chain length.\n - Root One Tail Zero: 1.0 at 0% chain length and 0.0 at 100% chain length.\n - Root Half Tail One: 0.5 at 0% chain length and 1.0 at 100% chain length.\n - Root Zero Tail One: 0.0 at 0% chain length and 1.0 at 100% chain length.\n - Custom: Custom curve.")]
			public BoingBones.Chain.CurveType CollisionRadiusCurveType;

			// Token: 0x04005FDE RID: 24542
			[ConditionalField("CollisionRadiusCurveType", BoingBones.Chain.CurveType.Custom, null, null, null, null, null, Label = "    Custom Curve")]
			public AnimationCurve CollisionRadiusCustomCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

			// Token: 0x04005FDF RID: 24543
			[ConditionalField(null, null, null, null, null, null, null, Label = "Boing Kit Collision", Tooltip = "Enable to allow this chain to collide with Boing Kit's own implementation of lightweight colliders")]
			public bool EnableBoingKitCollision;

			// Token: 0x04005FE0 RID: 24544
			[ConditionalField(null, null, null, null, null, null, null, Label = "Unity Collision", Tooltip = "Enable to allow this chain to collide with Unity colliders.")]
			public bool EnableUnityCollision;

			// Token: 0x04005FE1 RID: 24545
			[ConditionalField(null, null, null, null, null, null, null, Label = "Inter-Chain Collision", Tooltip = "Enable to allow this chain to collide with other chain (under the same BoingBones component) with inter-chain collision enabled.")]
			public bool EnableInterChainCollision;

			// Token: 0x04005FE2 RID: 24546
			public Vector3 Gravity = Vector3.zero;

			// Token: 0x04005FE3 RID: 24547
			internal Bounds Bounds;

			// Token: 0x04005FE4 RID: 24548
			[ConditionalField(null, null, null, null, null, null, null, Label = "Squash & Stretch", Tooltip = "Percentage (0.0 = 0%; 1.0 = 100%) of each bone's squash & stretch effect. Squash & stretch is the effect of volume preservation by scaling bones based on how compressed or stretched the distances between bones become.\n\nEach curve type provides a type of mapping for each bone's percentage down the chain (0.0 at root & 1.0 at maximum chain length) to the bone's squash & stretch effect amount:\n\n - Constant One: 1.0 all the way.\n - Constant Half: 0.5 all the way.\n - Constant Zero: 0.0 all the way.\n - Root One Tail Half: 1.0 at 0% chain length and 0.5 at 100% chain length.\n - Root One Tail Zero: 1.0 at 0% chain length and 0.0 at 100% chain length.\n - Root Half Tail One: 0.5 at 0% chain length and 1.0 at 100% chain length.\n - Root Zero Tail One: 0.0 at 0% chain length and 1.0 at 100% chain length.\n - Custom: Custom curve.")]
			public BoingBones.Chain.CurveType SquashAndStretchCurveType = BoingBones.Chain.CurveType.ConstantZero;

			// Token: 0x04005FE5 RID: 24549
			[ConditionalField("SquashAndStretchCurveType", BoingBones.Chain.CurveType.Custom, null, null, null, null, null, Label = "  Custom Curve")]
			public AnimationCurve SquashAndStretchCustomCurve = AnimationCurve.Linear(0f, 0f, 1f, 0f);

			// Token: 0x04005FE6 RID: 24550
			[ConditionalField(null, null, null, null, null, null, null, Label = "  Max Squash", Tooltip = "Maximum squash amount. For example, 2.0 means a maximum scale of 200% when squashed.", Min = 1f, Max = 5f)]
			public float MaxSquash = 1.1f;

			// Token: 0x04005FE7 RID: 24551
			[ConditionalField(null, null, null, null, null, null, null, Label = "  Max Stretch", Tooltip = "Maximum stretch amount. For example, 2.0 means a minimum scale of 50% when stretched (200% stretched).", Min = 1f, Max = 5f)]
			public float MaxStretch = 2f;

			// Token: 0x04005FE8 RID: 24552
			internal Transform m_scannedRoot;

			// Token: 0x04005FE9 RID: 24553
			internal Transform[] m_scannedExclusion;

			// Token: 0x04005FEA RID: 24554
			internal int m_hierarchyHash = -1;

			// Token: 0x04005FEB RID: 24555
			internal float MaxLengthFromRoot;

			// Token: 0x02000E53 RID: 3667
			public enum CurveType
			{
				// Token: 0x04005FED RID: 24557
				ConstantOne,
				// Token: 0x04005FEE RID: 24558
				ConstantHalf,
				// Token: 0x04005FEF RID: 24559
				ConstantZero,
				// Token: 0x04005FF0 RID: 24560
				RootOneTailHalf,
				// Token: 0x04005FF1 RID: 24561
				RootOneTailZero,
				// Token: 0x04005FF2 RID: 24562
				RootHalfTailOne,
				// Token: 0x04005FF3 RID: 24563
				RootZeroTailOne,
				// Token: 0x04005FF4 RID: 24564
				Custom
			}
		}

		// Token: 0x02000E54 RID: 3668
		private class RescanEntry
		{
			// Token: 0x06005BD1 RID: 23505 RVA: 0x001C4020 File Offset: 0x001C2220
			internal RescanEntry(Transform transform, int iParent, float lengthFromRoot)
			{
				this.Transform = transform;
				this.ParentIndex = iParent;
				this.LengthFromRoot = lengthFromRoot;
			}

			// Token: 0x04005FF5 RID: 24565
			internal Transform Transform;

			// Token: 0x04005FF6 RID: 24566
			internal int ParentIndex;

			// Token: 0x04005FF7 RID: 24567
			internal float LengthFromRoot;
		}
	}
}
