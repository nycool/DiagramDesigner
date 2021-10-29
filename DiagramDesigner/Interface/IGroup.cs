using DiagramDesigner.DesignerItemViewModel;
using System;

namespace DiagramDesigner.Interface
{
    public interface IGroup
    {
        /// <summary>
        /// 增加模块信息
        /// </summary>
        /// <param name="srcDesignerItem"></param>
        /// <param name="designerItem"></param>
        /// <returns></returns>
        bool TryAddDesignerItem(DesignerItemViewModelBase srcDesignerItem, DesignerItemViewModelBase designerItem);

        /// <summary>
        /// 移除模块信息
        /// </summary>
        /// <param name="src"></param>
        /// <param name="designerItem"></param>
        /// <returns></returns>
        bool TryGetDesignerItem(DesignerItemViewModelBase src, out DesignerItemViewModelBase designerItem);

        /// <summary>
        /// 根据目标找到源
        /// </summary>
        /// <param name="des"></param>
        /// <param name="src"></param>
        /// <returns></returns>

        bool TryGetDstToSrc(DesignerItemViewModelBase des, out DesignerItemViewModelBase src);

        /// <summary>
        /// 移除映射关系
        /// </summary>
        /// <returns></returns>
        bool Remove(DesignerItemViewModelBase key);

        /// <summary>
        /// 找到对应Id的模块
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        DesignerItemViewModelBase FindDesignerItem(Guid id);
    }
}