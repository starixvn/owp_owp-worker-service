using System.Reflection;
using Fanex.Data.Repository;
using Olympus.OWP.Constants;
using Olympus.Xml;
using OWP.Infosite.Criteria;
using OWP.Infosite.Helper;
using OWP.Infosite.Models;

namespace OWP.Infosite.Services
{
    public interface EventPickingLogsService
    {
        Task BuildLogs(List<Event> queriedResult, List<InfoSiteDbInfo> infoSiteDbInfos,
            DateTime eventDate,
            string fromService);

        void AddLogUpdateCheckingStatus(int infoSiteMatchId, IList<LoggedChangeModel> loggedChanges,
            string fromService);
    }

    public class EventPickingLogsServiceImp(IDynamicRepository repository) : EventPickingLogsService
    {
        public async Task BuildLogs(List<Event> queriedResult,
            List<InfoSiteDbInfo> infoSiteDbInfos,
            DateTime eventDate,
            string fromService)
        {
            var loggedChanges = new List<LoggedChangeModel>();
            var newInfoSiteIds = GetNewInfoSiteIds(queriedResult, infoSiteDbInfos);

            BuildLogForNewMatches(newInfoSiteIds, loggedChanges, fromService);
            BuildLogForChangedMatchesLog(queriedResult, infoSiteDbInfos, loggedChanges, fromService);
            await SaveLogChange(loggedChanges);
        }

        private async Task SaveLogChange(IList<LoggedChangeModel> loggedChange)
        {
            if (loggedChange.Any())
            {
                var xml = XmlConverter.Serialize(loggedChange);
                var criteria = new SaveLogsChangeCriteria
                {
                    Xml = xml
                };
                await repository.ExecuteAsync(criteria);
            }
        }

        private static IList<PropertyInfo> GetPropertyInfo()
            => typeof(InfoSiteDb).GetProperties()
                .Where(x => !x.Name.Equals("MatchStatus") && !x.Name.Equals("LastStatus")).ToList();

        public void AddLogUpdateCheckingStatus(int infoSiteMatchId, IList<LoggedChangeModel> loggedChanges, string fromService)
        {
            loggedChanges.Add(new LoggedChangeModel
            {
                InfoSiteMatchId = infoSiteMatchId,
                UpdateTypeId = null,
                Action = "Update the <b>checking status</b>",
                NewValue = "Unchecked",
                OldValue = "Checked",
                UpDateTime = InfoSiteSyncHelper.DateTimeNowGmt8(),
                FromService = fromService
            });
        }
        private void BuildLogForChangedMatchesLog(IList<Event> queriedResult, IList<InfoSiteDbInfo> infoSiteDbInfos, List<LoggedChangeModel> loggedChanges, string fromService)
        {
            foreach (var infoSiteOld in infoSiteDbInfos)
            {
                var infoSiteNew = queriedResult.First(x => x.InfoSiteMatchId == infoSiteOld.InfoSiteMatchId);
                var infoSiteClone = InfoSiteSyncHelper.CloneInfoSiteMatch(infoSiteNew);
                var loggedChangeItems = InfoSiteSyncHelper.BuildLoggedChanges(GetPropertyInfo(), infoSiteOld, infoSiteClone).ToList();
                
                BuildLogWhenMatchStatusChanged(infoSiteNew, infoSiteOld, loggedChangeItems, fromService);

                if (infoSiteOld is { IsCheck: true } && loggedChangeItems.Count != 0)
                {
                    AddLogUpdateCheckingStatus(infoSiteClone.InfoSiteMatchId, loggedChanges, fromService);
                }

                if (loggedChangeItems.Exists(log => log.UpdateTypeId == UpdateTypeConstant.MOVE_KICKOFF_DATE))
                {
                    var updateKODateLog = loggedChangeItems.First(l => l.UpdateTypeId == UpdateTypeConstant.MOVE_KICKOFF_DATE);
                    loggedChanges.Add(new LoggedChangeModel
                    {
                        InfoSiteMatchId = updateKODateLog.InfoSiteMatchId,
                        UpdateTypeId = UpdateTypeConstant.RENEW_MATCH,
                        Action = "Renew Match",
                        NewValue = updateKODateLog.NewValue,
                        OldValue = updateKODateLog.OldValue,
                        UpDateTime = InfoSiteSyncHelper.DateTimeNowGmt8(),
                        FromService = fromService
                    });
                }

                loggedChanges.AddRange(loggedChangeItems);
            }
        }

        private static void BuildLogWhenMatchStatusChanged(Event infoSiteNew, InfoSiteDbInfo infoSiteOld, List<LoggedChangeModel> loggedChangeItems, string fromService)
        {
            var isChangeRemark2 = infoSiteNew.Remark2 != infoSiteOld.Remark2;
            var isChangeMatchStatus = infoSiteNew.MatchStatus != infoSiteOld.MatchStatus;
            var oldStatus = infoSiteOld.LastStatus ?? "";
            var newStatusFromRemark2 = infoSiteNew.Remark2.GetStatusFromRemark2();
            var newStatusFromMatchStatus = infoSiteNew.MatchStatus.GetStatusFromMatchStatus();

            if (isChangeRemark2 && UpdateStatusIfChanged(loggedChangeItems, infoSiteOld.InfoSiteMatchId, oldStatus, newStatusFromRemark2, fromService))
            {
                infoSiteNew.LastStatus = newStatusFromRemark2;
            }
            else if (isChangeMatchStatus && UpdateStatusIfChanged(loggedChangeItems, infoSiteOld.InfoSiteMatchId, oldStatus, newStatusFromMatchStatus, fromService))
            {
                infoSiteNew.LastStatus = newStatusFromMatchStatus;
            }
            else
            {
                infoSiteNew.LastStatus = oldStatus;
            }
        }

        private static bool UpdateStatusIfChanged(List<LoggedChangeModel> loggedChangeItems, int matchId, string oldStatus, string newStatus, string fromService)
        {
            if (oldStatus == newStatus) return false;
            var updateTypeId = newStatus.GetUpdateTypeIdFromStatus();
            AddLoggedChange(loggedChangeItems, matchId, updateTypeId, newStatus, oldStatus, fromService);
            return true;
        }

        private static IEnumerable<int> GetNewInfoSiteIds(List<Event> queriedResult, List<InfoSiteDbInfo> infoSiteDbInfos)
        {
            return queriedResult
                .ExceptBy(infoSiteDbInfos.Select(x => x.InfoSiteMatchId), x => x.InfoSiteMatchId)
                .Select(x => x.InfoSiteMatchId);
        }

        private static void BuildLogForNewMatches(IEnumerable<int> newInfoSiteIds,
            List<LoggedChangeModel> loggedChanges,
            string fromService)
        {
            foreach (var newInfoSiteId in newInfoSiteIds)
            {
                AddLoggedChange(loggedChanges, newInfoSiteId, UpdateTypeConstant.NEW_MATCH, null, "Initialize", fromService);
            }
        }

        private static void AddLoggedChange(List<LoggedChangeModel> loggedChanges, int matchId, int updateTypeId, string newValue, string oldValue, string fromService)
        {
            loggedChanges.Add(new LoggedChangeModel
            {
                InfoSiteMatchId = matchId,
                UpdateTypeId = updateTypeId,
                Action = UpdateTypeConstant.UpdateType.GetValueOrDefault(updateTypeId),
                NewValue = newValue,
                OldValue = oldValue,
                UpDateTime = InfoSiteSyncHelper.DateTimeNowGmt8(),
                FromService = fromService
            });
        }
    }
}
