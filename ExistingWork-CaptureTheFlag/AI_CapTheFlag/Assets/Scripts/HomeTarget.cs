using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeTarget : MonoBehaviour
{
    public E_Color m_OwningSide;

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
            if (!collidedPlayer.m_OnEnemyTerrain && collidedPlayer.m_SafeRunBack)
            {
                collidedPlayer.FinishSafeRun();
            }
       
        }
    }

}
