using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CourantAerien : MonoBehaviour
{
    public GameObject entree, sortie;
    public float speedBoost; 
    private Vector3 direction;
    public ParticleSystem fx; 

    public TypeDeCourant type = TypeDeCourant.ascendant;
    private int typeModif = 1;

    public Color cAscendant;
    public Color cDescandant;


   
    private void Start()
    {
        direction = (sortie.transform.position - entree.transform.position).normalized;
        Debug.Log(direction);
    }



    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerControllerDjuloh>() != null)
        {
            ForcesDictionnaryScript.forcesDictionnaryScript.AddForce ("Courant" + transform.parent.name , direction * speedBoost*typeModif); 
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerControllerDjuloh>() !=null)
        {
            ForcesDictionnaryScript.forcesDictionnaryScript.RemoveForce("Courant" + transform.parent.name);
        }
    }

    [SerializeField]
    public enum TypeDeCourant
    {
        ascendant , 
        descendant
    }

    private void OnGUI()
    {
        if ((int)type == 1)
            Debug.Log("c'est descendant");

        switch (type)
        {
            case TypeDeCourant.ascendant:
                {
                    typeModif = 1;
                    print(typeModif);
                    fx.transform.position = entree.transform.position;
                    ParticleSystem.VelocityOverLifetimeModule psVel = fx.velocityOverLifetime;
                    psVel.speedModifier = 1;
                    ParticleSystem.MainModule psMain = fx.main;
                    psMain.startColor = cAscendant;

                }
                break;

            case TypeDeCourant.descendant:
                {
                    typeModif = -1;
                    print(typeModif);
                    fx.transform.position = sortie.transform.position;
                    ParticleSystem.VelocityOverLifetimeModule psVel = fx.velocityOverLifetime;
                    psVel.speedModifier = -1;
                    ParticleSystem.MainModule psMain = fx.main;
                    psMain.startColor = cDescandant;
                }
                break;

        }

    }

}
