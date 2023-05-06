using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// @TODO: Add Parallax
public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float CameraOffesetX;
    [SerializeField] private float CameraOffesetY;

    private void Start()
    {
        transform.position = new Vector3(player.position.x + CameraOffesetX, player.position.y + CameraOffesetY, -1);

    }
    private void Update()
    {
        transform.position = new Vector3(player.position.x + CameraOffesetX, player.position.y + CameraOffesetY, transform.position.z);
    }
}
