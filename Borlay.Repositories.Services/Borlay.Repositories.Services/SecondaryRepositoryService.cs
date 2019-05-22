using Borlay.Arrays;
using Borlay.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Borlay.Repositories.Services
{
    public class SecondaryRepositoryService<T> : ISecondaryRepositoryService<T>, IHasIndexMaps<T> where T : class, IEntity
    {
        protected readonly ISecondaryRepository repository;
        protected readonly ISerializer serializer;

        public IndexMapProvider<T> IndexMaps { get; }

        public SecondaryRepositoryService(ISecondaryRepository repository, ISerializer serializer)
            : this(repository, serializer, new IndexMapProvider<T>())
        {

        }

        public SecondaryRepositoryService(ISecondaryRepository repository, ISerializer serializer, IndexMapProvider<T> indexMaps)
        {
            if (repository == null)
                throw new ArgumentNullException(nameof(repository));

            if (serializer == null)
                throw new ArgumentNullException(nameof(serializer));

            if (indexMaps == null)
                throw new ArgumentNullException(nameof(indexMaps));

            this.repository = repository;
            this.serializer = serializer;
            this.IndexMaps = indexMaps;
        }

        public async Task SaveAsync(ByteArray parentId, T entity)
        {
            await SaveAsync(parentId, new T[] { entity });
        }

        public async Task SaveAsync(ByteArray parentId, T[] entities)
        {
            using (var transaction = repository.CreateTransaction())
            {
                var buffer = new byte[1024];
                foreach (var entity in entities)
                {
                    var index = 0;
                    serializer.AddBytes(entity, buffer, ref index);
                    var key = transaction.AppendValue(parentId, entity.Id, buffer, index);

                    foreach (var map in IndexMaps)
                    {
                        var score = map.GetScore(entity);
                        transaction.AppendScoreIndex(parentId, entity.Id, key, score, map.Level, map.Order);
                    }
                }

                await transaction.Commit();
            }
        }

        public async Task<T> GetAsync(ByteArray parentId, ByteArray entityId)
        {
            var bytes = repository.GetValue(parentId, entityId);
            var entity = Serialize(bytes);
            return entity;
        }

        public async Task<T[]> GetAsync(ByteArray parentId, ByteArray[] entityIds)
        {
            var entities = repository.GetValues(parentId, entityIds).Select(s => Serialize(s.Value)).ToArray();
            return entities;
        }

        public async Task<T[]> GetAsync(ByteArray parentId, bool distinct, int skip, int take)
        {
            var entities = repository.GetValues(parentId, distinct).Skip(skip).Take(take)
                 .Select(s => Serialize(s.Value)).ToArray();
            return entities;
        }

        public async Task<T[]> GetAsync(ByteArray parentId, OrderType orderType, bool distinct, int skip, int take)
        {
            var entities = repository.GetValues(parentId, orderType, distinct).Skip(skip).Take(take)
                .Select(s => Serialize(s.Value)).ToArray();
            return entities;
        }

        public async Task<bool> ExistAsync(ByteArray parentId, ByteArray entityId)
        {
            return repository.Contains(parentId, entityId);
        }

        public async Task RemoveAsync(ByteArray parentId, ByteArray entityId)
        {
            using (var transaction = repository.CreateTransaction())
            {
                transaction.Remove(parentId, entityId);
                await transaction.Commit();
            }
        }

        public async Task RemoveAsync(ByteArray parentId, ByteArray[] entityIds)
        {
            using (var transaction = repository.CreateTransaction())
            {
                transaction.Remove(parentId, entityIds);
                await transaction.Commit();
            }
        }

        protected virtual T Serialize(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return null;

            var index = 0;
            var entity = serializer.GetObject(bytes, ref index);
            return (T)entity;
        }
    }
}
