using UnityEngine;

namespace TerrainScanning
{
    public class HexTile : MonoBehaviour
    {
        private int x;
        private int y;
        private int z;
        private int id;
        public bool isStart = false;
        public bool isEnd = false;
        public void setCoordinates(int X, int Z)
        {
            this.x = X;
            this.z = Z;
            this.y = 0 - X - Z;
        }
        public int getX()
        {
            return this.x;
        }
        public int getY()
        {
            return this.y;
        }
        public int getZ()
        {
            return this.z;
        }
        public int[] getCoordinates(){
            return new int[]{x,y,z};
        }
        public void setID(int temp)
        {
            this.id = temp;
        }
        public int getID()
        {
            return this.id;
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            HexTile tile = obj as HexTile;
            return this.x == tile.getX() && this.y == tile.getY();
        }
        
        // override object.GetHashCode
        public override int GetHashCode()
        {
            // TODO: write your implementation of GetHashCode() here
            throw new System.NotImplementedException();
            return base.GetHashCode();
        }
    }
}
