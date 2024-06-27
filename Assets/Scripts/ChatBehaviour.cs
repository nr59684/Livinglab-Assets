using Mirror;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;

/* Script für den Chat
 * Jeder Chatteilnehmer bekommt dieses Script zugewiesen
 */
public class ChatBehaviour : NetworkBehaviour
{
    [SyncVar] private string Name = null;                           //Name des Teilnehmers
    [SerializeField] private GameObject chatUI = null;              //Verknüpfungen zur UI (hier: Chat Fenster)
    [SerializeField] private ChatUIScript chatPref = null;          //Die Textausgabe für den Chat
    [SerializeField] private TMP_InputField inputField = null;      //Texteingabe für neue Nachrichten
    [SerializeField] private string LogPath = @"ChatLog.txt";       //Pfad zur Log Datei
    [SerializeField] Transform AntwortScrollViewContent = null;
    [SerializeField] GameObject AntwortButtonPref = null;
    [SerializeField] private SpeechRecognition speechRecognition = null;

    System.Random rnd = new System.Random();

    private StreamWriter sw;

    public static event Action<string> OnMessage;                   //Event, wenn etwas in den Chat geschrieben wird
    public static event Action<List<string>> OnReciveAnswers;                   //Event, wenn etwas in den Chat geschrieben wird

    public void Start()
    {


    }
    [Server]
    public override void OnStartServer()        //Wird beim Start des Servers von diesem ausgeführt
    {
        //Erzeugenug der Log Datei, da jederzeit Schluss sein kann wird die datei immer wieder geöffnet und geschlossen -> Speichert auch bei Abstürzen
        FileStream fs = new FileStream(LogPath, FileMode.Append, FileAccess.Write, FileShare.Write);
        fs.Close();
        sw = new StreamWriter(LogPath, true, Encoding.ASCII);
        sw.WriteLine("Started Log at " + DateTime.Now.ToString("HH:mm:ss"));
        sw.Close();
    }

    [Server]
    public override void OnStopServer()
    {
        sw.Close();                                 //Vermutlich unnötig, da StreamWriter schon geschlossen
        Debug.Log("Closed StreamWriter");
    }

    public override void OnStartAuthority()         //Wird ausgeführt wenn das Clientspezifische ChatBehaviour started. UI wird aktiviert und das Script achtet auf neue Message-Events
    {
        chatUI.SetActive(true);                     //Die Eigene UI wird aktiviert -> Sonst könnte man die von allen Spielern sehen
        OnMessage += HandleNewMessage;              //OnMessage wird abboniert -> HandleNewMessage wenn eine neue Nachricht kommt
        OnReciveAnswers += HandleNewAnswers;
    }

    /* Zum Testen wenn kein Arzt da ist
    private void Update()
    {
        int dice = rnd.Next(1, 1001);
        if (dice == 1000)
        {
            List<string> antworten = new List<string>();
            antworten.Add("1");
            antworten.Add("2");
            antworten.Add("3");
            antworten.Add("4");
            AskQuestion("Hallo", antworten );
        }
    }*/

    [ClientCallback]
    private void OnDestroy()                        //Spieler bricht die Verbindung ab -> Aufräumen
    {
        if (!isOwned) { return; }              //Wenn man nicht selbst die Verbindung beendet passiert nichts
        OnMessage -= HandleNewMessage;
        OnReciveAnswers -= HandleNewAnswers;
    }

    private void HandleNewMessage(string message)   //Wurde an das Event ONMessage gehängt -> Wird ausgeführt, wenn eine neue Nachricht eintrifft
    {
        chatPref.addText(message);                   //neue Message wird in den Chat geschrieben
        speechRecognition.Speak(String.Concat(message.Split(']').Skip(1)));
    }

    private void HandleNewAnswers(List<string> antworten)
    {
        if (AntwortScrollViewContent != null)
        {
            int childs = AntwortScrollViewContent.transform.childCount;
            for (int i = childs - 1; i >= 0; i--)
            {
                GameObject.Destroy(AntwortScrollViewContent.transform.GetChild(i).gameObject);
            }

            int antwortCount = antworten.Count;
            for (int i = 0; i < antwortCount; i++)
            {
                Debug.Log("Creating AntwortButton" + antworten[i]);
                GameObject AntwortButton = Instantiate(AntwortButtonPref, AntwortScrollViewContent) as GameObject;
                AntwortButton.GetComponent<AntwortButtonScript>().setAntwort(antworten[i]);
                AntwortButton.gameObject.transform.GetComponent<Button>().onClick.AddListener(delegate { AutoSend(AntwortButton.GetComponent<AntwortButtonScript>().Antwort); clearAntworten(); });

            }
        }
    }

    public void clearAntworten()
    {
        if (AntwortScrollViewContent != null)
        {
            int childs = AntwortScrollViewContent.transform.childCount;
            for (int i = childs - 1; i >= 0; i--)
            {
                GameObject.Destroy(AntwortScrollViewContent.transform.GetChild(i).gameObject);
            }
        }
    }



    //[Client]
    public void setName(string name)                //set und get Funktionen für den Namen
    {
        Name = name;
    }

    [Client]
    public string getName()
    {
        return Name;
    }

    [Client]
    public void Send(string message)                //Wird durch das Eingabefeld aufgerufen und sendet eine Nachicht
    {
        if (!Input.GetKeyDown(KeyCode.Return)) { return; }      //Wenn nicht Return grdrückt wurde passier nichts

        if (string.IsNullOrWhiteSpace(message)) { return; }     //Wenn die Nachricht leer ist, passiert auch nichts

        CmdSendMessage($"[{Name}]: {message}");     //Nachricht wird zusammen mit dem Namen an den Server gesendet
        inputField.text = string.Empty;             //Eingabefeld wird wieder leer gemacht 
                
    }

    [Client]
    public void AutoSend(string message)            //Sendet eine Nachricht die nicht aus dem Eingabefeld kommt -> für vorgefertigte Fragen, Spracherkennung
    {
        CmdSendMessage($"[{Name}]: {message}");     //Nachricht wird zusammen mit dem Namen an den Server gesendet
    }

    [Client]
    public void AskQuestion(string message, List<string> antworten)
    {
        CmdSendMessage($"[{Name}]: {message}");     //Nachricht wird zusammen mit dem Namen an den Server gesendet
        CmdSendAnswers(antworten);
    }

    [Command]
    private void CmdSendAnswers(List<string> antworten)
    {
        RpcHandleAnswers(antworten);                  //Nachricht wird durch den Server bearbeitet
    }

    [ClientRpc]
    private void RpcHandleAnswers(List<string> antworten)
    {
        //Hier könnten Filter etc benutzt werden
        OnReciveAnswers?.Invoke(antworten);          //Nachricht wird verteilt -> OnMessage Event wird ausgeführt
    }


    [Command]
    private void CmdSendMessage(string message)     //Wird durch den Client auf dem Server Aufgerufen -> Verteilt neue Message
    {
        RpcHandleMessage(message);                  //Nachricht wird durch den Server bearbeitet
        sw = new StreamWriter(LogPath, true, Encoding.ASCII);   //Nachricht wird in den Log geschreiben (Vielleicht in RpcHandleMessage besser aufgehoben?)
        sw.WriteLine(DateTime.Now.ToString("hh:mm:ss tt")+" " + message);
        Debug.Log("Hab ich was geschrieben?");
        sw.Close();
    }

    [ClientRpc]
    private void RpcHandleMessage(string message)   //Nachricht wird durch den Server bearbeitet
    {
        //Hier könnten Filter etc benutzt werden
        OnMessage?.Invoke($"\n{message}");          //Nachricht wird verteilt -> OnMessage Event wird ausgeführt
    }
}