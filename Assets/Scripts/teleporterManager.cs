using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class teleporterManager : MonoBehaviour
{

    private List<teleporter> teleporterEntrances;
    private List<teleporter> teleporterExits;

    // Start is called before the first frame update
    void Start()
    {
        teleporterEntrances = new List<teleporter>();
        teleporterExits = new List<teleporter>();
    }

    public void addTeleporter(teleporter t) {
        if (t.isExit) {
            teleporterExits.Add(t);
            t.id = teleporterExits.Count - 1;
        } else {
            teleporterEntrances.Add(t);
            t.id = teleporterEntrances.Count - 1;
        }
    }

    public void removeTeleporter(teleporter t) {
        // check if the scene is changing and if so, don't remove the teleporter




        if (t.isExit) {
            teleporterExits.RemoveAt(t.id);
            // if cooresponding entrance exists, move it to the end of the list
            if (teleporterEntrances.Count > t.id) {
                teleporter entrance = teleporterEntrances[t.id];
                teleporterEntrances.RemoveAt(t.id);
                teleporterEntrances.Add(entrance);

                // and set the sibling index for the entrance to the very end
                entrance.transform.SetSiblingIndex(t.transform.parent.childCount - 1);
            }

        } else {
            teleporterEntrances.RemoveAt(t.id);
            // if cooresponding exit exists, move it to the end of the list
            if (teleporterExits.Count > t.id) {
                teleporter exit = teleporterExits[t.id];
                teleporterExits.RemoveAt(t.id);
                teleporterExits.Add(exit);

                // and set the sibling index for the exit to the very end
                exit.transform.SetSiblingIndex(t.transform.parent.childCount - 1);
            }

        }

        // finally fix the id numbers for all teleporters
        for (int i = 0; i < teleporterEntrances.Count; i++) {
            teleporterEntrances[i].id = i;
        }
        for (int i = 0; i < teleporterExits.Count; i++) {
            teleporterExits[i].id = i;
        }

    }

    public teleporter getExit(teleporter t) {
        teleporter exit = null;
        if (teleporterExits.Count > t.id) {
            exit = teleporterExits[t.id];
        }
        return exit;
    }

    public teleporter getEntrance(teleporter t) {
        teleporter entrance = null;
        if (teleporterEntrances.Count > t.id) {
            entrance = teleporterEntrances[t.id];
        }
        return entrance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
