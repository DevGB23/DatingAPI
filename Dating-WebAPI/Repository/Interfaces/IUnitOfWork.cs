namespace Dating_WebAPI.Repository.Interfaces;

public interface IUnitOfWork
{
    IUserRepository UserRepository {get; }
    IMessageRepository MessageRepository {get;}
    IUserLikeRepository LikesRepository {get; }
    Task<bool> Complete();
    bool HasChanges();
}

