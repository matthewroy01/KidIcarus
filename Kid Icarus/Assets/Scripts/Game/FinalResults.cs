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
    private PlayerUI refPlayerUI;

    private void Start()
    {
        refPlayerMovement = GameObject.FindObjectOfType<PlayerMovement>();
        refPlayerShoot = GameObject.FindObjectOfType<PlayerShoot>();
        refOrne = GameObject.FindObjectOfType<EnemyOrne>();
        refPlayerCollision = GameObject.FindObjectOfType<PlayerCollision>();
        refPlayerUI = GameObject.FindObjectOfType<PlayerUI>();
    }

    public void CalculateFinalResults()
    {
        if (!refPlayerUI.isTutorial)
        {
            // calculate meters
            int tmpMeters = refPlayerCollision.getCurrentMeters() + refPlayerUI.startingMeterOffset;

            // display the text
            display.text = refPlayerMovement.extraJumps + "/" + refPlayerMovement.extraJumpsMax + "\n" +
                            refPlayerShoot.arrowChargeLevel + "/" + refPlayerShoot.arrowLevelMax + "\n" +
                            refPlayerShoot.arrowRangeLevel + "/" + refPlayerShoot.arrowLevelMax + "\n" +
                            refPlayerShoot.arrowHomingLevel + "/" + refPlayerShoot.arrowLevelMax + "\n" +
                            refOrne.GetMetersSentBack() + "m" + "\n" +
                            refPlayerCollision.GetHeartsCollected() + "\n" +
                            refPlayerCollision.GetHeartsSpent() + "\n" +
                            "\n" +
                            tmpMeters + "m";
        }
        else
        {
            display.text = "";
        }
    }
}
