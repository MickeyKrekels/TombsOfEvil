using System.Collections.Generic;
using UnityEngine;

public class SelectableManager : MonoBehaviour
{
    public static SelectableManager current;
    public List<ISelectable> selectables;

    public List<SelectbleTypes> selectbleTypes = new List<SelectbleTypes>();

    [System.Serializable]
    public class SelectbleTypes
    {
        public string name;
        public int spawnPercentage;
        public List<GameObject> prefabs = new List<GameObject>();
    }

    private void Awake()
    {
        current = this;
    }

    public void Generate()
    {

        selectables = new List<ISelectable>();
        List<Vector3> spawnbleTiles = MapGenerator.current.GetSpawnbleTiles();

        foreach (var selectbleType in selectbleTypes)
        {
            for (int i = 0; i < selectbleType.spawnPercentage; i++)
            {
                foreach (var prefab in selectbleType.prefabs)
                {
                    int random = Random.Range(0, spawnbleTiles.Count);
                    GameObject g = Instantiate(prefab, spawnbleTiles[random], Quaternion.identity);

                    if (g.GetComponent<ISelectable>() == null)
                        continue;

                    ISelectable selectable = g.GetComponent<ISelectable>();
                    selectables.Add(selectable);
                }
            }
        }


    }




}

