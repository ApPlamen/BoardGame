using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GRP_Inputfields : MonoBehaviour
{
    private GRP_PromptManager prompt;
    public Text text;

    // Start is called before the first frame update
    void Start()
    {
        prompt = GetComponent<GRP_PromptManager>();
        prompt.SetReturn("Update Label", (string val) => { text.text = val; });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreatePrompt(int type)
    {
        prompt.promptType = (PromptType)type;
        prompt.Create();
    }

    private void UpdateLabel(string val)
    {
        text.text = val;
    }
}
