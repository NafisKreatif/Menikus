using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

class PlayerBom : MonoBehaviour
{
    public Tile bomHitam;
    public Tile bomMerah;
    public Tile ledakan;
    public Tilemap rintangan;
    public MazeGenerator mazeGenerator;
    public AudioSource bomBerdetik;
    public AudioSource bomMeledak;
    public IEnumerator BomHitam(float detik, Vector3Int currentTile)
    {
        yield return new WaitForSeconds(detik);
        rintangan.SetTile(currentTile, bomHitam);
        
    }
    public IEnumerator BomMerah(float detik, Vector3Int currentTile)
    {
        yield return new WaitForSeconds(detik);
        rintangan.SetTile(currentTile, bomMerah);
        bomBerdetik.Play();
    }
    public IEnumerator BomMeledak(float detik, Vector3Int currentTile)
    {
        yield return new WaitForSeconds(detik);
        rintangan.SetTile(currentTile, ledakan);
        bomMeledak.Play();
    }
    public IEnumerator SelesaiMeledak(float detik, Vector3Int currentTile)
    {
        yield return new WaitForSeconds(detik);
        rintangan.SetTile(currentTile, null);
    }
}