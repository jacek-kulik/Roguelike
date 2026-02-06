using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : PublicClasses
{
    public GameObject attachedRoom;
    public GameObject roomManager;

    bool playerTurn = true;

    public int x = 0, y = 0;
    int old_x = 0, old_y = 0;

    public int maxHealth = 5;
    int health;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        Location loc = attachedRoom.GetComponent<RoomScript>().GetStartingPlayerLocation();
        x = (int)loc.x;
        y = (int)loc.y;
        Location loc_world = attachedRoom.GetComponent<RoomScript>().GetTileLocation(x, y);
        transform.position = new Vector2(loc_world.x, loc_world.y);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTurn)
        {
            if(Input.GetKeyDown("d")){
                if (attachedRoom.GetComponent<RoomScript>().MoveRight(x, y))
                {
                    old_x = x; old_y = y;
                    x++;
                    Move(1);
                }
            }
            else if(Input.GetKeyDown("a")){
                if (attachedRoom.GetComponent<RoomScript>().MoveLeft(x, y))
                {
                    old_x = x; old_y = y;
                    x--;
                    Move(3);
                }
            }
            else if(Input.GetKeyDown("w")){
                if (attachedRoom.GetComponent<RoomScript>().MoveUp(x, y))
                {
                    old_x = x; old_y = y;
                    y--;
                    Move(0);
                }
            }
            else if(Input.GetKeyDown("s")){
                if (attachedRoom.GetComponent<RoomScript>().MoveDown(x, y))
                {
                    old_x = x; old_y = y;
                    y++;
                    Move(2);
                }
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                Attack(0);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                Attack(1);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                Attack(2);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                Attack(3);
            }
            else if (Input.GetKeyDown("e") || Input.GetKeyDown(KeyCode.Return))
            {
                // Check if player is on a staircase
                if (attachedRoom.GetComponent<RoomScript>().IsStaircase(x, y))
                {
                    roomManager.GetComponent<RoomManager>().DescendToNextFloor();
                }
            }
        }
    }

    void Move(int direction)
    {
        playerTurn = false;
        if (attachedRoom.GetComponent<RoomScript>().CheckIfMoveToNewRoom(x, y))
        {
            roomManager.GetComponent<RoomManager>().AttemptEnter(direction);
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else {
            attachedRoom.GetComponent<RoomScript>().UpdateTile(x, y, false, true);
            attachedRoom.GetComponent<RoomScript>().ResetTile(old_x, old_y);
            Location loc = attachedRoom.GetComponent<RoomScript>().GetTileLocation(x, y);
            transform.position = new Vector2(loc.x, loc.y);
            attachedRoom.GetComponent<RoomScript>().Turn(this, x, y);

            Vector3 rot = new Vector3(0f, 0f, 0f);
            if (direction == 0) rot = new Vector3(0f, 0f, 90f);
            else if (direction == 1) rot = new Vector3(0f, 0f, 0f);
            else if (direction == 2) rot = new Vector3(0f, 0f, 270f);
            else if (direction == 3) rot = new Vector3(0f, 180f, 0f);
            transform.localEulerAngles = rot;
        }
    }

    private void Attack(int direction)
    {
        attachedRoom.GetComponent<RoomScript>().Turn(this, x, y);
    }

    public void Turn()
    {
        playerTurn = true;
    }

    public void TakeDamage(int damage)
    {
        Debug.Log("Took damage");
        health -= damage;
        if(health <= 0)
        {
            Death();
        }
    }
    void Death()
    {
        Debug.Log("Died");
    }
}
