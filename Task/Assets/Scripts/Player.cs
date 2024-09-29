using System.Collections;
using Unity.Collections;
using UnityEngine;
using static UnityEditor.FilePathAttribute;


public enum E_Direction
{
    UP,
    DOWN,
    LEFT,
    RIGHT,
}

public class Player : MonoBehaviour
{


    [SerializeField] private float fMoveDistance = 1.0f;
    [SerializeField] private float fMoveDuration = 2.0f;

    [SerializeField, ReadOnly] private bool bPlayerMoving = false;

    private Coroutine activeMoveCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //checks inputs, if game is not paused
        if(!GameManager.instance.bIsPaused)
        {
            CheckInputs();
        }
        

    }

    //checks inputs
    void CheckInputs()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            MovePlayer(E_Direction.LEFT);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            MovePlayer(E_Direction.RIGHT);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            MovePlayer(E_Direction.UP);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {   
            MovePlayer(E_Direction.DOWN);
        }
    }


    //Decides new destination, before initiating the movement coroutine.
    void MovePlayer(E_Direction _direction)
    {
        Vector2 Destination = new Vector2(transform.position.x, transform.position.y);

        //calculate destination according to desired direction enum
        switch (_direction)
        {
            case E_Direction.LEFT:
                Destination.x -= fMoveDistance;
                break;
            case E_Direction.RIGHT:
                Destination.x += fMoveDistance;
                break;
            case E_Direction.UP:
                Destination.y += fMoveDistance;
                break;
           case E_Direction.DOWN:
                Destination.y -= fMoveDistance;
                break;
           
            default:
                Debug.Log("enum not valid @Player->MovePlayer");
                break;
        }


        //Calling the movment coroutine to initiate movement
        if(!bPlayerMoving)
        {
            //if the player is not currently moving, start the coroutine to move the player
            activeMoveCoroutine = StartCoroutine(MoveCoroutine(Destination));
        }
        else
        {
            //if player is moving, stop the current movement coroutine, and start a new one
            StopCoroutine(activeMoveCoroutine);
            activeMoveCoroutine = StartCoroutine(MoveCoroutine(Destination));
        }

        //Play the audio clip
        AudioManager.instance.PlayMovementAudio(_direction);
    }


    //Smoothly move the player from one point to another over a set period of time
    IEnumerator MoveCoroutine(Vector2 _destination)
    {
        //timer to make sure the player moves over a duration
        float moveTimer = 0.0f;

        //set player moving bool
        bPlayerMoving = true;

        //initial position before move started (to keep a constant move speed, as "distance" will not change)
        Vector2 InitialPosition = transform.position;

        while(moveTimer < fMoveDuration)
        {
            //smoothly move the player to the location
            transform.position = Vector3.MoveTowards(transform.position, _destination, (Vector3.Distance(_destination, InitialPosition) * Time.deltaTime) / fMoveDuration);


            //add time to the counter
            moveTimer += Time.deltaTime;


            yield return null;
        }

        //reset player moving bool
        bPlayerMoving = false;
    }
}
