﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerrainScanning
{
    public class AutoHexTileMapGenerator : MonoBehaviour
    {
        #region Public Variable
        /// <summary>
        /// Store the hex prefab you want to use
        /// </summary>
        [Tooltip("The HexTile Prefab you want to use.")]
        public GameObject hexTileTemplate;

        /// <summary>
        /// The parent transform you want to create the hex map.
        /// </summary>
        [Tooltip("The parent transform you want to create a hex map")]
        public GameObject parent;
        #endregion

        public float hexScale;
        public float hexRange;
        public GameObject RaycastHead;

        #region Private Variable
        /// <summary>
        /// The plane that we can generate hex on.
        /// Generate by PlaneGenerator during gameplay.
        /// </summary>
        private GameObject quadObject;
        #endregion

        private void addHex(float x, float y, float z, int coordx, int coordy, int coordz, int cellID, HexStatus status, HexType type)
        {
            GameObject tile = Instantiate(hexTileTemplate);
            tile.transform.position = new Vector3(x, y, z);
            tile.transform.parent = parent.transform;
            tile.transform.localScale = new Vector3(hexScale, hexScale, hexScale);
            //tile.AddComponent<HexTile>();
            tile.GetComponent<HexTile>().setStatus(status);
            tile.GetComponent<HexTile>().setType(type);
            tile.GetComponent<HexTile>().setCoordinates(coordx, coordy);
            tile.GetComponent<HexTile>().setID(cellID);
            tile.GetComponent<HexTile>().setAccessible(true);
            tile.name = "HexTile" + tile.GetComponent<HexTile>().getID() + " [" + coordx.ToString() + "," + coordy.ToString() + "," + coordz.ToString() + "]"; ;
        }

        public void createHex()
        {
            Vector3 RayHitPosition = RaycastHead.transform.GetChild(0).transform.position;
            quadObject = GameObject.Find("Quad(Clone)");
            Debug.Log("I am a " + quadObject.name);
            Renderer rend = quadObject.GetComponent<Renderer>();
            Debug.Log(rend.bounds.max);
            Debug.Log(rend.bounds.min);
            //int x = 0;
            //int y = 0;
            //int z = 0;
            //int x0 = 0;
            //int y0 = 0;
            //int z0 = 0;
            int cellID = 0;

            float height = quadObject.transform.position.y;
            quadObject.transform.position = new Vector3(quadObject.transform.position.x, height - 0.11f, quadObject.transform.position.z);
            Physics.SyncTransforms();

            Debug.LogFormat("{0}, {1}, {2}", rend.bounds.min.x, rend.bounds.min.y, rend.bounds.min.z);

            Vector3 cent = (rend.bounds.min + rend.bounds.max) / 2;

            hexTileTemplate.GetComponent<HexTile>().setCoordinates(-1000, -1000);

            for (int coordz = -3; coordz <= 3; coordz++)
                for (int coordx = -3; coordx <= 3 ; coordx++)
                {
                    int coordy = -coordx - coordz;
                    if (coordy < -3 || coordy > 3) continue;
                    float x = hexScale * 1.5f * coordz;
                    float z = hexScale * 1.732f * (coordx + coordz / 2f);
                    addHex(RayHitPosition.x+x, RayHitPosition.y+ 0.1f, RayHitPosition.z+z, coordx, coordy, coordz, cellID++, HexStatus.Normal, (HexType)Random.Range(1, 4));
                }

            //for (float i = rend.bounds.min.x + 0.5f; i <= rend.bounds.max.x - 0.5f; i = i + 0.9f)
            //{
            //    for (float j = rend.bounds.max.z + 0.3f; j >= rend.bounds.min.z - 0.3f; j = j - 0.52f)
            //    {
            //        Vector3 center = new Vector3(i, height + 0.4f, j);
            //        Collider[] hitColliders = Physics.OverlapSphere(center, 0.3f);
            //        Vector3 centertest = new Vector3(i, height - 0.4f, j);
            //        Collider[] hitColliderstest = Physics.OverlapSphere(centertest, 0.3f);
            //        //if (hitColliders.Length == 0 && hitColliderstest.Length == 1)
            //        addHex(i, height + 0.1f, j, x, y, z, cellID++, HexStatus.Normal, (HexType)Random.Range(1, 4));
            //        Vector3 center1 = new Vector3(i + 0.45f, height + 0.4f, j - 0.26f);
            //        Collider[] hitColliders1 = Physics.OverlapSphere(center1, 0.3f);
            //        Vector3 centertest1 = new Vector3(i, height - 0.4f, j);
            //        Collider[] hitColliderstest1 = Physics.OverlapSphere(centertest1, 0.3f);
            //        //if (hitColliders1.Length == 0 && hitColliderstest1.Length == 1)
            //        addHex(i + 0.45f, height + 0.1f, j - 0.26f, x + 1, y - 1, z, cellID++, HexStatus.Normal, (HexType)Random.Range(1, 4));
            //        y = y - 1;
            //        z = z + 1;
            //    }
            //    x0 = x0 + 2;
            //    y0 = y0 - 1;
            //    z0 = z0 - 1;
            //}
            //Destroy(quadObject);
        }
    }
}
