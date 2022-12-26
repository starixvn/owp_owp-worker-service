using Fanex.Data.Repository;
using Olympus.Xml;
using OWP.SumReport.LAReport.Criteria;
using OWP.SumReport.LAReport.Models;

namespace OWP.SumReport.LAReport;

public interface LARepository
{
    Task Update(DateTime eventDate, IEnumerable<LAReportModel> laReportModels);

    Task<IEnumerable<Assignment>> GetLAAssignment(DateTime eventDate, bool isGenerateData);

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
            Xml = XmlConverter.Serialize(laReportModels)
        };

        await repository.ExecuteAsync(command);
    }

    public async Task<IEnumerable<Assignment>> GetLAAssignment(DateTime eventDate, bool isGenerateData)
    {
        var criteria = new GetLAAssignmentCriteria
        {
            FromDate = eventDate,
            ToDate = eventDate
        };
        var generatedCriteria = new GetOldAndNewLAAssignmentCriteria
        {
            FromDate = eventDate,
            ToDate = eventDate
        };

        return await repository.FetchAsync<Assignment>(isGenerateData ? generatedCriteria : criteria);
    }

    public async Task<Dictionary<int, string>> GetUserSection()
    {
        var criteria = new GetUserSectionCriteria();
        var userSections = await repository.FetchAsync<UserSection>(criteria);

        return userSections.ToDictionary(u => u.UserId, u => u.Username);
    }
}