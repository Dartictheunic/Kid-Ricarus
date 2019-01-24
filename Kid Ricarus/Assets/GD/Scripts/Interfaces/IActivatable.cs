using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//A implémenter sur les objets qui pourront être activés (portes, effets, etc...) de préférence à utiliser en combinaison d'un collider trigger
public interface IActivatable
{
    void ActivateItem();
}
