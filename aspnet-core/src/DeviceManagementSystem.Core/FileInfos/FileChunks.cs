using Abp.Domain.Entities;
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
    /// 文件分片表
    /// </summary>
    [Table("FileChunk")]
    public class FileChunks : Entity<Guid>
    {
        /// <summary>
        /// 文件唯一标识(MD5)
        /// </summary>
        [Required]
        [StringLength(64)]
        public string FileIdentifier { get; set; }

        /// <summary>
        /// 当前分片索引(从1开始)
        /// </summary>
        public int ChunkNumber { get; set; }

        /// <summary>
        /// 分片大小
        /// </summary>
        public long ChunkSize { get; set; }

        /// <summary>
        /// 当前分片大小
        /// </summary>
        public long CurrentChunkSize { get; set; }

        /// <summary>
        /// 总分片数
        /// </summary>
        public int TotalChunks { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        [StringLength(500)]
        public string FileName { get; set; }

        /// <summary>
        /// 分片文件路径
        /// </summary>
        [StringLength(1000)]
        public string ChunkPath { get; set; }

        /// <summary>
        /// 上传完成状态
        /// </summary>
        public bool IsUploaded { get; set; }
    }
}
