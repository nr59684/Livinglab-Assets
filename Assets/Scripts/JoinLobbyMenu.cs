using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

//Script zur IP-Eingabe
public class JoinLobbyMenu : MonoBehaviour
{
    [SerializeField] private NetworkManagerLobby networkManager = null;         //Verkn�pfung zum NetworkManager

    [Header("UI")]      //Verkn�pfungen zur UI
    [SerializeField] private GameObject landingPagePanel = null;                //Panal zur IP eingabe
    [SerializeField] private TMP_InputField ipAddressInputField = null;         //Feld zur IP eingabe
    [SerializeField] private Button joinButton = null;                          //Button zum Beitreten
    [SerializeField] private TMP_Text IPError = null;                           //Warnung falls IP nicht funktioniert

    private void OnEnable()     //wenn dieses Menu ge�ffnet wird, achten wir darauf ob einem Server Beigetreten wird
    {
        NetworkManagerLobby.OnClientConnected += HandleClientConnected;
        NetworkManagerLobby.OnClientDisconnected += HandleClientDisconnected;
    }

    private void OnDisable()    //wenn das Menu nicht mehr ge�ffnet ist, ist uns hier egal, ob wir einem Server beitreten
    {
        NetworkManagerLobby.OnClientConnected -= HandleClientConnected;
        NetworkManagerLobby.OnClientDisconnected -= HandleClientDisconnected;
    }

    public void JoinLobby()     //Wird durch den Button ausgef�hrt
    {
        string ipAddress = ipAddressInputField.text;    //Wir nehmen die IP aus der Eingabe...
        networkManager.networkAddress = ipAddress;      //... und versuchen mit ihr...
        networkManager.StartClient();                   //... als Client beizutreten
        joinButton.interactable = false;                //Join Button wird deaktiviert, w�hren die Verbindung hergestellt wird um mehrfaches klicken w�hrend dem laden zu verhindern
    }

    private void HandleClientConnected()            //Verbindungsaufbau hat funktioniert
    {
        joinButton.interactable = true;             //Button wird f�r n�chstes mal aktiviert
        gameObject.SetActive(false);                //dieses Gameobjekt wird deaktiviert
        landingPagePanel.SetActive(false);          //Menu wird geschlossen
    }

    private void HandleClientDisconnected()         //Joinen hat nicht funktioniert/ Verbindung abgebrochen
    {
        joinButton.interactable = true;             //Button kann f�r einen neuen Versuch wieder benutzt werden
        IPError.gameObject.SetActive(true);         //Warnung wird angezeigt
    }
}
