using UnityEngine;
using System.Collections;

public class DebugControls : MonoBehaviour {

	void Update()
    {
        for(int i = 1; i < 5; i++)
        {
            if(Input.GetButtonDown("Start_" + i))
            {
                Application.LoadLevel(0);
            }
        }
    }
}
