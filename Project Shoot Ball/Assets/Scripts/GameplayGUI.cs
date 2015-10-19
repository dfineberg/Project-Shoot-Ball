using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameplayGUI : MonoBehaviour {

    public class AmmoCounter
    {
        public GameObject playerObject;
        public RectTransform counterTransform;
        public Image image;
        public Text text;
        public PlayerController player;

        public AmmoCounter(GameObject counterObject, PlayerController player)
        {
            counterTransform = counterObject.GetComponent<RectTransform>();
            image = counterObject.GetComponent<Image>();
            text = counterObject.GetComponentInChildren<Text>();
            this.player = player;
            playerObject = player.gameObject;
        }
    }

    public GameObject ammoCounterPrefab;
    public Text scoreText;
    public Text gameOverMessage;
    public GameObject[] gameOverObjects;

    static float referenceZoom = 7f;

    List<AmmoCounter> ammoCounters;
    new RectTransform transform;

	public void InitAmmoCounters()
    {
        ammoCounters = new List<AmmoCounter>();
        transform = GetComponent<RectTransform>();

        //for each object we need to track, instantiate an ammo counter on the GUI
        foreach(PlayerController player in FindObjectsOfType<PlayerController>())
        {
            GameObject counter = (GameObject)Instantiate(ammoCounterPrefab);
            counter.transform.SetParent(transform);
            counter.transform.localScale = Vector3.one;
            ammoCounters.Add(new AmmoCounter(counter, player));
        }
    }

    void Update()
    {
        float proportion = referenceZoom / Camera.main.orthographicSize;

        for(int i = 0; i < ammoCounters.Count; i++)
        {
            Vector2 playerScreenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, ammoCounters[i].playerObject.transform.position);
            Vector2 anchoredPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform, playerScreenPos, Camera.main, out anchoredPos);
            ammoCounters[i].counterTransform.anchoredPosition = anchoredPos;
            ammoCounters[i].text.text = ammoCounters[i].player.GetAmmoCount().ToString();
            ammoCounters[i].image.fillAmount = ammoCounters[i].player.GetChargeProportion();
            ammoCounters[i].counterTransform.localScale = new Vector3(proportion, proportion, proportion);
        }
    }

    public void SetScoreText(string newText)
    {
        scoreText.text = newText;
    }

    public void ShowGameOverMessage(bool show)
    {
        for(int i = 0; i < gameOverObjects.Length; i++)
        {
            gameOverObjects[i].SetActive(show);
        }
    }
}
