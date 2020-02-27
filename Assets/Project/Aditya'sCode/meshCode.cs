using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

public delegate void ScanFinished();

public class meshCode : MonoBehaviour
{
    public HexTileMapGenerator hx;
    public Material BlackMaterial;
    public MLSpatialMapper Mapper;
    public GameObject PlaneGenerator;
    public GameObject TileGenerator;
    public GameObject MLSpatialMapper;

    /// <summary>
    /// Define the object for delegate
    /// </summary>
    public ScanFinished ScanFinished;

    #region Private Variable
    private MLInputController _controller;

    /// <summary>
    /// A bool variable to control the system only generate once
    /// </summary>
    private bool AlreadyGenerate=false;
    #endregion

    void Start () {
      //Start receiving input by the Control
      MLInput.Start();
      MLInput.OnControllerButtonDown += OnButtonDown;
      _controller = MLInput.GetController(MLInput.Hand.Left);
    }
    void OnDestroy () {
      //Stop receiving input by the Control
      MLInput.Stop();
    }
    void Update () {
    }

    void OnButtonDown(byte controller_id, MLInputControllerButton button) {
      if (button == MLInputControllerButton.Bumper) {
            if (AlreadyGenerate) return;
            hx.createHex();
            AlreadyGenerate = true;
            ScanFinished();
            Mapper.DestroyAllMeshes();
            Destroy(Mapper);
            Destroy(PlaneGenerator);
            Destroy(TileGenerator);
            Destroy(MLSpatialMapper);
            Destroy(gameObject);
      }
    }
}
