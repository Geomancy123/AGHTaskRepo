using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagZone : MonoBehaviour
{

    static int FLAGS_PER_SIDE = 4;

    public E_Color m_OwningSide;

    public List<Flag> m_OwnedFlags;
    public List<Flag> m_StolenFlags;

    public int m_OwnedFlagCount = 4;
    public int m_StolenFlagCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        m_OwnedFlagCount = 4;
        m_StolenFlagCount = 0;

        foreach(Flag flag in m_StolenFlags)
        {
            flag.ActivateFlag(false);
        }
    }


    //dealing with player collisions
    private void OnCollisionEnter(Collision collision)
    {
        //making sur eits a player
        if (collision.gameObject.GetComponentInParent<Player>() != null)
        {
            Player collidedPlayer = collision.gameObject.GetComponentInParent<Player>();

            //if side does not match, and player does not have flag, then give player a flag
            if (collidedPlayer.m_OwningSide != m_OwningSide && !collidedPlayer.m_HasFlag && !collidedPlayer.m_Tagged)
            {
                RelinquishOwnedFlag();
                collidedPlayer.AquireFlag();
            }
            else if (collidedPlayer.m_OwningSide == m_OwningSide && collidedPlayer.m_HasFlag)
            {
                //if colours match, and player has flag, then recieve the stolen flag and remove it from character
                RecieveStolenFlag();
                collidedPlayer.GiveFlagToZone();
            }
        }
    }

    //Give away an owned flag, reducing the count and rendering a flag invisible
    public void RelinquishOwnedFlag()
    {
        if (m_OwnedFlagCount <= 0) { return; }

        //finding a disabled flag
        foreach (Flag flagIter in m_OwnedFlags)
        {
            if (flagIter.m_FlagActive)
            {
                //Activates the first inactive flag in the list, making it visible.
                flagIter.ActivateFlag(false);
                m_OwnedFlagCount--;
                break;
            }
        }
    }

    //recieves a stolen flag, activating the first inactive flag in the list. Then check win con
    public void RecieveStolenFlag()
    {
        //finding a disabled flag
        foreach (Flag flagIter in m_StolenFlags)
        {
            if (flagIter.m_FlagActive == false)
            {
                //Activates the first inactive flag in the list, making it visible.
                flagIter.ActivateFlag(true);
                m_StolenFlagCount++;
                break;
            }
        }

        //CHECK WIN CON
        CheckWin();
    }

    //Regains an owned flag
    public void RecoverOwnedFlag()
    {
        if (m_OwnedFlagCount >= FLAGS_PER_SIDE) { return; }

        //finding a disabled flag
        foreach(Flag flagIter in m_OwnedFlags)
        {
            if(!flagIter.m_FlagActive)
            {
                //Activates the first inactive flag in the list, making it visible.
                flagIter.ActivateFlag(true);
                m_OwnedFlagCount++;
                break;
            }
        }
        
    }


    //Checks to see if the zone has 4 stolen flags
    void CheckWin()
    {
        //If there are maximum stolen flags, WIN THE GAME!
        if(m_StolenFlagCount == FLAGS_PER_SIDE)
        {
            //Trigger win from game manager, passing owning team color.
            GameManager.instance.TriggerWin(m_OwningSide);
        }
    }
}
