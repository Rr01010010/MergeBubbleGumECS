using UnityEngine;

public class MakeLevelGrid : MonoBehaviour
{
    #region Grid
    public float CellSize
    {
        get 
        {
            if (_cellSize < 0.03f) _cellSize = 0.03f;
            return _cellSize;
        }
        set => _cellSize = value;
    }

    [Header("Grid Settings")]
    [SerializeField] float _cellSize;
    [SerializeField] int height;
    [SerializeField] int width;
    [SerializeField] string numberCells;
    private void GizmoDrawGrid()
    {
        Gizmos.color = Color.white;
        numberCells = (height * width).ToString();

        int y = 0;
        float maxHeight = (height * CellSize) / 2.0f;
        float minHeight = -(height * CellSize) / 2.0f;
        float maxWidth = (width * CellSize) / 2.0f;
        float minWidth = -(width * CellSize) / 2.0f;

        for (int h = 0; h < height / 2; h++)
        {
            Gizmos.DrawLine(new Vector3(minWidth, y, CellSize / 2 + h * CellSize), new Vector3(maxWidth, y, CellSize / 2 + h * CellSize));
            Gizmos.DrawLine(new Vector3(minWidth, y, -CellSize / 2 - h * CellSize), new Vector3(maxWidth, y, -CellSize / 2 - h * CellSize));
        }
        for (int w = 0; w < width / 2; w++)
        {
            Gizmos.DrawLine(new Vector3(CellSize / 2 + w * CellSize, y, minHeight), new Vector3(CellSize / 2 + w * CellSize, y, maxHeight));
            Gizmos.DrawLine(new Vector3(-CellSize / 2 - w * CellSize, y, minHeight), new Vector3(-CellSize / 2 - w * CellSize, y, maxHeight));
        }
    }
    #endregion
    public bool DrawGrid = true;
    private void OnDrawGizmos()
    {
        if (DrawGrid) { GizmoDrawGrid(); }

    }


}