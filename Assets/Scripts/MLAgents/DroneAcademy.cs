using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class DroneAcademy : Academy
{

    GameObject MLDrones;
    MLSwarmSimulationManager MLSwarmSimulationManager;

    public override void InitializeAcademy()
    {
        //Initialization is performed once in an Academy object's life cycle. 
        //Use the InitializeAcademy() method for any logic you would normally perform in the standard Unity Start() or Awake() methods.

        

    }

    public override void AcademyStep()
    {
        //Implement an AcademyReset() function to alter the environment at the start of each episode.
    }

    public override void AcademyReset()
    {
        //The AcademyStep() function is called at every step in the simulation before any Agents are updated. 
        //Use this function to update objects in the environment at every step or during the episode between environment resets.

        //MLDrones = GameObject.Find("MLDrones");
        //MLSwarmSimulationManager = MLDrones.GetComponent<MLSwarmSimulationManager>();

        //MLSwarmSimulationManager.ResetSimulation();

    }

}
