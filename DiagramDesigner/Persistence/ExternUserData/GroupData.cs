using DiagramDesigner.Interface;

namespace DiagramDesigner.Persistence.ExternUserData
{
    internal class GroupData : IUserData
    {
        public double ExpendWidth { get; set; }
        public double ExpendHeight { get; set; }
        public bool IsExpended { get; set; }
    }
}