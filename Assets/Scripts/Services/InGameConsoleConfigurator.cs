using IngameDebugConsole;
using Zenject;

namespace Services
{
    public class InGameConsoleConfigurator : IInitializable
    {
        private readonly DebugLogManager _debugLogManager;

        public InGameConsoleConfigurator(DebugLogManager debugLogManager)
        {
            _debugLogManager = debugLogManager;
        }

        public void Initialize()
        {
#if DEBUG_CONSOLE
            _debugLogManager.gameObject.SetActive(true);
#else
            _debugLogManager.gameObject.SetActive(false);
#endif
        }
    }
}