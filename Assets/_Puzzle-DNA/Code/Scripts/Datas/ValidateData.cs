using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ValidateDataValue
{
    public string email;
    public string job;
    public string f_pendidikan;
    public string username;
    public string survey_code;
    public string survey_dibuat;
    public string tgl_lahir;
    public string f_jenis_kelamin;
}

[Serializable]
public class ValidateData
{
    public bool success;
    public string message;
}
