using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Verwaltung von Events zum öffnen und schließen des Chats (dient der Kommunikation der verschiedenen Scripte)

public class MainSceneEventManager : MonoBehaviour
{
    public delegate void OpenChat();
    public delegate void CloseChat();

    public static event OpenChat OnOpenChat;    //Event wenn der Chat geöffnet wird
    public static event CloseChat OnCloseChat;  //Event wenn der Chat geschlossen wird


    public static void OpenChatTrigger() //Chat wird geöffnet (und z.B. Bewegung im MovementManager deaktiviert)
    {
        if (OnOpenChat != null)
        {
            OnOpenChat();
        }
    }

    public static void CloseChatTrigger() //Chat wird geschlossen
    {
        if (OnCloseChat != null)
        {
            OnCloseChat();
        }
    }
}
