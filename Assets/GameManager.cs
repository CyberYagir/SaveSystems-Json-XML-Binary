using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;




public class GameManager : MonoBehaviour
{
    public static int coins = 0;
    public static GameManager instance;

    public const float distToPlayer = -7.42f;

    public List<Rigidbody> players { get; private set; }
    [SerializeField] Explosion explosion;
    public GameObject coin { get; private set; }
    float time, cooldown = 1;
    public GameObject explodeParticles;
    public static void AddMoney(int add)
    {
        coins += add;
        if (coins >= 10)
        {
            for (int i = 0; i < instance.players.Count; i++)
            {
                instance.players[i].transform.localScale /= 1.1f;
            }
            var newCube = CreatePlayer();
            newCube.transform.localScale = instance.players[0].transform.localScale;
            instance.FindAllPlayers();
            coins = 0;
        }
        
    }
    public static GameObject CreatePlayer()
    {
       return Instantiate(GameObjectResources("JumpingCube"), instance.players[0].transform.position, Quaternion.identity);
    }
    public static GameObject CreatePlayer(bool empty)
    {
        return Instantiate(GameObjectResources("JumpingCube"));
    }
    public static void DestroyAllPlayers()
    {
        for (int i = 0; i < instance.players.Count; i++)
        {
            if (instance.players[i])
            {
                instance.players[i].gameObject.tag = "Destroy";
                Destroy(instance.players[i].gameObject);
            }
        }
        instance.players = new List<Rigidbody>();
    }
    public void FindAllPlayers()
    {
        players = FindObjectsOfType<Rigidbody>().ToList().FindAll(x=>x.tag != "Destroy");
    }

    public static GameObject GameObjectResources(string path)
    {
        return Resources.Load<GameObject>(path);
    }

    private void Start()
    {
        instance = this;
        FindAllPlayers();
    }

    public void CreateCoin()
    {
        var walls = FindObjectOfType<WallManager>();
        coin = Instantiate(GameObjectResources("Coin"), new Vector3 (Random.Range(walls.left.transform.position.x+1, walls.right.transform.position.x-1), Random.Range(5,walls.up.transform.position.y-2.5f), GameManager.distToPlayer), Quaternion.identity); 
    }

    void Update()
    {
        if (coin == null)
        {
            time += Time.deltaTime;
            if (time >= cooldown)
            {
                time = 0;
                CreateCoin();
            }
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                explodeParticles.transform.position = new Vector3(hit.point.x, 2.3f, GameManager.distToPlayer);
                explodeParticles.GetComponent<ParticleSystem>().Play();
                for (int i = 0; i < players.Count; i++)
                {
                    players[i].AddExplosionForce(explosion.force, new Vector3(hit.point.x, 1, players[i].transform.position.z), explosion.radius);
                }
            }   
        }
    }


    [System.Serializable]
    public class Explosion
    {
        public float radius, force;
    }
}
