using PSK.Server.Data.Entities;

namespace PSK.Server.Services
{
    public interface ILabelService : IGenericService<Label, LabelCreate, LabelUpdate>
    {

    }

    public class LabelService : GenericService<Label, LabelCreate, LabelUpdate>, ILabelService
    {
        private readonly GenericRepository<Label> _labelRepository;

        public LabelService(GenericRepository<Label> repository) : base(repository)
        {
            _labelRepository = repository;
        }
        public override async Task OnUpdatingAsync(Label entity, LabelUpdate update)
        {
            _repository.UpdateVersion(entity, update.Version);
        }


    }
}
