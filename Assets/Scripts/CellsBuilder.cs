using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellsBuilder : MonoBehaviour
{
	[Range(2, 50)] [SerializeField] int mazeWidth = 10;
	[Range(2, 50)] [SerializeField] int mazeHeight = 10;

	[SerializeField] Transform mazeStartPosition = null;

	[Tooltip("Add here the cell prefab to be generated and has 4 walls w/ colliders.")]
	[SerializeField] GameObject cellModel = null;
	[Tooltip("Distance between cells depend on the size of the cell.")]
	[SerializeField] float distanceBetweenCells = 2f;

	public List<Cells> cells = new List<Cells>();
	Vector3 pointer = new Vector3(0, 0, 0);

	[SerializeField] bool debugView = true;

	public class Cells
	{
		public Vector2 number;
		public Vector3 position = new Vector3(0, 0, 0);
		public GameObject model;
	}

	public void ResizeMazeHeight(float newHeight)
	{
		mazeHeight = (int)newHeight;
	}
	public void ResizeMazeWidth(float newWidth)
	{
		mazeWidth = (int)newWidth;
	}

	public void Reset()
	{
		foreach (var cell in cells)
		{
			Destroy(cell.model);
		}

		cells.Clear();
	}
	public void StartBuilding()
	{
		pointer = mazeStartPosition.position;

		int cellsNum = 0;
		float x = 0;

		for (int i = 0; i < mazeWidth; i++)
		{
			float z = 0;

			for (int j = 0; j < mazeHeight; j++)
			{
				pointer = new Vector3(x, pointer.y, z);

				Cells cell = new Cells()
				{
					number = new Vector2(i, j),
					position = pointer,
					model = null
				};

				cells.Add(cell);

				cells[cellsNum].model = Instantiate(cellModel);
				cells[cellsNum].model.transform.position = cells[cellsNum].position;
				cells[cellsNum].model.gameObject.name = "Cell " + cells[cellsNum].number.x + ", " + cells[cellsNum].number.y;

				cellsNum++;
				z += distanceBetweenCells;
			}
			x += distanceBetweenCells;
		}
	}

	void OnDrawGizmos()
	{
		if (debugView)
			foreach (var cell in cells)
			{
				Gizmos.color = Color.yellow;
				Gizmos.DrawSphere(cell.position, 0.3f);
			}
	}
}
