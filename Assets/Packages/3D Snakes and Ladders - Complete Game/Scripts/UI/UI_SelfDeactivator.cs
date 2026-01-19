using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_SelfDeactivator : MonoBehaviour
{
    public void SelfDeactive()
    {
        this.gameObject.SetActive(false);
    }
}
