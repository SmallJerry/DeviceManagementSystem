using Abp.Application.Services.Dto;
using Abp.Auditing;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Runtime.Caching;
using DeviceManagementSystem.System;
using DeviceManagementSystem.SystemConfigs.Enumeration;
using DeviceManagementSystem.SystemConfigs.param;
using DeviceManagementSystem.Users.Dto;
using DeviceManagementSystem.Utils.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeviceManagementSystem.SystemConfigs
{
    /// <summary>
    /// 系统配置服务实现
    /// </summary>
    public class SystemConfigAppService : DeviceManagementSystemAppServiceBase , ISystemConfigAppService
    {
        private readonly IRepository<SystemConfig, Guid> _systemConfigRepository;

        /// <summary>
        /// 构造函数
        /// </summary>
        public SystemConfigAppService(IRepository<SystemConfig, Guid> systemConfigRepository)
        {
            _systemConfigRepository = systemConfigRepository;
        }

        /// <summary>
        /// 获取系统基础配置列表
        /// </summary>
        /// <returns></returns>
        public async Task<CommonResult<List<SystemConfig>>> GetSystemBaseList()
        {
            try
            {
                var systemBaseList = await _systemConfigRepository.GetAll()
                    .AsNoTracking()
                    .Where(it => string.Equals(ConfigCategoryConstants.SYS_BASE, it.Category))
                    .ToListAsync();

                return CommonResult<List<SystemConfig>>.Success(systemBaseList);
            }
            catch (Exception ex)
            {
                return CommonResult<List<SystemConfig>>.Error("获取系统基础配置列表失败:" + ex.Message);
            }
        }

        /// <summary>
        ///  获取配置分页
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [DisableAuditing]
        public async Task<CommonResult<Page<SystemConfig>>> GetPageList(ConfigPageParam param)
        {
            try
            {
                var query = _systemConfigRepository.GetAll()
                    .Where(x => x.Category == ConfigCategoryConstants.BIZ_DEFINE)
                    .AsNoTracking();

                // 搜索条件
                if (!string.IsNullOrWhiteSpace(param.SearchKey))
                {
                    query = query.Where(x => x.ConfigKey.Contains(param.SearchKey));
                }

                // 排序
                if (!string.IsNullOrWhiteSpace(param.SortField) && !string.IsNullOrWhiteSpace(param.SortOrder))
                {
                    if (param.SortOrder.ToUpper() == "ASC")
                    {
                        query = query.OrderBy(x => x.SortCode);
                    }
                    else
                    {
                        query = query.OrderByDescending(x => x.SortCode);
                    }
                }
                else
                {
                    query = query.OrderBy(x => x.SortCode);
                }

                // 分页
                var total = await query.CountAsync();
                var records = await query
                    .Skip((param.Current - 1) * param.Size)
                    .Take(param.Size)
                    .Select(x => new SystemConfig
                    {
                        Id = x.Id,
                        ConfigKey = x.ConfigKey,
                        ConfigValue = x.ConfigValue,
                        Category = x.Category,
                        Remark = x.Remark,
                        SortCode = x.SortCode
                    })
                    .ToListAsync();

                // 创建分页对象
                var page = new Page<SystemConfig>(param.Current / param.Size + 1, param.Size)
                {
                    Total = total,
                    Current = param.Current,
                    Records = records
                };

                return CommonResult<Page<SystemConfig>>.Success(page);
            }
            catch (Exception ex)
            {
                return CommonResult<Page<SystemConfig>>.Error("获取配置分页失败:" + ex.Message);
            }
        }

        /// <summary>
        /// 获取配置列表
        /// </summary>
        /// <param name="category">分类</param>
        /// <returns></returns>
        public async Task<CommonResult<List<SystemConfig>>> GetList(string category)
        {
            try
            {
                var query = _systemConfigRepository.GetAll().AsNoTracking();

                if (!string.IsNullOrWhiteSpace(category))
                {
                    query = query.Where(x => x.Category == category);
                }

                var result = await query
                    .Select(x => new SystemConfig
                    {
                        Id = x.Id,
                        ConfigKey = x.ConfigKey,
                        ConfigValue = x.ConfigValue,
                        Category = x.Category,
                        SortCode = x.SortCode,
                        Remark = x.Remark
                    })
                    .OrderBy(x => x.SortCode)
                    .ToListAsync();

                return CommonResult<List<SystemConfig>>.Success(result);
            }
            catch (Exception ex)
            {
                return CommonResult<List<SystemConfig>>.Error("获取配置列表失败:" + ex.Message);
            }
        }

        /// <summary>
        /// 添加业务定义配置
        /// </summary>
        /// <param name="configAddParam"></param>
        /// <returns></returns>
        public async Task<CommonResult> Create(ConfigAddParam configAddParam)
        {
            try
            {
                // 1. 参数校验
                var checkResult = await CheckParamAsync(configAddParam);
                if (!checkResult.IsSuccess)
                {
                    return checkResult;
                }

                // 2. 参数映射到实体
                var systemConfig = new SystemConfig
                {
                    ConfigKey = configAddParam.ConfigKey,
                    ConfigValue = configAddParam.ConfigValue,
                    Category = ConfigCategoryConstants.BIZ_DEFINE, // 设置为业务配置
                    Remark = configAddParam.Remark,
                    SortCode = configAddParam.SortCode
                };

                // 3. 保存到数据库
                await _systemConfigRepository.InsertAsync(systemConfig);

                return CommonResult.Ok("创建成功");
            }
            catch (Exception ex)
            {
                return CommonResult.Error("创建配置失败:" + ex.Message);
            }
        }

        /// <summary>
        /// 参数校验方法
        /// </summary>
        private async Task<CommonResult> CheckParamAsync(ConfigAddParam configAddParam)
        {
            // 基础参数校验
            if (configAddParam == null)
            {
                return CommonResult.Error("参数不能为空");
            }

            if (string.IsNullOrWhiteSpace(configAddParam.ConfigKey))
            {
                return CommonResult.Error("配置键不能为空");
            }

            if (configAddParam.ConfigValue == null)
            {
                return CommonResult.Error("配置值不能为空");
            }

            // 检查配置键是否已存在
            bool hasSameConfig = await _systemConfigRepository.GetAll()
                .AnyAsync(it => it.ConfigKey == configAddParam.ConfigKey && it.Category == ConfigCategoryConstants.SYS_BASE);

            if (hasSameConfig)
            {
                return CommonResult.Error($"存在重复的配置，配置键为：{configAddParam.ConfigKey}");
            }

            return CommonResult.Ok("校验通过");
        }

        /// <summary>
        /// 编辑配置
        /// </summary>
        public async Task<CommonResult> UpdateById(ConfigEditParam param)
        {
            try
            {
                // 查询配置实体
                var config = await GetEntityAsync(param.Id);
                if (config == null)
                {
                    return CommonResult.Error("配置不存在");
                }

                // 参数校验
                var checkResult = await CheckEditParamAsync(param, config);
                if (!checkResult.IsSuccess)
                {
                    return checkResult;
                }

                // 更新配置
                config.ConfigKey = param.ConfigKey;
                config.ConfigValue = param.ConfigValue;
                config.Remark = param.Remark;
                config.SortCode = param.SortCode;
                config.ExtJson = param.ExtJson;
                config.Category = ConfigCategoryConstants.BIZ_DEFINE;

                await _systemConfigRepository.UpdateAsync(config);
                await CurrentUnitOfWork.SaveChangesAsync();

                return CommonResult.Ok("编辑成功");
            }
            catch (Exception ex)
            {
                return CommonResult.Error("编辑配置失败:" + ex.Message);
            }
        }

        /// <summary>
        /// 删除配置
        /// </summary>
        public async Task<CommonResult> DeleteByIds([FromBody]List<ConfigIdParam> paramList)
        {
            try
            {
                if (paramList == null || !paramList.Any())
                {
                    return CommonResult.Error("集合不能为空");
                }

                var ids = paramList.Select(x => x.Id).ToList();
                var configs = await _systemConfigRepository.GetAll()
                    .Where(x => ids.Contains(x.Id))
                    .ToListAsync();

                if (!configs.Any())
                {
                    return CommonResult.Error("配置不存在");
                }

                // 检查是否包含系统内置配置
                var nonBizConfigs = configs.Where(x => x.Category != ConfigCategoryConstants.BIZ_DEFINE).ToList();
                if (nonBizConfigs.Any())
                {
                    return CommonResult.Error("不可删除系统内置配置");
                }

                // 删除配置
                foreach (var config in configs)
                {
                    await _systemConfigRepository.DeleteAsync(config);
                }

                await CurrentUnitOfWork.SaveChangesAsync();

                return CommonResult.Ok("删除成功");
            }
            catch (Exception ex)
            {
                return CommonResult.Error("删除配置失败:" + ex.Message);
            }
        }

        /// <summary>
        /// 获取配置详情
        /// </summary>
        public async Task<CommonResult<SystemConfig>> GetById(ConfigIdParam param)
        {
            try
            {
                var config = await GetEntityAsync(param.Id);
                if (config == null)
                {
                    return CommonResult<SystemConfig>.Error("配置不存在");
                }

                return CommonResult<SystemConfig>.Success(config);
            }
            catch (Exception ex)
            {
                return CommonResult<SystemConfig>.Error("获取配置详情失败:" + ex.Message);
            }
        }


        /// <summary>
        /// 根据配置键获取基础配置
        /// </summary>
        /// <param name="configKey"></param>
        /// <returns></returns>
        public async Task<CommonResult<SystemConfig>> GetBaseConfigByConfigKey(string configKey)
        {
            try
            {
                var config = await _systemConfigRepository.FirstOrDefaultAsync(it => string.Equals(it.Category, ConfigCategoryConstants.SYS_BASE) && string.Equals(it.ConfigKey,configKey)); ;
                if (config == null)
                {
                    return CommonResult<SystemConfig>.Error("配置不存在");
                }

                return CommonResult<SystemConfig>.Success(config);
            }
            catch (Exception ex)
            {
                return CommonResult<SystemConfig>.Error("获取配置详情失败:" + ex.Message);
            }
        }



        /// <summary>
        /// 批量更新配置
        /// </summary>
        public async Task<CommonResult> EditBatch(List<ConfigBatchParam> paramList)
        {
            try
            {
                if (paramList == null || !paramList.Any())
                {
                    return CommonResult.Error("集合不能为空");
                }

                foreach (var param in paramList)
                {
                    var configs = await _systemConfigRepository.GetAll()
                        .Where(x => x.ConfigKey == param.ConfigKey)
                        .ToListAsync();

                    foreach (var config in configs)
                    {
                        config.ConfigValue = param.ConfigValue;
                        await _systemConfigRepository.UpdateAsync(config);
                    }
                }

                await CurrentUnitOfWork.SaveChangesAsync();

                return CommonResult.Ok("批量更新成功");
            }
            catch (Exception ex)
            {
                return CommonResult.Error("批量更新配置失败:" + ex.Message);
            }
        }

        /// <summary>
        /// 编辑参数校验
        /// </summary>
        private async Task<CommonResult> CheckEditParamAsync(ConfigEditParam param, SystemConfig existingConfig)
        {
            if (param == null)
            {
                return CommonResult.Error("参数不能为空");
            }

            if (string.IsNullOrWhiteSpace(param.ConfigKey))
            {
                return CommonResult.Error("配置键不能为空");
            }

            if (string.IsNullOrWhiteSpace(param.ConfigValue))
            {
                return CommonResult.Error("配置值不能为空");
            }

            // 检查配置键是否重复（排除自身）
            bool hasSameConfig = await _systemConfigRepository.GetAll()
                .AnyAsync(x => x.ConfigKey == param.ConfigKey && x.Id != param.Id);

            if (hasSameConfig)
            {
                return CommonResult.Error($"存在重复的配置，配置键为：{param.ConfigKey}");
            }

            return CommonResult.Ok("校验通过");
        }

        /// <summary>
        /// 获取配置实体
        /// </summary>
        private async Task<SystemConfig> GetEntityAsync(Guid id)
        {
            return await _systemConfigRepository.FirstOrDefaultAsync(id);
        }
}
}
