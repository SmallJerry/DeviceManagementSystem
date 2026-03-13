using Abp.Auditing;
using Abp.Domain.Repositories;
using DeviceManagementSystem.Dicts.param;
using DeviceManagementSystem.System;
using DeviceManagementSystem.Utils.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Dicts
{
    /// <summary>
    /// 字典服务类
    /// </summary>
    [AllowAnonymous]
    [Audited]
    public class DictAppService : DeviceManagementSystemAppServiceBase
    {

        private readonly IRepository<Dict, Guid> _dictRepository;


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dictRepository"></param>
        public DictAppService(IRepository<Dict, Guid> dictRepository)
        {
            _dictRepository = dictRepository;
        }





        /// <summary>
        /// 获取字典分页
        /// </summary>
        /// <param name="param">分页参数</param>
        /// <returns>分页结果</returns>
        [DisableAuditing]
        public async Task<CommonResult<Page<Dict>>> GetPageList(DictPageParam param)
        {
            var query = _dictRepository.GetAll()
                .Select(d => new Dict
                {
                    Id = d.Id,
                    ParentId = d.ParentId,
                    Category = d.Category,
                    DictLabel = d.DictLabel,
                    DictValue = d.DictValue,
                    SortCode = d.SortCode
                });


            var page = new Page<Dict>(param.Current / param.Size + 1, param.Size);

            // 父ID查询
            if (param.ParentId.HasValue && param.ParentId != Guid.Empty)
            {
                query = query.Where(d => d.ParentId == param.ParentId.Value || d.Id == param.ParentId.Value);
            }

            // 分类查询
            if (!string.IsNullOrEmpty(param.Category))
            {
                query = query.Where(d => d.Category == param.Category);
            }

            // 搜索关键词查询
            if (!string.IsNullOrEmpty(param.SearchKey))
            {
                query = query.Where(d => d.DictLabel.Contains(param.SearchKey));
            }

            // 排序
            if (!string.IsNullOrEmpty(param.SortField) && !string.IsNullOrEmpty(param.SortOrder))
            {
                var sortOrder = param.SortOrder.ToUpper();
                if (sortOrder == "ASC" || sortOrder == "DESC")
                {
                    if (param.SortField == nameof(Dict.SortCode))
                    {
                        query = sortOrder == "ASC"
                            ? query.OrderBy(d => d.SortCode)
                            : query.OrderByDescending(d => d.SortCode);
                    }
                    else if (param.SortField == nameof(Dict.DictLabel))
                    {
                        query = sortOrder == "ASC"
                            ? query.OrderBy(d => d.DictLabel)
                            : query.OrderByDescending(d => d.DictLabel);
                    }
                }
            }
            else
            {
                query = query.OrderBy(d => d.SortCode);
            }

            page.Total = await query.CountAsync();
            page.Records = await query
                .Skip((param.Current - 1) * param.Size)
                .Take(param.Size)
                .ToListAsync();
            page.Current = param.Current;
            page.Size = param.Size;

            return CommonResult.Ok(page);
        }





        /// <summary>
        /// 获取字典列表  
        /// </summary>
        /// <param name="param">查询参数</param>
        /// <returns>字典列表</returns>
        [DisableAuditing]
        public async Task<CommonResult<List<Dict>>> GetListAsync(DictListParam param)
        {
            try
            {
                var query = _dictRepository.GetAll();

                if (param.ParentId.HasValue)
                {
                    query = query.Where(d => d.ParentId == param.ParentId.Value);
                }

                if (!string.IsNullOrEmpty(param.Category))
                {
                    query = query.Where(d => d.Category == param.Category);
                }

                var result = await query.OrderBy(d => d.SortCode).ToListAsync();
                return CommonResult<List<Dict>>.Success(result);
            }
            catch (Exception ex)
            {
                return CommonResult<List<Dict>>.Error("获取字典列表失败:" + ex.Message);
            }
        }


        /// <summary>
        /// 获取字典树
        /// </summary>
        /// <param name="param">查询参数</param>
        /// <returns>字典树</returns>
        [DisableAuditing]
        public async Task<CommonResult<List<DictTreeNode>>> GetTreeListAsync(DictTreeParam param)
        {
            try
            {
                var query = _dictRepository.GetAll()
                    .Select(d => new DictTreeNode
                    {
                        Id = d.Id,
                        ParentId = d.ParentId,
                        Name = d.DictLabel,
                        Category = d.Category,
                        DictLabel = d.DictLabel,
                        DictValue = d.DictValue,
                        SortCode = d.SortCode,
                        ExtJson = d.ExtJson
                    });

                if (!string.IsNullOrEmpty(param.Category))
                {
                    query = query.Where(d => d.Category == param.Category);
                }

                var dicts = await query.OrderBy(d => d.SortCode).ToListAsync();
                var tree = BuildTree(dicts, Guid.Empty);

                return CommonResult<List<DictTreeNode>>.Success(tree);
            }
            catch (Exception ex)
            {
                return CommonResult<List<DictTreeNode>>.Error("获取字典树失败:" + ex.Message);
            }
        }


        /// <summary>
        /// 添加字典
        /// </summary>
        /// <param name="param">添加参数</param>
        public async Task<CommonResult> Create(DictAddParam param)
        {
            try
            {
                await CheckAddParamAsync(param);

                var dict = new Dict
                {
                    ParentId = param.ParentId,
                    DictLabel = param.DictLabel,
                    DictValue = param.DictValue,
                    Category = param.Category,
                    SortCode = param.SortCode,
                    ExtJson = param.ExtJson,
                };

                await _dictRepository.InsertAsync(dict);
                return CommonResult.Ok("添加字典成功");
            }
            catch (Exception ex)
            {
                return CommonResult.Error(ex.Message);
            }
        }



        /// <summary>
        /// 编辑字典
        /// </summary>
        /// <param name="param">编辑参数</param>
        public async Task<CommonResult> UpdateById(DictEditParam param)
        {
            try
            {
                var dict = await _dictRepository.GetAsync(param.Id);
                if (dict == null)
                {
                    return CommonResult.Error($"字典不存在，ID为：{param.Id}");
                }

                await CheckEditParamAsync(param);

                dict.ParentId = param.ParentId;
                dict.DictLabel = param.DictLabel;
                dict.DictValue = param.DictValue;
                dict.Category = param.Category;
                dict.SortCode = param.SortCode;
                dict.ExtJson = param.ExtJson;

                await _dictRepository.UpdateAsync(dict);
                return CommonResult.Ok("编辑字典成功");
            }
            catch (Exception ex)
            {
                return CommonResult.Error(ex.Message);
            }
        }





        /// <summary>
        /// 删除字典
        /// </summary>
        /// <param name="dictIdParams">字典ID列表</param>
        public async Task<CommonResult> DeleteByIds([FromBody] List<DictIdParam> dictIdParams)
        {
            try
            {
                if (dictIdParams == null || !dictIdParams.Any())
                {
                    return CommonResult.Error("集合不能为空");
                }

                var ids = dictIdParams.Select(x => x.Id).ToList();
                var dicts = await _dictRepository.GetAll()
                    .Where(x => ids.Contains(x.Id))
                    .ToListAsync();

                if (!dicts.Any())
                {
                    return CommonResult.Error("配置不存在");
                }

                if (dicts.Any(d => d.Category == "FRM"))
                {
                    return CommonResult.Error("不可删除系统内置字典");
                }

                // 检查是否有子节点
                var hasChildren = await _dictRepository.GetAll()
                    .AnyAsync(d => ids.Contains((Guid)d.ParentId) || d.ParentId != Guid.Empty);

                if (hasChildren)
                {
                    return CommonResult.Error("存在子节点，不能删除");
                }

                await _dictRepository.DeleteAsync(d => ids.Contains(d.Id));
                return CommonResult.Ok("删除字典成功");
            }
            catch (Exception ex)
            {
                return CommonResult.Error("删除字典失败:" + ex.Message);
            }
        }

        /// <summary>
        /// 获取字典详情
        /// </summary>
        /// <param name="id">字典ID</param>
        /// <returns>字典详情</returns>
        [DisableAuditing]
        public async Task<CommonResult<Dict>> GetById(Guid id)
        {
            try
            {
                var dict = await _dictRepository.GetAsync(id);
                if (dict == null)
                {
                    return CommonResult<Dict>.Error($"字典不存在，ID为：{id}");
                }
                return CommonResult<Dict>.Success(dict);
            }
            catch (Exception ex)
            {
                return CommonResult<Dict>.Error("获取字典详情失败:" + ex.Message);
            }
        }

        /// <summary>
        /// 根据字典类型和值获取字典标签
        /// </summary>
        /// <param name="typeCode">字典类型编码</param>
        /// <param name="value">字典值</param>
        /// <returns>字典标签</returns>
        [DisableAuditing]
        public async Task<CommonResult<string>> GetDictLabelAsync(string typeCode, string value)
        {
            try
            {
                var parentDict = await _dictRepository.GetAll()
                    .FirstOrDefaultAsync(d => d.DictValue == typeCode);

                if (parentDict != null)
                {
                    var dict = await _dictRepository.GetAll()
                        .FirstOrDefaultAsync(d => d.ParentId == parentDict.Id && d.DictValue == value);

                    if (dict != null)
                    {
                        return CommonResult<string>.Success(dict.DictLabel);
                    }
                }

                return CommonResult<string>.Success(null);
            }
            catch (Exception ex)
            {
                return CommonResult<string>.Error("获取字典标签失败:" + ex.Message);
            }
        }





        /// <summary>
        /// 检查添加参数
        /// </summary>
        private async Task CheckAddParamAsync(DictAddParam param)
        {
            // 检查字典文字是否重复
            var hasSameDictLabel = await _dictRepository.GetAll()
                .AnyAsync(d => d.ParentId == param.ParentId
                    && d.Category == param.Category
                    && d.DictLabel == param.DictLabel);

            if (hasSameDictLabel)
            {
                throw new Exception($"存在重复的字典文字，名称为：{param.DictLabel}");
            }

            // 检查字典值是否重复
            var hasSameDictValue = await _dictRepository.GetAll()
                .AnyAsync(d => d.ParentId == param.ParentId && d.DictValue == param.DictValue);

            if (hasSameDictValue)
            {
                throw new Exception($"存在重复的字典值，名称为：{param.DictValue}");
            }
        }

        /// <summary>
        /// 检查编辑参数
        /// </summary>
        private async Task CheckEditParamAsync(DictEditParam param)
        {
            // 检查字典文字是否重复（排除自身）
            var hasSameDictLabel = await _dictRepository.GetAll()
                .AnyAsync(d => d.ParentId == param.ParentId
                    && d.Category == param.Category
                    && d.DictLabel == param.DictLabel
                    && d.Id != param.Id);

            if (hasSameDictLabel)
            {
                throw new Exception($"存在重复的字典文字，名称为：{param.DictLabel}");
            }

            // 检查字典值是否重复（排除自身）
            var hasSameDictValue = await _dictRepository.GetAll()
                .AnyAsync(d => d.ParentId == param.ParentId
                    && d.DictValue == param.DictValue
                    && d.Id != param.Id);

            if (hasSameDictValue)
            {
                throw new Exception($"存在重复的字典值，名称为：{param.DictValue}");
            }
        }



        /// <summary>
        /// 构建树形结构
        /// </summary>
        private List<DictTreeNode> BuildTree(List<DictTreeNode> nodes, Guid parentId)
        {
            var tree = new List<DictTreeNode>();
            var children = nodes.Where(n => n.ParentId == parentId).OrderBy(n => n.SortCode).ToList();

            foreach (var child in children)
            {
                child.Children = BuildTree(nodes, child.Id);
                tree.Add(child);
            }

            return tree;
        }

    }
}
