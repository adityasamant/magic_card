using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

public class Controller : MonoBehaviour
{
    public Menu menu;

    // Start is called before the first frame update
    void Start()
    {
        MLInput.Start();
        MLInput.OnControllerButtonDown += OnButtonDown;
    }

    void OnDestroy()
    {
        MLInput.Stop();
    }

    private void OnButtonDown(byte controller_id, MLInputControllerButton button)
    {
        if (button == MLInputControllerButton.Bumper)
        {
            if (menu != null)
            {
                menu.StartServer();
            }
        }
        else if(button==MLInputControllerButton.HomeTap)
        {
            if(menu!=null)
            {
                menu.StartClient();
            }
        }
    }
    
}
