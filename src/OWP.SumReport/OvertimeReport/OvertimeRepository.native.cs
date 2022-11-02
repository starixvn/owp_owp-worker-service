using Fanex.Data.Repository;
using Olympus.Xml;
using OWP.SumReport.OvertimeReport.Criteria;
using OWP.SumReport.OvertimeReport.Models;

namespace OWP.SumReport.OvertimeReport;

public interface OvertimeRepository
{
    Task<IEnumerable<EmployeeWorkingShift>> GetEmployeeWorkingShift(DateTime date, bool isGenerateData);

    Task<(IEnumerable<Assignment>, IEnumerable<MainMatch>)> GetTradersAssignments(DateTime date, IEnumerable<int> userIds, bool isGenerateData);

    Task SaveData(DateTime date, IEnumerable<OvertimeData> data);
}

public class OvertimeRepositoryImp : OvertimeRepository
{
    private readonly IDynamicRepository _repository;

    public OvertimeRepositoryImp(IDynamicRepository dynamicRepository)
    {
        _repository = dynamicRepository;
    }

    public async Task<IEnumerable<EmployeeWorkingShift>> GetEmployeeWorkingShift(DateTime date, bool isGenerateData)
        => await _repository.FetchAsync<EmployeeWorkingShift>(isGenerateData
            ? new GetOldAndNewEmployeeWorkingShiftCriteria { EventDate = date }
            : new GetEmployeeWorkingShiftCriteria { EventDate = date });

    public async Task<(IEnumerable<Assignment>, IEnumerable<MainMatch>)> GetTradersAssignments(DateTime date, IEnumerable<int> userIds, bool isGenerateData)
        => await _repository.FetchMultipleAsync<Assignment, MainMatch>(isGenerateData
        ? new GetGetOldAndNewTradersAssignmentsCriteria
        {
            EventDate = date,
            UserIds = string.Join(",", userIds)
        }
        : new GetTradersAssignmentsCriteria
        {
            EventDate = date,
            UserIds = string.Join(",", userIds)
        });

    public async Task SaveData(DateTime date, IEnumerable<OvertimeData> data)
    {
        await _repository.ExecuteAsync(new SaveOvertimeReportCriteria
        {
            EventDate = date,
            Data = XmlConverter.Serialize(data)
        });
    }
}