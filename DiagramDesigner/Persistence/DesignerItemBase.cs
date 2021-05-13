using System;

namespace DiagramDesigner.Persistence
{
    public abstract class DesignerItemBase : PersistenceAbleItemBase
    {
        #region Filed
        /// <summary>
        /// 模块高
        /// </summary>
        public double ItemHeight { get; private set; }
        /// <summary>
        /// 模块宽
        /// </summary>
        public double ItemWidth { get; private set; }

        /// <summary>
        /// 左偏移量
        /// </summary>
        public double Left { get; private set; }

        /// <summary>
        /// 顶偏移量
        /// </summary>
        public double Top { get; private set; }

        #endregion Filed

        public DesignerItemBase(Guid id, double left, double top, double itemWidth, double itemHeight) :
            base(id)
        {
            Init(left,top,itemWidth,itemHeight);
        }

        private void Init(double left, double top, double itemWidth, double itemHeight)
        {
            this.Left = left;
            this.Top = top;
            this.ItemWidth = itemWidth;
            this.ItemHeight = itemHeight;
        }
    }
}