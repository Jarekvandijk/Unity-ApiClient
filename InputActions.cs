using UnityEngine;
using System.Collections.Generic;

public class InputActions : MonoBehaviour
{
    public GameObject prefab;
    private bool isClicked = false;
    private GameObject selectedObject;
    private List<GameObject> instantiatedObjects = new List<GameObject>();

    private float minX = 136f;
    private float maxX = 1755f;
    private float minY = 136f;
    private float maxY = 674f;

    public void ButtonClick()
    {
        isClicked = true;
    }

    public void CreateInstanceOfPrefab()
    {
        Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        position.z = 0;

        if (position.x < minX || position.x > maxX || position.y < minY || position.y > maxY)
        {
            Debug.Log("Kan object niet buiten het huis plaatsen!");
            return;
        }

        GameObject newObject = Instantiate(prefab, position, Quaternion.identity);

        if (newObject == null)
        {
            Debug.Log("Instantiate() gaf een null-object terug!");
            return;
        }

        instantiatedObjects.Add(newObject);
        Debug.Log($"Nieuw object gecreëerd en toegevoegd aan lijst.");
    }

    public void CreateFloor()
    {
        Vector3 position = new Vector3(925.5801f, 378.1346f, 0);
        GameObject newFloor = Instantiate(prefab, position, Quaternion.identity);

        if (newFloor == null)
        {
            Debug.Log("Instantiate() gaf een null-object terug!");
            return;
        }

        instantiatedObjects.Add(newFloor);
        Debug.Log("Nieuwe vloer gecreëerd en toegevoegd aan lijst.");
    }

    private void Update()
    {
        if (isClicked && Input.GetMouseButtonDown(0))
        {
            CreateInstanceOfPrefab();
            isClicked = false;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider != null)
            {
                selectedObject = hit.collider.gameObject;
                Debug.Log($"Object geselecteerd: {selectedObject.name}");
            }
        }

        if (selectedObject != null)
        {
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                bool removed = instantiatedObjects.Remove(selectedObject);
                if (removed)
                {
                    Debug.Log($"Object succesvol verwijderd uit de lijst.");
                }

                Destroy(selectedObject);
                selectedObject = null;
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                selectedObject.transform.Rotate(0, 0, 90);
                Debug.Log($"Object {selectedObject.name} geroteerd naar {selectedObject.transform.rotation.eulerAngles.z} graden.");
            }
        }

        if (Input.GetKeyDown(KeyCode.Delete))
        {
            ResetScene();
        }
    }

    public void ResetScene()
    {
        if (instantiatedObjects.Count > 0)
        {
            foreach (GameObject obj in instantiatedObjects)
            {
                Debug.Log($"Object verwijderd: {obj.name}");
                Destroy(obj);
            }

            instantiatedObjects.Clear();
            Debug.Log("Alle objecten zijn verwijderd.");
        }
    }
}