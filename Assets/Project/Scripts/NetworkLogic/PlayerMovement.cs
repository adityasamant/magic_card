using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : Bolt.EntityBehaviour<ICubeState>
{
    //void Start()
    public override void Attached()
    {
        base.Attached();
        state.SetTransforms(state.CubeTransform, transform);
    }

    // void Update()
    public override void SimulateOwner()
    {
        base.SimulateOwner();
        var speed = 4f;
        var movement = new Vector3(Random.value - 0.5f, Random.value - 0.5f, Random.value - 0.5f);
        if(movement!=Vector3.zero)
        {
            transform.position += movement * speed * BoltNetwork.FrameDeltaTime;
        }
    }
}
