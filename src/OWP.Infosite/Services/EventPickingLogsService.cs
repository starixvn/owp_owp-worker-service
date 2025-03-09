using OWP.Infosite.Models;
using System.Reflection;
using Fanex.Data.Repository;
using Olympus.OWP.Constants;
using OWP.Infosite.Helper;
using OWP.Infosite.Constants;
using Olympus.Xml;
using OWP.Infosite.Criteria;

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
                .Where(x => !x.Name.Equals("MatchStatus") && !x.Name.Equals("IsPostponed")).ToList();

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

                BuildLogForMatchPostponed(infoSiteNew, infoSiteOld, loggedChangeItems, fromService);
                BuildLogForMatchCancelled(infoSiteNew, infoSiteOld, loggedChangeItems, fromService);
                BuildLogForMatchSuspended(infoSiteNew, infoSiteOld, loggedChangeItems, fromService);

                if (infoSiteOld is { IsCheck: true } && loggedChangeItems.Any())
                {
                    AddLogUpdateCheckingStatus(infoSiteClone.InfoSiteMatchId, loggedChanges, fromService);
                }

                loggedChanges.AddRange(loggedChangeItems);
            }
        }

        private static LoggedChangeModel CreateLoggedChangeModel(
            int infoSiteMatchId,
            DateTime upDateTime,
            int? updateTypeId,
            string action,
            string newValue,
            string oldValue,
            string fromService)
        {
            return new LoggedChangeModel
            {
                InfoSiteMatchId = infoSiteMatchId,
                UpDateTime = upDateTime,
                UpdateTypeId = updateTypeId,
                Action = action,
                NewValue = newValue,
                OldValue = oldValue,
                FromService = fromService
            };
        }

        private static void BuildLogForMatchPostponed(Event infoSiteNew, InfoSiteDbInfo infoSiteDb, List<LoggedChangeModel> loggedChanges, string fromService)
        {
            if (infoSiteDb.IsPostponed == infoSiteNew.IsPostponed)
            {
                return;
            }
            var updateTypeId = infoSiteNew.IsPostponed ? UpdateTypeConstant.MATCH_POSTPONED : UpdateTypeConstant.RENEW_MATCH;
            var action = UpdateTypeConstant.UpdateType.GetValueOrDefault(updateTypeId);
            var newValue = infoSiteNew.IsPostponed ? "Postponed" : string.Empty;
            var oldValue = infoSiteNew.IsPostponed ? string.Empty : "Postponed";

            var loggedChange = CreateLoggedChangeModel(infoSiteDb.InfoSiteMatchId, InfoSiteSyncHelper.DateTimeNowGmt8(),
                updateTypeId, action, newValue, oldValue, fromService);
            loggedChanges.Add(loggedChange);
        }

       
        private static void BuildLogForMatchCancelled(Event infoSiteNew, InfoSiteDbInfo infoSiteDb, List<LoggedChangeModel> loggedChanges, string fromService)
        {
            if ((!string.IsNullOrEmpty(infoSiteDb.Remark2) && infoSiteDb.Remark2.Contains("Cancel", StringComparison.OrdinalIgnoreCase))
                || infoSiteDb.MatchStatus == InfoSiteMatchStatus.Cancelled)
            {
                return;
            }

            if ((!string.IsNullOrEmpty(infoSiteNew.Remark2) && infoSiteNew.Remark2.Contains("Cancel", StringComparison.OrdinalIgnoreCase))
                || infoSiteNew.MatchStatus == InfoSiteMatchStatus.Cancelled)
            {
                loggedChanges.Add(new LoggedChangeModel
                {
                    InfoSiteMatchId = infoSiteDb.InfoSiteMatchId,
                    UpdateTypeId = UpdateTypeConstant.MATCH_CANCELLED,
                    Action = UpdateTypeConstant.UpdateType.GetValueOrDefault(UpdateTypeConstant.MATCH_CANCELLED),
                    NewValue = "Cancelled",
                    OldValue = string.Empty,
                    UpDateTime = InfoSiteSyncHelper.DateTimeNowGmt8(),
                    FromService = fromService
                });
            }
        }

        private static void BuildLogForMatchSuspended(Event infoSiteNew, InfoSiteDbInfo infoSiteDb, List<LoggedChangeModel> loggedChanges, string fromService)
        {
            if (infoSiteDb.MatchStatus == InfoSiteMatchStatus.Suspended)
            {
                return;
            }

            if (infoSiteNew.MatchStatus == InfoSiteMatchStatus.Suspended)
            {
                loggedChanges.Add(new LoggedChangeModel
                {
                    InfoSiteMatchId = infoSiteDb.InfoSiteMatchId,
                    UpdateTypeId = UpdateTypeConstant.MATCH_SUSPENDED,
                    Action = UpdateTypeConstant.UpdateType.GetValueOrDefault(UpdateTypeConstant.MATCH_SUSPENDED),
                    NewValue = "Suspended",
                    OldValue = string.Empty,
                    UpDateTime = InfoSiteSyncHelper.DateTimeNowGmt8(),
                    FromService = fromService
                });
            }
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
            var newLoggedChanges = newInfoSiteIds.Select(newInfoSiteId => new LoggedChangeModel
            {
                UpdateTypeId = UpdateTypeConstant.NEW_MATCH,
                Action = UpdateTypeConstant.UpdateType.GetValueOrDefault(UpdateTypeConstant.NEW_MATCH),
                InfoSiteMatchId = newInfoSiteId,
                UpDateTime = InfoSiteSyncHelper.DateTimeNowGmt8(),
                NewValue = null,
                OldValue = "Initialize",
                FromService = fromService
            });

            loggedChanges.AddRange(newLoggedChanges);
        }
    }
}
