using LlsfMsgs;

namespace Simulator.Utility;
public static class CurrentGame {
    public static volatile uint height = 8;
    public static volatile uint width = 7;
    public static volatile GameState.Types.State GameState = LlsfMsgs.GameState.Types.State.Init;
    public static volatile GameState.Types.Phase GamePhase = LlsfMsgs.GameState.Types.Phase.PreGame;
}
