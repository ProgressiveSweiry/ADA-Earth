using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CopyText : MonoBehaviour
{
    private TextMeshProUGUI t;

    private void OnEnable()
    {
        t = transform.parent.GetComponent<TextMeshProUGUI>();
    }

    public void Copy()
    {
        string textToCopy = t.text;
        textToCopy = textToCopy.Replace("Sigma: ", "");
        textToCopy = textToCopy.Replace("Epoch Nonce: ", "");
        textToCopy = textToCopy.Replace("Address: ", "");
        Debug.Log("Copied: " + textToCopy);

        TextEditor editor = new TextEditor
        {
            text = textToCopy
        };
        editor.SelectAll();
        editor.Copy();
    }
}
