using UnityEngine;

namespace TerrainScanning
{
    public enum HexStatus
    {
        Normal,
        File,
        BlackHole,
    }
    public enum HexType
    {
        Empty,
        Ground,
        Grass,
        Mud
    }

    public class HexTile : MonoBehaviour
    {
        public GameObject mesh;
        public Material[] HexMats;

        private int x;
        private int y;
        private int z;
        private int id;
        private bool accessible;
        private HexStatus hexStatus = HexStatus.Normal;
        private HexType hexType = HexType.Empty;

        private void Update()
        {
            if(this.transform.childCount>1)
            {
                setAccessible(false);
            }
            else
            {
                setAccessible(true);
            }
        }

        public HexTile(int X, int Z, int myID){
            this.x = X;
            this.z = Z;
            this.y = 0 - X - Z;
            this.id = myID;
            this.accessible = true;
        }
        public void setCoordinates(int X, int Z)
        {
            this.x = X;
            this.z = Z;
            this.y = 0 - X - Z;
        }
        public int[] getCoordinates()
        {
            return new int[] { x, y, z };
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
        public void setID(int temp)
        {
            this.id = temp;
        }
        public int getID()
        {
            return this.id;
        }
        public void setAccessible(bool b){
            this.accessible = b;
        }
        public bool getAccessible(){
            return this.accessible;
        }
        public void setStatus(HexStatus status)
        {
            hexStatus = status;
            // TODO: operations when status is changed
        }
        public void setType(HexType type)
        {
            hexType = type;
            if(mesh != null)
            {
                Renderer rend = mesh.GetComponent<Renderer>();
                switch (type)
                {
                    case HexType.Ground:
                        rend.sharedMaterial = HexMats[1];
                        break;
                    case HexType.Grass:
                        rend.sharedMaterial = HexMats[2];
                        break;
                    case HexType.Mud:
                        rend.sharedMaterial = HexMats[3];
                        break;
                    default:
                        rend.sharedMaterial = HexMats[0];
                        break;
                }
            }
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
