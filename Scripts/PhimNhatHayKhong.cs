using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;


#if UNITY_IOS || UNITY_TVOS
public static class PhimNhatHayKhong
{
    [DllImport("__Internal")]
    public static extern void PhimNhatRatHay();
}
#endif