#nullable enable
using System.Collections.Generic;
using JetBrains.Annotations;
using Polymorphism4Unity.Editor.Utils;
using Polymorphism4Unity.Safety;
using UnityEngine.UIElements;

namespace Polymorphism4Unity.Editor.Containers.Stacks
{
    public interface IStackCommand
    {
        string StackId { get; }
    }

    public abstract class StackCommand<T> : CommandEventBase<T>, IStackCommand
        where T : StackCommand<T>, new()
    {
        public abstract string StackId { get; }
    }

    [PublicAPI]
    public class PopFrame : StackCommand<PopFrame>
    {
        private static readonly string _cmdName = nameof(PopFrame);
        private string _stackId = string.Empty;
        public override string StackId =>
            Asserts.IsNotNullOrEmpty(_stackId);
        
        public static new PopFrame GetPooled(string stackId)
        {
            PopFrame command = GetPooled();
            command.commandName = _cmdName;
            command._stackId = Asserts.IsNotNullOrEmpty(stackId);
            return command;
        }
    }

    [PublicAPI]
    public class PushFrame : StackCommand<PushFrame>
    {
        private static readonly string _cmdName = nameof(PushFrame);
        private string? _stackId;
        public override string StackId =>
            Asserts.IsNotNullOrEmpty(_stackId);
        public StackFrame Frame { get; private set; } = new();

        public static PushFrame GetPooled(string stackId, StackFrameHeader? header, params VisualElement[] frameContents) =>
            GetPooled(stackId, header, (IEnumerable<VisualElement>) frameContents);
        
        public static PushFrame GetPooled(string stackId, StackFrameHeader? header, IEnumerable<VisualElement> frameContents)
        {
            PushFrame command = GetPooled();
            command.commandName = _cmdName;
            command._stackId = Asserts.IsNotNullOrEmpty(stackId);
            command.Frame = new StackFrame();
            if (header is not null)
            {
                command.Frame.Header = header;
            }
            command.Frame.AddRange(frameContents);
            return command;
        }
        
        public static PushFrame GetPooled(string stackId, params VisualElement[] frameContents) =>
            GetPooled(stackId, null, (IEnumerable<VisualElement>) frameContents);
        
        public static PushFrame GetPooled(string stackId, IEnumerable<VisualElement>  frameContents) =>
            GetPooled(stackId, null, frameContents);
    }
}