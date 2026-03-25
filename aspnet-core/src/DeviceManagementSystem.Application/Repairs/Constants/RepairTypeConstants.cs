using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Repairs.Constants
{
    /// <summary>
    /// 维修类型常量
    /// </summary>
    public static class RepairTypeConstants
    {
        /// <summary>
        /// 维修
        /// </summary>
        public const int Repair = 1;
        /// <summary>
        /// 升级
        /// </summary>
        public const int Upgrade = 2;

        /// <summary>
        /// 获取维修类型名称
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetName(int type)
        {
            return type switch
            {
                Repair => "维修",
                Upgrade => "升级",
                _ => "未知"
            };
        }
    }

    /// <summary>
    /// 故障处理等级常量
    /// </summary>
    public static class FaultLevelConstants
    {
        /// <summary>
        /// 立刻处理
        /// </summary>
        public const int Urgent = 1;   
        
        /// <summary>
        /// 停机后处理
        /// </summary>
        public const int Important = 2; 
        /// <summary>
        /// 保养过程中处理
        /// </summary>
        public const int Normal = 3;    

        /// <summary>
        /// 其他情况
        /// </summary>
        public const int Minor = 4;

        /// <summary>
        /// 获取故障处理等级名称
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public static string GetName(int level)
        {
            return level switch
            {
                Urgent => "立刻处理",
                Important => "停机后处理",
                Normal => "保养过程中处理",
                Minor => "其他情况",
                _ => "未知"
            };
        }
    }

    /// <summary>
    /// 申报状态常量
    /// </summary>
    public static class RepairRequestStatusConstants
    {
        /// <summary>
        /// 待派单
        /// </summary>
        public const int PendingDispatch = 1;
        /// <summary>
        /// 已派单
        /// </summary>
        public const int Dispatched = 2;
        /// <summary>
        /// 维修中
        /// </summary>
        public const int Repairing = 3;

        /// <summary>
        /// 获取申报状态名称
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public static string GetName(int status)
        {
            return status switch
            {
                PendingDispatch => "待派单",
                Dispatched => "已派单",
                Repairing => "维修中",
                _ => "未知"
            };
        }
    }

    /// <summary>
    /// 工单状态常量
    /// </summary>
    public static class WorkOrderStatusConstants
    {
        /// <summary>
        /// 待接单
        /// </summary>
        public const int PendingAccept = 1;
        /// <summary>
        /// 维修中
        /// </summary>
        public const int Repairing = 2;
        /// <summary>
        /// 待验收
        /// </summary>
        public const int PendingAcceptance = 3;

        /// <summary>
        /// 已完成
        /// </summary>
        public const int Completed = 4;

        /// <summary>
        /// 获取工单状态名称
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public static string GetName(int status)
        {
            return status switch
            {
                PendingAccept => "待接单",
                Repairing => "维修中",
                PendingAcceptance => "待验收",
                Completed => "已完成",
                _ => "未知"
            };
        }
    }

    /// <summary>
    /// 维修结果常量
    /// </summary>
    public static class RepairResultConstants
    {
        /// <summary>
        /// 成功
        /// </summary>
        public const int Success = 1;

        /// <summary>
        /// 部分成功
        /// </summary>
        public const int Partial = 2;

        /// <summary>
        /// 失败
        /// </summary>
        public const int Failed = 3;       


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static string GetName(int result)
        {
            return result switch
            {
                Success => "成功",
                Partial => "部分成功",
                Failed => "失败",
                _ => "未知"
            };
        }
    }

    /// <summary>
    /// 验收结论常量
    /// </summary>
    public static class AcceptanceConclusionConstants
    {

        /// <summary>
        /// 正常
        /// </summary>
        public const int Normal = 1;

        /// <summary>
        /// 不正常
        /// </summary>
        public const int Abnormal = 2;


        /// <summary>
        /// 获取验收结论名称
        /// </summary>
        /// <param name="conclusion"></param>
        /// <returns></returns>
        public static string GetName(int conclusion)
        {
            return conclusion switch
            {
                Normal => "正常",
                Abnormal => "不正常",
                _ => "未知"
            };
        }
    }
}
