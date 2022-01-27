using Fanex.Data.Repository;
using OWP.Core.Extensions;
using OWP.SumReportData.LAReport.Criteria;
using OWP.SumReportData.LAReport.Models;

namespace OWP.SumReportData.LAReport;

public interface LARepository
{
    Task Update(DateTime eventDate, IEnumerable<LAReportModel> laReportModels);

    Task<IEnumerable<Assignment>> GetLAAssignment(DateTime eventDate);

    Task<Dictionary<int, string>> GetUserSection();
}

public class LARepositoryImp : LARepository
{
    private readonly IDynamicRepository repository;

    public LARepositoryImp(IDynamicRepository repository)
    {
        this.repository = repository;
    }

    public async Task Update(DateTime eventDate, IEnumerable<LAReportModel> laReportModels)
    {
        var command = new SumLAReportCommand
        {
            EventDate = eventDate,
            Xml = laReportModels.ToXml().ToString()
        };

        await repository.ExecuteAsync(command);
    }

    public async Task<IEnumerable<Assignment>> GetLAAssignment(DateTime eventDate)
    {
        var criteria = new GetLAAssignmentCriteria
        {
            FromDate = eventDate,
            ToDate = eventDate
        };
        return await repository.FetchAsync<Assignment>(criteria);
    }

    public async Task<Dictionary<int, string>> GetUserSection()
    {
        var criteria = new GetUserSectionCriteria();
        var userSections = await repository.FetchAsync<UserSection>(criteria);

        return userSections.ToDictionary(u => u.UserId, u => u.Username);
    }
}