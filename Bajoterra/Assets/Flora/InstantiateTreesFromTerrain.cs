using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateTreesFromTerrain : MonoBehaviour
{
    public Terrain terrain; // Asigna tu terreno en el Inspector

    [System.Serializable]
    public class TreeVariantGroup
    {
        public GameObject[] variants;
    }
    public TreeVariantGroup[] treeVariants; // Variantes de árboles

    void Start()
    {
        if (terrain == null)
        {
            Debug.LogError("El terreno no está asignado. Por favor, asígnalo en el Inspector.");
            return;
        }

        // Validar cada grupo de variantes
        foreach (var variantGroup in treeVariants)
        {
            if (variantGroup.variants == null || variantGroup.variants.Length == 0)
            {
                Debug.LogWarning("Uno de los grupos de variantes está vacío. Es posible que no se instancien árboles para este tipo.");
            }
        }

        TreeInstance[] trees = terrain.terrainData.treeInstances;
        if (trees == null || trees.Length == 0)
        {
            Debug.LogWarning("El terreno no contiene árboles.");
            return;
        }

        foreach (TreeInstance treeInstance in trees)
        {
            Vector3 worldPosition = Vector3.Scale(treeInstance.position, terrain.terrainData.size) + terrain.transform.position;

            int prototypeIndex = treeInstance.prototypeIndex;
            if (prototypeIndex >= 0 && prototypeIndex < treeVariants.Length)
            {
                GameObject[] variants = treeVariants[prototypeIndex].variants;
                if (variants != null && variants.Length > 0)
                {
                    GameObject randomVariant = variants[Random.Range(0, variants.Length)];
                    Quaternion randomRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
                    GameObject tree = Instantiate(randomVariant, worldPosition, randomRotation);
                    tree.transform.localScale = new Vector3(treeInstance.widthScale, treeInstance.heightScale, treeInstance.widthScale);
                }
                else
                {
                    Debug.LogWarning("No hay variantes configuradas para el índice de prototipo: " + prototypeIndex);
                }
            }
        }
    }
}
