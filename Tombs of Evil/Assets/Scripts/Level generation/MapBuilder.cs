using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapBuilder : MonoBehaviour
{
    public static MapBuilder current;

    [Header("Tile types")]
    public RuleTile Ground;

    [Header("Tilemaps")]
    public Tilemap groundTilemap;
    public Tilemap interactableTileMap;
    [Header("Dungeon exit Data")]
    public Tile Exit;
    public Vector3 ExitDungeonLevel;
    [Header("Test Mode")]
    public bool TestModeOn;
    public Tile[] testTiles;

    private void Awake()
    {
        current = this;
    }

    public void Start()
    {
        GameEvents.current.OnNextRound += ReachedExit;
    }

    private void ReachedExit()
    {

        if (PlayerController.current.transform.position != ExitDungeonLevel)
            return;

        MapGenerator.current.GenerateMap();
    }

    public void BuildMap(int[,] map)
    {
        groundTilemap.ClearAllTiles();
        interactableTileMap.ClearAllTiles();

        int width = map.GetLength(0);
        int height = map.GetLength(1);

        Vector3Int[] positions = new Vector3Int[map.Length];
        TileBase[] tileArray = new TileBase[positions.Length];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int index = x + (y * width);
                positions[index] = new Vector3Int(x, y, 0);
                if (!TestModeOn)
                {
                    if (map[x, y] == 0)
                    {
                        tileArray[index] = Ground;
                    }
                    if (map[x, y] == 2)
                    {
                        tileArray[index] = Ground;
                    }
                }
                else
                {
                    if (map[x, y] == 0)
                    {
                        tileArray[index] = testTiles[0];
                    }
                    if(map[x, y] == 2)
                    {
                        tileArray[index] = testTiles[1];
                    }
                    if (map[x, y] == 3)
                    {
                        tileArray[index] = testTiles[2];
                    }
                }
            }

        }

        groundTilemap.SetTiles(positions, tileArray);
    }

    public void SpawnExit()
    {
        var spawnbleTiles = MapGenerator.current.GetSpawnbleTiles();
        int random = UnityEngine.Random.Range(0, spawnbleTiles.Count);

        ExitDungeonLevel = spawnbleTiles[random];
        SetDungeonExit(ExitDungeonLevel);
    }

    public void SetTileNull(Vector3Int pos)
    {
        groundTilemap.SetTile(pos, null); // Remove tile at 0,0,0
    }

    public void SetDungeonExit(Vector3 pos)
    {
        Vector3Int posInt = new Vector3Int((int)pos.x, (int)pos.y, (int)pos.z);
        interactableTileMap.SetTile(posInt, Exit); // Remove tile at 0,0,0
    }

}
