using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000AE0 RID: 2784
	public class BuilderRecycler : MonoBehaviour
	{
		// Token: 0x06004376 RID: 17270 RVA: 0x00138260 File Offset: 0x00136460
		private void Awake()
		{
			this.hasFans = this.effectBehaviors.Count > 0 && this.bladeSoundPlayer != null && this.recycleParticles != null;
			this.hasPipes = this.outputPipes.Count > 0;
		}

		// Token: 0x06004377 RID: 17271 RVA: 0x001382B4 File Offset: 0x001364B4
		private void Start()
		{
			if (this.hasPipes)
			{
				this.numPipes = Mathf.Min(this.outputPipes.Count, 3);
				this.props = new MaterialPropertyBlock();
				this.ResetOutputPipes();
				this.totalRecycledCost = new int[3];
				this.currentChainCost = new int[3];
				for (int i = 0; i < this.totalRecycledCost.Length; i++)
				{
					this.totalRecycledCost[i] = 0;
					this.currentChainCost[i] = 0;
				}
			}
			this.zoneRenderers.Clear();
			if (this.hasPipes)
			{
				this.zoneRenderers.AddRange(this.outputPipes);
			}
			if (this.hasFans)
			{
				foreach (MonoBehaviour monoBehaviour in this.effectBehaviors)
				{
					Renderer component = monoBehaviour.GetComponent<Renderer>();
					if (component != null)
					{
						this.zoneRenderers.Add(component);
					}
				}
			}
			this.inBuilderZone = true;
			ZoneManagement instance = ZoneManagement.instance;
			instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
			this.OnZoneChanged();
		}

		// Token: 0x06004378 RID: 17272 RVA: 0x001383E8 File Offset: 0x001365E8
		private void OnDestroy()
		{
			if (ZoneManagement.instance != null)
			{
				ZoneManagement instance = ZoneManagement.instance;
				instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.OnZoneChanged));
			}
		}

		// Token: 0x06004379 RID: 17273 RVA: 0x00138420 File Offset: 0x00136620
		private void OnZoneChanged()
		{
			bool flag = ZoneManagement.instance.IsZoneActive(GTZone.monkeBlocks);
			if (flag && !this.inBuilderZone)
			{
				using (List<Renderer>.Enumerator enumerator = this.zoneRenderers.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Renderer renderer = enumerator.Current;
						renderer.enabled = true;
					}
					goto IL_008B;
				}
			}
			if (!flag && this.inBuilderZone)
			{
				foreach (Renderer renderer2 in this.zoneRenderers)
				{
					renderer2.enabled = false;
				}
			}
			IL_008B:
			this.inBuilderZone = flag;
		}

		// Token: 0x0600437A RID: 17274 RVA: 0x001384DC File Offset: 0x001366DC
		private void OnTriggerEnter(Collider other)
		{
			BuilderPiece builderPieceFromCollider = BuilderPiece.GetBuilderPieceFromCollider(other);
			if (builderPieceFromCollider == null)
			{
				return;
			}
			if (!builderPieceFromCollider.isBuiltIntoTable && !builderPieceFromCollider.isArmShelf)
			{
				this.table.RequestRecyclePiece(builderPieceFromCollider, true, this.recyclerID);
			}
		}

		// Token: 0x0600437B RID: 17275 RVA: 0x00138520 File Offset: 0x00136720
		public void OnRecycleRequestedAtRecycler(BuilderPiece piece)
		{
			if (this.hasPipes)
			{
				this.AddPieceCost(piece.cost);
			}
			if (this.hasFans)
			{
				foreach (MonoBehaviour monoBehaviour in this.effectBehaviors)
				{
					monoBehaviour.enabled = true;
				}
				this.recycleParticles.SetActive(true);
				this.bladeSoundPlayer.Play();
				this.timeToStopBlades = (double)(Time.time + this.recycleEffectDuration);
				this.playingBladeEffect = true;
			}
		}

		// Token: 0x0600437C RID: 17276 RVA: 0x001385C0 File Offset: 0x001367C0
		private void AddPieceCost(BuilderResources cost)
		{
			foreach (BuilderResourceQuantity builderResourceQuantity in cost.quantities)
			{
				if (builderResourceQuantity.type >= BuilderResourceType.Basic && builderResourceQuantity.type < BuilderResourceType.Count)
				{
					this.totalRecycledCost[(int)builderResourceQuantity.type] += builderResourceQuantity.count;
				}
			}
			if (!this.playingPipeEffect)
			{
				this.UpdatePipeLoop();
			}
		}

		// Token: 0x0600437D RID: 17277 RVA: 0x00138648 File Offset: 0x00136848
		private Vector2 GetUVShiftOffset()
		{
			float y = Shader.GetGlobalVector("_Time").y;
			Vector4 vector = new Vector4(500f, 0f, 0f, 0f);
			Vector4 vector2 = vector / this.recycleEffectDuration;
			return new Vector2(-1f * (Mathf.Floor(y * vector2.x) * 1f / vector.x % 1f) * vector.x - vector.x + 165f, 0f);
		}

		// Token: 0x0600437E RID: 17278 RVA: 0x001386D4 File Offset: 0x001368D4
		private void UpdatePipeLoop()
		{
			bool flag = false;
			for (int i = 0; i < this.numPipes; i++)
			{
				if (this.totalRecycledCost[i] > 0)
				{
					flag = true;
					this.outputPipes[i].GetPropertyBlock(this.props, 1);
					Vector4 vector = new Vector4(500f, 0f, 0f, 0f) / this.recycleEffectDuration;
					Vector2 uvshiftOffset = this.GetUVShiftOffset();
					this.props.SetColor("_BaseColor", this.builderResourceColors.colors[i].color);
					this.props.SetVector("_UvShiftRate", vector);
					this.props.SetVector("_UvShiftOffset", uvshiftOffset);
					this.outputPipes[i].SetPropertyBlock(this.props, 1);
					this.totalRecycledCost[i] = Mathf.Max(this.totalRecycledCost[i] - 1, 0);
				}
				else
				{
					this.outputPipes[i].GetPropertyBlock(this.props, 1);
					this.props.SetColor("_BaseColor", Color.black);
					this.outputPipes[i].SetPropertyBlock(this.props, 1);
				}
			}
			if (flag)
			{
				this.playingPipeEffect = true;
				this.timeToCheckPipes = (double)(Time.time + this.recycleEffectDuration);
				return;
			}
			this.playingPipeEffect = false;
		}

		// Token: 0x0600437F RID: 17279 RVA: 0x00138838 File Offset: 0x00136A38
		private void ResetOutputPipes()
		{
			foreach (MeshRenderer meshRenderer in this.outputPipes)
			{
				meshRenderer.GetPropertyBlock(this.props, 1);
				this.props.SetColor("_BaseColor", Color.black);
				meshRenderer.SetPropertyBlock(this.props, 1);
			}
		}

		// Token: 0x06004380 RID: 17280 RVA: 0x001388B4 File Offset: 0x00136AB4
		public void UpdateRecycler()
		{
			if (this.playingBladeEffect && (double)Time.time > this.timeToStopBlades)
			{
				if (this.hasFans)
				{
					foreach (MonoBehaviour monoBehaviour in this.effectBehaviors)
					{
						monoBehaviour.enabled = false;
					}
					this.recycleParticles.SetActive(false);
				}
				this.playingBladeEffect = false;
			}
			if (this.playingPipeEffect && (double)Time.time > this.timeToCheckPipes)
			{
				this.UpdatePipeLoop();
			}
		}

		// Token: 0x040045F3 RID: 17907
		public float recycleEffectDuration = 0.25f;

		// Token: 0x040045F4 RID: 17908
		private double timeToStopBlades = double.MinValue;

		// Token: 0x040045F5 RID: 17909
		private bool playingBladeEffect;

		// Token: 0x040045F6 RID: 17910
		private bool playingPipeEffect;

		// Token: 0x040045F7 RID: 17911
		private double timeToCheckPipes = double.MinValue;

		// Token: 0x040045F8 RID: 17912
		public List<MonoBehaviour> effectBehaviors;

		// Token: 0x040045F9 RID: 17913
		public GameObject recycleParticles;

		// Token: 0x040045FA RID: 17914
		public SoundBankPlayer bladeSoundPlayer;

		// Token: 0x040045FB RID: 17915
		public List<MeshRenderer> outputPipes;

		// Token: 0x040045FC RID: 17916
		public BuilderResourceColors builderResourceColors;

		// Token: 0x040045FD RID: 17917
		private bool hasFans;

		// Token: 0x040045FE RID: 17918
		private bool hasPipes;

		// Token: 0x040045FF RID: 17919
		private MaterialPropertyBlock props;

		// Token: 0x04004600 RID: 17920
		private int[] totalRecycledCost;

		// Token: 0x04004601 RID: 17921
		private int[] currentChainCost;

		// Token: 0x04004602 RID: 17922
		private int numPipes;

		// Token: 0x04004603 RID: 17923
		internal int recyclerID = -1;

		// Token: 0x04004604 RID: 17924
		internal BuilderTable table;

		// Token: 0x04004605 RID: 17925
		private List<Renderer> zoneRenderers = new List<Renderer>(10);

		// Token: 0x04004606 RID: 17926
		private bool inBuilderZone;
	}
}
