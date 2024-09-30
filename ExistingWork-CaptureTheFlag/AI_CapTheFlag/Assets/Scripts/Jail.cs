using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Jail : MonoBehaviour
{


    private int m_CapturedPlayers = 0;


    public E_Color m_OwningSide;

    public GameObject m_ReturnLocation;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        //making sur eits a player
        if (collision.gameObject.GetComponentInParent<Player>() != null)
        {
            Player collidedPlayer = collision.gameObject.GetComponentInParent<Player>();

            //if side does not match, and player does not have flag, then give player a flag
            if (collidedPlayer.m_OnEnemyTerrain && collidedPlayer.m_Tagged)
            {
                AddCapturedPlayer();
                collidedPlayer.m_Jailed = true;
                

            }
            else if(collidedPlayer.m_Attacker)
            {
                collidedPlayer.m_Jailed = true;
                collidedPlayer.m_Agent.radius = 0.5f;
                collidedPlayer.m_Agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
            }

        }
    }



    public void AddCapturedPlayer()
    {
        m_CapturedPlayers++;

        CheckWin();
    }

    public int GetCaptureCount()
    {
        return m_CapturedPlayers;
    }

    public void RemoveCapturedPlayer()
    {
        m_CapturedPlayers--;
    }



    public Vector3 FindJailSpot()
    {
        Vector3 ReturnSpot = Vector3.zero;

        float XLoc = Random.Range(0, m_CapturedPlayers);
        float YLoc = Random.Range(0, m_CapturedPlayers);



        return ReturnSpot;
    }

    void CheckWin()
    {
        //If there are maximum stolen flags, WIN THE GAME!
        if (m_CapturedPlayers == GameManager.instance.m_TeamSize)
        {
            //Trigger win from game manager, passing owning team color.
            GameManager.instance.TriggerWin(m_OwningSide);
        }
    }
}
