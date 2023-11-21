using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class LootDrop : MonoBehaviour
{
    [SerializeField] private LootTable lootTable;
    [SerializeField] private Vector2 minForceDirection;
    [SerializeField] private Vector2 maxForceDirection;
    [SerializeField, Range(0f,1f)] private float mayChanceItemsDropChance;
    [SerializeField] private Force itemDropForce;
    
    private float _totalDropChance;

    private void Start()
    {
        if (!CheckHasLootTable()) {return;}
        CalculateTotalDropChance();
    }

    private void CalculateTotalDropChance()
    {
        foreach (var elements in lootTable.chanceDrops)
        {
            _totalDropChance += elements.dropChance;
        }
    }

    public void DropLoot()
    {
        if (!CheckHasLootTable()) {return;}
        DropRequiredItems();
        DropChanceItems();
    }

    private bool CheckHasLootTable()
    {
        return lootTable != null;
    }

    private void DropRequiredItems()
    {
        if (!lootTable.HasRequiredDrops) {return;}
        foreach (var elements in lootTable.requiredDrops)
        {
            for (var i = 0; i < elements.dropAmount; i++)
            {
                DropItem(elements.item);
            }
        }
    }

    private bool MayChanceItemsDrop()
    {
        return Random.value > mayChanceItemsDropChance;
    }

    private void DropChanceItems()
    {
        if (!MayChanceItemsDrop() || !lootTable.HasChanceDrops){return;}

        var randomChance = CalculateDropChance();
        
        var element = GetElement(randomChance);
        
        PickRandomItems(element.items,element.dropItemsFromList);
    }

    private float CalculateDropChance()
    {
        return Random.Range(0, _totalDropChance);
    }

    private LootTable.ChanceDrops GetElement(float randomNumber)
    {
        foreach (var element in lootTable.chanceDrops)
        {
            if (randomNumber <= element.dropChance)
            {
                return element;
            }

            randomNumber -= element.dropChance;
        }

        return default;
    }

    private void PickRandomItems(IReadOnlyList<GameObject> targetItems, int maxItems)
    {
        for (var i = 0; i < maxItems; i++)
        {
            var item = targetItems[Random.Range(0, targetItems.Count)];
            DropItem(item);
        }
    }

    private void DropItem(GameObject item)
    {
        var spawnedItem = Instantiate(item, transform.position,quaternion.identity);
        spawnedItem.AddComponent<ForceBody>();
        var force = itemDropForce.Clone();
        force.Direction = new Vector3(Random.Range(minForceDirection.x, maxForceDirection.x), Random.Range(minForceDirection.y,maxForceDirection.y), 0);
        spawnedItem.GetComponent<ForceBody>().Add(force, new CallbackConfig());
    }
}
