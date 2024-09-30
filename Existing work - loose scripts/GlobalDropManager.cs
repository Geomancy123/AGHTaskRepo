using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalDropManager : MonoBehaviour
{
    //singleton instance
    public static GlobalDropManager instance;


    [SerializeField] PickupBase HealthDropCommonPF;
    [SerializeField] PickupBase HealthDropUnCommonPF;
    [SerializeField] PickupBase HealthDropRarePF;

    [SerializeField] PickupBase XPDropCommonPF;
    [SerializeField] PickupBase XPDropUnCommonPF;
    [SerializeField] PickupBase XPDropRarePF;

    [SerializeField] PickupBase UpgradeDropPF;
    [SerializeField] PickupBase MagnetDropPF;

    [SerializeField] float fPickupYhover = 1.0f;

    public List<PickupBase> ActivePickups;
    public List<PickupBase> PickupsToDestroy;

    private float fDropTickTimer = 0.0f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        ActivePickups = new List<PickupBase>();
    }

    
    private void Update()
    {
        fDropTickTimer += Time.deltaTime;
        if(fDropTickTimer >= 1.0f)
        {
            fDropTickTimer = 0.0f;

            //flyweight babyyyy
            foreach (PickupBase pickup in ActivePickups)
            {
                if (pickup.bImmortal) { continue; }
                if (pickup.TickLifetime(1))
                {
                    PickupsToDestroy.Add(pickup);
                    //ActivePickups.Remove(pickup);
                   // Destroy(pickup.gameObject);
                }
            }
        }

        if(PickupsToDestroy.Count > 0)
        {
            foreach (PickupBase pickup in PickupsToDestroy)
            {
                if (ActivePickups.Contains(pickup))
                {
                    ActivePickups.Remove(pickup);
                    Destroy(pickup.gameObject);
                }
            }
            PickupsToDestroy.Clear();
        }

        
    }


    //rolls what drop to spawn upon enemy death
    public void RollEnemyDrop(Enemy _enemy)
    {
        float rando = Random.Range(0, 100);

        if (_enemy.enemyType == Enemy.EnemyType.BASIC)
        {
            if (rando > 99.5)   //0.5% magnet
            {
                SpawnMagnetDrop(_enemy.transform.position); 
            }
            else if (rando > 98.5)  //1% hp uncommon
            {
                SpawnHealthDrop(_enemy.transform.position, E_PickupRarity.UNCOMMON);
            }
            else if (rando > 95.5)  //3% hp common drop
            {
                SpawnHealthDrop(_enemy.transform.position, E_PickupRarity.COMMON);
            }
            else if (rando > 92)    //3.5% xp uncommon
            {
                SpawnXPDrop(_enemy.transform.position, E_PickupRarity.UNCOMMON);
            }
            else if(rando > 14) //77% xp common
            {
                SpawnXPDrop(_enemy.transform.position, E_PickupRarity.COMMON);
            }
        }
        else if(_enemy.enemyType == Enemy.EnemyType.ELITE)
        {
            if (rando > 98.5)   //1.5% magnet
            {
                SpawnMagnetDrop(_enemy.transform.position);
            }
            else if (rando > 98)  //0.5% hp rare
            {
                SpawnHealthDrop(_enemy.transform.position, E_PickupRarity.RARE);
            }
            else if (rando > 96)  //2% hp uncommon drop
            {
                SpawnHealthDrop(_enemy.transform.position, E_PickupRarity.UNCOMMON);
            }
            else if (rando > 94)    //2% xp common
            {
                SpawnHealthDrop(_enemy.transform.position, E_PickupRarity.COMMON);
            }
            else if (rando > 89) //5% xp rare
            {
                SpawnXPDrop(_enemy.transform.position, E_PickupRarity.RARE);
            }
            else if (rando > 59) //30% xp rare
            {
                SpawnXPDrop(_enemy.transform.position, E_PickupRarity.UNCOMMON);
            }
            else if (rando > 14) //45% xp common
            {
                SpawnXPDrop(_enemy.transform.position, E_PickupRarity.COMMON);
            }
            
        }
        else if (_enemy.enemyType == Enemy.EnemyType. SHIELDED)
        {
            if (rando > 97.5)   //2.5% magnet
            {
                SpawnMagnetDrop(_enemy.transform.position);
            }
            else if (rando > 94.5)  //3% hp rare
            {
                SpawnHealthDrop(_enemy.transform.position, E_PickupRarity.RARE);
            }
            else if (rando > 91.5)  //3% hp uncommon drop
            {
                SpawnHealthDrop(_enemy.transform.position, E_PickupRarity.UNCOMMON);
            }
            else if (rando > 90)    //1.5% xp common
            {
                SpawnHealthDrop(_enemy.transform.position, E_PickupRarity.COMMON);
            }
            else if (rando > 60) //30% xp rare
            {
                SpawnXPDrop(_enemy.transform.position, E_PickupRarity.RARE);
            }
            else if (rando > 40) //30% xp rare
            {
                SpawnXPDrop(_enemy.transform.position, E_PickupRarity.UNCOMMON);
            }
            else if (rando > 10) //40% xp common
            {
                SpawnXPDrop(_enemy.transform.position, E_PickupRarity.COMMON);
            }
            
        }
        
    }


    public void SpawnHealthDrop(Vector3 _location, E_PickupRarity _rarity)
    {
        PickupBase dropToSpawn = null;
        switch (_rarity)
        {
            case E_PickupRarity.COMMON:
                dropToSpawn = HealthDropCommonPF;
                break;
            case E_PickupRarity.UNCOMMON:
                dropToSpawn = HealthDropUnCommonPF;
                break;
            case E_PickupRarity.RARE:
                dropToSpawn = HealthDropRarePF;
                break;
            default:
                break;
        }

        var drop = Instantiate(dropToSpawn, new Vector3(_location.x, fPickupYhover, _location.z), Quaternion.identity);
        
    }

    public void SpawnXPDrop(Vector3 _location, E_PickupRarity _rarity)
    {
        PickupBase dropToSpawn = null;
        switch (_rarity)
        {
            case E_PickupRarity.COMMON:
                dropToSpawn = XPDropCommonPF;
                break;
            case E_PickupRarity.UNCOMMON:
                dropToSpawn = XPDropUnCommonPF;
                break;
            case E_PickupRarity.RARE:
                dropToSpawn = XPDropRarePF;
                break;
            default:
                break;
        }

        var drop = Instantiate(dropToSpawn, new Vector3(_location.x, fPickupYhover, _location.z), Quaternion.identity);
        
    }


    public void SpawnUpgradeDrop(Vector3 _location)
    {
        var drop = Instantiate(UpgradeDropPF, new Vector3(_location.x, fPickupYhover, _location.z), Quaternion.identity);
        ActivePickups.Add(drop);
    }


    public void SpawnMagnetDrop(Vector3 _location)
    {
        var drop = Instantiate(MagnetDropPF, new Vector3(_location.x, fPickupYhover, _location.z), Quaternion.identity);
        ActivePickups.Add(drop);
    }
}
