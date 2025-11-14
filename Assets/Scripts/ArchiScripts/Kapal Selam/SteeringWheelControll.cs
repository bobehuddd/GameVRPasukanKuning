using UnityEngine;
 // **NEW: Required for XR Interaction Toolkit**
using UnityEngine.InputSystem; // Generally required for Action-Based XRI setup

// Add a Rigidbody and Collider (e.g., SphereCollider) to the Steering Wheel object for hand detection.
// Add a BoxCollider/SphereCollider to the Hand Interactors and tag them appropriately (e.g., "PlayerHand")
public class SteeringWheelControll : MonoBehaviour
{
    // RightHand
    // Changed GameObject to XRBaseInteractor for proper XRI event handling
    [Tooltip("The XR Direct Interactor for the Right Hand.")]
    public GameObject rightHandInteractorObject; // Changed name for clarity (this is the GameObject with the Interactor)
    private Transform rightHandOriginalParent;
    private bool rightHandOnWheel = false;
    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor rightHandInteractor; // Reference to the actual interactor component

    // LeftHand
    [Tooltip("The XR Direct Interactor for the Left Hand.")]
    public GameObject leftHandInteractorObject; // Changed name for clarity (this is the GameObject with the Interactor)
    private Transform leftHandOriginalParent;
    private bool lefttHandOnWheel = false;
    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor leftHandInteractor; // Reference to the actual interactor component

    public Transform[] snappPositions;

    private int numberOfHandOnWheel = 0; // Not used in the provided code, but kept

    //Object to controll with steeringwheel
    public GameObject Vehicle;
    private Rigidbody VehicleRigidbody;

    public float currentSteeringWheelRotation = 0;

    //dampening, semakin rendah angka dampening membuat rotasi saat belok semakin lambat
    private float turnDampening = 250;

    public Transform directionalObject;

    private void Start()
    {
        VehicleRigidbody = Vehicle.GetComponent<Rigidbody>();

        // **NEW: Get the Interactor components from the GameObjects**
        if (rightHandInteractorObject != null)
        {
            rightHandInteractor = rightHandInteractorObject.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor>();
            if (rightHandInteractor == null)
            {
                Debug.LogError("Right Hand Interactor Object does not have an XRBaseInteractor component.");
            }
        }
        if (leftHandInteractorObject != null)
        {
            leftHandInteractor = leftHandInteractorObject.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor>();
            if (leftHandInteractor == null)
            {
                Debug.LogError("Left Hand Interactor Object does not have an XRBaseInteractor component.");
            }
        }
    }

    void Update()
    {
        // Replaced OVRInput checks with XRI-compatible logic inside ReleaseHandFromWheel
        ReleaseHandFromWheel(); 

        ConvertHandRotationToSteeringWheelRotation();

        TurnVehicle();

        // Ensure the rotation is based on the directionalObject's Z-axis rotation relative to the world or its parent
        currentSteeringWheelRotation = -transform.localRotation.eulerAngles.z; 
    }

    // Fungsi NormalizeAngle dihilangkan dari tampilan untuk meringkas, tetapi harus ada di dalam script
    private void TurnVehicle()
    {
        // The directionalObject is what actually rotates based on the hands.
        // We use its rotation to determine the vehicle's turn angle.
        var turn = -directionalObject.localRotation.eulerAngles.z;
        
        // Normalize angle to be between -180 and 180 (original script's logic simplified)
        if (turn > 180f)
        {
            turn -= 360f;
        } 
        else if (turn < -180f)
        {
            turn += 360f;
        }

        // Map the steering wheel angle (e.g., -90 to 90) to a vehicle turning angle (e.g., -45 to 45)
        // You might need a factor here: VehicleRotation = turn * steeringRatio
        // Example: turn / 2 for a gentle turn.
        float vehicleTurnAngle = turn; 

        // Apply rotation to the vehicle (on the Y-axis for typical turning)
        // This line implements turning by rotating the vehicle body based on the steering wheel's rotation.
        VehicleRigidbody.MoveRotation(Quaternion.RotateTowards(
            Vehicle.transform.rotation, 
            Quaternion.Euler(0, vehicleTurnAngle, 0), 
            Time.deltaTime * turnDampening)
        );
    }

    private void ConvertHandRotationToSteeringWheelRotation()
    {
        // The original script's logic for determining the new rotation based on which hand is holding is fine.
        // We just ensure we are using the directionalObject as the anchor.
        
        if (rightHandOnWheel == true && lefttHandOnWheel == false)
        {
            // Use the original parent (the hand's original parent is the Interactor object)
            Quaternion newRot = Quaternion.Euler(0, 0, rightHandOriginalParent.transform.rotation.eulerAngles.z);
            directionalObject.rotation = newRot;
            transform.parent = directionalObject; // Parent the wheel to the directional object
        }
        else if (rightHandOnWheel == false && lefttHandOnWheel == true)
        {
            Quaternion newRot = Quaternion.Euler(0, 0, leftHandOriginalParent.transform.rotation.eulerAngles.z);
            directionalObject.rotation = newRot;
            transform.parent = directionalObject;
        }
        else if (rightHandOnWheel == true && lefttHandOnWheel== true)
        {
            Quaternion newRotLeft = Quaternion.Euler(0, 0, leftHandOriginalParent.transform.rotation.eulerAngles.z);
            Quaternion newRotRight = Quaternion.Euler(0, 0, rightHandOriginalParent.transform.rotation.eulerAngles.z);
            
            // Slerp to average the rotation when two hands are gripping
            Quaternion finalRot = Quaternion.Slerp(newRotLeft, newRotRight, 0.5f); // Use 0.5f for true average
            directionalObject.rotation = finalRot;
            transform.parent = directionalObject;
        }
        else
        {
            // If no hands on wheel, the directional object should maintain its current rotation
            // and the wheel should not be parented to it anymore.
            transform.parent = null;
        }
    }

    private void ReleaseHandFromWheel()
    {
        // **FIX: Replaced OVRInput.GetUp with a check on the Interactor's Select State**

        // Check if the right hand is on the wheel AND the interactor is no longer selecting (i.e., grip/trigger released)
        if (rightHandOnWheel == true && (rightHandInteractor == null || !rightHandInteractor.isSelectActive))
        {
            // Reset hand position and parent
            rightHandInteractorObject.transform.parent = rightHandOriginalParent;
            rightHandInteractorObject.transform.position = rightHandOriginalParent.position;
            rightHandInteractorObject.transform.rotation = rightHandOriginalParent.rotation;
            rightHandOnWheel = false;
        }

        // Check if the left hand is on the wheel AND the interactor is no longer selecting
        if (lefttHandOnWheel == true && (leftHandInteractor == null || !leftHandInteractor.isSelectActive))
        {
            // Reset hand position and parent
            leftHandInteractorObject.transform.parent = leftHandOriginalParent;
            leftHandInteractorObject.transform.position = leftHandOriginalParent.position;
            leftHandInteractorObject.transform.rotation = leftHandOriginalParent.rotation;
            lefttHandOnWheel = false;
        }

        // Check if both hands are released
        if (lefttHandOnWheel == false && rightHandOnWheel == false)
        {
            // Unparent the steering wheel itself
            transform.parent = transform.root;
        }
    }

    // **FIX: Corrected method name to standard Unity "OnTriggerStay"**
    private void OnTriggerStay(Collider other)
    {
        // **FIX: Check if the colliding object is an XRBaseInteractor**
        if (other.CompareTag("PlayerHand") && other.TryGetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor>(out var interactor))
        {
            // Logic for Right Hand
            if (rightHandInteractor != null && interactor == rightHandInteractor)
            {
                // **FIX: Replaced OVRInput.GetDown with interactor.isSelectActive**
                if (rightHandOnWheel == false && rightHandInteractor.isSelectActive)
                {
                    // Pass the Interactor's GameObject to the snapping function
                    PlaceHandOnWheel(ref rightHandInteractorObject, ref rightHandOriginalParent, ref rightHandOnWheel);
                }
            }
            
            // Logic for Left Hand
            else if (leftHandInteractor != null && interactor == leftHandInteractor)
            {
                // **FIX: Replaced OVRInput.GetDown with interactor.isSelectActive**
                if (lefttHandOnWheel == false && leftHandInteractor.isSelectActive)
                {
                    // Pass the Interactor's GameObject to the snapping function
                    PlaceHandOnWheel(ref leftHandInteractorObject, ref leftHandOriginalParent, ref lefttHandOnWheel);
                }
            }
        }
    }

    private void PlaceHandOnWheel(ref GameObject hand, ref Transform originalParent, ref bool handOnWheel)
    {
        var shortestDistance = Vector3.Distance(snappPositions[0].position, hand.transform.position);
        var bestSnapp = snappPositions[0];

        // Find the best available snap position
        foreach (var snappPosition in snappPositions)
        {
            // This childCount check is a simple way to see if another hand is already snapped to this spot
            // (assuming the snapped hand becomes a child).
            if (snappPosition.childCount == 0) 
            {
                var distance = Vector3.Distance(snappPosition.position, hand.transform.position);

                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    bestSnapp = snappPosition;
                }
            }
        }

        originalParent = hand.transform.parent;

        // Snap the hand (the interactor GameObject) to the best position
        hand.transform.parent = bestSnapp.transform;
        hand.transform.position = bestSnapp.transform.position;
        // Optionally, you might want to set rotation here: hand.transform.localRotation = Quaternion.identity;

        handOnWheel = true;
    }
}