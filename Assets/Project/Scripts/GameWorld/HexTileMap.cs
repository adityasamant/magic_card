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
        private static List<HexTile> hexTiles = new List<HexTile>();
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
                Debug.Log("num of Tiles" + hexTiles.Count);
            }
            List<int> path = new List<int>();
            Dictionary<int, int> cameFrom = new Dictionary<int, int>();
            Dictionary<int, int> costSoFar = new Dictionary<int, int>();
            var frontier = new PriorityQueue<int>();
            
            frontier.Enqueue(startId, 0);
            cameFrom.Add(startId, startId);
            costSoFar.Add(startId, 0);

            while (frontier.Count > 0)
            {
                int curr = frontier.Dequeue();

                if(curr == endId) break;

                foreach(HexTile neighbor in getEdgeByVertexId(curr)){
                    int neighborId = neighbor.getID();
                    int newCost = costSoFar[curr] + getDistance(curr, neighborId);
                    if(!costSoFar.ContainsKey(neighborId) || newCost < costSoFar[neighborId]){
                        if(costSoFar.ContainsKey(neighborId)){
                            costSoFar.Remove(neighborId);
                            cameFrom.Remove(neighborId);
                        }

                        costSoFar.Add(neighborId, newCost);
                        cameFrom.Add(neighborId, curr);
                        int priority = newCost + getDistance(neighborId, endId);
                        frontier.Enqueue(neighborId, priority);
                    }
                }
            }

            int currIdInPath = endId;
            while(currIdInPath != startId){
                if(!cameFrom.ContainsKey(currIdInPath)){
                    Debug.Log("No way!");
                    return new List<int>();
                }
                path.Add(currIdInPath);
                currIdInPath = cameFrom[currIdInPath];
            }
            path.Add(startId);
            path.Reverse();
            //Show path in blue
            ColorPath(path);
            return path;
        }

        public int getDistance(int startId, int endId)
        {
            HexTile s = HexMap.transform.GetChild(startId).GetComponent<HexTile>();
            HexTile e = HexMap.transform.GetChild(endId).GetComponent<HexTile>();
            return (Math.Abs(s.getX() - e.getX()) + Math.Abs(s.getY() - e.getY()) + Math.Abs(s.getZ() - e.getZ())) / 2;
        }

        //TODO: Add more constrains
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
                if (a.getX() == coord[0] && a.getY() == coord[1] && a.getZ() == coord[2])
                {
                    result = a;
                    return result;
                }
            }
            return result;
        }
        public class PriorityQueue<T>
        {
            private List<KeyValuePair<T, float>> elements = new List<KeyValuePair<T, float>>();

            public int Count
            {
                get { return elements.Count; }
            }

            public void Enqueue(T item, float priority)
            {
                elements.Add(new KeyValuePair<T, float>(item, priority));
            }

            // Returns the Location that has the lowest priority
            public T Dequeue()
            {
                int bestIndex = 0;

                for (int i = 0; i < elements.Count; i++)
                {
                    if (elements[i].Value < elements[bestIndex].Value)
                    {
                        bestIndex = i;
                    }
                }

                T bestItem = elements[bestIndex].Key;
                elements.RemoveAt(bestIndex);
                return bestItem;
            }
        }

        public void ColorPath(List<int> path)
        {
            for (int i = 0; i < path.Count; i++)
            {
                HexMap.transform.GetChild(path[i]).GetChild(0).GetComponent<Renderer>().material.color = Color.blue;
            }
        }
    }
}

