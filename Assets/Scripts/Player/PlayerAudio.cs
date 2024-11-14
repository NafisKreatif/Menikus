using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    public AudioSource playerJalan;
    public AudioSource playerMakan;
    public AudioSource playerKena;
    public AudioSource playerAmbil;
    public AudioSource batuDitabrak;
    public AudioSource batuHancur;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void SuaraJalan() {
        playerJalan.Play(0);
    }
    public void SuaraMakan() {
        playerMakan.time = 0.35f;
        playerMakan.Play(0);
    }
    public void SuaraKena() {
        playerKena.Stop();
        playerKena.time = 0.60f;
        playerKena.Play(0);
    }
    public void SuaraTabrak() {
        batuDitabrak.Stop();
        batuDitabrak.time = 1.3f;
        batuDitabrak.Play(0);
    }
    public void SuaraHancur() {
        //0.71f;
        batuHancur.Stop();
        batuHancur.time = 1.3f;
        batuHancur.Play(0);
    }
    public void SuaraAmbil() {
        playerAmbil.Stop();
        playerAmbil.Play(0);
    }
}
