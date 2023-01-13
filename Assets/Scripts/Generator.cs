using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MazeGenerator
{
    /// <summary>
    /// Generate the cells and destroy the walls in those cells to make way for players 
    /// to navigate through the maze.
    /// </summary>
    public class Generator : MonoBehaviour
    {
        [SerializeField] CellsBuilder _cellsBuilder;
        [Tooltip("We use time because unity won't detect a physics raycast if it was rayed once.")]
        [Range(0.02f, 5)] [SerializeField] float _collisionTimeCheck = 0.1f;
        [SerializeField] UnityEvent _onMazeCompleted;
        [SerializeField] bool _debugView = true;

        List<CellsBuilder.Cell> _BuiltCells = new List<CellsBuilder.Cell>();

        public void Generate()
        {
            Reset();
            BuildCells();
            StartCoroutine(DestroyCellsWalls());
        }

        private void Reset()
        {
            StopAllCoroutines();
            _BuiltCells.Clear();
            _cellsBuilder.Reset();
        }

        void BuildCells()
        {
            _cellsBuilder.StartBuildingCells();
        }

        /// <summary>
        /// We destroy the cells of the cells to make way for the maze. Otherwise it will only be cells with 4 walls.
        /// </summary>
        IEnumerator DestroyCellsWalls()
        {
            _BuiltCells = _cellsBuilder.Cells;

            for (int i = 0; i < _BuiltCells.Count; i++)
            {
                int randomX = 0;
                int randomY = 0;

                while (true)
                {
                    randomX = (int)UnityEngine.Random.Range(_BuiltCells[i].Number.x - 1, _BuiltCells[i].Number.x + 1);
                    randomY = (int)UnityEngine.Random.Range(_BuiltCells[i].Number.y - 1, _BuiltCells[i].Number.y + 1);

                    if (randomX < _BuiltCells[0].Number.x || randomY < _BuiltCells[0].Number.y)
                    {
                        continue;
                    }
                    else if (randomX > _BuiltCells[_BuiltCells.Count - 1].Number.x || randomY > _BuiltCells[_BuiltCells.Count - 1].Number.y)
                    {
                        continue;
                    }

                    int totalCellNumber = (int)_BuiltCells[i].Number.x + (int)_BuiltCells[i].Number.y;
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

                foreach (var checkCell in _BuiltCells)
                {
                    if (checkCell.Number == targetCellNum)
                    {
                        float timerToCheckIfRaycastHit = _collisionTimeCheck;

                        while (timerToCheckIfRaycastHit > 0)
                        {
                            RaycastHit hit;
                            if (Physics.Linecast(_BuiltCells[i].Position, checkCell.Position, out hit))
                            {
                                if (_debugView)
                                    Debug.DrawLine(_BuiltCells[i].Position, checkCell.Position, Color.green, 60);

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

            _onMazeCompleted.Invoke();
        }
    } 
}
