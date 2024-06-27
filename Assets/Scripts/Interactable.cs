using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//beh�lfsm��iges Script zur Verwaltung von Objekten, mit denen man interagieren kann
public class Interactable : MonoBehaviour
{
    public UnityEvent onInteract;       //Event, welches durch das Interagieren ausgel�st wird
    public Sprite interactIcon;         //Icon, zu dem der Kurser wechselt, wenn das Object betrachtet wird
    public int ID;

    // Start is called before the first frame update
    void Start()
    {
        ID = Random.Range(0, 999999);   //TODO: Theoretisch unpracktisch f�r viele Interactables, Chance f�r Probleme extrem gering 
    }
}