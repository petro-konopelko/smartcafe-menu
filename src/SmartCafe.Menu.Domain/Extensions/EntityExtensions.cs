using SmartCafe.Menu.Domain.Common;
using SmartCafe.Menu.Domain.Models;
using SmartCafe.Menu.Shared.Models;

namespace SmartCafe.Menu.Domain.Extensions;

internal static class EntityExtensions
{
    extension<TEntity>(List<TEntity> entities)
        where TEntity : IEntity
    {
        public Result SyncCollection<TUpdateInfo>(
        IReadOnlyCollection<TUpdateInfo> updateInfos,
        Func<Guid, ErrorDetail> notFoundErrorFn,
        Func<TEntity> createEntityFn,
        Func<TEntity, TUpdateInfo, int, Result> updateEntityFn,
        Func<TEntity, TUpdateInfo, Result>? syncNestedPropertiesFn = null)
        where TUpdateInfo : IUpdateInfoIdentity
        {
            var idsToKeep = new HashSet<Guid>(updateInfos
                .Select(i => i.Id)
                .Where(id => id.HasValue)
                .Select(id => id!.Value));

            entities.RemoveAll(e => !idsToKeep.Contains(e.Id));

            var entityDictionary = entities.ToDictionary(e => e.Id);
            var position = 0;
            var errorList = new List<ErrorDetail>();

            foreach (var updateInfo in updateInfos)
            {
                position++;

                TEntity entity;

                if (updateInfo.Id.HasValue)
                {
                    if (entityDictionary.TryGetValue(updateInfo.Id.Value, out var existingEntity))
                    {
                        entity = existingEntity;
                    }
                    else
                    {
                        errorList.Add(notFoundErrorFn(updateInfo.Id.Value));
                        continue;
                    }
                }
                else
                {
                    entity = createEntityFn();
                    entities.Add(entity);
                }

                var result = updateEntityFn(entity, updateInfo, position);

                if (result.IsFailure)
                {
                    errorList.AddRange(result.EnsureError().Details);
                }

                if (syncNestedPropertiesFn is not null)
                {
                    var nestedPropertiesUpdateResult = syncNestedPropertiesFn(entity, updateInfo);

                    if (nestedPropertiesUpdateResult.IsFailure)
                    {
                        errorList.AddRange(nestedPropertiesUpdateResult.EnsureError().Details);
                        continue;
                    }
                }
            }

            return errorList.Count > 0
                ? Result.Failure(Error.Validation(errorList))
                : Result.Success();
        }
    }
}
