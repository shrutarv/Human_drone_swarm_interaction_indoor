using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Threading;

public class GameManager : MonoBehaviour 
{   


    /// <summary>
    /// a list of boids is used to update the velocity of gameObjects that are subscribed in the list using flocking.
    /// </summary>
    private List<Boid>      _boids;

    /// <summary>
    /// a reference to the boids list that other classes can use to add, remove or loop through the boids.
    /// </summary>
    public List<Boid>       boids { get { return _boids; } }

    public int speed        { get; set; }

    /// <summary>
    /// The three elements that determine the velocity of boids.
    /// </summary>
    private Alignment   _alignment;
    private Cohesion    _cohesion;
    private Separation  _separation;

	void Awake( ) 
    {
        _boids                  = new List<Boid>( );
        _alignment              = new Alignment( );
        _cohesion               = new Cohesion( );
        _separation             = new Separation( );
        //_alignment.minDist = 0;
        //_alignment.maxDist = 24;
        //_alignment.scalar = 6;
        //_cohesion.minDist = 19;
        //_cohesion.maxDist = 82;
        //_cohesion.scalar = 2;
        //_separation.minDist = 0;
        //_separation.maxDist = 65;
        //_separation.scalar = 3;
        speed = 100;


    }

	// Update is called once per frame
	void Update( )
    {
        /*new Thread( ( ) =>
        {
            //run through all boids.
            for ( int i = _boids.Count - 1; i >= 0; --i )
            {

                Boid b         = _boids[i];
                //get the boids current velocity.
                Vector3 velocity    = b.velocity;

                //add the influences of neighboring boids to the velocity.
                velocity += _alignment.getResult( _boids, i );
                velocity += _cohesion.getResult( _boids, i );
                velocity += _separation.getResult( _boids, i );

                //normalize the velocity and make sure that the boids new velocity is updated.
                velocity.Normalize( );
                b.velocity = velocity;
                              
                b.lookat    = b.position + velocity;
            }
        } ).Start( );

        for ( int i = _boids.Count - 1; i >= 0; --i )
        {
            //update the boids position in the mainthread.
            _boids[i].transform.position += _boids[i].velocity * Time.deltaTime * speed;           
        }*/
    }

}
