using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.SystemConfigs.param
{
    /// <summary>
    /// 配置ID参数
    /// </summary>
    public class ConfigIdParam
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required(ErrorMessage = "ID不能为空")]
        public Guid Id { get; set; }
    }
}
