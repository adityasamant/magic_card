using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TerrainScanning;

namespace GameWorld
{
    public class HexTileMap : MonoBehaviour
    {
        public Material OriginalMat;
        public Material NewMat;

        private GameObject HexMap;
        private static int[][] directions = new int[][] { new int[] { 1, -1, 0 }, new int[] { 1, 0, -1 }, new int[] { 0, 1, -1 }, new int[] { -1, 1, 0 }, new int[] { -1, 0, 1 }, new int[] { 0, -1, 1 } };
        private static Dictionary<Tuple<int, int>, int> coordinatesToId = new Dictionary<Tuple<int,int>, int>();
        void Start()
        {
            HexMap = GameObject.Find("HexMap");
        }

        /// <summary>
        /// return a list of vertex's id
        /// </summary>
        /// <param name="startId"></param>
        /// <param name="endId"></param>
        /// <returns></returns>
        public List<int> getShortestPath(int startId, int endId)
        {
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

                foreach(int neighborId in getEdgeByVertexId(curr,endId)){
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
            //ColorPath(path);
            return path;
        }

        /// <summary>
        /// return all vertexs' id it close to.
        /// </summary>
        /// <param name="vertexId"></param>
        /// <returns></returns>
        public List<int> getEdgeByVertexId(int vertexId,int EndId)
        {
            //init coordinatesToId Dictionary
            int i = 0;
            int childCount = HexMap.transform.childCount;
            if (coordinatesToId.Count != childCount)
            {
                while (i < childCount)
                {
                    var key = Tuple.Create(HexMap.transform.GetChild(i).GetComponent<HexTile>().getX(), HexMap.transform.GetChild(i).GetComponent<HexTile>().getZ());
                    coordinatesToId.Add(key, i);
                    i++;
                }
            }

            List<int> edges = new List<int>();
            int[] currCoord = HexMap.transform.GetChild(vertexId).GetComponent<HexTile>().getCoordinates();
            foreach (int[] dirt in directions)
            {
                int[] targetCoord = new int[] { currCoord[0] + dirt[0], currCoord[1] + dirt[1], currCoord[2] + dirt[2] };
                var targetKey = Tuple.Create(targetCoord[0], targetCoord[2]);
                if (coordinatesToId.ContainsKey(targetKey) && isAccessible(targetCoord) || coordinatesToId.ContainsKey(targetKey) && coordinatesToId[targetKey]==EndId)
                {
                    edges.Add(coordinatesToId[targetKey]);
                }
            }
            return edges;
        }

        public int getDistance(int startId, int endId)
        {
            HexTile s = HexMap.transform.GetChild(startId).GetComponent<HexTile>();
            HexTile e = HexMap.transform.GetChild(endId).GetComponent<HexTile>();
            return (Math.Abs(s.getX() - e.getX()) + Math.Abs(s.getY() - e.getY()) + Math.Abs(s.getZ() - e.getZ())) / 2;
        }

        /// <summary>
        /// Return a HexTile by Index, return null if doesn't find
        /// </summary>
        /// <param name="HexIndex">The HexTile Id</param>
        /// <returns>Return a HexTile, if it doesn't find return null</returns>
        public HexTile getHexTileByIndex(int HexIndex)
        {
            if (HexMap.transform.childCount < HexIndex) return null;
            HexTile result = HexMap.transform.GetChild(HexIndex).GetComponent<HexTile>();
            return result;
        }

        /// <summary>
        /// return coordinates is accesible or not
        /// </summary>
        /// <param name="coord"></param>
        //TODO: Add more constrains
        public Boolean isAccessible(int[] coord)
        {
            var key = Tuple.Create(coord[0], coord[2]);
            int id = coordinatesToId[key];
            return HexMap.transform.GetChild(id).GetComponent<HexTile>().getAccessible();
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
            // for (int i = 0; i < path.Count; i++)
            // {
            //     HexMap.transform.GetChild(path[i]).GetChild(0).GetComponent<Renderer>().material = NewMat;
            // }
        }
    }
}

