using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cristaux : MonoBehaviour , IActivatable
{
    private bool hasBeenActivated;
    public Animator anim;
    public ParticleSystem[] FXs; 
    public ParticleSystem rayon;
    public Portail portail;


    public void ActivateItem()
    {
        if (hasBeenActivated !=true)
        {
            Portail.cristauxInLevel--;
            //rayon.transform.position = portail.cristauxSlot[Portail.cristauxInLevel].transform.position; 
            portail.cristauxCheck();
            gameObject.SetActive(false);
            print(Portail.cristauxInLevel);
            CristauxActivation(); 
        }
    }

    private void CristauxActivation ()
    {
        // anim.Play();
        if (FXs.Length ==0 )
        {
            return;
        }
        else
        {
            foreach (ParticleSystem fx in FXs)
            {
                fx.Play();
            }
        }
    }

}

   



