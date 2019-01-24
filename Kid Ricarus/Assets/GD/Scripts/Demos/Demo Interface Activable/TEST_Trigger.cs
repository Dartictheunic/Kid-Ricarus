using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TEST_Trigger : MonoBehaviour
{
    [Tooltip("Porte qui prend une couleur publique")]
    public GameObject itemToActivateWithCondition;
    [Tooltip("Portes qui seront trigger en même temps")]
    public GameObject[] multipleItemsToActivate;

    [Tooltip("Activer tout, la porte seule ou seulement l'array")]
    public ItemsToActivate itemsTo = ItemsToActivate.all;

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<TEST_FakePlayerDemoActivable>() != null)
        {
            switch(itemsTo)
            {
                case ItemsToActivate.withCondition:
                    {
                        itemToActivateWithCondition.GetComponent<IActivatable>().ActivateItem();
                    }   break;

                case ItemsToActivate.withoutCondition:
                    {
                        for(int i = 0; i < multipleItemsToActivate.Length; i++)
                        {
                            multipleItemsToActivate[i].GetComponent<IActivatable>().ActivateItem();
                        }
                    }   break;

                case ItemsToActivate.all:
                    {
                        itemToActivateWithCondition.GetComponent<IActivatable>().ActivateItem();
                        for (int i = 0; i < multipleItemsToActivate.Length; i++)
                        {
                            multipleItemsToActivate[i].GetComponent<IActivatable>().ActivateItem();
                        }
                    }   break;

            }
        }
    }

    public enum ItemsToActivate
    {
        all,
        withCondition,
        withoutCondition
    }
}
