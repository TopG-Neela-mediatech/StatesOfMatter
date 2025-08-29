// Assets/Editor/MatterSOTestGenerator.cs
using UnityEngine;
using UnityEditor;
using TMKOC.StatesOfMatter;

public static class MatterSOTestGenerator
{
    [MenuItem("Assets/Create/ScriptableObjects/MatterSO Test Data")]
    public static void GenerateTestData()
    {
        // Create instance of your SO type
        MatterSO asset = ScriptableObject.CreateInstance<MatterSO>();

        // Fill ItemList with test items (sprites left null intentionally)
        asset.ItemList = new ItemData[]
        {
            // Solids
            new ItemData { ItemName = "Ice Cube", StateType = StateType.Solid, Sprite = null },
            new ItemData { ItemName = "Rock", StateType = StateType.Solid, Sprite = null },
            new ItemData { ItemName = "Wooden Block", StateType = StateType.Solid, Sprite = null },
            new ItemData { ItemName = "Metal Spoon", StateType = StateType.Solid, Sprite = null },
            new ItemData { ItemName = "Brick", StateType = StateType.Solid, Sprite = null },

            // Liquids
            new ItemData { ItemName = "Water", StateType = StateType.Liquid, Sprite = null },
            new ItemData { ItemName = "Milk", StateType = StateType.Liquid, Sprite = null },
            new ItemData { ItemName = "Juice", StateType = StateType.Liquid, Sprite = null },
            new ItemData { ItemName = "Oil", StateType = StateType.Liquid, Sprite = null },
            new ItemData { ItemName = "Honey", StateType = StateType.Liquid, Sprite = null },

            // Gases
            new ItemData { ItemName = "Steam", StateType = StateType.Gas, Sprite = null },
            new ItemData { ItemName = "Oxygen", StateType = StateType.Gas, Sprite = null },
            new ItemData { ItemName = "Helium", StateType = StateType.Gas, Sprite = null },
            new ItemData { ItemName = "Carbon Dioxide", StateType = StateType.Gas, Sprite = null },
            new ItemData { ItemName = "Smoke", StateType = StateType.Gas, Sprite = null },
        };

        // Create asset and save
        string assetPath = AssetDatabase.GenerateUniqueAssetPath("Assets/MatterSO_Test.asset");
        AssetDatabase.CreateAsset(asset, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // Select it in Project window
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;

        Debug.Log($"Created MatterSO test asset at: {assetPath}");
    }
}
