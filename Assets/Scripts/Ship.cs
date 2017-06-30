using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour {

    private GameObject wing_left { get; set; }
    private GameObject wing_right { get; set; }
    private GameObject beek { get; set; }
    private GameObject seat { get; set; }
    private GameObject aft { get; set; }
    private GameObject weapon { get; set; }

    Ship(GameObject wing_left, GameObject wing_right, GameObject beek,
         GameObject seat, GameObject aft, GameObject weapon) {
        this.wing_left = wing_left;
        this.wing_right = wing_right;
        this.beek = beek;
        this.seat = seat;
        this.aft = aft;
        this.weapon = weapon;
    }
    Ship() { }


}