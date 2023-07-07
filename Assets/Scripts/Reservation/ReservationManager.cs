using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using Helper;
using Dependency;

namespace Reservation
{

    public class ReservationManager : MonoBehaviour
    {
        public static int globalCounter = 0;
        public int id = globalCounter++;

        public ReservationList reservationList;

        Material collisionMaterial;
        Material highlightMaterial;
        Material rememberMaterial = null;

        ReservationManager masterReservationManager;
        HashSet<ReservationManager> clientSet = new HashSet<ReservationManager>();
        public string MasterName = "No Master";

        public EventHistory history = new EventHistory();

        public DependencyManager dependencyManager;

        // Start is called before the first frame update
        void Start()
        {
            reservationList = new ReservationList(this);
            collisionMaterial = Resources.Load("Materials/Walls", typeof(Material)) as Material;
            highlightMaterial = Resources.Load("Materials/HighlightYellow", typeof(Material)) as Material;
            var agentManager = GameObject.Find("AgentManager");
            dependencyManager = agentManager?.GetComponent<DependencyManager>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void LogHistory()
        {
            Debug.Log(history.Text(), gameObject);
        }

        public void ToggleHighlight()
        {
            if (rememberMaterial == null)
            {
                rememberMaterial = gameObject.GetComponent<MeshRenderer>().material;
                gameObject.GetComponent<MeshRenderer>().material = highlightMaterial;
            }
            else
            {
                gameObject.GetComponent<MeshRenderer>().material = rememberMaterial;
                rememberMaterial = null;
            }
        }

        public int ReserveSlot(ReservationManager resource, int pathStepIndex, int timeStamp, IResourceUser ru)
        {
            if (masterReservationManager != null)
            {
                return masterReservationManager.ReserveSlot(resource ?? (this), pathStepIndex, timeStamp, ru); //time stamp is the counter for reservations of a path
            }

            return reservationList.ReserveSlot(resource ?? (this), pathStepIndex, timeStamp, ru);
        }

        public bool EntryPermitted(IResourceUser ru)
        {
            if (masterReservationManager != null)
            {
                return masterReservationManager.EntryPermitted(ru);
            }

            return reservationList.EntryPermitted(ru);
        }


        public void Entering(IResourceUser ru, int pathIndex)
        {
            if (masterReservationManager != null)
            {
                masterReservationManager.Entering(ru, pathIndex);
            }
            else
            {
                reservationList.Entering(ru, pathIndex);
            }
        }


        public void DeleteSlot(IResourceUser ru) //as soon as a new master is assigned anywhere on a already reserved path all slots that the agent had reserved before need to be deleted (and a new reservation started)
        {
            if (masterReservationManager != null)
            {
                masterReservationManager.DeleteSlot(ru);
                return;
            }

            reservationList.DeleteSlot(ru);
        }



        public void NotifyExit(IResourceUser ru, int pathIndex)
        {
            if (masterReservationManager != null)
            {
                masterReservationManager.NotifyExit(ru, pathIndex);
            }
            else
            {
                reservationList.NotifyExit(ru, pathIndex);
            }
        }


        void OnTriggerEnter(Collider col)
        {
            // If the object contains another reservation manager (is part of the path envelope) notify about collision
            if (col.gameObject.GetComponent<ReservationManager>() != null)
            {
                //Debug.Log("Collision: 1. "+ gameObject.name + " <-> 2. " + col.gameObject.name);

                if (gameObject.GetComponent<MeshRenderer>() != null)
                {
                    gameObject.GetComponent<MeshRenderer>().material = collisionMaterial;
                }

                var otherOne = col.gameObject.GetComponent<ReservationManager>();

                if (otherOne.masterReservationManager == null && this.masterReservationManager == null)
                {
                    if (otherOne.reservationList.clock >= this.reservationList.clock)
                    //if (otherOne.id > this.id)
                    {
                        otherOne.SetMasterReservationManager(this);
                    }
                    else
                    {
                        this.SetMasterReservationManager(otherOne);
                    }
                    return;
                }
                if (otherOne.masterReservationManager != null && this.masterReservationManager == null && otherOne.masterReservationManager != this)
                {
                    if (otherOne.masterReservationManager.reservationList.clock <= this.reservationList.clock)
                    //if (otherOne.masterReservationManager.id < this.id)
                    {
                        this.SetMasterReservationManager(otherOne.masterReservationManager);
                    }
                    else
                    {
                        otherOne.masterReservationManager.SetMasterReservationManager(this);
                    }
                    return;
                }
                if (otherOne.masterReservationManager == null && this.masterReservationManager != null && this.masterReservationManager != otherOne)
                {
                    if (otherOne.reservationList.clock >= this.masterReservationManager.reservationList.clock)
                    //if (otherOne.id > this.masterReservationManager.id)
                    {
                        otherOne.SetMasterReservationManager(this.masterReservationManager);
                    }
                    else
                    {
                        this.masterReservationManager.SetMasterReservationManager(otherOne);
                    }
                    return;
                }
                if (otherOne.masterReservationManager != null && this.masterReservationManager != null && otherOne.masterReservationManager != this.masterReservationManager)
                {
                    if (otherOne.ResolveMaster().reservationList.clock >= this.ResolveMaster().reservationList.clock)
                    //if (otherOne.ResolveMaster().id > this.ResolveMaster().id)
                    {
                        otherOne.ResolveMaster().SetMasterReservationManager(this.ResolveMaster());
                    }
                    else
                    {
                        this.ResolveMaster().SetMasterReservationManager(otherOne.ResolveMaster());
                    }
                    return;
                }
            }
        }

        public ReservationManager ResolveMaster()
        {
            if (this.masterReservationManager == null)
            {
                return this;
            }
            else
            {
                return this.masterReservationManager.ResolveMaster();
            }
        }


        public void SetMasterReservationManager(ReservationManager master)
        {

            if (this == master)
            {
                Debug.Log(this.gameObject.name + ": Cannot set myself as master.");
                return;
            }


            if (this.masterReservationManager != null && master != null)
            {
                Debug.Log(this.gameObject.name + ": Master already set.");
            }
            else if (this.masterReservationManager == null && master != null)
            {
                master.history.Add(master.reservationList.clock, $"{name} is my client now.");
                history.Add(reservationList.clock, $"My new master is {master.name}");
                this.masterReservationManager = master;
                this.MasterName = master.gameObject.name;
                master.clientSet.Add(this);

                history.Add(reservationList.clock, $"       Merging...");
                master.reservationList.MergeReservationList(this, this.reservationList.reservationList, reservationList.clock, reservationList.nextSlot);
                history.Add(reservationList.clock, $"       ...finished");

                //TODO: if master is null check if we need to inform resource users
                //InformResourceUsersAboutNewMasterAndClearReservationList(master);

                //reservationList._currentlyPresentResourceUsers = new HashSet<IResourceUser>();
                //reservationList.debugReservationList.Clear();
                //inform my clients about my new master to prevent masterchaining
                foreach (ReservationManager client in clientSet)
                {
                    history.Add(reservationList.clock, $"       Inform client {client.name} about new master");
                    //client.masterReservationManager = master;
                    client.SetMasterReservationManager(null);
                    client.SetMasterReservationManager(master);
                }
                clientSet.Clear();

                history.Add(reservationList.clock, $"       Resetting my reservation list");
                reservationList = new ReservationList(this);
            }
            else if (this.masterReservationManager != null && master == null)
            {
                this.masterReservationManager = null;
                this.MasterName = "No Master";
            }

        }


        void OnDisable()
        {
            //Debug.Log("Disabled " + gameObject.name);
            history.Add(reservationList.clock, $"I was disabled.");
            masterReservationManager?.ResolveMaster().DeleteClient(this);
            AppointNewMasterAndInformClients();
        }


        private void AppointNewMasterAndInformClients()
        {
            if (clientSet.Count > 0)
            {
                var newMaster = clientSet.First();
                clientSet.Remove(newMaster);
                history.Add(reservationList.clock, $"I made {newMaster.name} the new master.");
                newMaster.history.Add(reservationList.clock, $"{name} made me the new master.");
                //Debug.Log("Appointed new master: " + newMaster.gameObject.name);
                newMaster.SetMasterReservationManager(null);

                newMaster.reservationList.clock = this.reservationList.clock;
                newMaster.reservationList.nextSlot = this.reservationList.nextSlot;
                newMaster.reservationList = this.reservationList;
                newMaster.reservationList._manager = newMaster;
                newMaster.reservationList.debugReservationList = this.reservationList.debugReservationList;

                newMaster.reservationList._currentlyPresentResourceUsers.UnionWith(this.reservationList._currentlyPresentResourceUsers);
                newMaster.history.Add(newMaster.reservationList.clock, $"     Currently present users: {newMaster.reservationList.CurrentlyPresentResourceUsers}");
                foreach (IResourceUser ru in newMaster.reservationList._currentlyPresentResourceUsers)
                {
                    ru.RegisterReservationManager(newMaster);
                }
                //this.reservationList._currentlyPresentResourceUsers = new HashSet<IResourceUser>();
                this.reservationList = new ReservationList(this);




                foreach (ReservationManager client in clientSet)
                {
                    newMaster.history.Add(newMaster.reservationList.clock, $"     {client.name} is my client.");
                    //Debug.Log("Tell client " + client.gameObject.name + " about new master.");
                    client.SetMasterReservationManager(null);
                    client.SetMasterReservationManager(newMaster);
                }

                StringBuilder debug = new StringBuilder();
                debug.Append("     Reservation list (clock: " + newMaster.reservationList.clock + ", nextSlot: " + newMaster.reservationList.nextSlot + ")\n");
                newMaster.reservationList.reservationList.ToList().ForEach(kvp => debug.Append("         " + kvp.Key + ": " + kvp.Value.User.Name + ", " + kvp.Value.PathIndex + "\n"));
                newMaster.history.Add(newMaster.reservationList.clock, debug.ToString());
            }

        }


        private void DeleteClient(ReservationManager client)
        {
            clientSet.Remove(client);
        }




    }

}
