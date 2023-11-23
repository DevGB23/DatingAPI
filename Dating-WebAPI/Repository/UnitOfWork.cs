using AutoMapper;
using Dating_WebAPI.Data;
using Dating_WebAPI.Repository.Interfaces;

namespace Dating_WebAPI.Repository;
public class UnitOfWork : IUnitOfWork
{
    private readonly DataContext _dataContext;
    private readonly IMapper _mapper;

    public UnitOfWork(DataContext dataContext, IMapper mapper){
        _dataContext = dataContext;
        _mapper = mapper;
    }
    public IUserRepository UserRepository => new UserRepository(_dataContext, _mapper);

    public IMessageRepository MessageRepository => new MessageRepository(_dataContext, _mapper);

    public IUserLikeRepository LikesRepository => new UserLikeRepository(_dataContext);

    public async Task<bool> Complete()
    {
        int hasChanges = await _dataContext.SaveChangesAsync();
        return hasChanges > 0;
    }

    public bool HasChanges()
    {
        return _dataContext.ChangeTracker.HasChanges();
    }
}
