using UnityEngine;

public class MenuPages : MonoBehaviour
{
    private void Start()
    {
        OpenPage(transform.GetChild(0).gameObject);
    }

    public void OpenPage(GameObject page)
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        page.SetActive(true);
    }
}
