using SmartCafe.Menu.Domain.Models;

namespace SmartCafe.Menu.Domain.Extensions;

internal static class UpdateInfoExtensions
{
    extension<TUpdateInfo>(IReadOnlyCollection<TUpdateInfo> items)
        where TUpdateInfo : IUpdateInfoIdentity
    {
        internal bool HasDuplicateByKey<TKey>(
        Func<TUpdateInfo, TKey?> keySelector,
        Func<TKey?, bool> filterEmptyKeyFn)
        {
            return items
                .Select(keySelector)
                .Where(filterEmptyKeyFn)
                .GroupBy(key => key!)
                .Any(g => g.Count() > 1);
        }
    }
}
