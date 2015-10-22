using System;
using System.Windows.Input;
using AppStudio.DataProviders;
using AppStudio.Uwp.Actions;
using AppStudio.Uwp.Commands;

namespace Windows10News.Config
{
    public class ActionConfig<T> where T : SchemaBase
    {
        public string Text { get; set; }
        public string Style { get; set; }
        public ICommand Command { get; set; }
        public Func<T, object> CommandParameter { get; set; }

        public static ActionConfig<T> Link(string text, Func<T, string> param)
        {
            return new ActionConfig<T>
            {
                Text = text,
                Style = ActionKnownStyles.Link,
                Command = ActionCommands.NavigateToUrl,
                CommandParameter = param
            };
        }

        public static ActionConfig<T> Phone(string text, Func<T, string> param)
        {
            return new ActionConfig<T>
            {
                Text = text,
                Style = ActionKnownStyles.Phone,
                Command = ActionCommands.CallToPhone,
                CommandParameter = param
            };
        }

        public static ActionConfig<T> Mail(string text, Func<T, string> param)
        {
            return new ActionConfig<T>
            {
                Text = text,
                Style = ActionKnownStyles.Mail,
                Command = ActionCommands.Mailto,
                CommandParameter = param
            };
        }

        public static ActionConfig<T> Address(string text, Func<T, string> param)
        {
            return new ActionConfig<T>
            {
                Text = text,
                Style = ActionKnownStyles.Address,
                Command = ActionCommands.MapsPosition,
                CommandParameter = param
            };
        }

        public static ActionConfig<T> Directions(string text, Func<T, string> param)
        {
            return new ActionConfig<T>
            {
                Text = text,
                Style = ActionKnownStyles.Directions,
                Command = ActionCommands.MapsHowToGet,
                CommandParameter = param
            };
        }

        public static ActionConfig<T> Play(string text, Func<T, string> param)
        {
            return new ActionConfig<T>
            {
                Text = text,
                Style = ActionKnownStyles.Play,
                Command = ActionCommands.NavigateToUrl,
                CommandParameter = param
            };
        }
    }
}
