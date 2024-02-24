using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
	public int Slots;
	public ItemSlot[] items;


}

public struct ItemSlot{
	int itemType;
	int quantity;
}
