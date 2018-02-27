// Copyright (c) Arch team. All rights reserved.

namespace Grpc.MicroService.EF
{
    /// <summary>
    /// Defines the interfaces for <see cref="IDbRepository{TEntity}"/> interfaces.
    /// </summary>
    public interface IDbRepositoryFactory
    {
        /// <summary>
        /// Gets the specified repository for the <typeparamref name="TEntity"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>An instance of type inherited from <see cref="IDbRepository{TEntity}"/> interface.</returns>
        IDbRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity;
    }
}
