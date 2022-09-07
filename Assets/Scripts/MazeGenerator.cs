using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MazeGenerator : MonoBehaviour
{
	[SerializeField] CellsBuilder cellsBuilder;

	[Tooltip("We use time because unity won't detect a physics raycast if it was rayed once.")]
	[Range(0.02f, 5)] [SerializeField] float timeToCheckIfThereIsCollider = 0.02f;

	[SerializeField] UnityEvent OnMazeCompleted;

	[SerializeField] bool debugView = true;

	List<CellsBuilder.Cells> cells = new List<CellsBuilder.Cells>();

	public void Generate()
	{
		Reset();
		BuildCells();
		StartCoroutine(GenerateMaze());
	}

	private void Reset()
	{
		StopAllCoroutines();
		cells.Clear();
		cellsBuilder.Reset();
	}

	 void BuildCells()
	{
		cellsBuilder.StartBuilding();

	}

	IEnumerator GenerateMaze()
	{
		cells = cellsBuilder.cells;

		for (int i = 0; i < cells.Count; i++)
		{
			int randomX = 0;
			int randomY = 0;

			while (true)
			{
				randomX = (int)UnityEngine.Random.Range(cells[i].number.x - 1, cells[i].number.x + 1);
				randomY = (int)UnityEngine.Random.Range(cells[i].number.y - 1, cells[i].number.y + 1);

				if (randomX < cells[0].number.x || randomY < cells[0].number.y)
				{
					continue;
				}
				else if (randomX > cells[cells.Count - 1].number.x || randomY > cells[cells.Count - 1].number.y)
				{
					continue;
				}

				int totalCellNumber = (int)cells[i].number.x + (int)cells[i].number.y;
				int checkTargetCellTotalNum = randomX + randomY;

				if (checkTargetCellTotalNum == totalCellNumber
				   || checkTargetCellTotalNum == totalCellNumber + 2
					|| checkTargetCellTotalNum == totalCellNumber - 2)
				{
					continue;
				}

				break;
			}

			Vector2 targetCellNum = new Vector2(randomX, randomY);

			foreach (var checkCell in cells)
			{
				if (checkCell.number == targetCellNum)
				{
					float timerToCheckIfRaycastHit = timeToCheckIfThereIsCollider;

					while (timerToCheckIfRaycastHit > 0)
					{
						RaycastHit hit;
						if (Physics.Linecast(cells[i].position, checkCell.position, out hit))
						{
							if(debugView)
							Debug.DrawLine(cells[i].position, checkCell.position, Color.green, 60);

							Destroy(hit.collider.gameObject);
							yield return new WaitForSeconds(0.02f); //We use 0.02f to resemble FixedUpdate since its recommended for physics.
							continue;
						}
						else
						{
							timerToCheckIfRaycastHit -= 0.02f;
						yield return new WaitForSeconds(0.02f);
							continue;
						}
					}
					break;
				}
			}
		}

		OnMazeCompleted.Invoke();
	}
}
