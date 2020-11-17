using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FogOfWar : MonoBehaviour
{
    public static FogOfWar current;

    public Tilemap fovTileMap;
    public Tile fovTile; //That's your previously created Fog of War tile - assign in editor!

    private bool[,] FogFlags;
    private int fogWidth;
    private int fogHeight;

    // max revealed tile depth (from start)
    private const int MAX_RANGE = 5;
    private const int MAX_X_LOOKUP = 20;
    private const int MAX_Y_LOOKUP = 20;

    private void Awake()
    {
        current = this;
    }

    private void Start()
    {
        GameEvents.current.OnNextRound += UpdateFog;
    }

    public void BuildFog(int[,] map)
    {
        fogWidth = map.GetLength(0) +1;
        fogHeight = map.GetLength(1)+1;

        FogFlags = new bool[fogWidth, fogHeight];

        ResetMap();

    }

    public void UpdateFog()
    {
        Vector3 Playerpos = PlayerController.current.transform.position;
        int x = (int)Playerpos.x;
        int y = (int)Playerpos.y;

        Recalculate(x, y);
    }

    public void RayCasting(int level,int x, int y,int directionX,int directionY,bool leftCorner,bool rightCorner)
    {
        if (level < MAX_RANGE)
        {
            // if we are out of bounds
            if (x < 0 || y < 0 || x >= MapGenerator.current.width || y >= MapGenerator.current.height)
            {
                return;
            }
            else
            {
                FogFlags[x, y] = true;
                if (MapGenerator.current.map[x,y] != 1)
                {
                    if (leftCorner)
                    {
                        if (directionX != 0)
                        {
                            RayCasting(level + 1, x + directionX, y + directionX, directionX, directionY, leftCorner, false);
                        }
                        else
                        {
                            RayCasting(level + 1, x + directionY, y + directionY, directionX, directionY, leftCorner, false);
                        }
                    }

                    if (rightCorner)
                    {
                        if (directionX != 0)
                        {
                            RayCasting(level + 1, x + directionX, y - directionX, directionX, directionY, rightCorner, false);
                        }
                        else
                        {
                            RayCasting(level + 1, x - directionY, y + directionY, directionX, directionY, rightCorner, false);
                        }
                    }

                    RayCasting(level + 1, x + directionX, y + directionY, directionX, directionY, leftCorner, false);
                }
            }
        }
        else
        {
            return;
        }
    }

    public void Recalculate(int x, int y)
    {
        // the player moves here
        FogFlags[x, y] = true;

        // raycast to all directions
        RayCasting(0, x, y, 0, 1, true, true);
        RayCasting(0, x, y, 0, -1, true, true);
        RayCasting(0, x, y, 1, 0, true, true);
        RayCasting(0, x, y, -1, 0, true, true);

        RayCasting(0, x, y, -1, 1, true, true);
        RayCasting(0, x, y, 1, 1,  true, true);
        RayCasting(0, x, y, -1, -1, true, true);
        RayCasting(0, x, y, 1, -1,  true, true);

        Redraw(x, y);
    }


    public void Redraw(int currentX, int currentY)
    {
        for (int x = -MAX_X_LOOKUP + currentX; x < MAX_X_LOOKUP + currentX + 1; x++)
        {
            for (int y = -MAX_Y_LOOKUP + currentY; y < MAX_Y_LOOKUP + currentY; y++)
            {
                if (x < 0 || y < 0 || x > fogWidth - 1 || y > fogHeight - 1)
                {
                    continue;
                }
                if (FogFlags[x, y])
                {
                    SetTile(x, y, null);
                }
            }
        }
    }

    private void SetTile(int x, int y,Tile tile)
    {
        Vector3Int posInt = new Vector3Int(x, y, 0);
        fovTileMap.SetTile(posInt, tile);
    }

    private void ResetMap()
    {
        for (int x = 0; x < fogWidth; x++)
        {
            for (int y = 0; y < fogHeight; y++)
            {
                SetTile(x, y, fovTile);
            }
        }
    }
}
