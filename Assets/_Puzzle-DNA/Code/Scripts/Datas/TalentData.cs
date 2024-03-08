using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TalentDataSpace
{
    [Serializable]
    public class TalentValueData
    {
        public string id;
        public string nama;
        public TalentDescriptionData deskripsi;
        public string bahasa;
        public string warna;
    }

    [Serializable]
    public class TalentDescriptionData
    {
        public string deskripsi_singkat;
        public string deskripsi_paragraf;
        public string deskripsi;
        public List<string> keterangan_kuat;
        public List<string> keterangan_lemah;
        public List<string> keterangan_rekom;
        public string id;
    }

    [Serializable]
    public class TalentData
    {
        [HideInInspector] public string status;
        public List<TalentValueData> data;
    }
}