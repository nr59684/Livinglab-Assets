using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;

[XmlRoot("Thema")]
public class Thema                  //Klasse zum Speichern von Themen
{
    [XmlRoot("Frage")]
    public class Frage              //Klasse zum Speichern von Fragen
    {
        [XmlRoot("Antwort")]
        public class Antwort        //Klasse zum Speichern von Antworten
        {
            [XmlElement("AntwortText")]
            public string AntwortText;      //Der Text einer Antwortm�glichkeit, z.B. "Ja"
            [XmlElement("Next")]
            public string Next;             //die empfohlene n�chste Frage, -> id der Frage z.B. "5", falls Themenwechsel "neuesThema/5", Sonderf�lle m�glich z.B. "Stop", "Abbruch", "Ende"
        }


        [XmlElement("ID")]
        public uint ID;             //ID einer Frage sollte in einem Thema m�glichst eindeutig sein, immer eine positive Ganzzahl z.B. 5
        [XmlElement("Text")]
        public string Text;         //Der Text der Frage, die dem Patienten gestellt wird z.B. "Haben Sie schmerzen?"
        [XmlElement("Optional")]
        public bool Optional;       //bool der anzeigt, ob eine Antwort auf diese Frage erwartet wird
        [XmlElement("Next")]
        public string Next;         //Default empfohlene n�chste Frage f�r alle Antworten (oder keine Antwort), -> id der Frage z.B. "5", falls Themenwechsel "neuesThema/5", Sonderf�lle m�glich z.B. "Stop", "Abbruch", "Ende"

        [XmlArray("Antworten"), XmlArrayItem("Antwort")]
        public List<Antwort> Antworten = new List<Antwort>();   //Array aller voergegebenen Antwortm�glichkeiten

        public void addAntwort(string antwort)      //F�rgt eine neue Antwortm�glichkeit hinzu (vermutlich nicht notwendig f�r diese Anwendung)
        {
            Antworten.Add(new Antwort());
            Antworten[Antworten.Count - 1].AntwortText = antwort;
        }

        public List<string> getAntworten()
        {
            List<string> returnAntworten = new List<string>();
            foreach (Antwort antwort in Antworten)
            {
                returnAntworten.Add(antwort.AntwortText);
                
            }
            return returnAntworten;
        }

    }


    [XmlElement("Titel")]
    public string Titel;                            //Der Titel des Themas

    [XmlArray("Fragen"), XmlArrayItem("Frage")]
    public List<Frage> Fragen = new List<Frage>();  //Array aller Fragen zu diesem Thema
    
    public string toString()
    {
        return "Titel: " + Titel + " Anzahl Fragen: " + Fragen.Count;
    }

    public void addFrage(string frage)              //F�gt eine neue Frage hinzu (vermutlich nicht notwendig f�r diese Anwendung)
    {
        Fragen.Add(new Frage());
        Fragen[Fragen.Count - 1].Text = frage;
    }

}
