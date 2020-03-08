using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TerrainScanning;

namespace GameWorld
{
    public class HexTileMap : MonoBehaviour
    {
        private GameObject HexMap;
        private static int[][] directions = new int[][] { new int[] { 1, -1, 0 }, new int[] { 1, 0, -1 }, new int[] { 0, 1, -1 }, new int[] { -1, 1, 0 }, new int[] { -1, 0, 1 }, new int[] { 0, -1, 1 } };
        private List<HexTile> hexTiles = new List<HexTile>();
        /// <summary>
        /// return all vertexs' id it close to.
        /// </summary>
        /// <param name="vertexId"></param>
        /// <returns></returns>
        void Start()
        {
            HexMap = GameObject.Find("HexMap");
        }
        public List<HexTile> getEdgeByVertexId(int vertexId)
        {
            List<HexTile> edges = new List<HexTile>();
            HexTile curr = hexTiles[vertexId];
            int[] currCoord = curr.getCoordinates();
            foreach (int[] dirt in directions)
            {
                int[] targetCoord = new int[] { currCoord[0] + dirt[0], currCoord[1] + dirt[1], currCoord[2] + dirt[2] };
                if (isValid(targetCoord))
                {
                    edges.Add(getHexTileByCoord(targetCoord));
                }
                else
                {
                    //Debug.Log("noValid");
                }
            }
            return edges;
        }

        /// <summary>
        /// return a list of vertex's id
        /// </summary>
        /// <param name="startId"></param>
        /// <param name="endId"></param>
        /// <returns></returns>
        public List<int> getShortestPath(int startId, int endId)
        {
            int i = 0;
            int childCount = HexMap.transform.childCount;
            if (hexTiles.Count != childCount)
            {
                while (i < childCount)
                {
                    hexTiles.Add(HexMap.transform.GetChild(i).GetComponent<HexTile>());
                    i++;
                }
                Debug.Log("allNum" + hexTiles.Count);
            }
            List<int> path = new List<int>();
            HexTile endTile = hexTiles[endId];
            path.Add(startId);
            HexTile currNode = hexTiles[startId];
            while (true)
            {
                List<HexTile> allNeighbors = getEdgeByVertexId(currNode.getID());
                List<HexTile> neighbors = new List<HexTile>();
                foreach (HexTile each in allNeighbors)
                {
                    if (!path.Contains(each.getID()))
                    {
                        neighbors.Add(each);
                    }
                }
                //Debug.Log("nb#" + allNeighbors.Count);
                if (neighbors.Count == 0) break;
                if (neighbors.Contains(endTile))
                {
                    path.Add(endId);
                    break;
                }

                HexTile nearestNode = FindNearestNode(neighbors, endTile);
                path.Add(nearestNode.getID());
                currNode = nearestNode;
            }
            return path;
        }

        public int getDistance(int startId, int endId)
        {
            HexTile s = HexMap.transform.GetChild(startId).GetComponent<HexTile>();
            HexTile e = HexMap.transform.GetChild(endId).GetComponent<HexTile>();
            return (Math.Abs(s.getX() - e.getX()) + Math.Abs(s.getY() - e.getY()) + Math.Abs(s.getZ() - e.getZ())) / 2;
        }

        //TODO
        public Boolean isValid(int[] coord)
        {
            int numsOfID = hexTiles.Count;
            for (int i = 0; i < numsOfID; i++)
            {
                if (coord[0] == hexTiles[i].getX() && coord[1] == hexTiles[i].getY() && coord[2] == hexTiles[i].getZ())
                {
                    return true;
                }
            }
            return false;
        }

        public HexTile getHexTileByCoord(int[] coord)
        {
            HexTile result = new HexTile();
            foreach (HexTile a in hexTiles)
            {
                if (a.getX()== coord[0] && a.getY() == coord[1] && a.getZ() == coord[2])
                {
                    result = a;
                    return result;
                }
            }
            return result;
        }

        /// <summary>
        /// Return a HexTile by Index, return null if doesn't find
        /// </summary>
        /// <param name="HexIndex">The HexTile Id</param>
        /// <returns>Return a HexTile, if it doesn't find return null</returns>
        public HexTile getHexTileByIndex(int HexIndex)
        {
            HexTile result = null;
            foreach (HexTile a in hexTiles)
            {
                if (a.getID()==HexIndex)
                {
                    result = a;
                    return result;
                }
            }
            return result;
        }

        public HexTile FindNearestNode(List<HexTile> list, HexTile target)
        {
            int minDist = 999999;
            HexTile nearestNode = new HexTile();
            foreach (HexTile nb in list)
            {
                int dist = getDistance(nb.getID(), target.getID());
                if (dist < minDist)
                {
                    minDist = dist;
                    nearestNode = nb;
                }
            }
            return nearestNode;
        }
    }
}

