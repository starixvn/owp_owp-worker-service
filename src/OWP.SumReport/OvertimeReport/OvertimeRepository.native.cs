namespace OWP.SumReport.OvertimeReport;

using Criteria;
using Fanex.Data.Repository;
using Models;
using Olympus.Xml;

public interface OvertimeRepository
{
    Task<IEnumerable<EmployeeWorkingShift>> GetEmployeeWorkingShift(DateTime date, bool isGenerateData);

    Task<(IEnumerable<Assignment>, IEnumerable<MainMatch>)> GetTradersAssignments(DateTime date, IEnumerable<int> userIds, bool isGenerateData);

    Task SaveData(DateTime date, IEnumerable<OvertimeData> data);
}

public class OvertimeRepositoryImp(IDynamicRepository dynamicRepository) : OvertimeRepository
{
    public async Task<IEnumerable<EmployeeWorkingShift>> GetEmployeeWorkingShift(DateTime date, bool isGenerateData)
        => await dynamicRepository.FetchAsync<EmployeeWorkingShift>(isGenerateData
            ? new GetOldAndNewEmployeeWorkingShiftCriteria { EventDate = date }
            : new GetEmployeeWorkingShiftCriteria { EventDate = date });

    public async Task<(IEnumerable<Assignment>, IEnumerable<MainMatch>)> GetTradersAssignments(DateTime date, IEnumerable<int> userIds, bool isGenerateData)
        => await dynamicRepository.FetchMultipleAsync<Assignment, MainMatch>(isGenerateData
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
        await dynamicRepository.ExecuteAsync(new SaveOvertimeReportCriteria
        {
            EventDate = date,
            Data = XmlConverter.Serialize(data)
        });
    }
}