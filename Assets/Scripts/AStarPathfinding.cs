using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinding
{
    private GridManager _gridManager;

    public AStarPathfinding(GridManager gridManager)
    {
        _gridManager = gridManager;
    }

    public List<Tile> FindPath(Tile startTile, Tile targetTile)
    {
        List<Tile> openSet = new List<Tile>();
        HashSet<Tile> closedSet = new HashSet<Tile>();
        openSet.Add(startTile);

        while (openSet.Count > 0)
        {
            Tile currentTile = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FCost < currentTile.FCost || (openSet[i].FCost == currentTile.FCost && openSet[i].HCost < currentTile.HCost))
                {
                    currentTile = openSet[i];
                }
            }

            openSet.Remove(currentTile);
            closedSet.Add(currentTile);

            if (currentTile == targetTile)
            {
                return RetracePath(startTile, targetTile);
            }

            foreach (Tile neighbour in GetNeighbours(currentTile))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                {
                    continue;
                }

                int newMovementCostToNeighbour = currentTile.GCost + GetDistance(currentTile, neighbour);
                if (newMovementCostToNeighbour < neighbour.GCost || !openSet.Contains(neighbour))
                {
                    neighbour.GCost = newMovementCostToNeighbour;
                    neighbour.HCost = GetDistance(neighbour, targetTile);
                    neighbour.Parent = currentTile;

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                }
            }
        }

        return null;
    }

    private List<Tile> RetracePath(Tile startTile, Tile endTile)
    {
        List<Tile> path = new List<Tile>();
        Tile currentTile = endTile;

        while (currentTile != startTile)
        {
            path.Add(currentTile);
            currentTile = currentTile.Parent;
        }
        path.Reverse();
        return path;
    }

    private int GetDistance(Tile tileA, Tile tileB)
    {
        int dstX = Mathf.Abs(tileA._coordinates.x - tileB._coordinates.x);
        int dstY = Mathf.Abs(tileA._coordinates.y - tileB._coordinates.y);

        return dstX + dstY; // Manhattan distance for N/S/E/W movement
    }

    private List<Tile> GetNeighbours(Tile tile)
    {
        List<Tile> neighbours = new List<Tile>();

        // Define the four possible directions: North, South, East, West
        Vector2[] directions = new Vector2[]
        {
            new Vector2(0, 1),  // North
            new Vector2(0, -1), // South
            new Vector2(1, 0),  // East
            new Vector2(-1, 0)   // West
        };

        foreach (var direction in directions)
        {
            Vector2 checkPos = new Vector2(tile._coordinates.x + direction.x, tile._coordinates.y + direction.y);
            Tile neighbour = _gridManager.GetTileAtCord(checkPos);
            if (neighbour != null && neighbour.walkable)
            {
                neighbours.Add(neighbour);
            }
        }

        return neighbours;
    }
}