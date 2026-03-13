using Abp.Application.Services.Dto;

namespace DeviceManagementSystem.Users.Dto
{
    /// <summary>
    /// 用户分页请求对象
    /// </summary>
    public class PagedUserResultRequestDto : PagedResultRequestDto
    {


        /// <summary>
        /// 关键词
        /// </summary>
        public string Keyword { get; set; }
        
        /// <summary>
        /// 状态
        /// </summary>
        public bool? IsActive { get; set; }
    }
}
