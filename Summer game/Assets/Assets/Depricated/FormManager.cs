using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormManager : MonoBehaviour
{
    /// <summary>
    /// Controls form swapping and mechanics
    /// </summary>

    private string currentForm;
    private string previousForm;
    private List<string> availableForms = new List<string>();

    public bool khepriForm = false;
    public bool khnumForm = false;
    public bool raForm = false;
    public bool osirisForm = false;

    private bool ableToSwap = true;

    private float swapTimer = 10f;

    public delegate void FormSwap(string newForm);
    public static event  FormSwap OnFormSwap;

	// Use this for initialization
	void Start ()
    {
        availableForms.Add("Khepri");
        availableForms.Add("Khnum");
        availableForms.Add("Ra");
        availableForms.Add("Osiris");
    }
	
	// Update is called once per frame
	void Update ()
    {
        swapTimer -= Time.deltaTime;
        if(swapTimer <= 0)
        {
            ableToSwap = true;
            swapTimer = 10f;
        }
	}

    //Enables the unique abiliy for the form that you are in
    public void EnableFormAbility(string newForm)
    {
        switch (newForm)
        {
            case "Khepri":
                Debug.Log("Swapped to Khepri");
                khepriForm = true;
                khnumForm = false;
                raForm = false;
                osirisForm = false;
                break;
            case "Khnum":
                Debug.Log("Swapped to Khnum");
                khepriForm = false;
                khnumForm = true;
                raForm = false;
                osirisForm = false;
                break;
            case "Ra":
                Debug.Log("Swapped to Ra");
                khepriForm = false;
                khnumForm = false;
                raForm = true;
                osirisForm = false;
                break;
            case "Osiris":
                Debug.Log("Swapped to Osiris");
                khepriForm = false;
                khnumForm = false;
                raForm = false;
                osirisForm = true;
                break;
        }
    }

    //Changes to the defined new form
    public void ChangeForm(string newForm)
    {
        if(ableToSwap && availableForms.Contains(newForm))
        {
            EnableFormAbility(newForm);
            ableToSwap = false;
        }
    }

    //This is here for the eventsystem, currently not in use
    private void FormSwitch(string newForm, string previousForm)
    {
        OnFormSwap(newForm);
    }

    //Returns the current form
    public string GetCurrentForm()
    {
        return currentForm;
    }

    //Returns the previous form. 
    public string GetPreviousForm()
    {
        return previousForm;
    }

    //Adds a form to the list of usable forms, once unlocked
    public void AddAvailableForm(string formToAdd)
    {
        availableForms.Add(formToAdd);
    }
}
