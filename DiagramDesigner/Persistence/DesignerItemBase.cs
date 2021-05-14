using DiagramDesigner.BaseClass;
using DiagramDesigner.DesignerItemViewModel;
using DiagramDesigner.Interface;
using System;
using System.Linq;

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

        /// <summary>
        /// 用户存储的信息
        /// </summary>
        public ExternUserDataBase Data { get; set; }

        #endregion Filed

        #region Construstor

        //public DesignerItemBase()
        //{
        //}

        //public DesignerItemBase(Guid id)
        //: base(id)
        //{
        //}

        //public DesignerItemBase(Guid id, ref DesignerItemPosition position)
        //    : this(id)
        //{
        //    InitPosition(position);
        //}

        #endregion Construstor

        protected void InitPosition(DesignerItemPosition position)
        {
            this.Left = position.Left;
            this.Top = position.Top;
            this.ItemWidth = position.Width;
            this.ItemHeight = position.Height;
        }

        /// <summary>
        /// 获取保存控件信息的类型
        /// </summary>
        /// <returns></returns>
        public abstract Type GetDesignerItemType();

        /// <summary>
        /// 获取模块的基本数据以及用户数据
        /// </summary>
        /// <returns></returns>

        public abstract DesignerItemData GetDesignerItemData();

        public sealed override SelectableDesignerItemViewModelBase LoadSaveInfo(IDiagramViewModel parent)
        {
            var type = GetDesignerItemType();
            if (type == null)
            {
                throw new ArgumentNullException("DesignerItemViewModel Type is null");
            }

            var designerData = GetDesignerItemData();

            if (designerData == null)
            {
                throw new ArgumentNullException("DesignerData is null");
            }

            var userData = designerData.UserData;

            //if (userData == null)
            //{
            //    throw new ArgumentNullException("save user data object is null");
            //}

            designerData.Parent = parent;

            var info = Activator.CreateInstance(type);

            if (info is DesignerItemViewModelBase designerItem)
            {
                designerItem.LoadDesignerItemData(designerData);
            }

            if (info is GroupDesignerItemViewModelBase group && info is IDiagram diagram && diagram.DesignerAndConnectItems?.Any() == true)
            {
                OnLoadGroup(diagram, group);
            }

            return info as SelectableDesignerItemViewModelBase;
        }

        /// <summary>
        /// 加载分组数据
        /// </summary>
        /// <param name="diagram"></param>
        /// <param name="diagramVm"></param>
        private void OnLoadGroup(IDiagram diagram, IDiagramViewModel diagramVm)
        {
            foreach (var item in diagram.DesignerAndConnectItems)
            {
                var info = item.LoadSaveInfo(diagramVm);

                if (info != null)
                {
                    diagramVm.ItemsSource.Add(info);
                }
            }
        }
    }
}