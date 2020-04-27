using Dissonance.Editor;
using UnityEditor;

namespace Dissonance.Integrations.PhotonBolt.Editor
{
    [CustomEditor(typeof(BoltCommsNetwork))]
    public class BoltCommsNetworkEditor
        : BaseDissonnanceCommsNetworkEditor<BoltCommsNetwork, BoltServer, BoltClient, BoltPeer, Unit, Unit>
    {
    }
}