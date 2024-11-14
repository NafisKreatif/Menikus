using UnityEngine;
using System.Collections;
using Unity.Mathematics;
using UnityEngine.Tilemaps;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    private Transform playerTransform;
    public Transform kucingTransform;
    private Rigidbody2D playerBody;
    private SpriteRenderer playerRenderer;
    private PlayerAudio playerAudio;
    private PlayerBom playerBom;
    public Tilemap rintangan;
    public MazeGenerator mazeGenerator;
    public MazeSlider mazeSlider;
    public TMP_Text tulisanSkor;
    public Camera gameCamera;
    public GameOver gameOver;
    public int skor;
    public int jumlahHati;
    public GameObject[] hati;
    public int jumlahBom;
    public GameObject[] bom;
    public bool bisaJalan = true;
    public bool bisaTembus = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerTransform = GetComponent<Transform>();
        playerBody = GetComponent<Rigidbody2D>();
        playerAudio = GetComponent<PlayerAudio>();
        playerBom = GetComponent<PlayerBom>();
        playerRenderer = GetComponent<SpriteRenderer>();
        skor = 0;
        jumlahHati = 3;
        jumlahBom = 3;
        bisaJalan = false;
        bisaTembus = false;
        tulisanSkor.text = "Score: " + skor;
        playerTransform.localScale = new Vector3(1.25f, 1.25f, 1);
        playerBody.linearVelocity = new Vector2(4.5f - playerTransform.position.x, 0);
        playerAudio.SuaraKena();
        StartCoroutine(BisaJalanLagi(1f));
    }

    // Update is called once per frame
    void Update()
    {
        if (jumlahHati == 0) return;
        // Jalan ke atas
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && bisaJalan)
        {
            playerTransform.eulerAngles = Vector3.forward * 180;
            if (BisaJalanKeArah(new Vector2Int(0, 1)))
            {
                playerBody.linearVelocityY = 10;
            }
            bisaJalan = false;
            playerTransform.localScale = new Vector3(0.8f, 1.25f, 1);
            StartCoroutine(BisaJalanLagi(0.1f));
        }
        // Jalan ke bawah
        else if (Input.GetKeyDown(KeyCode.S) && bisaJalan)
        {
            playerTransform.eulerAngles = Vector3.forward * 0;
            if (BisaJalanKeArah(new Vector2Int(0, -1)))
            {
                playerBody.linearVelocityY = -10;
            }
            bisaJalan = false;
            playerTransform.localScale = new Vector3(0.8f, 1.25f, 1);
            StartCoroutine(BisaJalanLagi(0.1f));
        }
        // Jalan ke kiri
        if (Input.GetKeyDown(KeyCode.A) && bisaJalan)
        {
            playerTransform.eulerAngles = Vector3.forward * 270;
            if (BisaJalanKeArah(new Vector2Int(-1, 0)))
            {
                playerBody.linearVelocityX = -10;
            }
            bisaJalan = false;
            playerTransform.localScale = new Vector3(0.8f, 1.25f, 1);
            StartCoroutine(BisaJalanLagi(0.1f));
        }
        // Jalan ke kanan
        else if (Input.GetKeyDown(KeyCode.D) && bisaJalan)
        {
            playerTransform.eulerAngles = Vector3.forward * 90;
            if (BisaJalanKeArah(new Vector2Int(1, 0)))
            {
                playerBody.linearVelocityX = 10;
            }
            bisaJalan = false;
            playerTransform.localScale = new Vector3(0.8f, 1.25f, 1);
            StartCoroutine(BisaJalanLagi(0.1f));
        }
        if (bisaJalan && math.abs(kucingTransform.position.x - playerTransform.position.x) <= 1.5 && math.abs(kucingTransform.position.y - playerTransform.position.y) <= 1.5)
        {
            KenaDanMental();
        }
        if (Input.GetKeyDown(KeyCode.B) && bisaJalan)
        {
            PakeBom();
        }
    }

    IEnumerator BisaJalanLagi(float detik)
    {
        yield return new WaitForSeconds(detik);
        bisaJalan = true;
        if (jumlahHati > 0 && !bisaTembus) playerRenderer.color = Color.white;
        playerBody.linearVelocity = new Vector2(0, 0);
        playerTransform.localScale = new Vector3(1, 1, 1);
        playerTransform.position = new Vector3(math.round(playerTransform.position.x - 0.5f) + 0.5f, math.round(playerTransform.position.y - 0.5f) + 0.5f, playerTransform.position.z);
    }

    bool BisaJalanKeArah(Vector2Int arah)
    {
        Vector3Int koordinatPlayer = rintangan.WorldToCell(playerTransform.position);
        koordinatPlayer.x += arah.x - mazeGenerator.offsetX;
        koordinatPlayer.y += arah.y - mazeGenerator.offsetY;
        Vector3Int koordinatDiMaze = koordinatPlayer;
        if (mazeGenerator.isLooping)
        {
            koordinatDiMaze.x = (koordinatDiMaze.x + mazeGenerator.width) % (2 * mazeGenerator.width);
        }
        else
        {
            koordinatDiMaze.x = (koordinatDiMaze.x + 2 * mazeGenerator.width) % (2 * mazeGenerator.width);
        }
        if (koordinatPlayer.y < 0 || koordinatPlayer.y >= mazeGenerator.height)
        {
            playerAudio.SuaraJalan();
            return false;
        }
        else if (mazeGenerator.maze[koordinatDiMaze.x, koordinatPlayer.y].terisi == false || bisaTembus)
        {
            Vector3Int currentTile = new(koordinatPlayer.x + mazeGenerator.offsetX, koordinatPlayer.y + mazeGenerator.offsetY, 0);
            switch (mazeGenerator.maze[koordinatDiMaze.x, koordinatPlayer.y].tileType)
            {
                case "makanan":
                    mazeGenerator.rintangan.SetTile(currentTile, null);
                    mazeGenerator.maze[koordinatDiMaze.x, koordinatPlayer.y].tileType = "none";
                    skor += 100;
                    tulisanSkor.text = "Score: " + skor;
                    playerAudio.SuaraMakan();
                    break;
                case "hati":
                    mazeGenerator.rintangan.SetTile(currentTile, null);
                    mazeGenerator.maze[koordinatDiMaze.x, koordinatPlayer.y].tileType = "none";
                    TambahHati();
                    playerAudio.SuaraAmbil();
                    break;
                case "bom":
                    mazeGenerator.rintangan.SetTile(currentTile, null);
                    mazeGenerator.maze[koordinatDiMaze.x, koordinatPlayer.y].tileType = "none";
                    TambahBom();
                    playerAudio.SuaraAmbil();
                    break;
                case "tembus":
                    mazeGenerator.rintangan.SetTile(currentTile, null);
                    mazeGenerator.maze[koordinatDiMaze.x, koordinatPlayer.y].tileType = "tembus";
                    playerRenderer.color = Color.cyan;
                    playerAudio.SuaraAmbil();
                    bisaTembus = true;
                    StartCoroutine(SelesaiTembus(2f));
                    break;
                default:
                    playerAudio.SuaraJalan();
                    break;
            }
            return true;
        }
        else
        {
            Vector3Int currentTile = new(koordinatPlayer.x + mazeGenerator.offsetX, koordinatPlayer.y + mazeGenerator.offsetY, 0);
            switch (mazeGenerator.maze[koordinatDiMaze.x, koordinatPlayer.y].tileType)
            {
                case "batu":
                    mazeGenerator.maze[koordinatDiMaze.x, koordinatPlayer.y].breakPoint -= 1;
                    if (mazeGenerator.maze[koordinatDiMaze.x, koordinatPlayer.y].breakPoint == 0)
                    {
                        mazeGenerator.maze[koordinatDiMaze.x, koordinatPlayer.y].terisi = false;
                        mazeGenerator.maze[koordinatDiMaze.x, koordinatPlayer.y].tileType = "none";
                        playerAudio.SuaraHancur();
                    }
                    else
                    {
                        playerAudio.SuaraTabrak();
                    }
                    int indexBatu = 3 - ((mazeGenerator.maze[koordinatDiMaze.x, koordinatPlayer.y].breakPoint + 1) / 2);
                    mazeGenerator.rintangan.SetTile(currentTile, mazeGenerator.batu[indexBatu]);
                    break;
                default:
                    playerAudio.SuaraJalan();
                    break;
            }
            return false;
        }
    }
    // Mental == kelempar
    public void KenaDanMental()
    {
        Vector3Int koordinatTujuan = rintangan.WorldToCell(gameCamera.transform.position);
        koordinatTujuan.x += 10;
        Vector3Int koordinatDiMaze = koordinatTujuan;
        if (mazeGenerator.isLooping)
        {
            koordinatDiMaze.x = (koordinatDiMaze.x + mazeGenerator.width - mazeGenerator.offsetX) % (2 * mazeGenerator.width);
        }
        else
        {
            koordinatDiMaze.x = (koordinatDiMaze.x + 2 * mazeGenerator.width - mazeGenerator.offsetX) % (2 * mazeGenerator.width);
        }
        List<int> listKoordinatY = new();
        for (int j = 0; j < mazeGenerator.height; j++)
        {
            if (mazeGenerator.maze[koordinatDiMaze.x, j].terisi == false)
            {
                listKoordinatY.Add(j + mazeGenerator.offsetY);
            }
        }
        koordinatTujuan.y = listKoordinatY[UnityEngine.Random.Range(0, listKoordinatY.Count)];
        bisaJalan = false;
        playerAudio.SuaraKena();
        playerRenderer.color = new Color(1f, 0.5f, 0.5f, 1f);
        playerTransform.localScale = new Vector3(1.25f, 1.25f, 1);
        playerTransform.position = new Vector3(math.round(playerTransform.position.x - 0.5f) + 0.5f, math.round(playerTransform.position.y - 0.5f) + 0.5f, playerTransform.position.z);
        playerBody.linearVelocity = new Vector2(koordinatTujuan.x - playerTransform.position.x + 0.5f, koordinatTujuan.y - playerTransform.position.y + 0.5f);
        KurangiHati();
        StartCoroutine(BisaJalanLagi(1f));
    }
    public void KurangiHati()
    {
        if (jumlahHati == 0) return;
        jumlahHati--;
        hati[jumlahHati].SetActive(false);
        if (jumlahHati == 0)
        {
            gameOver.Kalah();
        }
    }
    public void TambahHati()
    {
        if (jumlahHati == 3) return;
        hati[jumlahHati].SetActive(true);
        jumlahHati++;
    }
    public void PakeBom()
    {
        if (jumlahBom == 0) return;
        jumlahBom--;
        bom[jumlahBom].SetActive(false);
        Vector3Int koordinatPlayer = rintangan.WorldToCell(playerTransform.position);
        koordinatPlayer.x -= mazeGenerator.offsetX;
        koordinatPlayer.y -= mazeGenerator.offsetY;
        Vector3Int koordinatDiMaze = koordinatPlayer;
        if (mazeGenerator.isLooping)
        {
            koordinatDiMaze.x = (koordinatDiMaze.x + mazeGenerator.width) % (2 * mazeGenerator.width);
        }
        else
        {
            koordinatDiMaze.x = (koordinatDiMaze.x + 2 * mazeGenerator.width) % (2 * mazeGenerator.width);
        }
        mazeGenerator.maze[koordinatDiMaze.x, koordinatPlayer.y].terisi = true;
        mazeGenerator.maze[koordinatDiMaze.x, koordinatPlayer.y].tileType = "meledak";

        StartCoroutine(HancurkanSegalanya(koordinatPlayer));
        StartCoroutine(KenaBomKah(koordinatPlayer));

        koordinatPlayer.x += mazeGenerator.offsetX;
        koordinatPlayer.y += mazeGenerator.offsetY;
        StartCoroutine(playerBom.BomMerah(0f, koordinatPlayer));
        StartCoroutine(playerBom.BomHitam(0.15f, koordinatPlayer));
        StartCoroutine(playerBom.BomMerah(0.30f, koordinatPlayer));
        StartCoroutine(playerBom.BomHitam(0.45f, koordinatPlayer));
        StartCoroutine(playerBom.BomMerah(0.60f, koordinatPlayer));
        StartCoroutine(playerBom.BomHitam(0.75f, koordinatPlayer));
        StartCoroutine(playerBom.BomMeledak(1f, koordinatPlayer));
    }
    public void TambahBom()
    {
        if (jumlahBom == 3) return;
        bom[jumlahBom].SetActive(true);
        jumlahBom++;
    }
    IEnumerator KenaBomKah(Vector3Int koordinatPlayer)
    {
        bool loop = mazeGenerator.isLooping;
        yield return new WaitForSeconds(1.1f);
        if (loop != mazeGenerator.isLooping) {
            koordinatPlayer.x -= mazeGenerator.width;
        }
        Vector3Int koordinatNow = rintangan.WorldToCell(playerTransform.position);
        koordinatNow.x -= mazeGenerator.offsetX;
        koordinatNow.y -= mazeGenerator.offsetY;
        if (math.abs(koordinatPlayer.x - koordinatNow.x) < 2 && math.abs(koordinatPlayer.y - koordinatNow.y) < 2)
        {
            KenaDanMental();
        }
    }
    IEnumerator HancurkanSegalanya(Vector3Int koordinatPlayer)
    {
        bool loop = mazeGenerator.isLooping;
        yield return new WaitForSeconds(1.1f);
        if (loop != mazeGenerator.isLooping) {
            koordinatPlayer.x -= mazeGenerator.width;
        }
        for (int i = -1; i <= 1; i++) {
            for (int j = -1; j <= 1; j++) {
                Vector3Int koordinatHancur = koordinatPlayer;
                koordinatHancur.x += i;
                koordinatHancur.y += j;
                Vector3Int koordinatDiMaze = koordinatHancur;
                if (mazeGenerator.isLooping)
                {
                    koordinatDiMaze.x = (koordinatDiMaze.x + mazeGenerator.width) % (2 * mazeGenerator.width);
                }
                else
                {
                    koordinatDiMaze.x = (koordinatDiMaze.x + 2 * mazeGenerator.width) % (2 * mazeGenerator.width);
                }
                if (koordinatDiMaze.y < 0 || koordinatDiMaze.y >= mazeGenerator.height) continue;
                mazeGenerator.maze[koordinatDiMaze.x, koordinatDiMaze.y].terisi = false;
                mazeGenerator.maze[koordinatDiMaze.x, koordinatDiMaze.y].tileType = "none";
                koordinatHancur.x += mazeGenerator.offsetX;
                koordinatHancur.y += mazeGenerator.offsetY; 
                StartCoroutine(playerBom.SelesaiMeledak(0f, koordinatHancur));
            }
        }
    }
    IEnumerator SelesaiTembus(float detik) {
        yield return new WaitForSeconds(detik);
        bisaTembus = false;
    }
}
