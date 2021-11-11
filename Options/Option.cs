using System;

namespace RayKeys.Options {
    public class Option {
        public OptionType OptionType { get; }
        public object currentValue;
        public Action<object> changedEvent;
        
        public Option(string id, OptionType optionType, object defaultValue, Action<object> changedFunc) {
            OptionType = optionType;
            currentValue = defaultValue;
            changedEvent = changedFunc;
        }
    }
}