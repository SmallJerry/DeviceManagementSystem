using DeviceManagementSystem.BasicDataManagements.Supplier.Dto;
using DeviceManagementSystem.Utils.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.BasicDataManagements.Supplier
{
    /// <summary>
    /// 供应商管理服务接口
    /// </summary>
    public interface ISupplierAppService
    {
        /// <summary>
        /// 获取供应商分页列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<CommonResult<Page<SupplierDto>>> GetPageList(SupplierPageInput input);

        /// <summary>
        /// 获取供应商列表
        /// </summary>
        /// <param name="searchKey"></param>
        /// <returns></returns>
        Task<CommonResult<List<SupplierDto>>> GetList(string searchKey = null);

        /// <summary>
        /// 获取供应商详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<CommonResult<SupplierDto>> GetById(Guid id);

        /// <summary>
        /// 创建供应商
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<CommonResult> Create(SupplierAddInput input);

        /// <summary>
        /// 更新供应商
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<CommonResult> UpdateById(SupplierEditInput input);

        /// <summary>
        /// 删除供应商
        /// </summary>
        /// <param name="supplierIds"></param>
        /// <returns></returns>
        Task<CommonResult> DeleteByIds(List<SupplierIdInput> supplierIds);
    }
}
