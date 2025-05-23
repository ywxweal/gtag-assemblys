using System;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaTagScripts;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000508 RID: 1288
public class BuilderResizeWatch : MonoBehaviour
{
	// Token: 0x1700032B RID: 811
	// (get) Token: 0x06001F44 RID: 8004 RVA: 0x0009CA84 File Offset: 0x0009AC84
	public int SizeLayerMaskGrow
	{
		get
		{
			int num = 0;
			if (this.growSettings.affectLayerA)
			{
				num |= 1;
			}
			if (this.growSettings.affectLayerB)
			{
				num |= 2;
			}
			if (this.growSettings.affectLayerC)
			{
				num |= 4;
			}
			if (this.growSettings.affectLayerD)
			{
				num |= 8;
			}
			return num;
		}
	}

	// Token: 0x1700032C RID: 812
	// (get) Token: 0x06001F45 RID: 8005 RVA: 0x0009CAD8 File Offset: 0x0009ACD8
	public int SizeLayerMaskShrink
	{
		get
		{
			int num = 0;
			if (this.shrinkSettings.affectLayerA)
			{
				num |= 1;
			}
			if (this.shrinkSettings.affectLayerB)
			{
				num |= 2;
			}
			if (this.shrinkSettings.affectLayerC)
			{
				num |= 4;
			}
			if (this.shrinkSettings.affectLayerD)
			{
				num |= 8;
			}
			return num;
		}
	}

	// Token: 0x06001F46 RID: 8006 RVA: 0x0009CB2C File Offset: 0x0009AD2C
	private void Start()
	{
		if (this.enlargeButton != null)
		{
			this.enlargeButton.onPressButton.AddListener(new UnityAction(this.OnEnlargeButtonPressed));
		}
		if (this.shrinkButton != null)
		{
			this.shrinkButton.onPressButton.AddListener(new UnityAction(this.OnShrinkButtonPressed));
		}
		this.ownerRig = base.GetComponentInParent<VRRig>();
		this.enableDist = GTPlayer.Instance.bodyCollider.height;
		this.enableDistSq = this.enableDist * this.enableDist;
	}

	// Token: 0x06001F47 RID: 8007 RVA: 0x0009CBC4 File Offset: 0x0009ADC4
	private void OnDestroy()
	{
		if (this.enlargeButton != null)
		{
			this.enlargeButton.onPressButton.RemoveListener(new UnityAction(this.OnEnlargeButtonPressed));
		}
		if (this.shrinkButton != null)
		{
			this.shrinkButton.onPressButton.RemoveListener(new UnityAction(this.OnShrinkButtonPressed));
		}
	}

	// Token: 0x06001F48 RID: 8008 RVA: 0x0009CC28 File Offset: 0x0009AE28
	private void OnEnlargeButtonPressed()
	{
		if (this.sizeManager == null)
		{
			if (this.ownerRig == null)
			{
				Debug.LogWarning("Builder resize watch has no owner rig");
				return;
			}
			this.sizeManager = this.ownerRig.sizeManager;
		}
		if (this.sizeManager != null && this.sizeManager.currentSizeLayerMaskValue != this.SizeLayerMaskGrow && !this.updateCollision)
		{
			this.DisableCollisionWithPieces();
			this.sizeManager.currentSizeLayerMaskValue = this.SizeLayerMaskGrow;
			if (this.fxForLayerChange != null)
			{
				ObjectPools.instance.Instantiate(this.fxForLayerChange, this.ownerRig.transform.position, true);
			}
			this.timeToCheckCollision = (double)(Time.time + this.growDelay);
			this.updateCollision = true;
		}
	}

	// Token: 0x06001F49 RID: 8009 RVA: 0x0009CCF8 File Offset: 0x0009AEF8
	private void DisableCollisionWithPieces()
	{
		BuilderTable builderTable;
		if (!BuilderTable.TryGetBuilderTableForZone(this.ownerRig.zoneEntity.currentZone, out builderTable))
		{
			return;
		}
		int num = Physics.OverlapSphereNonAlloc(GTPlayer.Instance.headCollider.transform.position, 1f, this.tempDisableColliders, builderTable.allPiecesMask);
		for (int i = 0; i < num; i++)
		{
			BuilderPiece builderPieceFromCollider = BuilderPiece.GetBuilderPieceFromCollider(this.tempDisableColliders[i]);
			if (builderPieceFromCollider != null && builderPieceFromCollider.state == BuilderPiece.State.AttachedAndPlaced && !builderPieceFromCollider.isBuiltIntoTable && !this.collisionDisabledPieces.Contains(builderPieceFromCollider))
			{
				foreach (Collider collider in builderPieceFromCollider.colliders)
				{
					collider.enabled = false;
				}
				foreach (Collider collider2 in builderPieceFromCollider.placedOnlyColliders)
				{
					collider2.enabled = false;
				}
				this.collisionDisabledPieces.Add(builderPieceFromCollider);
			}
		}
	}

	// Token: 0x06001F4A RID: 8010 RVA: 0x0009CE34 File Offset: 0x0009B034
	private void EnableCollisionWithPieces()
	{
		for (int i = this.collisionDisabledPieces.Count - 1; i >= 0; i--)
		{
			BuilderPiece builderPiece = this.collisionDisabledPieces[i];
			if (builderPiece == null)
			{
				this.collisionDisabledPieces.RemoveAt(i);
			}
			else if (Vector3.SqrMagnitude(GTPlayer.Instance.bodyCollider.transform.position - builderPiece.transform.position) >= this.enableDistSq)
			{
				this.EnableCollisionWithPiece(builderPiece);
				this.collisionDisabledPieces.RemoveAt(i);
			}
		}
	}

	// Token: 0x06001F4B RID: 8011 RVA: 0x0009CEC4 File Offset: 0x0009B0C4
	private void EnableCollisionWithPiece(BuilderPiece piece)
	{
		foreach (Collider collider in piece.colliders)
		{
			collider.enabled = piece.state != BuilderPiece.State.None && piece.state != BuilderPiece.State.Displayed;
		}
		foreach (Collider collider2 in piece.placedOnlyColliders)
		{
			collider2.enabled = piece.state == BuilderPiece.State.AttachedAndPlaced;
		}
	}

	// Token: 0x06001F4C RID: 8012 RVA: 0x0009CF74 File Offset: 0x0009B174
	private void Update()
	{
		if (this.updateCollision && (double)Time.time >= this.timeToCheckCollision)
		{
			this.EnableCollisionWithPieces();
			if (this.collisionDisabledPieces.Count <= 0)
			{
				this.updateCollision = false;
			}
		}
	}

	// Token: 0x06001F4D RID: 8013 RVA: 0x0009CFA8 File Offset: 0x0009B1A8
	private void OnShrinkButtonPressed()
	{
		if (this.sizeManager == null)
		{
			if (this.ownerRig == null)
			{
				Debug.LogWarning("Builder resize watch has no owner rig");
			}
			this.sizeManager = this.ownerRig.sizeManager;
		}
		if (this.sizeManager != null && this.sizeManager.currentSizeLayerMaskValue != this.SizeLayerMaskShrink)
		{
			this.sizeManager.currentSizeLayerMaskValue = this.SizeLayerMaskShrink;
		}
	}

	// Token: 0x04002304 RID: 8964
	[SerializeField]
	private HeldButton enlargeButton;

	// Token: 0x04002305 RID: 8965
	[SerializeField]
	private HeldButton shrinkButton;

	// Token: 0x04002306 RID: 8966
	[SerializeField]
	private GameObject fxForLayerChange;

	// Token: 0x04002307 RID: 8967
	private VRRig ownerRig;

	// Token: 0x04002308 RID: 8968
	private SizeManager sizeManager;

	// Token: 0x04002309 RID: 8969
	[HideInInspector]
	public Collider[] tempDisableColliders = new Collider[128];

	// Token: 0x0400230A RID: 8970
	[HideInInspector]
	public List<BuilderPiece> collisionDisabledPieces = new List<BuilderPiece>();

	// Token: 0x0400230B RID: 8971
	private float enableDist = 1f;

	// Token: 0x0400230C RID: 8972
	private float enableDistSq = 1f;

	// Token: 0x0400230D RID: 8973
	private bool updateCollision;

	// Token: 0x0400230E RID: 8974
	private float growDelay = 1f;

	// Token: 0x0400230F RID: 8975
	private double timeToCheckCollision;

	// Token: 0x04002310 RID: 8976
	public BuilderResizeWatch.BuilderSizeChangeSettings growSettings;

	// Token: 0x04002311 RID: 8977
	public BuilderResizeWatch.BuilderSizeChangeSettings shrinkSettings;

	// Token: 0x02000509 RID: 1289
	[Serializable]
	public struct BuilderSizeChangeSettings
	{
		// Token: 0x04002312 RID: 8978
		public bool affectLayerA;

		// Token: 0x04002313 RID: 8979
		public bool affectLayerB;

		// Token: 0x04002314 RID: 8980
		public bool affectLayerC;

		// Token: 0x04002315 RID: 8981
		public bool affectLayerD;
	}
}
