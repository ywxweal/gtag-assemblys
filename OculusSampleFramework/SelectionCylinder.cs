using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000BF0 RID: 3056
	public class SelectionCylinder : MonoBehaviour
	{
		// Token: 0x17000775 RID: 1909
		// (get) Token: 0x06004B7D RID: 19325 RVA: 0x0016608E File Offset: 0x0016428E
		// (set) Token: 0x06004B7E RID: 19326 RVA: 0x00166098 File Offset: 0x00164298
		public SelectionCylinder.SelectionState CurrSelectionState
		{
			get
			{
				return this._currSelectionState;
			}
			set
			{
				SelectionCylinder.SelectionState currSelectionState = this._currSelectionState;
				this._currSelectionState = value;
				if (currSelectionState != this._currSelectionState)
				{
					if (this._currSelectionState > SelectionCylinder.SelectionState.Off)
					{
						this._selectionMeshRenderer.enabled = true;
						this.AffectSelectionColor((this._currSelectionState == SelectionCylinder.SelectionState.Selected) ? this._defaultSelectionColors : this._highlightColors);
						return;
					}
					this._selectionMeshRenderer.enabled = false;
				}
			}
		}

		// Token: 0x06004B7F RID: 19327 RVA: 0x001660FC File Offset: 0x001642FC
		private void Awake()
		{
			this._selectionMaterials = this._selectionMeshRenderer.materials;
			int num = this._selectionMaterials.Length;
			this._defaultSelectionColors = new Color[num];
			this._highlightColors = new Color[num];
			for (int i = 0; i < num; i++)
			{
				this._defaultSelectionColors[i] = this._selectionMaterials[i].GetColor(SelectionCylinder._colorId);
				this._highlightColors[i] = new Color(1f, 1f, 1f, this._defaultSelectionColors[i].a);
			}
			this.CurrSelectionState = SelectionCylinder.SelectionState.Off;
		}

		// Token: 0x06004B80 RID: 19328 RVA: 0x001661A0 File Offset: 0x001643A0
		private void OnDestroy()
		{
			if (this._selectionMaterials != null)
			{
				foreach (Material material in this._selectionMaterials)
				{
					if (material != null)
					{
						Object.Destroy(material);
					}
				}
			}
		}

		// Token: 0x06004B81 RID: 19329 RVA: 0x001661E0 File Offset: 0x001643E0
		private void AffectSelectionColor(Color[] newColors)
		{
			int num = newColors.Length;
			for (int i = 0; i < num; i++)
			{
				this._selectionMaterials[i].SetColor(SelectionCylinder._colorId, newColors[i]);
			}
		}

		// Token: 0x04004E0F RID: 19983
		[SerializeField]
		private MeshRenderer _selectionMeshRenderer;

		// Token: 0x04004E10 RID: 19984
		private static int _colorId = Shader.PropertyToID("_Color");

		// Token: 0x04004E11 RID: 19985
		private Material[] _selectionMaterials;

		// Token: 0x04004E12 RID: 19986
		private Color[] _defaultSelectionColors;

		// Token: 0x04004E13 RID: 19987
		private Color[] _highlightColors;

		// Token: 0x04004E14 RID: 19988
		private SelectionCylinder.SelectionState _currSelectionState;

		// Token: 0x02000BF1 RID: 3057
		public enum SelectionState
		{
			// Token: 0x04004E16 RID: 19990
			Off,
			// Token: 0x04004E17 RID: 19991
			Selected,
			// Token: 0x04004E18 RID: 19992
			Highlighted
		}
	}
}
