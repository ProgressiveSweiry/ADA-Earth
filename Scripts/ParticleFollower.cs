using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ParticleFollower : MonoBehaviour
{

    private ParticleSystem ps;
    private GameObject Name;

    public int hash_id = 0;


    public Camera myCam;

    // Start is called before the first frame update
    void Start()
    {
        ps = GetComponentInChildren<ParticleSystem>();
        Name = GetComponentInChildren<TextMeshPro>().gameObject;

        if (gameObject.GetComponentInChildren<TextMeshPro>().text == "NAME")
        {
            Destroy(this.gameObject);
            Debug.Log("DESTROYED");
        }

        ps.gameObject.SetActive(false);

        Name.SetActive(!ps.gameObject.activeInHierarchy);

    }

    void OnEnable()
    {
        CameraCtrl.OnClicked += TogglePS;
    }


    void OnDisable()
    {
        CameraCtrl.OnClicked -= TogglePS;
    }

    void TogglePS()
    {
        Name.SetActive(ps.gameObject.activeInHierarchy);
        ps.gameObject.SetActive(!ps.gameObject.activeInHierarchy);

    }

    public Camera getCamera()
    {
        return myCam;
    }

}
