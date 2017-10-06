using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIControllerScript : MonoBehaviour {

    // NBH = neighbourhood

    public GameObject menuCanvas;                           // the canvas for the main menu

    public GameObject gameUICanvas;                         // the ingame UICanvas

    public GameObject NBHstatsPanel;                        // The panel that shows the statistics of all the NBHs

    public GameObject NBHinfoButton;                        // button that collapses/expands the NBHstatpanel

    [SerializeField]
    private GameObject phaseManager;                        // the gameOjbect with the phasemanager attached to this

    [SerializeField]
    private GameObject NBHstatsPanelCollapsed;              // the collapsed version of the NBHstatspanel

    [SerializeField]
    private Transform singleNBHinfo;                        // the panel that shows information about the selected NBH
    private Text[] singleNBHMeasurementDuration = new Text[3]; // The duration of the measurements per NBH in text

    [SerializeField]
    private Transform NBHParent;                            // the parent of all NBH gameObjects

    [SerializeField]
    private GameObject cameraPivot;                         // the position/angle of the camera, used for moving the camera to NBHs

    private Vector3 cameraTargetPos;                        // the position the camera will move to to zoom in on a NBH
    private Quaternion cameraTargetRot;                     // same, but the rotation
    private float cameraLerpFloat = 1;                      // used for lerping the camera to the cameratarget

    [SerializeField]
    private GameObject cityReport;                          // The city report, showing what happened to the NBHs after a phase
    [SerializeField]
    private Transform[] cityReportNBHs;                     // the sliders in the cityReport, showing the % flooding of that NBH
    [HideInInspector] public float[] waterAddition = new float[6]; // the amount of water that got added to a neighbourhood. Is set in neighbourhoodstats
    [SerializeField]
    private Text cityReportStats;                           // Other information shown in cityReport (season left, funds and happiness)

    [HideInInspector]
    public bool gameEnded;                                  // check if the game is ended

    private int seasonsLeft;                                // counts how many seasons are left before the game ends

    // lerping variables
    private float cityReportLerpFloat = 1;                  // used for lerping the NBH bars
    private int lerpingNBH;                                 // Indicates which bar is being lerped
    private float[] waterAmountStart = new float[6];        // the starting amount of water, needed for lerping
    private float[] waterAmountEnd = new float[6];          // the final amount of water, needed for lerping
    private float currentWater;                             // the current water during the lerp

    void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            singleNBHMeasurementDuration[i] = singleNBHinfo.GetChild(1).GetChild(i).GetChild(0).GetComponent<Text>();
        }
        seasonsLeft = 6;
    }

    void Update ()
    {
        if (cameraLerpFloat < 1)
        {
            MoveCam();
        }
        if (cityReportLerpFloat < 1)
        {
            CityReportVisuals();
        }
    }
    // switch the canvas from one scene to another (menu or ingame)
    public void SwitchCanvas(GameObject targetCanvas, bool gameOver)
    {
        if (targetCanvas == gameUICanvas)
        {
            menuCanvas.SetActive(false);
            gameUICanvas.SetActive(true);
        }

        else if (targetCanvas == menuCanvas && gameOver)
        {
            gameUICanvas.SetActive(false);
            menuCanvas.SetActive(true);

            menuCanvas.transform.Find("LoseText").gameObject.SetActive(true);
        }
    } 

    // expand or collaps the neighbourhoodstatspanel with the button
    public void collapsNBHstats()
    {
        if (NBHstatsPanel.active)
        {
            NBHstatsPanel.SetActive(false);
            NBHstatsPanelCollapsed.SetActive(true);
            NBHinfoButton.transform.GetChild(0).GetComponent<Text>().text = "V";
        }
        else
        {
            NBHstatsPanel.SetActive(true);
            NBHstatsPanelCollapsed.SetActive(false);
            NBHinfoButton.transform.GetChild(0).GetComponent<Text>().text = "^";
        }
    }

    // move the camera to the NBH when the text of that NBH is clicked
    public void MoveToNBH(int NBHnr)
    {
        if (!NBHParent.GetChild(NBHnr).GetComponent<NeighbourhoodStats>().flooded)
        {
            cameraTargetPos = (NBHParent.GetChild(NBHnr).GetChild(0).transform.position);
            cameraTargetRot = NBHParent.GetChild(NBHnr).GetChild(0).transform.rotation;
            cameraLerpFloat = 0;
            GetComponent<NBHManager>().SetNewTarget(NBHParent.GetChild(NBHnr).gameObject);
            NBHinfoButton.transform.parent.gameObject.SetActive(false);

            singleNBHinfo.gameObject.SetActive(true);
            singleNBHinfo.GetChild(0).GetComponent<Text>().text = NBHstatsPanel.transform.GetChild(NBHnr + 1).GetComponent<Text>().text;
            singleNBHinfo.GetChild(0).GetChild(0).GetComponent<Text>().text = NBHstatsPanel.transform.GetChild(NBHnr + 1).GetChild(0).GetComponent<Text>().text;
            SetMeasurementDuration(NBHnr);

            // remove the information of all the NBHs, except the one needed
            for (int i = 0; i < 6; i++)
            {
                NBHParent.GetChild(i).GetChild(2).gameObject.SetActive(false);
                NBHParent.GetChild(i).GetChild(3).gameObject.SetActive(false);
            }
            NBHParent.GetChild(NBHnr).GetChild(2).gameObject.SetActive(true);
            NBHParent.GetChild(NBHnr).GetChild(3).gameObject.SetActive(true);
        }   
    }

    // show how long the measurements still lasts of the selected NBH
    // Also use colors to indicate if a measurement is active or not
    public void SetMeasurementDuration(int NBHnr)
    {
        for (int i = 0; i < 3; i++)
        {
            singleNBHMeasurementDuration[i].text = NBHParent.transform.GetChild(NBHnr).GetComponent<NeighbourhoodStats>().measurementDurationIndicators[i].text;

            if (singleNBHMeasurementDuration[i].text == 0.ToString())
            {
                singleNBHMeasurementDuration[i].fontStyle = FontStyle.Normal;
                singleNBHMeasurementDuration[i].color = Color.grey;
            }
            else
            {
                singleNBHMeasurementDuration[i].fontStyle = FontStyle.Bold;
                singleNBHMeasurementDuration[i].color = Color.green;
            }
        }
    }

    // return the camera to the starting point
    public void ReturnCam()
    {
        cameraTargetPos = new Vector3(0, 45, -80);
        cameraTargetRot = Quaternion.Euler(40, 0, 0);
        cameraLerpFloat = 0;
        NBHinfoButton.transform.parent.gameObject.SetActive(true);
    }

    // Move the camera to the selected NBH
    private void MoveCam()
    {
        cameraLerpFloat += Time.deltaTime / 1.5f;
        cameraPivot.transform.position = Vector3.Lerp(cameraPivot.transform.position, cameraTargetPos, cameraLerpFloat);
        cameraPivot.transform.rotation = Quaternion.Lerp(cameraPivot.transform.rotation, cameraTargetRot, cameraLerpFloat);
    }

    // Highlight the text of the NBH if the mouse hovers over it
    public void highlightText(int nr)
    {
        if (NBHstatsPanel.transform.GetChild(nr).GetComponent<Text>().color == Color.black)
        {
            NBHstatsPanel.transform.GetChild(nr).GetComponent<Text>().color = Color.white;
        }
    }

    // remove the highlight if the mouse no longer hovers over it
    public void highlightTextRemove(int nr)
    {
        if (NBHstatsPanel.transform.GetChild(nr).GetComponent<Text>().color == Color.white)
        {
            NBHstatsPanel.transform.GetChild(nr).GetComponent<Text>().color = Color.black;
        }
    }


    // Show the cityreport. The bars of the cityreport move from their previous position to their new position, based on the flooding%
    // the barcolor changes from green -> yellow -> orange -> red depending on how much danger that NBH is in
    private void CityReportVisuals()
    {
        cityReportLerpFloat += Time.deltaTime * 4;

        currentWater = Mathf.Lerp(waterAmountStart[lerpingNBH], waterAmountEnd[lerpingNBH], cityReportLerpFloat);

        cityReportNBHs[lerpingNBH].GetChild(1).GetComponent<Slider>().value = currentWater; // water before, water after 
        Color barColor;
        if (currentWater <= 25)
            barColor = Color.green;
        else if (currentWater <= 50)
            barColor = Color.yellow;
        else if (currentWater <= 75)
            barColor = new Color(1, 0.5f, 0.1f); // orange
        else
            barColor = Color.red;
        cityReportNBHs[lerpingNBH].GetChild(1).GetChild(1).GetChild(0).GetComponent<Image>().color = barColor;

        waterAmountStart[lerpingNBH] = waterAmountEnd[lerpingNBH];

        if (cityReportLerpFloat >= 1 && lerpingNBH != 5)
        {
            cityReportLerpFloat = 0;
            lerpingNBH++;
        }
    }

    // show and update the city report. This contains the static part, which is all text
    public void ShowCityReport()
    {
        cityReport.SetActive(true);

        // show the water added for each city in text
        for (int i = 0; i < cityReportNBHs.Length; i++)
        {
            string additionSymbol = "+";
            if (waterAddition[i] < 0)
                additionSymbol = "";
            cityReportNBHs[i].GetChild(0).GetComponent<Text>().text = NBHParent.GetChild(i).name + ": " + additionSymbol + waterAddition[i] + "%";
            
            waterAmountEnd[i] = NBHParent.GetChild(i).GetComponent<NeighbourhoodStats>().floodedPercentage;
        }

        // show the other stats
        cityReportStats.text = 
            "Seasons left: " + seasonsLeft + "\n" +
            "Funds: " + (PhaseManager.funds + 250) + " (+ " + GetComponent<BalancingScript>().periodicFunds + ") \n" +
            "City Happiness: " + phaseManager.GetComponent<PhaseManager>().currentHappiness;

        seasonsLeft--;
        lerpingNBH = 0;
        cityReportLerpFloat = 0;
    }

    // hide the cityreport when its no longer needed
    public void HideCityReport()
    {
        cityReport.SetActive(false);
    }
}
