using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.FileInfos
{
    /// <summary>
    /// 附件表
    /// </summary>
    [Table("Attachment")]
    public class Attachments : FullAuditedEntity<Guid>
    {
        /// <summary>
        /// 文件唯一标识(MD5)
        /// </summary>
        [Required]
        [StringLength(64)]
        public string FileIdentifier { get; set; }

        /// <summary>
        /// 原始文件名
        /// </summary>
        [Required]
        [StringLength(500)]
        public string FileName { get; set; }

        /// <summary>
        /// 文件扩展名
        /// </summary>
        [StringLength(50)]
        public string FileExtension { get; set; }

        /// <summary>
        /// 文件大小(字节)
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// 文件大小(格式化)
        /// </summary>
        [StringLength(50)]
        public string FileSizeFormat { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        [Required]
        [StringLength(1000)]
        public string FilePath { get; set; }

        /// <summary>
        /// 文件类型
        /// </summary>
        [StringLength(200)]
        public string FileType { get; set; }

        /// <summary>
        /// 总片数
        /// </summary>
        public int TotalChunks { get; set; }

        /// <summary>
        /// 上传状态(0:上传中 1:上传完成)
        /// </summary>
        public int UploadStatus { get; set; }

        /// <summary>
        /// 下载次数
        /// </summary>
        public int DownloadCount { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500)]
        public string Remark { get; set; }

        /// <summary>
        /// 附件类型：0-本地文件，1-链接地址
        /// </summary>
        public int AttachmentType { get; set; }
    }
}