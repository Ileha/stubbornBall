using System.Collections;
using System.Collections.Generic;
using Game.Goods.Abstract;
using UnityEngine;

public class Fan : AbstractLevelComponent {
	public float liftingForse;
	public float sideForse;

	void OnTriggerStay2D(Collider2D other) {
		Rigidbody2D rigibody = other.GetComponent<Rigidbody2D>();

		if (rigibody != null) {
			Vector3 toFan = gameObject.transform.position - rigibody.transform.position;

			//Debug.DrawRay(rigibody.transform.position,(transform.right * 0.1f / Vector3.Dot(toFan, transform.right)* sideForse));
			//Debug.DrawRay(transform.position, -transform.up * 1f/Vector3.Dot(toFan, transform.up), Color.red);
//(transform.right * Vector3.Dot(toFan, transform.right) * sideForse)
			rigibody.AddForce((-transform.up * 1f / Vector3.Dot(toFan, transform.up) * liftingForse), ForceMode2D.Force);
		}
	}
}
