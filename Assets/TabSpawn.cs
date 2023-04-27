using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

//Force l'object sur lequel on met le script a avoir un ARTracked Image manager
[RequireComponent(typeof(ARTrackedImageManager))]
public class TabSpawn : MonoBehaviour
{

        //Liste pour stocker les �l�ments que l'on fait spawn
    List<GameObject> spawnedList = new List<GameObject>();

    int spawnedCount = 0;
    //Serialized field : Permet d'exposer la variable objectToSpawn dans l'�diteur
    //objectToSpawn : Liste des pr�fabs que l'ont veut instancier
    [SerializeField]
    List<GameObject> objectsToSpawn = new List<GameObject>();
    
    //R�f�rence au component ARTracked Image manager
    ARTrackedImageManager aRTrackedImageManager = null;

    //Fonction appel�e � l'initialisation de l'objet
    private void Awake()
    {
        //On r�cup�re la r�f�rence � l'ARTracked Image Manager
        aRTrackedImageManager = this.GetComponent<ARTrackedImageManager>();
    }

    //Fonction appel�e � l'initialisation de l'objet, apr�s le Awake
    private void OnEnable()
    {
        //On associe la fonction SwapOject (cr�� plus bas), a l'event d�clench� par le ARTracked image manager lorsqu'il d�tecte une nouvelle image
        aRTrackedImageManager.trackedImagesChanged += SwapObject;
    }

    //Fonction appel�e a la d�sactivation de l'objet
    private void OnDisable()
    {
        //On arr�te d'�couter l'event (la fonction SwapObject ne sera plus appel�e)
        aRTrackedImageManager.trackedImagesChanged -= SwapObject;
    }


    //Fonction appel�e lorsque l'event trackedImagesChanged est d�clench� par l'ARTracked image manager
    //Param : ARTrackedImagesChangedEventArgs trackedImages , arguement obligatoire , renvoy� par la fonction trackedImagesChanged
    public void SwapObject(ARTrackedImagesChangedEventArgs trackedImages)
    {
        //Tracked images contient des listes des images detect�es selon leur statut, dans ce cas on parcours la liste des images ajout�es
        foreach (ARTrackedImage trackedImage in trackedImages.added)
        {
            //Si l'image ajout�e a le nom de la premiere image
            //on appelle la fonction SetNewMesh avec l'index correspondant a la liste des pr�fabs
            if(trackedImage.referenceImage.name == "img1")
            {
                SetNewMesh(0);
            }
            //Sinon si l'image ajout�e a le nom de la seconde image
            //on appelle la fonction SetNewMesh avec l'index correspondant a la liste des pr�fabs
            else if (trackedImage.referenceImage.name == "img2")
            {
                SetNewMesh(1);
            }
        }
    }

    //Fonction pour faire apparaite le mesh d�sir�
    public void SetNewMesh(int index)
    {
        //On cherche un objet de type SpawnedDefaut (Objet vide par d�faut , avec comme seul composants un transform et le sript SpawnedDefault)
        SpawnedDefault[] objectsToReplace = GameObject.FindObjectsOfType<SpawnedDefault>();

        foreach(SpawnedDefault objectTotest in objectsToReplace)
        {
            if(objectTotest != null)
            {
                //Si la variable isSet de l'objet par d�faut est a false
                if (!objectTotest.iSSet)
                {
                    //On instantie un objet de la liste via son index
                    GameObject newObject = Instantiate(objectsToSpawn[index], objectTotest.transform);
                    //On passe la variable de notre objet a true
                    objectTotest.iSSet = true;

                    //On attache notre nouvelle objet a l'objet vide par defaut
                    newObject.transform.parent = objectTotest.transform;
                    //On reset sa position locale (relative au nouveau parent) � 0,0,0
                    newObject.transform.localPosition = Vector3.zero;

                     spawnedList.Add(newObject);
                    //DEBUG on fait vibrer l'appareil pour voir si on passe bien dans la fonction
                    Handheld.Vibrate();
                }
            }
        }
    }
}
