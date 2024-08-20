using AcademyKit.Infrastructure.Common;

public class LicenseService : ILisenceService
{
    private readonly IUnitOfWork _unitOfWork;

    public LicenseService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task AddLisence()
    {
        throw new NotImplementedException();
    }
}
