using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class KucingMovement : MonoBehaviour
{
    private Transform kucingTransform;
    private Rigidbody2D kucingBody;
    private KucingAudio kucingAudio;
    public Transform playerTransform;
    public Camera gameCamera;
    private KucingAttack serangan;
    private bool bisaJalan = true;
    private float waktuSerang = 5;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        kucingTransform = GetComponent<Transform>();
        kucingBody = GetComponent<Rigidbody2D>();
        kucingAudio = GetComponent<KucingAudio>();
        serangan = GetComponent<KucingAttack>();
    }

    // Update is called once per frame
    void Update()
    {
        waktuSerang -= Time.deltaTime;
        if (bisaJalan && waktuSerang < 0)
        {
            waktuSerang = 5;
            int randomSerangan = UnityEngine.Random.Range(0, 2);
            switch (randomSerangan)
            {
                case 0:
                    serangan.Meong();
                    break;
                case 1:
                    serangan.Misil();
                    break;
                default:
                    break;
            }
            StartCoroutine(Berhenti(1f));
            StartCoroutine(BisaJalanLagi(1.5f));
            bisaJalan = false;
        }
        if (gameCamera.transform.position.x - kucingTransform.position.x > 8 && bisaJalan)
        {
            if (playerTransform.position.x - kucingTransform.position.x < 2) {
                kucingBody.linearVelocityX = (playerTransform.position.x - kucingTransform.position.x) * 10;
            }
            else {
                kucingBody.linearVelocityX = 20;
            }
            kucingBody.linearVelocityX = 20;
            kucingTransform.localScale = new Vector3(0.8f, 1.25f, 1f);
            if (playerTransform.position.x - kucingTransform.position.x < 5)
            {
                kucingBody.linearVelocityY = (playerTransform.position.y - kucingTransform.position.y) * 10;
            }
            else if (kucingTransform.position.y - playerTransform.position.y > 2)
            {
                kucingBody.linearVelocityY = -20;
            }
            else if (kucingTransform.position.y - playerTransform.position.y < -2)
            {
                kucingBody.linearVelocityY = 20;
            }
            else if (kucingTransform.position.y - playerTransform.position.y > 0.5f)
            {
                kucingBody.linearVelocityY = -10;
            }
            else if (kucingTransform.position.y - playerTransform.position.y < -0.5f)
            {
                kucingBody.linearVelocityY = 10;
            }
            else
            {
                kucingBody.linearVelocityY = 0;
            }
            StartCoroutine(Berhenti(0.1f));
            StartCoroutine(BisaJalanLagi(0.2f));
            bisaJalan = false;
            kucingAudio.Jalan();
        }
    }
    IEnumerator BisaJalanLagi(float detik)
    {
        yield return new WaitForSeconds(detik);
        bisaJalan = true;
    }
    IEnumerator Berhenti(float detik)
    {
        yield return new WaitForSeconds(detik);
        kucingBody.linearVelocity = new Vector2(0, 0);
        kucingTransform.localScale = new Vector3(1, 1, 1);
        kucingTransform.position = new Vector3(math.round(kucingTransform.position.x - 0.5f) + 0.5f, math.round(kucingTransform.position.y - 0.5f) + 0.5f, kucingTransform.position.z);
    }
}
