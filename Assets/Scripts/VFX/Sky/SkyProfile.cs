using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Sky Profile", menuName = "VFX Profiles/Sky")]
public class SkyProfile : ScriptableObject
{
    public float lux;
    public float skyExposure;
    public float exposure;
    public float indirect;
}
