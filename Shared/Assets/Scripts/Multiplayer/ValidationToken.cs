using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "VersionToken", menuName = "Lilypad/Assets/VersionToken")]
public class ValidationToken : ScriptableObject
{
    public string VersionHeader = "";

    public byte MajorVersion = 0;
    public byte MinorVersion = 0;
    public byte MajorPatchVersion = 0;
    public byte MinorPatchVersion = 0;

    public string Version { get { return m_Version; }}
    [SerializeField]
    private string m_Version = null;

    public string HashedVersion { get { return m_HashedVersion; }}
    [SerializeField]
    private string m_HashedVersion = null;


    public string CalcVersion()
    {
        m_Version = $"{VersionHeader}{MajorVersion}.{MinorVersion}.{MajorPatchVersion}.{MinorPatchVersion}";
        return Version;
    }
    public string CalcHashedString()
    {
        MD5 md5hasher = MD5.Create();
        byte[] data = md5hasher.ComputeHash(Encoding.Default.GetBytes(Version));
        Guid convert = new Guid(data);
        m_HashedVersion = convert.ToString();
        return HashedVersion;
    }
}
