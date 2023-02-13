namespace OWP.Infosite.Builder;

using System.Collections.Immutable;
using Constants;
using Models;

public static class WatchBetBuilder
{
    public static string Build(SoccerEventSchedule extInfo)
    {
        var streamSrc1 = BuildStream(extInfo.create_streaming_src, extInfo.create_streaming_id, Constant.StreamNameSrc1InfoSite);
        var streamSrc2 = BuildStream(extInfo.create_streaming2_src, extInfo.create_streaming2_id, Constant.StreamNameSrc2InfoSite);
        var wb = string.Empty;
        if (!string.IsNullOrWhiteSpace(extInfo.wb))
        {
            wb = $"{extInfo.wb}";
        }

        if (!string.IsNullOrWhiteSpace(streamSrc1))
        {
            wb = SelectTemplateWb(wb, streamSrc1);
        }

        if (!string.IsNullOrWhiteSpace(streamSrc2))
        {
            wb = SelectTemplateWb(wb, streamSrc2);
        }

        return wb;
    }

    public static string SelectTemplateWb(string wb, string stream)
        => !string.IsNullOrWhiteSpace(wb)
            ? $"{wb}/{stream}"
            : stream;

    public static string BuildStream(int? src, string id, IImmutableDictionary<int, string> options)
    {
        var stream = GetStreamName(src, options);
        var streamId = string.IsNullOrEmpty(id) || id.Trim() == "0" ? string.Empty : id.Trim();
        var streamArray = new[] { stream, streamId }
            .Where(i => !string.IsNullOrEmpty(i));
        return string.Join("-", streamArray);
    }

    public static string GetStreamName(int? src, IImmutableDictionary<int, string> options)
    {
        if (!src.HasValue)
        {
            return string.Empty;
        }

        var existsValue = options.TryGetValue(src.Value, out var streamName);

        return existsValue ? (streamName ?? "") : $"{src.Value}";
    }
}