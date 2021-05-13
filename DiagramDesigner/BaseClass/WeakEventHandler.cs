using System;
using System.ComponentModel;
using System.Reflection;

namespace DiagramDesigner.BaseClass
{
    //[DebuggerNonUserCode]
    public sealed class WeakEventHandler 
    {
        private readonly WeakReference _targetReference;
        private readonly MethodInfo _method;

        public WeakEventHandler(PropertyChangedEventHandler callback)
        {
            _method = callback.Method;
            _targetReference = new WeakReference(callback.Target, true);
        }

        //[DebuggerNonUserCode]
        public void Handler(object sender, PropertyChangedEventArgs e)
        {
            var target = _targetReference.Target;
            if (target != null)
            {
                var callback = (Action<object, PropertyChangedEventArgs>)Delegate.CreateDelegate(typeof(Action<object, PropertyChangedEventArgs>), target, _method, true);
                callback?.Invoke(sender, e);
            }
        }
    }
}
