using System;

namespace DiagramDesigner.Persistence
{
    public abstract class DesignerItemBase : PersistenceAbleItemBase
    {
        #region Filed

        /// <summary>
        /// 模块高
        /// </summary>
        public double ItemHeight { get; set; }

        /// <summary>
        /// 模块宽
        /// </summary>
        public double ItemWidth { get; set; }

        /// <summary>
        /// 左偏移量
        /// </summary>
        public double Left { get; set; }

        /// <summary>
        /// 顶偏移量
        /// </summary>
        public double Top { get; set; }

        #endregion Filed

        #region Construstor

        public DesignerItemBase()
        {
        }

        public DesignerItemBase(Guid id)
        : base(id)
        {
        }

        public DesignerItemBase(Guid id, double left, double top, double itemWidth, double itemHeight)
            : this(id)
        {
            Init(left, top, itemWidth, itemHeight);
        }

        #endregion Construstor

        protected virtual void Init(double left, double top, double itemWidth, double itemHeight)
        {
            this.Left = left;
            this.Top = top;
            this.ItemWidth = itemWidth;
            this.ItemHeight = itemHeight;
        }
    }
}