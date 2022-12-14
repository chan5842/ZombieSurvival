using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AmmoPack : MonoBehaviourPun, IItem
{
    public int ammo = 30;
    public void Use(GameObject target)
    {
        PlayerShooter playerShooter = target.GetComponent<PlayerShooter>();
        if(playerShooter != null && playerShooter.gun != null)
        {
            playerShooter.gun.photonView.RPC("AddAmo", RpcTarget.All, ammo);
        }

        // �������� �Ծ����� ����
        PhotonNetwork.Destroy(gameObject);
    }
}
