using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PublicClasses : MonoBehaviour
{
    public class Location
    {
        public Location(float xSet, float ySet)
        {
            x = xSet;
            y = ySet;
        }
        public float x;
        public float y;
    }

    public class WorldLocation
    {
        public WorldLocation(int xSet, int ySet)
        {
            x = xSet;
            y = ySet;
        }
        public WorldLocation(WorldLocation loc)
        {
            x = loc.x;
            y = loc.y;
        }
        public int x;
        public int y;

        public bool Adjacent(WorldLocation loc)
        {
            int x1 = x;
            int x2 = loc.x;
            int y1 = y;
            int y2 = loc.y;

            if (x1 == x2)
            {
                if (y1 + 1 == y2) return true;
                if (y1 - 1 == y2) return true;
            }
            if (y1 == y2)
            {
                if (x1 + 1 == x2) return true;
                if (x1 - 1 == x2) return true;
            }
            return false;
        }
        public bool AdjacentIncDiagonal(WorldLocation loc)
        {
            int x1 = x;
            int x2 = loc.x;
            int y1 = y;
            int y2 = loc.y;

            if (x1 == x2 && y1 == y2 + 1) return true;
            if (x1 == x2 - 1 && y1 == y2 + 1) return true;
            if (x1 == x2 - 1 && y1 == y2) return true;
            if (x1 == x2 - 1 && y1 == y2 - 1) return true;
            if (x1 == x2 && y1 == y2 - 1) return true;
            if (x1 == x2 + 1 && y1 == y2 - 1) return true;
            if (x1 == x2 + 1 && y1 == y2) return true;
            if (x1 == x2 + 1 && y1 == y2 + 1) return true;

            return false;
        }
    }

    public class Room
    {
        public Room(int xLoc, int yLoc, GameObject obj, Connections connections, int floorNum = 0)
        {
            x = xLoc;
            y = yLoc;
            room = obj;
            con = connections;
            floor = floorNum;
        }
        public int x { get; set; }
        public int y { get; set; }
        public int floor { get; set; }
        public GameObject room { get; set; }
        public Connections con { get; set; }
    }

    public class Connections
    {
        public Connections(bool up = true, bool right = true, bool down = true, bool left = true)
        {
            isConnectedUp = up;
            isConnectedRight = right;
            isConnectedDown = down;
            isConnectedLeft = left;
        }
        public Connections(int up, int right, int down, int left)
        {
            if(up == 0)
            {
                isConnectedUp = Random.Range(0, 9) >= 7;
            }
            else if(up == 1)
            {
                isConnectedUp = true;
            }
            else if(up == 2)
            {
                isConnectedUp = false;
            }
            if(right == 0)
            {
                isConnectedRight = Random.Range(0, 9) >= 7;
            }
            else if(right == 1)
            {
                isConnectedRight = true;
            }
            else if(right == 2)
            {
                isConnectedRight = false;
            }
            if(down == 0)
            {
                isConnectedDown = Random.Range(0, 9) >= 7;
            }
            else if(down == 1)
            {
                isConnectedDown = true;
            }
            else if(down == 2)
            {
                isConnectedDown = false;
            }
            if(left == 0)
            {
                isConnectedLeft = Random.Range(0, 9) >= 7;
            }
            else if(left == 1)
            {
                isConnectedLeft = true;
            }
            else if(left == 2)
            {
                isConnectedLeft = false;
            }

        }
        public string GetLocationString()
        {
            return "Up: " + isConnectedUp + ", Right: " + isConnectedRight + ", Down: " + isConnectedDown + ", Left: " + isConnectedLeft;
        }

        public bool isConnectedUp { get; set; } = true;
        public bool isConnectedRight { get; set; } = true;
        public bool isConnectedDown { get; set; } = true;
        public bool isConnectedLeft { get; set; } = true;
    }
}
