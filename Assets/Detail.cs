using UnityEngine;

public class Detail : MonoBehaviour
{
	GameObject planetObj;
	Planet planetPuzzle;
	BoxCollider _collider;
	void Start()
	{
		Debug.Log("Start create detail");
		planetObj = transform.parent.parent.gameObject;
		planetPuzzle = planetObj.GetComponent<Planet>();

		float planetRadius = planetObj.transform.localScale.x;
		float randomRadius = Random.Range(planetRadius * 0.8f, planetRadius * 1.5f);

		transform.position = planetObj.transform.position + Random.onUnitSphere * randomRadius;
		transform.rotation = Random.rotation;
		transform.localScale = Vector3.one / (6 + planetPuzzle.level + 1) * planetObj.transform.localScale.x / 2;

		RecalculateCollider();
		Rigidbody rb = gameObject.AddComponent<Rigidbody>();
		rb.AddForce(Random.insideUnitSphere);
	}

	private void OnMouseOver()
	{
		Debug.Log(name);
	}

	void RecalculateCollider()
	{
		Debug.Log("Start Recalculate collider");
		_collider = gameObject.AddComponent<BoxCollider>();
		Bounds bounds = new Bounds(transform.position, Vector3.zero);

		foreach (BoxCollider child in GetComponentsInChildren<BoxCollider>())
		{
			bounds.Encapsulate(child.bounds);
			Debug.Log($"child collides size: {child.bounds.size}");
		}

		_collider.center = bounds.center;
		_collider.size = bounds.size;
		Debug.Log($"Detail collides size: {bounds.size}");
	}
}
