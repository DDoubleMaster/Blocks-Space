using UnityEngine;

public class Planet : MonoBehaviour
{
	[Range(0, 3)] public int level;
	Puzzle puzzle;
	[SerializeField] float rotateAroundSpeed;
    [SerializeField] float rotateSpeed;
	public bool isOpen = false;
    float currentAngle;

	CameraController camera_C;
    Transform sun_T;

	GameObject constaint;

    // Start is called before the first frame update
    void Start()
    {
		constaint = transform.GetChild(0).gameObject;
		camera_C = Camera.main.GetComponent<CameraController>();
		sun_T = GameObject.Find("Sun").transform;

		float randRotate = Random.value * 360;
		transform.RotateAround(sun_T.position, Vector3.up, randRotate);

		constaint.transform.localScale *= 4;
	}

    // Update is called once per frame
    void Update()
    {
        currentAngle = Mathf.Atan2(transform.position.x, transform.position.z);

		GameObject child = transform.GetChild(1).gameObject;
		if (camera_C.currentPlanet != name)
			child.transform.localScale = Vector3.Lerp(child.transform.localScale, Vector3.zero, 5 * Time.deltaTime);

		if (puzzle == null)
			transform.Rotate(Vector3.up * (rotateSpeed + rotateAroundSpeed));

		if (camera_C.currentPlanet == name && gameObject.layer != LayerMask.NameToLayer("CurrentLayer"))
		{
			gameObject.layer = LayerMask.NameToLayer("CurrentLayer");
			foreach (Transform toLayer in transform)
				toLayer.gameObject.layer = LayerMask.NameToLayer("CurrentLayer");
		}
		else if (camera_C.currentPlanet != name && gameObject.layer != LayerMask.NameToLayer("Default"))
		{
			gameObject.layer = LayerMask.NameToLayer("Default");
			foreach (Transform toLayer in transform)
			{
				toLayer.gameObject.layer = LayerMask.NameToLayer("Default");
			}
		}
	}

	public void LaunchPuzzle()
	{
		/*constaint.SetActive(true);*/
		puzzle = gameObject.AddComponent<Puzzle>();
	}

	public void EndingPuzzle()
	{
		/*constaint.SetActive(false);*/
		Destroy(puzzle);
	}

	private void OnMouseDown()
	{
		Debug.Log("Clicked");
		if (isOpen)
		{
			camera_C.currentPlanet = name;
			camera_C.timeToLaunch = 0;
			camera_C.planetIndex = camera_C.planetNames.IndexOf(name);
		}
	}

	private void FixedUpdate()
	{
		if (!isOpen)
			transform.RotateAround(sun_T.position, Vector3.up, rotateAroundSpeed);
		else
			transform.RotateAround(sun_T.position, Vector3.up, -currentAngle);
	}
}
