using UnityEngine;
using System.Collections;

public class Boid 
{
    public int Id { get; set; }
    public Transform Leader { get; set; }
    public Transform Body { set; get; }
    public float SeparationDistance { set; get; } = 0.6f;
}
