using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000B0F RID: 2831
	[CreateAssetMenu(fileName = "GorillaCaveCrystalSetup", menuName = "ScriptableObjects/GorillaCaveCrystalSetup", order = 0)]
	public class GorillaCaveCrystalSetup : ScriptableObject
	{
		// Token: 0x170006CA RID: 1738
		// (get) Token: 0x0600458F RID: 17807 RVA: 0x0014A9B3 File Offset: 0x00148BB3
		public static GorillaCaveCrystalSetup Instance
		{
			get
			{
				return GorillaCaveCrystalSetup.gInstance;
			}
		}

		// Token: 0x06004590 RID: 17808 RVA: 0x0014A9BA File Offset: 0x00148BBA
		private void OnEnable()
		{
			if (GorillaCaveCrystalSetup.gInstance == null)
			{
				GorillaCaveCrystalSetup.gInstance = this;
			}
		}

		// Token: 0x06004591 RID: 17809 RVA: 0x0014A9D0 File Offset: 0x00148BD0
		public GorillaCaveCrystalSetup.CrystalDef[] GetCrystalDefs()
		{
			return (from f in typeof(GorillaCaveCrystalSetup).GetRuntimeFields()
				where f != null && f.FieldType == typeof(GorillaCaveCrystalSetup.CrystalDef)
				select (GorillaCaveCrystalSetup.CrystalDef)f.GetValue(this)).ToArray<GorillaCaveCrystalSetup.CrystalDef>();
		}

		// Token: 0x04004827 RID: 18471
		public Material SharedBase;

		// Token: 0x04004828 RID: 18472
		public Texture2D CrystalAlbedo;

		// Token: 0x04004829 RID: 18473
		public Texture2D CrystalDarkAlbedo;

		// Token: 0x0400482A RID: 18474
		public GorillaCaveCrystalSetup.CrystalDef Red;

		// Token: 0x0400482B RID: 18475
		public GorillaCaveCrystalSetup.CrystalDef Orange;

		// Token: 0x0400482C RID: 18476
		public GorillaCaveCrystalSetup.CrystalDef Yellow;

		// Token: 0x0400482D RID: 18477
		public GorillaCaveCrystalSetup.CrystalDef Green;

		// Token: 0x0400482E RID: 18478
		public GorillaCaveCrystalSetup.CrystalDef Teal;

		// Token: 0x0400482F RID: 18479
		public GorillaCaveCrystalSetup.CrystalDef DarkBlue;

		// Token: 0x04004830 RID: 18480
		public GorillaCaveCrystalSetup.CrystalDef Pink;

		// Token: 0x04004831 RID: 18481
		public GorillaCaveCrystalSetup.CrystalDef Dark;

		// Token: 0x04004832 RID: 18482
		public GorillaCaveCrystalSetup.CrystalDef DarkLight;

		// Token: 0x04004833 RID: 18483
		public GorillaCaveCrystalSetup.CrystalDef DarkLightUnderWater;

		// Token: 0x04004834 RID: 18484
		[SerializeField]
		[TextArea(4, 10)]
		private string _notes;

		// Token: 0x04004835 RID: 18485
		[Space]
		[SerializeField]
		private GameObject _target;

		// Token: 0x04004836 RID: 18486
		private static GorillaCaveCrystalSetup gInstance;

		// Token: 0x04004837 RID: 18487
		private static GorillaCaveCrystalSetup.CrystalDef[] gCrystalDefs;

		// Token: 0x02000B10 RID: 2832
		[Serializable]
		public class CrystalDef
		{
			// Token: 0x04004838 RID: 18488
			public Material keyMaterial;

			// Token: 0x04004839 RID: 18489
			public CrystalVisualsPreset visualPreset;

			// Token: 0x0400483A RID: 18490
			[Space]
			public int low;

			// Token: 0x0400483B RID: 18491
			public int mid;

			// Token: 0x0400483C RID: 18492
			public int high;
		}
	}
}
