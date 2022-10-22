using System;
using org.bidib.netbidibc.core.Enumerations;
using org.bidib.netbidibc.core.Utils;

namespace org.bidib.netbidibc.core.Models.Messages.Input
{
    public class AccessoryStateMessage : BiDiBInputMessage
    {
        public AccessoryStateMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_ACCESSORY_STATE, 5)
        {
            Number = MessageParameters[0];
            Aspect = MessageParameters[1];
            Total = MessageParameters[2];
            Execute = MessageParameters[3];
            ExecutionState = GetExecutionState();
            Wait = MessageParameters[4];
            WaitTime = (Wait & 0x01) == 0x00 ? Wait & 127 / 10 : Wait & 127;

            if (MessageParameters.Length <= 5) { return; }

            Options = new byte[MessageParameters.Length - 5];
            Array.Copy(MessageParameters, 5, Options, 0, Options.LongLength);
        }

        public byte[] Options { get; }

        public byte Wait { get; }

        public double WaitTime { get; }

        public byte Execute { get; }

        public AccessoryExecutionState ExecutionState { get; }

        public byte Total { get; }

        public byte Aspect { get; }

        public byte Number { get; }

        public AccessoryErrorState ErrorState => (AccessoryErrorState)Wait;

        public bool HasMoreErrors => (Wait & 0x40) == 0x40;


        private AccessoryExecutionState GetExecutionState()
        {
            AccessoryExecutionState executionState;

            if (Aspect < 0xFF && (Execute & (byte)AccessoryState.BIDIB_ACC_STATE_ERROR) == 0x00)
            {
                // normal operation
                switch (Execute & 0x03)
                {
                    case (byte)AccessoryState.BIDIB_ACC_STATE_DONE: // done
                    case (byte)AccessoryState.BIDIB_ACC_STATE_NO_FB_AVAILABLE: // no feedback available
                        {
                            executionState = AccessoryExecutionState.Idle;
                            break;
                        }
                    default:
                        {
                            executionState = AccessoryExecutionState.Running;
                            break;
                        }
                }
            }
            else if (Aspect == 0xFE)
            {
                executionState = AccessoryExecutionState.EmergencyStop;
            }
            else
            {
                // error condition
                executionState = AccessoryExecutionState.Error;
            }
            return executionState;
        }

        private string GetWaitInfo()
        {
            return ExecutionState != AccessoryExecutionState.Error && ExecutionState != AccessoryExecutionState.EmergencyStop ? $"=> {WaitTime}s" : $"=>{ErrorState}";
        }

        public override string ToString()
        {
            return $"{base.ToString()}, Accessory: {Number}, Aspect: {Aspect}, Total: {Total}, " +
                   $"Execute: {Execute}>{ExecutionState}, Wait: {Wait}{GetWaitInfo()}, Options: {Options.GetDataString()}";
        }
    }
}