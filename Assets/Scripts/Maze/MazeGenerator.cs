using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
public class MazeGenerator : MonoBehaviour
{
    public RuleTile dinding;
    public RuleTile tanah;
    public Tile[] batu;
    public Tile[] makanan;
    public Tile bom;
    public Tile tembus;
    public Tile hati;
    public Tilemap lantai;
    public Tilemap rintangan;
    public MazeTile[,] maze;
    public int width = 20;
    public int height = 20;
    public int offsetX = 10;
    public int offsetY = 10;
    public int persenBatu = 10;
    public int persenMakanan = 20;
    public int persenHati = 5;
    public int persenBom = 5;
    public int persenTembus = 5;
    private int[,] visited;
    public bool isLooping = true;
    private MazeSlider mazeSlider;
    private List<Vector3Int> randomTile;
    private List<int> randomY;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mazeSlider = GetComponent<MazeSlider>();
        maze = new MazeTile[width*2, height];
        for (int i = 0; i < width*2; i++)
        {
            for (int j = 0; j < height; j++)
            {
                maze[i, j] = new();
            }
        }
        GenerateMaze();
        SetMaze();
        StartCoroutine(GenerationLoop(width / mazeSlider.slideSpeed / 2.0f));
        // Pinggir bawah
        for (int i = offsetX - width; i < offsetX + width; i++)
        {
            for (int j = offsetY; j < offsetY + height; j++)
            {
                Vector3Int currentTile = new(i, j, 0);
                lantai.SetTile(currentTile, tanah);
            }
        }
        for (int i = offsetX - width; i < offsetX + width; i++)
        {
            Vector3Int currentTile = new(i, offsetY - 1, 0);
            rintangan.SetTile(currentTile, dinding);
            lantai.SetTile(currentTile, tanah);
            currentTile.y -= 1;
            rintangan.SetTile(currentTile, dinding);
            lantai.SetTile(currentTile, tanah);
        }
        // Pinggir atas
        for (int i = offsetX - width; i < offsetX + width; i++)
        {
            Vector3Int currentTile = new(i, offsetY + height, 0);
            rintangan.SetTile(currentTile, dinding);
            rintangan.SetTile(currentTile, dinding);
            lantai.SetTile(currentTile, tanah);
            currentTile.y += 1;
            rintangan.SetTile(currentTile, dinding);
            lantai.SetTile(currentTile, tanah);
        }
    }
    void GenerateMaze()
    {
        int xAwal;
        if (isLooping)
        {
            xAwal = 0;
        }
        else
        {
            xAwal = width;
        }
        randomTile = new();
        randomY = new();
        visited = new int[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                maze[i + xAwal, j] = new();
                randomTile.Add(new Vector3Int(i, j, UnityEngine.Random.Range(0, 10000000)));
                maze[i + xAwal, j].terisi = true;
            }
        }
        randomTile.Sort((a, b) =>
            a.z.CompareTo(b.z)
        );
        for (int j = 0; j < height; j++)
        {
            if (!maze[width - xAwal, j].terisi) {
                randomY.Add(j);
            }
        }
        int yAwal = randomY[UnityEngine.Random.Range(0, randomY.Count())];
        int index = 1;
        int iteration = 1;
        int[] directionX = { 1, 0, -1, 0 };
        int[] directionY = { 0, 1, 0, -1 };
        visited[0, yAwal] = 1;
        maze[xAwal, yAwal].terisi = false;
        while (index < randomTile.Count())
        {
            List<Vector2Int> dfs = new()
            {
                new Vector2Int(randomTile[index].x, randomTile[index].y)
            };
            iteration++;
            index++;
            if (visited[dfs.Last().x, dfs.Last().y] > 0)
            {
                continue;
            }
            while (true)
            {
                Vector2Int now = dfs.Last();
                visited[now.x, now.y] = iteration;
                int p = UnityEngine.Random.Range(0, 4);
                Vector2Int next = new(now.x + directionX[p], now.y + directionY[p]);
                // Out of bound
                if (next.x < 0 || next.y < 0 || next.x >= width || next.y >= height)
                    continue;
                // Balik lagi
                if (dfs.Count() > 1 && next == dfs[dfs.Count() - 2])
                    continue;
                // Ada loop
                if (visited[next.x, next.y] == iteration)
                {
                    while (dfs.Last() != next)
                    {
                        visited[dfs.Last().x, dfs.Last().y] = 0;
                        dfs.RemoveAt(dfs.Count() - 1);
                    }
                    continue;
                }
                // Bertemu dengan maze utama
                if (visited[next.x, next.y] > 0)
                {
                    // Menandakan tidak boleh berdekatan
                    maze[next.x + xAwal, next.y].terisi = false;
                    for (int q = 0; q < 4; q++)
                    {
                        Vector2Int nextNext = new(next.x + directionX[q], next.y + directionY[q]);
                        if (nextNext.x < 0 || nextNext.y < 0 || nextNext.x >= width || nextNext.y >= height)
                            continue;
                        visited[nextNext.x, nextNext.y] = iteration;
                    }
                    while (dfs.Count() > 0)
                    {
                        next = dfs.Last();
                        dfs.RemoveAt(dfs.Count() - 1);
                        maze[next.x + xAwal, next.y].terisi = false;
                        for (int q = 0; q < 4; q++)
                        {
                            Vector2Int nextNext = new(next.x + directionX[q], next.y + directionY[q]);
                            if (nextNext.x < 0 || nextNext.y < 0 || nextNext.x >= width || nextNext.y >= height)
                                continue;
                            visited[nextNext.x, nextNext.y] = iteration;
                        }
                    }
                    break;
                }
                dfs.Add(next);
            }
        }
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (maze[i + xAwal, j].terisi == false) continue;
                for (int q = 0; q < 4; q++)
                {
                    Vector2Int samping = new(i + directionX[q], j + directionY[q]);
                    if (samping.x < 0 || samping.x >= width)
                    {
                        continue;
                    }
                    else if (samping.y < 0 || samping.y >= height)
                    {
                        maze[i + xAwal, j].dindingDekat += 1;
                    }
                    else
                    {
                        maze[samping.x + xAwal, samping.y].dindingDekat += 1;
                    }
                }
            }
        }
    }
    void SetMaze()
    {
        int xAwal;
        if (isLooping)
        {
            xAwal = 0;
        }
        else
        {
            xAwal = width;
        }
        isLooping = !isLooping;
        // Set sebelumnya
        for (int i = offsetX; i < offsetX + width; i++)
        {
            for (int j = offsetY; j < offsetY + height; j++)
            {
                Vector3Int tileTujuan = new(i - width, j, 0);
                Vector3Int tileSumber = new(i, j, 0);
                rintangan.SetTile(tileTujuan, rintangan.GetTile(tileSumber));
            }
        }
        // Set selanjutnya
        for (int i = offsetX; i < offsetX + width; i++)
        {
            for (int j = offsetY; j < offsetY + height; j++)
            {
                Vector3Int currentTile = new(i, j, 0);
                lantai.SetTile(currentTile, tanah);
                if (maze[i - offsetX + xAwal, j - offsetY].terisi)
                {
                    if (maze[i - offsetX + xAwal, j - offsetY].dindingDekat == 2 && UnityEngine.Random.Range(0, 100) < persenBatu)
                    {
                        rintangan.SetTile(currentTile, batu[0]);
                        maze[i - offsetX + xAwal, j - offsetY].tileType = "batu";
                        maze[i - offsetX + xAwal, j - offsetY].breakPoint = 5;
                    }
                    else
                    {
                        rintangan.SetTile(currentTile, dinding);
                        maze[i - offsetX + xAwal, j - offsetY].tileType = "dinding";
                    }
                }
                else
                {
                    int randomPersen = UnityEngine.Random.Range(0, 100);
                    if (maze[i - offsetX + xAwal, j - offsetY].dindingDekat == 3 && randomPersen < persenMakanan)
                    {
                        rintangan.SetTile(currentTile, makanan[UnityEngine.Random.Range(0, makanan.Count())]);
                        maze[i - offsetX + xAwal, j - offsetY].tileType = "makanan";
                        maze[i - offsetX + xAwal, j - offsetY].breakPoint = 1;
                    }
                    else if (maze[i - offsetX + xAwal, j - offsetY].dindingDekat == 3 && randomPersen < persenMakanan + persenHati) {
                        rintangan.SetTile(currentTile, hati);
                        maze[i - offsetX + xAwal, j - offsetY].tileType = "hati";
                    }
                    else if (maze[i - offsetX + xAwal, j - offsetY].dindingDekat == 3 && randomPersen < persenMakanan + persenHati + persenBom) {
                        rintangan.SetTile(currentTile, bom);
                        maze[i - offsetX + xAwal, j - offsetY].tileType = "bom";
                    }
                    else if (maze[i - offsetX + xAwal, j - offsetY].dindingDekat == 3 && randomPersen < persenMakanan + persenHati + persenBom + persenTembus) {
                        rintangan.SetTile(currentTile, tembus);
                        maze[i - offsetX + xAwal, j - offsetY].tileType = "tembus";
                    }
                    else
                    {
                        rintangan.SetTile(currentTile, null);
                    }
                }
            }
        }
    }
    IEnumerator GenerationLoop(float detik) {
        yield return new WaitForSeconds(detik);
        GenerateMaze();
        SetMaze();
        mazeSlider.Teleport(width);
        StartCoroutine(GenerationLoop(width / mazeSlider.slideSpeed));
    }
}

