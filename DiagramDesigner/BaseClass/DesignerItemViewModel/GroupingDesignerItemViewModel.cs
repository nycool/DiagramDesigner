using System;
using DiagramDesigner.BaseClass.Interface;

namespace DiagramDesigner.BaseClass.DesignerItemViewModel
{

    /// <summary>
    /// 类结构多继承设计的有点问题
    /// </summary>
    public class GroupingDesignerItemViewModel : GroupDesignerItemViewModelBase
    {

        #region Construstor

        public GroupingDesignerItemViewModel(Guid id, IDiagramViewModel parent, double left, double top)
            : base(id, parent, left, top)
        {

        }

        public GroupingDesignerItemViewModel()
        {

        }

        public GroupingDesignerItemViewModel(Guid id, IDiagramViewModel parent, double left, double top, double itemWidth, double itemHeight)
            : base(id, parent, left, top, itemWidth, itemHeight)
        {

        }


        #endregion
    }
}
