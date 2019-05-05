using Borlay.Arrays;
using Borlay.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Borlay.Repositories.Services
{
    public class SecondaryRepositoryService<T> : ISecondaryRepositoryService<T> where T : class, IEntity
    {
        protected readonly ISecondaryRepository repository;
        protected readonly ISerializer serializer;

        public SecondaryRepositoryService(ISecondaryRepository repository, ISerializer serializer)
        {
            if (repository == null)
                throw new ArgumentNullException(nameof(repository));

            if (serializer == null)
                throw new ArgumentNullException(nameof(serializer));

            this.repository = repository;
            this.serializer = serializer;
        }

        public async Task Save(ByteArray parentId, T entity)
        {
            using (var transaction = repository.CreateTransaction())
            {
                var index = 0;
                var buffer = new byte[1024];
                serializer.AddBytes(entity, buffer, ref index);
                transaction.AppendValue(parentId, entity.Id, buffer, index);

                await transaction.Commit();
            }
        }

        public async Task Save(ByteArray parentId, T[] entities)
        {
            using (var transaction = repository.CreateTransaction())
            {
                var buffer = new byte[1024];
                foreach (var entity in entities)
                {
                    var index = 0;
                    serializer.AddBytes(entity, buffer, ref index);
                    transaction.AppendValue(parentId, entity.Id, buffer, index);
                }

                await transaction.Commit();
            }
        }

        public async Task<T> Get(ByteArray parentId, ByteArray entityId)
        {
            var bytes = repository.Get(parentId, entityId);
            var entity = Serialize(bytes);
            return entity;
        }

        public async Task<T[]> Get(ByteArray parentId, ByteArray[] entityIds)
        {
            var entities = repository.Get(parentId, entityIds).Select(s => Serialize(s.Value)).ToArray();
            return entities;
        }

        public async Task<T[]> Get(ByteArray parentId, int skip, int take)
        {
            var keys = repository.Get(parentId).Select(s => new ByteArray(s)).Skip(skip).Take(take).ToArray();
            var entities = repository.Get(parentId, keys).Select(s => Serialize(s.Value)).ToArray();
            return entities;
        }

        public async Task<T[]> Get(ByteArray parentId, OrderType orderType, int skip, int take)
        {
            var keys = repository.Get(orderType).Select(s => new ByteArray(s)).Skip(skip).Take(take).ToArray();
            var entities = repository.Get(parentId, keys).Select(s => Serialize(s.Value)).ToArray();
            return entities;
        }

        public async Task<T[]> GetDistinct(ByteArray parentId, OrderType orderType, int skip, int take)
        {
            var keys = repository.Get(orderType).Select(s => new ByteArray(s)).Distinct().Skip(skip).Take(take).ToArray();
            var entities = repository.Get(parentId, keys).Select(s => Serialize(s.Value)).ToArray();
            return entities;
        }

        public async Task<bool> Contains(ByteArray parentId, ByteArray entityId)
        {
            return repository.Contains(parentId, entityId);
        }

        public async Task Remove(ByteArray parentId, ByteArray entityId)
        {
            using (var transaction = repository.CreateTransaction())
            {
                transaction.Remove(parentId, entityId);
                await transaction.Commit();
            }
        }

        public async Task Remove(ByteArray parentId, ByteArray[] entityIds)
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
