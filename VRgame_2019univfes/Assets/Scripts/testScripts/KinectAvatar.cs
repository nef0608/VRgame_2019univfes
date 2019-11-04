﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
//using Windows.Kinect;

/*
*************************************
このコードはUDP受信機能も押し込んである。
*************************************
*/

public class KinectAvatar : MonoBehaviour {
    
    //[SerializeField] BodySourceManager bodySourceManager;

    //ネットワーク関連
    int LOCAL_PORT = 2001;
    static UdpClient udp;
    Thread thread;
    static string rawtext;

    //自分の関節とUnityちゃんのボーンを入れるよう
    [SerializeField] GameObject Ref;
    [SerializeField] GameObject LeftUpLeg;
    [SerializeField] GameObject LeftLeg;
    [SerializeField] GameObject RightUpLeg;
    [SerializeField] GameObject RightLeg;
    [SerializeField] GameObject Spine1;
    [SerializeField] GameObject LeftArm;
    [SerializeField] GameObject LeftForeArm;
    [SerializeField] GameObject LeftHand;
    [SerializeField] GameObject RightArm;
    [SerializeField] GameObject RightForeArm;
    [SerializeField] GameObject RightHand;

    // Use this for initialization
    void Start () {
        udp = new UdpClient(AddressFamily.InterNetwork);
        IPEndPoint localEP = new IPEndPoint(IPAddress.Any, LOCAL_PORT);
        udp.Client.Bind(localEP);
        udp.Client.ReceiveTimeout = 0;
        thread = new Thread(new ThreadStart(ThreadMethod));
        thread.Start(); 
    }

    // Update is called once per frame
    void Update () {
        //飛んできた値を気合でパース
        string[] splitText = rawtext.Split(',');
        Vector3 headPos = new Vector3(
            float.Parse(splitText[0]),
            float.Parse(splitText[1]),
            float.Parse(splitText[2]));

        /*
        //最初に追尾している人のBodyデータを取得する
        Body body = bodySourceManager.GetData().FirstOrDefault(b => b.IsTracked);
        
        // Kinectを斜めに置いてもまっすぐにするようにする
        var floorPlane = bodySourceManager.FloorClipPlane;
        Quaternion comp = Quaternion.FromToRotation(
            new Vector3(-floorPlane.X, floorPlane.Y, floorPlane.Z), Vector3.up);
        */

        Quaternion SpineBase;
        Quaternion SpineMid;
        Quaternion SpineShoulder;
        Quaternion ShoulderLeft;
        Quaternion ShoulderRight;
        Quaternion ElbowLeft;
        Quaternion WristLeft;
        Quaternion HandLeft;
        Quaternion ElbowRight;
        Quaternion WristRight;
        Quaternion HandRight;
        Quaternion KneeLeft;
        Quaternion AnkleLeft;
        Quaternion KneeRight;
        Quaternion AnkleRight;

        Quaternion q;
        Quaternion comp2;
        //CameraSpacePoint pos;

        
        // 関節の回転を取得する
        if (splitText.Length > 0)
        {
            /*
            var joints = body.JointOrientations;

            //Kinectの関節回転情報をUnityのクォータニオンに変換
            SpineBase = joints[JointType.SpineBase].Orientation.ToQuaternion(comp);
            SpineMid = joints[JointType.SpineMid].Orientation.ToQuaternion(comp);
            SpineShoulder = joints[JointType.SpineShoulder].Orientation.ToQuaternion(comp);
            ShoulderLeft = joints[JointType.ShoulderLeft].Orientation.ToQuaternion(comp);
            ShoulderRight = joints[JointType.ShoulderRight].Orientation.ToQuaternion(comp);
            ElbowLeft = joints[JointType.ElbowLeft].Orientation.ToQuaternion(comp);
            WristLeft = joints[JointType.WristLeft].Orientation.ToQuaternion(comp);
            HandLeft = joints[JointType.HandLeft].Orientation.ToQuaternion(comp);
            ElbowRight = joints[JointType.ElbowRight].Orientation.ToQuaternion(comp);
            WristRight = joints[JointType.WristRight].Orientation.ToQuaternion(comp);
            HandRight = joints[JointType.HandRight].Orientation.ToQuaternion(comp);
            KneeLeft = joints[JointType.KneeLeft].Orientation.ToQuaternion(comp);
            AnkleLeft = joints[JointType.AnkleLeft].Orientation.ToQuaternion(comp);
            KneeRight = joints[JointType.KneeRight].Orientation.ToQuaternion(comp);
            AnkleRight = joints[JointType.AnkleRight].Orientation.ToQuaternion(comp);
            */

            // 関節の回転を計算する 

            //----------------ここまでを処理して送る------------------
            //----------------以下で代入-----------------------------
            q = transform.rotation;
            transform.rotation = Quaternion.identity;

            comp2 = Quaternion.AngleAxis(90, new Vector3(0, 1, 0)) *
                             Quaternion.AngleAxis(-90, new Vector3(0, 0, 1));

            Spine1.transform.rotation = SpineMid * comp2;

            RightArm.transform.rotation = ElbowRight * comp2;
            RightForeArm.transform.rotation = WristRight * comp2;
            RightHand.transform.rotation = HandRight * comp2;

            LeftArm.transform.rotation = ElbowLeft * comp2;
            LeftForeArm.transform.rotation = WristLeft * comp2;
            LeftHand.transform.rotation = HandLeft * comp2;

            RightUpLeg.transform.rotation = KneeRight * comp2;
            RightLeg.transform.rotation = AnkleRight * comp2;

            LeftUpLeg.transform.rotation = KneeLeft * Quaternion.AngleAxis(-90, new Vector3(0, 0, 1));

            LeftLeg.transform.rotation = AnkleLeft * Quaternion.AngleAxis(-90, new Vector3(0, 0, 1));

            // モデルの回転を設定する
            transform.rotation = q;

            // モデルの位置を移動する
            pos = body.Joints[JointType.SpineMid].Position;
            Ref.transform.position = new Vector3(-pos.X, pos.Y, -pos.Z);
        }
    }

    void OnApplicationQuit()
    {
        thread.Abort();
    }

    private static void ThreadMethod()
    {
        while(true)
        {
            IPEndPoint remoteEP = null;
            byte[] data = udp.Receive(ref remoteEP);
            rawtext = Encoding.ASCII.GetString(data);
            //Debug.Log(rawtext);
        }
    } 
}