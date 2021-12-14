using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuHandle : MonoBehaviour
{

    [SerializeField]
    GameObject ExplorerPanel, MenuPanel, BookButton, SearchButton, SearchPanel, AddressBookPanel, SigmaButton, SigmaPanel, loadingObject, RandomPanel, ParamsPanel;


    public void EnableMenuPanel()
    {
        ParamsPanel.SetActive(true);
        RandomPanel.SetActive(true);
        ExplorerPanel.SetActive(false);
        MenuPanel.SetActive(true);
        BookButton.SetActive(true);
        SearchButton.SetActive(true);
        SigmaButton.SetActive(true);
        SearchPanel.SetActive(false);
        CameraCtrl.canMove = false;
    }

    public void ExitPanel()
    {
        MenuPanel.SetActive(false);
        AddressBookPanel.SetActive(false);
        ExplorerPanel.SetActive(true);
        SigmaPanel.SetActive(false);
        loadingObject.SetActive(false);
        CameraCtrl.canMove = true;
    }

    public void EnableSearchPanel()
    {
        BookButton.SetActive(false);
        AddressBookPanel.SetActive(false);
        SearchButton.SetActive(false);
        SigmaButton.SetActive(false);
        RandomPanel.SetActive(false);
        ParamsPanel.SetActive(false);
        SearchPanel.SetActive(true);
    }

    public void EnableSigmaPanel()
    {
        BookButton.SetActive(false);
        SearchButton.SetActive(false);
        SearchPanel.SetActive(false);
        SigmaButton.SetActive(false);
        AddressBookPanel.SetActive(false);
        RandomPanel.SetActive(false);
        ParamsPanel.SetActive(false);
        SigmaPanel.SetActive(true);
    }

    public void EnableAddressBookPanel()
    {
        BookButton.SetActive(false);
        SearchButton.SetActive(false);
        SigmaButton.SetActive(false);
        SearchPanel.SetActive(false);
        RandomPanel.SetActive(false);
        ParamsPanel.SetActive(false);
        AddressBookPanel.SetActive(true);
    }
}
