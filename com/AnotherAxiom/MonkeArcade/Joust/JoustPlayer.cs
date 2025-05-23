using System;
using UnityEngine;

namespace com.AnotherAxiom.MonkeArcade.Joust
{
	// Token: 0x02000CB2 RID: 3250
	public class JoustPlayer : MonoBehaviour
	{
		// Token: 0x170007F3 RID: 2035
		// (get) Token: 0x06005058 RID: 20568 RVA: 0x0017FE71 File Offset: 0x0017E071
		// (set) Token: 0x06005059 RID: 20569 RVA: 0x0017FE79 File Offset: 0x0017E079
		public float HorizontalSpeed
		{
			get
			{
				return this.HSpeed;
			}
			set
			{
				this.HSpeed = value;
			}
		}

		// Token: 0x0600505A RID: 20570 RVA: 0x0017FE84 File Offset: 0x0017E084
		private void LateUpdate()
		{
			this.velocity.x = this.HSpeed * 0.001f;
			if (this.flap)
			{
				this.velocity.y = Mathf.Min(this.velocity.y + 0.0005f, 0.0005f);
				this.flap = false;
			}
			else
			{
				this.velocity.y = Mathf.Max(this.velocity.y - Time.deltaTime * 0.0001f, -0.001f);
				int i = 0;
				while (i < Physics2D.RaycastNonAlloc(base.transform.position, this.velocity.normalized, this.raycastHitResults, this.velocity.magnitude))
				{
					JoustTerrain joustTerrain;
					if (this.raycastHitResults[i].collider.TryGetComponent<JoustTerrain>(out joustTerrain))
					{
						this.velocity.y = 0f;
						if (joustTerrain.transform.localPosition.y < base.transform.localPosition.y)
						{
							base.transform.localPosition = new Vector2(base.transform.localPosition.x, joustTerrain.transform.localPosition.y + this.raycastHitResults[i].collider.bounds.size.y);
							break;
						}
						break;
					}
					else
					{
						i++;
					}
				}
			}
			base.transform.Translate(this.velocity);
			if ((double)Mathf.Abs(base.transform.localPosition.x) > 4.5)
			{
				base.transform.localPosition = new Vector3(base.transform.localPosition.x * -0.95f, base.transform.localPosition.y);
			}
		}

		// Token: 0x0600505B RID: 20571 RVA: 0x0018006A File Offset: 0x0017E26A
		public void Flap()
		{
			this.flap = true;
		}

		// Token: 0x04005383 RID: 21379
		private Vector2 velocity;

		// Token: 0x04005384 RID: 21380
		private RaycastHit2D[] raycastHitResults = new RaycastHit2D[8];

		// Token: 0x04005385 RID: 21381
		private float HSpeed;

		// Token: 0x04005386 RID: 21382
		private bool flap;
	}
}
