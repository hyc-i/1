namespace SecondClassroomManager.Models;

public static class ActivityOptions
{
    public const string PendingStatus = "待审核";
    public const string ApprovedStatus = "已通过";
    public const string RejectedStatus = "未通过";

    public static readonly string[] Categories =
    {
        "社会活动",
        "科研与竞赛",
        "评奖评优",
        "认证考试",
        "奖励表彰",
        "志愿服务",
        "创新创业"
    };

    public static readonly string[] Levels =
    {
        "校级",
        "院级",
        "市级",
        "省级",
        "国家级",
        "国际级"
    };

    public static readonly string[] Statuses =
    {
        PendingStatus,
        ApprovedStatus,
        RejectedStatus
    };
}
