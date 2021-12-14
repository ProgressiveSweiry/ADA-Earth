using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AddressBookHandle : MonoBehaviour
{

    private Contact testContact;

    private List<Contact> contactArray;

    private List<GameObject> ObjList;

    [SerializeField]
    GameObject listViewPort;

    [SerializeField]
    GameObject content;

    [SerializeField]
    TMP_InputField searchInput;

    [SerializeField]
    GameObject AddPanel, BookPanel;

    [SerializeField]
    TMP_InputField NameInput, AddressInput;

    [SerializeField]
    GameObject Delete_Text;

    [SerializeField]
    GetAddress getAddress;

    [SerializeField]
    MenuHandle menuHandle;

    [SerializeField]
    GameObject hideExitButton;

    public bool isDeleting = false;

    private void OnEnable()
    {
        
        StartCoroutine(InitListE());
    }

    public void InitList()
    {
        StartCoroutine(InitListE());
    }

    IEnumerator InitListE()
    {
        LoadContactList();
        yield return new WaitForFixedUpdate();

        try
        {
            foreach (GameObject g in ObjList)
            {
                Destroy(g);
            }
        }
        catch (Exception e) { }

        foreach (Transform child in listViewPort.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        ObjList = new List<GameObject>();


        foreach(Contact c in contactArray)
        {
            if (c.Name.ToLower().Contains(searchInput.text.ToLower()) || c.Address.ToLower().Contains(searchInput.text.ToLower()))
            {
                //Add Test Contact 
                GameObject Address0 = GameObject.Instantiate(content);
                Address0.transform.SetParent(listViewPort.transform);

                ObjList.Add(Address0);

                foreach (Transform t in Address0.transform)
                {
                    switch (t.name)
                    {
                        case "Name":
                            t.GetComponent<TextMeshProUGUI>().SetText(c.Name);
                            break;

                        case "Address":
                            t.GetComponent<TextMeshProUGUI>().SetText(c.Address);
                            break;
                    }
                }
            }
        }

    }

    void LoadContactList()
    {
        contactArray = new List<Contact>();

        string[] names = PlayerPrefsX.GetStringArray("ContactsNames");
        string[] addresses = PlayerPrefsX.GetStringArray("ContactsAddresses");

        for(int i=0; i < names.Length; i++)
        {
            Contact t = new Contact();
            t.Name = names[i];
            t.Address = addresses[i];

            contactArray.Add(t);
        }
    }

    void SaveContactList()
    {

        int s = contactArray.Count;
        List<string> names = new List<string>();

        for(int i = 0; i < s; i++)
        {
            names.Add(contactArray[i].Name);
        }

        List<string> addresses = new List<string>();

        for (int i = 0; i < s; i++)
        {
            addresses.Add(contactArray[i].Address);
        }

        string[] names_a = names.ToArray();

        PlayerPrefsX.SetStringArray("ContactsNames", names_a);

        string[] addresses_a = addresses.ToArray();

        PlayerPrefsX.SetStringArray("ContactsAddresses", addresses_a);

    }

    public void ShowAddPanel()
    {
        BookPanel.SetActive(false);
        AddPanel.SetActive(true);
        hideExitButton.SetActive(false);
    }

    public void ShowBookPanel()
    {
        BookPanel.SetActive(true);
        AddPanel.SetActive(false);
        hideExitButton.SetActive(true);
    }

    public void AddContact()
    {
        Contact tempContact = new Contact();
        tempContact.Name = NameInput.text;
        tempContact.Address = AddressInput.text;

        contactArray.Add(tempContact);

        NameInput.text = "";
        AddressInput.text = "";

        SaveContactList();

        ShowBookPanel();

        StartCoroutine(InitListE());
    }

    public void DeleteContact()
    {
        isDeleting = !isDeleting;

        Delete_Text.SetActive(isDeleting);
    }

    public void DeleteThisContact(Contact contact)
    {
        if (isDeleting)
        {

            int counter = 0;
            foreach(Contact c in contactArray)
            {               
                if(contact.Name == c.Name)
                {
                    contactArray.RemoveAt(counter);
                    break;
                }
                counter++;
            }

            isDeleting = false;
            Delete_Text.SetActive(isDeleting);

            SaveContactList();

            StartCoroutine(InitListE());
        }
        else
        {
            menuHandle.EnableSearchPanel();
            getAddress.getAddressContact(contact.Address);
        }
    }
}

public class Contact
{
    public string Name;
    public string Address;
}
