using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnPressed : MonoBehaviour
{


    private void OnMouseDown()
    {
        Debug.Log("Pressed!" + gameObject.GetComponentInParent<ParticleFollower>().hash_id);

        if(CameraCtrl.canMove)
        GameObject.FindGameObjectWithTag("Loader").GetComponent<LoadPoolUI>().SearchPool(gameObject.GetComponentInParent<ParticleFollower>().hash_id);
    }

    public void Launch()
    {
        Debug.Log("Pressed!" + int.Parse(gameObject.name));

        GameObject.FindGameObjectWithTag("Loader").GetComponent<LoadPoolUI>().SearchPool(int.Parse(gameObject.name));

    }
}
