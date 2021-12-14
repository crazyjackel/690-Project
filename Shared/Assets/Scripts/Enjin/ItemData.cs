using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card")]
public class ItemData : ScriptableObject
{
    [SerializeField] string _itemId;
    public string ItemId => _itemId;
}
