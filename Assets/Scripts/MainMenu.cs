using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script für das Hauptmenu das eigentlich nur einen neuen Server startet
public class MainMenu : MonoBehaviour
{
    [SerializeField] private NetworkManagerLobby networkManager = null; //Verknüpfung zum NetworkManager

    [Header("UI")]
    [SerializeField] private GameObject landingPagePanel = null;        //Verknüpfung zur Menu, ob man selbst Hosten will

    public void HostLobby()                 //Nutzer startet einen neuen Server
    {
        networkManager.StartHost();
        landingPagePanel.SetActive(false);  //Menu wird geschlosssen
    }
}
