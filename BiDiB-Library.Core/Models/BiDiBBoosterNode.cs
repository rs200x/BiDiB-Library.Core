using System;
using System.Diagnostics;
using System.Timers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using org.bidib.Net.Core.Enumerations;
using org.bidib.Net.Core.Models.BiDiB.Extensions;
using org.bidib.Net.Core.Models.Messages.Input;
using org.bidib.Net.Core.Models.Messages.Output;

namespace org.bidib.Net.Core.Models;

public sealed class BiDiBBoosterNode : ModelBase, IDisposable
{
    private readonly ILogger<BiDiBBoosterNode> logger;
    private CommandStationState dccState;
    private WatchdogState watchdogState;
    private int temp;
    private double voltage;
    private int current;
    private int maxCurrent;
    private BoosterState boosterState;
    private readonly Timer watchdogTimer;
    private readonly Stopwatch watchdogWatch;
    private BoosterControl boosterControl;

    public BiDiBBoosterNode(BiDiBNode node) : this(node, NullLogger<BiDiBBoosterNode>.Instance)
    {

    }

    public BiDiBBoosterNode(BiDiBNode node, ILogger<BiDiBBoosterNode> logger)
    {
        this.logger = logger;
        Node = node ?? throw new ArgumentNullException(nameof(node));

        DccState = CommandStationState.BIDIB_CS_STATE_OFF;
        WatchdogState = WatchdogState.OFF;
        BoosterState = BoosterState.BIDIB_BST_STATE_OFF;

        watchdogTimer = new Timer();
        watchdogTimer.Elapsed += HandleWatchdogTimerOnElapsed;
        watchdogWatch = new Stopwatch();
    }

    public BiDiBNode Node { get; }

    public CommandStationState DccState
    {
        get => dccState;
        set { Set(() => DccState, ref dccState, value); }
    }

    public WatchdogState WatchdogState
    {
        get => watchdogState;
        set { Set(() => WatchdogState, ref watchdogState, value); }
    }

    public BoosterState BoosterState
    {
        get => boosterState;
        set { Set(() => BoosterState, ref boosterState, value); }
    }

    public BoosterControl BoosterControl
    {
        get => boosterControl;
        set => Set(() => BoosterControl, ref boosterControl, value);
    }

    public int Temp
    {
        get => temp;
        set { Set(() => Temp, ref temp, value); }
    }

    public double Voltage
    {
        get => voltage;
        set { Set(() => Voltage, ref voltage, value); }
    }

    public int Current
    {
        get => current;
        set
        {
            Set(() => Current, ref current, value);
            OnPropertyChanged(nameof(CurrentPercentage));
        }
    }

    public int MaxCurrent
    {
        get => maxCurrent;
        set
        {
            Set(() => MaxCurrent, ref maxCurrent, value);
            OnPropertyChanged(nameof(CurrentPercentage));
        }
    }

    public int CurrentPercentage
    {
        get
        {
            if (MaxCurrent <= 0)
            {
                return 0;
            }

            return Current <= MaxCurrent ? Convert.ToInt32(Current * 100 / MaxCurrent) : 100;
        }
    } 

    public bool HasWatchdog => Node.IsFeatureActive(BiDiBFeature.FEATURE_GEN_WATCHDOG);


    public void TurnOn(bool withDcc, bool withWatchdog, bool broadCast)
    {
        if (withDcc)
        {
            var state = withWatchdog
                ? CommandStationState.BIDIB_CS_STATE_GO
                : CommandStationState.BIDIB_CS_STATE_GO_IGN_WD;
            var stateMessage = Node.MessageProcessor.SendMessage<CommandStationStateMessage>(new CommandStationSetStateMessage(state, Node.Address));
            if (stateMessage?.State == CommandStationState.BIDIB_CS_STATE_GO && HasWatchdog)
            {
                StartWatchdog();
            }
        }

        Node.MessageProcessor.SendMessage<BoostStatMessage>(Node, BiDiBMessage.MSG_BOOST_ON, (byte)(broadCast ? 0x00 : 0x01));
    }

    public void TurnOff(bool withDcc, bool broadCast)
    {
        if (withDcc)
        {
            Node.MessageProcessor.SendMessage<CommandStationStateMessage>(new CommandStationSetStateMessage(CommandStationState.BIDIB_CS_STATE_OFF, Node.Address));
            StopWatchdog();
        }

        Node.MessageProcessor.SendMessage<BoostStatMessage>(Node, BiDiBMessage.MSG_BOOST_OFF, (byte)(broadCast ? 0x00 : 0x01));

    }

    private void StartWatchdog()
    {
        watchdogTimer.Interval = Node.GetFeature(BiDiBFeature.FEATURE_GEN_WATCHDOG).Value * 100 - 50;
        watchdogTimer.Start();
        WatchdogState = WatchdogState.ON;

    }

    private void HandleWatchdogTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
    {
        watchdogWatch.Stop();
        var duration = watchdogTimer.Interval - watchdogWatch.ElapsedMilliseconds;
        logger.LogDebug("Watchdog event {NodeAddress} i:{Interval} w:{ElapsedMilliseconds} d:{Duration} dcc:{DccState}", 
            Node.GetFullAddressString(),
            watchdogTimer.Interval,
            watchdogWatch.ElapsedMilliseconds,
            duration, 
            dccState);

        if (DccState != CommandStationState.BIDIB_CS_STATE_PROG &&
            DccState != CommandStationState.BIDIB_CS_STATE_PROGBUSY)
        {
            Node.MessageProcessor.SendMessage(new CommandStationSetStateMessage(CommandStationState.BIDIB_CS_STATE_GO, Node.Address));
        }
            
        watchdogWatch.Restart();
    }

    private void StopWatchdog()
    {
        watchdogTimer.Stop();
        WatchdogState = WatchdogState.OFF;
    }

    public void SetCommandStationState(CommandStationState newState)
    {
        Node.MessageProcessor.SendMessage(new CommandStationSetStateMessage(newState, Node.Address));
        if (newState == CommandStationState.BIDIB_CS_STATE_GO && HasWatchdog)
        {
            StartWatchdog();
        }

        if (newState == CommandStationState.BIDIB_CS_STATE_STOP && HasWatchdog)
        {
            StopWatchdog();
        }
    }

    public void Dispose()
    {
        watchdogTimer.Stop();
        watchdogTimer.Elapsed -= HandleWatchdogTimerOnElapsed;
        watchdogTimer.Dispose();
    }
}