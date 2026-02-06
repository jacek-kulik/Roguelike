using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomScript : PublicClasses
{
    public int ROOMSIZEX = 7;
    public int ROOMSIZEY = 7;

    public Sprite emptyCellSprite;
    public Sprite staircaseSprite;

    public GameObject TilePrefab;
    public GameObject dangerPrefab;

    public GameObject[] EnemyPrefabs;

    public bool isStartingRoom = false;
    public bool isActive = false;
    public bool openExits = false;

    private Connections con;

    public class Tile
    {
        public Tile(Sprite spriteToSet)
        {
            sprite = spriteToSet;
        }

        public bool empty { get; set; } = true;
        public bool player { get; set; } = false;
        public bool blockade { get; set; } = false;
        public bool inDanger { get; set; } = false;
        public bool isStaircase { get; set; } = false;
        public int dangerTimer { get; set; } = 0;
        public GameObject attachedObject { get; set; } = null;
        public Sprite sprite { get; set; }
    }
    public List<Tile> TileGrid = new List<Tile>();
    List<Sprite> DisplayGrid = new List<Sprite>();
    List<GameObject> displayTiles = new List<GameObject>();
    List<GameObject> enemies = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        if (isStartingRoom)
        {
            for (int i = 0; i < ROOMSIZEX * ROOMSIZEY; i++)
            {
                Tile tile = new Tile(emptyCellSprite);
                TileGrid.Add(tile);
                DisplayGrid.Add(emptyCellSprite);
                con = new Connections();
            }
            RefreshDisplay();
        }
    }

    public void GenerateRoom(Connections connections)
    {
        //TODO Basic room generation and then walking between basic rooms should work. Should be fun
        con = connections;

        for (int i = 0; i < ROOMSIZEX * ROOMSIZEY; i++)
        {
            Tile tile = new Tile(emptyCellSprite);
            TileGrid.Add(tile);
            DisplayGrid.Add(emptyCellSprite);
            con = new Connections();
        }
        for (int j = 0; j < 1; j++)
        {
            GameObject o = Instantiate(EnemyPrefabs[0], transform);
            o.GetComponent<EnemyTurnScript>().Create(ROOMSIZEX / 2, ROOMSIZEY / 2);
            Location loc = GetTileLocation(ROOMSIZEX / 2, ROOMSIZEY / 2);
            o.transform.position = new Vector2(loc.x, loc.y);
            enemies.Add(o);
        }
        RefreshDisplay();
    }

    public void GenerateStaircase()
    {
        // Place staircase in a random location (but not center where enemy spawns)
        int staircaseX, staircaseY;
        do
        {
            staircaseX = Random.Range(1, ROOMSIZEX - 1);
            staircaseY = Random.Range(1, ROOMSIZEY - 1);
        } while (staircaseX == ROOMSIZEX / 2 && staircaseY == ROOMSIZEY / 2);

        TileGrid[staircaseY * ROOMSIZEY + staircaseX].isStaircase = true;
        TileGrid[staircaseY * ROOMSIZEY + staircaseX].empty = false;
        if (staircaseSprite != null)
        {
            TileGrid[staircaseY * ROOMSIZEY + staircaseX].sprite = staircaseSprite;
        }
        RefreshDisplay();
    }

    void RefreshDisplay()
    {
        for(int i = 0; i < ROOMSIZEX * ROOMSIZEY; i++)
        {
            DisplayGrid[i] = TileGrid[i].sprite;
        }
        foreach (GameObject o in displayTiles)
        {
            Destroy(o);
        }
        displayTiles.Clear();
        for(int y = 0; y < ROOMSIZEY; y++)
        {
            for(int x = 0; x < ROOMSIZEX; x++)
            {
                GameObject o = Instantiate(TilePrefab, transform);
                o.GetComponent<SpriteRenderer>().enabled = true;
                o.GetComponent<SpriteRenderer>().sprite = TileGrid[y * ROOMSIZEY + x].sprite;
                Location loc = GetTileLocation(x, y);
                o.transform.localPosition = new Vector3(loc.x, loc.y, 1f);
                displayTiles.Add(o);

                if(TileGrid[y * ROOMSIZEY + x].dangerTimer > 0)
                {
                    GameObject dan = Instantiate(dangerPrefab, transform);
                    dan.GetComponent<SpriteRenderer>().enabled = true;
                    dan.transform.localPosition = new Vector2(loc.x, loc.y);
                    displayTiles.Add(dan);
                }
            }
        }
    }

    public void Turn(PlayerScript player, int xPlayer, int yPlayer)
    {
        for(int y = 0; y < ROOMSIZEY; y++)
        {
            for (int x = 0; x < ROOMSIZEX; x++)
            {
                if (TileGrid[y * ROOMSIZEY + x].player && TileGrid[y * ROOMSIZEY + x].dangerTimer == 1)
                {
                    player.TakeDamage(1);
                }
                if(TileGrid[y * ROOMSIZEY + x].dangerTimer != 0)
                {
                    TileGrid[y * ROOMSIZEY + x].dangerTimer--;
                }
            }
        }
        RefreshDisplay();
        //TODO something about this, get the enemies x and y position, probably through a new class
        foreach (GameObject o in enemies) o.GetComponent<EnemyTurnScript>().Turn(xPlayer, yPlayer, this);
        player.Turn();
    }

    public void UpdateTile(int x, int y, bool empty = true, bool player = false, bool blockade = false, bool inDanger = false, GameObject attachedObject = null)
    {
        // Don't override staircase tiles
        if (!TileGrid[y * ROOMSIZEY + x].isStaircase)
        {
            TileGrid[y * ROOMSIZEY + x].empty = empty;
        }
        TileGrid[y * ROOMSIZEY + x].player = player;
        TileGrid[y * ROOMSIZEY + x].blockade = blockade;
        TileGrid[y * ROOMSIZEY + x].inDanger = inDanger;
        TileGrid[y * ROOMSIZEY + x].attachedObject = attachedObject;
    }
    public void SetTileDanger(int x, int y, int dangerTimer)
    {
        if(TileGrid[y * ROOMSIZEY + x].dangerTimer == 0)
        {
            TileGrid[y * ROOMSIZEY + x].dangerTimer = dangerTimer;
        }
        else if(TileGrid[y * ROOMSIZEY + x].dangerTimer > dangerTimer)
        {
            TileGrid[y * ROOMSIZEY + x].dangerTimer = dangerTimer;
        }
    }
    public void AttachGameObject(int x, int y, GameObject obj)
    {
        TileGrid[y * ROOMSIZEY + x].attachedObject = obj;
    }

    public void ResetTile(int x, int y)
    {
        TileGrid[y * ROOMSIZEY + x].empty = true;
        TileGrid[y * ROOMSIZEY + x].player = false;
        TileGrid[y * ROOMSIZEY + x].blockade = false;
        TileGrid[y * ROOMSIZEY + x].inDanger = false;
        TileGrid[y * ROOMSIZEY + x].attachedObject = null;
    }

    public Location GetTileLocation(int x , int y)
    {
        return new Location(ROOMSIZEX / -2 + (ROOMSIZEX % 2 == 0 ? 0.5f : 0f) + x, ROOMSIZEY / 2 - (ROOMSIZEY % 2 == 0 ? 0.5f : 0f) - y);
    }
    public Vector2 GetTileLocationAsVector(int x, int y)
    {
        return new Vector2(ROOMSIZEX / -2 + (ROOMSIZEX % 2 == 0 ? 0.5f : 0f) + x, ROOMSIZEY / 2 - (ROOMSIZEY % 2 == 0 ? 0.5f : 0f) - y);
    }

    public Location GetStartingPlayerLocation()
    {
        return new Location(ROOMSIZEX / 2, ROOMSIZEY / 2);
    }

    public bool MoveLeft(int x, int y)
    {
        if (x == 0) { 
            if(y == ROOMSIZEY / 2)
            {
                if (openExits && con.isConnectedLeft) return true;
                else return false;
            }
            else return false;
        }
        if (!TileGrid[y * ROOMSIZEY + x - 1].empty) return false;
        return true;
    }
    public bool MoveRight(int x, int y)
    {
        if (x == ROOMSIZEX - 1)
        {
            if (y == ROOMSIZEY / 2)
            {
                if (openExits && con.isConnectedRight) return true;
                else return false;
            }
            else return false;
        }
        if (!TileGrid[y * ROOMSIZEY + x + 1].empty) return false;
        return true;
    }
    public bool MoveUp(int x, int y)
    {
        if (y == 0)
        {
            if (x == ROOMSIZEX / 2)
            {
                if (openExits && con.isConnectedUp) return true;
                else return false;
            }
            else return false;
        }
        if (!TileGrid[(y - 1) * ROOMSIZEY + x].empty) return false;
        return true;
    }
    public bool MoveDown(int x, int y)
    {
        if (y == ROOMSIZEY - 1)
        {
            if (x == ROOMSIZEX / 2)
            {
                if (openExits && con.isConnectedDown) return true;
                else return false;
            }
            else return false;
        }
        if (!TileGrid[(y + 1) * ROOMSIZEY + x].empty) return false;
        return true;
    }
    public bool CheckIfMoveToNewRoom(int x, int y)
    {
        if (y == -1 || x == ROOMSIZEX || y == ROOMSIZEY || x == -1)
        {
            return true;
        }
        return false;
    }

    public bool IsStaircase(int x, int y)
    {
        return TileGrid[y * ROOMSIZEY + x].isStaircase;
    }
}
