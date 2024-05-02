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
        [HideInInspector] public string bahasa;
        [HideInInspector] public string warna;
    }

    [Serializable]
    public class TalentDescriptionData
    {
        public string deskripsi_singkat_game;
        public string deskripsi_singkat;
        public string deskripsi;
        [HideInInspector] public string deskripsi_paragraf;
        [HideInInspector] public List<string> keterangan_kuat;
        [HideInInspector] public List<string> keterangan_lemah;
        [HideInInspector] public List<string> keterangan_rekom;
        [HideInInspector] public string id;
    }

    [Serializable]
    public class TalentData
    {
        [HideInInspector] public string status;
        public List<TalentValueData> data;
    }
}