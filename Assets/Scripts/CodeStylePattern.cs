using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodeStylePattern : MonoBehaviour {


    /**
     * 
     *  Variables - Camel
     * 
     */



    /**
     *   
     *   Field sort in group by protection and type(primitive or link)
     *       sort by length and alphabet
     *
     *   public primitives
     *
     *   public links
     *  
     *   private primitives
     *
     *   private links
     *
     */

    /** 
     * 
     *   Fields and methods are separated by 3 lines
     *   
     *   One line between methods
     *   
     *   One line between groups
     *   
     *   Unity methods, like Update or Start etc, are above user methods
     * 
     */




    public int numberInt;
    public bool isNumber;
    public short smallNumber;

    public GameObject object1;
    public GameObject object2;

    private int counter;
    private bool isReady;
    private string error;

    private AudioClip sound;
    private GameObject garbage;
    private Rigidbody2D physicBody;



    void Start() {
        error = "error!";
        Debug.Log(sound.name + " is now playing");
    }

    void Update() {
        try {
            if(isReady)
                doIt();
            else if(isNumber)
                dontDoIt();
            else if(!isNumber && !isReady) {
                isReady = !isReady;
                isNumber = !isNumber;
                counter++;
            } else
                Debug.Log(error);
        } catch {
            Debug.Log(error);
            Destroy(physicBody);
        } finally {
            Destroy(garbage);
        }
    }

    void doIt() {
        Debug.Log("I did it!");
    }

    void dontDoIt() {
        Debug.Log("I did not do it!");
    }

}
