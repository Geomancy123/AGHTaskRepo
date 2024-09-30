using Microsoft.Win32.SafeHandles;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    //Team
    public E_Color m_OwningSide;

    //If the pawn has a flag
    public bool m_HasFlag = false;
    public bool m_Tagged = false;
    public bool m_Jailed = false;
    public bool m_SafeRunBack = false;
    public bool m_Chasing = false;
    public bool m_Wandering = false;
    public bool m_OnEnemyTerrain = false;

    public bool m_Attacker = false;

    //if pawn is controlled by the player
    public bool m_IsControlled = false;

    public NavMeshAgent m_Agent;


    [SerializeField] ParticleSystem m_ClickParticle;


    [SerializeField]
    private Flag m_Flag;

    public GameObject m_ControlledIcon;
    public CharacterController m_PlayerCOntroller;
    public Rigidbody m_RigidBody;


    float m_LookRotSpeed = 15.0f;
    float m_SearchRange = 10.0f;
    float m_WanderSpeed = 3.0f;
    float m_ActionSpeed = 6.0f;
    float m_FriendlyChaseSpeed = 5.0f;
    float m_ControlledSpeed = 8.0f;
    float m_AttackerSpeed = 8.5f;

    Vector3 m_RandomWanderTarget = new Vector3();


    public float m_AttackingAvoidance = 2.0f;
    float m_GeneralAvoidance = 0.48f;

    bool m_attackerDecisionMade = false;
    bool m_attackerGoingToRescue = false;

    private void Awake()
    {
        //makes sure the players on the red side are red, and the players on the blue side are blue
        CheckOnEnemyTerrain();
        if (m_OnEnemyTerrain == true)
        {
            m_OwningSide = E_Color.BLUE;
            m_OnEnemyTerrain = false;
        }
        else
        {
            m_OwningSide = E_Color.RED;
            m_OnEnemyTerrain = false;
        }


        //setting flag color
        if (m_OwningSide == E_Color.RED)
        {
            m_Flag.SetFlagColor(E_Color.BLUE);
            GetComponentInChildren<MeshRenderer>().material = GameManager.instance.m_RedMat;
        }
        else if (m_OwningSide == E_Color.BLUE)
        {
            m_Flag.SetFlagColor(E_Color.RED);
            GetComponentInChildren<MeshRenderer>().material = GameManager.instance.m_BlueMat;
        }

        //hiding flag
        m_Flag.ActivateFlag(false);
        m_ControlledIcon.SetActive(false);


        m_Agent = GetComponent<NavMeshAgent>();
    }
    // Start is called before the first frame update
    void Start()
    {
        CalculateRandomWanderTarget();
    }

    // Update is called once per frame
    void Update()
    {
        //updates the onenemyterrain status
        CheckOnEnemyTerrain();

        //adjusting speed
        
        if (m_Attacker)
        {
            m_Agent.speed = m_AttackerSpeed;
        }
        else if (m_IsControlled)
        {
            m_Agent.speed = m_ControlledSpeed;
        }
        else if (m_Chasing && m_OwningSide == E_Color.BLUE)
        {
            m_Agent.speed = m_FriendlyChaseSpeed;
        }
        else if (!m_Wandering)
        {
            m_Agent.speed = m_ActionSpeed;
        }
        else
        {
            m_Agent.speed = m_WanderSpeed;
        }

        
        if (m_Attacker && !m_Jailed)
        {
            
            m_Agent.avoidancePriority = 49;
            m_Agent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;

            float sped = m_Agent.desiredVelocity.magnitude;
            if (sped > m_Agent.speed || m_Agent.velocity.magnitude < 0.05f)
            {
                float multiplier = m_Agent.speed / sped;
                Vector3 newvec = m_Agent.desiredVelocity * multiplier;
                Vector3 direction = m_Agent.velocity.normalized;
                // m_Agent.desiredVelocity = newvec;
                if(m_Agent.velocity.magnitude > 0.01f)
                {
                    m_Agent.velocity = direction * m_Agent.speed;
                }
                
                m_Agent.radius -= 0.033f;
            }
            else
            {
              //  m_Agent.radius = m_AttackingAvoidance;
            }

        }
        else if(!m_Jailed)
        {
            m_Agent.radius = m_GeneralAvoidance;
            m_Agent.avoidancePriority = 48;
            m_Agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        }

        //Defender logic
        if (!m_Attacker && !m_IsControlled && !m_Tagged && !m_SafeRunBack && ! m_Jailed && ! m_HasFlag)
        {
            //try to chase, if not, wander
            if (!ChaseInvaders())
            {
                Wander();
            }
        }

        //Attacker logic
        if(m_Attacker)
        {
            AttackerBrain();
        }


        //make the model look the way its going
        Vector3 LookDir = (m_Agent.destination - transform.position).normalized;
        Quaternion LookRot = Quaternion.LookRotation(new Vector3(LookDir.x, 0.0f, LookDir.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, LookRot, Time.deltaTime * m_LookRotSpeed);
    }


    private void OnCollisionEnter(Collision collision)
    {
        //making sur eits a player
        if (collision.gameObject.GetComponentInParent<Player>() != null)
        {
            Player collidedPlayer = collision.gameObject.GetComponentInParent<Player>();

            //if side does not match, and player does not have flag, then give player a flag
            if (collidedPlayer.m_OwningSide != m_OwningSide && collidedPlayer.m_OnEnemyTerrain && !collidedPlayer.m_SafeRunBack)
            {
                collidedPlayer.RecieveTag();
            }
            else if (collidedPlayer.m_OwningSide == m_OwningSide && collidedPlayer.m_Jailed && collidedPlayer.m_Tagged && !m_Tagged && !collidedPlayer.m_SafeRunBack && !m_SafeRunBack)
            {
                //Recuing player formt the jail!

                if(m_OwningSide == E_Color.RED)
                {
                    GameManager.instance.m_BlueJail.RemoveCapturedPlayer();
                }
                else
                {
                    GameManager.instance.m_RedJail.RemoveCapturedPlayer();
                }


                collidedPlayer.m_Jailed = false;
                collidedPlayer.m_SafeRunBack = true;
                collidedPlayer.MoveOutOfJail();

                //moving self back from jail
                m_SafeRunBack = true;
                m_Jailed = false;
                m_Attacker = false;
                MoveOutOfJail();
            }
        }
    }

    void AttackerBrain()
    {
        
        if (m_HasFlag)
        {
            MoveToFriendlyFlagZone();
            return;
        }


        if(m_attackerDecisionMade)
        {
            if(m_attackerGoingToRescue)
            {
                MoveToRescueJailedAlly();
            }
            else
            {
                MoveToEnemyFlagZone();
            }
            return;
            
        }

        //chance based decision making - more of a chance to rescue the more prioners there are
        float rand = Random.Range(0, GameManager.instance.m_BlueJail.GetCaptureCount() + 1);
        if(rand > 1)
        {
            if (!MoveToRescueJailedAlly())
            {
                MoveToEnemyFlagZone();
                m_attackerGoingToRescue = false;
                m_attackerDecisionMade = true;
            }
            else
            {
                m_attackerGoingToRescue = true;
                m_attackerDecisionMade = true;
            }
        }
        else
        {
            MoveToEnemyFlagZone();
            m_attackerGoingToRescue = false;
            m_attackerDecisionMade = true;
        }
        
    }

    //moves the player to the desired location via navmesh
    public void ControlledMove(Vector3 _moveDestination)
    {

        m_Agent.destination = new Vector3(_moveDestination.x, 0.1f, _moveDestination.z);

        if (m_ClickParticle != null)
        {
            var parts = Instantiate(m_ClickParticle, new Vector3(_moveDestination.x, 0.1f, _moveDestination.z), m_ClickParticle.transform.rotation);
            Destroy(parts.gameObject, 2.0f);
        }
    }

    public void RelinquishControl()
    {
        if(m_OnEnemyTerrain)
        {
            float dist = 9999.0f;

            m_Attacker = true;
            m_Agent.velocity.Set(0.0f,0.0f,0.0f);
            
                if(m_HasFlag)
                {
                    MoveToFriendlyFlagZone();
                    return;
                }
                dist  = Vector3.Distance(transform.position, GameManager.instance.m_RedFlagZone.transform.position);
                float newDist  = Vector3.Distance(transform.position, GameManager.instance.m_RedJail.transform.position);
                if(newDist < dist)
                {
                    if(!MoveToRescueJailedAlly())
                    {
                        MoveToEnemyFlagZone();
                        m_attackerGoingToRescue = false;
                        m_attackerDecisionMade = true;
                    }
                    else
                    {
                        m_attackerGoingToRescue = true;
                        m_attackerDecisionMade = true;
                    }
                }
                else
                {
                    MoveToEnemyFlagZone();
                    m_attackerGoingToRescue = false;
                    m_attackerDecisionMade = true;
                }

        }
        else
        {
            //try to chase, if not, wander
            if(!ChaseInvaders())
            {
                Wander();
            }

        }
    }

    void CalculateRandomWanderTarget()
    {
        //moving to jail
        if (m_OwningSide == E_Color.RED)
        {
            float RandX = Random.Range(-18.0f, 18.0f);
            float RandZ = Random.Range(0.0f, 18.0f);
            m_RandomWanderTarget = new Vector3(RandX, 0.1f, RandZ);
        }
        else
        {
            float RandX = Random.Range(-18.0f,18.0f);
            float RandZ = Random.Range(-18.0f, 0.0f);
            m_RandomWanderTarget = new Vector3(RandX, 0.1f, RandZ);
        }
        
    }

    public void Wander()
    {
       
        m_Wandering = true;

        
        if(Vector3.Distance(transform.position, m_RandomWanderTarget) > 1.0f)
        {
            m_Agent.destination = m_RandomWanderTarget;
        }
        else
        {
            CalculateRandomWanderTarget();

        }
    }

    //GET TO THE JAIL
    public void MoveToJail()
    {
        m_Attacker = false;
        m_Agent.radius = m_GeneralAvoidance;
        m_Agent.avoidancePriority = 48;
        m_Agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        m_Agent.velocity.Set(0.0f, 0.0f, 0.0f);
        //moving to jail
        if (m_OwningSide == E_Color.RED)
        {
            m_Agent.destination = new Vector3(GameManager.instance.m_BlueJail.transform.position.x, 0.1f, GameManager.instance.m_BlueJail.transform.position.z);
        }
        else
        {
            m_Agent.destination = new Vector3(GameManager.instance.m_RedJail.transform.position.x, 0.1f, GameManager.instance.m_RedJail.transform.position.z);
        }
    }

    //moves to tag a friendly player in jail, returns false if no oone is in jail
    public bool MoveToRescueJailedAlly()
    {

        Vector3 targetLocation = Vector3.zero;

        if(m_OwningSide == E_Color.RED)
        {
            foreach (Player playIter in GameManager.instance.m_RedTeamPlayers)
            {
                if(playIter.m_Jailed && playIter.m_Tagged == true)
                {
                    targetLocation = playIter.transform.position;
                    break;
                }
            }
        }
        else
        {
            foreach (Player playIter in GameManager.instance.m_BlueTeamPlayers)
            {
                if (playIter.m_Jailed && playIter.m_Tagged == true)
                {
                    targetLocation = playIter.transform.position;
                    break;
                }
            }
        }


        //move to the player
        if (targetLocation != Vector3.zero)
        {
            m_Agent.destination = targetLocation;
            return true;
        }

        //no one in jail
        return false;
    }

    //move to a designated spot on the friendly side of the map
    public void MoveOutOfJail()
    {
        m_Tagged = false;
        m_Jailed = false;
        MakeControlled(false);
        m_Agent.velocity.Set(0.0f, 0.0f, 0.0f);
        //moving out of jail
        if (m_OwningSide == E_Color.RED)
        {
            m_Agent.destination = new Vector3(GameManager.instance.m_BlueJail.m_ReturnLocation.transform.position.x, 0.1f, GameManager.instance.m_BlueJail.m_ReturnLocation.transform.position.z);
        }
        else
        {
            m_Agent.destination = new Vector3(GameManager.instance.m_RedJail.m_ReturnLocation.transform.position.x, 0.1f, GameManager.instance.m_RedJail.m_ReturnLocation.transform.position.z);
        }
    }

    public void FinishSafeRun()
    {
        m_Attacker = false;
        m_attackerDecisionMade = false;
        m_attackerGoingToRescue = false;
        m_SafeRunBack = false;
        Wander();
    }

    //Move to the flag spot
    public void MoveToEnemyFlagZone()
    {
        //moving to flag zone
        if (m_OwningSide == E_Color.RED)
        {
            m_Agent.destination = new Vector3(GameManager.instance.m_BlueFlagZone.transform.position.x, 0.1f, GameManager.instance.m_BlueFlagZone.transform.position.z);
        }
        else
        {
            m_Agent.destination = new Vector3(GameManager.instance.m_RedFlagZone.transform.position.x, 0.1f, GameManager.instance.m_RedFlagZone.transform.position.z);
        }
    }

    //Move to the flag spot
    public void MoveToFriendlyFlagZone()
    {

        MakeControlled(false);

        //moving to flag zone
        if (m_OwningSide == E_Color.RED)
        {
            m_Agent.destination = new Vector3(GameManager.instance.m_RedFlagZone.transform.position.x, 0.1f, GameManager.instance.m_RedFlagZone.transform.position.z);
        }
        else
        {
            m_Agent.destination = new Vector3(GameManager.instance.m_BlueFlagZone.transform.position.x, 0.1f, GameManager.instance.m_BlueFlagZone.transform.position.z);
        }
    }

    public bool ChaseInvaders()
    {
        //closest invader holder
        Vector3 nearestInvaderPos = new Vector3(9999.0f, 9999.0f, 9999.0f) ;
        float distanceToNearestInvader = 9999.0f;

        if (m_OwningSide == E_Color.RED)
        {
            foreach (Player playIter in GameManager.instance.m_BlueTeamPlayers)
            {
                if (playIter.m_OnEnemyTerrain == true && !playIter.m_SafeRunBack && !playIter.m_Tagged && !playIter.m_Jailed)
                {
                    Vector3 InvaderLoc = playIter.transform.position;
                    float dist = Vector3.Distance(transform.position, InvaderLoc);

                    if(dist < distanceToNearestInvader)
                    {
                        distanceToNearestInvader = dist;
                        nearestInvaderPos = InvaderLoc;
                    }
                }
            }
        }
        else
        {
            foreach (Player playIter in GameManager.instance.m_RedTeamPlayers)
            {
                if (playIter.m_OnEnemyTerrain == true && !playIter.m_SafeRunBack && !playIter.m_Tagged && !playIter.m_Jailed)
                {
                    Vector3 InvaderLoc = playIter.transform.position;
                    float dist = Vector3.Distance(transform.position, InvaderLoc);

                    if (dist < distanceToNearestInvader)
                    {
                        distanceToNearestInvader = dist;
                        nearestInvaderPos = InvaderLoc;
                    }
                }
            }
        }

        //if close invader has been found
        if(distanceToNearestInvader != 9999.0f && distanceToNearestInvader < m_SearchRange)
        {
            m_Agent.destination = nearestInvaderPos;

            m_Chasing = true;
            m_Wandering = false;
            return true;
        }

        m_Chasing = false;
        //if no invaders...
        return false;
    }


    //Sets the controlled state on or off, and adjusts the icon accordingly.
    public void MakeControlled(bool _controlled)
    {
        if(m_IsControlled == false && _controlled == false)
        {
            return;
        }

        if(_controlled)
        {
            if (m_OwningSide == E_Color.RED)
            {
                foreach (Player playiter in GameManager.instance.m_RedTeamPlayers)
                {
                    if (playiter.m_Attacker)
                    {
                        playiter.m_Attacker = false;
                        playiter.m_attackerDecisionMade = false;
                        playiter.m_attackerGoingToRescue = false;
                    }
                }
            }
            else
            {
                foreach (Player playiter in GameManager.instance.m_BlueTeamPlayers)
                {
                    if (playiter.m_Attacker)
                    {
                        playiter.m_Attacker = false;
                        playiter.m_attackerDecisionMade = false;
                        playiter.m_attackerGoingToRescue = false;
                    }
                }
            }
        }
        
        

        m_IsControlled = _controlled;

        m_ControlledIcon.SetActive(m_IsControlled);

        if (_controlled == false)
        {
            GameManager.instance.m_ControlledPlayer = null;
        }
        else
        {
            m_Agent.destination = transform.position;
            m_Wandering = false;
            m_Chasing = false;
        }
    }

    //sets the status and visibility of the flag, and hasflag
    public void SetOwnedFlagStatus(bool _status)
    {
        if(m_Attacker)
        {
            m_Agent.radius = m_GeneralAvoidance;
            m_Agent.velocity.Set(0.0f, 0.0f, 0.0f);
        }

        m_Flag.ActivateFlag(_status);
        m_HasFlag = _status;
    }


    //Puck up flag from flagzone
    public void AquireFlag()
    {
        SetOwnedFlagStatus(true);
    }

    //Give flag to friendly flagzone
    public void GiveFlagToZone()
    {
        SetOwnedFlagStatus(false);

        //swaps attacker if not controlled
        if(m_IsControlled == false)
        {
           
        }
        if(m_Attacker)
        {
            m_Attacker = false;
            m_attackerGoingToRescue = false;
            m_attackerDecisionMade = false;
        }
        
        

    }


    //Get tagged by opposing player
    public void RecieveTag()
    {
        //unassigns all the states, sets tagged
        
        m_Tagged = true;
        if(m_Attacker)
        {
            m_Attacker = false;
            m_attackerGoingToRescue = false;
            m_attackerDecisionMade = false;
        }
       
        m_Wandering = false;
        MakeControlled(false);

        if(m_HasFlag)
        {
            //giving the flag back
            if (m_OwningSide == E_Color.RED)
            {
                GameManager.instance.m_BlueFlagZone.RecoverOwnedFlag();
            }
            else
            {
                GameManager.instance.m_RedFlagZone.RecoverOwnedFlag();
            }
        }

        SetOwnedFlagStatus(false);
        //get to the jail
        MoveToJail();
    }

    

    void CheckOnEnemyTerrain()
    {
        //Returns TRUE if player is on the RED side of the map
        Vector2 PlayerVec = new Vector2 (transform.position.x, transform.position.z);
        Vector2 CenterVec = new Vector2 (25.0f, 0.0f);

        float Result = -PlayerVec.x * CenterVec.y + PlayerVec.y * CenterVec.x;

        
        if (Result > 0.0f)
        {
            //IS ON RED SIDE
            
            if(m_OwningSide == E_Color.RED)
            {
                m_OnEnemyTerrain = false;
            }
            else
            {
                m_OnEnemyTerrain = true;
            }
        }
        else if(Result < 0.0f)
        {
            //IS ON BLUE SIDE
            if (m_OwningSide == E_Color.BLUE)
            {
                m_OnEnemyTerrain = false;
            }
            else
            {
                m_OnEnemyTerrain = true;
            }
        }


    }
}
