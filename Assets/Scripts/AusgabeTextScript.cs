using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


//Script um den Chat auf dem Monitor des Stuhls anzuzeigen

public class AusgabeTextScript : MonoBehaviour
{
    [SerializeField] private TMP_Text chatText = null;
    void Start()                                            //Subscribt zum OnMessage Event aus dem Chat
    {
        ChatBehaviour.OnMessage += HandleNewMessage;        //Wenn etwas in den Chat gesendet wird, schreibt das auch der Bildschirm mit
    }

    private void HandleNewMessage(string message)
    {
        //Der String muss passend portioniert werden, damit er nicht aus dem Bildschirm herausragt. -> Nicht perfeckt f�r lange Buchstaben TODO: Pixel statt Buchstaben z�hlen
        //TMP internes wrapping funktionierte nur horizontal
        string wrappedLine ="";                             //die Zeile, die als n�chstes ausgegeben wird
        string[] originalLines = message.Split(' ');        //Message wird an Leehrzeichen gesplittet -> Liste mit W�rtern
        foreach (var item in originalLines)                 //Einzelne W�rter werden durchgegeangen
        {
            if (wrappedLine.Length + item.Length >= 29)     //Wenn das Word nicht mehr in die Zeile (29 Zeichen) passt, wird die alte Zeile aufgeschrieben und eine Neue begonnen
            {
                chatText.text += wrappedLine;
                chatText.text += "\n";
                wrappedLine = "    " + item +" ";           //4 lehrzeichen um Sendernamen hervorzuheben (Text selbst ist einger�ckt)
            }
            else
            {
                wrappedLine += item;                        //Wenn das Wort noch passt, wird es der Zeile hinzugef�gt
                wrappedLine += " ";
            }
        }
        chatText.text += wrappedLine;                       //letzte Zeile wird am ende auch noch aufgeschreiben

        //Wird der Text zu lang, werden die oberen Zeilen abgeschritten
        while (chatText.text.Split('\n').Length > 19)
            chatText.text = chatText.text.Remove(0, chatText.text.Split('\n')[0].Length + 1);   //Der Text wird zum Text ohne die erste Zeile. Schleife bis es wieder weniger als 19 Zeilen sind
    }
}
