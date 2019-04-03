using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portail : MonoBehaviour
{
    public static int cristauxInLevel;

    [Range (0f ,4f )]
    public int nombreDeCristaux;
    public GameObject[] cristauxSlot;
    public GameObject interieurPortail;
    public ParticleSystem[] FXs; 
    private Mesh mesh;


    private void Awake()
    {
        cristauxInLevel = nombreDeCristaux;
       // interieurPortail.SetActive(false); Les comments c'est quand il y aura les graph 
    }

    public void cristauxCheck()
    {
        if (cristauxInLevel <= 0)
        {
            PortailOpen(); 
        }

    }

    private void PortailOpen ()
    {
          // interieurPortail.SetActive(true); 
            print("c'est ouvert Khey ");
        if (FXs.Length == 0)
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

    //void OnDrawGizmos(Color color)
    //{
    //    mesh = GetComponent<MeshFilter>().sharedMesh;

    //    Gizmos.color = color;
    //    Gizmos.DrawMesh(mesh, transform.position, transform.localRotation, transform.localScale);
    //}

}
