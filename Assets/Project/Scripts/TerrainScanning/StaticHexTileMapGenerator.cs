using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerrainScanning
{
    public class StaticHexTileMapGenerator : MonoBehaviour
    {
        #region Public Variable

        /// <summary>
        /// The parent transform you want to create the hex map.
        /// </summary>
        [Tooltip("The parent transform you want to create a hex map")]
        public GameObject parent;


        public float hexScale;
        public int hexRange = 8;
        public GameObject RaycastHead;
        public GameObject FancyGridObject;
        public GameObject hexTileTemplate;
        public GameObject BattleGround;
        public GameObject FantasyHexGameGrid;

        #endregion

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
            tile.transform.parent = parent.transform;
            tile.transform.localPosition = new Vector3(x, y, z);
            tile.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            //tile.AddComponent<HexTile>();
            tile.GetComponent<HexTile>().setStatus(status);
            tile.GetComponent<HexTile>().setType(type);
            tile.GetComponent<HexTile>().setCoordinates(coordx, coordy);
            tile.GetComponent<HexTile>().setID(cellID);
            tile.name = "HexTile" + tile.GetComponent<HexTile>().getID() + " [" + coordx.ToString() + "," + coordy.ToString() + "," + coordz.ToString() + "]"; ;
        }

        public void createHex()
        {
            Vector3 RayHitPosition = RaycastHead.transform.GetChild(0).transform.position;
            BattleGround.transform.position = RayHitPosition+new Vector3(0.0f, 0.1f, 0.0f);
            BattleGround.transform.localScale = new Vector3(hexScale, hexScale, hexScale);
            BattleGround.SetActive(true);
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

            for (int coordx = -hexRange; coordx <= hexRange; coordx++)
                for (int coordz = hexRange; coordz >= -hexRange; coordz--)
                {
                    int coordy = -coordx - coordz;
                    if (coordy < -hexRange || coordy > hexRange) continue;
                    float z = 0.075f * coordy;
                    float x = 0.0866f * (coordx + coordy / 2f);
                    HexStatus status = (FantasyHexGameGrid.transform.GetChild(cellID).GetComponent<Tile>().GetOccupied() ? HexStatus.Blocked : HexStatus.Normal);
                    addHex(x, 0.0f, z, coordx, coordy, coordz, cellID++, status, (HexType)0);
                }
        }
    }
}
