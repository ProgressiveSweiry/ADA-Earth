using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OnPressedContact : MonoBehaviour
{

    [SerializeField]
    TextMeshProUGUI name, address;


    private void OnMouseDown()
    {
        Launch();
    }

    public void Launch() {

        Contact temp = new Contact();
        temp.Name =  name.text.Replace(" ", "");
        temp.Address = address.text.Replace(" ", "");

        GameObject.FindGameObjectWithTag("AddressBookHandle").GetComponent<AddressBookHandle>().DeleteThisContact(temp);

    }
}
