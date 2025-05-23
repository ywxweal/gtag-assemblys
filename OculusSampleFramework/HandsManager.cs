using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000BD3 RID: 3027
	public class HandsManager : MonoBehaviour
	{
		// Token: 0x1700073D RID: 1853
		// (get) Token: 0x06004AAF RID: 19119 RVA: 0x00163A44 File Offset: 0x00161C44
		// (set) Token: 0x06004AB0 RID: 19120 RVA: 0x00163A4E File Offset: 0x00161C4E
		public OVRHand RightHand
		{
			get
			{
				return this._hand[1];
			}
			private set
			{
				this._hand[1] = value;
			}
		}

		// Token: 0x1700073E RID: 1854
		// (get) Token: 0x06004AB1 RID: 19121 RVA: 0x00163A59 File Offset: 0x00161C59
		// (set) Token: 0x06004AB2 RID: 19122 RVA: 0x00163A63 File Offset: 0x00161C63
		public OVRSkeleton RightHandSkeleton
		{
			get
			{
				return this._handSkeleton[1];
			}
			private set
			{
				this._handSkeleton[1] = value;
			}
		}

		// Token: 0x1700073F RID: 1855
		// (get) Token: 0x06004AB3 RID: 19123 RVA: 0x00163A6E File Offset: 0x00161C6E
		// (set) Token: 0x06004AB4 RID: 19124 RVA: 0x00163A78 File Offset: 0x00161C78
		public OVRSkeletonRenderer RightHandSkeletonRenderer
		{
			get
			{
				return this._handSkeletonRenderer[1];
			}
			private set
			{
				this._handSkeletonRenderer[1] = value;
			}
		}

		// Token: 0x17000740 RID: 1856
		// (get) Token: 0x06004AB5 RID: 19125 RVA: 0x00163A83 File Offset: 0x00161C83
		// (set) Token: 0x06004AB6 RID: 19126 RVA: 0x00163A8D File Offset: 0x00161C8D
		public OVRMesh RightHandMesh
		{
			get
			{
				return this._handMesh[1];
			}
			private set
			{
				this._handMesh[1] = value;
			}
		}

		// Token: 0x17000741 RID: 1857
		// (get) Token: 0x06004AB7 RID: 19127 RVA: 0x00163A98 File Offset: 0x00161C98
		// (set) Token: 0x06004AB8 RID: 19128 RVA: 0x00163AA2 File Offset: 0x00161CA2
		public OVRMeshRenderer RightHandMeshRenderer
		{
			get
			{
				return this._handMeshRenderer[1];
			}
			private set
			{
				this._handMeshRenderer[1] = value;
			}
		}

		// Token: 0x17000742 RID: 1858
		// (get) Token: 0x06004AB9 RID: 19129 RVA: 0x00163AAD File Offset: 0x00161CAD
		// (set) Token: 0x06004ABA RID: 19130 RVA: 0x00163AB7 File Offset: 0x00161CB7
		public OVRHand LeftHand
		{
			get
			{
				return this._hand[0];
			}
			private set
			{
				this._hand[0] = value;
			}
		}

		// Token: 0x17000743 RID: 1859
		// (get) Token: 0x06004ABB RID: 19131 RVA: 0x00163AC2 File Offset: 0x00161CC2
		// (set) Token: 0x06004ABC RID: 19132 RVA: 0x00163ACC File Offset: 0x00161CCC
		public OVRSkeleton LeftHandSkeleton
		{
			get
			{
				return this._handSkeleton[0];
			}
			private set
			{
				this._handSkeleton[0] = value;
			}
		}

		// Token: 0x17000744 RID: 1860
		// (get) Token: 0x06004ABD RID: 19133 RVA: 0x00163AD7 File Offset: 0x00161CD7
		// (set) Token: 0x06004ABE RID: 19134 RVA: 0x00163AE1 File Offset: 0x00161CE1
		public OVRSkeletonRenderer LeftHandSkeletonRenderer
		{
			get
			{
				return this._handSkeletonRenderer[0];
			}
			private set
			{
				this._handSkeletonRenderer[0] = value;
			}
		}

		// Token: 0x17000745 RID: 1861
		// (get) Token: 0x06004ABF RID: 19135 RVA: 0x00163AEC File Offset: 0x00161CEC
		// (set) Token: 0x06004AC0 RID: 19136 RVA: 0x00163AF6 File Offset: 0x00161CF6
		public OVRMesh LeftHandMesh
		{
			get
			{
				return this._handMesh[0];
			}
			private set
			{
				this._handMesh[0] = value;
			}
		}

		// Token: 0x17000746 RID: 1862
		// (get) Token: 0x06004AC1 RID: 19137 RVA: 0x00163B01 File Offset: 0x00161D01
		// (set) Token: 0x06004AC2 RID: 19138 RVA: 0x00163B0B File Offset: 0x00161D0B
		public OVRMeshRenderer LeftHandMeshRenderer
		{
			get
			{
				return this._handMeshRenderer[0];
			}
			private set
			{
				this._handMeshRenderer[0] = value;
			}
		}

		// Token: 0x17000747 RID: 1863
		// (get) Token: 0x06004AC3 RID: 19139 RVA: 0x00163B16 File Offset: 0x00161D16
		// (set) Token: 0x06004AC4 RID: 19140 RVA: 0x00163B1D File Offset: 0x00161D1D
		public static HandsManager Instance { get; private set; }

		// Token: 0x06004AC5 RID: 19141 RVA: 0x00163B28 File Offset: 0x00161D28
		private void Awake()
		{
			if (HandsManager.Instance && HandsManager.Instance != this)
			{
				Object.Destroy(this);
				return;
			}
			HandsManager.Instance = this;
			this.LeftHand = this._leftHand.GetComponent<OVRHand>();
			this.LeftHandSkeleton = this._leftHand.GetComponent<OVRSkeleton>();
			this.LeftHandSkeletonRenderer = this._leftHand.GetComponent<OVRSkeletonRenderer>();
			this.LeftHandMesh = this._leftHand.GetComponent<OVRMesh>();
			this.LeftHandMeshRenderer = this._leftHand.GetComponent<OVRMeshRenderer>();
			this.RightHand = this._rightHand.GetComponent<OVRHand>();
			this.RightHandSkeleton = this._rightHand.GetComponent<OVRSkeleton>();
			this.RightHandSkeletonRenderer = this._rightHand.GetComponent<OVRSkeletonRenderer>();
			this.RightHandMesh = this._rightHand.GetComponent<OVRMesh>();
			this.RightHandMeshRenderer = this._rightHand.GetComponent<OVRMeshRenderer>();
			this._leftMeshRenderer = this.LeftHand.GetComponent<SkinnedMeshRenderer>();
			this._rightMeshRenderer = this.RightHand.GetComponent<SkinnedMeshRenderer>();
			base.StartCoroutine(this.FindSkeletonVisualGameObjects());
		}

		// Token: 0x06004AC6 RID: 19142 RVA: 0x00163C34 File Offset: 0x00161E34
		private void Update()
		{
			HandsManager.HandsVisualMode visualMode = this.VisualMode;
			if (visualMode > HandsManager.HandsVisualMode.Skeleton)
			{
				if (visualMode != HandsManager.HandsVisualMode.Both)
				{
					this._currentHandAlpha = 1f;
				}
				else
				{
					this._currentHandAlpha = 0.6f;
				}
			}
			else
			{
				this._currentHandAlpha = 1f;
			}
			this._rightMeshRenderer.sharedMaterial.SetFloat(this.HandAlphaId, this._currentHandAlpha);
			this._leftMeshRenderer.sharedMaterial.SetFloat(this.HandAlphaId, this._currentHandAlpha);
		}

		// Token: 0x06004AC7 RID: 19143 RVA: 0x00163CAF File Offset: 0x00161EAF
		private IEnumerator FindSkeletonVisualGameObjects()
		{
			while (!this._leftSkeletonVisual || !this._rightSkeletonVisual)
			{
				if (!this._leftSkeletonVisual)
				{
					Transform transform = this.LeftHand.transform.Find("SkeletonRenderer");
					if (transform)
					{
						this._leftSkeletonVisual = transform.gameObject;
					}
				}
				if (!this._rightSkeletonVisual)
				{
					Transform transform2 = this.RightHand.transform.Find("SkeletonRenderer");
					if (transform2)
					{
						this._rightSkeletonVisual = transform2.gameObject;
					}
				}
				yield return null;
			}
			this.SetToCurrentVisualMode();
			yield break;
		}

		// Token: 0x06004AC8 RID: 19144 RVA: 0x00163CBE File Offset: 0x00161EBE
		public void SwitchVisualization()
		{
			if (!this._leftSkeletonVisual || !this._rightSkeletonVisual)
			{
				return;
			}
			this.VisualMode = (this.VisualMode + 1) % (HandsManager.HandsVisualMode)3;
			this.SetToCurrentVisualMode();
		}

		// Token: 0x06004AC9 RID: 19145 RVA: 0x00163CF4 File Offset: 0x00161EF4
		private void SetToCurrentVisualMode()
		{
			switch (this.VisualMode)
			{
			case HandsManager.HandsVisualMode.Mesh:
				this.RightHandMeshRenderer.enabled = true;
				this._rightMeshRenderer.enabled = true;
				this._rightSkeletonVisual.gameObject.SetActive(false);
				this.LeftHandMeshRenderer.enabled = true;
				this._leftMeshRenderer.enabled = true;
				this._leftSkeletonVisual.gameObject.SetActive(false);
				return;
			case HandsManager.HandsVisualMode.Skeleton:
				this.RightHandMeshRenderer.enabled = false;
				this._rightMeshRenderer.enabled = false;
				this._rightSkeletonVisual.gameObject.SetActive(true);
				this.LeftHandMeshRenderer.enabled = false;
				this._leftMeshRenderer.enabled = false;
				this._leftSkeletonVisual.gameObject.SetActive(true);
				return;
			case HandsManager.HandsVisualMode.Both:
				this.RightHandMeshRenderer.enabled = true;
				this._rightMeshRenderer.enabled = true;
				this._rightSkeletonVisual.gameObject.SetActive(true);
				this.LeftHandMeshRenderer.enabled = true;
				this._leftMeshRenderer.enabled = true;
				this._leftSkeletonVisual.gameObject.SetActive(true);
				return;
			default:
				return;
			}
		}

		// Token: 0x06004ACA RID: 19146 RVA: 0x00163E14 File Offset: 0x00162014
		public static List<OVRBoneCapsule> GetCapsulesPerBone(OVRSkeleton skeleton, OVRSkeleton.BoneId boneId)
		{
			List<OVRBoneCapsule> list = new List<OVRBoneCapsule>();
			IList<OVRBoneCapsule> capsules = skeleton.Capsules;
			for (int i = 0; i < capsules.Count; i++)
			{
				if (capsules[i].BoneIndex == (short)boneId)
				{
					list.Add(capsules[i]);
				}
			}
			return list;
		}

		// Token: 0x06004ACB RID: 19147 RVA: 0x00163E60 File Offset: 0x00162060
		public bool IsInitialized()
		{
			return this.LeftHandSkeleton && this.LeftHandSkeleton.IsInitialized && this.RightHandSkeleton && this.RightHandSkeleton.IsInitialized && this.LeftHandMesh && this.LeftHandMesh.IsInitialized && this.RightHandMesh && this.RightHandMesh.IsInitialized;
		}

		// Token: 0x04004D6D RID: 19821
		private const string SKELETON_VISUALIZER_NAME = "SkeletonRenderer";

		// Token: 0x04004D6E RID: 19822
		[SerializeField]
		private GameObject _leftHand;

		// Token: 0x04004D6F RID: 19823
		[SerializeField]
		private GameObject _rightHand;

		// Token: 0x04004D70 RID: 19824
		public HandsManager.HandsVisualMode VisualMode;

		// Token: 0x04004D71 RID: 19825
		private OVRHand[] _hand = new OVRHand[2];

		// Token: 0x04004D72 RID: 19826
		private OVRSkeleton[] _handSkeleton = new OVRSkeleton[2];

		// Token: 0x04004D73 RID: 19827
		private OVRSkeletonRenderer[] _handSkeletonRenderer = new OVRSkeletonRenderer[2];

		// Token: 0x04004D74 RID: 19828
		private OVRMesh[] _handMesh = new OVRMesh[2];

		// Token: 0x04004D75 RID: 19829
		private OVRMeshRenderer[] _handMeshRenderer = new OVRMeshRenderer[2];

		// Token: 0x04004D76 RID: 19830
		private SkinnedMeshRenderer _leftMeshRenderer;

		// Token: 0x04004D77 RID: 19831
		private SkinnedMeshRenderer _rightMeshRenderer;

		// Token: 0x04004D78 RID: 19832
		private GameObject _leftSkeletonVisual;

		// Token: 0x04004D79 RID: 19833
		private GameObject _rightSkeletonVisual;

		// Token: 0x04004D7A RID: 19834
		private float _currentHandAlpha = 1f;

		// Token: 0x04004D7B RID: 19835
		private int HandAlphaId = Shader.PropertyToID("_HandAlpha");

		// Token: 0x02000BD4 RID: 3028
		public enum HandsVisualMode
		{
			// Token: 0x04004D7E RID: 19838
			Mesh,
			// Token: 0x04004D7F RID: 19839
			Skeleton,
			// Token: 0x04004D80 RID: 19840
			Both
		}
	}
}
