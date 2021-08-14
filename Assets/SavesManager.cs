using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using UnityEngine;

public interface ISave
{
    void Save();
    void Load();
}

public class SavesManager : MonoBehaviour
{
    public static SavesManager instance;
    public enum SaveType { Binary, Json, XML};
    public static SaveType currentSaveType { get; private set; } = SaveType.Binary;
    public static string savePath { get; private set;}
    Dictionary<SaveType, ISave> savers = new Dictionary<SaveType, ISave>();

    public void ChangeType(SaveType saveType)
    {
        currentSaveType = saveType;
    }

    public void Save()
    {
        savers[currentSaveType].Save();
    }
    public void Load()
    {
        savers[currentSaveType].Load();
    }


    private void Start()
    {
        instance = this;
        savePath = Application.dataPath;
        savers.Add(SaveType.Binary, new SaveBinary());
        savers.Add(SaveType.Json, new SaveJson());
        savers.Add(SaveType.XML, new SaveXML());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Save();
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            Load();
        }
    }
}

public class SaveBinary : MonoBehaviour, ISave
{
    [System.Serializable]
    public class Bector3
    {
        public float x, y, z;
        public Bector3(Vector3 vector)
        {
            x = vector.x;
            y = vector.y;
            z = vector.z;
        }
        public Vector3 Convert()
        {
            return new Vector3(x, y, z);
        }
    }
    [System.Serializable]
    public class PlayerData
    {
        public Bector3 pos, rot, scale, velocity, angularVelocity;
    }
    [System.Serializable]
    public class WorldData
    {
        public List<PlayerData> playerDatas = new List<PlayerData>();
        public int coins = 0;
        public PlayerData coin = null;
    }
    public void Load()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        using (FileStream fs = new FileStream(SavesManager.savePath + "/binaryWorld.dat", FileMode.OpenOrCreate))
        {
            WorldData wordData = (WorldData)formatter.Deserialize(fs);

            //Destroy all players
            GameManager.DestroyAllPlayers();
            //Players Creating
            foreach (var playerData in wordData.playerDatas)
            {
                var newPlayer = GameManager.CreatePlayer(true); //Можно вычислять кол-во игроков но я так не делал по ряду причин.
                newPlayer.transform.position = playerData.pos.Convert();
                newPlayer.transform.localEulerAngles = playerData.rot.Convert();
                newPlayer.transform.localScale = playerData.scale.Convert();
                newPlayer.GetComponent<Rigidbody>().velocity = playerData.velocity.Convert();
                newPlayer.GetComponent<Rigidbody>().angularVelocity = playerData.angularVelocity.Convert();
            }

            GameManager.coins = wordData.coins;

            if (wordData.coin == null)
            {
                if (GameManager.instance.coin)
                    Destroy(GameManager.instance.coin.gameObject);
            }
            else
            {
                if (GameManager.instance.coin == null)
                {
                    GameManager.instance.CreateCoin();
                }
                GameManager.instance.coin.transform.position = wordData.coin.pos.Convert();
                GameManager.instance.coin.transform.localEulerAngles = wordData.coin.rot.Convert();
            }
            
            GameManager.instance.FindAllPlayers();
        }
    }

    public void Save()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        //Set players
        List<PlayerData> playerData = new List<PlayerData>();
        foreach (var pl in GameManager.instance.players)
        {
            playerData.Add(new PlayerData() { 
                pos = new Bector3(pl.transform.position), 
                rot = new Bector3(pl.transform.localEulerAngles), 
                scale = new Bector3(pl.transform.localScale), 
                velocity = new Bector3(pl.GetComponent<Rigidbody>().velocity), 
                angularVelocity = new Bector3(pl.GetComponent<Rigidbody>().angularVelocity) });
        }

        //Coin
        PlayerData coin = null;
        if (GameManager.instance.coin != null)
        {
            coin = new PlayerData() { 
                pos = new Bector3(GameManager.instance.coin.transform.position), 
                rot = new Bector3(GameManager.instance.coin.transform.localEulerAngles) };
        }

        //Formating Data
        var data = new WorldData()
        {
            coins = GameManager.coins,
            playerDatas = playerData,
            coin = coin
        };

        using (FileStream fs = new FileStream(SavesManager.savePath + "/binaryWorld.dat", FileMode.OpenOrCreate))
        {
            // сериализуем весь массив people
            formatter.Serialize(fs, data);
        }
#if UNITY_EDITOR
        print("Binary Complete: " + SavesManager.savePath + "/binaryWorld.dat");
#endif

    }
} //Стул 1

public class SaveJson : MonoBehaviour, ISave
{
    public class Player {

        public Vector3 pos, rot, scale, velocity, angularVelocity;
    }

    public class WorldData {
        public List<Player> playerDatas = new List<Player>();
        public int coins = 0;
        public Player coin = null;
    }

    public void Load()
    {
        
        WorldData wordData = JsonConvert.DeserializeObject<WorldData>(File.ReadAllText(SavesManager.savePath + "/jsonWorld.json"));

        if (wordData != null)
        {
            //Destroy all players
            GameManager.DestroyAllPlayers();
            //Players Creating
            foreach (var playerData in wordData.playerDatas)
            {
                var newPlayer = GameManager.CreatePlayer(true); //Можно вычислять кол-во игроков но я так не делал по ряду причин.
                newPlayer.transform.position = playerData.pos;
                newPlayer.transform.localEulerAngles = playerData.rot;
                newPlayer.transform.localScale = playerData.scale;
                newPlayer.GetComponent<Rigidbody>().velocity = playerData.velocity;
                newPlayer.GetComponent<Rigidbody>().angularVelocity = playerData.angularVelocity;
            }

            GameManager.coins = wordData.coins;

            if (wordData.coin == null)
            {
                if (GameManager.instance.coin)
                    Destroy(GameManager.instance.coin.gameObject);
            }
            else
            {
                if (GameManager.instance.coin == null)
                {
                    GameManager.instance.CreateCoin();
                }
                GameManager.instance.coin.transform.position = wordData.coin.pos;
                GameManager.instance.coin.transform.localEulerAngles = wordData.coin.rot;
            }

            GameManager.instance.FindAllPlayers();
        }
    }
    public void Save()
    {
        List<Player> playerData = new List<Player>();
        foreach (var pl in GameManager.instance.players)
        {
            playerData.Add(new Player() { 
                pos = pl.transform.position, 
                rot = pl.transform.localEulerAngles, 
                scale = pl.transform.localScale, 
                velocity = pl.GetComponent<Rigidbody>().velocity, 
                angularVelocity = pl.GetComponent<Rigidbody>().angularVelocity 
            });
        }

        //Coin
        Player coin = null;
        if (GameManager.instance.coin != null)
        {
            coin = new Player() { pos = GameManager.instance.coin.transform.position, rot = GameManager.instance.coin.transform.localEulerAngles };
        }

        //Formating Data
        var data = new WorldData()
        {
            coins = GameManager.coins,
            playerDatas = playerData,
            coin = coin
        };

        File.WriteAllText(
            SavesManager.savePath + "/jsonWorld.json", 
            JsonConvert.SerializeObject(data, Formatting.Indented, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore //Отмена залупливания обращений 
                })
            );

        #if UNITY_EDITOR
                print("Json Complete: " + SavesManager.savePath + "/jsonWorld.json");
        #endif
    }
} //Стул 2

public class SaveXML : MonoBehaviour, ISave 
{
    [System.Serializable]
    public class Player
    {
        public Vector3 pos, rot, scale, velocity, angularVelocity;
    }
    [System.Serializable]
    public class WorldData
    {
        public List<Player> playerDatas = new List<Player>();
        public int coins = 0;
        public Player coin = null;
    }

    public void Load()
    {
        XmlSerializer formatter = new XmlSerializer(typeof(WorldData));
      
        using (FileStream fs = new FileStream(SavesManager.savePath + "/XMLWorld.xml", FileMode.OpenOrCreate))
        {
            WorldData wordData = (WorldData)formatter.Deserialize(fs);

            //Destroy all players
            GameManager.DestroyAllPlayers();

            //Players Creating
            foreach (var playerData in wordData.playerDatas)
            {
                var newPlayer = GameManager.CreatePlayer(true); //Можно вычислять кол-во игроков но я так не делал по ряду причин.
                newPlayer.transform.position = playerData.pos;
                newPlayer.transform.localEulerAngles = playerData.rot;
                newPlayer.transform.localScale = playerData.scale;
                newPlayer.GetComponent<Rigidbody>().velocity = playerData.velocity;
                newPlayer.GetComponent<Rigidbody>().angularVelocity = playerData.angularVelocity;
            }

            GameManager.coins = wordData.coins;

            if (wordData.coin == null)
            {
                if (GameManager.instance.coin)
                    Destroy(GameManager.instance.coin.gameObject);
            }
            else
            {
                if (GameManager.instance.coin == null)
                {
                    GameManager.instance.CreateCoin();
                }
                GameManager.instance.coin.transform.position = wordData.coin.pos;
                GameManager.instance.coin.transform.localEulerAngles = wordData.coin.rot;
            }

            GameManager.instance.FindAllPlayers();
        }
    }

    public void Save()
    {
        XmlSerializer formatter = new XmlSerializer(typeof(WorldData));
        //Set players
        List<Player> playerData = new List<Player>();
        foreach (var pl in GameManager.instance.players)
        {
            playerData.Add(new Player()
            {
                pos = pl.transform.position,
                rot = pl.transform.localEulerAngles,
                scale = pl.transform.localScale,
                velocity = pl.GetComponent<Rigidbody>().velocity,
                angularVelocity = pl.GetComponent<Rigidbody>().angularVelocity
            });
        }

        //Coin
        Player coin = null;
        if (GameManager.instance.coin != null)
        {
            coin = new Player()
            {
                pos = (GameManager.instance.coin.transform.position),
                rot = (GameManager.instance.coin.transform.localEulerAngles)
            };
        }

        //Formating Data
        var data = new WorldData()
        {
            coins = GameManager.coins,
            playerDatas = playerData,
            coin = coin
        };

        if (File.Exists(SavesManager.savePath + "/XMLWorld.xml"))
        {
            File.WriteAllText(SavesManager.savePath + "/XMLWorld.xml", "");
        }

        using (FileStream fs = new FileStream(SavesManager.savePath + "/XMLWorld.xml", FileMode.OpenOrCreate))
        {
            // сериализуем весь массив people
            formatter.Serialize(fs, data);
        }
#if UNITY_EDITOR
        print("XML Complete: " + SavesManager.savePath + "/XMLWorld.xml");
#endif
    }
} //Стул 3



