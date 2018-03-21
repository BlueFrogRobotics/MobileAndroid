using UnityEngine;

/// <summary>
/// Receiver to get the sensors information
/// </summary>
public class SensorOTOReceiver : OTONetReceiver
{
    [SerializeField]
    private GameObject mLeftSensor;

    [SerializeField]
    private GameObject mMiddleSensor;

    [SerializeField]
    private GameObject mRightSensor;

    [SerializeField]
    private GameObject mBackSensor;
    
    public override void ReceiveData(byte[] iData, int iNData)
    {
        if (iNData != 4)
            return;
        
        //Get the info and display it
        mLeftSensor.GetComponent<ObstacleManager>().lvl = iData[0];
        mMiddleSensor.GetComponent<ObstacleManager>().lvl = iData[1];
        mRightSensor.GetComponent<ObstacleManager>().lvl = iData[2];
        mBackSensor.GetComponent<ObstacleManager>().lvl = iData[3];
    }
}
