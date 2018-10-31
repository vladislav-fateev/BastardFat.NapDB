namespace BastardFat.NapDB.Abstractions
{
    public abstract class NapDbPropertyProxy
    {
        private bool _enabled = true;
        public void Disable() => _enabled = false;
        public void Enable() => _enabled = true;

        public object InvokeGetter(object target, string propertyName, object value)
        {
            if (_enabled)
                return Getter(target, propertyName, value);
            return value;
        }
        public abstract object Getter(object target, string propertyName, object value);
    }

}
