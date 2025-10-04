using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShipScript : MonoBehaviour
{
    private TraitSystem traitSystem;

    void Awake()
    {
        traitSystem = GetComponent<TraitSystem>();
        if (traitSystem == null)
        {
            traitSystem = gameObject.AddComponent<TraitSystem>();
        }
        DialogUIController.Ensure();
        TraitsOverlayUI.Ensure();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
		// Seed three starter weapons with low stats if empty
		InventoryManager mgr = InventoryManager.Ensure();
		if (mgr.GetItems().Count == 0)
		{
			InventoryItem w1 = new InventoryItem("wpn_laser_mk1", "Laser Mk I", new Color(1f, 0.35f, 0.35f));
			w1.baseDamage = 12f; // out of 50
			w1.baseAccuracyPct = 35f; // %
			w1.baseShieldBypassPct = 10f; // %
			w1.baseArmorPenetration = 8f; // out of 50
			w1.armorShred = 3f; // out of 25
			mgr.AddItem(w1);

			InventoryItem w2 = new InventoryItem("wpn_rail_mk1", "Railgun Mk I", new Color(0.8f, 0.6f, 1f));
			w2.baseDamage = 18f;
			w2.baseAccuracyPct = 25f;
			w2.baseShieldBypassPct = 5f;
			w2.baseArmorPenetration = 15f;
			w2.armorShred = 6f;
			mgr.AddItem(w2);

			InventoryItem w3 = new InventoryItem("wpn_plasma_mk1", "Plasma Mk I", new Color(1f, 0.6f, 0.2f));
			w3.baseDamage = 14f;
			w3.baseAccuracyPct = 20f;
			w3.baseShieldBypassPct = 20f;
			w3.baseArmorPenetration = 10f;
			w3.armorShred = 10f;
			mgr.AddItem(w3);
		}
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.tKey.wasPressedThisFrame)
        {
            ShowSampleDialog();
        }
        if (Keyboard.current != null && Keyboard.current.cKey.wasPressedThisFrame)
        {
            TraitsOverlayUI.Ensure().Toggle(traitSystem);
        }
		if (Keyboard.current != null && Keyboard.current.iKey.wasPressedThisFrame)
		{
			InventoryUIController.Ensure().Toggle();
		}
    }

    private void ShowSampleDialog()
    {
        DialogConversation convo = new DialogConversation();
        convo.startNodeId = "intro";

        DialogNode intro = new DialogNode();
        intro.id = "intro";
        intro.speaker = "Station AI";
        intro.body = "Welcome, pilot. Do you accept the trial?";
        intro.options.Add(new DialogOption
        {
            text = "Accept trial (grants TRAIT_BRAVE)",
            grantTrait = "TRAIT_BRAVE",
            nextNodeId = "accepted",
            endsDialog = false
        });
        intro.options.Add(new DialogOption
        {
            text = "Decline (removes TRAIT_BRAVE)",
            removeTrait = "TRAIT_BRAVE",
            nextNodeId = "declined",
            endsDialog = false
        });
        intro.options.Add(new DialogOption
        {
            text = "Secret path (requires TRAIT_BRAVE)",
            requiredTrait = "TRAIT_BRAVE",
            nextNodeId = "secret",
            endsDialog = false
        });

        DialogNode accepted = new DialogNode();
        accepted.id = "accepted";
        accepted.speaker = "Station AI";
        accepted.body = "Courage noted. Proceed to the proving grounds.";
        accepted.options.Add(new DialogOption { text = "Continue", endsDialog = true });

        DialogNode declined = new DialogNode();
        declined.id = "declined";
        declined.speaker = "Station AI";
        declined.body = "Caution is wise. Return when you are ready.";
        declined.options.Add(new DialogOption { text = "Close", endsDialog = true });

        DialogNode secret = new DialogNode();
        secret.id = "secret";
        secret.speaker = "Station AI";
        secret.body = "Only the brave discover hidden routes.";
        secret.options.Add(new DialogOption { text = "Close", endsDialog = true });

        convo.nodes.Add(intro);
        convo.nodes.Add(accepted);
        convo.nodes.Add(declined);
        convo.nodes.Add(secret);

        DialogUIController.Ensure().ShowDialog(convo, traitSystem);
    }
}
