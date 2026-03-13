using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Auditing;
using Abp.Authorization;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore.Repositories;
using Abp.Extensions;
using Abp.IdentityFramework;
using Abp.Linq.Extensions;
using Abp.Localization;
using Abp.Runtime.Caching;
using Abp.Runtime.Session;
using Abp.UI;
using DeviceManagementSystem.Authorization;
using DeviceManagementSystem.Authorization.Organizations;
using DeviceManagementSystem.Authorization.Positions;
using DeviceManagementSystem.Authorization.Resources;
using DeviceManagementSystem.Authorization.Roles;
using DeviceManagementSystem.Authorization.Users;
using DeviceManagementSystem.Dicts.param;
using DeviceManagementSystem.Resources.Menus.Constants;
using DeviceManagementSystem.Resources.Modules.Constants;
using DeviceManagementSystem.Roles.Dto;
using DeviceManagementSystem.SystemConfigs;
using DeviceManagementSystem.SystemRelations;
using DeviceManagementSystem.Users.Dto;
using DeviceManagementSystem.Utils.Common;
using EFCore.BulkExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using SoftwareReleaseManagement.Roles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Users
{
    /// <summary>
    /// 用户服务接口
    /// </summary>
    [AbpAuthorize]
    public class UserAppService : AsyncCrudAppService<User, UserDto, long, PagedUserResultRequestDto, CreateUserDto, UserDto>, IUserAppService
    {
        private readonly UserManager _userManager;
        private readonly RoleManager _roleManager;
        private readonly IRepository<Role> _roleRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IAbpSession _abpSession;
        private readonly LogInManager _logInManager;
        private readonly IRepository<Resource, Guid> _resourceRepository;
        private readonly ISystemRelationAppService _systemRelationAppService;
        private readonly IRepository<SystemRelation, Guid> _systemRelationRepository;
        private readonly ISystemConfigAppService _systemConfigAppService;
        private readonly IRepository<Organization, Guid> _organizationRepository;
        private readonly IRepository<Position, Guid> _positionRepository;
        private readonly ICacheManager _cacheManager;
        private readonly IRoleAppService _roleAppService;

        /// <summary>
        /// 构造方法
        /// </summary>
        public UserAppService(
            IRepository<User, long> repository,
            UserManager userManager,
            RoleManager roleManager,
            IRoleAppService roleAppService,
            IRepository<Role> roleRepository,
            IPasswordHasher<User> passwordHasher,
            IAbpSession abpSession,
            LogInManager logInManager,
            IRepository<Resource, Guid> resourceRepository,
            ISystemRelationAppService systemRelationAppService,
            IRepository<SystemRelation, Guid> systemRelationRepository,
            ISystemConfigAppService systemConfigAppService,
            IRepository<Organization, Guid> organizationRepository,
            IRepository<Position, Guid> positionRepository,
            ICacheManager cacheManager)
            : base(repository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _roleRepository = roleRepository;
            _roleAppService = roleAppService;
            _passwordHasher = passwordHasher;
            _abpSession = abpSession;
            _logInManager = logInManager;
            _resourceRepository = resourceRepository;
            _systemRelationAppService = systemRelationAppService;
            _systemRelationRepository = systemRelationRepository;
            _systemConfigAppService = systemConfigAppService;
            _organizationRepository = organizationRepository;
            _positionRepository = positionRepository;
            _cacheManager = cacheManager;
        }


        /// <summary>
        /// 根据用户ID获取用户姓名
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [DisableAuditing]
        public async Task<string> GetNameByUserId(long userId)
        {
            var user = await Repository.FirstOrDefaultAsync(userId);
            return user?.Name;
        }





        /// <summary>
        /// 下载用户导入模板
        /// </summary>
        [DisableAuditing]
        public IActionResult GetDownloadImportUserTemplate()
        {
            try
            {
                // 创建工作簿
                using (var workbook = new XSSFWorkbook())
                {
                    

                    // 创建工作表
                    ISheet sheet = workbook.CreateSheet("用户导入模板");

                    // 创建标题行
                    IRow headerRow = sheet.CreateRow(0);

                    // 定义列标题 - 修改为使用名称
                    string[] headers = new string[]
                    {
                    "用户名*",
                    "姓名*",
                    "邮箱",
                    "手机号",
                    "机构名称",
                    "职位名称",
                    "备注",
                    };

                    // 设置标题样式
                    ICellStyle headerStyle = workbook.CreateCellStyle();
                    headerStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LightGreen.Index;
                    headerStyle.FillPattern = FillPattern.SolidForeground;
                    headerStyle.Alignment = HorizontalAlignment.Center;
                    IFont headerFont = workbook.CreateFont();
                    headerFont.IsBold = true;
                    headerStyle.SetFont(headerFont);

                    // 填充标题
                    for (int i = 0; i < headers.Length; i++)
                    {
                        ICell cell = headerRow.CreateCell(i);
                        cell.SetCellValue(headers[i]);
                        cell.CellStyle = headerStyle;
                        sheet.SetColumnWidth(i, 20 * 256); // 设置列宽
                    }

                    // 创建示例行
                    IRow exampleRow = sheet.CreateRow(1);
                    exampleRow.CreateCell(0).SetCellValue("zhangsan");
                    exampleRow.CreateCell(1).SetCellValue("张三");
                    exampleRow.CreateCell(2).SetCellValue("zhangsan@example.com");
                    exampleRow.CreateCell(3).SetCellValue("13800138000");
                    exampleRow.CreateCell(4).SetCellValue("技术部");
                    exampleRow.CreateCell(5).SetCellValue("软件工程师");
                    exampleRow.CreateCell(6).SetCellValue("示例备注");

                    // 添加说明行
                    IRow noteRow1 = sheet.CreateRow(3);
                    noteRow1.CreateCell(0).SetCellValue("说明：");
                    IRow noteRow2 = sheet.CreateRow(4);
                    noteRow2.CreateCell(0).SetCellValue("1. 带*号为必填项");
                    IRow noteRow3 = sheet.CreateRow(5);
                    noteRow3.CreateCell(0).SetCellValue("2. 机构名称和职位名称必须与系统中完全一致");
                    IRow noteRow4 = sheet.CreateRow(6);
                    noteRow4.CreateCell(0).SetCellValue("3. 机构如果有多级，请填写完整路径，如：总公司/技术部/研发组");

                    // 将工作簿写入内存流
                    using (var memoryStream = new MemoryStream())
                    {
                        workbook.Write(memoryStream);
                        var fileBytes = memoryStream.ToArray();
                        string fileName;
                        fileName = $"用户导入模板_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                        return new FileContentResult(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                        {
                            FileDownloadName = fileName
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("生成导入模板失败", ex);
                throw new UserFriendlyException("生成导入模板失败：" + ex.Message);
            }
        }





        /// <summary>
        /// 导入用户数据（从Excel文件）
        /// </summary>
        [DisableAuditing]
        public async Task<CommonResult<UserImportResultDto>> Import(IFormFile file)
        {
            var result = new UserImportResultDto();
            var importItems = new List<UserImportItemDto>();
            var failItems = new List<UserImportFailItemDto>();

            try
            {
                if (file == null || file.Length == 0)
                {
                    return CommonResult<UserImportResultDto>.Error("请选择要导入的文件");
                }

                // 检查文件格式
                string fileExtension = Path.GetExtension(file.FileName).ToLower();
                if (fileExtension != ".xls" && fileExtension != ".xlsx")
                {
                    return CommonResult<UserImportResultDto>.Error("只支持Excel文件(.xls, .xlsx)");
                }

                // 预先加载所有机构和职位数据，提高性能
                var allOrgs = await _organizationRepository.GetAll().ToListAsync();
                var allPositions = await _positionRepository.GetAll().ToListAsync();

                // 读取Excel文件
                using (var stream = file.OpenReadStream())
                {
                    IWorkbook workbook;
                    if (fileExtension == ".xlsx")
                    {
                        workbook = new XSSFWorkbook(stream);
                    }
                    else
                    {
                        workbook = new HSSFWorkbook(stream);
                    }

                    ISheet sheet = workbook.GetSheetAt(0);
                    if (sheet == null)
                    {
                        return CommonResult<UserImportResultDto>.Error("Excel文件中没有工作表");
                    }

                    // 从第1行开始读取（跳过标题行）
                    for (int rowIndex = 1; rowIndex <= sheet.LastRowNum; rowIndex++)
                    {
                        IRow row = sheet.GetRow(rowIndex);
                        if (row == null) continue;

                        // 检查前两列是否为空（用户名和姓名为必填）
                        var userName = GetCellStringValue(row.GetCell(0));
                        var name = GetCellStringValue(row.GetCell(1));

                        if (string.IsNullOrWhiteSpace(userName) && string.IsNullOrWhiteSpace(name))
                        {
                            // 如果两列都为空，跳过此行
                            continue;
                        }

                        try
                        {
                            var importItem = new UserImportItemDto
                            {
                                UserName = userName,
                                Name = name,
                                EmailAddress = GetCellStringValue(row.GetCell(2)),
                                PhoneNumber = GetCellStringValue(row.GetCell(3)),
                                OrgName = GetCellStringValue(row.GetCell(4)),
                                PositionName = GetCellStringValue(row.GetCell(5))
                            };


                            // 验证必填项
                            if (string.IsNullOrWhiteSpace(importItem.UserName))
                            {
                                throw new Exception("用户名不能为空");
                            }

                            if (string.IsNullOrWhiteSpace(importItem.Name))
                            {
                                throw new Exception("姓名不能为空");
                            }

                            // 验证用户名是否已存在
                            var existingUser = await _userManager.FindByNameAsync(importItem.UserName);
                            if (existingUser != null)
                            {
                                throw new Exception($"用户名 '{importItem.UserName}' 已存在");
                            }

                            // 验证邮箱是否已存在
                            if (!string.IsNullOrWhiteSpace(importItem.EmailAddress))
                            {
                                var existingEmail = await Repository.FirstOrDefaultAsync(x => x.EmailAddress == importItem.EmailAddress);
                                if (existingEmail != null)
                                {
                                    throw new Exception($"邮箱 '{importItem.EmailAddress}' 已被使用");
                                }
                            }

                            // 验证机构名称是否存在
                            if (!string.IsNullOrWhiteSpace(importItem.OrgName))
                            {
                                // 查找机构
                                var org = await FindOrganizationByName(importItem.OrgName, allOrgs);
                                if (org == null)
                                {
                                    throw new Exception($"机构 '{importItem.OrgName}' 不存在");
                                }
                            }

                            // 验证职位名称是否存在
                            if (!string.IsNullOrWhiteSpace(importItem.PositionName))
                            {
                                var position = allPositions.FirstOrDefault(p => p.Name == importItem.PositionName);
                                if (position == null)
                                {
                                    throw new Exception($"职位 '{importItem.PositionName}' 不存在");
                                }
                            }


                            importItems.Add(importItem);
                            result.TotalCount++;
                        }
                        catch (Exception ex)
                        {
                            var failItem = new UserImportFailItemDto
                            {
                                UserName = GetCellStringValue(row.GetCell(0)),
                                Name = GetCellStringValue(row.GetCell(1)),
                                ErrorMessage = ex.Message
                            };
                            failItems.Add(failItem);
                            result.FailCount++;
                            result.TotalCount++;
                        }
                    }
                }

                // 批量导入成功的数据
                if (importItems.Count > 0)
                {
                    foreach (var importItem in importItems)
                    {
                        try
                        {
                            var createUserDto = new CreateUserDto
                            {
                                UserName = importItem.UserName,
                                Name = importItem.Name,
                                EmailAddress = importItem.EmailAddress,
                                PhoneNumber = importItem.PhoneNumber,
                                IsActive = true,
                                Password = "Qwe123." // 默认密码
                            };

                            // 根据机构名称获取机构ID
                            if (!string.IsNullOrWhiteSpace(importItem.OrgName))
                            {
                                var org = await FindOrganizationByName(importItem.OrgName, null);
                                if (org != null)
                                {
                                    createUserDto.OrgId = org.Id;
                                }
                            }

                            // 根据职位名称获取职位ID
                            if (!string.IsNullOrWhiteSpace(importItem.PositionName))
                            {
                                var position = await _positionRepository.FirstOrDefaultAsync(x => x.Name == importItem.PositionName);
                                if (position != null)
                                {
                                    createUserDto.PositionId = position.Id;
                                }
                            }

                            createUserDto.RoleNames = [];
                       
                            var createResult = await CreateUserAsync(createUserDto);
                            if (createResult.Code == CommonResult.CODE_SUCCESS)
                            {
                                result.SuccessCount++;
                            }
                            else
                            {
                                var failItem = new UserImportFailItemDto
                                {
                                    UserName = importItem.UserName,
                                    Name = importItem.Name,
                                    ErrorMessage = createResult.Message
                                };
                                failItems.Add(failItem);
                                result.FailCount++;
                            }
                        }
                        catch (Exception ex)
                        {
                            var failItem = new UserImportFailItemDto
                            {
                                UserName = importItem.UserName,
                                Name = importItem.Name,
                                ErrorMessage = ex.Message
                            };
                            failItems.Add(failItem);
                            result.FailCount++;
                        }
                    }
                }

                result.FailItems = failItems;

                string message = $"导入完成。总计：{result.TotalCount}条，成功：{result.SuccessCount}条，失败：{result.FailCount}条";
                return CommonResult<UserImportResultDto>.Success(message, result);
            }
            catch (Exception ex)
            {
                Logger.Error("导入用户数据失败", ex);
                return CommonResult<UserImportResultDto>.Error($"导入失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 根据机构名称查找机构（支持多级路径）
        /// </summary>
        private async Task<Organization> FindOrganizationByName(string orgName, List<Organization> allOrgs = null)
        {
            if (string.IsNullOrWhiteSpace(orgName))
                return null;

            // 如果传入了所有机构列表，使用该列表，否则查询数据库
            List<Organization> orgs;
            if (allOrgs != null)
            {
                orgs = allOrgs;
            }
            else
            {
                orgs = await _organizationRepository.GetAll().ToListAsync();
            }

            // 检查是否有路径分隔符（如：总公司/技术部/研发组）
            if (orgName.Contains("/"))
            {
                var pathParts = orgName.Split('/', StringSplitOptions.RemoveEmptyEntries);

                Organization currentOrg = null;
                foreach (var part in pathParts)
                {
                    var partName = part.Trim();

                    // 查找当前部分
                    var foundOrg = orgs.FirstOrDefault(o =>
                        o.Name == partName &&
                        o.ParentId == (currentOrg?.Id ?? Guid.Empty));

                    if (foundOrg == null)
                    {
                        return null;
                    }

                    currentOrg = foundOrg;
                }

                return currentOrg;
            }
            else
            {
                // 没有路径分隔符，直接按名称查找（可能会有重名问题）
                // 优先查找根级机构
                var org = orgs.FirstOrDefault(o => o.Name == orgName && o.ParentId == Guid.Empty);
                if (org == null)
                {
                    // 如果找不到根级机构，返回第一个匹配的
                    org = orgs.FirstOrDefault(o => o.Name == orgName);
                }

                return org;
            }
        }

        /// <summary>
        /// 批量导入用户（高性能版）
        /// </summary>
        public async Task<CommonResult<UserImportResultDto>> ImportBatch(UserImportDto input)
        {
            var result = new UserImportResultDto();

            try
            {
                if (input == null || input.DataList == null || input.DataList.Count == 0)
                {
                    return CommonResult<UserImportResultDto>.Error("导入数据不能为空");
                }

                // 预先加载数据
                var allOrgs = await _organizationRepository.GetAll().ToListAsync();
                var allPositions = await _positionRepository.GetAll().ToListAsync();

                var successItems = new List<CreateUserDto>();
                var failItems = new List<UserImportFailItemDto>();

                result.TotalCount = input.DataList.Count;

                // 第一步：数据验证
                for (int i = 0; i < input.DataList.Count; i++)
                {
                    var item = input.DataList[i];
                    try
                    {
                        // 验证必填项
                        if (string.IsNullOrWhiteSpace(item.UserName))
                        {
                            throw new Exception("用户名不能为空");
                        }

                        if (string.IsNullOrWhiteSpace(item.Name))
                        {
                            throw new Exception("姓名不能为空");
                        }

                        // 验证用户名是否已存在
                        var existingUser = await Repository.FirstOrDefaultAsync(x => x.UserName == item.UserName);
                        if (existingUser != null)
                        {
                            throw new Exception($"用户名 '{item.UserName}' 已存在");
                        }

                        // 验证邮箱是否已存在
                        if (!string.IsNullOrWhiteSpace(item.EmailAddress))
                        {
                            var existingEmail = await Repository.FirstOrDefaultAsync(x => x.EmailAddress == item.EmailAddress);
                            if (existingEmail != null)
                            {
                                throw new Exception($"邮箱 '{item.EmailAddress}' 已被使用");
                            }
                        }

                        // 验证机构名称是否存在
                        if (!string.IsNullOrWhiteSpace(item.OrgName))
                        {
                            var org = await FindOrganizationByName(item.OrgName, allOrgs);
                            if (org == null)
                            {
                                throw new Exception($"机构 '{item.OrgName}' 不存在");
                            }
                        }

                        // 验证职位名称是否存在
                        if (!string.IsNullOrWhiteSpace(item.PositionName))
                        {
                            var position = allPositions.FirstOrDefault(p => p.Name == item.PositionName);
                            if (position == null)
                            {
                                throw new Exception($"职位 '{item.PositionName}' 不存在");
                            }
                        }

                     
                        // 转换为CreateUserDto
                        var createUserDto = new CreateUserDto
                        {
                            UserName = item.UserName,
                            Name = item.Name,
                            EmailAddress = item.EmailAddress,
                            PhoneNumber = item.PhoneNumber,                          
                            IsActive = true,
                            Password = "123456"
                        };

                        // 设置机构ID
                        if (!string.IsNullOrWhiteSpace(item.OrgName))
                        {
                            var org = await FindOrganizationByName(item.OrgName, allOrgs);
                            if (org != null)
                            {
                                createUserDto.OrgId = org.Id;
                            }
                        }

                        // 设置职位ID
                        if (!string.IsNullOrWhiteSpace(item.PositionName))
                        {
                            var position = allPositions.FirstOrDefault(p => p.Name == item.PositionName);
                            if (position != null)
                            {
                                createUserDto.PositionId = position.Id;
                            }
                        }

                        successItems.Add(createUserDto);
                    }
                    catch (Exception ex)
                    {
                        var failItem = new UserImportFailItemDto
                        {
                            UserName = item.UserName,
                            Name = item.Name,
                            ErrorMessage = ex.Message
                        };
                        failItems.Add(failItem);
                        result.FailCount++;
                    }
                }

                // 第二步：批量创建用户
                if (successItems.Count > 0)
                {
                    var usersToAdd = new List<User>();

                    foreach (var createDto in successItems)
                    {
                        try
                        {
                            var user = ObjectMapper.Map<User>(createDto);
                            user.IsEmailConfirmed = true;
                            user.IsActive = createDto.IsActive;
                            user.IsLockoutEnabled = false;
                            user.Password = _passwordHasher.HashPassword(user, createDto.Password);
                            user.SetNormalizedNames();

                            usersToAdd.Add(user);
                        }
                        catch (Exception ex)
                        {
                            var failItem = new UserImportFailItemDto
                            {
                                UserName = createDto.UserName,
                                Name = createDto.Name,
                                ErrorMessage = $"转换用户数据失败：{ex.Message}"
                            };
                            failItems.Add(failItem);
                            result.FailCount++;
                            result.SuccessCount--; // 从成功中移除
                        }
                    }

                    // 使用批量插入
                    if (usersToAdd.Count > 0)
                    {
                        try
                        {
                            // 使用EF Core的批量插入
                            var repository = Repository as IRepository<User, long>;

                            // 方法1：使用AddRange（适用于少量数据）
                            await repository.InsertRangeAsync(usersToAdd);

                            // 方法2：如果需要高性能批量插入，可以启用EFCore.BulkExtensions
                            // await _dbContext.BulkInsertAsync(usersToAdd);

                            result.SuccessCount = usersToAdd.Count;
                        }
                        catch (Exception ex)
                        {
                            Logger.Error("批量插入用户失败", ex);

                            // 将批量失败的项添加到失败列表
                            foreach (var user in usersToAdd)
                            {
                                var failItem = new UserImportFailItemDto
                                {
                                    UserName = user.UserName,
                                    Name = user.Name,
                                    ErrorMessage = $"批量插入失败：{ex.Message}"
                                };
                                failItems.Add(failItem);
                                result.FailCount++;
                            }
                            result.SuccessCount = 0;
                        }
                    }
                }

                result.FailItems = failItems;

                string message = $"导入完成。总计：{result.TotalCount}条，成功：{result.SuccessCount}条，失败：{result.FailCount}条";
                return CommonResult<UserImportResultDto>.Success(message, result);
            }
            catch (Exception ex)
            {
                Logger.Error("批量导入用户失败", ex);
                return CommonResult<UserImportResultDto>.Error($"批量导入失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 获取单元格字符串值
        /// </summary>
        private string GetCellStringValue(ICell cell)
        {
            if (cell == null) return string.Empty;

            switch (cell.CellType)
            {
                case CellType.String:
                    return cell.StringCellValue?.Trim();
                case CellType.Boolean:
                    return cell.BooleanCellValue.ToString();
                case CellType.Formula:
                    try
                    {
                        return cell.StringCellValue?.Trim();
                    }
                    catch
                    {
                        return cell.NumericCellValue.ToString();
                    }
                default:
                    return string.Empty;
            }
        }




        /// <summary>
        /// 获取登录用户PC端菜单
        /// </summary>
        public async Task<CommonResult<List<Tree<string>>>> GetLoginMenu()
        {
            try
            {
                var userId = _abpSession.GetUserId();
                var cacheKey = $"UserMenu_{userId}";

                // 尝试从缓存获取
                var cache = _cacheManager.GetCache("UserMenus");
                var cachedResult = await cache.GetOrDefaultAsync(cacheKey);
                if (cachedResult != null)
                {
                    return CommonResultHelper.Create(CommonResult.CODE_SUCCESS, "获取菜单成功（缓存）",
                        (List<Tree<string>>)cachedResult);
                }

                var sysUserIdParam = new SysUserIdParam
                {
                    Id = userId
                };

                var menus = await OwnMenu(sysUserIdParam);

                // 存入缓存，设置过期时间（30分钟）
                await cache.SetAsync(cacheKey, menus, TimeSpan.FromMinutes(30));

                return CommonResultHelper.Create(CommonResult.CODE_SUCCESS, "获取菜单成功", menus);
            }
            catch (Exception ex)
            {
                return CommonResultHelper.Create(CommonResult.CODE_ERROR, "获取菜单失败：" + ex.Message, default(List<Tree<string>>));
            }
        }

        /// <summary>
        /// 获取用户拥有的菜单（内部方法） - 修复版本
        /// </summary>
        private async Task<List<Tree<string>>> OwnMenu(SysUserIdParam sysUserIdParam)
        {
            // 获取当前用户信息
            var currentUser = await _userManager.GetUserByIdAsync(sysUserIdParam.Id);

            // 检查是否为admin用户
            bool isAdmin = currentUser?.UserName?.ToLower() == "admin";

            List<Guid> userMenuGuidList = new List<Guid>();

            if (!isAdmin)
            {
                // 非admin用户：正常流程
                // 获取角色id列表
                var roleIdList = await GetOwnRole(sysUserIdParam);

                // 获取菜单id列表（用户直接拥有的资源）
                var menuIdList = await _systemRelationAppService.GetRelationTargetListByObjectIdAndCategoryAsync(
                    sysUserIdParam.Id.ToString(),
                    SysRelationCategoryConstant.SYS_USER_HAS_RESOURCE);

                // 转换字符串ID为Guid列表
                userMenuGuidList = menuIdList.Select(Guid.Parse).ToList();

                // 如果用户有角色，获取角色拥有的资源
                if (roleIdList != null && roleIdList.Count > 0)
                {
                    // 获取角色拥有的资源ID列表
                    var roleMenuIds = await _systemRelationAppService.GetRelationTargetListByObjectIdListAndCategoryAsync(
                        roleIdList.Select(r => r.ToString()).ToList(),
                        SysRelationCategoryConstant.SYS_ROLE_HAS_RESOURCE);

                    // 添加到菜单ID列表
                    userMenuGuidList.AddRange(roleMenuIds.Select(Guid.Parse));
                }
            }
            else
            {
                // admin用户：获取所有菜单ID
                // 获取所有的菜单和模块列表
                var allMenuAndModuleList = await _resourceRepository.GetAll()
                    .Where(x => x.Category == ResourceCategoryConstant.MODULE ||
                               x.Category == ResourceCategoryConstant.MENU)
                    .ToListAsync();

                userMenuGuidList = allMenuAndModuleList.Select(x => x.Id).Distinct().ToList();
            }

            // 获取所有的菜单和模块列表，并按分类和排序码排序
            var allModuleAndMenuAndSpaList = await _resourceRepository.GetAll()
                .Where(x => x.Category == ResourceCategoryConstant.MODULE ||
                           x.Category == ResourceCategoryConstant.MENU)
                .OrderBy(x => x.Category)
                .ThenBy(x => x.SortCode)
                .ToListAsync();

            // 分离模块和菜单
            var allModuleList = allModuleAndMenuAndSpaList
                .Where(x => x.Category == ResourceCategoryConstant.MODULE)
                .ToList();

            var allMenuList = allModuleAndMenuAndSpaList
                .Where(x => x.Category == ResourceCategoryConstant.MENU)
                .ToList();

            // 定义结果列表 - 使用字典来确保唯一性
            var resultDict = new Dictionary<Guid, Resource>();

            // 获取用户拥有的菜单列表
            var menuList = allMenuList
                .Where(menu => userMenuGuidList.Contains(menu.Id))
                .ToList();

            // 对获取到的菜单列表进行处理，获取父列表
            foreach (var menu in menuList)
            {
                // 先将当前菜单添加到结果字典
                if (!resultDict.ContainsKey(menu.Id))
                {
                    resultDict[menu.Id] = menu;
                }

                // 递归查找父级菜单
                await ExecRecursionFindParent(allMenuList, menu.Id, resultDict);
            }

            // 获取模块id集合
            var moduleIdSet = resultDict.Values
                .Where(x => x.Module.HasValue)
                .Select(x => x.Module.Value)
                .Distinct()
                .ToHashSet();

            // 抽取拥有的模块列表
            var moduleList = allModuleList
                .Where(module => moduleIdSet.Contains(module.Id))
                .ToList();

            // 如果一个模块都没拥有
            if (moduleList.Count == 0)
            {
                // 如果系统中无模块（极端情况）
                if (allModuleList.Count == 0)
                {
                    // 如果系统中无菜单，则返回空列表
                    if (allMenuList.Count == 0)
                    {
                        return new List<Tree<string>>();
                    }
                    else
                    {
                        // 否则构造一个模块，并添加到拥有模块
                        var sysMenu = new Resource
                        {
                            Id = Guid.NewGuid(),
                            Name = "系统",
                            Title = "系统",
                            Code = "system",
                            Path = "/system",
                            Category = ResourceCategoryConstant.MODULE,
                            SortCode = 1
                        };
                        allModuleList.Add(sysMenu);
                        moduleList.Add(sysMenu);
                    }
                }
                else
                {
                    // 否则将系统中第一个模块作为拥有的模块
                    moduleList.Add(allModuleList[0]);
                }
            }

            // 将拥有的模块添加到结果字典
            foreach (var module in moduleList)
            {
                if (!resultDict.ContainsKey(module.Id))
                {
                    resultDict[module.Id] = module;
                }
            }

            // 将字典值转换为列表
            var resultList = resultDict.Values.ToList();

            // 获取第一个模块
            var firstModule = moduleList[0];

            // 获取第一个模块下的第一个菜单（非目录类型）
            var firstMenuInModule = menuList
                .Where(menu => menu.Module.HasValue && menu.Module.Value == firstModule.Id)
                .Where(menu => menu.MenuType != MenuTypeConstant.CATALOG)
                .OrderBy(menu => menu.SortCode)
                .FirstOrDefault();

            // 最终处理，构造返回数据结构 - 修复版本：使用临时变量，不修改原始实体
            var resultJsonObjectList = resultList.Select(resource =>
            {
                var resourceDict = new Dictionary<string, object>();

                // 使用临时变量，不修改原始对象
                Guid? tempParentId = resource.ParentId;
                string tempPath = resource.Path;
                string tempDisplayLayout = resource.DisplayLayout ?? "YES";
                string tempMenuType = resource.MenuType ?? string.Empty;
                Guid? tempModule = resource.Module;
                string tempName = resource.Name ?? resource.Title;
                string tempTitle = resource.Title ?? string.Empty;
                string tempCode = resource.Code ?? string.Empty;
                string tempCategory = resource.Category ?? string.Empty;
                string tempComponent = resource.Component ?? string.Empty;
                string tempIcon = resource.Icon ?? string.Empty;
                string tempColor = resource.Color ?? string.Empty;
                int tempSortCode = (int)resource.SortCode;
                string tempVisible = resource.Visible.HasValue && resource.Visible.Value ? "TRUE" : "FALSE";
                string tempUpdateTime = resource.LastModificationTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? string.Empty;
                string tempExtJson = resource.ExtJson ?? string.Empty;

                // 将模块的父id设置为"0"，设置随机path
                if (tempCategory == ResourceCategoryConstant.MODULE)
                {
                    tempParentId = Guid.Empty;
                    // 只在返回数据中使用临时path，不修改原始对象
                    tempPath = string.IsNullOrEmpty(tempPath) ? "/" + GenerateRandomString(10) : tempPath;
                }

                // 将根菜单的父id设置为模块的id
                if (tempCategory == ResourceCategoryConstant.MENU)
                {
                    if (tempParentId == Guid.Empty)
                    {
                        tempParentId = tempModule ?? Guid.Empty;
                    }
                }

                // 基础字段 - 使用临时变量
                resourceDict["id"] = resource.Id.ToString();
                resourceDict["parentId"] = tempParentId?.ToString() ?? Guid.Empty.ToString();
                resourceDict["weight"] = tempSortCode;
                resourceDict["name"] = tempName;
                resourceDict["title"] = tempTitle;
                resourceDict["code"] = tempCode;
                resourceDict["category"] = tempCategory;
                resourceDict["path"] = tempPath;
                resourceDict["component"] = tempComponent;
                resourceDict["icon"] = tempIcon;
                resourceDict["color"] = tempColor;
                resourceDict["sortCode"] = tempSortCode;

                // 处理visible字段（转换为字符串格式）
                resourceDict["visible"] = tempVisible;

                resourceDict["displayLayout"] = tempDisplayLayout;
                resourceDict["keepLive"] = "YES";
                resourceDict["menuType"] = tempMenuType;
                resourceDict["module"] = tempModule?.ToString() ?? Guid.Empty.ToString();
                resourceDict["deleteFlag"] = "NOT_DELETE";
                resourceDict["updateTime"] = tempUpdateTime;
                resourceDict["extJson"] = tempExtJson;

                // 构建meta信息
                var metaDict = new Dictionary<string, object>
                {
                    ["icon"] = tempIcon,
                    ["title"] = tempTitle
                };

                if (string.IsNullOrEmpty(tempMenuType))
                {
                    metaDict["type"] = tempCategory?.ToLower() ?? string.Empty;
                }
                else
                {
                    metaDict["type"] = tempMenuType.ToLower();
                }

                // 如果是菜单，则设置type菜单类型为小写
                if (tempCategory == ResourceCategoryConstant.MENU)
                {
                    if (tempMenuType != MenuTypeConstant.CATALOG)
                    {
                        metaDict["type"] = tempMenuType?.ToLower() ?? string.Empty;
                    }

                    // 如果是首页，则设置affix
                    if (firstMenuInModule != null && resource.Id == firstMenuInModule.Id)
                    {
                        metaDict["affix"] = true;
                    }

                    // 设置keepLive
                    metaDict["keepLive"] = false;

                    // 设置显示布局
                    if (string.IsNullOrEmpty(tempDisplayLayout) ||
                        tempDisplayLayout.ToUpper() == "YES")
                    {
                        metaDict["displayLayout"] = true;
                    }
                    else if (tempDisplayLayout.ToUpper() == "NO")
                    {
                        metaDict["displayLayout"] = false;
                    }
                }

                // 如果设置了不可见，那么设置为false
                if (resource.Visible.HasValue && resource.Visible.Value == false)
                {
                    metaDict["hidden"] = true;
                }
                else
                {
                    metaDict["hidden"] = false;
                }

                resourceDict["meta"] = metaDict;
                return resourceDict;
            }).ToList();

            var res = resultJsonObjectList;

            // 执行构造树
            var treeNodeList = resultJsonObjectList.Select(dict =>
            {
                var id = dict["id"].ToString();
                var parentId = dict["parentId"].ToString();
                var title = dict["title"].ToString();
                var name = dict["name"].ToString();
                var weight = dict["weight"] as int? ?? 0;
                var path = dict["path"].ToString();

                // 移除不需要的字段
                var extraDict = new Dictionary<string, object>(dict);
                extraDict.Remove("id");
                extraDict.Remove("parentId");

                var treeNode = new TreeNode<string>(id, parentId, name, title, weight);
                treeNode.Extra = extraDict;
                return treeNode;
            }).ToList();

            // 构建树结构
            return BuildTree(treeNodeList, Guid.Empty.ToString());
        }

        /// <summary>
        /// 获取用户拥有资源
        /// </summary>
        public async Task<CommonResult<SysUserOwnResourceResult>> GetOwnResource(SysUserIdParam sysUserIdParam)
        {
            try
            {
                // 获取当前用户信息
                var currentUser = await _userManager.GetUserByIdAsync(sysUserIdParam.Id);
                bool isAdmin = currentUser?.UserName?.ToLower() == "admin";

                var sysUserOwnResourceResult = new SysUserOwnResourceResult
                {
                    Id = sysUserIdParam.Id
                };

                if (isAdmin)
                {
                    // admin用户：获取所有菜单资源
                    var allMenus = await _resourceRepository.GetAll()
                        .Where(x => x.Category == ResourceCategoryConstant.MENU)
                        .Select(x => x.Id)
                        .ToListAsync();

                    var grantInfoList = allMenus.Select(menuId => new SysUserOwnResourceResult.SysUserOwnResource
                    {
                        MenuId = menuId
                    }).ToList();

                    sysUserOwnResourceResult.GrantInfoList = grantInfoList;
                }
                else
                {
                    // 正常流程
                    // 获取用户拥有的资源关系
                    var relations = await _systemRelationAppService.GetRelationListByObjectIdAndCategoryAsync(
                        sysUserIdParam.Id.ToString(),
                        SysRelationCategoryConstant.SYS_USER_HAS_RESOURCE);

                    // 转换扩展信息
                    var grantInfoList = relations.Select(relation =>
                    {
                        try
                        {
                            var extJson = relation.ExtJson;
                            if (!string.IsNullOrEmpty(extJson))
                            {
                                var grantInfo = JsonConvert.DeserializeObject<SysUserOwnResourceResult.SysUserOwnResource>(extJson);
                                return grantInfo;
                            }
                        }
                        catch (Exception)
                        {
                            // JSON解析失败时，创建默认对象
                        }

                        return new SysUserOwnResourceResult.SysUserOwnResource
                        {
                            MenuId = Guid.Parse(relation.Target)
                        };
                    }).ToList();

                    sysUserOwnResourceResult.GrantInfoList = grantInfoList;
                }

                return CommonResultHelper.Create(CommonResult.CODE_SUCCESS, "获取用户资源成功", sysUserOwnResourceResult);
            }
            catch (Exception ex)
            {
                return CommonResultHelper.Create(CommonResult.CODE_ERROR, "获取用户资源失败：" + ex.Message, default(SysUserOwnResourceResult));
            }
        }

        /// <summary>
        /// 给用户授权资源
        /// </summary>
        public async Task<CommonResult> GrantResource(SysUserGrantResourceParam sysUserGrantResourceParam)
        {
            try
            {
                // 检查是否为admin用户
                var currentUser = await _userManager.GetUserByIdAsync(sysUserGrantResourceParam.Id);
                bool isAdmin = currentUser?.UserName?.ToLower() == "admin";

                if (isAdmin)
                {
                    // admin用户不允许授权资源（因为已拥有所有权限）
                    return CommonResultHelper.Create(CommonResult.CODE_ERROR, "admin用户拥有所有权限，无需授权", null);
                }

                var menuIdList = sysUserGrantResourceParam.GrantInfoList
                    .Select(g => g.MenuId)
                    .ToList();

                if (menuIdList.Count > 0)
                {
                    var sysUserIdParam = new SysUserIdParam { Id = sysUserGrantResourceParam.Id };
                    var roleIdList = await GetOwnRole(sysUserIdParam);

                    // 获取模块ID列表
                    var resources = await _resourceRepository.GetAll()
                        .Where(x => menuIdList.Contains(x.Id))
                        .ToListAsync();

                    var moduleIdList = resources
                        .Where(x => x.Module.HasValue)
                        .Select(x => x.Module.Value)
                        .Distinct()
                        .ToList();

                    // 检查是否包含系统模块
                    var modules = await _resourceRepository.GetAll()
                        .Where(x => moduleIdList.Contains(x.Id) && x.Category == ResourceCategoryConstant.MODULE)
                        .ToListAsync();

                    var containsSystemModule = modules
                        .Any(module => module.Code == SysBuildInEnum.BUILD_IN_MODULE_CODE);

                    if (containsSystemModule)
                    {
                        if (roleIdList == null || roleIdList.Count == 0)
                        {
                            throw new UserFriendlyException("非超管角色用户不可被授权系统模块菜单资源");
                        }
                        else
                        {
                            // 检查是否有超管角色
                            var roles = await _roleRepository.GetAll()
                                .Where(x => roleIdList.Contains(x.Id))
                                .ToListAsync();

                            var hasSuperAdminRole = roles
                                .Any(role => role.Name == SysBuildInEnum.BUILD_IN_ROLE_CODE);

                            if (!hasSuperAdminRole)
                            {
                                throw new UserFriendlyException("非超管角色用户不可被授权系统模块菜单资源");
                            }
                        }
                    }
                }

                // 准备扩展信息列表
                var extJsonList = sysUserGrantResourceParam.GrantInfoList
                    .Select(grantInfo => JsonConvert.SerializeObject(grantInfo))
                    .ToList();

                var targetIdList = sysUserGrantResourceParam.GrantInfoList
                    .Select(g => g.MenuId.ToString())
                    .ToList();

                // 清理并保存关系
                await _systemRelationAppService.SaveRelationBatchWithClearAsync(
                    sysUserGrantResourceParam.Id.ToString(),
                    targetIdList,
                    SysRelationCategoryConstant.SYS_USER_HAS_RESOURCE,
                    extJsonList);

                // 清除用户菜单缓存
                var cacheKey = $"UserMenu_{sysUserGrantResourceParam.Id}";
                await _cacheManager.GetCache("UserMenus").RemoveAsync(cacheKey);

                return CommonResult.Ok("授权成功");
            }
            catch (UserFriendlyException ex)
            {
                return CommonResultHelper.Create(CommonResult.CODE_ERROR, ex.Message, null);
            }
            catch (Exception ex)
            {
                return CommonResultHelper.Create(CommonResult.CODE_ERROR, "授权失败：" + ex.Message, null);
            }
        }



        /// <summary>
        /// 给用户授权角色
        /// </summary>
        public async Task<CommonResult> GrantRole(SysUserGrantRoleParam sysUserGrantRoleParam)
        {
            try
            {
                // 检查是否为admin用户
                var currentUser = await _userManager.GetUserByIdAsync(sysUserGrantRoleParam.Id);
                bool isAdmin = currentUser?.UserName?.ToLower() == "admin";

                if (isAdmin)
                {
                    // admin用户不允许授权资源（因为已拥有所有权限）
                    return CommonResultHelper.Create(CommonResult.CODE_ERROR, "admin用户拥有所有权限，无需授权", null);
                }

                await _systemRelationAppService.SaveRelationBatchWithClearAsync(
                    sysUserGrantRoleParam.Id.ToString(),
                    sysUserGrantRoleParam.RoleIdList.Select(rid => rid.ToString()).ToList(),
                    SysRelationCategoryConstant.SYS_USER_HAS_ROLE);
                return CommonResult.Ok("授权成功");
            }
            catch (UserFriendlyException ex)
            {
                return CommonResultHelper.Create(CommonResult.CODE_ERROR, ex.Message, null);
            }
            catch (Exception ex)
            {
                return CommonResultHelper.Create(CommonResult.CODE_ERROR, "授权失败：" + ex.Message, null);
            }
        }





        /// <summary>
        /// 获取用户拥有的角色ID列表
        /// </summary>
        public async Task<List<long>> GetOwnRole(SysUserIdParam sysUserIdParam)
        {
            try
            {
                // 检查是否为admin用户
                var currentUser = await _userManager.GetUserByIdAsync(sysUserIdParam.Id);
                bool isAdmin = currentUser?.UserName?.ToLower() == "admin";

                if (isAdmin)
                {
                    // admin用户返回空角色列表（因为不需要角色授权）
                    return new List<long>();
                }

                // 正常流程
                // 获取用户拥有的角色关系
                var relations = await _systemRelationAppService.GetRelationTargetListByObjectIdAndCategoryAsync(
                    sysUserIdParam.Id.ToString(),
                    SysRelationCategoryConstant.SYS_USER_HAS_ROLE);

                // 转换字符串ID为long列表
                return relations
                    .Where(id => long.TryParse(id, out _))
                    .Select(long.Parse)
                    .ToList();
            }
            catch (Exception)
            {
                return new List<long>();
            }
        }

        /// <summary>
        /// 激活/禁用用户状态  
        /// </summary>
        /// <returns></returns>
        public async Task<CommonResult<string>> UpdateUserStatus(int id)
        {
            try
            {
                var user = await _userManager.GetUserByIdAsync(id);
                if (user == null)
                {
                    return CommonResult<string>.Error("修改失败，此用户信息获取失败！");
                }
                user.IsActive = !user.IsActive;
                await CurrentUnitOfWork.SaveChangesAsync();

                // 清除用户菜单缓存
                var cacheKey = $"UserMenu_{id}";
                await _cacheManager.GetCache("UserMenus").RemoveAsync(cacheKey);

                return CommonResult<string>.Success("修改成功");
            }
            catch (Exception ex)
            {
                return CommonResult<string>.Error("修改失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 编辑个人工作台快捷方式
        /// </summary>
        public async Task<CommonResult<string>> UpdateUserWorkbench(UserUpdateWorkbenchParam userUpdateWorkbenchParam)
        {
            try
            {
                var CurrentUserId = _abpSession.UserId.ToString();
                await _systemRelationAppService.SaveRelationWithClearAsync(CurrentUserId, null, SysRelationCategoryConstant.SYS_USER_WORKBENCH_DATA, userUpdateWorkbenchParam.WorkbenchData);
                return CommonResult<string>.Success("更新成功");
            }
            catch (Exception ex)
            {
                return CommonResult<string>.Error("更新失败" + ex.Message);
            }
        }

        /// <summary>
        /// 获取登录用户个人工作台快捷方式
        /// </summary>
        /// <returns></returns>
        public async Task<CommonResult<string>> GetLoginUserWorkbench()
        {
            var CurrentUserId = _abpSession.UserId.ToString();
            var workbenchData = await _systemRelationRepository.FirstOrDefaultAsync(it => string.Equals(it.ObjectId, CurrentUserId) && string.Equals(it.Category, SysRelationCategoryConstant.SYS_USER_WORKBENCH_DATA));
            if (workbenchData != null)
            {
                return CommonResult<string>.Success("获取成功", workbenchData.ExtJson);
            }
            var baseWorkbench = await _systemConfigAppService.GetBaseConfigByConfigKey(SysRelationCategoryConstant.SYS_DEFAULT_WORKBENCH_DATA);
            if (baseWorkbench != null)
            {
                return CommonResult<string>.Success("获取成功", baseWorkbench.Data.ConfigValue);
            }
            return CommonResult<string>.Success("获取失败", null);
        }

        /// <summary>
        /// 递归查找父级菜单（使用字典避免重复）
        /// </summary>
        private async Task ExecRecursionFindParent(List<Resource> allMenuList, Guid menuId, Dictionary<Guid, Resource> resultDict)
        {
            var menu = allMenuList.FirstOrDefault(m => m.Id == menuId);
            if (menu == null || menu.ParentId == null || menu.ParentId == Guid.Empty)
            {
                return;
            }

            var parentMenu = allMenuList.FirstOrDefault(m => m.Id == menu.ParentId);
            if (parentMenu != null)
            {
                if (!resultDict.ContainsKey(parentMenu.Id))
                {
                    resultDict[parentMenu.Id] = parentMenu;
                    await ExecRecursionFindParent(allMenuList, parentMenu.Id, resultDict);
                }
            }
        }

        /// <summary>
        /// 生成随机字符串
        /// </summary>
        private string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        /// <summary>
        /// 构建树结构
        /// </summary>
        private List<Tree<string>> BuildTree(List<TreeNode<string>> nodes, string rootId)
        {
            var tree = new List<Tree<string>>();
            var nodeDict = nodes.ToDictionary(n => n.Id);

            foreach (var node in nodes)
            {
                if (node.ParentId == rootId)
                {
                    var treeNode = ConvertToTree(node, nodeDict);
                    tree.Add(treeNode);
                }
            }

            return tree.OrderBy(t => t.Weight ?? 0).ToList();
        }

        /// <summary>
        /// 转换树节点
        /// </summary>
        private Tree<string> ConvertToTree(TreeNode<string> node, Dictionary<string, TreeNode<string>> nodeDict)
        {
            var tree = new Tree<string>
            {
                Id = node.Id,
                ParentId = node.ParentId,
                Title = node.Title,
                Name = node.Extra.ContainsKey("name") ? node.Extra["name"].ToString() : string.Empty,
                Weight = node.Weight,
                Path = node.Extra.ContainsKey("path") ? node.Extra["path"].ToString() : string.Empty,
                Component = node.Extra.ContainsKey("component") ? node.Extra["component"].ToString() : string.Empty,
                Icon = node.Extra.ContainsKey("icon") ? node.Extra["icon"].ToString() : string.Empty,
                Color = node.Extra.ContainsKey("color") ? node.Extra["color"].ToString() : string.Empty,
                Code = node.Extra.ContainsKey("code") ? node.Extra["code"].ToString() : string.Empty,
                Category = node.Extra.ContainsKey("category") ? node.Extra["category"].ToString() : string.Empty,
                MenuType = node.Extra.ContainsKey("menuType") ? node.Extra["menuType"].ToString() : string.Empty,
                Module = node.Extra.ContainsKey("module") ? node.Extra["module"].ToString() : string.Empty,
                Visible = node.Extra.ContainsKey("visible") ? node.Extra["visible"].ToString() : string.Empty,
                DisplayLayout = node.Extra.ContainsKey("displayLayout") ? node.Extra["displayLayout"].ToString() : string.Empty,
                KeepLive = node.Extra.ContainsKey("keepLive") ? node.Extra["keepLive"].ToString() : string.Empty,
                SortCode = node.Extra.ContainsKey("sortCode") ? Convert.ToInt32(node.Extra["sortCode"]) : 0,
                UpdateTime = node.Extra.ContainsKey("updateTime") ? node.Extra["updateTime"].ToString() : string.Empty,
                ExtJson = node.Extra.ContainsKey("extJson") ? node.Extra["extJson"].ToString() : string.Empty
            };

            // 提取meta信息
            if (node.Extra != null && node.Extra.ContainsKey("meta"))
            {
                if (node.Extra["meta"] is Dictionary<string, object> metaDict)
                {
                    tree.Meta = metaDict;
                }
            }

            // 查找子节点
            var children = nodeDict.Values
                .Where(n => n.ParentId == node.Id)
                .OrderBy(n => n.Weight)
                .ToList();

            if (children.Any())
            {
                tree.Children = new List<Tree<string>>();
                foreach (var child in children)
                {
                    var childTree = ConvertToTree(child, nodeDict);
                    tree.Children.Add(childTree);
                }
            }

            return tree;
        }


        #region 
        /// <summary>
        /// 新增用户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [UnitOfWork]
        public async Task<CommonResult<UserDto>> CreateUserAsync(CreateUserDto input)
        {
            try
            {
                var user = ObjectMapper.Map<User>(input);

                user.IsEmailConfirmed = true;

                user.IsActive = true;

                user.IsLockoutEnabled = false;


                await _userManager.CreateAsync(user, input.Password);

                await  CurrentUnitOfWork.SaveChangesAsync();

                var newUser = await Repository.FirstOrDefaultAsync(it => string.Equals(it.UserName,input.UserName));

                long userId = newUser.Id;

                //当在创建用户时不给用户赋予角色时，则进行赋予默认角色
                if (input.RoleNames.Length != 0)
                {
                    CheckErrors(await _userManager.SetRolesAsync(user, input.RoleNames));
                }
                else if (input.RoleNames.Length == 0)
                {
                    var roleIds = await _roleAppService.GetDefaultRoleIdsAsync();                    
                    if (roleIds.Length != 0)
                    {
                        var roleNames = await _roleAppService.GetDefaultRoleNamesAsync();
                        if (roleNames.Length != 0)
                        {
                            CheckErrors(await _userManager.SetRolesAsync(user, roleNames));
                        }
                        var userGrantRoleParam = new SysUserGrantRoleParam()
                        {
                            Id = userId,
                            RoleIdList = roleIds.ToList()
                        };
                        await GrantRole(userGrantRoleParam);
                    }
                }


                await CurrentUnitOfWork.SaveChangesAsync();

                return CommonResultHelper.Create(CommonResult.CODE_SUCCESS, "用户创建成功", MapToEntityDto(user));
            }
            catch (UserFriendlyException ex)
            {
                return CommonResultHelper.Create(CommonResult.CODE_ERROR, ex.Message, default(UserDto));
            }
            catch (Exception ex)
            {
                return CommonResultHelper.Create(CommonResult.CODE_ERROR, "创建用户失败：" + ex.Message + ex.StackTrace, default(UserDto));
            }
        }

        /// <summary>
        /// 根据id修改用户信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<CommonResult<UserDto>> UpdateByIdAsync(UserDto input)
        {
            try
            {
                var user = await _userManager.GetUserByIdAsync(input.Id);
                if (user == null)
                {
                    return CommonResultHelper.Create(404, "用户不存在", default(UserDto));
                }

                MapToEntity(input, user);

                var updateResult = await _userManager.UpdateAsync(user);
                CheckErrors(updateResult);

                if (input.RoleNames != null)
                {
                    var roleResult = await _userManager.SetRolesAsync(user, input.RoleNames);
                    CheckErrors(roleResult);
                }

                return CommonResultHelper.Create(CommonResult.CODE_SUCCESS, "用户更新成功", await GetAsync(input));
            }
            catch (UserFriendlyException ex)
            {
                return CommonResultHelper.Create(CommonResult.CODE_ERROR, ex.Message, default(UserDto));
            }
            catch (Exception ex)
            {
                return CommonResultHelper.Create(CommonResult.CODE_ERROR, "更新用户失败：" + ex.Message, default(UserDto));
            }
        }


        /// <summary>
        /// 当前登录用户修改基础用户信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<CommonResult> UpdateUserInfo(BaseUserInfo input)
        {
            try
            {
                var currentUserId = (long)AbpSession.UserId;
                var user = await _userManager.GetUserByIdAsync(currentUserId);
                if (user == null)
                {
                    return CommonResult.Error("用户不存在");
                }

                user.Name = !string.IsNullOrEmpty(input.Name) ? input.Name : user.Name;
                user.NickName = !string.IsNullOrEmpty(input.NickName) ? input.NickName : user.NickName;

                var updateResult = await _userManager.UpdateAsync(user);
                CheckErrors(updateResult);
                return CommonResult.Ok();
            }
            catch (UserFriendlyException ex)
            {
                return CommonResult.Error("修改失败!" + ex.Message);
            }
            catch (Exception ex)
            {
                return CommonResult.Error("修改失败!" + ex.Message);
            }
        }




        /// <summary>
        /// 根据id删除用户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<CommonResult> DeleteByIdAsync([FromBody] List<SysUserIdParam> input)
        {
            try
            {
                if(input == null || !input.Any())
                {
                    return CommonResult.Error("请选择要删除的数据！");
                }
                var ids = input.Select(x => x.Id).ToList();
                var users = await Repository.GetAll().AsNoTracking().Where(u => ids.Contains(u.Id)).ToListAsync();
                if(users == null || !users.Any())
                {
                    return CommonResult.Error("要删除的用户不存在！");
                }
                
                foreach(var user in users)
                {
                    await _userManager.DeleteAsync(user);
                }
                return CommonResultHelper.Ok("用户删除成功");
            }
            catch (Exception ex)
            {
                return CommonResultHelper.Create(CommonResult.CODE_ERROR, "删除用户失败：" + ex.Message, null);
            }
        }

        /// <summary>
        /// 激活用户
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns></returns>
        [AbpAuthorize]
        public async Task<CommonResult> Activate(EntityDto<long> userDto)
        {
            try
            {
                await Repository.UpdateAsync(userDto.Id, async (entity) =>
                {
                    entity.IsActive = true;
                });

                return CommonResultHelper.Ok("用户激活成功");
            }
            catch (EntityNotFoundException)
            {
                return CommonResultHelper.Create(404, "用户不存在", null);
            }
            catch (Exception ex)
            {
                return CommonResultHelper.Create(CommonResult.CODE_ERROR, "激活用户失败：" + ex.Message, null);
            }
        }

        /// <summary>
        /// 禁用用户
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns></returns>
        [AbpAuthorize]
        public async Task<CommonResult> DeActivate(EntityDto<long> userDto)
        {
            try
            {
                await Repository.UpdateAsync(userDto.Id, async (entity) =>
                {
                    entity.IsActive = false;
                });

                return CommonResultHelper.Ok("用户停用成功");
            }
            catch (EntityNotFoundException)
            {
                return CommonResultHelper.Create(404, "用户不存在", null);
            }
            catch (Exception ex)
            {
                return CommonResultHelper.Create(CommonResult.CODE_ERROR, "停用用户失败：" + ex.Message, null);
            }
        }

        /// <summary>
        /// 获取角色列表
        /// </summary>
        /// <returns></returns>
        public async Task<CommonResult<ListResultDto<RoleDto>>> GetRoles()
        {
            try
            {
                var roles = await _roleRepository.GetAllListAsync();
                var roleDtos = ObjectMapper.Map<List<RoleDto>>(roles);
                var result = new ListResultDto<RoleDto>(roleDtos);

                return CommonResultHelper.Create(CommonResult.CODE_SUCCESS, "获取角色列表成功", result);
            }
            catch (Exception ex)
            {
                return CommonResultHelper.Create(CommonResult.CODE_ERROR, "获取角色列表失败：" + ex.Message, default(ListResultDto<RoleDto>));
            }
        }

        /// <summary>
        /// 变更语言
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<CommonResult> ChangeLanguage(ChangeUserLanguageDto input)
        {
            try
            {
                await SettingManager.ChangeSettingForUserAsync(
                    AbpSession.ToUserIdentifier(),
                    LocalizationSettingNames.DefaultLanguage,
                    input.LanguageName
                );

                return CommonResultHelper.Ok("语言设置更新成功");
            }
            catch (Exception ex)
            {
                return CommonResultHelper.Create(CommonResult.CODE_ERROR, "更新语言设置失败：" + ex.Message, null);
            }
        }



        /// <summary>
        /// 根据id列表获取用户Dto列表
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<CommonResult<List<UserDto>>> GetUserListByIdList([FromBody]List<long> idList)
        {
            if (idList == null || idList.Count == 0)
            {
                return CommonResultHelper.Create(CommonResult.CODE_SUCCESS, "获取用户信息成功", new List<UserDto>());
            }
            try
            {
                var users = await Repository.GetAll()
                    .Where(u => idList.Contains(u.Id))
                    .ToListAsync();
                var userDtos = users.Select(u => ObjectMapper.Map<UserDto>(u)).ToList();
                return CommonResultHelper.Create(CommonResult.CODE_SUCCESS, "获取用户信息成功", userDtos);
            }
            catch (Exception ex)
            {
                return CommonResultHelper.Create(CommonResult.CODE_ERROR, "获取用户信息失败：" + ex.Message, new List<UserDto>());
            }
        }





        /// <summary>
        /// 根据id获取用户Dto
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [DisableAuditing]
        public async Task<CommonResult<UserDto>> GetByIdAsync(EntityDto<long> input)
        {
            try
            {
                var user = await GetEntityByIdAsync(input.Id);
                var userDto = MapToEntityDto(user);
                return CommonResultHelper.Create(CommonResult.CODE_SUCCESS, "获取用户信息成功", userDto);
            }
            catch (EntityNotFoundException)
            {
                return CommonResultHelper.Create(404, "用户不存在", default(UserDto));
            }
            catch (Exception ex)
            {
                return CommonResultHelper.Create(CommonResult.CODE_ERROR, "获取用户信息失败：" + ex.Message, default(UserDto));
            }
        }

        /// <summary>
        /// 分页查询列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [DisableAuditing]
        public async Task<CommonResult<Page<UserDto>>> GetPageListAsync(UserPageParam param)
        {
            try
            {
                var query = this.Repository.GetAll()
                    .WhereIf(param.OrgId.HasValue, x => x.OrgId == param.OrgId)
                    .WhereIf(param.IsActive != null, x => x.IsActive == param.IsActive)
                    .WhereIf(!string.IsNullOrEmpty(param.SearchKey), x =>
                        x.Name.Contains(param.SearchKey) || x.UserName.Contains(param.SearchKey));

                // 排序
                if (!string.IsNullOrEmpty(param.SortField) && !string.IsNullOrEmpty(param.SortOrder))
                {
                    if (param.SortOrder == "ASCEND")
                    {
                        query = query.OrderBy(x => x.CreationTime);
                    }
                    else
                    {
                        query = query.OrderByDescending(x => x.CreationTime);
                    }
                }
                else
                {
                    query = query.OrderBy(x => x.CreationTime);
                }

                var totalCount = await query.CountAsync();
                var items = await query
                    .Skip((param.Current - 1) * param.Size)
                    .Take(param.Size)
                    .ToListAsync();

                // 获取所有相关的ID用于批量查询
                var orgIds = items.Where(x => x.OrgId.HasValue).Select(x => x.OrgId.Value).Distinct().ToList();
                var positionIds = items.Where(x => x.PositionId.HasValue).Select(x => x.PositionId.Value).Distinct().ToList();
                var directorIds = items.Where(x => x.DirectorId.HasValue).Select(x => x.DirectorId.Value).Distinct().ToList();

                // 批量查询关联数据
                var organizations = await _organizationRepository.GetAll()
                    .Where(x => orgIds.Contains(x.Id))
                    .Select(x => new { x.Id, x.Name })
                    .ToDictionaryAsync(x => x.Id, x => x.Name);

                var positions = await _positionRepository.GetAll()
                    .Where(x => positionIds.Contains(x.Id))
                    .Select(x => new { x.Id, x.Name })
                    .ToDictionaryAsync(x => x.Id, x => x.Name);

                var directors = await this.Repository.GetAll()
                    .Where(x => directorIds.Contains(x.Id))
                    .Select(x => new { x.Id, x.Name })
                    .ToDictionaryAsync(x => x.Id, x => x.Name);

                // 转换为DTO并赋值
                var userDtos = new List<UserDto>();
                foreach (var user in items)
                {
                    var dto = ObjectMapper.Map<UserDto>(user);

                    // 设置机构名称
                    if (user.OrgId.HasValue && organizations.TryGetValue(user.OrgId.Value, out var orgName))
                    {
                        dto.OrgName = orgName;
                    }

                    // 设置职位名称
                    if (user.PositionId.HasValue && positions.TryGetValue(user.PositionId.Value, out var positionName))
                    {
                        dto.PositionName = positionName;
                    }

                    // 设置主管名称
                    if (user.DirectorId.HasValue && directors.TryGetValue(user.DirectorId.Value, out var directorName))
                    {
                        dto.DirectorName = directorName;
                    }

                    userDtos.Add(dto);
                }

                var page = new Page<UserDto>
                {
                    Current = param.Current,
                    Size = param.Size,
                    Total = totalCount,
                    Records = userDtos
                };

                return CommonResult<Page<UserDto>>.Success(page);
            }
            catch (Exception ex)
            {
                Logger.Error("获取用户分页失败", ex);
                return CommonResult<Page<UserDto>>.Error("获取用户分页失败");
            }
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [DisableAuditing]
        public async Task<CommonResult> ChangePassword(ChangePasswordDto input)
        {
            try
            {
                await _userManager.InitializeOptionsAsync(AbpSession.TenantId);

                var user = await _userManager.FindByIdAsync(AbpSession.GetUserId().ToString());
                if (user == null)
                {
                    return CommonResultHelper.Create(CommonResult.CODE_ERROR, "当前用户不存在", null);
                }

                if (await _userManager.CheckPasswordAsync(user, input.CurrentPassword))
                {
                    var result = await _userManager.ChangePasswordAsync(user, input.NewPassword);
                    CheckErrors(result);
                    return CommonResultHelper.Ok("密码修改成功");
                }
                else
                {
                    return CommonResultHelper.Create(CommonResult.CODE_ERROR, "当前密码不正确", null);
                }
            }
            catch (UserFriendlyException ex)
            {
                return CommonResultHelper.Create(CommonResult.CODE_ERROR, ex.Message, null);
            }
            catch (Exception ex)
            {
                return CommonResultHelper.Create(CommonResult.CODE_ERROR, "修改密码失败：" + ex.Message, null);
            }
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [DisableAuditing]
        public async Task<CommonResult> ResetPassword(ResetPasswordDto input)
        {
            try
            {
                if (_abpSession.UserId == null)
                {
                    return CommonResultHelper.Create(CommonResult.CODE_ERROR, "请先登录再尝试重置密码", null);
                }

                var currentUser = await _userManager.GetUserByIdAsync(_abpSession.GetUserId());
                var loginAsync = await _logInManager.LoginAsync(currentUser.UserName, input.AdminPassword, shouldLockout: false);
                if (loginAsync.Result != AbpLoginResultType.Success)
                {
                    return CommonResultHelper.Create(CommonResult.CODE_ERROR, "管理员密码不正确，请重试", null);
                }

                if (currentUser.IsDeleted || !currentUser.IsActive)
                {
                    return CommonResultHelper.Create(CommonResult.CODE_ERROR, "当前用户已被删除或停用", null);
                }

                var roles = await _userManager.GetRolesAsync(currentUser);
                if (!roles.Contains(StaticRoleNames.Tenants.Admin))
                {
                    return CommonResultHelper.Create(CommonResult.CODE_ERROR, "只有管理员可以重置密码", null);
                }

                var user = await _userManager.GetUserByIdAsync(input.UserId);
                if (user != null)
                {
                    user.Password = _passwordHasher.HashPassword(user, input.NewPassword);
                    await CurrentUnitOfWork.SaveChangesAsync();
                    return CommonResultHelper.Ok("密码重置成功");
                }
                else
                {
                    return CommonResultHelper.Create(404, "用户不存在", null);
                }
            }
            catch (UserFriendlyException ex)
            {
                return CommonResultHelper.Create(CommonResult.CODE_ERROR, ex.Message, null);
            }
            catch (Exception ex)
            {
                return CommonResultHelper.Create(CommonResult.CODE_ERROR, "重置密码失败：" + ex.Message, null);
            }
        }


        /// <summary>
        /// 统计业务数据
        /// </summary>
        /// <returns></returns>
        [DisableAuditing]
        public async Task<CommonResult<BizDataCountDto>> GetBizDataCount()
        {
            try
            {
                var orgCount = await _organizationRepository.GetAll().AsNoTracking().CountAsync();
                var positionCount = await _positionRepository.GetAll().AsNoTracking().CountAsync();
                var userCount = await this.Repository.GetAll().AsNoTracking().CountAsync();
                var roleCount = await _roleRepository.GetAll().AsNoTracking().CountAsync();
                var result = new BizDataCountDto
                {
                    OrgCount = orgCount,
                    PositionCount = positionCount,
                    UserCount = userCount,
                    RoleCount = roleCount
                };

                return CommonResult<BizDataCountDto>.Success(result);
            }
            catch (Exception ex)
            {
                return CommonResult<BizDataCountDto>.Error("获取业务数据统计失败：" + ex.Message);
            }
        }


        /// <summary>
        /// 获取用户选择器
        /// </summary>
        /// <param name="param">查询参数</param>
        /// <returns>用户选择器结果</returns>
        [DisableAuditing]
        public async Task<CommonResult<Page<UserSelectorDto>>> GetUserSelector(UserSelectorParam param)
        {
            try
            {
                // 如果查询条件为空，则直接查询所有启用的用户
                if (param.OrgId == null && string.IsNullOrWhiteSpace(param.SearchKey))
                {
                    return await GetAllUserSelectorList(param);
                }
                else
                {
                    return await GetFilteredUserSelectorList(param);
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"获取用户选择器失败，参数: {JsonConvert.SerializeObject(param)}", ex);
                return CommonResult<Page<UserSelectorDto>>.Error("获取用户选择器失败：" + ex.Message);
            }
        }



        /// <summary>
        /// 获取用户列表（不分页，简化查询）
        /// </summary>
        /// <returns>用户选择器结果</returns>
        [DisableAuditing]
        public async Task<CommonResult<List<UserSelectorDto>>> GetUserList()
        {
            try
            {
                var userList = await   Repository.GetAll().AsNoTracking()
                    .Where(x => x.IsActive) // 只查询状态为正常的用户
                    .Select(x => new UserSelectorDto
                    {
                        Id = x.Id,
                        OrgId = x.OrgId,
                        PositionId = x.PositionId,
                        UserName = x.UserName,
                        Name = x.Name
                    })
                    .ToListAsync();

                return CommonResult.Ok(userList);
            }
            catch (Exception ex)
            {
                Logger.Error($"获取用户列表失败", ex);
                return CommonResult<List<UserSelectorDto>>.Error("获取用户列表失败：" + ex.Message);
            }
        }






        /// <summary>
        /// 获取所有用户选择器列表（简化查询）
        /// </summary>
        [DisableAuditing]
        private async Task<CommonResult<Page<UserSelectorDto>>> GetAllUserSelectorList(UserSelectorParam param)
        {
            var query = Repository.GetAll()
                .Where(x => x.IsActive) // 只查询状态为正常的用户
                .OrderBy(x => x.CreationTime);

            var totalCount = await query.CountAsync();

            var items = await query
                .Select(x => new UserSelectorDto
                {
                    Id = x.Id,
                    OrgId = x.OrgId,
                    PositionId = x.PositionId,
                    UserName = x.UserName,
                    Name = x.Name
                })
                .Skip((param.Current - 1) * param.Size)
                .Take(param.Size)
                .ToListAsync();


            var page = new Page<UserSelectorDto>
            {
                Current = param.Current,
                Size = param.Size,
                Total = totalCount,
                Records = items
            };

            return CommonResult<Page<UserSelectorDto>>.Success(page);
        }

        /// <summary>
        /// 获取筛选后的用户选择器列表
        /// </summary>
        private async Task<CommonResult<Page<UserSelectorDto>>> GetFilteredUserSelectorList(UserSelectorParam param)
        {
            var query = Repository.GetAll()
                .Where(x => x.IsActive); // 只查询状态为正常的用户

            // 如果组织id不为空，则查询该组织及其子组织下的所有人
            if (param.OrgId.HasValue)
            {
                // 获取该组织及其所有子组织的ID列表
                var childOrgIds = await GetChildOrgIds(param.OrgId.Value);
                if (childOrgIds.Any())
                {
                    query = query.Where(x => x.OrgId.HasValue && childOrgIds.Contains(x.OrgId.Value));
                }
                else
                {
                    // 如果没有子组织，返回空结果
                    return CommonResult<Page<UserSelectorDto>>.Success(new Page<UserSelectorDto>
                    {
                        Current = param.Current,
                        Size = param.Size,
                        Total = 0,
                        Records = new List<UserSelectorDto>()
                    });
                }
            }

            // 搜索关键词过滤
            if (!string.IsNullOrWhiteSpace(param.SearchKey))
            {
                query = query.Where(x => x.Name.Contains(param.SearchKey) || x.UserName.Contains(param.SearchKey));
            }

            // 排序
            query = query.OrderBy(x => x.CreationTime);

            var totalCount = await query.CountAsync();

            var items = await query
                .Select(x => new UserSelectorDto
                {
                    Id = x.Id,
                    OrgId = x.OrgId,
                    PositionId = x.PositionId,
                    UserName = x.UserName,
                    Name = x.Name
                })
                .Skip((param.Current - 1) * param.Size)
                .Take(param.Size)
                .ToListAsync();

            var page = new Page<UserSelectorDto>
            {
                Current = param.Current,
                Size = param.Size,
                Total = totalCount,
                Records = items
            };

            return CommonResult<Page<UserSelectorDto>>.Success(page);
        }

        /// <summary>
        /// 获取组织及其所有子组织的ID列表
        /// </summary>
        private async Task<List<Guid>> GetChildOrgIds(Guid orgId)
        {
            // 获取所有组织
            var allOrgs = await _organizationRepository.GetAll().ToListAsync();

            // 递归查找子组织
            var result = new List<Guid> { orgId };
            FindChildOrgIds(orgId, allOrgs, result);

            return result;
        }

        /// <summary>
        /// 递归查找子组织ID
        /// </summary>
        private void FindChildOrgIds(Guid parentId, List<Organization> allOrgs, List<Guid> result)
        {
            var childOrgs = allOrgs.Where(x => x.ParentId == parentId).ToList();
            foreach (var child in childOrgs)
            {
                if (!result.Contains(child.Id))
                {
                    result.Add(child.Id);
                    FindChildOrgIds(child.Id, allOrgs, result);
                }
            }
        }




        /// <summary>
        /// 实体转换
        /// </summary>
        /// <param name="createInput"></param>
        /// <returns></returns>
        protected override User MapToEntity(CreateUserDto createInput)
        {
            var user = ObjectMapper.Map<User>(createInput);
            user.SetNormalizedNames();
            return user;
        }

        /// <summary>
        /// 实体转换
        /// </summary>
        /// <param name="input"></param>
        /// <param name="user"></param>
        protected override void MapToEntity(UserDto input, User user)
        {
            ObjectMapper.Map(input, user);
            user.SetNormalizedNames();
        }
        /// <summary>
        /// 实体转换成数据Dto
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>

        protected override UserDto MapToEntityDto(User user)
        {
            var roleIds = user.Roles.Select(x => x.RoleId).ToArray();
            var roles = _roleManager.Roles.Where(r => roleIds.Contains(r.Id)).Select(r => r.NormalizedName);

            var userDto = base.MapToEntityDto(user);
            userDto.RoleNames = roles.ToArray();

            return userDto;
        }

        /// <summary>
        /// 创建查询过滤条件
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        protected override IQueryable<User> CreateFilteredQuery(PagedUserResultRequestDto input)
        {
            return Repository.GetAllIncluding(x => x.Roles)
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.UserName.Contains(input.Keyword) || x.Name.Contains(input.Keyword) || x.EmailAddress.Contains(input.Keyword))
                .WhereIf(input.IsActive.HasValue, x => x.IsActive == input.IsActive);
        }

        /// <summary>
        /// 根据id获取用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected override async Task<User> GetEntityByIdAsync(long id)
        {
            var user = await Repository.GetAllIncluding(x => x.Roles).FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                throw new EntityNotFoundException(typeof(User), id);
            }

            return user;
        }

        /// <summary>
        /// 根据用户名排序
        /// </summary>
        /// <param name="query"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        protected override IQueryable<User> ApplySorting(IQueryable<User> query, PagedUserResultRequestDto input)
        {
            return query.OrderBy(r => r.UserName);
        }

        /// <summary>
        /// 检查错误
        /// </summary>
        /// <param name="identityResult"></param>
        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
        #endregion
    }
}