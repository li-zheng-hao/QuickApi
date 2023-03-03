using MongoDB.Entities;

namespace QuickApi.UnitOfWork.MongoDB;

public interface IMongoUnitOfWork:IUnitOfWork
{
    public Transaction? Transaction { get; }
}