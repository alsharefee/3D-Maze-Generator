using System.Collections.Generic;
using UnityEngine;

namespace MazeGenerator
{
    public class CellsBuilder : MonoBehaviour
    {
        public List<Cell> Cells = new List<Cell>();

        [Range(2, 50)] [SerializeField] int _mazeWidth = 10;
        [Range(2, 50)] [SerializeField] int _mazeHeight = 10;
        [SerializeField] Transform _mazeStartPosition = null;
        [Tooltip("Add here the cell prefab to be generated and has 4 walls w/ colliders.")]
        [SerializeField] GameObject _cellModel = null;
        [Tooltip("Distance between cells depend on the size of the cell.")]
        [SerializeField] float _distanceBetweenCells = 2f;
        [SerializeField] bool _debugView = true;

        Vector3 _pointer = new Vector3(0, 0, 0);

        public class Cell
        {
            public Vector2 Number;
            public Vector3 Position = new Vector3(0, 0, 0);
            public GameObject Model;
        }

        public void ResizeMazeHeight(float newHeight)
        {
            _mazeHeight = (int)newHeight;
        }
        public void ResizeMazeWidth(float newWidth)
        {
            _mazeWidth = (int)newWidth;
        }

        public void Reset()
        {
            foreach (var cell in Cells)
            {
                Destroy(cell.Model);
            }

            Cells.Clear();
        }
        public void StartBuildingCells()
        {
            _pointer = _mazeStartPosition.position;

            int cellsNum = 0;
            float x = 0;

            for (int i = 0; i < _mazeWidth; i++)
            {
                float z = 0;

                for (int j = 0; j < _mazeHeight; j++)
                {
                    BuildCell(ref cellsNum, x, i, ref z, j);
                }

                x += _distanceBetweenCells;
            }
        }

        private void BuildCell(ref int cellsNum, float x, int i, ref float z, int j)
        {
            _pointer = new Vector3(x, _pointer.y, z);

            Cell cell = new Cell()
            {
                Number = new Vector2(i, j),
                Position = _pointer,
                Model = null
            };

            Cells.Add(cell);

            Cells[cellsNum].Model = Instantiate(_cellModel);
            Cells[cellsNum].Model.transform.position = Cells[cellsNum].Position;
            Cells[cellsNum].Model.gameObject.name = "Cell " + Cells[cellsNum].Number.x + ", " + Cells[cellsNum].Number.y;

            cellsNum++;
            z += _distanceBetweenCells;
        }

        void OnDrawGizmos()
        {
            if (_debugView)
                foreach (var cell in Cells)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawSphere(cell.Position, 0.3f);
                }
        }
    }

}