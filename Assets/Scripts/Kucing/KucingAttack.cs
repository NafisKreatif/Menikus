using System.Collections;
using NUnit.Framework;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class KucingAttack : MonoBehaviour
{
    private Transform kucingTransform;
    private SpriteRenderer kucingRender;
    private KucingAudio kucingAudio;
    public Tilemap rintangan;
    public Tile target;
    public Tile ledakan;
    public MazeGenerator mazeGenerator;
    public GameObject seranganSuara;
    public GameObject player;
    private PlayerMovement playerMovement;
    private Transform suaraTransform;
    private Rigidbody2D suaraBody;
    private bool bisaKenaSuara;
    void Start()
    {
        kucingTransform = GetComponent<Transform>();
        kucingRender = GetComponent<SpriteRenderer>();
        kucingAudio = GetComponent<KucingAudio>();
        playerMovement = player.GetComponent<PlayerMovement>();
        suaraTransform = seranganSuara.GetComponent<Transform>();
        suaraBody = seranganSuara.GetComponent<Rigidbody2D>();
        bisaKenaSuara = false;
        seranganSuara.SetActive(false);
    }
    void Update()
    {
        if (playerMovement.bisaJalan && bisaKenaSuara && math.abs(suaraTransform.position.x - player.transform.position.x) < 1f && math.abs(suaraTransform.position.y - player.transform.position.y) < 2f)
        {
            Debug.Log("Tikus kena MEOONG!");
            playerMovement.KenaDanMental();
            bisaKenaSuara = false;
        }
    }
    public void Meong()
    {
        StartCoroutine(Bersuara(1f));
        kucingRender.color = Color.cyan;
    }
    public void Misil(int banyakTarget = 5)
    {
        StartCoroutine(BuatTarget(1f, banyakTarget));
        kucingRender.color = Color.magenta;
    }
    IEnumerator Bersuara(float detik)
    {
        yield return new WaitForSeconds(detik);
        seranganSuara.SetActive(true);
        bisaKenaSuara = true;
        suaraTransform.position = new Vector3(kucingTransform.position.x + 2, kucingTransform.position.y, suaraTransform.transform.position.z);
        suaraBody.linearVelocityX = 10;
        kucingRender.color = Color.white;
        kucingAudio.MeongSerang();
        StartCoroutine(HilangkanSuara(3f));
    }
    IEnumerator HilangkanSuara(float detik)
    {
        yield return new WaitForSeconds(detik);
        seranganSuara.SetActive(false);
        seranganSuara.transform.position = new Vector3(seranganSuara.transform.position.x, 100, seranganSuara.transform.position.z);
        suaraBody.linearVelocity = new Vector2(0, 0);
    }
    IEnumerator BuatTarget(float detik, int banyakTarget)
    {
        yield return new WaitForSeconds(detik);
        Vector3Int koordinatKucing = rintangan.WorldToCell(kucingTransform.position);
        for (int i = 0; i < banyakTarget; i++)
        {
            Vector3Int koordinatTarget = koordinatKucing;
            koordinatTarget.x += UnityEngine.Random.Range(8, 20);
            koordinatTarget.y = UnityEngine.Random.Range(-5, 5);
            if (rintangan.GetTile(koordinatTarget) != null)
            {
                i--;
                continue;
            }
            rintangan.SetTile(koordinatTarget, target);
            StartCoroutine(Kabum(1f, koordinatTarget));
        }
        kucingRender.color = Color.white;
        kucingAudio.MeongSerang();
    }
    IEnumerator Kabum(float detik, Vector3Int tile)
    {
        bool loop = mazeGenerator.isLooping;
        yield return new WaitForSeconds(detik);
        if (loop != mazeGenerator.isLooping)
        {
            tile.x -= mazeGenerator.width;
        }
        kucingAudio.Kabum();
        rintangan.SetTile(tile, ledakan);
        Vector3Int koordinatPlayer = rintangan.WorldToCell(player.transform.position);
        if (playerMovement.bisaJalan && math.abs(koordinatPlayer.x - tile.x) < 2f && math.abs(koordinatPlayer.y - tile.y) < 2f)
        {
            Debug.Log("Tikus kena KABUM!");
            playerMovement.KenaDanMental();
        }
        StartCoroutine(SelesaiKabum(0.2f, tile));
    }
    IEnumerator SelesaiKabum(float detik, Vector3Int tile)
    {
        bool loop = mazeGenerator.isLooping;
        yield return new WaitForSeconds(detik);
        if (loop != mazeGenerator.isLooping)
        {
            tile.x -= mazeGenerator.width;
        }
        rintangan.SetTile(tile, null);
    }
}
