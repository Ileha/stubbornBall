using UnityEngine;
using System.Collections;
using System;
using Cysharp.Threading.Tasks;
using Game.Goods.Abstract;

public class Finish : AbstractLevelComponent
{
	[SerializeField] private ParticleSystem particle;
	public float time = 5;

	void Awake() 
	{
		levelDataModel.OnRestart += Reset;
	}

	async void OnTriggerEnter2D(Collider2D other) 
	{
		if (!levelDataModel.IsCircle(other.gameObject)) { return; }

		Rigidbody2D rigidbody = other.gameObject.GetComponent<Rigidbody2D>();
		if (rigidbody != null)
		{
			await FallAsync(rigidbody);
			await UniTask.Delay(TimeSpan.FromSeconds(time));
			ParticleSystem particleResult = Instantiate(particle, other.transform.position, Quaternion.identity);
			Destroy(particleResult, 5);
			levelDataModel.Finish(other.gameObject);
		}
	}

	private async UniTask FallAsync(Rigidbody2D rigidbody)
	{
		await Fall(rigidbody);
	}

	private IEnumerator Fall(Rigidbody2D rigidbody)
	{
		Transform targetTransform = rigidbody.transform;
		Vector3 axis = Vector3.Cross(rigidbody.transform.position - gameObject.transform.position, rigidbody.velocity);

		rigidbody.velocity = Vector2.zero;
		rigidbody.angularVelocity = 0;
		rigidbody.bodyType = RigidbodyType2D.Kinematic;
		rigidbody.transform.position = new Vector3(rigidbody.transform.position.x, rigidbody.transform.position.y, transform.position.z - 1);
		rigidbody.GetComponent<Collider2D>().enabled = false;

		while (targetTransform.localScale != Vector3.zero) {
			targetTransform.localScale = Vector3.Lerp(targetTransform.localScale, Vector3.zero, Time.deltaTime);
			targetTransform.position = Vector3.Lerp(targetTransform.position, transform.position, Time.deltaTime);
			targetTransform.RotateAround(transform.position, axis, 270 * Time.deltaTime);
			yield return null;
		}	
	}

	private void OnDestroy()
	{
		levelDataModel.OnRestart -= Reset;
	}

	private void Reset() 
	{
		StopAllCoroutines();	
	}
}
