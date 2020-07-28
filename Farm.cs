using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Farm : MonoBehaviour
{
    public Dictionary<Vector2Int, Chunk> chunks = new Dictionary<Vector2Int, Chunk>();
    public List<Sprite> backgroundSprites;
    public Material bacgroundMaterial;

    private const int farmSize = 5;
    float offsetX;
    float offsetY;
    float noiseScale = 20f;

    private Vector2Int prevChunk;
    private Vector2Int prevTile;
    private Vector2Int selectedChunk = Vector2Int.down;
    private Vector2Int selectedTile;

    bool breakingTile = false;

    public Transform player;

    public Inventory inventory;
    public Slider breakInfo;

    void Start()
    {
        GenerateNewFarm();
        TimeTickSystem.OnTick += delegate (object sender, TimeTickSystem.OnTickEventArgs e)
        {
            if (breakingTile)
            {
                if (selectedTile != prevTile)
                {
                    breakingTile = false;
                    Debug.Log("Moved from selecter");
                }
                else
                {
                    float dis = Vector2.Distance(chunks[selectedChunk].map[selectedTile.x, selectedTile.y].back.transform.position, player.position);
                    
                    if (dis >= 3f)
                    {
                        breakingTile = false;
                    }
                }
                bool broken = chunks[selectedChunk].map[selectedTile.x, selectedTile.y].front.Break();
                breakInfo.value = chunks[selectedChunk].map[selectedTile.x, selectedTile.y].front.TickPercent;
                //Debug.Log(chunks[selectedChunk].map[selectedTile.x, selectedTile.y].front.TickPercent);
                if (broken)
                {
                    GameObject obj = chunks[selectedChunk].map[selectedTile.x, selectedTile.y].front.obj;
                    GameObject dropObj = Instantiate(chunks[selectedChunk].map[selectedTile.x, selectedTile.y].front.drobObj);
                    dropObj.transform.position = obj.transform.position;
                    dropObj.transform.SetParent(chunks[selectedChunk].map[selectedTile.x, selectedTile.y].back.transform);
                    dropObj.GetComponent<SpriteRenderer>().sortingOrder = Mathf.FloorToInt(dropObj.transform.position.y * -100);
                    Destroy(obj);
                    chunks[selectedChunk].map[selectedTile.x, selectedTile.y].front = null;
                    selectedChunk = Vector2Int.down;
                    breakingTile = false;
                    breakInfo.gameObject.SetActive(false);
                }
            }
            else if (selectedChunk != Vector2Int.down)
            {
                chunks[selectedChunk].map[selectedTile.x, selectedTile.y].front.ResetTick();
                selectedChunk = Vector2Int.down;
                breakInfo.gameObject.SetActive(false);
            }
        };
    }
    void Update()
    {
        Vector2 vector2 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int chunkPos = new Vector2Int(Mathf.FloorToInt((vector2.x + 5) / 10), Mathf.FloorToInt((vector2.y + 5) / 10));
        Vector2Int tilePos = new Vector2Int(Mathf.FloorToInt(vector2.x + 5) % 10, Mathf.FloorToInt(vector2.y + 5) % 10);
        if (prevChunk != null)
        {
            chunks[prevChunk].map[prevTile.x, prevTile.y].back.GetComponent<SpriteRenderer>().material.SetInt("_Alpha", 1);
            if (chunkPos.x < 5 && chunkPos.y < 5 && chunkPos.x > -1 && chunkPos.y > -1 && tilePos.x > -1 && !IsMouseOverUI())
            {
                GameObject tile = chunks[chunkPos].map[tilePos.x, tilePos.y].back;
                tile.GetComponent<SpriteRenderer>().material.SetInt("_Alpha", 0);
                prevChunk = chunkPos;
                prevTile = tilePos;
                if (Input.GetMouseButtonDown(0))
                {
                    if (chunks[chunkPos].map[tilePos.x, tilePos.y].front != null)
                    {
                        selectedChunk = chunkPos;
                        selectedTile = tilePos;
                        Equipment hand = inventory.GetSelected as Equipment;
                        if (hand != null && hand.toolType == chunks[chunkPos].map[tilePos.x, tilePos.y].front.toolType)
                        {
                            breakingTile = true;
                        }
                        else if (ToolType.HAND == chunks[chunkPos].map[tilePos.x, tilePos.y].front.toolType)
                        {
                            breakingTile = true;
                        }
                        breakInfo.value = 0;
                        breakInfo.gameObject.SetActive(true);
                    }
                }
                if (Input.GetMouseButtonUp(0) && breakingTile)
                {
                    breakingTile = false;
                }
            }
        }
    }
    private void GenerateNewFarm()
    {
        for(int i = 0; i < farmSize; ++i)
        {
            for(int j = 0; j < farmSize; ++j)
            {
                Chunk actChunk = new Chunk(i, j);
                Vector2Int chunkPos = new Vector2Int(i, j);
                chunks.Add(chunkPos, actChunk);
                offsetX = Random.Range(0, 9999);
                offsetY = Random.Range(0, 9999);
                for(int x = -5; x < 5; ++x)
                {
                    for(int y = -5; y < 5; ++y)
                    {
                        int indX = (x + 5) % 10;
                        int indY = (y + 5) % 10;
                        GameObject obj = new GameObject(indX + " " + indY);
                        obj.transform.SetParent(actChunk.chunkObj.transform);
                        obj.transform.localPosition = new Vector2(x + 0.5f, y + 0.5f);
                        SpriteRenderer spRend = obj.AddComponent<SpriteRenderer>();
                        spRend.material = bacgroundMaterial;
                        spRend.sprite = backgroundSprites[0];
                        actChunk.map[indX, indY] = new TileSlot(obj);

                        float noise = CalculateNoise(x, y);
                        if(noise >= 0.55f && noise < 0.6f)
                        {
                            //tree
                            GameObject tree = Instantiate(Resources.Load("Prefabs/tree", typeof(GameObject))) as GameObject;
                            tree.transform.SetParent(obj.transform);
                            tree.transform.localPosition = new Vector3(0, 2f);
                            tree.GetComponent<SpriteRenderer>().sortingOrder = Mathf.FloorToInt(obj.transform.position.y * -100);
                            actChunk.map[indX, indY].front = new Tile(tree, true, 12, ToolType.AXE);
                            actChunk.map[indX, indY].front.dropNumber = 10;
                            //actChunk.map[indX, indY].front.itemId = 1;
                        }
                        else if(noise >= 0.2f && noise < 0.29f)
                        {
                            //rock
                            GameObject rock = Instantiate(Resources.Load("Prefabs/rock", typeof(GameObject))) as GameObject;
                            rock.transform.SetParent(obj.transform);
                            rock.transform.localPosition = new Vector3(0, 0);
                            rock.GetComponent<SpriteRenderer>().sortingOrder = Mathf.FloorToInt(obj.transform.position.y * -100);
                            actChunk.map[indX, indY].front = new Tile(rock, true, 15, ToolType.PICKAXE);
                            actChunk.map[indX, indY].front.dropNumber = 2;
                            actChunk.map[indX, indY].front.drobObj = Resources.Load("Prefabs/stone", typeof(GameObject)) as GameObject;
                            actChunk.map[indX, indY].front.drobObj.GetComponent<ItemPickUp>().LoadItemDrop(2);
                        }
                    }
                }
            }
        }
    }
    private float CalculateNoise(int x, int y)
    {
        float xCoord =  (float)x / (Chunk.chunkSize * 10) * noiseScale + offsetX;
        float yCoord = (float)y / (Chunk.chunkSize * 10) * noiseScale + offsetY;

        return Mathf.PerlinNoise(xCoord, yCoord);
    }
    private bool IsMouseOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

}