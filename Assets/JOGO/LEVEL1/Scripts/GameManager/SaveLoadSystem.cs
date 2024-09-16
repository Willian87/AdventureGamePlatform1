using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveLoadSystem
{
    private static string GetFilePath()
    {
        return Application.persistentDataPath + "/playerData.dat";
    }

    public static void SavePlayerData(PlayerCombat player)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = GetFilePath();

        using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            PlayerData data = new PlayerData(player);
            formatter.Serialize(fileStream, data);
            Debug.Log("Salvando... " + path);
        }

        //public static void SavePlayerData(PlayerCombat player)
        //{
        //    BinaryFormatter bFormatter = new BinaryFormatter();

        //    string path = Application.persistentDataPath + "/PlayerData.dat";

        //    FileStream fileStream = new FileStream(path, FileMode.Create);

        //    PlayerData data = new PlayerData(player);
        //    Debug.Log("Saving Game at " + path);
        //    bFormatter.Serialize(fileStream, data);

        //    fileStream.Close();


        //}

    }

    public static PlayerData LoadPlayerData()
    {
        static string GetFilePath()
        {
            return Application.persistentDataPath + "/playerData.dat";
        }
        string path = GetFilePath();

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();

            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return (PlayerData)formatter.Deserialize(fileStream);
            }
        }
        else
        {
            Debug.LogWarning("No save file found at " + path);
            return null;
        }

        //string path = Application.persistentDataPath + "/PlayerData.dat";

        //if (File.Exists(path))
        //{
        //    BinaryFormatter bFormatter = new BinaryFormatter();
        //    FileStream file = new FileStream(path, FileMode.Open);

        //    PlayerData data = bFormatter.Deserialize(file) as PlayerData;
        //    Debug.Log("Carregando...");
        //    file.Close();
        //    return data;
        //}
        //else
        //{
        //    Debug.LogError("Save file not found in " + path);
        //    return null;
        //}
    }
}



