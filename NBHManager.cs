using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NBHControl : MonoBehaviour {

    [SerializeField]
    private GameObject selectedMeasurementDisplay;

    public string selectedMeasurement;

    [SerializeField]
    private Transform NBHinfoPanel;

    [SerializeField]
    private Transform singleNBHinfo;

    [SerializeField]
    private GameObject happinessText;

    [SerializeField]
    private GameObject durationText;

    [SerializeField]
    private Transform imageParent;


    [SerializeField]
    private GameObject phaseManager;

    [HideInInspector] public GameObject selectedTarget;      // the object that is currently selected

    public Button greenRoofButton;
    public Button gutterButton;
    public Button porousButton;

    //public GameObject placementButtons;
    public GameObject controlButtons;

    [SerializeField]
    private Text fundsText;

    public GameObject camPivot;  // the camera pivot point
    private Camera cam;          // the camera

	// Use this for initialization
	void Start ()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (PhaseManager.funds < GetComponent<BalancingScript>().greenroofCost)
        {
            greenRoofButton.GetComponent<Image>().color = Color.red;
        }
        if (PhaseManager.funds < GetComponent<BalancingScript>().guttersCost)
        {
            gutterButton.GetComponent<Image>().color = Color.red;
        }
        if (PhaseManager.funds < GetComponent<BalancingScript>().pavingCost)
        {
            porousButton.GetComponent<Image>().color = Color.red;
        }
    }

    // if a new neighbourhood is selected. Is called from neighbourhoodstats by clicking on a neighbourhood
    public void SetNewTarget(GameObject neighbourhood)
    {
        if (selectedTarget != null)
        {
            CancelSelection();
        }

        selectedTarget = neighbourhood;

        neighbourhood.GetComponent<NeighbourhoodStats>().neighbourhoodInfo.color = Color.green;

        controlButtons.SetActive(true);
        ButtonCheckMark();

        greenRoofButton.interactable = true;
    }

    public void SelectGreenRoofs()
    {
        if (selectedTarget.tag == "Neighbourhood" && PhaseManager.funds >= GetComponent<BalancingScript>().greenroofCost && selectedTarget.GetComponent<NeighbourhoodStats>().greenroofs == 0 && TutorialManager.tutorialPhase >= 2)
        {
            selectedMeasurement = "GreenRoofs";
            SetImage(1);

            happinessText.GetComponent<GetHappiness>().UpdateHappiness();
            durationText.GetComponent<GetDuration>().UpdateDuration();

            selectedMeasurementDisplay.transform.GetChild(0).GetComponentInChildren<Text>().text = selectedMeasurement;

            //TODO: Set an inactive greenroof component of the selected neighbourhood to active
        }
    }

    public void SelectGutters()
    {
        if (selectedTarget.tag == "Neighbourhood" && PhaseManager.funds >= GetComponent<BalancingScript>().guttersCost && selectedTarget.GetComponent<NeighbourhoodStats>().gutters == 0 && TutorialManager.tutorialPhase >= 7)
        {
            selectedMeasurement = "Gutters";
            SetImage(2);

            happinessText.GetComponent<GetHappiness>().UpdateHappiness();
            durationText.GetComponent<GetDuration>().UpdateDuration();

            selectedMeasurementDisplay.transform.GetChild(0).GetComponentInChildren<Text>().text = selectedMeasurement;

            //TODO: Set an inactive gutter component of the selected neighbourhood to active
        }
    }

    public void SelectPorousPaving()
    {
        if (selectedTarget.tag == "Neighbourhood" && PhaseManager.funds >= GetComponent<BalancingScript>().pavingCost && selectedTarget.GetComponent<NeighbourhoodStats>().porousPaved == 0 && TutorialManager.tutorialPhase >= 7)
        {
            selectedMeasurement = "PorousPaving";
            SetImage(3);

            happinessText.GetComponent<GetHappiness>().UpdateHappiness();
            durationText.GetComponent<GetDuration>().UpdateDuration();

            selectedMeasurementDisplay.transform.GetChild(0).GetComponentInChildren<Text>().text = selectedMeasurement;

            //TODO: Set an inactive paving component of the selected neighbourhood to active
        }
    }

    public void BuySelectedMeasurement()
    {
        if (selectedMeasurement == "GreenRoofs" && PhaseManager.funds >= GetComponent<BalancingScript>().greenroofCost && selectedTarget.GetComponent<NeighbourhoodStats>().greenroofs == 0)
        {
            if (TutorialManager.tutorialPhase == 2)
                GetComponent<TutorialManager>().ToNextPhase(3);

            PhaseManager.funds -= GetComponent<BalancingScript>().greenroofCost;
            fundsText.text = "Funds: " + PhaseManager.funds;

            phaseManager.GetComponent<PhaseManager>().RemoveHappiness(GetComponent<BalancingScript>().greenroofHappiness * -1);

            selectedTarget.GetComponent<NeighbourhoodStats>().greenroofs = GetComponent<BalancingScript>().greenroofDuration;
            selectedTarget.GetComponent<NeighbourhoodStats>().setMeasurementDuration();

            greenRoofButton.transform.GetChild(1).gameObject.SetActive(true);

            selectedMeasurementDisplay.transform.GetChild(0).GetComponentInChildren<Text>().text = null;
            happinessText.GetComponent<GetHappiness>().UpdateHappiness();
            durationText.GetComponent<GetDuration>().UpdateDuration();
        }
        else if (selectedMeasurement == "Gutters" && PhaseManager.funds >= GetComponent<BalancingScript>().guttersCost && selectedTarget.GetComponent<NeighbourhoodStats>().gutters == 0)
        {
            PhaseManager.funds -= GetComponent<BalancingScript>().guttersCost;
            fundsText.text = "Funds: " + PhaseManager.funds;

            phaseManager.GetComponent<PhaseManager>().RemoveHappiness(GetComponent<BalancingScript>().gutterHappiness * -1);

            selectedTarget.GetComponent<NeighbourhoodStats>().gutters = GetComponent<BalancingScript>().gutterDuration;
            selectedTarget.GetComponent<NeighbourhoodStats>().setMeasurementDuration();
            gutterButton.transform.GetChild(1).gameObject.SetActive(true);

            selectedMeasurementDisplay.transform.GetChild(0).GetComponentInChildren<Text>().text = null;
            happinessText.GetComponent<GetHappiness>().UpdateHappiness();
            durationText.GetComponent<GetDuration>().UpdateDuration();
        }
        else if (selectedMeasurement == "PorousPaving" && PhaseManager.funds >= GetComponent<BalancingScript>().pavingCost && selectedTarget.GetComponent<NeighbourhoodStats>().porousPaved == 0)
        {
            PhaseManager.funds -= GetComponent<BalancingScript>().pavingCost;
            fundsText.text = "Funds: " + PhaseManager.funds;

            phaseManager.GetComponent<PhaseManager>().RemoveHappiness(GetComponent<BalancingScript>().pavingHapiness * -1);

            selectedTarget.GetComponent<NeighbourhoodStats>().porousPaved = GetComponent<BalancingScript>().pavingDuration;
            selectedTarget.GetComponent<NeighbourhoodStats>().setMeasurementDuration();
            porousButton.transform.GetChild(1).gameObject.SetActive(true);

            selectedMeasurementDisplay.transform.GetChild(0).GetComponentInChildren<Text>().text = null;
            happinessText.GetComponent<GetHappiness>().UpdateHappiness();
            durationText.GetComponent<GetDuration>().UpdateDuration();
        }

        phaseManager.GetComponent<PhaseManager>().UpdateCurrentScore();
    }

    // set the checkmark over a button when it is placed
    private void ButtonCheckMark()
    {
        if (selectedTarget.GetComponent<NeighbourhoodStats>().greenroofs > 0)
        {
            greenRoofButton.transform.GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            greenRoofButton.transform.GetChild(1).gameObject.SetActive(false);
        }
        if (selectedTarget.GetComponent<NeighbourhoodStats>().gutters > 0)
        {
            gutterButton.transform.GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            gutterButton.transform.GetChild(1).gameObject.SetActive(false);
        }
        if (selectedTarget.GetComponent<NeighbourhoodStats>().porousPaved > 0)
        {
            porousButton.transform.GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            porousButton.transform.GetChild(1).gameObject.SetActive(false);
        }
    }

    // activates when the cancelbutton is pressed
    public void CancelButton()
    {
        if (TutorialManager.tutorialPhase == 3)
        {
            GetComponent<TutorialManager>().ToNextPhase(4);
        }
        if (TutorialManager.tutorialPhase > 3)
        {
            CancelSelection();
        }
    }

    // deselect the current NBH, which will remove all UI involved with it
    public void CancelSelection()
    {
        controlButtons.SetActive(false);
        NBHinfoPanel.gameObject.SetActive(true);
        GetComponent<UIControllerScript>().ReturnCam();
        singleNBHinfo.gameObject.SetActive(false);

        selectedTarget.GetComponent<NeighbourhoodStats>().neighbourhoodInfo.color = Color.black;

        selectedTarget = null;
    }

    // set the image for the selected NBH
    private void SetImage(int imageNr)
    {
        foreach (Transform child in imageParent)
            child.gameObject.SetActive(false);

        imageParent.GetChild(imageNr).gameObject.SetActive(true);
    }
}
