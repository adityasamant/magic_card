using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerrainScanning
{
    public class HexTileMapGenerator : MonoBehaviour
    {
        #region Public Variable
        /// <summary>
        /// Store the hex prefab you want to use
        /// </summary>
        [Tooltip("The HexTile Prefab you want to use.")]
        public GameObject hexTilePrefab;

        /// <summary>
        /// The parent transform you want to create the hex map.
        /// </summary>
        [Tooltip("The parent transform you want to create a hex map")]
        public GameObject parent;

        #endregion

        #region Private Variable
        /// <summary>
        /// The plane that we can generate hex on.
        /// Generate by PlaneGenerator during gameplay.
        /// </summary>
        private GameObject quadObject;
        #endregion

        public void createHex()
        {
            quadObject = GameObject.Find("Quad(Clone)");
            //Debug.Log("I am a " + quadObject.name);
            Renderer rend = quadObject.GetComponent<Renderer>();
            Debug.Log(rend.bounds.max);
            Debug.Log(rend.bounds.min);
            int x = 0;
            int y = 0;
            int z = 0;
            int x0 = 0;
            int y0 = 0;
            int z0 = 0;
            int cellID = 0;
            for (float i = rend.bounds.min.x + 0.475f; i <= rend.bounds.max.x - 0.475f; i = i + 0.97f)
            {
                x = x0;
                y = y0;
                z = z0;
                for (float j = rend.bounds.max.z + 0.28f; j >= rend.bounds.min.z - 0.28f; j = j - 0.56f)
                {
                    Vector3 center = new Vector3(i, 0.4f, j);
                    Collider[] hitColliders = Physics.OverlapSphere(center, 0.3f);
                    if (hitColliders.Length == 0)
                    {
                        // GameObject temp = Instantiate(hexTilePrefab);
                        // temp.name = "HexTile [" + x.ToString() + "," + y.ToString() + "," + z.ToString() + "]";
                        // temp.transform.position = new Vector3(i, quadObject.transform.position.y+0.1f, j);
                        // temp.transform.parent = parent.transform;

                        GameObject tile = Instantiate(hexTilePrefab);
                        tile.AddComponent<HexTile>();
                        tile.transform.position = new Vector3(i, quadObject.transform.position.y + 0.1f, j);
                        tile.transform.parent = parent.transform;
                        tile.GetComponent<HexTile>().setCoordinates(x, z);
                        tile.GetComponent<HexTile>().setID(cellID++);
                        tile.name = "HexTile" + tile.GetComponent<HexTile>().getID() + " [" + x.ToString() + "," + y.ToString() + "," + z.ToString() + "]";
                    }
                    Vector3 center1 = new Vector3(i + 0.485f, 0.4f, j - 0.28f);
                    Collider[] hitColliders1 = Physics.OverlapSphere(center1, 0.3f);
                    if (hitColliders1.Length == 0)
                    {
                        // GameObject temp1 = Instantiate(hexTilePrefab);
                        // temp1.name = "HexTile [" + (x + 1).ToString() + "," + (y - 1).ToString() + "," + z.ToString() + "]";
                        // temp1.transform.position = new Vector3(i + 0.485f, quadObject.transform.position.y+0.1f, j - 0.28f);
                        // temp1.transform.parent = parent.transform;

                        GameObject tile1 = Instantiate(hexTilePrefab);
                        tile1.AddComponent<HexTile>();
                        tile1.transform.position = new Vector3(i + 0.485f, quadObject.transform.position.y + 0.1f, j - 0.28f);
                        tile1.transform.parent = parent.transform;
                        tile1.GetComponent<HexTile>().setCoordinates(x + 1, z);
                        tile1.GetComponent<HexTile>().setID(cellID++);
                        tile1.name = "HexTile" + tile1.GetComponent<HexTile>().getID() + " [" + (x + 1).ToString() + "," + (y - 1).ToString() + "," + z.ToString() + "]";
                    }
                    y = y - 1;
                    z = z + 1;
                }
                x0 = x0 + 2;
                y0 = y0 - 1;
                z0 = z0 - 1;
            }
        }

    }
}
