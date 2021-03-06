
namespace GameCore
{
    public class GameState
    {
        public State CurrentState { get; set; } = State.Start;
        public Scene CurrentScene { get; set; } = Scene.MainMenu;

        public enum State
        {
            Start,
            Play,
            Paused,
            Over
        }

        public enum Scene
        {
            MainMenu = 0,
            Game = 1,
        }
    }
}
