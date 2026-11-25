using System.Reflection;
using MegaCrit.Sts2.Core.Modding;

namespace DehyaMod.Loader;

/// <summary>登记内容程序集，供游戏原生模型扫描使用。</summary>
internal static class GameAssemblyRegistration
{
    public static void Register(Mod owner, Assembly contentAssembly)
        => ModManager.AssociateAssemblyWithMod(owner.manifest!.id!, contentAssembly);
}
