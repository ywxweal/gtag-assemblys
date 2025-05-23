using System;
using System.Collections.Generic;
using GorillaTagScripts;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020004FB RID: 1275
public class BuilderPiecePrivatePlot : MonoBehaviour
{
	// Token: 0x06001F06 RID: 7942 RVA: 0x00099EB6 File Offset: 0x000980B6
	private void Awake()
	{
		this.Init();
	}

	// Token: 0x06001F07 RID: 7943 RVA: 0x00099EC0 File Offset: 0x000980C0
	private void Init()
	{
		if (this.initDone)
		{
			return;
		}
		this.materialProps = new MaterialPropertyBlock();
		this.usedResources = new int[3];
		for (int i = 0; i < this.usedResources.Length; i++)
		{
			this.usedResources[i] = 0;
		}
		this.tempResourceCount = new int[3];
		this.piece = base.GetComponent<BuilderPiece>();
		this.SetPlotState(BuilderPiecePrivatePlot.PlotState.Vacant);
		this.piecesToCount = new Queue<BuilderPiece>(1024);
		this.initDone = true;
		this.privatePlotIndex = -1;
	}

	// Token: 0x06001F08 RID: 7944 RVA: 0x00099F48 File Offset: 0x00098148
	private void Start()
	{
		if (this.piece != null && this.piece.GetTable() != null)
		{
			BuilderTable table = this.piece.GetTable();
			this.doesLocalPlayerOwnAPlot = table.DoesPlayerOwnPlot(PhotonNetwork.LocalPlayer.ActorNumber);
			table.OnLocalPlayerClaimedPlot.AddListener(new UnityAction<bool>(this.OnLocalPlayerClaimedPlot));
			this.UpdateVisuals();
			foreach (BuilderResourceMeter builderResourceMeter in this.resourceMeters)
			{
				builderResourceMeter.table = this.piece.GetTable();
			}
		}
		this.buildArea.gameObject.SetActive(true);
		this.buildArea.enabled = true;
		this.buildAreaBounds = this.buildArea.bounds;
		this.buildArea.gameObject.SetActive(false);
		this.buildArea.enabled = false;
		this.zoneRenderers.Clear();
		this.zoneRenderers.Add(this.tmpLabel.GetComponent<Renderer>());
		foreach (BuilderResourceMeter builderResourceMeter2 in this.resourceMeters)
		{
			this.zoneRenderers.AddRange(builderResourceMeter2.GetComponentsInChildren<Renderer>());
		}
		this.zoneRenderers.AddRange(this.borderMeshes);
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
		this.inBuilderZone = true;
		this.OnZoneChanged();
	}

	// Token: 0x06001F09 RID: 7945 RVA: 0x0009A104 File Offset: 0x00098304
	private void OnDestroy()
	{
		if (this.piece != null && this.piece.GetTable() != null)
		{
			this.piece.GetTable().OnLocalPlayerClaimedPlot.RemoveListener(new UnityAction<bool>(this.OnLocalPlayerClaimedPlot));
		}
		if (ZoneManagement.instance != null)
		{
			ZoneManagement instance = ZoneManagement.instance;
			instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.OnZoneChanged));
		}
	}

	// Token: 0x06001F0A RID: 7946 RVA: 0x0009A188 File Offset: 0x00098388
	private void OnZoneChanged()
	{
		bool flag = ZoneManagement.instance.IsZoneActive(this.piece.GetTable().tableZone);
		if (flag && !this.inBuilderZone)
		{
			using (List<Renderer>.Enumerator enumerator = this.zoneRenderers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Renderer renderer = enumerator.Current;
					renderer.enabled = true;
				}
				goto IL_0099;
			}
		}
		if (!flag && this.inBuilderZone)
		{
			foreach (Renderer renderer2 in this.zoneRenderers)
			{
				renderer2.enabled = false;
			}
		}
		IL_0099:
		this.inBuilderZone = flag;
	}

	// Token: 0x06001F0B RID: 7947 RVA: 0x0009A254 File Offset: 0x00098454
	private void OnLocalPlayerClaimedPlot(bool claim)
	{
		this.doesLocalPlayerOwnAPlot = claim;
		this.UpdateVisuals();
	}

	// Token: 0x06001F0C RID: 7948 RVA: 0x0009A264 File Offset: 0x00098464
	public void UpdatePlot()
	{
		if (BuilderPieceInteractor.instance == null || BuilderPieceInteractor.instance.heldChainLength == null || BuilderPieceInteractor.instance.heldChainLength.Length < 2)
		{
			return;
		}
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		if (!this.initDone)
		{
			this.Init();
		}
		if ((this.plotState == BuilderPiecePrivatePlot.PlotState.Occupied && this.owningPlayerActorNumber == PhotonNetwork.LocalPlayer.ActorNumber) || (this.plotState == BuilderPiecePrivatePlot.PlotState.Vacant && !this.doesLocalPlayerOwnAPlot))
		{
			BuilderPiece parentPiece = BuilderPieceInteractor.instance.prevPotentialPlacement[0].parentPiece;
			BuilderPiece parentPiece2 = BuilderPieceInteractor.instance.prevPotentialPlacement[1].parentPiece;
			bool flag = false;
			if (parentPiece == null && this.leftPotentialParent != null)
			{
				this.isLeftOverPlot = false;
				this.leftPotentialParent = null;
				flag = true;
			}
			else if ((this.leftPotentialParent == null && parentPiece != null) || (parentPiece != null && !parentPiece.Equals(this.leftPotentialParent)))
			{
				BuilderPiece attachedBuiltInPiece = parentPiece.GetAttachedBuiltInPiece();
				this.isLeftOverPlot = attachedBuiltInPiece != null && attachedBuiltInPiece.Equals(this.piece);
				this.leftPotentialParent = parentPiece;
				flag = true;
			}
			if (parentPiece2 == null && this.rightPotentialParent != null)
			{
				this.isRightOverPlot = false;
				this.rightPotentialParent = null;
				flag = true;
			}
			else if ((this.rightPotentialParent == null && parentPiece2 != null) || (parentPiece2 != null && !parentPiece2.Equals(this.rightPotentialParent)))
			{
				BuilderPiece attachedBuiltInPiece2 = parentPiece2.GetAttachedBuiltInPiece();
				this.isRightOverPlot = attachedBuiltInPiece2 != null && attachedBuiltInPiece2.Equals(this.piece);
				this.rightPotentialParent = parentPiece2;
				flag = true;
			}
			if (flag)
			{
				this.UpdateVisuals();
			}
		}
		else if (this.isRightOverPlot || this.isLeftOverPlot)
		{
			this.isRightOverPlot = false;
			this.isLeftOverPlot = false;
			this.UpdateVisuals();
		}
		foreach (BuilderResourceMeter builderResourceMeter in this.resourceMeters)
		{
			builderResourceMeter.UpdateMeterFill();
		}
	}

	// Token: 0x06001F0D RID: 7949 RVA: 0x0009A49C File Offset: 0x0009869C
	public void RecountPlotCost()
	{
		this.Init();
		this.piece.GetChainCost(this.usedResources);
		this.UpdateVisuals();
	}

	// Token: 0x06001F0E RID: 7950 RVA: 0x0009A4BB File Offset: 0x000986BB
	public void OnPieceAttachedToPlot(BuilderPiece attachPiece)
	{
		this.AddChainResourcesToCount(attachPiece, true);
		this.UpdateVisuals();
	}

	// Token: 0x06001F0F RID: 7951 RVA: 0x0009A4CB File Offset: 0x000986CB
	public void OnPieceDetachedFromPlot(BuilderPiece detachPiece)
	{
		this.AddChainResourcesToCount(detachPiece, false);
		this.UpdateVisuals();
	}

	// Token: 0x06001F10 RID: 7952 RVA: 0x0009A4DB File Offset: 0x000986DB
	public void ChangeAttachedPieceCount(int delta)
	{
		this.attachedPieceCount += delta;
		this.UpdateVisuals();
	}

	// Token: 0x06001F11 RID: 7953 RVA: 0x0009A4F4 File Offset: 0x000986F4
	public void AddChainResourcesToCount(BuilderPiece chain, bool attach)
	{
		if (chain == null)
		{
			return;
		}
		this.piecesToCount.Clear();
		for (int i = 0; i < this.tempResourceCount.Length; i++)
		{
			this.tempResourceCount[i] = 0;
		}
		this.piecesToCount.Enqueue(chain);
		this.AddPieceCostToArray(chain, this.tempResourceCount);
		bool flag = false;
		while (this.piecesToCount.Count > 0 && !flag)
		{
			BuilderPiece builderPiece = this.piecesToCount.Dequeue().firstChildPiece;
			while (builderPiece != null)
			{
				this.piecesToCount.Enqueue(builderPiece);
				if (!this.AddPieceCostToArray(builderPiece, this.tempResourceCount))
				{
					Debug.LogWarning("Builder plot placing pieces over limits");
					flag = true;
					break;
				}
				builderPiece = builderPiece.nextSiblingPiece;
			}
		}
		for (int j = 0; j < this.usedResources.Length; j++)
		{
			if (attach)
			{
				this.usedResources[j] += this.tempResourceCount[j];
			}
			else
			{
				this.usedResources[j] -= this.tempResourceCount[j];
			}
		}
	}

	// Token: 0x06001F12 RID: 7954 RVA: 0x0009A5F5 File Offset: 0x000987F5
	public void ClaimPlotForPlayerNumber(int player)
	{
		this.owningPlayerActorNumber = player;
		this.SetPlotState(BuilderPiecePrivatePlot.PlotState.Occupied);
	}

	// Token: 0x06001F13 RID: 7955 RVA: 0x0009A605 File Offset: 0x00098805
	public int GetOwnerActorNumber()
	{
		return this.owningPlayerActorNumber;
	}

	// Token: 0x06001F14 RID: 7956 RVA: 0x0009A610 File Offset: 0x00098810
	public void ClearPlot()
	{
		this.Init();
		this.attachedPieceCount = 0;
		for (int i = 0; i < this.usedResources.Length; i++)
		{
			this.usedResources[i] = 0;
		}
		this.SetPlotState(BuilderPiecePrivatePlot.PlotState.Vacant);
	}

	// Token: 0x06001F15 RID: 7957 RVA: 0x0009A64D File Offset: 0x0009884D
	public void FreePlot()
	{
		this.SetPlotState(BuilderPiecePrivatePlot.PlotState.Vacant);
	}

	// Token: 0x06001F16 RID: 7958 RVA: 0x0009A656 File Offset: 0x00098856
	public bool IsPlotClaimed()
	{
		return this.plotState > BuilderPiecePrivatePlot.PlotState.Vacant;
	}

	// Token: 0x06001F17 RID: 7959 RVA: 0x0009A664 File Offset: 0x00098864
	public bool IsChainUnderCapacity(BuilderPiece chain)
	{
		if (chain == null)
		{
			return true;
		}
		this.piecesToCount.Clear();
		for (int i = 0; i < this.tempResourceCount.Length; i++)
		{
			this.tempResourceCount[i] = this.usedResources[i];
		}
		this.piecesToCount.Enqueue(chain);
		if (!this.AddPieceCostToArray(chain, this.tempResourceCount))
		{
			return false;
		}
		while (this.piecesToCount.Count > 0)
		{
			BuilderPiece builderPiece = this.piecesToCount.Dequeue().firstChildPiece;
			while (builderPiece != null)
			{
				this.piecesToCount.Enqueue(builderPiece);
				if (!this.AddPieceCostToArray(builderPiece, this.tempResourceCount))
				{
					return false;
				}
				builderPiece = builderPiece.nextSiblingPiece;
			}
		}
		return true;
	}

	// Token: 0x06001F18 RID: 7960 RVA: 0x0009A718 File Offset: 0x00098918
	public bool AddPieceCostToArray(BuilderPiece addedPiece, int[] array)
	{
		if (addedPiece == null)
		{
			return true;
		}
		if (addedPiece.cost != null)
		{
			foreach (BuilderResourceQuantity builderResourceQuantity in addedPiece.cost.quantities)
			{
				if (builderResourceQuantity.type >= BuilderResourceType.Basic && builderResourceQuantity.type < BuilderResourceType.Count)
				{
					array[(int)builderResourceQuantity.type] += builderResourceQuantity.count;
					if (array[(int)builderResourceQuantity.type] > this.piece.GetTable().GetPrivateResourceLimitForType((int)builderResourceQuantity.type))
					{
						return false;
					}
				}
			}
			return true;
		}
		return true;
	}

	// Token: 0x06001F19 RID: 7961 RVA: 0x0009A7D4 File Offset: 0x000989D4
	public bool CanPlayerAttachToPlot(int actorNumber)
	{
		return (this.plotState == BuilderPiecePrivatePlot.PlotState.Occupied && this.owningPlayerActorNumber == actorNumber) || (this.plotState == BuilderPiecePrivatePlot.PlotState.Vacant && !this.piece.GetTable().DoesPlayerOwnPlot(actorNumber));
	}

	// Token: 0x06001F1A RID: 7962 RVA: 0x0009A808 File Offset: 0x00098A08
	public bool CanPlayerGrabFromPlot(int actorNumber, Vector3 worldPosition)
	{
		if (this.owningPlayerActorNumber == actorNumber || this.plotState == BuilderPiecePrivatePlot.PlotState.Vacant)
		{
			return true;
		}
		int num;
		if (this.piece.GetTable().plotOwners.TryGetValue(actorNumber, out num))
		{
			BuilderPiece builderPiece = this.piece.GetTable().GetPiece(num);
			BuilderPiecePrivatePlot builderPiecePrivatePlot;
			if (builderPiece != null && builderPiece.TryGetPlotComponent(out builderPiecePrivatePlot))
			{
				return builderPiecePrivatePlot.IsLocationWithinPlotExtents(worldPosition);
			}
		}
		return false;
	}

	// Token: 0x06001F1B RID: 7963 RVA: 0x0009A870 File Offset: 0x00098A70
	private void SetPlotState(BuilderPiecePrivatePlot.PlotState newState)
	{
		this.plotState = newState;
		BuilderPiecePrivatePlot.PlotState plotState = this.plotState;
		if (plotState != BuilderPiecePrivatePlot.PlotState.Vacant)
		{
			if (plotState == BuilderPiecePrivatePlot.PlotState.Occupied)
			{
				if (this.tmpLabel != null && NetworkSystem.Instance != null)
				{
					string text = string.Empty;
					NetPlayer player = NetworkSystem.Instance.GetPlayer(this.owningPlayerActorNumber);
					RigContainer rigContainer;
					if (player != null && VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
					{
						text = rigContainer.Rig.playerNameVisible;
					}
					if (string.IsNullOrEmpty(text) && !this.tmpLabel.text.Equals("OCCUPIED"))
					{
						this.tmpLabel.text = "OCCUPIED";
					}
					else if (!this.tmpLabel.text.Equals(text))
					{
						this.tmpLabel.text = text;
					}
				}
				else if (this.tmpLabel != null && !this.tmpLabel.text.Equals("OCCUPIED"))
				{
					this.tmpLabel.text = "OCCUPIED";
				}
			}
		}
		else
		{
			this.owningPlayerActorNumber = -1;
			if (this.tmpLabel != null && !this.tmpLabel.text.Equals(string.Empty))
			{
				this.tmpLabel.text = string.Empty;
			}
		}
		this.UpdateVisuals();
	}

	// Token: 0x06001F1C RID: 7964 RVA: 0x0009A9C4 File Offset: 0x00098BC4
	public bool IsLocationWithinPlotExtents(Vector3 worldPosition)
	{
		if (!this.buildAreaBounds.Contains(worldPosition))
		{
			return false;
		}
		Vector3 vector = this.buildArea.transform.InverseTransformPoint(worldPosition);
		Vector3 vector2 = this.buildArea.center + this.buildArea.size / 2f;
		Vector3 vector3 = this.buildArea.center - this.buildArea.size / 2f;
		return vector.x >= vector3.x && vector.x <= vector2.x && vector.y >= vector3.y && vector.y <= vector2.y && vector.z >= vector3.z && vector.z <= vector2.z;
	}

	// Token: 0x06001F1D RID: 7965 RVA: 0x0009AA98 File Offset: 0x00098C98
	public void OnAvailableResourceChange()
	{
		this.UpdateVisuals();
	}

	// Token: 0x06001F1E RID: 7966 RVA: 0x0009AAA0 File Offset: 0x00098CA0
	private void UpdateVisuals()
	{
		if (this.usedResources == null || this.piece.GetTable() == null)
		{
			return;
		}
		BuilderPiecePrivatePlot.PlotState plotState = this.plotState;
		if (plotState != BuilderPiecePrivatePlot.PlotState.Vacant)
		{
			if (plotState != BuilderPiecePrivatePlot.PlotState.Occupied)
			{
				return;
			}
			if (this.owningPlayerActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
			{
				this.UpdateVisualsForOwner();
				return;
			}
			this.SetBorderColor(this.placementDisallowedColor);
			int num = 0;
			while (num < this.resourceMeters.Count && num < 3)
			{
				int privateResourceLimitForType = this.piece.GetTable().GetPrivateResourceLimitForType(num);
				if (privateResourceLimitForType != 0)
				{
					this.resourceMeters[num].SetNormalizedFillTarget((float)(privateResourceLimitForType - this.usedResources[num]) / (float)privateResourceLimitForType);
				}
				num++;
			}
		}
		else
		{
			if (!this.doesLocalPlayerOwnAPlot)
			{
				this.UpdateVisualsForOwner();
				return;
			}
			this.SetBorderColor(this.placementDisallowedColor);
			for (int i = 0; i < this.resourceMeters.Count; i++)
			{
				if (i >= 3)
				{
					return;
				}
				int privateResourceLimitForType2 = this.piece.GetTable().GetPrivateResourceLimitForType(i);
				if (privateResourceLimitForType2 != 0)
				{
					this.resourceMeters[i].SetNormalizedFillTarget((float)(privateResourceLimitForType2 - this.usedResources[i]) / (float)privateResourceLimitForType2);
				}
			}
			return;
		}
	}

	// Token: 0x06001F1F RID: 7967 RVA: 0x0009ABBC File Offset: 0x00098DBC
	private void UpdateVisualsForOwner()
	{
		bool flag = true;
		if (this.usedResources == null)
		{
			return;
		}
		if (BuilderPieceInteractor.instance == null || BuilderPieceInteractor.instance.heldChainCost == null)
		{
			return;
		}
		for (int i = 0; i < 3; i++)
		{
			int num = this.usedResources[i];
			if (this.isLeftOverPlot)
			{
				num += BuilderPieceInteractor.instance.heldChainCost[0][i];
			}
			if (this.isRightOverPlot)
			{
				num += BuilderPieceInteractor.instance.heldChainCost[1][i];
			}
			int privateResourceLimitForType = this.piece.GetTable().GetPrivateResourceLimitForType(i);
			if (num < privateResourceLimitForType)
			{
				flag = false;
			}
			if (privateResourceLimitForType != 0 && this.resourceMeters.Count > i)
			{
				this.resourceMeters[i].SetNormalizedFillTarget((float)(privateResourceLimitForType - num) / (float)privateResourceLimitForType);
			}
		}
		if (flag)
		{
			this.SetBorderColor(this.placementDisallowedColor);
			return;
		}
		this.SetBorderColor(this.placementAllowedColor);
	}

	// Token: 0x06001F20 RID: 7968 RVA: 0x0009ACA8 File Offset: 0x00098EA8
	private void SetBorderColor(Color color)
	{
		this.borderMeshes[0].GetPropertyBlock(this.materialProps);
		this.materialProps.SetColor("_BaseColor", color);
		foreach (MeshRenderer meshRenderer in this.borderMeshes)
		{
			meshRenderer.SetPropertyBlock(this.materialProps);
		}
	}

	// Token: 0x04002280 RID: 8832
	[SerializeField]
	private Color placementAllowedColor;

	// Token: 0x04002281 RID: 8833
	[SerializeField]
	private Color placementDisallowedColor;

	// Token: 0x04002282 RID: 8834
	[SerializeField]
	private Color overCapacityColor;

	// Token: 0x04002283 RID: 8835
	public List<MeshRenderer> borderMeshes;

	// Token: 0x04002284 RID: 8836
	public BoxCollider buildArea;

	// Token: 0x04002285 RID: 8837
	[SerializeField]
	private TMP_Text tmpLabel;

	// Token: 0x04002286 RID: 8838
	[SerializeField]
	private List<BuilderResourceMeter> resourceMeters;

	// Token: 0x04002287 RID: 8839
	[NonSerialized]
	public int[] usedResources;

	// Token: 0x04002288 RID: 8840
	[NonSerialized]
	public int[] tempResourceCount;

	// Token: 0x04002289 RID: 8841
	[SerializeField]
	private GameObject plotClaimedFX;

	// Token: 0x0400228A RID: 8842
	private BuilderPiece leftPotentialParent;

	// Token: 0x0400228B RID: 8843
	private BuilderPiece rightPotentialParent;

	// Token: 0x0400228C RID: 8844
	private bool isLeftOverPlot;

	// Token: 0x0400228D RID: 8845
	private bool isRightOverPlot;

	// Token: 0x0400228E RID: 8846
	private Bounds buildAreaBounds;

	// Token: 0x0400228F RID: 8847
	[HideInInspector]
	public BuilderPiece piece;

	// Token: 0x04002290 RID: 8848
	private int owningPlayerActorNumber;

	// Token: 0x04002291 RID: 8849
	private int attachedPieceCount;

	// Token: 0x04002292 RID: 8850
	[HideInInspector]
	public int privatePlotIndex;

	// Token: 0x04002293 RID: 8851
	[HideInInspector]
	public BuilderPiecePrivatePlot.PlotState plotState;

	// Token: 0x04002294 RID: 8852
	private bool doesLocalPlayerOwnAPlot;

	// Token: 0x04002295 RID: 8853
	private Queue<BuilderPiece> piecesToCount;

	// Token: 0x04002296 RID: 8854
	private bool initDone;

	// Token: 0x04002297 RID: 8855
	private MaterialPropertyBlock materialProps;

	// Token: 0x04002298 RID: 8856
	private List<Renderer> zoneRenderers = new List<Renderer>(12);

	// Token: 0x04002299 RID: 8857
	private bool inBuilderZone;

	// Token: 0x020004FC RID: 1276
	public enum PlotState
	{
		// Token: 0x0400229B RID: 8859
		Vacant,
		// Token: 0x0400229C RID: 8860
		Occupied
	}
}
