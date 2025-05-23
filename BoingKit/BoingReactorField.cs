using System;
using System.Collections.Generic;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02000E6D RID: 3693
	public class BoingReactorField : BoingBase
	{
		// Token: 0x17000907 RID: 2311
		// (get) Token: 0x06005C63 RID: 23651 RVA: 0x001C527A File Offset: 0x001C347A
		public static BoingReactorField.ShaderPropertyIdSet ShaderPropertyId
		{
			get
			{
				if (BoingReactorField.s_shaderPropertyId == null)
				{
					BoingReactorField.s_shaderPropertyId = new BoingReactorField.ShaderPropertyIdSet();
				}
				return BoingReactorField.s_shaderPropertyId;
			}
		}

		// Token: 0x06005C64 RID: 23652 RVA: 0x001C5294 File Offset: 0x001C3494
		public bool UpdateShaderConstants(MaterialPropertyBlock props, float positionSampleMultiplier = 1f, float rotationSampleMultiplier = 1f)
		{
			if (this.HardwareMode != BoingReactorField.HardwareModeEnum.GPU)
			{
				return false;
			}
			if (this.m_fieldParamsBuffer == null || this.m_cellsBuffer == null)
			{
				return false;
			}
			props.SetFloat(BoingReactorField.ShaderPropertyId.PositionSampleMultiplier, positionSampleMultiplier);
			props.SetFloat(BoingReactorField.ShaderPropertyId.RotationSampleMultiplier, rotationSampleMultiplier);
			props.SetBuffer(BoingReactorField.ShaderPropertyId.RenderFieldParams, this.m_fieldParamsBuffer);
			props.SetBuffer(BoingReactorField.ShaderPropertyId.RenderCells, this.m_cellsBuffer);
			return true;
		}

		// Token: 0x06005C65 RID: 23653 RVA: 0x001C5310 File Offset: 0x001C3510
		public bool UpdateShaderConstants(Material material, float positionSampleMultiplier = 1f, float rotationSampleMultiplier = 1f)
		{
			if (this.HardwareMode != BoingReactorField.HardwareModeEnum.GPU)
			{
				return false;
			}
			if (this.m_fieldParamsBuffer == null || this.m_cellsBuffer == null)
			{
				return false;
			}
			material.SetFloat(BoingReactorField.ShaderPropertyId.PositionSampleMultiplier, positionSampleMultiplier);
			material.SetFloat(BoingReactorField.ShaderPropertyId.RotationSampleMultiplier, rotationSampleMultiplier);
			material.SetBuffer(BoingReactorField.ShaderPropertyId.RenderFieldParams, this.m_fieldParamsBuffer);
			material.SetBuffer(BoingReactorField.ShaderPropertyId.RenderCells, this.m_cellsBuffer);
			return true;
		}

		// Token: 0x17000908 RID: 2312
		// (get) Token: 0x06005C66 RID: 23654 RVA: 0x001C5389 File Offset: 0x001C3589
		public int GpuResourceSetId
		{
			get
			{
				return this.m_gpuResourceSetId;
			}
		}

		// Token: 0x06005C67 RID: 23655 RVA: 0x001C5394 File Offset: 0x001C3594
		public BoingReactorField()
		{
			this.Params.Init();
			this.m_bounds = Aabb.Empty;
			this.m_init = false;
		}

		// Token: 0x06005C68 RID: 23656 RVA: 0x001C5474 File Offset: 0x001C3674
		public void Reboot()
		{
			this.m_gridCenter = base.transform.position;
			Vector3 vector = this.QuantizeNorm(this.m_gridCenter);
			this.m_qPrevGridCenterNorm = vector;
			BoingReactorField.CellMoveModeEnum cellMoveMode = this.CellMoveMode;
			if (cellMoveMode == BoingReactorField.CellMoveModeEnum.Follow)
			{
				this.m_gridCenter = base.transform.position;
				this.m_iCellBaseX = 0;
				this.m_iCellBaseY = 0;
				this.m_iCellBaseZ = 0;
				this.m_iCellBaseZ = 0;
				this.m_iCellBaseZ = 0;
				return;
			}
			if (cellMoveMode != BoingReactorField.CellMoveModeEnum.WrapAround)
			{
				return;
			}
			this.m_gridCenter = vector * this.CellSize;
			this.m_iCellBaseX = MathUtil.Modulo((int)this.m_qPrevGridCenterNorm.x, this.CellsX);
			this.m_iCellBaseY = MathUtil.Modulo((int)this.m_qPrevGridCenterNorm.y, this.CellsY);
			this.m_iCellBaseZ = MathUtil.Modulo((int)this.m_qPrevGridCenterNorm.z, this.CellsZ);
		}

		// Token: 0x06005C69 RID: 23657 RVA: 0x001C5553 File Offset: 0x001C3753
		public void OnEnable()
		{
			this.Reboot();
			BoingManager.Register(this);
		}

		// Token: 0x06005C6A RID: 23658 RVA: 0x001C5561 File Offset: 0x001C3761
		public void Start()
		{
			this.Reboot();
			this.m_cellMoveMode = this.CellMoveMode;
		}

		// Token: 0x06005C6B RID: 23659 RVA: 0x001C5575 File Offset: 0x001C3775
		public void OnDisable()
		{
			BoingManager.Unregister(this);
			this.DisposeCpuResources();
			this.DisposeGpuResources();
		}

		// Token: 0x06005C6C RID: 23660 RVA: 0x001C5589 File Offset: 0x001C3789
		public void DisposeCpuResources()
		{
			this.m_aCpuCell = null;
		}

		// Token: 0x06005C6D RID: 23661 RVA: 0x001C5594 File Offset: 0x001C3794
		public void DisposeGpuResources()
		{
			if (this.m_effectorIndexBuffer != null)
			{
				this.m_effectorIndexBuffer.Dispose();
				this.m_effectorIndexBuffer = null;
			}
			if (this.m_reactorParamsBuffer != null)
			{
				this.m_reactorParamsBuffer.Dispose();
				this.m_reactorParamsBuffer = null;
			}
			if (this.m_fieldParamsBuffer != null)
			{
				this.m_fieldParamsBuffer.Dispose();
				this.m_fieldParamsBuffer = null;
			}
			if (this.m_cellsBuffer != null)
			{
				this.m_cellsBuffer.Dispose();
				this.m_cellsBuffer = null;
			}
			if (this.m_cellsBuffer != null)
			{
				this.m_cellsBuffer.Dispose();
				this.m_cellsBuffer = null;
			}
		}

		// Token: 0x06005C6E RID: 23662 RVA: 0x001C5624 File Offset: 0x001C3824
		public bool SampleCpuGrid(Vector3 p, out Vector3 positionOffset, out Vector4 rotationOffset)
		{
			bool flag = false;
			switch (this.FalloffDimensions)
			{
			case BoingReactorField.FalloffDimensionsEnum.XYZ:
				flag = this.m_bounds.Contains(p);
				break;
			case BoingReactorField.FalloffDimensionsEnum.XY:
				flag = this.m_bounds.ContainsX(p) && this.m_bounds.ContainsY(p);
				break;
			case BoingReactorField.FalloffDimensionsEnum.XZ:
				flag = this.m_bounds.ContainsX(p) && this.m_bounds.ContainsZ(p);
				break;
			case BoingReactorField.FalloffDimensionsEnum.YZ:
				flag = this.m_bounds.ContainsY(p) && this.m_bounds.ContainsZ(p);
				break;
			}
			if (!flag)
			{
				positionOffset = Vector3.zero;
				rotationOffset = QuaternionUtil.ToVector4(Quaternion.identity);
				return false;
			}
			float num = 0.5f * this.CellSize;
			Vector3 vector = p - (this.m_gridCenter + this.GetCellCenterOffset(0, 0, 0));
			Vector3 vector2 = this.QuantizeNorm(vector + new Vector3(-num, -num, -num));
			Vector3 vector3 = vector2 * this.CellSize;
			int num2 = Mathf.Clamp((int)vector2.x, 0, this.CellsX - 1);
			int num3 = Mathf.Clamp((int)vector2.y, 0, this.CellsY - 1);
			int num4 = Mathf.Clamp((int)vector2.z, 0, this.CellsZ - 1);
			int num5 = Mathf.Min(num2 + 1, this.CellsX - 1);
			int num6 = Mathf.Min(num3 + 1, this.CellsY - 1);
			int num7 = Mathf.Min(num4 + 1, this.CellsZ - 1);
			int num8;
			int num9;
			int num10;
			this.ResolveCellIndex(num2, num3, num4, 1, out num8, out num9, out num10);
			int num11;
			int num12;
			int num13;
			this.ResolveCellIndex(num5, num6, num7, 1, out num11, out num12, out num13);
			bool flag2 = num8 != num11;
			bool flag3 = num9 != num12;
			bool flag4 = num10 != num13;
			Vector3 vector4 = (vector - vector3) / this.CellSize;
			Vector3 vector5 = p - base.transform.position;
			switch (this.FalloffDimensions)
			{
			case BoingReactorField.FalloffDimensionsEnum.XY:
				vector5.z = 0f;
				break;
			case BoingReactorField.FalloffDimensionsEnum.XZ:
				vector5.y = 0f;
				break;
			case BoingReactorField.FalloffDimensionsEnum.YZ:
				vector5.x = 0f;
				break;
			}
			int num14 = Mathf.Max(this.CellsX, Mathf.Max(this.CellsY, this.CellsZ));
			float num15 = 1f;
			BoingReactorField.FalloffModeEnum falloffMode = this.FalloffMode;
			if (falloffMode != BoingReactorField.FalloffModeEnum.Circle)
			{
				if (falloffMode == BoingReactorField.FalloffModeEnum.Square)
				{
					Vector3 vector6 = num * new Vector3((float)this.CellsX, (float)this.CellsY, (float)this.CellsZ);
					Vector3 vector7 = this.FalloffRatio * vector6 - num * Vector3.one;
					vector7.x = Mathf.Max(0f, vector7.x);
					vector7.y = Mathf.Max(0f, vector7.y);
					vector7.z = Mathf.Max(0f, vector7.z);
					Vector3 vector8 = (1f - this.FalloffRatio) * vector6 - num * Vector3.one;
					vector8.x = Mathf.Max(MathUtil.Epsilon, vector8.x);
					vector8.y = Mathf.Max(MathUtil.Epsilon, vector8.y);
					vector8.z = Mathf.Max(MathUtil.Epsilon, vector8.z);
					Vector3 vector9 = new Vector3(1f - Mathf.Clamp01((Mathf.Abs(vector5.x) - vector7.x) / vector8.x), 1f - Mathf.Clamp01((Mathf.Abs(vector5.y) - vector7.y) / vector8.y), 1f - Mathf.Clamp01((Mathf.Abs(vector5.z) - vector7.z) / vector8.z));
					switch (this.FalloffDimensions)
					{
					case BoingReactorField.FalloffDimensionsEnum.XY:
						vector9.x = 1f;
						break;
					case BoingReactorField.FalloffDimensionsEnum.XZ:
						vector9.y = 1f;
						break;
					case BoingReactorField.FalloffDimensionsEnum.YZ:
						vector9.z = 1f;
						break;
					}
					num15 = Mathf.Min(vector9.x, Mathf.Min(vector9.y, vector9.z));
				}
			}
			else
			{
				float num16 = num * (float)num14;
				Vector3 vector10 = new Vector3((float)num14 / (float)this.CellsX, (float)num14 / (float)this.CellsY, (float)num14 / (float)this.CellsZ);
				vector5.x *= vector10.x;
				vector5.y *= vector10.y;
				vector5.z *= vector10.z;
				float magnitude = vector5.magnitude;
				float num17 = Mathf.Max(0f, this.FalloffRatio * num16 - num);
				float num18 = Mathf.Max(MathUtil.Epsilon, (1f - this.FalloffRatio) * num16 - num);
				num15 = 1f - Mathf.Clamp01((magnitude - num17) / num18);
			}
			BoingReactorField.s_aCellOffset[0] = this.m_aCpuCell[num10, num9, num8].PositionSpring.Value - this.m_gridCenter - this.GetCellCenterOffset(num2, num3, num4);
			BoingReactorField.s_aCellOffset[1] = this.m_aCpuCell[num10, num9, num11].PositionSpring.Value - this.m_gridCenter - this.GetCellCenterOffset(num5, num3, num4);
			BoingReactorField.s_aCellOffset[2] = this.m_aCpuCell[num10, num12, num8].PositionSpring.Value - this.m_gridCenter - this.GetCellCenterOffset(num2, num6, num4);
			BoingReactorField.s_aCellOffset[3] = this.m_aCpuCell[num10, num12, num11].PositionSpring.Value - this.m_gridCenter - this.GetCellCenterOffset(num5, num6, num4);
			BoingReactorField.s_aCellOffset[4] = this.m_aCpuCell[num13, num9, num8].PositionSpring.Value - this.m_gridCenter - this.GetCellCenterOffset(num2, num3, num7);
			BoingReactorField.s_aCellOffset[5] = this.m_aCpuCell[num13, num9, num11].PositionSpring.Value - this.m_gridCenter - this.GetCellCenterOffset(num5, num3, num7);
			BoingReactorField.s_aCellOffset[6] = this.m_aCpuCell[num13, num12, num8].PositionSpring.Value - this.m_gridCenter - this.GetCellCenterOffset(num2, num6, num7);
			BoingReactorField.s_aCellOffset[7] = this.m_aCpuCell[num13, num12, num11].PositionSpring.Value - this.m_gridCenter - this.GetCellCenterOffset(num5, num6, num7);
			positionOffset = VectorUtil.TriLerp(ref BoingReactorField.s_aCellOffset[0], ref BoingReactorField.s_aCellOffset[1], ref BoingReactorField.s_aCellOffset[2], ref BoingReactorField.s_aCellOffset[3], ref BoingReactorField.s_aCellOffset[4], ref BoingReactorField.s_aCellOffset[5], ref BoingReactorField.s_aCellOffset[6], ref BoingReactorField.s_aCellOffset[7], flag2, flag3, flag4, vector4.x, vector4.y, vector4.z);
			rotationOffset = VectorUtil.TriLerp(ref this.m_aCpuCell[num10, num9, num8].RotationSpring.ValueVec, ref this.m_aCpuCell[num10, num9, num11].RotationSpring.ValueVec, ref this.m_aCpuCell[num10, num12, num8].RotationSpring.ValueVec, ref this.m_aCpuCell[num10, num12, num11].RotationSpring.ValueVec, ref this.m_aCpuCell[num13, num9, num8].RotationSpring.ValueVec, ref this.m_aCpuCell[num13, num9, num11].RotationSpring.ValueVec, ref this.m_aCpuCell[num13, num12, num8].RotationSpring.ValueVec, ref this.m_aCpuCell[num13, num12, num11].RotationSpring.ValueVec, flag2, flag3, flag4, vector4.x, vector4.y, vector4.z);
			positionOffset *= num15;
			rotationOffset = QuaternionUtil.ToVector4(QuaternionUtil.Pow(QuaternionUtil.FromVector4(rotationOffset, true), num15));
			return true;
		}

		// Token: 0x06005C6F RID: 23663 RVA: 0x001C5EF4 File Offset: 0x001C40F4
		private void UpdateFieldParamsGpu()
		{
			this.m_fieldParams.CellsX = this.CellsX;
			this.m_fieldParams.CellsY = this.CellsY;
			this.m_fieldParams.CellsZ = this.CellsZ;
			this.m_fieldParams.NumEffectors = 0;
			if (this.Effectors != null)
			{
				foreach (BoingEffector boingEffector in this.Effectors)
				{
					if (!(boingEffector == null))
					{
						BoingEffector component = boingEffector.GetComponent<BoingEffector>();
						if (!(component == null) && component.isActiveAndEnabled)
						{
							this.m_fieldParams.NumEffectors = this.m_fieldParams.NumEffectors + 1;
						}
					}
				}
			}
			this.m_fieldParams.iCellBaseX = this.m_iCellBaseX;
			this.m_fieldParams.iCellBaseY = this.m_iCellBaseY;
			this.m_fieldParams.iCellBaseZ = this.m_iCellBaseZ;
			this.m_fieldParams.FalloffMode = (int)this.FalloffMode;
			this.m_fieldParams.FalloffDimensions = (int)this.FalloffDimensions;
			this.m_fieldParams.PropagationDepth = this.PropagationDepth;
			this.m_fieldParams.GridCenter = this.m_gridCenter;
			this.m_fieldParams.UpWs = (this.Params.Bits.IsBitSet(6) ? this.Params.RotationReactionUp : (base.transform.rotation * VectorUtil.NormalizeSafe(this.Params.RotationReactionUp, Vector3.up)));
			this.m_fieldParams.FieldPosition = base.transform.position;
			this.m_fieldParams.FalloffRatio = this.FalloffRatio;
			this.m_fieldParams.CellSize = this.CellSize;
			this.m_fieldParams.DeltaTime = Time.deltaTime;
			if (this.m_fieldParamsBuffer != null)
			{
				this.m_fieldParamsBuffer.SetData(new BoingReactorField.FieldParams[] { this.m_fieldParams });
			}
		}

		// Token: 0x06005C70 RID: 23664 RVA: 0x001C60D8 File Offset: 0x001C42D8
		private void UpdateFlags()
		{
			this.Params.Bits.SetBit(0, this.TwoDDistanceCheck);
			this.Params.Bits.SetBit(1, this.TwoDPositionInfluence);
			this.Params.Bits.SetBit(2, this.TwoDRotationInfluence);
			this.Params.Bits.SetBit(3, this.EnablePositionEffect);
			this.Params.Bits.SetBit(4, this.EnableRotationEffect);
			this.Params.Bits.SetBit(6, this.GlobalReactionUpVector);
			this.Params.Bits.SetBit(7, this.EnablePropagation);
			this.Params.Bits.SetBit(8, this.AnchorPropagationAtBorder);
		}

		// Token: 0x06005C71 RID: 23665 RVA: 0x001C61A0 File Offset: 0x001C43A0
		public void UpdateBounds()
		{
			this.m_bounds = new Aabb(this.m_gridCenter + this.GetCellCenterOffset(0, 0, 0), this.m_gridCenter + this.GetCellCenterOffset(this.CellsX - 1, this.CellsY - 1, this.CellsZ - 1));
			this.m_bounds.Expand(this.CellSize);
		}

		// Token: 0x06005C72 RID: 23666 RVA: 0x001C6208 File Offset: 0x001C4408
		public void PrepareExecute()
		{
			this.Init();
			if (this.SharedParams != null)
			{
				BoingWork.Params.Copy(ref this.SharedParams.Params, ref this.Params);
			}
			this.UpdateFlags();
			this.UpdateBounds();
			BoingReactorField.HardwareModeEnum hardwareModeEnum;
			if (this.m_hardwareMode != this.HardwareMode)
			{
				hardwareModeEnum = this.m_hardwareMode;
				if (hardwareModeEnum != BoingReactorField.HardwareModeEnum.CPU)
				{
					if (hardwareModeEnum == BoingReactorField.HardwareModeEnum.GPU)
					{
						this.DisposeGpuResources();
					}
				}
				else
				{
					this.DisposeCpuResources();
				}
				this.m_hardwareMode = this.HardwareMode;
			}
			hardwareModeEnum = this.m_hardwareMode;
			if (hardwareModeEnum != BoingReactorField.HardwareModeEnum.CPU)
			{
				if (hardwareModeEnum == BoingReactorField.HardwareModeEnum.GPU)
				{
					this.ValidateGpuResources();
				}
			}
			else
			{
				this.ValidateCpuResources();
			}
			this.HandleCellMove();
			hardwareModeEnum = this.m_hardwareMode;
			if (hardwareModeEnum == BoingReactorField.HardwareModeEnum.CPU)
			{
				this.FinishPrepareExecuteCpu();
				return;
			}
			if (hardwareModeEnum != BoingReactorField.HardwareModeEnum.GPU)
			{
				return;
			}
			this.FinishPrepareExecuteGpu();
		}

		// Token: 0x06005C73 RID: 23667 RVA: 0x001C62C4 File Offset: 0x001C44C4
		private void ValidateCpuResources()
		{
			this.CellsX = Mathf.Max(1, this.CellsX);
			this.CellsY = Mathf.Max(1, this.CellsY);
			this.CellsZ = Mathf.Max(1, this.CellsZ);
			if (this.m_aCpuCell == null || this.m_cellsX != this.CellsX || this.m_cellsY != this.CellsY || this.m_cellsZ != this.CellsZ)
			{
				this.m_aCpuCell = new BoingWork.Params.InstanceData[this.CellsZ, this.CellsY, this.CellsX];
				for (int i = 0; i < this.CellsZ; i++)
				{
					for (int j = 0; j < this.CellsY; j++)
					{
						for (int k = 0; k < this.CellsX; k++)
						{
							int num;
							int num2;
							int num3;
							this.ResolveCellIndex(k, j, i, -1, out num, out num2, out num3);
							this.m_aCpuCell[i, j, k].Reset(this.m_gridCenter + this.GetCellCenterOffset(num, num2, num3), false);
						}
					}
				}
				this.m_cellsX = this.CellsX;
				this.m_cellsY = this.CellsY;
				this.m_cellsZ = this.CellsZ;
			}
		}

		// Token: 0x06005C74 RID: 23668 RVA: 0x001C63F4 File Offset: 0x001C45F4
		private void ValidateGpuResources()
		{
			bool flag = false;
			bool flag2 = this.m_shader == null || BoingReactorField.s_computeKernelId == null;
			if (flag2)
			{
				this.m_shader = Resources.Load<ComputeShader>("Boing Kit/BoingReactorFieldCompute");
				flag = true;
				if (BoingReactorField.s_computeKernelId == null)
				{
					BoingReactorField.s_computeKernelId = new BoingReactorField.ComputeKernelId();
					BoingReactorField.s_computeKernelId.InitKernel = this.m_shader.FindKernel("Init");
					BoingReactorField.s_computeKernelId.MoveKernel = this.m_shader.FindKernel("Move");
					BoingReactorField.s_computeKernelId.WrapXKernel = this.m_shader.FindKernel("WrapX");
					BoingReactorField.s_computeKernelId.WrapYKernel = this.m_shader.FindKernel("WrapY");
					BoingReactorField.s_computeKernelId.WrapZKernel = this.m_shader.FindKernel("WrapZ");
					BoingReactorField.s_computeKernelId.ExecuteKernel = this.m_shader.FindKernel("Execute");
				}
			}
			bool flag3 = this.m_effectorIndexBuffer == null || (this.Effectors != null && this.m_numEffectors != this.Effectors.Length);
			if (flag3 && this.Effectors != null)
			{
				if (this.m_effectorIndexBuffer != null)
				{
					this.m_effectorIndexBuffer.Dispose();
				}
				this.m_effectorIndexBuffer = new ComputeBuffer(this.Effectors.Length, 4);
				flag = true;
				this.m_numEffectors = this.Effectors.Length;
			}
			if (flag2 || flag3)
			{
				this.m_shader.SetBuffer(BoingReactorField.s_computeKernelId.ExecuteKernel, BoingReactorField.ShaderPropertyId.EffectorIndices, this.m_effectorIndexBuffer);
			}
			bool flag4 = this.m_reactorParamsBuffer == null;
			if (flag4)
			{
				this.m_reactorParamsBuffer = new ComputeBuffer(1, BoingWork.Params.Stride);
				flag = true;
			}
			if (flag2 || flag4)
			{
				this.m_shader.SetBuffer(BoingReactorField.s_computeKernelId.ExecuteKernel, BoingReactorField.ShaderPropertyId.ReactorParams, this.m_reactorParamsBuffer);
			}
			bool flag5 = this.m_fieldParamsBuffer == null;
			if (flag5)
			{
				this.m_fieldParamsBuffer = new ComputeBuffer(1, BoingReactorField.FieldParams.Stride);
				flag = true;
			}
			if (flag2 || flag5)
			{
				this.m_shader.SetBuffer(BoingReactorField.s_computeKernelId.InitKernel, BoingReactorField.ShaderPropertyId.ComputeFieldParams, this.m_fieldParamsBuffer);
				this.m_shader.SetBuffer(BoingReactorField.s_computeKernelId.MoveKernel, BoingReactorField.ShaderPropertyId.ComputeFieldParams, this.m_fieldParamsBuffer);
				this.m_shader.SetBuffer(BoingReactorField.s_computeKernelId.WrapXKernel, BoingReactorField.ShaderPropertyId.ComputeFieldParams, this.m_fieldParamsBuffer);
				this.m_shader.SetBuffer(BoingReactorField.s_computeKernelId.WrapYKernel, BoingReactorField.ShaderPropertyId.ComputeFieldParams, this.m_fieldParamsBuffer);
				this.m_shader.SetBuffer(BoingReactorField.s_computeKernelId.WrapZKernel, BoingReactorField.ShaderPropertyId.ComputeFieldParams, this.m_fieldParamsBuffer);
				this.m_shader.SetBuffer(BoingReactorField.s_computeKernelId.ExecuteKernel, BoingReactorField.ShaderPropertyId.ComputeFieldParams, this.m_fieldParamsBuffer);
			}
			this.m_cellBufferNeedsReset = this.m_cellsBuffer == null || this.m_cellsX != this.CellsX || this.m_cellsY != this.CellsY || this.m_cellsZ != this.CellsZ;
			if (this.m_cellBufferNeedsReset)
			{
				if (this.m_cellsBuffer != null)
				{
					this.m_cellsBuffer.Dispose();
				}
				int num = this.CellsX * this.CellsY * this.CellsZ;
				this.m_cellsBuffer = new ComputeBuffer(num, BoingWork.Params.InstanceData.Stride);
				BoingWork.Params.InstanceData[] array = new BoingWork.Params.InstanceData[num];
				for (int i = 0; i < num; i++)
				{
					array[i].PositionSpring.Reset();
					array[i].RotationSpring.Reset();
				}
				this.m_cellsBuffer.SetData(array);
				flag = true;
				this.m_cellsX = this.CellsX;
				this.m_cellsY = this.CellsY;
				this.m_cellsZ = this.CellsZ;
			}
			if (flag2 || this.m_cellBufferNeedsReset)
			{
				this.m_shader.SetBuffer(BoingReactorField.s_computeKernelId.InitKernel, BoingReactorField.ShaderPropertyId.ComputeCells, this.m_cellsBuffer);
				this.m_shader.SetBuffer(BoingReactorField.s_computeKernelId.MoveKernel, BoingReactorField.ShaderPropertyId.ComputeCells, this.m_cellsBuffer);
				this.m_shader.SetBuffer(BoingReactorField.s_computeKernelId.WrapXKernel, BoingReactorField.ShaderPropertyId.ComputeCells, this.m_cellsBuffer);
				this.m_shader.SetBuffer(BoingReactorField.s_computeKernelId.WrapYKernel, BoingReactorField.ShaderPropertyId.ComputeCells, this.m_cellsBuffer);
				this.m_shader.SetBuffer(BoingReactorField.s_computeKernelId.WrapZKernel, BoingReactorField.ShaderPropertyId.ComputeCells, this.m_cellsBuffer);
				this.m_shader.SetBuffer(BoingReactorField.s_computeKernelId.ExecuteKernel, BoingReactorField.ShaderPropertyId.ComputeCells, this.m_cellsBuffer);
			}
			if (flag)
			{
				this.m_gpuResourceSetId++;
				if (this.m_gpuResourceSetId < 0)
				{
					this.m_gpuResourceSetId = -1;
				}
			}
		}

		// Token: 0x06005C75 RID: 23669 RVA: 0x001C68D4 File Offset: 0x001C4AD4
		private void FinishPrepareExecuteCpu()
		{
			Quaternion rotation = base.transform.rotation;
			for (int i = 0; i < this.CellsZ; i++)
			{
				for (int j = 0; j < this.CellsY; j++)
				{
					for (int k = 0; k < this.CellsX; k++)
					{
						int num;
						int num2;
						int num3;
						this.ResolveCellIndex(k, j, i, -1, out num, out num2, out num3);
						this.m_aCpuCell[i, j, k].PrepareExecute(ref this.Params, this.m_gridCenter, rotation, this.GetCellCenterOffset(num, num2, num3));
					}
				}
			}
		}

		// Token: 0x06005C76 RID: 23670 RVA: 0x001C695C File Offset: 0x001C4B5C
		private void FinishPrepareExecuteGpu()
		{
			if (this.m_cellBufferNeedsReset)
			{
				this.UpdateFieldParamsGpu();
				this.m_shader.Dispatch(BoingReactorField.s_computeKernelId.InitKernel, this.CellsX, this.CellsY, this.CellsZ);
			}
		}

		// Token: 0x06005C77 RID: 23671 RVA: 0x001C6993 File Offset: 0x001C4B93
		public void Init()
		{
			if (this.m_init)
			{
				return;
			}
			this.m_hardwareMode = this.HardwareMode;
			this.m_init = true;
		}

		// Token: 0x06005C78 RID: 23672 RVA: 0x001C69B1 File Offset: 0x001C4BB1
		public void Sanitize()
		{
			if (this.PropagationDepth < 0)
			{
				Debug.LogWarning("Propagation iterations must be a positive number.");
			}
			else if (this.PropagationDepth > 3)
			{
				Debug.LogWarning("For performance reasons, propagation is limited to 3 iterations.");
			}
			this.PropagationDepth = Mathf.Clamp(this.PropagationDepth, 1, 3);
		}

		// Token: 0x06005C79 RID: 23673 RVA: 0x001C69F0 File Offset: 0x001C4BF0
		public void HandleCellMove()
		{
			if (this.m_cellMoveMode != this.CellMoveMode)
			{
				this.Reboot();
				this.m_cellMoveMode = this.CellMoveMode;
			}
			BoingReactorField.CellMoveModeEnum cellMoveMode = this.CellMoveMode;
			BoingReactorField.HardwareModeEnum hardwareModeEnum;
			if (cellMoveMode == BoingReactorField.CellMoveModeEnum.Follow)
			{
				Vector3 vector = base.transform.position - this.m_gridCenter;
				hardwareModeEnum = this.HardwareMode;
				if (hardwareModeEnum != BoingReactorField.HardwareModeEnum.CPU)
				{
					if (hardwareModeEnum == BoingReactorField.HardwareModeEnum.GPU)
					{
						this.UpdateFieldParamsGpu();
						this.m_shader.SetVector(BoingReactorField.ShaderPropertyId.MoveParams, vector);
						this.m_shader.Dispatch(BoingReactorField.s_computeKernelId.MoveKernel, this.CellsX, this.CellsY, this.CellsZ);
					}
				}
				else
				{
					for (int i = 0; i < this.CellsZ; i++)
					{
						for (int j = 0; j < this.CellsY; j++)
						{
							for (int k = 0; k < this.CellsX; k++)
							{
								ref BoingWork.Params.InstanceData ptr = ref this.m_aCpuCell[i, j, k];
								ptr.PositionSpring.Value = ptr.PositionSpring.Value + vector;
							}
						}
					}
				}
				this.m_gridCenter = base.transform.position;
				this.m_qPrevGridCenterNorm = this.QuantizeNorm(this.m_gridCenter);
				return;
			}
			if (cellMoveMode != BoingReactorField.CellMoveModeEnum.WrapAround)
			{
				return;
			}
			this.m_gridCenter = base.transform.position;
			Vector3 vector2 = this.QuantizeNorm(this.m_gridCenter);
			this.m_gridCenter = vector2 * this.CellSize;
			int num = (int)(vector2.x - this.m_qPrevGridCenterNorm.x);
			int num2 = (int)(vector2.y - this.m_qPrevGridCenterNorm.y);
			int num3 = (int)(vector2.z - this.m_qPrevGridCenterNorm.z);
			this.m_qPrevGridCenterNorm = vector2;
			if (num == 0 && num2 == 0 && num3 == 0)
			{
				return;
			}
			hardwareModeEnum = this.m_hardwareMode;
			if (hardwareModeEnum != BoingReactorField.HardwareModeEnum.CPU)
			{
				if (hardwareModeEnum == BoingReactorField.HardwareModeEnum.GPU)
				{
					this.WrapGpu(num, num2, num3);
				}
			}
			else
			{
				this.WrapCpu(num, num2, num3);
			}
			this.m_iCellBaseX = MathUtil.Modulo(this.m_iCellBaseX + num, this.CellsX);
			this.m_iCellBaseY = MathUtil.Modulo(this.m_iCellBaseY + num2, this.CellsY);
			this.m_iCellBaseZ = MathUtil.Modulo(this.m_iCellBaseZ + num3, this.CellsZ);
		}

		// Token: 0x06005C7A RID: 23674 RVA: 0x001C6C2E File Offset: 0x001C4E2E
		private void InitPropagationCpu(ref BoingWork.Params.InstanceData data)
		{
			data.PositionPropagationWorkData = Vector3.zero;
			data.RotationPropagationWorkData = Vector3.zero;
		}

		// Token: 0x06005C7B RID: 23675 RVA: 0x001C6C4C File Offset: 0x001C4E4C
		private void PropagateSpringCpu(ref BoingWork.Params.InstanceData data, float dt)
		{
			data.PositionSpring.Velocity = data.PositionSpring.Velocity + BoingReactorField.kPropagationFactor * this.PositionPropagation * data.PositionPropagationWorkData * dt;
			data.RotationSpring.VelocityVec = data.RotationSpring.VelocityVec + BoingReactorField.kPropagationFactor * this.RotationPropagation * data.RotationPropagationWorkData * dt;
		}

		// Token: 0x06005C7C RID: 23676 RVA: 0x001C6CCC File Offset: 0x001C4ECC
		private void ExtendPropagationBorder(ref BoingWork.Params.InstanceData data, float weight, int adjDeltaX, int adjDeltaY, int adjDeltaZ)
		{
			data.PositionPropagationWorkData += weight * (data.PositionOrigin + new Vector3((float)adjDeltaX, (float)adjDeltaY, (float)adjDeltaZ) * this.CellSize);
			data.RotationPropagationWorkData += weight * data.RotationOrigin;
		}

		// Token: 0x06005C7D RID: 23677 RVA: 0x001C6D3C File Offset: 0x001C4F3C
		private void AccumulatePropagationWeightedNeighbor(ref BoingWork.Params.InstanceData data, ref BoingWork.Params.InstanceData neighbor, float weight)
		{
			data.PositionPropagationWorkData += weight * (neighbor.PositionSpring.Value - neighbor.PositionOrigin);
			data.RotationPropagationWorkData += weight * (neighbor.RotationSpring.ValueVec - neighbor.RotationOrigin);
		}

		// Token: 0x06005C7E RID: 23678 RVA: 0x001C6DB0 File Offset: 0x001C4FB0
		private void GatherPropagation(ref BoingWork.Params.InstanceData data, float weightSum)
		{
			data.PositionPropagationWorkData = data.PositionPropagationWorkData / weightSum - (data.PositionSpring.Value - data.PositionOrigin);
			data.RotationPropagationWorkData = data.RotationPropagationWorkData / weightSum - (data.RotationSpring.ValueVec - data.RotationOrigin);
		}

		// Token: 0x06005C7F RID: 23679 RVA: 0x001C6C2E File Offset: 0x001C4E2E
		private void AnchorPropagationBorder(ref BoingWork.Params.InstanceData data)
		{
			data.PositionPropagationWorkData = Vector3.zero;
			data.RotationPropagationWorkData = Vector3.zero;
		}

		// Token: 0x06005C80 RID: 23680 RVA: 0x001C6E18 File Offset: 0x001C5018
		private void PropagateCpu(float dt)
		{
			int[] array = new int[this.PropagationDepth * 2 + 1];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = i - this.PropagationDepth;
			}
			for (int j = 0; j < this.CellsZ; j++)
			{
				for (int k = 0; k < this.CellsY; k++)
				{
					for (int l = 0; l < this.CellsX; l++)
					{
						this.InitPropagationCpu(ref this.m_aCpuCell[j, k, l]);
					}
				}
			}
			for (int m = 0; m < this.CellsZ; m++)
			{
				for (int n = 0; n < this.CellsY; n++)
				{
					for (int num = 0; num < this.CellsX; num++)
					{
						int num2;
						int num3;
						int num4;
						this.ResolveCellIndex(num, n, m, -1, out num2, out num3, out num4);
						float num5 = 0f;
						foreach (int num7 in array)
						{
							foreach (int num9 in array)
							{
								foreach (int num11 in array)
								{
									if (num11 != 0 || num9 != 0 || num7 != 0)
									{
										int num12 = num11 * num11 + num9 * num9 + num7 * num7;
										float num13 = BoingReactorField.s_aSqrtInv[num12];
										num5 += num13;
										if ((this.CellsX <= 2 || ((num2 != 0 || num11 >= 0) && (num2 != this.CellsX - 1 || num11 <= 0))) && (this.CellsY <= 2 || ((num3 != 0 || num9 >= 0) && (num3 != this.CellsY - 1 || num9 <= 0))) && (this.CellsZ <= 2 || ((num4 != 0 || num7 >= 0) && (num4 != this.CellsZ - 1 || num7 <= 0))))
										{
											int num14 = MathUtil.Modulo(num + num11, this.CellsX);
											int num15 = MathUtil.Modulo(n + num9, this.CellsY);
											int num16 = MathUtil.Modulo(m + num7, this.CellsZ);
											this.AccumulatePropagationWeightedNeighbor(ref this.m_aCpuCell[m, n, num], ref this.m_aCpuCell[num16, num15, num14], num13);
										}
									}
								}
							}
						}
						if (num5 > 0f)
						{
							this.GatherPropagation(ref this.m_aCpuCell[m, n, num], num5);
						}
					}
				}
			}
			if (this.AnchorPropagationAtBorder)
			{
				for (int num17 = 0; num17 < this.CellsZ; num17++)
				{
					for (int num18 = 0; num18 < this.CellsY; num18++)
					{
						for (int num19 = 0; num19 < this.CellsX; num19++)
						{
							int num20;
							int num21;
							int num22;
							this.ResolveCellIndex(num19, num18, num17, -1, out num20, out num21, out num22);
							if (((num20 == 0 || num20 == this.CellsX - 1) && this.CellsX > 2) || ((num21 == 0 || num21 == this.CellsY - 1) && this.CellsY > 2) || ((num22 == 0 || num22 == this.CellsZ - 1) && this.CellsZ > 2))
							{
								this.AnchorPropagationBorder(ref this.m_aCpuCell[num17, num18, num19]);
							}
						}
					}
				}
			}
			for (int num23 = 0; num23 < this.CellsZ; num23++)
			{
				for (int num24 = 0; num24 < this.CellsY; num24++)
				{
					for (int num25 = 0; num25 < this.CellsX; num25++)
					{
						this.PropagateSpringCpu(ref this.m_aCpuCell[num23, num24, num25], dt);
					}
				}
			}
		}

		// Token: 0x06005C81 RID: 23681 RVA: 0x001C71C8 File Offset: 0x001C53C8
		private void WrapCpu(int deltaX, int deltaY, int deltaZ)
		{
			if (deltaX != 0)
			{
				int num = ((deltaX > 0) ? (-1) : 1);
				for (int i = 0; i < this.CellsZ; i++)
				{
					for (int j = 0; j < this.CellsY; j++)
					{
						int num2 = ((deltaX > 0) ? (deltaX - 1) : (this.CellsX + deltaX));
						while (num2 >= 0 && num2 < this.CellsX)
						{
							int num3;
							int num4;
							int num5;
							this.ResolveCellIndex(num2, j, i, 1, out num3, out num4, out num5);
							int num6;
							int num7;
							int num8;
							this.ResolveCellIndex(num3 - deltaX, num4 - deltaY, num5 - deltaZ, -1, out num6, out num7, out num8);
							this.m_aCpuCell[num5, num4, num3].Reset(this.m_gridCenter + this.GetCellCenterOffset(num6, num7, num8), true);
							num2 += num;
						}
					}
				}
			}
			if (deltaY != 0)
			{
				int num9 = ((deltaY > 0) ? (-1) : 1);
				for (int k = 0; k < this.CellsZ; k++)
				{
					int num10 = ((deltaY > 0) ? (deltaY - 1) : (this.CellsY + deltaY));
					while (num10 >= 0 && num10 < this.CellsY)
					{
						for (int l = 0; l < this.CellsX; l++)
						{
							int num11;
							int num12;
							int num13;
							this.ResolveCellIndex(l, num10, k, 1, out num11, out num12, out num13);
							int num14;
							int num15;
							int num16;
							this.ResolveCellIndex(num11 - deltaX, num12 - deltaY, num13 - deltaZ, -1, out num14, out num15, out num16);
							this.m_aCpuCell[num13, num12, num11].Reset(this.m_gridCenter + this.GetCellCenterOffset(num14, num15, num16), true);
						}
						num10 += num9;
					}
				}
			}
			if (deltaZ != 0)
			{
				int num17 = ((deltaZ > 0) ? (-1) : 1);
				int num18 = ((deltaZ > 0) ? (deltaZ - 1) : (this.CellsZ + deltaZ));
				while (num18 >= 0 && num18 < this.CellsZ)
				{
					for (int m = 0; m < this.CellsY; m++)
					{
						for (int n = 0; n < this.CellsX; n++)
						{
							int num19;
							int num20;
							int num21;
							this.ResolveCellIndex(n, m, num18, 1, out num19, out num20, out num21);
							int num22;
							int num23;
							int num24;
							this.ResolveCellIndex(num19 - deltaX, num20 - deltaY, num21 - deltaZ, -1, out num22, out num23, out num24);
							this.m_aCpuCell[num21, num20, num19].Reset(this.m_gridCenter + this.GetCellCenterOffset(num22, num23, num24), true);
						}
					}
					num18 += num17;
				}
			}
		}

		// Token: 0x06005C82 RID: 23682 RVA: 0x001C741C File Offset: 0x001C561C
		private void WrapGpu(int deltaX, int deltaY, int deltaZ)
		{
			this.UpdateFieldParamsGpu();
			this.m_shader.SetInts(BoingReactorField.ShaderPropertyId.WrapParams, new int[] { deltaX, deltaY, deltaZ });
			if (deltaX != 0)
			{
				this.m_shader.Dispatch(BoingReactorField.s_computeKernelId.WrapXKernel, 1, this.CellsY, this.CellsZ);
			}
			if (deltaY != 0)
			{
				this.m_shader.Dispatch(BoingReactorField.s_computeKernelId.WrapYKernel, this.CellsX, 1, this.CellsZ);
			}
			if (deltaZ != 0)
			{
				this.m_shader.Dispatch(BoingReactorField.s_computeKernelId.WrapZKernel, this.CellsX, this.CellsY, 1);
			}
		}

		// Token: 0x06005C83 RID: 23683 RVA: 0x001C74C8 File Offset: 0x001C56C8
		public void ExecuteCpu(float dt)
		{
			this.PrepareExecute();
			if (this.Effectors == null || this.Effectors.Length == 0)
			{
				return;
			}
			if (this.EnablePropagation)
			{
				this.PropagateCpu(dt);
			}
			foreach (BoingEffector boingEffector in this.Effectors)
			{
				if (!(boingEffector == null))
				{
					BoingEffector.Params @params = default(BoingEffector.Params);
					@params.Fill(boingEffector);
					if (this.m_bounds.Intersects(ref @params))
					{
						for (int j = 0; j < this.CellsZ; j++)
						{
							for (int k = 0; k < this.CellsY; k++)
							{
								for (int l = 0; l < this.CellsX; l++)
								{
									this.m_aCpuCell[j, k, l].AccumulateTarget(ref this.Params, ref @params, dt);
								}
							}
						}
					}
				}
			}
			for (int m = 0; m < this.CellsZ; m++)
			{
				for (int n = 0; n < this.CellsY; n++)
				{
					for (int num = 0; num < this.CellsX; num++)
					{
						this.m_aCpuCell[m, n, num].EndAccumulateTargets(ref this.Params);
						this.m_aCpuCell[m, n, num].Execute(ref this.Params, dt);
					}
				}
			}
		}

		// Token: 0x06005C84 RID: 23684 RVA: 0x001C761C File Offset: 0x001C581C
		public void ExecuteGpu(float dt, ComputeBuffer effectorParamsBuffer, Dictionary<int, int> effectorParamsIndexMap)
		{
			this.PrepareExecute();
			this.UpdateFieldParamsGpu();
			this.m_shader.SetBuffer(BoingReactorField.s_computeKernelId.ExecuteKernel, BoingReactorField.ShaderPropertyId.Effectors, effectorParamsBuffer);
			if (this.m_fieldParams.NumEffectors > 0)
			{
				int[] array = new int[this.m_fieldParams.NumEffectors];
				int num = 0;
				foreach (BoingEffector boingEffector in this.Effectors)
				{
					if (!(boingEffector == null))
					{
						BoingEffector component = boingEffector.GetComponent<BoingEffector>();
						int num2;
						if (!(component == null) && component.isActiveAndEnabled && effectorParamsIndexMap.TryGetValue(component.GetInstanceID(), out num2))
						{
							array[num++] = num2;
						}
					}
				}
				this.m_effectorIndexBuffer.SetData(array);
			}
			this.s_aReactorParams[0] = this.Params;
			this.m_reactorParamsBuffer.SetData(this.s_aReactorParams);
			this.m_shader.SetVector(BoingReactorField.ShaderPropertyId.PropagationParams, new Vector4(this.PositionPropagation, this.RotationPropagation, BoingReactorField.kPropagationFactor, 0f));
			this.m_shader.Dispatch(BoingReactorField.s_computeKernelId.ExecuteKernel, this.CellsX, this.CellsY, this.CellsZ);
		}

		// Token: 0x06005C85 RID: 23685 RVA: 0x001C7756 File Offset: 0x001C5956
		public void OnDrawGizmosSelected()
		{
			if (!base.isActiveAndEnabled)
			{
				return;
			}
			this.DrawGizmos(true);
		}

		// Token: 0x06005C86 RID: 23686 RVA: 0x001C7768 File Offset: 0x001C5968
		private void DrawGizmos(bool drawEffectors)
		{
			Vector3 vector = this.GetGridCenter();
			BoingReactorField.CellMoveModeEnum cellMoveMode = this.CellMoveMode;
			if (cellMoveMode != BoingReactorField.CellMoveModeEnum.Follow)
			{
				if (cellMoveMode == BoingReactorField.CellMoveModeEnum.WrapAround)
				{
					vector = new Vector3(Mathf.Round(base.transform.position.x / this.CellSize), Mathf.Round(base.transform.position.y / this.CellSize), Mathf.Round(base.transform.position.z / this.CellSize)) * this.CellSize;
				}
			}
			else
			{
				vector = base.transform.position;
			}
			BoingWork.Params.InstanceData[,,] array = null;
			BoingReactorField.HardwareModeEnum hardwareMode = this.HardwareMode;
			if (hardwareMode != BoingReactorField.HardwareModeEnum.CPU)
			{
				if (hardwareMode == BoingReactorField.HardwareModeEnum.GPU)
				{
					if (this.m_cellsBuffer != null)
					{
						array = new BoingWork.Params.InstanceData[this.CellsZ, this.CellsY, this.CellsX];
						this.m_cellsBuffer.GetData(array);
					}
				}
			}
			else
			{
				array = this.m_aCpuCell;
			}
			int num = 1;
			if (this.CellsX * this.CellsY * this.CellsZ > 1024)
			{
				num = 2;
			}
			if (this.CellsX * this.CellsY * this.CellsZ > 4096)
			{
				num = 3;
			}
			if (this.CellsX * this.CellsY * this.CellsZ > 8192)
			{
				num = 4;
			}
			for (int i = 0; i < this.CellsZ; i++)
			{
				for (int j = 0; j < this.CellsY; j++)
				{
					for (int k = 0; k < this.CellsX; k++)
					{
						int num2;
						int num3;
						int num4;
						this.ResolveCellIndex(k, j, i, -1, out num2, out num3, out num4);
						Vector3 vector2 = vector + this.GetCellCenterOffset(num2, num3, num4);
						if (array != null && k % num == 0 && j % num == 0 && i % num == 0)
						{
							BoingWork.Params.InstanceData instanceData = array[i, j, k];
							Gizmos.color = new Color(1f, 1f, 1f, 1f);
							Gizmos.matrix = Matrix4x4.TRS(instanceData.PositionSpring.Value, instanceData.RotationSpring.ValueQuat, Vector3.one);
							Gizmos.DrawCube(Vector3.zero, Mathf.Min(0.1f, 0.5f * this.CellSize) * Vector3.one);
							Gizmos.matrix = Matrix4x4.identity;
						}
						Gizmos.color = new Color(1f, 0.5f, 0.2f, 1f);
						Gizmos.DrawWireCube(vector2, this.CellSize * Vector3.one);
					}
				}
			}
			BoingReactorField.FalloffModeEnum falloffMode = this.FalloffMode;
			if (falloffMode != BoingReactorField.FalloffModeEnum.Circle)
			{
				if (falloffMode == BoingReactorField.FalloffModeEnum.Square)
				{
					Vector3 vector3 = this.CellSize * this.FalloffRatio * new Vector3((float)this.CellsX, (float)this.CellsY, (float)this.CellsZ);
					Gizmos.color = new Color(1f, 1f, 0.2f, 0.5f);
					Gizmos.DrawWireCube(vector, vector3);
				}
			}
			else
			{
				float num5 = (float)Mathf.Max(this.CellsX, Mathf.Max(this.CellsY, this.CellsZ));
				Gizmos.color = new Color(1f, 1f, 0.2f, 0.5f);
				Gizmos.matrix = Matrix4x4.Translate(vector) * Matrix4x4.Scale(new Vector3((float)this.CellsX, (float)this.CellsY, (float)this.CellsZ) / num5);
				Gizmos.DrawWireSphere(Vector3.zero, 0.5f * this.CellSize * num5 * this.FalloffRatio);
				Gizmos.matrix = Matrix4x4.identity;
			}
			if (drawEffectors && this.Effectors != null)
			{
				foreach (BoingEffector boingEffector in this.Effectors)
				{
					if (!(boingEffector == null))
					{
						boingEffector.OnDrawGizmosSelected();
					}
				}
			}
		}

		// Token: 0x06005C87 RID: 23687 RVA: 0x001C7B40 File Offset: 0x001C5D40
		private Vector3 GetGridCenter()
		{
			BoingReactorField.CellMoveModeEnum cellMoveMode = this.CellMoveMode;
			if (cellMoveMode == BoingReactorField.CellMoveModeEnum.Follow)
			{
				return base.transform.position;
			}
			if (cellMoveMode != BoingReactorField.CellMoveModeEnum.WrapAround)
			{
				return base.transform.position;
			}
			return this.QuantizeNorm(base.transform.position) * this.CellSize;
		}

		// Token: 0x06005C88 RID: 23688 RVA: 0x001C7B91 File Offset: 0x001C5D91
		private Vector3 QuantizeNorm(Vector3 p)
		{
			return new Vector3(Mathf.Round(p.x / this.CellSize), Mathf.Round(p.y / this.CellSize), Mathf.Round(p.z / this.CellSize));
		}

		// Token: 0x06005C89 RID: 23689 RVA: 0x001C7BD0 File Offset: 0x001C5DD0
		private Vector3 GetCellCenterOffset(int x, int y, int z)
		{
			return this.CellSize * (-0.5f * (new Vector3((float)this.CellsX, (float)this.CellsY, (float)this.CellsZ) - Vector3.one) + new Vector3((float)x, (float)y, (float)z));
		}

		// Token: 0x06005C8A RID: 23690 RVA: 0x001C7C28 File Offset: 0x001C5E28
		private void ResolveCellIndex(int x, int y, int z, int baseMult, out int resX, out int resY, out int resZ)
		{
			resX = MathUtil.Modulo(x + baseMult * this.m_iCellBaseX, this.CellsX);
			resY = MathUtil.Modulo(y + baseMult * this.m_iCellBaseY, this.CellsY);
			resZ = MathUtil.Modulo(z + baseMult * this.m_iCellBaseZ, this.CellsZ);
		}

		// Token: 0x04006040 RID: 24640
		private static BoingReactorField.ShaderPropertyIdSet s_shaderPropertyId;

		// Token: 0x04006041 RID: 24641
		private BoingReactorField.FieldParams m_fieldParams;

		// Token: 0x04006042 RID: 24642
		public BoingReactorField.HardwareModeEnum HardwareMode = BoingReactorField.HardwareModeEnum.GPU;

		// Token: 0x04006043 RID: 24643
		private BoingReactorField.HardwareModeEnum m_hardwareMode;

		// Token: 0x04006044 RID: 24644
		public BoingReactorField.CellMoveModeEnum CellMoveMode = BoingReactorField.CellMoveModeEnum.WrapAround;

		// Token: 0x04006045 RID: 24645
		private BoingReactorField.CellMoveModeEnum m_cellMoveMode;

		// Token: 0x04006046 RID: 24646
		[Range(0.1f, 10f)]
		public float CellSize = 1f;

		// Token: 0x04006047 RID: 24647
		public int CellsX = 8;

		// Token: 0x04006048 RID: 24648
		public int CellsY = 1;

		// Token: 0x04006049 RID: 24649
		public int CellsZ = 8;

		// Token: 0x0400604A RID: 24650
		private int m_cellsX = -1;

		// Token: 0x0400604B RID: 24651
		private int m_cellsY = -1;

		// Token: 0x0400604C RID: 24652
		private int m_cellsZ = -1;

		// Token: 0x0400604D RID: 24653
		private int m_iCellBaseX;

		// Token: 0x0400604E RID: 24654
		private int m_iCellBaseY;

		// Token: 0x0400604F RID: 24655
		private int m_iCellBaseZ;

		// Token: 0x04006050 RID: 24656
		public BoingReactorField.FalloffModeEnum FalloffMode = BoingReactorField.FalloffModeEnum.Square;

		// Token: 0x04006051 RID: 24657
		[Range(0f, 1f)]
		public float FalloffRatio = 0.7f;

		// Token: 0x04006052 RID: 24658
		public BoingReactorField.FalloffDimensionsEnum FalloffDimensions = BoingReactorField.FalloffDimensionsEnum.XZ;

		// Token: 0x04006053 RID: 24659
		public BoingEffector[] Effectors = new BoingEffector[1];

		// Token: 0x04006054 RID: 24660
		private int m_numEffectors = -1;

		// Token: 0x04006055 RID: 24661
		private Aabb m_bounds;

		// Token: 0x04006056 RID: 24662
		public bool TwoDDistanceCheck;

		// Token: 0x04006057 RID: 24663
		public bool TwoDPositionInfluence;

		// Token: 0x04006058 RID: 24664
		public bool TwoDRotationInfluence;

		// Token: 0x04006059 RID: 24665
		public bool EnablePositionEffect = true;

		// Token: 0x0400605A RID: 24666
		public bool EnableRotationEffect = true;

		// Token: 0x0400605B RID: 24667
		public bool GlobalReactionUpVector;

		// Token: 0x0400605C RID: 24668
		public BoingWork.Params Params;

		// Token: 0x0400605D RID: 24669
		public SharedBoingParams SharedParams;

		// Token: 0x0400605E RID: 24670
		public bool EnablePropagation;

		// Token: 0x0400605F RID: 24671
		[Range(0f, 1f)]
		public float PositionPropagation = 1f;

		// Token: 0x04006060 RID: 24672
		[Range(0f, 1f)]
		public float RotationPropagation = 1f;

		// Token: 0x04006061 RID: 24673
		[Range(1f, 3f)]
		public int PropagationDepth = 1;

		// Token: 0x04006062 RID: 24674
		public bool AnchorPropagationAtBorder;

		// Token: 0x04006063 RID: 24675
		private static readonly float kPropagationFactor = 600f;

		// Token: 0x04006064 RID: 24676
		private BoingWork.Params.InstanceData[,,] m_aCpuCell;

		// Token: 0x04006065 RID: 24677
		private ComputeShader m_shader;

		// Token: 0x04006066 RID: 24678
		private ComputeBuffer m_effectorIndexBuffer;

		// Token: 0x04006067 RID: 24679
		private ComputeBuffer m_reactorParamsBuffer;

		// Token: 0x04006068 RID: 24680
		private ComputeBuffer m_fieldParamsBuffer;

		// Token: 0x04006069 RID: 24681
		private ComputeBuffer m_cellsBuffer;

		// Token: 0x0400606A RID: 24682
		private int m_gpuResourceSetId = -1;

		// Token: 0x0400606B RID: 24683
		private static BoingReactorField.ComputeKernelId s_computeKernelId;

		// Token: 0x0400606C RID: 24684
		private bool m_init;

		// Token: 0x0400606D RID: 24685
		private Vector3 m_gridCenter;

		// Token: 0x0400606E RID: 24686
		private Vector3 m_qPrevGridCenterNorm;

		// Token: 0x0400606F RID: 24687
		private static Vector3[] s_aCellOffset = new Vector3[8];

		// Token: 0x04006070 RID: 24688
		private bool m_cellBufferNeedsReset;

		// Token: 0x04006071 RID: 24689
		private static float[] s_aSqrtInv = new float[]
		{
			0f, 1f, 0.70711f, 0.57735f, 0.5f, 0.44721f, 0.40825f, 0.37796f, 0.35355f, 0.33333f,
			0.31623f, 0.30151f, 0.28868f, 0.27735f, 0.26726f, 0.2582f, 0.25f, 0.24254f, 0.2357f, 0.22942f,
			0.22361f, 0.21822f, 0.2132f, 0.20851f, 0.20412f, 0.2f, 0.19612f, 0.19245f
		};

		// Token: 0x04006072 RID: 24690
		private BoingWork.Params[] s_aReactorParams = new BoingWork.Params[1];

		// Token: 0x02000E6E RID: 3694
		public enum HardwareModeEnum
		{
			// Token: 0x04006074 RID: 24692
			CPU,
			// Token: 0x04006075 RID: 24693
			GPU
		}

		// Token: 0x02000E6F RID: 3695
		public enum CellMoveModeEnum
		{
			// Token: 0x04006077 RID: 24695
			Follow,
			// Token: 0x04006078 RID: 24696
			WrapAround
		}

		// Token: 0x02000E70 RID: 3696
		public enum FalloffModeEnum
		{
			// Token: 0x0400607A RID: 24698
			None,
			// Token: 0x0400607B RID: 24699
			Circle,
			// Token: 0x0400607C RID: 24700
			Square
		}

		// Token: 0x02000E71 RID: 3697
		public enum FalloffDimensionsEnum
		{
			// Token: 0x0400607E RID: 24702
			XYZ,
			// Token: 0x0400607F RID: 24703
			XY,
			// Token: 0x04006080 RID: 24704
			XZ,
			// Token: 0x04006081 RID: 24705
			YZ
		}

		// Token: 0x02000E72 RID: 3698
		public class ShaderPropertyIdSet
		{
			// Token: 0x06005C8C RID: 23692 RVA: 0x001C7CB0 File Offset: 0x001C5EB0
			public ShaderPropertyIdSet()
			{
				this.MoveParams = Shader.PropertyToID("moveParams");
				this.WrapParams = Shader.PropertyToID("wrapParams");
				this.Effectors = Shader.PropertyToID("aEffector");
				this.EffectorIndices = Shader.PropertyToID("aEffectorIndex");
				this.ReactorParams = Shader.PropertyToID("reactorParams");
				this.ComputeFieldParams = Shader.PropertyToID("fieldParams");
				this.ComputeCells = Shader.PropertyToID("aCell");
				this.RenderFieldParams = Shader.PropertyToID("aBoingFieldParams");
				this.RenderCells = Shader.PropertyToID("aBoingFieldCell");
				this.PositionSampleMultiplier = Shader.PropertyToID("positionSampleMultiplier");
				this.RotationSampleMultiplier = Shader.PropertyToID("rotationSampleMultiplier");
				this.PropagationParams = Shader.PropertyToID("propagationParams");
			}

			// Token: 0x04006082 RID: 24706
			public int MoveParams;

			// Token: 0x04006083 RID: 24707
			public int WrapParams;

			// Token: 0x04006084 RID: 24708
			public int Effectors;

			// Token: 0x04006085 RID: 24709
			public int EffectorIndices;

			// Token: 0x04006086 RID: 24710
			public int ReactorParams;

			// Token: 0x04006087 RID: 24711
			public int ComputeFieldParams;

			// Token: 0x04006088 RID: 24712
			public int ComputeCells;

			// Token: 0x04006089 RID: 24713
			public int RenderFieldParams;

			// Token: 0x0400608A RID: 24714
			public int RenderCells;

			// Token: 0x0400608B RID: 24715
			public int PositionSampleMultiplier;

			// Token: 0x0400608C RID: 24716
			public int RotationSampleMultiplier;

			// Token: 0x0400608D RID: 24717
			public int PropagationParams;
		}

		// Token: 0x02000E73 RID: 3699
		private struct FieldParams
		{
			// Token: 0x06005C8D RID: 23693 RVA: 0x001C7D84 File Offset: 0x001C5F84
			private void SuppressWarnings()
			{
				this.m_padding0 = 0;
				this.m_padding1 = 0;
				this.m_padding2 = 0f;
				this.m_padding4 = 0f;
				this.m_padding5 = 0f;
				this.m_padding0 = this.m_padding1;
				this.m_padding1 = (int)this.m_padding2;
				this.m_padding2 = this.m_padding3;
				this.m_padding3 = this.m_padding4;
				this.m_padding4 = this.m_padding5;
			}

			// Token: 0x0400608E RID: 24718
			public static readonly int Stride = 112;

			// Token: 0x0400608F RID: 24719
			public int CellsX;

			// Token: 0x04006090 RID: 24720
			public int CellsY;

			// Token: 0x04006091 RID: 24721
			public int CellsZ;

			// Token: 0x04006092 RID: 24722
			public int NumEffectors;

			// Token: 0x04006093 RID: 24723
			public int iCellBaseX;

			// Token: 0x04006094 RID: 24724
			public int iCellBaseY;

			// Token: 0x04006095 RID: 24725
			public int iCellBaseZ;

			// Token: 0x04006096 RID: 24726
			public int m_padding0;

			// Token: 0x04006097 RID: 24727
			public int FalloffMode;

			// Token: 0x04006098 RID: 24728
			public int FalloffDimensions;

			// Token: 0x04006099 RID: 24729
			public int PropagationDepth;

			// Token: 0x0400609A RID: 24730
			public int m_padding1;

			// Token: 0x0400609B RID: 24731
			public Vector3 GridCenter;

			// Token: 0x0400609C RID: 24732
			private float m_padding3;

			// Token: 0x0400609D RID: 24733
			public Vector3 UpWs;

			// Token: 0x0400609E RID: 24734
			private float m_padding2;

			// Token: 0x0400609F RID: 24735
			public Vector3 FieldPosition;

			// Token: 0x040060A0 RID: 24736
			public float m_padding4;

			// Token: 0x040060A1 RID: 24737
			public float FalloffRatio;

			// Token: 0x040060A2 RID: 24738
			public float CellSize;

			// Token: 0x040060A3 RID: 24739
			public float DeltaTime;

			// Token: 0x040060A4 RID: 24740
			private float m_padding5;
		}

		// Token: 0x02000E74 RID: 3700
		private class ComputeKernelId
		{
			// Token: 0x040060A5 RID: 24741
			public int InitKernel;

			// Token: 0x040060A6 RID: 24742
			public int MoveKernel;

			// Token: 0x040060A7 RID: 24743
			public int WrapXKernel;

			// Token: 0x040060A8 RID: 24744
			public int WrapYKernel;

			// Token: 0x040060A9 RID: 24745
			public int WrapZKernel;

			// Token: 0x040060AA RID: 24746
			public int ExecuteKernel;
		}
	}
}
