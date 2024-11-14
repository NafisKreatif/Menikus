using System.Collections;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public PlayerMovement player;
    public GameObject parentTulisan;
    public TMP_Text tulisanKalah;
    [TextArea(2, 5)]
    public string[] pesanKalah;
    public TMP_Text tulisanScore;
    public GameObject tulisanPressAnyKey;
    private SaveSystem saveSystem;
    void Start()
    {
        saveSystem = GetComponent<SaveSystem>();
    }
    void Update()
    {
        if (tulisanPressAnyKey.activeInHierarchy)
        {
            if (Input.anyKeyDown)
            {
                ToMainMenu();
            }
        }
    }
    public void Kalah()
    {
        parentTulisan.SetActive(true);
        tulisanPressAnyKey.SetActive(false);
        tulisanKalah.text = "OH NO! \n" + pesanKalah[Random.Range(0, pesanKalah.Count())];
        string path = Application.persistentDataPath + "/SaveData.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            if (data.highscore < player.skor)
            {
                tulisanScore.text = "Anyway, highscore baru: " + player.skor;
            }
            else {
                tulisanScore.text = "Anyway, score: " + player.skor;
            }
        }
        else {
            tulisanScore.text = "Anyway, highscore baru: " + player.skor;
        }
        saveSystem.SaveScore();
        IEnumerator pressAny()
        {
            yield return new WaitForSeconds(2f);
            tulisanPressAnyKey.SetActive(true);
        }
        StartCoroutine(pressAny());
    }
    public void ToMainMenu()
    {
        parentTulisan.SetActive(false);
        SceneManager.LoadScene(0);
    }
}
