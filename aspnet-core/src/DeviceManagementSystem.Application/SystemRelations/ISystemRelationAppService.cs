// ISystemRelationAppService.cs
using DeviceManagementSystem.Authorization.Resources;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeviceManagementSystem.SystemRelations
{
    /// <summary>
    /// 关系服务接口
    /// 用于管理系统实体间的多对多关系
    /// </summary>
    public interface ISystemRelationAppService
    {
        #region 保存关系方法

        /// <summary>
        /// 保存关系（单个）
        /// </summary>
        /// <param name="objectId">对象ID</param>
        /// <param name="target">目标标识（可能是Guid的字符串形式或其他标识符）</param>
        /// <param name="category">关系分类（使用SystemRelationCategoryConst中定义的常量）</param>
        /// <param name="extJson">扩展信息（JSON格式）</param>
        /// <param name="clear">是否先清除该对象同分类下的所有关系</param>
        Task SaveRelationAsync(string objectId, string target, string category, string extJson, bool clear);

        /// <summary>
        /// 保存关系（批量）
        /// </summary>
        /// <param name="objectId">对象ID</param>
        /// <param name="targetList">目标标识列表</param>
        /// <param name="category">关系分类</param>
        /// <param name="extJsonList">扩展信息列表（与targetList一一对应）</param>
        /// <param name="clear">是否先清除该对象同分类下的所有关系</param>
        Task SaveRelationBatchAsync(string objectId, List<string> targetList, string category, List<string> extJsonList, bool clear);

        /// <summary>
        /// 追加保存关系（单个，无扩展信息）
        /// 在现有关系基础上追加，不清除已有关系
        /// </summary>
        Task SaveRelationWithAppendAsync(string objectId, string target, string category);

        /// <summary>
        /// 追加保存关系（单个，有扩展信息）
        /// 在现有关系基础上追加，不清除已有关系
        /// </summary>
        Task SaveRelationWithAppendAsync(string objectId, string target, string category, string extJson);

        /// <summary>
        /// 追加保存关系（批量，无扩展信息）
        /// 在现有关系基础上追加，不清除已有关系
        /// </summary>
        Task SaveRelationBatchWithAppendAsync(string objectId, List<string> targetList, string category);

        /// <summary>
        /// 追加保存关系（批量，有扩展信息）
        /// 在现有关系基础上追加，不清除已有关系
        /// </summary>
        Task SaveRelationBatchWithAppendAsync(string objectId, List<string> targetList, string category, List<string> extJsonList);

        /// <summary>
        /// 清理并保存关系（单个，无扩展信息）
        /// 先清除该对象同分类下的所有关系，再保存新关系
        /// </summary>
        Task SaveRelationWithClearAsync(string objectId, string target, string category);

        /// <summary>
        /// 清理并保存关系（单个，有扩展信息）
        /// 先清除该对象同分类下的所有关系，再保存新关系
        /// </summary>
        Task SaveRelationWithClearAsync(string objectId, string target, string category, string extJson);

        /// <summary>
        /// 清理并保存关系（批量，无扩展信息）
        /// 先清除该对象同分类下的所有关系，再保存新关系
        /// </summary>
        Task SaveRelationBatchWithClearAsync(string objectId, List<string> targetList, string category);

        /// <summary>
        /// 清理并保存关系（批量，有扩展信息）
        /// 先清除该对象同分类下的所有关系，再保存新关系
        /// </summary>
        Task SaveRelationBatchWithClearAsync(string objectId, List<string> targetList, string category, List<string> extJsonList);

        #endregion

        #region 根据对象ID查询关系

        /// <summary>
        /// 根据对象ID获取所有关系
        /// </summary>
        /// <param name="objectId">对象ID</param>
        /// <returns>关系实体列表</returns>
        Task<List<SystemRelation>> GetRelationListByObjectIdAsync(string objectId);

        /// <summary>
        /// 根据对象ID列表获取所有关系
        /// </summary>
        /// <param name="objectIdList">对象ID列表</param>
        /// <returns>关系实体列表</returns>
        Task<List<SystemRelation>> GetRelationListByObjectIdListAsync(List<string> objectIdList);

        /// <summary>
        /// 根据对象ID和分类获取关系
        /// </summary>
        /// <param name="objectId">对象ID</param>
        /// <param name="category">关系分类（可选，为null时查询所有分类）</param>
        /// <returns>关系实体列表</returns>
        Task<List<SystemRelation>> GetRelationListByObjectIdAndCategoryAsync(string objectId, string category);

        /// <summary>
        /// 根据对象ID列表和分类获取关系
        /// </summary>
        /// <param name="objectIdList">对象ID列表</param>
        /// <param name="category">关系分类（可选，为null时查询所有分类）</param>
        /// <returns>关系实体列表</returns>
        Task<List<SystemRelation>> GetRelationListByObjectIdListAndCategoryAsync(List<string> objectIdList, string category);

        #endregion

        #region 根据目标标识查询关系

        /// <summary>
        /// 根据目标标识获取所有关系
        /// </summary>
        /// <param name="target">目标标识</param>
        /// <returns>关系实体列表</returns>
        Task<List<SystemRelation>> GetRelationListByTargetAsync(string target);

        /// <summary>
        /// 根据目标标识列表获取所有关系
        /// </summary>
        /// <param name="targetList">目标标识列表</param>
        /// <returns>关系实体列表</returns>
        Task<List<SystemRelation>> GetRelationListByTargetListAsync(List<string> targetList);

        /// <summary>
        /// 根据目标标识和分类获取关系
        /// </summary>
        /// <param name="target">目标标识</param>
        /// <param name="category">关系分类（可选，为null时查询所有分类）</param>
        /// <returns>关系实体列表</returns>
        Task<List<SystemRelation>> GetRelationListByTargetAndCategoryAsync(string target, string category);

        /// <summary>
        /// 根据目标标识列表和分类获取关系
        /// </summary>
        /// <param name="targetList">目标标识列表</param>
        /// <param name="category">关系分类（可选，为null时查询所有分类）</param>
        /// <returns>关系实体列表</returns>
        Task<List<SystemRelation>> GetRelationListByTargetListAndCategoryAsync(List<string> targetList, string category);

        #endregion

        #region 获取目标标识列表

        /// <summary>
        /// 根据对象ID获取所有目标标识
        /// </summary>
        /// <param name="objectId">对象ID</param>
        /// <returns>目标标识列表</returns>
        Task<List<string>> GetRelationTargetListByObjectIdAsync(string objectId);

        /// <summary>
        /// 根据对象ID列表获取所有目标标识
        /// </summary>
        /// <param name="objectIdList">对象ID列表</param>
        /// <returns>目标标识列表</returns>
        Task<List<string>> GetRelationTargetListByObjectIdListAsync(List<string> objectIdList);

        /// <summary>
        /// 根据对象ID和分类获取目标标识
        /// </summary>
        /// <param name="objectId">对象ID</param>
        /// <param name="category">关系分类（可选，为null时查询所有分类）</param>
        /// <returns>目标标识列表</returns>
        Task<List<string>> GetRelationTargetListByObjectIdAndCategoryAsync(string objectId, string category);

        /// <summary>
        /// 根据对象ID列表和分类获取目标标识
        /// </summary>
        /// <param name="objectIdList">对象ID列表</param>
        /// <param name="category">关系分类（可选，为null时查询所有分类）</param>
        /// <returns>目标标识列表</returns>
        Task<List<string>> GetRelationTargetListByObjectIdListAndCategoryAsync(List<string> objectIdList, string category);

        #endregion

        #region 获取对象ID列表

        /// <summary>
        /// 根据目标标识获取所有对象ID
        /// </summary>
        /// <param name="target">目标标识</param>
        /// <returns>对象ID列表（去重）</returns>
        Task<List<string>> GetRelationObjectIdListByTargetAsync(string target);

        /// <summary>
        /// 根据目标标识列表获取所有对象ID
        /// </summary>
        /// <param name="targetList">目标标识列表</param>
        /// <returns>对象ID列表（去重）</returns>
        Task<List<string>> GetRelationObjectIdListByTargetListAsync(List<string> targetList);

        /// <summary>
        /// 根据目标标识和分类获取对象ID
        /// </summary>
        /// <param name="target">目标标识</param>
        /// <param name="category">关系分类（可选，为null时查询所有分类）</param>
        /// <returns>对象ID列表（去重）</returns>
        Task<List<string>> GetRelationObjectIdListByTargetAndCategoryAsync(string target, string category);

        /// <summary>
        /// 根据目标标识列表和分类获取对象ID
        /// </summary>
        /// <param name="targetList">目标标识列表</param>
        /// <param name="category">关系分类（可选，为null时查询所有分类）</param>
        /// <returns>对象ID列表（去重）</returns>
        Task<List<string>> GetRelationObjectIdListByTargetListAndCategoryAsync(List<string> targetList, string category);

        #endregion
    }
}