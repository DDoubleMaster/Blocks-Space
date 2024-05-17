using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 2;
	[SerializeField] float rotationSpeed = 2;

    Vector3 viewPos = new Vector3(-1.5f, 0.8f, -1.5f);
	Vector3 mainPos;
    Quaternion mainRot;

    Dictionary<string, Planet> planets = new Dictionary<string, Planet>();
	[HideInInspector] public List<string> planetNames = new List<string>();
    [HideInInspector] public string currentPlanet = "";
    Vector3 finalPos;
    Quaternion finalRot;

    bool puzzleLaunched = false;

	// Start is called before the first frame update
	void Start()
    {
        Transform planetsParent = GameObject.Find("Planets").transform;
        int planetsCount = planetsParent.childCount;

		for (int planetIndex = 0; planetIndex < planetsCount; planetIndex++)
        {
            GameObject currentPlanet = planetsParent.GetChild(planetIndex).gameObject;
            planets.Add(currentPlanet.name, currentPlanet.GetComponent<Planet>());
            planetNames.Add(currentPlanet.name);

		}

        mainPos = transform.position;
        mainRot = transform.rotation;
    }

	[HideInInspector] public float timeToLaunch;
	[HideInInspector] public int planetIndex;
    // Update is called once per frame
    void Update()
    {
        if (!puzzleLaunched)
        {

			if (Input.GetKeyDown(KeyCode.Escape))
				currentPlanet = "";

            if (Input.GetKey(KeyCode.Space) && currentPlanet != "" && !isAutoRotate)
            {
				timeToLaunch += Time.deltaTime;

                if (timeToLaunch > 5)
                {
					puzzleLaunched = true;
                    Launch();
				}
			}
            else
				timeToLaunch = Mathf.Lerp(timeToLaunch, 0, 5 * Time.deltaTime);

			if (currentPlanet != "")
			{
				if (Input.GetKeyDown(KeyCode.A))
				{
					timeToLaunch = 0;
					planetIndex++;
					int currentIndex = Mathf.Clamp(planetIndex, 0, planetNames.Count - 1);
					currentPlanet = planets[planetNames[currentIndex]].name;
					planetIndex = currentIndex;
				}
				else if (Input.GetKeyDown(KeyCode.D))
				{
					timeToLaunch = 0;
					planetIndex--;
					int currentIndex = Mathf.Clamp(planetIndex, 0, planetNames.Count - 1);
					currentPlanet = planets[planetNames[currentIndex]].name;
					planetIndex = currentIndex;
				}

				planets[currentPlanet].transform.GetChild(1).transform.localScale = Vector3.one * timeToLaunch * (2 + Random.Range(0f, .1f));

				Vector3 currentPlanetPos = planets[currentPlanet].transform.position;
				float currentPlanetRadius = planets[currentPlanet].transform.localScale.x;

				finalPos = currentPlanetPos + viewPos * currentPlanetRadius * 1.2f;
				finalRot = Quaternion.LookRotation(currentPlanetPos - transform.position);
			}
			else
			{
				finalPos = mainPos;
				finalRot = mainRot;
			}
		}else
        {
			planets[currentPlanet].transform.GetChild(1).transform.localScale = Vector3.one * Random.Range(5f + timeToLaunch, 10f);

            if (Input.GetKey(KeyCode.Escape))
            {
                timeToLaunch -= Time.deltaTime;

                if (timeToLaunch < 0)
                {
					puzzleLaunched = false;
                    Ending();
				}
			}
			else
                timeToLaunch = Mathf.Lerp(timeToLaunch, 5, 1 * Time.deltaTime);

		}
    }

	string oldPlanet = "";
	bool isAutoRotate = true;
	private void LateUpdate()
	{
        Vector3 currentPos = Vector3.Slerp(transform.position, finalPos, moveSpeed * Time.deltaTime);
        Quaternion currentRot = Quaternion.Slerp(transform.rotation, finalRot, rotationSpeed * Time.deltaTime);

		if (isAutoRotate || currentPlanet != oldPlanet)
		{
			float posMagnitude = (currentPos - finalPos).magnitude;
			float rotMagnitude = (currentRot.eulerAngles - finalRot.eulerAngles).magnitude;
			transform.position = currentPos;
			transform.rotation = currentRot;
			oldPlanet = currentPlanet;

			isAutoRotate = true; if (Mathf.Floor(posMagnitude) == 0 && Mathf.Floor(rotMagnitude) == 0)
				isAutoRotate = false;
		}
		else if (!isAutoRotate && currentPlanet != "")
		{
			float radius = Mathf.Clamp(timeToLaunch / 3, 1.2f, 5) * planets[currentPlanet].transform.localScale.x;
			transform.position = (viewPos * radius) + planets[currentPlanet].transform.position;
		}
        // Future Camera Movement inside the puzzle
	}

    GameObject sun;
    private void Launch()
    {
		planets[currentPlanet].LaunchPuzzle();
        RenderSettings.ambientLight = Color.white;

        sun = GameObject.Find("Sun");
        sun.SetActive(false);
        foreach (Planet planet in planets.Values)
			if (planet.name != currentPlanet)
				planet.gameObject.SetActive(false);
	}

	private void Ending()
	{
		planets[currentPlanet].EndingPuzzle();
		RenderSettings.ambientLight = Color.clear;

		sun.SetActive(true);
		foreach (Planet planet in planets.Values)
			planet.gameObject.SetActive(true);
	}
}
