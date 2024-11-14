using System.Linq;
using UnityEngine;

public class KucingAudio : MonoBehaviour
{
    public AudioSource jalan;
    public AudioSource[] meongSerang;
    public AudioSource warMeong;
    public AudioSource kabum;
    public void Jalan()
    {
        warMeong.Stop();
        jalan.Play();
    }
    public void MeongSerang()
    {
        int index = Random.Range(0, meongSerang.Count());
        meongSerang[index].Stop();
        meongSerang[index].Play();
    }
    public void WarMeong()
    {
        warMeong.Stop();
        warMeong.Play();
    }
    public void Kabum()
    {
        kabum.Stop();
        kabum.Play();
    }
}
