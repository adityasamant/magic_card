using Bolt;
using UnityEngine;
using System;

namespace Dissonance.Integrations.PhotonBolt
{
    public abstract class BoltPlayer<TState> : EntityBehaviour<TState>, IDissonancePlayer
        where TState : IState
    {
        private static readonly Log Log = Logs.Create(LogCategory.Network, "Bolt Player Component");

        private DissonanceComms _comms;

        private readonly string _idPropertyName;
        private readonly Func<TState, string> _getPlayerId;
        private readonly Action<TState, string> _setPlayerId;
        
        public bool IsTracking { get; private set; }

        public string PlayerId { get; private set; }

        public Vector3 Position
        {
            get { return transform.position; }
        }

        public Quaternion Rotation
        {
            get { return transform.rotation; }
        }

        public NetworkPlayerType Type
        {
            get { return !entity.IsAttached || entity.IsOwner ? NetworkPlayerType.Local : NetworkPlayerType.Remote; }
        }

        protected BoltPlayer(string idPropertyName, Func<TState, string> get, Action<TState, string> set)
        {
            _idPropertyName = idPropertyName;
            _getPlayerId = get;
            _setPlayerId = set;
        }
        
        public override void Attached()
        {
            _comms = FindObjectOfType<DissonanceComms>();
            if (_comms == null)
            {
                throw Log.CreateUserErrorException(
                    "cannot find DissonanceComms component in scene",
                    "not placing a DissonanceComms component on a game object in the scene",
                    "https://dissonance.readthedocs.io/en/latest/Basics/Quick-Start-Photon/",
                    "00077AC8-3CBF-4DD8-A1C7-3ED3E8F64914");
            }
            
            state.AddCallback(_idPropertyName, IdChanged);

            if (entity.IsOwner)
            {
                Log.Trace("Initializing local bolt player");

                // This method is called on the client which has control authority over this object. This will be the local client of whichever player we are tracking.
                if (_comms.LocalPlayerName != null)
                    SetPlayerId(_comms.LocalPlayerName);

                //Subscribe to future name changes (this is critical because we may not have run the initial set name yet and this will trigger that initial call)
                _comms.LocalPlayerNameChanged += SetPlayerId;
            }
        }
        
        public void OnDestroy()
        {
            if (_comms != null)
                _comms.LocalPlayerNameChanged -= SetPlayerId;
        }

        public void OnEnable()
        {
            if (!IsTracking)
                StartTracking();
        }

        public void OnDisable()
        {
            if (IsTracking)
                StopTracking();
        }

        private void SetPlayerId(string playerId)
        {
            _setPlayerId(state, playerId);
        }
        
        private void IdChanged()
        {
            //We need to stop and restart tracking to handle the name change
            if (IsTracking)
                StopTracking();
            
            //Perform the actual work
            PlayerId = _getPlayerId(state);
            StartTracking();
        }
        
        private void StartTracking()
        {
            if (IsTracking)
                throw Log.CreatePossibleBugException("Attempting to start player tracking, but tracking is already started", "0663D808-ACCC-4D13-8913-03F9BA0C8578");

            if (_comms != null)
            {
                _comms.TrackPlayerPosition(this);
                IsTracking = true;
            }
        }

        private void StopTracking()
        {
            if (!IsTracking)
                throw Log.CreatePossibleBugException("Attempting to stop player tracking, but tracking is not started", "48802E32-C840-4C4B-BC58-4DC741464B9A");

            if (_comms != null)
            {   
                _comms.StopTracking(this);
                IsTracking = false;
            }
        }
    }
}
