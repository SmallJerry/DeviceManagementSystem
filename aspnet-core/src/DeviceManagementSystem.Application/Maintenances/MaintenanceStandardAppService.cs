using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.UI;
using DeviceManagementSystem.Maintenances.Dto;
using DeviceManagementSystem.Maintenances.Interface;
using DeviceManagementSystem.Utils.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Maintenances
{
    /// <summary>
    /// 保养标准服务实现
    /// </summary>
    public class MaintenanceStandardAppService : DeviceManagementSystemAppServiceBase, IMaintenanceStandardAppService
    {
        private readonly IRepository<MaintenanceStandards, Guid> _standardRepository;


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="standardRepository"></param>
        public MaintenanceStandardAppService(
            IRepository<MaintenanceStandards, Guid> standardRepository)
        {
            _standardRepository = standardRepository;
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        public async Task<CommonResult<Page<MaintenanceStandardDto>>> GetPageList([FromQuery] MaintenanceStandardPageInput input)
        {
            try
            {
                if (input.Size > 100) input.Size = 100;

                var query = _standardRepository.GetAll().AsNoTracking();

                // 应用过滤条件
                if (!string.IsNullOrWhiteSpace(input.SearchKey))
                {
                    query = query.Where(x =>
                        x.PointName.Contains(input.SearchKey) ||
                        x.InspectionContent.Contains(input.SearchKey));
                }

                if (!string.IsNullOrWhiteSpace(input.PointType))
                {
                    query = query.Where(x => x.PointType == input.PointType);
                }

                var total = await query.CountAsync();

                var items = await query
                    .OrderBy(x => x.PointType)
                    .Skip((input.Current - 1) * input.Size)
                    .Take(input.Size)
                    .ToListAsync();

                var result = items.Select(x => new MaintenanceStandardDto
                {
                    Id = x.Id,
                    PointName = x.PointName,
                    PointType = x.PointType,
                    InspectionContent = x.InspectionContent,
                    InspectionMethod = x.InspectionMethod,
                    Remark = x.Remark,
                    CreationTime = x.CreationTime
                }).ToList();

                var page = new Page<MaintenanceStandardDto>(input.Current, input.Size, total)
                {
                    Records = result
                };

                return CommonResult<Page<MaintenanceStandardDto>>.Success(page);
            }
            catch (Exception ex)
            {
                Logger.Error("获取保养标准分页列表失败", ex);
                return CommonResult<Page<MaintenanceStandardDto>>.Error($"获取保养标准分页列表失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取详情
        /// </summary>
        public async Task<CommonResult<MaintenanceStandardDto>> GetById(Guid id)
        {
            try
            {
                var standard = await _standardRepository.FirstOrDefaultAsync(id);
                if (standard == null)
                {
                    return CommonResult<MaintenanceStandardDto>.Error("保养标准不存在");
                }

                var dto = new MaintenanceStandardDto
                {
                    Id = standard.Id,
                    PointName = standard.PointName,
                    PointType = standard.PointType,
                    InspectionContent = standard.InspectionContent,
                    InspectionMethod = standard.InspectionMethod,
                    Remark = standard.Remark,
                    CreationTime = standard.CreationTime
                };

                return CommonResult<MaintenanceStandardDto>.Success(dto);
            }
            catch (Exception ex)
            {
                Logger.Error("获取保养标准详情失败", ex);
                return CommonResult<MaintenanceStandardDto>.Error($"获取保养标准详情失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 检查是否存在重复的保养标准
        /// </summary>
        private async Task<bool> IsDuplicateAsync(string pointName, string pointType, string inspectionContent, Guid? excludeId = null)
        {
            var query = _standardRepository.GetAll()
                .Where(x => x.PointName == pointName &&
                           x.PointType == pointType &&
                           x.InspectionContent == inspectionContent);

            if (excludeId.HasValue)
            {
                query = query.Where(x => x.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        /// <summary>
        /// 创建
        /// </summary>
        [UnitOfWork]
        public async Task<CommonResult<Guid>> Create(MaintenanceStandardInput input)
        {
            try
            {
                // 检查唯一性
                var isDuplicate = await IsDuplicateAsync(input.PointName, input.PointType, input.InspectionContent);
                if (isDuplicate)
                {
                    return CommonResult<Guid>.Error("已存在相同的保养标准记录");
                }

                var standard = new MaintenanceStandards
                {
                    PointName = input.PointName,
                    PointType = input.PointType,
                    InspectionContent = input.InspectionContent,
                    Remark = input.Remark
                };

                // 设置点检方法（自动序列化为JSON）
                standard.InspectionMethod = input.InspectionMethod ?? new List<string>();

                var id = await _standardRepository.InsertAndGetIdAsync(standard);
                await CurrentUnitOfWork.SaveChangesAsync();

                return CommonResult<Guid>.Success(id);
            }
            catch (Exception ex)
            {
                Logger.Error("创建保养标准失败", ex);
                return CommonResult<Guid>.Error($"创建保养标准失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        [UnitOfWork]
        public async Task<CommonResult> Update(MaintenanceStandardInput input)
        {
            try
            {
                if (!input.Id.HasValue)
                {
                    return CommonResult.Error("ID不能为空");
                }

                // 检查唯一性（排除自身）
                var isDuplicate = await IsDuplicateAsync(input.PointName, input.PointType, input.InspectionContent, input.Id.Value);
                if (isDuplicate)
                {
                    return CommonResult.Error("已存在相同的保养标准记录");
                }

                var standard = await _standardRepository.FirstOrDefaultAsync(input.Id.Value);
                if (standard == null)
                {
                    return CommonResult.Error("保养标准不存在");
                }

                standard.PointName = input.PointName;
                standard.PointType = input.PointType;
                standard.InspectionContent = input.InspectionContent;
                standard.InspectionMethod = input.InspectionMethod ?? new List<string>();
                standard.Remark = input.Remark;

                await _standardRepository.UpdateAsync(standard);
                await CurrentUnitOfWork.SaveChangesAsync();

                return CommonResult.Ok("更新成功");
            }
            catch (Exception ex)
            {
                Logger.Error("更新保养标准失败", ex);
                return CommonResult.Error($"更新保养标准失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        public async Task<CommonResult> Delete(Guid id)
        {
            try
            {
                await _standardRepository.DeleteAsync(id);
                return CommonResult.Ok("删除成功");
            }
            catch (Exception ex)
            {
                Logger.Error("删除保养标准失败", ex);
                return CommonResult.Error($"删除保养标准失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        public async Task<CommonResult> BatchDelete([FromBody] List<Guid> ids)
        {
            try
            {
                if (ids == null || ids.Count == 0)
                {
                    return CommonResult.Error("请选择要删除的保养标准");
                }

                foreach (var id in ids)
                {
                    await _standardRepository.DeleteAsync(id);
                }

                return CommonResult.Ok("批量删除成功");
            }
            catch (Exception ex)
            {
                Logger.Error("批量删除保养标准失败", ex);
                return CommonResult.Error($"批量删除保养标准失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取所有点检部位
        /// </summary>
        public async Task<CommonResult<List<string>>> GetPointTypes()
        {
            try
            {
                var pointTypes = await _standardRepository.GetAll()
                    .Where(x => !string.IsNullOrEmpty(x.PointType))
                    .Select(x => x.PointType)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToListAsync();

                return CommonResult<List<string>>.Success(pointTypes);
            }
            catch (Exception ex)
            {
                Logger.Error("获取点检部位列表失败", ex);
                return CommonResult<List<string>>.Error($"获取点检部位列表失败: {ex.Message}");
            }
        }


        /// <summary>
        /// 下载导入模板
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> DownloadImportTemplate()
        {
            try
            {
                // 创建工作簿
                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("印刷机");

                // 创建标题行样式
                ICellStyle headerStyle = workbook.CreateCellStyle();
                headerStyle.FillPattern = FillPattern.SolidForeground;
                headerStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Grey25Percent.Index;
                var headerFont = workbook.CreateFont();
                headerFont.IsBold = true;
                headerStyle.SetFont(headerFont);

                // 创建标题行
                string[] headers = { "NO.", "点    检    项   目", "", "", "点 检 内  容", "点检方法", "点检结果", "备注" };
                IRow headerRow = sheet.CreateRow(0);
                for (int i = 0; i < headers.Length; i++)
                {
                    ICell cell = headerRow.CreateCell(i);
                    cell.SetCellValue(headers[i]);
                    cell.CellStyle = headerStyle;
                }

                // 合并单元格（处理点检项目列）
                sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 1, 3));

                // 设置列宽
                sheet.SetColumnWidth(0, 5 * 256);   // NO.
                sheet.SetColumnWidth(1, 15 * 256);  // 点检项目（部位）
                sheet.SetColumnWidth(2, 8 * 256);   // 序号
                sheet.SetColumnWidth(3, 20 * 256);  // 点检项目名称
                sheet.SetColumnWidth(4, 40 * 256);  // 点检内容
                sheet.SetColumnWidth(5, 15 * 256);  // 点检方法
                sheet.SetColumnWidth(6, 10 * 256);  // 点检结果
                sheet.SetColumnWidth(7, 20 * 256);  // 备注

                // 添加示例数据
                var exampleData = new[]
                {
            new object[] { "1", "供气装置", "①", "主气压", "主气压表确认0.5MPa～0.60MPa", "目测", "", "" },
            new object[] { "2", "钢网清洗装置", "①", "真空吸嘴", "清洁吸嘴表面并确认升降是否顺畅", "清洁、目测", "", "" },
            new object[] { "", "", "②", "真空、过滤网", "过滤网清洁并确认真空是否正常", "清洁、目测", "", "" },
            new object[] { "3", "喷液装置", "①", "喷头", "确认喷液是否正常", "目测", "", "" },
            new object[] { "4", "刮刀升降装置", "①", "刮刀升降装置", "升降是否正常", "目测", "", "" },
            new object[] { "", "", "②", "刮刀", "刮刀是否完整，刮刀片有无异常", "目测", "", "" },
            new object[] { "5", "视觉装置", "①", "相机表面及镜片", "是否有损伤，污迹，棉布清洁", "清洁、目测", "", "" },
            new object[] { "6", "传送单元", "①", "基板 Support 平台", "清洁、去除异物", "清洁、目测", "", "" },
            new object[] { "", "", "②", "基板传送皮带", "是否有损伤，污迹", "目测", "", "" },
        };

                for (int row = 0; row < exampleData.Length; row++)
                {
                    IRow dataRow = sheet.CreateRow(row + 1);
                    var rowData = exampleData[row];
                    for (int col = 0; col < rowData.Length; col++)
                    {
                        ICell cell = dataRow.CreateCell(col);
                        if (rowData[col] != null)
                        {
                            cell.SetCellValue(rowData[col].ToString());
                        }
                    }
                }

                // 添加说明行
                IRow noteRow = sheet.CreateRow(exampleData.Length + 2);
                noteRow.CreateCell(0).SetCellValue("说明：");
                noteRow.CreateCell(1).SetCellValue("1. 点检方法多个时请用顿号（、）或逗号（,）分隔");

                IRow noteRow2 = sheet.CreateRow(exampleData.Length + 3);
                noteRow2.CreateCell(1).SetCellValue("2. 序号列使用符号（①、②、③...）");

                IRow noteRow3 = sheet.CreateRow(exampleData.Length + 4);
                noteRow3.CreateCell(1).SetCellValue("3. 点检项目、点检内容、点检方法为必填项");

                // 写入流
                using (var stream = new MemoryStream())
                {
                    workbook.Write(stream);
                    var bytes = stream.ToArray();
                    return
                         new FileContentResult(bytes, "application/octet-stream")
                         {
                             FileDownloadName = "印刷机系列保养记录模板.xlsx"
                         };
                }
            }
            catch (Exception ex)
            {
                Logger.Error("下载导入模板失败", ex);
                throw new UserFriendlyException("下载导入模板失败");
            }
        }

        /// <summary>
        /// 获取合并单元格的值
        /// </summary>
        private string GetMergedCellValue(ISheet sheet, int rowIndex, int cellIndex)
        {
            // 先获取单元格本身的值
            IRow row = sheet.GetRow(rowIndex);
            string cellValue = GetCellValue(row?.GetCell(cellIndex));

            // 如果单元格有值，直接返回
            if (!string.IsNullOrWhiteSpace(cellValue))
            {
                return cellValue;
            }

            // 查找合并区域
            for (int i = 0; i < sheet.NumMergedRegions; i++)
            {
                var region = sheet.GetMergedRegion(i);
                // 检查当前行是否在合并区域内，并且列索引在合并区域内
                if (region.FirstRow <= rowIndex && region.LastRow >= rowIndex &&
                    region.FirstColumn <= cellIndex && region.LastColumn >= cellIndex)
                {
                    // 获取合并区域的第一个单元格的值
                    IRow firstRow = sheet.GetRow(region.FirstRow);
                    if (firstRow != null)
                    {
                        ICell firstCell = firstRow.GetCell(region.FirstColumn);
                        return GetCellValue(firstCell);
                    }
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// 批量导入
        /// </summary>
        [UnitOfWork]
        public async Task<CommonResult<int>> BatchImport(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return CommonResult<int>.Error("请上传文件");
                }

                var standards = new List<MaintenanceStandards>();
                var errors = new List<string>();
                var duplicateInfos = new List<string>();
                int successCount = 0;
                int duplicateCount = 0;

                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    stream.Position = 0;

                    IWorkbook workbook;
                    if (file.FileName.EndsWith(".xlsx"))
                    {
                        workbook = new XSSFWorkbook(stream);
                    }
                    else if (file.FileName.EndsWith(".xls"))
                    {
                        workbook = new HSSFWorkbook(stream);
                    }
                    else
                    {
                        return CommonResult<int>.Error("不支持的文件格式，请上传.xlsx或.xls文件");
                    }

                    // 获取第一个工作表
                    ISheet sheet = workbook.GetSheetAt(0);
                    if (sheet == null)
                    {
                        return CommonResult<int>.Error("工作表不存在");
                    }

                    string currentPointType = string.Empty;
                    string currentPointNo = string.Empty;

                    // 从第2行开始读取（跳过标题行）
                    for (int rowIdx = 1; rowIdx <= sheet.LastRowNum; rowIdx++)
                    {
                        IRow row = sheet.GetRow(rowIdx);
                        if (row == null) continue;

                        // 获取合并单元格的值
                        string pointType = GetMergedCellValue(sheet, rowIdx, 1);     // 点检部位（供气装置）

                        // 如果当前行没有点检部位，则沿用上一行的值
                        if (!string.IsNullOrWhiteSpace(pointType))
                        {
                            currentPointType = pointType;
                        }
                        else
                        {
                            pointType = currentPointType;
                        }

                        string pointNo = GetMergedCellValue(sheet, rowIdx, 2);       // 序号（①）
                        if (!string.IsNullOrWhiteSpace(pointNo))
                        {
                            currentPointNo = pointNo;
                        }
                        else
                        {
                            pointNo = currentPointNo;
                        }

                        string pointName = GetMergedCellValue(sheet, rowIdx, 3);     // 点检项目名称（主气压）
                        string inspectionContent = GetMergedCellValue(sheet, rowIdx, 4); // 点检内容
                        string inspectionMethodStr = GetMergedCellValue(sheet, rowIdx, 5); // 点检方法
                        string remark = GetMergedCellValue(sheet, rowIdx, 7);        // 备注

                        // 跳过空行（点检项目名称、点检内容、点检方法都为空）
                        if (string.IsNullOrWhiteSpace(pointName) &&
                            string.IsNullOrWhiteSpace(inspectionContent) &&
                            string.IsNullOrWhiteSpace(inspectionMethodStr))
                        {
                            continue;
                        }

                        // 验证必填项
                        var rowErrors = new List<string>();
                        if (string.IsNullOrWhiteSpace(pointName))
                            rowErrors.Add("点检项目名称为空");
                        if (string.IsNullOrWhiteSpace(inspectionContent))
                            rowErrors.Add("点检内容为空");
                        if (string.IsNullOrWhiteSpace(inspectionMethodStr))
                            rowErrors.Add("点检方法为空");

                        if (rowErrors.Any())
                        {
                            errors.Add($"第{rowIdx + 1}行: {string.Join("; ", rowErrors)}");
                            continue;
                        }

                        // 如果点检部位为空，使用默认值
                        if (string.IsNullOrWhiteSpace(pointType))
                        {
                            pointType = "其他";
                        }

                        // 解析点检方法（支持顿号、逗号分隔）
                        var inspectionMethod = inspectionMethodStr
                            .Split(new[] { '、', ',' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(m => m.Trim())
                            .Where(m => !string.IsNullOrEmpty(m))
                            .ToList();

                        // 检查是否存在重复数据
                        bool isDuplicate = await IsDuplicateAsync(pointName, pointType, inspectionContent);

                        if (isDuplicate)
                        {
                            duplicateCount++;
                            duplicateInfos.Add($"第{rowIdx + 1}行: 点检部位【{pointType}】、点检项目【{pointName}】、点检内容【{inspectionContent}】已存在，已跳过");
                            continue;
                        }

                        var standard = new MaintenanceStandards
                        {
                            PointName = pointName,
                            PointType = pointType,
                            InspectionContent = inspectionContent,
                            Remark = remark
                        };
                        standard.InspectionMethod = inspectionMethod;

                        standards.Add(standard);
                        successCount++;
                    }
                }

                // 批量保存
                if (standards.Any())
                {
                    foreach (var standard in standards)
                    {
                        await _standardRepository.InsertAsync(standard);
                    }
                    await CurrentUnitOfWork.SaveChangesAsync();
                }

                // 构建返回消息
                var message = new StringBuilder();
                message.Append($"成功导入 {successCount} 条数据");

                if (duplicateCount > 0)
                {
                    message.Append($"，跳过 {duplicateCount} 条重复数据");
                }

                if (errors.Any())
                {
                    message.Append($"，{errors.Count} 条数据格式错误");
                    return CommonResult<int>.Error($"导入完成，但存在以下问题：\n{message}\n\n详细错误：\n{string.Join("\n", errors)}\n\n重复数据：\n{string.Join("\n", duplicateInfos)}");
                }

                if (duplicateInfos.Any())
                {
                    return CommonResult<int>.Success($"导入完成：{message}", successCount);
                }

                return CommonResult<int>.Success(message.ToString(), successCount);
            }
            catch (Exception ex)
            {
                Logger.Error("批量导入保养标准失败", ex);
                return CommonResult<int>.Error($"导入失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取单元格值
        /// </summary>
        private string GetCellValue(ICell cell)
        {
            if (cell == null) return string.Empty;

            switch (cell.CellType)
            {
                case CellType.String:
                    return cell.StringCellValue?.Trim() ?? string.Empty;
                case CellType.Numeric:
                    if (DateUtil.IsCellDateFormatted(cell))
                    {
                        return cell.DateCellValue.ToString();
                    }
                    return cell.NumericCellValue.ToString();
                case CellType.Boolean:
                    return cell.BooleanCellValue.ToString();
                case CellType.Formula:
                    try
                    {
                        return cell.StringCellValue;
                    }
                    catch
                    {
                        return cell.CellFormula;
                    }
                default:
                    return string.Empty;
            }
        }
    }
}