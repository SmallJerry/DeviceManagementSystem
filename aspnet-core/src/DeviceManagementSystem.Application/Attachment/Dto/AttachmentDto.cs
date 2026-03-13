// AttachmentDto.cs 增加新字段
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Attachment.Dto
{
    /// <summary>
    /// 附件DTO
    /// </summary>
    public class AttachmentDto
    {
        /// <summary>
        /// 主键
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 文件MD5值
        /// </summary>
        public string FileIdentifier { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 文件拓展名
        /// </summary>
        public string FileExtension { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// 文件格式化大小
        /// </summary>
        public string FileSizeFormat { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 文件地址
        /// </summary>
        public string FileUrl { get; set; }

        /// <summary>
        /// 文件类型
        /// </summary>
        public string FileType { get; set; }

        /// <summary>
        /// 文件块
        /// </summary>
        public int TotalChunks { get; set; }

        /// <summary>
        /// 文件上传状态
        /// </summary>
        public int UploadStatus { get; set; }

        /// <summary>
        /// 文件下载次数
        /// </summary>
        public int DownloadCount { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 上传时间
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 上传者
        /// </summary>
        public string Creator { get; set; }


        /// <summary>
        /// 附件类型：0-本地文件，1-链接地址
        /// </summary>
        public int AttachmentType { get; set; }

        /// <summary>
        /// 文件链接地址（当AttachmentType=1时有效）
        /// </summary>
        public string LinkUrl { get; set; }
    }


    /// <summary>
    /// 带分类的附件DTO
    /// </summary>
    public class AttachmentWithCategoryDto : AttachmentDto
    {
        /// <summary>
        /// 附件分类
        /// </summary>
        public string Category { get; set; }
    }

    /// <summary>
    /// 附件分页查询输入参数
    /// </summary>
    public class AttachmentPageInput
    {
        /// <summary>
        /// 查询关键字
        /// </summary>
        public string SearchKey { get; set; }

        /// <summary>
        /// 当前页码
        /// </summary>
        public int Current { get; set; } = 1;

        /// <summary>
        /// 页数大小
        /// </summary>
        public int Size { get; set; } = 10;

        /// <summary>
        /// 附件类型筛选：null-全部，0-本地文件，1-链接地址
        /// </summary>
        public int? AttachmentType { get; set; }
    }

    /// <summary>
    /// 文件分片DTO
    /// </summary>
    public class FileChunkDto
    {
        /// <summary>
        /// 文件md5值
        /// </summary>
        public string FileIdentifier { get; set; }

        /// <summary>
        /// 块数
        /// </summary>
        public int ChunkNumber { get; set; }

        /// <summary>
        /// 块大小
        /// </summary>
        public long ChunkSize { get; set; }

        /// <summary>
        /// 当前块大小
        /// </summary>
        public long CurrentChunkSize { get; set; }

        /// <summary>
        /// 总块数
        /// </summary>        
        public int TotalChunks { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 总大小
        /// </summary>
        public long TotalSize { get; set; }
    }

    /// <summary>
    /// 文件分片上传参数
    /// </summary>
    public class FileChunkParam
    {
        /// <summary>
        /// 文件md5值
        /// </summary>
        public string FileIdentifier { get; set; }

        /// <summary>
        /// 块数
        /// </summary>
        public int ChunkNumber { get; set; }

        /// <summary>
        /// 块大小
        /// </summary>
        public long ChunkSize { get; set; }

        /// <summary>
        /// 当前块大小
        /// </summary>
        public long CurrentChunkSize { get; set; }

        /// <summary>
        /// 总块数
        /// </summary>
        public int TotalChunks { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 文件类型
        /// </summary>
        public string FileType { get; set; }

        /// <summary>
        /// 总大小
        /// </summary>
        public long TotalSize { get; set; }

        /// <summary>
        /// 文件本体
        /// </summary>
        public Microsoft.AspNetCore.Http.IFormFile File { get; set; }
    }

    /// <summary>
    /// 检查文件上传状态参数
    /// </summary>
    public class CheckUploadInput
    {
        /// <summary>
        /// 文件md5值
        /// </summary>
        public string FileIdentifier { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 文件总大小
        /// </summary>
        public long TotalSize { get; set; }

        /// <summary>
        /// 文件总分片数(如果文件已上传成功，则返回文件的总分片数；如果文件未上传成功，则返回0)。前端可以根据这个总分片数来判断文件是否已经上传成功，如果大于0则说明文件已经上传成功，可以直接使用返回的URL或附件ID来预览或下载文件；如果等于0则说明文件未上传成功，需要继续上传分片或者重新上传整个文件。
        /// </summary>
        public int TotalChunks { get; set; }
    }

    /// <summary>
    /// 检查文件上传结果
    /// </summary>
    public class CheckUploadResult
    {
        /// <summary>
        /// 是否已上传成功(如果文件已上传成功，则返回true；如果文件未上传成功，则返回false)。前端可以根据这个状态来判断文件是否已经上传成功，如果为true则说明文件已经上传成功，可以直接使用返回的URL或附件ID来预览或下载文件；如果为false则说明文件未上传成功，需要继续上传分片或者重新上传整个文件。
        /// </summary>
        public bool Uploaded { get; set; }

        /// <summary>
        /// 附件URL(如果文件已上传成功，则返回附件URL；如果文件未上传成功，则返回null)。前端可以根据这个URL来判断文件是否已经上传成功，如果有URL则说明文件已经上传成功，可以直接使用这个URL来预览或下载文件；如果没有URL则说明文件未上传成功，需要继续上传分片或者重新上传整个文件。
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 附件ID(如果文件已上传成功，则返回附件ID；如果文件未上传成功，则返回null)。前端可以根据这个附件ID来判断文件是否已经上传成功，如果有附件ID则说明文件已经上传成功，可以直接使用这个附件ID来绑定到业务上；如果没有附件ID则说明文件未上传成功，需要继续上传分片或者重新上传整个文件。
        /// </summary>
        public Guid? AttachmentId { get; set; }

        /// <summary>
        /// 文件已上传的分片索引列表(从1开始)。如果文件已上传，则返回已上传的分片索引列表；如果文件未上传，则返回空列表或null。前端可以根据这个列表来判断哪些分片需要重新上传，哪些分片已经上传成功，可以跳过上传。
        /// </summary>
        public int[] UploadedChunks { get; set; }
    }


    /// <summary>
    /// 设置业务附件关系参数
    /// </summary>
    public class SetBusinessAttachmentInput
    {
        /// <summary>
        /// 业务ID(必填)。根据业务ID和业务类型来设置附件关系。
        /// </summary>
        public Guid BusinessId { get; set; }

        /// <summary>
        /// 业务类型(如:Device,Type等)。指定要设置附件关系的业务类型。（兼容旧版）
        /// </summary>
        public string BusinessType { get; set; }

        /// <summary>
        /// 附件ID列表。传入最终需要绑定到该业务下的附件ID列表。
        /// 系统会自动将传入的附件与该业务建立关系，并解除其他附件与该业务的关系。
        /// </summary>
        public List<Guid> AttachmentIds { get; set; }


        /// <summary>
        /// 附件与分类关系列表（新版）
        /// </summary>
        public List<AttachmentWithCategory> AttachmentWithCategories { get; set; }
    }

    /// <summary>
    /// 附件与分类关系
    /// </summary>
    public class AttachmentWithCategory
    {
        /// <summary>
        /// 附件ID
        /// </summary>
        public Guid AttachmentId { get; set; }

        /// <summary>
        /// 附件分类
        /// </summary>
        public string Category { get; set; }


        /// <summary>
        /// 文件名（可选）
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 文件大小（可选，单位：字节）
        /// </summary>
        public long? FileSize { get; set; }
    }



    /// <summary>
    /// 绑定附件参数
    /// </summary>
    public class BindAttachmentInput : SetBusinessAttachmentInput
    {
    }

    /// <summary>
    /// 解绑附件参数
    /// </summary>
    public class UnbindAttachmentInput : SetBusinessAttachmentInput
    {
    }



    /// <summary>
    /// 批量设置附件关系输入参数（支持分类）
    /// </summary>
    public class SetBusinessAttachmentsInput
    {
        /// <summary>
        /// 业务ID
        /// </summary>
        [Required]
        public Guid BusinessId { get; set; }

        /// <summary>
        /// 业务类型
        /// </summary>
        [Required]
        public string BusinessType { get; set; }

        /// <summary>
        /// 附件列表（包含分类信息）
        /// </summary>
        public List<BusinessAttachmentInput> Attachments { get; set; }
    }



    /// <summary>
    /// 业务附件输入项
    /// </summary>
    public class BusinessAttachmentInput
    {
        /// <summary>
        /// 附件ID
        /// </summary>
        [Required]
        public Guid AttachmentId { get; set; }

        /// <summary>
        /// 附件分类
        /// </summary>
        public string Category { get; set; }
    }




    /// <summary>
    /// 获取业务附件列表参数
    /// </summary>
    public class GetBusinessAttachmentsInput
    {
        /// <summary>
        /// 业务ID(必填)。根据业务ID和业务类型来获取附件列表。如果不传业务ID，则无法获取任何附件，因为附件必须绑定到具体的业务上；如果传了业务ID但不传业务类型，则会获取该业务ID下所有业务类型的附件；如果同时传了业务ID和业务类型，则只获取该业务ID和业务类型对应的附件。
        /// </summary>
        public Guid BusinessId { get; set; }

        /// <summary>
        /// 业务类型(如:Device,Type等)。如果不传，则获取所有业务类型的附件；如果传了，则只获取指定业务类型的附件。
        /// </summary>
        public string BusinessType { get; set; }

        /// <summary>
        /// 附件分类（如：Manual, Drawing, TechnicalAgreement等）
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// 附件类型筛选：null-全部，0-本地文件，1-链接地址
        /// </summary>
        public int? AttachmentType { get; set; }
    }

    /// <summary>
    /// ID参数
    /// </summary>
    public class AttachmentIdInput
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required(ErrorMessage = "ID不能为空")]
        public Guid Id { get; set; }
    }

    /// <summary>
    /// 添加链接附件参数
    /// </summary>
    public class AddLinkAttachmentInput
    {
        /// <summary>
        /// 链接地址
        /// </summary>
        [Required(ErrorMessage = "链接地址不能为空")]
        [Url(ErrorMessage = "请输入有效的URL地址")]
        public string LinkUrl { get; set; }

        /// <summary>
        /// 文件名（可选，如果不提供则从URL中提取）
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 文件类型（可选）
        /// </summary>
        public string FileType { get; set; }

        /// <summary>
        /// 文件大小（可选，单位：字节）
        /// </summary>
        public long? FileSize { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}