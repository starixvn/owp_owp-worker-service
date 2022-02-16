using Fanex.Data.Repository;
using Olympus.Xml;
using OWP.SumReportData.OvertimeReport.Criteria;
using OWP.SumReportData.OvertimeReport.Models;

namespace OWP.SumReportData.OvertimeReport;

public interface OvertimeRepository
{
    Task<IEnumerable<EmployeeWorkingShift>> GetEmployeeWorkingShift(DateTime date);

    Task<(IEnumerable<Assignment>, IEnumerable<MainMatch>)> GetTradersAssignments(DateTime date, IEnumerable<int> userIds);

    Task SaveData(DateTime date, IEnumerable<OvertimeData> data);
}

public class OvertimeRepositoryImp : OvertimeRepository
{
    private readonly IDynamicRepository _repository;

    public OvertimeRepositoryImp(IDynamicRepository dynamicRepository)
    {
        _repository = dynamicRepository;
    }

    public async Task<IEnumerable<EmployeeWorkingShift>> GetEmployeeWorkingShift(DateTime date)
        => await _repository.FetchAsync<EmployeeWorkingShift>(new GetEmployeeWorkingShiftCriteria { EventDate = date });

    public async Task<(IEnumerable<Assignment>, IEnumerable<MainMatch>)> GetTradersAssignments(DateTime date, IEnumerable<int> userIds)
        => await _repository.FetchMultipleAsync<Assignment, MainMatch>(new GetTradersAssignmentsCriteria
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