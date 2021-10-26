using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGeneratorController : MonoBehaviour {
    private const float debugLineHeight = 10.0f;

    [Header("Templates")]
    public List<TerrainTemplateController> terrainTemplates;
    public float terrainTemplateWidth;

    [Header("Generator Area")]
    public Camera gameCamera;
    public float areaStartOffset;
    public float areaEndOffset;

    public float yOffset;

    private List<GameObject> spawnedTerrain;
    private float lastGeneratedPositionX;
    private float lastRemovedPositionX;

    [Header("Force Early Template")]
    public List<TerrainTemplateController> earlyTerrainTemplates;

    private Dictionary<string, List<GameObject>> pool;

    // Start is called before the first frame update
    void Start() {
        pool = new Dictionary<string, List<GameObject>>();
        spawnedTerrain = new List<GameObject>();

        lastGeneratedPositionX = GetHorizontalPositionStart();
        lastRemovedPositionX = lastGeneratedPositionX - terrainTemplateWidth;

        foreach (TerrainTemplateController terrain in earlyTerrainTemplates) {
            GenerateTerrain(lastGeneratedPositionX, terrain);
            lastGeneratedPositionX += terrainTemplateWidth;
        }

        while (lastGeneratedPositionX < GetHorizontalPositionEnd()) {
            GenerateTerrain(lastGeneratedPositionX);
            lastGeneratedPositionX += terrainTemplateWidth;
        }
    }

    // Update is called once per frame
    void Update() {
        while (lastGeneratedPositionX < GetHorizontalPositionEnd()) {
            GenerateTerrain(lastGeneratedPositionX);
            lastGeneratedPositionX += terrainTemplateWidth;
        }

        while (lastRemovedPositionX + terrainTemplateWidth < GetHorizontalPositionStart()) {
            lastRemovedPositionX += terrainTemplateWidth;
            RemoveTerrain(lastRemovedPositionX);
        }
    }

    private void RemoveTerrain(float posX) {
        GameObject terrainToRemove = null;

        foreach (GameObject item in spawnedTerrain) {
            if (item.transform.position.x == posX) {
                terrainToRemove = item;
                break;
            }
        }

        if (terrainToRemove != null) {
            spawnedTerrain.Remove(terrainToRemove);
            Destroy(terrainToRemove);
        }
    }

    private float GetHorizontalPositionStart() {
        return gameCamera.ViewportToWorldPoint(new Vector2(0f, 0f)).x + areaStartOffset;
    }

    private float GetHorizontalPositionEnd() {
        return gameCamera.ViewportToWorldPoint(new Vector2(1f, 0f)).x + areaEndOffset;
    }

    // debug
    private void OnDrawGizmos() {
        Debug.DrawLine(transform.position + Vector3.up * debugLineHeight / 2, transform.position + Vector3.down * debugLineHeight / 2, Color.green);

        Vector3 areaStartPosition = transform.position;
        Vector3 areaEndPosition = transform.position;

        areaStartPosition.x = GetHorizontalPositionStart();
        areaEndPosition.x = GetHorizontalPositionEnd();

        Debug.DrawLine(areaStartPosition + Vector3.up * debugLineHeight / 2, areaStartPosition + Vector3.down * debugLineHeight / 2, Color.red);
        Debug.DrawLine(areaEndPosition + Vector3.up * debugLineHeight / 2, areaEndPosition + Vector3.down * debugLineHeight / 2, Color.red);
    }

    private void GenerateTerrain(float posX, TerrainTemplateController forceTerrain = null) {
        GameObject newTerrain = null;

        if (forceTerrain == null) {
            newTerrain = GenerateFromPool(terrainTemplates[Random.Range(0, terrainTemplates.Count)].gameObject, transform);
        }
        else {
            newTerrain = GenerateFromPool(forceTerrain.gameObject, transform);
        }

        newTerrain.transform.position = new Vector2(posX, yOffset);
        spawnedTerrain.Add(newTerrain);
    }

    private GameObject GenerateFromPool(GameObject item, Transform parent) {
        if (pool.ContainsKey(item.name)) {
            // if item available in pool
            if (pool[item.name].Count > 0) {
                GameObject newItemFromPool = pool[item.name][0];
                pool[item.name].Remove(newItemFromPool);
                newItemFromPool.SetActive(true);
                return newItemFromPool;
            }
        }
        else {
            // if item list not defined, create new one
            pool.Add(item.name, new List<GameObject>());
        }

        // create new one if no item available in pool
        GameObject newItem = Instantiate(item, parent);
        newItem.name = item.name;
        return newItem;
    }

    private void ReturnToPool(GameObject item) {
        if (!pool.ContainsKey(item.name)) {
            Debug.LogError("INVALID POOL ITEM!!");
        }

        pool[item.name].Add(item);
        item.SetActive(false);
    }
}
