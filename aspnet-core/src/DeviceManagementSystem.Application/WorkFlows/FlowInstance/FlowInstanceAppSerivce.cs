using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using Abp.UI;
using DeviceManagementSystem.DeviceInfos;
using DeviceManagementSystem.DeviceInfos.Dto;
using DeviceManagementSystem.DeviceInfos.Utils;
using DeviceManagementSystem.FlowManagement;
using DeviceManagementSystem.Maintenances;
using DeviceManagementSystem.Maintenances.Constant;
using DeviceManagementSystem.Maintenances.Dto;
using DeviceManagementSystem.Users;
using DeviceManagementSystem.Utils.Common;
using DeviceManagementSystem.WorkFlows.FlowDefinition.Dto;
using DeviceManagementSystem.WorkFlows.FlowInstance.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeviceManagementSystem.WorkFlows.FlowInstance
{
    /// <summary>
    /// 流程引擎实现
    /// </summary>
    public class FlowInstanceAppService : DeviceManagementSystemAppServiceBase, IFlowInstanceAppService
    {
        private readonly IRepository<FlowDefinitions, Guid> _flowDefRepository;
        private readonly IRepository<FlowInstances, Guid> _flowInstanceRepository;
        private readonly IRepository<FlowInstanceHistories, Guid> _historyRepository;
        private readonly IRepository<FlowNodeTasks, Guid> _taskRepository;
        private readonly IUserAppService _userAppService;
        private readonly IRepository<BusinessForms, Guid> _formRepository;
        private readonly IRepository<Devices, Guid> _deviceRepository;
        private readonly IRepository<DeviceChangeApplications, Guid> _changeApplyRepository;
        private readonly IRepository<DeviceAndChangeApplicationRelations, Guid> _deviceChangeRelationRepository;

        // 新增保养计划相关仓储
        private readonly IRepository<MaintenancePlans, Guid> _maintenancePlanRepository;
        private readonly IRepository<DeviceMaintenancePlanRelation, Guid> _deviceMaintenancePlanRelationRepository;
        private readonly IRepository<MaintenanceTemplates, Guid> _maintenanceTemplateRepository;

        // 操作指令常量
        private const int CMD_START = 0;
        private const int CMD_AUTO_REJECTED = 1;
        private const int CMD_AUTO_APPROVED = 2;
        private const int CMD_REJECTED = 3;
        private const int CMD_APPROVED = 4;
        private const int CMD_CANCELED = 5;
        private const int CMD_BACK = 7;
        private const int CMD_COPY = 12;

        // 流程状态常量
        private const int STATUS_UNDERWAY = 0;
        private const int STATUS_APPROVED = 1;
        private const int STATUS_REJECTED = 2;
        private const int STATUS_CANCELLED = 3;

        // 节点类型常量
        private const int NODE_START = 0;
        private const int NODE_APPROVE = 1;
        private const int NODE_COPY = 2;
        private const int NODE_CONDITION = 3;
        private const int NODE_GATEWAY = 4;
        private const int NODE_TRANSACT = 5;
        private const int NODE_END = 9;

        // 任务状态常量
        private const int TASK_PENDING = 0;
        private const int TASK_HANDLED = 1;
        private const int TASK_SKIPPED = 2;

        // 多实例类型常量
        private const int MULTI_INSTANCE_COUNTERSIGN = 1; // 会签：需要所有人同意
        private const int MULTI_INSTANCE_OR_SIGN = 2;     // 或签：一人同意即可
        private const int MULTI_INSTANCE_SEQUENTIAL = 3;  // 依次审批：按顺序审批

        /// <summary>
        /// 构造函数
        /// </summary>
        public FlowInstanceAppService(
            IRepository<FlowDefinitions, Guid> flowDefRepository,
            IRepository<FlowInstances, Guid> flowInstanceRepository,
            IRepository<FlowInstanceHistories, Guid> historyRepository,
            IRepository<FlowNodeTasks, Guid> taskRepository,
            IUserAppService userAppService,
            IRepository<DeviceChangeApplications, Guid> changeApplyRepository,
            IRepository<BusinessForms, Guid> formRepository,
            IRepository<DeviceAndChangeApplicationRelations, Guid> deviceChangeRelationRepository,
            IRepository<Devices, Guid> deviceRepository,
            IRepository<MaintenancePlans, Guid> maintenancePlanRepository,
            IRepository<DeviceMaintenancePlanRelation, Guid> deviceMaintenancePlanRelationRepository,
            IRepository<MaintenanceTemplates, Guid> maintenanceTemplateRepository)
        {
            _flowDefRepository = flowDefRepository;
            _flowInstanceRepository = flowInstanceRepository;
            _historyRepository = historyRepository;
            _taskRepository = taskRepository;
            _userAppService = userAppService;
            _changeApplyRepository = changeApplyRepository;
            _formRepository = formRepository;
            _deviceChangeRelationRepository = deviceChangeRelationRepository;
            _deviceRepository = deviceRepository;
            _maintenancePlanRepository = maintenancePlanRepository;
            _deviceMaintenancePlanRelationRepository = deviceMaintenancePlanRelationRepository;
            _maintenanceTemplateRepository = maintenanceTemplateRepository;
        }

        #region 流程启动

        /// <summary>
        /// 启动流程
        /// </summary>
        /// <param name="flowDefId">流程定义ID</param>
        /// <param name="businessId">业务ID</param>
        /// <param name="businessType">业务类型</param>
        /// <param name="initiatorId">发起人ID</param>
        /// <param name="formData">表单数据</param>
        /// <returns>流程实例ID</returns>
        [UnitOfWork]
        public async Task<Guid> StartProcessAsync(Guid flowDefId, Guid businessId, string businessType, long initiatorId, string formData)
        {
            var flowDef = await _flowDefRepository.FirstOrDefaultAsync(flowDefId);
            if (flowDef == null)
                throw new Exception("流程定义不存在");

            // 解析节点配置
            var nodeConfig = JsonConvert.DeserializeObject<JObject>(flowDef.NodeConfig ?? "{}");

            // 获取发起人姓名
            var initiatorName = await _userAppService.GetNameByUserId(initiatorId);

            // 创建流程实例
            var flowInstance = new FlowInstances
            {
                Code = GenerateFlowCode(),
                FlowName = flowDef.Name,
                FlowDefinitionId = flowDefId,
                BusinessId = businessId,
                BusinessType = businessType,
                InitiatorId = initiatorId,
                InitiatorName = initiatorName,
                CurrentNodeId = "start",
                CurrentNodeName = nodeConfig["name"]?.ToString() ?? "开始",
                CurrentNodeType = NODE_START,
                Status = STATUS_UNDERWAY,
                BeginTime = DateTime.Now,
                Cancelable = flowDef.Cancelable == 1,
                NodeConfig = flowDef.NodeConfig,
                FormData = formData
            };

            var instanceId = await _flowInstanceRepository.InsertAndGetIdAsync(flowInstance);
            await CurrentUnitOfWork.SaveChangesAsync();

            // 记录发起历史
            await AddHistoryAsync(instanceId, "start", nodeConfig["name"]?.ToString() ?? "开始", NODE_START,
                CMD_START, initiatorId, initiatorName, "发起流程", formData, null);

            // 处理第一个节点（跳过开始节点）
            var firstNode = nodeConfig["childNode"] as JObject;
            if (firstNode != null)
            {
                await ProcessNodeAsync(flowInstance, firstNode, instanceId, businessId, businessType, initiatorId, formData, 0);
            }

            return instanceId;
        }

        /// <summary>
        /// 处理节点
        /// </summary>
        /// <param name="flowInstance">流程实例</param>
        /// <param name="node">节点配置</param>
        /// <param name="instanceId">流程实例ID</param>
        /// <param name="businessId">业务ID</param>
        /// <param name="businessType">业务类型</param>
        /// <param name="initiatorId">发起人ID</param>
        /// <param name="formData">表单数据</param>
        /// <param name="nodeIndex">节点索引</param>
        private async Task ProcessNodeAsync(FlowInstances flowInstance, JObject node, Guid instanceId, Guid businessId, string businessType, long initiatorId, string formData, int nodeIndex)
        {
            if (node == null)
            {
                Logger.Info($"节点为null，流程结束: FlowInstanceId={instanceId}");
                await CompleteProcessAsync(instanceId, STATUS_APPROVED);
                return;
            }

            int nodeType = node["type"]?.Value<int>() ?? 0;
            string nodeName = node["name"]?.ToString() ?? "未知节点";

            // 使用节点名称+索引作为唯一标识
            string nodeIdentifier = $"{nodeName}_{nodeIndex}";

            Logger.Info($"处理节点: 标识={nodeIdentifier}, 名称={nodeName}, 类型={nodeType}, 索引={nodeIndex}");

            // 如果是结束节点
            if (nodeType == NODE_END)
            {
                Logger.Info($"到达结束节点，流程结束: FlowInstanceId={instanceId}");
                await CompleteProcessAsync(instanceId, STATUS_APPROVED);
                return;
            }

            // 更新当前节点
            flowInstance.CurrentNodeId = nodeIdentifier;
            flowInstance.CurrentNodeName = nodeName;
            flowInstance.CurrentNodeType = nodeType;
            await _flowInstanceRepository.UpdateAsync(flowInstance);
            await CurrentUnitOfWork.SaveChangesAsync();

            Logger.Info($"更新当前节点: {nodeName}, 标识={nodeIdentifier}");

            // 处理不同类型节点
            if (nodeType == NODE_APPROVE || nodeType == NODE_TRANSACT)
            {
                // 审批/办理节点 - 创建任务，等待用户处理
                Logger.Info($"创建审批节点任务: {nodeName}, 标识={nodeIdentifier}");
                await CreateApprovalTasksAsync(instanceId, node, initiatorId, formData, nodeIdentifier, nodeIndex);
                // 注意：这里不继续处理下一个节点，等待用户处理完成后由 HandleNodeCompleteAsync 继续
            }
            else if (nodeType == NODE_COPY)
            {
                // 抄送节点 - 自动完成，然后继续下一个节点
                Logger.Info($"处理抄送节点: {nodeName}");
                await CreateCopyTasksAsync(instanceId, node, initiatorId, nodeIdentifier);

                // 自动记录抄送历史
                await AddHistoryAsync(instanceId, nodeIdentifier, nodeName, nodeType,
                    CMD_COPY, null, "系统", "自动抄送", formData, null);

                // 继续处理下一个节点
                var nextNode = node["childNode"] as JObject;
                if (nextNode != null)
                {
                    Logger.Info($"抄送节点完成，继续处理下一个节点");
                    await ProcessNodeAsync(flowInstance, nextNode, instanceId, businessId, businessType, initiatorId, formData, nodeIndex + 1);
                }
                else
                {
                    Logger.Info($"抄送节点完成后无下一个节点，流程结束");
                    await CompleteProcessAsync(instanceId, STATUS_APPROVED);
                }
            }
            else if (nodeType == NODE_GATEWAY)
            {
                // 网关节点 - 处理条件分支
                Logger.Info($"处理网关节点: {nodeName}");
                await ProcessGatewayAsync(flowInstance, node, instanceId, businessId, businessType, initiatorId, formData, nodeIndex);
            }
            else
            {
                // 其他类型节点，继续下一个
                var nextNode = node["childNode"] as JObject;
                if (nextNode != null)
                {
                    Logger.Info($"继续处理下一个节点");
                    await ProcessNodeAsync(flowInstance, nextNode, instanceId, businessId, businessType, initiatorId, formData, nodeIndex + 1);
                }
                else
                {
                    Logger.Info($"无下一个节点，流程结束");
                    await CompleteProcessAsync(instanceId, STATUS_APPROVED);
                }
            }
        }

        /// <summary>
        /// 创建审批任务
        /// </summary>
        /// <param name="instanceId">流程实例ID</param>
        /// <param name="node">节点配置</param>
        /// <param name="initiatorId">发起人ID</param>
        /// <param name="formData">表单数据</param>
        /// <param name="nodeIdentifier">节点标识</param>
        /// <param name="nodeIndex">节点索引</param>
        private async Task CreateApprovalTasksAsync(Guid instanceId, JObject node, long initiatorId, string formData, string nodeIdentifier, int nodeIndex)
        {
            var nodeName = node["name"]?.ToString() ?? "审批";
            var nodeType = node["type"]?.Value<int>() ?? NODE_APPROVE;
            var approvalType = node["approvalType"]?.Value<int>() ?? 0;
            var multiInstanceType = node["multiInstanceApprovalType"]?.Value<int>() ?? 0;

            Logger.Info($"创建审批任务: 节点标识={nodeIdentifier}, 节点名称={nodeName}, 类型={nodeType}, 审批类型={approvalType}, 多实例类型={multiInstanceType}");

            // 自动通过/拒绝
            if (approvalType == 1) // 自动通过
            {
                Logger.Info($"节点自动通过: {nodeName}");
                await AddHistoryAsync(instanceId, nodeIdentifier, nodeName, nodeType,
                    CMD_AUTO_APPROVED, null, "系统", "自动通过", formData, null);

                // 自动通过后继续处理下一个节点
                var instance = await _flowInstanceRepository.GetAsync(instanceId);
                var nextNode = node["childNode"] as JObject;
                if (nextNode != null)
                {
                    Logger.Info($"自动通过后继续处理下一个节点");
                    await ProcessNodeAsync(instance, nextNode, instanceId, instance.BusinessId, instance.BusinessType, instance.InitiatorId, formData, nodeIndex + 1);
                }
                else
                {
                    Logger.Info($"自动通过后无下一个节点，流程结束");
                    await CompleteProcessAsync(instanceId, STATUS_APPROVED);
                }
                return;
            }
            else if (approvalType == 2) // 自动拒绝
            {
                Logger.Info($"节点自动拒绝: {nodeName}");
                await AddHistoryAsync(instanceId, nodeIdentifier, nodeName, nodeType,
                    CMD_AUTO_REJECTED, null, "系统", "自动拒绝", formData, null);
                await CompleteProcessAsync(instanceId, STATUS_REJECTED);
                return;
            }

            // 解析审批人
            var assignees = node["assignees"] as JArray;
            if (assignees == null || !assignees.Any())
            {
                // 审批人为空时处理
                var noAuditorType = node["flowNodeNoAuditorType"]?.Value<int>() ?? 0;
                Logger.Info($"审批人为空，处理方式: {noAuditorType}");

                if (noAuditorType == 0) // 自动通过
                {
                    await AddHistoryAsync(instanceId, nodeIdentifier, nodeName, nodeType,
                        CMD_AUTO_APPROVED, null, "系统", "审批人为空，自动通过", formData, null);

                    // 自动通过后继续处理下一个节点
                    var instance = await _flowInstanceRepository.GetAsync(instanceId);
                    var nextNode = node["childNode"] as JObject;
                    if (nextNode != null)
                    {
                        Logger.Info($"审批人为空自动通过后继续处理下一个节点");
                        await ProcessNodeAsync(instance, nextNode, instanceId, instance.BusinessId, instance.BusinessType, instance.InitiatorId, formData, nodeIndex + 1);
                    }
                    else
                    {
                        Logger.Info($"审批人为空自动通过后无下一个节点，流程结束");
                        await CompleteProcessAsync(instanceId, STATUS_APPROVED);
                    }
                    return;
                }
                else if (noAuditorType == 1) // 指定人员
                {
                    var assignee = node["flowNodeNoAuditorAssignee"]?.Value<string>();
                    if (!string.IsNullOrEmpty(assignee) && long.TryParse(assignee, out long userId))
                    {
                        var userName = await _userAppService.GetNameByUserId(userId);
                        await CreateTaskAsync(instanceId, nodeIdentifier, nodeName, nodeType,
                            userId, userName, multiInstanceType, 1, node["formAuths"]?.ToString());
                        Logger.Info($"创建任务(审批人为空指定人员): 用户={userName}, 用户ID={userId}, 节点标识={nodeIdentifier}");
                    }
                }
                return;
            }

            // 获取表单权限
            var formAuths = node["formAuths"]?.ToString();

            // 创建任务 - 根据多实例类型处理
            int order = 1;
            int taskCount = 0;

            foreach (var assignee in assignees)
            {
                var users = await ParseAssigneesAsync(assignee as JObject, initiatorId);
                foreach (var user in users)
                {
                    await CreateTaskAsync(instanceId, nodeIdentifier, nodeName, nodeType,
                        user.UserId, user.UserName, multiInstanceType, order++, formAuths);
                    taskCount++;
                    Logger.Info($"创建任务: 用户={user.UserName}, 用户ID={user.UserId}, 节点标识={nodeIdentifier}, 多实例类型={multiInstanceType}, 顺序={order - 1}");
                }
            }

            Logger.Info($"节点 {nodeName} (标识={nodeIdentifier}) 创建了 {taskCount} 个待处理任务");
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// 创建抄送任务
        /// </summary>
        /// <param name="instanceId">流程实例ID</param>
        /// <param name="node">节点配置</param>
        /// <param name="initiatorId">发起人ID</param>
        /// <param name="nodeIdentifier">节点标识</param>
        private async Task CreateCopyTasksAsync(Guid instanceId, JObject node, long initiatorId, string nodeIdentifier)
        {
            var nodeName = node["name"]?.ToString() ?? "抄送";

            var ccs = node["ccs"] as JArray;
            if (ccs == null || !ccs.Any()) return;

            foreach (var cc in ccs)
            {
                var users = await ParseAssigneesAsync(cc as JObject, initiatorId);
                foreach (var user in users)
                {
                    var task = new FlowNodeTasks
                    {
                        FlowInstanceId = instanceId,
                        NodeId = nodeIdentifier,
                        NodeName = nodeName,
                        NodeType = NODE_COPY,
                        AssigneeId = user.UserId,
                        AssigneeName = user.UserName,
                        Status = TASK_HANDLED,
                        CreateTime = DateTime.Now,
                        HandleTime = DateTime.Now,
                        HandleCmd = CMD_COPY
                    };
                    await _taskRepository.InsertAsync(task);
                    Logger.Info($"创建抄送任务: 用户={user.UserName}, 节点标识={nodeIdentifier}");
                }
            }

            await CurrentUnitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// 解析审批人
        /// </summary>
        /// <param name="assignee">审批人配置</param>
        /// <param name="initiatorId">发起人ID</param>
        /// <returns>用户列表</returns>
        private async Task<List<(long UserId, string UserName)>> ParseAssigneesAsync(JObject assignee, long initiatorId)
        {
            var result = new List<(long, string)>();
            if (assignee == null) return result;

            var assigneeType = assignee["assigneeType"]?.Value<int>() ?? 0;

            switch (assigneeType)
            {
                case 0: // 发起人本人
                    var initiatorName = await _userAppService.GetNameByUserId(initiatorId);
                    result.Add((initiatorId, initiatorName));
                    Logger.Info($"解析审批人: 发起人本人, userId={initiatorId}, userName={initiatorName}");
                    break;

                case 4: // 指定成员
                    var assignees = assignee["assignees"] as JArray;
                    if (assignees != null)
                    {
                        foreach (var item in assignees)
                        {
                            if (long.TryParse(item.ToString(), out long userId))
                            {
                                var userName = await _userAppService.GetNameByUserId(userId);
                                result.Add((userId, userName));
                                Logger.Info($"解析审批人: 指定成员, userId={userId}, userName={userName}");
                            }
                        }
                    }
                    break;

                case 3: // 角色
                    Logger.Info($"解析审批人: 角色, 暂未实现");
                    break;
            }

            return result;
        }

        /// <summary>
        /// 处理条件分支
        /// </summary>
        /// <param name="flowInstance">流程实例</param>
        /// <param name="gateway">网关节点配置</param>
        /// <param name="instanceId">流程实例ID</param>
        /// <param name="businessId">业务ID</param>
        /// <param name="businessType">业务类型</param>
        /// <param name="initiatorId">发起人ID</param>
        /// <param name="formData">表单数据</param>
        /// <param name="nodeIndex">节点索引</param>
        private async Task ProcessGatewayAsync(FlowInstances flowInstance, JObject gateway, Guid instanceId, Guid businessId, string businessType, long initiatorId, string formData, int nodeIndex)
        {
            var conditionNodes = gateway["conditionNodes"] as JArray;
            if (conditionNodes == null || !conditionNodes.Any())
            {
                var firstChild = conditionNodes?.FirstOrDefault()?["childNode"] as JObject;
                if (firstChild != null)
                {
                    await ProcessNodeAsync(flowInstance, firstChild, instanceId, businessId, businessType, initiatorId, formData, nodeIndex + 1);
                }
                else
                {
                    await CompleteProcessAsync(instanceId, STATUS_APPROVED);
                }
                return;
            }

            for (int i = 0; i < conditionNodes.Count; i++)
            {
                var conditionNode = conditionNodes[i] as JObject;
                var conditionGroups = conditionNode["conditionGroups"] as JArray;

                if (i == conditionNodes.Count - 1)
                {
                    var nextNode = conditionNode["childNode"] as JObject;
                    if (nextNode != null)
                    {
                        await ProcessNodeAsync(flowInstance, nextNode, instanceId, businessId, businessType, initiatorId, formData, nodeIndex + 1);
                    }
                    else
                    {
                        await CompleteProcessAsync(instanceId, STATUS_APPROVED);
                    }
                    break;
                }

                if (conditionGroups != null && conditionGroups.Any())
                {
                    var nextNode = conditionNode["childNode"] as JObject;
                    if (nextNode != null)
                    {
                        await ProcessNodeAsync(flowInstance, nextNode, instanceId, businessId, businessType, initiatorId, formData, nodeIndex + 1);
                    }
                    else
                    {
                        await CompleteProcessAsync(instanceId, STATUS_APPROVED);
                    }
                    break;
                }
            }
        }

        #endregion

        #region 任务处理

        /// <summary>
        /// 处理节点任务
        /// </summary>
        /// <param name="taskId">任务ID</param>
        /// <param name="flowCmd">操作指令</param>
        /// <param name="operatorId">操作人ID</param>
        /// <param name="comment">操作意见</param>
        /// <param name="formData">表单数据</param>
        /// <param name="backNodeId">回退节点ID</param>
        /// <returns>处理结果</returns>
        public async Task<bool> HandleTaskAsync(Guid taskId, int flowCmd, long operatorId, string comment, string formData, string backNodeId = null)
        {
            var task = await _taskRepository.GetAsync(taskId);
            if (task == null)
                throw new Exception("任务不存在");

            if (task.Status != TASK_PENDING)
                throw new Exception("任务已处理");

            var operatorName = await _userAppService.GetNameByUserId(operatorId);

            var instance = await _flowInstanceRepository.GetAsync(task.FlowInstanceId);
            if (instance == null)
                throw new Exception("流程实例不存在");

            string beforeFormData = instance.FormData;

            Logger.Info($"处理任务: TaskId={taskId}, 节点标识={task.NodeId}, 节点名称={task.NodeName}, 操作={GetCmdName(flowCmd)}, 操作人={operatorName}, 回退节点ID={backNodeId}");

            task.Status = TASK_HANDLED;
            task.HandleTime = DateTime.Now;
            task.HandleCmd = flowCmd;
            task.HandleComment = comment;
            await _taskRepository.UpdateAsync(task);

            await AddHistoryAsync(task.FlowInstanceId, task.NodeId, task.NodeName, task.NodeType,
                flowCmd, operatorId, operatorName, comment, beforeFormData, formData);

            if (!string.IsNullOrEmpty(formData) && formData != beforeFormData)
            {
                instance.FormData = formData;
                await _flowInstanceRepository.UpdateAsync(instance);
            }

            await CurrentUnitOfWork.SaveChangesAsync();

            await HandleNodeCompleteAsync(instance, task, flowCmd, backNodeId);

            return true;
        }

        /// <summary>
        /// 处理节点完成
        /// </summary>
        /// <param name="instance">流程实例</param>
        /// <param name="completedTask">已完成的任务</param>
        /// <param name="flowCmd">操作指令</param>
        /// <param name="backNodeId">回退节点ID</param>
        private async Task HandleNodeCompleteAsync(FlowInstances instance, FlowNodeTasks completedTask, int flowCmd, string backNodeId)
        {
            Logger.Info($"HandleNodeCompleteAsync 开始: 完成节点标识={completedTask.NodeId}, 节点名称={completedTask.NodeName}, 操作={flowCmd}");

            if (flowCmd == CMD_REJECTED)
            {
                Logger.Info($"流程被拒绝: FlowInstanceId={instance.Id}");
                await CompleteProcessAsync(instance.Id, STATUS_REJECTED);
                return;
            }
            else if (flowCmd == CMD_BACK)
            {
                Logger.Info($"流程回退: FlowInstanceId={instance.Id}, 回退到节点={backNodeId}");
                await HandleBackOperation(instance, completedTask, backNodeId);
                return;
            }
            else if (flowCmd == CMD_APPROVED)
            {
                await HandleApproveOperation(instance, completedTask);
            }
        }

        /// <summary>
        /// 处理通过操作
        /// </summary>
        /// <param name="instance">流程实例</param>
        /// <param name="completedTask">已完成的任务</param>
        private async Task HandleApproveOperation(FlowInstances instance, FlowNodeTasks completedTask)
        {
            int multiInstanceType = completedTask.MultiInstanceType ?? 0;

            Logger.Info($"处理通过操作: 节点={completedTask.NodeName}, 节点标识={completedTask.NodeId}, 多实例类型={multiInstanceType}");

            // 获取当前节点的所有待处理任务
            var pendingTasks = await _taskRepository.GetAll()
                .Where(x => x.FlowInstanceId == instance.Id
                    && x.NodeId == completedTask.NodeId
                    && x.Status == TASK_PENDING)
                .ToListAsync();

            Logger.Info($"当前节点还有 {pendingTasks.Count} 个待处理任务");

            // 判断当前节点是否已完成
            bool isNodeCompleted = false;

            if (multiInstanceType == MULTI_INSTANCE_COUNTERSIGN) // 会签：需要所有人同意
            {
                // 检查是否还有待处理任务
                var remainingTasks = await _taskRepository.GetAll()
                    .Where(x => x.FlowInstanceId == instance.Id
                        && x.NodeId == completedTask.NodeId
                        && x.Status == TASK_PENDING)
                    .CountAsync();

                isNodeCompleted = remainingTasks == 0;
                Logger.Info($"会签节点剩余任务数: {remainingTasks}, 节点完成: {isNodeCompleted}");
            }
            else if (multiInstanceType == MULTI_INSTANCE_OR_SIGN) // 或签：一人同意即可
            {
                // 取消其他待处理任务
                foreach (var task in pendingTasks)
                {
                    task.Status = TASK_SKIPPED;
                    await _taskRepository.UpdateAsync(task);
                    Logger.Info($"取消其他或签任务: TaskId={task.Id}");
                }
                await CurrentUnitOfWork.SaveChangesAsync();
                isNodeCompleted = true;
            }
            else if (multiInstanceType == MULTI_INSTANCE_SEQUENTIAL) // 依次审批
            {
                // 获取下一个顺序的任务
                var nextTask = await _taskRepository.GetAll()
                    .Where(x => x.FlowInstanceId == instance.Id
                        && x.NodeId == completedTask.NodeId
                        && x.Status == TASK_PENDING)
                    .OrderBy(x => x.SortOrder)
                    .FirstOrDefaultAsync();

                isNodeCompleted = nextTask == null;
                Logger.Info($"依次审批节点下一个任务存在: {nextTask != null}, 节点完成: {isNodeCompleted}");
            }
            else // 默认按或签处理
            {
                // 取消其他待处理任务
                foreach (var task in pendingTasks)
                {
                    task.Status = TASK_SKIPPED;
                    await _taskRepository.UpdateAsync(task);
                }
                await CurrentUnitOfWork.SaveChangesAsync();
                isNodeCompleted = true;
            }

            if (isNodeCompleted)
            {
                Logger.Info($"节点完成，准备进入下一个节点或结束流程");
                await MoveToNextNodeOrEndAsync(instance, completedTask);
            }
            else
            {
                Logger.Info($"节点未完成，等待其他任务处理");
            }
        }

        /// <summary>
        /// 处理回退操作
        /// </summary>
        /// <param name="instance">流程实例</param>
        /// <param name="completedTask">已完成的任务</param>
        /// <param name="backNodeId">回退节点ID</param>
        private async Task HandleBackOperation(FlowInstances instance, FlowNodeTasks completedTask, string backNodeId)
        {
            var histories = await _historyRepository.GetAll()
                .Where(x => x.FlowInstanceId == instance.Id)
                .OrderBy(x => x.OperateTime)
                .ToListAsync();

            if (!string.IsNullOrEmpty(backNodeId))
            {
                var targetHistory = histories.LastOrDefault(x => x.NodeId == backNodeId);
                if (targetHistory != null)
                {
                    await BackToNodeAsync(instance, targetHistory);
                }
                else
                {
                    Logger.Error($"找不到回退目标节点: {backNodeId}");
                    await BackToPreviousNodeAsync(instance, histories, completedTask);
                }
            }
            else
            {
                await BackToPreviousNodeAsync(instance, histories, completedTask);
            }
        }

        /// <summary>
        /// 移动到下一个节点或结束流程
        /// </summary>
        /// <param name="instance">流程实例</param>
        /// <param name="completedTask">已完成的任务</param>
        private async Task MoveToNextNodeOrEndAsync(FlowInstances instance, FlowNodeTasks completedTask)
        {
            // 解析当前节点的索引
            int currentIndex = 0;
            var parts = completedTask.NodeId.Split('_');
            if (parts.Length >= 2 && int.TryParse(parts[1], out int index))
            {
                currentIndex = index;
            }

            Logger.Info($"当前节点索引: {currentIndex}");

            // 解析节点配置
            var nodeConfig = JsonConvert.DeserializeObject<JObject>(instance.NodeConfig);

            // 根据索引查找下一个节点
            var nextNode = FindNextNodeByIndex(nodeConfig, currentIndex + 1);

            if (nextNode == null)
            {
                Logger.Info($"没有下一个节点，流程结束");
                await CompleteProcessAsync(instance.Id, STATUS_APPROVED);
                return;
            }

            var nextNodeName = nextNode["name"]?.ToString() ?? "未知节点";
            var nextNodeType = nextNode["type"]?.Value<int>() ?? 0;

            Logger.Info($"找到下一个节点: 名称={nextNodeName}, 类型={nextNodeType}, 索引={currentIndex + 1}");

            // 如果是结束节点
            if (nextNodeType == NODE_END)
            {
                Logger.Info($"下一个节点是结束节点，流程结束");
                await CompleteProcessAsync(instance.Id, STATUS_APPROVED);
                return;
            }

            // 处理下一个节点
            await ProcessNodeAsync(instance, nextNode, instance.Id, instance.BusinessId, instance.BusinessType,
                instance.InitiatorId, instance.FormData, currentIndex + 1);
        }

        /// <summary>
        /// 根据索引查找下一个节点
        /// </summary>
        /// <param name="node">节点配置</param>
        /// <param name="targetIndex">目标索引</param>
        /// <param name="currentIndex">当前索引（递归用）</param>
        /// <returns>节点配置</returns>
        private JObject FindNextNodeByIndex(JObject node, int targetIndex, int currentIndex = 0)
        {
            if (node == null) return null;

            // 如果是开始节点，从子节点开始计数
            var nodeType = node["type"]?.Value<int>() ?? 0;
            if (nodeType == NODE_START)
            {
                var childNode = node["childNode"] as JObject;
                return FindNextNodeByIndex(childNode, targetIndex, 0);
            }

            // 如果当前索引等于目标索引，返回当前节点
            if (currentIndex == targetIndex)
            {
                Logger.Info($"FindNextNodeByIndex: 找到索引 {targetIndex} 的节点, 名称={node["name"]?.ToString()}");
                return node;
            }

            // 检查子节点
            var nextNode = node["childNode"] as JObject;
            if (nextNode != null)
            {
                return FindNextNodeByIndex(nextNode, targetIndex, currentIndex + 1);
            }

            return null;
        }

        /// <summary>
        /// 回退到上一个节点
        /// </summary>
        /// <param name="instance">流程实例</param>
        /// <param name="histories">历史记录</param>
        /// <param name="completedTask">已完成的任务</param>
        private async Task BackToPreviousNodeAsync(FlowInstances instance, List<FlowInstanceHistories> histories, FlowNodeTasks completedTask)
        {
            Logger.Info($"开始回退到上一个节点，当前节点={completedTask.NodeName}");

            var currentHistoryIndex = histories.FindIndex(x => x.Id == completedTask.Id);

            if (currentHistoryIndex <= 0)
            {
                Logger.Info("没有上一个节点，流程结束");
                await CompleteProcessAsync(instance.Id, STATUS_APPROVED);
                return;
            }

            var previousHistory = histories[currentHistoryIndex - 1];

            if (previousHistory.NodeType == NODE_START)
            {
                Logger.Info("上一个节点是开始节点，流程结束");
                await CompleteProcessAsync(instance.Id, STATUS_APPROVED);
                return;
            }

            Logger.Info($"找到上一个节点: 名称={previousHistory.NodeName}, 标识={previousHistory.NodeId}");

            await BackToNodeAsync(instance, previousHistory);
        }

        /// <summary>
        /// 回退到指定节点
        /// </summary>
        /// <param name="instance">流程实例</param>
        /// <param name="targetHistory">目标历史记录</param>
        private async Task BackToNodeAsync(FlowInstances instance, FlowInstanceHistories targetHistory)
        {
            Logger.Info($"开始回退到节点: 名称={targetHistory.NodeName}, 标识={targetHistory.NodeId}");

            // 删除所有待处理任务
            var pendingTasks = await _taskRepository.GetAll()
                .Where(x => x.FlowInstanceId == instance.Id && x.Status == TASK_PENDING)
                .ToListAsync();

            foreach (var task in pendingTasks)
            {
                await _taskRepository.DeleteAsync(task);
                Logger.Info($"删除待处理任务: TaskId={task.Id}, 节点={task.NodeName}");
            }
            await CurrentUnitOfWork.SaveChangesAsync();

            // 重置当前节点为目标节点
            instance.CurrentNodeId = targetHistory.NodeId;
            instance.CurrentNodeName = targetHistory.NodeName;
            instance.CurrentNodeType = targetHistory.NodeType;
            await _flowInstanceRepository.UpdateAsync(instance);
            await CurrentUnitOfWork.SaveChangesAsync();

            // 解析节点配置
            var nodeConfig = JsonConvert.DeserializeObject<JObject>(instance.NodeConfig);

            // 解析节点索引
            int nodeIndex = 0;
            var parts = targetHistory.NodeId.Split('_');
            if (parts.Length >= 2 && int.TryParse(parts[1], out int index))
            {
                nodeIndex = index;
            }

            // 根据索引查找节点配置
            var targetNode = FindNextNodeByIndex(nodeConfig, nodeIndex);

            if (targetNode != null)
            {
                Logger.Info($"找到目标节点配置，重新创建任务");
                await CreateApprovalTasksAsync(instance.Id, targetNode, instance.InitiatorId, instance.FormData, targetHistory.NodeId, nodeIndex);
            }
            else
            {
                Logger.Error($"找不到目标节点配置: {targetHistory.NodeId}");
            }
        }

        #endregion

        #region 审批操作接口

        /// <summary>
        /// 审批通过
        /// </summary>
        /// <param name="input">输入参数</param>
        /// <returns>操作结果</returns>
        [HttpPost]
        public async Task<CommonResult> Approve(ApproveInput input)
        {
            try
            {
                var userId = AbpSession.UserId.Value;
                await HandleTaskAsync(input.TaskId, CMD_APPROVED, userId, input.Comment, input.FormData);
                return CommonResult.Ok("审批通过成功");
            }
            catch (Exception ex)
            {
                Logger.Error("审批通过失败", ex);
                return CommonResult.Error("审批通过失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 审批拒绝
        /// </summary>
        /// <param name="input">输入参数</param>
        /// <returns>操作结果</returns>
        [HttpPost]
        public async Task<CommonResult> Reject(RejectInput input)
        {
            try
            {
                var userId = AbpSession.UserId.Value;
                await HandleTaskAsync(input.TaskId, CMD_REJECTED, userId, input.Comment, null);
                return CommonResult.Ok("审批拒绝成功");
            }
            catch (Exception ex)
            {
                Logger.Error("审批拒绝失败", ex);
                return CommonResult.Error("审批拒绝失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 回退
        /// </summary>
        /// <param name="input">输入参数</param>
        /// <returns>操作结果</returns>
        [HttpPost]
        public async Task<CommonResult> Back(BackInput input)
        {
            try
            {
                var userId = AbpSession.UserId.Value;
                await HandleTaskAsync(input.TaskId, CMD_BACK, userId, input.Comment, null, input.BackNodeId);
                return CommonResult.Ok("回退成功");
            }
            catch (Exception ex)
            {
                Logger.Error("回退失败", ex);
                return CommonResult.Error("回退失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 撤销流程
        /// </summary>
        /// <param name="input">输入参数</param>
        /// <returns>操作结果</returns>
        [HttpPost]
        public async Task<CommonResult> Cancel(CancelFlowInput input)
        {
            try
            {
                var userId = AbpSession.UserId.Value;
                await CancelProcessAsync(input.FlowInstanceId, userId, input.Reason);
                return CommonResult.Ok("撤销成功");
            }
            catch (Exception ex)
            {
                Logger.Error("撤销失败", ex);
                return CommonResult.Error("撤销失败：" + ex.Message);
            }
        }

        #endregion

        #region 流程控制

        /// <summary>
        /// 撤销流程
        /// </summary>
        /// <param name="flowInstanceId">流程实例ID</param>
        /// <param name="operatorId">操作人ID</param>
        /// <param name="reason">撤销原因</param>
        /// <returns>操作结果</returns>
        public async Task<bool> CancelProcessAsync(Guid flowInstanceId, long operatorId, string reason)
        {
            var instance = await _flowInstanceRepository.GetAsync(flowInstanceId);
            if (instance == null)
                throw new Exception("流程实例不存在");

            if (instance.Status != STATUS_UNDERWAY)
                throw new Exception("流程已结束，不能撤销");

            var isAdmin = false;
            if (instance.InitiatorId != operatorId && !isAdmin)
                throw new Exception("只有发起人才能撤销流程");

            var operatorName = await _userAppService.GetNameByUserId(operatorId);

            var tasks = await _taskRepository.GetAll()
                .Where(x => x.FlowInstanceId == flowInstanceId && x.Status == TASK_PENDING)
                .ToListAsync();

            foreach (var task in tasks)
            {
                task.Status = TASK_SKIPPED;
                await _taskRepository.UpdateAsync(task);
                Logger.Info($"撤销流程，跳过任务: TaskId={task.Id}");
            }

            await AddHistoryAsync(flowInstanceId, instance.CurrentNodeId, instance.CurrentNodeName, instance.CurrentNodeType,
                CMD_CANCELED, operatorId, operatorName, reason, instance.FormData, instance.FormData);

            instance.Status = STATUS_CANCELLED;
            instance.EndTime = DateTime.Now;
            await _flowInstanceRepository.UpdateAsync(instance);
            await CurrentUnitOfWork.SaveChangesAsync();

            await TriggerProcessCompletedCallback(flowInstanceId, STATUS_CANCELLED);

            return true;
        }

        /// <summary>
        /// 完成流程
        /// </summary>
        /// <param name="flowInstanceId">流程实例ID</param>
        /// <param name="status">流程状态</param>
        private async Task CompleteProcessAsync(Guid flowInstanceId, int status)
        {
            var instance = await _flowInstanceRepository.GetAsync(flowInstanceId);
            instance.Status = status;
            instance.EndTime = DateTime.Now;
            await _flowInstanceRepository.UpdateAsync(instance);
            await CurrentUnitOfWork.SaveChangesAsync();

            Logger.Info($"流程完成: FlowInstanceId={flowInstanceId}, 状态={GetStatusName(status)}");

            await TriggerProcessCompletedCallback(flowInstanceId, status);
        }

        /// <summary>
        /// 触发流程完成回调
        /// </summary>
        /// <param name="flowInstanceId">流程实例ID</param>
        /// <param name="status">流程状态</param>
        private async Task TriggerProcessCompletedCallback(Guid flowInstanceId, int status)
        {
            var instance = await _flowInstanceRepository.GetAsync(flowInstanceId);
            if (instance == null) return;

            if (instance.BusinessType == "DeviceChangeApplication")
            {
                try
                {
                    Logger.Info($"流程完成回调: FlowInstanceId={flowInstanceId}, Status={status}, BusinessId={instance.BusinessId}");

                    // 直接通过仓储调用设备服务的方法
                    await OnDeviceProcessCompleted(instance.BusinessId, status);

                    var apply = await _changeApplyRepository.FirstOrDefaultAsync(instance.BusinessId);
                    if (apply != null)
                    {
                        if (status == STATUS_APPROVED)
                            apply.ApplicationStatus = "已通过";
                        else if (status == STATUS_REJECTED)
                            apply.ApplicationStatus = "已拒绝";
                        else if (status == STATUS_CANCELLED)
                            apply.ApplicationStatus = "已撤销";

                        await _changeApplyRepository.UpdateAsync(apply);
                        await CurrentUnitOfWork.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"调用设备流程回调失败: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// 处理设备流程完成
        /// </summary>
        /// <param name="applyId">申请ID</param>
        /// <param name="status">流程状态</param>
        private async Task OnDeviceProcessCompleted(Guid applyId, int status)
        {
            try
            {
                var apply = await _changeApplyRepository.FirstOrDefaultAsync(applyId);
                if (apply == null) return;

                var relation = await _deviceChangeRelationRepository
                    .FirstOrDefaultAsync(x => x.DeviceChangeApplicationId == applyId);

                if (relation == null) return;

                // 根据流程状态处理
                if (status == STATUS_APPROVED) // 通过
                {
                    var newData = DeviceJsonHelper.DeserializeDeviceEditInput(apply.NewData);

                    if (apply.ChangeType == "新增")
                    {
                        // 创建设备
                        var device = new Devices();
                        UpdateDeviceEntity(device, newData);
                        device.Creator = apply.SubmitterName;

                        var deviceId = await _deviceRepository.InsertAndGetIdAsync(device);

                        // 更新关系中的设备ID
                        relation.DeviceId = deviceId;
                        await _deviceChangeRelationRepository.UpdateAsync(relation);

                        // 创建保养计划
                        await CreateMaintenancePlansForDevice(deviceId, newData);
                    }
                    else if (apply.ChangeType == "编辑")
                    {
                        var device = await _deviceRepository.FirstOrDefaultAsync(relation.DeviceId);
                        if (device != null)
                        {
                            UpdateDeviceEntity(device, newData);
                            await _deviceRepository.UpdateAsync(device);

                            // 检查设备是否已有保养计划
                            var hasExistingPlans = await _maintenancePlanRepository.GetAll()
                                .AnyAsync(p => p.DeviceId == device.Id);

                            if (hasExistingPlans)
                            {
                                // 已有保养计划：更新现有计划
                                await UpdateMaintenancePlansForDevice(device, newData);
                            }
                            else
                            {
                                // 没有保养计划：创建新计划
                                await CreateMaintenancePlansForDevice(device.Id, newData);
                            }
                        }
                    }
                    else if (apply.ChangeType == "删除")
                    {
                        // 软删除设备
                        await _deviceRepository.DeleteAsync(relation.DeviceId);

                        // 停用保养计划
                        await DeactivateMaintenancePlansForDevice(relation.DeviceId);
                    }
                }
                else if (status == STATUS_REJECTED || status == STATUS_CANCELLED)
                {
                    // 拒绝或撤销，无需处理
                    Logger.Info($"设备申请被拒绝或撤销: ApplyId={applyId}, Status={status}");
                }

                await CurrentUnitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logger.Error($"处理设备流程完成失败: {ex.Message}", ex);
                throw;
            }
        }

        /// <summary>
        /// 更新设备实体
        /// </summary>
        private void UpdateDeviceEntity(Devices device, DeviceEditInput input)
        {
            device.DeviceCode = input.DeviceCode;
            device.DeviceName = input.DeviceName;
            device.Specification = input.Specification;
            device.DeviceLevel = input.DeviceLevel;
            device.IsKeyDevice = input.IsKeyDevice;
            device.TechnicalParameters = JsonConvert.SerializeObject(input.TechnicalParameters ?? new List<TechnicalParameterItem>());
            device.CustomerRequirements = JsonConvert.SerializeObject(input.CustomerRequirements ?? new List<CustomerRequirementItem>());
            device.LogisticsNo = input.LogisticsNo;
            device.FactoryNo = input.FactoryNo;
            device.Manufacturer = input.Manufacturer;
            device.ManufactureDate = input.ManufactureDate;
            device.PurchaseNo = input.PurchaseNo;
            device.SourceType = input.SourceType;
            device.Location = input.Location;
            device.DeviceStatus = input.DeviceStatus;
            device.EnableDate = input.EnableDate;
        }

        /// <summary>
        /// 为设备创建保养计划
        /// </summary>
        private async Task CreateMaintenancePlansForDevice(Guid deviceId, DeviceEditInput data)
        {
            try
            {
                var plans = new List<(string Level, MaintenancePlanDto PlanData)>();

                // 收集非空的计划
                if (data.MonthlyMaintenance != null)
                    plans.Add(("月度", data.MonthlyMaintenance));

                if (data.QuarterlyMaintenance != null)
                    plans.Add(("季度", data.QuarterlyMaintenance));

                if (data.HalfYearlyMaintenance != null)
                    plans.Add(("半年度", data.HalfYearlyMaintenance));

                if (data.AnnualMaintenance != null)
                    plans.Add(("年度", data.AnnualMaintenance));

                foreach (var (level, planData) in plans)
                {
                    // 获取模板
                    var template = await _maintenanceTemplateRepository.FirstOrDefaultAsync(planData.TemplateId);
                    if (template == null)
                    {
                        Logger.Warn($"模板不存在: {planData.TemplateId}");
                        continue;
                    }

                    // 计算首次保养日期 = 启用日期 + 周期天数
                    DateTime firstDate = 
                        (DateTime)data.EnableDate < DateTime.Today ?
                        CalculateNextMaintenanceDate(DateTime.Today, level) : CalculateNextMaintenanceDate((DateTime)data.EnableDate, level);
                    DateTime nextDate = firstDate;

                    // 创建计划
                    var plan = new MaintenancePlans
                    {
                        PlanName = $"{template.TemplateName} - {level}保养计划",
                        DeviceId = deviceId,
                        TemplateId = template.Id,
                        MaintenanceLevel = level,
                        CycleType = MaintenanceCycleConstants.GetCycleType(level),
                        CycleDays = MaintenanceCycleConstants.GetCycleDays(level),
                        FirstMaintenanceDate = firstDate,
                        NextMaintenanceDate = nextDate,
                        Status = "启用",
                    };

                    var planId = await _maintenancePlanRepository.InsertAndGetIdAsync(plan);

                    // 创建关联关系
                    var relation = new DeviceMaintenancePlanRelation
                    {
                        DeviceId = deviceId,
                        MaintenancePlanId = planId,
                        MaintenanceLevel = level,
                        TemplateId = template.Id
                    };
                    await _deviceMaintenancePlanRelationRepository.InsertAsync(relation);
                }

                await CurrentUnitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logger.Error($"创建保养计划失败: DeviceId={deviceId}", ex);
                // 不抛出异常，避免影响设备创建
            }
        }

        /// <summary>
        /// 更新设备保养计划
        /// </summary>
        private async Task UpdateMaintenancePlansForDevice(Devices device, DeviceEditInput data)
        {
            try
            {
                // 获取设备现有计划
                var existingRelations = await _deviceMaintenancePlanRelationRepository.GetAll()
                    .Where(x => x.DeviceId == device.Id)
                    .ToListAsync();

                var existingPlanIds = existingRelations.Select(x => x.MaintenancePlanId).ToList();
                var existingPlans = await _maintenancePlanRepository.GetAll()
                    .Where(x => existingPlanIds.Contains(x.Id))
                    .ToDictionaryAsync(x => x.MaintenanceLevel);

                // 处理各个周期的计划
                await UpdateSinglePlan(device, existingPlans, "月度", data.MonthlyMaintenance);
                await UpdateSinglePlan(device, existingPlans, "季度", data.QuarterlyMaintenance);
                await UpdateSinglePlan(device, existingPlans, "半年度", data.HalfYearlyMaintenance);
                await UpdateSinglePlan(device, existingPlans, "年度", data.AnnualMaintenance);

                await CurrentUnitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logger.Error($"更新保养计划失败: DeviceId={device.Id}", ex);
                // 不抛出异常，避免影响设备更新
            }
        }

        /// <summary>
        /// 更新单个计划
        /// </summary>
        private async Task UpdateSinglePlan(Devices device, Dictionary<string, MaintenancePlans> existingPlans, string level, MaintenancePlanDto input)
        {
            // 如果输入为空或模板ID为空，且存在旧计划，则删除
            if (input == null)
            {
                if (existingPlans.ContainsKey(level))
                {
                    var oldPlan = existingPlans[level];
                    await _maintenancePlanRepository.DeleteAsync(oldPlan.Id);
                    await _deviceMaintenancePlanRelationRepository.DeleteAsync(x => x.MaintenancePlanId == oldPlan.Id);
                }
                return;
            }

           
            int cycleDays = MaintenanceCycleConstants.GetCycleDays(level);
            // 计算首次保养日期
            DateTime firstDate = (DateTime)device.EnableDate < DateTime.Today ?
            CalculateNextMaintenanceDate(DateTime.Today, level) : CalculateNextMaintenanceDate((DateTime)device.EnableDate, level);
            DateTime nextDate = firstDate;

            // 如果存在旧计划，更新；否则创建新计划
            if (existingPlans.ContainsKey(level))
            {
                var oldPlan = existingPlans[level];
                oldPlan.TemplateId = input.TemplateId;
                oldPlan.NextMaintenanceDate = firstDate;
                await _maintenancePlanRepository.UpdateAsync(oldPlan);

                // 更新关系
                var relation = await _deviceMaintenancePlanRelationRepository.FirstOrDefaultAsync(x => x.MaintenancePlanId == oldPlan.Id);
                if (relation != null)
                {
                    relation.TemplateId = input.TemplateId;
                    await _deviceMaintenancePlanRelationRepository.UpdateAsync(relation);
                }
            }
            else
            {
                // 创建新计划
                var template = await _maintenanceTemplateRepository.GetAsync(input.TemplateId);
                var plan = new MaintenancePlans
                {
                    PlanName = $"{template.TemplateName} - {level}保养计划",
                    DeviceId = device.Id,
                    TemplateId = template.Id,
                    MaintenanceLevel = level,
                    CycleType = MaintenanceCycleConstants.GetCycleType(level),
                    CycleDays = cycleDays,
                    FirstMaintenanceDate = firstDate,
                    NextMaintenanceDate = nextDate, 
                    Status = "启用",
                };
                var planId = await _maintenancePlanRepository.InsertAndGetIdAsync(plan);

                var relation = new DeviceMaintenancePlanRelation
                {
                    DeviceId = device.Id,
                    MaintenancePlanId = planId,
                    MaintenanceLevel = level,
                    TemplateId = template.Id
                };
                await _deviceMaintenancePlanRelationRepository.InsertAsync(relation);
            }
        }


        /// <summary>
        /// 停用设备保养计划
        /// </summary>
        private async Task DeactivateMaintenancePlansForDevice(Guid deviceId)
        {
            try
            {
                var relations = await _deviceMaintenancePlanRelationRepository.GetAll()
                    .Where(x => x.DeviceId == deviceId)
                    .Select(x => x.MaintenancePlanId)
                    .ToListAsync();

                foreach (var planId in relations)
                {
                    var plan = await _maintenancePlanRepository.FirstOrDefaultAsync(planId);
                    if (plan != null)
                    {
                        plan.Status = "停用";
                        await _maintenancePlanRepository.UpdateAsync(plan);
                    }
                }

                await CurrentUnitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logger.Error($"停用设备保养计划失败: DeviceId={deviceId}", ex);
            }
        }

        /// <summary>
        /// 计算下次保养日期
        /// </summary>
        /// <param name="lastDate">上次保养日期</param>
        /// <param name="level">保养等级</param>
        /// <returns>下次保养日期（确保在当前日期之后，且为工作日）</returns>
        private static DateTime CalculateNextMaintenanceDate(DateTime lastDate, string level)
        {
            int days = MaintenanceCycleConstants.GetCycleDays(level);
            DateTime today = DateTime.Today;

            DateTime nextDate;

            if (lastDate < today)
            {
                // 如果上次保养日期早于今天，计算需要多少个周期才能超过今天
                // 计算从lastDate到today已经过去的天数
                int daysPassed = (today - lastDate).Days;

                // 计算需要的周期倍数（向上取整）
                int cyclesNeeded = (int)Math.Ceiling((double)daysPassed / days);

                // 确保至少一个周期
                cyclesNeeded = Math.Max(1, cyclesNeeded);

                // 计算下次保养日期
                nextDate = lastDate.AddDays(days * cyclesNeeded);

                // 如果计算出的日期还是今天或之前（由于取整问题），再加一个周期
                while (nextDate <= today)
                {
                    nextDate = nextDate.AddDays(days);
                }
            }
            else
            {
                // 如果上次保养日期是今天或未来，正常加一个周期
                nextDate = lastDate.AddDays(days);
            }

            // 确保是工作日，如果不是则顺延到下一个工作日
            while (!WorkdayHelper.IsWorkday(nextDate))
            {
                nextDate = nextDate.AddDays(1);
            }

            return nextDate;
        }


        #endregion

        #region 查询方法

        /// <summary>
        /// 获取待办任务列表
        /// </summary>
        /// <param name="businessType">业务类型</param>
        /// <param name="searchKey">搜索关键字</param>
        /// <param name="current">当前页</param>
        /// <param name="size">每页大小</param>
        /// <returns>待办任务列表</returns>
        public async Task<Page<FlowPendingItemDto>> GetPendingTasksAsync(string businessType, string searchKey, int current, int size)
        {
            var userId = AbpSession.UserId;

            var query = from task in _taskRepository.GetAll()
                        join instance in _flowInstanceRepository.GetAll() on task.FlowInstanceId equals instance.Id
                        where task.AssigneeId == userId
                            && task.Status == TASK_PENDING
                            && (string.IsNullOrEmpty(businessType) || instance.BusinessType == businessType)
                        select new { task, instance };

            if (!string.IsNullOrEmpty(searchKey))
            {
                query = query.Where(x => x.instance.FlowName.Contains(searchKey)
                    || x.instance.Code.Contains(searchKey));
            }

            query = query.OrderByDescending(x => x.task.CreationTime);

            var total = await query.CountAsync();

            var items = await query
                .Skip((current - 1) * size)
                .Take(size)
                .Select(x => new FlowPendingItemDto
                {
                    TaskId = x.task.Id,
                    FlowInstanceId = x.instance.Id,
                    FlowCode = x.instance.Code,
                    FlowName = x.instance.FlowName,
                    BusinessId = x.instance.BusinessId,
                    BusinessType = x.instance.BusinessType,
                    NodeName = x.task.NodeName,
                    NodeType = x.task.NodeType,
                    InitiatorName = x.instance.InitiatorName,
                    BeginTime = x.instance.BeginTime,
                    CreateTime = x.task.CreationTime
                })
                .ToListAsync();

            foreach (var item in items)
            {
                item.BusinessName = await GetBusinessNameAsync(item.BusinessId, item.BusinessType);
            }

            return new Page<FlowPendingItemDto>(current, size, total)
            {
                Records = items
            };
        }

        /// <summary>
        /// 获取我已审批列表
        /// </summary>
        /// <param name="businessType">业务类型</param>
        /// <param name="searchKey">搜索关键字</param>
        /// <param name="status">流程状态</param>
        /// <param name="current">当前页</param>
        /// <param name="size">每页大小</param>
        /// <returns>已审批列表</returns>
        public async Task<Page<FlowProcessedItemDto>> GetMyApprovedAsync(string businessType, string searchKey, int? status, int current, int size)
        {
            try
            {
                var userId = AbpSession.UserId;

                var query = from history in _historyRepository.GetAll()
                            join instance in _flowInstanceRepository.GetAll() on history.FlowInstanceId equals instance.Id
                            where history.OperatorId == userId
                                && (history.FlowCmd == CMD_APPROVED || history.FlowCmd == CMD_REJECTED || history.FlowCmd == CMD_BACK)
                                && (string.IsNullOrEmpty(businessType) || instance.BusinessType == businessType)
                            select instance;

                query = query.Distinct();

                if (!string.IsNullOrEmpty(searchKey))
                {
                    query = query.Where(x => x.FlowName.Contains(searchKey)
                        || x.Code.Contains(searchKey));
                }

                if (status.HasValue)
                {
                    query = query.Where(x => x.Status == status.Value);
                }

                query = query.OrderByDescending(x => x.CreationTime);

                var total = await query.CountAsync();

                var items = await query
                    .Skip((current - 1) * size)
                    .Take(size)
                    .Select(x => new FlowProcessedItemDto
                    {
                        FlowInstanceId = x.Id,
                        FlowCode = x.Code,
                        FlowName = x.FlowName,
                        BusinessId = x.BusinessId,
                        BusinessType = x.BusinessType,
                        InitiatorName = x.InitiatorName,
                        BeginTime = x.BeginTime,
                        EndTime = x.EndTime,
                        Status = x.Status,
                        StatusName = GetStatusName(x.Status)
                    })
                    .ToListAsync();

                foreach (var item in items)
                {
                    item.BusinessName = await GetBusinessNameAsync(item.BusinessId, item.BusinessType);
                }

                return new Page<FlowProcessedItemDto>(current, size, total)
                {
                    Records = items
                };
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("获取已审批列表失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 获取我发起的列表
        /// </summary>
        /// <param name="businessType">业务类型</param>
        /// <param name="searchKey">搜索关键字</param>
        /// <param name="status">流程状态</param>
        /// <param name="current">当前页</param>
        /// <param name="size">每页大小</param>
        /// <returns>我发起的列表</returns>
        public async Task<Page<FlowProcessedItemDto>> GetMyInitiatedAsync(string businessType, string searchKey, int? status, int current, int size)
        {
            var userId = AbpSession.UserId;

            var query = _flowInstanceRepository.GetAll()
                .Where(x => x.InitiatorId == userId);

            if (!string.IsNullOrEmpty(businessType))
            {
                query = query.Where(x => x.BusinessType == businessType);
            }

            if (!string.IsNullOrEmpty(searchKey))
            {
                query = query.Where(x => x.FlowName.Contains(searchKey)
                    || x.Code.Contains(searchKey));
            }

            if (status.HasValue)
            {
                query = query.Where(x => x.Status == status.Value);
            }

            query = query.OrderByDescending(x => x.CreationTime);

            var total = await query.CountAsync();

            var items = await query
                .Skip((current - 1) * size)
                .Take(size)
                .Select(x => new FlowProcessedItemDto
                {
                    FlowInstanceId = x.Id,
                    FlowCode = x.Code,
                    FlowName = x.FlowName,
                    BusinessId = x.BusinessId,
                    BusinessType = x.BusinessType,
                    InitiatorName = x.InitiatorName,
                    BeginTime = x.BeginTime,
                    EndTime = x.EndTime,
                    Status = x.Status,
                    StatusName = GetStatusName(x.Status)
                })
                .ToListAsync();

            foreach (var item in items)
            {
                item.BusinessName = await GetBusinessNameAsync(item.BusinessId, item.BusinessType);
            }

            return new Page<FlowProcessedItemDto>(current, size, total)
            {
                Records = items
            };
        }

        /// <summary>
        /// 获取流程详情
        /// </summary>
        /// <param name="flowInstanceId">流程实例ID</param>
        /// <returns>流程详情</returns>
        public async Task<FlowDetailDto> GetFlowDetailAsync(Guid flowInstanceId)
        {
            try
            {
                var instance = await _flowInstanceRepository.GetAsync(flowInstanceId);
                if (instance == null)
                    throw new Exception("流程实例不存在");

                var histories = await _historyRepository.GetAll()
                    .Where(x => x.FlowInstanceId == flowInstanceId)
                    .OrderBy(x => x.OperateTime)
                    .ToListAsync();

                var nodeRecords = new List<FlowNodeRecordDto>();

                foreach (var history in histories)
                {
                    nodeRecords.Add(new FlowNodeRecordDto
                    {
                        NodeId = history.NodeId,
                        NodeName = history.NodeName,
                        NodeType = history.NodeType,
                        NodeTypeName = GetNodeTypeName(history.NodeType),
                        FlowCmd = history.FlowCmd,
                        FlowCmdName = GetCmdName(history.FlowCmd),
                        OperatorId = history.OperatorId,
                        OperatorName = history.OperatorName,
                        OperateTime = history.OperateTime,
                        Comment = history.Comment,
                        Underway = false
                    });
                }

                // 获取待处理节点
                var futureNodes = new List<FlowNodeRecordDto>();
                var pendingTasks = await _taskRepository.GetAll()
                    .Where(x => x.FlowInstanceId == flowInstanceId && x.Status == TASK_PENDING)
                    .ToListAsync();

                var nodeGroups = pendingTasks.GroupBy(x => x.NodeId);
                foreach (var group in nodeGroups)
                {
                    var task = group.First();
                    var userNames = new List<string>();
                    foreach (var t in group)
                    {
                        if (t.AssigneeId.HasValue)
                        {
                            var userName = await _userAppService.GetNameByUserId(t.AssigneeId.Value);
                            userNames.Add(userName);
                        }
                    }

                    futureNodes.Add(new FlowNodeRecordDto
                    {
                        NodeId = task.NodeId,
                        NodeName = task.NodeName,
                        NodeType = task.NodeType,
                        NodeTypeName = GetNodeTypeName(task.NodeType),
                        Underway = true,
                        UserIds = group.Select(x => x.AssigneeId.Value).ToList(),
                        UserNames = userNames // 新增字段，用于前端显示
                    });
                }

                // 判断当前是否有正在处理的节点（第一个待处理节点即为当前节点）
                bool hasCurrentNode = futureNodes.Count > 0;

                // 处理结束节点显示
                if (instance.Status != STATUS_UNDERWAY)
                {
                    // 检查是否已有结束节点记录
                    bool hasEndNode = nodeRecords.Any(x => x.NodeType == NODE_END);

                    if (!hasEndNode)
                    {
                        // 创建结束节点记录
                        var endNodeRecord = new FlowNodeRecordDto
                        {
                            NodeId = "END_NODE",
                            NodeName = "结束",
                            NodeType = NODE_END,
                            NodeTypeName = GetNodeTypeName(NODE_END),
                            Underway = false,
                            OperateTime = instance.EndTime ?? DateTime.Now
                        };

                        // 根据不同的结束状态设置操作信息
                        if (instance.Status == STATUS_APPROVED)
                        {
                            endNodeRecord.FlowCmd = CMD_AUTO_APPROVED;
                            endNodeRecord.FlowCmdName = "流程通过";
                            endNodeRecord.Comment = "流程审批通过，自动结束";
                        }
                        else if (instance.Status == STATUS_REJECTED)
                        {
                            endNodeRecord.FlowCmd = CMD_AUTO_REJECTED;
                            endNodeRecord.FlowCmdName = "流程拒绝";
                            endNodeRecord.Comment = "流程被拒绝，已结束";
                        }
                        else if (instance.Status == STATUS_CANCELLED)
                        {
                            endNodeRecord.FlowCmd = CMD_CANCELED;
                            endNodeRecord.FlowCmdName = "流程撤销";
                            endNodeRecord.Comment = "流程已被撤销";
                        }

                        var lastHistory = histories.LastOrDefault();
                        if (lastHistory != null)
                        {
                            endNodeRecord.OperatorId = lastHistory.OperatorId;
                            endNodeRecord.OperatorName = lastHistory.OperatorName;
                        }

                        // 将结束节点添加到历史记录中
                        nodeRecords.Add(endNodeRecord);
                    }
                }

                // 获取业务数据
                object businessData = null;
                if (instance.BusinessType == "DeviceChangeApplication")
                {
                    var apply = await _changeApplyRepository.FirstOrDefaultAsync(instance.BusinessId);
                    if (apply != null)
                    {
                        var relation = await _deviceChangeRelationRepository
                            .FirstOrDefaultAsync(x => x.DeviceChangeApplicationId == apply.Id);

                        Devices device = null;
                        // 获取保养计划信息（如果设备已存在）
                        object maintenancePlans = null;
                        if (relation != null && relation.DeviceId != Guid.Empty)
                        {
                            device = await _deviceRepository.FirstOrDefaultAsync(relation.DeviceId);

                            try
                            {
                                // 直接使用仓储获取保养计划
                                var plans = await _maintenancePlanRepository.GetAll()
                                    .Where(p => p.DeviceId == device.Id)
                                    .ToListAsync();
                                maintenancePlans = plans;
                            }
                            catch (Exception ex)
                            {
                                Logger.Warn($"获取保养计划失败: {ex.Message}");
                            }
                        }

                        DeviceEditInput snapshot = null;
                        if (!string.IsNullOrEmpty(apply.Snapshot))
                        {
                            snapshot = DeviceJsonHelper.DeserializeDeviceEditInput(apply.Snapshot);
                        }

                        DeviceEditInput newData = null;
                        if (!string.IsNullOrEmpty(apply.NewData))
                        {
                            newData = DeviceJsonHelper.DeserializeDeviceEditInput(apply.NewData);
                        }

                        businessData = new
                        {
                            ApplyId = apply.Id,
                            DeviceId = relation?.DeviceId,
                            DeviceName = device?.DeviceName ?? newData?.DeviceName,
                            ChangeType = apply.ChangeType,
                            ApplicationStatus = apply.ApplicationStatus,
                            ApplicantId = relation?.SubmitterId,
                            ApplicantName = relation?.SubmitterName,
                            ApplyReason = apply.ApplyReason,
                            SubmitTime = apply.SubmitTime,
                            Snapshot = snapshot,
                            NewData = newData,
                            MaintenancePlans = maintenancePlans
                        };
                    }
                }

                return new FlowDetailDto
                {
                    FlowInstance = new FlowInstanceDto
                    {
                        Id = instance.Id,
                        Code = instance.Code,
                        FlowName = instance.FlowName,
                        FlowDefinitionId = instance.FlowDefinitionId,
                        BusinessId = instance.BusinessId,
                        BusinessType = instance.BusinessType,
                        InitiatorId = instance.InitiatorId,
                        InitiatorName = instance.InitiatorName,
                        CurrentNodeId = instance.CurrentNodeId,
                        CurrentNodeName = instance.CurrentNodeName,
                        CurrentNodeType = instance.CurrentNodeType,
                        Status = instance.Status,
                        StatusName = GetStatusName(instance.Status),
                        BeginTime = instance.BeginTime,
                        EndTime = instance.EndTime,
                        Cancelable = instance.Cancelable,
                        Editable = pendingTasks.Any(x => x.FormAuths != null && x.FormAuths.Contains("\"editable\":true"))
                    },
                    NodeRecords = nodeRecords,
                    FutureNodes = futureNodes,
                    BusinessData = businessData
                };
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        /// <summary>
        /// 获取节点表单权限
        /// </summary>
        /// <param name="taskId">任务ID</param>
        /// <returns>表单权限配置</returns>
        public async Task<string> GetNodeFormAuthsAsync(Guid taskId)
        {
            var task = await _taskRepository.GetAsync(taskId);
            return task?.FormAuths;
        }

        /// <summary>
        /// 获取可回退节点列表
        /// </summary>
        /// <param name="flowInstanceId">流程实例ID</param>
        /// <param name="currentNodeId">当前节点ID</param>
        /// <returns>可回退节点列表</returns>
        public async Task<List<object>> GetBackableNodesAsync(Guid flowInstanceId, string currentNodeId)
        {
            var histories = await _historyRepository.GetAll()
                .Where(x => x.FlowInstanceId == flowInstanceId
                    && (x.NodeType == NODE_APPROVE || x.NodeType == NODE_TRANSACT)
                    && x.FlowCmd != CMD_AUTO_APPROVED
                    && x.FlowCmd != CMD_AUTO_REJECTED)
                .OrderBy(x => x.OperateTime)
                .ToListAsync();

            var nodes = new List<object>();
            var addedNodeIds = new HashSet<string>();

            foreach (var history in histories)
            {
                if (!addedNodeIds.Contains(history.NodeId) && history.NodeId != currentNodeId)
                {
                    nodes.Add(new
                    {
                        id = history.NodeId,
                        name = history.NodeName,
                        type = history.NodeType
                    });
                    addedNodeIds.Add(history.NodeId);
                }
            }

            return nodes;
        }

        #endregion

        #region 流程预览

        /// <summary>
        /// 预览流程节点图表
        /// </summary>
        /// <param name="formCode">表单编码</param>
        /// <returns>节点列表</returns>
        [HttpGet]
        public async Task<CommonResult<List<FlowNodePreviewDto>>> ViewProcessChartAsync(string formCode)
        {
            var result = new List<FlowNodePreviewDto>();

            var flowForm = await _formRepository.FirstOrDefaultAsync(it => string.Equals(it.FormCode, formCode));
            if (flowForm == null || flowForm.FlowDefId == null)
            {
                return CommonResult<List<FlowNodePreviewDto>>.Error("当前表单未绑定流程，请联系管理员！");
            }

            var flowDef = await _flowDefRepository.FirstOrDefaultAsync(x => x.Id == flowForm.FlowDefId);
            if (flowDef == null || string.IsNullOrEmpty(flowDef.NodeConfig))
                return CommonResult<List<FlowNodePreviewDto>>.Error("流程定义不存在");

            try
            {
                var nodeConfig = JsonConvert.DeserializeObject<JObject>(flowDef.NodeConfig);
                if (nodeConfig == null)
                    return CommonResult<List<FlowNodePreviewDto>>.Error("预览流程节点图表失败");

                ParseNodeForPreview(nodeConfig, result);

                if (result.Count > 0)
                {
                    var lastNode = result.Last();
                    if (lastNode.NodeType != NODE_END)
                    {
                        result.Add(CreateEndNode());
                    }
                }
                else
                {
                    result.Add(CreateStartNode());
                    result.Add(CreateEndNode());
                }

                return CommonResult<List<FlowNodePreviewDto>>.Success(result);
            }
            catch (Exception ex)
            {
                Logger.Error("预览流程节点图表失败", ex);
                return CommonResult<List<FlowNodePreviewDto>>.Error("预览流程节点图表失败" + ex.Message);
            }
        }

        /// <summary>
        /// 解析节点用于预览
        /// </summary>
        /// <param name="node">节点配置</param>
        /// <param name="nodes">节点列表</param>
        private static void ParseNodeForPreview(JObject node, List<FlowNodePreviewDto> nodes)
        {
            if (node == null) return;

            var nodeType = node["type"]?.Value<int>() ?? 0;

            var nodeId = node["id"]?.ToString() ?? Guid.NewGuid().ToString();

            var previewNode = new FlowNodePreviewDto
            {
                Id = GenerateNodeId(nodeType),
                Name = node["name"]?.ToString() ?? GetNodeTypeName(nodeType),
                NodeId = nodeId,
                NodeType = nodeType
            };

            switch (nodeType)
            {
                case NODE_APPROVE:
                case NODE_TRANSACT:
                    ParseApproveNode(node, previewNode);
                    break;

                case NODE_COPY:
                    ParseCopyNode(node, previewNode);
                    break;
            }

            if (nodeType != NODE_GATEWAY)
            {
                nodes.Add(previewNode);
            }

            var childNode = node["childNode"] as JObject;
            if (childNode != null)
            {
                ParseNodeForPreview(childNode, nodes);
            }

            var conditionNodes = node["conditionNodes"] as JArray;
            if (conditionNodes != null)
            {
                foreach (var conditionNode in conditionNodes)
                {
                    var conditionChild = (conditionNode as JObject)?["childNode"] as JObject;
                    if (conditionChild != null)
                    {
                        ParseNodeForPreview(conditionChild, nodes);
                    }
                }
            }
        }

        /// <summary>
        /// 解析审批/办理节点
        /// </summary>
        /// <param name="node">节点配置</param>
        /// <param name="previewNode">预览节点</param>
        private static void ParseApproveNode(JObject node, FlowNodePreviewDto previewNode)
        {
            previewNode.ApprovalType = node["approvalType"]?.Value<int>() ?? 0;
            previewNode.MultiInstanceApprovalType = node["multiInstanceApprovalType"]?.Value<int>() ?? 0;
            previewNode.FlowNodeNoAuditorType = node["flowNodeNoAuditorType"]?.Value<int>() ?? 0;
            previewNode.FlowNodeNoAuditorAssignee = node["flowNodeNoAuditorAssignee"]?.ToString();

            var assignees = node["assignees"] as JArray;
            if (assignees != null && assignees.Any())
            {
                foreach (var assignee in assignees)
                {
                    ParseAssignee(assignee as JObject, previewNode);
                }
            }
        }

        /// <summary>
        /// 解析抄送节点
        /// </summary>
        /// <param name="node">节点配置</param>
        /// <param name="previewNode">预览节点</param>
        private static void ParseCopyNode(JObject node, FlowNodePreviewDto previewNode)
        {
            var ccs = node["ccs"] as JArray;
            if (ccs != null && ccs.Any())
            {
                foreach (var cc in ccs)
                {
                    ParseAssignee(cc as JObject, previewNode);
                }
            }
        }

        /// <summary>
        /// 解析审批人
        /// </summary>
        /// <param name="assignee">审批人配置</param>
        /// <param name="previewNode">预览节点</param>
        private static void ParseAssignee(JObject assignee, FlowNodePreviewDto previewNode)
        {
            if (assignee == null) return;

            var assigneeType = assignee["assigneeType"]?.Value<int>() ?? 0;

            switch (assigneeType)
            {
                case 0:
                    previewNode.InitiatorChoice = true;
                    break;

                case 3:
                    var roles = assignee["roles"] as JArray;
                    if (roles != null)
                    {
                        foreach (var role in roles)
                        {
                            previewNode.RoleIds.Add(role.ToString());
                        }
                    }
                    break;

                case 4:
                    var assignees = assignee["assignees"] as JArray;
                    if (assignees != null)
                    {
                        foreach (var userId in assignees)
                        {
                            previewNode.UserIds.Add(userId.ToString());
                        }
                    }
                    break;

                case 1:
                case 2:
                    previewNode.RoleIds.Add($"dynamic_{assigneeType}");
                    break;
            }
        }

        /// <summary>
        /// 创建开始节点
        /// </summary>
        /// <returns>开始节点</returns>
        private static FlowNodePreviewDto CreateStartNode()
        {
            var timestamp = DateTime.Now.Ticks.ToString();
            return new FlowNodePreviewDto
            {
                Id = $"SE{timestamp}",
                Name = "开始",
                NodeId = $"SE{timestamp}",
                NodeType = NODE_START,
                InitiatorChoice = false
            };
        }

        /// <summary>
        /// 创建结束节点
        /// </summary>
        /// <returns>结束节点</returns>
        private static FlowNodePreviewDto CreateEndNode()
        {
            var timestamp = (DateTime.Now.Ticks + 1).ToString();
            return new FlowNodePreviewDto
            {
                Id = $"EE{timestamp}",
                Name = "结束",
                NodeId = $"EE{timestamp}",
                NodeType = NODE_END,
                InitiatorChoice = false
            };
        }

        /// <summary>
        /// 生成节点ID
        /// </summary>
        /// <param name="nodeType">节点类型</param>
        /// <returns>节点ID</returns>
        private static string GenerateNodeId(int nodeType)
        {
            var timestamp = DateTime.Now.Ticks.ToString();
            return nodeType switch
            {
                NODE_START => $"SE{timestamp}",
                NODE_APPROVE => $"UT{timestamp}",
                NODE_COPY => $"CC{timestamp}",
                NODE_TRANSACT => $"TT{timestamp}",
                NODE_END => $"EE{timestamp}",
                _ => $"ND{timestamp}"
            };
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 创建任务
        /// </summary>
        /// <param name="instanceId">流程实例ID</param>
        /// <param name="nodeId">节点ID</param>
        /// <param name="nodeName">节点名称</param>
        /// <param name="nodeType">节点类型</param>
        /// <param name="userId">用户ID</param>
        /// <param name="userName">用户名</param>
        /// <param name="multiInstanceType">多实例类型</param>
        /// <param name="sortOrder">排序顺序</param>
        /// <param name="formAuths">表单权限</param>
        private async Task CreateTaskAsync(Guid instanceId, string nodeId, string nodeName, int nodeType,
            long userId, string userName, int multiInstanceType, int sortOrder, string formAuths)
        {
            if (string.IsNullOrEmpty(nodeId))
            {
                nodeId = $"Task_Node_{DateTime.Now.Ticks}";
                Logger.Warn($"节点ID为空，生成新ID: {nodeId}");
            }

            var task = new FlowNodeTasks
            {
                FlowInstanceId = instanceId,
                NodeId = nodeId,
                NodeName = nodeName,
                NodeType = nodeType,
                AssigneeId = userId,
                AssigneeName = userName,
                Status = TASK_PENDING,
                MultiInstanceType = multiInstanceType,
                SortOrder = sortOrder,
                CreateTime = DateTime.Now,
                FormAuths = formAuths
            };

            await _taskRepository.InsertAsync(task);
            Logger.Info($"创建任务: Task节点ID={nodeId}, 节点名称={nodeName}, 用户={userName}, 用户ID={userId}");
        }

        /// <summary>
        /// 添加历史记录
        /// </summary>
        /// <param name="instanceId">流程实例ID</param>
        /// <param name="nodeId">节点ID</param>
        /// <param name="nodeName">节点名称</param>
        /// <param name="nodeType">节点类型</param>
        /// <param name="flowCmd">操作指令</param>
        /// <param name="operatorId">操作人ID</param>
        /// <param name="operatorName">操作人姓名</param>
        /// <param name="comment">操作意见</param>
        /// <param name="beforeFormData">操作前表单数据</param>
        /// <param name="afterFormData">操作后表单数据</param>
        private async Task AddHistoryAsync(Guid instanceId, string nodeId, string nodeName, int? nodeType,
            int flowCmd, long? operatorId, string operatorName, string comment,
            string beforeFormData, string afterFormData)
        {
            var history = new FlowInstanceHistories
            {
                FlowInstanceId = instanceId,
                NodeId = nodeId,
                NodeName = nodeName,
                NodeType = nodeType ?? 0,
                FlowCmd = flowCmd,
                OperatorId = operatorId,
                OperatorName = operatorName,
                OperateTime = DateTime.Now,
                Comment = comment,
                BeforeFormData = beforeFormData,
                AfterFormData = afterFormData
            };
            await _historyRepository.InsertAsync(history);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// 生成流程编号
        /// </summary>
        /// <returns>流程编号</returns>
        private static string GenerateFlowCode()
        {
            return "FL" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
        }

        /// <summary>
        /// 获取状态名称
        /// </summary>
        /// <param name="status">状态码</param>
        /// <returns>状态名称</returns>
        private static string GetStatusName(int status)
        {
            return status switch
            {
                STATUS_UNDERWAY => "审批中",
                STATUS_APPROVED => "已通过",
                STATUS_REJECTED => "不通过",
                STATUS_CANCELLED => "已撤销",
                _ => "未知"
            };
        }

        /// <summary>
        /// 获取操作指令名称
        /// </summary>
        /// <param name="cmd">操作指令</param>
        /// <returns>指令名称</returns>
        private static string GetCmdName(int cmd)
        {
            return cmd switch
            {
                CMD_START => "发起",
                CMD_AUTO_REJECTED => "自动拒绝",
                CMD_AUTO_APPROVED => "自动通过",
                CMD_REJECTED => "拒绝",
                CMD_APPROVED => "通过",
                CMD_CANCELED => "撤销",
                CMD_BACK => "回退",
                CMD_COPY => "抄送",
                _ => "操作"
            };
        }

        /// <summary>
        /// 获取节点类型名称
        /// </summary>
        /// <param name="nodeType">节点类型</param>
        /// <returns>类型名称</returns>
        private static string GetNodeTypeName(int nodeType)
        {
            return nodeType switch
            {
                NODE_START => "开始",
                NODE_APPROVE => "审批",
                NODE_COPY => "抄送",
                NODE_CONDITION => "条件",
                NODE_GATEWAY => "分支",
                NODE_TRANSACT => "办理",
                NODE_END => "结束",
                _ => "未知"
            };
        }

        /// <summary>
        /// 获取业务名称
        /// </summary>
        /// <param name="businessId">业务ID</param>
        /// <param name="businessType">业务类型</param>
        /// <returns>业务名称</returns>
        private async Task<string> GetBusinessNameAsync(Guid businessId, string businessType)
        {
            try
            {
                if (businessType == "DeviceChangeApplication")
                {
                    var apply = await _changeApplyRepository.FirstOrDefaultAsync(businessId);
                    if (apply != null)
                    {
                        var newData = DeviceJsonHelper.DeserializeDeviceEditInput(apply.NewData);
                        return $"{newData?.DeviceName ?? "设备"}({apply.ChangeType})";
                    }
                    return businessId.ToString().Substring(0, 8);
                }
                return businessId.ToString().Substring(0, 8);
            }
            catch (Exception ex)
            {
                Logger.Warn($"获取业务名称失败: BusinessId={businessId}, BusinessType={businessType}, Error={ex.Message}");
                return businessId.ToString().Substring(0, 8);
            }
        }

        /// <summary>
        /// 获取流程实例
        /// </summary>
        /// <param name="flowInstanceId">流程实例ID</param>
        /// <returns>流程实例</returns>
        public async Task<FlowInstances> GetFlowInstanceAsync(Guid flowInstanceId)
        {
            return await _flowInstanceRepository.FirstOrDefaultAsync(flowInstanceId);
        }

        /// <summary>
        /// 根据变更申请ID获取流程实例
        /// </summary>
        /// <param name="applyId">申请ID</param>
        /// <returns>流程实例</returns>
        public async Task<FlowInstances> GetFlowInstanceByApplyIdAsync(Guid applyId)
        {
            var relation = await _deviceChangeRelationRepository
                .FirstOrDefaultAsync(x => x.DeviceChangeApplicationId == applyId);

            if (relation?.FlowInstanceId != null)
            {
                return await _flowInstanceRepository.FirstOrDefaultAsync(relation.FlowInstanceId.Value);
            }

            return null;
        }

        #endregion
    }
}