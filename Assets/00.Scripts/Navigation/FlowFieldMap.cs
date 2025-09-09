using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class FlowFieldMap : MonoBehaviour
{
    [Required, SerializeField] private Transform pivot;
    [SerializeField] private Collider[] colliders;
    [SerializeField] private Vector2Int offset;
    [SerializeField] private Vector2Int mapSize = new Vector2Int(10, 10);

    [Required, SerializeField] Grid grid;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    

    [Button]
    public void RebuildMap()
    {

    }

    struct Obstacle
    {
        public Vector3Int min;
        public Vector3Int max;

        public Obstacle(Vector3Int min, Vector3Int max)
        {
            this.min = min;
            this.max = max;
        }

        public bool IsOuter(Vector3Int pos)
        {
            if (pos.x < min.x || pos.x > max.x || pos.y < min.y || pos.y > max.y) return true;
            
            return false;
        }
    }
    
    
    private void OnDrawGizmos()
    {
       
        var cellLayout = grid.cellLayout;

        var offsetVector3Int = new Vector3Int(offset.x, offset.y, 0);
        var toCenter = grid.cellSize * 0.5f;

        HashSet<Obstacle> blockCells = new HashSet<Obstacle>();
        foreach (Collider collider in colliders)
        {
            var bound = collider.bounds;
            var minCell = grid.WorldToCell(collider.bounds.min);
            var maxCell = grid.WorldToCell(collider.bounds.max);
            
            blockCells.Add(new Obstacle(minCell, maxCell));
        }
        
        Gizmos.color = Color.red;
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                Vector3Int cellPos = new Vector3Int(x, y, 0);
                cellPos += offsetVector3Int;

                bool skip = false;
                foreach (var obstacle in blockCells)
                {
                    if (!obstacle.IsOuter(cellPos))
                    {
                        skip = true;
                        break;
                    }
                }
                
                if(skip) continue;
                
                var cellWorldPosition =  grid.CellToWorld(cellPos);
                
                Gizmos.DrawCube(cellWorldPosition + toCenter, grid.cellSize);
            }
        }
    }
}
