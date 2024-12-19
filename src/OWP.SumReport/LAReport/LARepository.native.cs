namespace OWP.SumReport.LAReport;

using Criteria;
using Fanex.Data.Repository;
using Models;
using Olympus.Xml;

public interface LaRepository
{
    Task Update(DateTime eventDate, IEnumerable<LaReportModel> laReportModels);

    Task<IEnumerable<Assignment>> GetLaAssignment(DateTime eventDate, bool isGenerateData);

    Task<Dictionary<int, string>> GetUserSection();
}

public class LaRepositoryImp(IDynamicRepository repository) : LaRepository
{
    public async Task Update(DateTime eventDate, IEnumerable<LaReportModel> laReportModels)
    {
        var command = new SumLaReportCommand
        {
            EventDate = eventDate,
            Xml = XmlConverter.Serialize(laReportModels)
        };

        await repository.ExecuteAsync(command);
    }

    public async Task<IEnumerable<Assignment>> GetLaAssignment(DateTime eventDate, bool isGenerateData)
    {
        var criteria = new GetLaAssignmentCriteria
        {
            FromDate = eventDate,
            ToDate = eventDate
        };
        var generatedCriteria = new GetOldAndNewLaAssignmentCriteria
        {
            FromDate = eventDate,
            ToDate = eventDate
        };

        return (await repository.FetchAsync<Assignment>(isGenerateData ? generatedCriteria : criteria)).ToList();
    }

    public async Task<Dictionary<int, string>> GetUserSection()
    {
        var criteria = new GetUserSectionCriteria();
        var userSections = await repository.FetchAsync<UserSection>(criteria);

        return userSections.ToDictionary(u => u.UserId, u => u.Username);
    }
}