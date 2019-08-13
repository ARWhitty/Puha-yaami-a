using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Vec3
{
    public float x;
    public float y;
    public float z;

    public Vec3(float xIn, float yIn, float zIn)
    {
        x = xIn;
        y = yIn;
        z = zIn;
    }
}

[System.Serializable]
public class GameManagerData
{
    public int score;
    //public Vec3[] checkpointPositions;
    public float[] lastCheckpoint;

    public GameManagerData(GameManager gm)
    {
        int currScore = gm.GetScore();
        //List<Vector3> checkpointPos = gm.GetCheckpoints();
        Vector3 lastChkpt = gm.GetLastCheckpoint();

        score = currScore;
        //checkpointPositions = new Vec3[10];
        //PopulateCheckpointArray(checkpointPos);
        lastCheckpoint = new float[3];
        lastCheckpoint[0] = lastChkpt.x;
        lastCheckpoint[1] = lastChkpt.y;
        lastCheckpoint[2] = lastChkpt.z;
    }

    /*    private void PopulateCheckpointArray(List<Vector3> posToFill)
        {
            for(int i = 0; i < posToFill.Count; i++)
            {
                Vec3 internalVec = new Vec3(posToFill[i].x, posToFill[i].y, posToFill[i].z);
                checkpointPositions[i] = internalVec;
            }
        }*/
}
