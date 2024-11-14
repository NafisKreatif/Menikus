using TMPro;
using Unity.Mathematics;
using UnityEngine;
using System.IO;

public class SaveSystem : MonoBehaviour
{
    public PlayerMovement player;
    public TMP_Text tulisanHighscore;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadScore();
    }
    public void SaveScore()
    {
        string path = Application.persistentDataPath + "/SaveData.json";
        SaveData data = new();
        data.highscore = player.skor;
        if (File.Exists(path))
        {
            string oldJson = File.ReadAllText(path);
            SaveData oldData = JsonUtility.FromJson<SaveData>(oldJson);
            data.highscore = math.max(player.skor, oldData.highscore);
        }
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(path, json);
    }

    public void LoadScore()
    {
        string path = Application.persistentDataPath + "/SaveData.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            tulisanHighscore.text = "Highscore: " + data.highscore;
        }
    }
}
