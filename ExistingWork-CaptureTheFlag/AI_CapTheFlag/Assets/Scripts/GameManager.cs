using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum E_Color
{
    RED,
    BLUE,
}

public enum E_GAMESTATE
{
    DEFAULT,
    STARTMENU,
    INGAME,
    PAUSE,
    GAMEEND,
}


public class GameManager : MonoBehaviour
{
    //Singleton instance
    public static GameManager instance;

    //Game state
    public E_GAMESTATE m_GameState = E_GAMESTATE.INGAME;

    public Material m_RedMat;
    public Material m_BlueMat;

    //Game object references
    public FlagZone m_RedFlagZone;
    public FlagZone m_BlueFlagZone;
    public Jail m_RedJail;
    public Jail m_BlueJail;

    //player references
    public List<Player> m_RedTeamPlayers;
    public List<Player> m_BlueTeamPlayers;
    public int m_TeamSize = 7;
    public Player m_Player_PF;

    public Player m_ControlledPlayer = null;
    [SerializeField] LayerMask m_IgnoreThis;

    //UI
    public GameObject UI_MainMenu;
    public GameObject UI_EndGame;
    public GameObject UI_RedWin;
    public GameObject UI_BlueWin;




    private void Awake()
    {
        //Singleton
        if (instance == null )
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_GameState = E_GAMESTATE.INGAME;

        UI_MainMenu.SetActive(true);
        UI_EndGame.SetActive(false);
        UI_BlueWin.SetActive(false);
        UI_RedWin.SetActive(false);


        //assigning variables
        var jails = FindObjectsOfType<Jail>();
        foreach (Jail jailIter in jails)
        {
            if(jailIter.m_OwningSide == E_Color.RED)
            {
                m_RedJail = jailIter;
            }
            else if(jailIter.m_OwningSide == E_Color.BLUE)
            {
                m_BlueJail = jailIter;
            }
        }

        //assigning variables
        var flagzones = FindObjectsOfType<FlagZone>();
        foreach (FlagZone zoneIter in flagzones)
        {
            if (zoneIter.m_OwningSide == E_Color.RED)
            {
                m_RedFlagZone = zoneIter;
            }
            else if (zoneIter.m_OwningSide == E_Color.BLUE)
            {
                m_BlueFlagZone = zoneIter;
            }
        }

        

        //REMOVE THIS - simply skips the menu
       // StartGame();
    }

    // Update is called once per frame
    void Update()
    {
        if(m_GameState == E_GAMESTATE.INGAME)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastLMB();
            }
            else if (Input.GetMouseButtonDown(1))
            {
                RaycastRMB();
            }
        }


        AssignAttacker();
    }

    //assigns attacker for red team;
    void AssignAttacker()
    {
        foreach(Player playIter in m_RedTeamPlayers)
        {
            if(playIter.m_Attacker)
            {
                
                //if an attacker exists, return
                return;
            }
            if (playIter.m_OnEnemyTerrain && !playIter.m_Jailed )
            {
                //prevents multiple on the same side
                return;
            }
        }

        foreach (Player playIter in m_RedTeamPlayers)
        {
            if ( !playIter.m_OnEnemyTerrain && !playIter.m_Jailed && !playIter.m_Tagged && !playIter.m_SafeRunBack && !playIter.m_Chasing)
            {
                //if avalable, mak into an attacker
                playIter.m_Agent.radius = playIter.m_AttackingAvoidance;
                playIter.m_Attacker = true;
                return;
            }
        }
    }



    //selects and unselects the characters upon left click.
    void RaycastLMB()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~m_IgnoreThis))
        {

            Vector3 mousePointHit = hit.point;
            if (hit.transform.GetComponent<Player>()!= null)
            {
                Player hitPlayer = hit.transform.GetComponent<Player>();

                if (hitPlayer.m_Jailed || hitPlayer.m_Tagged || hitPlayer.m_SafeRunBack || hitPlayer.m_OwningSide == E_Color.RED)
                {
                    return;
                }



                if (m_ControlledPlayer != null)
                {
                    m_ControlledPlayer.RelinquishControl();
                    m_ControlledPlayer.MakeControlled(false);
                    
                   
                }
                
                m_ControlledPlayer = hit.transform.GetComponent<Player>();
                m_ControlledPlayer.MakeControlled(true); 
            }
            else
            {
                if (m_ControlledPlayer != null)
                {
                    m_ControlledPlayer.RelinquishControl();
                    m_ControlledPlayer.MakeControlled(false);
                   
                    
                }
                m_ControlledPlayer = null;
            }
        }
    }

    void RaycastRMB()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~m_IgnoreThis))
        {

            Vector3 mousePointHit = hit.point;
            if (m_ControlledPlayer != null)
            {
                m_ControlledPlayer.ControlledMove(new Vector3(mousePointHit.x, 0.1f, mousePointHit.z));
            }
        }
    }



    public void SpawnPlayers()
    {
        for(int i = 0; i < m_TeamSize; i++)
        {
           Player newplayer =  Instantiate<Player>(m_Player_PF, new Vector3( -12.0f + 4.0f * i , 0.1f, -10.0f), GameManager.instance.m_RedJail.transform.rotation);

            m_BlueTeamPlayers.Add(newplayer);

            Player newplayer1 = Instantiate<Player>(m_Player_PF, new Vector3(-12.0f + 4.0f * i, 0.1f, 10.0f), GameManager.instance.m_BlueJail.transform.rotation);

            m_RedTeamPlayers.Add(newplayer1);
        }

    }

    //Start the game, remove UI, change state
    public void StartGame()
    {
        m_GameState = E_GAMESTATE.INGAME;
        UI_MainMenu.SetActive(false);

        SpawnPlayers();
    }



    //leaves the game
    public void ExitGame()
    {
        Application.Quit();
    }


    //open the main menu from the end game menu, changing game state.
    public void LaunchMainMenu()
    {
        //change game state
        m_GameState = E_GAMESTATE.STARTMENU;

        //changing UI
        UI_MainMenu.SetActive(true);
        UI_EndGame.SetActive(false);
    }

    //trigger the win, showing UI
    public void TriggerWin(E_Color _winningColor)
    {
        //chage state
        m_GameState = E_GAMESTATE.GAMEEND;

        UI_EndGame.SetActive(true);

        //show the winner via UI
        if(_winningColor == E_Color.RED)
        {
            UI_RedWin.SetActive(true);
        }
        else
        {
            UI_BlueWin.SetActive(true);
        }
    }
}
