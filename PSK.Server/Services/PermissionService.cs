using Ardalis.Specification;
using PSK.Server.Data.Entities;

namespace PSK.Server.Services
{
    public interface IPermissionService
    {
        Task<List<Permission>> GetAllAsync(ISpecification<Permission> spec);
    }
    public class PermissionService : IPermissionService
    {
        private readonly IGenericRepository<Permission> _permissionRepository;

        public PermissionService(IGenericRepository<Permission> repository)
        {
            _permissionRepository = repository;
        }

        public async Task<List<Permission>> GetAllAsync(ISpecification<Permission> spec)
        {
            var entities = await _permissionRepository.ListAsync(spec);

            return entities;
        }
    }
}
