using UnityEngine;
using UnityEditor;
using Framework.Core;

public class DebugModuleEditor : Editor
{
    private const string Symbol = "OPEN_LOG";

    [MenuItem("Log/日志系统 %#l", false, 0)]
    public static void ToggleLogSystem()
    {
        bool currentState = IsLogSystemEnabled();
        SetLogSystemState(!currentState);
    }


    private static void SetLogSystemState(bool state)
    {
        string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
        bool containsSymbol = defines.Contains(Symbol);

        if (state && !containsSymbol)
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(
                EditorUserBuildSettings.selectedBuildTargetGroup,
                (defines + ";" + Symbol).Trim(';')
            );
        }
        else if (!state && containsSymbol)
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(
                EditorUserBuildSettings.selectedBuildTargetGroup,
                defines.Replace(Symbol, "").Replace(";;", ";").Trim(';')
            );
        }
        AssetDatabase.Refresh();
        EditorApplication.delayCall += () =>
        {
            Debug.Log(state ? "日志系统已打开" : "日志系统已关闭");
        };
        Menu.SetChecked("Log/日志系统 %#l", state);
    }

    private static bool IsLogSystemEnabled()
    {
        string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
        return defines.Contains(Symbol);
    }
}