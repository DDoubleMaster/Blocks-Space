using System.Collections.Generic;
using UnityEngine;

public class Puzzle : MonoBehaviour
{
	int level;
	[HideInInspector] public int multiplicity;

	GameObject puzzleGMObj;

	// Start is called before the first frame update
	void Start()
	{
		level = GetComponent<Planet>().level;
		multiplicity = (int)Mathf.Pow(2, level + 1);
		Texture2D texture = TextureMaker();

		int width = texture.width;
		int height = texture.height;

		// Создайте новый пустой объект для каждого цвета
		Dictionary<Color, List<GameObject>> colorGroups = new Dictionary<Color, List<GameObject>>();

		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				Color cubeColor = texture.GetPixel(i, j);

				// Создайте куб и добавьте его в соответствующий ColorGroup
				GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

				if (!colorGroups.ContainsKey(cubeColor))
					colorGroups.Add(cubeColor, new List<GameObject>());

				cube.transform.position = transform.position + new Vector3(i, j, 0);
				cube.layer = LayerMask.NameToLayer("CurrentLayer");

				// Установите цвет материала куба
				Material cubeMat = cube.GetComponent<Renderer>().material;
				cubeMat.shader = Shader.Find("Universal Render Pipeline/Unlit");
				cubeMat.color = cubeColor;
				colorGroups[cubeColor].Add(cube);
			}
		}
		puzzleGMObj = new GameObject("Puzzle");
		puzzleGMObj.transform.position = transform.position;
		puzzleGMObj.transform.parent = transform;
		puzzleGMObj.layer = LayerMask.NameToLayer("CurrentLayer");
		foreach (var colorGroup in colorGroups)
		{
			Color color = colorGroup.Key;
			List<GameObject> cubes = colorGroup.Value;

			Vector3 center = Vector3.zero;
			foreach (GameObject cube in cubes)
				center += cube.transform.position;

			GameObject parentObject = new GameObject(color + "CubesParent");
			parentObject.transform.position = center / cubes.Count;
			parentObject.layer = LayerMask.NameToLayer("CurrentLayer");

			foreach (GameObject cube in cubes)
				cube.transform.SetParent(parentObject.transform);
			parentObject.transform.SetParent(puzzleGMObj.transform);
			parentObject.AddComponent<Detail>();
		}
	}

	private void OnDestroy()
	{
		Destroy(puzzleGMObj);
	}

	private Texture2D TextureMaker()
	{
		List<Color> colorList = new List<Color>();
		Color[,] textureColor = new Color[multiplicity, multiplicity];

		bool haveEmptyColor = CheckForEmptyColors(textureColor);

		while (haveEmptyColor)
		{
			List<int[]> colorsIndex = new List<int[]>();
			Color currentColor = Random.ColorHSV(0, 1, 1, 1, 1, 1);

			while (colorList.Contains(currentColor))
				currentColor = Random.ColorHSV(0, 1, 1, 1, 1, 1);
			colorList.Add(currentColor);

			for (int color = 0; color < Random.Range(1, multiplicity * 2); color++)
			{
				if (colorsIndex.Count == 0)
				{
					int arrayWidth;
					int arrayHeight;
					do
					{
						arrayWidth = Random.Range(0, multiplicity);
						arrayHeight = Random.Range(0, multiplicity);
					} while (textureColor[arrayWidth, arrayHeight] != Color.clear);

					textureColor[arrayWidth, arrayHeight] = currentColor;
					colorsIndex.Add(new int[] { arrayWidth, arrayHeight });
				}
				else
				{

					List<int[]> emptyIndex = new List<int[]>();
					List<int[]> availableColorsIndex = colorsIndex;

					do
					{
						int arrayIndex = Random.Range(0, colorsIndex.Count - 1);
						int[] colorIndex = availableColorsIndex[arrayIndex];

						for (int currentIndex = 0; currentIndex < 4; currentIndex++)
						{
							switch (currentIndex)
							{
								case 0:
									try
									{
										int[] index = { colorIndex[0] + 1, colorIndex[1] };
										Color selectedColor = textureColor[index[0], index[1]];
										if (selectedColor == Color.clear)
											emptyIndex.Add(new int[] { index[0], index[1] });
										break;
									}
									catch
									{
										break;
									}

								case 1:
									try
									{
										int[] index = { colorIndex[0], colorIndex[1] + 1 };
										Color selectedColor = textureColor[index[0], index[1]];
										if (selectedColor == Color.clear)
											emptyIndex.Add(new int[] { index[0], index[1] });
										break;
									}
									catch
									{
										break;
									}

								case 2:
									try
									{
										int[] index = { colorIndex[0] - 1, colorIndex[1] };
										Color selectedColor = textureColor[index[0], index[1]];
										if (selectedColor == Color.clear)
											emptyIndex.Add(new int[] { index[0], index[1] });
										break;
									}
									catch
									{
										break;
									}

								case 3:
									try
									{
										int[] index = { colorIndex[0], colorIndex[1] - 1 };
										Color selectedColor = textureColor[index[0], index[1]];
										if (selectedColor == Color.clear)
											emptyIndex.Add(new int[] { index[0], index[1] });
										break;
									}
									catch
									{
										break;
									}
							}
						}
						if (emptyIndex.Count == 0)
						{
							availableColorsIndex.RemoveAt(arrayIndex);
							if (availableColorsIndex.Count == 0)
								break;
						}
					} while (availableColorsIndex.Count == 0);

					if (emptyIndex.Count == 0)
						break;
					int[] selectedIndex = emptyIndex[Random.Range(0, emptyIndex.Count - 1)];
					textureColor[selectedIndex[0], selectedIndex[1]] = currentColor;
					colorsIndex.Add(new int[] { selectedIndex[0], selectedIndex[1] });
				}
			}
			haveEmptyColor = CheckForEmptyColors(textureColor);
		}

		Color[] finallyTextureColor = new Color[multiplicity * multiplicity];

		for (int x = 0; x < multiplicity; x++)
		{
			for (int y = 0; y < multiplicity; y++)
			{
				finallyTextureColor[x + y * multiplicity] = textureColor[x, y];
			}
		}

		Texture2D texture = new Texture2D(multiplicity, multiplicity);

		texture.SetPixels(finallyTextureColor);
		texture.filterMode = FilterMode.Point;
		texture.Apply();
		return texture;
	}

	private bool CheckForEmptyColors(Color[,] colors)
	{
		int rows = colors.GetLength(0);
		int cols = colors.GetLength(1);

		for (int row = 0; row < rows; row++)
		{
			for (int col = 0; col < cols; col++)
			{
				if (colors[row, col] == Color.clear) // Проверка на пустой цвет (Color.clear)
				{
					return true; // Найден пустой цвет
				}
			}
		}

		return false; // Нет пустых цветов
	}
}
