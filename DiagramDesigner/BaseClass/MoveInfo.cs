using DiagramDesigner.DesignerItemViewModel;

namespace DiagramDesigner.BaseClass
{
    public class MoveInfo
    {
        /// <summary>
        /// 移动对象
        /// </summary>
        public DesignerItemViewModelBase DesignerItem { get; set; }

        /// <summary>
        /// 移动偏移量
        /// </summary>
        public double Offset { get; set; }

        /// <summary>
        /// 移动方向
        /// </summary>
        public Orientation Orientation { get; set; }

        public MoveInfo(DesignerItemViewModelBase designerItem,double offset,Orientation orientation)
        {
            DesignerItem = designerItem;

            Offset = offset;

            Orientation = orientation;
        }
    }
}