using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BurgerGame : MonoBehaviour
{
    [Header("접시 설정")]
    public Transform plate;
    public float plateMoveSpeed = 5f;
    public float plateMinX = -3f;
    public float plateMaxX = 3f;

    [Header("재료 설정")]
    public GameObject[] ingredientPrefabs;
    public float dropInterval = 1.5f;
    public float dropY = 4f;
    public float dropSpeed = 5f;
    public int totalIngredients = 5;

    [Header("판정 설정")]
    public float catchWidth = 1.2f;
    public float ingredientCatchWidth = 0.8f;
    public float ingredientHeight = 0.3f;
    public float plateOffset = 0.5f;

    private int caughtCount = 0;
    private int spawnedCount = 0;
    private bool gameRunning = false;
    private bool isEnded = false;
    private List<GameObject> activeIngredients = new List<GameObject>();
    private List<GameObject> stackedIngredients = new List<GameObject>();
    private float nextCatchY;

void Start()
{
    if (plate != null)
        nextCatchY = plate.position.y + GetObjectHeight(plate.gameObject) + plateOffset;

    StartCoroutine(StartCountdown());
}

    private float GetObjectHeight(GameObject obj)
    {
        BoxCollider2D col = obj.GetComponent<BoxCollider2D>();
        if (col != null) return col.bounds.size.y * 0.5f;

        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        if (sr != null) return sr.bounds.size.y * 0.5f;

        return ingredientHeight;
    }

    private float GetObjectWidth(GameObject obj)
    {
        BoxCollider2D col = obj.GetComponent<BoxCollider2D>();
        if (col != null) return col.bounds.size.x;

        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        if (sr != null) return sr.bounds.size.x;

        return ingredientCatchWidth;
    }

    private IEnumerator StartCountdown()
    {
        yield return new WaitForSeconds(0.8f);
        gameRunning = true;
        StartCoroutine(SpawnIngredients());
    }

    void Update()
    {
        if (!gameRunning || isEnded) return;

        float input = 0f;
        if (Input.GetKey(KeyCode.LeftArrow)) input = -1f;
        if (Input.GetKey(KeyCode.RightArrow)) input = 1f;

        if (plate != null)
        {
            Vector3 pos = plate.position;
            pos.x += input * plateMoveSpeed * Time.deltaTime;
            pos.x = Mathf.Clamp(pos.x, plateMinX, plateMaxX);
            plate.position = pos;

            foreach (GameObject stacked in stackedIngredients)
            {
                if (stacked == null) continue;
                Vector3 sPos = stacked.transform.position;
                sPos.x = pos.x;
                stacked.transform.position = sPos;
            }
        }

        List<GameObject> toRemove = new List<GameObject>();

        foreach (GameObject ingredient in activeIngredients)
        {
            if (ingredient == null) continue;

            ingredient.transform.position += Vector3.down * dropSpeed * Time.deltaTime;

            float ingX = ingredient.transform.position.x;
            float ingY = ingredient.transform.position.y;

            if (ingY <= nextCatchY)
            {
                float catchX;
                float currentCatchWidth;

                if (stackedIngredients.Count == 0)
                {
                    catchX = plate.position.x;
                    currentCatchWidth = GetObjectWidth(plate.gameObject);
                }
                else
                {
                    GameObject lastStacked = stackedIngredients[stackedIngredients.Count - 1];
                    catchX = lastStacked.transform.position.x;
                    currentCatchWidth = GetObjectWidth(lastStacked);
                }

                float dist = Mathf.Abs(ingX - catchX);

                if (dist <= currentCatchWidth * 0.5f)
                {
                    float stackY = nextCatchY + GetObjectHeight(ingredient) * 0.5f;
                    ingredient.transform.position = new Vector3(plate.position.x, stackY, 0f);

                    stackedIngredients.Add(ingredient);
                    toRemove.Add(ingredient);
                    caughtCount++;

                    nextCatchY += GetObjectHeight(ingredient);

                    if (caughtCount >= totalIngredients)
                    {
                        StartCoroutine(ClearRoutine());
                        return;
                    }
                }
                else
                {
                    toRemove.Add(ingredient);
                    Destroy(ingredient);
                    StartCoroutine(FailRoutine());
                    return;
                }
            }
        }

        foreach (GameObject obj in toRemove)
            activeIngredients.Remove(obj);
    }

    private IEnumerator SpawnIngredients()
    {
        while (spawnedCount < totalIngredients)
        {
            if (!gameRunning) yield break;
            SpawnIngredient();
            spawnedCount++;
            yield return new WaitForSeconds(dropInterval);
        }
    }

    private void SpawnIngredient()
    {
        if (ingredientPrefabs == null || ingredientPrefabs.Length == 0) return;

        int idx = spawnedCount % ingredientPrefabs.Length;
        float randomX = Random.Range(plateMinX, plateMaxX);
        Vector3 spawnPos = new Vector3(randomX, dropY, 0f);

        GameObject ingredient = Instantiate(ingredientPrefabs[idx], spawnPos, Quaternion.identity);
        activeIngredients.Add(ingredient);
    }

    private IEnumerator ClearRoutine()
    {
        isEnded = true;
        gameRunning = false;
        yield return new WaitForSeconds(0.5f);

        if (GameManager.instance != null)
            GameManager.instance.GameClear();
        else
            Debug.Log("GameClear!");
    }

    private IEnumerator FailRoutine()
    {
        isEnded = true;
        gameRunning = false;

        foreach (GameObject obj in activeIngredients)
            if (obj != null) Destroy(obj);
        activeIngredients.Clear();

        yield return new WaitForSeconds(0.5f);

        if (GameManager.instance != null)
            GameManager.instance.GameOver();
        else
            Debug.Log("GameOver!");
    }
}