using Abp.Auditing;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using DeviceManagementSystem.Attachment.Dto;
using DeviceManagementSystem.FileInfos;
using DeviceManagementSystem.Users;
using DeviceManagementSystem.Utils.Common;
using EFCore.BulkExtensions;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Attachment
{
    /// <summary>
    /// 附件管理服务
    /// </summary>
    [AbpAuthorize]
    [DisableAuditing]
    public class AttachmentAppService : DeviceManagementSystemAppServiceBase, IAttachmentAppService
    {
        private readonly IRepository<Attachments, Guid> _attachmentRepository;
        private readonly IRepository<FileChunks, Guid> _fileChunkRepository;
        private readonly IRepository<BusinessAttachmentRelations, Guid> _relationRepository;
        private readonly IUserAppService _userAppService;

        // 分片大小 5MB
        private const long CHUNK_SIZE = 5 * 1024 * 1024;
        // 最大文件大小 500MB
        private const long MAX_FILE_SIZE = 500 * 1024 * 1024;
        // 附件类型常量
        private const int ATTACHMENT_TYPE_FILE = 0; // 本地文件
        private const int ATTACHMENT_TYPE_LINK = 1; // 链接地址


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="attachmentRepository"></param>
        /// <param name="fileChunkRepository"></param>
        /// <param name="relationRepository"></param>
        /// <param name="userAppService"></param>
        public AttachmentAppService(
            IRepository<Attachments, Guid> attachmentRepository,
            IRepository<FileChunks, Guid> fileChunkRepository,
            IRepository<BusinessAttachmentRelations, Guid> relationRepository,
            IUserAppService userAppService)
        {
            _attachmentRepository = attachmentRepository;
            _fileChunkRepository = fileChunkRepository;
            _relationRepository = relationRepository;
            _userAppService = userAppService;
        }

        #region 文件上传相关

        /// <summary>
        /// 检查文件上传状态
        /// </summary>
        [HttpPost]
        public async Task<CommonResult<CheckUploadResult>> CheckUpload(CheckUploadInput input)
        {
            try
            {
                var result = new CheckUploadResult
                {
                    Uploaded = false,
                    UploadedChunks = new int[0]
                };

                // 检查文件大小
                if (input.TotalSize > MAX_FILE_SIZE)
                {
                    return CommonResult<CheckUploadResult>.Error($"文件大小不能超过500MB");
                }

                // 查询附件表
                var attachment = await _attachmentRepository.FirstOrDefaultAsync(x => x.FileIdentifier == input.FileIdentifier);

                // 文件已完整上传
                if (attachment != null && attachment.UploadStatus == 1)
                {
                    result.Uploaded = true;
                    result.AttachmentId = attachment.Id;
                    result.Url = GetFileUrl(attachment.FilePath);
                    return CommonResult<CheckUploadResult>.Success(result);
                }

                // 查询分片记录
                var chunks = await _fileChunkRepository.GetAll()
                    .Where(x => x.FileIdentifier == input.FileIdentifier)
                    .OrderBy(x => x.ChunkNumber)
                    .ToListAsync();

                if (chunks.Any())
                {
                    result.UploadedChunks = chunks.Select(x => x.ChunkNumber).ToArray();

                    // 如果分片数量等于总分片数，触发合并
                    if (chunks.Count == input.TotalChunks)
                    {
                        var mergeResult = await MergeFile(input.FileIdentifier, input.FileName, input.TotalSize, input.TotalChunks);
                        if (mergeResult.IsSuccess)
                        {
                            result.Uploaded = true;
                            result.AttachmentId = mergeResult.Data;
                            result.Url = GetFileUrl(await GetAttachmentPath(mergeResult.Data));
                        }
                    }
                }

                return CommonResult<CheckUploadResult>.Success(result);
            }
            catch (Exception ex)
            {
                Logger.Error("检查文件上传状态失败", ex);
                return CommonResult<CheckUploadResult>.Error($"检查文件上传状态失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 上传文件分片
        /// </summary>
        [HttpPost]
        public async Task<CommonResult<object>> ChunkUpload([FromForm] FileChunkParam input)
        {
            try
            {
                if (input.File == null || input.File.Length == 0)
                {
                    return CommonResult.Error("上传文件不能为空");
                }

                // 验证文件大小
                if (input.TotalSize > MAX_FILE_SIZE)
                {
                    return CommonResult.Error($"文件大小不能超过500MB");
                }

                // 单文件上传(不分片)
                if (input.TotalChunks == 1)
                {
                    return await UploadSingleFile(input);
                }

                // 分片上传
                return await UploadChunkFile(input);
            }
            catch (Exception ex)
            {
                Logger.Error("文件上传失败", ex);
                return CommonResult.Error($"文件上传失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 上传单个文件
        /// </summary>
        private async Task<CommonResult<object>> UploadSingleFile(FileChunkParam input)
        {
            string fileDir = GetFileDir();
            string fileName = $"{input.FileIdentifier}{Path.GetExtension(input.FileName)}";
            string filePath = Path.Combine(fileDir, fileName);

            try
            {
                // 检查文件是否已存在
                var existingAttachment = await _attachmentRepository.FirstOrDefaultAsync(x => x.FileIdentifier == input.FileIdentifier);
                if (existingAttachment != null)
                {
                    return CommonResult<object>.Success(new { attachmentId = existingAttachment.Id });
                }

                // 保存文件
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await input.File.CopyToAsync(stream);
                }

                // 创建附件记录
                var creatorUser = await _userAppService.GetNameByUserId(AbpSession.UserId.Value);
                var attachment = new Attachments
                {
                    FileIdentifier = input.FileIdentifier,
                    FileName = input.FileName,
                    FileExtension = Path.GetExtension(input.FileName),
                    FileSize = input.TotalSize,
                    FileSizeFormat = GetReadableFileSize(input.TotalSize),
                    FilePath = filePath,
                    FileType = input.FileType,
                    TotalChunks = 1,
                    UploadStatus = 1,
                    AttachmentType = ATTACHMENT_TYPE_FILE
                };

                var attachmentId = await _attachmentRepository.InsertAndGetIdAsync(attachment);
                await CurrentUnitOfWork.SaveChangesAsync();

                return CommonResult<object>.Success(new { attachmentId });
            }
            catch (Exception ex)
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                Logger.Error("单文件上传失败", ex);
                return CommonResult.Error($"文件上传失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 上传分片文件
        /// </summary>
        private async Task<CommonResult<object>> UploadChunkFile(FileChunkParam input)
        {
            string tempDir = Path.Combine(GetTempFileDir(), input.FileIdentifier);

            try
            {
                // 创建临时目录
                if (!Directory.Exists(tempDir))
                {
                    Directory.CreateDirectory(tempDir);
                }

                // 验证分片大小
                if (input.File.Length != input.CurrentChunkSize)
                {
                    return CommonResult.Error($"分片大小不匹配: 预期 {input.CurrentChunkSize}, 实际 {input.File.Length}");
                }

                // 保存分片
                string chunkFilePath = Path.Combine(tempDir, input.ChunkNumber.ToString());

                // 使用 FileMode.Create 确保覆盖已存在的文件
                using (var stream = new FileStream(chunkFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await input.File.CopyToAsync(stream);
                    await stream.FlushAsync();
                }

                // 验证保存后的文件大小
                var savedFileInfo = new FileInfo(chunkFilePath);
                if (savedFileInfo.Length != input.CurrentChunkSize)
                {
                    return CommonResult.Error($"分片保存后大小不匹配: 预期 {input.CurrentChunkSize}, 实际 {savedFileInfo.Length}");
                }

                // 检查分片是否已存在
                var existingChunk = await _fileChunkRepository.FirstOrDefaultAsync(x =>
                    x.FileIdentifier == input.FileIdentifier && x.ChunkNumber == input.ChunkNumber);

                if (existingChunk != null)
                {
                    // 更新现有记录
                    existingChunk.CurrentChunkSize = input.CurrentChunkSize;
                    existingChunk.IsUploaded = true;
                    await _fileChunkRepository.UpdateAsync(existingChunk);
                }
                else
                {
                    // 保存分片记录
                    var fileChunk = new FileChunks
                    {
                        FileIdentifier = input.FileIdentifier,
                        ChunkNumber = input.ChunkNumber,
                        ChunkSize = input.ChunkSize,
                        CurrentChunkSize = input.CurrentChunkSize,
                        TotalChunks = input.TotalChunks,
                        FileName = input.FileName,
                        ChunkPath = chunkFilePath,
                        IsUploaded = true
                    };
                    await _fileChunkRepository.InsertAsync(fileChunk);
                }

                await CurrentUnitOfWork.SaveChangesAsync();

                // 检查是否所有分片都上传完成
                if (input.ChunkNumber == input.TotalChunks)
                {
                    // 等待一小段时间确保所有分片都已写入
                    await Task.Delay(100);

                    // 触发合并
                    var mergeResult = await MergeFile(input.FileIdentifier, input.FileName, input.TotalSize, input.TotalChunks);
                    if (mergeResult.IsSuccess)
                    {
                        // 删除分片目录和记录
                        await CleanupChunks(input.FileIdentifier);
                        return CommonResult<object>.Success(new { attachmentId = mergeResult.Data });
                    }
                    else
                    {
                        return CommonResult.Error(mergeResult.Message);
                    }
                }

                return CommonResult.Success();
            }
            catch (Exception ex)
            {
                Logger.Error("分片上传失败", ex);
                return CommonResult.Error($"分片上传失败: {ex.Message}");
            }
        }
        /// <summary>
        /// 清理分片文件和记录
        /// </summary>
        private async Task CleanupChunks(string fileIdentifier)
        {
            try
            {
                // 删除分片记录
                await _fileChunkRepository.DeleteAsync(x => x.FileIdentifier == fileIdentifier);

                // 删除分片目录
                string tempDir = Path.Combine(GetTempFileDir(), fileIdentifier);
                if (Directory.Exists(tempDir))
                {
                    Directory.Delete(tempDir, true);
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"清理分片文件失败: {fileIdentifier}", ex);
            }
        }

        /// <summary>
        /// 合并文件
        /// </summary>
        private async Task<CommonResult<Guid>> MergeFile(string fileIdentifier, string fileName, long totalSize, int totalChunks)
        {
            string tempDir = Path.Combine(GetTempFileDir(), fileIdentifier);
            string fileDir = GetFileDir();
            string newFileName = $"{fileIdentifier}{Path.GetExtension(fileName)}";
            string filePath = Path.Combine(fileDir, newFileName);

            try
            {
                // 检查临时目录是否存在
                if (!Directory.Exists(tempDir))
                {
                    return CommonResult<Guid>.Error("临时文件目录不存在");
                }

                // 获取所有分片并按数字排序
                var chunkFiles = Directory.GetFiles(tempDir)
                    .Where(f => int.TryParse(Path.GetFileName(f), out _))
                    .OrderBy(f => int.Parse(Path.GetFileName(f)))
                    .ToArray();

                if (chunkFiles.Length != totalChunks)
                {
                    Logger.Error($"分片数量不完整: 期望 {totalChunks}, 实际 {chunkFiles.Length}");
                    return CommonResult<Guid>.Error($"分片数量不完整 (期望 {totalChunks}, 实际 {chunkFiles.Length})");
                }

                // 验证每个分片的大小
                long totalChunkSize = 0;
                for (int i = 0; i < chunkFiles.Length; i++)
                {
                    var chunkFile = chunkFiles[i];
                    var fileInfo = new FileInfo(chunkFile);
                    totalChunkSize += fileInfo.Length;

                    // 验证分片是否存在且大小合理
                    if (fileInfo.Length == 0)
                    {
                        return CommonResult<Guid>.Error($"分片 {i + 1} 文件大小为0");
                    }
                }

                // 验证总分片大小与预期总大小是否接近（允许微小误差）
                long sizeDifference = Math.Abs(totalChunkSize - totalSize);
                if (sizeDifference > 1024) // 允许1KB的误差
                {
                    Logger.Error($"分片总大小不匹配: 预期 {totalSize}, 实际 {totalChunkSize}, 差值 {sizeDifference}");
                    return CommonResult<Guid>.Error($"文件大小验证失败 (预期 {totalSize}, 实际 {totalChunkSize})");
                }

                // 合并文件
                using (var mergedStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    foreach (var chunkFile in chunkFiles)
                    {
                        try
                        {
                            using (var chunkStream = new FileStream(chunkFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                            {
                                await chunkStream.CopyToAsync(mergedStream);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Error($"读取分片文件失败: {chunkFile}", ex);
                            throw new Exception($"读取分片文件失败: {Path.GetFileName(chunkFile)}", ex);
                        }
                    }
                    await mergedStream.FlushAsync();
                }

                // 等待文件写入完成
                await Task.Delay(100);

                // 验证合并后的文件
                var mergedFileInfo = new FileInfo(filePath);
                if (!mergedFileInfo.Exists)
                {
                    return CommonResult<Guid>.Error("文件合并失败，生成的文件不存在");
                }

                // 验证合并后的文件大小
                long mergedSize = mergedFileInfo.Length;
                long finalSizeDifference = Math.Abs(mergedSize - totalSize);

                if (finalSizeDifference > 1024) // 允许1KB的误差
                {
                    // 删除错误的文件
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }

                    Logger.Error($"合并文件大小不匹配: 预期 {totalSize}, 实际 {mergedSize}");
                    return CommonResult<Guid>.Error($"文件合并失败，大小不匹配 (预期 {totalSize}, 实际 {mergedSize})");
                }

                // 检查附件是否已存在
                var existingAttachment = await _attachmentRepository.FirstOrDefaultAsync(x => x.FileIdentifier == fileIdentifier);
                if (existingAttachment != null)
                {
                    // 如果已存在，删除刚合并的文件
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                    return CommonResult<Guid>.Success(existingAttachment.Id);
                }

                // 创建附件记录
                var creatorUser = await _userAppService.GetNameByUserId(AbpSession.UserId.Value);
                var attachment = new Attachments
                {
                    FileIdentifier = fileIdentifier,
                    FileName = fileName,
                    FileExtension = Path.GetExtension(fileName),
                    FileSize = totalSize,
                    FileSizeFormat = GetReadableFileSize(totalSize),
                    FilePath = filePath,
                    FileType = Path.GetExtension(fileName)?.TrimStart('.'),
                    TotalChunks = totalChunks,
                    UploadStatus = 1,
                    AttachmentType = ATTACHMENT_TYPE_FILE,
                    DownloadCount = 0,
                    Remark = null
                };

                var attachmentId = await _attachmentRepository.InsertAndGetIdAsync(attachment);
                await CurrentUnitOfWork.SaveChangesAsync();

                return CommonResult<Guid>.Success(attachmentId);
            }
            catch (Exception ex)
            {
                Logger.Error("合并文件失败", ex);

                // 清理可能已创建的错误文件
                try
                {
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                }
                catch { }

                return CommonResult<Guid>.Error($"合并文件失败: {ex.Message}");
            }
        }

        #endregion

        #region 链接附件管理

        /// <summary>
        /// 添加链接附件
        /// </summary>
        [HttpPost]
        public async Task<CommonResult<Guid>> AddLinkAttachment(AddLinkAttachmentInput input)
        {
            try
            {
                // 验证URL
                if (!IsValidUrl(input.LinkUrl))
                {
                    return CommonResult<Guid>.Error("请输入有效的URL地址");
                }

                // 如果未提供文件名，从URL中提取
                string fileName = input.FileName;
                if (string.IsNullOrEmpty(fileName))
                {
                    fileName = ExtractFileNameFromUrl(input.LinkUrl);
                }

                // 如果未提供文件类型，从文件名中提取
                string fileExtension = Path.GetExtension(fileName);
                string fileType = input.FileType;
                if (string.IsNullOrEmpty(fileType) && !string.IsNullOrEmpty(fileExtension))
                {
                    fileType = fileExtension.TrimStart('.');
                }

                // 生成文件标识（使用URL的MD5作为标识）
                string fileIdentifier = GenerateUrlIdentifier(input.LinkUrl);

                // 检查是否已存在相同链接的附件
                var existingAttachment = await _attachmentRepository.FirstOrDefaultAsync(x =>
                    x.FileIdentifier == fileIdentifier && x.FilePath == input.LinkUrl);

                if (existingAttachment != null)
                {
                    return CommonResult<Guid>.Success(existingAttachment.Id);
                }

                // 创建链接附件记录
                var attachment = new Attachments
                {
                    FileIdentifier = fileIdentifier,
                    FileName = fileName,
                    FileExtension = fileExtension,
                    FileSize = input.FileSize ?? 0,
                    FileSizeFormat = input.FileSize.HasValue ? GetReadableFileSize(input.FileSize.Value) : "0 B",
                    FilePath = input.LinkUrl,
                    FileType = fileType,
                    TotalChunks = 1,
                    UploadStatus = 1,
                    AttachmentType = ATTACHMENT_TYPE_LINK,
                    Remark = input.Remark
                };

                var attachmentId = await _attachmentRepository.InsertAndGetIdAsync(attachment);
                await CurrentUnitOfWork.SaveChangesAsync();

                return CommonResult<Guid>.Success(attachmentId);
            }
            catch (Exception ex)
            {
                Logger.Error("添加链接附件失败", ex);
                return CommonResult<Guid>.Error($"添加链接附件失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 批量添加链接附件
        /// </summary>
        [HttpPost]
        public async Task<CommonResult<List<Guid>>> BatchAddLinkAttachments(List<AddLinkAttachmentInput> inputs)
        {
            try
            {
                if (inputs == null || !inputs.Any())
                {
                    return CommonResult<List<Guid>>.Error("请至少输入一个链接地址");
                }

                var result = new List<Guid>();

                foreach (var input in inputs)
                {
                    try
                    {
                        var addResult = await AddLinkAttachment(input);
                        if (addResult.IsSuccess && addResult.Data != Guid.Empty)
                        {
                            result.Add(addResult.Data);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn($"批量添加链接时单个链接失败: {ex.Message}");
                    }
                }

                return CommonResult<List<Guid>>.Success(result);
            }
            catch (Exception ex)
            {
                Logger.Error("批量添加链接附件失败", ex);
                return CommonResult<List<Guid>>.Error($"批量添加链接附件失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 验证URL是否有效
        /// </summary>
        private bool IsValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        /// <summary>
        /// 从URL中提取文件名
        /// </summary>
        private string ExtractFileNameFromUrl(string url)
        {
            try
            {
                Uri uri = new Uri(url);
                string fileName = Path.GetFileName(uri.LocalPath);

                if (string.IsNullOrEmpty(fileName))
                {
                    fileName = $"{uri.Host}_{DateTime.Now:yyyyMMddHHmmss}.link";
                }

                return fileName;
            }
            catch
            {
                return $"link_{DateTime.Now:yyyyMMddHHmmss}.link";
            }
        }

        /// <summary>
        /// 生成URL标识（MD5）
        /// </summary>
        private string GenerateUrlIdentifier(string url)
        {
            using (var md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(url);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                return Convert.ToHexString(hashBytes).ToLower();
            }
        }

        #endregion

        #region 附件关系管理

        /// <summary>
        /// 设置业务附件关系（支持分类）
        /// </summary>
        [HttpPost]
        public async Task<CommonResult> SetBusinessAttachments(SetBusinessAttachmentInput input)
        {
            try
            {
                // 参数验证
                if (input.BusinessId == Guid.Empty)
                {
                    return CommonResult.Error("业务ID不能为空");
                }

                if (string.IsNullOrWhiteSpace(input.BusinessType))
                {
                    return CommonResult.Error("业务类型不能为空");
                }


                // 统一获取需要处理的附件列表
                var attachmentsToProcess = input.GetAttachmentsToProcess();

                // 获取当前已绑定的附件关系
                var existingRelations = await _relationRepository.GetAll()
                    .Where(x => x.BusinessId == input.BusinessId &&
                               x.BusinessType == input.BusinessType)
                    .ToListAsync();

                // 创建字典以便快速查找
                var existingRelationsDict = existingRelations.ToDictionary(x => x.AttachmentId);

                // 需要新增的附件关系
                var relationsToAdd = new List<BusinessAttachmentRelations>();

                // 需要保留的附件ID集合（用于确定需要删除的附件）
                var attachmentIdsToKeep = new HashSet<Guid>();

                // 处理传入的附件
                foreach (var item in attachmentsToProcess)
                {
                    attachmentIdsToKeep.Add(item.AttachmentId);

                    // 检查是否已存在相同附件ID的关系
                    if (existingRelationsDict.TryGetValue(item.AttachmentId, out var existing))
                    {
                        // 如果存在但分类不同，更新分类
                        if (existing.Category != item.Category)
                        {
                            existing.Category = item.Category;
                            await _relationRepository.UpdateAsync(existing);
                        }
                    }
                    else
                    {
                        // 新增关系
                        relationsToAdd.Add(new BusinessAttachmentRelations
                        {
                            Id = Guid.NewGuid(),
                            AttachmentId = item.AttachmentId,
                            BusinessId = input.BusinessId,
                            BusinessType = input.BusinessType,
                            Category = item.Category
                        });
                    }
                }

                // 需要删除的附件关系（已存在但不在传入列表中的）
                var relationsToRemove = existingRelations
                    .Where(x => !attachmentIdsToKeep.Contains(x.AttachmentId))
                    .ToList();

                // 批量新增
                if (relationsToAdd.Any())
                {
                    await _relationRepository.GetDbContext().BulkInsertAsync(relationsToAdd);
                }

                // 批量删除
                if (relationsToRemove.Any())
                {
                    var removeIds = relationsToRemove.Select(x => x.Id).ToList();
                    await _relationRepository.DeleteAsync(x => removeIds.Contains(x.Id));

                    // 清理无关联的附件
                    var removeAttachmentIds = relationsToRemove.Select(x => x.AttachmentId).ToList();
                    await CleanupUnusedAttachments(removeAttachmentIds, input.BusinessId);
                }

                return CommonResult.Ok();
            }
            catch (Exception ex)
            {
                Logger.Error($"设置业务附件关系失败，BusinessId: {input.BusinessId}, BusinessType: {input.BusinessType}", ex);
                return CommonResult.Error($"设置业务附件关系失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取业务附件列表（带分类）
        /// </summary>
        [HttpGet]
        public async Task<CommonResult<List<AttachmentWithCategoryDto>>> GetBusinessAttachmentsWithCategory([FromQuery] GetBusinessAttachmentsInput input)
        {
            try
            {
                var query = from r in _relationRepository.GetAll()
                            join a in _attachmentRepository.GetAll() on r.AttachmentId equals a.Id
                            where r.BusinessId == input.BusinessId && r.BusinessType == input.BusinessType
                            select new { Relation = r, Attachment = a };

                // 按附件类型筛选
                if (input.AttachmentType.HasValue)
                {
                    if (input.AttachmentType.Value == ATTACHMENT_TYPE_FILE)
                    {
                        query = query.Where(x => !x.Attachment.FilePath.StartsWith("http://") && !x.Attachment.FilePath.StartsWith("https://"));
                    }
                    else if (input.AttachmentType.Value == ATTACHMENT_TYPE_LINK)
                    {
                        query = query.Where(x => x.Attachment.FilePath.StartsWith("http://") || x.Attachment.FilePath.StartsWith("https://"));
                    }
                }

                var attachments = await query
                    .OrderByDescending(x => x.Attachment.CreationTime)
                    .Select(x => new AttachmentWithCategoryDto
                    {
                        Id = x.Attachment.Id,
                        FileIdentifier = x.Attachment.FileIdentifier,
                        FileName = x.Attachment.FileName,
                        FileExtension = x.Attachment.FileExtension,
                        FileSize = x.Attachment.FileSize,
                        FileSizeFormat = x.Attachment.FileSizeFormat,
                        FilePath = x.Attachment.FilePath,
                        FileUrl = x.Attachment.FilePath.StartsWith("http") ? x.Attachment.FilePath : GetFileUrl(x.Attachment.FilePath),
                        FileType = x.Attachment.FileType,
                        TotalChunks = x.Attachment.TotalChunks,
                        UploadStatus = x.Attachment.UploadStatus,
                        DownloadCount = x.Attachment.DownloadCount,
                        Remark = x.Attachment.Remark,
                        CreationTime = x.Attachment.CreationTime,
                        AttachmentType = x.Attachment.AttachmentType,
                        LinkUrl = x.Attachment.FilePath.StartsWith("http") ? x.Attachment.FilePath : null,
                        Category = x.Relation.Category
                    })
                    .ToListAsync();

                return CommonResult<List<AttachmentWithCategoryDto>>.Success(attachments);
            }
            catch (Exception ex)
            {
                Logger.Error("获取业务附件列表失败", ex);
                return CommonResult<List<AttachmentWithCategoryDto>>.Error($"获取业务附件列表失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取业务附件列表（原有接口，保持兼容）
        /// </summary>
        [HttpGet]
        public async Task<CommonResult<List<AttachmentDto>>> GetBusinessAttachments([FromQuery] GetBusinessAttachmentsInput input)
        {
            try
            {
                var query = from r in _relationRepository.GetAll()
                            join a in _attachmentRepository.GetAll() on r.AttachmentId equals a.Id
                            where r.BusinessId == input.BusinessId && r.BusinessType == input.BusinessType
                            select a;

                // 按附件类型筛选
                if (input.AttachmentType.HasValue)
                {
                    if (input.AttachmentType.Value == ATTACHMENT_TYPE_FILE)
                    {
                        query = query.Where(x => !x.FilePath.StartsWith("http://") && !x.FilePath.StartsWith("https://"));
                    }
                    else if (input.AttachmentType.Value == ATTACHMENT_TYPE_LINK)
                    {
                        query = query.Where(x => x.FilePath.StartsWith("http://") || x.FilePath.StartsWith("https://"));
                    }
                }

                var attachments = await query
                    .OrderByDescending(x => x.CreationTime)
                    .Select(x => new AttachmentDto
                    {
                        Id = x.Id,
                        FileIdentifier = x.FileIdentifier,
                        FileName = x.FileName,
                        FileExtension = x.FileExtension,
                        FileSize = x.FileSize,
                        FileSizeFormat = x.FileSizeFormat,
                        FilePath = x.FilePath,
                        FileUrl = x.FilePath.StartsWith("http") ? x.FilePath : GetFileUrl(x.FilePath),
                        FileType = x.FileType,
                        TotalChunks = x.TotalChunks,
                        UploadStatus = x.UploadStatus,
                        DownloadCount = x.DownloadCount,
                        Remark = x.Remark,
                        CreationTime = x.CreationTime,
                        AttachmentType = x.AttachmentType,
                        LinkUrl = x.FilePath.StartsWith("http") ? x.FilePath : null
                    })
                    .ToListAsync();

                return CommonResult<List<AttachmentDto>>.Success(attachments);
            }
            catch (Exception ex)
            {
                Logger.Error("获取业务附件列表失败", ex);
                return CommonResult<List<AttachmentDto>>.Error($"获取业务附件列表失败: {ex.Message}");
            }
        }

        #endregion

        #region 附件管理

        /// <summary>
        /// 获取附件分页列表
        /// </summary>
        [HttpGet]
        public async Task<CommonResult<Page<AttachmentDto>>> GetPageList([FromQuery] AttachmentPageInput input)
        {
            try
            {
                var query = _attachmentRepository.GetAll().AsNoTracking()
                    .WhereIf(!string.IsNullOrWhiteSpace(input.SearchKey),
                        x => x.FileName.Contains(input.SearchKey) || x.FileIdentifier.Contains(input.SearchKey));

                // 按附件类型筛选
                if (input.AttachmentType.HasValue)
                {
                    query = query.Where(x => x.AttachmentType == input.AttachmentType.Value);
                }

                var total = await query.CountAsync();

                var items = await query
                    .OrderByDescending(x => x.CreationTime)
                    .Skip((input.Current - 1) * input.Size)
                    .Take(input.Size)
                    .Select(x => new AttachmentDto
                    {
                        Id = x.Id,
                        FileIdentifier = x.FileIdentifier,
                        FileName = x.FileName,
                        FileExtension = x.FileExtension,
                        FileSize = x.FileSize,
                        FileSizeFormat = x.FileSizeFormat,
                        FilePath = x.FilePath,
                        FileUrl = x.AttachmentType == ATTACHMENT_TYPE_LINK ? x.FilePath : GetFileUrl(x.FilePath),
                        FileType = x.FileType,
                        TotalChunks = x.TotalChunks,
                        UploadStatus = x.UploadStatus,
                        DownloadCount = x.DownloadCount,
                        Remark = x.Remark,
                        CreationTime = x.CreationTime,
                        AttachmentType = x.AttachmentType,
                        LinkUrl = x.AttachmentType == ATTACHMENT_TYPE_LINK ? x.FilePath : null
                    })
                    .ToListAsync();

                var page = new Page<AttachmentDto>(input.Current, input.Size, total)
                {
                    Records = items
                };

                return CommonResult<Page<AttachmentDto>>.Success(page);
            }
            catch (Exception ex)
            {
                Logger.Error("获取附件分页列表失败", ex);
                return CommonResult<Page<AttachmentDto>>.Error($"获取附件分页列表失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 删除附件
        /// </summary>
        [HttpDelete]
        public async Task<CommonResult> Delete([FromBody] List<AttachmentIdInput> input, Guid? businessId)
        {
            try
            {
                if (input == null || !input.Any())
                {
                    return CommonResult.Error("请选择要删除的附件");
                }

                var fileIds = input.Select(x => x.Id).ToList();
                foreach (var id in fileIds)
                {
                    await DeleteAttachment(id, businessId);
                }

                return CommonResult.Ok();
            }
            catch (Exception ex)
            {
                Logger.Error("删除附件失败", ex);
                return CommonResult.Error($"删除附件失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 删除单个附件
        /// </summary>
        private async Task DeleteAttachment(Guid id, Guid? businessId)
        {
            var attachment = await _attachmentRepository.FirstOrDefaultAsync(id);
            if (attachment == null)
            {
                return;
            }

            // 检查是否有其他业务关联
            var relationCount = await _relationRepository.CountAsync(x => x.AttachmentId == id && x.BusinessId != businessId);

            if (relationCount > 0)
            {
                // 只删除当前业务的关系
                await _relationRepository.DeleteAsync(x => x.AttachmentId == id && x.BusinessId == businessId);
            }
            else
            {
                // 删除物理文件（仅对本地文件有效）
                if (attachment.AttachmentType == ATTACHMENT_TYPE_FILE && File.Exists(attachment.FilePath))
                {
                    try
                    {
                        File.Delete(attachment.FilePath);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"删除物理文件失败: {attachment.FilePath}", ex);
                    }
                }

                // 删除所有关联关系
                await _relationRepository.DeleteAsync(x => x.AttachmentId == id);

                // 删除附件记录
                await _attachmentRepository.DeleteAsync(id);
            }
        }

        /// <summary>
        /// 清理无关联的附件
        /// </summary>
        private async Task CleanupUnusedAttachments(List<Guid> attachmentIds, Guid? businessId)
        {
            foreach (var attachmentId in attachmentIds)
            {
                var relationCount = await _relationRepository.CountAsync(x => x.AttachmentId == attachmentId);
                if (relationCount == 0)
                {
                    await DeleteAttachment(attachmentId, businessId);
                }
            }
        }

        /// <summary>
        /// 下载附件
        /// </summary>
        [HttpGet]
        [AbpAllowAnonymous]
        public async Task<IActionResult> Download(Guid id)
        {
            var attachment = await _attachmentRepository.FirstOrDefaultAsync(id);
            if (attachment == null)
            {
                throw new UserFriendlyException("附件不存在");
            }

            // 如果是链接附件，重定向到链接地址
            if (attachment.AttachmentType == ATTACHMENT_TYPE_LINK)
            {
                return new RedirectResult(attachment.FilePath);
            }

            if (!File.Exists(attachment.FilePath))
            {
                throw new UserFriendlyException("文件不存在");
            }

            // 更新下载次数
            attachment.DownloadCount++;
            await _attachmentRepository.UpdateAsync(attachment);

            var fileBytes = await File.ReadAllBytesAsync(attachment.FilePath);
            return new FileContentResult(fileBytes, "application/octet-stream")
            {
                FileDownloadName = attachment.FileName
            };
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 获取文件存储目录
        /// </summary>
        private string GetFileDir()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "files");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }

        /// <summary>
        /// 获取临时文件目录
        /// </summary>
        private string GetTempFileDir()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "temp");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }

        /// <summary>
        /// 获取文件访问URL
        /// </summary>
        private static string GetFileUrl(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return null;
            }

            var fileName = Path.GetFileName(filePath);
            return $"/api/Attachment/Download?fileName={fileName}";
        }

        /// <summary>
        /// 根据附件ID获取文件路径
        /// </summary>
        private async Task<string> GetAttachmentPath(Guid attachmentId)
        {
            var attachment = await _attachmentRepository.FirstOrDefaultAsync(attachmentId);
            return attachment?.FilePath;
        }

        /// <summary>
        /// 格式化文件大小
        /// </summary>
        private string GetReadableFileSize(long fileSizeInBytes)
        {
            if (fileSizeInBytes < 1024)
            {
                return $"{fileSizeInBytes} B";
            }
            else if (fileSizeInBytes < 1024 * 1024)
            {
                var fileSizeInKB = fileSizeInBytes / 1024.0;
                return $"{fileSizeInKB:F2} KB";
            }
            else if (fileSizeInBytes < 1024 * 1024 * 1024)
            {
                var fileSizeInMB = fileSizeInBytes / (1024.0 * 1024);
                return $"{fileSizeInMB:F2} MB";
            }
            else
            {
                var fileSizeInGB = fileSizeInBytes / (1024.0 * 1024 * 1024);
                return $"{fileSizeInGB:F2} GB";
            }
        }

        #endregion
    }
}