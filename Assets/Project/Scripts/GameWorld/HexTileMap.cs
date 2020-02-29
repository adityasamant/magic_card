using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameWorld
{
    public class HexTileMap : MonoBehaviour
    {
        /// <summary>
        /// return all vertexs' id it close to.
        /// </summary>
        /// <param name="vertexId"></param>
        /// <returns></returns>
        public List<int> getEdgeByVertexId(int vertexId)
        {
            return null;
        }
        /// <summary>
        /// return a list of vertex's id
        /// </summary>
        /// <param name="startId"></param>
        /// <param name="endId"></param>
        /// <returns></returns>
        public List<int> getShortestPath(int startId, int endId)
        {
            return null; 
        }

        public int getDistance(int startId, int endId)
        {
            return 0;
        }
    }
}

