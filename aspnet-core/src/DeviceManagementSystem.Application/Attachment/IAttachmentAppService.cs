using DeviceManagementSystem.Attachment.Dto;
using DeviceManagementSystem.Utils.Common;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Attachment
{
    /// <summary>
    /// 附件服务接口
    /// </summary>
    public interface IAttachmentAppService
    {

        /// <summary>
        /// 检查文件上传状态
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<CommonResult<CheckUploadResult>> CheckUpload(CheckUploadInput input);


        /// <summary>
        /// 上传文件分片
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<CommonResult<object>> ChunkUpload([FromForm] FileChunkParam input);


        /// <summary>
        /// 添加链接附件
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<CommonResult<Guid>> AddLinkAttachment(AddLinkAttachmentInput input);


        /// <summary>
        /// 批量添加链接附件
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        Task<CommonResult<List<Guid>>> BatchAddLinkAttachments(List<AddLinkAttachmentInput> inputs);


        /// <summary>
        /// 设置业务附件关系（统一的绑定/解绑接口）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<CommonResult> SetBusinessAttachments(SetBusinessAttachmentInput input);


        /// <summary>
        /// 获取业务附件列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<CommonResult<List<AttachmentDto>>> GetBusinessAttachments([FromQuery] GetBusinessAttachmentsInput input);


        /// <summary>
        /// 获取附件分页列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<CommonResult<Page<AttachmentDto>>> GetPageList([FromQuery] AttachmentPageInput input);


        /// <summary>
        /// 删除附件
        /// </summary>
        /// <param name="input"></param>
        /// <param name="businessId"></param>
        /// <returns></returns>
        Task<CommonResult> Delete([FromBody] List<AttachmentIdInput> input, Guid? businessId);


        /// <summary>
        /// 下载附件
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IActionResult> Download(Guid id);



        /// <summary>
        /// 获取业务附件列表（带分类）
        /// </summary>
        Task<CommonResult<List<AttachmentWithCategoryDto>>> GetBusinessAttachmentsWithCategory([FromQuery] GetBusinessAttachmentsInput input);

    }
}
