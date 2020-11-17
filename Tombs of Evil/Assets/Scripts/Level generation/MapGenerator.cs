using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public static MapGenerator current;

    public int width;
    public int height;
    [Range(0, 100)]
    public int randomFillPercent;
    [Range(0, 100)]
    public int wallThresholdSize;
    [Range(0, 100)]
    public int roomThresholdSize;

    public string seed;

    public bool UseRandomSeed;
    public float offset = 5f;   
    public int[,] map;


    private void Awake()
    {
        current = this;
    }

    private void Start()
    {
        GenerateMap();
    }


    #region Detecting Regions

    public struct Coord
    {
        public int tileX;
        public int tileY;

        public Coord(int x, int y)
        {
            tileX = x;
            tileY = y;
        }
    }

    public class Room : IComparable<Room>
    {
        public List<Coord> tiles;
        public List<Coord> edgeTiles;
        public List<Room> connectedRooms;
        public int roomSize;
        public bool isAccesibleFromMainRoom;
        public bool isMainRoom;

        public Room()
        {
        }

        public Room(List<Coord> roomTiles, int[,] map)
        {
            tiles = roomTiles;
            roomSize = tiles.Count;
            connectedRooms = new List<Room>();
            edgeTiles = new List<Coord>();

            foreach (var tile in tiles)
            {
                for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++)
                {
                    for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
                    {
                        if (x == tile.tileX || y == tile.tileY)
                        {
                            if (!MapGenerator.current.CheckIfInBorders(x,y))
                                continue;

                            if (map[x, y] == 1)
                            {
                                edgeTiles.Add(tile);
                                map[x, y] = 2;
                            }
                        }
                    }
                }
            }
        }

        public void SetAccesibleFromMainRoom()
        {
            if (!isAccesibleFromMainRoom)
            {
                isAccesibleFromMainRoom = true;
                foreach (var connectedRoom in connectedRooms)
                {
                    connectedRoom.SetAccesibleFromMainRoom();

                }
            }
        }

        public static void ConnectRooms(Room a, Room b)
        {
            if (a.isAccesibleFromMainRoom)
                b.SetAccesibleFromMainRoom();
            else if (b.isAccesibleFromMainRoom)
                a.SetAccesibleFromMainRoom();

            a.connectedRooms.Add(b);
            b.connectedRooms.Add(a);
        }
        public bool IsConnected(Room otherRoom)
        {
            return connectedRooms.Contains(otherRoom);
        }
        public int CompareTo(Room otherRoom)
        {
            return otherRoom.roomSize.CompareTo(roomSize);
        }

    }


    void ProcessRegionsInMap()
    {
        List<List<Coord>> wallRegions = GetRegions(1);

        foreach (List<Coord> regions in wallRegions)
        {
            if (regions.Count < wallThresholdSize)
            {
                foreach (var tile in regions)
                {
                    map[tile.tileX, tile.tileY] = 0;
                }
            }
        }
        List<List<Coord>> roomRegions = GetRegions(0);
        List<Room> leftOverRooms = new List<Room>();

        foreach (List<Coord> regions in roomRegions)
        {
            if (regions.Count < roomThresholdSize)
            {
                foreach (var tile in regions)
                {
                    map[tile.tileX, tile.tileY] = 1;
                }
            }
            else
            {
                leftOverRooms.Add(new Room(regions, map));
            }
        }
        leftOverRooms.Sort();
        //afther the list is sorted the room with the bigest roomsize is the mainroom 
        leftOverRooms[0].isMainRoom = true;
        leftOverRooms[0].isAccesibleFromMainRoom = true;
        ConnectClosestRooms(leftOverRooms);
    }

    void ConnectClosestRooms(List<Room> allRooms, bool forceAccessToMainRoom = false)
    {

        List<Room> roomListA = new List<Room>();
        List<Room> roomListB = new List<Room>();

        if (forceAccessToMainRoom)
        {
            foreach (var room in allRooms)
            {
                if (room.isAccesibleFromMainRoom)
                {
                    roomListB.Add(room);
                }
                else
                {
                    roomListA.Add(room);
                }
            }
        }
        else
        {
            roomListA = allRooms;
            roomListB = allRooms;
        }

        int recordDistance = 0;
        Coord recordTileA = new Coord();
        Coord recordTileB = new Coord();
        Room recordRoomA = new Room();
        Room recordRoomB = new Room();
        bool ConnectionFound = false;

        foreach (var roomA in roomListA)
        {
            if (!forceAccessToMainRoom)
            {
                ConnectionFound = false;
                if (roomA.connectedRooms.Count > 0)
                {
                    continue;
                }

            }

            foreach (var roomB in roomListB)
            {
                if (roomA == roomB || roomA.IsConnected(roomB))
                    continue;


                for (int a = 0; a < roomA.edgeTiles.Count; a++)
                {
                    for (int b = 0; b < roomB.edgeTiles.Count; b++)
                    {
                        Coord tileA = roomA.edgeTiles[a];
                        Coord tileB = roomB.edgeTiles[b];

                        int distanceRooms = (int)(Mathf.Pow(tileA.tileX - tileB.tileX, 2) + Mathf.Pow(tileA.tileY - tileB.tileY, 2));

                        if (distanceRooms < recordDistance || !ConnectionFound)
                        {
                            recordDistance = distanceRooms;
                            ConnectionFound = true;
                            recordTileA = tileA;
                            recordTileB = tileB;
                            recordRoomA = roomA;
                            recordRoomB = roomB;
                        }
                    }
                }
            }
            if (ConnectionFound && !forceAccessToMainRoom)
            {
                CreatePassage(recordRoomA, recordRoomB, recordTileA, recordTileB);
            }
        }
        if (ConnectionFound && forceAccessToMainRoom)
        {
            CreatePassage(recordRoomA, recordRoomB, recordTileA, recordTileB);
            ConnectClosestRooms(allRooms, true);
        }
        if (!forceAccessToMainRoom)
        {
            ConnectClosestRooms(allRooms, true);
        }
    }

    private void CreatePassage(Room roomA, Room roomB, Coord tileA, Coord tileB)
    {
        Room.ConnectRooms(roomA, roomB);

        List<Coord> line = GetLine(tileA, tileB);
        foreach (var coord in line)
        {
            DrawCircle(coord, 4);
        }

    }

    public void DrawCircle(Coord c ,int r)
    {
        for (int x = -r; x <= r; x++)
        {
            for (int y = -r; y <= r; y++)
            {
                //if coord is insinde or the radius(circle)
                if(x*x + y*y <= r * r)
                {
                    int realX = c.tileX + x;
                    int realY = c.tileY + y;

                    if (CheckIfInBorders(realX, realY))
                    {
                        map[realX, realY] = 0;
                    }
                }
            }
        }
    }

    public List<Coord> GetLine(Coord a,Coord b)
    {
        List<Coord> line = new List<Coord>();

        int x = a.tileX;
        int y = a.tileY;

        int dx = b.tileX - a.tileX;
        int dy = b.tileY - a.tileY;

        bool Inverted = false;
        int step = Math.Sign(dx);
        int gradientStep = Math.Sign(dy);

        int longest = Mathf.Abs(dx);
        int shortest = Mathf.Abs(dy);

        if(longest < shortest)
        {
            Inverted = true;
            shortest = Mathf.Abs(dx);
            longest = Mathf.Abs(dy);

            step = Math.Sign(dy);
            gradientStep = Math.Sign(dx);
        }

        int gradientAccumulation = longest / 2;
        for (int i = 0; i < longest; i++)
        {
            line.Add(new Coord(x, y));

            if (Inverted)
            {
                y += step;
            }else
            {
                x += step;
            }
            gradientAccumulation += shortest;
            if (gradientAccumulation >= longest)
            {
                if (Inverted)
                    x += gradientStep;
                else
                    y += gradientStep;

                gradientAccumulation -= longest;
            }
        }

        return line;
    }

    public List<List<Coord>> GetRegions(int tileType)
    {
        List<List<Coord>> regions = new List<List<Coord>>();
        int[,] mapFlags = new int[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (mapFlags[x, y] == 0 && map[x, y] == tileType)
                {
                    List<Coord> newRegion = GetRegionTiles(x, y);
                    regions.Add(newRegion);
                    //set flaged tiles to look at (1)
                    foreach (var tile in newRegion)
                    {
                        mapFlags[tile.tileX, tile.tileY] = 1;
                    }
                }
            }
        }

        return regions;
    }

    public List<Coord> GetRegionTiles(int startX, int startY)
    {
        List<Coord> tiles = new List<Coord>();
        int[,] mapFlags = new int[width, height];

        int tileType = map[startX, startY];

        Queue<Coord> queue = new Queue<Coord>();
        Coord startTile = new Coord(startX, startY);

        queue.Enqueue(startTile);
        mapFlags[startX, startY] = 1;

        while (queue.Count > 0)
        {
            Coord tile = queue.Dequeue();
            tiles.Add(tile);

            //looking at neighbouring 8 tiles
            for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++)
            {
                for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
                {
                    if (CheckIfInBorders(x, y) && (x == tile.tileX || y == tile.tileY))
                    {
                        if (mapFlags[x, y] == 0 && map[x, y] == tileType)
                        {
                            mapFlags[x, y] = 1;
                            queue.Enqueue(new Coord(x, y));
                        }
                    }
                }
            }
        }

        return tiles;
    }

    #endregion

    #region Cellular Automata

    public void GenerateMap()
    {
        map = new int[width, height];

        RandomFillMap();
        SmoothMap(5);
        ProcessRegionsInMap();
        BuildBorder();

        //builds map interieur in steps 
        StartCoroutine(EventCaller(0.5f));
    }

    private void RandomFillMap()
    {
        if (UseRandomSeed)
            seed = Time.time.ToString();

        System.Random randomHash = new System.Random(seed.GetHashCode());

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //makes all borders are walls 
                if (CheckIfAtBorder(x, y))
                {
                    map[x, y] = 1;

                }

                map[x, y] = (randomHash.Next(0, 100) < randomFillPercent) ? 1 : 0;
            }
        }
    }

    private void SmoothMap(int times)
    {
        for (int i = 0; i < times; i++)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int neighbourWallTiles = GetSeroundingWallCount(x, y);
                    //makes all tiles at border walls 
                    if (CheckIfAtBorder(x, y))
                    {
                        map[x, y] = 2;
                        continue;
                    }
                    if (neighbourWallTiles > 4)
                        map[x, y] = 1;
                    else if (neighbourWallTiles < 4)
                        map[x, y] = 0;

                }
            }
        }
    }

    private int GetSeroundingWallCount(int x, int y)
    {
        //count all wall tiles from the 8 neighbour tiles at position x,y 
        int wallCount = 0;
        for (int neightbourX = x - 1; neightbourX <= x + 1; neightbourX++)
        {
            for (int neightbourY = y - 1; neightbourY <= y + 1; neightbourY++)
            {
                if (CheckIfInBorders(neightbourX, neightbourY))
                {
                    if (neightbourX != x || neightbourY != y)
                    {
                        wallCount += map[neightbourX, neightbourY];
                    }
                }
                else
                {
                    wallCount++;
                }
            }
        }

        return wallCount;
    }

    public void BuildBorder()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                    if (x == 0 || y == 0 || x == width - 1 || y == height - 1)
                    {
                        map[x, y] = 1;
                    }
            }

        }
    }

    #endregion

    #region CheckFunctions

    public List<Vector3> GetSpawnbleTiles()
    {
        List<Vector3> result = new List<Vector3>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] != 0)
                    continue;

                result.Add(new Vector3(x, y, 0));
            }
        }
        return result;
    }


    public Vector3 CoordToWorldPoint(Coord tile)
    {
        return new Vector3(-width / 2 + tile.tileX + offset, 5, -height / 2 + tile.tileY + offset);
    }

    public bool CheckIfInBorders(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    public bool CheckIfAtBorder(int x, int y)
    {
        return x == 0 || x == width - 1 || y == 0 || y == height - 1;
    }

    public bool CheckTileIndex(int index, int x, int y)
    {
        if (!CheckIfInBorders(x, y))
            return false;

        return (map[x, y] == index);
    }

    public bool CheckTileIndex(int index, Vector3 pos)
    {
        int x = (int)pos.x;
        int y = (int)pos.y;

        if (!CheckIfInBorders(x, y))
            return false;

        return (map[x, y] == index);
    }

    public void MoveInDungeon(Transform moveObject)
    {
        var spawnbleTiles = GetSpawnbleTiles();
        int random = UnityEngine.Random.Range(0, spawnbleTiles.Count);
        moveObject.position = spawnbleTiles[random];
    }

    #endregion

    private IEnumerator EventCaller(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        //buildes tile map 
        MapBuilder.current.BuildMap(map);
        //spawns end point in dungeon
        MapBuilder.current.SpawnExit();
        //builds Fog of war
        FogOfWar.current.BuildFog(map);
        //spawns Interacble objects
        SelectableManager.current.Generate();
        //spawns subscribed gameobjects in the dungeon
        GameEvents.current.DungeonCreated();
    }
}
