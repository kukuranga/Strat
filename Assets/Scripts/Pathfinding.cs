using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{
    public static List<Tile> FindPath(Tile startTile, Tile endTile, List<Vector2> movementOffsets)
    {
        List<Tile> openList = new List<Tile>();
        HashSet<Tile> closedList = new HashSet<Tile>();
        Dictionary<Tile, Tile> cameFrom = new Dictionary<Tile, Tile>();
        Dictionary<Tile, float> gScore = new Dictionary<Tile, float>(); // Cost from start to current
        Dictionary<Tile, float> fScore = new Dictionary<Tile, float>(); // Estimated total cost (g + h)

        openList.Add(startTile);
        gScore[startTile] = 0f;
        fScore[startTile] = Heuristic(startTile, endTile);

        while (openList.Count > 0)
        {
            // Get the tile with the lowest fScore
            Tile current = GetTileWithLowestFScore(openList, fScore);

            if (current == endTile)
            {
                // Reconstruct the path
                List<Tile> path = new List<Tile>();
                while (cameFrom.ContainsKey(current))
                {
                    path.Insert(0, current);
                    current = cameFrom[current];
                }
                return path;
            }

            openList.Remove(current);
            closedList.Add(current);

            foreach (var offset in movementOffsets)
            {
                Vector2 targetPos = new Vector2(current._coordinates.x + (int)offset.x, current._coordinates.y + (int)offset.y);
                if (GridManager.Instance._tiles.TryGetValue(targetPos, out Tile neighbor))
                {
                    if (closedList.Contains(neighbor) || !neighbor.walkable || neighbor.occupiedUnit != null)
                    {
                        continue; // Skip already processed or invalid neighbors
                    }

                    float tentativeGScore = gScore[current] + 1; // Assuming uniform cost

                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                    else if (tentativeGScore >= gScore[neighbor])
                    {
                        continue;
                    }

                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = gScore[neighbor] + Heuristic(neighbor, endTile);
                }
            }
        }

        return null; // No path found
    }

    // Heuristic function: using Manhattan distance
    private static float Heuristic(Tile a, Tile b)
    {
        return Mathf.Abs(a._coordinates.x - b._coordinates.x) + Mathf.Abs(a._coordinates.y - b._coordinates.y);
    }

    // Get the tile with the lowest fScore
    private static Tile GetTileWithLowestFScore(List<Tile> openList, Dictionary<Tile, float> fScore)
    {
        Tile lowest = openList[0];
        foreach (var tile in openList)
        {
            if (fScore[tile] < fScore[lowest])
            {
                lowest = tile;
            }
        }
        return lowest;
    }
}
