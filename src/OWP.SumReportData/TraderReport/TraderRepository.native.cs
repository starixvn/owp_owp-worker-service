using Fanex.Data.Repository;
using Olympus.Xml;
using OWP.SumReportData.TraderReport.Criteria;
using OWP.SumReportData.TraderReport.Models;

namespace OWP.SumReportData.TraderReport;

public interface TraderRepository
{
    Task UpdateNoneRobotFields(DateTime eventDate);

    Task UpdateRobotFields(DateTime eventDate, List<RobotByByUser> robotByByUsers);

    Task<IEnumerable<RobotX3RawModel>> GetAssignmentsX3ByDate(DateTime eventDate, bool isGenerateData);

    Task<IEnumerable<RobotX5RawModel>> GetAssignmentsX5ByDate(DateTime eventDate, bool isGenerateData);

    Task<List<Assignment>> GetSaBaAssignmentByDate(DateTime eventDate, bool isGenerateData);

    Task UpdateESoccerFields(DateTime eventDate, IEnumerable<ESoccerByUser> eSoccerByUsers);
}

public class TraderRepositoryImp : TraderRepository
{
    private readonly IDynamicRepository repository;

    public TraderRepositoryImp(IDynamicRepository repository)
    {
        this.repository = repository;
    }

    public async Task UpdateNoneRobotFields(DateTime eventDate)
    {
        var command = new UpdateNonRobotFieldsCommand
        {
            EventDate = eventDate
        };

        await repository.ExecuteAsync(command);
    }

    public async Task UpdateRobotFields(DateTime eventDate, List<RobotByByUser> robotByByUsers)
    {
        var command = new UpdateRobotFieldsCommand
        {
            EventDate = eventDate,
            Xml = XmlConverter.Serialize(robotByByUsers)
        };

        await repository.ExecuteAsync(command);
    }

    public async Task<IEnumerable<RobotX3RawModel>> GetAssignmentsX3ByDate(DateTime eventDate, bool isGenerateData)
    {
        var criteria = new GetAssignmentX3ByDateCriteria { FromDate = eventDate, ToDate = eventDate };
        var generatedCriteria = new GetOldAndNewAssignmentX3ByDateCriteria { FromDate = eventDate, ToDate = eventDate };

        return await repository.FetchAsync<RobotX3RawModel>(isGenerateData ? generatedCriteria : criteria);
    }

    public async Task<IEnumerable<RobotX5RawModel>> GetAssignmentsX5ByDate(DateTime eventDate, bool isGenerateData)
    {
        var criteria = new GetAssignmentX5ByDateCriteria { FromDate = eventDate, ToDate = eventDate };
        var generatedCriteria = new GetOldAndNewAssignmentX5ByDateCriteria { FromDate = eventDate, ToDate = eventDate };

        return await repository.FetchAsync<RobotX5RawModel>(isGenerateData ? generatedCriteria : criteria);
    }

    public async Task<List<Assignment>> GetSaBaAssignmentByDate(DateTime eventDate, bool isGenerateData)
    {
        var criteria = new GetSaBaAssignmentsCriteria { EventDate = eventDate };
        var generatedCriteria = new GetOldAndNewSaBaAssignmentsCriteria { EventDate = eventDate };

        return (await repository.FetchAsync<Assignment>(isGenerateData ? generatedCriteria : criteria)).ToList();
    }

    public async Task UpdateESoccerFields(DateTime eventDate, IEnumerable<ESoccerByUser> eSoccerByUsers)
    {
        var command = new UpdateESoccerFieldsCommand
        {
            EventDate = eventDate,
            ESoccerFieldsXml = XmlConverter.Serialize(eSoccerByUsers)
        };

        await repository.ExecuteAsync(command);
    }
}