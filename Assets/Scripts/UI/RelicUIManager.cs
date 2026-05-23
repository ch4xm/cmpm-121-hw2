using System.Linq;
using UnityEngine;

public class RelicUIManager : MonoBehaviour
{
    public GameObject relicUIPrefab;
    public PlayerController player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventBus.Instance.OnGameStart += (_, _) => Reset();
        EventBus.Instance.OnRelicSelected += OnRelicPickup;
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    void Reset()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void OnRelicPickup(Relic r)
    {
        // make a new Relic UI representation
        int activeRelicCount = player.GetActiveRelicCount();

        GameObject rui = Instantiate(relicUIPrefab, transform);
        rui.transform.localPosition = new Vector3(-450 + 40 * (activeRelicCount - 1), 0, 0);
        RelicUI ruic = rui.GetComponent<RelicUI>();
        ruic.SetRelic(r);
        ruic.player = player;
        ruic.index = activeRelicCount - 1;
        
    }

    void OnDestroy()
    {
        EventBus.Instance.OnGameStart -= (_, _) => Reset();
        EventBus.Instance.OnRelicSelected -= OnRelicPickup;
    }

}
