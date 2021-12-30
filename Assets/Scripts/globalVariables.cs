using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public static class globalVariables
{
    // Main Game Variables
    public static bool oneLifeMode = false;
    public static TimeSpan timeElapsed;
    public static ArrayList levelTimes = new ArrayList();


    // Custom Level Variables
    // For game styles, 0=HB, 1=Ballance, 2=MBG
    // For ball types, 0=HB, 1=WB, 2=SB, 3=PB, 4=M
    public static String filePath = "temp";
    public static String levelName = "";
    public static String levelCreator = "";

    public static Color extraColor = Color.white;
    public static int gameStyle = 0;
    public static int startingBall = 0;
    public static int cameraStyle = 0;
    public static bool deathPlaneHeightActive = false;
    public static decimal deathPlaneHeight = 0;
    public static decimal levelScale = 1;
    public static int song = 0;
    /* Song decoding:

        Hamsterball: 0-14 = songs for levels 1-15, 15 = unused song, 16 = hb2 world 4 song
        Ballance: 0-4 = song packs 1-5
        Level to Song Pack Chart:
        1 - 1
        2 - 5
        3 - 2
        4 - 3
        5 - 1
        6 - 5
        7 - 4
        8 - 2
        9 - 3
        10 - 1
        11 - 3
        12 - 4
        13 - 5

        Marble Blast: 0 = Beach Party, 1 = Classic Vibe, 2 = Groove Police

    */

    // Hamsterball specific variables
    public static Color backgroundColor = Color.white;
    public static Color floorColor1 = Color.white;
    public static Color floorColor2 = Color.white;
    public static Color wallColor = Color.white;

    // Ballance specific variables
    public static int skybox = 0; // 0-11 = skybox for levels 1-12

    // Marble Blast specific variables
    public static int floorTexture = 0; // just go in the order the files appear in (in the interiors texture folder)

    // Playing Custom Level Variables

    public static float respawnHeight = -999;

    // Checkpoints
    public static Vector3 checkpointLoc = new Vector3();
    public static int checkpointNum = -1;
    public static int savedBallType = -1;

    // Powerup

    // none: -1
    // super jump: 0
    // super speed: 1
    // super bounce: 2
    // shock absorber: 3
    

    public static int storedPowerup = -1;

    //public static int activePowerup = -1;
    // should only be -1, 2, or 3

}