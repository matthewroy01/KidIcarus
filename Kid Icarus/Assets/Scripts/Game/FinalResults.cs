using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinalResults : MonoBehaviour
{
   public Text display;

   // for roc's feathers
   private PlayerMovement refPlayerMovement;

   // for charge and range
   private PlayerShoot refPlayerShoot;

   // for meters Orne was sent back
   private EnemyOrne refOrne;

   // for heart totals
   private PlayerCollision refPlayerCollision;

   private void Start()
   {
      refPlayerMovement = GameObject.FindObjectOfType<PlayerMovement>();
      refPlayerShoot = GameObject.FindObjectOfType<PlayerShoot>();
      refOrne = GameObject.FindObjectOfType<EnemyOrne>();
      refPlayerCollision = GameObject.FindObjectOfType<PlayerCollision>();
   }

   void Update ()
   {
      display.text = refPlayerMovement.extraJumps + "/" + refPlayerMovement.extraJumpsMax + "\n" +
                     refPlayerShoot.arrowChargeLevel + "/" + refPlayerShoot.arrowLevelMax + "\n" +
                     refPlayerShoot.arrowRangeLevel + "/" + refPlayerShoot.arrowLevelMax + "\n" +
                     refOrne.GetMetersSentBack() + "m" + "\n" +
                     refPlayerCollision.GetHeartsCollected() + "\n" +
                     refPlayerCollision.GetHeartsSpent() + "\n" +
                     "\n" +
                     refPlayerCollision.getCurrentMeters() + "m";
   }
}
