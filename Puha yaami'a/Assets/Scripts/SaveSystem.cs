using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{

    public static void SaveAll(Player player, GameManager gameMgr)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        SavePlayer(player, formatter);
        SaveGameManager(gameMgr, formatter);
    }

    private static void SavePlayer(Player player, BinaryFormatter fmt)
    {
        string path = Path.Combine(Application.persistentDataPath, "player.dat");
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData playerData = new PlayerData(player);
        fmt.Serialize(stream, playerData);
        stream.Close();
    }

    private static void SaveGameManager(GameManager gm, BinaryFormatter fmt)
    {
        string path = Path.Combine(Application.persistentDataPath, "game.dat");
        FileStream stream = new FileStream(path, FileMode.Create);

        GameManagerData gmData = new GameManagerData(gm);
        fmt.Serialize(stream, gmData);
        stream.Close();
    }


    //NOTE: can make this one parent method calling these 2 methods that returns a list of objects with the correct data in the future if we add more stuff to save
    public static PlayerData LoadPlayer()
    {
        string path = Path.Combine(Application.persistentDataPath, "player.dat");
        if(File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerData data = (PlayerData)formatter.Deserialize(stream);
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("Save File Not found in " + path);
            return null;
        }
    }

    public static GameManagerData LoadGM()
    {
        string path = Path.Combine(Application.persistentDataPath, "game.dat");
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            GameManagerData data = (GameManagerData)formatter.Deserialize(stream);
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("Save File Not found in " + path);
            return null;
        }
    }
}
