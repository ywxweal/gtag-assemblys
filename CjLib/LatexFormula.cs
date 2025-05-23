using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02000E3C RID: 3644
	[ExecuteInEditMode]
	public class LatexFormula : MonoBehaviour
	{
		// Token: 0x04005F1B RID: 24347
		public static readonly string BaseUrl = "http://tex.s2cms.ru/svg/f(x) ";

		// Token: 0x04005F1C RID: 24348
		private int m_hash = LatexFormula.BaseUrl.GetHashCode();

		// Token: 0x04005F1D RID: 24349
		[SerializeField]
		private string m_formula = "";

		// Token: 0x04005F1E RID: 24350
		private Texture m_texture;
	}
}
