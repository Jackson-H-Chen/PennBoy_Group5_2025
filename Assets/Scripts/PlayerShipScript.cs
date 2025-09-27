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
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.tKey.wasPressedThisFrame)
        {
            ShowSampleDialog();
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
