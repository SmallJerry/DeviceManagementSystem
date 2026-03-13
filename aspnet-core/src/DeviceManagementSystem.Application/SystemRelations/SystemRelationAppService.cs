// SystemRelationAppService.cs
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore.Repositories;
using DeviceManagementSystem.Authorization.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeviceManagementSystem.SystemRelations
{
    /// <summary>
    /// 关系服务实现类
    /// 提供系统实体间多对多关系的增删改查功能
    /// 注意：ABP框架默认会对应用服务层的方法自动开启事务
    /// </summary>
    public class SystemRelationAppService : DeviceManagementSystemAppServiceBase,ISystemRelationAppService
    {
        private readonly IRepository<SystemRelation, Guid> _systemRelationRepository;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="systemRelationRepository">关系仓储</param>
        public SystemRelationAppService(IRepository<SystemRelation, Guid> systemRelationRepository)
        {
            _systemRelationRepository = systemRelationRepository;
        }

        #region 核心保存方法

        /// <summary>
        /// 保存关系（单个）- 核心方法
        /// </summary>
        [HttpPost]
        [Route("SaveRelation")]
        public async Task SaveRelationAsync(string objectId, string target, string category, string extJson, bool clear)
        {
            // 如果需要先清除该对象同分类下的所有关系
            if (clear)
            {
                await _systemRelationRepository.DeleteAsync(x =>
                    x.ObjectId == objectId && x.Category == category);
            }

            // 创建新的关系实体
            var systemRelation = new SystemRelation
            {
                ObjectId = objectId,
                Target = target,
                Category = category,
                ExtJson = extJson
            };

            // 保存到数据库
            await _systemRelationRepository.InsertAsync(systemRelation);
        }

        /// <summary>
        /// 保存关系（批量）- 核心方法
        /// </summary>
        [HttpPost]
        [Route("SaveRelationBatch")]
        public async Task SaveRelationBatchAsync(string objectId, List<string> targetList, string category, List<string> extJsonList, bool clear)
        {
            // 如果需要先清除该对象同分类下的所有关系
            if (clear)
            {
                await _systemRelationRepository.DeleteAsync(x =>
                    x.ObjectId == objectId && x.Category == category);
            }

            // 创建关系实体列表
            var systemRelations = new List<SystemRelation>();
            for (int i = 0; i < targetList.Count; i++)
            {
                var systemRelation = new SystemRelation
                {
                    ObjectId = objectId,
                    Target = targetList[i],
                    Category = category,
                    // 如果提供了扩展信息列表且当前索引有效，则设置扩展信息
                    ExtJson = extJsonList != null && i < extJsonList.Count ? extJsonList[i] : null
                };
                systemRelations.Add(systemRelation);
            }

            // 批量保存到数据库
            if (systemRelations.Any())
            {
                await _systemRelationRepository.InsertRangeAsync(systemRelations);
            }
        }

        #endregion

        #region 追加保存方法（不清除已有关系）

        /// <summary>
        /// 追加保存关系（单个，无扩展信息）
        /// </summary>
        [HttpPost]
        [Route("AppendRelation")]
        public async Task SaveRelationWithAppendAsync(string objectId, string target, string category)
        {
            await SaveRelationAsync(objectId, target, category, null, false);
        }

        /// <summary>
        /// 追加保存关系（单个，有扩展信息）
        /// </summary>
        [HttpPost]
        [Route("AppendRelationWithExt")]
        public async Task SaveRelationWithAppendAsync(string objectId, string target, string category, string extJson)
        {
            await SaveRelationAsync(objectId, target, category, extJson, false);
        }

        /// <summary>
        /// 追加保存关系（批量，无扩展信息）
        /// </summary>
        [HttpPost]
        [Route("AppendRelationBatch")]
        public async Task SaveRelationBatchWithAppendAsync(string objectId, List<string> targetList, string category)
        {
            await SaveRelationBatchAsync(objectId, targetList, category, null, false);
        }

        /// <summary>
        /// 追加保存关系（批量，有扩展信息）
        /// </summary>
        [HttpPost]
        [Route("AppendRelationBatchWithExt")]
        public async Task SaveRelationBatchWithAppendAsync(string objectId, List<string> targetList, string category, List<string> extJsonList)
        {
            await SaveRelationBatchAsync(objectId, targetList, category, extJsonList, false);
        }

        #endregion

        #region 清理并保存方法（先清除后保存）

        /// <summary>
        /// 清理并保存关系（单个，无扩展信息）
        /// </summary>
        [HttpPost]
        [Route("ClearAndSaveRelation")]
        public async Task SaveRelationWithClearAsync(string objectId, string target, string category)
        {
            await SaveRelationAsync(objectId, target, category, null, true);
        }

        /// <summary>
        /// 清理并保存关系（单个，有扩展信息）
        /// </summary>
        [HttpPost]
        [Route("ClearAndSaveRelationWithExt")]
        public async Task SaveRelationWithClearAsync(string objectId, string target, string category, string extJson)
        {
            await SaveRelationAsync(objectId, target, category, extJson, true);
        }

        /// <summary>
        /// 清理并保存关系（批量，无扩展信息）
        /// </summary>
        [HttpPost]
        [Route("ClearAndSaveRelationBatch")]
        public async Task SaveRelationBatchWithClearAsync(string objectId, List<string> targetList, string category)
        {
            await SaveRelationBatchAsync(objectId, targetList, category, null, true);
        }

        /// <summary>
        /// 清理并保存关系（批量，有扩展信息）
        /// </summary>
        [HttpPost]
        [Route("ClearAndSaveRelationBatchWithExt")]
        public async Task SaveRelationBatchWithClearAsync(string objectId, List<string> targetList, string category, List<string> extJsonList)
        {
            await SaveRelationBatchAsync(objectId, targetList, category, extJsonList, true);
        }

        #endregion

        #region 根据对象ID查询关系

        /// <summary>
        /// 根据对象ID获取所有关系
        /// </summary>
        [HttpGet]
        [Route("Relations/ByObject/{objectId}")]
        public async Task<List<SystemRelation>> GetRelationListByObjectIdAsync(string objectId)
        {
            return await GetRelationListByObjectIdAndCategoryAsync(objectId, null);
        }

        /// <summary>
        /// 根据对象ID列表获取所有关系
        /// </summary>
        [HttpPost]
        [Route("Relations/ByObjectList")]
        public async Task<List<SystemRelation>> GetRelationListByObjectIdListAsync(List<string> objectIdList)
        {
            return await GetRelationListByObjectIdListAndCategoryAsync(objectIdList, null);
        }

        /// <summary>
        /// 根据对象ID和分类获取关系
        /// </summary>
        [HttpGet]
        [Route("Relations/ByObject/{objectId}/Category/{category}")]
        public async Task<List<SystemRelation>> GetRelationListByObjectIdAndCategoryAsync(string objectId, string category)
        {
            var query = _systemRelationRepository.GetAll()
                .Where(x => x.ObjectId == objectId);

            // 如果指定了分类，则按分类过滤
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(x => x.Category == category);
            }

            return await query.ToListAsync();
        }

        /// <summary>
        /// 根据对象ID列表和分类获取关系
        /// </summary>
        [HttpPost]
        [Route("Relations/ByObjectList/Category/{category}")]
        public async Task<List<SystemRelation>> GetRelationListByObjectIdListAndCategoryAsync(List<string> objectIdList, string category)
        {
            var query = _systemRelationRepository.GetAll()
                .Where(x => objectIdList.Contains(x.ObjectId));

            // 如果指定了分类，则按分类过滤
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(x => x.Category == category);
            }

            return await query.ToListAsync();
        }

        #endregion

        #region 根据目标标识查询关系

        /// <summary>
        /// 根据目标标识获取所有关系
        /// </summary>
        [HttpGet]
        [Route("Relations/ByTarget/{target}")]
        public async Task<List<SystemRelation>> GetRelationListByTargetAsync(string target)
        {
            return await GetRelationListByTargetAndCategoryAsync(target, null);
        }

        /// <summary>
        /// 根据目标标识列表获取所有关系
        /// </summary>
        [HttpPost]
        [Route("Relations/ByTargetList")]
        public async Task<List<SystemRelation>> GetRelationListByTargetListAsync(List<string> targetList)
        {
            return await GetRelationListByTargetListAndCategoryAsync(targetList, null);
        }

        /// <summary>
        /// 根据目标标识和分类获取关系
        /// </summary>
        [HttpGet]
        [Route("Relations/ByTarget/{target}/Category/{category}")]
        public async Task<List<SystemRelation>> GetRelationListByTargetAndCategoryAsync(string target, string category)
        {
            var query = _systemRelationRepository.GetAll()
                .Where(x => x.Target == target);

            // 如果指定了分类，则按分类过滤
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(x => x.Category == category);
            }

            return await query.ToListAsync();
        }

        /// <summary>
        /// 根据目标标识列表和分类获取关系
        /// </summary>
        [HttpPost]
        [Route("Relations/ByTargetList/Category/{category}")]
        public async Task<List<SystemRelation>> GetRelationListByTargetListAndCategoryAsync(List<string> targetList, string category)
        {
            var query = _systemRelationRepository.GetAll()
                .Where(x => targetList.Contains(x.Target));

            // 如果指定了分类，则按分类过滤
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(x => x.Category == category);
            }

            return await query.ToListAsync();
        }

        #endregion

        #region 获取目标标识列表

        /// <summary>
        /// 根据对象ID获取所有目标标识
        /// </summary>
        [HttpGet]
        [Route("Targets/ByObject/{objectId}")]
        public async Task<List<string>> GetRelationTargetListByObjectIdAsync(string objectId)
        {
            return await GetRelationTargetListByObjectIdAndCategoryAsync(objectId, null);
        }

        /// <summary>
        /// 根据对象ID列表获取所有目标标识
        /// </summary>
        [HttpPost]
        [Route("Targets/ByObjectList")]
        public async Task<List<string>> GetRelationTargetListByObjectIdListAsync(List<string> objectIdList)
        {
            return await GetRelationTargetListByObjectIdListAndCategoryAsync(objectIdList, null);
        }

        /// <summary>
        /// 根据对象ID和分类获取目标标识
        /// </summary>
        [HttpGet]
        [Route("Targets/ByObject/{objectId}/Category/{category}")]
        public async Task<List<string>> GetRelationTargetListByObjectIdAndCategoryAsync(string objectId, string category)
        {
            var relations = await GetRelationListByObjectIdAndCategoryAsync(objectId, category);
            return relations.Select(x => x.Target).ToList();
        }

        /// <summary>
        /// 根据对象ID列表和分类获取目标标识
        /// </summary>
        [HttpPost]
        [Route("Targets/ByObjectList/Category/{category}")]
        public async Task<List<string>> GetRelationTargetListByObjectIdListAndCategoryAsync(List<string> objectIdList, string category)
        {
            var relations = await GetRelationListByObjectIdListAndCategoryAsync(objectIdList, category);
            return relations.Select(x => x.Target).ToList();
        }

        #endregion

        #region 获取对象ID列表

        /// <summary>
        /// 根据目标标识获取所有对象ID
        /// </summary>
        [HttpGet]
        [Route("Objects/ByTarget/{target}")]
        public async Task<List<string>> GetRelationObjectIdListByTargetAsync(string target)
        {
            return await GetRelationObjectIdListByTargetAndCategoryAsync(target, null);
        }

        /// <summary>
        /// 根据目标标识列表获取所有对象ID
        /// </summary>
        [HttpPost]
        [Route("Objects/ByTargetList")]
        public async Task<List<string>> GetRelationObjectIdListByTargetListAsync(List<string> targetList)
        {
            return await GetRelationObjectIdListByTargetListAndCategoryAsync(targetList, null);
        }

        /// <summary>
        /// 根据目标标识和分类获取对象ID
        /// </summary>
        [HttpGet]
        [Route("Objects/ByTarget/{target}/Category/{category}")]
        public async Task<List<string>> GetRelationObjectIdListByTargetAndCategoryAsync(string target, string category)
        {
            var relations = await GetRelationListByTargetAndCategoryAsync(target, category);
            // 使用Distinct()确保返回的ObjectId不重复
            return relations.Select(x => x.ObjectId).Distinct().ToList();
        }

        /// <summary>
        /// 根据目标标识列表和分类获取对象ID
        /// </summary>
        [HttpPost]
        [Route("Objects/ByTargetList/Category/{category}")]
        public async Task<List<string>> GetRelationObjectIdListByTargetListAndCategoryAsync(List<string> targetList, string category)
        {
            var relations = await GetRelationListByTargetListAndCategoryAsync(targetList, category);
            // 使用Distinct()确保返回的ObjectId不重复
            return relations.Select(x => x.ObjectId).Distinct().ToList();
        }

        #endregion
    }
}